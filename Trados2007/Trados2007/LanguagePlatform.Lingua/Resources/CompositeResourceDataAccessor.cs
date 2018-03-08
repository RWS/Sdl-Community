using System;
using System.Collections.Generic;
using System.Text;
using Sdl.LanguagePlatform.Core.Resources;

namespace Sdl.LanguagePlatform.Lingua.Resources
{
	/// <summary>
	/// A simple container for individual accessors which implement a sequential lookup order for a specific resource.
	/// The individual accessors are tried in turn for a resource status and the actual resource data. No merging
	/// of resources is available. 
	/// <para>Note that conflicts may exist with the resolution strategy built into each individual accessor. For example, 
	/// one accessor may fall back to a more generic culture and thus resolve a resource request, while another
	/// accessor which is further down in the lookup chain may have a resource for the specialized culture.</para>
	/// </summary>
	public class CompositeResourceDataAccessor : IResourceDataAccessor
	{
		// TODO implement merge?

		private List<IResourceDataAccessor> _IndividualAccessors;

		/// <summary>
		/// Initializes a new instance. 
		/// </summary>
		/// <param name="addDefaultAccessor">If true, a default <see cref="ResourceFileResourceAccessor"/> is added.</param>
		public CompositeResourceDataAccessor(bool addDefaultAccessor)
		{
			_IndividualAccessors = new List<IResourceDataAccessor>();
			if (addDefaultAccessor)
				AddDefaultAccessor();
		}

		/// <summary>
		/// Inserts a new resource data accessor at the specified position in the list of 
		/// elements.
		/// </summary>
		/// <param name="index">The position at which to add the item.</param>
		/// <param name="racc">The accessor to add.</param>
		public void Insert(int index, IResourceDataAccessor racc)
		{
			_IndividualAccessors.Insert(index, racc);
		}

		/// <summary>
		/// Inserts a new resource data accessor at the beginning of the list.
		/// </summary>
		/// <param name="racc">The accessor to add.</param>
		public void Insert(IResourceDataAccessor racc)
		{
			_IndividualAccessors.Insert(0, racc);
		}

		/// <summary>
		/// Adds a default <see cref="ResourceFileResourceAccessor"/> at
		/// the end of the list giving it the least priority.
		/// </summary>
		public void AddDefaultAccessor()
		{
			_IndividualAccessors.Add(new ResourceFileResourceAccessor());
		}

		/// <summary>
		/// Adds the accessor to the list of individual accessors. The item will be added
		/// at the end of the accessor list, giving it lower priority than the preceding items.
		/// </summary>
		/// <param name="racc">The accessor to add.</param>
		public void Add(IResourceDataAccessor racc)
		{
			_IndividualAccessors.Add(racc);
		}

		/// <summary>
		/// Returns the number of individual accessors configured for this instance.
		/// </summary>
		public int Count
		{
			get { return _IndividualAccessors.Count; }
		}

		/// <summary>
		/// Gets the accessor at the specified index.
		/// </summary>
		public IResourceDataAccessor this[int index]
		{
			get { return _IndividualAccessors[index]; }
		}

		/// <summary>
		/// See <see cref="IResourceDataAccessor.GetResourceStatus"/>. The
		/// status returned is the status returned by the first accessor which differs from 
		/// <see cref="ResourceStatus.NotAvailable"/>.
		/// </summary>
		public ResourceStatus GetResourceStatus(System.Globalization.CultureInfo culture, 
			LanguageResourceType t, bool fallback)
		{
			for (int i = 0; i < _IndividualAccessors.Count; ++i)
			{
				ResourceStatus s = _IndividualAccessors[i].GetResourceStatus(culture, t, fallback);
				if (s != ResourceStatus.NotAvailable)
					return s;
			}
			return ResourceStatus.NotAvailable;
		}

		/// <summary>
		/// See <see cref="IResourceDataAccessor.ReadResourceData"/>. 
		/// The data returned is the data returned by the first accessor which has available data.
		/// </summary>
		public System.IO.Stream ReadResourceData(System.Globalization.CultureInfo culture, 
			LanguageResourceType t, bool fallback)
		{
			for (int i = 0; i < _IndividualAccessors.Count; ++i)
			{
				// TODO could cache this information
				ResourceStatus s = _IndividualAccessors[i].GetResourceStatus(culture, t, fallback);
				if (s != ResourceStatus.NotAvailable)
				{
					return _IndividualAccessors[i].ReadResourceData(culture, t, fallback);
				}
			}
			return null;
		}

		/// <summary>
		/// See <see cref="IResourceDataAccessor.GetResourceData"/>.
		/// The data returned is the data returned by the first accessor which has available data.
		/// </summary>
		public byte[] GetResourceData(System.Globalization.CultureInfo culture, LanguageResourceType t, bool fallback)
		{
			using (System.IO.Stream str = ReadResourceData(culture, t, fallback))
				return ResourceStorage.StreamToByteArray(str);
		}

		/// <summary>
		/// See <see cref="IResourceDataAccessor.GetSupportedCultures"/>. 
		/// The list of supported cultures is the combination of supported cultures of all individual
		/// accessors for the specified resource type.
		/// </summary>
		public List<System.Globalization.CultureInfo> GetSupportedCultures(LanguageResourceType t)
		{
			List<System.Globalization.CultureInfo> result = new List<System.Globalization.CultureInfo>();

			for (int i = 0; i < _IndividualAccessors.Count; ++i)
			{
				foreach (System.Globalization.CultureInfo ci in _IndividualAccessors[i].GetSupportedCultures(t))
				{
					if (!result.Contains(ci))
						result.Add(ci);
				}
			}

			return result.Count > 0 ? result : null;
		}

	}
}
