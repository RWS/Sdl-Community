using System.Collections.Generic;
using System.Windows;
using MultiTermIX;

namespace MultiTermStandAloneTestApp
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			var app = new ApplicationClass();
			
			// Termbase Server connection
			//app.ServerRepository.Location = "";
			//app.ServerRepository.Connect("user", "password");
			//var termbases = app.ServerRepository.Termbases;
			//app.ServerRepository.Disconnect();

			// Use local termbase
			app.LocalRepository.Connect("", "");
			var localTermbases = app.LocalRepository.Termbases;

			//set the path to an existing termbase
			var path = "";
			localTermbases.Add(path, "", "");
			var termbase = localTermbases[path];			
			var numberOfEntriesForLanguage = GetNumberOfEntriesForEachLanguage(termbase);

			// Add local termbase
			//localTermbases.New("testT", "ss", "", @"C:\Users\{UserName}\Desktop\test.sdltb");
		}

		// Get number of entries for each language, using the Language Index (eg: "English")
		private Dictionary<string,int> GetNumberOfEntriesForEachLanguage(Termbase localTermbase)
		{
			var numberOfEntries = new Dictionary<string, int>();
			var termbaseInformation = localTermbase.Information;

			var languageIndexes = localTermbase.Definition.Indexes;
			foreach (Index languageIndex in languageIndexes)
			{
				var numberOfTermEntries = termbaseInformation.NumberOfEntriesInIndex[languageIndex.Language];
				numberOfEntries.Add(languageIndex.Language, numberOfTermEntries);
			}
			return numberOfEntries;
		}
	}
}