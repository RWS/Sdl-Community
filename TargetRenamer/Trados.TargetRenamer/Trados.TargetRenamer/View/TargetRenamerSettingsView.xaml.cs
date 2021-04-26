using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Trados.TargetRenamer.Helpers;
using Trados.TargetRenamer.ViewModel;

namespace Trados.TargetRenamer.View
{
	/// <summary>
	/// Interaction logic for TargetRenamerSettingsView.xaml
	/// </summary>
	public partial class TargetRenamerSettingsView : UserControl
	{
		public TargetRenamerSettingsView()
		{
			InitializeComponent();
		}

		private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var comboBox = (ComboBox) sender;
			var vm = (TargetRenamerSettingsViewModel) comboBox.DataContext;

			if (vm == null) return;

			switch (e.AddedItems[0])
			{
				case ReplacementEnum.Suffix:
					vm.AppendAsSuffix = true;
					vm.AppendAsPrefix = false;
					vm.UseRegularExpression = false;
					TargetComboBox.SelectedItem = TargetLanguageEnum.TargetLanguage;
					break;
				case ReplacementEnum.Prefix:
					vm.AppendAsPrefix = true;
					vm.AppendAsSuffix = false;
					vm.UseRegularExpression = false;
					TargetComboBox.SelectedItem = TargetLanguageEnum.TargetLanguage;
					break;
				case ReplacementEnum.RegularExpression:
					vm.UseRegularExpression = true;
					vm.AppendAsSuffix = false;
					vm.AppendAsPrefix = false;
					vm.AppendTargetLanguage = false;
					vm.AppendCustomString = false;
					TargetComboBox.SelectedItem = null;
					break;
				default:
					throw new ArgumentOutOfRangeException($"{nameof(e)}. Selection does not exist as an option.");
			}
		}

		private void Combobox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var comboBox = (ComboBox) sender;
			var vm = (TargetRenamerSettingsViewModel) comboBox.DataContext;

			if (vm == null || e.AddedItems.Count < 1) return;

			switch (e.AddedItems[0])
			{
				case TargetLanguageEnum.TargetLanguage:
					vm.AppendTargetLanguage = true;
					vm.AppendCustomString = false;
					break;
				case TargetLanguageEnum.CustomString:
					vm.AppendCustomString = true;
					vm.AppendTargetLanguage = false;
					break;
				default:
					throw new ArgumentOutOfRangeException($"{nameof(e)}. Selection does not exist as an option.");
			}

		}
	}
}
