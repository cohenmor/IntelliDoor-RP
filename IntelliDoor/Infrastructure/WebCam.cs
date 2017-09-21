using System;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace IntelliDoor.Infrastructure
{
    class WebCam
    {
        private MediaCapture _mediaCapture;
        private readonly string PHOTO_FILE_NAME = "photo.jpg";

        private WebCam()
        {
            _mediaCapture = new MediaCapture();  
        }

        public static WebCam Create()
        {
            var webCam = new WebCam();
            webCam.Initialize();

            return webCam;
        }

        private void Initialize()
        {
            _mediaCapture.InitializeAsync().AsTask().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public async void StartLiveStreamAsync(CaptureElement element)
        {
            element.Source = _mediaCapture;
            await _mediaCapture.StartPreviewAsync().AsTask().ConfigureAwait(false);
        }

        public async Task<StorageFile> TakePhotoAsync()
        {
            StorageFile photoFile = await KnownFolders.PicturesLibrary.CreateFileAsync(
                                PHOTO_FILE_NAME, CreationCollisionOption.GenerateUniqueName).AsTask().ConfigureAwait(false);
            ImageEncodingProperties imageProperties = ImageEncodingProperties.CreateJpeg();
            await _mediaCapture.CapturePhotoToStorageFileAsync(imageProperties, photoFile).AsTask().ConfigureAwait(false);
            return photoFile;
        }

        public void Dispose()
        {
            _mediaCapture.Dispose();
            _mediaCapture = null;
        }
    }
}
