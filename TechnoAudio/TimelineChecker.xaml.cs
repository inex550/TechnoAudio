using System;
using System.Threading;
using System.Threading.Tasks;
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
        readonly MainWindow mainWindow;

        public static int endSecond = 0;

        public TimelineChecker(Timeline tm, double width, MainWindow mainWindow)
        {
            InitializeComponent();

            this.mainWindow = mainWindow;

            this.width = width;

            timerSlider.Minimum = tm.startSeconds;
            timerSlider.Maximum = tm.endSeconds;
            timerSlider.Value = tm.startSeconds;

            timerSlider.Width = width;
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            timerSeconds = timerSlider.Minimum;

            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(TimerTick);
            timer.Interval = TimeSpan.FromSeconds(0.01);
        }

        private void TimerTick(object sender, EventArgs e)
        {
            if (timerSeconds >= endSecond + 1)
            {
                timerSlider.Value = timerSlider.Minimum;
                timerSeconds = timerSlider.Minimum;

                timer.Stop();

                mainWindow.Stop();
            }

            timerSlider.Value = timerSeconds;
            timerSeconds += 0.017;
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

    }
}
