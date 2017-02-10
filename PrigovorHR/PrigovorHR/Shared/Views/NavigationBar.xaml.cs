using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace PrigovorHR.Shared.Views
{
    public partial class NavigationBar : ContentView
    {
        private Controllers.TAPController TAPController;
        internal delegate void BackButtonPressedHandler();
        internal event BackButtonPressedHandler BackButtonPressedEvent;

        public NavigationBar()
        {
            InitializeComponent();

            TAPController = new Controllers.TAPController(imgBack);
            TAPController.SingleTaped += (async (string viewId, View view) =>
            {
                await imgBack.RotateTo(90, 75);
                BackButtonPressedEvent?.Invoke();
            });
        }
    }
}
