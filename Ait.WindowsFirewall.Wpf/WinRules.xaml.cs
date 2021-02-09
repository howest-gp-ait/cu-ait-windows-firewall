using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Ait.WindowsFirewall.Core.Services;
using NetFwTypeLib;

namespace Ait.WindowsFirewall.Wpf
{
    /// <summary>
    /// Interaction logic for winRules.xaml
    /// </summary>
    public partial class WinRules : Window
    {
        public WinRules()
        {
            InitializeComponent();
        }

        FirewallService firewallService;
        ProtocolService protocolService;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            firewallService = new FirewallService();
            protocolService = new ProtocolService();

            rdbInboundRules.IsChecked = true;
            rdbOutboundRules.IsChecked = false;
            ClearControls();
            ShowRules();
        }
        private void ShowRules()
        {
            lstRules.Items.Clear();
            ListBoxItem listBoxItem;
            if(rdbInboundRules.IsChecked == true)
            {
                foreach (INetFwRule regel in firewallService.InboundRules)
                {
                    listBoxItem = new ListBoxItem();
                    listBoxItem.Content = regel.Name;
                    listBoxItem.Tag = regel;
                    lstRules.Items.Add(listBoxItem);
                }
            }
            else
            {
                foreach (INetFwRule regel in firewallService.OutboundRules)
                {
                    listBoxItem = new ListBoxItem();
                    listBoxItem.Content = regel.Name;
                    listBoxItem.Tag = regel;
                    lstRules.Items.Add(listBoxItem);
                }
            }
        }

        private void BtnNewRule_Click(object sender, RoutedEventArgs e)
        {
            WinAddRule winAddRule = new WinAddRule();
            winAddRule.ShowDialog();
            if(winAddRule.ReloadRequired)
            {
                firewallService = new FirewallService();
                ShowRules();
            }
        }

        private void RdbInboundRules_Checked(object sender, RoutedEventArgs e)
        {
            ShowRules();
        }

        private void RdbOutboundRules_Checked(object sender, RoutedEventArgs e)
        {
            ShowRules();
        }

        private void LstRules_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ClearControls();
            if (lstRules.SelectedItem == null) return;

            ListBoxItem itm = (ListBoxItem)lstRules.SelectedItem;
            INetFwRule rule = (INetFwRule)itm.Tag;

            txtFirewallRuleName.Text = rule.Name;
            lblBeschrijving.Content = rule.Description;

            /*  Ter info : waarden van enumeratie : 
             * (int)NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_DOMAIN;  = 1
             * (int)NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_PRIVATE; = 2
             * (int)NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_PUBLIC;  = 4
             * (int)NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_ALL; =2147483647
            */
            if (rule.Profiles == (int)NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_ALL)
            {
                chkProfielDomein.IsChecked = true;
                chkProfielOpenbaar.IsChecked = true;
                chkProfielPrive.IsChecked = true;
            }
            if (rule.Profiles == 4 || rule.Profiles == 6 || rule.Profiles == 7)
                chkProfielOpenbaar.IsChecked = true;
            if (rule.Profiles == 2 || rule.Profiles == 3 || rule.Profiles == 6 || rule.Profiles == 7)
                chkProfielPrive.IsChecked = true;
            if (rule.Profiles == 1 || rule.Profiles == 3 || rule.Profiles == 5 || rule.Profiles == 7)
                chkProfielOpenbaar.IsChecked = true;

            chkIsActief.IsChecked = rule.Enabled;

            if (rule.Action == NET_FW_ACTION_.NET_FW_ACTION_ALLOW)
            {
                chkActionAllow.IsChecked = true;
                chkActionAllow.Background = Brushes.ForestGreen;
            }
            if (rule.Action == NET_FW_ACTION_.NET_FW_ACTION_BLOCK)
            {
                chkActionBlock.IsChecked = true;
                chkActionBlock.Background = Brushes.Tomato;
            }

            if (rule.ApplicationName != null)
                txtProgramma.Text = rule.ApplicationName;
            else
                txtProgramma.Text = "<alle programma's>";

            if (rule.LocalAddresses == "*")
                txtLokaalAdres.Text = "<elk willekeurig adres>";
            else
                txtLokaalAdres.Text = rule.LocalAddresses;

            if (rule.RemoteAddresses == "*")
                txtExternAdres.Text = "<elk willekeurig adres>";
            else
                txtExternAdres.Text = rule.RemoteAddresses;

            if (rule.LocalPorts == "*")
                txtLokalePoorten.Text = "<elke willekeurige poort>";
            else
                txtLokalePoorten.Text = rule.LocalPorts;

            if (rule.RemotePorts == "*")
                txtExternePoorten.Text = "<elke willekeurige poort>";
            else
                txtExternePoorten.Text = rule.RemotePorts;

            txtProtocol.Text = protocolService.FindProtocol(rule.Protocol).Name;
        }
        private void ClearControls()
        {
            txtFirewallRuleName.Text = "";
            lblBeschrijving.Content = "";
            chkProfielDomein.IsChecked = false;
            chkProfielOpenbaar.IsChecked = false;
            chkProfielPrive.IsChecked = false;
            chkIsActief.IsChecked = false;
            chkActionAllow.IsChecked = false;
            chkActionAllow.Background = Brushes.White;
            chkActionBlock.IsChecked = false;
            chkActionBlock.Background = Brushes.White;
            txtProgramma.Text = "";
            txtLokaalAdres.Text = "";
            txtExternAdres.Text = "";
            txtLokalePoorten.Text = "";
            txtExternePoorten.Text = "";
            txtProtocol.Text = "";
        }

    }
}
