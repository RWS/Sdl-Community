using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Sdl.LanguagePlatform.Core.Resources;
using System.Reflection;
using System.Windows.Forms;

namespace Sdl.LanguagePlatform.Lingua.Resources
{
	/// <summary>
	/// Implements a resource accessor which retrieves resources from the default (fallback)
	/// resources file (Sdl.LanguagePlatform.NLP.resources)
	/// </summary>
	public class ResourceFileResourceAccessor : ResourceStorage
	{
		private static Dictionary<string, ResourceFileData> _Cache;
		private readonly static object _cacheLock = new object();

		static ResourceFileResourceAccessor()
		{
			_Cache = new Dictionary<string, ResourceFileData>();
		}

		private string _FileName;
		private ResourceFileData _ResourceData;

		// TODO hit counter to free cache if there are many requests to different files (unlikely)

		private class ResourceFileData
		{
			public List<string> ResourceNames;
			public Dictionary<string, byte[]> Data;
		}

		/// <summary>
		/// The file name of the default resource file
		/// </summary>
		public static readonly string DefaultResourceFileName = "Sdl.LanguagePlatform.NLP.resources";

		/// <summary>
		/// Initializes a new instance with the specified values.
		/// </summary>
		/// <param name="resourceFileName">The path to the resource file to use</param>
		public ResourceFileResourceAccessor(string resourceFileName)
			: base(null)
		{
			// NOTE we need to support a null filename here since the class is also
			//  used to obtain the resource names during resource file generation
			//  (during which the output file doesn't yet exist)
			_FileName = resourceFileName;
			_ResourceData = null;
		}

		/// <summary>
		/// Initializes a new instance, using default values. The path to the resource file
		/// is determined by a search heuristics which searches several standard locations in which 
		/// the resource file may be located. The usual location is the where the current assembly
		/// is located. An exception is thrown if the resource file isn't found.
		/// </summary>
		public ResourceFileResourceAccessor()
			: base(null)
		{
			try
			{
				string loc = GetResourceFile();
				// TODO check whether file has changed? (timestamp, size)
				_FileName = loc;
				_ResourceData = null;
			}
			catch (FileNotFoundException)
			{
				throw new Core.LanguagePlatformException(Core.ErrorCode.LanguageResourceFileNotFound, Core.FaultStatus.Fatal);
			}
		}

		/// <summary>
		/// Attempts to locate the resource file in a variety of locations. The first one is returned.
		/// This method can be used to test whether the resource file can be located before instantiating 
		/// the accessor.
		/// </summary>
		/// <returns>A resource file location, if it exists, and null otherwise.</returns>
		public static string GetResourceFile()
		{
			// TODO probe other potential locations, i.e. installation folder, global app data, user app data, 
			//  local app data, etc.
			string resourceLocation = string.Empty;

			// FIRST try the location where the assembly is, unless it's GAC'ed.
			System.Reflection.Assembly ass = typeof(ResourceFileResourceAccessor).Assembly;
			if (!ass.GlobalAssemblyCache)
			{
				// First try to load it from the CodeBase value. This is necessary because ASP.Net loads
				//  from the temp assembly cache, so the location property is set to that.
				if ((ass.Location != ass.CodeBase) && (!string.IsNullOrEmpty(ass.CodeBase)))
				{
					Uri codeBaseUri = new Uri(ass.CodeBase);
					string assemblyPath = codeBaseUri.LocalPath;

					FileInfo f = new FileInfo(assemblyPath);
					resourceLocation = GetResourceLocation(f.DirectoryName);
				}

				// If the codebase isn't set, or the file hasn't been found there, 
				//  try the assembly's location property.
				if (resourceLocation == String.Empty)
				{
					resourceLocation = GetResourceLocation(ass.Location);
				}
			}

            if (String.IsNullOrEmpty(resourceLocation))
            {
                resourceLocation = GetResourceLocation(Path.GetDirectoryName(Application.ExecutablePath));
            }

			// If it's still not found, try some special folders
			if (resourceLocation == String.Empty)
			{
				Environment.SpecialFolder[] specialFolders = new Environment.SpecialFolder[]
		                                                         {
		                                                             System.Environment.SpecialFolder.CommonApplicationData,
		                                                             System.Environment.SpecialFolder.ApplicationData,
		                                                             System.Environment.SpecialFolder.LocalApplicationData
		                                                         };

				foreach (System.Environment.SpecialFolder sf in specialFolders)
				{
					string loadPath = Path.Combine(Environment.GetFolderPath(sf),
												   String.Format("SDL International{0}Lingua{0}", Path.DirectorySeparatorChar));
					resourceLocation = GetResourceLocation(loadPath);

					// If we've found a matching file, break out of the loop.
					if (resourceLocation != String.Empty)
						break;
				}
			}

			if (String.IsNullOrEmpty(resourceLocation))
			{
				throw new FileNotFoundException("Could not find language resource file", DefaultResourceFileName);
			}

			return resourceLocation;
		}

