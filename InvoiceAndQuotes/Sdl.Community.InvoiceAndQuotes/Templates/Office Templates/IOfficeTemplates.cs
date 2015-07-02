using System;
using System.Collections.Generic;

namespace Sdl.Community.InvoiceAndQuotes.Templates
{
    public interface IOfficeTemplates
    {
        List<KeyValuePair<String, String>> GetAllTemplates();
    }
}
