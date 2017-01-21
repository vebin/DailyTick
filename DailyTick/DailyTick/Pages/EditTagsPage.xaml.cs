using System;
using System.Linq;
using DailyTick.Models;
using Realms;
using Xamarin.Forms;

namespace DailyTick.Pages
{
    public partial class EditTagsPage : Xamarin.Forms.ContentPage
    {
        private RecordPage _parent;
        private ListItem _item;
        private Activity _activity;
        public EditTagsPage(RecordPage parent, ListItem item)
        {
            InitializeComponent();
            _parent = parent;
            _item = item;

            var realm = Realm.GetInstance();
            _activity = realm.Find<Activity>(item.Id);

            InitializeListView();
            InitializeEntry();
        }

        private void InitializeEntry()
        {
            if (_activity.Tags.Count() > 0) {
                entryTags.Text = string.Join(" ", _activity.Tags.Select(t => t.Name));
            }
        }

        private void InitializeListView()
        {
            var realm = Realm.GetInstance();
            var query = from r in realm.All<Tag>()
                        orderby r.LastUsedTime descending
                        select r;
            listView.ItemsSource = query.ToList();
        }

        public void OnSelection(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null) {
                return; // ItemSelected is called on deselection, which results in SelectedItem being set to null
            }

            ((ListView)sender).SelectedItem = null; //uncomment line if you want to disable the visual selection state.
        }

        public void OnTextChanged(object sender, TextChangedEventArgs args)
        {

        }
    }
}
