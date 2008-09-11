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
    public delegate void MenuIndexChangedHandler(object sender, SelectedMenuItemArgs e);

    public class SelectedMenuItemArgs : EventArgs
    {
        private readonly CoolMenuItem m_cmi;
        private readonly int m_index;

        public SelectedMenuItemArgs(CoolMenuItem menuItem, int menuIndex)
        {
            m_cmi = menuItem;
            m_index = menuIndex;
        }

        public CoolMenuItem Item
        {
            get { return m_cmi; }
        }

        public int Index
        {
            get { return m_index;  }   
        }
    }
}
