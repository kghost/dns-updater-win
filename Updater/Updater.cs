using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Updater
{
    public partial class updater : ServiceBase
    {
        public updater()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            this.updateTimer.Start();
            System.Net.NetworkInformation.NetworkChange.NetworkAddressChanged += addressChangedCallback;
        }

        protected override void OnStop()
        {
            System.Net.NetworkInformation.NetworkChange.NetworkAddressChanged -= addressChangedCallback;
            this.updateTimer.Stop();
        }

        private void addressChangedCallback(Object sender, EventArgs e)
        {
            checkUpdate();
        }

        private void updateTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            checkUpdate();
        }

        private void checkUpdate()
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;

                System.Net.NetworkInformation.NetworkInterface nic = null;
                if (settings["network"] != null)
                {
                    var name = settings["network"].Value;
                    foreach (var n in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces())
                    {
                        if (n.Name == name)
                        {
                            nic = n;
                            break;
                        }
                    }
                }

                if (nic == null)
                {
                    foreach (var n in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces())
                    {
                        var properties = n.GetIPProperties();
                        if (properties == null) continue;
                        var ipv4 = properties.GetIPv4Properties();
                        if (ipv4 == null) continue;
                        if (ipv4.IsDhcpEnabled)
                        {
                            var hasip = false;
                            foreach (var address in nic.GetIPProperties().UnicastAddresses)
                            {
                                if (address.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                                {
                                    hasip = true;
                                    break;
                                }
                            }

                            if (hasip)
                            {
                                nic = n;
                                settings["network"].Value = n.Name;
                                configFile.Save(ConfigurationSaveMode.Modified);
                                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
                                break;
                            }
                        }
                    }
                }

                if (nic == null) return;

                var host = settings["host"].Value;
                var domain = settings["domain"].Value;
                System.Net.NetworkInformation.IPAddressInformation ip = null;
                foreach (var address in nic.GetIPProperties().UnicastAddresses)
                {
                    if (address.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        ip = address;
                    }
                }

                var skip = false;
                try
                {
                    foreach (var current in Dns.GetHostEntry(host + "." + domain).AddressList)
                    {
                        if (current.Equals(ip.Address))
                        {
                            skip = true;
                            break;
                        }
                    }
                }
                catch (System.Net.Sockets.SocketException)
                {
                }

                if (!skip)
                {
                    var address = ip.Address.ToString();
                    var request = WebRequest.Create(string.Format("http://kghost.info/update?host={0}&ip={1}", host, address));
                    var response = request.GetResponse();
                    var reader = new StreamReader(response.GetResponseStream());
                    this.logger.WriteEntry(string.Format("Update {0} to {1}: {2}", host, address, reader.ReadToEnd()), EventLogEntryType.Information);
                    reader.Close();
                    response.Close();
                }
            }
            catch (Exception e)
            {
                this.logger.WriteEntry(e.ToString(), EventLogEntryType.Error);
            }
        }
    }
}
