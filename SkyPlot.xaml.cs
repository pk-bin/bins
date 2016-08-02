using SciChart.Charting.Model.DataSeries;
using SciChart.Charting.Numerics.TickProviders;
using SciChart.Charting.Visuals.Axes;
using SciChart.Charting.Visuals.PointMarkers;
using SciChart.Charting.Visuals.RenderableSeries;
using SciChart.Core.Extensions;
using SciChart.Examples.Examples.AnnotateAChart.CreateAnnotationsDynamically;
using SciChart.Examples.ExternalDependencies.Data;
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
    /// SkyPlot.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SkyPlot : UserControl
    {
        private Point_Marker[] point_marker;
        XyDataSeries<short, short>[] satellite_list;
        XyDataSeries<short, short> temp_data = new XyDataSeries<short, short>();
        public int select_key = 0;
        short num_of_satellite;
        short[] exist_satellite;
        short[] sate_del_count;
        public SkyPlot()
        {
            InitializeComponent();
            satellite_list = new XyDataSeries<short, short>[300];
            sate_del_count = new short[300];
            exist_satellite = new short[300];
            point_marker = new Point_Marker[300];
            num_of_satellite = 0;

            for (int i = 0; i < 300; i++)
            {
                exist_satellite[i] = -1;
                satellite_list[i] = null;
                sate_del_count[i] = 0;
                point_marker[i] = new Point_Marker();
                point_marker[i].Margin = new Thickness(-5.7, -5.5, 0, 0);
                point_marker[i].X1 = 570;
                point_marker[i].Y1 = -100;
                polarChart.Annotations.Add(point_marker[i]);
            }

            sate_del_count = new short[300];
        }
        public void ClearDataSeries()
        {
            polarChart.Annotations.Clear();
            satellite_list = new XyDataSeries<short, short>[300];
            sate_del_count = new short[300];
            exist_satellite = new short[300];
            point_marker = new Point_Marker[300];
            num_of_satellite = 0;

            for (int i = 0; i < 300; i++)
            {
                exist_satellite[i] = -1;
                satellite_list[i] = null;
                sate_del_count[i] = 0;
                point_marker[i] = new Point_Marker();
                point_marker[i].Margin = new Thickness(-5.7, -5.5, 0, 0);
                point_marker[i].X1 = 570;
                point_marker[i].Y1 = -100;
                polarChart.Annotations.Add(point_marker[i]);
            }
            sate_del_count = new short[300];

            for (int i = 0; i < num_of_satellite; i++)
            {
                point_marker[i].X1 = 570;
                point_marker[i].Y1 = -100;
                exist_satellite[i] = -1;
            }
            num_of_satellite = 0;
            polarChart.RenderableSeries.Clear();
        }
        private void DeleteSeries(short number)
        {
            int find_index = 0;
            for (int i = 0; i < num_of_satellite; i++)
            {
                if (exist_satellite[i] == number)
                {
                    find_index = i;
                    break;
                }
            }

            polarChart.RenderableSeries.RemoveAt(find_index);
            satellite_list[number] = null;

            for (int i = find_index; i < num_of_satellite - 1; i++)
            {
                exist_satellite[i] = exist_satellite[i + 1];
            }
            exist_satellite[num_of_satellite - 1] = -1;
            num_of_satellite--;

            point_marker[number].X1 = 570;
            point_marker[number].Y1 = -100;
        }

        private void AddSeries(short number, short elevation, short azimuth, int key)
        {
            // Create a DataSeries and append some data
            satellite_list[number] = new XyDataSeries<short, short>();
            satellite_list[number].AcceptsUnsortedData = true;
            sate_del_count[number] = Constant.REMAIN_COUNT;
            exist_satellite[num_of_satellite] = number;
            num_of_satellite++;
            satellite_list[number].Append(azimuth, elevation);

            // Create a RenderableSeries and ensure DataSeries is set
            var k = new EllipsePointMarker
            {
                Fill = DataManager.Instance.GetRandomColor(),
                StrokeThickness = 0,
                Width = 3,
                Height = 3
            };
            var renderSeries = new XyScatterRenderableSeries
            {
                DataSeries = satellite_list[number],
                PointMarker = k,
            };
            point_marker[number].X1 = azimuth;
            point_marker[number].Y1 = elevation;
            point_marker[number].Point.Fill = new SolidColorBrush(k.Fill);
            point_marker[number].point_value.Text = number.ToString();
            // Add the new RenderableSeries
            polarChart.RenderableSeries.Add(renderSeries);

            Filtering_skyplot(num_of_satellite - 1, key, number);
        }
        private void UpdateSeries(short number, short elevation, short azimuth, int key)
        {
            point_marker[number].X1 = azimuth;
            point_marker[number].Y1 = elevation;

            for (int i = 0; exist_satellite[i] != -1; i++)
            {
                if (exist_satellite[i] == number)
                {
                    Filtering_skyplot(i, key, number);
                    break;
                }
            }
            if (satellite_list[number].XValues.Contains(azimuth) && satellite_list[number].YValues.Contains(elevation))
            {
                sate_del_count[number] = Constant.REMAIN_COUNT;
                return;
            }
            else
            {
                satellite_list[number].Append(azimuth, elevation);
                sate_del_count[number] = Constant.REMAIN_COUNT;
            }
        }
        public void OnNewData(Satellite[] satellite, int length, int key)
        {
            for (int i = 0; i < length; i++)
            {
                if (satellite[i].number == -1 || satellite[i].elevation == -1 || satellite[i].azimuth == -1 || satellite[i].SNR == -1) continue;
                using (polarChart.SuspendUpdates())
                {
                    if (satellite_list[satellite[i].number] != null)
                    {
                        UpdateSeries(satellite[i].number, satellite[i].elevation, satellite[i].azimuth, key);
                    }
                    else
                    {
                        AddSeries(satellite[i].number, satellite[i].elevation, satellite[i].azimuth, key);
                    }
                }
            }
            for (int i = 0; i < num_of_satellite; i++)
            {
                sate_del_count[exist_satellite[i]]--;
                if (sate_del_count[exist_satellite[i]] == 0) DeleteSeries(exist_satellite[i]);
            }
        }
        public void Redraw()
        {
            for (int i = 0; exist_satellite[i] != -1; i++)
            {
                Filtering_skyplot(i, select_key, exist_satellite[i]);
            }
        }
        private void Filtering_skyplot(int index, int key, short number)
        {
            if (key == 0)
            {
                polarChart.RenderableSeries[index].DataSeries = satellite_list[number];
                point_marker[number].Show();
            }
            else if (key == 1)
            {
                if (number > 0 && number < 33)
                {
                    polarChart.RenderableSeries[index].DataSeries = satellite_list[number];
                    point_marker[number].Show();
                }
                else
                {
                    polarChart.RenderableSeries[index].DataSeries = temp_data;
                    point_marker[number].Hide();
                }
            }
            else if (key == 2)
            {
                if (number > 32 && number < 55)
                {
                    polarChart.RenderableSeries[index].DataSeries = satellite_list[number];
                    point_marker[number].Show();
                }
                else
                {
                    polarChart.RenderableSeries[index].DataSeries = temp_data;
                    point_marker[number].Hide();
                }
            }
            else if (key == 3)
            {
                if (number > 64 && number < 97)
                {
                    polarChart.RenderableSeries[index].DataSeries = satellite_list[number];
                    point_marker[number].Show();
                }
                else
                {
                    polarChart.RenderableSeries[index].DataSeries = temp_data;
                    point_marker[number].Hide();
                }
            }
            else if (key == 4)
            {
                if (number > 192 && number < 201)
                {
                    polarChart.RenderableSeries[index].DataSeries = satellite_list[number];
                    point_marker[number].Show();
                }
                else
                {
                    polarChart.RenderableSeries[index].DataSeries = temp_data;
                    point_marker[number].Hide();
                }
            }
            else if (key == 5)
            {
                if (number > 200 && number < 236)
                {
                    polarChart.RenderableSeries[index].DataSeries = satellite_list[number];
                    point_marker[number].Show();
                }
                else
                {
                    polarChart.RenderableSeries[index].DataSeries = temp_data;
                    point_marker[number].Hide();
                }
            }
            else if (key == 6)
            {
                if ((number > 54 && number < 65) || (number > 96 && number < 193))
                {
                    polarChart.RenderableSeries[index].DataSeries = satellite_list[number];
                    point_marker[number].Show();
                }
                else
                {
                    polarChart.RenderableSeries[index].DataSeries = temp_data;
                    point_marker[number].Hide();
                }
            }
        }
    }
}
