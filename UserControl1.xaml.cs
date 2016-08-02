using SciChart.Charting.Model.DataSeries;
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

namespace WpfApplication1
{
    /// <summary>
    /// UserControl1.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        private int FifoSize = 10;
        private IXyDataSeries<TimeSpan, double> altitude_data;

        public UserControl1()
        {
            InitializeComponent();

            CreateDataSetAndSeries();
        }

        private void CreateDataSetAndSeries()
        {
            // Create new Dataseries of type X=double, Y=double
            altitude_data = new XyDataSeries<TimeSpan, double>();

            altitude_data.FifoCapacity = FifoSize;

            // Set the dataseries on the chart's RenderableSeries
            LineRenderSeries.DataSeries = altitude_data;

        }
        public void ClearDataSeries()
        {
            if (altitude_data == null)
                return;

            using (sciChart.SuspendUpdates())
            {
                altitude_data.Clear();
            }
        }
        private void OnIsFifoSeriesChanged(object sender, RoutedEventArgs e)
        {
            CreateDataSetAndSeries();
        }
        public void OnNewData(GPGGA gpgga)   //lat 위도 lng 경도
        {
            double dy;
            int t;
            dy = gpgga.altitude - Constant.ALTITUDE;
            t = gpgga.time_hour * 60 * 60  + gpgga.time_minute * 60  + gpgga.time_second ;
            // Suspending updates is optional, and ensures we only get one redraw
            // once all three dataseries have been appended to
            using (sciChart.SuspendUpdates())
            {
                // Append x,y data to previously created series
                altitude_data.Append(TimeSpan.FromSeconds(t), dy);
            }
        }
    }
}
