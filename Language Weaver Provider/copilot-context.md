# Language Weaver Provider — Copilot Context

This document gives Copilot everything it needs to work confidently on this codebase.

---

## What this project is

**Language Weaver Provider** is a machine-translation plugin for **Trados Studio 19.x** (RWS/SDL). It integrates the Language Weaver MT service into Trados Studio's translation workflow, supporting both:

- **Language Weaver Cloud** – hosted API, EU and US regions
- **Language Weaver Edge** – on-premises server deployment

Plugin version: `3.0.1.1`
Manifest: `pluginpackage.manifest.xml`
Assembly attribute: `[Plugin("LanguageWeaverProvider")]` in `Properties/PluginProperties.cs`
Source: https://github.com/RWS/Sdl-Community/tree/master/LanguageWeaverProvider

---

## Technology stack

| Concern | Choice |
|---|---|
| Target framework | .NET Framework 4.8 (`net48`) |
| C# language version | `latest` |
| UI framework | WPF (`UseWPF = true`) |
| UI pattern | MVVM (`BaseViewModel : INotifyPropertyChanged`) |
| Serialization | Newtonsoft.Json (loaded from Trados Studio folder) |
| Logging | NLog (loaded from Trados Studio folder) |
| Database | SQLite via `LanguageMappingProvider` NuGet package (Dapper-backed) |
| Platform target | x64 |
| Plugin framework | `Sdl.Core.PluginFramework` 2.1.0 (NuGet) |
| Assembly signing | `SdlCommunity.snk` |

All Trados Studio DLLs are referenced directly via `$(TradosFolder)` (`%ProgramW6432%\Trados\Trados Studio\Studio19Beta`). There is **no NuGet restore** for those assemblies.

### NuGet packages (non-Studio)
- `LanguageMappingProvider` 1.0.107 – SQLite language-code mapping database
- `Sdl.Core.PluginFramework` 2.1.0
- `Sdl.Core.PluginFramework.Build` 18.0.1 – MSBuild tasks that create the `.sdlplugin` package
- `System.Configuration.ConfigurationManager` 8.0.0

---

## Build & deploy

```
msbuild LanguageWeaverProvider.csproj /p:Configuration=Debug
```

- The `CreatePluginPackage` MSBuild property packages the output into a `.sdlplugin` file automatically.
- Debug output is deployed to `%APPDATA%\Trados\Trados Studio\19beta\Plugins`.
- Logs are written to `%APPDATA%\Trados AppStore\Language Weaver\Logs\LanguageWeaverProvider.Logs.txt` (daily archive via NLog).
- Run tests via `vstest.console.exe` (not `dotnet test`) — the plugin project uses an MSBuild task that requires full VS MSBuild.

---

## Project layout

