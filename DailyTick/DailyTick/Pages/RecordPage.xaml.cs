using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DailyTick.Models;
using DailyTick.Views;
using Realms;
using Rg.Plugins.Popup.Extensions;
using Xamarin.Forms;

namespace DailyTick.Pages
{
    public class ListItem
    {
        public string Id { get; set; }
        public ImageSource Duration { get; set; }
        public string Text { get; set; }
        public string Detail { get; set; }
    }

    public partial class RecordPage : ContentPage
    {
        ObservableCollection<ListItem> _items = new ObservableCollection<ListItem>();
        private CircleButtonView _circle;
        private ApplicationSettings _settings;

        public RecordPage()
        {
            InitializeComponent();
            InitializeCircelButton();
            InitializeListView();

            Device.StartTimer(new TimeSpan(0, 0, 5), () => {
                _circle.Update();
                return true;
            });
        }

        private void InitializeListView()
        {
            var realm = Realm.GetInstance();

            var query = from r in realm.All<Activity>()
                        orderby r.StartTime descending
                        select r;

            foreach (var activity in query) {
                _items.Add(ActivityToListItem(activity));
            }
            listView.ItemsSource = _items;
        }

        private void InitializeCircelButton()
        {
            _circle = new CircleButtonView();

            relativeLayout.Children.Add(_circle, Constraint.RelativeToParent((parent) => {
                return 0.5 * parent.Width - parent.Height * 0.8 * 0.5;
            }), Constraint.RelativeToParent((parent) => {
                return 0.5 * (parent.Height - parent.Height * 0.8);
            }), Constraint.RelativeToParent((parent) => {
                return parent.Height * 0.8;
            }), Constraint.RelativeToParent((parent) => {
                return parent.Height * 0.8;
            }));

            var realm = Realm.GetInstance();

            try {
                _settings = realm.All<ApplicationSettings>().First();
            }
            catch (Exception ex) {
                realm.Write(() => {
                    _settings = new ApplicationSettings {
                        LastStartTime = DateTimeOffset.Now,
                    };
                    realm.Add(_settings);
                });
            }

            _circle.Clicked += OnButtonClicked;
            _circle.DurationStart = _settings.LastStartTime;
            _circle.Update();
        }

        public async void OnSelection(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null) {
                return; // ItemSelected is called on deselection, which results in SelectedItem being set to null
            }

            await EditListItem(e.SelectedItem as ListItem);

            ((ListView)sender).SelectedItem = null; //uncomment line if you want to disable the visual selection state.
        }

        public void OnButtonClicked(object sender, EventArgs args)
        {
            var startTime = _circle.DurationStart;
            var stopTime = DateTimeOffset.Now;

            if (stopTime - startTime > TimeSpan.FromSeconds(60)) {
                _circle.DurationStart = stopTime;
                _circle.Update();

                var realm = Realm.GetInstance();
                realm.Write(() => {
                    var newActivity = new Activity {
                        Id = Guid.NewGuid().ToString(),
                        StartTime = startTime,
                        StopTime = stopTime,
                        Subject = "",
                        Memo = ""
                    };
                    realm.Add(newActivity);

                    // Insert to list top.
                    _items.Insert(0, ActivityToListItem(newActivity));

                    _settings.LastStartTime = stopTime;
                });
            } else {
                // popup a message
            }
        }

        private async Task EditListItem(ListItem item)
        {
            var selection = await DisplayActionSheet("选择操作", "取消", null, "修改活动主题", "修改活动标签", "拆分时间段", "与上一日间段合并", "与下一时间段合并");
            switch (selection) {
                case "修改活动主题":
                    var editSubjectPage = new EditSubjectPage(this, item);
                    await Navigation.PushPopupAsync(editSubjectPage);
                    break;
                case "修改活动标签":
                    var editTagsPage = new EditTagsPage(this, item);
                    await Navigation.PushModalAsync(editTagsPage);
                    break;
                default:
                    break;
            }
        }

        public void UpdateSubject(ListItem item, string subject)
        {
            var realm = Realm.GetInstance();
            var activity = realm.Find<Activity>(item.Id);
            realm.Write(() => {
                activity.Subject = subject;
            });

            // Update ListView
            var index = _items.IndexOf(item);
            _items.RemoveAt(index);

            item.Text = string.Format("{0} {1}", MakeDateTimeText(activity), subject);

            _items.Insert(index, item);
        }

        private ListItem ActivityToListItem(Activity activity)
        {
            var durationText = (activity.StopTime - activity.StartTime).ToString(@"hh\:mm");
            string subject = string.IsNullOrEmpty(activity.Subject) ? "点了一下" : activity.Subject;

            return new ListItem {
                Id = activity.Id,
                Text = string.Format("{0} {1}", MakeDateTimeText(activity), subject),
                Detail = MakeDetail(activity),
                Duration = ImageSource.FromStream(() => {
                    var stream = new MemoryStream();
                    var durationImageSource = DependencyService.Get<IDurationImageSource>();
                    durationImageSource.Generate(stream, durationText);
                    stream.Seek(0, SeekOrigin.Begin);

                    return stream;
                }),
            };
        }

        public string MakeDetail(Activity activity)
        {
            if (activity.Tags.Count() > 0) {
                return string.Join(" ", activity.Tags.Select(t => "#" + t).ToList());
            } else {
                return "还没加标签";
            }
        }

        public string MakeDateTimeText(Activity activity)
        {
            var today = DateTimeOffset.Now.Date;
            var startDate = activity.StartTime.Date;
            var timeSpan = DateTimeOffset.Now.Date - activity.StartTime.Date;

            string dateStr;
            switch(timeSpan.Days) {
                case 0:
                    dateStr = "今天";
                    break;
                case 1:
                    dateStr = "昨天";
                    break;
                default:
                    dateStr = activity.StartTime.ToString(@"yyyy-mm-dd");
                    break;
            }
            string timeStr = activity.StartTime.ToString(@"hh\:mm");

            return string.Format("{0} {1}", dateStr, timeStr);
        }
    }
}
