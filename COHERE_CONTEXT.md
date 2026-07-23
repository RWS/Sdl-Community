# Cohere Subscription Endpoint Handoff

## Purpose

The **Language Weaver Provider** Trados Studio plugin contains a Cohere subscription-prompt feature. It is intended to advertise and guide users through the Trados LLM / Cohere add-on lifecycle, without implementing LLM translation itself.

The feature had been deliberately disabled because its backend endpoint did not exist. It has now been wired to the deployed endpoint:

```text
GET /account-portal/v1/weaver/details/{recurlyAccountId}
```

The currently available response contract is:

```csharp
public class LanguageWeaverDetails
{
	private Integer accountId;
	private String trialStatus;
	private String groupId;
	private Boolean isProActive;
}
```

## Current Implementation

### Endpoint invocation

`Language Weaver Provider\CohereSubscription\Workflow\Services\CohereSubscriptionWorkflow.cs`

- Uses `LanguageCloudIdentityApi.Instance`.
- Requires a Language Cloud access token and `ActiveTenantId`.
- Uses `ActiveTenantId` as the best available proxy for `recurlyAccountId`.
- Sends `Authorization: Bearer {accessToken}`.
- Builds the current request URL from the EU Language Weaver API host:

```text
https://api.languageweaver.com/account-portal/v1/weaver/details/{ActiveTenantId}
```

- Deserializes the response into `LanguageWeaverDetails`.

### Entitlement mapping

`CohereSubscriptionWorkflow.MapDetails` maps the endpoint response into existing `CohereSubscriptionData`:

| Endpoint state | Prompt state |
| --- | --- |
| `isProActive == true` and not trial | Paid / no prompt |
| `trialStatus` contains `trial` | Active trial |
| `trialStatus` also contains `expired` or `ended` | Expired trial |
| no Pro and no trial | Cohere not detected |

The endpoint does **not** return a trial end date or remaining days, so active-trial copy says the trial is active instead of claiming it is ending soon.

The endpoint does **not** return the current user's role. `IsAdmin` is therefore deliberately set to `false`, which ensures the feature displays only the safe non-admin copy and never presents account-management actions to a potentially unauthorized user.

### Startup and prompt lifecycle

`Language Weaver Provider\ApplicationInitializer.cs`

- Re-enabled the `StudioWindowCreatedNotificationEvent` subscription.
- This creates `CohereStartupManager`, which subscribes to activation events for Studio views.

`Language Weaver Provider\CohereSubscription\CohereSubscriptionOrchestrator.cs`

- Ensures only one subscription lookup/dialog can run at a time with `_isRunning`.
- Marks the dialog as shown for the current Studio session with `_hasShownThisSession`.
- Persists the checkbox selection from the dialog.

The persisted suppression setting is stored at:

```text
%APPDATA%\Trados AppStore\Language Weaver\Settings\CohereSubscriptionSettings.json
```

Delete that file to reset **Do not show this again** during manual testing.

## Files Changed

- `Language Weaver Provider\ApplicationInitializer.cs`
- `Language Weaver Provider\CohereSubscription\CohereSubscriptionOrchestrator.cs`
- `Language Weaver Provider\CohereSubscription\Decision\Services\CohereSubscriptionDecisionService.cs`
- `Language Weaver Provider\CohereSubscription\Workflow\Services\CohereSubscriptionWorkflow.cs`
- `Language Weaver Provider\CohereSubscription\Workflow\Model\LanguageWeaverDetails.cs` (new)
- `Language Weaver Provider\LanguageWeaverProviderTests\UnitTests\CohereSubscriptionWorkflowTests.cs` (new)

## Manual Test Procedure

1. Build and deploy the plugin to Trados Studio 19.
2. Sign in to Trados Language Cloud so `LanguageCloudIdentityApi` has a token and active tenant.
3. Open or activate a Studio view such as Welcome, Projects, Files, or Editor.
4. Confirm the endpoint call reaches the expected account and the resulting prompt matches the entitlement state.
5. Select **Do not show this again**, restart Studio, and confirm the dialog does not recur.
6. Open or switch multiple Studio views rapidly; only one prompt should appear.

### Simulating a non-Pro account in the debugger

Set a breakpoint in:

```text
Language Weaver Provider\CohereSubscription\Workflow\Services\CohereSubscriptionWorkflow.cs
```

at the `return response.Success && response.Response is not null` statement (previously line 36).

When it breaks, use the Visual Studio Immediate window:

```csharp
response.Response.IsProActive = false;
response.Response.TrialStatus = null;
```

Continue with F5. This should produce the non-Pro / no-trial availability prompt.

## Validation Status

- The provider project built successfully after the changes.
- The dedicated test project could not be executed in this environment because the installed preview .NET SDK cannot load the legacy `Microsoft.Build.Utilities.v4.0` dependency required by `Sdl.Core.PluginFramework.Build.CreatePluginManifestTask`.
- `CohereSubscriptionWorkflowTests` covers the pure response mapping for paid, active-trial, and expired-trial states.

## Known Assumptions and Follow-ups

1. **Identifier:** `ActiveTenantId` is assumed to be accepted by the endpoint as `{recurlyAccountId}`. LW should confirm this mapping.
2. **Region:** the request currently uses `Constants.CloudEUUrl`. The endpoint host for US-region accounts has not been confirmed. If the account portal is regional, select `CloudUSUrl` when appropriate.
3. **Trial values:** mapping is intentionally resilient by checking whether `trialStatus` contains `trial`, `expired`, or `ended`. LW should provide the definitive enum/string values.
4. **Role:** until the API exposes admin/role data or a supported client API provides it, all users follow non-admin messaging.
5. **CTA URLs:** existing subscription decision copy still contains `https://example.com/` placeholders. Replace them with the approved trial, purchase, and documentation URLs before shipping.
6. **Observability:** request failures intentionally result in no popup. Consider structured logging for endpoint failures once privacy/logging expectations are agreed.
