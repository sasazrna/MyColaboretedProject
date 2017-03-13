
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PrigovorHR.Shared.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class NavigationBar : ContentView
    {
        private Controllers.TAPController TAPController;
        internal delegate void BackButtonPressedHandler();
        internal event BackButtonPressedHandler BackButtonPressedEvent;
        public Image imgBack { get { return ImgBack ?? new Image(); } }
        public Label lblNavigationTitle { get { return LblNavigationTitle ?? new Label(); } }

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

        public async void InitBackButtonPressed()
        {
            await imgBack.RotateTo(90, 75);
            BackButtonPressedEvent?.Invoke();
        }
    }
}
