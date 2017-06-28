using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Complio.Shared.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FindAndFilterMessages : ContentPage
    {
        private Controllers.SearchController SC;
        public FindAndFilterMessages()
        {
            InitializeComponent();
            ListOfComplaintsView.IsFilterAndSearchActivated = true;
            SC = new Controllers.SearchController(SearchCompanyElement, null);
            SC.SearchActivated += SC_SearchActivated;
            SC = new Controllers.SearchController(SearchMessageText, null);
            SC.SearchActivated += SC_SearchActivated;

                dpDateFrom.Date = new DateTime(DateTime.Now.Year, 1, 1);
                  }

        private void SC_SearchActivated(string searchtext, bool IsDirectTag)
        {
            InitSearchAndFilter();
        }

        private void sw_Toggled(object sender, ToggledEventArgs e)
        {
            InitSearchAndFilter();
        }

        private void dpDate_DateSelected(object sender, DateChangedEventArgs e)
        {
            InitSearchAndFilter();
        }

        private void InitSearchAndFilter()
        {
            ListOfComplaintsView.FilterAndFindMessages(string.IsNullOrEmpty(SearchCompanyElement.Text) ? string.Empty : SearchCompanyElement.Text,
                                                        dpDateFrom.Date, dpDateTo.Date, swActive.IsToggled, swClosed.IsToggled,
                                                        string.IsNullOrEmpty(SearchMessageText.Text) ? string.Empty : SearchMessageText.Text);
        }

        private void SearchButtonPressed(object sender, EventArgs e)
        {
            InitSearchAndFilter();
        }
    }
}