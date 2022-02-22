using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GalleryApp.Models.Enums;

namespace GalleryApp.Models.Interfaces
{
    public interface IPhotoImporter
    {
        Task<ObservableCollection<Photo>> Get(int start, int count, Quality quality = Quality.Low);
        Task<ObservableCollection<Photo>> Get(List<string> filenames, Quality quality = Quality.Low);
    }
}
