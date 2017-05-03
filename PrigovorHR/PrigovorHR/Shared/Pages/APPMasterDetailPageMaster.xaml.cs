using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PrigovorHR.Shared.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class APPMasterDetailPageMaster 
    {
        public ListView ListView => ListViewMenuItems;

        public APPMasterDetailPageMaster()
        {
            InitializeComponent();
            BindingContext = new APPMasterDetailPageMasterViewModel();
        }

        class APPMasterDetailPageMasterViewModel : INotifyPropertyChanged
        {
            public ObservableCollection<APPMasterDetailPageMenuItem> MenuItems { get; }
            public APPMasterDetailPageMasterViewModel()
            {
                MenuItems = new ObservableCollection<APPMasterDetailPageMenuItem>(new[]
                {
                    new APPMasterDetailPageMenuItem { Id = 0, Title = "Moj profil", TargetType = typeof(ProfilePage) },
                    new APPMasterDetailPageMenuItem { Id = 1, Title = "Aktivni prigovori" },
                    new APPMasterDetailPageMenuItem { Id = 2, Title = "Zatvoreni prigovori" },
                    new APPMasterDetailPageMenuItem { Id = 3, Title = "Skice" },
                    new APPMasterDetailPageMenuItem { Id = 4, Title = "Neposlani prigovori" },
                    new APPMasterDetailPageMenuItem { Id = 5, Title = "Kontakt", TargetType=typeof(ContactUsPage) },
                    new APPMasterDetailPageMenuItem { Id = 6, Title = "Odjava" },
                });
            }
            public event PropertyChangedEventHandler PropertyChanged;
            void OnPropertyChanged([CallerMemberName]string propertyName = "") =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));


        }
    }
}
