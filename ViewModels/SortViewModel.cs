using SportStore.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportStore.ViewModels
{
    public class SortViewModel
    {
        public SortState TitleSort { get; set; } // значение для сортировки по имени
        public SortState PriceSort { get; set; }    // значение для сортировки по возрасту
        public SortState IdSort { get; set; }   // значение для сортировки по компании
        public SortState Current { get; set; }     // значение свойства, выбранного для сортировки
        public bool Up { get; set; }  // Сортировка по возрастанию или убыванию

        public SortViewModel(SortState sortOrder)
        {
            // значения по умолчанию
            TitleSort = SortState.TitleAsc;
            PriceSort = SortState.PriceAsc;
            IdSort = SortState.IdAsc;
            Up = true;

            if (sortOrder == SortState.PriceDesc || sortOrder == SortState.TitleDesc
                || sortOrder == SortState.IdDesc)
            {
                Up = false;
            }

            switch (sortOrder)
            {
                case SortState.TitleDesc:
                    Current = TitleSort = SortState.TitleAsc;
                    break;
                case SortState.PriceAsc:
                    Current = PriceSort = SortState.PriceDesc;
                    break;
                case SortState.PriceDesc:
                    Current = PriceSort = SortState.PriceAsc;
                    break;
                case SortState.IdAsc:
                    Current = IdSort = SortState.IdDesc;
                    break;
                case SortState.IdDesc:
                    Current = IdSort = SortState.IdAsc;
                    break;
                default:
                    Current = TitleSort = SortState.TitleDesc;
                    break;
            }
        }
    }
}
