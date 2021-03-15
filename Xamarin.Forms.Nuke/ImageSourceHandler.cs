using System.Threading;
using System.Threading.Tasks;

using Foundation;

using UIKit;

using Xamarin.Forms;
using Xamarin.Forms.Nuke;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportImageSourceHandler(typeof(FileImageSource), typeof(ImageSourceHandler))]
[assembly: ExportImageSourceHandler(typeof(UriImageSource), typeof(ImageSourceHandler))]

namespace Xamarin.Forms.Nuke
{
    [Preserve(AllMembers = true)]
    public class ImageSourceHandler : IImageSourceHandler, IAnimationSourceHandler
    {
        internal static readonly FileImageSourceHandler DefaultFileImageSourceHandler = new FileImageSourceHandler();

        private static readonly ImageLoaderSourceHandler DefaultUriImageSourceHandler = new ImageLoaderSourceHandler();

        private static readonly StreamImagesourceHandler DefaultStreamImageSourceHandler = new StreamImagesourceHandler();

        private static readonly FontImageSourceHandler DefaultFontImageSourcehandler = new FontImageSourceHandler();


        public async Task<UIImage> LoadImageAsync(
            ImageSource imageSource,
            CancellationToken cancellationToken = new CancellationToken(),
            float scale = 1)
        {
            var result = await NukeHelper.LoadViaNuke(imageSource, cancellationToken, scale);
            if (result == null)
               result = await LoadPlaceholderAsync();

            return result;
        }

        public Task<FormsCAKeyFrameAnimation> LoadImageAnimationAsync(
            ImageSource imageSource,
            CancellationToken cancellationToken = new CancellationToken(),
            float scale = 1)
        {
            FormsHandler.Debug(() => $"Delegating animation of {imageSource} to default Xamarin.Forms handler");

            if (imageSource is UriImageSource)
            {
                return DefaultUriImageSourceHandler.LoadImageAnimationAsync(imageSource, cancellationToken, scale);
            }

            return DefaultFileImageSourceHandler.LoadImageAnimationAsync(imageSource, cancellationToken, scale);
        }


        private static Task<UIImage> LoadPlaceholderAsync()
        {
            switch (FormsHandler.PlaceholderImageSource)
            {
                case StreamImageSource streamImageSource:
                    FormsHandler.Warn($"loading placeholder from resource");
                    return DefaultStreamImageSourceHandler.LoadImageAsync(streamImageSource);
                    ;
                case FontImageSource fontImageSource:
                    FormsHandler.Warn($"loading placeholder from Font");
                    return DefaultFontImageSourcehandler.LoadImageAsync(fontImageSource);
                default:
                    FormsHandler.Warn($"no valid placeholder found");
                    return null;
            }
        }
    }
}