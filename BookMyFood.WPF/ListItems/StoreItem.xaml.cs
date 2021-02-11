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
using BookMyFood.Model;

namespace BookMyFood.WPF.ListItems
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class StoreItem : UserControl
    {
        public StoreItem(Deliverer deliverer)
        {
            InitializeComponent();
            StoreDeliverer = deliverer;
            tbStoreName.Text = deliverer.Name;
            tbStoreDesc.Text = deliverer.Description;/*.Substring(0, 100)*/
        }

        public Deliverer StoreDeliverer { get; set; }


        //private void BtStoreHomeSite_Click(object sender, RoutedEventArgs e)
        //{

        //}

        //private void BtStorePick_Click(object sender, RoutedEventArgs e)
        //{
            
        //}

        //public ()
    }
}
