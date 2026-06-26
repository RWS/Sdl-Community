# Language Weaver Edge — Translations API Reference

Base URL: `https://<your-lw-edge-host>/api/v2`

## Authentication

Basic HTTP authentication. Format: `username_apikey:` (append a colon, no password field).

```
Authorization: Basic base64(username_apikey:)
```

---

## Endpoints

### List Translations

```
GET /api/v2/translations
```

Returns a paginated list of translation jobs.

**Query Parameters**

| Parameter | Type | Description |
|-----------|------|-------------|
| `username` | string | Filter by owner username |
| `statuses` | string[] | Filter by state/substate |
| `labels` | string[] | Filter by labels |
| `timeQueuedFrom` | string (ISO 8601) | Filter by queue time (start) |
| `timeQueuedTo` | string (ISO 8601) | Filter by queue time (end) |
| `translationMethod` | string | `MTE` or `GroupShare` |
| `title` | string | Filter by job title |
| `languagePairId` | string | Filter by language pair ID |
| `sourceLanguageId` | string | Filter by source language |
| `targetLanguageId` | string | Filter by target language |
| `page` | integer | Page number |
| `perPage` | integer | Items per page |

**Response**

```json
{
  "page": 1,
  "perPage": 20,
  "totalPages": 5,
  "totalItems": 100,
  "translations": [ /* Translation objects */ ]
}
```

---

### Create Translation (Async)

```
POST /api/v2/translations
```

Submits an asynchronous translation job. Returns immediately with a `translationId`; poll for status separately.

**Request Body**

```json
{
  "languagePairId": "string (required)",
  "input": "string (required, base64-encoded content)",
  "inputFormat": "text/html",
  "outputFormat": "default",
  "encoding": "UTF-8",
  "title": "string",
  "labels": ["string"],
  "dictionaryIds": ["string"],
  "unknownWordHandling": "original",
  "highlightDictionary": false,
  "highlightBrand": false,
  "highlightFeedback": false,
  "highlightUnknown": false,
  "pdfConverter": "Standard",
  "imageConverter": "Standard"
}
```

**Response**

```json
{
  "translationId": "string"
}
```

---

### Create Quick Translation (Sync)

```
POST /api/v2/translations/quick
```

Synchronous translation — blocks until complete and returns the output directly. Recommended for texts under 1 KB.

**Request Body** — same parameters as async `POST /api/v2/translations`.

**Response**

Returns the translated output directly (base64-encoded).

---

### Retrieve Translation Status

```
GET /api/v2/translations/{translationId}
```

Returns full metadata and current state of a translation job.

**Path Parameters**

| Parameter | Type | Description |
|-----------|------|-------------|
| `translationId` | string | ID of the translation job |

**Response**

```json
{
  "translationId": "string",
  "title": "string",
  "state": "done",
  "substate": "succeeded",
  "profile": { /* language pair details */ },
  "results": { /* output info */ },
  "timeQueued": "ISO 8601",
  "timeStarted": "ISO 8601",
  "timeCompleted": "ISO 8601",
  "qualityEstimation": 0.95
}
```

> **Per-segment QE lives in the downloaded XLIFF, not here.** This status `qualityEstimation` is a single job-level number (and is returned as an empty object `{}` for models that do not emit QE). The authoritative per-segment quality estimate ships **inside the translated XLIFF** returned by `/download`, as the `match-quality` attribute on each `<alt-trans>` element (values `Good` / `Adequate` / `Poor`) — identical to the Cloud API. The plugin (`EdgeService.Translate`) reads QE from that attribute via `SegmentSerializer.ExtractQualityEstimation`, never from this field. Verified live: pair `EngFra_Generative-Gen-32888_Cloud` (`qeSupport: true`) returns `match-quality` per segment; pair `EngGer_AutoAdaptive_SRV_TNM` returns `<alt-trans>` with no `match-quality` and a status `qualityEstimation: {}`.

**States & Substates**

| State | Substates |
|-------|-----------|
| `preparing` | `created`, `scheduled` |
| `inProgress` | `translating` |
| `done` | `succeeded`, `failed`, `canceled` |

