using System;
using System.Collections.ObjectModel;
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
			var httpRequest = new HttpRequestMessage
			{
				Method = HttpMethod.Get,
				RequestUri = new Uri(ApiUrls.GetTermTypeUri("true", "en", "100", "0")),
			};

			var httpResponse = await IateApplicationInitializer.Clinet.SendAsync(httpRequest);
			try
			{
				httpResponse?.EnsureSuccessStatusCode();

				if (httpResponse?.Content != null)
				{
					var httpResponseAsString = await httpResponse.Content.ReadAsStringAsync();

					var jsonTermTypesModel = JsonConvert.DeserializeObject<TermTypeResponseModel>(httpResponseAsString);

					if (jsonTermTypesModel?.Items != null)
					{
						IateTermType = new ObservableCollection<ItemsResponseModel>(jsonTermTypesModel.Items);

						return IateTermType;
					}

				}
			}
			finally
			{
				httpResponse?.Dispose();
			}
			return new ObservableCollection<ItemsResponseModel>(); 
		}
	}
}