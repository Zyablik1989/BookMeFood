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
using BookMyFoodWCF;
using Item = BookMyFood.ServiceChat.Item;

namespace BookMyFood.WPF.ListItems
{
    /// <summary>
    /// Interaction logic for OrderItem.xaml
    /// </summary>
    public partial class OrderItem : UserControl
    {
        public OrderItem(BookMyFood.ServiceChat.Item item, bool QuantityChangable=false)
        {
            InitializeComponent();
            //rtbItemName.AppendText();
            //rtbItemName.Document.Blocks.Clear();
            //rtbItemName.Document.Blocks.r;
            //var a = rtbItemName.Document.Blocks.FirstBlock;
            //imgMinus.Opacity=
            CurrentItem = item;
            btPlus.Opacity = QuantityChangable ? 100 : 0;
            btMinus.Opacity = QuantityChangable ? 100 : 0;

            btPlus.IsEnabled = QuantityChangable;
            btMinus.IsEnabled = QuantityChangable; 
            //((Run) ((Paragraph) rtbItemName.Document.Blocks.FirstBlock).Inlines.LastInline).Text = item.Name;
            tbbItemName.Text = item.Name;
            //(x => ((x as Paragraph).Inlines.LastInline as Run).Text = item.Name);
            //((rtbItemName.Document.Blocks.FirstBlock).Inlines.LastInline as Run).Text;
            //rtbItemName.Document.Blocks.Add(new Paragraph(new Run("Text")));
            //tbItemName.Text =
            //"                              |" +
            //item.Name;
            //tbItemQuantity.Text = item.Quantity.ToString();
            
            
            
                
            if (ClientMaintaining.CurrentDiscount != 0)
            {

                decimal pricedisc = (item.Price - ((item.Price / 100) * (decimal)ClientMaintaining.CurrentDiscount));
                tbSum.Text = item.Price.ToString("F1") + " - " + (ClientMaintaining.CurrentDiscount / 100).ToString("P", CultureInfo.InvariantCulture)
                           +  ((item.Quantity > 1) ?
                    " x " + item.Quantity : "")
                                   + " = " + (item.Quantity * pricedisc).ToString("F2");
            }
            else
            {
                tbSum.Text =
                    item.Price.ToString("F1") +
                ((item.Quantity > 1) ? 
                    " x " + item.Quantity : "")
                          + " = " + (item.Quantity * item.Price).ToString("F2");
            }


        }

        private Item CurrentItem;


        private void Plus(object sender, RoutedEventArgs e)
        {
            if (!ClientMaintaining.ClientReadiness)
            {
                Model.Item first = null;
                foreach (var x in ServerUser.Current.Order.Items)
                {
                    if (x.ID == CurrentItem.ID)
                    {
                        first = x;
                        break;
                    }
                }

                if (first != null)
                    first
                        .Quantity += 1;
            }
        }

        private void Minus(object sender, RoutedEventArgs e)
        {
            if (!ClientMaintaining.ClientReadiness)
            {
                var q = ServerUser.Current.Order.Items.FirstOrDefault(x => x.ID == CurrentItem.ID);
                if (q != null)
                {

                    if (q.Quantity > 1)
                    {
                        q.Quantity -= 1;
                    }
                    else
                    {
                        ServerUser.Current.Order.Items.Remove(q);
                    }
                }
            }
        }




        //private void imgPlus_MouseDown(object sender, MouseButtonEventArgs e)
        //{

        //    ServerUser.Current.Order.Items.FirstOrDefault(x => x.ID == CurrentItem.ID)
        //        .Quantity += 1;
        //}

        //private void imgMinus_MouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    var q = ServerUser.Current.Order.Items.FirstOrDefault(x => x.ID == CurrentItem.ID);

        //    if (q.Quantity > 1)
        //    {
        //        q.Quantity -= 1;
        //    }
        //    else
        //    {
        //        ServerUser.Current.Order.Items.Remove(q);
        //    }


        //}





        //private void imgPlus_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    ServerUser.Current.Order.Items.FirstOrDefault(x => x.ID == CurrentItem.ID).Price +=
        //        CurrentItem.Price;
        //    ServerUser.Current.Order.Items.FirstOrDefault(x => x.ID == CurrentItem.ID)
        //        .Quantity += 1;
        //}


       
    }
}
