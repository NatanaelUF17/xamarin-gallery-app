using System;
using Autofac;
using GalleryApp.iOS.Utilities;
using GalleryApp.Models.Interfaces;

namespace GalleryApp.iOS
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
