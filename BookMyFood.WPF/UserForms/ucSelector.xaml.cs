using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Odbc;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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
using BookMyFood.Common;
using BookMyFood.WPF.ListItems;
using BookMyFood.WPF.UserForms;
using BookMyFood.Model;
using BookMyFood.ServerFunction;
using BookMyFoodWCF;


namespace BookMyFood.WPF.UserForms
{
    /// <summary>
    /// Interaction logic for UserControlBase.xaml
    /// </summary>
    public partial class ucSelector : UserControl
    {
        bool updating = false;
        private static ObservableCollection<UserItem> UsersList = new ObservableCollection<UserItem>();
        private static ObservableCollection<UserControl> Pickableitems = new ObservableCollection<UserControl>();

        private static ObservableCollection<UserItem> TempUsersList = new ObservableCollection<UserItem>();
        private static ObservableCollection<UserControl> TempPickableitems = new ObservableCollection<UserControl>();

        private static ClientState CurrentState = ClientState.Offline;
        private static Deliverer CurrentDeliverer = null;
        
        private static string preCurrentSearch = "";
        private static string currentSearch = "";

        private static double discount = 0;

        //private static ObservableCollection<UserItem> tempUsers;

        public ucSelector()
        {
            InitializeComponent();

            tbDiscount.IsEnabled = ClientMaintaining.isSelfServer;
            FieldsInitialise();
            //DisplayListOfElements(ClientState.Offline);
            //StartButtonInit();

            if (lbUsers.ItemsSource == null)
                lbUsers.ItemsSource = UsersList;

            lbElementsToChoose.ItemsSource = Pickableitems;

            
            ClientMaintaining.ClientStateChanged += (state) =>
            {
                Dispatcher?.BeginInvoke(new ThreadStart(delegate
                {
                    if (CurrentState != state || Math.Abs(ClientMaintaining.CurrentDiscount - discount) > 0 || currentSearch!= preCurrentSearch)
                    {
                        preCurrentSearch = currentSearch;
                        discount = ClientMaintaining.CurrentDiscount;
                        tbDiscount.Text = ClientMaintaining.CurrentDiscount.ToString();
                        CurrentState = state;
                        DisplayListOfElements(state);
                    }
                }));
            };

            ClientMaintaining.ClientUsersChanged += (users) =>
            {
                Dispatcher?.BeginInvoke(new ThreadStart(delegate { ListUsersUpdate(users); }));
            };

        }

        private void FieldsInitialise()
        {
            LableOfPick.Text = ClientUIPresenter.GetString("STORESLISTpickone");
            lSearch.Text = ClientUIPresenter.GetString("Search");
            lDiscount.Text = ClientUIPresenter.GetString("Discount");


        }

