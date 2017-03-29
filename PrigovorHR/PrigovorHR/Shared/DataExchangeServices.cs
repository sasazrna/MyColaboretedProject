using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using Xamarin.Auth;
using Plugin.Messaging;
using Android.Content.PM;
using System.IO;
using Newtonsoft.Json.Linq;
using PrigovorHR.Shared.Controllers;

namespace PrigovorHR.Shared
{
    /// <summary>
    /// Root class for dataexchange functions, outside calls only can see this class
    /// </summary>
    public class DataExchangeServices
    {
        public static async Task<string> GetSearchResults(string searchfor)
        {
            return await new ServerCommuncationServices().GetData(ServerCommuncationServices.ServiceCommands.GetSearchResults, searchfor);
        }

        public static async Task<string> GetDirectTagResult(string searchfor)
        {
            return await new ServerCommuncationServices().GetData(ServerCommuncationServices.ServiceCommands.GetDirectTagResult, searchfor);
        }

        public static async Task<string> GetCompanyElementData(string ElementSlug)
        {
            return await new ServerCommuncationServices().GetData(ServerCommuncationServices.ServiceCommands.GetCompanyElementData, ElementSlug);
        }

        //public static async Task<string> GetGoogleUserInfo(string usertoken)
        //{
        //    return await new ServerCommuncationServices().GetData(ServerCommuncationServices.ServiceCommands.GetGoogleUserInfo, usertoken);
        //}

        //public static async Task<string> GetFacebookUserInfo(string usertoken)
        //{
        //    return await new ServerCommuncationServices().GetData(ServerCommuncationServices.ServiceCommands.GetFacebookUserInfo, usertoken);
        //}

        public static async Task<string> RegisterUser_EMail(string jsonvalue)
        {
            return await new ServerCommuncationServices().SendData(ServerCommuncationServices.ServiceCommands.RegisterUser, jsonvalue);
        }

        public static async Task<string> LoginUser_EMail(string jsonvalue)
        {
            return await new ServerCommuncationServices().SendData(ServerCommuncationServices.ServiceCommands.LoginUser, jsonvalue);
        }

        public static async Task<string> GetUserAvatar(string jsonvalue)
        {
            return await new ServerCommuncationServices().GetData(ServerCommuncationServices.ServiceCommands.GetUserAvatar, jsonvalue);
        }

        public static async Task<string> ChangeUserInfo(string jsonvalue)
        {
            return await new ServerCommuncationServices().SendData(ServerCommuncationServices.ServiceCommands.ChangeUserInfo, jsonvalue);
        }

        public static async Task<bool> ContactUs(string jsonvalue)
        {
            var result = await new ServerCommuncationServices().SendData(ServerCommuncationServices.ServiceCommands.ContactUs, jsonvalue);
            return !result.Contains("Error");
        }

        public static async Task<string> GetMyComplaints()
        {
            return await new ServerCommuncationServices().GetData(ServerCommuncationServices.ServiceCommands.GetMyComplaints, string.Empty);
        }

        public static async Task<string> ResetPassword(string jsonvalue)
        {
            return await new ServerCommuncationServices().SendData(ServerCommuncationServices.ServiceCommands.ResetPassword, jsonvalue);
        }
        public static async Task<string> ComplaintReaded(string jsonvalue)
        {
            return await new ServerCommuncationServices().SendData(ServerCommuncationServices.ServiceCommands.ComplaintReaded, jsonvalue);
        }

