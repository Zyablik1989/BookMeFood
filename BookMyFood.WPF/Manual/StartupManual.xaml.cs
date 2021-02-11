using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace BookMyFood.WPF.Manual
{
    /// <summary>
    /// Interaction logic for StartupManual.xaml
    /// </summary>
    public partial class StartupManual : Window
    {
        public static bool ok = false;
        public StartupManual()
        {
            InitializeComponent();

            try
            {
                if (Equals(CultureInfo.CurrentCulture, CultureInfo.GetCultureInfo("ru-RU")))
                {
                    manualpng.Source = new BitmapImage(new Uri("Resources/Manual-RU.png", UriKind.Relative)); ;
                }
            }
            catch (Exception)
            {
                
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
