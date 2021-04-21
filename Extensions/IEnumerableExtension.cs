using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace StoreMVC.Extensions
{
    public static class IEnumerableExtension
    {
        // Создайём метод, который по умолцчанийю будет возвращать тип IEnumerable<SelectListItem>  
        // А принимать в кацчестве параметров дзженерик коллекцийю IEnumerable и int выбранного в списке знацченийя
        public static IEnumerable<SelectListItem> ToSelectListItem<T>(this IEnumerable<T> items, int selectedValue)
        {
            return from item in items
                   select new SelectListItem
                   {
                       Text = item.GetPropertyValue("Name"),
                       Value = item.GetPropertyValue("Id"),
                       Selected = item.GetPropertyValue("Id").Equals(selectedValue.ToString())
                   };
        }

        public static IEnumerable<SelectListItem> ToSelectListItem<T>(this IEnumerable<T> items, string selectedValue)
        {
            if (selectedValue is null)
                selectedValue = string.Empty;

            return from item in items
                   select new SelectListItem
                   {
                       Text = item.GetPropertyValue("Name"),
                       Value = item.GetPropertyValue("Id"),
                       Selected = item.GetPropertyValue("Id").Equals(selectedValue.ToString())
                   };
        }
    }
}
