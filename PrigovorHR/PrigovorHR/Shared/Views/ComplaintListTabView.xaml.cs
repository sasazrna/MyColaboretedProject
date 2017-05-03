using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PrigovorHR.Shared.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class ComplaintListTabView 
    {
        public static ComplaintListTabView ReferenceToView;
        public enum Tabs { ActiveComplaints=1, ClosedComplaints=2, DraftComplaints=3, UnsentComplaints=4 }
        private Dictionary<FontAwesomeLabel, Tabs> LabelsToTabsConnection;
        private Tabs SelectedTab { get; set; }

        public delegate void SelectedTabChangedHandler(Tabs SelectedTab);
        public event SelectedTabChangedHandler SelectedTabChangedEvent;

        private Dictionary<bool, Color> SelectedUnselectedColor = 
            new Dictionary<bool, Color>() { { false, Color.FromHex("#79b2d6") }, { true, Color.FromHex("#FF7e65") } };

        public ComplaintListTabView()
        {
            InitializeComponent();

            new Controllers.TAPController(lytActiveComplaints, lytClosedComplaints, lytStoredComplaints, lytUnsentComplaints).SingleTaped += ComplaintListTabView_SingleTaped;
            ReferenceToView = this;

            lblActiveComplaints.Text = '\uf06d'.ToString();
            lblActiveComplaints.TextColor = SelectedUnselectedColor[true];
            lblClosedComplaints.Text = '\uf187'.ToString();
            lblClosedComplaints.TextColor = SelectedUnselectedColor[false];
            lblStoredComplaints.Text = FontAwesomeLabel.Images.FAAmbulance;
            lblStoredComplaints.TextColor = SelectedUnselectedColor[false];
            lblUnsentComplaints.Text = FontAwesomeLabel.Images.FAAndroid;
            lblUnsentComplaints.TextColor = SelectedUnselectedColor[false];

            LabelsToTabsConnection = new Dictionary<FontAwesomeLabel, Tabs>() { { lblActiveComplaints, Tabs.ActiveComplaints },
                                                                                { lblClosedComplaints, Tabs.ClosedComplaints },
                                                                                { lblStoredComplaints, Tabs.DraftComplaints },
                                                                                { lblUnsentComplaints, Tabs.UnsentComplaints } };
        }

        public void InvokeSelectedTabChanged(Tabs SelectedTab)
        {
            ComplaintListTabView_SingleTaped("ChangedByOutsideView", LabelsToTabsConnection.First(l => l.Value == SelectedTab).Key);
        }

        private void ComplaintListTabView_SingleTaped(string viewId, View view)
        {
                var SelectedLabel = view.GetType() == typeof(StackLayout) ? ((FontAwesomeLabel)((StackLayout)view).Children.FirstOrDefault()) : (FontAwesomeLabel)view;

                SelectedTab = LabelsToTabsConnection[SelectedLabel];
                foreach (var label in LabelsToTabsConnection.Keys)
                    label.TextColor = SelectedUnselectedColor[label == SelectedLabel];

                if (viewId != "ChangedByOutsideView")
                {
                    SelectedTabChangedEvent?.Invoke(SelectedTab);
                    ListOfComplaintsView_BasicUser.ReferenceToView.ChangeVisibleLayout(SelectedTab, true);
                }
        }
    }
}
