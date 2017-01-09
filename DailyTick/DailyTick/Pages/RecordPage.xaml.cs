using System;
using Xamarin.Forms;

namespace DailyTick
{
    public class ListItem
    {
        public string title { get; set; }
        public string subtitle { get; set; }
    }

    public partial class RecordPage : ContentPage
    {
        public RecordPage()
        {
            InitializeComponent();

            listView.ItemsSource = new ListItem[] {
                new ListItem { title="Title1", subtitle="SubTitle1" },
                new ListItem { title="Title1", subtitle="SubTitle1" },
                new ListItem { title="Title1", subtitle="SubTitle1" },
                new ListItem { title="Title1", subtitle="SubTitle1" },
                new ListItem { title="Title1", subtitle="SubTitle1" },
                new ListItem { title="Title1", subtitle="SubTitle1" },
                new ListItem { title="Title1", subtitle="SubTitle1" },
                new ListItem { title="Title1", subtitle="SubTitle1" },
                new ListItem { title="Title1", subtitle="SubTitle1" },
                new ListItem { title="Title1", subtitle="SubTitle1" },
                new ListItem { title="Title1", subtitle="SubTitle1" },
                new ListItem { title="Title1", subtitle="SubTitle1" },
                new ListItem { title="Title1", subtitle="SubTitle1" },
                new ListItem { title="Title1", subtitle="SubTitle1" },
            };
        }

        public void OnMerge(object sender, EventArgs e)
        {

        }

        public void OnSplit(object sender, EventArgs e)
        {

        }

        public void OnEdit(object sender, EventArgs e)
        {

        }
    }
}
