using System.Globalization;

namespace Sdl.Community.AntidoteVerifier.Connectix.Protocol
{
    /// <summary>
    /// Shortens a frame for the plugin log. A getTextZones response carries the text of EVERY
    /// correctable segment — about 165 bytes per segment once escaped, so roughly 2 MB on a
    /// 12,000-segment document — and Antidote re-requests all zones whenever its window moves or
    /// the editor scrolls. Writing that verbatim on every frame costs more than the correction
    /// itself on exactly the documents that were already slow, and it happens inside the transport's
    /// send lock. The transport therefore logs the verbatim frame at Trace (the live harness enables
    /// Trace; Studio does not — see <c>Log.Setup</c>) and this abbreviation at Debug.
    /// </summary>
    public static class FrameLog
    {
        // Enough head to carry idFrame/totalFrame/idMessage and the start of the payload; enough
        // tail to carry the message name, which on an INBOUND frame closes the escaped payload.
        private const int HeadChars = 400;
        private const int TailChars = 120;

        public static string Abbreviate(string frameJson)
        {
            if (frameJson == null || frameJson.Length <= HeadChars + TailChars)
                return frameJson;

            var omitted = frameJson.Length - HeadChars - TailChars;

            return frameJson.Substring(0, HeadChars)
                   + " ...[" + omitted.ToString(CultureInfo.InvariantCulture) + " chars omitted]... "
                   + frameJson.Substring(frameJson.Length - TailChars);
        }
    }
}
