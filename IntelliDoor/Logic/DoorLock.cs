using IntelliDoor.Infrastructure;
using System;
using System.Threading;
using Windows.UI.Xaml;

namespace IntelliDoor.Logic
{
    class DoorLock
    {
        private LedLight led;  // simulates locking/unlocking the door
        private Timer timer;

        private DoorLock()
        {
            //timer = new DispatcherTimer()
            //{
            //    Interval = TimeSpan.FromMilliseconds(3000)  // the door will open for 5 seconds
            //};
            //timer.Tick += Lock;
            timer = new Timer(Lock, null, Timeout.Infinite, Timeout.Infinite);
        }

        public static DoorLock Create()
        {
            var doorLock = new DoorLock() { led = LedLight.Create() };
            return doorLock;
        }

        public void Unlock()
        {
            //timer.Start();
            timer.Change(0, 3000);
            led.TurnOn();
        }

        public void Lock(object sender)
        {
            // , object e
            //timer.Stop();
            timer.Change(Timeout.Infinite, Timeout.Infinite);
            led.TurnOff();
        }

    }
}
