using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using Sdl.Community.XLIFF.Manager.Commands;
using Sdl.Community.XLIFF.Manager.Model;

namespace Sdl.Community.XLIFF.Manager.Wizard.ViewModel
{
	public class ExceptionViewerViewModel : INotifyPropertyChanged
	{
		private List<Exception> _errors;
		private List<Exception> _warnings;
		private List<TreeViewItem> _treeViewItems;
		private Span _subtitleSpan;

		public ExceptionViewerViewModel(List<Exception> errors, List<Exception> warnings)
		{
			_errors = errors;
			_warnings = warnings;

			SelectedItemChangedCommand = new SelectedItemChangedCommand(SelectedItemChanged, CanSelectItem);

			BuildExceptionTree();
		}

		public Span SubtitleSpan
		{
			get => _subtitleSpan;
			set
			{
				_subtitleSpan = value;
				OnPropertyChanged(nameof(SubtitleSpan));
			}
		}

		public ICommand SelectedItemChangedCommand { get; }

		public void SelectedItemChanged(TreeViewItem item)
		{
			if (item == null)
			{
				return;
			}

			var span = new Span();

			if (item.Tag is List<Exception> exceptions)
			{
				foreach (var exception in exceptions)
				{
					WriteExceptionSpan(exception, span);
				}
			}
			else if (item.Tag is Exception exception)
			{
				WriteExceptionSpan(exception, span);
			}
			else if (item.Tag is string text)
			{
				WriteTextSpan(text, span);
			}

			SubtitleSpan = span;
		}

		public bool CanSelectItem(TreeViewItem item)
		{
			return item.IsEnabled;
		}

		public List<Exception> Errors
		{
			get { return _errors ?? (_errors = new List<Exception>()); }
			set
			{
				_errors = value;
				OnPropertyChanged(nameof(Errors));
			}
		}

		public List<Exception> Warnings
		{
			get { return _warnings ?? (_warnings = new List<Exception>()); }
			set
			{
				_warnings = value;
				OnPropertyChanged(nameof(Warnings));
			}
		}

		public List<TreeViewItem> TreeViewItems
		{
			get { return _treeViewItems ?? (_treeViewItems = new List<TreeViewItem>()); }
			set
			{
				_treeViewItems = value;
				OnPropertyChanged(nameof(TreeViewItems));
			}
		}

		private void BuildExceptionTree()
		{
			_treeViewItems = new List<TreeViewItem>
			{
				new TreeViewItem
				{
					Header = "All Messages",
					Tag = _errors,
					IsSelected = true
				}
			};

			foreach (var exception in _errors)
			{
				var treeItem = WriteException(exception);
				_treeViewItems.Add(treeItem);
			}

			TreeViewItems = _treeViewItems;
		}

		private static TreeViewItem WriteException(Exception exception, ItemsControl innerTreeViewItem = null)
		{
			var treeViewItem = new TreeViewItem
			{
				Header = exception.GetType(),
				Tag = exception
			};

			treeViewItem.Items.Add(new TreeViewItem
			{
				Header = "Stack Trace",
				Tag = exception.StackTrace
			});

			treeViewItem.Items.Add(new TreeViewItem
			{
				Header = "Data",
				Tag = RenderDictionary(exception.Data)
			});

			innerTreeViewItem?.Items.Add(treeViewItem);

			if (exception.InnerException != null)
			{
				var innerException = new TreeViewItem
				{
					Header = "InnerException",
					Tag = exception.InnerException
				};

				if (innerTreeViewItem != null)
				{
					innerTreeViewItem.Items.Add(innerException);
					WriteException(exception.InnerException, innerException);
				}
				else
				{
					treeViewItem.Items.Add(innerException);
					WriteException(exception.InnerException, innerException);
				}
			}

			return treeViewItem;
		}

		private static string RenderDictionary(IDictionary data)
		{
			if (data == null)
			{
				return string.Empty;
			}

			var result = new StringBuilder();

			foreach (var key in data.Keys)
			{
				if (key != null && data[key] != null)
				{
					result.AppendLine(key + " = " + data[key]);
				}
			}

			if (result.Length > 0)
			{
				result.Length = result.Length - 1;
			}

			return result.ToString();
		}

		private static void WriteTextSpan(string text, Span span)
		{
			var run = new Run
			{
				Text = text,
				FontWeight = FontWeights.Normal
			};

			span.Inlines.Add(run);
			span.Inlines.Add(new LineBreak());
		}

		private static void WriteExceptionSpan(Exception exception, Span span)
		{
			var run = new Run
			{
				Text = exception.Message,
				FontWeight = FontWeights.Bold
			};
			span.Inlines.Add(run);
			span.Inlines.Add(new LineBreak());

			if (exception is WizardException helixException)
			{
				var blockDetails = new Run
				{
					Text = helixException.Description,
					FontWeight = FontWeights.Normal
				};

				span.Inlines.Add(blockDetails);
				span.Inlines.Add(new LineBreak());
			}

			span.Inlines.Add(new LineBreak());
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