        private void DisplayListOfElements(ClientState state)
        {
            List<Deliverer> ListDeliverers;
            switch (state)
                {

                    case ClientState.DelivererSet:
                    {
                        ListDeliverers = SQLiteHelper.GetReader<Deliverer>("Deliverers");
                        {
                                Pickableitems.Clear();

                                var r = new Regex($"(?:{currentSearch})+", RegexOptions.IgnoreCase);
                                foreach (var listDeliverer in ListDeliverers.Where(x => r.IsMatch(x.Name) || r.IsMatch(x.Description)))
                                {
                                    Pickableitems.Add(new StoreItem(listDeliverer));
   


                                }

                            }
                        lbElementsToChoose.IsEnabled = true;
                        btToStartToOrder.IsEnabled = true;
                        btToStartToOrder.Content = ClientUIPresenter.GetString("Pickadeliverer")
                            ;
                        break;
                            
                        }
                    case ClientState.DelivererWait:
                    {
                        Pickableitems.Clear();
                    lbElementsToChoose.IsEnabled = false;
                        ListDeliverers = SQLiteHelper.GetReader<Deliverer>("Deliverers");

                        var r = new Regex($"(?:{currentSearch})+", RegexOptions.IgnoreCase);
                        foreach (var listDeliverer in ListDeliverers.Where(x => r.IsMatch(x.Name) || r.IsMatch(x.Description)))
                        {
                            Pickableitems.Add(new StoreItem(listDeliverer));



                        }

                    }

                    LableOfPick.Text = "";
                        //Pickableitems.Clear();

                        btToStartToOrder.IsEnabled = false;
                        btToStartToOrder.Content = ClientUIPresenter.GetString("WaitingforLeader")
;
                    break;

                        
                    case ClientState.OrderSet:
                    {
                        lbElementsToChoose.IsEnabled = true;
                        LableOfPick.Text = ClientUIPresenter.GetString("PICKORDERPOSITIONS")
                            ;
                        var deliverer = ClientMaintaining.CurrentDeliverer;
                        if (deliverer != null)
                        {
                            List<Item> ListItems = SQLiteHelper.GetReader<Item>("Items", "SELECT * FROM ",
                                deliverer.ID);

                         
                            Pickableitems.Clear();

                            var r = new Regex($"(?:{currentSearch})+", RegexOptions.IgnoreCase);
                            foreach (var listItem in ListItems.Where(x => r.IsMatch(x.Name) || r.IsMatch(x.Description)))
                            {
                                Pickableitems.Add(new Items(listItem));



                            }
                            btToStartToOrder.IsEnabled = true;
                            if (ClientMaintaining.isSelfServer)
                            {
                                btToStartToOrder.IsEnabled = true;

                                btToStartToOrder.Content = ClientUIPresenter.GetString("Starttocalculate" );
                            }
                            else
                            {
                                btToStartToOrder.IsEnabled = true;
                                btToStartToOrder.Content = ClientUIPresenter.GetString("Myorderisdone")
                                    ;
                            }
                        }

                        break;
                    }

                    case ClientState.WaitingForCalculation:
                    {
                        Pickableitems.Clear();
                        btToStartToOrder.IsEnabled = false;
                        break;
                    }

                    //case ClientState.WaitingForCalculation:
                    //{
                    //    btToStartToOrder.IsEnabled = false;
                    //    break;
                    //}

                default:
                        btToStartToOrder.IsEnabled = false;
                    btToStartToOrder.Content = ClientUIPresenter.GetString("WaitingforLeader");
                    break;


            }
        }

        private async Task ListUsersUpdate(List<BookMyFood.ServiceChat.ServerUser> users)
        {
            if (!updating)
            {
                updating = true;
        
                    ClientMaintaining.Update()
                        ;
             
                try
                {
                    var me = users[users.FindIndex(x => x.ID == ClientMaintaining.ID)];
                    users.Remove(me);
                    users.Insert(0,me);
                }
                catch (Exception e)
                {
                    Log.Instance.W(this, e.Message);
                    updating = false;
                    return;
                    
                }


                var cp = new SequenceEqualityComparer<UserItem>(new UserComparer());
                
                    TempUsersList.Clear();
                //await Task.Run(async () => { Thread.Sleep(3000); });
                foreach (var user in users)
                {
                    TempUsersList.Add(new UserItem(user));
                    //await Task.Run(async () => { TempUsersList.Add(new UserItem(user)); });
                    //await
                    //    Task.Run( () = {

                    //    TempUsersList.Add(new UserItem(user));
                    //}

                }
            

                

                
                if (!cp.Equals(TempUsersList,UsersList) || Math.Abs(ClientMaintaining.CurrentDiscount - discount) > 0)
                {

                    UsersList.Clear();
                    foreach (var user in users)
                    {
                        UsersList.Add(new UserItem(user));

                    };
                }

                updating = false;
            }
        }

        
        private void LbStoresToChoose_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            var pick =(sender as ListBox)?.SelectedValue;
            if (pick is StoreItem)
            {
                CurrentDeliverer = (pick as StoreItem).StoreDeliverer;
              
            }

