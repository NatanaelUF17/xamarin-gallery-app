using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Foundation;
using GalleryApp.Models;
using GalleryApp.Models.Enums;
using GalleryApp.Models.Interfaces;
using Photos;

namespace GalleryApp.iOS.Utilities
{
    public class PhotoImporter : IPhotoImporter
    {
        private PHAsset[] results;

        private async Task<bool> Import()
        {
            var status = await PHPhotoLibrary.RequestAuthorizationAsync();

            if (status != PHAuthorizationStatus.Authorized)
            {
                return false;
            }

            results = PHAsset.FetchAssets(PHAssetMediaType.Image, null)
                .Select(x => (PHAsset)x).ToArray();

            return true;
        }

        private void RequestImage(ObservableCollection<Photo> photos, PHAsset asset, string filename, PHImageRequestOptions options)
        {
            PHImageManager.DefaultManager.RequestImageForAsset(asset,
                PHImageManager.MaximumSize, PHImageContentMode.AspectFill, options, (image, info) =>
                {
                    using (NSData imageData = image.AsPNG())
                    {
                        var bytes = new byte[imageData.Length];
                        System.Runtime.InteropServices.Marshal.Copy
                        (imageData.Bytes, bytes, 0, Convert.ToInt32(imageData.Length));

                        var photo = new Photo()
                        {
                            Filename = filename,
                            Bytes = bytes
                        };

                        photos.Add(photo);
                    }
                });
        }

        public async Task<ObservableCollection<Photo>> Get(int start, int count, Quality quality = Quality.Low)
        {
            if (results == null)
            {
                var succeded = await Import();
                if (!succeded)
                {
                    return new ObservableCollection<Photo>();
                }
            }

            var photos = new ObservableCollection<Photo>();
            var options = new PHImageRequestOptions()
            {
                NetworkAccessAllowed = true,
                DeliveryMode = quality == Quality.Low ? PHImageRequestOptionsDeliveryMode.FastFormat : PHImageRequestOptionsDeliveryMode.HighQualityFormat
            };

            Index startIndex = start;
            Index endIndex = start + count;

            if (endIndex.Value >= results.Length)
            {
                endIndex = results.Length - 1;
            }

            if (startIndex.Value > endIndex.Value)
            {
                return new ObservableCollection<Photo>();
            }

            foreach (var asset in results[startIndex..endIndex])
            {
                var filename = (NSString)asset.ValueForKey((NSString)"filename");
                RequestImage(photos, asset, filename, options);
            }

            return photos;
        }

        public async Task<ObservableCollection<Photo>> Get(List<string> filenames, Quality quality = Quality.Low)
        {
            if (results == null)
            {
                var succeded = await Import();
                if (!succeded)
                {
                    return new ObservableCollection<Photo>();
                }
            }

            var photos = new ObservableCollection<Photo>();

            var options = new PHImageRequestOptions()
            {
                NetworkAccessAllowed = true,
                DeliveryMode = quality == Quality.Low ? PHImageRequestOptionsDeliveryMode.FastFormat : PHImageRequestOptionsDeliveryMode.HighQualityFormat
            };

            foreach (var asset in results)
            {
                var filename = (NSString)asset.ValueForKey((NSString)"filename");

                if (filename.Contains(filename))
                {
                    RequestImage(photos, asset, filename, options);
                }
            }

            return photos;
        }
    }
}
