using System;
using Sdl.Community.Structures.Comparer;
using Sdl.Community.Structures.QualityMetrics;

namespace Sdl.Community.Structures.Profile
{
  
    [Serializable]
    public class CompanyProfile: Base.Profile, ICloneable
    {

        public CompanyProfileRate ProfileRate { get; set; }
        public ComparerSettings ComparerOptions { get; set; }
        public QualityMetricGroup MetricGroup { get; set; }

        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }

        public CompanyProfile()
        {

            Id = -1;
            Name = string.Empty;
            Street = string.Empty;
            City = string.Empty;
            State = string.Empty;
            Zip = string.Empty;
            Country = string.Empty;
            TaxCode = string.Empty;
            VatCode = string.Empty;
            Email = string.Empty;
            Web = string.Empty;
            Phone = string.Empty;
            Mobile = string.Empty;
            Fax = string.Empty;
            Note = string.Empty;

            ContactName = string.Empty;
            ContactEmail = string.Empty;
            ContactPhone = string.Empty;


            ProfileRate = new CompanyProfileRate();
            ComparerOptions = new ComparerSettings();
            MetricGroup = new QualityMetricGroup();

        }

        public object Clone()
        {
            var cpi = new CompanyProfile
            {
                Id = Id,
                Name = Name,
                Street = Street,
                City = City,
                State = State,
                Zip = Zip,
                Country = Country,
                TaxCode = TaxCode,
                VatCode = VatCode,
                Email = Email,
                Web = Web,
                Phone = Phone,
                Mobile = Mobile,
                Note = Note,
                ContactName = ContactName,
                ContactEmail = ContactEmail,
                ContactPhone = ContactPhone,
                ProfileRate = (CompanyProfileRate) ProfileRate.Clone(),
                ComparerOptions = (ComparerSettings) ComparerOptions.Clone(),
                MetricGroup = (QualityMetricGroup) MetricGroup.Clone()
            };







            return cpi;
        }
    }
}
