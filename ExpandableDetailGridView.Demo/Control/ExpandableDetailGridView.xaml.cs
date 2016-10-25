﻿using System;
using System.Collections;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using System.Linq;

namespace Control
{
    public sealed partial class ExpandableDetailGridView : UserControl
    {
        public int ItemHeight
        {
            get { return (int)GetValue(ItemHeightProperty); }
            set { SetValue(ItemHeightProperty, value); }
        }

        public static readonly DependencyProperty ItemHeightProperty =
            DependencyProperty.Register("ItemHeight", typeof(int), 
                typeof(ExpandableDetailGridView), new PropertyMetadata(0));


        public int ItemWidth
        {
            get { return (int)GetValue(ItemWidthProperty); }
            set { SetValue(ItemWidthProperty, value); }
        }

        public static readonly DependencyProperty ItemWidthProperty =
            DependencyProperty.Register("ItemWidth", typeof(int), 
                typeof(ExpandableDetailGridView), new PropertyMetadata(0));


        public int DetailRowSpan
        {
            get { return (int)GetValue(DetailRowSpanProperty); }
            set { SetValue(DetailRowSpanProperty, value); }
        }

        public static readonly DependencyProperty DetailRowSpanProperty =
            DependencyProperty.Register("DetailRowSpan", typeof(int), 
                typeof(ExpandableDetailGridView), new PropertyMetadata(1));

        public Brush DetailBackground
        {
            get { return (Brush)GetValue(DetailBackgroundProperty); }
            set { SetValue(DetailBackgroundProperty, value); }
        }

        public static readonly DependencyProperty DetailBackgroundProperty =
            DependencyProperty.Register("DetailBackground", typeof(Brush), 
                typeof(ExpandableDetailGridView), new PropertyMetadata(null));


        public IEnumerable Items
        {
            get { return (IEnumerable)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register("Items", typeof(IEnumerable), 
                typeof(ExpandableDetailGridView), new PropertyMetadata(null, OnItemsChanged));

        private static void OnItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ExpandableDetailGridView)d).HandleItemsChanged();
        }

        public bool IsOverviewDataSameAsDetailData
        {
            get { return (bool)GetValue(IsOverviewDataSameAsDetailDataProperty); }
            set { SetValue(IsOverviewDataSameAsDetailDataProperty, value); }
        }

