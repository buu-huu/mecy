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

namespace MecyInformation
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Mesocyclone activeMeso = null;
        List<Mesocyclone> mesos;

        public MainWindow()
        {
            InitializeComponent();

            mesos = new List<Mesocyclone>();
            lvMesos.ItemsSource = mesos;

            mesos = XMLParser.ParseMesos(@"C:\Users\buuhuu\source\repos\MecyInformation\MecyInformation\XMLFiles\meso_20210621_0220.xml");
            lvMesos.ItemsSource = mesos;
            gridDetails.DataContext = activeMeso;
        }

        private void UpdateDetailsPanel()
        {
            activeMeso = (Mesocyclone)lvMesos.SelectedItem;
            gridDetails.DataContext = activeMeso;
        }

        private void lvMesos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateDetailsPanel();
        }
    }
}
