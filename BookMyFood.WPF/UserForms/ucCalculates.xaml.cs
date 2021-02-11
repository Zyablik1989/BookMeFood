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
using BookMyFood.ClientUI;
using BookMyFood.ServiceChat;

namespace BookMyFood.WPF.UserForms
{
    /// <summary>
    /// Interaction logic for UserControlBase.xaml
    /// </summary>
    public partial class ucCalculates : UserControl
    {
        public ucCalculates()
        {
            InitializeComponent();
            FieldsInitialisation();
            FillUsersInformationFields();
            FillItemsInformationFields();
        }

        private void FieldsInitialisation()
        {
            btCopyItems.Content = ClientUIPresenter.GetString("COPYTHIS");
            btCopyUsers.Content = ClientUIPresenter.GetString("COPYTHIS");
            lDesigned.Text = ClientUIPresenter.GetString("DesignedandProgrammedby");
            ;

        }

        private void FillItemsInformationFields()
        {
            FlowDocument myFlowDoc = new FlowDocument();
            var users = ClientFuncion.ClientMaintaining.FinalUsersStatus;

            List<BookMyFood.ServiceChat.Item> itemsUnfiltered = new List<Item>();
            foreach (var user in users)
            {
                if (user.Order != null && user.Order.Items.Count > 0)
                {
                    itemsUnfiltered.AddRange(user.Order.Items);
                }
            }

            

            decimal sumDisc = 0;
                Paragraph Para = new Paragraph();
            foreach (var id in itemsUnfiltered.OrderBy(x=>x.ID).Select(x=>x.ID).Distinct())
            {
                var item = itemsUnfiltered.FirstOrDefault(x => x.ID == id);
                if (item != null) 
                {
                    int quantity = 0;
                    quantity = itemsUnfiltered.Where(x=>x.ID==id).Select(x => x.Quantity).Sum();
                    //Para.Inlines.Add(
                    //    new Bold((new Run($"[{id:0000}] \"{item.Name}\""))))
                    //;
                    //Para.Inlines.Add(
                    //    ((new Run($"{quantity} x {item.Price:F2} = {(item.Price * quantity):F2}")))
                    //);

                    if (ClientMaintaining.CurrentDiscount != 0)
                    {
                        decimal pricedisc = (item.Price - ((item.Price / 100) * (decimal)ClientMaintaining.CurrentDiscount));
                        string percent = (ClientMaintaining.CurrentDiscount / 100).ToString("P",
                            CultureInfo.InvariantCulture);
                        sumDisc += pricedisc * quantity;
  
                          Para.Inlines.Add(
                            (new Run(
                                $"[{id:0000}] ")));

                        Para.Inlines.Add(new Bold(new Run(item.Name)));
                    Para.Inlines.Add((new Run(
                        $" {item.Price:F2} - {percent} ")));

                        Para.Inlines.Add(new Bold(new Run($"x {quantity}")));

                        Para.Inlines.Add((new Run(
                                $" = {(pricedisc * quantity):F2}")
                            ));
                    }
                    else
                    {
                        Para.Inlines.Add(
                            (new Run(
                                $"[{id:0000}] ")));

                        Para.Inlines.Add(new Bold(new Run(item.Name)));
                        Para.Inlines.Add((new Run(
                            $" {item.Price:F2} x ")));

                        Para.Inlines.Add(new Bold(new Run($"{quantity}")));

                        Para.Inlines.Add((new Run(
                                $" = {(item.Price * quantity):F2}")
                            ));
                    }

                    //    ;
                    //Para.Inlines.Add(
                    //    ((new Run($"{quantity} x {item.Price:F2} = {(item.Price * quantity):F2}")))
                    //);

                    Para.Inlines.Add(
                        new LineBreak()
                    );

                }
                //for
            }

      

                Para.Inlines.Add(
                new Underline((new Run(ClientUIPresenter.GetString("Sum")
                                       + $" = [{itemsUnfiltered.Select(x => x.Price * x.Quantity).Sum():F2}]")))
            );

            if (sumDisc > 0)
            {
                Para.Inlines.Add(
                    new LineBreak()
                );
                Para.Inlines.Add(
                    new Underline((new Run(ClientUIPresenter.GetString("Sumdiscounted")
                                           + $" = [{sumDisc:F2}]")))
                );
            }
            //itemsUnfiltered.AddRange( users.Where(x => x.Order != null && x.Order.Items.Count > 0).Select(x => x.Order.Items));
            //var distinctNames = itemsNames.Select(x=>x.)

            myFlowDoc.Blocks.Add(Para);
            rtbOrderItemsInfo.Document = myFlowDoc;
        }

