﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace PrigovorHR.Shared.Views
{
    public partial class ComplaintCoversationHeaderView : ContentView
    {
        public ComplaintCoversationHeaderView()
        {
            InitializeComponent();
        }

        public void SetHeaderInfo(string ContactPerson, string StoreName, bool Replying)
        {
            lblNameOfContactPerson.Text = ContactPerson;
            lblStoreName.Text = StoreName;
            imgSend.Text = "\uf2c6";
            imgSend2.Text = "\uf2c6";

            lytSendResponse.IsVisible = Replying;
        }
    }
}
