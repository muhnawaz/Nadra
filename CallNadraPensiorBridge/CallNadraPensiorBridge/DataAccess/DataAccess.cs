using System;
using System.Data.SqlClient;
using System.Data;
using System.Data.OracleClient;
using System.Configuration;
using System.Data.OleDb;
using System.Data.Odbc;
using System.Web;

namespace DataAccess
{
    public class DataAccess
    {
        public DataAccess()
        {
            m_ConnectionString = ConfigurationManager.ConnectionStrings["oracle_constr"].ToString();
        }
    #region ---- Variables ----

    public static string err = "";
    private string m_server;
    private string m_database;
    private string m_username;
    private string m_password;
    private string m_ConnectionString;
    public string ErrMessage = "<br>No Errors";
    private OracleConnection Oracle_connection = new OracleConnection();
    public OracleTransaction DbTxn;
    protected OracleDataReader reader;
    protected OracleCommand mobjSqlCommand;
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

    public string ExecuteDatabaseReader()
    {
        string word = null;
        mobjSqlCommand.CommandText = Query;
        OpenDatabaseConnection();
        reader = mobjSqlCommand.ExecuteReader();
        reader.Read();
        word = reader.GetString(0);
        CloseDatabaseConnection();
        return word;
    }
    #endregion

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
            err += ex.Message;
        }
        catch (Exception ex)
        {
            err += ex.Message;
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
            err += ex.Message;
            return false;
        }
        catch (Exception ex)
        {
            err += ex.Message;
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
                err += ex.Message;
                return false;
            }
            catch (Exception ex)
            {
                err += ex.Message;
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

        //public DataTable ExecuteDataTable()
        //{
        //    DataTable dt = new DataTable("DT");
        //    try
        //    {
        //        if (OpenDataBase() == false) 
        //        {

        //        }
        //        else
        //        {
        //            OracleDataAdapter DataAdaptor;
        //            DataAdaptor = new OracleDataAdapter(Query, Oracle_connection);
        //            DataAdaptor.Fill(dt);

        //            return dt;
        //        }
        //    }

        //    catch (OracleException ex)
        //    {
        //        err += ex.Message;
                
        //    }
        //    catch (Exception ex)
        //    {
        //        err += ex.Message;
                
        //    }
        //    finally
        //    {
        //        CloseDataBase();
        //    }
        //    return dt;
        //}

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
                    Query = Query.Replace("\n", "").Replace("\t", "").Replace("&", "& ");
                    cmd.CommandType = CommandType.Text;
                    iRowsAffected = cmd.ExecuteNonQuery();


                    return iRowsAffected;


                }
            }

            catch (OracleException ex)
            {
                err += ex.Message;

            }
            catch (Exception ex)
            {
                err += ex.Message;

            }
            finally
            {
                CloseDataBase();
            }
            Query = "";
            return iRowsAffected;

        }
        //public int ExecuteProc()
        //{
        //    int iRowsAffected = 0;
        //    try
        //    {
        //        if (OpenDataBase() == false)
        //        {

        //        }
        //        else
        //        {
        //            OracleCommand cmd = new OracleCommand();
        //            cmd.Connection = Oracle_connection;
        //            cmd.CommandText = Query;
        //            Query = Query.Replace("\n", "").Replace("\t", "").Replace("&", "& ");
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            cmd.ExecuteNonQuery();
        //            return iRowsAffected;
        //        }
        //    }

        //    catch (OracleException ex)
        //    {
        //        err += ex.Message;

        //    }
        //    catch (Exception ex)
        //    {
        //        err += ex.Message;

        //    }
        //    finally
        //    {
        //        CloseDataBase();
        //    }
        //    Query = "";
        //    return iRowsAffected;

        //}
        //public string ExecuteProc(String clobdata1, String clobdata2, String clobdata3, String clobdata4, String clobdata5, String clobdata6, String clobdata7, String clobdata8, String clobdata9, String clobdata10, String clobdata11, String clobdata12, String clobdata13, String clobdata14, String clobdata15, String clobdata16, String clobdata17, String clobdata18, String clobdata19, String clobdata20, String clobdata21, String clobdata22, String clobdata23, String masterid)
        //{
        //    object iRowsAffected = "";
        //    try
        //    {
        //        if (OpenDataBase() == false)
        //        {

        //        }
        //        else
        //        {
        //            OracleCommand cmd = new OracleCommand();
        //            cmd.Connection = Oracle_connection;
        //            cmd.CommandText = Query;
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            OracleParameter c1 = new OracleParameter("c1", OracleType.Clob); cmd.Parameters.Add(clobdata1); c1.Direction = ParameterDirection.Input;
        //            OracleParameter c2 = new OracleParameter("c2", OracleType.Clob); cmd.Parameters.Add(clobdata2); c2.Direction = ParameterDirection.Input;
        //            OracleParameter c3 = new OracleParameter("c3", OracleType.Clob); cmd.Parameters.Add(clobdata3); c3.Direction = ParameterDirection.Input;
        //            OracleParameter c4 = new OracleParameter("c4", OracleType.Clob); cmd.Parameters.Add(clobdata4); c4.Direction = ParameterDirection.Input;
        //            OracleParameter c5 = new OracleParameter("c5", OracleType.Clob); cmd.Parameters.Add(clobdata5); c5.Direction = ParameterDirection.Input;
        //            OracleParameter c6 = new OracleParameter("c6", OracleType.Clob); cmd.Parameters.Add(clobdata6); c6.Direction = ParameterDirection.Input;
        //            OracleParameter c7 = new OracleParameter("c7", OracleType.Clob); cmd.Parameters.Add(clobdata7); c7.Direction = ParameterDirection.Input;
        //            OracleParameter c8 = new OracleParameter("c8", OracleType.Clob); cmd.Parameters.Add(clobdata8); c8.Direction = ParameterDirection.Input;
        //            OracleParameter c9 = new OracleParameter("c9", OracleType.Clob); cmd.Parameters.Add(clobdata9); c9.Direction = ParameterDirection.Input;
        //            OracleParameter c10 = new OracleParameter("c10", OracleType.Clob); cmd.Parameters.Add(clobdata10); c10.Direction = ParameterDirection.Input;
        //            OracleParameter c11 = new OracleParameter("c11", OracleType.Clob); cmd.Parameters.Add(clobdata11); c11.Direction = ParameterDirection.Input;
        //            OracleParameter c12 = new OracleParameter("c12", OracleType.Clob); cmd.Parameters.Add(clobdata12); c12.Direction = ParameterDirection.Input;
        //            OracleParameter c13 = new OracleParameter("c13", OracleType.Clob); cmd.Parameters.Add(clobdata13); c13.Direction = ParameterDirection.Input;
        //            OracleParameter c14 = new OracleParameter("c14", OracleType.Clob); cmd.Parameters.Add(clobdata14); c14.Direction = ParameterDirection.Input;
        //            OracleParameter c15 = new OracleParameter("c15", OracleType.Clob); cmd.Parameters.Add(clobdata15); c15.Direction = ParameterDirection.Input;
        //            OracleParameter c16 = new OracleParameter("c16", OracleType.Clob); cmd.Parameters.Add(clobdata16); c16.Direction = ParameterDirection.Input;
        //            OracleParameter c17 = new OracleParameter("c17", OracleType.Clob); cmd.Parameters.Add(clobdata17); c17.Direction = ParameterDirection.Input;
        //            OracleParameter c18 = new OracleParameter("c18", OracleType.Clob); cmd.Parameters.Add(clobdata18); c18.Direction = ParameterDirection.Input;
        //            OracleParameter c19 = new OracleParameter("c19", OracleType.Clob); cmd.Parameters.Add(clobdata19); c19.Direction = ParameterDirection.Input;
        //            OracleParameter c20 = new OracleParameter("c20", OracleType.Clob); cmd.Parameters.Add(clobdata20); c20.Direction = ParameterDirection.Input;
        //            OracleParameter c21 = new OracleParameter("c21", OracleType.Clob); cmd.Parameters.Add(clobdata21); c21.Direction = ParameterDirection.Input;
        //            OracleParameter c22 = new OracleParameter("c22", OracleType.Clob); cmd.Parameters.Add(clobdata22); c22.Direction = ParameterDirection.Input;
        //            OracleParameter c23 = new OracleParameter("c23", OracleType.Clob); cmd.Parameters.Add(clobdata23); c23.Direction = ParameterDirection.Input;
        //            OracleParameter master = new OracleParameter("id", OracleType.Number); cmd.Parameters.Add(masterid); master.Direction = ParameterDirection.Input;
        //            iRowsAffected = cmd.ExecuteScalar();
                    
        //            return iRowsAffected.ToString();
        //        }
        //    }

        //    catch (OracleException ex)
        //    {
        //        err += ex.Message;

        //    }
        //    catch (Exception ex)
        //    {
        //        err += ex.Message;

        //    }
        //    finally
        //    {
        //        CloseDataBase();
        //    }
        //    Query = "";
        //    return iRowsAffected.ToString();

        //}
        //public int ExecuteProc(string sdate,string edate)
        //{
        //    int iRowsAffected = 0;
        //    try
        //    {
        //        if (OpenDataBase() == false)
        //        {

        //        }
        //        else
        //        {
        //            OracleCommand cmd = new OracleCommand();
        //            cmd.Connection = Oracle_connection;
        //            cmd.CommandText = Query;
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            OracleParameter pstdate = new OracleParameter("PSTDATE", OracleType.DateTime);
        //            pstdate.Value = sdate;
        //            cmd.Parameters.Add(pstdate);

        //            OracleParameter pendate = new OracleParameter("PENDATE", OracleType.DateTime);
        //            pendate.Value = edate;
        //            cmd.Parameters.Add(pendate);

        //            cmd.ExecuteNonQuery();
        //            return iRowsAffected;
        //        }
        //    }

        //    catch (OracleException ex)
        //    {
        //        err += ex.Message;

        //    }
        //    catch (Exception ex)
        //    {
        //        err += ex.Message;

        //    }
        //    finally
        //    {
        //        CloseDataBase();
        //    }
        //    Query = "";
        //    return iRowsAffected;

        //}

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
                    return Convert.ToString(objScaler);
                }
            }

            catch (OracleException ex)
            {
                return "";

            }
            catch (Exception ex)
            {
                return "";

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
                err += ex.Message;

            }
            catch (Exception ex)
            {
                err += ex.Message;

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
                    blobParameter.OracleType = OracleType.Blob;
                    blobParameter.ParameterName = "1";
                    blobParameter.Value = imageData;

                    blobPhoto.OracleType = OracleType.Blob;
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
                    blobParameter.OracleType = OracleType.Blob;
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
        protected Boolean GetDataSet(ref DataSet dataset, string sqlString)
        {
            try
            {
                if (OpenDataBase() == false) { return false; }

                OracleDataAdapter DataAdaptor;
                DataAdaptor = new OracleDataAdapter(sqlString, Oracle_connection);
                DataTable dt = new DataTable("DT");
                DataAdaptor.Fill(dt);
                dataset.Tables.Add();
                dataset.Tables[0].Merge(dt);
                //DataAdaptor.Fill(dataset);

                return true;
            }

            catch (OracleException ex)
            {
                err += ex.Message;
                return false;
            }
            catch (Exception ex)
            {
                err += ex.Message;
                return false;
            }
            finally
            {
                CloseDataBase();
            }
        }
        protected Boolean ExecuteByRef(string SPName, ref IDataParameter[] SPParam)
        {
            try
            {
                if (OpenDataBase() == false) { return false; }

                OracleCommand cmd = new OracleCommand();
                cmd.Connection = Oracle_connection;
                cmd.CommandText = SPName;
                cmd.CommandType = CommandType.StoredProcedure;

                for (int i = 0; i < SPParam.Length; i++)
                {
                    cmd.Parameters.Add(SPParam[i]);
                }

                cmd.ExecuteNonQuery();
                SPParam[0].Value = cmd.Parameters[SPParam[0].ParameterName].Value;

                return true;
            }
            catch (OracleException ex)
            {
                err += ex.Message;
                return false;
            }
            catch (Exception ex)
            {
                err += ex.Message;
                return false;
            }
            finally
            {
                CloseDataBase();
            }
        }

        #region ---- Overloded Methods For 'CreateParameter' ----

        protected OracleParameter CreateParameter(string paramName, System.Data.OracleClient.OracleType paramType, object paramValue)
        {
            OracleParameter param = new OracleParameter(paramName, (System.Data.OracleClient.OracleType)paramType);
            param.Value = paramValue;
            return param;
        }
        protected OracleParameter CreateParameter(string paramName, System.Data.OracleClient.OracleType paramType, ParameterDirection direction)
        {
            OracleParameter returnVal = CreateParameter(paramName, (System.Data.OracleClient.OracleType)paramType, DBNull.Value);
            returnVal.Direction = direction;
            return returnVal;
        }

        protected OracleParameter CreateParameter(string paramName, System.Data.OracleClient.OracleType paramType, int size, object paramValue)
        {
            OracleParameter param = new OracleParameter(paramName, (System.Data.OracleClient.OracleType)paramType);
            param.Size = size;
            param.Value = paramValue;
            return param;
        }
        protected OracleParameter CreateParameter(string paramName, System.Data.OracleClient.OracleType paramType, int size, ParameterDirection direction)
        {
            OracleParameter param = new OracleParameter(paramName, (System.Data.OracleClient.OracleType)paramType);
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