```
LanguageWeaverProvider/
├── ApplicationInitializer.cs       # Plugin entry point ([ApplicationInitializer])
├── Constants.cs                    # All string constants (URLs, scheme names, metadata keys)
├── Enums.cs                        # PluginVersion, AuthenticationType, QualityEstimations, …
├── Log.cs                          # NLog setup (call Log.Setup() once on startup)
│
├── Model/
│   ├── Interface/
│   │   ├── ITranslationOptions.cs  # Core options contract
│   │   └── IPathInfo.cs
│   ├── TranslationOptions.cs       # Concrete options (JSON-serialized into provider state)
│   ├── ProviderSettings.cs         # Per-provider settings (pre/post lookup, tags, feedback)
│   ├── PairMapping.cs              # A source→target language pair + selected model + dictionaries
│   ├── PairModel.cs                # An MT model within a language pair
│   ├── PairDictionary.cs           # A dictionary attached to a language pair
│   ├── AccessToken.cs              # Bearer token + expiry + base URI
│   ├── CloudCredentials.cs         # Cloud username/password or API key
│   ├── EdgeCredentials.cs          # Edge URL + username/password or API key
│   ├── StandaloneCredentials.cs    # Wrapper for credentials in standalone (non-Studio) mode
│   ├── RatedSegment.cs             # QE result stored per segment
│   ├── LWSegmentEditor.cs          # Pre/post-lookup replacement rules (plain text or regex)
│   └── …
│
├── Services/
│   ├── Service.cs                  # Shared HTTP helpers (SendRequest, DeserializeResponse, ValidateTokenAsync)
│   ├── CloudService.cs             # All Cloud API calls (auth, translate, QE, feedback, account)
│   ├── EdgeService.cs              # All Edge API calls (auth, translate, QE, dictionaries, feedback)
│   ├── ITranslationEngine.cs       # Abstraction over Cloud / Edge plain-text MT calls
│   ├── CloudTranslationEngine.cs   # ITranslationEngine adapter for CloudService.Translate
│   ├── EdgeTranslationEngine.cs    # ITranslationEngine adapter for EdgeService.Translate
│   ├── IBatchTranslator.cs         # Batch-level abstraction returning EvaluatedSegment[]
│   ├── PlainTextBatchTranslator.cs # Default IBatchTranslator: encodes tags as placeholders, calls engine, rehydrates tags
│   ├── SegmentTagPlacer.cs         # Encodes inline Tag elements as <x id="N"/> placeholders and rebuilds target Segments
│   ├── Model/TranslationResult.cs  # Translated text + optional QE label (returned by ITranslationEngine)
│   ├── Model/EvaluatedSegment.cs   # Per-segment translation result (Segment + QE label) returned by IBatchTranslator
│   ├── Model/CloudTranslationRequest.cs   # POST body for v4/mt/translations/async
│   ├── Model/CloudTranslationStatus.cs    # GET status payload incl. qualityEstimation array + DominantLabel()
│   ├── Model/EdgeTranslationRequestContent.cs  # POST body for Edge translate (inputFormat: text/html)
│   └── CredentialManager.cs       # Reads/writes credentials to/from the Trados credential store
│
├── Studio/
│   ├── TranslationProvider/
│   │   ├── TranslationProviderFactory.cs        # [TranslationProviderFactory] – creates providers
│   │   ├── TranslationProvider.cs               # ITranslationProvider implementation; selects Cloud vs Edge ITranslationEngine
│   │   └── TranslationProviderLanguageDirection.cs  # Core translation loop; delegates each batch to IBatchTranslator
│   │
│   ├── BatchTask/
│   │   ├── ApplyMetadataBatchTask.cs            # Writes QE metadata into bilingual target files
│   │   ├── CreateQeReportBatchTask.cs           # Generates XML/XSL QE report for a project
│   │   └── Send Feedback/
│   │       └── SendFeedbackBatchTask.cs         # Sends batch feedback to Cloud or Edge
│   │
│   ├── FeedbackController/
│   │   └── FeedbackController.cs               # Side-panel [ViewPart] docked in Editor
│   │
│   ├── TellMe/
│   │   └── TellMeProvider.cs                   # [TellMeProvider] – forum, docs, source, settings
│   │
│   └── Actions/
│       └── CreateDictionaryTerm.cs             # Studio action to add a term to an Edge dictionary
│
├── ViewModel/
│   ├── BaseViewModel.cs                        # INotifyPropertyChanged base
│   ├── CredentialsMainViewModel.cs             # Top-level credentials dialog VM
│   ├── Cloud/CloudCredentialsViewModel.cs
│   ├── Cloud/CloudAuth0ViewModel.cs
│   ├── Edge/EdgeCredentialsViewModel.cs
│   ├── Edge/EdgeAuth0ViewModel.cs
│   ├── SettingsViewModel.cs
│   ├── PairMappingViewModel.cs
│   └── ErrorDialogViewModel.cs
│
├── View/                                       # WPF XAML views (code-behind only)
│
├── LanguageWeaverProviderTests/
│   ├── SegmentTagPlacerTests.cs                # Round-trip + placeholder-edge-case coverage for SegmentTagPlacer
│   ├── PlainTextBatchTranslatorTests.cs        # Orchestration coverage for PlainTextBatchTranslator (stub ITranslationEngine)
│   └── CloudServiceIntegrationTests.cs        # Live integration tests against the Language Weaver Cloud API
│
├── LanguageMappingProvider/
│   └── DatabaseControl.cs                      # Initializes the language-code SQLite DB
│
├── CohereSubscription/                         # Cohere upsell journey (orchestrator, workflow, decision, settings)
├── SubscriptionJourney/                        # Generic subscription dialog (ViewModel + View)
├── Send feedback/                              # LanguageWeaverFeedback, CloudFeedback, EdgeFeedback
├── WindowsCredentialStore/                     # P/Invoke wrapper for Windows Credential Manager
├── Helpers/                                    # ErrorHandling, AnimationsHelper, LoginGeneratorsHelper
├── Extensions/                                 # TranslationOriginExtension, ObjectExtensions
├── Converters/                                 # WPF value converters
├── Controls/                                   # ProgressIndicator, ToggleOption, watermark helpers
└── Style/                                      # Shared WPF ResourceDictionaries
```

---

## Core concepts

### Plugin versions (flavours)

```csharp
public enum PluginVersion { None = 0, LanguageWeaverCloud = 1, LanguageWeaverEdge = 2 }
```

