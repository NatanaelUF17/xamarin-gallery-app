using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Windows.Input;
using GalleryApp.Models;
using GalleryApp.Models.Interfaces;
using GalleryApp.ViewModels.Helpers;
using Xamarin.Forms;

namespace GalleryApp.ViewModels
{
    public class GalleryViewModel : ViewModel
    {
        private int itemsAdded;

        private readonly IPhotoImporter _photoImporter;
        private readonly ILocalStorage _localStorage;

        public GalleryViewModel(IPhotoImporter photoImporter, ILocalStorage localStorage)
        {
            _photoImporter = photoImporter;
            _localStorage = localStorage;
            Task.Run(Initialize);
        }

        public ObservableCollection<Photo> Photos { get; set; }

        private async Task Initialize()
        {
            IsBusy = true;
            Photos = await _photoImporter.Get(0, 20);
            RaisePropertyChanged(nameof(Photos));

            Photos.CollectionChanged += Photos_CollectionChanged;
            await Task.Delay(3000);
            IsBusy = false;
        }

        #region COMMANDS
        private int currentStartIndex = 0;
        public ICommand LoadMorePhotos => new Command(async () =>
        {
            currentStartIndex += 20;
            itemsAdded = 0;
            var collection = await _photoImporter.Get(currentStartIndex, 20);
            collection.CollectionChanged += PhotoCollection_CollectionChanged;
        });

        public ICommand AddFavorites => new Command<List<Photo>>((photos) =>
        {
            foreach (var photo in photos)
            {
                _localStorage.Store(photo.Filename);
            }
            MessagingCenter.Send(this, "FavoritesAdded");
        });
        #endregion

        #region EVENTS
        private void Photos_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                IsBusy = false;
                Photos.CollectionChanged -= Photos_CollectionChanged;
            }
        }

        private void PhotoCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach (Photo photo in e.NewItems)
            {
                itemsAdded++;
                Photos.Add(photo);
            }

            if (itemsAdded == 20)
            {
                var collection = (ObservableCollection<Photo>)sender;
                collection.CollectionChanged -= PhotoCollection_CollectionChanged;
            }
        }
        #endregion
    }
}
