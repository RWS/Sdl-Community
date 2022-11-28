namespace TMX_TranslationProvider
{
	public partial class TmxOptionsForm
	{
		private class EditOptions
		{
			public static bool operator ==(EditOptions left, EditOptions right)
			{
				return Equals(left, right);
			}

			public static bool operator !=(EditOptions left, EditOptions right)
			{
				return !Equals(left, right);
			}

			protected bool Equals(EditOptions other)
			{
				return Guid == other.Guid && FileName == other.FileName && Connection == other.Connection && Password == other.Password && DatabaseName == other.DatabaseName;
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
					var hashCode = (Guid != null ? Guid.GetHashCode() : 0);
					hashCode = (hashCode * 397) ^ (FileName != null ? FileName.GetHashCode() : 0);
					hashCode = (hashCode * 397) ^ (Connection != null ? Connection.GetHashCode() : 0);
					hashCode = (hashCode * 397) ^ (Password != null ? Password.GetHashCode() : 0);
					hashCode = (hashCode * 397) ^ (DatabaseName != null ? DatabaseName.GetHashCode() : 0);
					return hashCode;
				}
			}

			public string Guid = "";
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
					Guid = options.Guid,
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
					Guid = Guid,
				};
			}
		}
	}
}