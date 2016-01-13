using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace Sdl.Community.TMRepair
{
    public partial class SDLTMRepair : Form
    {
        public SDLTMRepair()
        {
            InitializeComponent();
        }

        private void SDLTMRepair_Load(object sender, EventArgs e)
        {
            var waitForm = new WaitForm
            {
                Width = this.Width - 20,
                Height = this.Height - 42,
                Location = new Point(this.Location.X + 10, this.Location.Y + 32),
                Opacity = 0.7
            };
            waitForm.Show();

        }

        private void btnSelectTM_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                txtFile.Text = openFileDialog.FileName;
            }
        }

        private void btnIntegrityCheck_Click(object sender, EventArgs e)
        {
            if (txtFile.Text == string.Empty)
                return;

            txtFile.Enabled = btnIntegrityCheck.Enabled = btnRepair.Enabled = btnSelectTM.Enabled = false;

            txtLog.Text = "Integrity check started ...";
            var path = Environment.GetFolderPath(Environment.SpecialFolder.System);
            var log = Guid.NewGuid();
            var logFolder = Path.GetDirectoryName(txtFile.Text);
            var logFilePath = Path.Combine(logFolder, $"log_{log}.txt");
            var p = Process.Start(Path.Combine(path, "cmd.exe"),
                $@"/c echo .dump | sqlite3.exe {txtFile.Text} ""pragma integrity_check;"" >{logFilePath}");
            if (p != null)
            {
                while (!p.HasExited)
                {
                    Thread.Sleep(100);
                }

                var output = string.Empty;

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
            txtFile.Enabled = btnIntegrityCheck.Enabled = btnRepair.Enabled = btnSelectTM.Enabled = true;
        }

        private void btnRepair_Click(object sender, EventArgs e)
        {
            if (txtFile.Text == string.Empty)
                return;

            txtFile.Enabled = btnIntegrityCheck.Enabled = btnRepair.Enabled = btnSelectTM.Enabled = false;
            txtLog.Text = @"Repair started ...";

            var path = Environment.GetFolderPath(Environment.SpecialFolder.System);
            var log = Guid.NewGuid();
            var sql = Guid.NewGuid();

            var logFolder = Path.GetDirectoryName(txtFile.Text);
            if (logFolder != null)
            {
                var logFilePath = Path.Combine(logFolder, $"log_{log}.txt");
                var sqlFilePath = Path.Combine(logFolder, $"{sql}.sql");

                var p = Process.Start(Path.Combine(path, "cmd.exe"),
                    $@"/c echo .dump | sqlite3.exe {txtFile.Text} >{sqlFilePath}");
                if (p != null)
                {
                    while (!p.HasExited)
                    {
                        Thread.Sleep(100);
                    }

                    var newFileName = Path.Combine(Path.GetDirectoryName(txtFile.Text),
                        $"repaired_{Path.GetFileName(txtFile.Text)}");
                    var p1 = Process.Start(Path.Combine(path, "cmd.exe"),
                        $@"/c sqlite3.exe -init {sqlFilePath} {newFileName} "".quit"" >{logFilePath}");
                    if (p1 != null)
                    {
                        while (!p1.HasExited)
                        {
                            Thread.Sleep(100);
                        }

                    }
                    var output = string.Empty;

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
    }
}