---

### Download Translation

```
GET /api/v2/translations/{translationId}/download
```

Downloads the translated output as a base64-encoded response.

When the input was submitted as `application/x-xliff`, the decoded output is a consolidated XLIFF 1.2 document that echoes the request `<trans-unit>` ids. Each `<trans-unit>` wraps an `<alt-trans>` holding the `<target>`; when the language pair supports QE (`qeSupport: true`), that `<alt-trans>` also carries a `match-quality="Good|Adequate|Poor"` attribute — the per-segment quality estimate the plugin consumes.

**Query Parameters**

| Parameter | Type | Description |
|-----------|------|-------------|
| `format` | string | `default` or `text/plain` |

---

### Download Multiple Translations (Archive)

```
GET /api/v2/translations/download-archive
```

Downloads multiple translations as a `.zip` archive. Maximum total output size: **1 GB**.

**Query Parameters**

| Parameter | Type | Description |
|-----------|------|-------------|
| `translationIds` | string[] | Specific IDs to include |
| `username` | string | Filter by owner |
| `statuses` | string[] | Filter by status |
| `labels` | string[] | Filter by labels |
| `timeQueuedFrom` | string (ISO 8601) | Filter start time |
| `timeQueuedTo` | string (ISO 8601) | Filter end time |
| `translationMethod` | string | `MTE` or `GroupShare` |

---

### Retrieve Translation Metadata

```
GET /api/v2/translations/{translationId}/metadata
```

Returns translation metadata as a base64-encoded JSON. Only available for completed translations.

---

### Retrieve Content Insights

```
GET /api/v2/translations/{translationId}/content-insights
```

Returns segment-level importance scores for the translated content.

**Response**

```json
{
  "title": "string",
  "segments": [
    {
      "text": "string",
      "importanceScore": 0.87
    }
  ]
}
```

Importance scores range from `0` (low) to `1` (high).

---

### GroupShare Translation

```
POST /api/v2/translations/group-share
```

Submits a translation to GroupShare for human translation and editing. Output format matches the input format.

**Request Body** — same parameters as async `POST /api/v2/translations`.

---

### GroupShare Editor Link

```
GET /api/v2/translations/group-share/{translationId}
```

Returns a link to the GroupShare editor for the given job.

**Response**

```json
{
  "username": "string",
  "editorLink": "string (URL)",
  "authenticationToken": "string"
}
```

---

### Cancel Translation

```
PUT /api/v2/translations/{translationId}/cancel
```

Cancels a queued or in-progress translation. The job remains in queue history with substate `canceled`.

---

### Update Translation

```
PUT /api/v2/translations/{translationId}
```

Updates mutable fields of a translation job. Currently only `labels` can be updated.

**Request Body**

```json
{
  "labels": ["string"]
}
```

---

### Delete Translation

```
DELETE /api/v2/translations/{translationId}
```

Removes a single translation job from the queue and history.

---

### Batch Delete Translations

```
DELETE /api/v2/translations/batch-delete
```

Deletes multiple translation jobs matching the given filter criteria.

**Query Parameters** — same filters as List Translations.

**Response**

```json
{
  "deletedCount": 42
}
```

---

## Parameter Reference

### Input Formats (`inputFormat`)

- `text/html`
- `text/xml`
- `application/pdf`
- `application/msword`
- `image/*` (e.g. `image/png`, `image/jpeg`)
- `message/rfc822`

### Output Formats (`outputFormat`)

- `` (empty / `default`) — matches input format
- `text/plain`
- `application/x-xliff`
- `application/xliff`
- `text/x-tmx`

### Unknown Word Handling (`unknownWordHandling`)

| Value | Behavior |
|-------|----------|
| `hide` | Unknown words are omitted |
| `original` | Keep the source word as-is |
| `transliteration` | Transliterate into target script |
| `transliterationAndOriginal` | Transliteration followed by original in parentheses |

### PDF / Image Converters

- `Standard`
- `Abbyy`

### Translation Methods (`translationMethod`)

- `MTE` — Language Weaver Edge (machine translation)
- `GroupShare` — Human translation via GroupShare
