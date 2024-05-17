using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.IO;
using System.Xml.Serialization;
using NADRA_PENSIONER_API.Model;
using System.Xml;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using NADRA_PENSIONER_API.Model.Nadra;

namespace NADRA_PENSIONER_API
{
    public class BusinessLogic
    {
        Image resized;
        static string TRANSACTION_ID = "";
        static string FRANCHISEE_ID = "1505";
        private readonly DataAccess _dataAccess;

        public BusinessLogic(DataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public bool UserAuthentication(List<JsonUserSetup> source ,string Id, string password)
        {
            bool result = false;

            for (int i = 0; i < source.Count; i++)
            {
                //if (source[i].ID == user && DecryptValue(source[i].PASSWORD.Substring(0, source[i].PASSWORD.Length - 16), source[i].PASSWORD.Substring(source[i].PASSWORD.Length - 16, 16)) == password)
                if (source[i].ID == Id && source[i].PASSWORD == password)
                {
                    result = true;
                }
                else
                {
                    //result = false;
                }
            }

            return result;
        }

        //Inserting Logs into database
        public int InsertLogs(string callingFunc, string request, string response, string msg)
        {
            _dataAccess.Query = @"INSERT INTO DIB_RDP_CIF_INQUIRY_LOG C VALUES 
                                  ('" + callingFunc + "', '" + request + "', SYSDATE, '" + response + "', SYSDATE, '" + msg + "')";
            return _dataAccess.ExecuteNonQuery();
        }
        public int InsertLogs(Logs logs)
        {
            _dataAccess.Query = @"INSERT INTO dib_sandboxapi_logs VALUES 
                                  ('" + logs.controller_name+ "', '" + logs.action_name + "', '"+logs.request+ "', TO_DATE('" + logs.request_date_time + "', 'DD-MM-YYYY HH:MI:SS AM'), '" + logs.response+ "', TO_DATE('" + logs.response_date_time + "', 'DD-MM-YYYY HH:MI:SS AM'),'" + logs.status_code+"','"+logs.status_msg+"')";
            return _dataAccess.ExecuteNonQuery();

            
        }
        public int InsertNewLogs(Logs logs)
        {
            

            _dataAccess.Query = @"INSERT INTO dib_sandboxapi_logs VALUES 
                                  ('" + logs.controller_name + "','" + logs.action_name + "', '" + logs.request + "', TO_DATE('" + logs.request_date_time + "','DD-MM-YYYY HH:MI:SS AM'), '" + logs.response + "',  TO_DATE('" + logs.response_date_time + "','DD-MM-YYYY HH:MI:SS AM'), '" + logs.status_code + "','" + logs.status_msg + "','" + logs.project_name + "')";
            return _dataAccess.ExecuteNonQuery();
        }

        //For maintaining logs in text file
        public void WriteLog(string message)
        {
            string path = $"{Directory.GetCurrentDirectory()}{@"\Logs"}";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = path + "\\" + DateTime.Now.ToString("yyyyMMdd") + " - Log.txt";

            if (!File.Exists(filepath))
            {
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(message);
                }
            }
        }

        public string ToXML<T>(T obj)
        {
            using (var stringwriter = new System.IO.StringWriter())
            {
                var serializer = new XmlSerializer(obj.GetType());
                serializer.Serialize(stringwriter, obj);
                var str = stringwriter.ToString();
                return str;
            }
        }

        public string GetRequiredFieldsFromXML(string xmlResponseString, string tagName)
        {
            //Convert response string to xml
            XmlDocument xmlContainer = new XmlDocument();
            xmlContainer.LoadXml(xmlResponseString);

            //Get the required values from response
            XmlNodeList ParentNode = xmlContainer.GetElementsByTagName(tagName);
            string result = ParentNode.Item(0).InnerText;

            return result;
        }
        
