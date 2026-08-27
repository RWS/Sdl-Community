# Cohere Subscription Handoff

## Purpose

The **Language Weaver Provider** Trados Studio plug-in contains a Cohere subscription-prompt feature. It advertises and guides users through the Trados LLM / Language Weaver Pro add-on lifecycle; it does not implement LLM translation itself. Nothing in this feature calls Cohere — a purchased add-on takes effect through the existing Language Weaver translation path.

Specs: [DET-421](https://rws-dev.atlassian.net/browse/DET-421) (epic), [DET-555](https://rws-dev.atlassian.net/browse/DET-555) (state retrieval), and [Account Portal - Language Weaver Pro subscriptions](https://rws-dev.atlassian.net/wiki/spaces/ECO/pages/2415755416/Account+Portal+-+Language+Weaver+Pro+subscriptions).

## Which identity this uses

Everything is resolved from the **Trados Studio sign-in** (`LanguageCloudIdentityApi.Instance`). The configured Language Weaver provider and its credentials are not consulted at all — the prompt works whether or not any provider exists.

This is deliberate and was reversed once. An interim revision read entitlement from the provider's own credentials, which avoided an identity mismatch (Studio holds a Trados account id, 24-char hex; a provider holds a Language Weaver account id, numeric — and they can belong to different accounts). But the Language Weaver side exposes no trial state, and trial state drives two of the four DET-421 cases. Only the Account Portal route carries it, and that route is keyed on the Trados identity. The mismatch that motivated the interim revision no longer arises — see "No configured provider is involved" below.

## How entitlement is resolved

`CohereSubscription\Workflow\Services\CohereSubscriptionWorkflow.cs`

**`businessAccountId` is a signal, not just a lookup key.** The Account Portal record is created when a trial starts, so an account that has never started one has no `businessAccountId` at all. Its absence therefore *means* "never trialed" — which is exactly the state the prompt exists to address. Treating it as a failure, as an earlier revision did, made the feature structurally unable to serve its primary case: it could only speak to users who had already done the thing it was trying to persuade them to do.

The resolution branches on it:

```text
hop 1 -> businessAccountId

  null : never trialed. Trial flags are false by knowledge, not ignorance.
         Read the account's own language pairs to see whether Pro is already
         held (an account can be provisioned with Pro without ever trialing):
             Pro pairs present -> paid, stay silent
             no Pro pairs      -> case A, offer the trial
             no account id     -> undeterminable, stay silent

  set  : hop 2 -> trialStatus + isProActive -> cases B, C, D
```

Neither source covers all four cases alone, and the split falls exactly on the trial boundary:

| Case | Never trialed (language pairs) | Has trial record (Account Portal) |
| --- | --- | --- |
| A. Never trialed, offer trial | yes | unreachable — no `businessAccountId` |
| B. Trial active | not distinguishable | yes |
| C. Trial expired | not distinguishable | yes |
| D. Paid | yes | yes |

The two hops, used when a trial record exists:

```text
1. GET https://eu.cloud.trados.com/lc-api/gw-account-web/accounts/{ActiveTenantId}
      -> businessAccountId

2. GET https://account-portal-api.sdl.com/account-portal/v1/weaver/details/{businessAccountId}
      -> LanguageWeaverDetails { accountId, trialStatus, groupId, isProActive }
```

Plus a third call for the role:

```text
3. GET https://api.languageweaver.com/v4/accounts/users/self   -> userRole
```

Hop 3 works from the Trados identity because the Studio sign-in token is issued by the same Auth0 application and audience (`https://api.sdl.com`) that the Language Weaver Cloud API accepts — verified against a live token. Neither the account-web nor the account-portal response carries a role, so this is the only known source.

`recurlyId` is **not** involved. The details endpoint originally required a `recurlyAccountId`, costing a third Account Portal request; that hop was removed service-side and the endpoint now takes `businessAccountId` directly.

**Hop 1's body is wrapped.** The account object is nested under an `account` property — `{"account":{"id":"…","businessAccountId":"…"}}` — so it deserializes into `LanguageCloudAccountResponse`, not straight into `LanguageCloudAccount`. Getting this wrong is silent and total: Newtonsoft finds no top-level `businessAccountId`, leaves it null, raises nothing, and every account reads as "not provisioned through Account Portal". That bug disabled the entire trial branch until 24 Aug 2026 — cases B, C and D had never once executed. `LanguageCloudAccountResponseTests` pins the shape, including a test that reproduces the flat-deserialization failure directly.

| Fact | Source | Signal |
| --- | --- | --- |
| Paid | `/weaver/details/` | `isProActive` |
| Trial state | `/weaver/details/` | `trialStatus`: `NOT_STARTED`, `IN_PROGRESS`, or a terminal value (`CANCELLED` / `EXPIRED` / `ENDED`) |
| Is an admin | `v4/accounts/users/self` | `userRole == "ADMIN"` |

Two traps when editing `MapDetails`:

- **`isProActive` decides "paid" on its own.** Converting a trial to paid *cancels* the trial, so `isProActive=true` alongside a terminal `trialStatus` is the ordinary state of a paying customer. Pairing the two conditions shows a paying customer the "trial ended" prompt.
- **Do not match on the substring `trial`.** None of the real status values contain it.

### `CANCELLED` shows no prompt

`CANCELLED` means somebody deliberately cancelled — a trial or a paid subscription, and the endpoint carries nothing to tell those apart (a cancelled account returns exactly `{"trialStatus":"CANCELLED","isProActive":null}`). Either way the decision was intentional, so `MapDetails` returns `null` and no prompt is shown. Only `EXPIRED` / `ENDED` — a trial that ran its course — produce the "trial ended" prompt.

The suppression is an explicit early return, not merely the absence of `CANCELLED` from the terminal set. Dropping it from that set would leave `IsCohereDetected` false, which reads as "never had Cohere" and offers a 14-day free trial to someone who just cancelled one. `ACancelledAccount_IsNeverOfferedAFreeTrial` pins this.

Status values observed live so far are `NOT_STARTED`, `IN_PROGRESS` and `CANCELLED`; `EXPIRED` and `ENDED` come from the documented vocabulary and have not yet been seen in a real response. Whether a naturally-lapsed trial reports `EXPIRED` or `CANCELLED` is unconfirmed — if it turns out to be the latter, the trial-ended prompt never fires and the service team should be asked for the authoritative list.

Any failure returns `null`, which the decision service renders as no prompt — the specified behaviour for an undeterminable entitlement (DET-421, case E): show nothing, log the reason, re-evaluate next startup. A null role is treated as non-admin.

## No configured provider is involved

Everything runs off the Trados sign-in. A configured Language Weaver provider is never consulted, and the prompt works whether or not one exists.

The language-pair check on the never-trialed branch does not need the provider's credentials — it needs an LW-authenticated token and an account id, and both come from the Trados sign-in. The token is accepted by the Language Weaver Cloud API because Studio's sign-in and the plug-in's own RWS ID SSO are issued by the same Auth0 application, audience (`https://api.sdl.com`) and scopes; verified against a live token. The account id comes from `users/self` in the same request as the role.

That also means both branches describe the same account, so the identity mismatch an earlier revision had — entitlement read from one account while translation happens on another — does not arise.

## Known gap: trial days

`/weaver/details/` returns `trialStatus` but no start or end date, so DET-421's "14–8 days remaining: no pop-up / 7–1 days: show *ends in {X} day(s)*" rule cannot be implemented. An active trial prompts at any point in its 14 days, with copy that omits `{X}`. Either date would unblock it — DET-555 notes remaining days is calculable client-side.

## Startup and prompt lifecycle

`ApplicationInitializer.cs` subscribes to `StudioWindowCreatedNotificationEvent`, creating `CohereStartupManager`, which listens for activation of the major Studio views.

`CohereSubscription\CohereSubscriptionOrchestrator.cs`:

- `_isRunning` prevents concurrent lookups and duplicate dialogs.
- `_hasShownThisSession` limits the prompt to one per Studio session.
- Persists the **Do not show this again** selection.

The manager only unsubscribes once a prompt has actually been shown, so a check that resolves to "no prompt" is retried on the next view activation — which covers signing in to Language Cloud after Studio has launched. It also means an account that legitimately warrants no prompt (a paying customer, say) re-queries on every view switch for the whole session; see follow-ups.

Suppression setting:

```text
%APPDATA%\Trados AppStore\Language Weaver\Settings\CohereSubscriptionSettings.json
```

Delete that file to reset **Do not show this again** during manual testing.

## Manual testing

**Which branch you exercise depends on whether the account has ever started a trial.** An account with no Account Portal record takes the never-trialed branch (case A, the discovery prompt); one that has started a trial at some point takes the Account Portal branch (cases B, C, D). Signing in to Trados Studio is the only precondition either way — no Language Weaver provider is needed.

Do not infer an account's state from older log lines. Until the hop-1 envelope fix, *every* account logged "no businessAccountId", so any conclusion drawn from entries before 24 Aug 2026 about which accounts are provisioned is unreliable.

If no prompt appears, the log says which branch ran and why. Both request URLs and the resolved entitlement are recorded:

```text
%APPDATA%\Trados AppStore\Language Weaver\Logs\LanguageWeaverProvider.Logs.txt
```

1. Build and deploy to Trados Studio 19. **Close Studio first** — it locks the plug-in assemblies and the packaging step fails with `PFE402: Access to the path ... is denied`.
2. Sign in to Trados Language Cloud so `LanguageCloudIdentityApi` has a token and an active tenant. No Language Weaver provider needs to be configured.
3. Activate a Studio view such as Welcome, Projects, Files, or Editor.
4. Select **Do not show this again**, restart Studio, confirm the dialog does not recur.
5. Switch rapidly between views; only one prompt should appear.

To reach a state the account cannot produce, break on `return response.Response;` in `GetLanguageWeaverDetails` and set values in the Immediate window before continuing:

```csharp
response.Response.IsProActive = false; response.Response.TrialStatus = "NOT_STARTED";  // not detected
response.Response.IsProActive = false; response.Response.TrialStatus = "IN_PROGRESS";  // trial active
response.Response.IsProActive = false; response.Response.TrialStatus = "EXPIRED";      // trial ended
response.Response.IsProActive = false; response.Response.TrialStatus = "CANCELLED";    // no prompt
```

For the admin and non-admin variants, override the role returned by `GetSelf` the same way.

## Build and test

Use the Visual Studio MSBuild and `vstest.console.exe`, not the `dotnet` CLI — `Sdl.Core.PluginFramework.Build.CreatePluginManifestTask` needs the legacy `Microsoft.Build.Utilities.v4.0` assembly, which the preview .NET SDK cannot load. Add `/p:CreatePluginPackage=false` to compile while Studio is running.

```text
"C:\Program Files\Microsoft Visual Studio\18\Professional\MSBuild\Current\Bin\MSBuild.exe" LanguageWeaverProvider.sln /t:Restore;Build /p:Configuration=Debug
"C:\Program Files\Microsoft Visual Studio\18\Professional\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe" LanguageWeaverProviderTests\bin\Debug\net48\LanguageWeaverProviderTests.dll /Platform:x64
```

## Follow-ups

1. **Trial days** — as above. `trialStatus` distinguishes active from expired, but with no dates the "14–8 days silent / 7–1 days show `{X}`" rule cannot be implemented. Ask the service team for a trial start or end date.
2. **CTA deep links.** Buttons now navigate to public product pages, held in `Constants`:

   | Button | Destination |
   | --- | --- |
   | Start free trial, Buy now | `LanguageWeaverProPricingUrl` — the Freelance pricing page, where the add-on and its price are presented |
   | Learn more | `LanguageWeaverProLearnMoreUrl` — the Language Weaver for Trados product page |
   | OK, Cancel | no URL; dismiss the prompt |

   These are marketing pages, not what DET-421 ultimately asks for. The trial is actually activated in the RWS Account Portal (`PUT /account-portal/v1/weaver/trial/{recurlyAccountId}`), and no public URL starts it, so "Start free trial" currently lands the user on pricing rather than starting anything. DET-555 scopes deep-link generation and portal navigation out and that work has no ticket yet. When it happens, both identifiers such a link needs (`businessAccountId`, `businessSubscriptionId`) arrive together in one `gw-account-web/accounts/{tradosAccountId}` response, so no extra "account context" endpoint is required.

3. **Public naming.** The dialog copy names the product **Trados LLM**, "powered by Cohere" — the wording approved in Confluence, and what currently ships. The customer-facing site uses neither phrase: the product is branded **Language Weaver Pro**, and the pricing page never mentions Cohere at all (the launch announcements do credit it, as "in partnership with Cohere"). So a user reading "Trados LLM" in the prompt and clicking Buy now lands on a page selling something under a different name. Six strings in `CohereSubscriptionDecisionService`; worth settling with whoever owns the Confluence copy before the prompts ship.
4. **Eligibility gate.** DET-421 scopes the journey to Trados Go / Freelance accounts. No such check exists; any account that resolves an entitlement is evaluated.
5. **Region.** Both the account-web host and the `users/self` host are hardcoded to EU. A US-region account will fail hop 1, and resolve no role, so it simply gets no prompt — the safe direction, but untested. The request URLs are logged so a wrong host is visible in the field.
6. **Repeated lookups.** `RunAsync` only unsubscribes after a dialog is actually shown, so any account that resolves to "no prompt" — every paying customer included — re-runs the whole lookup on every view activation for the rest of the session. Unsubscribing whenever the entitlement resolves successfully would fix it.
7. **Silent role failures.** `GetSelf` returns `(null, null)` on a non-success response without logging, so a failed role lookup is indistinguishable from a genuinely non-admin user. One log line would separate them.
8. **Open product questions** from DET-421, never answered: whether to offer both "Start Free Trial" and "Buy Now" or trial only, and whether "Don't show again" is permanent or resets on entitlement state change (currently permanent).
