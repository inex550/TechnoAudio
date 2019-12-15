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

            timeline = new Timeline(1900, 500);
            timeline.Setup(0, 6, 310, 3);

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

                    /*Thread readPortThread = new Thread(() =>
                    {
                        while (true)
                        {
                            string readedData = musicPort.ReadLine();

                            if (readedData == "ok play\r")
                                MessageBox.Show(readedData, "Message", MessageBoxButton.OK, MessageBoxImage.Information);
                            else if (readedData == "ok interrupt\r")
                                MessageBox.Show(readedData, "Message", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }) { IsBackground = true };
                    readPortThread.Start();*/
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

        List<Button> falsedButtons = new List<Button>();
        void AddElement(string text, int songNum, string data, Button clickedButton, bool okRepeat = false)
        {
            string src = $@"{Environment.CurrentDirectory}\Media\song{songNum}.mp3";
            if (File.Exists(src))
            {
                player.Source = new Uri(src, UriKind.Absolute);
                player.Play();
                if (okRepeat) timeline.AddElement(text, text, data, clickedButton);
                else timeline.AddElement(text, StrWithoutNumbers(text), data, clickedButton);
            }
            else MessageBox.Show($"Ringtone song{songNum} in folder Media does not exist", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        //Добавление элементов
        private void Congas1_Click(object sender, RoutedEventArgs e) =>
            AddElement(congas1.Content.ToString(), 1, "co1", congas1, true);

        private void Congas2_Click(object sender, RoutedEventArgs e) =>
            AddElement(congas2.Content.ToString(), 2, "co2", congas2, true);

        private void Cymbals1_Click(object sender, RoutedEventArgs e) =>
            AddElement(cymbals1.Content.ToString(), 3, "cy1", cymbals1, true);

        private void Cymbals2_Click(object sender, RoutedEventArgs e) =>
            AddElement(cymbals2.Content.ToString(), 4, "cy2", cymbals2, true);

        private void BassDrum_Click(object sender, RoutedEventArgs e) =>
            AddElement(bassDrum.Content.ToString(), 5, "ba", bassDrum);

        private void Blank_Click(object sender, RoutedEventArgs e) =>
            AddElement(Blank.Content.ToString(), 6, "bl", Blank);

        private void Bongos1_Click(object sender, RoutedEventArgs e) =>
            AddElement(bongos1.Content.ToString(), 7, "bo1", bongos1, true);

        private void Bongos2_Click(object sender, RoutedEventArgs e) =>
            AddElement(bongos2.Content.ToString(), 8, "bo2", bongos2, true);

        private void Xylophone1_Click(object sender, RoutedEventArgs e) =>
            AddElement(xylophone1.Content.ToString(), 9, "xy1", xylophone1);

        private void Xylophone2_Click(object sender, RoutedEventArgs e) =>
            AddElement(xylophone2.Content.ToString(), 10, "xy2", xylophone2);

        private void Xylophone3_Click(object sender, RoutedEventArgs e) =>
            AddElement(xylophone3.Content.ToString(), 11, "xy3", xylophone3);

        private void Xylophone4_Click(object sender, RoutedEventArgs e) =>
            AddElement(xylophone4.Content.ToString(), 12, "xy4", xylophone4);

        private void Flute1_Click(object sender, RoutedEventArgs e) =>
            AddElement(flute1.Content.ToString(), 13, "fl1", flute1);

        private void Flute2_Click(object sender, RoutedEventArgs e) =>
            AddElement(flute2.Content.ToString(), 14, "fl2", flute2);

        private void Flute3_Click(object sender, RoutedEventArgs e) =>
            AddElement(flute3.Content.ToString(), 15, "fl3", flute3);

        private void Flute4_Click(object sender, RoutedEventArgs e) =>
            AddElement(flute4.Content.ToString(), 16, "fl4", flute4);

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
