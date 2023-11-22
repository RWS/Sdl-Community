using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Formatting;
using Sdl.FileTypeSupport.Framework.IntegrationApi.QuickInserts;

namespace Multilingual.Excel.FileType.QuickTags
{
    public sealed class QuickInsert : IQuickInsert
    {

        private QuickInsertIds _id;
        private string _name;
        private string _description;
        private IAbstractMarkupDataContainer _markupData;
        private IFormattingGroup _formatting;
        private bool _displayOnToolBar;

        public QuickInsert()
        {

        }

        public QuickInsert(QuickInsertIds id, string name, string description, IAbstractMarkupDataContainer markupData, IFormattingGroup formatting, bool displayOnToolbar)
        {
            _id = id;
            _name = name;
            _description = description;
            _markupData = markupData;
            _formatting = formatting;
            _displayOnToolBar = displayOnToolbar;
        }

        public QuickInsert(QuickInsert old)
        {
            this._id = old.Id;
            this._name = old.Name.Clone() as string;
            this._description = old.Description.Clone() as string;
            this._markupData = CloneMarkupData(old.MarkupData);
            this._formatting = old.Formatting?.Clone() as IFormattingGroup;
            this._displayOnToolBar = old.DisplayOnToolBar;
        }

        private IAbstractMarkupDataContainer CloneMarkupData(IAbstractMarkupDataContainer markupData)
        {
            var container = new MarkupDataContainer();
            foreach (var item in markupData.AllSubItems)
            {
                var clonedItem = item.Clone() as IAbstractMarkupData;
                container.Add(clonedItem);
            }
            return container;
        }

        /// <summary>
        /// ID of QuickInsert
        /// </summary>
        public QuickInsertIds Id { get { return _id; } }

        /// <summary>
        /// Name of QuickInsert
        /// </summary>
        public string Name { get { return _name; } }

        /// <summary>
        /// Description of QuickInsert
        /// </summary>
        public string Description { get { return _description; } }
        /// <summary>
        /// Markup data
        /// </summary>
        public IAbstractMarkupDataContainer MarkupData { get { return _markupData; } }

        /// <summary>
        /// Formatting for QuickInsert
        /// </summary>
        public IFormattingGroup Formatting { get { return _formatting; } }

        /// <summary>
        /// Boolean indicating whether this QuickInsert should be displayed on the QI Toolbar
        /// </summary>
        public bool DisplayOnToolBar { get { return _displayOnToolBar; } }


        public object Clone()
        {
            return new QuickInsert(this);
        }

        // override object.Equals
        // Note: At the moment, equality is based on the Command property only
        // In theory we could have two objects with the same Command property but with different
        // markup content. We may need to take this into account in future.
        public override bool Equals(object obj)
        {

            //       
            // See the full list of guidelines at
            //   http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconequals.asp    
            // and also the guidance for operator== at
            //   http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconimplementingequalsoperator.asp
            //

            if (!base.Equals(obj))
            {
                return false;
            }

            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            QuickInsert qi = obj as QuickInsert;
            if (Id != qi.Id)
            {
                return false;
            }

            // everything matches - we don't check markup and other settings as
            return true;
        }


        // override object.GetHashCode
        // At present, we only use the Id property to calculate the HashCode.
        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }
    }
}
