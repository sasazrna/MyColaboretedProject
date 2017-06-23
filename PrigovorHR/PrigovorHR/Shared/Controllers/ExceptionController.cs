using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Complio.Shared.Controllers
{
    public class ExceptionController
    {
        public static async void HandleException(Exception ex, string developermessage)
        {
            var ExceptionData = new Models.ExceptionInfoModel.ExceptionInfo()
            {
                exception = ex.ToString(),
                developer_message = developermessage
            };

            if (ExceptionData.is_network_available)
            {
                if (!await DataExchangeServices.SendExceptionData(JsonConvert.SerializeObject(ExceptionData)))
                {
                    Application.Current.Properties.Add("Exception_" + GetLastExceptionId(), JsonConvert.SerializeObject(ExceptionData));
                    await Application.Current.SavePropertiesAsync();
                }
                else RemoveExceptionDataFiles();
            }
            else
            {
                Application.Current.Properties.Add("Exception_" + GetLastExceptionId(), JsonConvert.SerializeObject(ExceptionData));
                await Application.Current.SavePropertiesAsync();
            }
        }

        public static async void CheckAndSendUnsentExceptions()
        {
            object obj;
            bool SendSuccessfull = true;

            for (int i = 0; i < int.MaxValue; i++)
                if (App.Current.Properties.TryGetValue("Exception_" + i.ToString(), out obj))
                {
                    if (!await DataExchangeServices.SendExceptionData((string)obj))
                    {
                        SendSuccessfull = false;
                        break;
                    }
                }
                else break;

            if (SendSuccessfull)
                RemoveExceptionDataFiles();
        }

        private static string GetLastExceptionId()
        {
            object obj;
            var NumOfUnSentExceptions = -1;
            for (int i = 0; i < int.MaxValue; i++)
                if (Application.Current.Properties.TryGetValue("Exception_" + i.ToString(), out obj))
                    NumOfUnSentExceptions = i;
                else
                {
                    NumOfUnSentExceptions = i;
                    break;
                }

            return NumOfUnSentExceptions.ToString();
        }

        private static void RemoveExceptionDataFiles()
        {
            object obj;
            for (int i = 0; i < int.MaxValue; i++)
                if (App.Current.Properties.TryGetValue("Exception_" + i.ToString(), out obj))
                {
                    Application.Current.Properties.Remove("Exception_" + i.ToString());
                    Application.Current.SavePropertiesAsync();
                }
                else
                    break;
        }
    }
}