		/// <summary>
		/// Checks the specified loadpath for the resource file, returns the full filepath if it's there or an empty string if it's not.
		/// </summary>
		/// <param name="loadPath"></param>
		/// <returns></returns>
		private static string GetResourceLocation(string loadPath)
		{
			string fullPath = Path.Combine(loadPath, DefaultResourceFileName);

			//canonicalize the loadpath before using it - ensures that the path is valid for the FileInfo object.
			string canonicalizedPath = Path.GetFullPath(fullPath);
			FileInfo fi = new FileInfo(canonicalizedPath);

			string resourceLocation = String.Empty;

			if (fi.Exists)
			{
				resourceLocation = fi.FullName;
			}

			return resourceLocation;
		}

		/// <summary>
		/// See <see cref="ResourceStorage.GetAllResourceKeys"/>
		/// </summary>
		/// <returns>The list of resource keys known to the storage.</returns>
		public override List<string> GetAllResourceKeys()
		{
			lock (_cacheLock)
			{
				if (_ResourceData == null)
				{
					if (!_Cache.TryGetValue(_FileName, out _ResourceData))
					{
						if (!System.IO.File.Exists(_FileName))
							throw new Core.LanguagePlatformException(Core.ErrorCode.LanguageResourceFileNotFound, Core.FaultStatus.Fatal);

						_ResourceData = new ResourceFileData();
						_ResourceData.ResourceNames = new List<string>();
						_ResourceData.Data = new Dictionary<string, byte[]>();

						System.Resources.ResourceReader rdr = null;
						try
						{
							rdr = new System.Resources.ResourceReader(_FileName);
							foreach (System.Collections.DictionaryEntry entry in rdr)
							{
								_ResourceData.ResourceNames.Add((string)entry.Key);
								// this will effectively load all resources. Since the enumerator (which we
								//  need to determine the available resources) also returns the data itself, 
								//  and since ResourceReader.GetResourceData() prefixes the data with an 
								//  int which contains the size of the byte array, we rather store right now:
								_ResourceData.Data.Add((string)entry.Key, (byte[])entry.Value);
							}
							_Cache.Add(_FileName, _ResourceData);
						}
						catch
						{
							throw new Core.LanguagePlatformException(Core.ErrorCode.InvalidLanguageResourceFile, Core.FaultStatus.Fatal);
						}
						finally
						{
							if (rdr != null)
								rdr.Close();
						}
					}
				}
			}
			return _ResourceData.ResourceNames;
		}

		/// <summary>
		/// See <see cref="ResourceStorage.ReadResourceData"/>
		/// </summary>
		public override System.IO.Stream ReadResourceData(System.Globalization.CultureInfo culture, LanguageResourceType t, bool fallback)
		{
			byte[] data = GetResourceData(culture, t, fallback);
			if (data == null)
				return null;
			else
				return new System.IO.MemoryStream(data, false);
		}

		/// <summary>
		/// See <see cref="ResourceStorage.GetResourceData"/>
		/// </summary>
		public override byte[] GetResourceData(System.Globalization.CultureInfo culture, LanguageResourceType t, bool fallback)
		{
			lock (_cacheLock)
			{
				if (_ResourceData == null)
				{
					// fill caches
					GetAllResourceKeys();
					if (_ResourceData == null)
						return null;
				}
			}

			string name = GetResourceName(culture, t, fallback);
			byte[] resourceData;
			if (name != null && _ResourceData.Data.TryGetValue(name, out resourceData))
				return resourceData;
			return null;
		}
	}
}
