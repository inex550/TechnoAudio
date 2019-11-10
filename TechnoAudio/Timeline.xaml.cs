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

namespace TechnoAudio
{
    /// <summary>
    /// Логика взаимодействия для Timeline.xaml
    /// </summary>
    public partial class Timeline : UserControl
    {
        public int width;
        public int height;

        public int startSeconds;
        public int endSeconds;

        public int tmCount;

        const int marginConst = 2;

        public int intervalCount;
        readonly List<TimelineMark> tmMarks = new List<TimelineMark>();
        public TmElementList[,] tmElementLists;

        public Timeline(int width, int height)
        {
            InitializeComponent();

            this.width = width;
            this.height = height;

            tmBorder.Width = width;
            tmBorder.Height = height;
        }

        public int endRow = 0;
        public int endColumn = 0;

        public void AddElement(string text, string textWihoutNum)
        {
            if (endRow < 3 && text != "BLANK")
                for (int i = 0; i <= endRow; i++)
                    if (i != endRow && endColumn >= 0)
                        if (textWihoutNum == tmElementLists[i, endColumn].textWihoutNum)
                        {
                            MessageBox.Show("You cannot repeat an item multiple times", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
                            return;
                        }

            if (endRow >= tmCount)
            {
                endColumn += 1;
                TimelineChecker.endSecond += 1;
                endRow = 0;
            }
            if (endColumn >= intervalCount)
            {
                MessageBox.Show("Timeline is full", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            tmElementLists[endRow, endColumn].AddElement(text, textWihoutNum);

            endRow += 1;
        }

        public void RemoveAllElements()
        {
            if (TmElementList.Count > 0)
            {
                foreach (TmElementList tmList in tmElementLists)
                    tmList.RemoveElement();
                endRow = 0;
                endColumn = 0;
                TimelineChecker.endSecond = 0;
            }
            else MessageBox.Show("timeline so empty", "Message", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void Setup(int startSeconds, int endSeconds, int space, int tmCount)
        {
            this.startSeconds = startSeconds;
            this.endSeconds = endSeconds;

            this.tmCount = tmCount;

            TimelineMark tmStart = new TimelineMark(startSeconds);
            tmMarks.Add(tmStart);
            tmCanvas.Children.Add(tmStart);

            this.intervalCount = endSeconds - startSeconds;
            for (int i = 1; i <= intervalCount - 1; i++)
            {
                TimelineMark tm = new TimelineMark(startSeconds + i);
                tmMarks.Add(tm);
                tmCanvas.Children.Add(tm);
            }

            TimelineMark tmEnd = new TimelineMark(endSeconds);
            tmMarks.Add(tmEnd);
            tmCanvas.Children.Add(tmEnd);

            for (int i = 0; i < tmMarks.Count; i++)
            {
                Canvas.SetLeft(tmMarks[i], space * i + marginConst);
                Canvas.SetTop(tmMarks[i], marginConst);
            }

            /*
            TmElementList tmElementList = new TmElementList(this, Canvas.GetLeft(tmEnd) - 1, (height / tmCount) - 10);
            tmElementList.Setup(startSeconds, endSeconds);

            Canvas.SetLeft(tmElementList, 2);
            Canvas.SetTop(tmElementList, fromTop);

            fromTop += (height / tmCount) - 10;

            tmElementLists.Add(tmElementList);
            tmCanvas.Children.Add(tmElementList);
            */

            Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            Arrange(new Rect(0, 0, width, height));

            tmElementLists = new TmElementList[tmCount, intervalCount];

            double fromTop = (int)tmStart.ActualHeight + marginConst;
            double fromLeft = Canvas.GetLeft(tmStart);


            for (int i = 0; i < tmCount; i++)
            {
                for (int j = 0; j < intervalCount; j++)
                {
                    tmElementLists[i, j] = new TmElementList(this, width / intervalCount - 5.8, height / tmCount);
                    tmElementLists[i, j].Setup(startSeconds, endSeconds);

                    tmCanvas.Children.Add(tmElementLists[i, j]);

                    Canvas.SetTop(tmElementLists[i, j], fromTop);
                    Canvas.SetLeft(tmElementLists[i, j], fromLeft);

                    fromLeft += width / intervalCount - 5.8;
                }
                fromTop += height / tmCount - 10;
                fromLeft = marginConst;
            }
        }
    }
}