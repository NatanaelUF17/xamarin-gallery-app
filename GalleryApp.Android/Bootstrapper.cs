using System;
using Autofac;
using GalleryApp.Droid.Utilities;
using GalleryApp.Models.Interfaces;

namespace GalleryApp.Droid
{
    public class Bootstrapper : GalleryApp.Bootstrapper
    {
        protected override void Initialize()
        {
            base.Initialize();

            ContainerBuilder.RegisterType<PhotoImporter>().As<IPhotoImporter>().SingleInstance();
        }
    }
}
