using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace PrigovorHR.Shared.Pages
{
    public partial class CloseComplaintPage
    {
        private Controllers.TAPController TAPController;
        private View[] AllEvaluationsStars;
        private Dictionary<View, int> StoredEvaluationGrades = new Dictionary<View, int>();

        public CloseComplaintPage()
        {
            InitializeComponent();

            AllEvaluationsStars = AnswerEvaluationLayout.Children.Concat(
                                                                        SpeedEvaluationLayout.Children.Concat(
                                                                        CommunicationEvaluationLayout.Children))
                                                                 .ToArray();

            foreach (var star in AllEvaluationsStars)
                star.BackgroundColor = Color.Gray;

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

            foreach (var EvaluationStar in StarParent.Children.Where(child => child != StarParent.Children.First()))
            {
                if (ResetStars)
                {
                    EvaluationStar.BackgroundColor = Color.Gray;
                    continue;
                }

                if (EvaluationStar != SelectedEvaluationStar & !StarIsFound)
                {
                    StarValue++;
                    EvaluationStar.BackgroundColor = Color.Orange;
                }
                else
                {
                    if (!StarIsFound)
                    {
                        StarIsFound = true;
                        StarValue++;
                        EvaluationStar.BackgroundColor = Color.Orange;

                        if (!StoredEvaluationGrades.ContainsKey(StarParent))
                            StoredEvaluationGrades.Add(StarParent, StarValue);
                        else
                            StoredEvaluationGrades[StarParent] = StarValue;
                    }
                    else EvaluationStar.BackgroundColor = Color.Gray;
                }
            }
        }
    }
}
