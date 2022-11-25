namespace TMX_TranslationProvider
{
	public partial class TmxOptionsForm
	{
		private class EditOptions
		{
			protected bool Equals(EditOptions other)
			{
				return FileName == other.FileName && Connection == other.Connection && Password == other.Password && DatabaseName == other.DatabaseName;
			}

			public override bool Equals(object obj)
			{
				if (ReferenceEquals(null, obj)) return false;
				if (ReferenceEquals(this, obj)) return true;
				if (obj.GetType() != this.GetType()) return false;
				return Equals((EditOptions)obj);
			}

			public override int GetHashCode()
			{
				unchecked
				{
					var hashCode = (FileName != null ? FileName.GetHashCode() : 0);
					hashCode = (hashCode * 397) ^ (Connection != null ? Connection.GetHashCode() : 0);
					hashCode = (hashCode * 397) ^ (Password != null ? Password.GetHashCode() : 0);
					hashCode = (hashCode * 397) ^ (DatabaseName != null ? DatabaseName.GetHashCode() : 0);
					return hashCode;
				}
			}

			public string FileName = "";
			public string Connection = "";
			public string Password = "";
			public string DatabaseName = "";

			public static EditOptions FromTranslationOptions(TmxTranslationsOptions options)
			{
				return new EditOptions
				{
					FileName = options.FileName,
					Connection = options.DbConnectionNoPassword,
					Password = options.Password,
					DatabaseName = options.DbName,
				};
			}

			public TmxTranslationsOptions ToOptions()
			{
				return new TmxTranslationsOptions
				{
					FileName = FileName,
					DbConnectionNoPassword = Connection,
					Password = Password,
					DbName = DatabaseName,
				};
			}
		}
	}
}