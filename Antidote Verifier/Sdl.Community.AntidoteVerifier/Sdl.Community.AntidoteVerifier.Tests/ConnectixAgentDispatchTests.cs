using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Sdl.Community.AntidoteVerifier.Connectix;
using Sdl.Community.AntidoteVerifier.Connectix.Protocol;
using Sdl.Community.AntidoteVerifier.Tests.Fakes;
using Xunit;

namespace Sdl.Community.AntidoteVerifier.Tests
{
    /// <summary>
    /// Verifies the dispatcher turns incoming framed messages into the correct outgoing frames:
    /// idMessage echoing, the right response payload per message type, frame reassembly, and the
    /// no-response messages (select/returnToDocument) staying silent.
    /// </summary>
    public class ConnectixAgentDispatchTests
    {
        private sealed class SpyAgent : IWordProcessorAgent
        {
            public bool? LastForActiveSelection;
            public AllowEditParams LastAllowEdit;
            public ReplaceParams LastReplace;
            public SelectParams LastSelect;
            public int ReturnToDocumentCalls;

            public WordProcessorConfiguration Configuration() =>
                new WordProcessorConfiguration { DocumentTitle = "Doc", ActiveMarkup = "text", ReplaceWithoutSelection = true };

            public string DocumentPath() => "the/path";

            public bool IsDocumentAvailable() => true;

            public IReadOnlyList<TextZone> ZonesToCorrect(bool forActiveSelection)
            {
                LastForActiveSelection = forActiveSelection;
                return new List<TextZone> { new TextZone { Text = "hi", ZoneId = "1", ZoneIsFocused = true } };
            }

            public bool AllowEdit(AllowEditParams parameters) { LastAllowEdit = parameters; return true; }

            public bool Replace(ReplaceParams parameters) { LastReplace = parameters; return true; }

            public void Select(SelectParams parameters) => LastSelect = parameters;

            public void ReturnToDocument() => ReturnToDocumentCalls++;
        }

        private static string Frame(string message, string idMessage, string dataJson = null)
        {
            var inner = dataJson == null
                ? $"{{\"idMessage\":\"{idMessage}\",\"message\":\"{message}\"}}"
                : $"{{\"idMessage\":\"{idMessage}\",\"message\":\"{message}\",\"data\":{dataJson}}}";
            return AntidoteJson.Serialize(new Frame { IdFrame = 0, TotalFrame = 1, Data = inner });
        }

        // Unwraps the single outgoing frame and returns its inner message as a JObject.
        private static JObject LastResponse(FakeTransport transport)
        {
            var frame = AntidoteJson.Deserialize<Frame>(transport.Sent[transport.Sent.Count - 1]);
            return JObject.Parse(frame.Data);
        }

        private static (ConnectixAgent agent, FakeTransport transport, SpyAgent spy) NewAgent()
        {
            var transport = new FakeTransport();
            var spy = new SpyAgent();
            var agent = new ConnectixAgent(transport, spy);
            agent.Connect();
            return (agent, transport, spy);
        }

        [Fact]
        public void Launch_corrector_sends_LaunchTool_frame()
        {
            var (agent, transport, _) = NewAgent();

            agent.Launch(AntidoteTool.Corrector);

            var response = LastResponse(transport);
            Assert.Equal("LaunchTool", (string)response["message"]);
            Assert.Equal("Corrector", (string)response["toolApi"]);
            Assert.Equal(2, (int)response["apiVersion"]);
        }

        [Fact]
        public void Init_message_is_answered_with_configuration_echoing_idMessage()
        {
            var (_, transport, _) = NewAgent();

            transport.PushFrame(Frame(IncomingMessageType.Init, "id-init"));

            var response = LastResponse(transport);
            Assert.Equal("id-init", (string)response["idMessage"]);
            Assert.Equal("text", (string)response["activeMarkup"]);
            Assert.True((bool)response["replaceWithoutSelection"]);
        }

        [Fact]
        public void DocumentPath_message_returns_path_in_data()
        {
            var (_, transport, _) = NewAgent();

            transport.PushFrame(Frame(IncomingMessageType.DocumentPath, "id-dp"));

            var response = LastResponse(transport);
            Assert.Equal("id-dp", (string)response["idMessage"]);
            Assert.Equal("the/path", (string)response["data"]);
        }

        [Fact]
        public void DocIsAvailable_message_returns_donnees_boolean()
        {
            var (_, transport, _) = NewAgent();

            transport.PushFrame(Frame(IncomingMessageType.DocIsAvailable, "id-av"));

            var response = LastResponse(transport);
            Assert.Equal("id-av", (string)response["idMessage"]);
            Assert.True((bool)response["donnees"]);
        }

