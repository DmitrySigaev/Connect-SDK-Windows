﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Navigation;
using ConnectSdk.Demo.Common;
using ConnectSdk.Demo.Demo;
using ConnectSdk.Windows.Device;
using ConnectSdk.Windows.Discovery;
using ConnectSdk.Windows.Etc.Helper;
using ConnectSdk.Windows.Service;
using ConnectSdk.Windows.Service.Config;
using UpdateControls.Fields;
using UpdateControls.XAML;

namespace ConnectSdk.Demo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Search : Page
    {
        private readonly Model model;
        private WebOstvService webOstvService;
        private DiscoveryManagerListener listener;

        public Search()
        {
            InitializeComponent();
            model = App.ApplicationModel;
            DataContext = ForView.Wrap(model);
        }

        private void SearchTvs()
        {
            listener = new DiscoveryManagerListener();

            listener.Paired += (sender, o) =>
            {
                model.SelectedDevice = new Independent<ConnectableDevice>(o as ConnectableDevice);
                Dispatcher.RunAsync(CoreDispatcherPriority.High, () => Frame.Navigate(typeof(Main)));
            };
            DiscoveryManager.Init();
            var discoveryManager = DiscoveryManager.GetInstance();
            discoveryManager.AddListener(listener);
            discoveryManager.PairingLevel = DiscoveryManager.PairingLevelEnum.On;
            discoveryManager.Start();
        }

        private void TvListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var tvdef = ForView.Unwrap<DeviceServiceViewModel>(e.AddedItems[0]);

            App.ApplicationModel.SelectedDevice = tvdef.Device;

            var netCastService = (NetcastTvService)tvdef.Device.GetServiceByName(NetcastTvService.Id);
            if (netCastService != null)
            {
                if (!(netCastService.ServiceConfig is NetcastTvServiceConfig))
                {
                    netCastService.ServiceConfig = new NetcastTvServiceConfig(netCastService.ServiceConfig.ServiceUuid);
                }
                netCastService.ShowPairingKeyOnTv();
            }
        }

        private void PairOkButton_OnClick(object sender, RoutedEventArgs e)
        {
            var senderButton = sender as Button;
            if (senderButton == null) return;

            var tvdeft = ForView.Unwrap<DeviceServiceViewModel>((senderButton.DataContext));
            model.SelectedDevice = tvdeft.Device;
            var tvdef = model.SelectedDevice;
            var netCastService = (NetcastTvService)tvdef.GetServiceByName(NetcastTvService.Id);
            if (netCastService != null)
            {
                var netCastServiceConfig = (NetcastTvServiceConfig)netCastService.ServiceConfig;
                var txtBox = Util.FindChild<TextBox>(senderButton.Parent, "PairingKeyTextBox");

                netCastServiceConfig.PairingKey = txtBox.Text;

                netCastService.ServiceConnectionState = NetcastTvService.ConnectionState.Initial;

                tvdef.Connect();
                if (tvdef.IsConnected())
                {
                    netCastService.RemovePairingKeyOnTv();
                    tvdef.OnConnectionSuccess(netCastService);
                    model.SelectedDevice = tvdef;
                    Frame.Navigate(typeof(Main));
                }
            }
            else
            {
                webOstvService = (WebOstvService)tvdef.GetServiceByName(WebOstvService.Id);
                if (webOstvService != null)
                {
                    if (!(webOstvService.ServiceConfig is WebOsTvServiceConfig))
                    {
                        webOstvService.ServiceConfig = new WebOsTvServiceConfig(webOstvService.ServiceConfig.ServiceUuid);
                    }
                    tvdef.AddListener(listener);
                    tvdef.Connect();
                    model.SelectedDevice.OnConnectionSuccess(webOstvService);

                }
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            model.SelectedDevice = null;
            model.DiscoverredDeviceServices.Clear();

            SearchTvs();
        }

        private void RefreshButton_OnClick(object sender, RoutedEventArgs e)
        {
            SearchTvs();
        }
    }

    public class VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var target = parameter as string;
            var device = value as string;

            if (target == null || device == null) return Visibility.Visible;

            switch (target)
            {
                case "PairingKeyLabel":
                    return !device.Equals("Netcast TV") ? Visibility.Collapsed : Visibility.Visible;
                case "PairingKeyTextBox":
                    return !device.Equals("Netcast TV") ? Visibility.Collapsed : Visibility.Visible;
                case "ConnectButton":
                    return !device.Equals("Netcast TV") ? Visibility.Collapsed : Visibility.Visible;
                case "ConnectWebOsButton":
                    return !device.Equals("Netcast TV") ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