        public string ParseForMobile(string NADRAresult, string cnic, string deviceID, string username, string transactionID,string NADRALog,string apid)
        {
            string code = "";
            string message = "";
            string sessionid = "";
            string citizenNumber = "";
            string name = "";
            string father_husband_name = "";
            string presentAddress = "";
            string permanentAddress = "";
            string mother_name = "";
            string dob = "";
            string birthPlace = "";
            string expiryDate = "";
            string cardType = "";
            string finger_index = "";
            string photo = "";

            //Checking Purpose
            string issuedate = "";
            string birthdate = "";
            string cnicnumber = "";
            string area = "";

            string[] results = NADRAresult.Split(',');
            string ResultText = "<VERIFICATION   xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">" + "\r\n";
            ResultText += "<RESPONSE_DATA>" + "\r\n";
            ResultText += "<RESPONSE_STATUS>" + "\r\n";
            string ResultDB = "";
            string ResultTextImage = "";
            string verified;
            int isSuccess = 0;
            for (int i = 0; i < results.Length; i++)
            {
                string[] feilddata = results[i].Split('"');
                if (feilddata.Length >= 1)
                {
                    if (feilddata[1] == "photograph" && !String.IsNullOrEmpty(feilddata[3]))
                    {
                        string profileStr = feilddata[3];
                        Image img = Base64ToImage(profileStr);
                        resized = FixedSize(img, 260, 320);
                        isSuccess = 1;
                        photo = "<" + feilddata[1] + ">" + feilddata[3] + "</" + feilddata[1] + ">" + "\r\n";
                    }
                    else
                    {
                        ResultTextImage += feilddata[1] + " : " + feilddata[3] + "\r\n";
                        ResultDB += feilddata[1] + " : " + feilddata[3] + ";";

                        switch (feilddata[1])
                        {
                            case "status_code":
                                code = "<" + feilddata[1] + ">" + feilddata[3] + "</" + feilddata[1] + ">" + "\r\n";
                                break;
                            case "status_message":
                                message = "<" + feilddata[1] + ">" + feilddata[3] + "</" + feilddata[1] + ">" + "\r\n";
                                break;
                            case "session_id":
                                sessionid = "<" + feilddata[1] + ">" + feilddata[3] + "</" + feilddata[1] + ">" + "\r\n";
                                break;
                            case "citizen_number":
                                citizenNumber = "<" + feilddata[1] + ">" + feilddata[3] + "</" + feilddata[1] + ">" + "\r\n";
                                break;
                            case "name":
                                name = "<" + feilddata[1] + ">" + feilddata[3] + "</" + feilddata[1] + ">" + "\r\n";
                                break;
                            case "father_husband_name":
                                father_husband_name = "<" + feilddata[1] + ">" + feilddata[3] + "</" + feilddata[1] + ">" + "\r\n";
                                break;
                            case "present_address":
                                presentAddress = "<" + feilddata[1] + ">" + feilddata[3] + "</" + feilddata[1] + ">" + "\r\n";
                                break;
                            case "permanent_address":
                                permanentAddress = "<" + feilddata[1] + ">" + feilddata[3] + "</" + feilddata[1] + ">" + "\r\n";
                                break;
                            case "date_of_birth":
                                dob = "<" + feilddata[1] + ">" + feilddata[3] + "</" + feilddata[1] + ">" + "\r\n";
                                break;
                            case "birth_place":
                                birthPlace = "<" + feilddata[1] + ">" + feilddata[3] + "</" + feilddata[1] + ">" + "\r\n";
                                i = i + 1;
                                break;
                            case "mother_name":
                                mother_name = "<" + feilddata[1] + ">" + feilddata[3] + "</" + feilddata[1] + ">" + "\r\n";
                                break;
                            case "photograph":
                                photo = "<" + feilddata[1] + ">" + feilddata[3] + "</" + feilddata[1] + ">" + "\r\n";
                                break;
                            case "card_type":
                                cardType = "<" + feilddata[1] + ">" + feilddata[3] + "</" + feilddata[1] + ">" + "\r\n";
                                break;
                            case "expiry_date":
                                expiryDate = "<" + feilddata[1] + ">" + feilddata[3] + "</" + feilddata[1] + ">" + "\r\n";
                                break;
                            case "code":
                                code = "<" + feilddata[1] + ">" + feilddata[3] + "</" + feilddata[1] + ">" + "\r\n";
                                break;
                            case "message":
                                message = "<" + feilddata[1] + ">" + feilddata[3] + "</" + feilddata[1] + ">" + "\r\n";
                                break;
                            case "finger_index":
                                finger_index = "<" + feilddata[1] + ">" + feilddata[3] + "</" + feilddata[1] + ">" + "\r\n";
                                break;
                        }
                    }
                }
            }
            ResultText += code + message + "</RESPONSE_STATUS>";
            ResultText += sessionid + citizenNumber;
            ResultText += "<PERSON_DATA>" + "\r\n";
            ResultText += name;
            if (father_husband_name != "")
            {
                ResultText += father_husband_name;
            }
            if (presentAddress != "")
            {
                ResultText += presentAddress;
            }
            if (permanentAddress != "")
            {
                ResultText += permanentAddress;
            }
            if (dob != "")
            {
                ResultText += dob;
            }
            if (birthPlace != "")
            {
                ResultText += birthPlace;
            }
            if (mother_name != "")
            {
                ResultText += mother_name;
            }
            if (photo != "")
            {
                ResultText += photo;
            }
            if (expiryDate != "")
            {
                ResultText += expiryDate;
            }           
            if (cardType != "")
            {
                ResultText += cardType;
            }
            if (code != "")
            {
                ResultText += code;
            }
            if (message != "")
            {
                ResultText += message;
            }
            if (finger_index != "")
            {
                ResultText += finger_index;
            }

            ResultText += "</PERSON_DATA></RESPONSE_DATA></VERIFICATION>";

            Image img1 = DrawText(ResultTextImage, 20);
            byte[] ResultByteArray;
            byte[] ResultBytePhoto;
            using (MemoryStream ms = new MemoryStream())
            {
                img1.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                ResultByteArray = ms.ToArray();
            }
            using (MemoryStream ms1 = new MemoryStream())
            {
                if (isSuccess == 1)
                {
                    
                    resized.Save(ms1, System.Drawing.Imaging.ImageFormat.Jpeg);
                    ResultBytePhoto = ms1.ToArray();
                }
                else
                {
                    Image img2 = DrawText("Error in Verification", 22);
                    img2.Save(ms1, System.Drawing.Imaging.ImageFormat.Jpeg);
                    ResultBytePhoto = ms1.ToArray();
                }


            }
            if (results[0].Contains("100"))
            {
                verified = "1";
            }
            else
            {
                verified = "0";
            }
            string upd = UpdateMobileNadraBranchResultsDB(cnic, deviceID, verified, NADRALog, ResultText, ResultByteArray, ResultBytePhoto, transactionID, username, apid);


            return ResultText;
        }


