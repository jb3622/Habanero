// LastInputMonitor Class
// Revision 1 (2004-12-21)
// Copyright (C) 2004 Dennis Dietrich
//
// Released unter the BSD License
// http://www.opensource.org/licenses/bsd-license.php

using System;
using System.ComponentModel;
using System.Timers;

namespace Disney.Menu
{
    public class LastInputMonitor : IInactivityMonitor
    {
        #region Private Fields

        private double interval = 100.0;
        private uint lastTickCount = 0;

        private Win32LastInputInfo lastInputBuffer = new Win32LastInputInfo();
        private DateTime lastInputDate = DateTime.Now;

        private bool disposed = false;
        private bool enabled = false;

        private bool monitorMouse = true;
        private bool monitorKeyboard = true;

        private bool timeElapsed = false;
        private bool reactivated = false;

        private Timer pollingTimer = null;

        #endregion Private Fields

        public LastInputMonitor()
        {
            lastInputBuffer.cbSize = (uint)Win32LastInputInfo.SizeOf;
            pollingTimer = new Timer();
            pollingTimer.AutoReset = true;
            pollingTimer.Elapsed += new ElapsedEventHandler(TimerElapsed);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                disposed = true;
                if (disposing)
                {
                    Delegate[] delegateBuffer = null;

                    pollingTimer.Elapsed -= new ElapsedEventHandler(TimerElapsed);
                    pollingTimer.Dispose();

                    delegateBuffer = Elapsed.GetInvocationList();
                    foreach (ElapsedEventHandler item in delegateBuffer)
                        Elapsed -= item;
                    Elapsed = null;

                    delegateBuffer = Reactivated.GetInvocationList();
                    foreach (EventHandler item in delegateBuffer)
                        Reactivated -= item;
                    Reactivated = null;
                }
            }
        }

        ~LastInputMonitor()
        {
            Dispose(false);
        }

    #region Public Events

        public event ElapsedEventHandler Elapsed;

        public event EventHandler Reactivated;

    #endregion Public Events

    #region Public Properties

        public virtual double Interval
        {
            get
            {
                return interval;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Invalid interval (value less than zero)");
                interval = value;
            }
        }

        public virtual bool Enabled
        {
            get
            {
                return enabled;
            }
            set
            {
                pollingTimer.Enabled = enabled = value;
                lastInputDate = DateTime.Now;
            }
        }

        public virtual bool MonitorMouseEvents
        {
            get
            {
                return monitorMouse;
            }
            set
            {
                monitorMouse = value;
            }
        }

        public virtual bool MonitorKeyboardEvents
        {
            get
            {
                return monitorKeyboard;
            }
            set
            {
                monitorKeyboard = value;
            }
        }

        /// <summary>
        /// Object to use for synchronization (the execution of
        /// event handlers will be marshalled to the thread that
        /// owns the synchronization object)
        /// </summary>
        public virtual ISynchronizeInvoke SynchronizingObject
        {
            get
            {
                return pollingTimer.SynchronizingObject;
            }
            set
            {
                pollingTimer.SynchronizingObject = value;
            }
        }

    #endregion Properties

        #region Public Methods

        public virtual void Reset()
        {
            if (disposed)
                throw new ObjectDisposedException("Object has already been disposed");

            if (enabled)
            {
                lastInputDate = DateTime.Now;
                timeElapsed = false;
                reactivated = false;
            }
        }

        #endregion Public Methods

        #region Proteced Methods

        protected void OnElapsed(ElapsedEventArgs e)
        {
            timeElapsed = true;
            if (Elapsed != null && enabled && (monitorKeyboard || monitorMouse))
                Elapsed(this, e);
        }

        protected void OnReactivated(EventArgs e)
        {
            reactivated = true;
            if (Reactivated != null && enabled && (monitorKeyboard || monitorMouse))
                Reactivated(this, e);
        }

        #endregion Proteced Methods

        #region Private Methods

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (pollingTimer.SynchronizingObject != null)
                if (pollingTimer.SynchronizingObject.InvokeRequired)
                {
                    pollingTimer.SynchronizingObject.BeginInvoke(
                        new ElapsedEventHandler(TimerElapsed),
                        new object[] { sender, e });
                    return;
                }
            if (User32.GetLastInputInfo(out lastInputBuffer))
            {
                if (lastInputBuffer.dwTime != lastTickCount)
                {
                    if (timeElapsed && !reactivated)
                    {
                        OnReactivated(new EventArgs());
                        Reset();
                    }
                    lastTickCount = lastInputBuffer.dwTime;
                    lastInputDate = DateTime.Now;
                }
                else if (!timeElapsed && (monitorMouse || monitorKeyboard))
                    if (DateTime.Now.Subtract(lastInputDate).TotalMilliseconds > interval)
                        OnElapsed(e);
            }
        }

        #endregion Private Methods
    }
}