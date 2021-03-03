using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
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
using Microsoft.Win32;

namespace CSS_Converter_App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<string> files = new List<string>();

        public MainWindow()
        {
            InitializeComponent();
            FillComboBox();
        }

        private void FileSelect_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                this.FileName.Text = dialog.FileName;
            }
        }

        private void RunButton_Click(object sender, RoutedEventArgs e)
        {
            var index = this.ExecList.SelectedIndex;
            var program = files[index];
            var file = this.FileName.Text;

            var startInfo = new ProcessStartInfo();
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/C python " + file;

            var process = new Process();
            process.StartInfo = startInfo;
            process.OutputDataReceived += OnProgramOutputEvent;
            process.ErrorDataReceived += OnProgramErrorEvent;
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
        }

        private void FillComboBox()
        {
            var path = System.IO.Path.Join(Directory.GetCurrentDirectory(), "exec");
            foreach (var file in Directory.GetFiles(path, "*.exe"))
            {
                files.Add(file);
                this.ExecList.Items.Add(System.IO.Path.GetFileName(file));
            }
        }

        private void OnProgramOutputEvent(object sender, DataReceivedEventArgs e)
        {
            var output = e.Data + Environment.NewLine;
            Dispatcher.BeginInvoke((Action)(() =>
            {
                ProgramOutputs.Foreground = Brushes.Black;
                ProgramOutputs.AppendText(e.Data + Environment.NewLine);
            }));
        }

        private void OnProgramErrorEvent(object sender, DataReceivedEventArgs e)
        {
            var output = e.Data + Environment.NewLine;
            Dispatcher.BeginInvoke((Action)(() =>
            {
                ProgramOutputs.Foreground = Brushes.Red;
                ProgramOutputs.AppendText(output);
            }));
        }
    }
}