        [Fact]
        public void GetTextZones_reads_top_level_forActiveSelection_and_returns_zones()
        {
            var (_, transport, spy) = NewAgent();
            var frame = AntidoteJson.Serialize(new Frame
            {
                IdFrame = 0,
                TotalFrame = 1,
                Data = "{\"idMessage\":\"id-z\",\"message\":\"getTextZones\",\"forActiveSelection\":true}"
            });

            transport.PushFrame(frame);

            Assert.True(spy.LastForActiveSelection);
            var response = LastResponse(transport);
            Assert.Equal("id-z", (string)response["idMessage"]);
            var zones = (JArray)response["data"];
            Assert.Single(zones);
            Assert.Equal("1", (string)zones[0]["zoneId"]);
        }

        [Fact]
        public void AllowEdit_message_parses_params_and_returns_boolean_data()
        {
            var (_, transport, spy) = NewAgent();

            transport.PushFrame(Frame(IncomingMessageType.AllowEdit, "id-ae",
                "{\"zoneId\":\"1\",\"context\":\"eat\",\"positionStart\":3,\"positionEnd\":6}"));

            Assert.Equal("1", spy.LastAllowEdit.ZoneId);
            Assert.Equal("eat", spy.LastAllowEdit.Context);
            Assert.Equal(3, spy.LastAllowEdit.PositionStart);
            Assert.Equal(6, spy.LastAllowEdit.PositionEnd);
            var response = LastResponse(transport);
            Assert.Equal("id-ae", (string)response["idMessage"]);
            Assert.True((bool)response["data"]);
        }

        [Fact]
        public void Replace_message_parses_positionStartReplace_and_returns_boolean_data()
        {
            var (_, transport, spy) = NewAgent();

            transport.PushFrame(Frame(IncomingMessageType.Replace, "id-rp",
                "{\"zoneId\":\"1\",\"newString\":\"eats\",\"positionStartReplace\":3,\"positionReplaceEnd\":6}"));

            Assert.Equal("eats", spy.LastReplace.NewString);
            Assert.Equal(3, spy.LastReplace.PositionStartReplace);
            Assert.Equal(6, spy.LastReplace.PositionReplaceEnd);
            var response = LastResponse(transport);
            Assert.Equal("id-rp", (string)response["idMessage"]);
            Assert.True((bool)response["data"]);
        }

        [Fact]
        public void Select_message_forwards_params_and_sends_no_response()
        {
            var (_, transport, spy) = NewAgent();
            var before = transport.Sent.Count;

            transport.PushFrame(Frame(IncomingMessageType.Select, "id-sel",
                "{\"zoneId\":\"2\",\"positionStart\":1,\"positionEnd\":4}"));

            Assert.Equal("2", spy.LastSelect.ZoneId);
            Assert.Equal(1, spy.LastSelect.PositionStart);
            Assert.Equal(4, spy.LastSelect.PositionEnd);
            Assert.Equal(before, transport.Sent.Count); // no response frame
        }

        [Fact]
        public void ReturnToDocument_message_invokes_agent_and_sends_no_response()
        {
            var (_, transport, spy) = NewAgent();
            var before = transport.Sent.Count;

            transport.PushFrame(Frame(IncomingMessageType.ReturnToDocument, "id-ret"));

            Assert.Equal(1, spy.ReturnToDocumentCalls);
            Assert.Equal(before, transport.Sent.Count);
        }

        [Fact]
        public void Send_and_newCorrectionMemory_messages_are_silently_ignored()
        {
            var (_, transport, _) = NewAgent();
            var before = transport.Sent.Count;

            transport.PushFrame(Frame(IncomingMessageType.Send, "id-send"));
            transport.PushFrame(Frame(IncomingMessageType.NewCorrectionMemory, "id-mem", "\"base64\""));

            Assert.Equal(before, transport.Sent.Count);
        }

        [Fact]
        public void Multi_frame_init_message_is_reassembled_then_answered()
        {
            var (_, transport, _) = NewAgent();
            var inner = "{\"idMessage\":\"id-mf\",\"message\":\"init\"}";
            var half = inner.Length / 2;
            var f0 = AntidoteJson.Serialize(new Frame { IdFrame = 0, TotalFrame = 2, Data = inner.Substring(0, half) });
            var f1 = AntidoteJson.Serialize(new Frame { IdFrame = 1, TotalFrame = 2, Data = inner.Substring(half) });
            var before = transport.Sent.Count;

            transport.PushFrame(f0);
            Assert.Equal(before, transport.Sent.Count); // nothing yet
            transport.PushFrame(f1);

            var response = LastResponse(transport);
            Assert.Equal("id-mf", (string)response["idMessage"]);
        }
    }
}
