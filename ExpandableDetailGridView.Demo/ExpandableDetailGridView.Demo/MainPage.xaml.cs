using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Windows.UI.Xaml.Controls;

namespace ExpandableDetailGridView.Demo
{
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        public List<DataItem> TestItems { get; set; }

        private DetailItem detailItem;

        public DetailItem DetailItem
        {
            get { return detailItem; }
            set
            {
                detailItem = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DetailItem)));
            }
        }

        public MainPage()
        {
            TestItems = DataFactory.LoadData().ToList();
            this.InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        

        private async void ExpandableDetailGridView_SelectedItemChanged(object sender, EventArgs e)
        {
            //DetailItem = await DataFactory.GetDetailItemAsync(item);
        }

        DataItem _SelectedItem;
        public DataItem SelectedItem
        {
            get
            {
                return _SelectedItem;
            }
            set
            {
                _SelectedItem = value;
                LoadDetailItem(value);
            }
        }

        private async void LoadDetailItem(DataItem item)
        {
            if (item != null)
                DetailItem = await DataFactory.GetDetailItemAsync(item);
            else
                DetailItem = null;
        }

        private void Button_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            GV1.SelectedItem = this.TestItems.ElementAt(3);
        }

        private void Button_Tapped2(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            GV2.SelectedItem = this.TestItems.ElementAt(3);
        }
    }
}
