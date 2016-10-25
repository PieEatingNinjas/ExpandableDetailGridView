using Windows.UI;
using Windows.UI.Xaml.Media;

namespace ExpandableDetailGridView.Demo
{
    public class DetailItem
    {
        public string Title { get; set; }

        public string SubTitle { get; set; }

        public Color Background { get; set; }
        public SolidColorBrush BackgroundBrush
        {
            get { return new SolidColorBrush(Background); }
        }
    }
}
