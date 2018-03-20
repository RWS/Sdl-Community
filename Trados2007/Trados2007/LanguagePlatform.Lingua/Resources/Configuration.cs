using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Sdl.LanguagePlatform.Lingua.Resources
{
	public class Configuration
	{
		public static readonly string ConfigurationSectionName = "sdl.languageplatform.resourceaccessors";

		public static CompositeResourceDataAccessor Load()
		{
			CompositeResourceDataAccessor result 
				= new CompositeResourceDataAccessor(false);

			ResourceDataAccessorConfigurationSection section = System.Configuration.ConfigurationManager.GetSection(ConfigurationSectionName)
				as ResourceDataAccessorConfigurationSection;

			if (section == null
				|| section.ResourceDataAccessors == null
				|| section.ResourceDataAccessors.Count == 0)
			{
				// create default/fallback collection
				result.AddDefaultAccessor();
				return result;
			}

			for (int e = 0; e < section.ResourceDataAccessors.Count; ++e)
			{
				try
				{
					ResourceDataAccessorConfigurationElement element
						= section.ResourceDataAccessors[e];

					System.Type rdaType = Type.GetType(element.Type);
					if (rdaType == null)
						throw new Core.LanguagePlatformException(Core.ErrorCode.ConfigurationCannotResolveType, Core.FaultStatus.Fatal, element.Type.ToString());

					bool found = false;
					foreach (System.Type t in rdaType.GetInterfaces())
					{
						if (t == typeof(Core.Resources.IResourceDataAccessor))
						{
							found = true;
							break;
						}
					}
					if (!found)
						throw new Core.LanguagePlatformException(Core.ErrorCode.ConfigurationInvalidType, Core.FaultStatus.Fatal, element.Type.ToString());

					if (rdaType.IsAbstract)
						throw new Core.LanguagePlatformException(Core.ErrorCode.ConfigurationAbstractType, Core.FaultStatus.Fatal, element.Type.ToString());

					object instance = rdaType.Assembly.CreateInstance(rdaType.FullName, false, System.Reflection.BindingFlags.CreateInstance,
						null, String.IsNullOrEmpty(element.Parameter) ? null : new object[] { element.Parameter },
						System.Globalization.CultureInfo.CurrentCulture, null);

					// We could check the constructors to test whether they match the parameter

					if (instance == null)
						throw new Core.LanguagePlatformException(Core.ErrorCode.ConfigurationCannotInstantiateOrCastType, Core.FaultStatus.Fatal, element.Type.ToString());

					Core.Resources.IResourceDataAccessor rda = instance as Core.Resources.IResourceDataAccessor;
					if (rda == null)
						throw new Core.LanguagePlatformException(Core.ErrorCode.ConfigurationCannotInstantiateOrCastType, Core.FaultStatus.Fatal, element.Type.ToString());

					result.Add(rda);
				}
				catch (System.Exception)
				{
					throw;
				}
			}

			if (result.Count == 0)
				result.AddDefaultAccessor();

			return result;
		}
	}

	internal class ResourceDataAccessorConfigurationSection : System.Configuration.ConfigurationSection
	{
		public ResourceDataAccessorConfigurationSection()
		{
		}

		[System.Configuration.ConfigurationProperty("accessors")]
		public ResourceDataAccessorConfigurationElementCollection ResourceDataAccessors
		{

			get
			{
				ResourceDataAccessorConfigurationElementCollection accessors =
					(ResourceDataAccessorConfigurationElementCollection)base["accessors"];
				return accessors;
			}
		}
	}

	internal class ResourceDataAccessorConfigurationElementCollection
		: System.Configuration.ConfigurationElementCollection
	{

		public ResourceDataAccessorConfigurationElementCollection()
		{
		}

		public override System.Configuration.ConfigurationElementCollectionType CollectionType
		{
			get { return System.Configuration.ConfigurationElementCollectionType.AddRemoveClearMap; }
		}

		protected override System.Configuration.ConfigurationElement CreateNewElement()
		{
			return new ResourceDataAccessorConfigurationElement();
		}

		protected override System.Configuration.ConfigurationElement CreateNewElement(string name)
		{
			return new ResourceDataAccessorConfigurationElement(name);
		}

		protected override object GetElementKey(System.Configuration.ConfigurationElement element)
		{
			return ((ResourceDataAccessorConfigurationElement)element).Name;
		}

		public new string AddElementName
		{
			get { return base.AddElementName; }
			set { base.AddElementName = value; }
		}

		public new string ClearElementName
		{
			get { return base.ClearElementName; }
			set { base.AddElementName = value; }
		}

		public new string RemoveElementName
		{
			get { return base.RemoveElementName; }
		}

		public new int Count
		{
			get { return base.Count; }
		}

		public ResourceDataAccessorConfigurationElement this[int index]
		{
			get { return (ResourceDataAccessorConfigurationElement)BaseGet(index); }
			set
			{
				if (BaseGet(index) != null)
				{
					BaseRemoveAt(index);
				}
				BaseAdd(index, value);
			}
		}

		new public ResourceDataAccessorConfigurationElement this[string Name]
		{
			get { return (ResourceDataAccessorConfigurationElement)BaseGet(Name); }
		}

		public int IndexOf(ResourceDataAccessorConfigurationElement element)
		{
			return BaseIndexOf(element);
		}

		public void Add(ResourceDataAccessorConfigurationElement element)
		{
			BaseAdd(element);
		}

		protected override void BaseAdd(System.Configuration.ConfigurationElement element)
		{
			BaseAdd(element, false);
		}

		public void Remove(ResourceDataAccessorConfigurationElement element)
		{
			if (BaseIndexOf(element) >= 0)
				BaseRemove(element.Name);
		}

		public void RemoveAt(int index)
		{
			BaseRemoveAt(index);
		}

		public void Remove(string name)
		{
			BaseRemove(name);
		}

		public void Clear()
		{
			BaseClear();
		}
	}


	internal class ResourceDataAccessorConfigurationElement : System.Configuration.ConfigurationElement
	{
		public ResourceDataAccessorConfigurationElement()
		{
		}

		public ResourceDataAccessorConfigurationElement(string name, string type, string parameter)
		{
			Name = name;
			Type = type;
			Parameter = parameter;
		}

		public ResourceDataAccessorConfigurationElement(string name)
		{
			Name = name;
		}

		[System.Configuration.ConfigurationProperty("name", IsRequired = true, IsKey = true)]
		public string Name
		{
			get { return (string)this["name"]; }
			set { this["name"] = value; }
		}

		[System.Configuration.ConfigurationProperty("type", IsRequired = true)]
		public string Type
		{
			get { return (string)this["type"]; }
			set { this["type"] = value; }
		}

		[System.Configuration.ConfigurationProperty("parameter", IsRequired = false)]
		public string Parameter
		{
			get { return (string)this["parameter"]; }
			set { this["parameter"] = value; }
		}
	}

}
