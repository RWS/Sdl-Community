using System.Collections.Generic;

namespace Sdl.Community.DQF.Core
{
    public class Configuration
    {
        public static string DqfServerRoot = @"https://dqf.taus.net";
        public static string DqfApiVersion = @"/api/v1/";
		
        public static List<QualityLevel> QualityLevel = new List<QualityLevel>
        {
            new QualityLevel { Id=1, Name="Good Enough", DisplayName="Good Enough"},
            new QualityLevel { Id=2, Name="Similar or equal to human translation", DisplayName="Similar or equal to human translation"}
        };
        public static List<CatTool> CatTools = new List<CatTool>
        {
            new CatTool {Id=1, Archive=0, Name="<none>"},
            new CatTool {Id=2, Archive=0, Name="Across"},
            new CatTool {Id=3, Archive=0, Name="Cloudwords"},
            new CatTool {Id=4, Archive=0, Name="Coach"},
            new CatTool {Id=5, Archive=0, Name="Conyac"},
            new CatTool {Id=6, Archive=0, Name="Easyling"},
            new CatTool {Id=7, Archive=0, Name="Gengo"},
            new CatTool {Id=8, Archive=0, Name="Globalsight"},
            new CatTool {Id=9, Archive=0, Name="Google Translator Toolkit"},
            new CatTool {Id=10, Archive=0, Name="Kinetic Globalizer"},
            new CatTool {Id=11, Archive=0, Name="Language Studio"},
            new CatTool {Id=12, Archive=0, Name="MateCat"},
            new CatTool {Id=13, Archive=0, Name="MemoQ"},
            new CatTool {Id=14, Archive=0, Name="Memsource"},
            new CatTool {Id=15, Archive=0, Name="MultiTRANS"},
            new CatTool {Id=16, Archive=0, Name="OmegaT"},
            new CatTool {Id=17, Archive=0, Name="Ontram"},
            new CatTool {Id=18, Archive=0, Name="Plunet"},
            new CatTool {Id=19, Archive=0, Name="SDL TMS"},
            new CatTool {Id=20, Archive=0, Name="SmartCAT"},
            new CatTool {Id=21, Archive=0, Name="SmartMATE"},
            new CatTool {Id=22, Archive=0, Name="TDC Globalink"},
            new CatTool {Id=23, Archive=0, Name="SDL Trados Studio"},
            new CatTool {Id=24, Archive=0, Name="Transit - Star"},
            new CatTool {Id=25, Archive=0, Name="Translation Workspace"},
            new CatTool {Id=26, Archive=0, Name="Wordbee"},
            new CatTool {Id=27, Archive=0, Name="Wordfast"},
            new CatTool {Id=28, Archive=0, Name="Wordfish"},
            new CatTool {Id=29, Archive=0, Name="Worldserver"},
            new CatTool {Id=30, Archive=0, Name="XTM Cloud"},
            new CatTool {Id=31, Archive=0, Name="XTRF"}
        
        };

        public static List<Process> Processes = new List<Process>
        {
            new Process { Id=1, Name="MT+PE+Human"},
            new Process { Id=2, Name="MT+PE"},
            new Process { Id=3, Name="MT+PE+TM+Human"},
            new Process { Id=4, Name="TM+Human"},
            new Process { Id=5, Name="Human"}
        };

        public static List<ContentType> ContentTypes = new List<ContentType>
        {
            new ContentType { Id=1, Archive=0, Name="Audio/Video Content", DisplayName="Audio/Video Content"},
            new ContentType { Id=2, Archive=0, Name="DGT Documents", DisplayName="DGT Documents"},
            new ContentType { Id=3, Archive=0, Name="Marketing Material", DisplayName="Marketing Material"},
            new ContentType { Id=4, Archive=0, Name="Online Help", DisplayName="Online Help"},
            new ContentType { Id=5, Archive=0, Name="Social Media", DisplayName="Social Media"},
            new ContentType { Id=6, Archive=0, Name="Training Material", DisplayName="Training Material"},
            new ContentType { Id=7, Archive=0, Name="User Documentation", DisplayName="User Documentation"},
            new ContentType { Id=8, Archive=0, Name="User Interface Text", DisplayName="User Interface Text"},
            new ContentType { Id=9, Archive=0, Name="Website Content", DisplayName="Website Content"},
            new ContentType { Id=10, Archive=0, Name="Knowledge Base", DisplayName="Knowledge Base"},
            new ContentType { Id=11, Archive=0, Name="Legal", DisplayName="Legal"}
        };

