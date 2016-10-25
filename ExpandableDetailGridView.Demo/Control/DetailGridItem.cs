using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace Control
{
    public class DetailGridItem : GridViewItem
    {
        public DataTemplate DetailItemTemplate
        {
            get { return (DataTemplate)GetValue(DetailItemTemplateProperty); }
            set { SetValue(DetailItemTemplateProperty, value); }
        }

        public static readonly DependencyProperty DetailItemTemplateProperty =
            DependencyProperty.Register("DetailItemTemplate", 
                typeof(DataTemplate), typeof(DetailGridItem), new PropertyMetadata(null));

        public ExpandableDetailGridView Grid { get; set; }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var button = GetTemplateChild("ButtonHide") as ButtonBase;
            if (button != null)
            {
                button.Tapped += DetailGridItem_Tapped;
            }
        }

        private void DetailGridItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Grid.HideDetail();
        }
    }
}
