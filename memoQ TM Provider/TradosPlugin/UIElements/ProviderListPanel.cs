using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using TMProvider;

namespace TradosPlugin
{
    public partial class ProviderListPanel : UserControl
    {
        // provider chosen
        public delegate void ProviderChosenHandler(TMProviderBase provider);
        public event ProviderChosenHandler ProviderChosen;
        // provider deleted
        public delegate void ProviderDeletedHandler(TMProviderBase provider);
        public event ProviderDeletedHandler ProviderDeleted;
        // provider list size changed
        public delegate void SizeChangedHandler(ProviderListPanel sender);
        public event SizeChangedHandler SizeChanged;


        List<TMProviderBase> providerList;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<TMProviderBase> ProviderList
        {
            get { return providerList; }
            set { providerList = value; }
        }

        private int itemHeight;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int IdealItemHeight
        {
            get { return itemHeight; }
            set { itemHeight = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ProviderItemControl SelectedItem { get; private set; }

        private List<ProviderItemControl> providerItemControls = new List<ProviderItemControl>();

        /// <summary>
        /// For the designer only.
        /// </summary>
        public ProviderListPanel()
        {
            InitializeComponent();
        }

        public ProviderListPanel(List<TMProviderBase> providers, int startingWidth, int idealHeightForItems)
        {
            this.Width = startingWidth;
            this.itemHeight = idealHeightForItems;
            this.providerList = providers;
            RefreshProviderList(providerList);
            int ix = getItemToShow();
            if (ix != -1) (this.Controls[ix] as ProviderItemControl).Selected = true;

        }

        void onItemMouseLeave(object sender, EventArgs e)
        {
            ProviderItemControl p = sender as ProviderItemControl;
            if (p == null) return;
            //p.OnMouseLeave(sender, e);
        }

        private void onItemMouseHover(object sender, EventArgs e)
        {
            ProviderItemControl p = sender as ProviderItemControl;
            if (p == null) return;
            p.OnMouseHover(sender, e);
        }

        public void RefreshProviderList(List<TMProviderBase> providerList)
        {
            foreach (ProviderItemControl c in providerItemControls)
            {
                this.Controls.Remove(c);
                c.DeleteClicked -= onItemDeleteClicked;
                c.ItemSelected -= onItemChosen;
                c.MouseHover -= onItemMouseHover;
                c.MouseEnter -= onItemMouseHover;
                c.MouseLeave -= onItemMouseLeave;
                c.Click -= onItemChosen;
                c.SizeChanged -= providerItem_SizeChanged;
                //c.GotFocus -= onItemChosen;
                c.Dispose();
            }
            providerItemControls.Clear();
            int ix = 0;
            foreach (TMProviderBase p in providerList)
            {
                ProviderItemControl c = new ProviderItemControl(p, itemHeight, this.Width);

                c.Height = itemHeight;
                c.Width = this.Width;
                c.Anchor = AnchorStyles.Left | AnchorStyles.Right;
                c.Name = p.ProviderName;
                c.TabIndex = ix;
                c.SetSize(this.Width);
                c.Left = 0;
                if (ix == 0) c.Top = 0;
                else c.Top = this.Controls[ix - 1].Bottom;

                c.DeleteClicked += onItemDeleteClicked;
                c.ItemSelected += onItemChosen;
                c.MouseHover += onItemMouseHover;
                c.MouseEnter += onItemMouseHover;
                c.MouseLeave += onItemMouseLeave;
                c.Click += onItemChosen;
                c.SizeChanged += providerItem_SizeChanged;
                // keeps getting the focus from messagebox
                //c.GotFocus += onItemChosen;

                this.Controls.Add(c);
                providerItemControls.Add(c);
                ix++;
            }
            SetSize(this.Width);
            int first = getItemToShow();
            SelectItem(first);
        }

        void providerItem_SizeChanged(ProviderItemControl sender)
        {
            SetSize(this.Width);
            if (SizeChanged != null) SizeChanged(this);
        }

        public void RefreshProvider(TMProviderBase p)
        {
            // get the control this item belongs to
            int ix = providerList.FindIndex(item => item.ProviderName == p.ProviderName);
            if (ix == -1) throw new InvalidOperationException(String.Format("{0} is not in the list.", p.ProviderName));
            providerItemControls[ix].RefreshProvider(p);
            providerList[ix] = p;
        }

        public void SelectItem(int index)
        {
            if (index < -1 || index > providerList.Count) return;
            foreach (ProviderItemControl item in providerItemControls)
            {
                item.Selected = false;
            }
            if (index == -1)
            {
                SelectedItem = null;
            }
            else
            {
                SelectedItem = providerItemControls[index];
                providerItemControls[index].Selected = true;
            }
        }

        protected override void OnResize(EventArgs e)
        {
            int x = this.Width;
            int y = this.Height;
            base.OnResize(e);
        }

        private void onItemChosen(object sender, EventArgs e)
        {
            ProviderItemControl c = sender as ProviderItemControl;
            if (ProviderChosen != null) ProviderChosen(c.Provider);
            SelectedItem = c;
            foreach (ProviderItemControl item in this.providerItemControls)
            {
                item.Selected = false;
            }
            c.Selected = true;
        }

        private void onItemDeleteClicked(TMProviderBase p)
        {
            if (ProviderDeleted != null) ProviderDeleted(p);
        }

        public void SetSize(int width)
        {
            int count = 0;
            int allHeight = 0;
            while (count < providerItemControls.Count)
            {
                ProviderItemControl item = providerItemControls[count];
                if (item == null) continue;

                item.SetSize(width);
                allHeight += item.Height;
                if (count == 0) item.Top = 0;
                else item.Top = providerItemControls[count - 1].Bottom;
                count++;
            }
            if (allHeight == 0) this.Height = 40;
            this.Height = allHeight;

        }

        private int getItemToShow()
        {
            if (providerList == null || providerList.Count == 0) return -1;

            // if it's not set up but there are no more items
            if (providerList.Count == 1) return -1;
            // else return first memoQ server
            return 1;
        }
    }
}
