using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sdl.Community.IATETerminologyProvider.Helpers;
using Sdl.Community.IATETerminologyProvider.Model.ResponseModels;

namespace Sdl.Community.IATETerminologyProvider.Service
{
	public class TermTypeService
	{
		public static readonly Log Log = Log.Instance;
		public static ObservableCollection<ItemsResponseModel> IateTermType { get; set; }

		public async Task<ObservableCollection<ItemsResponseModel>> GetTermTypes()
		{
			var itateTermTypes = new ObservableCollection<ItemsResponseModel>();
			var httpClient = new HttpClient
			{
				BaseAddress = new Uri(ApiUrls.GetTermTypeUri("true", "en", "100", "0")),
				Timeout = TimeSpan.FromMinutes(2)
			};

			Utils.AddDefaultParameters(httpClient);
			var httpRequest = new HttpRequestMessage
			{
				Method = HttpMethod.Get
			};

			try
			{
				var httpResponse = await httpClient.SendAsync(httpRequest);
				if (httpResponse.StatusCode == HttpStatusCode.OK)
				{
					var httpResponseAsString = await httpResponse.Content.ReadAsStringAsync();
					var jsonTermTypesModel = JsonConvert.DeserializeObject<TermTypeResponseModel>(httpResponseAsString);

					if (jsonTermTypesModel?.Items != null)
					{
						IateTermType = new ObservableCollection<ItemsResponseModel>(jsonTermTypesModel.Items);

						return IateTermType;
					}
				}
				else
				{
					Log.Logger.Error($"Get Term Type status code:{httpResponse.StatusCode}");
				}
			}
			catch (Exception e)
			{
				Log.Logger.Error($"{e.Message}\n{e.StackTrace}");
			}
			return itateTermTypes;
		}
	}
}