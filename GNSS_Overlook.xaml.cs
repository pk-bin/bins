using SciChart.Charting.Model.DataSeries;
using SciChart.Data.Model;
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
    /// GNSS_Overlook.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class GNSS_Overlook : UserControl
    {
        public LocateTrace locate_trace = new LocateTrace();
        public UserControl1 alti = new UserControl1();
        public SkyPlot sky = new SkyPlot();
        private IXyDataSeries<short, short> series0;
        int t = 0;
        public GNSS_Overlook()
        {
            InitializeComponent();
            Locate_Window.Children.Add(locate_trace);
            Altitude_Window.Children.Add(alti);
            SkyPlot_Window.Children.Add(sky);
        }
        public void SateNewData(Satellite[] satellite, int length)
        {
            datagrid2.Items.Clear();
            for (int i = 0; i < length; i++)
            {
                datagrid2.Items.Add(new SkyPlots(satellite[i].number, satellite[i].elevation, satellite[i].azimuth, satellite[i].SNR));
            }
            Filtering(satellite_combo.SelectedIndex);
            sky.OnNewData(satellite, length, satellite_combo.SelectedIndex);
            sky.polarChart.XAxis.VisibleRange = new DoubleRange(0, 359.999);
            sky.polarChart.YAxis.VisibleRange = new DoubleRange(0, 90);
        }
        public void GPSNewData(GPGGA gpgga)
        {
            locate_trace.OnNewData(gpgga.latitude, gpgga.longitude);
            alti.OnNewData(gpgga);
        }

        private void satellite_combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox currentComboBox = sender as ComboBox;
            Filtering(currentComboBox.SelectedIndex);
            sky.select_key = currentComboBox.SelectedIndex;
            sky.Redraw();
        }
        private void Filtering(int k)
        {
            switch (k)
            {
                case 0:
                    if (datagrid2 != null)
                    {
                        datagrid2.Items.Filter = new Predicate<object>(item => ((SkyPlots)item).Name.Contains("GPS") || ((SkyPlots)item).Name.Contains("SBAS") || ((SkyPlots)item).Name.Contains("Glonass") || ((SkyPlots)item).Name.Contains("QZSS") || ((SkyPlots)item).Name.Contains("Beidou") || ((SkyPlots)item).Name.Contains("Not_Use"));
                        datagrid2.Items.Refresh();
                    }
                    break;
                case 1:
                    if (datagrid2 != null)
                    {
                        datagrid2.Items.Filter = new Predicate<object>(item => ((SkyPlots)item).Name.Contains("GPS"));
                        datagrid2.Items.Refresh();
                    }
                    break;
                case 2:
                    if (datagrid2 != null)
                    {
                        datagrid2.Items.Filter = new Predicate<object>(item => ((SkyPlots)item).Name.Contains("SBAS"));
                        datagrid2.Items.Refresh();
                    }
                    break;
                case 3:
                    if (datagrid2 != null)
                    {
                        datagrid2.Items.Filter = new Predicate<object>(item => ((SkyPlots)item).Name.Contains("Glonass"));
                        datagrid2.Items.Refresh();
                    }
                    break;
                case 4:
                    if (datagrid2 != null)
                    {
                        datagrid2.Items.Filter = new Predicate<object>(item => ((SkyPlots)item).Name.Contains("QZSS"));
                        datagrid2.Items.Refresh();
                    }
                    break;
                case 5:
                    if (datagrid2 != null)
                    {
                        datagrid2.Items.Filter = new Predicate<object>(item => ((SkyPlots)item).Name.Contains("Beidou"));
                        datagrid2.Items.Refresh();
                    }
                    break;
                case 6:
                    if (datagrid2 != null)
                    {
                        datagrid2.Items.Filter = new Predicate<object>(item => ((SkyPlots)item).Name.Contains("Not Use"));
                        datagrid2.Items.Refresh();
                    }
                    break;
                default:
                    break;
            }
        }
    }
    class SkyPlots
    {
        public string Name { get; set; }
        public short Number1 { get; set; }
        public short elevation1 { get; set; }
        public short azimuth1 { get; set; }
        public short SNR1 { get; set; }

        public SkyPlots(short Number1, short elevation1, short azimuth1, short SNR1)
        {
            this.Number1 = Number1;
            this.elevation1 = elevation1;
            this.azimuth1 = azimuth1;
            this.SNR1 = SNR1;
            if (Number1 > 0 && Number1 < 33)
                this.Name = "GPS";
            else if (Number1 > 32 && Number1 < 55)
                this.Name = "SBAS";
            else if (Number1 > 64 && Number1 < 97)
                this.Name = "Glonass";
            else if (Number1 > 192 && Number1 < 201)
                this.Name = "QZSS";
            else if (Number1 > 200 && Number1 < 236)
                this.Name = "Beidou";
            else
                this.Name = "Not_Use";
        }
    }
}
