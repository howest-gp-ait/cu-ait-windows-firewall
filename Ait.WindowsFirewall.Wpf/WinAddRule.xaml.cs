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
using Ait.WindowsFirewall.Core.Entities;
using Microsoft.Win32;
using NetFwTypeLib;

namespace Ait.WindowsFirewall.Wpf
{
    /// <summary>
    /// Interaction logic for winAddRule.xaml
    /// </summary>
    public partial class WinAddRule : Window
    {
        public WinAddRule()
        {
            InitializeComponent();
        }

        public bool ReloadRequired = false;
        ProtocolService protocolService;
        FirewallService firewallService;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            protocolService = new ProtocolService();
            firewallService = new FirewallService();
            PopulateCombo();
            MakeSomeDummyData();
        }
        private void PopulateCombo()
        {
            CmbType.Items.Clear();

            ComboBoxItem itm;
            itm = new ComboBoxItem();
            itm.Content = "INBOUND RULE";
            itm.Tag = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN;
            CmbType.Items.Add(itm);
            itm = new ComboBoxItem();
            itm.Content = "OUTBOUND RULE";
            itm.Tag = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_OUT;
            CmbType.Items.Add(itm);

            CmbProtocol.ItemsSource = protocolService.Protocols;

        }
        private void MakeSomeDummyData()
        {
            CmbType.SelectedIndex = 1;
            txtFirewallRuleName.Text = "_blokkeer_ping_howest";
            txtBeschrijving.Text = "testregel waarbij pingen naar howest (185.162.30.243) onmogelijk wordt gemaakt";
            chkProfielDomein.IsChecked = true;
            chkProfielOpenbaar.IsChecked = true;
            chkProfielPrive.IsChecked = true;
            chkIsActief.IsChecked = true;
            rdbBlock.IsChecked = true;
            txtAdresExtern.Text = "185.162.30.243";
            CmbProtocol.SelectedIndex = 0;
        }
        private void BtnProgramma_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == true)
            {
                txtProgramma.Text = ofd.FileName;
            }
        }

        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxItem itm = (ComboBoxItem)CmbType.SelectedItem;
            NET_FW_RULE_DIRECTION_ richting = (NET_FW_RULE_DIRECTION_)itm.Tag;

            string name = txtFirewallRuleName.Text;
            string description = txtBeschrijving.Text;
            int profileValue = 0;
            if ((bool)chkProfielDomein.IsChecked) profileValue += 1;
            if ((bool)chkProfielPrive.IsChecked) profileValue += 2;
            if ((bool)chkProfielOpenbaar.IsChecked) profileValue += 4;
            bool enabled = (bool)chkIsActief.IsChecked;
            NET_FW_ACTION_ typeOfRule = NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
            if ((bool)rdbBlock.IsChecked) typeOfRule = NET_FW_ACTION_.NET_FW_ACTION_BLOCK;
            string application = txtProgramma.Text;
            if (application.ToString() == "")
                application = null;
            string localAdresses = txtAdresLokaal.Text;
            if (localAdresses.Trim() == "") localAdresses = "*";
            string remoteAdresses = txtAdresExtern.Text;
            if (remoteAdresses.Trim() == "") remoteAdresses = "*";
            string localPorts = txtPoortLokaal.Text;
            string remotePorts = txtPoortExtern.Text;
            Protocol protocol = (Protocol)CmbProtocol.SelectedItem;
            

            bool success;
            if(CmbType.SelectedIndex == 0)
            {
                success = firewallService.CreateInboundRule(name, description, profileValue, enabled, typeOfRule, application, localAdresses, remoteAdresses, localPorts, remotePorts, protocol.Value);
            }
            else
            {
                success = firewallService.CreateInboundRule(name, description, profileValue, enabled, typeOfRule, application, localAdresses, remoteAdresses, localPorts, remotePorts, protocol.Value);
            }
            if (success)
            {
                ReloadRequired = true;
                this.Close();
            }
        }
    }
}
