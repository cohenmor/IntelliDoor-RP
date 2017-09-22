using IntelliDoor.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelliDoor.Logic
{
    class KeyPad
    {
        private static readonly Dictionary<int, string> DIGIT_MAP = new Dictionary<int, string> {
            { 3, "1" },
            { 7, "2" },
            { 11, "3" },
            { 2, "4" },
            { 6, "5" },
            { 10, "6" },
            { 1, "7" },
            { 5, "8" },
            { 9, "9" },
            { 0, "Reset" },
            { 4, "0" },
            { 8, "Bell" }
        };
        private KeyPadTouchDetector keypad;
        public event Action<string> OnPress;
        public event Action<string> OnRing;
        public event Action OnReset;
        private string aptNum;

        public KeyPad()
        {
            keypad = KeyPadTouchDetector.Create();
        }

        public void GetAparmentNumber()
        {
            OnReset?.Invoke();
            aptNum = "";
            keypad.StartListening(ParseInput);
        }

        private void ParseInput(int obj)
        {
            var input = DIGIT_MAP[obj];
            Debug.Write(input);
            switch (input)
            {
                case "Bell":
                    keypad.StopListening();
                    OnRing?.Invoke(aptNum);
                    break;
                case "Reset":
                    OnReset?.Invoke();
                    aptNum = "";
                    break;
                default:
                    OnPress?.Invoke(input);
                    aptNum += input;
                    break;
            }
        }
    }
}
