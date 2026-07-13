using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sdl.Community.AntidoteVerifier.Connectix.Protocol
{
    /// <summary>
    /// Central JSON (de)serialization for the Antidote Connectix protocol.
    /// Wire field names are pinned per-property via <see cref="JsonPropertyAttribute"/> to match
    /// Druide's reference implementation; null members are omitted so optional keys disappear.
    /// </summary>
    public static class AntidoteJson
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.None
        };

        public static string Serialize(object value) => JsonConvert.SerializeObject(value, Settings);

        public static T Deserialize<T>(string json) => JsonConvert.DeserializeObject<T>(json, Settings);
    }

    /// <summary>
    /// Transport frame. Large messages are split across consecutive frames and reassembled
    /// by concatenating <see cref="Data"/> once all frames have arrived.
    /// </summary>
    public sealed class Frame
    {
        [JsonProperty("idFrame")] public int IdFrame { get; set; }
        [JsonProperty("totalFrame")] public int TotalFrame { get; set; }
        [JsonProperty("data")] public string Data { get; set; }
    }

    public static class IncomingMessageType
    {
        public const string Init = "init";
        public const string DocumentPath = "documentPath";
        public const string DocIsAvailable = "docIsAvailable";
        public const string GetTextZones = "getTextZones";
        public const string AllowEdit = "allowEdit";
        public const string Replace = "replace";
        public const string Select = "select";
        public const string ReturnToDocument = "returnToDocument";
        public const string NewCorrectionMemory = "newCorrectionMemory";
        public const string Send = "send";
        public const string AntiOopsResponse = "antiOopsResponse";
    }

    /// <summary>Envelope for any message pushed by Antidote.</summary>
    public sealed class IncomingMessage
    {
        [JsonProperty("idMessage")] public string IdMessage { get; set; }
        [JsonProperty("message")] public string Message { get; set; }
        [JsonProperty("data")] public JToken Data { get; set; }

        // Antidote 11 v4 sends this at the top level instead of inside "data".
        [JsonProperty("forActiveSelection")] public bool? ForActiveSelection { get; set; }
    }

    public sealed class AllowEditParams
    {
        [JsonProperty("zoneId")] public string ZoneId { get; set; }
        [JsonProperty("context")] public string Context { get; set; }
        [JsonProperty("positionStart")] public int PositionStart { get; set; }
        [JsonProperty("positionEnd")] public int PositionEnd { get; set; }
    }

    public sealed class ReplaceParams
    {
        [JsonProperty("zoneId")] public string ZoneId { get; set; }
        [JsonProperty("newString")] public string NewString { get; set; }
        [JsonProperty("positionStartReplace")] public int PositionStartReplace { get; set; }
        [JsonProperty("positionReplaceEnd")] public int PositionReplaceEnd { get; set; }
    }

    public sealed class SelectParams
    {
        [JsonProperty("zoneId")] public string ZoneId { get; set; }
        [JsonProperty("positionStart")] public int PositionStart { get; set; }
        [JsonProperty("positionEnd")] public int PositionEnd { get; set; }
    }

    public static class AntidoteTool
    {
        public const string Corrector = "Corrector";
        public const string Dictionaries = "Dictionaries";
        public const string Guides = "Guides";
    }

    /// <summary>How Antidote identifies a document's correction cache (see <c>cacheIdType</c>).</summary>
    public static class CacheIdentifierType
    {
        /// <summary>Use the document path as the key for Antidote's persisted correction-state cache
        /// AND (within one connection) as the correction-tab identity: launching a new path opens a
        /// new tab, relaunching an existing path re-focuses its tab (verified live; see the
        /// migration doc for the Studio-specific single-tab behaviour).</summary>
        public const string ForcePath = "forcePath";
    }

    /// <summary>Outgoing message that opens one of the Antidote tools.</summary>
    public sealed class LaunchToolMessage
    {
        [JsonProperty("apiVersion")] public int ApiVersion { get; set; } = 2;
        [JsonProperty("message")] public string Message { get; set; } = "LaunchTool";
        [JsonProperty("toolApi")] public string ToolApi { get; set; }
    }

    /// <summary>Word-processor configuration returned to the <c>init</c> message. All keys are optional.</summary>
    public class WordProcessorConfiguration
    {
        [JsonProperty("documentTitle")] public string DocumentTitle { get; set; }
        [JsonProperty("carriageReturn")] public string CarriageReturn { get; set; }
        [JsonProperty("cacheIdType")] public string CacheIdType { get; set; }
        [JsonProperty("allowCarriageReturn")] public bool? AllowCarriageReturn { get; set; }
        [JsonProperty("allowNBSpace")] public bool? AllowNbSpace { get; set; }
        [JsonProperty("allowThinSpace")] public bool? AllowThinSpace { get; set; }
        [JsonProperty("allowSending")] public bool? AllowSending { get; set; }
        [JsonProperty("replaceWithoutSelection")] public bool? ReplaceWithoutSelection { get; set; }
        [JsonProperty("correctionMemory")] public string CorrectionMemory { get; set; }
        [JsonProperty("activeMarkup")] public string ActiveMarkup { get; set; }
    }

    public sealed class InitResponse : WordProcessorConfiguration
    {
        [JsonProperty("idMessage")] public string IdMessage { get; set; }
    }

    public sealed class DocumentPathResponse
    {
        [JsonProperty("idMessage")] public string IdMessage { get; set; }
        [JsonProperty("data")] public string Data { get; set; }
    }

    public sealed class DocIsAvailableResponse
    {
        [JsonProperty("idMessage")] public string IdMessage { get; set; }

        // Antidote reads the availability flag from "data" (matching the README and every other
        // response DTO). Druide's Python reference still serializes the legacy "donnees" key, so we
        // emit BOTH: answering the docIsAvailable poll under "data" keeps the correction tab open,
        // instead of Antidote reading a missing "data", treating the document as gone, and marking
        // the tab "(closed)". "donnees" is kept for parity with older Antidote builds.
        [JsonProperty("donnees")] public bool Donnees { get; set; }
        [JsonProperty("data")] public bool Data => Donnees;
    }

    /// <summary>Response carrying a single boolean in <c>data</c> (allowEdit / replace).</summary>
    public sealed class BoolDataResponse
    {
        [JsonProperty("idMessage")] public string IdMessage { get; set; }
        [JsonProperty("data")] public bool Data { get; set; }
    }

    public sealed class TextZonesResponse
    {
        [JsonProperty("idMessage")] public string IdMessage { get; set; }
        [JsonProperty("data")] public IReadOnlyList<TextZone> Data { get; set; }
    }

    public sealed class TextZone
    {
        [JsonProperty("text")] public string Text { get; set; }
        [JsonProperty("zoneId")] public string ZoneId { get; set; }
        [JsonProperty("positionSelectionStart")] public int? PositionSelectionStart { get; set; }
        [JsonProperty("positionSelectionEnd")] public int? PositionSelectionEnd { get; set; }
        [JsonProperty("zoneIsFocused")] public bool ZoneIsFocused { get; set; }
        [JsonProperty("styleInfo")] public IReadOnlyList<StyleInfo> StyleInfo { get; set; }
    }

    public sealed class StyleInfo
    {
        [JsonProperty("positionStart")] public int PositionStart { get; set; }
        [JsonProperty("positionEnd")] public int PositionEnd { get; set; }
        [JsonProperty("style")] public string Style { get; set; }
    }
}
