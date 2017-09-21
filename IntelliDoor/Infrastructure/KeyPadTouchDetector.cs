using MPR121;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;
using Windows.UI.Xaml;

namespace IntelliDoor.Infrastructure
{
    public class KeyPadTouchDetector
    {
        private MPR121Handler _handler;
        private Timer _timer;
        private Action<int> _action;    // an action to perform on each touched key

        public static KeyPadTouchDetector Create()
        {
            var keypad = new KeyPadTouchDetector();
            keypad.Initialize();

            return keypad;
        }

        private KeyPadTouchDetector()
        {
            _handler = new MPR121Handler();
            //_timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(200) };
            //_timer.Tick += WriteToStream;
            _timer = new Timer(WriteToStream, null, Timeout.Infinite, Timeout.Infinite);
        }

        public void StartListening(Action<int> action)
        {
            _action = action;
            //_timer.Start();
            _timer.Change(0, 200);
        }

        public void StopListening()
        {
            //if (_timer.IsEnabled)
            //    _timer.Stop();
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private void WriteToStream(object sender)
        {
            // , object e
            ushort touched = ReadRegister16(0x00); // read registers 0 and 1
            var touchedBits = touched & 0x0FFF;    // there are only 12 keys

            if (touchedBits > 0)  // a key was touched
            {
                int touchedBitNum = (int) (Math.Log(touchedBits, 2.0));  // index of touched key
                _action(touchedBitNum);  // perform the action on the touched key
            }
        }

        // read 16 bits starting from register reg
        private ushort ReadRegister16(byte reg)
        {
            byte[] readBuffer = new byte[2];

            _handler.Connection.WriteRead(new byte[] { reg }, readBuffer);

            // turn two bytes into short
            return (ushort)(readBuffer[0] + (readBuffer[1] << 8));
        }

        private void Initialize()
        {
            // todo: handle async
            string queryString = I2cDevice.GetDeviceSelector();
            var deviceList = DeviceInformation.FindAllAsync(queryString).AsTask().GetAwaiter().GetResult();
            bool connected = _handler.OpenConnection(deviceList[0].Id).ConfigureAwait(false).GetAwaiter().GetResult();
            Debug.Write(connected);
        }
    }
}
