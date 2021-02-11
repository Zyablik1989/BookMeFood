using BookMyFood.ClientUI;
using MaterialDesignThemes.Wpf;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Threading;
using BookMyFood.Common;
using BookMyFood.ClientFuncion;
using BookMyFood.ServerFunction;
using System.ServiceModel;
using System.ServiceModel.Syndication;
using System.ServiceProcess;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using BookMyFood.WPF.Manual;
using BookMyFood.WPF.UserForms;
using BookMyFoodWCF;



namespace BookMyFood.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IViewClientUI
    {


        private static DoubleAnimation da = new DoubleAnimation(0, -180, new Duration(TimeSpan.FromSeconds(0.5)));
        private static RotateTransform rt = new RotateTransform();
        private static System.Timers.Timer aTimer;
        //private bool subscribed;
        private bool IsScanningForServers = false;
        bool switching = false;

        //Подгружаем настройки клиента
        Settings.ClientSettings settings = new Settings.ClientSettings();

        public MainWindow()
        {
            const string WINDOWS_FIREWALL_SERVICE = "MpsSvc";


            using (var sc = new ServiceController(WINDOWS_FIREWALL_SERVICE))
            {
                if ((sc.Status == ServiceControllerStatus.Running))
                {
                    Log.Instance.W(this, "Found working Windows Firewall Service");
                    MessageBoxResult result = MessageBox.Show(ClientUIPresenter.GetString("FoundworkingWindowsFirewallServiceWouldyouliketostopit")
                        , ClientUIPresenter.GetString("Question")
                        , MessageBoxButton.YesNo);
                    switch (result)
                    {
                        case MessageBoxResult.Yes:
                            if (ClientMaintaining.IsRunningAsLocalAdmin())
                            {
                                Log.Instance.W(this, ClientUIPresenter.GetString("Tryingtostop")
                                );
                                try
                                {
                                    sc.Stop();
                                    ServiceHelper.ChangeStartMode(sc, ServiceStartMode.Disabled);
                                    System.Windows.MessageBox.Show(
                                        ClientUIPresenter.GetString("WindowsFirewallServicehasbeenstopped")
                                        ,
                                        ClientUIPresenter.GetString("Success")
                                        , MessageBoxButton.OK,
                                        MessageBoxImage.Information);
                                }
                                catch (Exception e)
                                {
                                    Log.Instance.W(this, ClientUIPresenter.GetString("Errorwhilestoptrying")

                                                         + e.Message);
                                    System.Windows.MessageBox.Show(
                                        "Error while stop trying: " + e.Message,
                                        "Attention", MessageBoxButton.OK,
                                        MessageBoxImage.Warning);
                                }
                                //sc.sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(30));
                                if (sc.Status == ServiceControllerStatus.Stopped)
                                {
                                    Console.WriteLine("Service '{0}' has been stopped", sc.DisplayName);

                                }



                            }
                            else
                            {
                                System.Windows.MessageBox.Show(
                                    ClientUIPresenter.GetString("TostoptheWindowsFirewallServiceyoumustRunthisappasAdministrator")
                                    ,
                                    ClientUIPresenter.GetString("Insufficientrightstostoptheservice")
                                    , MessageBoxButton.OK,
                                    MessageBoxImage.Warning);
                            }
                            break;

                        case MessageBoxResult.No:
                            System.Windows.MessageBox.Show(
                                ClientUIPresenter.GetString("Youmayencountersomeproblemswhensearchingforotherpeoplesservers")
                                ,
                                ClientUIPresenter.GetString("Attention")
                                , MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                            break;
                        
                    }
                }
            }

                settings.LocaleChanged += ChangeFormLocale;
                ClientMaintaining.ClientStateChanged += ClientEventsHandler;
                InitializeComponent();
                cbLanguage.ItemsSource = new List<string> {"ru-RU", "en-US", "ms-MY"};
                FillUpTextFields();
                ChatGrid.Width = 0;
                SetDefaultValues();

            ChatClientHandler.ChatMessageCame += (string s) =>
            {
                Dispatcher.BeginInvoke(new ThreadStart(delegate
                {
                    lbChatMessages.ItemsSource = new List<string>();
                    lbChatMessages.ItemsSource = ChatClientHandler.ChatBoxContainment;
                    lbChatMessages.ScrollIntoView(lbChatMessages.Items[lbChatMessages.Items.Count - 1]);
                }));
            };

            void SetDefaultValues()
            {
                tbLeaderName.Text = ClientMaintaining.IsRunningAsLocalAdmin() ? Miscelanious.GetComputerName() : string.Empty;
                tbPort.Text = ClientMaintaining.IsRunningAsLocalAdmin() ? "666" : string.Empty;
                tbYourName.Text = Miscelanious.GetComputerName();
            }


             void ChangeFormLocale(string locale)
            {

                FillUpTextFields();

            }

             void FillUpTextFields()
            {
                ClientUIPresenter presenter = new ClientUIPresenter(this);
                presenter.FillUpTextFields();
            }


            StartupManual manual = new StartupManual();

            if (!File.Exists(Path.Combine(Assembly.GetExecutingAssembly().Location
                                             .Replace(Assembly.GetExecutingAssembly().ManifestModule.Name, "") +
                                         "firstrun")))
            {
                manual.ShowDialog();
                File.Create(Path.Combine(Assembly.GetExecutingAssembly().Location
                                             .Replace(Assembly.GetExecutingAssembly().ManifestModule.Name, "") +
                                         "firstrun"));
            }
            

            // Create a timer with a two second interval.
            aTimer = new System.Timers.Timer(2000);
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += MonitoringStringAutoUpdating;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
            
        }


        private  void MonitoringStringAutoUpdating(Object source, ElapsedEventArgs e)
        {
            Dispatcher.BeginInvoke(new ThreadStart(delegate
            {
               MonitoringString.Text = ClientMaintaining.GetMonitoringInfo();
            }));
            
        }

        #region Реализация презентера

        public string LeaderAddressFieldLabel
        {
            get { return HintAssist.GetHint(tbLeaderName).ToString(); }
            set { HintAssist.SetHint(tbLeaderName, value); }
        }

        public string tbChatMessageSend
        {
            get { return HintAssist.GetHint(tbChatMessage).ToString(); }
            set { HintAssist.SetHint(tbChatMessage, value); }
        }

        

        public string tbServersListFieldLabel
        {
            get { return tbServersListField.Text; }
            set
            {
                tbServersListField.Text = value;
            }
        }

        

        public string YourNameFieldLabel
        {
            get { return HintAssist.GetHint(tbYourName).ToString(); }
            set { HintAssist.SetHint(tbYourName, value); }
        }

        public string PortFieldLabel
        {
            get { return HintAssist.GetHint(tbPort).ToString(); }
            set { HintAssist.SetHint(tbPort, value); }
        }

        public string LanguageFieldLabel
        {
            get { return HintAssist.GetHint(cbLanguage).ToString(); }
            set { HintAssist.SetHint(cbLanguage, value); }
        }

        public string EditingModeRadioButtonLabel
        {
            get { return ""; }
            set { }
        }

        public string JoiningModeRadioButtonLabel
        {
            get { return btJoin.Content.ToString(); }
            set { btJoin.Content = value; }
        }

        public string LeadModeRadioButtonLabel
        {
            get { return btHost.Content.ToString(); }
            set { btHost.Content = value; }
        }

        public string ExpanderLabel
        {
            get { return tbExpanderLabel.Text; }
            set { tbExpanderLabel.Text = value; }
        }



        #endregion

        private void BtExit_Click(object sender, RoutedEventArgs e)
        {
            Window_Closing(new object(),null);
            Environment.Exit(0);
        }

        private void BtMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void CbLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var s = (ComboBox) sender;
            settings.SetLocale(s.SelectedItem.ToString());
        }

        private void Expander_Collapsed(object sender, RoutedEventArgs e)
        {

            ThicknessAnimation borderSettingAnimation = new ThicknessAnimation();
            Thickness thin = new Thickness(685, 0, 0, 0);

            borderSettingAnimation.From = SettingsBorder.Margin;
            borderSettingAnimation.To = thin;

            borderSettingAnimation.Duration = TimeSpan.FromSeconds(0.3);
            borderSettingAnimation.AccelerationRatio = 0.5;

            SettingsBorder.BeginAnimation(Border.MarginProperty, borderSettingAnimation);
            DoubleAnimation borderChatAnimation = new DoubleAnimation();

            double wide = 680;
            borderChatAnimation.From = ChatGrid.Width;
            borderChatAnimation.To = wide;

            borderChatAnimation.Duration = TimeSpan.FromSeconds(0.30);
            borderChatAnimation.AccelerationRatio = 0.5;

            ChatGrid.BeginAnimation(Border.WidthProperty, borderChatAnimation);

            //ChatGrid.Width = 670;
            //680

        }

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {

            ThicknessAnimation borderSettingAnimation = new ThicknessAnimation();

            Thickness wide = new Thickness(0, 0, 0, 0);
            borderSettingAnimation.From = SettingsBorder.Margin;
            borderSettingAnimation.To = wide;

            borderSettingAnimation.Duration = TimeSpan.FromSeconds(0.15);
            borderSettingAnimation.DecelerationRatio = 0.5;



            SettingsBorder.BeginAnimation(Border.MarginProperty, borderSettingAnimation);
            if (ChatGrid != null)
            {

                DoubleAnimation borderChatAnimation = new DoubleAnimation();
                double thin = 0;
                borderChatAnimation.From = ChatGrid.Width;
                borderChatAnimation.To = thin;

                borderChatAnimation.Duration = TimeSpan.FromSeconds(0.15);
                borderChatAnimation.DecelerationRatio = 0.5;

                ChatGrid.BeginAnimation(WidthProperty, borderChatAnimation);
            }


        }

        private void BtScan_Click(object sender, RoutedEventArgs e)
        {
            if (!IsScanningForServers)
            {
                IsScanningForServers = true;


                imgCircleArrows.RenderTransform = rt;
                imgCircleArrows.RenderTransformOrigin = new Point(0.5, 0.5);
                da.RepeatBehavior = RepeatBehavior.Forever;
                da.AccelerationRatio = 0.3;
                da.DecelerationRatio = 0.3;
                rt.BeginAnimation(RotateTransform.AngleProperty, da);



                //Нахождение компьютеров с запущенным сервером
                ClientMaintaining.scan.PercentPassed += (decimal p) =>
                {
                    Dispatcher.BeginInvoke(new ThreadStart(delegate
                    {
                        pbScan.Value = (double) p;
                        if (p >= 100)
                        {
                            imgCircleArrows.RenderTransform = new RotateTransform();
                            IsScanningForServers = false;

                        }
                    }));
                };
                ClientMaintaining.scan.ServerListUpdated += (string s) =>
                {
                    //Log.Instance.W(this, $" new Server Added {s}");
                    List<string> list = new List<string>();
                    foreach (var server in NetworkComputers.ServerList)
                    {
                        list.Add(server.VisibleName);
                    }

                    Dispatcher.BeginInvoke(new ThreadStart(delegate { lvListOfServers.ItemsSource = list; }));


                };


                ClientMaintaining.scan.Scan();



            }


            else
            {
                IsScanningForServers = false;
                ClientMaintaining.scan.ScanCancel();

                imgCircleArrows.RenderTransform = new RotateTransform();
            }

        }

        private async void BtHost_Click(object sender, RoutedEventArgs e)
        {
            LeaderServer.Leader.ServerDNSName = tbLeaderName.Text.Trim();
            LeaderServer.Leader.VisibleName = tbYourName.Text.Trim();
            LeaderServer.Leader.ServerIP = Miscelanious.ResolveIP(tbLeaderName.Text.Trim()).FirstOrDefault();


            if (ClientMaintaining.IsRunningAsLocalAdmin())
            {
                if (ServerMaintaining.host.State != CommunicationState.Opened &&
                    ServerMaintaining.host.State != CommunicationState.Opening)
                {


                    imgCircleArrows.RenderTransform = new RotateTransform();
                    
                        IsScanningForServers = false;
                        ClientMaintaining.scan.ScanCancel();

                    bool ServerSuccessfullyStarted = false;

                        try
                        {
                        ServerSuccessfullyStarted =
                            await Task.Run(async () =>
                                {
                            ServerMaintaining.ServerStart();
                                    return true;
                                });
                    }
                        catch (InvalidOperationException exception)
                        {
                            System.Windows.MessageBox.Show(
                                ClientUIPresenter.GetString("ThehostnameandportarealreadyinuseWaitforalittle")
                                + exception.Message,
                                ClientUIPresenter.GetString("Error")
                                , MessageBoxButton.OK,
                                MessageBoxImage.Error);
                            ClientMaintaining.Disconnect();
                        }
                    catch (AddressAlreadyInUseException exception)
                        {
                            System.Windows.MessageBox.Show(
                                ClientUIPresenter.GetString("Thehostnameandportarealreadyinuse")
                                ,
                                ClientUIPresenter.GetString("Error")
                                , MessageBoxButton.OK,
                                MessageBoxImage.Error);
                            ClientMaintaining.Disconnect();
                    }

                    if (ServerSuccessfullyStarted)
                    {

                        ClientMaintaining.isSelfServer = true;
                        expander.IsExpanded = false;
                        Expander_Collapsed(this, null);


                        LeaderServer.Leader.ServerState = ServerStates.DelivererSet;
                        //ServerStatus.Current.ServerState = ServerStates.DelivererSet;


                        GridContainerForUC.Children.Clear();
                        GridContainerForUC.Children.Add(new ucSelector());


                    
                        Log.Instance.W(this, "Server is launched now trying to connect to ourselves");
                        ClientMaintaining.Join(tbYourName.Text, true);

                        //if (ClientMaintaining.GetConnectionStatus())
                        //{
                        //    ClientMaintaining.SendMessage("Hello to Everyone!");
                        //}
                    }

                }
                else
                {
                    string infoString = ClientUIPresenter.GetString("Yourserverisalreadystarted")
                        ;
                    if (LeaderServer.Leader.ServerDNSName != string.Empty)
                    {
                        infoString += Environment.NewLine + ClientUIPresenter.GetString("Itiscalled")
                                                          + LeaderServer.Leader.VisibleName +
                                      Environment.NewLine + ClientUIPresenter.GetString("Theotherusersoughttolookfor")
                                                          +
                                      LeaderServer.Leader.ServerDNSName;
                        foreach (var ip in NetworkComputers.ResolveIP(LeaderServer.Leader.ServerDNSName).Select(x=>x.ToString()).Distinct())
                        {
                            infoString += Environment.NewLine + ClientUIPresenter.GetString("or")
                                                              + ip;
                        }

                    }

                    System.Windows.MessageBox.Show(infoString
                        , "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                System.Windows.MessageBox.Show(ClientUIPresenter.GetString("TolaunchyourownserveryouneedtoRunthisappasAdministrator")
                    ,
                    ClientUIPresenter.GetString("Insufficientrightstolaunchaserver")
                    , MessageBoxButton.OK, MessageBoxImage.Warning);
            }


        }

        private void BtJoin_Click(object sender, RoutedEventArgs e)
        {
            uint port = 0;
            LeaderServer.Leader.ServerDNSName = tbLeaderName.Text.Trim();
            LeaderServer.Leader.VisibleName = tbYourName.Text.Trim();
            LeaderServer.Leader.Port = uint.TryParse(tbPort.Text.Trim(), out port) ? port : 0;
            LeaderServer.Leader.ServerIP = Miscelanious.ResolveIP(tbLeaderName.Text.Trim()).FirstOrDefault();

            IsScanningForServers = false;
            ClientMaintaining.scan.ScanCancel();

            imgCircleArrows.RenderTransform = new RotateTransform();

            ServerStop();
            if (ClientMaintaining.isConnected)
            {
                ClientMaintaining.Disconnect();
            }
            ClientMaintaining.Join(tbYourName.Text);

            if (ClientMaintaining.isConnected)
            {


                expander.IsExpanded = false;
                Expander_Collapsed(this, null);
                //ChatClientHandler.ChatMessageCame += (string s) =>
                //{
                //    Dispatcher.BeginInvoke(new ThreadStart(delegate
                //    {
                //        lbChatMessages.ItemsSource = new List<string>();
                //        lbChatMessages.ItemsSource = ChatClientHandler.ChatBoxContainment;
                //        lbChatMessages.ScrollIntoView(lbChatMessages.Items[lbChatMessages.Items.Count - 1]);
                //    }));
                //};
                GridContainerForUC.Children.Clear();
                GridContainerForUC.Children.Add(new ucSelector());


            }
            //var u = ClientMaintaining.GetUsers();

        }

        private void ServerStop()
        {


            if (ClientMaintaining.IsRunningAsLocalAdmin() &&
                ServerMaintaining.host.State != CommunicationState.Closed &&
                ServerMaintaining.host.State != CommunicationState.Closing &&
                ServerMaintaining.host.State != CommunicationState.Created)

            {
                ServerMaintaining.ServerStop();
            }



        }



        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ClientMaintaining.Disconnect();
            ServerStop();
           
        }


        private void MainWindow_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }



        private void LvListOfServers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var pick = (sender as ListView).SelectedValue;
            if (pick != null)
            {
                LeaderServer pickedLeader = NetworkComputers.ServerList
                    .FirstOrDefault(x => x.VisibleName == pick.ToString().ToUpper());

                if (pickedLeader != null)
                {
                    tbLeaderName.Text = pickedLeader.ServerDNSName;
                    tbPort.Text = "";

                }

            }
        }

        private void TbChatMessageSend_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && ClientMaintaining.isConnected)
            {

                ClientMaintaining.SendMessage(tbChatMessage.Text.Trim());
                tbChatMessage.Text = string.Empty;
                tbChatMessage.Focus();
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void ClientEventsHandler(ClientFuncion.ClientState state)
        {
            if (!switching)
            {
                switching = true;
                switch (state)
                {
                    case ClientState.Offline:
                        Dispatcher?.BeginInvoke(new ThreadStart(delegate { ClientGoesOffline(); }));

                        //Dispatcher.BeginInvoke(new ThreadStart());
                        break;
                    case ClientState.Calculated:
                        Dispatcher?.BeginInvoke(new ThreadStart(delegate
                        {
                            GridContainerForUC.Children.Clear();
                            GridContainerForUC.Children.Add(new ucCalculates());
                            Expander_Expanded(this, null);
                            expander.IsExpanded = true;
                            tbLeaderName.Text = "";
                            tbPort.Text = "";
                            //ClientGoesOffline();

                        }));

                        break;
                }
                switching = false;
            }
        }

        private void ClientGoesOffline()
        {
            
            GridContainerForUC.Children.Clear();
            lbChatMessages.ItemsSource = new List<string>();
            tbLeaderName.Text = "";
            tbPort.Text = "";
            
            Expander_Expanded(this, null);
            expander.IsExpanded = true;

            //System.Windows.MessageBox.Show(
            //    "You've been kicked by the Leader",
            //    "Error", MessageBoxButton.OK,
            //    MessageBoxImage.Error);
            //lvListOfServers.ItemsSource = new List<string>();
        }


    }
}
