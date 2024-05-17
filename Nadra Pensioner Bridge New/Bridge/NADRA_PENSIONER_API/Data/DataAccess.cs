using System;
using System.Data.SqlClient;
using System.Data;
using System.Data.OracleClient;
using System.Configuration;
using System.Data.OleDb;
using System.Data.Odbc;
using System.Web;
using System.IO;
using Oracle.ManagedDataAccess.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using System.Text;
using NADRA_PENSIONER_API;

namespace Data
{
    public class DataAccess
    {
        public DataAccess()
        {
            m_ConnectionString = Startup.Configuration.GetConnectionString("oracle_constr");
            //m_ConnectionString = Decrypt(m_ConnectionString.Substring(0, m_ConnectionString.Length - 16), m_ConnectionString.Substring(m_ConnectionString.Length - 16, 16));
        }
        #region ---- Variables ----

        public string staffid = "192291";
        public string password = "q3:0CHMo";
        public string bankcode = "DI";
        public string env = "PROD";
        private string m_server;
        private string m_database;
        private string m_username;
        private string m_password;
        private string m_ConnectionString;
        public string ErrMessage = "<br>No Errors";
        protected OracleConnection Oracle_connection = new OracleConnection();
        public OracleTransaction DbTxn;
        protected OracleDataReader reader;
        protected OracleCommand cmd;
        protected OracleDataAdapter mobjSqlDataAdapter;
        protected OracleCommandBuilder mobjSqlCommandBuilder;
        protected OracleTransaction mobjSqlTransaction;
        protected OracleConnection mobjSqlConn;
        public string server;
        public string dbname;
        public string userid;
        public string pass;
        public string mstrConnectionString;
        public string Query;
        public string excError;

        //public string ExecuteDatabaseReader()
        //{
        //    string word = null;
        //    mobjSqlCommand.CommandText = Query;
        //    OpenDatabaseConnection();
        //    reader = mobjSqlCommand.ExecuteReader();
        //    reader.Read();
        //    word = reader.GetString(0);
        //    CloseDatabaseConnection();
        //    return word;
        //}
        #endregion

        //for decryption
        public string Decrypt(string input, string key)
        {
            byte[] inputArray = Convert.FromBase64String(input);
            TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
            tripleDES.Key = UTF8Encoding.UTF8.GetBytes(key);
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tripleDES.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            tripleDES.Clear();
            return UTF8Encoding.UTF8.GetString(resultArray);
        }
        protected Boolean OpenDataBase()
        {
            try
            {
                if (Oracle_connection.State == ConnectionState.Closed)
                {
                    if (m_ConnectionString == null)
                    {
                        throw new Exception("Connection String Problem");
                    }

                    Oracle_connection.ConnectionString = this.m_ConnectionString;
                    Oracle_connection.Open();
                    return true;
                }
            }

            catch (OracleException ex)
            {
                excError = ex.Message;
                return false;
            }
            catch (Exception ex)
            {
                excError = ex.Message;
                return false;
            }
            return true;
        }

        protected Boolean OpenDataBase(string strCon)
        {
            try
            {
                if (Oracle_connection.State == ConnectionState.Closed)
                {
                    Oracle_connection.ConnectionString = strCon;
                    Oracle_connection.Open();
                    return true;
                }
            }

            catch (OracleException ex)
            {
                excError = ex.Message;
                return false;
            }
            catch (Exception ex)
            {
                excError = ex.Message;
                return false;
            }
            return true;
        }

        private void CloseDataBase()
        {
            if (Oracle_connection.State != ConnectionState.Closed)
            {
                Oracle_connection.Close();
            }
        }


        public bool OpenDatabaseConnection()
        {
            try
            {
                if (Oracle_connection.State == ConnectionState.Closed)
                {
                    if (m_ConnectionString == null)
                    {
                        throw new Exception("Connection String Problem");
                    }

                    Oracle_connection.ConnectionString = this.m_ConnectionString;
                    Oracle_connection.Open();
                    return true;
                }
            }
            
            catch (OracleException ex)
            {
                excError = ex.Message;
                return false;
            }
            catch (Exception ex)
            {
                excError = ex.Message;
                return false;
            }
            return true;
        }
        public void CloseDatabaseConnection()
        {
            try
            {
                if (Oracle_connection.State != ConnectionState.Closed)
                    Oracle_connection.Close();
            }
            catch (System.Exception)
            { }
        }

