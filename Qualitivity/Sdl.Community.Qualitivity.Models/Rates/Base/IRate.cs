namespace Sdl.Community.Structures.Rates.Base
{
    public enum RoundType
    {
        Roundup = 0,
        Rounddown = 1,
        Round = 2
    }

    
    public interface IRate
    {
        
             
        string SourceLanguage { get; set; }
        string TargetLanguage { get; set; }

        RoundType RndType { get; set; }
        decimal BaseRate { get; set; }
        decimal RatePm { get; set; }
        decimal RateCm { get; set; }
        decimal RateRep { get; set; }
        decimal Rate100 { get; set; }
        decimal Rate95 { get; set; }
        decimal Rate85 { get; set; }
        decimal Rate75 { get; set; }
        decimal Rate50 { get; set; }
        decimal RateNew { get; set; }

       


    }
}
