using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace Sdl.Community.TMRepair
{
    public partial class SDLTMRepair : Form
    {
	    public readonly string DeployPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"SDL Community\TMRepair");

		public SDLTMRepair()
        {
            InitializeComponent();
	        var sqlStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Sdl.Community.TMRepair._3rd_party.sqlite3.exe");
	        if (sqlStream != null)
	        {
		        if (!Directory.Exists(DeployPath))
		        {
			        Directory.CreateDirectory(DeployPath);
		        }
		        if (!File.Exists(Path.Combine(DeployPath, "sqlite3.exe")))
		        {
					var dllPath = Path.Combine(DeployPath, "sqlite3.exe");
			        var fileStream = File.Create(dllPath, (int)sqlStream.Length);
			        var assemblyData = new byte[(int)sqlStream.Length];
			        sqlStream.Read(assemblyData, 0, assemblyData.Length);
			        fileStream.Write(assemblyData, 0, assemblyData.Length);
			        fileStream.Close();
				}  
	        }
		}

        private void SDLTMRepair_Load(object sender, EventArgs e)
        {
            var waitForm = new WaitForm
            {
                Width = Width - 20,
                Height = Height - 42,
                Location = new Point(Location.X + 10, Location.Y + 32),
                Opacity = 0.7
            };

        }

        private void btnSelectTM_Click(object sender, EventArgs e)
        {
	        openFileDialog.Filter = @"Translation memories|*.sdltm";
			if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                txtFile.Text = openFileDialog.FileName;
            }
        }

        private void btnIntegrityCheck_Click(object sender, EventArgs e)
        {
            if (txtFile.Text == string.Empty)
                return;

	        var extension = Path.GetExtension(txtFile.Text);
	        if (extension != null && extension.Equals(".sdltm"))
	        {
		        txtFile.Enabled = btnIntegrityCheck.Enabled = btnRepair.Enabled = btnSelectTM.Enabled = false;

		        txtLog.Text = @"Integrity check started ...";
		        var log = Guid.NewGuid();
		        var logFolder = Path.GetDirectoryName(txtFile.Text);
		        if (logFolder != null)
		        {
			        var logFilePath = Path.Combine(logFolder, $"log_{log}.txt");

			        var processStartInfo = new ProcessStartInfo("cmd.exe")
			        {
				        WorkingDirectory = DeployPath,
				        Arguments =
					        $@"/c echo .dump | sqlite3.exe ""{txtFile.Text}"" ""pragma integrity_check;"" >""{logFilePath}""",
				        UseShellExecute = false
			        };


			        var p = Process.Start(processStartInfo);

			        if (p != null)
			        {
				        while (!p.HasExited)
				        {
					        Thread.Sleep(100);
				        }

				        string output;

				        using (var sr = new StreamReader(logFilePath))
				        {
					        output = sr.ReadToEnd();
				        }
				        File.Delete(logFilePath);
				        txtLog.Text += "\r\n";
				        txtLog.Text += "\r\n";
				        txtLog.Text += output;
				        txtLog.Text += "\r\n";
				        txtLog.Text += "\r\n";
				        txtLog.Text += @"Integrity check completed.";
			        }
		        }
		        txtFile.Enabled = btnIntegrityCheck.Enabled = btnRepair.Enabled = btnSelectTM.Enabled = true;
			}
	        else
	        {
				MessageBox.Show(@"Please select an .sdltm file", "", MessageBoxButtons.OK);
			}
	       
        }

        private void btnRepair_Click(object sender, EventArgs e)
        {
            if (txtFile.Text == string.Empty)
                return;

	        var extension = Path.GetExtension(txtFile.Text);
	        if (extension != null && extension.Equals(".sdltm"))
	        {
		        txtFile.Enabled = btnIntegrityCheck.Enabled = btnRepair.Enabled = btnSelectTM.Enabled = false;
		        txtLog.Text = @"Repair started ...";

		        var log = Guid.NewGuid();
		        var sql = Guid.NewGuid();

		        var logFolder = Path.GetDirectoryName(txtFile.Text);
		        if (logFolder != null)
		        {
			        var logFilePath = Path.Combine(logFolder, $"log_{log}.txt");
			        var sqlFilePath = Path.Combine(logFolder, $"{sql}.sql");


					var processStartInfo = new ProcessStartInfo("cmd.exe")
			        {
				        WorkingDirectory = DeployPath,

						Arguments = $@"/c echo .dump | sqlite3.exe ""{txtFile.Text}"" >""{sqlFilePath}""",
				        UseShellExecute = false
			        };


			        var p = Process.Start(processStartInfo);

			        if (p != null)
			        {
				        while (!p.HasExited)
				        {
					        Thread.Sleep(100);
				        }

				        var newFileName = Path.Combine(Path.GetDirectoryName(txtFile.Text), $"repaired_{Path.GetFileName(txtFile.Text)}");

				        var processStartInfo1 = new ProcessStartInfo("cmd.exe")
				        {
					        Arguments = $@"/c sqlite3.exe -init ""{sqlFilePath}"" ""{newFileName}"" "".quit"" >""{logFilePath}""",
					        WorkingDirectory = DeployPath,
							UseShellExecute = false
				        };

				        var p1 = Process.Start(processStartInfo1);

				        if (p1 != null)
				        {
					        while (!p1.HasExited)
					        {
						        Thread.Sleep(100);
					        }

				        }
				        string output;

				        using (var sr = new StreamReader(logFilePath))
				        {
					        output = sr.ReadToEnd();
				        }
				        File.Delete(logFilePath);
				        File.Delete(sqlFilePath);
				        txtLog.Text += "\r\n";
				        txtLog.Text += "\r\n";
				        txtLog.Text += output != string.Empty ? output : "Repair completed.";
			        }
		        }

		        txtFile.Enabled = btnIntegrityCheck.Enabled = btnRepair.Enabled = btnSelectTM.Enabled = true;
			}
	        else
	        {
		        MessageBox.Show(@"Please select an .sdltm file", "", MessageBoxButtons.OK);
	        }
			
        }
    }
}
