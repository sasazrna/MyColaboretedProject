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

        public bool _isTyping = false;
        public bool _isSpecial = false;//označavati će dali se nešto specifično pretražuje, treba definirati
        public bool _stopTextChangedEvent = false;
        public bool _isQRTextActive = false;
        SearchBar _SearchBarField;
        Entry _EntryBarField;
    
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

            _WaitBeforeActivation = WaitBeforeActivation;

            _SearchBarField = SearchBarField;
            _EntryBarField = EntryBarField;
        }

        #region search function
        private int _NumberOfTextChangesBeforeSearchStart = 0;
        private int _NumberOfActivatedTimers = 0;
        private int _WaitBeforeActivation = 500;
        private void SearchBarField_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_stopTextChangedEvent | e.NewTextValue == "@" | e.NewTextValue == "#" | e.NewTextValue == "#@" | e.NewTextValue == "@#") return;

            if (e.NewTextValue != e.OldTextValue & !string.IsNullOrWhiteSpace(e.NewTextValue))
            {
                _isTyping = true;
                _NumberOfTextChangesBeforeSearchStart++;
                string typedtext = e.NewTextValue.Replace("#", "");
                typedtext = typedtext.Replace("@", "");

                Device.StartTimer(new TimeSpan(0, 0, 0, 0, _WaitBeforeActivation), () =>
                {
                    _NumberOfActivatedTimers++;
                    if (_NumberOfActivatedTimers == _NumberOfTextChangesBeforeSearchStart)
                    {
                        _NumberOfActivatedTimers = 0;
                        _NumberOfTextChangesBeforeSearchStart = 0;
                        _isTyping = false;
                        if (!_isQRTextActive || (_isQRTextActive & e.NewTextValue != string.Empty))
                        {
                            SearchActivated?.Invoke(typedtext, _isQRTextActive);
                            if (_isQRTextActive)
                            {
                                if (_SearchBarField != null)
                                    _SearchBarField.Text = string.Empty;
                                else _EntryBarField.Text = string.Empty;
                            }
                        }
                        else if (_isQRTextActive & e.NewTextValue == string.Empty)
                            _isQRTextActive = false;
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

