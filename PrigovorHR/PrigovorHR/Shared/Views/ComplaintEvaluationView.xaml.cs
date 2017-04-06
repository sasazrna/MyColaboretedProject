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
    public partial class ComplaintEvaluationView : ContentView
    {
        private Controllers.TAPController TAPController;
        private View[] AllEvaluationsStars;
        public Dictionary<View, int> StoredEvaluationGrades = new Dictionary<View, int>();
        private const string StarFont = "\xf005";

        public ComplaintEvaluationView()
        {
            InitializeComponent();
            SetStars(null);
        }

        public ComplaintEvaluationView(Models.ComplaintModel complaint)
        {
            InitializeComponent();


            SetStars(complaint);
        }

        private void SetStars(Models.ComplaintModel complaint)
        {
            try
            {
                AllEvaluationsStars = SatisfactionEvaluationLayout.Children.Concat(
                                                                SpeedEvaluationLayout.Children.Concat(
                                                                CommunicationEvaluationLayout.Children))
                                                         .ToArray();

                var Evaluation = Models.ComplaintModel.RefToAllComplaints.user.element_reviews?.SingleOrDefault(er => er.complaint_id == complaint?.id);

                if (Evaluation != null)
                {
                    var Grades = new Dictionary<StackLayout, int?>() {
                { SatisfactionEvaluationLayout, Evaluation.satisfaction },
                { SpeedEvaluationLayout, Evaluation.speed },
                { CommunicationEvaluationLayout, Evaluation.communication_level_user } };

                    foreach (var starParent in AllEvaluationsStars.Cast<FontAwesomeLabel>().GroupBy(g => (StackLayout)g.Parent))
                    {
                        var Grade = Grades[starParent.Key];
                        int starId = 0;
                        foreach (var star in starParent.Where(s => s != starParent.FirstOrDefault()))
                        {
                            star.TextColor = ++starId <= Grade ? Color.Orange : Color.Gray;
                            star.Text = FontAwesomeLabel.Images.FAStar;
                        }
                    }
                }
                else
                {
                    foreach (var star in AllEvaluationsStars.Cast<FontAwesomeLabel>())
                    {
                        star.TextColor = Color.Gray;
                        star.Text = FontAwesomeLabel.Images.FAStar;
                    }
                }

                Reset3.Text = FontAwesomeLabel.Images.FABan;
                Reset2.Text = FontAwesomeLabel.Images.FABan;
                Reset1.Text = FontAwesomeLabel.Images.FABan;

                if (complaint == null)
                {
                    TAPController = new Controllers.TAPController(AllEvaluationsStars);
                    TAPController.SingleTaped += TAPController_SingleTaped;
                }
            }
            catch(Exception ex) { Acr.UserDialogs.UserDialogs.Instance.Alert(ex.ToString()); }
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
