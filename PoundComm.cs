using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace SerialCommunication
{
    internal class PoundComm
    {
        private SerialPort SerialPort;
        public PoundComm(string PortName, WeightUpdate Callback)
        {
            this.Callback = Callback;
            SerialPort = new SerialPort();
            SerialPort.PortName = PortName;
            SerialPort.BaudRate = 2400;
            SerialPort.DataBits = 8;
            SerialPort.StopBits = StopBits.OnePointFive;
            SerialPort.Parity = Parity.Even;
            SerialPort.Open();
            SerialPort.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(OnDataReceived);
        }
        public delegate void WeightUpdate(double? Weight);
        private WeightUpdate Callback;
        public void Close() { 
            SerialPort.Close();
        }
        private void OnDataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            try
            {
                string strReceive;
                byte firstByte = Convert.ToByte(SerialPort.ReadByte());
                if (firstByte == 0x02)
                {
                    int bytesRead = SerialPort.BytesToRead;
                    byte[] bytesData = new byte[bytesRead];
                    byte byteData;

                    for (int i = 0; i < bytesRead; i++)
                    {
                        byteData = Convert.ToByte(SerialPort.ReadByte());
                        bytesData[i] = byteData;
                        if (byteData == 0x03)//结束
                        {
                            break;
                        }
                    }
                    strReceive = Encoding.Default.GetString(bytesData);
                    byte[] firstBytes = new byte[1];
                    firstBytes[0] = firstByte;
                    Application.Current.Dispatcher.Invoke(()=>{ Callback(GetWeightOfPort(strReceive)); });
                }
                
            }
            catch(Exception ee)
            {
                
            }


        }
        private double? GetWeightOfPort(string weight)
        {
            try
            {
                if (string.IsNullOrEmpty(weight) || (weight.IndexOf("+") < 0 && weight.IndexOf("-") < 0) || weight.Length < 6)
                {
                    return null;
                }
                char sign = weight[0];
                weight = weight.Replace("+", "");
                weight = weight.Replace("-", "");
                double num = int.Parse(weight.Substring(0, 6));
                num = num / Math.Pow(10, weight[6] - 0x30);
                return (sign=='+'? 1.0 : -1.0) * num;
            }
            catch
            {
                return null;
            }
        }
    }
}
