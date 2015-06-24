using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Sdl.Community.InvoiceAndQuotes.Customers;
using Sdl.Community.InvoiceAndQuotes.Projects;

namespace Sdl.Community.InvoiceAndQuotes.Templates
{
    public interface ITemplateRates : ISerializable
    {
        string Name { get; set; }
        List<RateValue> Rates { get; set; }
        List<RateValue> AdditionalRates { get; set; }
        List<RateValue> GetStandardRates();
        List<RateValue> GetAdditionalStandardRates();
        List<ProjectProperty> FillRatesForProject(List<ProjectProperty> porjectProperties);
        void ClearRates();
        void GenerateClipboardData(List<ProjectFile> projectFiles, Customer customer, User user);
        void GenerateExcelData(List<ProjectFile> files, Customer customer, User user, String fileToSave, String template);
        void GenerateWordData(List<ProjectFile> files, Customer customer, User user, string fileToSave, string template);
    }
}