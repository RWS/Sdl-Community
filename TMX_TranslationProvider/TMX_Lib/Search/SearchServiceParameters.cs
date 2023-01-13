using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TMX_Lib.Search
{
	// the idea -- simple way to copy the original options
	public class SearchServiceParameters : ISearchServiceParameters
	{
		public string FullFileName { get; set; } = "";

		// at this time, we allow only local connections , so this is pretty much always localhost
		// later, we may allow for cloud atlas connections
		public string DbConnectionNoPassword { get; set; } = "localhost:27017";

		public string DbName { get; set; } = "";
		public bool QuickImport { get; set; } = false;

		[JsonIgnore]
		public string FileName => Path.GetFileName(FullFileName);
		[JsonIgnore]
		public string Folder => Path.GetDirectoryName(FullFileName);

		public static bool operator ==(SearchServiceParameters left, SearchServiceParameters right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(SearchServiceParameters left, SearchServiceParameters right)
		{
			return !Equals(left, right);
		}

		public static SearchServiceParameters Copy(ISearchServiceParameters args)
		{
			return new SearchServiceParameters
			{
				FullFileName = args.FullFileName,
				DbConnectionNoPassword = args.DbConnectionNoPassword,
				DbName = args.DbName,
				QuickImport = args.QuickImport,
			};
		}
		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = 0;
				hashCode = (hashCode * 397) ^ (FullFileName != null ? FullFileName.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (DbConnectionNoPassword != null ? DbConnectionNoPassword.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (DbName != null ? DbName.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ QuickImport.GetHashCode();
				return hashCode;
			}
		}
		public override bool Equals(object obj)
		{
			var other = obj as ISearchServiceParameters;
			if (other != null)
			{
				return FullFileName == other.FullFileName && DbConnectionNoPassword == other.DbConnectionNoPassword && DbName == other.DbName && QuickImport == other.QuickImport;
			}
			return base.Equals(obj);
		}
	}
}