            if (pick is ListItems.Items)
            {
                var p = (pick as Items).PickedItem;
                lbElementsToChoose.UnselectAll();
                if ( ServerUser.Current.Order.Items.FirstOrDefault(x => x.ID == ((pick as Items).PickedItem).ID) == null)
               
                {
                    if (!ClientMaintaining.ClientReadiness)
                    {
                        ServerUser.Current.Order.Items.Add(new Item()
                        {
                            Description = p.Description,
                            ID = p.ID,
                            Name = p.Name,
                            Price = p.Price,
                            Quantity = 1
                        });
                        
                    }
                }
                else
                {
                    if (!ClientMaintaining.ClientReadiness)
                    {
                        //ServerUser.Current.Order.Items.FirstOrDefault(x => x.ID == ((pick as Items).PickedItem).ID).Price +=
                        //    p.Price;
                        var user = ServerUser.Current.Order.Items.FirstOrDefault(x =>
                            x.ID == ((pick as Items).PickedItem).ID);
                        if (user != null)
                        user.Quantity += 1;
                    }
                }
            }

        }

        private void BtToStartToOrder_Click(object sender, RoutedEventArgs e)
        {
            

            

            ServerStates state = 0;

            
            
                if (ClientMaintaining.isSelfServer)
                    state = LeaderServer.Leader.ServerState;
                else
                {
                    if (LeaderServer.CurrentServer != null)
                        state = LeaderServer.CurrentServer.ServerState;
                }

            if (state == ServerStates.OrdersSet)
            {
                if (ClientMaintaining.isSelfServer)
                {
                    if (ServerStatus
                        .users.Count>1 &&
                        ServerStatus
                            .users.Count(x => x.ID!=ClientMaintaining.ID && x.Ready==false)>0)
                    {
                        MessageBoxResult result = MessageBox.Show(
                            ClientUIPresenter.GetString("NotallusersarereadytocalculatetheirordersContinueforcibly")
                        
                            , ClientUIPresenter.GetString("Question"), MessageBoxButton.YesNo);
                        if (result == MessageBoxResult.Yes)
                        {
                            btToStartToOrder.IsEnabled = false;
                            LeaderServer.Leader.ServerState = ServerStates.ReadyToCalculate;
                        }
                        
                    }
                    else
                    {
                        btToStartToOrder.IsEnabled = false;
                        LeaderServer.Leader.ServerState = ServerStates.ReadyToCalculate;
                    }                  
                    
                }
                else
                {
                    if (ClientMaintaining.ClientReadiness)
                    {
                        ClientMaintaining.ClientReadiness = false;
                        btToStartToOrder.Content = ClientUIPresenter.GetString("Myorderisdone")
                            ;
                    }
                    else
                    {
                        ClientMaintaining.ClientReadiness = true;
                        btToStartToOrder.Content = ClientUIPresenter.GetString("Changemyorder")
                            ;

                    }
                }
            }
            if (state == ServerStates.DelivererSet
                && ClientMaintaining.isSelfServer
                //&& LeaderServer.Leader.ServerState == ServerStates.DelivererSet
                && CurrentDeliverer != null)
            {
                //btToStartToOrder.IsEnabled = false;
                ServerStatus.Current.ServerDeliverer = CurrentDeliverer;
                LeaderServer.Leader.ServerState = ServerStates.OrdersSet;

            }
            //switch (LeaderServer.Leader.ServerState)

        }

