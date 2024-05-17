using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using DataAccess;
using System.IO;
using CallNadraPensiorBridge.CallNadraPensiorDMZ;
using System.Drawing;

namespace CallNadraPensiorBridge
{
    /// <summary>
    /// Summary description for Nadra_Pensior_Bridge
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class Nadra_Pensior_Bridge : System.Web.Services.WebService
    {
        

        CallNadraPensiorDMZ.CallNadraPensior obj = new CallNadraPensiorDMZ.CallNadraPensior();
        BusinessLogics _businessLogic = new BusinessLogics();
        //string APID = "1";
        //string branch = "3351";
        string sessionid = string.Empty;
        
        //database Logging
        #region
        [WebMethod]
        public string VerifyUser(string Username)
        {
            return _businessLogic.VerifyUserDB(Username);
        }
        [WebMethod]
        public string GetCNIC(string IPAddress)
        {
            return _businessLogic.GetCNICdb(IPAddress);

        }
        [WebMethod]
        public string GetVerificationResult(string IPAddress)
        {
            return _businessLogic.GetVerificationResultDB(IPAddress);

        }
        [WebMethod]
        public string InsertRunlog(string user_id, string sys_user, string sys_name, string sys_ip)
        {

            return _businessLogic.InsertRunlogdb(user_id, sys_user, sys_name, sys_ip);
        }
        //[WebMethod]
        //public string UpdateNadraRessults(string cnic, string ipaddress, string VERIFYED, string NADRA, byte[] imageData, byte[] photoData)
        //{
        //    return _businessLogic.UpdateNadraBranchResultsDB(cnic, ipaddress, VERIFYED, NADRA, imageData, photoData);
        //}
        [WebMethod]
        public string InsertImage(byte[] imageData, string cnic, string ipaddress)
        {
            return  _businessLogic.InsertResponseImageinDB(imageData, cnic, ipaddress);
        }
       // [WebMethod]
        //public string SubmitBankAccountDetails(string TRANSACTION_ID, string SESSION_ID, string CITIZEN_NUMBER, string CONTACT_NUMBER, string FINGER_INDEX, string AREA_NAME)
        //{
        //    return obj.DIBSubmitBankAccountDetails(TRANSACTION_ID, SESSION_ID, CITIZEN_NUMBER, CONTACT_NUMBER, FINGER_INDEX, AREA_NAME);
        //}
        #endregion

        [WebMethod]
        public string InsertNadra(string cnic)
        {
            //string requesttype = "PensionerVerif";
            //string requester = System.Security.Principal.WindowsIdentity.GetCurrent().Name.Substring(7); 
            return _businessLogic.InsertNadradb(cnic);
        }

        [WebMethod]
        public string UpdateNadraResults(string cnic, string ipaddress, string VERIFYED, string NADRA)
        {
            
            return _businessLogic.UpdateNadraBranchResultsDB(cnic, ipaddress, VERIFYED, NADRA);
        }

        [WebMethod]
        public string verifyPensiorFingerPrint(string SESSION_ID,string CITIZEN_NUMBER,string CONTACT_NUMBER,string FINGER_INDEX,string FINGER_TEMPLATE,string AREA_NAME,string ACCOUNT_TYPE)
        {
            try
            {
                
                
                string str = obj.dibVerifyFingerPrints(SESSION_ID, CITIZEN_NUMBER, CONTACT_NUMBER, FINGER_INDEX, FINGER_TEMPLATE, AREA_NAME);

                //code here for update log in database
                string[] results = str.Split('|');
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
                

             

                string response = _businessLogic.updateResponseImmediate(CITIZEN_NUMBER, ResultText, ResultByteArray);
                return str;
            }
            catch(Exception ex) 
            {
                //_businessLogic.WriteLog("Exception At Bridge : " + ex.Message);
                return ex.Message;

            }
        }

        [WebMethod]
        public string getLastVerificationResult(string CITIZEN_NUMBER) 
        {
            try
            {
                //_businessLogic.WriteLog("------------------------------------------------------------------------------");
                //_businessLogic.WriteLog("Pensior Last Verification Hitted At Bridge : " + DateTime.Now);
                //_businessLogic.WriteLog("Request from User : {CITIZEN_NUMBER : " + CITIZEN_NUMBER+ " }");

                string response = obj.dibGetLastVerification(CITIZEN_NUMBER);
                
                //_businessLogic.WriteLog("Response from DMZ : " + response);
                //_businessLogic.WriteLog("Response from DMZ At : " + DateTime.Now);
                return response;
            }
            catch(Exception ex)
            {
                //_businessLogic.WriteLog("Exception At Bridge : " + ex.Message);
                return ex.Message;
            }
        }

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