        private void FillUsersInformationFields()
        {
                FlowDocument myFlowDoc = new FlowDocument();
            
            var users = ClientFuncion.ClientMaintaining.FinalUsersStatus;
            decimal totalSum = 0;
            decimal totalSumDisc = 0;
                Paragraph Para = new Paragraph();
            foreach (var user in users.OrderBy(x=>x.ID))
            {
                decimal sumdisc = 0;
                Para.Inlines.Add(
                    new Bold((new Run($"[{user.Name}]")))
                    );
                Para.Inlines.Add(
                    new LineBreak()
                );
                if (user.Order!=null && user.Order.Items.Count>0)
                {
                    foreach (var item in user.Order.Items)
                    {
                        Para.Inlines.Add(
                            new Run($"— [{item.ID:0000}] "));

                        Para.Inlines.Add((new Run($"\"{item.Name}\". {item.Price}")));
                        if (ClientMaintaining.CurrentDiscount != 0)
                        {
                            decimal pricedisc = (item.Price - ((item.Price / 100) * (decimal)ClientMaintaining.CurrentDiscount));
                            string percent = (ClientMaintaining.CurrentDiscount / 100).ToString("P",
                                CultureInfo.InvariantCulture);
                            Para.Inlines.Add(
                                new Run($" - {percent:F2}"));
                            sumdisc += (pricedisc * item.Quantity);
                        }

                        

                        Para.Inlines.Add(
                            new Run($" x {item.Quantity}"));
                        if (ClientMaintaining.CurrentDiscount != 0)
                        {
                            decimal pricedisc = (item.Price -
                                                 ((item.Price / 100) * (decimal) ClientMaintaining.CurrentDiscount));
                            Para.Inlines.Add((new Run(
                                    $"  = {(pricedisc * item.Quantity):F2}"))
                            );
                        }
                        else
                        {
                            Para.Inlines.Add((new Run(
                                    $"  = {(item.Price * item.Quantity):F2}"))
                            );
                        }

                        Para.Inlines.Add( 
                            new LineBreak()
                        );
                    }
                    totalSumDisc += sumdisc;

                    decimal sum = user.Order.Items.Select(x => x.Price * x.Quantity).Sum();
                    totalSum += sum;
                    Para.Inlines.Add(
                        new Underline((new Run(ClientUIPresenter.GetString("Sum")
                                               + $" = [{sum:F2}]")))
                    );
                    Para.Inlines.Add(
                        new LineBreak()
                    );
                    if (sumdisc > 0)
                    {
                        Para.Inlines.Add(
                            new Underline((new Run(ClientUIPresenter.GetString("Sumdiscounted")
                                                   + $" = [{sumdisc:F2}]")))
                        );
                        Para.Inlines.Add(
                            new LineBreak()
                        );
                    }
                }
                //Bold myBold = new Bold(new Run($"[{user.Name}]"));

                //Para.Inlines.Add(
                //    new Separator()
                //);
                Para.Inlines.Add(
                    new LineBreak()
                );

                //if (users.Count(x => x.Order != null) != 0 && user.Order.Items.Count > 0)
                //{
                //decimal sum = 0;
                //foreach (var user in users.Where(x => x.Order != null).Where(x => x.Order.Items.Any()))
                //{
                //    sum+= user.Order.Items.Select(x=>x.Price*x.Quantity)
                //}
                //}

               

                //myFlowDoc.Blocks.Add(Para);



            }

            if (totalSum > 0)
            {
                Para.Inlines.Add(
                        new Underline(
                            new Bold(
                                new Run(ClientUIPresenter.GetString("TOTALSUM")
                                        + $" = [{totalSum:F2}]"
                                ))))
                    ;
                Para.Inlines.Add(
                    new LineBreak()
                );
            }

            if (totalSumDisc > 0)
            {
                Para.Inlines.Add(
                    new Underline((new Run(ClientUIPresenter.GetString("TOTALSUMDISCOUNTED")
                + $" = [{totalSumDisc:F2}]")))
                );
                Para.Inlines.Add(
                    new LineBreak()
                );
            }
            myFlowDoc.Blocks.Add(Para);

            rtbUsersOrdersInfo.Document = myFlowDoc;
        }

        private void BtCopyUsers_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.Clear();
            //Clipboard.SetDataObject(rtbUsersOrdersInfo.Document.);
            //var a = new TextRange(rtbUsersOrdersInfo.Document.ContentStart, rtbUsersOrdersInfo.Document.ContentEnd);
            Clipboard.SetText(new TextRange(rtbUsersOrdersInfo.Document.ContentStart, rtbUsersOrdersInfo.Document.ContentEnd).Text);
        }

        private void BtCopyItems_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.Clear();
            //Clipboard.SetDataObject(rtbUsersOrdersInfo.Document.);
            //var a = new TextRange(rtbUsersOrdersInfo.Document.ContentStart, rtbUsersOrdersInfo.Document.ContentEnd);
            Clipboard.SetText(new TextRange(rtbOrderItemsInfo.Document.ContentStart, rtbOrderItemsInfo.Document.ContentEnd).Text);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://money.yandex.ru/to/41001421002888");
        }
    }
}