        private void TbDiscount_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ClientMaintaining.isSelfServer)
            {
                double disc = 0;
                ServerFunction.ServerStatus.discount = (double.TryParse(tbDiscount.Text.Trim(), out disc) && disc >= 0 && disc<=100) ? disc : 0;
            }
        }

        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            
            if (string.IsNullOrEmpty(Search.Text))
            {
                currentSearch = "";
            }
            else
            {
                currentSearch = Search.Text.ToLower().Trim();
            }
        }
    }

    public class SequenceEqualityComparer<T> : EqualityComparer<IEnumerable<T>>,
                                           IEquatable<SequenceEqualityComparer<T>>
    {
        readonly IEqualityComparer<T> comparer;

        public SequenceEqualityComparer(IEqualityComparer<T> comparer = null)
        {
            this.comparer = comparer ?? EqualityComparer<T>.Default;
        }

        public override bool Equals(IEnumerable<T> x, IEnumerable<T> y)
        {
           
            if (ReferenceEquals(x, y))
                return true;

            if (x == null || y == null)
                return false;

            var xICollection = x as ICollection<T>;
            if (xICollection != null)
            {
                var yICollection = y as ICollection<T>;
                if (yICollection != null)
                {
                    if (xICollection.Count != yICollection.Count)
                        return false;

                    var xIList = x as IList<T>;
                    if (xIList != null)
                    {
                        var yIList = y as IList<T>;
                        if (yIList != null)
                        {
                            // optimization - loops from bottom
                            for (int i = xIList.Count - 1; i >= 0; i--)
                                if (!comparer.Equals(xIList[i], yIList[i]))
                                    return false;

                            return true;
                        }
                    }
                }
            }

            return x.SequenceEqual(y, comparer);
        }

        public override int GetHashCode(IEnumerable<T> sequence)
        {
            unchecked
            {
                int hash = 397;
                foreach (var item in sequence)
                    hash = hash * 31 + comparer.GetHashCode(item);

                return hash;
            }
        }

        public bool Equals(SequenceEqualityComparer<T> other)
        {
            if (ReferenceEquals(null, other))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return this.comparer.Equals(other.comparer);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as SequenceEqualityComparer<T>);
        }

        public override int GetHashCode()
        {
            return comparer.GetHashCode();
        }
    }

    class UserComparer : IEqualityComparer<UserItem>
    {

        public bool Equals(UserItem x, UserItem y)
        {

            //Check whether the compared objects reference the same data.
            if (Object.ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null.
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            var oc = new SequenceEqualityComparer<BookMyFood.ServiceChat.Item>(new ItemComparer());
            bool orderEq = false;
            if (x.pickedUser.Order!=null && y.pickedUser.Order!=null)
            {
                 orderEq = oc.Equals( x.pickedUser.Order.Items, y.pickedUser.Order.Items);

            }

            return (x.pickedUser.ID == y.pickedUser.ID
                    &&
                    x.pickedUser.MissionForClient == y.pickedUser.MissionForClient

                    &&
                    x.pickedUser.Name == y.pickedUser.Name
                    &&
                    x.pickedUser.Ready == y.pickedUser.Ready
                    &&
                    x.pickedUser.isLeader == y.pickedUser.isLeader
                    &&
                    orderEq
                    );

        }


        public int GetHashCode(UserItem product)
        {
            //Check whether the object is null
            if (Object.ReferenceEquals(product, null)) return 0;

            //Get hash code for the Name field if it is not null.
            int hashProductName = product.pickedUser == null ? 0 : product.Name.GetHashCode();
            
            //Calculate the hash code for the product.
            return hashProductName;
        }
    }

    class ItemComparer : IEqualityComparer<BookMyFood.ServiceChat.Item>
    {
        public bool Equals(BookMyFood.ServiceChat.Item x, BookMyFood.ServiceChat.Item y)
        { //Check whether the compared objects reference the same data.
            if (Object.ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null.
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;
            return (x.ID == y.ID
                    &&
                    x.Price == y.Price
                    &&
                    x.Quantity == y.Quantity
                    &&
                    x.Name == y.Name);

        }

        public int GetHashCode(BookMyFood.ServiceChat.Item product)
        {
            //Check whether the object is null
            if (Object.ReferenceEquals(product, null)) return 0;

            //Get hash code for the Name field if it is not null.
            int hashProductName = product.Name == null ? 0 : product.Name.GetHashCode();

            //Get hash code for the Code field.
            //int hashProductCode = product.Precision.GetHashCode();

            //Calculate the hash code for the product.
            return hashProductName;
        }
    }
}
