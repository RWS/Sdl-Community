using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using IATETerminologyProvider.Helpers;
using IATETerminologyProvider.Model.ResponseModels;
using Newtonsoft.Json;

namespace IATETerminologyProvider.Service
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
				BaseAddress = new Uri(ApiUrls.GetTermTypeUri("true", "en", "100", "0"))
			};

			Utils.AddDefaultParameters(httpClient);
			var httpRequest = new HttpRequestMessage
			{
				Method = HttpMethod.Get
			};

			try
			{
				var httpResponse = await httpClient.SendAsync(httpRequest);
				var httpResponseAsString = await httpResponse.Content.ReadAsStringAsync();
				//TODO: Check the status code, if is not 200 log it 

				var jsonTermTypesModel = JsonConvert.DeserializeObject<TermTypeResponseModel>(httpResponseAsString);

				if (jsonTermTypesModel?.Items != null)
				{
					IateTermType = new ObservableCollection<ItemsResponseModel>(jsonTermTypesModel.Items);

					return IateTermType;
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