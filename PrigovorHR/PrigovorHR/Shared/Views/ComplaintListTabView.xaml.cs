using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace PrigovorHR.Shared.Views
{
    public partial class ComplaintListTabView 
    {
     //   public static ComplaintListTabView ReferenceToView;
        public enum enumTabs { ActiveComplaints, ClosedComplaints }
        private enumTabs SelectedTab { get; set; }

        public delegate void SelectedTabChangedHandler(enumTabs SelectedTab);
        public event SelectedTabChangedHandler SelectedTabChangedEvent;

        public ComplaintListTabView()
        {
            InitializeComponent();
             new Controllers.TAPController(lblActiveComplaints, lblClosedComplaints).SingleTaped += ComplaintListTabView_SingleTaped;
            lytClosedComplaintsUnderline.IsVisible = false;
            // ReferenceToView = this;

            lblActiveComplaints.Text = '\uf06d'.ToString();
            lblActiveComplaints.TextColor = Color.FromHex("#FF7e65");
            lblClosedComplaints.Text = '\uf187'.ToString();
            lblClosedComplaints.TextColor = Color.FromHex("#FF7e65");

        }

        private void ComplaintListTabView_SingleTaped(string viewId, View view)
        {
            if (view == lblActiveComplaints)
                SelectedTab = enumTabs.ActiveComplaints;
            else SelectedTab = enumTabs.ClosedComplaints;

            lytActiveComplaintsUnderline.IsVisible = SelectedTab == enumTabs.ActiveComplaints;
            lytClosedComplaintsUnderline.IsVisible = !lytActiveComplaintsUnderline.IsVisible;
        
            SelectedTabChangedEvent?.Invoke(SelectedTab);
        }
    }
}
