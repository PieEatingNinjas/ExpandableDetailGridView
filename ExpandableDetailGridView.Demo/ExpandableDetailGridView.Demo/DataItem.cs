using Windows.UI;
using Windows.UI.Xaml.Media;

namespace ExpandableDetailGridView.Demo
{
    public class DataItem
    {
        public string Title
        {
            get;
            set;
        }

        public Color Background { get; set; }
        public SolidColorBrush BackgroundBrush
        {
            get { return new SolidColorBrush(Background); }
        }
    }
}
