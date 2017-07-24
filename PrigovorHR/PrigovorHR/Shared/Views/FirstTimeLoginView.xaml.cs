using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Complio.Shared.Views
{
    public partial class FirstTimeLoginView : ContentView
    {
        private Controllers.TAPController TAPController;
        public delegate void SearchIconClickedHandler();
        public event SearchIconClickedHandler SearchIconClickedEvent;

        public FirstTimeLoginView()
        {
            InitializeComponent();
            lblSearch.Text = Views.FontAwesomeLabel.Images.FASearch;
            lblSearch.TextColor = Color.Gray;
            lblSearch.FontSize = 25;

            lblPen.Text = Views.FontAwesomeLabel.Images.FAPencil;
            lblPen.TextColor = Color.Gray;
            lblPen.FontSize = 25;

            lblInfo.Text = Views.FontAwesomeLabel.Images.FAInfo;
            lblInfo.TextColor = Color.Gray;
            lblInfo.FontSize = 25;

            lblHandShake.Text = Views.FontAwesomeLabel.Images.FAHandShake;
            lblHandShake.TextColor = Color.Gray;
            lblHandShake.FontSize = 25;

            TAPController = new Controllers.TAPController(lblSearch);
            TAPController.SingleTaped += TAPController_SingleTaped;

            if (AppGlobal.AppIsComplio == true)
            {
                lblPrig_ComplioHeader.Text = "Kako Complio kao usluga funkcionira";
                lblHeaderDetaild.Text = "Pojednostavili smo proces prigovora kako bi krajnji korisnici imali što bržu i lakšu komunikaciju s gradom/općinom";
                lblFirstTitle.Text = "Korisnik sastavlja prigovor";
                lblFirstDetails.Text = "Putem mobilne aplikacije ili web stranice korisnik piše prigovor ili sugestiju";
                lblSecondTitle.Text = "Obrada prigovora";
                lblSecondDetails.Text = "Tvrtka dobiva informaciju o zaprimljenom prigovoru ili sugestiji i započinje s obradom";
                lblThirdTitle.Text = "Korisnik zaprima odgovor";
                lblThirdDetails.Text = "Complio.me se brine o tome da korisnik bude obaviješten o odgovoru i statusu prigovora ili sugestije korisnika";
                lblFourthTitle.Text = "Complio.me obrađuje podatke";
                lblFourthDetails.Text = "Zadnji korak ali najbitniji je da Complio.me obrađuje podatke i provodi statistiku kako bi javno poduzeće bila upućena u detalje i kako bi se kvaliteta usluge poboljšala";
            }

            else
            {
                lblPrig_ComplioHeader.Text = "Kako Prigovor kao usluga funkcionira";
                lblHeaderDetaild.Text = "Pojednostavili smo proces prigovora kako bi krajnji korisnici imali što bržu i lakšu komunikaciju s tvrtkom";
                lblFirstTitle.Text = "Korisnik sastavlja prigovor";
                lblFirstDetails.Text = "Putem mobilne aplikacije ili web stranice korisnik piše prigovor ili sugestiju";
                lblSecondTitle.Text = "Obrada prigovora";
                lblSecondDetails.Text = "Grad/Općina dobiva informaciju o zaprimljenom prigovoru ili sugestiji i započinje s obradom";
                lblThirdTitle.Text = "Korisnik zaprima odgovor";
                lblThirdDetails.Text = "Prigovor.hr se brine o tome da korisnik bude obaviješten o odgovoru i statusu prigovora ili sugestije korisnika";
                lblFourthTitle.Text = "Prigovor.hr obrađuje podatke";
                lblFourthDetails.Text = "Zadnji korak ali najbitniji je da Prigovor.hr obrađuje podatke i provodi statistiku kako bi tvrtka bila upućena u detalje i kako bi se kvaliteta usluge poboljšala";
            }
        }

        private  void TAPController_SingleTaped(string viewId, View view)
        {
            SearchIconClickedEvent?.Invoke();
          //  await ((Page)Parent).Navigation.PushPopupAsync(new Pages.CompanySearchPage());
        }
    }
}
