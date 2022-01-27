using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterpretBank.Service;
using Xunit;

namespace InterpretBankTests
{
    public class SqLiteDatabaseProviderTests
    {
	    [Theory]
		[InlineData("Data Source='{filepath}';Cache=Shared")]
	    public void LoadData(string filepath)
	    {
		    var sqlProvider = new SqLiteDatabaseProvider(filepath);
		    var data = sqlProvider.GlossaryData;
	    }
    }
}
