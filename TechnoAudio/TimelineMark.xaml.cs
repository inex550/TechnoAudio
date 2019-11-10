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
    /// Логика взаимодействия для TimelineMark.xaml
    /// </summary>
    public partial class TimelineMark : UserControl
    {
        public TimelineMark(int seconds)
        {
            InitializeComponent();

            string m = (seconds / 60).ToString();
            if (m.Length < 2)
                m = "0" + m;
            string s = (seconds % 60).ToString();
            if (s.Length < 2)
                s = "0" + s;
            text.Text = m + ":" + s;
        }
    }
}