        public static async Task<int> SendComplaintAttachment(byte[] ByteData, string FileName)
        {
            try
            {
                var Result = await new ServerCommuncationServices().SendData(ServerCommuncationServices.ServiceCommands.SendComplaintAttachment, FileName, ByteData);
                JObject Jobj = JObject.Parse(Result);
                return (int)Jobj["attachments_id"][0];
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public static async Task<int> SendReplyAttachment(byte[] ByteData, string FileName)
        {
            try
            {
                var Result = await new ServerCommuncationServices().SendData(ServerCommuncationServices.ServiceCommands.SendReplyAttachment, FileName, ByteData);
                JObject Jobj = JObject.Parse(Result);
                return (int)Jobj["attachments_id"][0];
            }
            catch
            {
                return 0;
            }
        }

        public static async Task<string> SendComplaint(string jsonvalue)
        {
            return await new ServerCommuncationServices().SendData(ServerCommuncationServices.ServiceCommands.SendComplaint, jsonvalue);
        }

        public static async Task<string> SendReply(string jsonvalue)
        {
            return await new ServerCommuncationServices().SendData(ServerCommuncationServices.ServiceCommands.SendReply, jsonvalue);
        }

        public static async Task<string> GetLongLatFromAddress(string Address)
        {
            return await new ServerCommuncationServices().GetData(ServerCommuncationServices.ServiceCommands.GetLongLatFromAddress, Address);
        }

        public static async Task<string> GetComplaintAttachmentData(int Id, string FileName)
        {
            string JsonData = JsonConvert.SerializeObject(new { Id = Id, FileName = FileName });
            return await new ServerCommuncationServices().GetData(ServerCommuncationServices.ServiceCommands.GetComplaintAttachmentData, JsonData);
        }

        public static async Task<string> GetReplyAttachmentData(int Id, string FileName)
        {
            string JsonData = JsonConvert.SerializeObject(new { Id = Id, FileName = FileName });
            return await new ServerCommuncationServices().GetData(ServerCommuncationServices.ServiceCommands.GetReplyAttachmentData, JsonData);
        }

        public static async Task<bool> SendExceptionData(string jsonvalue)
        {
            var ResultData = await new ServerCommuncationServices().SendData(ServerCommuncationServices.ServiceCommands.SendExceptionData, jsonvalue);
            return !ResultData.Contains("Error:");
        }

        public static async Task<bool> CloseComplaint(string jsonvalue)
        {
            var ResultData = await new ServerCommuncationServices().SendData(ServerCommuncationServices.ServiceCommands.CloseComplaint, jsonvalue);
            return !ResultData.Contains("Error:");
        }

        public static async Task<string> GetCompanyLogo(string jsonvalue)
        {
            return await new ServerCommuncationServices().GetData(ServerCommuncationServices.ServiceCommands.GetCompanyLogo, jsonvalue);
        }

        public static async Task<string> CheckForNewReplys(string jsonvalue)
        {
           return await new ServerCommuncationServices().GetData(ServerCommuncationServices.ServiceCommands.CheckForNewReplys, jsonvalue);
        }

        /// <summary>
        /// Private class that handles all the communications and returns result to root dataexchangeservices class
        /// </summary>
        private class ServerCommuncationServices
        {

            #if DEBUG
            private const string ServiceAddress = "http://138.68.85.217/api/";
            #else
            private const string ServiceAddress = "https://prigovor.hr/api/";
           #endif

            public enum ServiceCommands : int
            {
                GetSearchResults,
                GetDirectTagResult,
                //GetGoogleUserInfo = 3,
                //GetFacebookUserInfo = 4,
                RegisterUser,
                LoginUser,
                GetUserAvatar,
                ChangeUserInfo,
                ContactUs,
                GetMyComplaints,
                ResetPassword,
                ComplaintReaded,
                SendComplaint,
                SendReply,
                GetLongLatFromAddress,
                SendComplaintAttachment,
                SendReplyAttachment,
                GetComplaintAttachmentData,
                GetReplyAttachmentData,
                GetCompanyElementData,
                SendExceptionData,
                CloseComplaint,
                GetCompanyLogo,
                CheckForNewReplys
            };


            private Dictionary<ServiceCommands, string> APIAdresses =
                new Dictionary<ServiceCommands, string> { { ServiceCommands.GetSearchResults, "pretraga/" },
                                                          { ServiceCommands.GetDirectTagResult, "qr/" },
                                                          { ServiceCommands.RegisterUser, "register" },
                                                          { ServiceCommands.LoginUser, "login" },
                                                          { ServiceCommands.GetUserAvatar, "avatar" } ,
                                                          { ServiceCommands.ChangeUserInfo, "user/" },
                                                          { ServiceCommands.ContactUs, "kontakt" },
                                                          { ServiceCommands.GetMyComplaints,"svi-moji-prigovori" },
                                                          { ServiceCommands.ResetPassword, "zahtjev-za-novu-lozinku" },
                                                          { ServiceCommands.ComplaintReaded, "prigovor-procitan" },
                                                          { ServiceCommands.SendComplaint, "predaj-prigovor" },
                                                          { ServiceCommands.SendReply, "prigovor/odgovor" },
                                                          { ServiceCommands.GetLongLatFromAddress, "https://maps.googleapis.com/maps/api/geocode/json?address=" },
                                                          { ServiceCommands.SendComplaintAttachment,"predaj-prigovor/single-upload" },
                                                          { ServiceCommands.SendReplyAttachment, "odgovori-na-prigovor/single-upload" },
                                                          { ServiceCommands.GetComplaintAttachmentData, "prilozi/" },
                                                          { ServiceCommands.GetReplyAttachmentData, "odgovor-prilozi/" },
                                                          { ServiceCommands.GetCompanyElementData, "prigovori/" },
                                                          { ServiceCommands.SendExceptionData, "xamarin-exceptions"} ,
                                                          { ServiceCommands.CloseComplaint, "prigovor/zatvori/" },
                                                          { ServiceCommands.GetCompanyLogo, "logo-tvrtke/" },
                                                          { ServiceCommands.CheckForNewReplys, "svi-moji-prigovori-nakon-datuma/" } };



            internal async Task<string> GetData(ServiceCommands ServiceCommand, string value = null, string serviceAddress = ServiceAddress)
            {
                var byteArray = Encoding.UTF8.GetBytes("forge:123123p");
                var header = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                //json = json.Trim('"');

                try
                {
                    using (var client = new HttpClient())
                    {
                        HttpContent content = new StringContent(value, Encoding.UTF8, "application/json");
                        var response = new HttpResponseMessage();
                        client.DefaultRequestHeaders.Authorization = header;
                        client.DefaultRequestHeaders.Add("Accept", "application/json");

                        switch (ServiceCommand)
                        {
                            case ServiceCommands.GetSearchResults:
                            case ServiceCommands.GetDirectTagResult:
                            case ServiceCommands.GetCompanyElementData:
                            case ServiceCommands.GetCompanyLogo:
                                response = await client.GetAsync(serviceAddress + APIAdresses[ServiceCommand] + value);
                                break;

                            case ServiceCommands.GetUserAvatar:
                            case ServiceCommands.GetMyComplaints:
                            case ServiceCommands.GetComplaintAttachmentData:
                            case ServiceCommands.GetReplyAttachmentData:
                            case ServiceCommands.CheckForNewReplys:
                                header = new AuthenticationHeaderValue("Bearer", Models.UserToken.token);
                                client.DefaultRequestHeaders.Authorization = header;
                                var fullAddress = serviceAddress + APIAdresses[ServiceCommand];

                                if (ServiceCommand == ServiceCommands.GetComplaintAttachmentData |
                                    ServiceCommand == ServiceCommands.GetReplyAttachmentData)
                                {
                                    JObject Jobj = JObject.Parse(value);
                                    fullAddress += Jobj["Id"] + "/" + Jobj["FileName"];
                                }
                                if (ServiceCommand != ServiceCommands.GetUserAvatar)
                                    fullAddress += value;
                                response = await client.GetAsync(fullAddress);
                                break;

                            //case ServiceCommands.GetGoogleUserInfo:
                            //    client.DefaultRequestHeaders.Add("Accept", "application/json");
                            //    response = await client.GetAsync("https://www.googleapis.com/plus/v1/people/me" + "?access_token=" + value);
                            //    break;

                            //case ServiceCommands.GetFacebookUserInfo:
                            //    client.DefaultRequestHeaders.Add("Accept", "application/json");
                            //    response = await client.GetAsync(new Uri("https://graph.facebook.com/me?fields=email,first_name,last_name,gender,picture") + "?access_token=" + value);
                            //    break;

                            case ServiceCommands.GetLongLatFromAddress:
                                client.DefaultRequestHeaders.Add("Accept", "application/json");
                                var bundle = Android.App.Application.Context.PackageManager.GetApplicationInfo("com.prigovorHR.android", PackageInfoFlags.MetaData).MetaData;

                                response = await client.GetAsync(string.Format(APIAdresses[ServiceCommand] + "{0}" + "&key={1}", value, bundle.Get("com.google.android.maps.v2.API_KEY")));
                                break;
                        }

                        if (response.IsSuccessStatusCode)
                        {
                            try { Models.UserToken.token = response.Headers.GetValues("Authorization").Last(); } catch { }

                            if (ServiceCommand == ServiceCommands.GetUserAvatar |
                                ServiceCommand == ServiceCommands.GetComplaintAttachmentData |
                                ServiceCommand == ServiceCommands.GetReplyAttachmentData | 
                                ServiceCommand == ServiceCommands.GetCompanyLogo)
                                return Convert.ToBase64String(await response.Content.ReadAsByteArrayAsync());
                            else return await response.Content.ReadAsStringAsync();
                        }
                        else
                        {
                            var ErrorMessage =  "Error:" + response.ReasonPhrase + await response.Content.ReadAsStringAsync();
                            ExceptionController.HandleException(new Exception(ErrorMessage), "Došlo je do greške na  internal async Task<string> SendData");
                            return ErrorMessage;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionController.HandleException(ex, "Došlo je do greške na  internal async Task<string> GetData(");
                    return "Error:";
                }
            }

            internal async Task<string> SendData(ServiceCommands ServiceCommand, string value, byte[] byteData = null, string serviceAddress = ServiceAddress)
            {
                try
                {
                    using (var client = new HttpClient())
                    {
                        HttpContent content = new StringContent(value, Encoding.UTF8, "application/json");
                        var response = new HttpResponseMessage();
                        string urlAddress = serviceAddress + APIAdresses[ServiceCommand];
                        var multipartData = new MultipartFormDataContent();

                        if (ServiceCommand != ServiceCommands.LoginUser & 
                            ServiceCommand != ServiceCommands.ResetPassword & 
                            ServiceCommand != ServiceCommands.SendExceptionData)
                        {
                            var header = new AuthenticationHeaderValue("Bearer", Models.UserToken.token);
                            client.DefaultRequestHeaders.Authorization = header;
                        }
                        client.DefaultRequestHeaders.Add("Accept", "application/json");

                        switch (ServiceCommand)
                        {
                            default:
                                response = await client.PostAsync(urlAddress, content);
                                break;

                            case ServiceCommands.SendComplaintAttachment:
                            case ServiceCommands.SendReplyAttachment:
                                multipartData.Add(new ByteArrayContent(byteData), "file", value);
                                response = await client.PostAsync(urlAddress, multipartData);
                                break;

                            case ServiceCommands.ChangeUserInfo:
                                JObject Jobj = JObject.Parse(value);

                                foreach (var prop in Jobj.Properties())
                                    if (!string.IsNullOrEmpty((string)Jobj[prop.Name]))
                                        multipartData.Add(new StringContent((string)Jobj[prop.Name]), prop.Name);

                                multipartData.Add(new ByteArrayContent(Pages.ProfilePage.ProfileImageByte), "profile_image_input", "avatar.jpg");
                                urlAddress += LoginRegisterController.LoggedUser.id.ToString();
                                response = await client.PostAsync(urlAddress, multipartData);
                                break;
                        }

                        if (response.IsSuccessStatusCode)
                        {
                            try { Models.UserToken.token = response.Headers.GetValues("Authorization").Last(); } catch { }
                            return await response.Content.ReadAsStringAsync();
                        }
                        else
                        {
                            var ErrorMessage = "Error:" + response.ReasonPhrase + await response.Content.ReadAsStringAsync();
                            ExceptionController.HandleException(new Exception(ErrorMessage), "Došlo je do greške na  internal async Task<string> SendData");
                            return ErrorMessage;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionController.HandleException(ex, "Došlo je do greške na  internal async Task<string> SendData");
                    return "Error:";
                }
            }
        }
    }
}
