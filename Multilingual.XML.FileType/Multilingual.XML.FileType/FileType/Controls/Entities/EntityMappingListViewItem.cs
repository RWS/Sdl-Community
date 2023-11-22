using System.Windows.Forms;

namespace Multilingual.XML.FileType.FileType.Controls.Entities
{
    class EntityMappingListViewItem : ListViewItem
    {
        string _Name = "";
        char _Char;

        public EntityMappingListViewItem(string name, char unicodeChar, bool isActive)
        {
            _Name = name;
            _Char = unicodeChar;
            this.Text = name;
            this.SubItems.Add(unicodeChar.ToString());
            int charAsInt = (int)_Char;
            this.SubItems.Add(charAsInt.ToString());
         //   this.Checked = isActive;
        }

        public string EntityName
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
                this.SubItems[0].Text = value;
            }
        }

        public char UnicodeChar
        {
            get
            {
                return _Char;
            }
            set
            {
                _Char = value;
                int charAsInt = (int)value;
                this.SubItems[1].Text = value.ToString();
                this.SubItems[2].Text = charAsInt.ToString();
            }
        }
    }
}
