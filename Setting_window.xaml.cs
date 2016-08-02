using SciChart.Charting.Model.DataSeries;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Setting_window.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Setting_window : UserControl
    {
        public Transport client = new Transport();
        public Setting_window()
        {
            InitializeComponent();
            
            Parameter_Init();
        }
        private void ConnectToServer(object sender, RoutedEventArgs e)
        {
            client.ConnectToServer(hostName.Text, UInt16.Parse(hostPort.Text));
            if (client.Connected)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    ((MainWindow)Application.Current.MainWindow).timer.Stop();
                    ((MainWindow)Application.Current.MainWindow).gnss.locate_trace.ClearDataSeries();
                    ((MainWindow)Application.Current.MainWindow).gnss.sky.ClearDataSeries();
                    ((MainWindow)Application.Current.MainWindow).gnss.alti.ClearDataSeries();
                });
                GPS_Request_Send(null, null);
                Left_Send_Spec_In(null, null);
                Right_Send_Spec_In(null, null);
            }
        }
        private void DisConnectToServer(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                ((MainWindow)Application.Current.MainWindow).timer.Start();
            });
            client.DisconnectToServer();
        }
        private void GPS_Request_Send(object sender, RoutedEventArgs e)
        {
            client.Send_GPS_In();
        }
        private void Left_Send_Spec_In(object sender, RoutedEventArgs e)
        {
            client.Send_Spec_In(Convert.ToDouble(Left_spec_dFreq.Text),
                                Convert.ToSingle(Left_spec_fSpan.Text),
                                Convert.ToByte(Left_spec_u8Ant_Mode.Text),
                                Convert.ToByte(1),
                                Convert.ToByte(2));
            Text_Setting();
        }
        private void Right_Send_Spec_In(object sender, RoutedEventArgs e)
        {
            client.Send_Spec_In(Convert.ToDouble(Right_spec_dFreq.Text),
                                Convert.ToSingle(Right_spec_fSpan.Text),
                                Convert.ToByte(Right_spec_u8Ant_Mode.Text),
                                Convert.ToByte(1),
                                Convert.ToByte(2));
            Text_Setting();
        }
        private void Text_Setting()
        {
            FileStream init_file;
            String init_string = "";
            byte[] init_buffer;

            init_string = "";

            init_file = new FileStream("PS_L16.ini", FileMode.Create, FileAccess.Write, FileShare.None);

            init_string += "Left_spec_dFreq = " + Left_spec_dFreq.Text + "\n";
            init_string += "Left_spec_fSpan = " + Left_spec_fSpan.Text + "\n";
            init_string += "Left_spec_u8Ant_Mode = " + Left_spec_u8Ant_Mode.Text + "\n";

            init_string += "Right_spec_dFreq = " + Right_spec_dFreq.Text + "\n";
            init_string += "Right_spec_fSpan = " + Right_spec_fSpan.Text + "\n";
            init_string += "Right_spec_u8Ant_Mode = " + Right_spec_u8Ant_Mode.Text + "\n";

            init_buffer = Encoding.UTF8.GetBytes(init_string);
            init_file.Write(init_buffer, 0, init_buffer.Length);
            init_file.Close();
        }
        void Parameter_Init()
        {
            try
            {
                string[] lines = System.IO.File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + @"PS_L16.ini");
                string[] tokens;
                if (lines.Length != 6) throw new FileNotFoundException();

                tokens = lines[0].Split(' ');
                Left_spec_dFreq.Text = tokens[2];
                tokens = lines[1].Split(' ');
                Left_spec_fSpan.Text = tokens[2];
                tokens = lines[2].Split(' ');
                Left_spec_u8Ant_Mode.Text = tokens[2];

                tokens = lines[3].Split(' ');
                Right_spec_dFreq.Text = tokens[2];
                tokens = lines[4].Split(' ');
                Right_spec_fSpan.Text = tokens[2];
                tokens = lines[5].Split(' ');
                Right_spec_u8Ant_Mode.Text = tokens[2];
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
                Reset_Parameter(null, null);
            }
        }
        private void Reset_Parameter(object sender, RoutedEventArgs e)
        {
            Left_spec_dFreq.Text = "752";
            Left_spec_fSpan.Text = "120";
            Left_spec_u8Ant_Mode.Text = "0";

            Right_spec_dFreq.Text = "752";
            Right_spec_fSpan.Text = "120";
            Right_spec_u8Ant_Mode.Text = "0";

            Text_Setting();
        }
    }
}
