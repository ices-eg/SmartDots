using System;
using System.DirectoryServices.AccountManagement;

namespace SmartDots.Helpers
{
    public interface IResponseViewer
    {
        void HandleBroadcast(object sender, EventArgs e, ServiceReferenceBroadCast.EventDataType message);

        void SetTextBoxValue(string tekst);
    }
    class BroadCasterClient
    {
        public ServiceReferenceBroadCast.BroadCastServiceClient Client { get; set; }
        private static string username = UserPrincipal.Current.DisplayName;
        private readonly string _applicationName = "SmartDots";
        private readonly object _lock = new object();

        public BroadCasterClient()
        {
            RegisterClient();
            System.Threading.Thread myThread = new System.Threading.Thread(delegate ()
            {
                KeepAlive();
            });
            myThread.Start();
        }



        /// <summary>
        /// connecteerd met de service
        /// </summary>
        private void RegisterClient()
        {
            if ((this.Client != null))
            {
                this.Client.Abort();
                this.Client = null;
            }

            BroadCasterCallback cb = new BroadCasterCallback();
            cb.SetHandler(this.HandleBroadcast);

            System.ServiceModel.InstanceContext context = new System.ServiceModel.InstanceContext(cb);
            this.Client = new ServiceReferenceBroadCast.BroadCastServiceClient(context);

            this.Client.RegisterClient(username + _applicationName);
        }

        private void KeepAlive()
        {
            try
            {
                System.Threading.Thread.Sleep(100000);
                Client.KeepAlive();
            }
            catch (Exception)
            {
                Client = null;

                Reconnect();
            }
            KeepAlive();
        }

        private void Reconnect()
        {
            try
            {
                RegisterClient();

            }
            catch (Exception)
            {
                System.Threading.Thread.Sleep(10000);
                Reconnect();
            }
        }

        public void HandleBroadcast(object sender, EventArgs e)
        {
            

        }

    }
}
