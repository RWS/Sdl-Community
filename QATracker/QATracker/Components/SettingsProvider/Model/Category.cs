using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QATracker.Components.SettingsProvider.Model
{
    public class Category
    {
        public string Name { get; set; }
        public Category SubCategory { get; set; }
    }
}
