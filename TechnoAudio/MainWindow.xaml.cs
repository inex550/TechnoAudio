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
using System.Threading;
using System.IO;
using System.Drawing;

namespace TechnoAudio
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        readonly Timeline timeline;
        readonly TimelineChecker tmChecker;
        readonly MediaElement player;

        SerialPort musicPort;

        public MainWindow()
        {
            InitializeComponent();

            player = new MediaElement
            {
                LoadedBehavior = MediaState.Manual,
                UnloadedBehavior = MediaState.Manual
            };

            timeline = new Timeline(1800, 524);
            timeline.Setup(0, 6, 3);

            tmChecker = new TimelineChecker(timeline, timeline.width, this)
            {
                Margin = new Thickness(10, 0, 10, 5)
            };

            timelineList.Children.Add(tmChecker);
            timelineList.Children.Add(timeline);

            tmChecker.timerSlider.ValueChanged += (o, e) =>
            {
                if (tmChecker.timerSeconds != 0 && isPlay)
                {
                    for (int i = 0; i < timeline.tmCount; i++)
                    {
                        if (tmChecker.timerSeconds >= timeline.intervalCount)
                        {
                            timeline.tmElementLists[i, timeline.intervalCount - 1].DontActive();
                            continue;
                        }
                        timeline.tmElementLists[i, (int)tmChecker.timerSeconds].Active();
                        if ((int)tmChecker.timerSeconds > 0) timeline.tmElementLists[i, (int)tmChecker.timerSeconds - 1].DontActive();
                    }
                }
                else if (!isPlay)
                    for (int i = 0; i < timeline.tmCount; i++)
                        timeline.tmElementLists[i, (int)tmChecker.timerSeconds].DontActive();
            };
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            musicPort = new SerialPort();

            if (!File.Exists("portOptions.txt"))
            {
                MessageBox.Show("File portOptions.txt is not exist", "Message", MessageBoxButton.OK, MessageBoxImage.Error);

                ComPortOptionsWindow comPortOptionsWindow = new ComPortOptionsWindow();
                comPortOptionsWindow.Show();


                this.Close();
            }
            else try
                {
                    using (StreamReader reader = new StreamReader("portOptions.txt", Encoding.Default))
                    {
                        musicPort.PortName = reader.ReadLine();
                        musicPort.BaudRate = int.Parse(reader.ReadLine());
                    }

                    musicPort.Open();

                    //Thread readPortThread = new Thread(() =>
                    //{
                    //    while (true)
                    //    {
                    //        string readedData = musicPort.ReadLine();

                    //        if (readedData == "ok play\r")
                    //            MessageBox.Show(readedData, "Message", MessageBoxButton.OK, MessageBoxImage.Information);
                    //        else if (readedData == "ok interrupt\r")
                    //            MessageBox.Show(readedData, "Message", MessageBoxButton.OK, MessageBoxImage.Information);
                    //    }
                    //}) { IsBackground = true };
                    //readPortThread.Start();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, ex.Source, MessageBoxButton.OK, MessageBoxImage.Error);

                    ComPortOptionsWindow comPortOptionsWindow = new ComPortOptionsWindow();
                    comPortOptionsWindow.Show();

                    this.Close();
                }
        }

        bool isPlay = false;
        private void PlayPauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isPlay)
            {
                if (TimelineChecker.endSecond == 0 && timeline.endRow == 0)
                {
                    string src = $@"{Environment.CurrentDirectory}\Media\song0.mp3";
                    if (File.Exists(src))
                    {
                        player.Source = new Uri(src, UriKind.Absolute);
                        player.Play();
                    }
                    else MessageBox.Show($"Ringtone song0 in folder Media does not exist", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                string data = timeline.GetForSendPlayData();
                musicPort.WriteLine(data);

                playPauseButton.Content = "Stop";
                tmChecker.Play();
                isPlay = true;
            }
            else Stop();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            Stop();
            timeline.RemoveAllElements();
        }

        public void Stop()
        {
            if (isPlay)
            {
                musicPort.WriteLine("interrupt");

                isPlay = false;
                playPauseButton.Content = "Play";
                tmChecker.Reset();
            }
        }

        string StrWithoutNumbers(string text)
        {
            string str = "";

            for (int i = 0; i < text.Length; i++)
                if (!char.IsDigit(text[i])) str += text[i];
            
            return str;
        }

        void AddElement(string imageSource, int songNum, string data, Button clickedButton, Grid forDisableGrid, bool isBlank = false)
        {
            string src = $@"{Environment.CurrentDirectory}\Media\song{songNum}.mp3";
            if (File.Exists(src))
            {
                player.Source = new Uri(src, UriKind.Absolute);
                player.Play();
                timeline.AddElement(imageSource, data, clickedButton, forDisableGrid, isBlank);
            }
            else MessageBox.Show($"Ringtone song{songNum} in folder Media does not exist", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        //Добавление элементов
        private void Congas1_Click(object sender, RoutedEventArgs e) =>
            AddElement("conga1.png", 1, "co1", congas1, congas1_grid);

        private void Congas2_Click(object sender, RoutedEventArgs e) =>
            AddElement("conga2.png", 2, "co2", congas2, congas2_grid);

        private void Cymbals1_Click(object sender, RoutedEventArgs e) =>
            AddElement("cymbal1.png", 3, "cy1", cymbals1, cymbals1_grid);

        private void Cymbals2_Click(object sender, RoutedEventArgs e) =>
            AddElement("cymbal2.png", 4, "cy2", cymbals2, cymbals2_grid);

        private void BassDrum_Click(object sender, RoutedEventArgs e) =>
            AddElement("bass drum.png", 5, "ba", bassDrum, bassDrum_grid);

        private void Blank_Click(object sender, RoutedEventArgs e) =>
            AddElement("blank.png", 6, "bl", Blank, null, true);

        private void Bongos1_Click(object sender, RoutedEventArgs e) =>
            AddElement("bongos1.png", 7, "bo1", bongos1, bongos1_grid);

        private void Bongos2_Click(object sender, RoutedEventArgs e) =>
            AddElement("bongos2.png", 8, "bo2", bongos2, bongos2_grid);

        private void Xylophone1_Click(object sender, RoutedEventArgs e) =>
            AddElement("xylophone1.png", 9, "xy1", xylophone1, xylophone1_grid);

        private void Xylophone2_Click(object sender, RoutedEventArgs e) =>
            AddElement("xylophone2.png", 10, "xy2", xylophone2, xylophone2_grid);

        private void Xylophone3_Click(object sender, RoutedEventArgs e) =>
            AddElement("xylophone3.png", 11, "xy3", xylophone3, xylophone3_grid);

        private void Xylophone4_Click(object sender, RoutedEventArgs e) =>
            AddElement("xylophone4.png", 12, "xy4", xylophone4, xylophone4_grid);

        private void Flute1_Click(object sender, RoutedEventArgs e) =>
            AddElement("flute1.png", 13, "fl1", flute1, flute1_grid);

        private void Flute2_Click(object sender, RoutedEventArgs e) =>
            AddElement("flute2.png", 14, "fl2", flute2, flute2_grid);

        private void Flute3_Click(object sender, RoutedEventArgs e) =>
            AddElement("flute3.png", 15, "fl3", flute3, flute3_grid);

        private void Flute4_Click(object sender, RoutedEventArgs e) =>
            AddElement("flute4.png", 16, "fl4", flute4, flute4_grid);

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.P)
            {
                if (musicPort.IsOpen) musicPort.Close();

                ComPortOptionsWindow comPortOptionsWindow = new ComPortOptionsWindow();
                comPortOptionsWindow.Show();

                this.Close();
            }
        }
    }
}
