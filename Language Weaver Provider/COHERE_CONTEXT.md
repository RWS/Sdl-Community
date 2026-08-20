# Cohere Subscription Handoff

## Purpose

The **Language Weaver Provider** Trados Studio plug-in contains a Cohere subscription-prompt feature. It advertises and guides users through the Trados LLM / Language Weaver Pro add-on lifecycle; it does not implement LLM translation itself. Nothing in this feature calls Cohere — a purchased add-on takes effect through the existing Language Weaver translation path.

Specs: [DET-421](https://rws-dev.atlassian.net/browse/DET-421) (epic), [DET-555](https://rws-dev.atlassian.net/browse/DET-555) (state retrieval), and [Account Portal - Language Weaver Pro subscriptions](https://rws-dev.atlassian.net/wiki/spaces/ECO/pages/2415755416/Account+Portal+-+Language+Weaver+Pro+subscriptions).

## Which identity this uses, and why

Entitlement is read with **the credentials of the Language Weaver Cloud provider the user has configured** — never the Trados Studio sign-in.

Those are two different identities in two different id spaces: Studio holds a Trados account id (24-char hex), the provider holds a Language Weaver account id (numeric). They can belong to different accounts — a user may be signed into Studio as themselves while the provider is configured against a client's account — so an entitlement read through Studio cannot be shown to say anything about the account the user is actually translating with.

An earlier revision resolved entitlement through the Studio sign-in via a two-hop Account Portal lookup (`gw-account-web/accounts/{tradosAccountId}` to `businessAccountId` to `/account-portal/v1/weaver/details/`). That is **removed**. With it went `LanguageCloudAccount.cs`, `LanguageWeaverDetails.cs`, and the `Sdl.LanguageCloud.IdentityApi` assembly reference, which had been added for this feature and had no other consumer in the plug-in.

## How entitlement is resolved

`CohereSubscription\Workflow\Services\CohereSubscriptionWorkflow.cs`

Both facts come from data the plug-in already fetches, using the provider's own `AccessToken`:

| Fact | Source | Signal |
| --- | --- | --- |
| Has the add-on | `v4/accounts/{accountId}/subscriptions/language-pairs` via `CloudService.GetResources<PairModel>` | any pair with `Type == "GENERICPLUS"` |
| Is an admin | `v4/accounts/users/self` via `CloudService.GetUserRole` | `userRole == "ADMIN"` |

`GENERICPLUS` (`model: "pro"`, `displayName: "Pro"`) is the Language Weaver Pro tier, which is what the add-on grants — the Account Portal trial feature is literally `GENERIC_PLUS_LANGUAGE_PAIRS`. `PairModel` already carried `Type`, so no new model or endpoint was needed.

`v4/accounts/users/self` was already called by `CloudService.SetAccountId`, which read only `accountId` and discarded the rest of the payload; `userRole` was in it all along.

Any failure returns `null`, which the decision service renders as no prompt — the specified behaviour for an undeterminable entitlement (DET-421, case E): show nothing, log the reason, re-evaluate next startup.

## Scope

Only **Language Weaver Cloud** providers are evaluated. Edge providers never prompt: the add-on is a cloud commerce concept sold through Account Portal and cannot apply to an on-prem Edge server. With no authenticated cloud provider configured, nothing is shown.

With several cloud providers configured, the first authenticated one is used — the prompt is a single account-level message, so any authenticated cloud account answers "does this organisation have the add-on".

## Known gap: trial state

Nothing currently exposes whether an account is on a trial, when it started, or when it ends. `MapEntitlement` therefore leaves both trial flags false, which collapses two of the four DET-421 cases:

| DET-421 case | Behaviour | Status |
| --- | --- | --- |
| D. Paid, no prompt | has Pro pairs | correct |
| A. Not detected, prompt | no Pro pairs | correct |
| B. Trial active, "ends in {X} days" | reads as paid; stays silent | **collapsed** |
| C. Trial expired, "buy now" | reads as never-had-it; offers a trial | **collapsed** |

Neither collapse shows the user anything untrue — both miss an upsell. An account mid-trial holds Pro pairs and so is silently treated as paid; an expired trial is offered a free trial again.

The trial-state branches and their approved copy remain in `CohereSubscriptionDecisionService` so they light up as soon as a signal exists. Candidate source: subscriptions carry `startDate`/`endDate`, so a trial may surface as a short-dated subscription — unverified, and Account Portal documentation suggests trial pairs attach to the *group* while paid pairs attach to the *subscription*, which would look different.

## Startup and prompt lifecycle

`ApplicationInitializer.cs` subscribes to `StudioWindowCreatedNotificationEvent`, creating `CohereStartupManager`, which listens for activation of the major Studio views.

`CohereSubscription\CohereSubscriptionOrchestrator.cs`:

- `_isRunning` prevents concurrent lookups and duplicate dialogs.
- `_hasShownThisSession` limits the prompt to one per Studio session.
- Persists the **Do not show this again** selection.

The manager only unsubscribes once a prompt has actually been shown, so a check that finds no configured provider is retried on the next view activation — which covers configuring a provider after Studio has launched.

Suppression setting:

```text
%APPDATA%\Trados AppStore\Language Weaver\Settings\CohereSubscriptionSettings.json
```

Delete that file to reset **Do not show this again** during manual testing.

## Manual testing

The prompt appears only for an account **without** Pro language pairs, on a configured and authenticated Language Weaver Cloud provider. An account that already holds `GENERICPLUS` pairs is treated as paid and stays silent — check the log first if nothing appears:

```text
%APPDATA%\Trados AppStore\Language Weaver\Logs\LanguageWeaverProvider.Logs.txt
```

1. Build and deploy to Trados Studio 19. **Close Studio first** — it locks the plug-in assemblies and the packaging step fails with `PFE402: Access to the path ... is denied`.
2. Configure a Language Weaver Cloud provider and authenticate it.
3. Activate a Studio view such as Welcome, Projects, Files, or Editor.
4. Select **Do not show this again**, restart Studio, confirm the dialog does not recur.
5. Switch rapidly between views; only one prompt should appear.

To exercise the admin and non-admin variants without two accounts, break on the `return` in `CohereSubscriptionWorkflow.ExecuteAsync` and edit the mapped result in the Immediate window.

## Build and test

Use the Visual Studio MSBuild and `vstest.console.exe`, not the `dotnet` CLI — `Sdl.Core.PluginFramework.Build.CreatePluginManifestTask` needs the legacy `Microsoft.Build.Utilities.v4.0` assembly, which the preview .NET SDK cannot load. Add `/p:CreatePluginPackage=false` to compile while Studio is running.

```text
"C:\Program Files\Microsoft Visual Studio\18\Professional\MSBuild\Current\Bin\MSBuild.exe" LanguageWeaverProvider.sln /t:Restore;Build /p:Configuration=Debug
"C:\Program Files\Microsoft Visual Studio\18\Professional\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe" LanguageWeaverProviderTests\bin\Debug\net48\LanguageWeaverProviderTests.dll /Platform:x64
```

## Follow-ups

1. **Trial state** — as above. The only remaining functional gap.
2. **CTA deep links.** Buttons now navigate to public product pages, held in `Constants`:

   | Button | Destination |
   | --- | --- |
   | Start free trial, Buy now | `LanguageWeaverProPricingUrl` — the Freelance pricing page, where the add-on and its price are presented |
   | Learn more | `LanguageWeaverProLearnMoreUrl` — the Language Weaver for Trados product page |
   | OK, Cancel | no URL; dismiss the prompt |

   These are marketing pages, not what DET-421 ultimately asks for. The trial is actually activated in the RWS Account Portal (`PUT /account-portal/v1/weaver/trial/{recurlyAccountId}`), and no public URL starts it, so "Start free trial" currently lands the user on pricing rather than starting anything. DET-555 scopes deep-link generation and portal navigation out and that work has no ticket yet. When it happens, both identifiers such a link needs (`businessAccountId`, `businessSubscriptionId`) arrive together in one `gw-account-web/accounts/{tradosAccountId}` response, so no extra "account context" endpoint is required.

7. **Public naming.** The dialog copy approved in Confluence says "Trados LLM, powered by Cohere". The public pricing page never mentions Cohere — the product is branded **Language Weaver Pro**, "RWS's own LLM built specifically for translation". Six user-facing strings in `CohereSubscriptionDecisionService` name a vendor the customer-facing site does not. Worth settling with product before the prompts ship.
3. **Eligibility gate.** DET-421 scopes the journey to Trados Go / Freelance accounts. No such check exists; any configured cloud account is evaluated.
4. **`AuthenticationType.CloudAPI`.** API-credential logins identify an application rather than a person and resolve identity through `v4/accounts/api-credentials/self`, which may carry no `userRole`. A missing role is treated as non-admin, which is the safe default, but the endpoint has not been checked.
5. **`GenericHTTPService`** (`Infrastructure\Http\`) now has no consumers — it existed only for the removed two-hop lookup. Left in place deliberately; delete if nothing else claims it.
6. **Open product questions** from DET-421, never answered: whether to offer both "Start Free Trial" and "Buy Now" or trial only, and whether "Don't show again" is permanent or resets on entitlement state change (currently permanent).