        public DataTable ExecuteDataTable()
        {
            DataTable dt = new DataTable("DT");
            try
            {
                if (OpenDataBase() == false)
                {

                }
                else
                {
                    OracleDataAdapter DataAdaptor;
                    DataAdaptor = new OracleDataAdapter(Query, Oracle_connection);

                    DataAdaptor.Fill(dt);

                    return dt;
                }
            }

            catch (OracleException ex)
            {
                excError = ex.Message;

            }
            catch (Exception ex)
            {
                excError = ex.Message;

            }
            finally
            {
                CloseDataBase();
            }
            return dt;
        }


        public DataTable ExecuteDataTableprms()
        {
            DataTable dt = new DataTable("DT");
            try
            {


                if (OpenDataBase() == false)
                {

                }
                else
                {
                    cmd.CommandText = Query;
                    cmd.Connection = Oracle_connection;
                    OracleDataAdapter DataAdaptor;
                    DataAdaptor = new OracleDataAdapter(cmd);

                    DataAdaptor.Fill(dt);

                    return dt;
                }
            }

            catch (OracleException ex)
            {
                excError = ex.Message;

            }
            catch (Exception ex)
            {
                excError = ex.Message;

            }
            finally
            {
                CloseDataBase();
            }
            return dt;
        }
        public int ExecuteNonQuery()
        {
            int iRowsAffected = 0;
            try
            {
                if (OpenDataBase() == false)
                {

                }
                else
                {
                    OracleCommand cmd = new OracleCommand();
                    cmd.Connection = Oracle_connection;
                    cmd.CommandText = Query;
                    Query = Query.Replace("\n", " ").Replace("\t", "").Replace("&", "& ");
                    cmd.CommandType = CommandType.Text;
                    iRowsAffected = cmd.ExecuteNonQuery();


                    return iRowsAffected;


                }
            }

            catch (OracleException ex)
            {
                excError = ex.Message;

            }
            catch (Exception ex)
            {
                excError = ex.Message;

            }
            finally
            {
                CloseDataBase();
            }
            Query = "";
            return iRowsAffected;

        }

