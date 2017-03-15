using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PrigovorHR.Shared.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class CloseComplaintPage
    {
        private Controllers.TAPController TAPController;
        private View[] AllEvaluationsStars;
        private Dictionary<View, int> StoredEvaluationGrades = new Dictionary<View, int>();
        private const string StarFont = "\xf005";

        public CloseComplaintPage()
        {
            InitializeComponent();


            //imgStar.Text = Views.FontAwesomeLabel.Images.FAStar;
            //imgStar.TextColor = Color.FromHex("#FF7e65");
            //imgReset.Text = Views.FontAwesomeLabel.Images.FABan;
            //imgReset.TextColor = Color.FromHex("#FF7e65");


            AllEvaluationsStars = AnswerEvaluationLayout.Children.Concat(
                                                                        SpeedEvaluationLayout.Children.Concat(
                                                                        CommunicationEvaluationLayout.Children))
                                                                 .ToArray();

            foreach (var star in AllEvaluationsStars.Cast<Views.FontAwesomeLabel>())
            {
                star.TextColor = Color.Gray;
                star.Text = Views.FontAwesomeLabel.Images.FAStar;
            }

            Reset3.Text= Views.FontAwesomeLabel.Images.FABan;
            Reset2.Text = Views.FontAwesomeLabel.Images.FABan;
            Reset1.Text = Views.FontAwesomeLabel.Images.FABan;
          
            TAPController = new Controllers.TAPController( AllEvaluationsStars);
            TAPController.SingleTaped += TAPController_SingleTaped;
        }

        private void TAPController_SingleTaped(string viewId, View view)
        {
            ColorTheStar(view);
        }

        private void ColorTheStar(View SelectedEvaluationStar)
        {
            var StarParent = (StackLayout)SelectedEvaluationStar.Parent;
            int StarValue = 0;
            bool StarIsFound = false;
            bool ResetStars = SelectedEvaluationStar == StarParent.Children.First();

            if (ResetStars)
                StoredEvaluationGrades.Remove(StarParent);

            foreach (var EvaluationStar in StarParent.Children.Where(child => child != StarParent.Children.First()).Cast<Views.FontAwesomeLabel>())
            {
                if (ResetStars)
                {
                    EvaluationStar.TextColor = Color.Gray;
                    continue;
                }

                if (EvaluationStar != SelectedEvaluationStar & !StarIsFound)
                {
                    StarValue++;
                    EvaluationStar.TextColor = Color.Orange;
                }
                else
                {
                    if (!StarIsFound)
                    {
                        StarIsFound = true;
                        StarValue++;
                        EvaluationStar.TextColor = Color.Orange;

                        if (!StoredEvaluationGrades.ContainsKey(StarParent))
                            StoredEvaluationGrades.Add(StarParent, StarValue);
                        else
                            StoredEvaluationGrades[StarParent] = StarValue;
                    }
                    else EvaluationStar.TextColor = Color.Gray;
                }
            }
        }
    }
}
