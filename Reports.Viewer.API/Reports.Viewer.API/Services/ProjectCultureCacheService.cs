using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Sdl.Reports.Viewer.API.Model;

namespace Sdl.Reports.Viewer.API.Services
{
	public class ProjectCultureCacheService
	{
		private readonly PathInfo _pathInfo;

		public ProjectCultureCacheService(PathInfo pathInfo)
		{
			_pathInfo = pathInfo;
		}

		public List<ProjectCulture> GetProjectCultureCache()
		{
			if (File.Exists(_pathInfo.ProjectCultureCachePath))
			{
				var json = File.ReadAllText(_pathInfo.ProjectCultureCachePath);
				return JsonConvert.DeserializeObject<List<ProjectCulture>>(json);
			}

			return new List<ProjectCulture>();
		}

		public void UpdateProjectCultureCache(IReadOnlyCollection<ProjectCulture> projectCultures)
		{
			File.WriteAllText(_pathInfo.ProjectCultureCachePath, JsonConvert.SerializeObject(projectCultures));
		}
	}
}
