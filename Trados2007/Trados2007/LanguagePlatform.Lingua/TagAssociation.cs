using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sdl.LanguagePlatform.Lingua
{
	public class TagAssociation
	{
		public TagAssociation(PairedTag sourceTag, PairedTag targetTag)
			: this(sourceTag, targetTag, Core.EditDistance.EditOperation.Undefined)
		{
		}

		public TagAssociation(PairedTag sourceTag, PairedTag targetTag,
			Core.EditDistance.EditOperation op)
		{
			SourceTag = sourceTag;
			TargetTag = targetTag;
			Operation = op;
		}

		public PairedTag SourceTag;
		public PairedTag TargetTag;
		public Core.EditDistance.EditOperation Operation;

		/// <summary>
		/// <see cref="object.ToString()"/>
		/// </summary>
		/// <returns>A string representation of the object, for display purposes.</returns>
		public override string ToString()
		{
			return String.Format("{0} <-> {1}, {2}",
				SourceTag == null ? "(null)" : SourceTag.ToString(),
				TargetTag == null ? "(null)" : TargetTag.ToString(),
				Operation);
		}

		/// <summary>
		/// <see cref="M:System.Object.Equals(object)"/>
		/// </summary>
		/// <param name="obj">The object to compare with the current object.</param>
		/// <returns>true if the specified object is equal to the current object;
		/// otherwise, false.
		/// </returns>
		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			if (obj.GetType() != this.GetType())
				return false;

			TagAssociation other = obj as TagAssociation;

			bool ok = this.Operation == other.Operation;

			if (ok)
				ok = this.SourceTag == null ? other.SourceTag == null : this.SourceTag.Equals(other.SourceTag);

			if (ok)
				ok = this.TargetTag == null ? other.TargetTag == null : this.TargetTag.Equals(other.TargetTag);

			return ok;
		}

		/// <summary>
		/// <see cref="M:System.Object.GetHashCode(object)"/>
		/// </summary>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

	}

}
