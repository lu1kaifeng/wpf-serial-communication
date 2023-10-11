using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace SerialCommunication
{
    /// <summary>
    /// Interaction logic for PoundView.xaml
    /// </summary>
    public partial class PoundView : UserControl, INotifyPropertyChanged
    {
        public bool Online
        {
            get { return (bool)GetValue(OnlineProperty); }
            set {
                if (value)
                {
                    OfflineStat.Visibility = Visibility.Hidden;
                    OnlineStat.Visibility = Visibility.Visible;
                }
                else
                {
                    OfflineStat.Visibility = Visibility.Visible;
                    OnlineStat.Visibility = Visibility.Hidden;
                }
                SetValue(OnlineProperty, value);
                OnPropertyChanged();
            }
        }
        public float Weight
        {
            get { return (float)GetValue(WeightProperty); }
            set
            {
                SetValue(WeightProperty, value);
                OnPropertyChanged();
            }
        }
        private PoundComm PoundComm = null;
        public static readonly DependencyProperty OnlineProperty = DependencyProperty.Register("Online", typeof(bool), typeof(PoundView), new PropertyMetadata(false));
        public static readonly DependencyProperty WeightProperty = DependencyProperty.Register("Weight", typeof(float), typeof(PoundView), new PropertyMetadata((float)0.0, (e,s) => {
            ((PoundView)e).Weight = (float)s.NewValue;
        }));
        public PoundView()
        {
            InitializeComponent();
            Online = false;
            string[] ports = SerialPort.GetPortNames();
            //Console.WriteLine("The following serial ports were found:");            
            foreach (string port in ports)
            {
                //Console.WriteLine(port); // Display each port name to the console.
                cBoxComPort.Items.Add(port);
            }
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PoundComm = new PoundComm(cBoxComPort.Text, (double? weight) => { 
                    if(weight.HasValue)
                    {
                        Online = true;
                        this.Dispatcher.Invoke(() => { 
                            WeightDisplay.Text = weight.Value.ToString("0.00");
                            Weight = (float)weight.Value;
                        });
                    }
                    else
                    {
                        Online = false;
                    }
                });
                btnOpen.IsEnabled = false;
                cBoxComPort.IsEnabled = false;
                btnClose.IsEnabled = true;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.PoundComm.Close();
            this.PoundComm = null;
            Online = false;
            btnOpen.IsEnabled = true;
            btnClose.IsEnabled = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
