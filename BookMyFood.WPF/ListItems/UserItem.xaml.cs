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
using BookMyFood.ClientFuncion;
using BookMyFood.ServiceChat;

namespace BookMyFood.WPF.ListItems
{
    /// <summary>
    /// Interaction logic for UserItem.xaml
    /// </summary>
    public partial class UserItem : UserControl
    {
        public UserItem(ServerUser user)
        {
            InitializeComponent();
            tbUserNameInList.Text = user.Name;
            bool ChangableQuantity = (user.ID == ClientMaintaining.ID && !ClientMaintaining.ClientReadiness);
            if (user.isLeader)
                imgUserLeaderOrDelete.Source = new BitmapImage(new Uri("Resources/leader.png", UriKind.Relative));

            if (user.Ready)
                Readiness.Source = new BitmapImage(new Uri("Resources/ready.png", UriKind.Relative));

            pickedUser = user;

           if (ClientMaintaining.isSelfServer && ClientMaintaining.ID != user.ID)
           {
               btCross.Opacity = 1;
               //imgCross.Source = new BitmapImage(new Uri("Resources/X.png", UriKind.Relative));   
           }

           else
           {
               btCross.Opacity = 0;
            }

            

            if (user.Order != null)
            {

                foreach (var item in user.Order.Items)
                {
                
                    lbOrderItems.Items.Add(new OrderItem(item, ChangableQuantity));
                    //lbUsers.Items.Add(new UserItem(user));
                }
            }
        }

        public ServerUser pickedUser { get; set; }
        //private void ImgUserLeaderOrDelete_OnMouseDown(object sender, MouseButtonEventArgs e)
        //{

        //    //BookMyFood.ClientFuncion..users[ServerStatus.users.FindIndex(ind => ind.ID == user.ID)].


        //}

        ////private void BtDelete_OnMouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    if (ClientMaintaining.isSelfServer && pickedUser.ID != ClientMaintaining.ID)
        //        ServerFunction.ServerStatus
        //            .users[ServerFunction.ServerStatus.users.FindIndex(ind => ind.ID == pickedUser.ID)]
        //            .MissionForClient = 1;
        //}

        private void BtCross_OnClick(object sender, RoutedEventArgs e)
        {
            if (ClientMaintaining.isSelfServer && pickedUser.ID != ClientMaintaining.ID && !ClientMaintaining.ClientReadiness)
                ServerFunction.ServerStatus
                    .users[ServerFunction.ServerStatus.users.FindIndex(ind => ind.ID == pickedUser.ID)]
                    .MissionForClient = 1;
        }
    }
}
