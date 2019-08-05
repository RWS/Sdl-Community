using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using IATETerminologyProvider.Helpers;
using IATETerminologyProvider.Model;
using IATETerminologyProvider.Model.ResponseModels;
using Newtonsoft.Json;

namespace IATETerminologyProvider.Service
{
	public static class TermTypeService
	{
		public static readonly Log Log = Log.Instance;

		/// <summary>
		/// Get term types from IATE database.
		/// </summary>
		/// <returns>termTypes</returns>
		public static ObservableCollection<TermTypeModel> GetTermTypes()
		{
			var termTypes = new ObservableCollection<TermTypeModel>();
			// the parameters set below to get term types are the same used in IATE environment.
			//var client = new RestClient(ApiUrls.GetTermTypeUri("true", "en", "100", "0"));
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
				var httpResponseAsString = httpClient.SendAsync(httpRequest).Result.Content.ReadAsStringAsync().Result;
				var jsonTermTypesModel = JsonConvert.DeserializeObject<TermTypeResponseModel>(httpResponseAsString);
				// the below Logger.Info should be removed, because it is used only to identify user's issue (otherwise will impact the performance of the app).
				Log.Logger.Info($"GetTermTypes method: The response when receiving term types: {httpResponseAsString}");

				if (jsonTermTypesModel?.Items != null)
				{
					int result;
					foreach (var item in jsonTermTypesModel.Items)
					{
						var selectedTermTypeName = Utils.UppercaseFirstLetter(item.Name.ToLower());

						var termType = new TermTypeModel
						{
							Code = int.TryParse(item.Code, out result) ? int.Parse(item.Code) : 0,
							Name = selectedTermTypeName
						};
						termTypes.Add(termType);
					}
				}
				return termTypes;
			}
			catch (Exception e)
			{
				Log.Logger.Error($"{e.Message}\n{e.StackTrace}");
			}
			return null;
		}
	}
}