        public string ParseForMobile2(string NADRAresult, string cnic, string deviceID, string username, string transactionID, string NADRALog, string apid)
        {
            string code = "";
            string message = "";
            string sessionid = "";
            string citizenNumber = "";
            string name = "";
            string father_husband_name = "";
            string presentAddress = "";
            string permanentAddress = "";
            string mother_name = "";
            string dob = "";
            string birthPlace = "";
            string expiryDate = "";
            string cardType = "";
            string finger_index = "";
            string photo = "";

            //Checking Purpose
            string issuedate = "";
            string birthdate = "";
            string cnicnumber = "";
            string area = "";

            string[] results = NADRAresult.Split('"');
            string ResultText = "<VERIFICATION   xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">" + "\r\n";
            ResultText += "<RESPONSE_DATA>" + "\r\n";
            ResultText += "<RESPONSE_STATUS>" + "\r\n";
            string ResultDB = "";
            string ResultTextImage = "";
            string verified;
            int isSuccess = 0;
            for (int i = 1; i < results.Length; i=i+4)
            {
                //string[] feilddata = results[i].Split('"');
                if (results.Length >= 1)
                {
                    if (results[i] == "photograph" && !String.IsNullOrEmpty(results[i+2]))
                    {
                        string profileStr = results[i+2];
                        Image img = Base64ToImage(profileStr);
                        resized = FixedSize(img, 260, 320);
                        isSuccess = 1;
                        photo = "<" + results[i] + ">" + results[i+2] + "</" + results[i] + ">" + "\r\n";
                    }
                    else
                    {
                        ResultTextImage += results[i] + " : " + results[i+2] + "\r\n";
                        ResultDB += results[i] + " : " + results[i+2] + ";";

                        switch (results[i])
                        {
                            case "status_code":
                                code = "<" + results[i] + ">" + results[i+2] + "</" + results[i] + ">" + "\r\n";
                                break;
                            case "status_message":
                                message = "<" + results[i] + ">" + results[i + 2] + "</" + results[i] + ">" + "\r\n";
                                break;
                            case "session_id":
                                sessionid = "<" + results[i] + ">" + results[i + 2] + "</" + results[i] + ">" + "\r\n";
                                break;
                            case "citizen_number":
                                citizenNumber = "<" + results[i] + ">" + results[i + 2] + "</" + results[i] + ">" + "\r\n";
                                break;
                            case "name":
                                name = "<" + results[i] + ">" + results[i + 2] + "</" + results[i] + ">" + "\r\n";
                                break;
                            case "father_husband_name":
                                father_husband_name = "<" + results[i] + ">" + results[i + 2] + "</" + results[i] + ">" + "\r\n";
                                break;
                            case "present_address":
                                presentAddress = "<" + results[i] + ">" + results[i + 2] + "</" + results[i] + ">" + "\r\n";
                                break;
                            case "permanent_address":
                                permanentAddress = "<" + results[i] + ">" + results[i + 2] + "</" + results[i] + ">" + "\r\n";
                                break;
                            case "date_of_birth":
                                dob = "<" + results[i] + ">" + results[i + 2] + "</" + results[i] + ">" + "\r\n";
                                break;
                            case "birth_place":
                                birthPlace = "<" + results[i] + ">" + results[i + 2] + "</" + results[i] + ">" + "\r\n";
                                break;
                            case "mother_name":
                                mother_name = "<" + results[i] + ">" + results[i + 2] + "</" + results[i] + ">" + "\r\n";
                                break;
                            case "photograph":
                                photo = "<" + results[i] + ">" + results[i + 2] + "</" + results[i] + ">" + "\r\n";
                                break;
                            case "card_type":
                                cardType = "<" + results[i] + ">" + results[i + 2] + "</" + results[i] + ">" + "\r\n";
                                break;
                            case "expiry_date":
                                expiryDate = "<" + results[i] + ">" + results[i + 2] + "</" + results[i] + ">" + "\r\n";
                                break;
                            case "code":
                                code = "<" + results[i] + ">" + results[i + 2] + "</" + results[i] + ">" + "\r\n";
                                break;
                            case "message":
                                message = "<" + results[i] + ">" + results[i + 2] + "</" + results[i] + ">" + "\r\n";
                                break;
                            case "finger_index":
                                finger_index = "<" + results[i] + ">" + results[i + 2] + "</" + results[i] + ">" + "\r\n";
                                break;
                        }
                    }
                }
            }
            ResultText += code + message + "</RESPONSE_STATUS>";
            ResultText += sessionid + citizenNumber;
            ResultText += "<PERSON_DATA>" + "\r\n";
            ResultText += name;
            if (father_husband_name != "")
            {
                ResultText += father_husband_name;
            }
            if (presentAddress != "")
            {
                ResultText += presentAddress;
            }
            if (permanentAddress != "")
            {
                ResultText += permanentAddress;
            }
            if (dob != "")
            {
                ResultText += dob;
            }
            if (birthPlace != "")
            {
                ResultText += birthPlace;
            }
            if (mother_name != "")
            {
                ResultText += mother_name;
            }
            if (photo != "")
            {
                ResultText += photo;
            }
            if (expiryDate != "")
            {
                ResultText += expiryDate;
            }
            if (cardType != "")
            {
                ResultText += cardType;
            }
            if (code != "")
            {
                ResultText += code;
            }
            if (message != "")
            {
                ResultText += message;
            }
            if (finger_index != "")
            {
                ResultText += finger_index;
            }

            ResultText += "</PERSON_DATA></RESPONSE_DATA></VERIFICATION>";

            Image img1 = DrawText(ResultTextImage, 20);
            byte[] ResultByteArray;
            byte[] ResultBytePhoto;
            using (MemoryStream ms = new MemoryStream())
            {
                img1.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                ResultByteArray = ms.ToArray();
            }
            using (MemoryStream ms1 = new MemoryStream())
            {
                if (isSuccess == 1)
                {

                    resized.Save(ms1, System.Drawing.Imaging.ImageFormat.Jpeg);
                    ResultBytePhoto = ms1.ToArray();
                }
                else
                {
                    Image img2 = DrawText("Error in Verification", 22);
                    img2.Save(ms1, System.Drawing.Imaging.ImageFormat.Jpeg);
                    ResultBytePhoto = ms1.ToArray();
                }


            }
            if (results[3].Contains("100"))
            {
                verified = "1";
            }
            else
            {
                verified = "0";
            }
            string upd = UpdateMobileNadraBranchResultsDB(cnic, deviceID, verified, NADRALog, ResultText, ResultByteArray, ResultBytePhoto, transactionID, username, apid);


            return ResultText;
        }

