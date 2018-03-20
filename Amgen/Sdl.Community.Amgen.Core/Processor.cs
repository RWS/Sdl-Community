using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Sdl.Community.Amgen.Core.EventArgs;
using Sdl.Community.Amgen.Core.SDLXLIFF;
using Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;

namespace Sdl.Community.Amgen.Core
{
	public class Processor
    {
		private readonly IFileTypeManager _fileTypeManager;

		public event EventHandler<ProgressEventArgs> ProgressEvent;

		//public Processor() : this(DefaultFileTypeManager.CreateInstance(true))
		//{
		////	LoadAssemblies();
		//}

		public Processor()
		{
			DefaultFileTypeManager.CreateInstance();
			LoadAssemblies();
		}

		public Processor(IFileTypeManager fileTypeManager)
		{
			_fileTypeManager = fileTypeManager;
		}

		public List<SegmentInfo> ReadFile(string filePath, ProcessorOptions options)
		{
			var parser = new Parser(_fileTypeManager);

			try
			{
				parser.ProgressEvent += ParserProgressEvent;

				return parser.ReadFile(filePath, options);
			}
			finally
			{
				parser.ProgressEvent -= ParserProgressEvent;
			}
		}

		private void ParserProgressEvent(object sender, ProgressEventArgs e)
		{
			ProgressEvent?.Invoke(this, e);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void LoadAssemblies()
		{
			var _assemblies = new Dictionary<string, Assembly>();
			Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
			{
				var shortName = new AssemblyName(args.Name).Name;
				if (_assemblies.TryGetValue(shortName, out var assembly))
				{
					return assembly;
				}
				return null;
			}
			var appAssembly = typeof(Processor).Assembly;
			foreach (var resourceName in appAssembly.GetManifestResourceNames())
			{
				if (resourceName.EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase))
				{
					using (var stream = appAssembly.GetManifestResourceStream(resourceName))
					{
						var assemblyData = new byte[(int)stream.Length];
						stream.Read(assemblyData, 0, assemblyData.Length);
						var assembly = Assembly.Load(assemblyData);
						_assemblies.Add(assembly.GetName().Name, assembly);
					}
				}
			}
			AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
		}
	}
}