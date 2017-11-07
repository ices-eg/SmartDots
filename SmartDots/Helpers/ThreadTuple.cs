using System;
using System.Threading;
using System.Threading.Tasks;

namespace SmartDots.Helpers
{
    public class ThreadTuple
    {
        public CancellationTokenSource CancelToken { get; private set; }

        public delegate void ThreadEventHandler(object sender, ThreadEventArgs e);
        public event EventHandler BeforeStart;
        public event ThreadEventHandler BeginAnimation;
        public event ThreadEventHandler BeginAction;
        public event EventHandler PostAction;
        public event EventHandler PostAnimation;
        public event EventHandler Completed;
        public event ThreadEventHandler OnError;

        public ThreadTuple() : this(new CancellationTokenSource()) { }
        public ThreadTuple(CancellationTokenSource token)
        {
            CancelToken = token;
        }

        public async Task Start()
        {
            if (BeginAnimation == null) throw new Exception("No work assigned to event BeginAnimation.");
            if (BeginAction == null) throw new Exception("No work assigned to event BeginAction.");

            BeforeStart?.Invoke(this, new EventArgs());

            var e = new ThreadEventArgs();
            e.Token = CancelToken;

            try
            {
                var ui = new Task(() => BeginAnimation.Invoke(this, e));
                ui.Start();

                var code = new Task(() => BeginAction.Invoke(this, e));
                code.Start();

                await code;
                CancelToken.Cancel();

                PostAction?.Invoke(this, new EventArgs());
                PostAnimation?.Invoke(this, new EventArgs());
                Completed?.Invoke(this, new EventArgs());
            }
            catch (Exception ex) //TODO: ensure errors within tasks get caught here
            {
                CancelToken.Cancel();
                e.Error = ex;
                OnError?.Invoke(this, e);
            }
        }
    }

    public class ThreadEventArgs
    {
        public CancellationTokenSource Token { get; set; }
        public object Value { get; set; }
        public Exception Error { get; set; }
        public string ErrorMessage => Error?.Message;
    }
}
