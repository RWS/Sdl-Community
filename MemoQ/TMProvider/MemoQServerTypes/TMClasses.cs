using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMProvider.MemoQServerTypes
{
    internal class TMModel
    {
        public ResourceAccessLevel AccessLevel { get; set; }
        public string Client { get; set; }
        public string Domain { get; set; }
        public string Subject { get; set; }
        public string Project { get; set; }
        public int NumEntries { get; set; }
        public string FriendlyName { get; set; }
        public string SourceLangCode { get; set; }
        public string TargetLangCode { get; set; }
        public string TMGuid { get; set; }
        public string TMOwner { get; set; }

    }


    //  Later it may be needed to create separete classes for different purposes (e.g. UpdateTranslationUnitModel)
    internal class TMEntryModel
    {
        public string Client { get; set; }
        public string ContextID { get; set; }
        public DateTime Created { get; set; }
        public string Creator { get; set; }
        public string Document { get; set; }
        public string Domain { get; set; }
        public string FollowingSegment { get; set; }
        public int Key { get; set; }
        public DateTime Modified { get; set; }
        public string Modifier { get; set; }
        public string PrecedingSegment { get; set; }
        public string Project { get; set; }
        public string SourceSegment { get; set; }
        public string Subject { get; set; }
        public string TargetSegment { get; set; }
    }

 
    internal class TMHitsPerSegment
    {
        public TMHit[] TMHits;
    }

    internal class TMHit
    {
        public int MatchRate;
        public TMEntryModel TransUnit;
    }
}
