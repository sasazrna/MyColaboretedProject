﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PrigovorHR.Shared.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class ComplaintOriginalView : ContentView
    {

        public ComplaintOriginalView()
        {
            InitializeComponent();
        }
        public ComplaintOriginalView(Models.ComplaintModel Complaint, Models.ComplaintModel.ComplaintReplyModel LastReply)
        {
            InitializeComponent();

            Acr.UserDialogs.UserDialogs.Instance.ShowLoading();
            Task.Run(() => { Device.BeginInvokeOnMainThread(() => { DisplayData(Complaint); }); });
            lblOriginalComplaint_TextLong.IsVisible = true;
        }

        private void DisplayData(Models.ComplaintModel Complaint)
        {
            var NameSurname = Controllers.LoginRegisterController.LoggedUser.name_surname;

            lblUsername.Text = NameSurname;
            lblOriginalComplaint_TextLong.Text = Complaint.complaint;
            lblNameInitials.Text = NameSurname.Substring(0, 1) + "." + NameSurname.Substring(NameSurname.LastIndexOf(" ") + 1, 1);

            if (!string.IsNullOrEmpty(Complaint.suggestion))
                lytSuggestion.IsVisible = true;

            lytAttachmentsLayout.Children.Clear();
            foreach (var Attachment in Complaint.attachments)
                lytAttachmentsLayout.Children.Add(new AttachmentView(false, Complaint.id, Attachment.id, Attachment.attachment_url));

            lblProblemDateTime.Text = !string.IsNullOrEmpty(Complaint.problem_occurred) ? DateTime.Parse(Complaint.problem_occurred).ToString() : "nedefinirano";
            lblComplaintDateTime.Text = DateTime.Parse( Complaint.created_at).ToString();
            lytLine.IsVisible = false;// !LastReplyId.HasValue;
            Acr.UserDialogs.UserDialogs.Instance.HideLoading();
        }

    }
}
