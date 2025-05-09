namespace GroupshareExcelAddIn.Models
{
    public class Progress
    {
        public Progress(int numerator, int denominator = 0)
        {
            Numerator = numerator;
            Denominator = denominator;
        }

        public int Denominator { get; set; }
        public int Numerator { get; set; }
    }
}