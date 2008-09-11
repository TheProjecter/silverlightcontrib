using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace SilverlightContrib.Controls
{
    public class CoolMenuItem
    {
        public string Text { get; set; }
        public FrameworkElement Content {get;set; }
        public object Tag { get; set; }
    }
}
