using SportStore.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportStore.ViewModels
{
    public class OutfitEdit : OutfitCreate
    {
        public int Id { get; set; }
        public string ExistingPhotoPath { get; set; }
    }
}
