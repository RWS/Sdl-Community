namespace ExportToExcel
{
    public class LayoutType
    {          
        public enum TableType { SideBySide, TopDown };

        public LayoutType(TableType currentLayout)
        {
            CurrentLayout = currentLayout;
        }

        public TableType CurrentLayout
        {
            get;
            set;
        }
    }
}