| Flavour | URI scheme | Full URI |
|---|---|---|
| Cloud | `languageweavercloud` | `languageweavercloud:///` |
| Edge  | `languageweaveredge`  | `languageweaveredge:///`  |

`TranslationProviderFactory.SupportsTranslationProviderUri` returns `true` when the scheme starts with `"languageweaver"`.

### Authentication types

```csharp
public enum AuthenticationType
{
	None = 0,
	CloudCredentials = 1,   // username + password
	CloudAPI = 2,            // client_id + API key
	CloudSSO = 3,            // Auth0 PKCE flow
	EdgeCredentials = 4,     // username + password → /api/v2/auth
	EdgeApiKey = 5,          // API key header
	EdgeSSO = 6              // SSO via WebView2
}
```

Auth0 endpoint (Cloud SSO): `https://sdl-prod.eu.auth0.com/oauth/token`
Auth0 client_id (Cloud refresh): `F4NpOGG1sBaEzk379M6ZxX3gGa0iH1Ff`

### TranslationOptions (provider state)

`TranslationOptions` is JSON-serialized into the Trados provider state string and back. Fields marked `[JsonIgnore]` (`AccessToken`, `CloudCredentials`, `EdgeCredentials`) are not persisted — they are re-hydrated at load time by `CredentialManager.GetCredentials()`.

Key properties:
- `Id` – GUID, uniquely identifies this provider instance at runtime
- `PluginVersion` – Cloud or Edge
- `AuthenticationType`
- `PairMappings` – list of `PairMapping` (one per language pair in the project)
- `ProviderSettings` – tags, pre/post-lookup, feedback, resend-drafts flags
- `Uri` – the scheme URI

### PairMapping

Each `PairMapping` links a `LanguagePair` to a chosen `PairModel` (the MT model string) plus optional `PairDictionary` list (Edge only). `TranslationProvider.SupportsLanguageDirection` returns `true` only when a mapping exists **and** `SelectedModel.Model` is non-empty.

---

## Translation flow (per segment)

1. Trados calls `TranslationProviderLanguageDirection.SearchTranslationUnitsMasked` (or `SearchSegment` for single-segment lookup).
2. The method refreshes options from `ApplicationInitializer.TranslationOptions` and validates the token.
3. Segments are extracted and optionally processed by the **pre-lookup editor** (`LWSegmentEditor`).
4. Segments are split into batches and each batch is delegated to `IBatchTranslator.Translate(...)` (default: `PlainTextBatchTranslator`).
5. The batch translator builds a `SegmentTagPlacer` per source segment, encoding inline tags as `<x id="N"/>` placeholders (with consecutive adjacent tags grouped into a single placeholder), and produces a `string[]` to send to the engine.
6. The configured `ITranslationEngine` (`CloudTranslationEngine` or `EdgeTranslationEngine`) calls the underlying static service:
   - **Cloud**: `CloudService.Translate` → 3-step async at `{baseUri}v4/mt/translations/async` (submit → poll status → GET `/content`)
   - **Edge**: `EdgeService.Translate` → POST to `{baseUri}api/v2/translations` (form-urlencoded with base64-encoded `\n`-joined input), poll, then GET `/download` and base64-decode
7. Each `TranslationResult` (translated text + optional QE label) is rehydrated by the matching `SegmentTagPlacer` into a target `Segment` with the original `Tag` instances re-inserted.
8. Results are returned as `EvaluatedSegment` (`{ Segment Translation, string QualityEstimation }`) and the QE score is stored in `ApplicationInitializer.RatedSegments`.
9. The **post-lookup editor** is applied to the translated text.
10. Results are returned to Trados as `SearchResults` with `TranslationMethod.MachineTranslation`.

---

## Tag preservation — `SegmentTagPlacer`

`SegmentTagPlacer` walks `Segment.Elements`, replaces each run of consecutive `Tag` elements with a single numbered placeholder `<x id="N"/>` (grouping consecutive tags so the API never sees two adjacent placeholders with no text between them), and stores the original `Tag` instances in an internal list. Text content is HTML-encoded (`WebUtility.HtmlEncode`) before sending.

After translation, `BuildTargetSegment(translatedText, targetCulture)` scans the translated string with a regex, splices the original `Tag` group back at the matching position, HTML-decodes (`WebUtility.HtmlDecode`) text spans, and treats anything between placeholders as `Text`. Out-of-range placeholder indices are silently dropped; placeholders the engine drops simply omit the corresponding tags.

