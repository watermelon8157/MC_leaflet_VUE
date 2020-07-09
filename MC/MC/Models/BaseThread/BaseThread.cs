using System.Threading; 

namespace RCS.Models
{
    /// <summary>
    /// Thread is a sealed class you can't inherit from it. So you should create your own BaseThread class.
    /// </summary>
    public abstract class BaseThread
    { 

        private Thread _thread;
        private object _locker = new object();

        public BaseThread() { _thread = new Thread(new ThreadStart(this.RunThread)); }
        public void Start()
        {
            lock (_locker)
            {
                if (!this.IsAlive)
                {
                    _thread = new Thread(new ThreadStart(this.RunThread));
                }
                _thread.Start();
            }
        }

        public void Join() { _thread.Join(); }

        public bool IsAlive { get { return _thread.IsAlive; } }

        /// <summary>
        /// abstract RunThread method
        /// </summary>
        public abstract void RunThread();

    }

}