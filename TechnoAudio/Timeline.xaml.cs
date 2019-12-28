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
        public int intervalCount;

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

        List<Button> disabledButtons = new List<Button>();
        List<Grid> disabledGrids = new List<Grid>();
        public void AddElement(string imageSource, string data, Button[] btns, Grid[] forGreyGrids, bool isBlank)
        {

            if (endRow >= tmCount)
            {
                endColumn += 1;
                TimelineChecker.endSecond += 1;
                endRow = 0;
            }

            if (endRow >= tmCount - 1)
                while (disabledButtons.Count != 0)
                {
                    disabledButtons[0].IsEnabled = true;
                    disabledGrids[0].Opacity = 0;
                    disabledButtons.RemoveAt(0);
                    disabledGrids.RemoveAt(0);
                }

            if (endColumn >= intervalCount) return;

            tmElementLists[endRow, endColumn].AddElement(imageSource, data);
            if (endRow < tmCount - 1 && !isBlank)
            {
                for (int i = 0; i < btns.Length; i++)
                {
                    btns[i].IsEnabled = false;
                    forGreyGrids[i].Opacity = 0.6;
                    disabledButtons.Add(btns[i]);
                    disabledGrids.Add(forGreyGrids[i]);
                }
            }

            endRow += 1;
        }

        public void RemoveAllElements()
        {
            if (TmElementList.Count > 0)
            {
                for (int i = 0; i < tmCount; i++)
                    for (int j = 0; j < intervalCount; j++)
                        tmElementLists[i, j].RemoveElement();

                endRow = 0;
                endColumn = 0;
                TimelineChecker.endSecond = 0;

                while (disabledButtons.Count != 0)
                {
                    disabledButtons[0].IsEnabled = true;
                    disabledGrids[0].Opacity = 0;
                    disabledButtons.RemoveAt(0);
                    disabledGrids.RemoveAt(0);
                }
            }
        }

        public string GetForSendPlayData()
        {
            string forReturnStrData = $"play";
            
            for (int i = 0; i < intervalCount; i++)
            {
                if (tmElementLists[0, i].data != null) forReturnStrData += $" {i + 1} ";

                for (int j = 0; j < tmCount; j++)
                {
                    if (tmElementLists[j, i].data != null)
                    {
                        forReturnStrData += $"{tmElementLists[j, i].data}";
                        try
                        {
                            if (GetNextElement(j, i).data != null)
                                forReturnStrData += ",";
                        }
                        catch (NullReferenceException) { }
                    }
                    else break;
                }
            }

            return forReturnStrData;
        }

        TmElementList GetNextElement(int row, int column)
        {
            if (row == tmCount - 1)
            {
                if (column == intervalCount - 1) return null;
                column += 1;
                row = 0;
            }
            else row += 1;

            return tmElementLists[row, column];
        }

        public void Setup(int startSeconds, int endSeconds, int tmCount)
        {
            this.startSeconds = startSeconds;
            this.endSeconds = endSeconds;

            this.tmCount = tmCount;

            intervalCount = 6;

            Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            Arrange(new Rect(0, 0, width, height));

            tmElementLists = new TmElementList[tmCount, intervalCount];

            double fromTop = 0;
            double fromLeft = 0;

            for (int i = 0; i < tmCount; i++)
            {
                for (int j = 0; j < intervalCount; j++)
                {
                    tmElementLists[i, j] = new TmElementList(this, width / intervalCount, height / tmCount);
                    tmElementLists[i, j].Setup(startSeconds, endSeconds);

                    tmCanvas.Children.Add(tmElementLists[i, j]);

                    Canvas.SetTop(tmElementLists[i, j], fromTop);
                    Canvas.SetLeft(tmElementLists[i, j], fromLeft);

                    fromLeft += width / intervalCount;
                }
                fromTop += height / tmCount;
                fromLeft = 0;
            }
        }
    }
}