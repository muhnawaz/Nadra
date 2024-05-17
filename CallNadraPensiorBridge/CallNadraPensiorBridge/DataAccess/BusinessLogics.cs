using System;
using System.Data;
using System.Data.OracleClient;
using System.Collections.Generic;
using System.IO;

namespace DataAccess
{
    public class BusinessLogics : DataAccess
    {

        public string GetCNICdb(string IPAddress)
        {
            //Query = "select CNIC from dib_nadra where IPADDRESS='" + IPAddress + "'";
            Query = "select cnic from dib_nadra n where IPADDRESS='" + IPAddress + "' and requestdate = (select max(requestdate) from dib_nadra where IPADDRESS=n.IPADDRESS) and nvl(verifyed,0) = 0";
            return ExecuteScaler();
        }
        public string VerifyUserDB(string username)
        {
            Query = "select ID from dib_nadra_users where username='" + username + "'";
            return ExecuteScaler();
        }
        //public string UpdateNadraBranchResultsDB(string cnic, string ipaddress, string VERIFYED, string NADRA, byte[] imageData, byte[] photoData)
        //{
        //    Query = "update dib_nadra n set RESPONSE_BLOB=:1 , RESPONSE_PHOTO=:2  ,VERIFYED='" + VERIFYED + "',responsedate=sysdate,response='" + NADRA + "' where ipaddress='" + ipaddress + "' and cnic='" + cnic + "' and VERIFYED is null and nvl(requestdate,sysdate) = (select max(nvl(requestdate,sysdate)) from dib_nadra where ipaddress=n.ipaddress and cnic=n.cnic)";
        //    if (ExecuteNonQueryBLOB(imageData, photoData) > 0)
        //    {
        //        return "Updated";
        //    }
        //    else
        //    {
        //        return "Not Updated";
        //    }
        //}

        public string UpdateNadraBranchResultsDB(string cnic, string ipaddress, string VERIFYED, string NADRA)
        {
            Query = "update dib_nadra n set ipaddress = '" + ipaddress + "',VERIFYED='" + VERIFYED + "',responsedate=sysdate,response='" + NADRA + "' where cnic='" + cnic + "' and VERIFYED is null and nvl(requestdate,sysdate) = (select max(nvl(requestdate,sysdate)) from dib_nadra where cnic=n.cnic)";
            return ExecuteScaler();
        }

        public string UpdateNadraBranchlessResultsDB(string cnic, string ipaddress, string VERIFYED, string NADRA, byte[] imageData)
        {
            Query = "update dib_nadra n set RESPONSE_BLOB=:1 ,VERIFYED='" + VERIFYED + "',responsedate=sysdate,response='" + NADRA + "' where ipaddress='" + ipaddress + "' and cnic='" + cnic + "' and VERIFYED is null and nvl(requestdate,sysdate) = (select max(nvl(requestdate,sysdate)) from dib_nadra where ipaddress=n.ipaddress and cnic=n.cnic)";
            if (ExecuteNonQueryBLOB(imageData) > 0)
            {
                return "Updated";
            }
            else
            {
                return "Not Updated";
            }
        }
        public string GetVerificationResultDB(string IPAddress)
        {
            //Query = "select CNIC from dib_nadra where IPADDRESS='" + IPAddress + "'";
            Query = "select response from dib_nadra n where IPADDRESS='" + IPAddress + "' and requestdate = (select max(requestdate) from dib_nadra where IPADDRESS=n.IPADDRESS) and nvl(verifyed,0) = 1";
            return ExecuteScaler();
        }

        public string InsertRunlogdb(string puser_id, string psys_user, string psys_name, string psys_ip)
        {
            //Query = "select CNIC from dib_nadra where IPADDRESS='" + IPAddress + "'";
            Query = "insert into dib_nadra_run_log(user_id, rundatetime, sys_user, sys_name, sys_ip) values ('" + puser_id + "',SYSDATE ,'" + psys_user + "','" + psys_name + "','" + psys_ip + "')";
            return ExecuteScaler();
        }

        public string InsertNadradb(string cnic)
        {
            Query = "insert into dib_nadra(cnic,requestdate) values ('" + cnic + "',sysdate)";
            return ExecuteScaler();

        }


        public string InsertResponseImageinDB(byte[] imageData, string cnic, string ipaddress)
        {

            Query = "update dib_nadra n set RESPONSE_BLOB=:1 where ipaddress='" + ipaddress + "' and cnic='" + cnic + "' and VERIFYED is not null and nvl(requestdate,sysdate) = (select max(nvl(requestdate,sysdate)) from dib_nadra where ipaddress=n.ipaddress and cnic=n.cnic) and response_blob is null";
            return ExecuteNonQueryBLOB(imageData).ToString();
        }

        public string updateResponseImmediate(string cnic, string response,byte[] bytearray)
        {
            Query = "update dib_nadra n set RESPONSE_BLOB=:1, RESPONSE='" + response + "', responsedate=sysdate,instrument_code='"+cnic+"'|| to_char(requestdate,'DDMMRRHHMISS') where cnic='" + cnic + "' and VERIFYED is null and nvl(requestdate,sysdate) = (select max(nvl(requestdate,sysdate)) from dib_nadra where cnic=n.cnic)";
            //_dataAccess.Query = "update utils.dib_nadra n set RESPONSE_BLOB=:1 , RESPONSE_PHOTO=:2  ,RESPONSE_CLOB= :3, VERIFYED='" + VERIFYED + "',responsedate=sysdate,response='" + parsed + "' where apid='" + APID + "' and ipaddress='" + ipaddress + "' and cnic = '" + cnic + "' and instrument_code='" + transactionID + "' and VERIFYED is null and upper(requester)=UPPER('" + username + "') and nvl(requestdate,sysdate) = (select max(nvl(requestdate,sysdate)) from utils.dib_nadra where ipaddress=n.ipaddress and cnic = n.cnic and apid = '" + APID + "')
            if (ExecuteNonQueryBLOB(bytearray) > 0)
            {
                return "Updated";
            }
            else
            {
                return "Not Updated";
            }
            
            //return ExecuteNonQueryBLOB(bytearray).ToString();
        }

        //For maintaining logs in text file
        public void WriteLog(string message)
        {
            //string path = Directory.GetCurrentDirectory();
            //path += "/Logs";
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\";

           // string path = Environment.CurrentDirectory;
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

    }
}
