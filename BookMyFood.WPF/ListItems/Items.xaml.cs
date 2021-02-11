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
using System.Windows.Navigation;
using System.Windows.Shapes;
using BookMyFood.ClientFuncion;
using BookMyFood.Model;

namespace BookMyFood.WPF.ListItems
{
    /// <summary>
    /// Interaction logic for Items.xaml
    /// </summary>
    public partial class Items : UserControl
    {
        public Items(Item item)
        {
            InitializeComponent();
            PickedItem = item;
            tbName.Text = item.Name;
            tbPrice.Text = item.Price.ToString("F2");
            if (ClientMaintaining.CurrentDiscount != 0)
            {

                decimal pricedisc = (item.Price-((item.Price / 100) * (decimal)ClientMaintaining.CurrentDiscount));
                tbPrice.Text += " - " + (ClientMaintaining.CurrentDiscount/100).ToString("P",CultureInfo.InvariantCulture) + " = " +
                                pricedisc.ToString("F2");
            }
            tbDesc.Text = item.Description;
        }
        public Item PickedItem { get; set; }
    }
}
