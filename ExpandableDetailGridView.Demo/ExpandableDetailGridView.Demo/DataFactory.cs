using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Windows.UI;

namespace ExpandableDetailGridView.Demo
{
    public static class DataFactory
    {
        public static IEnumerable<DataItem> LoadData()
        {
            Type t = typeof(Colors);
            System.Reflection.PropertyInfo[] infos = t.GetProperties();
            foreach (System.Reflection.PropertyInfo info in infos)
            {
                if (info.PropertyType == typeof(Color))
                {
                    yield return new DataItem()
                    {
                        Title = info.Name,
                        Background = (Color)info.GetValue(null, null)
                    };
                }
            }
        }

        public static async Task<DetailItem> GetDetailItemAsync(DataItem item)
        {
            await Task.Delay(1000);
            return new DetailItem()
            {
                Title = item.Title,
                Background = item.Background,
                SubTitle = "Foo-" + item.Title + "-Bar"
            };
        }
    }
}
