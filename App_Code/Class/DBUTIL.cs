//*********************************************************
//  Copyright (c), Thaiintersol co,ltd. All rights reserved.
//**********************************************************



using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

#region ".NET Framework Class Import"
using Oracle.ManagedDataAccess;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Configuration;
#endregion

public class DBUTIL
{

    #region "Internal member variables"
    private string _ConnectStr;
    private string _DB_Provider;
    private string _DB_Name;
    private DbTypes _DB_Type;

    private ArrayList _Params = new ArrayList();
    #endregion

    #region "Declarations"

    public const int opINSERT = 1;
    public const int opUPDATE = 2;
    public const int opDELETE = 3;


    public enum FieldTypes
    {
        ftNumeric = 1,
        ftText = 2,
        ftDate = 3,
        ftDateTime = 6,
        ftBinary = 7
    }

    public enum DbTypes
    {
        dbtOracleOleDb = 1,
        dbtOracleDataAccess = 2,
        dbtOracleClient = 3,
        dbtSqlOleDb = 4,
        dbtSqlClient = 5,
        dbtODBC = 6
    }

    #endregion

    #region "Properties"

    public string ConnectStr
    {
        get { return _ConnectStr; }
        set { _ConnectStr = value; }
    }

    public string DB_Provider
    {
        get { return _DB_Provider; }
        set { _DB_Provider = value; }
    }

    public DbTypes DB_Type
    {
        get { return _DB_Type; }
        set { _DB_Type = value; }          
    }

    #endregion

    #region "Generic DB Helpers"


    public DbTypes GetDbTypes(string _ConnectStr = "")
    {
        DbTypes result = default(DbTypes);
        string DB_Name = _DB_Name;
        string DB_Provider = _DB_Provider;

        if (!string.IsNullOrEmpty(_ConnectStr))
        {
            DB_Provider = GetConnectParam(_ConnectStr, "Provider");
            DB_Name = GetConnectParam(_ConnectStr, "Initial Catalog");
        }

        if (_ConnectStr.IndexOf("DSN") >= 0)
        {
            result = DbTypes.dbtODBC;
        }
        else if (string.IsNullOrEmpty(DB_Provider))
        {
            if (string.IsNullOrEmpty(DB_Name))
            {
                result = DbTypes.dbtOracleDataAccess;
            }
            else
            {
                result = DbTypes.dbtSqlClient;
            }
        }
        else
        {
            //-- aor edit 31/05/2017 --
            if (DB_Provider == "SQLOLEDB")
            {
                result = DbTypes.dbtSqlOleDb;
            }
            else 
                if (string.IsNullOrEmpty(DB_Name))
                {
                    result = DbTypes.dbtOracleOleDb;
                }
                else
                {
                    result = DbTypes.dbtSqlOleDb;
                }
        }

        return result;
    }

    public string GetConnectParam(string _ConnectStr, string ParamName)
    {
        int I = 0;
        int J = 0;
        int K = 0;
        string Value = "";

        I = _ConnectStr.ToLower().IndexOf(ParamName.ToLower());
        if (I >= 0)
        {
            J = _ConnectStr.IndexOf("=", I + 1);
            if (J >= 0)
            {
                K = _ConnectStr.IndexOf(";", J + 1);
                if (K >= 0)
                {
                    Value = _ConnectStr.Substring(J + 1, K - J - 1);
                }
                else
                {
                    Value = _ConnectStr.Substring(J + 1);
                }
            }
        }

        return Value;
    }

    public void SetOracleSessionInfo(ref Oracle.ManagedDataAccess.Client.OracleConnection Conn)
    {
        if ((Conn != null) && Conn.State == ConnectionState.Open)
        {
            Oracle.ManagedDataAccess.Client.OracleGlobalization Glob = default(Oracle.ManagedDataAccess.Client.OracleGlobalization);
            Glob = Conn.GetSessionInfo();
            Glob.Language = "THAI";
            Glob.Territory = "THAILAND";
            Conn.SetSessionInfo(Glob);
        }
    }

    public dynamic CreateConnection(string _ConnectStr = "")
    {
        DbTypes DBType = default(DbTypes);

        if (string.IsNullOrEmpty(_ConnectStr))
        {
            _ConnectStr = ConnectStr;
            DBType = _DB_Type;
        }
        else
        {
            DBType = GetDbTypes(_ConnectStr);
        }

        switch (DBType)
        {
            case DbTypes.dbtODBC:
                return new System.Data.Odbc.OdbcConnection(_ConnectStr);
            case DbTypes.dbtOracleDataAccess:
                return new Oracle.ManagedDataAccess.Client.OracleConnection(_ConnectStr);
            case DbTypes.dbtSqlClient:
                return new System.Data.SqlClient.SqlConnection(_ConnectStr);
            default:
                return new System.Data.OleDb.OleDbConnection(_ConnectStr);
        }
    }