`inputFormat` is set to `"HTML"` for both Cloud and Edge. HTML mode preserves `<x id="N"/>` placeholders verbatim without repositioning or inserting spaces, and requires the HTML encode/decode round-trip for text content.

**History:** The original implementation used `inputFormat = "XML"`, which caused the API to linguistically reposition tags adjacent to punctuation. Switching to `"PLAIN"` avoided that but caused spurious spaces between consecutive tag placeholders. The fix that stuck combines tag grouping with `"HTML"` format.

`"XLIFF"` was also evaluated. It correctly translates isolated words tightly sandwiched between two `<x/>` placeholders (a case where HTML leaves the word untranslated), but it reintroduces the XML-mode tag-repositioning bug for tags adjacent to punctuation — which is a worse failure because it corrupts document structure. `"HTML"` is therefore the definitive choice. The XLIFF format also requires wrapping each segment in a full XML document and parsing the `<target>` element out of the response, adding payload size and fragility for no net benefit.

**Known engine limitation:** when a single word is directly sandwiched between two `<x/>` placeholders with no whitespace between the word and the placeholders (e.g. `<x id="0"/>puff<x id="1"/>`), the HTML-mode engine does not translate that word — it is returned as-is (sometimes capitalised). This was confirmed by a targeted test using regular spaces on both sides of the placeholder pair, which produced the same untranslated result, ruling out `&#160;` as the cause. The `&#160;` entities in the originally observed case are outside the placeholder sandwich and are not relevant. This is an MT engine behaviour; no available `inputFormat` value fixes it reliably (XLIFF handles this specific case but reintroduces tag repositioning on punctuation, which is worse).

---

## Cloud quality-estimation mapping

The Cloud API returns a per-segment `qualityEstimation` array of `{ good, adequate, poor }` percentages. `CloudQualityEstimation.DominantLabel()` collapses each entry to the single label (`"Good"`, `"Adequate"`, or `"Poor"`) with the highest percentage. QE is requested only when `mappedPair.SelectedModel.QeSupport` is true (sets `qualityEstimation: 1` in the request body). Edge does not currently surface QE through this flow.

---

## Cloud API endpoints

| Region | Base URL |
|---|---|
| EU | `https://api.languageweaver.com/` |
| US | `https://us.api.languageweaver.com/` |

Key paths (all under `v4/`):
- `v4/mt/translations/async` – submit translation (POST, returns `requestId`)
- `v4/mt/translations/async/{requestId}` – poll status (`INIT` / `TRANSLATING` / `DONE` / `FAILED`)
- `v4/mt/translations/async/{requestId}/content` – retrieve translated segments + QE array
- `v4/token` / `v4/token/user` – authenticate (API credentials / username+password)
- `v4/accounts/users/self` / `v4/accounts/api-credentials/self` – fetch `accountId`
- `v4/accounts/{accountId}/subscriptions/language-pairs` – list available language pairs
- `v4/accounts/{accountId}/dictionaries` – list dictionaries
- `v4/accounts/{accountId}/feedback/translations` – submit/update feedback

Portal: EU `https://portal.languageweaver.com/login` | US `https://us.portal.languageweaver.com/login`

---

## Edge API endpoints

All paths relative to the Edge server base URI (stored in `AccessToken.BaseUri`):

| Path | Method | Purpose |
|---|---|---|
| `/api/v2/auth` | POST | Authenticate (username/password) |
| `/api/v2/language-pairs` | GET | List language pairs |
| `/api/v2/mt/translations/...` | POST | Translate |
| `/api/v2/dictionaries` | GET | List dictionaries (paged, 1000/page) |
| `/api/v2/dictionaries/{id}/term` | POST | Add dictionary term |
| `/api/v2/feedback` | POST | Submit feedback |

---

## Batch tasks

| Task ID | Class | Purpose |
|---|---|---|
| `Apply Language Weaver Metadata` | `ApplyMetadataBatchTask` | Writes QE data from `RatedSegments` into bilingual target segment metadata |
| `Create QE Report` | `CreateQEReportBatchTask` | Generates an XML report transformed with `Resources/Report.xsl` |
| `Send Feedback` | `SendFeedbackBatchTask` | Sends bulk post-translation feedback to Cloud or Edge |

---

## Feedback system

- **Interactive feedback**: the `FeedbackController` side-panel (docked left in the Editor) lets translators rate segments and flag error types.
- **Auto-send feedback**: controlled by `ProviderSettings.AutosendFeedback`; sends after each translation.
- **Batch feedback**: `SendFeedbackBatchTask` processes all segments in a project.
- Error categories: Words Omission, Words Addition, Word Choice, Unintelligible, Grammar, Spelling, Punctuation, Capitalization, Capitalization+Punctuation.

