namespace Sdl.Community.TMOptimizerLib
{
    public class Settings
    {        
        public Settings()
        {
            //Set default settings
            RemoveOrphan = true;
            ReplaceSoftHyphen = true;
        }        
        
        public bool RemoveOrphan { get; set; }
        public bool ReplaceSoftHyphen { get; set; }
        
    }
}