        public void WriteLog(string message)
        {
            //string path = HttpContext.Current.Server.MapPath(@"~\Logs");

            string path = $"{Directory.GetCurrentDirectory()}{@"\Logs"}";
            

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = path + "\\Log.txt";

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

        public int ExecuteNonQueryWithDates(string date1, string date2, string xmlRequest, string xmlResponse)
        {
            int iRowsAffected = 0;
            try
            {
                if (OpenDataBase() == false)
                {
                    WriteLog("Error: Not Connected to database!");
                }
                else
                {
                    OracleCommand cmd = new OracleCommand();
                    cmd.Connection = Oracle_connection;
                    cmd.CommandText = Query;
                    cmd.Parameters.Add(new OracleParameter("requestDate", OracleDbType.Date)).Value = date1;
                    cmd.Parameters.Add(new OracleParameter("responseDate", OracleDbType.Date)).Value = date2;
                    cmd.Parameters.Add(new OracleParameter("xmlRequest", OracleDbType.Clob)).Value = xmlRequest;
                    cmd.Parameters.Add(new OracleParameter("xmlResponse", OracleDbType.Clob)).Value = xmlResponse;
                    Query = Query.Replace("\n", " ").Replace("\t", "").Replace("&", "& ");
                    cmd.CommandType = CommandType.Text;
                    iRowsAffected = cmd.ExecuteNonQuery();


                    return iRowsAffected;


                }
            }

            catch (OracleException ex)
            {
                excError = ex.Message;
                WriteLog("Oracle Exception: " + ex.Message + "\n\n");
            }
            catch (Exception ex)
            {
                excError = ex.Message;
                WriteLog("Exception: " + ex.Message + "\n\n");
            }
            finally
            {
                CloseDataBase();
            }
            Query = "";
            return iRowsAffected;

        }

        public int ExecuteNonQueryprms()
        {
            int iRowsAffected = 0;
            try
            {
                if (OpenDataBase() == false)
                {

                }
                else
                {

                    cmd.Connection = Oracle_connection;
                    cmd.CommandText = Query;
                    Query = Query.Replace("\n", "").Replace("\t", "").Replace("&", "& ");
                    cmd.CommandType = CommandType.Text;
                    iRowsAffected = cmd.ExecuteNonQuery();
                    return iRowsAffected;


                }
            }

            catch (OracleException ex)
            {
                excError = ex.Message;

            }
            catch (Exception ex)
            {
                excError = ex.Message;

            }
            finally
            {
                CloseDataBase();
            }
            Query = "";
            return iRowsAffected;

        }

        public string ExecuteScalerprms()
        {
            object objScaler = "";

            try
            {
                if (OpenDataBase() == false)
                {

                }
                else
                {

                    cmd.Connection = Oracle_connection;
                    cmd.CommandText = Query;
                    cmd.CommandType = CommandType.Text;
                    objScaler = cmd.ExecuteScalar();
                    return objScaler.ToString();
                }
            }

            catch (OracleException ex)
            {
                excError = ex.Message;

            }
            catch (Exception ex)
            {
                excError = ex.Message;

            }
            finally
            {
                CloseDataBase();
            }
            return "";
        }

        public string ExecuteScaler()
        {
            object objScaler = "";

            try
            {
                if (OpenDataBase() == false)
                {

                }
                else
                {
                    OracleCommand cmd = new OracleCommand();
                    cmd.Connection = Oracle_connection;
                    cmd.CommandText = Query;
                    cmd.CommandType = CommandType.Text;
                    objScaler = cmd.ExecuteScalar();
                    return objScaler.ToString();
                }
            }

            catch (OracleException ex)
            {
                excError = ex.Message;
                return "error";

            }
            catch (Exception ex)
            {
                excError = ex.Message;

            }
            finally
            {
                CloseDataBase();
            }
            return "";
        }
        public int ExecuteScalerInt()
        {
            object objScaler = "";

            try
            {
                if (OpenDataBase() == false)
                {

                }
                else
                {
                    OracleCommand cmd = new OracleCommand();
                    cmd.Connection = Oracle_connection;
                    cmd.CommandText = Query;
                    cmd.CommandType = CommandType.Text;
                    objScaler = cmd.ExecuteScalar();
                    return Convert.ToInt32(objScaler.ToString());
                }
            }

            catch (OracleException ex)
            {
                excError = ex.Message;

            }
            catch (Exception ex)
            {
                excError = ex.Message;

            }
            finally
            {
                CloseDataBase();
            }
            return -1;
        }


        public int ExecuteNonQueryBLOB(byte[] imageData, byte[] photoData)
        {
            int iRowsAffected = 0;

            try
            {
                if (OpenDataBase() == false)
                {

                }
                else
                {
                    OracleCommand cmd = new OracleCommand();
                    OracleParameter blobParameter = new OracleParameter();
                    OracleParameter blobPhoto = new OracleParameter();
                    blobParameter.OracleDbType = OracleDbType.Blob;
                    blobParameter.ParameterName = "1";
                    blobParameter.Value = imageData;

                    blobPhoto.OracleDbType = OracleDbType.Blob;
                    blobPhoto.ParameterName = "2";
                    blobPhoto.Value = photoData;

                    Query = Query.Replace("\n", "").Replace("\t", "").Replace("&", "& ");
                    cmd = new OracleCommand(Query, Oracle_connection);
                    cmd.Parameters.Add(blobParameter);
                    cmd.Parameters.Add(blobPhoto);
                    iRowsAffected = cmd.ExecuteNonQuery();

                    //OracleCommand cmd = new OracleCommand();
                    //cmd.Connection = Oracle_connection;
                    //cmd.CommandText = Query;
                    //cmd.CommandType = CommandType.StoredProcedure;
                    //OracleParameter c1 = new OracleParameter("1", OracleType.Blob); cmd.Parameters.Add(imageData); c1.Direction = ParameterDirection.Input;
                    //iRowsAffected = cmd.ExecuteScalar();

                    return iRowsAffected;

                }
            }

            catch (OracleException ex)
            {
                return -1;

            }
            catch (Exception ex)
            {
                return -2;

            }
            finally
            {
                CloseDataBase();
            }
            return -3;
        }

        public int ExecuteNonQueryBLOB(byte[] imageData)
        {
            int iRowsAffected = 0;

            try
            {
                if (OpenDataBase() == false)
                {

                }
                else
                {
                    OracleCommand cmd = new OracleCommand();
                    OracleParameter blobParameter = new OracleParameter();
                    blobParameter.OracleDbType = OracleDbType.Blob;
                    blobParameter.ParameterName = "1";
                    blobParameter.Value = imageData;
                    Query = Query.Replace("\n", "").Replace("\t", "").Replace("&", "& ");
                    cmd = new OracleCommand(Query, Oracle_connection);
                    cmd.Parameters.Add(blobParameter);
                    iRowsAffected = cmd.ExecuteNonQuery();

                    //OracleCommand cmd = new OracleCommand();
                    //cmd.Connection = Oracle_connection;
                    //cmd.CommandText = Query;
                    //cmd.CommandType = CommandType.StoredProcedure;
                    //OracleParameter c1 = new OracleParameter("1", OracleType.Blob); cmd.Parameters.Add(imageData); c1.Direction = ParameterDirection.Input;
                    //iRowsAffected = cmd.ExecuteScalar();

                    return iRowsAffected;

                }
            }

            catch (OracleException ex)
            {
                return -1;

            }
            catch (Exception ex)
            {
                return -2;

            }
            finally
            {
                CloseDataBase();
            }
            return -3;
        }


        public int ExecuteNonQueryBLOB(byte[] imageData, byte[] photoData, string CLOB)
        {
            int iRowsAffected = 0;

            try
            {
                if (OpenDataBase() == false)
                {

                }
                else
                {
                    OracleCommand cmd = new OracleCommand();
                    OracleParameter blobParameter = new OracleParameter();
                    OracleParameter blobPhoto = new OracleParameter();
                    OracleParameter CLOBParam = new OracleParameter();
                    blobParameter.OracleDbType = OracleDbType.Blob;
                    blobParameter.ParameterName = "1";
                    blobParameter.Value = imageData;

                    blobPhoto.OracleDbType = OracleDbType.Blob;
                    blobPhoto.ParameterName = "2";
                    blobPhoto.Value = photoData;

                    CLOBParam.OracleDbType = OracleDbType.Clob;
                    CLOBParam.ParameterName = "3";
                    CLOBParam.Value = CLOB;

                    Query = Query.Replace("\n", "").Replace("\t", "").Replace("&", "& ");
                    cmd = new OracleCommand(Query, Oracle_connection);
                    cmd.Parameters.Add(blobParameter);
                    cmd.Parameters.Add(blobPhoto);
                    cmd.Parameters.Add(CLOBParam);
                    iRowsAffected = cmd.ExecuteNonQuery();

                    //OracleCommand cmd = new OracleCommand();
                    //cmd.Connection = Oracle_connection;
                    //cmd.CommandText = Query;
                    //cmd.CommandType = CommandType.StoredProcedure;
                    //OracleParameter c1 = new OracleParameter("1", OracleType.Blob); cmd.Parameters.Add(imageData); c1.Direction = ParameterDirection.Input;
                    //iRowsAffected = cmd.ExecuteScalar();

                    return iRowsAffected;

                }
            }

            catch (OracleException ex)
            {
                return -1;

            }
            catch (Exception ex)
            {
                return -2;

            }
            finally
            {
                CloseDataBase();
            }
            return -3;
        }

        #region ---- Overloded Methods For 'CreateParameter' ----

        protected OracleParameter CreateParameter(string paramName, OracleDbType paramType, object paramValue)
        {
            OracleParameter param = new OracleParameter(paramName, (OracleDbType)paramType);
            param.Value = paramValue;
            return param;
        }
        protected OracleParameter CreateParameter(string paramName, OracleDbType paramType, ParameterDirection direction)
        {
            OracleParameter returnVal = CreateParameter(paramName, (OracleDbType)paramType, DBNull.Value);
            returnVal.Direction = direction;
            return returnVal;
        }

        protected OracleParameter CreateParameter(string paramName, OracleDbType paramType, int size, object paramValue)
        {
            OracleParameter param = new OracleParameter(paramName, (OracleDbType)paramType);
            param.Size = size;
            param.Value = paramValue;
            return param;
        }
        protected OracleParameter CreateParameter(string paramName, OracleDbType paramType, int size, ParameterDirection direction)
        {
            OracleParameter param = new OracleParameter(paramName, (OracleDbType)paramType);
            param.Size = size;
            param.Direction = (ParameterDirection)direction;
            return param;
        }


        #endregion

        public OracleTransaction BeginTransaction()
        {
            OpenDataBase();
            return Oracle_connection.BeginTransaction();
        }
    }


}