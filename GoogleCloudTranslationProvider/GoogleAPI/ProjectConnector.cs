using System.Collections.Generic;
using System.Linq;
using System;
using GoogleCloudTranslationProvider.Models;
using System.Windows.Media;
using System.Web.Caching;
using GoogleCloudTranslationProvider.Helpers;
using static Google.Protobuf.Reflection.SourceCodeInfo.Types;
using System.Text.RegularExpressions;

namespace GoogleCloudTranslationProvider.GoogleAPI
{
	public class ProjectConnector
	{
		private static readonly string DummyLocation = "gctp-sdl";

		public static List<string> GetLocations(GCTPTranslationOptions tempOptions)
		{
			string errorMessage;
			var output = new List<string>();
			try
			{
				var v3Connector = new V3Connector(tempOptions);
				v3Connector.TryToAuthenticateUser();
				errorMessage = string.Empty;
			}
			catch (Exception e)
			{
				errorMessage = e.Message;
			}

			if (string.IsNullOrEmpty(errorMessage))
			{
				return null;
			}

			if (!errorMessage.Contains("Unsupported location"))
			{
				ErrorHandler.HandleError(PluginResources.Validation_AuthenticationFailed, "Authentication failed");
				return null;
			}

			var matches = new Regex(@"(['])(?:(?=(\\?))\2.)*?\1").Matches(errorMessage);
			return matches.Cast<object>()
						  .Select(match => match.ToString().Replace("'", ""))
						  .Where(locationValue => !locationValue.Equals(DummyLocation) && !locationValue.Equals("parent"))
						  .Distinct()
						  .ToList();
		}

		public static List<RetrievedGlossary> GetGlossaries(GCTPTranslationOptions translationOptions)
		{
			var output = new List<RetrievedGlossary>();

			try
			{
				var v3Connector = new V3Connector(translationOptions);
				v3Connector.TryToAuthenticateUser();

				output.Add(new(new()));
				output.AddRange(v3Connector.GetGlossaries(translationOptions.ProjectLocation).Select(retrievedGlossary => new RetrievedGlossary(retrievedGlossary)));
			}
			catch
			{
				output.Clear();
				output.Add(new(null));
			}

			return output;
		}

		public static List<RetrievedCustomModel> GetProjectCustomModels(GCTPTranslationOptions translationOptions)
		{
			var output = new List<RetrievedCustomModel>();

			try
			{
				var v3Connector = new V3Connector(translationOptions);
				v3Connector.TryToAuthenticateUser();

				output.Add(new(new()));
				output.AddRange(v3Connector.GetCustomModels().Select(retrievedCustomModel => new RetrievedCustomModel(retrievedCustomModel)));
			}
			catch
			{
				output.Clear();
				output.Add(new(null));
			}

			return output;
		}
	}
}