    public dynamic CreateCommand(string SQL = "", dynamic _Conn = null, dynamic _Trans = null)
    {
        dynamic cmd = null;
        DbTypes DBType = new DbTypes();

        try
        {
            if (_Conn != null)
            {
                DBType = GetDbTypes(_Conn.ConnectionString);
                switch (DBType)
                {
                    case DbTypes.dbtODBC:
                        cmd = new System.Data.Odbc.OdbcCommand(SQL, (OdbcConnection)_Conn);
                        break;
                    case DbTypes.dbtOracleDataAccess:
                        cmd = new Oracle.ManagedDataAccess.Client.OracleCommand(SQL, (Oracle.ManagedDataAccess.Client.OracleConnection)_Conn);
                        break;
                    case DbTypes.dbtSqlClient:
                        cmd = new System.Data.SqlClient.SqlCommand(SQL, (System.Data.SqlClient.SqlConnection)_Conn);
                        break;
                    default:
                        cmd = new System.Data.OleDb.OleDbCommand(SQL, (System.Data.OleDb.OleDbConnection)_Conn);
                        break;
                }
                cmd.Transaction = _Trans;
            }
            else
            {
                DBType = _DB_Type;
                switch (DBType)
                {
                    case DbTypes.dbtOracleDataAccess:
                        cmd = new Oracle.ManagedDataAccess.Client.OracleCommand(SQL);
                        break;
                    case DbTypes.dbtSqlClient:
                        cmd = new System.Data.SqlClient.SqlCommand(SQL);
                        break;
                    default:
                        cmd = new System.Data.OleDb.OleDbCommand(SQL);
                        break;
                }
                cmd.Connection = OpenConn(_ConnectStr);
            }

            return cmd;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public void ClearCommand(ref dynamic cmd, dynamic _Conn = null)
    {
        if ((cmd != null))
        {
            if (_Conn == null)
            {
                dynamic Conn = cmd.Connection;
                CloseConn(ref Conn);
                cmd.Connection = Conn;
            }
            cmd.Dispose();
            cmd = null;
        }
    }

    public dynamic CreateDataAdapter(string SQL = "", dynamic _Conn = null, dynamic _Trans = null)
    {
        dynamic DA = null;
        DbTypes DBType = default(DbTypes);

        try
        {
            if ((_Conn != null))
            {
                DBType = GetDbTypes(_Conn.ConnectionString);
                switch (DBType)
                {
                    case DbTypes.dbtODBC:
                        DA = new System.Data.Odbc.OdbcDataAdapter(SQL, (OdbcConnection)_Conn);
                        if ((_Trans != null))
                        {
                            DA.SelectCommand.Transaction = _Trans;
                        }
                        break;
                    case DbTypes.dbtOracleDataAccess:
                        

                        //-- 03/09/2018 -- ระบบนี้ connect oracle ขาดเป็นช่วงๆ
                        try
                        {
                            DA = new Oracle.ManagedDataAccess.Client.OracleDataAdapter(SQL, (Oracle.ManagedDataAccess.Client.OracleConnection)_Conn);
                            if ((_Trans != null))
                            {
                                DA.SelectCommand.Transaction = (Oracle.ManagedDataAccess.Client.OracleTransaction)_Trans;

                            }
                        }
                        catch (Exception ex)
                        {
                            //-- delay of 2 seconds
                            int seconds = 2;
                            System.Threading.Thread.Sleep(seconds * 1000);
                            DA = new Oracle.ManagedDataAccess.Client.OracleDataAdapter(SQL, (Oracle.ManagedDataAccess.Client.OracleConnection)_Conn);
                            if ((_Trans != null))
                            {
                                DA.SelectCommand.Transaction = (Oracle.ManagedDataAccess.Client.OracleTransaction)_Trans;

                            }
                        }



                        break;
                    case DbTypes.dbtSqlClient:
                        DA = new System.Data.SqlClient.SqlDataAdapter(SQL, (System.Data.SqlClient.SqlConnection)_Conn);
                        if ((_Trans != null))
                        {
                            DA.SelectCommand.Transaction = (System.Data.SqlClient.SqlTransaction)_Trans;
                        }
                        break;
                    default:
                        DA = new System.Data.OleDb.OleDbDataAdapter(SQL, (System.Data.OleDb.OleDbConnection)_Conn);
                        if ((_Trans != null))
                        {
                            DA.SelectCommand.Transaction = (System.Data.OleDb.OleDbTransaction)_Trans;
                        }
                        break;
                }
                //DA.Transaction = _Trans
            }
            else
            {
                DBType = _DB_Type;
                switch (DBType)
                {
                    case DbTypes.dbtODBC:
                        DA = new System.Data.Odbc.OdbcDataAdapter(SQL, ConnectStr);
                        break;
                    case DbTypes.dbtOracleDataAccess:
                        

                        //-- 03/09/2018 -- ระบบนี้ connect oracle ขาดเป็นช่วงๆ
                        try
                        {
                            DA = new Oracle.ManagedDataAccess.Client.OracleDataAdapter(SQL, ConnectStr);
                        }
                        catch (Exception ex)
                        {
                            //-- delay of 2 seconds
                            int seconds = 2;
                            System.Threading.Thread.Sleep(seconds * 1000);
                            DA = new Oracle.ManagedDataAccess.Client.OracleDataAdapter(SQL, ConnectStr);
                        }


                        break;
                    case DbTypes.dbtSqlClient:
                        DA = new System.Data.SqlClient.SqlDataAdapter(SQL, ConnectStr);
                        break;
                    default:
                        DA = new System.Data.OleDb.OleDbDataAdapter(SQL, ConnectStr);
                        break;
                }
            }

            return DA;
        }
        catch (Exception ex)
        {
            //Return Nothing
            throw ex;
        }
    }

    public void ClearDataAdapter(ref dynamic DA, dynamic _Conn = null)
    {
        if ((DA != null))
        {
            if ((_Conn == null) && (DA.SelectCommand != null))
            {
                dynamic Conn = DA.SelectCommand.Connection;
                CloseConn(ref Conn);
                DA.SelectCommand.Connection = Conn;
            }
            DA.Dispose();
            DA = null;
        }
    }

    public dynamic CreateParameter(dynamic _Conn = null)
    {
        DbTypes DBType = default(DbTypes);
        dynamic param = null;

        if ((_Conn != null))
        {
            DBType = GetDbTypes(_Conn.ConnectionString);
        }
        else
        {
            DBType = _DB_Type;
        }

        switch (DBType)
        {
            case DbTypes.dbtOracleDataAccess:
                param = new Oracle.ManagedDataAccess.Client.OracleParameter();
                break;
            case DbTypes.dbtSqlClient:
                param = new System.Data.SqlClient.SqlParameter();
                break;
            default:
                param = new System.Data.OleDb.OleDbParameter();
                break;
        }

        return param;
    }

    #endregion


    public dynamic OpenConn(string ConnectStr)
    {
        if (!string.IsNullOrEmpty(ConnectStr))
            _ConnectStr = ConnectStr;
        return OpenDBConn();
    }

    public dynamic OpenConn(string DB_Provider, string DB_DataSource, string DB_UserName, string DB_Password, string DB_Name)
    {
        _DB_Provider = DB_Provider;
        _DB_Name = DB_Name; //-- aor edit 31/05/2017 --
        if (DB_Provider.ToUpper() == "MSDASQL")
        {
            _ConnectStr = "DSN=" + DB_DataSource + ";UID=" + DB_UserName + ";PWD=" + DB_Password;
            if (!string.IsNullOrEmpty(DB_Name))
                _ConnectStr += ";Initial Catalog=" + DB_Name;
        }
        else
        {
            // 29-04-2017 : Add support to SQL Client
            if (!string.IsNullOrEmpty(DB_Provider))
                _ConnectStr = "Provider=" + DB_Provider + ";Data Source=" + DB_DataSource + ";User ID=" + DB_UserName + ";Password=" + DB_Password;
            else if (!string.IsNullOrEmpty(DB_Name))
            {
                _ConnectStr += "Initial Catalog=" + DB_Name + ";server=" + DB_DataSource + ";User ID=" + DB_UserName + ";Password=" + DB_Password;
            }
            else {
                _ConnectStr = "Data Source=" + DB_DataSource + ";User ID=" + DB_UserName + ";Password=" + DB_Password;
            }
        }
        return OpenDBConn();
    }

    private dynamic OpenDBConn()
    {
        dynamic Conn = null;
        int I = 0;
        DbTypes DBType = default(DbTypes);

        if (string.IsNullOrEmpty(_ConnectStr))
        {
            _ConnectStr = ConnectStr;
            DBType = _DB_Type;
        }
        else
        {
            DBType = GetDbTypes(_ConnectStr);
        }

        try
        {
            Conn = CreateConnection(_ConnectStr);
            Conn.Open();

            if (DBType == DbTypes.dbtOracleDataAccess)
            {
                Oracle.ManagedDataAccess.Client.OracleConnection OConn = (Oracle.ManagedDataAccess.Client.OracleConnection)Conn;
                SetOracleSessionInfo(ref OConn);
                Conn = OConn;
            }

            return Conn;
        }
        catch (Exception ex)
        {
            CloseConn(ref Conn);

            throw ex;
        }
    }

    //========================================
    // Close Database Connection
    public void CloseConn(ref dynamic Conn)
    {
        if ((Conn != null))
        {
            try
            {
                Conn.Close();
                Conn.Dispose();
            }
            catch (Exception ex)
            {
            }
            Conn = null;
        }
    }

    //========================================
    // Begin Transaction
    public dynamic BeginTrans(ref dynamic Conn)
    {
        dynamic result = null;
        if ((Conn != null))
        {
            result = Conn.BeginTransaction();
        }
        else
        {
            throw new Exception("Connection has not been initialized!");
        }
        return result;
    }

    //========================================
    // Commit Transaction
    public void CommitTrans(ref dynamic Trans)
    {
        if ((Trans != null))
        {
            Trans.Commit();
            Trans = null;
        }
    }

    //========================================
    // Rollback Transaction
    public void RollbackTrans(ref dynamic Trans)
    {
        try
        {
            if ((Trans != null))
            {
                Trans.Rollback();
            }
        }
        catch
        {
        }
        Trans = null;
    }

    //========================================
    // Open DataSet
    public DataRow OpenDS(ref DataSet DS, string SQL, dynamic Conn = null, dynamic Trans = null)
    {
        dynamic DA = null;
        try
        {
            if (DS == null)
            {
                DS = new DataSet();
            }
            else
            {
                DS.Clear();
            }

            DA = CreateDataAdapter(SQL, Conn, Trans);
            DA.Fill(DS);
            if ((Conn == null) && (DA.SelectCommand.Connection != null))
            {
                dynamic oConn = DA.SelectCommand.Connection;
                CloseConn(ref oConn);
                DA.SelectCommand.Connection = oConn;
            }
            DA.Dispose();
            DA = null;

            if ((DS.Tables.Count > 0) && (DS.Tables[0].Rows.Count > 0))
            {
                return DS.Tables[0].Rows[0];
            }
            else
            {
                return null;
            }

        }
        catch (Exception ex)
        {
            if ((DA != null))
            {
                if ((Conn == null) && (DA.SelectCommand.Connection != null))
                {
                    dynamic oConn2 = DA.SelectCommand.Connection;
                    CloseConn(ref oConn2);
                    DA.SelectCommand.Connection = oConn2;
                }
                DA.Dispose();
                DA = null;
            }

            throw (ex);
        }
    }


    //========================================
    // Close DataSet
    public void CloseDS(ref DataSet DS)
    {
        try
        {
            if ((DS != null))
                DS.Dispose();
        }
        catch
        {
        }
        DS = null;
    }

    //========================================
    // Open data table
    public DataRow OpenDT(ref DataTable DT, string SQL, dynamic _Conn = null, dynamic _Trans = null)
    {
        dynamic DA = null;

        try
        {
            if (DT == null)
            {
                DT = new DataTable();
            }
            else
            {
                DT.Clear();
            }

            DA = CreateDataAdapter(SQL, _Conn, _Trans);
          
            //-- 03/09/2018 -- ระบบนี้ connect oracle ขาดเป็นช่วงๆ
            try
            {
                DA.Fill(DT);
            }
            catch
            {
                //-- delay of 2 seconds
                int seconds = 2;
                System.Threading.Thread.Sleep(seconds * 1000);
                DA.Fill(DT);
            }


            if ((DT.Rows.Count > 0))
            {
                return DT.Rows[0];
            }
            else
            {
                return null;
            }

        }
        catch (Exception ex)
        {
            throw (ex);
        }
        finally
        {
            ClearDataAdapter(ref DA, _Conn);
        }
    }

    //========================================
    // Close data table
    public void CloseDT(ref DataTable DT)
    {
        try
        {
            if ((DT != null))
                DT.Dispose();
        }
        catch
        {
        }
        DT = null;
    }

    //========================================
    // Open Data Recordset
    public void OpenRs(ref dynamic Rs, string sql, dynamic _Conn = null, dynamic _Trans = null)
    {
        dynamic cmd = null;

        try
        {
            cmd = CreateCommand(sql, _Conn, _Trans);
            Rs = cmd.ExecuteReader;

        }
        catch (Exception ex)
        {
            throw (ex);
        }
        finally
        {
            //ClearCommand(cmd, _Conn)
            cmd.Dispose();
            cmd = null;
        }
    }

    public DataTable QueryData(string SQL, dynamic _Conn = null, dynamic _Trans = null)
    {

        DataTable DT = new DataTable();

        try
        {
            if ((_Conn == null))
            {
                OpenDT(ref DT, SQL, _Conn, _Trans);
            }
            else
            {
                OpenDT(ref DT, SQL);
            }

            return DT;
        }
        catch (Exception ex)
        {
            throw (ex);
        }
        finally
        {
            DT = null;
        }
    }

    //========================================
    // Execute an SQL Command
    //========================================
    public int ExecuteSQL(string SQL, dynamic _Conn = null, dynamic _Trans = null)
    {
        dynamic cmd = null;
        int rows = 0;

        try
        {
            cmd = CreateCommand(SQL, _Conn, _Trans);
          

            //-- 03/09/2018 -- ระบบนี้ connect oracle ขาดเป็นช่วงๆ
            try
            {
                rows = cmd.ExecuteNonQuery();
            }
            catch
            {
                //-- delay of 2 seconds
                int seconds = 2;
                System.Threading.Thread.Sleep(seconds * 1000);
                rows = cmd.ExecuteNonQuery();
            }

            return rows;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            ClearCommand(ref cmd, _Conn);
        }
    }

 
    public object LookupSQL(string SQL, dynamic Conn = null, dynamic Trans = null)
    {
        dynamic tmpConn = Conn;
        dynamic cmd = null;
        object Value = "";
        try
        {
            if ((Conn == null))
                tmpConn = OpenConn(_ConnectStr);
            cmd = CreateCommand(SQL, tmpConn);
            if ((Trans != null))
            {
                cmd.Transaction = Trans;
            }
            Value = cmd.ExecuteScalar();
            cmd.Dispose();
            cmd = null;
            if ((Conn == null))
                CloseConn(ref tmpConn);

            return Value;
        }
        catch (Exception ex)
        {
            if ((Conn == null))
                CloseConn(ref tmpConn);
            cmd.Dispose();
            cmd = null;
            throw ex;
        }
    }

    //========================================
    // Lookup data in a table
    public dynamic LookupData(string usrTable, string FieldName, string usrCriteria, dynamic Conn = null, dynamic Trans = null)
    {
        string SQL = null;

        if (!string.IsNullOrEmpty(usrCriteria))
        {
            SQL = " SELECT " + FieldName + "  FROM " + usrTable + " WHERE  " + usrCriteria;
        }
        else
        {
            SQL = " SELECT " + FieldName + "  FROM " + usrTable;
        }
        return LookupSQL(SQL);
    }

    public string SQLDate(System.DateTime D)
    {
        string result = null;
        int Y = 0;
        if ((D != null) && (Convert.ToDouble(D.ToOADate()) > 0))
        {
            switch (_DB_Type)
            {
                case DbTypes.dbtOracleDataAccess:
                case DbTypes.dbtOracleOleDb:
                    Y = D.Year;
                    if (Y > 2500)
                        Y = Y - 543;
                    result = "TO_DATE('" + Y + "/" + D.Month.ToString().PadLeft(2,'0') + "/" + D.Day.ToString().PadLeft(2, '0') + "','YYYY/MM/DD')";
                    break;
                default:
                    result = "convert(datetime," + (Convert.ToDouble(D.ToOADate()) - 2) + ")";
                    break;
            }
        }
        else
        {
            result = "NULL";
        }
        return result;
    }

    //========================================
    // Format DateTime to Oracle SQL DateTime
    public string SQLDateTime(System.DateTime DT)
    {
        string result = null;
        int Y = 0;

        if ((DT != null) && (Convert.ToDouble(DT.ToOADate()) > 0))
        {
            switch (_DB_Type)
            {
                case DbTypes.dbtOracleDataAccess:
                case DbTypes.dbtOracleOleDb:
                    Y = DT.Year;
                    if (Y > 2500)
                        Y = Y - 543;
                    result = "TO_DATE('" + Y + "." + DT.Month + "." + DT.Day + " " + DT.Hour + ":" + DT.Minute + ":" + DT.Second + "','YYYY.MM.DD HH24:MI:SS')";
                    break;
                default:
                    Y = DT.Year;
                    if (Y > 2500)
                        Y -= 543;
                    result = "'" + Y + DT.ToString("-MM-dd HH:mm:ss") + "'";

                    break;
            }
        }
        else
        {
            result = "NULL";
        }
        return result;
    }

    //========================================
    // Append a SQL criteria
    public void AddCriteria(ref string CriteriaSQL, string FieldName, object FieldValue, FieldTypes FieldType, bool AllowIN = false)
    {
        string Oper = "=";
        string FVal = null;
        string[] ValList = null;
        string V = null;

        FVal = (FieldValue + "").Replace("*", "%");

        FVal = Convert.ToString(FieldValue) + "";
        if (FVal.ToUpper().StartsWith("IN ("))
        {
            Oper = " IN ";
            ValList = FVal.Trim().Substring(4, FVal.Length - 5).Split(',');
            FVal = "";
            foreach (string VI in ValList)
            {
                V = VI;
                switch (FieldType)
                {
                    case FieldTypes.ftNumeric:
                        if (Utility.IsNumeric(V))
                        {
                            V = Convert.ToString(V.Replace(",", ""));
                        }
                        break;
                    case FieldTypes.ftText:
                        if (V != "NULL")
                        {
                            V = "'" + V.Replace("'", "''") + "'";
                        }
                        break;
                    case FieldTypes.ftDate:
                        if (Utility.IsDate(V))
                        {
                            V = SQLDate(Convert.ToDateTime(V));
                        }
                        break;
                    case FieldTypes.ftDateTime:
                        if (Utility.IsDate(V))
                        {
                            V = SQLDateTime(Convert.ToDateTime(V));
                        }
                        break;
                }
                if (!string.IsNullOrEmpty(V))
                    FVal += "," + V;
            }
            if (!string.IsNullOrEmpty(FVal))
            {
                FVal = "(" + FVal.Substring(1) + ")";
            }
        }
        else if (!string.IsNullOrEmpty(FVal))
        {
            if ((FVal.IndexOf("%") >= 0))
            {
                Oper = " LIKE ";
                FieldType = FieldTypes.ftText;
            }
            if (FVal.StartsWith("<"))
            {
                if (FVal.Substring(1, 1) == ">")
                {
                    Oper = "<>";
                    FieldValue = FVal.Substring(2);
                }
                else if (FVal.Substring(1, 1) == "=")
                {
                    Oper = "<=";
                    FieldValue = FVal.Substring(2);
                }
                else
                {
                    Oper = "<";
                    FieldValue = FVal.Substring(1);
                }
            }
            else if (FVal.StartsWith(">"))
            {
                if (FVal.Substring(1, 1) == "=")
                {
                    Oper = ">=";
                    FieldValue = FVal.Substring(2);
                }
                else
                {
                    Oper = ">";
                    FieldValue = FVal.Substring(1);
                }
            }
            else if (FVal.StartsWith("="))
            {
                Oper = "=";
                FieldValue = FVal.Substring(1);
            }

            switch (FieldType)
            {
                case FieldTypes.ftNumeric:
                    if (Utility.IsNumeric(FieldValue))
                    {
                        FVal = Convert.ToString(FieldValue).Replace(",", "");
                    }
                    break;
                case FieldTypes.ftText:
                    if (AllowIN && FVal.IndexOf(",") >= 0)
                    {
                        Oper = " IN ";
                        FVal = "('" + FieldValue.ToString().Replace(",", "', '") + "')";
                    }
                    else
                    {
                        FVal = "'" + FieldValue.ToString().Replace("'", "''") + "'";
                    }
                    break;
                case FieldTypes.ftDate:
                    if (Utility.IsDate(FieldValue) && (Convert.ToDouble(Convert.ToDateTime(FieldValue).ToOADate()) > 0))
                    {
                        FVal = SQLDate(Convert.ToDateTime(FieldValue));
                    }
                    break;
                case FieldTypes.ftDateTime:
                    if (Utility.IsDate(FieldValue) && (Convert.ToDouble(Convert.ToDateTime(FieldValue).ToOADate()) > 0))
                    {
                        FVal = SQLDateTime(Convert.ToDateTime(FieldValue));
                    }
                    break;
            }

        }

        if (!string.IsNullOrEmpty(FVal))
        {
            if (!string.IsNullOrEmpty(CriteriaSQL))
                CriteriaSQL += " AND ";
            CriteriaSQL += FieldName + Oper + FVal;
        }
    }

    //========================================
    public void AddCriteriaRange(ref string CriteriaSQL, string FieldName, object FromValue, object ToValue, FieldTypes FieldType)
    {
        string FromVal = "";
        string ToVal = "";

        if (!string.IsNullOrEmpty(FromValue + ""))
        {
            switch (FieldType)
            {
                case FieldTypes.ftNumeric:
                    if (Utility.IsNumeric(FromValue))
                        FromVal = Convert.ToString(FromValue);
                    if (Utility.IsNumeric(ToValue))
                        ToVal = Convert.ToString(ToValue);
                    break;
                case FieldTypes.ftText:
                    FromVal = "'" + FromValue.ToString().Replace("'", "''") + "'";
                    if (!string.IsNullOrEmpty(ToValue + ""))
                        ToVal = "'" + ToValue.ToString().Replace("'", "''") + "'";
                    break;
                //If ToVal & "" <> "" Then ToVal = "'" + Replace(CStr(ToValue), "'", "''") + "'"
                case FieldTypes.ftDate:
                    if (Utility.IsDate(FromValue) && (Convert.ToDouble(Convert.ToDateTime(FromValue).ToOADate()) > 0))
                        FromVal = SQLDate(Convert.ToDateTime(FromValue));
                    if (Utility.IsDate(ToValue) && (Convert.ToDouble(Convert.ToDateTime(ToValue).ToOADate()) > 0))
                        ToVal = SQLDate(Convert.ToDateTime(ToValue).AddDays(1));
                    break;
                //case FieldTypes.ftDateTime:
                //    if (Utility.IsDate(FromValue) && (Convert.ToDouble(Convert.ToDateTime(FromValue).ToOADate()) > 0))
                //        FromVal = SQLDateTime(Convert.ToDateTime(FromValue));
                //    //If IsDate(ToValue) AndAlso (CDbl(CDate(ToValue).ToOADate()) > 0) Then ToVal = SQLDateTime(CType(ToValue, Date))
                //    if (Utility.IsDate(ToValue) && (Convert.ToDouble(Convert.ToDateTime(ToValue).ToOADate()) > 0))
                //        ToVal = SQLDateTime(Convert.ToDateTime(ToValue).AddDays(1));
                //    break;

                case FieldTypes.ftDateTime:
                    if (Utility.IsDate(FromValue) && (Convert.ToDouble(Convert.ToDateTime(FromValue).ToOADate()) > 0))
                        FromVal = SQLDateTime(Convert.ToDateTime(FromValue));
                    if (Utility.IsDate(ToValue) && (Convert.ToDouble(Convert.ToDateTime(ToValue).ToOADate()) > 0))
                        //ToVal = SQLDateTime(Convert.ToDateTime(ToValue).AddDays(1));
                        ToVal = SQLDateTime(Convert.ToDateTime(ToValue).AddSeconds(1));  //-- aor edit 17/07/2018 -- พิจารณาเรื่องเวลาด้วย จึงไม่สามารถ เพิ่มวันได้                  
                    break;

            }
        }

        if (!string.IsNullOrEmpty(FromVal + ""))
        {
            if (string.IsNullOrEmpty(ToVal + ""))
            {
                AddCriteria(ref CriteriaSQL, FieldName, FromValue, FieldType);
            }
            else
            {
                if (!string.IsNullOrEmpty(CriteriaSQL))
                    CriteriaSQL += " AND ";
                if (FieldType == FieldTypes.ftDate)
                {
                    CriteriaSQL += "(" + FieldName + ">=" + FromVal + " AND " + FieldName + "<" + ToVal + ")";
                }
                else
                {
                    CriteriaSQL += "(" + FieldName + " BETWEEN " + FromVal + " AND " + ToVal + ")";
                }
            }
        }
    }

    //========================================
    // Format Value to SQL Command 
    public string SQLValue(object Value, FieldTypes DataType)
    {
        string result = null;
        string S = Utility.ToString(Value);
        string T = "";
        if (S == "" || S == "NULL")
        {
            result = "NULL";
        }
        else
        {
            T = Value.GetType().ToString().ToUpper();
            switch (DataType)
            {
                case FieldTypes.ftDate:
                    if (T.StartsWith("SYSTEM.DATE"))
                    {
                        result = SQLDate((DateTime)Value);
                    }
                    else
                    {
                        result = Value.ToString().ToUpper();
                    }
                    break;
                case FieldTypes.ftDateTime:
                    if (T.StartsWith("SYSTEM.DATE")) 
                    {
                        result = SQLDateTime((DateTime)Value);
                    }
                    else
                    {
                        result = Value.ToString().ToUpper();
                    }
                    break;
                case FieldTypes.ftNumeric:
                    if (Utility.IsNumeric(Value))
                    {
                        result = Utility.ToNum(Value).ToString();
                    }
                    else if (Value.ToString().ToUpper().EndsWith("NEXTVAL"))
                    {
                        result = Value.ToString();
                    }
                    else
                    {
                        result = "NULL";
                    }
                    break;
                case FieldTypes.ftText:
                    result = "'" + Value.ToString().Replace("'", "''") + "'";
                    break;
                default:
                    result = Value.ToString();
                    break;
            }
        }
        return result;

    }

    //========================================
    // Add Parameter to INSERT/UPDATE SQL Command
    public void AddSQL(int operation, ref string SQL1, ref string SQL2, string FieldName, object FieldValue, FieldTypes ColType)
    {
        string Data = null;

        if (!string.IsNullOrEmpty(FieldName))
        {
            Data = Convert.ToString(SQLValue(FieldValue, ColType));
            if (operation == opINSERT)
            {
                if (!string.IsNullOrEmpty(SQL1))
                {
                    SQL1 = SQL1 + ", ";
                    SQL2 = SQL2 + ", ";
                }
                SQL1 = SQL1 + FieldName;
                SQL2 = SQL2 + Data;
            }
            else
            {
                if (!string.IsNullOrEmpty(SQL1))
                    SQL1 = SQL1 + ", ";
                SQL1 = SQL1 + FieldName + "=" + Data;
            }
        }
    }

    public void AddSQL2(int operation, ref string SQL1, ref string SQL2, string FieldName, object FieldValue, FieldTypes ColType)
    {
        //สำหรับ Mng - ถ้าไม่ส่งข้อมูลมา จะเป็น null ซึ่งจะไม่ AddSQL
        if ((FieldValue != null))
        {
            if (FieldValue + "" == "NULL")
            {
                AddSQL(operation, ref SQL1, ref SQL2, FieldName, FieldValue, ColType);
            }
            else
            {
                switch (ColType)
                {
                    case FieldTypes.ftDate:
                    case FieldTypes.ftDateTime:
                        if (FieldValue.GetType().Name.ToUpper().StartsWith("SYSTEM.DATE"))
                        {
                            AddSQL(operation, ref SQL1, ref SQL2, FieldName, FieldValue, ColType);
                        }
                        else
                        {
                            AddSQL(operation, ref SQL1, ref SQL2, FieldName, Utility.AppDateValue(FieldValue), ColType);
                        }
                        break;
                    default:
                        AddSQL(operation, ref SQL1, ref SQL2, FieldName, FieldValue, ColType);
                        break;
                }
            }
        }
    }

    //========================================
    // Combine Parameter for INSERT/UPDATE SQL Command
    //-- aor edit 23/07/2018 --- ระบบนี้ใช้  CREATED_BY, CREATED_DATE, MODIFIED_BY, MODIFIED_DATE
    public string CombineSQL(int operation, ref string SQL1, ref string SQL2, string TableName, string CriteriaSQL, bool TimeStamp = true)
    {
        string SQL = "";

        switch (operation)
        {
            case opINSERT:
                if (TimeStamp)
                {
                    AddSQL(operation, ref SQL1, ref SQL2, "CREATED_DATE", System.DateTime.Now, FieldTypes.ftDateTime);
                    AddSQL(operation, ref SQL1, ref SQL2, "CREATED_BY", System.Web.HttpContext.Current.Session["USER_NAME"] + "", FieldTypes.ftText);
                    AddSQL(operation, ref SQL1, ref SQL2, "MODIFIED_DATE", System.DateTime.Now, FieldTypes.ftDateTime);
                    AddSQL(operation, ref SQL1, ref SQL2, "MODIFIED_BY", System.Web.HttpContext.Current.Session["USER_NAME"] + "", FieldTypes.ftText);
                }

                SQL = "INSERT INTO " + TableName + " (" + SQL1 + ") VALUES (" + SQL2 + ")";
                break;
            case opUPDATE:
                if (TimeStamp)
                {
                    AddSQL(operation, ref SQL1, ref SQL2, "MODIFIED_DATE", System.DateTime.Now, FieldTypes.ftDateTime);
                    AddSQL(operation, ref SQL1, ref SQL2, "MODIFIED_BY", System.Web.HttpContext.Current.Session["USER_NAME"] + "", FieldTypes.ftText);
                }

                SQL = "UPDATE " + TableName + " SET " + SQL1;
                if (CriteriaSQL.IndexOf("WHERE") + 1 > 0)
                {
                    if (!CriteriaSQL.Trim().ToUpper().StartsWith("AND"))
                    {
                        SQL = SQL + " AND " + CriteriaSQL;
                    }
                    else
                    {
                        SQL = SQL + " " + CriteriaSQL;
                    }
                }
                else if (!string.IsNullOrEmpty(CriteriaSQL))
                {
                    if (CriteriaSQL.Trim().ToUpper().StartsWith("AND"))
                    {
                        SQL = SQL + " WHERE " + CriteriaSQL.Trim().Substring(4);
                    }
                    else
                    {
                        SQL = SQL + " WHERE " + CriteriaSQL;
                    }
                }
                break;
            case opDELETE:
                SQL = "DELETE FROM " + TableName;
                if (!string.IsNullOrEmpty(CriteriaSQL))
                {
                    if (CriteriaSQL.Trim().ToUpper().StartsWith("AND"))
                    {
                        SQL = SQL + " WHERE " + CriteriaSQL.Trim().Substring(4);
                    }
                    else
                    {
                        SQL = SQL + " WHERE " + CriteriaSQL;
                    }
                }

                break;
        }
        if (operation == opINSERT)
        {
            // UPDATE
        }
        else
        {
        }
        return SQL;
    }

    //========================================
    // Combine Parameter for INSERT/UPDATE SQL Command with date_updated, user_updated
    public string CombineSQL2(ref int operation, ref string SQL1, ref string SQL2, string TableName, string CriteriaSQL)
    {
        string SQL = null;
        int I = 0;

        //default Stamp updated record
        AddSQL(operation, ref SQL1, ref SQL2, "MODIFIED_BY", System.Web.HttpContext.Current.Session["USER_NAME"], FieldTypes.ftText);
        AddSQL(operation, ref SQL1, ref SQL2, "MODIFIED_DATE", "SYSDATE", FieldTypes.ftDate);

        if (operation == opINSERT)
        {
            SQL = "INSERT INTO " + TableName + " (" + SQL1 + ") VALUES (" + SQL2 + ")";
        }
        else
        {
            SQL = "UPDATE " + TableName + " SET " + SQL1;
            I = CriteriaSQL.IndexOf("WHERE") + 1;
            if (CriteriaSQL.IndexOf("WHERE") + 1 > 0)
            {
                if (!CriteriaSQL.Trim().ToUpper().StartsWith("AND"))
                {
                    SQL = SQL + " AND " + CriteriaSQL;
                }
                else
                {
                    SQL = SQL + " " + CriteriaSQL;
                }
            }
            else if (!string.IsNullOrEmpty(CriteriaSQL))
            {
                if (CriteriaSQL.Trim().ToUpper().StartsWith("AND"))
                {
                    SQL = SQL + " WHERE " + CriteriaSQL.Trim().Substring(4);
                }
                else
                {
                    SQL = SQL + " WHERE " + CriteriaSQL;
                }
            }
        }
        return SQL;
    }


    //========================================
    // Initialize stored procedure parameters
    public void InitParams()
    {
        _Params.Clear();
    }

    //========================================
    // Add a stored procedure parameter
    public void AddParam(string ParamName, dynamic Value, FieldTypes DataType)
    {
        dynamic P = null;

        if (_ConnectStr.ToLower().IndexOf("provider") < 0)
        {
            P = new Oracle.ManagedDataAccess.Client.OracleParameter(ParamName, Value);
            switch (DataType)
            {
                case FieldTypes.ftNumeric:
                    P.OracleDbType = Oracle.ManagedDataAccess.Client.OracleDbType.Decimal;
                    P.DbType = DbType.Decimal;
                    break;
                case FieldTypes.ftText:
                    P.OracleDbType = Oracle.ManagedDataAccess.Client.OracleDbType.Varchar2;
                    P.DbType = DbType.String;
                    break;
                case FieldTypes.ftDate:
                    if (!Utility.IsDate(Value) || Convert.ToDouble(Convert.ToDateTime(Value).ToOADate()) == 0)
                        P.Value = null;
                    P.OracleDbType = Oracle.ManagedDataAccess.Client.OracleDbType.Date;
                    P.DbType = DbType.Date;
                    break;
                case FieldTypes.ftDateTime:
                    if (!Utility.IsDate(Value) || Convert.ToDouble(Convert.ToDateTime(Value).ToOADate()) == 0)
                        P.Value = null;
                    P.OracleDbType = Oracle.ManagedDataAccess.Client.OracleDbType.Date;
                    P.DbType = DbType.DateTime;
                    break;
                case FieldTypes.ftBinary:
                    P.OracleDbType = Oracle.ManagedDataAccess.Client.OracleDbType.Blob;
                    P.DbType = DbType.Binary;
                    break;
            }
        }
        else
        {
            P = new System.Data.OleDb.OleDbParameter(ParamName, Value);
            switch (DataType)
            {
                case FieldTypes.ftNumeric:
                    P.OleDbType = System.Data.OleDb.OleDbType.Numeric;
                    break;
                case FieldTypes.ftText:
                    P.OleDbType = System.Data.OleDb.OleDbType.VarChar;
                    P.DbType = DbType.String;
                    break;
                case FieldTypes.ftDate:
                    if (!Utility.IsDate(Value) || Convert.ToDouble(Convert.ToDateTime(Value).ToOADate()) == 0)
                        P.Value = null;
                    P.OleDbType = System.Data.OleDb.OleDbType.Date;
                    break;
                case FieldTypes.ftDateTime:
                    if (!Utility.IsDate(Value) || Convert.ToDouble(Convert.ToDateTime(Value).ToOADate()) == 0)
                        P.Value = null;
                    P.OleDbType = System.Data.OleDb.OleDbType.Date;
                    break;
                case FieldTypes.ftBinary:
                    P.OleDbType = System.Data.OleDb.OleDbType.Binary;
                    P.DbType = DbType.Binary;
                    break;
            }
        }

        _Params.Add(P);
    }

    //========================================
    // Execute a stored procedure

    public int ExecuteParamSQL(string SQL, dynamic _Conn = null, dynamic _Trans = null)
    {

        dynamic cmd = null;
        int rows = 0;
        int I = 0;

        try
        {
            cmd = CreateCommand(SQL, _Conn, _Trans);
            cmd.CommandType = CommandType.StoredProcedure;

            for (I = 0; I <= _Params.Count - 1; I++)
            {
                if (_ConnectStr.ToLower().IndexOf("provider") < 0)
                {
                    cmd.Parameters.Add((Oracle.ManagedDataAccess.Client.OracleParameter)_Params[I]);
                }
                else
                {
                    cmd.Parameters.Add((System.Data.OleDb.OleDbParameter)_Params[I]);
                }
            }

            rows = cmd.ExecuteNonQuery();

            InitParams();

            return rows;

        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            ClearCommand(ref cmd, _Conn);
        }
    }

    public dynamic ExecProc(string ProcName, dynamic Conn, dynamic Trans = null)
    {
        dynamic tmpConn = Conn;
        dynamic cmd = null;
        int I = 0;
        dynamic results = null;

        try
        {
            cmd = CreateCommand(ProcName);
            if ((tmpConn == null))
            {
                tmpConn = OpenConn(_ConnectStr);
            }
            cmd.Connection = tmpConn;
            cmd.CommandType = CommandType.StoredProcedure;
            for (I = 0; I <= _Params.Count - 1; I++)
            {
                if ((_DB_Provider.ToLower().IndexOf("provider")) < 0)
                {
                    cmd.Parameters.Add((Oracle.ManagedDataAccess.Client.OracleParameter)_Params[I]);
                }
                else
                {
                    cmd.Parameters.Add((System.Data.OleDb.OleDbParameter)_Params[I]);
                }
            }

            cmd.ExecuteNonQuery();
            if (cmd.Parameters.Contains("RETURN_VALUE"))
            {
                results = cmd.Parameters("RETURN_VALUE").Value;
            }

            cmd.Dispose();
            cmd = null;
            InitParams();

            return results;
        }
        catch (Exception ex)
        {
            cmd.Dispose();
            cmd = null;
            throw ex;
        }
        finally
        {
            if ((Conn == null))
                CloseConn(ref tmpConn);
        }
    }

    //========================================
    // Get Max Data

    public dynamic GetMaxData(string usrTable, string FieldName, string usrSQL, dynamic Conn = null, dynamic Trans = null)
    {
        string SQL = "";

        SQL = "SELECT MAX(" + FieldName + ") FROM " + usrTable;
        if (!string.IsNullOrEmpty(usrSQL + ""))
        {
            SQL = SQL + " WHERE " + usrSQL;
        }

        return LookupSQL(SQL, Conn, Trans);
    }

    public int ExecImageSQL(string SQL, string ImgFieldName, ref byte[] ImgFieldValue, dynamic Conn = null, dynamic Trans = null)
    {
        int rows = 0;

        try
        {
            InitParams();
            AddParam(ImgFieldName, ImgFieldValue, FieldTypes.ftBinary);
            rows = ExecuteParamSQL(SQL, Conn, Trans);

            return rows;

        }
        catch (Exception ex)
        {
            throw ex;
        }

    }


}


