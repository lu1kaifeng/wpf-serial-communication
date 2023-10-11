using System;
using System.Collections.Generic;
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

using System.IO.Ports;
using System.Windows.Threading;
using System.Threading;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SerialCommunication
{
    class MainDataContext: INotifyPropertyChanged
    {
        private bool _EnterFlag = true;
        public bool EnterFlag { set {
                _EnterFlag = value;
                OnPropertyChanged();
                OnPropertyChanged("EnterButtonEnabled");
                OnPropertyChanged("ExitButtonEnabled");
            } get {
                return _EnterFlag;
            } }
        
        private float _Weight = 0.0f;
        public float Weight { get { 
                return _Weight;
            } set { 
                _Weight = value;
                
                VehicleRecord vehicleRecord = new VehicleRecord();
                if (VehicleRecord != null)
                { 
                    vehicleRecord.ExitWeight = VehicleRecord.ExitWeight;
                    vehicleRecord.EnterWeight   = VehicleRecord.EnterWeight;
                    vehicleRecord.License = VehicleRecord.License;
                    vehicleRecord.DateCreated = VehicleRecord.DateCreated;
                }
                else
                {
                    VehicleRecordInit(vehicleRecord);
                }

                    if (EnterFlag) { 
                    vehicleRecord.EnterWeight = _Weight;
                }
                else
                {
                    vehicleRecord.ExitWeight = _Weight;
                }
                VehicleRecord = vehicleRecord;
                OnPropertyChanged();
                OnPropertyChanged("EnterButtonEnabled");
                OnPropertyChanged("ExitButtonEnabled");
            } 
        }

        public VehicleRecord _VehicleRecord = null;

        public bool EnterButtonEnabled { get {
                if(VehicleRecord == null) return false; 
                return VehicleRecord.EnterWeight > 0 && EnterFlag;
            } }

        public bool ExitButtonEnabled
        {
            get
            {
                if (VehicleRecord == null) return false;
                return VehicleRecord.ExitWeight > 0 && !EnterFlag;
            }
        }

        public VehicleRecord VehicleRecord { get {
            return _VehicleRecord;
            } set { 
                _VehicleRecord = value;
                OnPropertyChanged();
            } }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Reset()
        {
            VehicleRecord vehicleRecord = new VehicleRecord();
            VehicleRecordInit(vehicleRecord);
            VehicleRecord = vehicleRecord;
            EnterFlag = true;
        }
        private void VehicleRecordInit(VehicleRecord vehicleRecord)
        {
            vehicleRecord.EnterWeight = -1;
            vehicleRecord.ExitWeight = -1;
            vehicleRecord.License = null;
            vehicleRecord.DateCreated = DateTime.Now;
        }
        public void Load(VehicleRecord externalVehicleRecord)
        {
            VehicleRecord vehicleRecord = new VehicleRecord();
            vehicleRecord.EnterWeight = externalVehicleRecord.EnterWeight;
            vehicleRecord.ExitWeight = externalVehicleRecord.ExitWeight;
            vehicleRecord.License = externalVehicleRecord.License;
            vehicleRecord.DateCreated = externalVehicleRecord.DateCreated;
            VehicleRecord = vehicleRecord;
            EnterFlag = false;
        }
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SerialPort serialPort = new SerialPort();
        string recievedData;
        FlowDocument mcFlowDoc = new FlowDocument();
        Paragraph para = new Paragraph();
        MainDataContext MainDataContext = new MainDataContext();
        public MainWindow()
        {
            InitializeComponent();
            DataContext = MainDataContext;
        }
        int i = 1;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            i++;
           
            VehicleRecord vehicleRecord = new VehicleRecord();
            vehicleRecord.EnterWeight = i;
            vehicleRecord.ExitWeight = -1;
            vehicleRecord.License = null;
            MainDataContext.VehicleRecord = vehicleRecord;
            MessageBox.Show(Vehicle.VehicleRecord.EnterWeight.ToString()+"应为"+i.ToString());
            //MainDataContext.text = "cock";
            //MessageBox.Show(tb1.Text);
        }

        private void btnEnter_Click(object sender, RoutedEventArgs e)
        {
            MainDataContext.EnterFlag = false;
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {

            MessageBox.Show(MainDataContext.VehicleRecord.ToString());
            MainDataContext.Reset();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            VehicleRecord vehicleRecord = new VehicleRecord();
            vehicleRecord.EnterWeight = 100;
            vehicleRecord.ExitWeight = -1;
            vehicleRecord.License = "豫B123456789";
            MainDataContext.Load(vehicleRecord);
        }
    }
}
