using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMProvider
{
    public class UpdateData
    {
        public int EntryID { get; private set; }
        public Guid TMGuid { get; private set; }
        public DateTime Modified { get; private set; }

        public UpdateData(int entryID, Guid tmGuid, DateTime modified)
        {
            this.EntryID = entryID;
            this.TMGuid = tmGuid;
            this.Modified = modified;
        }
    }
}
