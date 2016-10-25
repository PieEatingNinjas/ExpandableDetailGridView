using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace ExpandableDetailGridView.Demo
{
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        public IEnumerable<DataItem> TestItems { get; set; }

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
            TestItems = DataFactory.LoadData();
            this.InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        

        private async void ExpandableDetailGridView_SelectedItemChanged(object sender, EventArgs e)
        {
            await Task.Delay(1000); //Do some backend calls or whatever
            var item = ((Control.ExpandableDetailGridView)sender).SelectedItem as DataItem;
            this.DetailItem = new DetailItem()
            {
                Title = item.Title,
                Background = item.Background,
                SubTitle = "Foo-" + item.Title + "-Bar" 
            };
        }
    }
}
