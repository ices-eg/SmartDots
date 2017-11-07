using System;
using System.ComponentModel;
using SmartDots.ServiceReferenceBroadCast;

namespace SmartDots.Helpers
{
    class BroadCasterCallback : IBroadCastServiceCallback
    {
        //private string userName = Environment.UserName;
        private System.Threading.SynchronizationContext syncContext = AsyncOperationManager.SynchronizationContext;

        private EventHandler _broadcastorCallBackHandler;
        public void SetHandler(EventHandler handler)
        {
            this._broadcastorCallBackHandler = handler;
        }

        public void BroadCastToClients(EventDataType eventData)
        {
            syncContext.Post(new System.Threading.SendOrPostCallback(OnBroadcast), eventData);
        }

        private void OnBroadcast(object eventData)
        {
            this._broadcastorCallBackHandler.Invoke(eventData, null);
        }

 
    }
}
