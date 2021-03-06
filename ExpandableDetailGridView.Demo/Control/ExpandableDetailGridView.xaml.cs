﻿using System;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Control
{
    public sealed partial class ExpandableDetailGridView : UserControl
    {
        #region DependencyProperties
        public int ItemHeight
        {
            get { return (int)GetValue(ItemHeightProperty); }
            set { SetValue(ItemHeightProperty, value); }
        }

        public static readonly DependencyProperty ItemHeightProperty =
            DependencyProperty.Register(nameof(ItemHeight), typeof(int), 
                typeof(ExpandableDetailGridView), new PropertyMetadata(0));


        public int ItemWidth
        {
            get { return (int)GetValue(ItemWidthProperty); }
            set { SetValue(ItemWidthProperty, value); }
        }

        public static readonly DependencyProperty ItemWidthProperty =
            DependencyProperty.Register(nameof(ItemWidth), typeof(int), 
                typeof(ExpandableDetailGridView), new PropertyMetadata(0));


        public int DetailRowSpan
        {
            get { return (int)GetValue(DetailRowSpanProperty); }
            set { SetValue(DetailRowSpanProperty, value); }
        }

        public static readonly DependencyProperty DetailRowSpanProperty =
            DependencyProperty.Register(nameof(DetailRowSpan), typeof(int), 
                typeof(ExpandableDetailGridView), new PropertyMetadata(1));

        public Brush DetailBackground
        {
            get { return (Brush)GetValue(DetailBackgroundProperty); }
            set { SetValue(DetailBackgroundProperty, value); }
        }

        public static readonly DependencyProperty DetailBackgroundProperty =
            DependencyProperty.Register(nameof(DetailBackground), typeof(Brush), 
                typeof(ExpandableDetailGridView), new PropertyMetadata(null));


        public IEnumerable Items
        {
            get { return (IEnumerable)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register(nameof(Items), typeof(IEnumerable), 
                typeof(ExpandableDetailGridView), new PropertyMetadata(null, 
                    (d,e) => ((ExpandableDetailGridView)d).HandleItemsChanged(e.OldValue as IEnumerable)));


        public bool IsOverviewDataSameAsDetailData
        {
            get { return (bool)GetValue(IsOverviewDataSameAsDetailDataProperty); }
            set { SetValue(IsOverviewDataSameAsDetailDataProperty, value); }
        }

        public static readonly DependencyProperty IsOverviewDataSameAsDetailDataProperty =
            DependencyProperty.Register(nameof(IsOverviewDataSameAsDetailData), typeof(bool),
                typeof(ExpandableDetailGridView), new PropertyMetadata(true));


        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set
            {
                SetValue(SelectedItemProperty, value);
            }
        }

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(nameof(SelectedItem), typeof(object),
                typeof(ExpandableDetailGridView), new PropertyMetadata(null, 
                    (d,e) => ((ExpandableDetailGridView)d).HandleSelectedItemChanged()));


        public object DetailItem
        {
            get { return (object)GetValue(DetailItemProperty); }
            set { SetValue(DetailItemProperty, value); }
        }

        public static readonly DependencyProperty DetailItemProperty =
            DependencyProperty.Register(nameof(DetailItem), typeof(object),
                typeof(ExpandableDetailGridView), new PropertyMetadata(null, 
                    (d,e) => ((ExpandableDetailGridView)d).HandleDetailItemChanged()));

        public DataTemplate DetailItemTemplate
        {
            get
            {
                return (DataTemplate)GetValue(DetailItemTemplateProperty);
            }
            set { SetValue(DetailItemTemplateProperty, value); }
        }

        public static readonly DependencyProperty DetailItemTemplateProperty =
            DependencyProperty.Register(nameof(DetailItemTemplate), typeof(DataTemplate),
                typeof(ExpandableDetailGridView), new PropertyMetadata(null));


        public DataTemplate GridItemTemplate
        {
            get { return (DataTemplate)GetValue(GridItemTemplateProperty); }
            set { SetValue(GridItemTemplateProperty, value); }
        }

        public static readonly DependencyProperty GridItemTemplateProperty =
            DependencyProperty.Register(nameof(GridItemTemplate), typeof(DataTemplate),
                typeof(ExpandableDetailGridView), new PropertyMetadata(null));


        public bool Animate
        {
            get { return (bool)GetValue(AnimateProperty); }
            set { SetValue(AnimateProperty, value); }
        }

        public static readonly DependencyProperty AnimateProperty =
            DependencyProperty.Register(nameof(Animate), typeof(bool),
                typeof(ExpandableDetailGridView), new PropertyMetadata(false));


        public DetailGridItem DetailGridItem
        {
            get { return (DetailGridItem)GetValue(DetailGridItemProperty); }
            set { SetValue(DetailGridItemProperty, value); }
        }

        public static readonly DependencyProperty DetailGridItemProperty =
            DependencyProperty.Register(nameof(DetailGridItem), typeof(DetailGridItem),
                typeof(ExpandableDetailGridView), new PropertyMetadata(null));
        #endregion

        bool selectedItemChangedInternally;

        public event EventHandler SelectedItemChanged;

        public ExpandableDetailGridView()
        {
            this.InitializeComponent();
        }

        private void HandleSelectedItemChanged()
        {
            if (!selectedItemChangedInternally)
            {
                if (SelectedItem != null)
                {
                    OpenDetailAsync(SelectedItem);
                    GridView.SelectedItem = SelectedItem;
                }
            }
        }

        private void HandleItemsChanged(IEnumerable oldValue)
        {
            //If old value was ObservableCollection 
            //=> no longer interested in its CollectionChanged event
            if(oldValue is INotifyCollectionChanged)
            {
                ((INotifyCollectionChanged)oldValue).CollectionChanged -=
                    ExpandableDetailGridView_CollectionChanged;
            }

            LoadGridItems();

            if (Items is INotifyCollectionChanged)
            {
                //If current collection is ObservableCollection
                //=> we need to listen to the CollectionChanged event in order to process the changes
                ((INotifyCollectionChanged)Items).CollectionChanged += 
                    ExpandableDetailGridView_CollectionChanged;
            }
        }

        private void LoadGridItems()
        {
            GridView.Items.Clear();

            foreach (var item in Items)
            {
                GridView.Items.Add(item);
            }
        }

        private void ExpandableDetailGridView_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if(e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var item in e.NewItems)
                {
                    GridView.Items.Add(item);
                }
            }
            else if(e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var item in e.OldItems)
                {
                    //if current SelectedItem is removed, Hide detail.
                    if (item == SelectedItem)
                        HideDetail();

                    GridView.Items.Remove(item);
                }
            }
            else if(e.Action == NotifyCollectionChangedAction.Reset)
            {
                LoadGridItems();
            }
            else if(e.Action == NotifyCollectionChangedAction.Move)
            {
                //ToDo?
            }
            else if(e.Action == NotifyCollectionChangedAction.Replace)
            {
                //ToDo?
            }
        }

        private void HandleDetailItemChanged()
        {
            if (DetailGridItem != null)
                DetailGridItem.DataContext = DetailItem;
        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem != SelectedItem) //Making sure we don't re-open what's already open
            {
                OpenDetailAsync(e.ClickedItem);
            }
        }

        private async Task OpenDetailAsync(object selectedItem)
        {
            if (selectedItem != null)
            {
                var wg = GridView.ItemsPanelRoot as VariableSizedWrapGrid;

                var itemsPerRow = Math.Floor(wg.ActualWidth / wg.ItemWidth);

                var currentIndexOfDetailItem = GridView.Items.IndexOf(DetailGridItem);

                var itemIndex = GridView.Items.IndexOf(selectedItem);

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
                    DetailItem = selectedItem;

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

                selectedItemChangedInternally = true;
                SelectedItem = selectedItem;
                SelectedItemChanged?.Invoke(this, null);
                selectedItemChangedInternally = false;
            }
        }

        private void GridView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            HideDetail();
        }

        public void HideDetail()
        {
            GridView.Items.Remove(DetailGridItem);
            GridView.SelectedItem = null;
            SelectedItem = null;
            DetailItem = null;
            SelectedItemChanged?.Invoke(this, null);
        }

        private void GridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(GridView.SelectedItem is DetailGridItem) 
            {
                //When the user taps/clicks the DetailItem, it becomes the SelectedItem of the GridView
                //The DetailGridItem may not be the selected item in the GridView because in that case
                //the 'arrow' of the selected GridItem won't be shown (as this arrow is only shown 
                //on the SelectedItem of the GridView). 
                //So we have to reset the GridView's SelectedItem to the SelectedItem
                GridView.SelectedItem = SelectedItem;
            }
        }
    }
}
