using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MtComparisonTool.Models;
using RestSharp;

namespace MtComparisonTool.Service
{
	public class AuthenticationService
	{
		private readonly ApiUrls _apiUrls;

		public AuthenticationService()
		{
			_apiUrls = new ApiUrls();
		}

		public async Task<IRestResponse> Login(string email,string password)
		{
			var client = new RestClient(_apiUrls.LoginUrl());
			var request = new RestRequest("", Method.POST);
			request.AddHeader("Content-type", "application/json");
			request.AddJsonBody(
				new
				{
					email = email,
					password = password
				});

			var response = await client.ExecuteTaskAsync(request);
			return response;
		}
	}
}