---

## Credential storage

1. **Trados credential store** (`ITranslationProviderCredentialStore`) – primary store, session-scoped.
2. **Windows Credential Manager** (`WindowsCredentialStore/CredentialStore.cs`) – persists credentials across sessions using P/Invoke (`advapi32.dll`).

`CredentialManager` (in `Extensions/`) bridges these two stores.

---

## ApplicationInitializer (global state)

`ApplicationInitializer` is marked `[ApplicationInitializer]` so Trados loads it on startup.

| Static member | Purpose |
|---|---|
| `CurrentAppVersion` | Trados Studio file version |
| `RatedSegments` | In-memory list of all QE-rated segments in the current session |
| `CredentialStore` | Active `ITranslationProviderCredentialStore` |
| `PluginVersion` | Currently active flavour |
| `TranslationOptions` | Dictionary keyed by options `Id` — updated per provider instance |
| `IsStandalone` | `true` when used outside Trados Studio (e.g., via automation) |

---

## MVVM conventions

- All ViewModels inherit `BaseViewModel` which implements `INotifyPropertyChanged` with `[CallerMemberName]`.
- Views are WPF XAML; code-behind is minimal (only wiring DataContext or event routing).
- Commands use `RelayCommand` (sync) or `AsyncRelayCommand` (async).
- Two separate `RelayCommand` implementations exist: `Command/RelayCommand.cs` (main) and `SubscriptionJourney/Command/RelayCommand.cs` (subscription journey only).

---

## Key constants quick reference

```csharp
Constants.CloudFullScheme        = "languageweavercloud:///"
Constants.EdgeFullScheme         = "languageweaveredge:///"
Constants.CloudEUUrl             = "https://api.languageweaver.com/"
Constants.CloudUSUrl             = "https://us.api.languageweaver.com/"
Constants.PluginName             = "Language Weaver Provider"
Constants.PluginNameCloud        = "Language Weaver Cloud"
Constants.PluginNameEdge         = "Language Weaver Edge"
Constants.DatabaseName           = "languageweaver"
Constants.TellMe_Documentation_Url = "https://appstore.rws.com/Plugin/240?tab=documentation"
Constants.TellMe_SourceCode_Url    = "https://github.com/RWS/Sdl-Community/tree/master/LanguageWeaverProvider"
```

---

## Coding conventions

- **Static service classes** – `CloudService`, `EdgeService`, `Service` are all `public static class`. The Studio-facing translation path goes through the `ITranslationEngine` / `IBatchTranslator` seam so the orchestrator stays decoupled and unit-testable.
- **Async** – all HTTP calls are `async`/`await`; the SDL `ITranslationProvider` contract is synchronous, so `PlainTextBatchTranslator` blocks on `.Result` at the seam.
- **Null-handling** – C# 8+ null-conditional (`?.`) and null-coalescing (`??`) are used; nullable reference types are NOT enabled project-wide.
- **Error dialogs** – `ErrorHandling.ShowDialog(owner, title, message)` for user-facing errors; NLog logger (`LogManager.GetCurrentClassLogger()`) for structured logs.
- **PluginResources** – all user-visible strings that need localization come from `PluginResources.resx` / `PluginResources.Designer.cs`.
- **No DI container** – dependencies are passed as constructor arguments or resolved via static factory methods.

---

## Integration test project — `LanguageWeaverProviderTests\CloudServiceIntegrationTests.cs`

A live integration test file that exercises the Language Weaver API directly (outside of Trados Studio).

**Key design decisions:**
- Auth is bypassed entirely — a Bearer token is hardcoded in `AccessTokenValue`. Paste a current token there before running.
- `SourceLang`, `TargetLang`, `Model`, and `Region` are hardcoded constants. Change them directly in the file.
- The token is hardcoded (not an env var) because this is a private diagnostic tool, not CI.
- Tests use `ITestOutputHelper` for all output. Run via `vstest.console.exe` (not `dotnet test`).

**Tests:**
- `TagPlacement_XML` / `TagPlacement_PLAIN` — send two representative segments under each format and log every raw HTTP response body.
- `HtmlEncoding_SpecialCharsInText_RoundTripsCorrectly` — confirms HTML special characters survive the encode/decode round-trip.
- `PluginPath_CloudServiceTranslate` — calls `CloudService.Translate` directly, reproducing exactly what Studio does.

---

## Additional reference documents

- [languageweaver-async-translation.md](languageweaver-async-translation.md) — async translation flow details
- [lw-edge-translations-api.md](lw-edge-translations-api.md) — Language Weaver Edge translations API reference
