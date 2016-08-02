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
using System.Windows.Threading;

namespace WpfApplication1
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        private int time_second;
        private SubWindow sub = new SubWindow();
        public GNSS_Overlook gnss = new GNSS_Overlook();
        private Setting_window setting = new Setting_window();
        //    private Setting_window setting = new Setting_window();
        private int mains = 0; //0 main ,1 감시, 2설정
        public DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Render);
        public MainWindow()
        {
            InitializeComponent();
            var k = DateTime.Now.TimeOfDay;
            Chart_window.Children.Add(sub);
            mains = 0;
            if(!setting.client.Connected)
            {
                Position_Time.Text =" System Time : "+ DateTime.Now.ToString("yyyy-MM-dd  HH:mm:ss") + "  " + DateTime.Now.DayOfWeek;
                timer.Interval = TimeSpan.FromMilliseconds(1000);
                timer.Tick += TimerOnTick_Spec;
                timer.Start();
            }
            
        }
        private void TimerOnTick_Spec(object sender, EventArgs e)
        {
            Position_Time.Text = " System Time : " + DateTime.Now.ToString("yyyy-MM-dd  HH:mm:ss") + "  " + DateTime.Now.DayOfWeek;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (mains == 0)
                return;
            else
            {
                Chart_window.Children.Clear();
                Chart_window.Children.Add(sub);
                mains = 0;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (mains == 1)
                return;
            else
            {
                Chart_window.Children.Clear();
                Chart_window.Children.Add(gnss);
                mains = 1;
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (mains == 2)
                return;
            else
            {
                Chart_window.Children.Clear();
                Chart_window.Children.Add(setting);
                mains = 2;
            }
        }/*
        private void Control_open(object sender, RoutedEventArgs e)
        {
            if (Controls.Visibility == Visibility.Collapsed)
            {
                Controls.Visibility = Visibility.Visible;
                BitmapImage img = new BitmapImage(new Uri("icon\\control_close.png", UriKind.Relative));
                Control_image.Source = img;
            }
            else
            {
                Controls.Visibility = Visibility.Collapsed;
                BitmapImage img = new BitmapImage(new Uri("icon\\control_open.png", UriKind.Relative));
                Control_image.Source = img;
            }
        }*/
        public void Left_Spectrum(ref float[] yvalue)
        {
            sub.display_Left_Spec(ref yvalue);
        }
        public void Right_Spectrum(ref float[] yvalue)
        {
            sub.display_Right_Spec(ref yvalue);
        }
        public void Add_GPS_Data(ref byte[] data_buffer)
        {
            //GPRMC gprmc = new GPRMC();
            //GPVTG gpvtg = new GPVTG();
            GPGGA gpgga = new GPGGA();
            //GPGLL gpgll = new GPGLL();
            Satellite[] satellite;
            int num_of_satellite;
            int buffer_pointer = 0;
            byte[] deserialize_buffer = new byte[1024];
            byte[] satellite_buffer;

            //Buffer.BlockCopy(data_buffer, buffer_pointer, deserialize_buffer, 0,buffer_pointer + System.Runtime.InteropServices.Marshal.SizeOf(gprmc));
            //gprmc.Deserialize(ref deserialize_buffer);
            //buffer_pointer += System.Runtime.InteropServices.Marshal.SizeOf(gprmc);

            //Buffer.BlockCopy(data_buffer, buffer_pointer, deserialize_buffer, 0, buffer_pointer + System.Runtime.InteropServices.Marshal.SizeOf(gpvtg));
            //gpvtg.Deserialize(ref deserialize_buffer);
            //buffer_pointer += System.Runtime.InteropServices.Marshal.SizeOf(gpvtg);

            Buffer.BlockCopy(data_buffer, buffer_pointer, deserialize_buffer, 0, System.Runtime.InteropServices.Marshal.SizeOf(gpgga));
            gpgga.Deserialize(ref deserialize_buffer);
            buffer_pointer += System.Runtime.InteropServices.Marshal.SizeOf(gpgga);

            //Buffer.BlockCopy(data_buffer, buffer_pointer, deserialize_buffer, 0, buffer_pointer + System.Runtime.InteropServices.Marshal.SizeOf(gpgll));
            //gpgll.Deserialize(ref deserialize_buffer);
            //buffer_pointer += System.Runtime.InteropServices.Marshal.SizeOf(gpgll);

            num_of_satellite = BitConverter.ToInt32(data_buffer, buffer_pointer);
            buffer_pointer += sizeof(int);
            satellite_buffer = new byte[8 * num_of_satellite];
            Buffer.BlockCopy(data_buffer, buffer_pointer, satellite_buffer, 0, 8 * num_of_satellite);

            satellite = new Satellite[num_of_satellite];
            for (int i = 0; i < num_of_satellite; i++)
            {
                satellite[i].number = BitConverter.ToInt16(data_buffer,buffer_pointer);
                satellite[i].elevation = BitConverter.ToInt16(data_buffer, buffer_pointer + 2);
                satellite[i].azimuth = BitConverter.ToInt16(data_buffer, buffer_pointer + 4);
                satellite[i].SNR = BitConverter.ToInt16(data_buffer, buffer_pointer + 6);

                buffer_pointer += 8;
            }
            if (setting.client.Connected)
            {
                Position_Time.Text = "GPS Time : " + DateTime.Now.ToString("yyyy-MM-dd ") + gpgga.time_hour.ToString("D2")
    + ":" + gpgga.time_minute.ToString("D2") + ":" + gpgga.time_second.ToString("D2") + "  " + DateTime.Now.DayOfWeek;
                timer.Stop();
            }
            gnss.SateNewData(satellite, num_of_satellite);
            gnss.GPSNewData(gpgga);
            if (gnss.locate_trace.isJam == true) GPS_Spec_L1.Fill = new SolidColorBrush(Colors.Red);
            if (gnss.locate_trace.isJam == false) GPS_Spec_L1.Fill = new SolidColorBrush(Colors.LightGreen);
        }
    }
}
