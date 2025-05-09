using QuickInfo;
using System;
using System.Collections.Generic;
using System.Net;
using static QuickInfo.NodeFactory;

namespace TradosStudioQuickInfo.Processors
{
    public class Ip : IProcessor

    {

        private static HashSet<string> triggers = new HashSet<string>(StringComparer.OrdinalIgnoreCase)

        {

            "ip",

            "ip address",

            "my ip",

            "what is my ip"

        };



        public object GetResult(Query query)

        {

            if (query.IsHelp)

            {
                return HelpTable(("ip", "Your IP address"));

            }





            if (triggers.Contains(query.OriginalInput.Trim()))

            {
                var strHostName = Dns.GetHostName();
                IPHostEntry ipHostEntry = Dns.GetHostEntry(strHostName);
                IPAddress[] address = ipHostEntry.AddressList;
                return new[]
                {
                    FixedParagraph($"Your ip address is {address[3].ToString()}")
                };

            }

            return null;

        }

    }
}
