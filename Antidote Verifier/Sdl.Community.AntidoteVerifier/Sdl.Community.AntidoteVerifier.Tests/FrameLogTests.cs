using System.Text;
using Sdl.Community.AntidoteVerifier.Connectix.Protocol;
using Xunit;

namespace Sdl.Community.AntidoteVerifier.Tests
{
    /// <summary>
    /// Performance guard on the plugin log. Studio logs frames at Debug (Log.Setup deliberately stops
    /// short of Trace), and a getTextZones response carries the text of every correctable segment, so
    /// logging frames verbatim wrote megabytes to disk on every zone refresh — and Antidote refreshes
    /// whenever its window moves or the editor scrolls. Abbreviation has to stay cheap AND still say
    /// which message the frame was.
    /// </summary>
    public class FrameLogTests
    {
        [Fact]
        public void Abbreviate_keeps_short_frames_verbatim()
        {
            const string frame = "{\"idFrame\":0,\"totalFrame\":1,\"data\":\"{\\\"message\\\":\\\"init\\\"}\"}";

            Assert.Equal(frame, FrameLog.Abbreviate(frame));
        }

        [Fact]
        public void Abbreviate_keeps_the_message_name_that_closes_an_inbound_frame()
        {
            // Inbound frames name the message at the END of the escaped payload, so a head-only
            // truncation would drop the one field that says what Antidote asked for.
            var frame = "{\"data\":\"{\\\"data\\\":{\\\"forActiveSelection\\\":false}," + new string('x', 5000) +
                        ",\\\"message\\\":\\\"getTextZones\\\"}\",\"idFrame\":0,\"totalFrame\":1}";

            var abbreviated = FrameLog.Abbreviate(frame);

            Assert.Contains("getTextZones", abbreviated);
            Assert.Contains("idFrame", abbreviated);
            Assert.Contains("chars omitted", abbreviated);
        }

        [Fact]
        public void Abbreviate_shrinks_a_whole_document_zone_response_to_a_log_line()
        {
            var frame = ZoneResponse(12000);

            var abbreviated = FrameLog.Abbreviate(frame);

            Assert.True(frame.Length > 1_000_000, "the payload this guards against is megabytes");
            Assert.True(abbreviated.Length < 1000, "abbreviated frame was " + abbreviated.Length + " chars");
        }

        // Shaped like a real getTextZones response: one zone per correctable segment.
        private static string ZoneResponse(int zones)
        {
            var sb = new StringBuilder("{\"idFrame\":0,\"totalFrame\":1,\"data\":\"{\\\"data\\\":[");
            for (var i = 1; i <= zones; i++)
            {
                if (i > 1) sb.Append(',');
                sb.Append("{\\\"text\\\":\\\"The topic structure is the result of some conditions we established.\\\",")
                  .Append("\\\"zoneId\\\":\\\"d1:").Append(i).Append("\\\",\\\"zoneIsFocused\\\":false}");
            }

            return sb.Append("]}\"}").ToString();
        }
    }
}
