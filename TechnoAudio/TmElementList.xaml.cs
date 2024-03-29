﻿using System;
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
using System.IO.Ports;
using System.Drawing;

namespace TechnoAudio
{
    /// <summary>
    /// Логика взаимодействия для TmElementList.xaml
    /// </summary>
    public partial class TmElementList : UserControl
    {
        Timeline parent;

        public TimelineElement tmElement;
        public bool isTmElement = false;

        public double width;
        public double height;

        public int startSeconds;
        public int endSeconds;

        public string data;

        bool isActive = false;

        public int Id { get; private set; }
        public static int Count { get; private set; }

        public TmElementList(Timeline parent, double width, double height)
        {
            InitializeComponent();

            this.parent = parent;

            this.width = width;
            this.height = height;
        }

        public void Setup(int startSeconds, int endSeconds)
        {
            border.Width = width;
            border.Height = height;
            
            this.startSeconds = startSeconds;
            this.endSeconds = endSeconds;
        }

        public void Active()
        {
            if (isTmElement && !isActive)
            {
                tmElement.smoke_grid.Opacity = 0.6;
                isActive = true;
            }
        }

        public void DontActive()
        {
            if (isTmElement && isActive)
            {
                tmElement.smoke_grid.Opacity = 0;
                isActive = false;
            }
        }

        public double ValueToSecond(double value) =>
            (value * (endSeconds - startSeconds) / width) + startSeconds;

        public double SecondToValue(double seconds) =>
            width * (seconds - startSeconds) / (endSeconds - startSeconds);

        public void AddElement(string imageSource, string data)
        {
            this.data = data;

            Count += 1;
            Id = Count;

            tmElement = new TimelineElement(this, width - 10, height - 10, imageSource);
            tmCanvas.Children.Add(tmElement);
            isTmElement = true;

            Canvas.SetLeft(tmElement, 4);
            Canvas.SetTop(tmElement, 4);
        }

        public void RemoveElement()
        {
            tmCanvas.Children.Remove(tmElement);
            isTmElement = false;

            if (Count > 0) Count -= 1;

            data = null;
        }
    }
}