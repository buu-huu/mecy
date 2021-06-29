using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Shapes;

namespace MecyApplication
{
    /// <summary>
    /// AboutWindow with a little bit event handling
    /// </summary>
    public partial class AboutWindow : Window
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public AboutWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Event handler for clicking the hyperlink to GitHub repository
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Arguments</param>
        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
