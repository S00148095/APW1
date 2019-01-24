using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace APW1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent(); backgroundWorker = (BackgroundWorker)FindResource("backgroundWorker");
        }
        private BackgroundWorker backgroundWorker;

        private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            mainProgressBar.Value = e.ProgressPercentage;
            outputBox.Text = (string)e.UserState;
        }
        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            int iterations = 0;
            if (int.TryParse(inputBox.Text, out iterations))
            {
                if (!backgroundWorker.IsBusy)
                {
                    backgroundWorker.RunWorkerAsync(iterations);
                    startButton.IsEnabled = false;
                    cancelButton.IsEnabled = true;
                    outputBox.Text = "";
                }
            }
        }
        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            backgroundWorker.CancelAsync();
        }

        private int DoSlowProcess(int iterations, BackgroundWorker worker, DoWorkEventArgs e)
        {
            //testing purposes
            //throw new System.Exception("Something bad happened");

            int result = 0;

            for (int i = 1; i <= iterations; i++)
            {
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }
                if (worker != null)
                {

                    if (worker.WorkerReportsProgress)
                    {
                        int percentComplete = (int)((float)i / (float)iterations * 100);
                        string updateMessage = string.Format("Iteration {0} of {1}", i, iterations);
                        worker.ReportProgress(percentComplete, updateMessage);
                    }
                }
                Thread.Sleep(100); result = i;
            }

            return result;
        }
        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var bgw = sender as BackgroundWorker;
            e.Result = DoSlowProcess((int)e.Argument, bgw, e);
        }
        private void BackgroundWorker_RunWorkerCompleted(object sender,
            RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                outputBox.Text = e.Error.Message;
                MessageBox.Show(e.Error.StackTrace);

            }
            else if (e.Cancelled)
            {
                outputBox.Text = "Cancelled";
            }
            else
            {
                outputBox.Text = e.Result.ToString();
                mainProgressBar.Value = 0;
            }
            startButton.IsEnabled = true; cancelButton.IsEnabled = false;
        }

    }
}
