using System;
using System.Collections.Generic;
using System.IO.Ports;
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
using System.Windows.Threading;

namespace SerialDiagnostic
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SerialPort serialPort = new SerialPort();
        string recievedData;

        FlowDocument mcFlowDoc = new FlowDocument();
        Paragraph para = new Paragraph();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string[] ports = SerialPort.GetPortNames();
            //Console.WriteLine("The following serial ports were found:");            
            foreach (string port in ports)
            {
                //Console.WriteLine(port); // Display each port name to the console.
                cBoxComPort.Items.Add(port);
            }
        }

        private void BtnOpen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                serialPort.PortName = cBoxComPort.Text;
                serialPort.BaudRate = Convert.ToInt32(cBoxBaudRate.Text);
                serialPort.DataBits = Convert.ToInt32(cBoxDataBits.Text);
                serialPort.StopBits = (StopBits)Enum.Parse(typeof(StopBits), cBoxStopBits.Text);
                serialPort.Parity = (Parity)Enum.Parse(typeof(Parity), cBoxParityBits.Text);
                serialPort.Open(); // Open port.
                //serialPort.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(serialPort_DataRecieved);
                serialPort.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(port_DataReceived);
                pBar.Value = 100;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        /// <summary>
        /// 串口读取数据
        /// </summary>
        private void port_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            try
            {
                string strReceive;
                byte firstByte = Convert.ToByte(serialPort.ReadByte());
                if (firstByte == 0x02)
                {
                    int bytesRead = serialPort.BytesToRead;
                    byte[] bytesData = new byte[bytesRead];
                    byte byteData;

                    for (int i = 0; i < bytesRead; i++)
                    {
                        byteData = Convert.ToByte(serialPort.ReadByte());
                        bytesData[i] = byteData;
                        if (byteData == 0x03)//结束
                        {
                            break;
                        }
                    }
                    strReceive = Encoding.Default.GetString(bytesData);
                    byte[] firstBytes = new byte[1];
                    firstBytes[0] = firstByte;
                    recievedData = Encoding.Default.GetString(firstBytes) + Encoding.Default.GetString(bytesData);
                    Dispatcher.Invoke(DispatcherPriority.Send, new UpdateUiTextDelegate(DataWrited), recievedData);
                    Dispatcher.Invoke(DispatcherPriority.Send, new UpdateUiTextDelegate(HexDataWrited), BitConverter.ToString(firstBytes) + "-" + BitConverter.ToString(bytesData));
                    Dispatcher.Invoke(DispatcherPriority.Send, new UpdateUiTextDelegate(WeightDataWrited), GetWeightOfPort(strReceive));
                }
                else
                {
                    // Collecting the characters received to our 'buffer' (string).
                    byte[] firstBytes = new byte[1];
                    firstBytes[0] = firstByte;
                    string existing = serialPort.ReadExisting();
                    recievedData = Encoding.Default.GetString(firstBytes) + existing;
                    Dispatcher.Invoke(DispatcherPriority.Send, new UpdateUiTextDelegate(HexDataWrited), BitConverter.ToString(firstBytes) + "-" + BitConverter.ToString(Encoding.Default.GetBytes(existing)));
                    // Delegate a function to display the received data.
                    Dispatcher.Invoke(DispatcherPriority.Send, new UpdateUiTextDelegate(DataWrited), recievedData);
                    Dispatcher.Invoke(DispatcherPriority.Send, new UpdateUiTextDelegate(WeightDataWrited), "No Weight Data");
                }
            }
            catch (Exception ex)
            {

            }


        }

        /// <summary>
        /// 返回串口读取的重量
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        private string GetWeightOfPort(string weight)
        {
            try
            {
                if (string.IsNullOrEmpty(weight) || (weight.IndexOf("+") < 0 && weight.IndexOf("-") < 0) || weight.Length < 6)
                {
                    return "0.000";
                }
                char sign = weight[0];
                weight = weight.Replace("+", "");
                weight = weight.Replace("-", "");
                double num = int.Parse(weight.Substring(0, 6));
                num = num / Math.Pow(10, weight[6] - 0x30);
                return sign.ToString() + num.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message + " Data:" + weight);
                return "Error: " + ex.Message + " Data:" + weight;
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            if (serialPort.IsOpen)
            {
                serialPort.Close(); // Close port.

                pBar.Value = 0;
            }
        }

        private void BtnSendData_Click(object sender, RoutedEventArgs e)
        {
            if (serialPort.IsOpen)
            {
                //serialPort.Write(tBoxOutData.Text);
            }
        }

        private delegate void UpdateUiTextDelegate(string text);
        private void serialPort_DataRecieved(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {

            // Collecting the characters received to our 'buffer' (string).
            recievedData = serialPort.ReadExisting();

            // Delegate a function to display the received data.
            Dispatcher.Invoke(DispatcherPriority.Send, new UpdateUiTextDelegate(DataWrited), recievedData);
        }

        private void DataWrited(string text)
        {
            tBoxInData.Text += text + "\n";
        }
        private void HexDataWrited(string text)
        {
            tBoxInDataHex.Text += text + "\n";
        }
        private void WeightDataWrited(string text)
        {
            tbWeight.Text += text + "\n";
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            tBoxInData.Text = "";
            tBoxInDataHex.Text = "";
            tbWeight.Text = "";
        }
    }
    }
