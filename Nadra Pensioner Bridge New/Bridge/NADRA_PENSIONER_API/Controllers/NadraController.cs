using NADRA_PENSIONER_API.Model.Nadra;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NADRA_PENSIONER_API.Model;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using System.Xml.Serialization;
using System.IO;
using System.Drawing;

namespace NADRA_PENSIONER_API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class NadraController : Controller
    {

        private readonly BusinessLogic _businessLogic;
        private readonly AppSettings _appSettings;
        private readonly URLConfiguration _urlconfig;
        private readonly IHttpClientFactory _clientFactory;
        private string _username;
        private string _password;
        private string _ipaddress;
        private string _correspondentid;
        private static string _franchiseID;
        string branch = "1505";
        string transaction_id = "";
        public NadraController(IOptions<AppSettings> appSettings, BusinessLogic businessLogic, IOptions<URLConfiguration> urlConfiguration, IHttpClientFactory clientFactory)
        {
            _businessLogic = businessLogic;
            _appSettings = appSettings.Value;
            _urlconfig = urlConfiguration.Value;
            _username = _appSettings.username;
            _password = _appSettings.password;
            _franchiseID = _appSettings.franchiseID;
            _clientFactory = clientFactory;
        }

        
        [HttpPost]
        [Authorize]
        public async Task<JsonResult> GetLastVerificationResults([FromBody] GetLastVerifResultsRequest VerifRequest)
        {
            GetLastVerifResultsResponse responsebody = new GetLastVerifResultsResponse();
            transaction_id = _businessLogic.GenerateTransactionID();

            try
            {
                if (!String.IsNullOrEmpty(VerifRequest.CNIC) && !String.IsNullOrEmpty(VerifRequest.transactionId))
                {
                    using (var httpClient = new HttpClient())
                    {
                        using (var request = new HttpRequestMessage(HttpMethod.Post, _urlconfig.URLforVerification))
                        {

                            //Sample for Citizen Verification


                            string requestBody = @"<?xml version=""1.0"" encoding=""utf-8""?>
                                          <soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
                                                   <soap:Body>  
                                                      <GetLastVerification xmlns=""http://tempuri.org/"">
                                                         <CITIZEN_NUMBER>" + VerifRequest.CNIC + @"</CITIZEN_NUMBER>
                                                         <TRANSACTION_ID>" + VerifRequest.transactionId + @"</TRANSACTION_ID>
                                                       </GetLastVerification>
                                                     </soap:Body>
                                                    </soap:Envelope>";

                            //request.Content =
                            //    new StringContent(requestBody, Encoding.UTF8, "text/xml");
                            //HttpResponseMessage responseMessage = await httpClient.SendAsync(request);
                            //var json_string = await responseMessage.Content.ReadAsStringAsync();
                            //responsebody.status_code = _businessLogic.getBetweenString(json_string, "&lt;CODE&gt;", "&lt;/CODE&gt;");


                            ////for CHECKING - Failure Body
                            //var json_string = @"<?xml version=""1.0"" encoding=""utf-8""?>
                            //<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
                            //           <soap:Body>
                            //                <GetLastVerificationResponse xmlns=""http://tempuri.org/"">         
                            //                     <GetLastVerificationResult>&lt;?xml version=""1.0"" encoding=""utf-16""?&gt;
                            //                      &lt;VERIFICATION xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""&gt;
                            //                      &lt;RESPONSE_DATA&gt;
                            //                      &lt;RESPONSE_STATUS&gt;
                            //                      &lt;CODE&gt;151&lt;/CODE&gt;
                            //                      &lt;MESSAGE&gt;No request found against citizen number/ transaction id&lt;/MESSAGE&gt;
                            //                      &lt;/RESPONSE_STATUS&gt;
                            //                      &lt;CITIZEN_NUMBER&gt;4210114220683&lt;/CITIZEN_NUMBER&gt;
                            //                      &lt;/RESPONSE_DATA&gt;
                            //                      &lt;/VERIFICATION&gt;</GetLastVerificationResult>                          
                            //                                 </GetLastVerificationResponse>
                            //                             </soap:Body>
                            //                          </soap:Envelope>";
                            //responsebody.status_code = "151";


                            //      //for CHECKING - Failure Body
                            var json_string = @"<?xml version=""1.0"" encoding=""utf-8""?>
                            <soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd =""http://www.w3.org/2001/XMLSchema"">
                                       <soap:Body>         
                                            <GetLastVerificationResponse xmlns=""http://tempuri.org/"">
                                                 <GetLastVerificationResult>&lt;?xml version=""1.0"" encoding=""utf-16""?&gt;
                                                  &lt;VERIFICATION xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""&gt;
                                                  &lt;RESPONSE_DATA&gt;
                                                  &lt;RESPONSE_STATUS&gt;
                                                  &lt;CODE&gt;100&lt;/CODE&gt;
                                                  &lt;MESSAGE&gt;successful&lt;/MESSAGE&gt;
                                                  &lt;/RESPONSE_STATUS&gt;
                                                  &lt;SESSION_ID&gt;" + VerifRequest.transactionId + @"&lt;/SESSION_ID&gt;
                                                  &lt;CITIZEN_NUMBER&gt;" + VerifRequest.CNIC + @"&lt;/CITIZEN_NUMBER&gt;
                                                  &lt;/RESPONSE_DATA&gt;
                                                  &lt;/VERIFICATION&gt;</GetLastVerificationResult>
                                                             </GetLastVerificationResponse>
                                                     </soap:Body>
                                                  </soap:Envelope>";
                              responsebody.status_code = "100";

                            if (responsebody.status_code == "100")
                            {
                                //For Returning Purpose
                                responsebody.status_code = _businessLogic.getBetweenString(json_string, "&lt;CODE&gt;", "&lt;/CODE&gt;");
                                responsebody.status_message = _businessLogic.getBetweenString(json_string, "&lt;MESSAGE&gt;", "&lt;/MESSAGE&gt;");
                                responsebody.session_id = _businessLogic.getBetweenString(json_string, "&lt;SESSION_ID&gt;", "&lt;/SESSION_ID&gt;");
                                responsebody.citizen_number = VerifRequest.CNIC;// _businessLogic.getBetweenString(json_string, "&lt;CITIZEN_NUMBER&gt;", "&lt;/CITIZEN_NUMBER&gt;");

                            }
                            else
                            {
                                //For Returning Purpose
                                responsebody.citizen_number = VerifRequest.CNIC; //_businessLogic.getBetweenString(json_string, "&lt;CITIZEN_NUMBER&gt;", "&lt;/CITIZEN_NUMBER&gt;");
                                responsebody.status_code = _businessLogic.getBetweenString(json_string, "&lt;CODE&gt;", "&lt;/CODE&gt;");
                                responsebody.status_message = _businessLogic.getBetweenString(json_string, "&lt;MESSAGE&gt;", "&lt;/MESSAGE&gt;");
                                responsebody.session_id = "";


                            }

                        }

                    }
                }
                else
                {
                    //For Returning Purpose
                    responsebody.status_code = "001";
                    responsebody.status_message = "Fill all mandatory fields";
                    responsebody.session_id = "";
                    responsebody.citizen_number = VerifRequest.CNIC;
                    
                }


                return new JsonResult(responsebody);
            }
            catch (WebException ex)
            {
                responsebody.status_code = "500";
                responsebody.status_message = ex.Message;
                return new JsonResult(responsebody);
            }
            finally
            {
                //Get UserName
                string Username = System.Security.Principal.WindowsIdentity.GetCurrent().Name.Substring(7);
                ////Get Host IP
                string hostName = Dns.GetHostName();
                string IpAddress = Dns.GetHostByName(hostName).AddressList[1].ToString();

                ////Serialized Response
                string response = JsonConvert.SerializeObject(responsebody);
                //string responseLog = JsonConvert.SerializeObject(responsebodyLog);
                //string TRANSACTION_ID = _businessLogic.GenerateTransactionID();

                //Generate ID
                //DataTable dt = new DataTable();
                //dt = _businessLogic.getID();
                int ID = 1;

                string purpose = "GetLastVerif";
                string APID = "1";


                string ins = _businessLogic.InsertMobileNADRAVerification(ID, VerifRequest.CNIC, IpAddress, Username, transaction_id, branch, purpose, APID, responsebody.session_id);
                ////Update for the Image
                string ret = _businessLogic.ParseForMobile(response, VerifRequest.CNIC, IpAddress, Username, transaction_id, response, APID);
            }

        }




        /// <summary>
        /// //////////////////
        /// 
        /// 
        /// 
        /// </summary>


        [HttpPost]
        [Authorize]
        public async Task<JsonResult> verifyPensiorFingerPrint([FromBody]FingerprintVerifyRequest FPRequest)
        {
            FingerprintVerifyResponse responsebody = new FingerprintVerifyResponse();

          
            transaction_id = _businessLogic.GenerateTransactionID();
            try
            {
                //if (!String.IsNullOrEmpty(FPRequest.CNIC) && !String.IsNullOrEmpty(FPRequest.ContactNumber) && !String.IsNullOrEmpty(FPRequest.Area) && !String.IsNullOrEmpty(FPRequest.FPTemplate))
                //{
                using (var httpClient = new HttpClient())
                {
                    using (var request = new HttpRequestMessage(HttpMethod.Post, _urlconfig.URLforVerification))
                    {

                        ////Sample for FP Verification
                        //string requestBody = @"<?xml version=""1.0"" encoding=""utf-8""?>
                        //          <soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
                        //                     <soap:Body>
                        //                          <FingerprintVerificationForAccountOpening xmlns=""http://tempuri.org/"">
                        //                               <CNIC>" + FPRequest.CNIC + @"</CNIC>
                        //                               <ContactNumber>" + FPRequest.ContactNumber + @"</ContactNumber>
                        //                               <FingerprintIndex>" + FPRequest.FingerprintIndex + @"</FingerprintIndex>
                        //                               <FPTemplate>" + FPRequest.FPTemplate + @"</FPTemplate>
                        //                               <Area>" + FPRequest.Area + @"</Area>
                        //                               <TRANSACTION_ID>" + transaction_id + @"</TRANSACTION_ID>
                        //                           </FingerprintVerificationForAccountOpening>
                        //                       </soap:Body>
                        //                    </soap:Envelope>";


                        string requestBody = @"<?xml version=""1.0"" encoding=""utf-8""?>
                                                <soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
                                                  <soap:Body>
                                                    <dibVerifyFingerPrints xmlns=""http://tempuri.org/"">
                                                      <SESSION_ID> "+ FPRequest.SESSION_ID+ @" </SESSION_ID>
                                                      <CITIZEN_NUMBER>" + FPRequest.CITIZEN_NUMBER + @"</CITIZEN_NUMBER>
                                                      <CONTACT_NUMBER>" + FPRequest.CONTACT_NUMBER + @"</CONTACT_NUMBER>
                                                      <FINGER_INDEX>" + FPRequest.FINGER_INDEX + @"</FINGER_INDEX>
                                                      <FINGER_TEMPLATE>" + FPRequest.FINGER_TEMPLATE + @"</FINGER_TEMPLATE>
                                                      <AREA_NAME>" + FPRequest.AREA_NAME + @"</AREA_NAME>
                                                    </dibVerifyFingerPrints>
                                                  </soap:Body>
                                                </soap:Envelope>";



                        //request.Content =
                        //    new StringContent(requestBody, Encoding.UTF8, "text/xml");
                        //HttpResponseMessage responseMessage = await httpClient.SendAsync(request);
                        //var json_string = await responseMessage.Content.ReadAsStringAsync();


                        var json_string = @"<?xml version=""1.0"" encoding=""utf-8""?>
                                                                        <soap:Envelope
                                                                                xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/""
                                                                                xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
                                                                                xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
                                                                                <soap:Body>
                                                                                                <MobileNADRAVerificationResponse
                                                                                                                xmlns=""http://tempuri.org/"">
                                                                                                                <MobileNADRAVerificationResult>&lt;BIOMETRIC_VERIFICATION
                                                                                                                                xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
                                                                                                                                xmlns:xsd=""http://www.w3.org/2001/XMLSchema""&gt;
                                                                        &lt;RESPONSE_DATA&gt;
                                                                        &lt;RESPONSE_STATUS&gt;
                                                                        &lt;CODE&gt;100&lt;/CODE&gt;
                                                                        &lt;MESSAGE&gt;successful&lt;/MESSAGE&gt;
                                                                        &lt;/RESPONSE_STATUS&gt;
                                                                        &lt;SESSION_ID&gt;/SESSION_ID&gt;
                                                                        &lt;CITIZEN_NUMBER&gt;/CITIZEN_NUMBER&gt;
                                                                        &lt;PERSON_DATA&gt;
                                                                        &lt;NAME&gt;&lt;/NAME&gt;
                                                                        &lt;FATHER_HUSBAND_NAME&gt;&lt;/FATHER_HUSBAND_NAME&gt;
                                                                        &lt;PRESENT_ADDRESS&gt;&lt;/PRESENT_ADDRESS&gt;
                                                                        &lt;PERMANANT_ADDRESS&gt;&lt;/PERMANANT_ADDRESS&gt;
                                                                        &lt;DATE_OF_BIRTH&gt;&lt;/DATE_OF_BIRTH&gt;
                                                                        &lt;BIRTH_PLACE&gt;&lt;/BIRTH_PLACE&gt;
                                                                        &lt;EXPIRY_DATE&gt;&lt;/EXPIRY_DATE&gt;
                                                                        &lt;PHOTOGRAPH&gt;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&lt;/PHOTOGRAPH&gt;
                                                                        &lt;CARD_TYPE&gt;&lt;/CARD_TYPE&gt;
                                                                        &lt;/PERSON_DATA&gt;
                                                                        &lt;/RESPONSE_DATA&gt;&lt;/BIOMETRIC_VERIFICATION&gt;
                                                                                                                </MobileNADRAVerificationResult>
                                                                                                </MobileNADRAVerificationResponse>
                                                                                </soap:Body>
                                                                        </soap:Envelope>";

                        //code here for update log in database
                        string[] results = json_string.Split('|');
                        string ResultText = "";


                        for (int i = 0; i < results.Length; i++)
                        {
                            string[] feilddata = results[i].Split(':');
                            if (feilddata.Length > 1)
                            {
                                if (feilddata[0] != "PHOTOGRAPH")
                                {
                                    ResultText += feilddata[0] + " : " + feilddata[1] + ";" + "\r\n";
                                    if (feilddata[0] == "SESSION_ID")
                                    {
                                        sessionid = feilddata[1];
                                    }
                                }
                            }

                        }

                        Image img1 = DrawText(ResultText, 20);

                        byte[] ResultByteArray;
                        //byte[] ResultBytePhoto;
                        using (MemoryStream ms = new MemoryStream())
                        {
                            img1.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                            ResultByteArray = ms.ToArray();
                        }


                        string response = await _businessLogic.updateResponseImmediate(FPRequest.CITIZEN_NUMBER, ResultText, ResultByteArray);
                        //return str;


                        responsebody.status_code = "100";
                        responsebody.status_message = "successful";
                        responsebody.session_id = "1505100000000162792";
                        responsebody.citizen_number = FPRequest.CITIZEN_NUMBER;
                        responsebody.finger_index= FPRequest.FINGER_INDEX;


                        //var json_string = "";
                        ////For checking************
                        //responsebody.status_code = "000";
                        //if (FPRequest.CNIC.Equals("4220107210781"))
                        //{
                        //    json_string = @"<?xml version=""1.0"" encoding=""utf-8""?>
                        //<soap:Envelope xmlns:soap =""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
                        //           <soap:Body>
                        //                <FingerprintVerificationForAccountOpeningResponse xmlns=""http://tempuri.org/"">
                        //                     <FingerprintVerificationForAccountOpeningResult>&lt;?xml version=""1.0"" encoding=""utf-16""?&gt;
                        //                      &lt;VERIFICATION xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""&gt;
                        //                      &lt;RESPONSE_DATA&gt;
                        //                      &lt;RESPONSE_STATUS&gt;
                        //                      &lt;CODE&gt;100&lt;/CODE&gt;
                        //                      &lt;MESSAGE&gt;successful&lt;/MESSAGE&gt;
                        //                      &lt;/RESPONSE_STATUS&gt;
                        //                      &lt;SESSION_ID&gt;1505100000000165137&lt;/SESSION_ID&gt;
                        //                      &lt;CITIZEN_NUMBER&gt;4220107210781&lt;/CITIZEN_NUMBER&gt;
                        //                      &lt;/RESPONSE_DATA&gt;
                        //                      &lt;/VERIFICATION&gt;</FingerprintVerificationForAccountOpeningResult>
                        //                     </FingerprintVerificationForAccountOpeningResponse>
                        //             </soap:Body>
                        // </soap:Envelope>";
                        //}
                        //else if (FPRequest.CNIC.Equals("4220120581768"))
                        //{
                        //    json_string = @"<?xml version=""1.0"" encoding=""utf-8""?>
                        //<soap:Envelope xmlns:soap =""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
                        //           <soap:Body>
                        //                <FingerprintVerificationForAccountOpeningResponse xmlns=""http://tempuri.org/"">
                        //                     <FingerprintVerificationForAccountOpeningResult>&lt;?xml version=""1.0"" encoding=""utf-16""?&gt;
                        //                      &lt;VERIFICATION xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""&gt;
                        //                      &lt;RESPONSE_DATA&gt;
                        //                      &lt;RESPONSE_STATUS&gt;
                        //                      &lt;CODE&gt;100&lt;/CODE&gt;
                        //                      &lt;MESSAGE&gt;successful&lt;/MESSAGE&gt;
                        //                      &lt;/RESPONSE_STATUS&gt;
                        //                      &lt;SESSION_ID&gt;1505100000000165137&lt;/SESSION_ID&gt;
                        //                      &lt;CITIZEN_NUMBER&gt;4220120581768&lt;/CITIZEN_NUMBER&gt;
                        //                      &lt;/RESPONSE_DATA&gt;
                        //                      &lt;/VERIFICATION&gt;</FingerprintVerificationForAccountOpeningResult>
                        //                     </FingerprintVerificationForAccountOpeningResponse>
                        //             </soap:Body>
                        // </soap:Envelope>";
                        //}
                        //else
                        //{
                        //    json_string = @"<?xml version=""1.0"" encoding=""utf-8""?>
                        //<soap:Envelope xmlns:soap =""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
                        //           <soap:Body>
                        //                <FingerprintVerificationForAccountOpeningResponse xmlns=""http://tempuri.org/"">
                        //                     <FingerprintVerificationForAccountOpeningResult>&lt;?xml version=""1.0"" encoding=""utf-16""?&gt;
                        //                      &lt;VERIFICATION xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""&gt;
                        //                      &lt;RESPONSE_DATA&gt;
                        //                      &lt;RESPONSE_STATUS&gt;
                        //                      &lt;CODE&gt;100&lt;/CODE&gt;
                        //                      &lt;MESSAGE&gt;successful&lt;/MESSAGE&gt;
                        //                      &lt;/RESPONSE_STATUS&gt;
                        //                      &lt;SESSION_ID&gt;1505100000000165137&lt;/SESSION_ID&gt;
                        //                      &lt;CITIZEN_NUMBER&gt;4220153579503&lt;/CITIZEN_NUMBER&gt;
                        //                      &lt;/RESPONSE_DATA&gt;
                        //                      &lt;/VERIFICATION&gt;</FingerprintVerificationForAccountOpeningResult>
                        //                     </FingerprintVerificationForAccountOpeningResponse>
                        //             </soap:Body>
                        // </soap:Envelope>";
                        //}




                        //responsebody.status_code = _businessLogic.getBetweenString(json_string, "&lt;CODE&gt;", "&lt;/CODE&gt;");
                        //if (responsebody.status_code == "100")
                        //{
                        //    //For Returning Purpose
                        //    responsebody.status_code = _businessLogic.getBetweenString(json_string, "&lt;CODE&gt;", "&lt;/CODE&gt;");
                        //    responsebody.status_message = _businessLogic.getBetweenString(json_string, "&lt;MESSAGE&gt;", "&lt;/MESSAGE&gt;");
                        //    responsebody.session_id = _businessLogic.getBetweenString(json_string, "&lt;SESSION_ID&gt;", "&lt;/SESSION_ID&gt;");
                        //    responsebody.citizen_number = _businessLogic.getBetweenString(json_string, "&lt;CITIZEN_NUMBER&gt;", "&lt;/CITIZEN_NUMBER&gt;");
                        //    responsebody.finger_index = null;


                        //}
                        //else if (responsebody.status_code == "122")
                        //{
                        //    responsebody.status_code = _businessLogic.getBetweenString(json_string, "&lt;CODE&gt;", "&lt;/CODE&gt;");
                        //    responsebody.status_message = _businessLogic.getBetweenString(json_string, "&lt;MESSAGE&gt;", "&lt;/MESSAGE&gt;");
                        //    responsebody.session_id = _businessLogic.getBetweenString(json_string, "&lt;SESSION_ID&gt;", "&lt;/SESSION_ID&gt;");
                        //    responsebody.citizen_number = _businessLogic.getBetweenString(json_string, "&lt;CITIZEN_NUMBER&gt;", "&lt;/CITIZEN_NUMBER&gt;");
                        //    responsebody.finger_index = _businessLogic.getBetweenStringfingerindex(json_string, "&lt;FINGER&gt;", "&lt;/FINGER&gt;");


                        //}
                        //else
                        //{
                        //    //For Returning Purpose
                        //    responsebody.citizen_number = _businessLogic.getBetweenString(json_string, "&lt;CITIZEN_NUMBER&gt;", "&lt;/CITIZEN_NUMBER&gt;");
                        //    responsebody.status_code = _businessLogic.getBetweenString(json_string, "&lt;CODE&gt;", "&lt;/CODE&gt;");
                        //    responsebody.status_message = _businessLogic.getBetweenString(json_string, "&lt;MESSAGE&gt;", "&lt;/MESSAGE&gt;");
                        //    responsebody.session_id = "";
                        //    responsebody.finger_index = null;

                        //}
                    }

                }

                
            }
            catch (WebException ex)
            {
                responsebody.status_code = "500";
                responsebody.status_message = ex.Message;
                responsebody.session_id = "";
                responsebody.citizen_number = "";
                responsebody.finger_index = FPRequest.FINGER_INDEX;
                return new JsonResult(responsebody);
            }
            finally
            {
                //Get UserName
                string Username = System.Security.Principal.WindowsIdentity.GetCurrent().Name.Substring(7);
                ////Get Host IP
                string hostName = Dns.GetHostName();
                string IpAddress = Dns.GetHostByName(hostName).AddressList[1].ToString();

                ////Serialized Response
                string response = JsonConvert.SerializeObject(responsebody);
                //string responseLog = JsonConvert.SerializeObject(responsebodyLog);
                //string TRANSACTION_ID = _businessLogic.GenerateTransactionID();

                int ID = 1;

                string purpose = "FPVerif";
                string APID = "1";
                string ins = _businessLogic.InsertMobileNADRAVerification(ID, FPRequest.CITIZEN_NUMBER, IpAddress, Username, transaction_id, branch, purpose, APID, responsebody.session_id);
                ////Update updating the log
                string ret = _businessLogic.InsertFPData(response, FPRequest.CITIZEN_NUMBER, IpAddress, Username, transaction_id, response, APID);

               

            }

            return new JsonResult(responsebody);
        }




        //CallNadraPensiorDMZ.CallNadraPensior obj = new CallNadraPensiorDMZ.CallNadraPensior();
     
        //string APID = "1";
        //string branch = "3351";
        string sessionid = string.Empty;

        //database Logging
        #region


        [HttpGet]
        [Authorize]
        public async Task<ApiResponse> VerifyUser(string Username)
        {
            try
            {
                var response = new ApiResponse()
                {
                    message = "Success",
                    status = "200",
                    result = await _businessLogic.VerifyUserDB(Username)
                };

                return response;
            }
            catch (Exception ex)
            {
                var response = new ApiResponse()
                {
                    message = "failed",
                    status = "500",
                    result = "Expection " + ex.Message
                };

                return response;
            }
           
        }


        [HttpGet]
        [Authorize]
        public async Task<ApiResponse> GetCNIC(string IPAddress)
        {
            var response = new ApiResponse()
            {
                message = "Success",
                status = "200",
                result = await _businessLogic.GetCNICdb(IPAddress)
            };

            //string requesttype = "PensionerVerif";
            //string requester = System.Security.Principal.WindowsIdentity.GetCurrent().Name.Substring(7); 
            return response;

        }

        [HttpGet]
        [Authorize]
        public async Task<ApiResponse> GetVerificationResult(string IPAddress)
        {
            try
            {
                var response = new ApiResponse()
                {
                    message = "Success",
                    status = "200",
                    result = await _businessLogic.GetVerificationResultDB(IPAddress)
                };

                return response;
               
            }
            catch (Exception ex)
            {

                var response = new ApiResponse()
                {
                    message = "failed",
                    status = "500",
                    result = "Expection " + ex.Message
                };

                return response;
            }
            

        }

        [HttpPost]
        [Authorize]
        public async Task<ApiResponse> InsertRunlog([FromBody] InsertLogModel model)
        {
            try
            {
                var response = new ApiResponse()
                {
                    message = "Success",
                    status = "200",
                    result = await _businessLogic.InsertRunlogdb(model.user_id, model.sys_user, model.sys_name, model.sys_ip)
                };

                return response;

            }
            catch (Exception ex)
            {

                var response = new ApiResponse()
                {
                    message = "failed",
                    status = "500",
                    result = "Expection " + ex.Message
                };

                return response;
            }
            
        }


        [HttpPost]
        [Authorize]
        public async Task<ApiResponse> InsertImage([FromBody] InsertImageModel model)
        {
            try
            {
                byte [] imageData = null;
                if (model.imageData!=null)
                {
                    imageData = Convert.FromBase64String(model.imageData);
                }

                var response = new ApiResponse()
                {
                    message = "Success",
                    status = "200",
                    result = await _businessLogic.InsertResponseImageinDB(imageData, model.cnic, model.ipaddress)
                };

                return response;
               
            }
            catch (Exception ex)
            {

                var response = new ApiResponse()
                {
                    message = "failed",
                    status = "500",
                    result = "Expection " + ex.Message
                };

                return response;
            }
            
        }

        #endregion

        [HttpGet]
        [Authorize]
        public async Task<ApiResponse> InsertNadra(string cnic)
        {
            try
            {
                var response = new ApiResponse()
                {
                    message = "Success",
                    status = "200",
                    result = await _businessLogic.InsertNadradb(cnic)
                };
                return response;
            }
            catch (Exception ex)
            {

                var response = new ApiResponse()
                {
                    message = "failed",
                    status = "501",
                    result = "Expection " + ex.Message
                };

                return response;
            }

            //string requesttype = "PensionerVerif";
            //string requester = System.Security.Principal.WindowsIdentity.GetCurrent().Name.Substring(7); 
            
        }

        [HttpPost]
        [Authorize]
        public async Task<ApiResponse> UpdateNadraResults([FromBody] UpdateNadraResultModel model)
        {
            try
            {
                var response = new ApiResponse()
                {
                    message = "Success",
                    status = "200",
                    result = await _businessLogic.UpdateNadraBranchResultsDB(model.cnic, model.ipaddress, model.VERIFYED, model.NADRA)
                };

                return response;
            }
            catch (Exception ex)
            {

                var response = new ApiResponse()
                {
                    message = "failed",
                    status = "500",
                    result = "Expection " + ex.Message
                };

                return response;
            }
            
           // return await _businessLogic.UpdateNadraBranchResultsDB(model.cnic, model.ipaddress, model.VERIFYED, model.NADRA);
        }

        
        //public async Task<string> verifyPensiorFingerPrint([FromBody] FingerprintVerifyRequest request)
        //{
        //    try
        //    {


        //        string str = obj.dibVerifyFingerPrints(request.SESSION_ID, request.CITIZEN_NUMBER, request.CONTACT_NUMBER, request.FINGER_INDEX, request.FINGER_TEMPLATE, request.AREA_NAME);

        //        //code here for update log in database
        //        string[] results = str.Split('|');
        //        string ResultText = "";
        //        for (int i = 0; i < results.Length; i++)
        //        {
        //            string[] feilddata = results[i].Split(':');
        //            if (feilddata.Length > 1)
        //            {
        //                if (feilddata[0] != "PHOTOGRAPH")
        //                {
        //                    ResultText += feilddata[0] + " : " + feilddata[1] + ";" + "\r\n";
        //                    if (feilddata[0] == "SESSION_ID")
        //                    {
        //                        sessionid = feilddata[1];
        //                    }
        //                }
        //            }

        //        }

        //        Image img1 = DrawText(ResultText, 20);

        //        byte[] ResultByteArray;
        //        //byte[] ResultBytePhoto;
        //        using (MemoryStream ms = new MemoryStream())
        //        {
        //            img1.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
        //            ResultByteArray = ms.ToArray();
        //        }

        //        string response = await _businessLogic.updateResponseImmediate(request.CITIZEN_NUMBER, ResultText, ResultByteArray);
        //        return str;
        //    }
        //    catch (Exception ex)
        //    {
        //        //_businessLogic.WriteLog("Exception At Bridge : " + ex.Message);
        //        return ex.Message;

        //    }
        //}

        
        //public async Task<string> getLastVerificationResult(string CITIZEN_NUMBER)
        //{
        //    try
        //    {
        //        //_businessLogic.WriteLog("------------------------------------------------------------------------------");
        //        //_businessLogic.WriteLog("Pensior Last Verification Hitted At Bridge : " + DateTime.Now);
        //        //_businessLogic.WriteLog("Request from User : {CITIZEN_NUMBER : " + CITIZEN_NUMBER+ " }");

        //        string response = obj.dibGetLastVerification(CITIZEN_NUMBER);

        //        //_businessLogic.WriteLog("Response from DMZ : " + response);
        //        //_businessLogic.WriteLog("Response from DMZ At : " + DateTime.Now);
        //        return response;
        //    }
        //    catch (Exception ex)
        //    {
        //        //_businessLogic.WriteLog("Exception At Bridge : " + ex.Message);
        //        return ex.Message;
        //    }
        //}

        private Image DrawText(String text, int fontSize)
        {
            //first, create a dummy bitmap just to get a graphics object
            Font font = new Font("Arial", fontSize, FontStyle.Bold);
            Color textColor = new Color();
            textColor = ColorTranslator.FromHtml("#000000");
            Color backColor = new Color();
            backColor = ColorTranslator.FromHtml("#FFFFFF");
            Image img = new Bitmap(1, 1);
            Graphics drawing = Graphics.FromImage(img);

            //measure the string to see how big the image needs to be
            SizeF textSize = drawing.MeasureString(text, font);

            //free up the dummy image and old graphics object
            img.Dispose();
            drawing.Dispose();

            //create a new image of the right size
            img = new Bitmap((int)textSize.Width, (int)textSize.Height);

            drawing = Graphics.FromImage(img);

            //paint the background
            drawing.Clear(backColor);

            //create a brush for the text
            Brush textBrush = new SolidBrush(textColor);

            drawing.DrawString(text, font, textBrush, 0, 0);

            drawing.Save();

            textBrush.Dispose();
            drawing.Dispose();

            return img;

        }

    }
}
