using Windows.UI.Xaml.Controls;
using System.Diagnostics;
using IntelliDoor.Infrastructure;
using IntelliDoor.Logic;
using System.Threading;
using System.Threading.Tasks;
using System;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace IntelliDoor
{

    public sealed partial class MainPage : Page
    {
        private WebCam webCam;
        private KeyPad keypad;
        private DoorLock doorLock;
        private ManualResetEvent resetEvent = new ManualResetEvent(false);

        public MainPage()
        {
            this.InitializeComponent();
            webCam = WebCam.Create();
            webCam.StartLiveStreamAsync(liveStreamElement);  // display camera's live stream in UI

            keypad = new KeyPad();
            keypad.OnPress += UpdateApartmentTextBoxAsync;  // UI
            keypad.OnRing += HandleVisitorAsync;
            keypad.OnReset += ResetApartmentTextBoxAsync;   // UI

            doorLock = DoorLock.Create();

            // wait for a visitor to enter an apartment number
            Task a = Task.Run( () => {
                while (true)
                {
                    resetEvent.Reset();
                    ResetAptNumElement();
                    keypad.GetAparmentNumber();
                    resetEvent.WaitOne();
                }
            }
            );
        }

        private async void ResetAptNumElement()
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                aptNumElement.Header = "Welcome! Please insert apartment #";
                aptNumElement.Text = "";
            });
        }

        private async void UpdateApartmentTextBoxAsync(string input)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                aptNumElement.Text += input;
                Debug.Write(input);
            });
        }

        private async void ResetApartmentTextBoxAsync()
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                aptNumElement.Text = "";
            });
        }

        private async void HandleVisitorAsync(string aptNum)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                aptNumElement.Header = "Smile to the camera :)";
            });
            var url = CaptureImage();
            // todo: send aptNum and url to server and get reply
            doorLock.Unlock();
            resetEvent.Set();
        }

        private string CaptureImage()
        {
            // capture snapshot of visitor
            var photo = webCam.TakePhotoAsync().GetAwaiter().GetResult();

            // upload the photo to imgur (to get url)
            ImgurApi client = new ImgurApi();
            var url = client.UploadImageFromFileAsync(photo).GetAwaiter().GetResult();

            return url;
        }
    }
}