        public static readonly DependencyProperty IsOverviewDataSameAsDetailDataProperty =
            DependencyProperty.Register("IsOverviewDataSameAsDetailData", typeof(bool), 
                typeof(ExpandableDetailGridView), new PropertyMetadata(true));


        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), 
                typeof(ExpandableDetailGridView), new PropertyMetadata(null));


        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register("SelectedIndex", typeof(int), 
                typeof(ExpandableDetailGridView), new PropertyMetadata(-1));


        public object DetailItem
        {
            get { return (object)GetValue(DetailItemProperty); }
            set { SetValue(DetailItemProperty, value); }
        }

        public static readonly DependencyProperty DetailItemProperty =
            DependencyProperty.Register("DetailItem", typeof(object), 
                typeof(ExpandableDetailGridView), new PropertyMetadata(null, OnDetailItemChanged));

        private static void OnDetailItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ExpandableDetailGridView)d).HandleDetailItemChanged();
        }

        public DataTemplate DetailItemTemplate
        {
            get
            {
                return (DataTemplate)GetValue(DetailItemTemplateProperty);
            }
            set { SetValue(DetailItemTemplateProperty, value); }
        }

        public static readonly DependencyProperty DetailItemTemplateProperty =
            DependencyProperty.Register("DetailItemTemplate", typeof(DataTemplate), 
                typeof(ExpandableDetailGridView), new PropertyMetadata(null));


        public DataTemplate GridItemTemplate
        {
            get { return (DataTemplate)GetValue(GridItemTemplateProperty); }
            set { SetValue(GridItemTemplateProperty, value); }
        }

        public static readonly DependencyProperty GridItemTemplateProperty =
            DependencyProperty.Register("GridItemTemplate", typeof(DataTemplate), 
                typeof(ExpandableDetailGridView), new PropertyMetadata(null));


        public bool Animate
        {
            get { return (bool)GetValue(AnimateProperty); }
            set { SetValue(AnimateProperty, value); }
        }

        public static readonly DependencyProperty AnimateProperty =
            DependencyProperty.Register("Animate", typeof(bool), 
                typeof(ExpandableDetailGridView), new PropertyMetadata(false));


        public DetailGridItem DetailGridItem
        {
            get { return (DetailGridItem)GetValue(DetailGridItemProperty); }
            set { SetValue(DetailGridItemProperty, value); }
        }

        public static readonly DependencyProperty DetailGridItemProperty =
            DependencyProperty.Register("DetailGridItem", typeof(DetailGridItem), 
                typeof(ExpandableDetailGridView), new PropertyMetadata(null));



        public ExpandableDetailGridView()
        {
            this.InitializeComponent();
        }

        private void HandleItemsChanged()
        {
            //ToDo: Observable collection
            GridView.Items.Clear();
            foreach (var item in Items)
            {
                GridView.Items.Add(item);
            }
        }

        private void HandleDetailItemChanged()
        {
            if (DetailGridItem != null)
                DetailGridItem.DataContext = DetailItem;
        }

        private async void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem != null && e.ClickedItem != SelectedItem)
            {
                var wg = GridView.ItemsPanelRoot as VariableSizedWrapGrid;

                var itemsPerRow = Math.Floor(wg.ActualWidth / wg.ItemWidth);

                var currentIndexOfDetailItem = GridView.Items.IndexOf(DetailGridItem);

                var itemIndex = GridView.Items.IndexOf(e.ClickedItem);

                if (currentIndexOfDetailItem > -1 && currentIndexOfDetailItem < itemIndex)
                {
                    itemIndex--;
                }

                var row = Math.Min(((int)(itemIndex / itemsPerRow) + 1) * itemsPerRow, 
                    GridView.Items.Count);

                if (DetailGridItem == null)
                {
                    DetailGridItem = new DetailGridItem();
                    DetailGridItem.DetailItemTemplate = DetailItemTemplate;
                    DetailGridItem.Grid = this;

                    Binding bgBinding = new Binding();
                    bgBinding.Path = new PropertyPath(nameof(DetailBackground));
                    bgBinding.Source = this;
                    DetailGridItem.SetBinding(DetailGridItem.BackgroundProperty, bgBinding);
                }
                else
                {
                    if (currentIndexOfDetailItem != row)
                    {
                        GridView.Items.Remove(DetailGridItem);

                        if (Animate)
                            await Task.Delay(55);
                    }
                    DetailGridItem.DataContext = null;
                }

                if (IsOverviewDataSameAsDetailData)
                {
                    DetailItem = e.ClickedItem;

                    //Else = user should do it manualy by listening to the SelectedIndexChanged 
                    //or something like that and set the DetailItem manually
                }

                if (!GridView.Items.Contains(DetailGridItem))
                {
                    if (row < GridView.Items.Count(i => !(i is DetailGridItem)))
                    {
                        GridView.Items.Insert((int)row, DetailGridItem);
                    }
                    else
                    {
                        GridView.Items.Add(DetailGridItem);
                    }
                }

                var itemContainer = GridView.ContainerFromItem(DetailGridItem) as GridViewItem;
                if (itemContainer != null)
                {
                    itemContainer.SetValue(VariableSizedWrapGrid.ColumnSpanProperty, itemsPerRow);
                    itemContainer.SetValue(VariableSizedWrapGrid.RowSpanProperty, DetailRowSpan);
                }

                SelectedIndex = itemIndex;
                SelectedItem = e.ClickedItem;
                SelectedIndexChanged?.Invoke(this, null);
                SelectedItemChanged?.Invoke(this, null);
            }
        }

        public event EventHandler SelectedIndexChanged;
        public event EventHandler SelectedItemChanged;

        private void GridView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            HideDetail();
        }

        public void HideDetail()
        {
            GridView.Items.Remove(DetailGridItem);
            GridView.SelectedItem = null;
        }
    }
}