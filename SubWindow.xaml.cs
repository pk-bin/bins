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
    /// SubWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SubWindow : UserControl
    {
        private string[] List_value = new string[] { "GPS L1", "GPS L2", "GPS L5", "Glonass L1", "Glonass L5", "Galileo E1", "Galileo E5", "Beidou B1", "Beidou B2" };

        public XyDataSeries<float, float> Left_spectrom_data = new XyDataSeries<float, float>();
        public Heatmap2DArrayDataSeries<float> Left_spectrogram_data;
        private float[,] Left_spectrogramBuffer = new float[Constant.TIME_VALUE, Constant.MAX_SPEC];

        public XyDataSeries<float, float> Right_spectrom_data = new XyDataSeries<float, float>();
        public Heatmap2DArrayDataSeries<float> Right_spectrogram_data;
        private float[,] Right_spectrogramBuffer = new float[Constant.TIME_VALUE, Constant.MAX_SPEC];

        private Window1_spectrogram win1_spectrogram = new Window1_spectrogram();
        private Window2_spectrogram win2_spectrogram = new Window2_spectrogram();
        private Window1_spectrom win1_spectrom = new Window1_spectrom();
        private Window2_spectrom win2_spectrom = new Window2_spectrom();
        private Window1_Global_search win1_glo = new Window1_Global_search();
        private Window2_Global_search win2_glo = new Window2_Global_search();
        private float[] xvalue = new float[Constant.MAX_SPEC];
        private int cnt = 0;
        public SubWindow()
        {
            InitializeComponent();
            for (int i = 0; i < Constant.MAX_SPEC; i++)
            {
                xvalue[i] = i;
            }
            for (int i = 0; i < List_value.Length; i++)
            {
                ComboBax1.Items.Add(List_value[i]);
                ComboBax2.Items.Add(List_value[i]);
            }
            ComboBax1.SelectedIndex = 0;
            ComboBax2.SelectedIndex = 0;

            Left_spectrogram_data = new Heatmap2DArrayDataSeries<float>(Left_spectrogramBuffer, ix =>ix, iy =>iy);
            win1_spectrogram.HeatmapRenderableSeries.DataSeries = Left_spectrogram_data;
            win1_spectrom.LineRenderableSeries.DataSeries = Left_spectrom_data;

            Right_spectrogram_data = new Heatmap2DArrayDataSeries<float>(Right_spectrogramBuffer, ix => ix, iy => iy);
            win2_spectrogram.HeatmapRenderableSeries.DataSeries = Right_spectrogram_data;
            win2_spectrom.LineRenderableSeries.DataSeries = Right_spectrom_data;

            window1_spectrom.Children.Add(win1_spectrom);
            window2_spectrom.Children.Add(win2_spectrom);
            window1_spectrogram.Children.Add(win1_spectrogram);
            window2_spectrogram.Children.Add(win2_spectrogram);
            window1_global.Children.Add(win1_glo);
            window2_global.Children.Add(win2_glo);
        }
        private void control_Window1(object sender, RoutedEventArgs e)
        {
            if (window1.Visibility == Visibility.Collapsed)
            {
                window1.Visibility = Visibility.Visible;
                BitmapImage img = new BitmapImage(new Uri("icon\\side_close.png", UriKind.Relative));
                window1_image.Source = img;
            }
            else
            {
                window1.Visibility = Visibility.Collapsed;
                BitmapImage img = new BitmapImage(new Uri("icon\\side_open.png", UriKind.Relative));
                window1_image.Source = img;
            }
        }
        private void control_Window2(object sender, RoutedEventArgs e)
        {
            if (window2.Visibility == Visibility.Collapsed)
            {
                window2.Visibility = Visibility.Visible;
                BitmapImage img = new BitmapImage(new Uri("icon\\side_close.png", UriKind.Relative));
                window2_image.Source = img;
            }
            else
            {
                window2.Visibility = Visibility.Collapsed;
                BitmapImage img = new BitmapImage(new Uri("icon\\side_open.png", UriKind.Relative));
                window2_image.Source = img;
            }
        }
        public void display_Left_Spec(ref float[] yvalue)
        {
            Left_spectrom_data.Clear();
            Left_spectrom_data.Append(xvalue, yvalue);
            if (cnt < Constant.TIME_VALUE)
            {
                Buffer.BlockCopy(yvalue, 0, Left_spectrogramBuffer, cnt * Constant.MAX_SPEC * 4, Constant.MAX_SPEC * 4);
                cnt++;
            }
            else
            {
                Buffer.BlockCopy(Left_spectrogramBuffer, 0, Left_spectrogramBuffer, Constant.MAX_SPEC * 4, Constant.MAX_SPEC * 4 * (Constant.TIME_VALUE - 1));
                Buffer.BlockCopy(yvalue, 0, Left_spectrogramBuffer, 0, Constant.MAX_SPEC * 4);
            }
            Left_spectrogram_data.InvalidateParentSurface(RangeMode.None);
        }
        public void display_Right_Spec(ref float[] yvalue)
        {
            Right_spectrom_data.Clear();
            Right_spectrom_data.Append(xvalue, yvalue);
            if (cnt < Constant.TIME_VALUE)
            {
                Buffer.BlockCopy(yvalue, 0, Right_spectrogramBuffer, cnt * Constant.MAX_SPEC * 4, Constant.MAX_SPEC * 4);
                cnt++;
            }
            else
            {
                Buffer.BlockCopy(Right_spectrogramBuffer, 0, Right_spectrogramBuffer, Constant.MAX_SPEC * 4, Constant.MAX_SPEC * 4 * (Constant.TIME_VALUE - 1));
                Buffer.BlockCopy(yvalue, 0, Right_spectrogramBuffer, 0, Constant.MAX_SPEC * 4);
            }
            Right_spectrogram_data.InvalidateParentSurface(RangeMode.None);
        }
    }
}
