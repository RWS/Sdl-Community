# LanguageWeaver — Asynchronous Text Translation API

**Base URL:** `https://api.languageweaver.com`

---

## Overview

Translation is a three-step asynchronous process:

1. **Submit** — POST the source text and parameters, receive a `requestId`
2. **Poll** — GET the status using the `requestId` until it is `DONE` or `FAILED`
3. **Retrieve** — GET the translated content using the same `requestId`

---

## Authentication

Every request requires a Bearer token in the `Authorization` header.

```
Authorization: Bearer <access_token>
```

Two token types are accepted:

- **User credentials** — roles: `Admin`, `Linguist`, `Translator`
- **API credentials**

An optional `Trace-ID` header (client-generated UUID) can be included for request tracing. It is echoed back in the response headers.

---

## Step 1 — Submit Translation

```
POST /v4/mt/translations/async
Content-Type: application/json
```

### Request Body

| Field | Type | Required | Default | Description |
|---|---|---|---|---|
| `sourceLanguageId` | string | yes | — | Three-letter language code (e.g. `eng`) or `auto` for automatic detection |
| `targetLanguageId` | string | yes | — | Three-letter language code (e.g. `fra`) |
| `model` | string | no* | — | Identifies the language pair model (e.g. `generic`). Ignored when `sourceLanguageId` is `auto` |
| `input` | string array | yes | — | Content to translate. Each element is treated as a separate segment. Max 2 million characters total |
| `submissionType` | string | no | `text` | Currently only `text` is supported |
| `inputFormat` | string | no | `PLAIN` | Format of the input content. See [Input Formats](#input-formats) |
| `outputFormat` | string | no | Derived from `inputFormat` | Format of the output content |
| `translationMode` | string | no | `quality` | `quality` performs an extended statistical search; `speed` optimises for fastest response |
| `dictionaries` | string array | no | `[]` | List of dictionary UUIDs to apply. When dictionaries share a source term, the first match in the list wins |
| `linguisticOptions` | object | no | — | Key-value pairs of linguistic option name and value (e.g. `{"formality": "Polite"}`). Values are case-sensitive. Must be enabled on the account |
| `qualityEstimation` | int | no | `0` | `0` = disabled, `1` = enabled. Requires the `genericqe` model and must be enabled on the account |

#### Language pair selection rules

- `sourceLanguageId` + `targetLanguageId` + `model` — uniquely identifies a language pair (e.g. `eng-fra-generic`)
- `sourceLanguageId=auto` + `targetLanguageId` — source language is detected automatically; the first alphabetically matching pair in the account is selected; `model` is ignored

### Response Body — 202 Accepted

| Field | Type | Description |
|---|---|---|
| `requestId` | string | ID to use in all subsequent calls |
| `sourceLanguageId` | string | Confirmed source language |
| `targetLanguageId` | string | Confirmed target language |
| `model` | string | Model used |
| `inputFormat` | string | Input format |
| `submissionType` | string | Submission type |

---

## Step 2 — Check Translation Status

```
GET /v4/mt/translations/async/{requestId}
GET /v4/mt/translations/async/{requestId}?includeProgressInfo=true
```

### Query Parameters

| Parameter | Type | Required | Default | Description |
|---|---|---|---|---|
| `includeProgressInfo` | boolean | no | `false` | When `true`, includes `translationProgress` (0–100) in the response |

### Response Body — 200 OK

| Field | Type | Description |
|---|---|---|
| `translationStatus` | string | See status values below |
| `translationProgress` | int | Percentage complete. Populated only when `includeProgressInfo=true` |
| `inputFormat` | string | Present when status is `DONE` |
| `outputFormat` | string | Present when status is `DONE` |
| `translationStats` | object | Present when status is `DONE`. See below |
| `qualityEstimation` | array | Present when status is `DONE` and quality estimation was requested. See below |

#### Translation status values

| Status | Description |
|---|---|
| `INIT` | Request received and being validated |
| `TRANSLATING` | Translation in progress |
| `DONE` | Translation complete. Proceed to Step 3 |
| `FAILED` | Translation failed. The request must be resubmitted; this state is unrecoverable |

#### `translationStats` fields

| Field | Type | Description |
|---|---|---|
| `inputWordCount` | int | Word count of source input |
| `inputCharCount` | int | Character count of source input |
| `inputByteCount` | int | Byte count of source input |
| `translationWordCount` | int | Word count of translated output |
| `translationCharCount` | int | Character count of translated output |
| `translationByteCount` | int | Byte count of translated output |

#### `qualityEstimation` fields (per input segment)

| Field | Type | Description |
|---|---|---|
| `good` | int | Percentage of segment estimated as good quality |
| `adequate` | int | Percentage estimated as adequate |
| `poor` | int | Percentage estimated as poor |

---

## Step 3 — Retrieve Translated Content

```
GET /v4/mt/translations/async/{requestId}/content
```

Only call this once status is `DONE`.

### Response Body — 200 OK

| Field | Type | Description |
|---|---|---|
| `sourceLanguageId` | string | Source language used |
| `targetLanguageId` | string | Target language used |
| `model` | string | Model used |
| `inputFormat` | string | Format of input |
| `translation` | string array | Translated segments, positionally matching the `input` array |

---

## Input Formats

| Value | Description |
|---|---|
| `PLAIN` | Plain text (UTF-8) |
| `HTML` | HTML |
| `XLINE` | Plain text, one sentence per line |
| `XLIFF` | XML Localization Interchange File Format |
| `TMX` | Translation Memory eXchange |
| `XML` | Extensible Markup Language |
| `SDLXML` | XML where each closing tag marks a segment boundary (contrast with `XML`) |
| `BCM` | Proprietary format |

---

## Error Response

Returned on `400`, `401`, `403`, `500`.

```json
{
  "errors": [
    {
      "code": 47,
      "description": "sourceLanguageId language is not code 3 format or auto. (was 5)"
    }
  ]
}
```

| Field | Type | Description |
|---|---|---|
| `errors` | list | List of errors |
| `code` | int | Error code |
| `description` | string | Human-readable description |

---

## HTTP Status Codes

| Code | Endpoint | Description |
|---|---|---|
| `202` | POST | Translation successfully initiated |
| `200` | GET status, GET content | Request succeeded |
| `400` | All | Invalid input |
| `401` | All | Authentication failed or token invalid |
| `403` | All | Forbidden |
| `500` | All | Internal server error |

---

## Response Headers

| Header | Description |
|---|---|
| `BeGlobal-Request-ID` | Server-generated unique request UUID |
| `Trace-ID` | Echoed back from the request `Trace-ID` header, if provided |
