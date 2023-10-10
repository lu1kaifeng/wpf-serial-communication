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

namespace SerialCommunication
{
    /// <summary>
    /// Interaction logic for VehicleView.xaml
    /// </summary>
    public partial class VehicleView : UserControl
    {
        public VehicleRecord VehicleRecord
        {
            get {
                return (VehicleRecord)GetValue(VehicleRecordProperty); 
            }
            set
            {
                SetValue(VehicleRecordProperty, value);
            }
        }
        public string License
        {
            get
            {
                if (VehicleRecord == null)
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
            get {
                if (VehicleRecord == null)
                {
                    return "无数据";
                }
                return ((VehicleRecord)GetValue(VehicleRecordProperty)).DateCreated.ToString("yyyy年MM月dd日"); 
            }
        }

        public static readonly DependencyProperty VehicleRecordProperty = DependencyProperty.Register("VehicleRecord", typeof(VehicleRecord), typeof(VehicleView), new PropertyMetadata(null));
        public VehicleView()
        {
            InitializeComponent();
        }
    }
}
