using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for VehicleView.xaml
    /// </summary>
    public partial class VehicleView : UserControl, INotifyPropertyChanged
    {
        public VehicleRecord VehicleRecord
        {
            get {
                return (VehicleRecord)GetValue(VehicleRecordProperty); 
            }
            set
            {
                SetValue(VehicleRecordProperty, value);
                OnPropertyChanged();
                
            }
        }
        public string License
        {
            get
            {
                if (((VehicleRecord)GetValue(VehicleRecordProperty)) == null)
                {
                    return "无车辆";
                }
                if (((VehicleRecord)GetValue(VehicleRecordProperty)).License == null)
                {
                    return "无车辆";
                }
                return ((VehicleRecord)GetValue(VehicleRecordProperty)).License;
            }
        }
        public string EnterWeight
        {
            get {
                if (VehicleRecord == null)
                {
                    return "无数据";
                }
                return ((VehicleRecord)GetValue(VehicleRecordProperty)).EnterWeight <0.0?"无数据": ((VehicleRecord)GetValue(VehicleRecordProperty)).EnterWeight.ToString()+"KG"; 
            }
        }

        public string ExitWeight
        {
            get {
                if (VehicleRecord == null)
                {
                    return "无数据";
                }
                return ((VehicleRecord)GetValue(VehicleRecordProperty)).ExitWeight <0.0 ? "无数据" : ((VehicleRecord)GetValue(VehicleRecordProperty)).ExitWeight.ToString() + "KG"; 
            }
        }
        public string DateCreated
        {
            get
            {
                if (VehicleRecord == null)
                {
                    return "无数据";
                }
                return VehicleRecord.DateCreated.ToString("yyyy年\nmm月dd日");
            }
        }

        public string NetWeight
        {
            get {
                if (VehicleRecord == null)
                {
                    return "无数据";
                }
                if(((VehicleRecord)GetValue(VehicleRecordProperty)).ExitWeight < 0.0 || ((VehicleRecord)GetValue(VehicleRecordProperty)).EnterWeight < 0.0)
                {
                    return "无数据";
                }
                return (((VehicleRecord)GetValue(VehicleRecordProperty)).EnterWeight - ((VehicleRecord)GetValue(VehicleRecordProperty)).ExitWeight).ToString() + "KG";
            }
        }

        public static readonly DependencyProperty VehicleRecordProperty = DependencyProperty.Register("VehicleRecord", typeof(VehicleRecord), typeof(VehicleView), new PropertyMetadata(null, (e, s) => { 
            if ((s.NewValue as VehicleRecord) != null)
            {
                ((VehicleView)e).VehicleRecord.License = (s.NewValue as VehicleRecord).License;
                ((VehicleView)e).VehicleRecord.EnterWeight = (s.NewValue as VehicleRecord).EnterWeight;
                ((VehicleView)e).VehicleRecord.ExitWeight = (s.NewValue as VehicleRecord).ExitWeight;
                ((VehicleView)e).VehicleRecord.DateCreated = (s.NewValue as VehicleRecord).DateCreated;
                ((VehicleView)e).OnPropertyChanged("License");
                ((VehicleView)e).OnPropertyChanged("EnterWeight");
                ((VehicleView)e).OnPropertyChanged("ExitWeight");
                ((VehicleView)e).OnPropertyChanged("DateCreated");
                ((VehicleView)e).OnPropertyChanged("NetWeight");
            }
        }));
        public VehicleView()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
