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
            //try
            //{
            //    throw new Exception("bla");
            //}
            //catch(Exception ex)
            //{
            //    // Acr.UserDialogs.UserDialogs.Instance.Alert(ex.StackTrace);
            //  // new Controllers.ExceptionController(ex, "", "");
            //}
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
                //new Controllers.ExceptionController(ex, "Došlo je do greške prilikom slanja vašeg privitka!" + Environment.NewLine + "Detalji greške: " + ex.Message,
                //    "public static async Task<int> SendComplaintAttachment(byte[] ByteData, string FileName)" + Environment.NewLine + ex.ToString());
                return -1;
            }
        }

        public static async Task<string> SendComplaint(string jsonvalue)
        {
            return await new ServerCommuncationServices().SendData(ServerCommuncationServices.ServiceCommands.SendComplaint, jsonvalue);
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

        /// <summary>
        /// Private class that handles all the communications and returns result to root dataexchangeservices class
        /// </summary>
        private class ServerCommuncationServices
        {
            private const string ServiceAddress = "http://138.68.85.217/api/"; /* //"https://prigovor.hr/api/";*/
            public enum ServiceCommands : int
            {
                GetSearchResults,
                GetDirectTagResult ,
                //GetGoogleUserInfo = 3,
                //GetFacebookUserInfo = 4,
                RegisterUser ,
                LoginUser  ,
                GetUserAvatar ,
                ChangeUserInfo ,
                ContactUs,
                GetMyComplaints ,
                ResetPassword,
                ComplaintReaded ,
                SendComplaint ,
                GetLongLatFromAddress ,
                SendComplaintAttachment ,
                SendReplyAttachment,
                GetComplaintAttachmentData,
                GetReplyAttachmentData,
                GetCompanyElementData,
            };


            private Dictionary<ServiceCommands, string> APIAdresses =
                new Dictionary<ServiceCommands, string> { { ServiceCommands.GetSearchResults, "pretraga/" },
                                                          { ServiceCommands.GetDirectTagResult, "direktni-pristup/" },
                                                          { ServiceCommands.RegisterUser, "register" },
                                                          { ServiceCommands.LoginUser, "login" },
                                                          { ServiceCommands.GetUserAvatar, "avatar" } ,
                                                          { ServiceCommands.ChangeUserInfo, "user/" },
                                                          { ServiceCommands.ContactUs, "kontakt" },
                                                          { ServiceCommands.GetMyComplaints,"svi-moji-prigovori" },
                                                          { ServiceCommands.ResetPassword, "zahtjev-za-novu-lozinku" },
                                                          { ServiceCommands.ComplaintReaded, "prigovor-procitan" },
                                                          { ServiceCommands.SendComplaint, "predaj-prigovor" },
                                                          { ServiceCommands.GetLongLatFromAddress, "https://maps.googleapis.com/maps/api/geocode/json?address=" },
                                                          { ServiceCommands.SendComplaintAttachment,"predaj-prigovor/single-upload" },
                                                          {ServiceCommands.SendReplyAttachment, "odgovori-na-prigovor/single-upload" },
                                                          { ServiceCommands.GetComplaintAttachmentData, "prilozi/" },
                                                          {ServiceCommands.GetReplyAttachmentData, "odgovor-prilozi/" },
                                                          { ServiceCommands.GetCompanyElementData, "prigovori/" } };




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
                                response = await client.GetAsync(serviceAddress + APIAdresses[ServiceCommand] + value);
                                break;

                            case ServiceCommands.GetUserAvatar:
                            case ServiceCommands.GetMyComplaints:
                            case ServiceCommands.GetComplaintAttachmentData:
                            case ServiceCommands.GetReplyAttachmentData:
                                header = new AuthenticationHeaderValue("Bearer", Models.UserToken.token);
                                client.DefaultRequestHeaders.Authorization = header;
                                var fullAddress = serviceAddress + APIAdresses[ServiceCommand];

                                if (ServiceCommand == ServiceCommands.GetComplaintAttachmentData | 
                                    ServiceCommand == ServiceCommands.GetReplyAttachmentData)
                                {
                                    JObject Jobj = JObject.Parse(value);
                                    fullAddress += Jobj["Id"] + "/" + Jobj["FileName"];
                                }
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
                                var bundle = Android.App.Application.Context.PackageManager.GetApplicationInfo("com.prigovorhr.android", PackageInfoFlags.MetaData).MetaData;

                                response = await client.GetAsync(string.Format(APIAdresses[ServiceCommand] + "{0}" + "&key={1}", value, bundle.Get("com.google.android.maps.v2.API_KEY")));
                                break;
                        }

                        if (response.IsSuccessStatusCode)
                        {
                            try
                            {
                                Models.UserToken.token = response.Headers.GetValues("Authorization").Last();
                            }
                            catch { }

                            if (ServiceCommand == ServiceCommands.GetUserAvatar | 
                                ServiceCommand == ServiceCommands.GetComplaintAttachmentData |
                                ServiceCommand == ServiceCommands.GetReplyAttachmentData)
                                return Convert.ToBase64String(await response.Content.ReadAsByteArrayAsync());
                            else return await response.Content.ReadAsStringAsync();
                        }
                        else
                        {

                            //write this to filelog and send it after next success login
                            //   return response.ToString();
                            return "Error:" + response.ReasonPhrase +  await response.Content.ReadAsStringAsync();
                        }
                    }
                }
                catch (Exception err)
                {
                    return "Error:" + err.ToString();
                }
            }

            internal async Task<string> SendData(ServiceCommands ServiceCommand, string value, byte[] byteData = null, string serviceAddress = ServiceAddress)
            {
                //var byteArray = Encoding.UTF8.GetBytes("forge:123123p");
                //var header = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                try
                {
                    using (var client = new HttpClient())
                    {
                        HttpContent content = new StringContent(value, Encoding.UTF8, "application/json");
                        var response = new HttpResponseMessage();
                        string urlAddress = serviceAddress + APIAdresses[ServiceCommand];
                        var multipartData = new MultipartFormDataContent();

                        if (ServiceCommand != ServiceCommands.LoginUser)
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
                                multipartData.Add(new ByteArrayContent(byteData),"file", value);
                                response = await client.PostAsync(urlAddress, multipartData);
                                break;

                            case ServiceCommands.ChangeUserInfo:
                                JObject Jobj = JObject.Parse(value);

                                foreach (var prop in Jobj.Properties())
                                    if (!string.IsNullOrEmpty((string)Jobj[prop.Name]))
                                        multipartData.Add(new StringContent((string)Jobj[prop.Name]), prop.Name);

                                multipartData.Add(new ByteArrayContent(Views.ProfileView.ProfileImageByte), "profile_image_input", "avatar.jpg");
                                urlAddress += Controllers.LoginRegisterController._LoggedUser.id.ToString();
                                response = await client.PostAsync(urlAddress, multipartData);
                                break;
                        }

                        if (response.IsSuccessStatusCode)
                        {
                            try { Models.UserToken.token = response.Headers.GetValues("Authorization").Last(); } catch { }
                            return await response.Content.ReadAsStringAsync();
                        }
                        else return await response.Content.ReadAsStringAsync(); //"Error: " + response.StatusCode.ToString();
                    }
                }
                catch (Exception err) { return "Error:" + err.ToString(); }
            }
        }
    }
}