        public string InsertFPData(string NADRAresult, string cnic, string deviceID, string username, string transactionID, string NADRALog, string apid)
        {

            string verified;
            string[] results = NADRAresult.Split(',');
            
            if (results[0].Contains("100"))
            {
                verified = "1";
            }
            else
            {
                verified = "0";
            }
            string upd = UpdateMobileNadraBranchResultsDBforFP(cnic, deviceID, verified, NADRALog, transactionID, username, apid);
            return upd;
        }
        //For Adding the Text to Image
        private Image Base64ToImage(string base64String)
        {
            // Convert Base64 String to byte[]
            byte[] imageBytes = Convert.FromBase64String(base64String);
            // Convert byte[] to Image
            using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
            {
                Image image = Image.FromStream(ms, true);
                return image;
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
        static Image FixedSize(Image imgPhoto, int Width, int Height)
        {
            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;
            int sourceX = 0;
            int sourceY = 0;
            int destX = 0;
            int destY = 0;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)Width / (float)sourceWidth);
            nPercentH = ((float)Height / (float)sourceHeight);
            if (nPercentH < nPercentW)
            {
                nPercent = nPercentH;
                destX = System.Convert.ToInt16((Width -
                              (sourceWidth * nPercent)) / 2);
            }
            else
            {
                nPercent = nPercentW;
                destY = System.Convert.ToInt16((Height -
                              (sourceHeight * nPercent)) / 2);
            }

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap bmPhoto = new Bitmap(Width, Height,
                              PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution,
                             imgPhoto.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.Clear(Color.Red);
            grPhoto.InterpolationMode =
                    InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(imgPhoto,
                new Rectangle(destX, destY, destWidth, destHeight),
                new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel);

            grPhoto.Dispose();
            return bmPhoto;
        }

        public string GenerateTransactionID()
        {
            Random random = new Random();
            // First four digits of Transaction ID to be Franchisee ID
            TRANSACTION_ID = FRANCHISEE_ID + random.Next(1000000, 9999999).ToString("0000000") + random.Next(10000000, 99999999).ToString("00000000");
            return TRANSACTION_ID;
        }

        public DataTable getID()
        {
            _dataAccess.Query = @"select * from (select * from utils.dib_nadra order by requestdate desc) where ROWNUM <= 1";
            return _dataAccess.ExecuteDataTable();
        }
        public string InsertMobileNADRAVerification(int id, string cnic, string deviceID, string username, string transactionID, string branch,string purpose,string APID,string session_id)
        {
            if (cnic.Length > 13 || cnic.Length < 13)
            {
                _dataAccess.Query = @"insert into utils.dib_nadra(id,cnic, apid, ipaddress, requestdate, requester, branch, instrument_code,requesttype,reference_no) values('','Invalid CNIC','"+APID+"','" + deviceID + "',SYSDATE,upper('" + username + "'),'" + branch + "','" + transactionID + "','" + purpose + "','" + session_id + "')";
                return _dataAccess.ExecuteNonQuery().ToString();
            }
            else 
            {
                _dataAccess.Query = @"insert into utils.dib_nadra(id,cnic, apid, ipaddress, requestdate, requester, branch, instrument_code,requesttype,reference_no) values('','" + cnic + "','" + APID + "','" + deviceID + "',SYSDATE,upper('" + username + "'),'" + branch + "','" + transactionID + "','" + purpose + "','" + session_id + "')";
                return _dataAccess.ExecuteNonQuery().ToString();
            }
        }

        public string UpdateMobileNadraBranchResultsDB(string cnic, string ipaddress, string VERIFYED, string parsed, string SentToAPI, byte[] imageData, byte[] photoData, string transactionID, string username,string APID)
        {
            if (cnic.Length > 13 || cnic.Length < 13)
            {
                _dataAccess.Query = "update utils.dib_nadra n set RESPONSE_BLOB=:1 , RESPONSE_PHOTO=:2  ,RESPONSE_CLOB= :3, VERIFYED='" + VERIFYED + "',responsedate=sysdate,response='" + parsed + "' where apid='"+APID+"' and ipaddress='" + ipaddress + "' and cnic = 'Invalid CNIC' and instrument_code='" + transactionID + "' and VERIFYED is null and upper(requester)=UPPER('" + username + "') and nvl(requestdate,sysdate) = (select max(nvl(requestdate,sysdate)) from utils.dib_nadra where ipaddress=n.ipaddress and cnic = n.cnic and apid = '" + APID + "')";
                if (_dataAccess.ExecuteNonQueryBLOB(imageData, photoData, SentToAPI) > 0)
                {
                    return "Updated";
                }
                else
                {
                    return "Not Updated";
                }
            }
            else 
            {
                _dataAccess.Query = "update utils.dib_nadra n set RESPONSE_BLOB=:1 , RESPONSE_PHOTO=:2  ,RESPONSE_CLOB= :3, VERIFYED='" + VERIFYED + "',responsedate=sysdate,response='" + parsed + "' where apid='" + APID + "' and ipaddress='" + ipaddress + "' and cnic = '" + cnic + "' and instrument_code='" + transactionID + "' and VERIFYED is null and upper(requester)=UPPER('" + username + "') and nvl(requestdate,sysdate) = (select max(nvl(requestdate,sysdate)) from utils.dib_nadra where ipaddress=n.ipaddress and cnic = n.cnic and apid = '" + APID + "')";
                if (_dataAccess.ExecuteNonQueryBLOB(imageData, photoData, SentToAPI) > 0)
                {
                    return "Updated";
                }
                else
                {
                    return "Not Updated";
                }
            }
        }

        public string UpdateMobileNadraBranchResultsDBforFP(string cnic, string ipaddress, string VERIFYED, string parsed, string transactionID, string username, string APID)
        {
            if (cnic.Length > 13 || cnic.Length < 13)
            {
                _dataAccess.Query = "update utils.dib_nadra n set VERIFYED='" + VERIFYED + "',responsedate=sysdate,response='" + parsed + "' where apid='" + APID + "' and ipaddress='" + ipaddress + "' and cnic = 'Invalid CNIC' and instrument_code='" + transactionID + "' and VERIFYED is null and upper(requester)=UPPER('" + username + "') and nvl(requestdate,sysdate) = (select max(nvl(requestdate,sysdate)) from utils.dib_nadra where ipaddress=n.ipaddress and cnic = n.cnic and apid = '" + APID + "')";
                return _dataAccess.ExecuteNonQuery().ToString();
            }
            else
            {
                _dataAccess.Query = "update utils.dib_nadra n set VERIFYED='" + VERIFYED + "',responsedate=sysdate,response='" + parsed + "' where apid='" + APID + "' and ipaddress='" + ipaddress + "' and cnic = '" + cnic + "' and instrument_code='" + transactionID + "' and VERIFYED is null and upper(requester)=UPPER('" + username + "') and nvl(requestdate,sysdate) = (select max(nvl(requestdate,sysdate)) from utils.dib_nadra where ipaddress=n.ipaddress and cnic = n.cnic and apid = '" + APID + "')";
                return _dataAccess.ExecuteNonQuery().ToString();
            }
        }


        public string getBetweenString(string xml, string startString, string endString)
        {
            XmlDocument xmlContainer = new XmlDocument();
            xmlContainer.LoadXml(xml);

            int TextFrom = xml.IndexOf(startString) + startString.Length;
            int TextTo = xml.LastIndexOf(endString);

            string result = xml.Substring(TextFrom, TextTo - TextFrom);


            return result;
        }

        public string[] getBetweenStringfingerindex(string xml, string startString, string endString)

        {


            //XmlSerializer serializer = new XmlSerializer(typeof(Response));
            //using (StringReader reader = new StringReader(xml))
            //{
            //    var test = (Response)serializer.Deserialize(reader);
            //}


            XmlDocument xmlContainer = new XmlDocument();
            xmlContainer.LoadXml(xml);

            int TextFrom = xml.IndexOf(startString) + startString.Length;
            int TextTo = xml.LastIndexOf(endString);

            string result = xml.Substring(TextFrom, TextTo - TextFrom);

            string[] results = result.Split("&lt;/FINGER&gt;\r\n                                                  &lt;FINGER&gt;");


            //int TextFrom = xml.IndexOf(startString);
            //int TextTo = xml.LastIndexOf(endString)+ endString.Length;

            //string result = xml.Substring(TextFrom, TextTo);
            return results;


        }




        #region PENSIONER




        public async Task<string> GetCNICdb(string IPAddress)
        {
            //Query = "select CNIC from dib_nadra where IPADDRESS='" + IPAddress + "'";
            _dataAccess.Query = "select cnic from dib_nadra n where IPADDRESS='" + IPAddress + "' and requestdate = (select max(requestdate) from dib_nadra where IPADDRESS=n.IPADDRESS) and nvl(verifyed,0) = 0";

            return _dataAccess.ExecuteScaler().ToString();
             
        }
        public async Task<string> VerifyUserDB(string username)
        {
            _dataAccess.Query   = "select ID from dib_nadra_users where username='" + username + "'";
             
            return _dataAccess.ExecuteScaler().ToString();
             
        }

        public async Task<string> UpdateNadraBranchResultsDB(string cnic, string ipaddress, string VERIFYED, string NADRA)
        {
             _dataAccess.Query = "update dib_nadra n set ipaddress = '" + ipaddress + "',VERIFYED='" + VERIFYED + "',responsedate=sysdate,response='" + NADRA + "' where cnic='" + cnic + "' and VERIFYED is null and nvl(requestdate,sysdate) = (select max(nvl(requestdate,sysdate)) from dib_nadra where cnic=n.cnic)";
            
            return _dataAccess.ExecuteScaler().ToString();
        }

        public string UpdateNadraBranchlessResultsDB(string cnic, string ipaddress, string VERIFYED, string NADRA, byte[] imageData)
        {


              _dataAccess.Query = "update dib_nadra n set RESPONSE_BLOB=:1 ,VERIFYED='" + VERIFYED + "',responsedate=sysdate,response='" + NADRA + "' where ipaddress='" + ipaddress + "' and cnic='" + cnic + "' and VERIFYED is null and nvl(requestdate,sysdate) = (select max(nvl(requestdate,sysdate)) from dib_nadra where ipaddress=n.ipaddress and cnic=n.cnic)";
            if (_dataAccess.ExecuteNonQueryBLOB(imageData) > 0)
            {
                return "Updated";
            }
            else
            {
                return "Not Updated";
            }
        }
        public async Task<string> GetVerificationResultDB(string IPAddress)
        {
            //Query = "select CNIC from dib_nadra where IPADDRESS='" + IPAddress + "'";
            _dataAccess.Query = "select response from dib_nadra n where IPADDRESS='" + IPAddress + "' and requestdate = (select max(requestdate) from dib_nadra where IPADDRESS=n.IPADDRESS) and nvl(verifyed,0) = 1";

            return _dataAccess.ExecuteScaler().ToString();
        }

        public async Task<string> InsertRunlogdb(string puser_id, string psys_user, string psys_name, string psys_ip)
        {
            //Query = "select CNIC from dib_nadra where IPADDRESS='" + IPAddress + "'";
            _dataAccess.Query = "insert into dib_nadra_run_log(user_id, rundatetime, sys_user, sys_name, sys_ip) values ('" + puser_id + "',SYSDATE ,'" + psys_user + "','" + psys_name + "','" + psys_ip + "')";
            
            return _dataAccess.ExecuteScaler().ToString();
        }

        public async Task<string> InsertNadradb(string cnic)
        {
            _dataAccess.Query = "insert into dib_nadra(cnic,requestdate) values ('" + cnic + "',sysdate)";
            
            return _dataAccess.ExecuteScaler().ToString();

        }


        public async Task<string> InsertResponseImageinDB(byte[] imageData, string cnic, string ipaddress)
        {

           _dataAccess.Query ="update dib_nadra n set RESPONSE_BLOB=:1 where ipaddress='" + ipaddress + "' and cnic='" + cnic + "' and VERIFYED is not null and nvl(requestdate,sysdate) = (select max(nvl(requestdate,sysdate)) from dib_nadra where ipaddress=n.ipaddress and cnic=n.cnic) and response_blob is null";
            return _dataAccess.ExecuteNonQueryBLOB(imageData).ToString();
        }

        public async Task<string> updateResponseImmediate(string cnic, string response, byte[] bytearray)
        {
           _dataAccess.Query ="update dib_nadra n set RESPONSE_BLOB=:1, RESPONSE='" + response + "', responsedate=sysdate,instrument_code='" + cnic + "'|| to_char(requestdate,'DDMMRRHHMISS') where cnic='" + cnic + "' and VERIFYED is null and nvl(requestdate,sysdate) = (select max(nvl(requestdate,sysdate)) from dib_nadra where cnic=n.cnic)";
            //_dataAccess.Query = "update utils.dib_nadra n set RESPONSE_BLOB=:1 , RESPONSE_PHOTO=:2  ,RESPONSE_CLOB= :3, VERIFYED='" + VERIFYED + "',responsedate=sysdate,response='" + parsed + "' where apid='" + APID + "' and ipaddress='" + ipaddress + "' and cnic = '" + cnic + "' and instrument_code='" + transactionID + "' and VERIFYED is null and upper(requester)=UPPER('" + username + "') and nvl(requestdate,sysdate) = (select max(nvl(requestdate,sysdate)) from utils.dib_nadra where ipaddress=n.ipaddress and cnic = n.cnic and apid = '" + APID + "')
            if (_dataAccess.ExecuteNonQueryBLOB(bytearray) > 0)
            {
                return "Updated";
            }
            else
            {
                return "Not Updated";
            }

            //return ExecuteNonQueryBLOB(bytearray).ToString();
        }

        #endregion


    }

}
