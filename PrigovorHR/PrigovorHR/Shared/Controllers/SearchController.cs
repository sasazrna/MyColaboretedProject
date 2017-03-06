//SearchController funkcija čiji je cilj podignuti event kada korisnik prestane tipkati u search polje
//Autor: Vedran Mikuličić, SAMO-RAZVOJ d.o.o. 
//Datum starta: 11.10.2016
//Datum zadnje izmjene:

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using PrigovorHR.Shared.Views;

namespace PrigovorHR.Shared.Controllers
{
    class SearchController:IDisposable
    {
        public delegate void SearchActivatedHandler(string searchtext, bool isQRCoded);
        public delegate void SearchDeactivatedHandler();
        public event SearchActivatedHandler SearchActivated;
        public event SearchDeactivatedHandler SearchDeactivated;

        public bool isTyping = false;
        public bool isSpecial = false;//označavati će dali se nešto specifično pretražuje, treba definirati
        public bool stopTextChangedEvent = false;
        public bool isQRTextActive = false;
        private SearchBar SearchBarField;
        private Entry EntryBarField;

        private int NumberOfTextChangesBeforeSearchStart = 0;
        private int NumberOfActivatedTimers = 0;
        private int WaitBeforeActivation = 500;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SearchBarField">Proslijedi searchbar polje u koje se unosi tekst</param>
        /// <param name="WaitBeforeActivation">Opcionalno podesi broj milisekunda prije nego se aktivira pretraživanje, defaultno je 500</param>
        public SearchController(SearchBar SearchBarField, Entry EntryBarField, int WaitBeforeActivation = 1000)
        {
            if (SearchBarField != null)
                SearchBarField.TextChanged += SearchBarField_TextChanged;

            if (EntryBarField != null)
                EntryBarField.TextChanged += SearchBarField_TextChanged;

            this.WaitBeforeActivation = WaitBeforeActivation;

            this.SearchBarField = SearchBarField;
            this.EntryBarField = EntryBarField;
        }

        #region search function
        private void SearchBarField_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (stopTextChangedEvent | e.NewTextValue == "@" | e.NewTextValue == "#" | e.NewTextValue == "#@" | e.NewTextValue == "@#") return;

            if (e.NewTextValue != e.OldTextValue & !string.IsNullOrWhiteSpace(e.NewTextValue))
            {
                isTyping = true;
                NumberOfTextChangesBeforeSearchStart++;
                string typedtext = e.NewTextValue.Replace("#", "");
                typedtext = typedtext.Replace("@", "");

                Device.StartTimer(new TimeSpan(0, 0, 0, 0, WaitBeforeActivation), () =>
                {
                    NumberOfActivatedTimers++;
                    if (NumberOfActivatedTimers == NumberOfTextChangesBeforeSearchStart)
                    {
                        NumberOfActivatedTimers = 0;
                        NumberOfTextChangesBeforeSearchStart = 0;
                        isTyping = false;
                        if (!isQRTextActive || (isQRTextActive & e.NewTextValue != string.Empty))
                        {
                            SearchActivated?.Invoke(typedtext, isQRTextActive);
                            if (isQRTextActive)
                            {
                                if (SearchBarField != null)
                                    SearchBarField.Text = string.Empty;
                                else EntryBarField.Text = string.Empty;
                            }
                        }
                        else if (isQRTextActive & e.NewTextValue == string.Empty)
                            isQRTextActive = false;
                    }
                    return false;//Stop the timer
                });
            }
            else if (string.IsNullOrWhiteSpace(e.NewTextValue))
            {
                SearchDeactivated?.Invoke();
            }
        }
        #endregion

        public void Dispose()
        {
            SearchActivated = null;
            SearchDeactivated = null;
        }
    }
}

