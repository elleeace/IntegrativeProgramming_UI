using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace IntegrativeProgramming_UI
{
    internal class MainAction
    {
        public string Label { get; set; }
        public RoutedEventHandler ClickHandler { get; set; }
        public Brush HighlightColor { get; set; }
    }

}
