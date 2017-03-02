using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrigovorHR.Shared.Controllers
{
    public class ExceptionController 
    {
        //Zadatak exception controlera je podignutu iznimku hendlati.
        //Prvo prikazuje customermessage korisniku, developermessage sastavi za programere te iz ex-a izvuče podatke o grešci za developera
        //Sve to pošalje bruni na API server a bruno za mene na moj email svako 12 sati šalje izvještaj o iznimkama.
        //Uz to šaljem datum događaja, ekran na kojem se desio?? (kako??) , i informacije o deviceu i dostupnosti interneta.
        public static void HandleException(Exception ex, string customermessage, string developermessage)
        {
           var Base = ex.GetBaseException()?.ToString();
           var Inner = ex.InnerException?.ToString(); 
          //  Acr.UserDialogs.UserDialogs.Instance.Alert(ex.ToString());
        }
    }
}
