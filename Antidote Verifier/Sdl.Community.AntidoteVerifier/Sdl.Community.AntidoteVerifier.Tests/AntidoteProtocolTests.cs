using Newtonsoft.Json.Linq;
using Sdl.Community.AntidoteVerifier.Connectix.Protocol;
using Xunit;

namespace Sdl.Community.AntidoteVerifier.Tests
{
    /// <summary>
    /// Locks the on-the-wire contract: exact field names/casing, null omission, the docIsAvailable
    /// availability keys ("data" + "donnees"), frame reassembly, and the positionStartReplace override.
    /// These mirror Druide's reference code (the authority over the README's looser JSON samples).
    /// </summary>
    public class AntidoteProtocolTests
    {
        [Fact]
        public void LaunchTool_serializes_with_camelCase_and_apiVersion2()
        {
            var json = AntidoteJson.Serialize(new LaunchToolMessage { ToolApi = AntidoteTool.Corrector });

            var o = JObject.Parse(json);
            Assert.Equal(2, (int)o["apiVersion"]);
            Assert.Equal("LaunchTool", (string)o["message"]);
            Assert.Equal("Corrector", (string)o["toolApi"]);
        }

        [Fact]
        public void ReplaceParams_use_positionStartReplace_and_positionReplaceEnd()
        {
            const string json = "{\"zoneId\":\"3\",\"newString\":\"eats\",\"positionStartReplace\":3,\"positionReplaceEnd\":6}";

            var p = AntidoteJson.Deserialize<ReplaceParams>(json);

            Assert.Equal("3", p.ZoneId);
            Assert.Equal("eats", p.NewString);
            Assert.Equal(3, p.PositionStartReplace);
            Assert.Equal(6, p.PositionReplaceEnd);
        }

        [Fact]
        public void AllowEditParams_map_context_and_positions()
        {
            const string json = "{\"zoneId\":\"1\",\"context\":\"eat\",\"positionStart\":3,\"positionEnd\":6}";

            var p = AntidoteJson.Deserialize<AllowEditParams>(json);

            Assert.Equal("1", p.ZoneId);
            Assert.Equal("eat", p.Context);
            Assert.Equal(3, p.PositionStart);
            Assert.Equal(6, p.PositionEnd);
        }

        [Fact]
        public void SelectParams_map_positions()
        {
            const string json = "{\"zoneId\":\"2\",\"positionStart\":1,\"positionEnd\":4}";

            var p = AntidoteJson.Deserialize<SelectParams>(json);

            Assert.Equal("2", p.ZoneId);
            Assert.Equal(1, p.PositionStart);
            Assert.Equal(4, p.PositionEnd);
        }

        [Fact]
        public void Configuration_omits_null_keys_but_keeps_text_markup_and_replaceWithoutSelection()
        {
            var config = new WordProcessorConfiguration
            {
                DocumentTitle = "Doc",
                ActiveMarkup = "text",
                CarriageReturn = "\n",
                ReplaceWithoutSelection = true
            };

            var json = AntidoteJson.Serialize(config);
            var o = JObject.Parse(json);

            Assert.Equal("text", (string)o["activeMarkup"]);
            Assert.True((bool)o["replaceWithoutSelection"]);
            Assert.Equal("Doc", (string)o["documentTitle"]);
            // Unset optional keys must be absent, not null.
            Assert.False(o.ContainsKey("allowNBSpace"));
            Assert.False(o.ContainsKey("correctionMemory"));
            Assert.False(o.ContainsKey("cacheIdType"));
        }

        [Fact]
        public void InitResponse_uses_allowNBSpace_key()
        {
            var json = AntidoteJson.Serialize(new InitResponse { IdMessage = "x", AllowNbSpace = true });

            var o = JObject.Parse(json);
            Assert.True((bool)o["allowNBSpace"]);
        }

        [Fact]
        public void DocIsAvailableResponse_serializes_availability_under_data_and_donnees()
        {
            var json = AntidoteJson.Serialize(new DocIsAvailableResponse { IdMessage = "x", Donnees = true });

            var o = JObject.Parse(json);
            // Antidote reads availability from "data"; "donnees" is kept for older-build parity. Emitting
            // only "donnees" made Antidote treat the document as gone and mark the tab "(closed)".
            Assert.True((bool)o["data"]);
            Assert.True((bool)o["donnees"]);
        }

        [Fact]
        public void IncomingMessage_reads_forActiveSelection_at_top_level()
        {
            const string json = "{\"idMessage\":\"a\",\"message\":\"getTextZones\",\"forActiveSelection\":true}";

            var m = AntidoteJson.Deserialize<IncomingMessage>(json);

            Assert.Equal(IncomingMessageType.GetTextZones, m.Message);
            Assert.True(m.ForActiveSelection);
        }

        [Fact]
        public void TextZone_omits_selection_positions_when_absent()
        {
            var json = AntidoteJson.Serialize(new TextZone { Text = "hi", ZoneId = "1", ZoneIsFocused = true });

            var o = JObject.Parse(json);
            Assert.Equal("hi", (string)o["text"]);
            Assert.Equal("1", (string)o["zoneId"]);
            Assert.True((bool)o["zoneIsFocused"]);
            Assert.False(o.ContainsKey("positionSelectionStart"));
            Assert.False(o.ContainsKey("positionSelectionEnd"));
            Assert.False(o.ContainsKey("styleInfo"));
        }

        [Fact]
        public void FrameAssembler_returns_null_until_all_frames_arrive_then_concatenates()
        {
            var assembler = new FrameAssembler();

            var first = assembler.Add(new Frame { IdFrame = 0, TotalFrame = 2, Data = "{\"a\":" });
            Assert.Null(first);

            var complete = assembler.Add(new Frame { IdFrame = 1, TotalFrame = 2, Data = "1}" });
            Assert.Equal("{\"a\":1}", complete);
        }

        [Fact]
        public void FrameAssembler_handles_single_frame_message()
        {
            var assembler = new FrameAssembler();

            var complete = assembler.Add(new Frame { IdFrame = 0, TotalFrame = 1, Data = "{}" });

            Assert.Equal("{}", complete);
        }

        [Fact]
        public void FrameAssembler_resets_between_messages()
        {
            var assembler = new FrameAssembler();

            Assert.Equal("A", assembler.Add(new Frame { IdFrame = 0, TotalFrame = 1, Data = "A" }));
            Assert.Equal("B", assembler.Add(new Frame { IdFrame = 0, TotalFrame = 1, Data = "B" }));
        }
    }
}
