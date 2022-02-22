using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Android.Database;
using Android.Provider;
using GalleryApp.Models;
using GalleryApp.Models.Enums;
using GalleryApp.Models.Interfaces;
using Xamarin.Forms.Platform.Android;

namespace GalleryApp.Droid.Utilities
{
    public class PhotoImporter : IPhotoImporter
    {
        private bool hasCheckedPermission;
        private string[] result;

        public bool ContinueWithPermission(bool granted)
        {
            if (!granted)
            {
                return false;
            }

            Android.Net.Uri imageUri =
                MediaStore.Images.Media.ExternalContentUri;

            var cursor =
                MainActivity.Current.ContentResolver.Query(imageUri, null,
                MediaStore.Images.ImageColumns.MimeType + "=? or " +
                MediaStore.Images.ImageColumns.MimeType + "=?",
                new string[] { "image/jpeg", "image/png" },
                MediaStore.Images.ImageColumns.DateModified);

            var paths = new List<string>();

            while (cursor.MoveToNext())
            {
                string path = cursor.GetString(cursor.GetColumnIndex(
                    MediaStore.Images.ImageColumns.Data));

                paths.Add(path);
            }

            result = paths.ToArray();

            hasCheckedPermission = true;

            return true;
        }

        public Task<ObservableCollection<Photo>> Get(int start, int count, Quality quality = Quality.Low)
        {
            throw new NotImplementedException();
        }

        public Task<ObservableCollection<Photo>> Get(List<string> filenames, Quality quality = Quality.Low)
        {
            throw new NotImplementedException();
        }
    }
}
