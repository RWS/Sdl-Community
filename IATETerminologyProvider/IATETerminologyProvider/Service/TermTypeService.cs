using System.Collections.ObjectModel;
using IATETerminologyProvider.Helpers;
using IATETerminologyProvider.Model;
using IATETerminologyProvider.Model.ResponseModels;
using Newtonsoft.Json;
using RestSharp;

namespace IATETerminologyProvider.Service
{
	public static class TermTypeService
    {
		#region Public Methods
		/// <summary>
		/// Get term types from IATE database.
		/// </summary>
		/// <returns>termTypes</returns>
		public static ObservableCollection<TermTypeModel> GetTermTypes()
		{
			var termTypes = new ObservableCollection<TermTypeModel>();
			// the parameters set below to get term types are the same used in IATE environment.
			var client = new RestClient(ApiUrls.GetTermTypeUri("true", "en", "100", "0"));
			var request = new RestRequest("", Method.GET);
			request.AddHeader("Connection", "Keep-Alive");
			request.AddHeader("Cache-Control", "no-cache");
			request.AddHeader("Pragma", "no-cache");
			request.AddHeader("Accept", "application/json");
			request.AddHeader("Accept-Encoding", "gzip, deflate, br");
			request.AddHeader("Content-Type", "application/json");
			request.AddHeader("Origin", "https://iate.europa.eu");
			request.AddHeader("Host", "iate.europa.eu");
			request.AddHeader("Access-Control-Allow-Origin", "*");

			var response = client.Execute(request);
			var jsonTermTypesModel = JsonConvert.DeserializeObject<TermTypeResponseModel>(response.Content);

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
		#endregion
	}
}