using System;

namespace BCMToBilingualConverter
{
	internal class Program
	{
		static void Main(string[] args)
		{
			#region | Debug |

			//const string action_debug = @"BilingualToBCM"; 
			//const string inputFile_debug = @"";
			//const string outputFile_debug = inputFile_debug + ".json";

			//const string action_debug = @"BCMToBilingual";
			//const string inputFile_debug = @"" + ".json";
			//const string outputFile_debug = inputFile_debug+".sdlxliff";

			//args = new string[] { action_debug, inputFile_debug , outputFile_debug };

			#endregion

			if (args.Length == 3)
			{
				var action = args[0];
				var inputFile = args[1];
				var outputFile = args[2];

				if (string.Compare(action, "BCMToBilingual", StringComparison.InvariantCultureIgnoreCase) != 0 &&
				    string.Compare(action, "BilingualToBCM", StringComparison.InvariantCultureIgnoreCase) != 0)
				{
					Console.WriteLine("Invalid argument [action]\n"
					                  + "Error: the action should be BCMToBilingual or BilingualToBCM");
					WriteHelpMessage();
				}
				else if (string.IsNullOrEmpty(inputFile?.Trim()) || !System.IO.File.Exists(inputFile))
				{
					Console.WriteLine("Invalid argument [inputFilePath]\n"
									  + "Error: the input file is not found in the path specified!");
					WriteHelpMessage();
				}
				else if (string.IsNullOrEmpty(outputFile?.Trim()))
				{
					Console.WriteLine("Invalid argument [outputFilePath]\n"
									  + "Error: the output file path cannot be null or empty!");
					WriteHelpMessage();
				}
				else
				{
					try
					{
						var converter = new Converter();

						if (string.Compare(action, "BCMToBilingual", StringComparison.InvariantCultureIgnoreCase) == 0)
						{
							converter.BCMToBilingual(inputFile, outputFile);
						}
						if (string.Compare(action, "BilingualToBCM", StringComparison.InvariantCultureIgnoreCase) == 0)
						{
							converter.BilingualToBCM(inputFile, outputFile);
						}
						Console.WriteLine("Operation complete");
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
				}
			}
			else
			{
				Console.WriteLine("Incorrect number of arguments!");
				WriteHelpMessage();
			}

			Console.ReadLine();
		}

		private static void WriteHelpMessage()
		{
			Console.WriteLine("\nExpected: BCMToBilingualConverter.exe [action] [inputFilePath] [outputFilePath]");
		}
	}
}
