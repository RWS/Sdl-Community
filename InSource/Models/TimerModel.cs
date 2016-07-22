using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Timers;
using Sdl.Community.InSource.Annotations;

namespace Sdl.Community.InSource.Models
{
   public  class TimerModel//:INotifyPropertyChanged
   {
       private bool _hasTimer;
       private int _minutes;
        //private Timer _timer;
        //private int _timeLeft;
        //private Persistence _persistence;


       

        public bool HasTimer
       {
           get { return _hasTimer; }
           set
           {
               _hasTimer = value;
               
           }
       }

       public int Minutes
       {
           get { return _minutes; }
           set
           {
               _minutes = value;
              //  OnTheTimeChanged(_minutes);
           }
       }

       // public delegate void TimerChangedHandler(object sender, EventArgs e);

       //public delegate void EventHandler(object sender, EventArgs e);
       // //public event TimerChangedHandler TheTimeChanged;

       // //protected void OnTheTimeChanged(int newTime)
       // //{
       // //    TheTimeChanged?.Invoke(newTime);
       // //}
       // internal void TimeChanged(object sender, EventArgs e)
       //{
           
       //}
    }
}
