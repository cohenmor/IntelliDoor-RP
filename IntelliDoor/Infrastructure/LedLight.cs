using System;
using Windows.Devices.Gpio;

namespace IntelliDoor.Infrastructure
{
    public class LedLight
    {
        private const int LED_PIN = 17;  // GPIO (BCM) numbering

        private GpioPin _pin;

        public static LedLight Create()
        {
            var gpio = GpioController.GetDefault();
            
            // Show an error if there is no GPIO controller
            if (gpio == null)
                throw new Exception("There is no GPIO controller on this device.");

            var pin = gpio.OpenPin(LED_PIN);
            pin.Write(GpioPinValue.Low);  // LED is off by default
            pin.SetDriveMode(GpioPinDriveMode.Output);

            return new LedLight(pin);
        }

        private LedLight(GpioPin pin)
        {
            _pin = pin;
        }

        public void TurnOn()
        {
            _pin.Write(GpioPinValue.High);
        }

        public void TurnOff()
        {
            _pin.Write(GpioPinValue.Low);
        }
    }
}
