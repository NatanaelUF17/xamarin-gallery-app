using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GalleryApp.Models.Interfaces
{
    public interface ILocalStorage
    {
        Task Store(string filename);
        Task<List<string>> Get();
    }
}
