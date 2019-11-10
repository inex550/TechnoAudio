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
    /// Логика взаимодействия для TimelineElement.xaml
    /// </summary>
    public partial class TimelineElement : UserControl
    {
        TmElementList parent;

        public double height;
        public double width;

        public string text;

        public TimelineElement(TmElementList parent, double width, double height, string text)
        {
            InitializeComponent();

            this.parent = parent;
            this.height = height;

            tmGridElement.Width = width;
            tmGridElement.Height = height;

            textOnElemtnt.Text = text;

            this.text = text;
        }
    }
}