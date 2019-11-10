using System;
using System.Windows.Controls;

using System.Windows.Threading;

namespace TechnoAudio
{
    /// <summary>
    /// Логика взаимодействия для TimelineChecker.xaml
    /// </summary>
    public partial class TimelineChecker : UserControl
    {
        public double timerSeconds;
        public double width;

        DispatcherTimer timer;

        MainWindow mainWindow;

        public TimelineChecker(Timeline tm, double width, MainWindow mainWindow)
        {
            InitializeComponent();

            this.mainWindow = mainWindow;

            this.width = width;

            timerSlider.Minimum = tm.startSeconds;
            timerSlider.Maximum = tm.endSeconds;
            timerSlider.Value = tm.startSeconds;

            timerSlider.Width = width - 35;
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            timerSeconds = timerSlider.Minimum;

            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(TimerTick);
            timer.Interval = TimeSpan.FromMilliseconds(10);
        }

        private void TimerTick(object sender, EventArgs e)
        {
            if (timerSeconds >= timerSlider.Maximum)
            {
                timerSlider.Value = timerSlider.Minimum;
                timerSeconds = timerSlider.Minimum;

                timer.Stop();
            }

            setTime((int)timerSeconds);
            timerSlider.Value = timerSeconds;

            timerSeconds += 0.01;
        }

        public void Play()
        {
            if (!timer.IsEnabled) timer.Start();
        }

        public void Reset()
        {
            timer.Stop();

            timerSlider.Value = timerSlider.Minimum;
            timerSeconds = timerSlider.Minimum;
        }

        public void setTime(int seconds)
        {
            string m = (seconds / 60).ToString();
            if (m.Length < 2)
                m = "0" + m;
            string s = (seconds % 60).ToString();
            if (s.Length < 2)
                s = "0" + s;

            timerText.Text = m + ":" + s;

        }

    }
}
