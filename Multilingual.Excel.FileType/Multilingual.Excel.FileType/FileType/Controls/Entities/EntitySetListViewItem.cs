using System.Windows.Forms;

namespace Multilingual.XML.FileType.FileType.Controls.Entities
{
    class EntitySetListViewItem : ListViewItem
    {
        string _EntitySetId = "";
        string _EntitySetName = "";
     //   string _Type = "";

        public EntitySetListViewItem(string setId, string setDisplayName)
        {
            _EntitySetId = setId;
            _EntitySetName = setDisplayName;
         //   _Type = setType;
            this.Text = setDisplayName;
        //    this.SubItems.Add(setType);
            
       //     this.Checked = isActive;
        }

        public string EntitySetId
        {
            get
            {
                return _EntitySetId;
            }
        }


        public string EntitySetDisplayName
        {
            get
            {
                return _EntitySetName;
            }
        }


        //public string EntitySetType
        //{
        //    get
        //    {
        //        return _Type;
        //    }
        //    set
        //    {
        //        _Type = value;
        //        this.SubItems[1].Text = _Type;
        //    }
        //}
    }
}
