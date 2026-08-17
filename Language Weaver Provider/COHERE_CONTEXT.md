# Cohere Subscription Endpoint Handoff

## Purpose

The **Language Weaver Provider** Trados Studio plugin contains a Cohere subscription-prompt feature. It advertises and guides users through the Trados LLM / Cohere add-on lifecycle; it does not implement LLM translation itself.

Specs: [DET-421](https://rws-dev.atlassian.net/browse/DET-421) (epic), [DET-555](https://rws-dev.atlassian.net/browse/DET-555) (state retrieval), and the endpoint owner's page [Account Portal - Language Weaver Pro subscriptions](https://rws-dev.atlassian.net/wiki/spaces/ECO/pages/2415755416/Account+Portal+-+Language+Weaver+Pro+subscriptions).

The feature had been deliberately disabled because its backend endpoint did not exist. It is now wired to the deployed endpoints.

## Account resolution

The entitlement endpoint is keyed on the **Account Portal** identity, not the Trados one, so the lookup is two hops:

```text
1. GET https://eu.cloud.trados.com/lc-api/gw-account-web/accounts/{tradosAccountId}
      -> businessAccountId

2. GET https://account-portal-api.sdl.com/account-portal/v1/weaver/details/{businessAccountId}
      -> LanguageWeaverDetails
```

`tradosAccountId` is `LanguageCloudIdentityApi.Instance.ActiveTenantId`. Both requests send `Authorization: Bearer {accessToken}` from the same identity API.

**`recurlyId` is no longer involved.** The details endpoint originally required a `recurlyAccountId`, which cost a third request (`GET /account-portal/v1/accounts/{businessAccountId}` -> `recurlyId`). That hop was removed on the service side; the endpoint accepts `businessAccountId` directly and returns the same body.

`businessAccountId` is null when the Trados account was not provisioned through Account Portal. There is then no entitlement record, so the check stops and no prompt is shown.

The hop-1 payload is the full account object. Two fields of note beyond `businessAccountId`:

- **`businessSubscriptionId`** sits alongside it, so both identifiers the DET-421 deep-link note asks for come from this single request. No separate "account context" call is needed.
- There is **no role or permission field describing the caller**. The `accessControllList` entries name the account and its owner, not the signed-in user, so they cannot answer "is this user an Admin".

Response contract:

```csharp
public class LanguageWeaverDetails
{
    public int? AccountId { get; set; }
    public string TrialStatus { get; set; }
    public string GroupId { get; set; }
    public bool IsProActive { get; set; }
}
```

## Current Implementation

### Endpoint invocation

`CohereSubscription\Workflow\Services\CohereSubscriptionWorkflow.cs`

Performs both hops through the existing `GenericHTTPService`, over one shared static `HttpClient`. Every failure path returns `null`, which the decision service renders as "no prompt" — the specified behaviour for an undeterminable entitlement (DET-421, case E). Each failure is logged with the request URL and error detail.

### Entitlement mapping

`CohereSubscriptionWorkflow.MapDetails` maps the response onto `CohereSubscriptionData`. The documented `trialStatus` values are `NOT_STARTED`, `IN_PROGRESS`, and the terminal states a started trial ends in (`CANCELLED`, and by extension `EXPIRED` / `ENDED`):

| `isProActive` | `trialStatus` | Prompt state |
| --- | --- | --- |
| `true` | any | Paid / no prompt |
| `false` | `IN_PROGRESS` | Active trial |
| `false` | `CANCELLED` / `EXPIRED` / `ENDED` | Expired trial |
| `false` | `NOT_STARTED`, null, unknown | Cohere not detected |

Two things worth keeping in mind when editing this mapping:

- **`isProActive` decides "paid" on its own.** Converting a trial to paid *cancels* the trial, so `isProActive=true` alongside a terminal `trialStatus` is the ordinary state of a paying customer. Pairing the two conditions shows a paying customer the "trial expired" prompt.
- **Do not match on the substring `trial`.** None of the real status values contain it.

The endpoint returns **no trial start or end date**, so active-trial copy cannot state days remaining (see follow-ups).

The endpoint returns **no role information**. `IsAdmin` is therefore fixed at `false`, so only non-admin copy is shown and no account-management action is offered to a user who may not be authorized to perform it.

### Startup and prompt lifecycle

`ApplicationInitializer.cs` subscribes to `StudioWindowCreatedNotificationEvent`, which creates `CohereStartupManager`, which subscribes to activation of the major Studio views.

`CohereSubscription\CohereSubscriptionOrchestrator.cs`:

- `_isRunning` prevents concurrent lookups and duplicate dialogs.
- `_hasShownThisSession` limits the prompt to one per Studio session.
- Persists the **Do not show this again** selection.

Because the manager only unsubscribes once a prompt has actually been shown, an entitlement check that finds no Language Cloud session is retried on the next view activation — which covers signing in to Auth0 after Studio has already launched.

Suppression setting:

```text
%APPDATA%\Trados AppStore\Language Weaver\Settings\CohereSubscriptionSettings.json
```

Delete that file to reset **Do not show this again** during manual testing.

## Files Changed

- `ApplicationInitializer.cs`
- `CohereSubscription\CohereStartupManager.cs`
- `CohereSubscription\CohereSubscriptionOrchestrator.cs`
- `CohereSubscription\Decision\Services\CohereSubscriptionDecisionService.cs`
- `CohereSubscription\Workflow\Services\CohereSubscriptionWorkflow.cs`
- `CohereSubscription\Workflow\Model\LanguageWeaverDetails.cs` (new)
- `CohereSubscription\Workflow\Model\LanguageCloudAccount.cs` (new)
- `SubscriptionJourney\ViewModel\SubscriptionViewModel.cs`
- `LanguageWeaverProviderTests\UnitTests\CohereSubscriptionWorkflowTests.cs` (new)
- `LanguageWeaverProviderTests\LanguageWeaverProviderTests.csproj` (NLog reference)

## Manual Test Procedure

**A suitable test account is a precondition.** The prompt only appears for an account provisioned through Account Portal (non-null `businessAccountId`), and DET-421 scopes the journey to Trados Go / Freelance. An `ENTERPRISE` account with `provisioningSource: "OOS"` — the Trados AppStore Team account, for instance — returns a null `businessAccountId` and will correctly show nothing, no matter which entitlement state you are trying to reproduce. Check hop 1 first if no prompt appears.

1. Build and deploy the plugin to Trados Studio 19.
2. Sign in to Trados Language Cloud so `LanguageCloudIdentityApi` has a token and active tenant.
3. Activate a Studio view such as Welcome, Projects, Files, or Editor.
4. Confirm the prompt matches the entitlement state. The log records both request URLs and the resolved entitlement:

```text
%APPDATA%\Trados AppStore\Language Weaver\Logs\LanguageWeaverProvider.Logs.txt
```

5. Select **Do not show this again**, restart Studio, and confirm the dialog does not recur.
6. Switch rapidly between several Studio views; only one prompt should appear.

### Simulating other entitlement states in the debugger

Break in `CohereSubscriptionWorkflow.GetLanguageWeaverDetails` on `return response.Response;`, then set values in the Immediate window before continuing:

```csharp
// Cohere not detected
response.Response.IsProActive = false; response.Response.TrialStatus = "NOT_STARTED";

// Active trial
response.Response.IsProActive = false; response.Response.TrialStatus = "IN_PROGRESS";

// Expired trial
response.Response.IsProActive = false; response.Response.TrialStatus = "CANCELLED";
```

## Validation Status

- Solution builds clean; full suite passes **43/43**, six of them covering this mapping.
- Build and test with the Visual Studio MSBuild and `vstest.console.exe`, not the `dotnet` CLI — `Sdl.Core.PluginFramework.Build.CreatePluginManifestTask` needs the legacy `Microsoft.Build.Utilities.v4.0` assembly, which the preview .NET SDK cannot load:

```text
"C:\Program Files\Microsoft Visual Studio\18\Professional\MSBuild\Current\Bin\MSBuild.exe" LanguageWeaverProvider.sln /t:Restore;Build /p:Configuration=Debug
"C:\Program Files\Microsoft Visual Studio\18\Professional\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe" LanguageWeaverProviderTests\bin\Debug\net48\LanguageWeaverProviderTests.dll /Platform:x64
```

## Known Assumptions and Follow-ups

Blocking, both outstanding against the service team:

1. **Admin role.** Nothing in the current contracts exposes whether the signed-in user is an Admin on the Language Weaver account, so the entire admin half of DET-421 — "Start free trial" and the admin "Buy now" — cannot render. Confirmed against a live account-web response: the account object carries no caller role or permission field. In Trados, admin means membership in the `ACCOUNT ADMINS` group; the group endpoints are all admin-credentialed (`/lc-api/group/v1/admin/...`) and unusable with the plugin's user token. Account-user objects also carry a `membership` field. **Ask:** is there a user-scoped endpoint returning the caller's own groups or `membership`? `ActiveUserId` is already available.
2. **Trial dates.** The details endpoint returns no trial start or end date, so the DET-421 rule "14–8 days remaining: no pop-up / 7–1 days: show *ends in {X} day(s)*" cannot be implemented. DET-555 notes remaining days is calculable client-side from the dates, so either date unblocks it. Until then an active trial prompts at any point in its 14 days, with copy that omits `{X}`.

Non-blocking:

3. **Region.** The account-web host is currently the documented EU one. Whether US-region accounts need a different host is unconfirmed; the request URL is logged so a wrong choice is visible in the field.
4. **CTA URLs.** The `https://example.com/` placeholders have been removed, so buttons now simply dismiss the prompt. Two of them sat on buttons labelled "OK". Restoring them is not just a matter of pasting URLs: DET-421 requires Account Portal *deep links*, and DET-555 explicitly scopes deep-link generation and portal navigation out. That work has no ticket yet.
5. **Subscription ID — not required.** Earlier notes suggested the navigation endpoint needs a business ID *and* a subscription ID. DET-555 lists the Account Context Endpoint as "(if required)", conditional on the deep-link work above. Trados account ID plus `businessAccountId` cover everything currently in scope — and `businessSubscriptionId` already arrives in the same hop-1 payload, so no such endpoint is needed even when deep links are built.
6. **Eligibility gate.** DET-421 scopes the journey to Trados Go / Freelance accounts. No such check is implemented; every account that resolves an entitlement is evaluated.
7. **Open product questions** from DET-421, never answered: whether to offer both "Start Free Trial" and "Buy Now" or trial only, and whether "Don't show again" is permanent or resets on entitlement state change (currently permanent).
