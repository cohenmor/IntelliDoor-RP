using Imgur.API.Authentication.Impl;
using Imgur.API.Endpoints.Impl;
using Imgur.API.Models;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace IntelliDoor.Infrastructure
{
    class ImgurApi
    {
        private const string CLIENT_ID = "e6376674cfd42a7";
        private const string CLIENT_SECRET = "d43effe737e76af710504d8f527b68d9db915ba7";
        private ImgurClient client;

        public ImgurApi()
        {
            client = new ImgurClient(CLIENT_ID, CLIENT_SECRET);
        }

        public async Task<string> UploadImageFromFileAsync(StorageFile photo)
        {
            var endpoint = new ImageEndpoint(client);
            IImage image = await Task.Run(async () => 
            {
                using (var fs = new FileStream(photo.Path, FileMode.Open))
                {
                    image = await endpoint.UploadImageStreamAsync(fs).ConfigureAwait(false);
                }
                return image;
            }).ConfigureAwait(false);
            
            Debug.WriteLine("Image uploaded. Image Url: " + image.Link);
            return image.Link;
        }
    }
}
