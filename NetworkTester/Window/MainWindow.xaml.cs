using System;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace NetworkTester
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void TbIPAddress_TextInput(object sender, TextCompositionEventArgs e)
        {

        }

        private void CbIsSSLPort_Checked(object sender, RoutedEventArgs e)
        {
            cbPort.Visibility = Visibility.Collapsed;
            cbSSLPort.Visibility = Visibility.Visible;
        }

        private void CbIsSSLPort_Unchecked(object sender, RoutedEventArgs e)
        {
            cbPort.Visibility = Visibility.Visible;
            cbSSLPort.Visibility = Visibility.Collapsed;
        }

        private void BtnPing_Click(object sender, RoutedEventArgs e)
        {
            string _pingCount;
            string _timeOutSeconds;

            if (tbIPAddress.Text.Length == 0)
            {
                tbPingError.Text = "IPアドレスが入力されていません。";
                return;
            }

            if (tbTimeoutSeconds.Text.Length != 0)
            {
                _timeOutSeconds = tbTimeoutSeconds.Text;
            }
            else
            {
                _timeOutSeconds = "1";
            }
            if (tbPingCount.Text.Length != 0)
            {
                _pingCount = tbPingCount.Text;
            }
            else
            {
                _pingCount = "5";
            }
            StartPing(_timeOutSeconds, _pingCount);
        }

        private void StartPing(string timeout, string countofping)
        {
            string Address = tbIPAddress.Text;
            tbConsole.AppendText("\nStarting Ping...");
            Thread thread = new Thread(() =>
            {
                try
                {
                    if (int.TryParse(timeout, out int timeOutSeconds))
                    {
                        if (int.TryParse(countofping, out int pingCount))
                        {
                            Ping ping = new Ping();

                            if (Address == "localhost")
                            {
                                Address = "127.0.0.1";
                            }
                            for (int i = 0; i < pingCount; i++)
                            {
                                PingReply reply = ping.Send(Address);

                                this.Dispatcher.Invoke((Action)(() =>
                                {

                                    if (reply.Status == IPStatus.Success)
                                    {
                                        tbConsole.AppendText("\nReply from " + reply.Address.ToString() + ": bytes=" + reply.Buffer.Length.ToString() + " time=" + reply.RoundtripTime.ToString() + "ms TTL=" + reply.Options.Ttl.ToString());
                                    }
                                    else
                                    {
                                        tbConsole.AppendText("\n" + reply.Status.ToString());
                                    }
                                }));
                                // ping送信の間隔を取る
                                if (i < pingCount)
                                {
                                    System.Threading.Thread.Sleep(timeOutSeconds * 1000);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        tbConsole.AppendText("\nPing Failed");
                        tbConsole.AppendText("\n" + ex.Message);
                        tbConsole.AppendText("\nStackTrace: \n" + ex.StackTrace);
                    }));
                }
            });
            thread.Start();
        }

        private void TbPingCount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            bool cut = false;
            string check = "0123456789\b";

            if (check.Contains(e.Text) == false)
            {
                cut = true;
            }

            e.Handled = cut;
        }

    }
}