        public static List<Industry> Industries = new List<Industry>
        {
            new Industry { Id=1, Archive=0, Name="Automotive", DisplayName="Automotive"},
            new Industry { Id=2, Archive=0, Name="Chemicals", DisplayName="Chemicals"},
            new Industry { Id=3, Archive=0, Name="Computer Hardware", DisplayName="Computer Hardware"},
            new Industry { Id=4, Archive=0, Name="Computer Software", DisplayName="Computer Software"},
            new Industry { Id=5, Archive=0, Name="Consumer Electronics", DisplayName="Consumer Electronics"},
            new Industry { Id=6, Archive=0, Name="Energy, Water and Utilities", DisplayName="Energy, Water and Utilities"},
            new Industry { Id=7, Archive=0, Name="Financials", DisplayName="Financials"},
            new Industry { Id=8, Archive=0, Name="Healthcare", DisplayName="Healthcare"},
            new Industry { Id=9, Archive=0, Name="Industrial Electronics", DisplayName="Industrial Electronics"},
            new Industry { Id=10, Archive=0, Name="Industrial Manufacturing", DisplayName="Industrial Manufacturing"},
            new Industry { Id=11, Archive=0, Name="Legal Services", DisplayName="Legal Services"},
            new Industry { Id=12, Archive=0, Name="Leisure, Tourism and Arts", DisplayName="Leisure, Tourism and Arts"},
            new Industry { Id=13, Archive=0, Name="Medical Equipment and Supplies", DisplayName="Medical Equipment and Supplies"},
            new Industry { Id=14, Archive=0, Name="Pharmaceuticals and Biotechnology", DisplayName="Pharmaceuticals and Biotechnology"},
            new Industry { Id=15, Archive=0, Name="Professional and Business Services", DisplayName="Professional and Business Services"},
            new Industry { Id=16, Archive=0, Name="Public Sector", DisplayName="Public Sector"},
            new Industry { Id=17, Archive=0, Name="Stores and Retail Distribution", DisplayName="Stores and Retail Distribution"},
            new Industry { Id=18, Archive=0, Name="Telecommunications", DisplayName="Telecommunications"},
            new Industry { Id=19, Archive=0, Name="Undefined Sector", DisplayName="Undefined Sector"},
            new Industry { Id=20, Archive=0, Name="Religion", DisplayName="Religion"}
        };

        public static List<MtEngine> MtEngines = new List<MtEngine>
        {
            new MtEngine { Id=1, Archive=0, Name="<none>"},
            new MtEngine { Id=2, Archive=0, Name="Apertium"},
            new MtEngine { Id=3, Archive=0, Name="Apertium-Moses Hybrid"},
            new MtEngine { Id=4, Archive=0, Name="Asia Online"},
            new MtEngine { Id=5, Archive=0, Name="Bing Translator"},
            new MtEngine { Id=6, Archive=0, Name="Capita MT"},
            new MtEngine { Id=7, Archive=0, Name="Carabao MT"},
            new MtEngine { Id=8, Archive=0, Name="CCID Translation Platform"},
            new MtEngine { Id=9, Archive=0, Name="CrossLang"},
            new MtEngine { Id=10, Archive=0, Name="East Linden"},
            new MtEngine { Id=11, Archive=0, Name="Firma8"},
            new MtEngine { Id=12, Archive=0, Name="FreeT"},
            new MtEngine { Id=13, Archive=0, Name="Google Translate"},
            new MtEngine { Id=14, Archive=0, Name="Iconic"},
            new MtEngine { Id=15, Archive=0, Name="KantanMT"},
            new MtEngine { Id=16, Archive=0, Name="Kodensha"},
            new MtEngine { Id=17, Archive=0, Name="LDS Translator"},
            new MtEngine { Id=18, Archive=0, Name="Linguasys"},
            new MtEngine { Id=19, Archive=0, Name="Lucy Software"},
            new MtEngine { Id=20, Archive=0, Name="Microsoft Translator Hub"},
            new MtEngine { Id=21, Archive=0, Name="Moses"},
            new MtEngine { Id=22, Archive=0, Name="MyMemory"},
            new MtEngine { Id=23, Archive=0, Name="myMT"},
            new MtEngine { Id=24, Archive=0, Name="Opentrad"},
            new MtEngine { Id=25, Archive=0, Name="PangeaMT"},
            new MtEngine { Id=26, Archive=0, Name="Pragma"},
            new MtEngine { Id=27, Archive=0, Name="PROMT"},
            new MtEngine { Id=28, Archive=0, Name="Reverso"},
            new MtEngine { Id=29, Archive=0, Name="Safaba"},
            new MtEngine { Id=30, Archive=0, Name="SDL Language Cloud "},
            new MtEngine { Id=31, Archive=0, Name="Sovee"},
            new MtEngine { Id=32, Archive=0, Name="Systran"},
            new MtEngine { Id=33, Archive=0, Name="T-Text"},
            new MtEngine { Id=34, Archive=0, Name="Tauyou"},
            new MtEngine { Id=35, Archive=0, Name="Toshiba"},
            new MtEngine { Id=36, Archive=0, Name="TransSphere"},
            new MtEngine { Id=37, Archive=0, Name="Weblio"},
            new MtEngine { Id=38, Archive=0, Name="WebTrance"},
            new MtEngine { Id=39, Archive=0, Name="weMT"},
            new MtEngine { Id=40, Archive=0, Name="Other"}
        };

        
    }
}
