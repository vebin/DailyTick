using System;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
using Realms;
using DailyTick.Models;

namespace DailyTick.Pages
{
    /// <summary>
    /// Popup Page Repository
    /// https://github.com/rotorgames/Rg.Plugins.Popup
    /// </summary>
    public partial class EditSubjectPage : PopupPage
    {
        private RecordPage _parent;
        private ListItem _item;
        private Activity _activity;

        public EditSubjectPage(RecordPage parent, ListItem item)
        {
            InitializeComponent();
            _parent = parent;
            _item = item;

            var realm = Realm.GetInstance();
            _activity = realm.Find<Activity>(item.Id);

            labelHeader.Text = _parent.MakeDateTimeText(_activity) + " 你干什么了？";
            if (!string.IsNullOrEmpty(item.Text)) {
                entrySubject.Text = _activity.Subject;
            }
        }

        public async void OnCancelClicked(object sender, EventArgs args)
        {
            await Navigation.PopPopupAsync();
        }

        public async void OnOkClicked(object sender, EventArgs args)
        {
            if (!string.IsNullOrEmpty(entrySubject.Text)) {
                _parent.UpdateSubject(_item, entrySubject.Text);
            }
            await Navigation.PopPopupAsync();
        }
    }
}
