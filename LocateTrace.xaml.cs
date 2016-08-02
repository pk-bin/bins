using SciChart.Charting.Model.DataSeries;
using SciChart.Core.Utility;
using SciChart.Data.Model;
using SciChart.Examples.Examples.AnnotateAChart.CreateAnnotationsDynamically;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
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
    /// LocateTrace.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LocateTrace : UserControl
    {
        private Point_Marker point_marker = new Point_Marker();
        private int FifoSize = 86400;
        private IXyDataSeries<double, double> series0;
        public bool isJam = false;

        public LocateTrace()
        {
            InitializeComponent();
            point_marker.Margin = new Thickness(-5.7, -5.5, 0, 0);
            point_marker.Point.Height = 10;
            point_marker.Point.Width = 10;
            point_marker.X1 = -100;
            point_marker.Y1 = -100;
            point_marker.Point.Fill = new SolidColorBrush(System.Windows.Media.Colors.Red);
            point_marker.point_value.Text = "";
            sciChart.Annotations.Add(point_marker);
            CreateDataSetAndSeries();
        }
        private void CreateDataSetAndSeries()
        {
            // Create new Dataseries of type X=double, Y=double
            series0 = new XyDataSeries<double, double>();

            series0.FifoCapacity = FifoSize;

            // Set the dataseries on the chart's RenderableSeries
            RenderableSeries0.DataSeries = series0;

            // Set UnSorted data Accept
            RenderableSeries0.DataSeries.AcceptsUnsortedData = true;
        }
        public void ClearDataSeries()
        {
            if (series0 == null)
                return;

            using (sciChart.SuspendUpdates())
            {
                series0.Clear();
            }
        }

        public void OnNewData(double lat, double lng)   //lat 위도 lng 경도
        {
            sciChart.XAxis.VisibleRange = new DoubleRange(-25, 25);
            sciChart.YAxis.VisibleRange = new DoubleRange(-25, 25);
            double dx, dy;

            dx = calDistance(lat, lng, lat, Constant.LONGITUDE);
            dy = calDistance(lat, lng, Constant.LATITUDE, lng);

            if (Double.IsNaN(dx) || Double.IsNaN(dy))
            {
                Console.WriteLine();
            }

            if (dx > 5 || dy > 5) isJam = true;
            else isJam = false;

            dx = (lng < Constant.LONGITUDE) ? dx : -dx;
            dy = (lat < Constant.LATITUDE) ? dy : -dy;

            point_marker.X1 = dx;
            point_marker.Y1 = dy;
            // Suspending updates is optional, and ensures we only get one redraw
            // once all three dataseries have been appended to

            using (sciChart.SuspendUpdates())
            {
                // Append x,y data to previously created series
                series0.Append(dx, dy);
            }
        }
        private void OnIsFifoSeriesChanged(object sender, RoutedEventArgs e)
        {
            CreateDataSetAndSeries();
        }
        private double calDistance(double lat1, double lng1, double lat2, double lng2)
        {
            double theta, dist;
            theta = lng1 - lng2;
            dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1))
                  * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
            if (dist >= 1) return 0;
            dist = Math.Acos(dist);
            dist = rad2deg(dist);

            dist = dist * 60 * 1.1515;
            dist = dist * 1.609344;    // 단위 mile 에서 km 변환.  
            dist = dist * 1000.0;      // 단위  km 에서 m 로 변환  

            return dist;
        }
        // 주어진 도(degree) 값을 라디언으로 변환  
        private double deg2rad(double deg)
        {
            return (double)(deg * Math.PI / (double)180d);
        }

        // 주어진 라디언(radian) 값을 도(degree) 값으로 변환  
        private double rad2deg(double rad)
        {
            return (double)(rad * (double)180d / Math.PI);
        }
    }
}
