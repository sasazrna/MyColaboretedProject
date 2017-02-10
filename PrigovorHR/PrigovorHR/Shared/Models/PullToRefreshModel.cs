using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using PrigovorHR.Shared.Views;
using Xamarin.Forms;

namespace PrigovorHR.Shared.Models
{
    class PullToRefreshModel: INotifyPropertyChanged
    {
        public delegate void PulledEventHandler();
        public event PulledEventHandler Pulled;
        public PullToRefreshModel()
        {
            //this.page = page;
        }
        bool isBusy;

        public bool IsBusy
        {
            get { return isBusy; }
            set
            {
                if (isBusy == value)
                    return;

                isBusy = value;
                OnPropertyChanged("IsBusy");
            }
        }

        ICommand refreshCommand;
        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand RefreshCommand
        {
            get {return refreshCommand ?? (refreshCommand = new Command(() =>  ExecuteRefreshCommand())); }
        }

        void ExecuteRefreshCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            Pulled?.Invoke();
        }

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged == null)
                return;

            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
