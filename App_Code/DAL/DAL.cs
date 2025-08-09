//*********************************************************
//  Copyright (c), Thaiintersol co,ltd. All rights reserved.
//**********************************************************



using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.OleDb;
using System.Data;


public partial class DAL
{
    private String _dbProvider, _dbDataSource, _dbName, _dbUserName, _dbPassword;
    public String _ConnectionString;
    public DBUTIL DB;

    #region Internal routines

    public DAL()
    {
        ReadDALConfigurations();
        DB = null;
        DB = new DBUTIL();
        DB.DB_Provider = _dbProvider;
        DB.ConnectStr = _ConnectionString;
        DB.DB_Type = DB.GetDbTypes(_ConnectionString);
    }

    private void ReadDALConfigurations()
    {
        //SecurityUtil Encrypt = new SecurityUtil();
        try
        {
            _dbProvider = Utility.ToString(ConfigurationManager.AppSettings["DB_Provider"]);
            _dbDataSource = Utility.ToString(ConfigurationManager.AppSettings["DB_DataSource"]);
            _dbName = Utility.ToString(ConfigurationManager.AppSettings["DB_Name"]);
            _dbUserName = Utility.ToString(ConfigurationManager.AppSettings["DB_UserName"]);
            _dbPassword = Utility.ToString(ConfigurationManager.AppSettings["DB_Password"]);

            if (!string.IsNullOrEmpty(_dbProvider))
            {
                _ConnectionString = "Provider=" + _dbProvider + ";Data Source=" + _dbDataSource + ";Initial Catalog=" + _dbName + ";Persist Security Info=True;User ID=" + _dbUserName + ";Password=" + _dbPassword;
            }
            else if (!string.IsNullOrEmpty(_dbName))
            {
                _ConnectionString += "Initial Catalog=" + _dbName + ";Persist Security Info=True;server=" + _dbDataSource + ";User ID=" + _dbUserName + ";Password=" + _dbPassword;
            }
            else {
                _ConnectionString = "Data Source=" + _dbDataSource + ";User ID=" + _dbUserName + ";Password=" + _dbPassword;
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {

        }
    }

    #endregion

    #region General

    public String GetConnectStr()
    {
        if (_ConnectionString == "") ReadDALConfigurations();
        return (_ConnectionString);
    }

    public OleDbConnection OpenConn(string ConnStr = "")
    {
        if (ConnStr == "") {
            if (_ConnectionString == "") ReadDALConfigurations();
            ConnStr = _ConnectionString;
        }
        return DB.OpenConn(ConnStr);
    }

    public void CloseConn(ref dynamic Conn)
    {
        DB.CloseConn(ref Conn);
    }

    public OleDbTransaction BeginTrans(dynamic Conn)
    {
        return DB.BeginTrans(ref Conn);
    }

    public void CommitTrans(dynamic Trans)
    {
        DB.CommitTrans(ref Trans);
    }

    public void RollbackTrans(dynamic Trans)
    {
        DB.RollbackTrans(Trans);
    }

    public DataTable QueryData(string SQL, OleDbConnection Conn = null, OleDbTransaction Trans = null)
    {
        return DB.QueryData(SQL, Conn, Trans); ;
    }

    public int ExecuteSQL(string SQL, OleDbConnection Conn = null, OleDbTransaction Trans = null)
    {
        return DB.ExecuteSQL(SQL, Conn, Trans);
    }

    public int ExecuteParamSQL(string sql, OleDbConnection usrConn = null, OleDbTransaction usrTrans = null)
    {
        return DB.ExecuteParamSQL(sql, usrConn, usrTrans);
    }

    public void AddCriteria(ref String criteriaSQL, String fieldName, Object fieldValue, DBUTIL.FieldTypes fieldType, bool AllowIN = false)
    {
        DB.AddCriteria(ref criteriaSQL, fieldName, fieldValue, fieldType, AllowIN);
    }

    public void AddCriteriaRange(ref String criteriaSQL, String fieldName, Object fromValue, Object toValue, DBUTIL.FieldTypes fieldType)
    {
        DB.AddCriteriaRange(ref criteriaSQL, fieldName, fromValue, toValue, fieldType);
    }

    public Object SQLValue(Object value, DBUTIL.FieldTypes fieldType)
    {
        return DB.SQLValue(value, fieldType);
    }

    public String SQLDate(System.DateTime d)
    {
        return DB.SQLDate(d);
    }

    public void AddSQL(Int32 operation, ref String sql1, ref String sql2, String fieldName, Object fieldValue, DBUTIL.FieldTypes fieldType)
    {
        DB.AddSQL(operation, ref sql1, ref sql2, fieldName, fieldValue, fieldType);
    }

    public void AddSQL2(Int32 operation, ref String sql1, ref String sql2, String fieldName, Object fieldValue, DBUTIL.FieldTypes fieldType)
    {
        DB.AddSQL2(operation, ref sql1, ref sql2, fieldName, fieldValue, fieldType);
    }

    public String CombineSQL(Int32 operation, ref String sql1, ref String sql2, String tableName, String criteriaSQL)
    {
        return CombineSQL(operation, ref sql1, ref sql2, tableName, criteriaSQL, timeStamp: true);
    }

    public String CombineSQL(Int32 operation, ref String sql1, ref String sql2, String tableName, String criteriaSQL, Boolean timeStamp = true)
    {
        return DB.CombineSQL(operation, ref sql1, ref sql2, tableName, criteriaSQL, timeStamp);
    }

    public Object GenerateID(String tableName, String idField, String criteria = "", String prefix = "", Int32 idLength = 0, OleDbConnection conn = null, OleDbTransaction trans = null)
    {
        String sql = "";
        Object id = null; String tmp = null;
        try
        {
            sql = "SELECT MAX(" + idField + ") FROM " + tableName;
            if (prefix != "")
            {
                if (criteria != "") { criteria += " AND "; }
                criteria += idField + " LIKE '" + prefix + "%'";
            }
            if (criteria != "") { sql += " WHERE " + criteria; }
            id = LookupSQL(sql, conn, trans);//Lookup

            if (Utility.ToString(id) != "")
            {
                if (id.ToString().IndexOf("-") > 0 && prefix == "")
                {
                    tmp = id.ToString().Substring(id.ToString().IndexOf("-") + 1);
                    tmp = (Utility.ToNum(tmp) + 1).ToString();
                }
                else
                {
                    if (prefix != "") { id = id.ToString().Substring(prefix.Length + 1); }
                    id = Utility.ToNum(id) + 1;
                }
            }
            else
            {
                id = "1";
            }
            if (prefix != "" || idLength > 0) { id = prefix + Utility.ToString(id).PadLeft(idLength, '0'); }

            return id;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    //running is Number only!!
    public Object GenerateID2(String tableName, String idField, String criteria, String prefix, Int32 idLength, OleDbConnection conn, OleDbTransaction trans)
    {
        String sql = "";
        DataTable DT;
        Object id = null; String tmp = null;
        int firstValue = 0;
        int currentValue = 0;
        int newValue = 0;
        int nextValue = 0;
        bool IsEmpty = false;
        try
        {

            try
            {
                sql = "SELECT " + idField + " FROM " + tableName;
                if (prefix != "")
                {
                    if (criteria != "") { criteria += " AND "; }
                    criteria += idField + " LIKE '" + prefix + "%'";
                }
                if (criteria != "") { sql += " WHERE " + criteria; }
                sql += " ORDER BY " + idField;
                DT = QueryData(sql, conn, trans);

                for (int running = 0; running < DT.Rows.Count; running++)
                {
                    firstValue = Utility.ToInt(DT.Rows[0][idField]);
                    currentValue = Utility.ToInt(DT.Rows[running][idField]);
                    newValue = Utility.ToInt(DT.Rows[running][idField]) + 1;

                    nextValue = Utility.ToInt(DT.Rows[running + 1][idField]);
                    if (newValue != nextValue)
                    {
                        IsEmpty = true;
                        break;
                    }
                }
            }
            catch
            {
                IsEmpty = true;
            }

            if (firstValue != 1)
            {
                id = 1;
            }
            else if (IsEmpty)
            {
                id = newValue;  // มีช่องวาง
            }
            else
            {
                sql = "SELECT MAX(" + idField + ") FROM " + tableName;
                if (prefix != "")
                {
                    if (criteria != "") { criteria += " AND "; }
                    criteria += idField + " LIKE '" + prefix + "%'";
                }
                if (criteria != "") { sql += " WHERE " + criteria; }
                id = LookupSQL(sql, conn, trans);//Lookup
                if (id != null)
                {
                    if (id.ToString().IndexOf("-") > 0)
                    {
                        tmp = id.ToString().Substring(id.ToString().IndexOf("-") + 1);
                        tmp = (Utility.ToNum(tmp) + 1).ToString();
                    }
                    else
                    {
                        if (prefix != "") { id = id.ToString().Substring(prefix.Length + 1); }
                        id = Utility.ToNum(id) + 1;
                    }
                }
                else
                {
                    id = "1";
                }
            }
            if (prefix != "" || idLength > 0) { id = prefix + Utility.ToString(id).PadLeft(idLength, '0'); }
            return id;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    public Object LookupSQL(String sql, OleDbConnection conn = null, OleDbTransaction trans = null)
    {
        return (DB.LookupSQL(sql, conn, trans));
    }

    public string GetSQLValue(string SQL)
    {
        string value;
        try
        {
            value = Utility.ToString(LookupSQL(SQL));
            return value;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    #endregion

    #region Admin

    //-- aor edit 23/12/2016 --
    public DataTable SearchUserList(String UserName, String UserDesc = "", String RoleID = "", String orderSQL = "", String OtherCriteria = "")
    {
        String sql = ""; String criteria = "";

        try
        {
            if (OtherCriteria != "") { criteria = OtherCriteria; }
            AddCriteria(ref criteria, "UPPER(U.USER_NAME)", UserName.ToUpper(), DBUTIL.FieldTypes.ftText);
            AddCriteria(ref criteria, "UPPER(U.USER_DESC)", UserDesc.ToUpper(), DBUTIL.FieldTypes.ftText);
            AddCriteria(ref criteria, "U.ROLE_ID", RoleID, DBUTIL.FieldTypes.ftNumeric);


            sql = "SELECT U.*, R.ROLE_NAME, R.RIGHTS, CASE DISABLED_FLAG WHEN 'Y' THEN 'Disable' ELSE 'Enable' END AS USER_STATUS " +
                  "FROM O_SYS_USERS U LEFT OUTER JOIN O_SYS_ROLES R ON U.ROLE_ID = R.ROLE_ID ";

            if (criteria != "") { sql += " WHERE " + criteria; }


            if (orderSQL != "")
            {
                sql += " ORDER BY " + orderSQL;
            }
            else
            {
                sql += " ORDER BY U.ROLE_ID, U.USER_NAME";
            }

            return QueryData(sql);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    //-- aor edit 30/01/2018 --
    public DataTable SearchUserData(String UserName, String RoleID)
    {
        String sql = ""; String criteria = "";

        try
        {
            AddCriteria(ref criteria, "UPPER(U.USER_NAME)", UserName.ToUpper(), DBUTIL.FieldTypes.ftText);
            AddCriteria(ref criteria, "U.ROLE_ID", RoleID, DBUTIL.FieldTypes.ftNumeric);

            sql = "SELECT U.*, R.ROLE_NAME, R.RIGHTS, CASE DISABLED_FLAG WHEN 'Y' THEN 'Disabled' ELSE 'Enable' END AS USER_STATUS " +
                  " FROM O_SYS_USERS U LEFT OUTER JOIN O_SYS_ROLES R ON U.ROLE_ID = R.ROLE_ID ";


            if (criteria != "") { sql += " WHERE " + criteria; }

            sql += " ORDER BY U.ROLE_ID, U.USER_NAME";

            return QueryData(sql);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    //-- aor edit 30/01/2018 --
    public DataRow GetUserData(String UserName)
    {
        DataTable DT = null;
        String sql = ""; String criteria = "";
        string ret = "";
        try
        {
            AddCriteria(ref criteria, "U.USER_NAME", UserName, DBUTIL.FieldTypes.ftText);

            sql = "SELECT U.*, R.ROLE_NAME , R.ROLE_DESC , R.RIGHTS " +
                  " FROM O_SYS_USERS U LEFT OUTER JOIN O_SYS_ROLES R ON U.ROLE_ID = R.ROLE_ID ";

            if (criteria != "") { sql += " WHERE " + criteria; }

            DB.OpenDT(ref DT, sql);

            return Utility.GetDR(ref DT);

        }
        catch (Exception ex)
        {
            ret = Utility.GetErrorMessage(ex, UsrMsg: "Load UserData Error: Username=" + UserName + " Message=" + ex.Message);
            throw ex;
        }
    }

    //-- edit 05/07/2018 --
    public void MngUserData(int op, String UserName, String UserDesc = null, String RoleID = null, String EmployeeID = null, String PositionName = null,
                            String UnitName = null, String UserEmail = null, String DisableFlag = null, String SignFilename = null)
    {
        string SQL = "", SQL1, SQL2, Criteria = "";
        try
        {
            SQL = ""; SQL1 = ""; SQL2 = "";
            if (op != DBUTIL.opINSERT)
            {
                AddCriteria(ref Criteria, "USER_NAME", UserName, DBUTIL.FieldTypes.ftText);
            }
            if (op != DBUTIL.opDELETE)
            {
                if (op == DBUTIL.opINSERT)
                {
                    AddSQL(op, ref SQL1, ref SQL2, "USER_NAME", UserName, DBUTIL.FieldTypes.ftText);
                }
                else
                {
                    op = DBUTIL.opUPDATE;
                }

                AddSQL2(op, ref SQL1, ref SQL2, "USER_DESC", UserDesc, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "ROLE_ID", RoleID, DBUTIL.FieldTypes.ftNumeric);
                AddSQL2(op, ref SQL1, ref SQL2, "EMPLOYEE_ID", EmployeeID, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "POSITION_NAME", PositionName, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "UNIT_NAME", UnitName, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "USER_EMAIL", UserEmail, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "DISABLED_FLAG", DisableFlag, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "SIGN_FILENAME", SignFilename, DBUTIL.FieldTypes.ftText);
               

            }

            if (op != DBUTIL.opINSERT && Criteria == "")
            {
                throw new Exception("Insufficient data!");
            }
            else
            {
                SQL = CombineSQL(op, ref SQL1, ref SQL2, "O_SYS_USERS", Criteria);
                ExecuteSQL(SQL);
            }


        }
        catch (Exception ex)
        {
            BLL.InsertAudit(Project.catErrorLog, SQL, "MngUserData", "", "");
            throw ex;
        }

    }

    //-- aor edit 23/12/2016 --
    public DataTable SearchRoleData(String RoleID, String RoleName, String RoleDesc, String orderSQL = "", String OtherCriteria = "")
    {
        String sql = ""; String criteria = "";

        try
        {
            if (OtherCriteria != "") { criteria = OtherCriteria; }
            AddCriteria(ref criteria, "ROLE_ID", RoleID, DBUTIL.FieldTypes.ftNumeric);
            AddCriteria(ref criteria, "UPPER(ROLE_NAME)", RoleName.ToUpper(), DBUTIL.FieldTypes.ftText);
            AddCriteria(ref criteria, "UPPER(ROLE_DESC)", RoleDesc.ToUpper(), DBUTIL.FieldTypes.ftText);

            sql = "SELECT * FROM O_SYS_ROLES ";

            if (criteria != "") { sql += " WHERE " + criteria; }

            if (orderSQL != "")
            {
                sql += " ORDER BY " + orderSQL;
            }
            else
            {
                sql += " ORDER BY ROLE_ID";
            }


            return QueryData(sql);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    //-- aor edit 25/05/2018 --
    public void MngRoleData(int op, ref String RoleID, String RoleName, String RoleDesc, String Rights)
    {
        string SQL, SQL1, SQL2, Criteria = "";
        try
        {
            SQL = ""; SQL1 = ""; SQL2 = "";
            if (op != DBUTIL.opINSERT)
            {
                AddCriteria(ref Criteria, "ROLE_ID", RoleID, DBUTIL.FieldTypes.ftNumeric);
            }
            if (op != DBUTIL.opDELETE)
            {
                if (op == DBUTIL.opINSERT)
                {
                    RoleID = GenerateID("O_SYS_ROLES", "ROLE_ID").ToString();
                    AddSQL(op, ref SQL1, ref SQL2, "ROLE_ID", RoleID, DBUTIL.FieldTypes.ftNumeric);
                }
                else
                {
                    op = DBUTIL.opUPDATE;
                }

                AddSQL2(op, ref SQL1, ref SQL2, "ROLE_NAME", RoleName, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "ROLE_DESC", RoleDesc, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "RIGHTS", Rights, DBUTIL.FieldTypes.ftText);

            }

            if (op != DBUTIL.opINSERT && Criteria == "")
            {
                throw new Exception("Insufficient data!");
            }
            else
            {
                SQL = CombineSQL(op, ref SQL1, ref SQL2, "O_SYS_ROLES", Criteria);
                ExecuteSQL(SQL);
            }


        }
        catch (Exception ex)
        {
            throw ex;
        }

    }


    //-- aor edit 23/12/2016 --
    public DataTable SearchTaskList(String orderSQL = "")
    {
        String sql = "";
        try
        {
            sql = "SELECT * FROM O_SYS_TASKS ";

            if (orderSQL != "")
            {
                sql += " ORDER BY " + orderSQL;
            }
            else
            {
                sql += " ORDER BY TASK_ID";
            }


            return QueryData(sql);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    //-- aor edit 17/05/2017 --
    public String GetSysConfigValue(String cfgItem)
    {
        String sql = ""; String criteria = "";
        DataTable DT = null;
        String result = "";
        try
        {
            AddCriteria(ref criteria, "CFG_ITEM", cfgItem, DBUTIL.FieldTypes.ftText);

            sql = "SELECT CFG_VALUE1 FROM O_SYS_CONFIGS ";
            if (criteria != "") { sql += " WHERE " + criteria; }

            DT = QueryData(sql);
            if (DT.Rows.Count > 0) result = Utility.ToString(DT.Rows[0]["CFG_VALUE1"]);

            return result;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            Utility.ClearObject(ref DT);
        }
    }

    //-- aor edit 26/07/2018 --
    public DataTable SearchSysConfig(String cfgItem, String orderSQL = "", String OtherCriteria = "")
    {
        String sql = ""; String criteria = "";

        try
        {
            if (OtherCriteria != "") { criteria = OtherCriteria; }
            AddCriteria(ref criteria, "CFG_ITEM", cfgItem, DBUTIL.FieldTypes.ftText);

            sql = "SELECT * FROM O_SYS_CONFIGS ";

            if (criteria != "") { sql += " WHERE " + criteria; }

            if (orderSQL != "")
            {
                sql += " ORDER BY " + orderSQL;
            }
            else
            {
                sql += " ORDER BY CFG_ITEM ";
            }


            return QueryData(sql);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    //-- aor edit 25/07/2018 --
    public void MngSysConfig(int op, String cfgItem, String cfgValue1, string userName = "")
    {
        string SQL, SQL1, SQL2, Criteria = "";
        try
        {
            if (userName == "") userName = Utility.ToString(System.Web.HttpContext.Current.Session["USER_NAME"]);

            SQL = ""; SQL1 = ""; SQL2 = "";
            if (op != DBUTIL.opINSERT)
            {
                AddCriteria(ref Criteria, "CFG_ITEM", cfgItem, DBUTIL.FieldTypes.ftText);
            }
            if (op != DBUTIL.opDELETE)
            {
                if (op == DBUTIL.opINSERT)
                {
                    AddSQL(op, ref SQL1, ref SQL2, "CFG_ITEM", cfgItem, DBUTIL.FieldTypes.ftText);
                }
                else
                {
                    op = DBUTIL.opUPDATE;
                }

                AddSQL2(op, ref SQL1, ref SQL2, "CFG_VALUE1", cfgValue1, DBUTIL.FieldTypes.ftText);

                AddSQL2(op, ref SQL1, ref SQL2, "MODIFIED_DATE", System.DateTime.Now, DBUTIL.FieldTypes.ftDateTime);
                AddSQL2(op, ref SQL1, ref SQL2, "MODIFIED_BY", userName, DBUTIL.FieldTypes.ftText);

            }

            if (op != DBUTIL.opINSERT && Criteria == "")
            {
                throw new Exception("Insufficient data!");
            }
            else
            {
                SQL = CombineSQL(op, ref SQL1, ref SQL2, "O_SYS_CONFIGS", Criteria,timeStamp:false);
                ExecuteSQL(SQL);
            }


        }
        catch (Exception ex)
        {
            throw ex;
        }

    }


    #endregion

    #region Audit

    //-- aor edit 14/03/2017 --
    public DataTable SearchLastLogOn(String UserName, String EventDetail, Object DateF)
    {
        String sql = ""; String criteria = "";

        try
        {
            AddCriteria(ref criteria, "USER_NAME", UserName, DBUTIL.FieldTypes.ftText);
            AddCriteria(ref criteria, "EVENT_DETAIL", EventDetail, DBUTIL.FieldTypes.ftText);
            if (DateF != null ) AddCriteria(ref criteria, "TRANS_DATE", "<" + Utility.AppDateValue(DateF), DBUTIL.FieldTypes.ftDateTime);

            sql = "SELECT * FROM O_SYS_LOGS WHERE TRANS_ID = (SELECT MAX(TRANS_ID) FROM O_SYS_LOGS WHERE CATEGORY = 'LOGON' ";
            if (criteria != "") { sql += " AND " + criteria; }
            sql += " ) ";


            return QueryData(sql);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    //-- aor edit 17/07/2018 --
    public DataTable SearchLastEventLog(String UserName, String EventDetail, Object DateF, String Category, String OtherCriteria = "")
    {
        String sql = ""; String criteria = "";

        try
        {
            criteria = OtherCriteria;
            AddCriteria(ref criteria, "USER_NAME", UserName, DBUTIL.FieldTypes.ftText);
            AddCriteria(ref criteria, "EVENT_DETAIL", EventDetail, DBUTIL.FieldTypes.ftText);
            AddCriteria(ref criteria, "CATEGORY", Category, DBUTIL.FieldTypes.ftText);
            if (DateF != null) AddCriteria(ref criteria, "TRANS_DATE", "<" + Utility.AppDateValue(DateF), DBUTIL.FieldTypes.ftDateTime);


            sql = "SELECT * FROM O_SYS_LOGS WHERE TRANS_ID = (SELECT MAX(TRANS_ID) FROM O_SYS_LOGS ";
            if (criteria != "") { sql += " WHERE " + criteria; }
            sql += " ) ";


            return QueryData(sql);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    //-- aor edit 15/02/2018 --
    public DataTable SearchEventLog(String UserName, String EventDetail, Object DateF, Object DateT, String Category, String OtherCriteria = "")
    {
        String sql = ""; String criteria = "";
        String criteria2 = "";
        try
        {
            criteria = OtherCriteria;
            AddCriteria(ref criteria, "UPPER(USER_NAME)", UserName.ToUpper(), DBUTIL.FieldTypes.ftText);
            AddCriteria(ref criteria, "UPPER(EVENT_DETAIL)", EventDetail.ToUpper(), DBUTIL.FieldTypes.ftText);
            AddCriteria(ref criteria, "CATEGORY", Category, DBUTIL.FieldTypes.ftText);
            AddCriteriaRange(ref criteria, "TRANS_DATE", Utility.AppDateValue(DateF), Utility.AppDateValue(DateT), DBUTIL.FieldTypes.ftDate);

  
            sql = "SELECT TRANS_ID, TRANS_DATE, CATEGORY, EVENT_DETAIL, USER_NAME, IP_ADDRESS FROM O_SYS_LOGS WHERE NOT CATEGORY LIKE 'ERROR%' ";
            if (criteria != "") { sql += " AND " + criteria; }


            AddCriteriaRange(ref criteria2, "SEND_DATE", Utility.AppDateValue(DateF), Utility.AppDateValue(DateT), DBUTIL.FieldTypes.ftDate);
            if (EventDetail != "")
            {
                if (criteria2 != "") criteria2 += " AND ";
                criteria2 += " ( CASE WHEN STATUS= 99 THEN SUBJECT WHEN STATUS= 9 THEN 'Completed, Mail To:' || NVL(MAIL_TO, '') || CASE WHEN NVL(MAIL_CC, '') <> '' THEN  ', Mail CC:' ||  NVL(MAIL_CC, '') END  " +
                            "        WHEN STATUS = 8 THEN 'Resend Completed, Mail To:' || NVL(MAIL_TO, '') || CASE WHEN NVL(MAIL_CC, '') <> '' THEN  ', Mail CC:' || NVL(MAIL_CC, '') END " +
                            "        WHEN STATUS = 7 THEN 'Error[' || ERR_MSG || '], Mail To:' || NVL(MAIL_TO, '') || CASE WHEN NVL(MAIL_CC, '') <> '' THEN  ', Mail CC:' || NVL(MAIL_CC, '') END " +
                            "        ELSE 'Created, Mail To:' || NVL(MAIL_TO, '') || CASE WHEN NVL(MAIL_CC, '') <> '' THEN  ', Mail CC:' || NVL(MAIL_CC, '') END END LIKE '" + EventDetail + "' )";
            }
            if (Category!= "")
            {
                if (criteria2 != "") criteria2 += " AND ";
                criteria2 += " 'SEND MAIL' ='" + Category + "' ";
            }

            sql += " UNION   " +
                 "  SELECT TRANS_ID, SEND_DATE AS TRANS_DATE, 'SEND MAIL' AS CATEGORY,   " +
                 " CASE WHEN STATUS = 99 THEN SUBJECT WHEN STATUS = 9 THEN 'Completed, Mail To:' || NVL(MAIL_TO, '') || CASE WHEN NVL(MAIL_CC, '') <> '' THEN  ', Mail CC:' || NVL(MAIL_CC, '') END " +
                 "      WHEN STATUS = 8 THEN 'Resend Completed, Mail To:' || NVL(MAIL_TO, '') || CASE WHEN NVL(MAIL_CC, '') <> '' THEN  ', Mail CC:' || NVL(MAIL_CC, '') END " +
                 "      WHEN STATUS = 7 THEN 'Error[' || ERR_MSG || '], Mail To:' || NVL(MAIL_TO, '') || CASE WHEN NVL(MAIL_CC, '') <> '' THEN  ', Mail CC:' || NVL(MAIL_CC, '') END " +
                 "      ELSE 'Created, Mail To:' || NVL(MAIL_TO, '') || CASE WHEN NVL(MAIL_CC, '') <> '' THEN  ', Mail CC:' || NVL(MAIL_CC, '') END END EVENT_DETAIL " +
                 "    ,E.CREATED_BY ,'' FROM O_EMAIL_LOG  E ";

            if (criteria2 != "") { sql += " WHERE " + criteria2; }


            sql += " ORDER BY TRANS_DATE DESC ";




            return QueryData(sql);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    //-- aor edit 15/02/2019 --
    //-- edit 21/06/2019 ---
    public DataTable SearchChangeLog(String UserName, String Data, Object DateF, Object DateT, String FID, String orderSQL = "", String OtherCriteria = "")
    {
        String sql = ""; String criteria = "";

        try
        {
            criteria = OtherCriteria;
            AddCriteria(ref criteria, "UPPER(USER_NAME)", UserName.ToUpper(), DBUTIL.FieldTypes.ftText);
            AddCriteria(ref criteria, "UPPER(FIELD_NAME)", Data.ToUpper(), DBUTIL.FieldTypes.ftText);
            AddCriteria(ref criteria, "UPPER(FID)", FID, DBUTIL.FieldTypes.ftText);
            AddCriteriaRange(ref criteria, "TRANS_DATE", Utility.AppDateValue(DateF), Utility.AppDateValue(DateT), DBUTIL.FieldTypes.ftDate);

            sql = "SELECT * FROM O_SYS_CHLOGS ";
            if (criteria != "") { sql += " WHERE " + criteria; }

            if (orderSQL != "")
            {
                sql += " ORDER BY " + orderSQL;
            }
            else
            {
                sql += " ORDER BY TRANS_DATE, USER_NAME";
            }

            return QueryData(sql);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    //-- aor edit 04/01/2017 --
    //-- aor edit 27/04/2017 -- กำหนด TRANS_ID เป็น identity
    public void InsertAuditLog(string Category, string Event, string User)
    {
        string SQL, SQL1, SQL2, Criteria = "";
        try
        {
            SQL = ""; SQL1 = ""; SQL2 = "";

            if (Event.Length > 8000) Event = Event.Substring(0, 8000);
            if (User.Length > 20) User = User.Substring(0, 20);
            string nID = GenerateID("O_SYS_LOGS", "TRANS_ID").ToString();
            AddSQL(DBUTIL.opINSERT, ref SQL1, ref SQL2, "TRANS_ID", nID, DBUTIL.FieldTypes.ftNumeric);

            AddSQL(DBUTIL.opINSERT, ref SQL1, ref SQL2, "TRANS_DATE", System.DateTime.Now, DBUTIL.FieldTypes.ftDateTime);
            AddSQL(DBUTIL.opINSERT, ref SQL1, ref SQL2, "CATEGORY", Category, DBUTIL.FieldTypes.ftText);
            AddSQL(DBUTIL.opINSERT, ref SQL1, ref SQL2, "EVENT_DETAIL", Event, DBUTIL.FieldTypes.ftText);
            AddSQL(DBUTIL.opINSERT, ref SQL1, ref SQL2, "USER_NAME", User, DBUTIL.FieldTypes.ftText);
            AddSQL(DBUTIL.opINSERT, ref SQL1, ref SQL2, "IP_ADDRESS", HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString(), DBUTIL.FieldTypes.ftText);
            AddSQL(DBUTIL.opINSERT, ref SQL1, ref SQL2, "URL", HttpContext.Current.Request.Url.AbsoluteUri, DBUTIL.FieldTypes.ftText);



            SQL = CombineSQL(DBUTIL.opINSERT, ref SQL1, ref SQL2, "O_SYS_LOGS", Criteria, false);
            ExecuteSQL(SQL);

        }
        catch (Exception ex)
        {
            throw ex;
        }

    }

    //-- aor edit 04/01/2017 --
    public DataTable SearchAuditLog(String FromDate, String ToDate, string Category, string Action, string User, string OrderSQL = "", string OtherCriteria = "")
    {
        DataTable DT = null;
        String sql = ""; String criteria = "";

        try
        {
            criteria = OtherCriteria;
            AddCriteriaRange(ref criteria, "TRANS_DATE", Utility.AppDateValue(FromDate), Utility.AppDateValue(ToDate), DBUTIL.FieldTypes.ftDate);
            AddCriteria(ref criteria, "CATEGORY", Category, DBUTIL.FieldTypes.ftText);
            AddCriteria(ref criteria, "UPPER(EVENT_DETAIL)", Action.ToUpper(), DBUTIL.FieldTypes.ftText);
            AddCriteria(ref criteria, "UPPER(USER_NAME)", User.ToUpper(), DBUTIL.FieldTypes.ftText);

            sql = "SELECT * FROM O_SYS_LOGS  ";

            if (criteria != "") { sql += " WHERE " + criteria; }
            if (OrderSQL == "")
            {
                sql += " ORDER BY TRANS_ID DESC";
            }
            else
            {
                sql += " ORDER BY " + OrderSQL;
            }
            DB.OpenDT(ref DT, sql);

            return DT;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    #endregion

    #region MasterData


    //-- edit 06/07/2018 --
    public DataTable SearchDimRegion(String RegionID = "", String orderSQL = "", String OtherCriteria = "")
    {
        String sql = ""; String criteria = "";
        try
        {
            if (OtherCriteria != "") { criteria = OtherCriteria; }
            AddCriteria(ref criteria, "REGION_ID", RegionID, DBUTIL.FieldTypes.ftText);


            sql = "SELECT * FROM O_DIM_REGION ";
            if (criteria != "") { sql += " WHERE " + criteria; }


            if (orderSQL != "")
            {
                sql += " ORDER BY " + orderSQL;
            }
            else
            {
                sql += " ORDER BY REGION_ID";
            }

            return QueryData(sql);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    //-- edit 26/06/2023 ---
    public void MngDimRegion(int op, string RegionID, string RegionName, string RegionFull, string RegionAddr)
    {
        string SQL, SQL1, SQL2, Criteria = "";
        try
        {
            SQL = ""; SQL1 = ""; SQL2 = "";
            if (op != DBUTIL.opINSERT)
            {
                Project.dal.AddCriteria(ref Criteria, "REGION_ID", RegionID, DBUTIL.FieldTypes.ftText);
            }
            if (op != DBUTIL.opDELETE)
            {
                if (op == DBUTIL.opINSERT)
                {
                    Project.dal.AddSQL(op, ref SQL1, ref SQL2, "REGION_ID", Utility.ToString(RegionID).Trim(), DBUTIL.FieldTypes.ftText);

                }
                else
                {
                    op = DBUTIL.opUPDATE;
                }

                Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "REGION_NAME", Utility.ToString(RegionName).Trim(), DBUTIL.FieldTypes.ftText);
                Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "REGION_FULL", RegionFull, DBUTIL.FieldTypes.ftText);
                Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "REGION_ADDR", RegionAddr, DBUTIL.FieldTypes.ftText);

            }

            if (op != DBUTIL.opINSERT && Criteria == "")
            {
                throw new Exception("Insufficient data!");
            }
            else
            {
                SQL = Project.dal.CombineSQL(op, ref SQL1, ref SQL2, "O_DIM_REGION", Criteria, timeStamp: false);
                ExecuteSQL(SQL);
            }


        }
        catch (Exception ex)
        {
            throw ex;
        }
    }



    //-- edit 06/07/2018 --
    //-- EDIT 19/07/2019 -- S.FID AS FID_NAME,
    //-- EDIT 26/06/2023 -- เพิ่ม REGION_FULL, REGION_ADDR
    public DataTable SearchSiteFID(String SiteID = "", String FID = "", String SiteName = "", String RegionID = "", String isoFlag = "", String h2sFlag = "", String orderSQL = "", String OtherCriteria = "")
    {
        String sql = ""; String criteria = "";
        try
        {
            if (OtherCriteria != "") { criteria = OtherCriteria; }
            AddCriteria(ref criteria, "S.SITE_ID", SiteID, DBUTIL.FieldTypes.ftNumeric);
            AddCriteria(ref criteria, "UPPER(S.FID)", FID.ToUpper(), DBUTIL.FieldTypes.ftText);
            AddCriteria(ref criteria, "UPPER(S.SITE_NAME)", SiteName.ToUpper(), DBUTIL.FieldTypes.ftText);
            AddCriteria(ref criteria, "S.REGION_ID", RegionID, DBUTIL.FieldTypes.ftText);
            AddCriteria(ref criteria, "S.ISO_FLAG", isoFlag, DBUTIL.FieldTypes.ftText);
            AddCriteria(ref criteria, "S.H2S_FLAG", h2sFlag, DBUTIL.FieldTypes.ftText);

            sql = "SELECT S.* , S.FID AS FID_NAME, R.REGION_NAME , R.REGION_FULL, R.REGION_ADDR " +
                " ,CASE ISO_FLAG WHEN 'Y' THEN 'Yes' WHEN 'N' THEN 'No' ELSE '' END  AS ISO_FLAG_DESC " +
                " ,CASE H2S_FLAG WHEN 'Y' THEN 'Yes' WHEN 'N' THEN 'No' ELSE '' END  AS H2S_FLAG_DESC " +
                " ,CASE ALERT_FLAG WHEN 'Y' THEN 'Yes' WHEN 'N' THEN 'No' ELSE '' END  AS ALERT_FLAG_DESC " +
                " FROM O_SITE_FID S LEFT OUTER JOIN O_DIM_REGION R ON S.REGION_ID = R.REGION_ID ";


            if (criteria != "") { sql += " WHERE " + criteria; }

            if (orderSQL != "")
            {
                sql += " ORDER BY " + orderSQL;
            }
            else
            {
                sql += " ORDER BY NVL(R.REGION_ID,'ZZ'), UPPER(S.FID) ";
            }

            return QueryData(sql);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    //-- EDIT 30/03/2022 --  
    public DataTable SearchSiteFID_REPORT(String SiteID = "", String FID = "", String SiteName = "", String RegionID = "", String isoFlag = "", String h2sFlag = "", String orderSQL = "", String OtherCriteria = "")
    {
        String sql = ""; String criteria = "";
        try
        {
            if (OtherCriteria != "") { criteria = OtherCriteria; }
            AddCriteria(ref criteria, "S.SITE_ID", SiteID, DBUTIL.FieldTypes.ftNumeric);
            AddCriteria(ref criteria, "UPPER(S.FID)", FID.ToUpper(), DBUTIL.FieldTypes.ftText);
            AddCriteria(ref criteria, "UPPER(S.SITE_NAME)", SiteName.ToUpper(), DBUTIL.FieldTypes.ftText);
            AddCriteria(ref criteria, "S.REGION_ID", RegionID, DBUTIL.FieldTypes.ftText);
            AddCriteria(ref criteria, "S.ISO_FLAG", isoFlag, DBUTIL.FieldTypes.ftText);
            AddCriteria(ref criteria, "S.H2S_FLAG", h2sFlag, DBUTIL.FieldTypes.ftText);

 
            //-- edit 08/03/2022
            sql = "SELECT S.* , S.FID AS FID_NAME, R.REGION_NAME   " +
              " ,CASE ISO_FLAG WHEN 'Y' THEN 'Yes' WHEN 'N' THEN 'No' ELSE '' END AS ISO_FLAG_DESC " +
              " ,CASE H2S_FLAG WHEN 'Y' THEN 'Yes' WHEN 'N' THEN 'No' ELSE '' END AS H2S_FLAG_DESC " +
              " ,CASE ALERT_FLAG WHEN 'Y' THEN 'Yes' WHEN 'N' THEN 'No' ELSE '' END AS ALERT_FLAG_DESC " +
              " ,P1.NGBILL_RPT_NO RPT_DAILY20, P2.NGBILL_RPT_NO RPT_20DAY, P3.NGBILL_RPT_NO RPT_DAILY27 " +
              " , P4.NGBILL_RPT_NO RPT_27DAY, P5.NGBILL_RPT_NO RPT_DAILY, P6.NGBILL_RPT_NO RPT_ENDMTH " +
              " FROM O_SITE_FID S LEFT OUTER JOIN O_DIM_REGION R ON S.REGION_ID = R.REGION_ID " +
              " LEFT OUTER JOIN O_SITE_REPORT P1 ON S.SITE_ID = P1.SITE_ID AND P1.RPT_TYPE = 'DAILY20' " +
              " LEFT OUTER JOIN O_SITE_REPORT P2 ON S.SITE_ID = P2.SITE_ID AND P2.RPT_TYPE = '20DAY' " +
              " LEFT OUTER JOIN O_SITE_REPORT P3 ON S.SITE_ID = P3.SITE_ID AND P3.RPT_TYPE = 'DAILY27' " +
              " LEFT OUTER JOIN O_SITE_REPORT P4 ON S.SITE_ID = P4.SITE_ID AND P4.RPT_TYPE = '27DAY' " +
              " LEFT OUTER JOIN O_SITE_REPORT P5 ON S.SITE_ID = P5.SITE_ID AND P5.RPT_TYPE = 'DAILY' " +
              " LEFT OUTER JOIN O_SITE_REPORT P6 ON S.SITE_ID = P6.SITE_ID AND P6.RPT_TYPE = 'ENDMTH'   ";


            if (criteria != "") { sql += " WHERE " + criteria; }

            if (orderSQL != "")
            {
                sql += " ORDER BY " + orderSQL;
            }
            else
            {
                sql += " ORDER BY NVL(R.REGION_ID,'ZZ'), UPPER(S.FID) ";
            }

            return QueryData(sql);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }



    // -- 23/04/2562
    public DataTable SearchSiteOffshoreFID(String SiteID = "", String FID = "", String SiteName = "", String orderSQL = "", String OtherCriteria = "")
    {
        String sql = ""; String criteria = "";
        try
        {
            if (OtherCriteria != "") { criteria = OtherCriteria; }
            AddCriteria(ref criteria, "SITE_ID", SiteID, DBUTIL.FieldTypes.ftNumeric);
            AddCriteria(ref criteria, "UPPER(FID)", FID.ToUpper(), DBUTIL.FieldTypes.ftText);
            AddCriteria(ref criteria, "UPPER(SITE_NAME)", SiteName.ToUpper(), DBUTIL.FieldTypes.ftText);

            sql = "SELECT * FROM O_OFFSHORE_FID ";
            if (criteria != "") { sql += " WHERE " + criteria; }

            if (orderSQL != "")
            {
                sql += " ORDER BY " + orderSQL;
            }
            else
            {
                sql += " ORDER BY SITE_ID";
            }

            return QueryData(sql);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    //-- Add by Turk 18/04/2562 --> SearchOffshoreFID --
    //-- edit 21/06/2019 
    public DataTable SearchOffshoreFID(String SiteID = "", String FID = "", String SiteDesc = "", String Company = "", String orderSQL = "", String OtherCriteria = "")
    {
        String sql = ""; String criteria = "";
        try
        {
            if (OtherCriteria != "") { criteria = OtherCriteria; }
            AddCriteria(ref criteria, "SITE_ID", SiteID, DBUTIL.FieldTypes.ftNumeric);
            AddCriteria(ref criteria, "UPPER(FID)", FID.ToUpper(), DBUTIL.FieldTypes.ftText);
            AddCriteria(ref criteria, "UPPER(SITE_NAME)", SiteDesc.ToUpper(), DBUTIL.FieldTypes.ftText);
            AddCriteria(ref criteria, "UPPER(COMPANY)", Company.ToUpper(), DBUTIL.FieldTypes.ftText);

            sql = "SELECT * FROM O_OFFSHORE_FID ";
            if (criteria != "") { sql += " WHERE " + criteria; }

            if (orderSQL != "")
            {
                sql += " ORDER BY " + orderSQL;
            }
            else
            {
                sql += " ORDER BY SITE_ID ";
            }

            return QueryData(sql);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    //-- edit 06/07/2018 --
    //-- .2/09/2018 -- เพิ่ม OGC สำรอง
    //-- Add by Turk 18/04/2562 --> H2S_NAME1, H2S_NAME2, HG_NAME1, HG_NAME2, O2_NAME1, O2_NAME2, HC_NAME1, HC_NAME2 --
    //-- EDIT 28/06/2023 -- เพิ่ม ANLMET_ID
    public void MngSiteFID(int op, ref String SiteID, String FID = "", String SiteName = "", String RegionID = "", String alertFlag = "", String isoFlag = "", String h2sFlag = "", String OmaName1 = "", String OmaName2 = "", String flowName1 = "", String flowName2 = "", String totalRun = "", String toleranceRun = "", String OgcName1 = "", String OgcName2 = "", String OgcName3 = "", String H2sName1 = "", String H2sName2 = "", String HgName1 = "", String HgName2 = "", String O2Name1 = "", String O2Name2 = "", String HcName1 = "", String HcName2 = "", String anlmetID = "")
    {
        string SQL, SQL1, SQL2, Criteria = "";
        try
        {
            SQL = ""; SQL1 = ""; SQL2 = "";
            if (op != DBUTIL.opINSERT)
            {
                AddCriteria(ref Criteria, "SITE_ID", SiteID, DBUTIL.FieldTypes.ftNumeric);
            }
            if (op != DBUTIL.opDELETE)
            {
                if (op == DBUTIL.opINSERT)
                {
                    SiteID = GenerateID("O_SITE_FID", "SITE_ID").ToString();
                    AddSQL(op, ref SQL1, ref SQL2, "SITE_ID", SiteID, DBUTIL.FieldTypes.ftNumeric);
                }
                else
                {
                    op = DBUTIL.opUPDATE;
                }

                AddSQL2(op, ref SQL1, ref SQL2, "FID", FID, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "SITE_NAME", SiteName, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "REGION_ID", RegionID, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "ALERT_FLAG", alertFlag, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "ISO_FLAG", isoFlag , DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "H2S_FLAG", h2sFlag, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "OMA_NAME1", OmaName1, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "OMA_NAME2", OmaName2, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "FLOW_NAME1", flowName1, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "FLOW_NAME2", flowName2, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "TOTAL_RUN", totalRun , DBUTIL.FieldTypes.ftNumeric);
                AddSQL2(op, ref SQL1, ref SQL2, "TOLERANCE_RUN", toleranceRun, DBUTIL.FieldTypes.ftNumeric);
                AddSQL2(op, ref SQL1, ref SQL2, "OGC_NAME1", OgcName1, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "OGC_NAME2", OgcName2, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "OGC_NAME3", OgcName3, DBUTIL.FieldTypes.ftText);
                //-- Add by Turk 18/04/2562 --> H2S_NAME1, H2S_NAME2, HG_NAME1, HG_NAME2, O2_NAME1, O2_NAME2, HC_NAME1, HC_NAME2 --
                AddSQL2(op, ref SQL1, ref SQL2, "H2S_NAME1", H2sName1, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "H2S_NAME2", H2sName2, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "HG_NAME1", HgName1, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "HG_NAME2", HgName2, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "O2_NAME1", O2Name1, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "O2_NAME2", O2Name2, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "HC_NAME1", HcName1, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "HC_NAME2", HcName2, DBUTIL.FieldTypes.ftText);
                
                //-- EDIT 28/06/2023 --
                AddSQL2(op, ref SQL1, ref SQL2, "ANLMET_ID", anlmetID, DBUTIL.FieldTypes.ftNumeric);
                
            }

            if (op != DBUTIL.opINSERT && Criteria == "")
            {
                throw new Exception("Insufficient data!");
            }
            else
            {

                if (op == DBUTIL.opDELETE && SiteID != "")
                {
                    SQL = "DELETE FROM O_SITE_REPORT WHERE SITE_ID=" + SiteID;
                    ExecuteSQL(SQL);

                    SQL = "DELETE FROM O_SITE_SGC WHERE SITE_ID=" + SiteID;
                    ExecuteSQL(SQL);

                    //-- edit 07/10/2018 ----- ต้อง update ที่ GQMS_MAP_GCDS ด้วย
                    SQL = "DELETE FROM GQMS_MAP_GCDS  WHERE GQMS_FID = (SELECT FID FROM O_SITE_FID WHERE SITE_ID=" + SiteID +")";
                    ExecuteSQL(SQL);

                }

                SQL = CombineSQL(op, ref SQL1, ref SQL2, "O_SITE_FID", Criteria);
                ExecuteSQL(SQL);

            }


        }
        catch (Exception ex)
        {
            throw ex;
        }

    }

    //-- Add by Turk 18/04/2562 --> MngSiteOFFFID --
    //-- EDIT 21/06/2019 ---
    public void MngSiteOFFFID(int op, ref String SiteID, String FID = "", String SiteName = "", String Company = "")
    {
        string SQL, SQL1, SQL2, Criteria = "";
        try
        {
            SQL = ""; SQL1 = ""; SQL2 = "";
            if (op != DBUTIL.opINSERT)
            {
                AddCriteria(ref Criteria, "SITE_ID", SiteID, DBUTIL.FieldTypes.ftNumeric);
            }
            if (op != DBUTIL.opDELETE)
            {
                if (op == DBUTIL.opINSERT)
                {
                    SiteID = GenerateID("O_OFFSHORE_FID", "SITE_ID", "SITE_ID >= 1000", "1", 3).ToString();
                    AddSQL(op, ref SQL1, ref SQL2, "SITE_ID", SiteID, DBUTIL.FieldTypes.ftNumeric);
                }
                else
                {
                    op = DBUTIL.opUPDATE;
                }

                AddSQL2(op, ref SQL1, ref SQL2, "FID", FID, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "SITE_NAME", SiteName, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "COMPANY", Company, DBUTIL.FieldTypes.ftText);
            }

            if (op != DBUTIL.opINSERT && Criteria == "")
            {
                throw new Exception("Insufficient data!");
            }
            else
            {
                SQL = CombineSQL(op, ref SQL1, ref SQL2, "O_OFFSHORE_FID", Criteria);
                ExecuteSQL(SQL);

            }


        }
        catch (Exception ex)
        {
            throw ex;
        }

    }

    //-- edit 06/07/2018 --
    public DataTable SearchSiteReport(String RptID = "", String SiteID = "", String FID = "", String RptType = "", String orderSQL = "", String OtherCriteria = "")
    {
        String sql = ""; String criteria = "";
        try
        {
            if (OtherCriteria != "") { criteria = OtherCriteria; }
            AddCriteria(ref criteria, "RPT_ID", RptID, DBUTIL.FieldTypes.ftNumeric);
            AddCriteria(ref criteria, "SITE_ID", SiteID, DBUTIL.FieldTypes.ftNumeric);
            AddCriteria(ref criteria, "FID", FID, DBUTIL.FieldTypes.ftText);
            AddCriteria(ref criteria, "RPT_TYPE", RptType, DBUTIL.FieldTypes.ftText);

            sql = "SELECT * FROM O_SITE_REPORT ";
            if (criteria != "") { sql += " WHERE " + criteria; }

            if (orderSQL != "")
            {
                sql += " ORDER BY " + orderSQL;
            }
            else
            {
                sql += " ORDER BY RPT_ID";
            }

            return QueryData(sql);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    //-- edit 06/07/2018 --
    public void MngSiteReport(int op, ref String rptID, String SiteID="", String FID = "", String RptType = "", String NgbillRptNo = "", String GcRptNo = "", String GcRptName = "")
    {
        string SQL, SQL1, SQL2, Criteria = "";
        try
        {
            SQL = ""; SQL1 = ""; SQL2 = "";
            if (op != DBUTIL.opINSERT)
            {
                AddCriteria(ref Criteria, "RPT_ID", rptID, DBUTIL.FieldTypes.ftNumeric);
            }
            if (op != DBUTIL.opDELETE)
            {
                if (op == DBUTIL.opINSERT)
                {
                    rptID = GenerateID("O_SITE_REPORT", "RPT_ID").ToString();
                    AddSQL(op, ref SQL1, ref SQL2, "RPT_ID", rptID, DBUTIL.FieldTypes.ftNumeric);
                }
                else
                {
                    op = DBUTIL.opUPDATE;
                }

                AddSQL2(op, ref SQL1, ref SQL2, "SITE_ID", SiteID, DBUTIL.FieldTypes.ftNumeric);
                AddSQL2(op, ref SQL1, ref SQL2, "FID", FID, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "RPT_TYPE", RptType, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "NGBILL_RPT_NO", NgbillRptNo, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "GC_RPT_NO", GcRptNo, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "GC_RPT_NAME", GcRptName, DBUTIL.FieldTypes.ftText);

            }

            if (op != DBUTIL.opINSERT && Criteria == "")
            {
                throw new Exception("Insufficient data!");
            }
            else
            {

                //-- edit 09/11/2018 ----- ต้อง update ที่ GQMS_MAP_GCDS ด้วย
                string SQLg = "";
                if (rptID != "" && op == DBUTIL.opDELETE)
                {
                    SQLg = " DELETE FROM GQMS_MAP_GCDS " +
                     " WHERE ID = (SELECT T.ID FROM GQMS_MAP_GCDS T INNER JOIN O_SITE_REPORT S  " +
                     "             ON T.GQMS_FID=S.FID AND T.REPORT_TYPE=(CASE RPT_TYPE WHEN 'DAILY' THEN 1 ELSE 2 END) " +
                     "             WHERE  S.RPT_TYPE IN ('DAILY','27DAY') AND S.RPT_ID=" + rptID + ") ";
                    ExecuteSQL(SQLg);
                }

                SQL = CombineSQL(op, ref SQL1, ref SQL2, "O_SITE_REPORT", Criteria);
                ExecuteSQL(SQL);

                if (rptID != "" && (op == DBUTIL.opINSERT || op == DBUTIL.opUPDATE) && NgbillRptNo != "" && (RptType == "DAILY" || RptType == "27DAY"))
                {
                    //-- EDIT 20/04/2021 -- เนื่องจากบางครั้งมีการเปลี่ยนชื่อ FID แต่เป็น FID เดียวกัน จึงทำให้เกิดรายการซ้ำ
                    //-- ให้ลบ REPORT_NUMBER เก่าก่อน --
                    SQLg = " DELETE FROM GQMS_MAP_GCDS WHERE REPORT_NUMBER='" + NgbillRptNo+ "' AND GQMS_FID<>'" + FID+"' ";
                    ExecuteSQL(SQLg);


                    //NgbillRptNo  ม RptType

                    SQLg = "MERGE INTO GQMS_MAP_GCDS T " +
                     "    USING (SELECT RPT_ID, NGBILL_RPT_NO, FID,CASE RPT_TYPE WHEN 'DAILY' THEN 1 ELSE 2 END REPORT_TYPE " +
                     "       ,CREATED_BY, TO_CHAR(CREATED_DATE,'MM/DD/YYYY HH:MI:SS AM') CREATED_DATE, MODIFIED_BY, TO_CHAR(MODIFIED_DATE,'MM/DD/YYYY HH:MI:SS AM') MODIFIED_DATE " +
                     "       , GC_RPT_NO,GC_RPT_NAME,CASE RPT_TYPE WHEN 'DAILY' THEN 1 ELSE 0 END AVR_TYPE, 0 COMP_TYPE " +
                     "        FROM O_SITE_REPORT WHERE RPT_TYPE IN ('DAILY','27DAY') AND RPT_ID=" + rptID + ") S " +
                     "    ON (T.GQMS_FID=S.FID AND T.REPORT_TYPE=S.REPORT_TYPE)  " +
                     "  WHEN MATCHED THEN  " +
                     "    UPDATE SET T.REPORT_NUMBER=S.NGBILL_RPT_NO, T.MODIFIED_BY=S.MODIFIED_BY, T.MODIFIED_DATE=S.MODIFIED_DATE ,T.GC_REPORT_ID=S.GC_RPT_NO,T.GC_REPORT_DES=S.GC_RPT_NAME  " +
                     " WHEN NOT MATCHED THEN  " +
                     "  INSERT ( ID, REPORT_NUMBER, GQMS_FID, REPORT_TYPE, CREATED_BY, CREATED_DATE, MODIFIED_BY, MODIFIED_DATE, GC_REPORT_ID, GC_REPORT_DES, AVR_TYPE, COMP_TYPE)  " +
                     "     VALUES ( (SELECT MAX(ID)+1 FROM GQMS_MAP_GCDS) ,S.NGBILL_RPT_NO, S.FID,S.REPORT_TYPE,S.CREATED_BY, S.CREATED_DATE, S.MODIFIED_BY, S.MODIFIED_DATE  " +
                     "                  ,S.GC_RPT_NO,S.GC_RPT_NAME, S.AVR_TYPE, S.COMP_TYPE ) ";

                    ExecuteSQL(SQLg);
                }


                }


        }
        catch (Exception ex)
        {
            throw ex;
        }

    }


    //-- edit 16/07/2018 --
    public String GetNgbillRptNo(String SiteID = "", String FID = "", String RptType = "")
    {
        String sql = ""; String criteria = "";
        DataTable DT = null;
        String result = "";
        try
        {

            AddCriteria(ref criteria, "SITE_ID", SiteID, DBUTIL.FieldTypes.ftNumeric);
            AddCriteria(ref criteria, "FID", FID, DBUTIL.FieldTypes.ftText);
            AddCriteria(ref criteria, "RPT_TYPE", RptType, DBUTIL.FieldTypes.ftText);

            sql = "SELECT * FROM O_SITE_REPORT ";
            if (criteria != "") { sql += " WHERE " + criteria; }

            DT = QueryData(sql);
            if (DT.Rows.Count > 0) result = Utility.ToString(DT.Rows[0]["NGBILL_RPT_NO"]);

            return result;

        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            Utility.ClearObject(ref DT);
        }
    }



    //-- edit 11/07/2018 --
    //-- EDIT 19/07/2019 --
    //-- EDIT 10/09/2019 -- เพิ่ม stdID
    public DataTable SearchSiteSgc( String SiteID = "", String StdID = "", String orderSQL = "", String OtherCriteria = "")
    {
        String sql = ""; String criteria = "";
        try
        {
            if (OtherCriteria != "") { criteria = OtherCriteria; }
            AddCriteria(ref criteria, "S.SITE_ID", SiteID, DBUTIL.FieldTypes.ftNumeric);
            AddCriteria(ref criteria, "S.STD_ID", StdID, DBUTIL.FieldTypes.ftNumeric);

            sql = "SELECT S.* , I.C1_MIN, I.C2_MIN, I.C3_MIN, I.IC4_MIN, I.NC4_MIN, I.IC5_MIN, I.NC5_MIN, I.C6_MIN, I.N2_MIN, I.CO2_MIN, I.H2S_MIN, I.HG_MIN " +
            ", I.C1_MAX, I.C2_MAX, I.C3_MAX, I.IC4_MAX, I.NC4_MAX, I.IC5_MAX, I.NC5_MAX, I.C6_MAX, I.N2_MAX, I.CO2_MAX, I.H2S_MAX, I.HG_MAX " +
            "FROM O_SITE_SGC S LEFT OUTER JOIN O_SITE_TISI I ON S.SITE_ID = I.SITE_ID AND S.STD_ID=I.STD_ID ";

            if (criteria != "") { sql += " WHERE " + criteria; }

            if (orderSQL != "")
            {
                sql += " ORDER BY " + orderSQL;
            }
            else
            {
                sql += " ORDER BY S.SITE_ID, S.ORDER_DATE DESC";
            }

            return QueryData(sql);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    //-- edit 11/07/2018 --
    //-- edit 19/09/2019 -- เพิ่ม STD_ID
    public void MngSiteSgc(int op, String SiteID , ref String StdID , String Cylinder = null, String OrderDate = null, String ExpireDate = null, String C1 = null, String C2 = null, String C3 = null, String IC4 = null, String NC4 = null, String IC5 = null, String NC5 = null, 
                          String C6 = null, String N2 = null, String CO2 = null, String H2S = null, String HG = null)
    {
        string SQL, SQL1, SQL2, Criteria = "";
        try
        {
            SQL = ""; SQL1 = ""; SQL2 = "";
            if (op != DBUTIL.opINSERT)
            {
                AddCriteria(ref Criteria, "SITE_ID", SiteID, DBUTIL.FieldTypes.ftNumeric);
                AddCriteria(ref Criteria, "STD_ID", StdID, DBUTIL.FieldTypes.ftNumeric);
            }
            if (op != DBUTIL.opDELETE)
            {
                if (op == DBUTIL.opINSERT)
                {
                    AddSQL(op, ref SQL1, ref SQL2, "SITE_ID", SiteID, DBUTIL.FieldTypes.ftNumeric);

                    StdID = GenerateID("O_SITE_SGC", "STD_ID", "SITE_ID=" + SiteID).ToString();
                    AddSQL(op, ref SQL1, ref SQL2, "STD_ID", StdID, DBUTIL.FieldTypes.ftNumeric);
                }
                else
                {
                    op = DBUTIL.opUPDATE;
                }

                
                AddSQL2(op, ref SQL1, ref SQL2, "CYLINDER_NO", Cylinder, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "ORDER_DATE", Utility.AppDateValue(OrderDate), DBUTIL.FieldTypes.ftDate);
                AddSQL2(op, ref SQL1, ref SQL2, "EXPIRE_DATE", Utility.AppDateValue(ExpireDate), DBUTIL.FieldTypes.ftDate);
                AddSQL2(op, ref SQL1, ref SQL2, "C1", C1, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "C2", C2, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "C3", C3, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "IC4", IC4, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "NC4", NC4, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "IC5", IC5, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "NC5", NC5, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "C6", C6, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "N2", N2, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "CO2", CO2, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "H2S", H2S, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "HG", HG, DBUTIL.FieldTypes.ftText);

            }

            if (op != DBUTIL.opINSERT && Criteria == "")
            {
                throw new Exception("Insufficient data!");
            }
            else
            {
                SQL = CombineSQL(op, ref SQL1, ref SQL2, "O_SITE_SGC", Criteria);
                ExecuteSQL(SQL);
            }


        }
        catch (Exception ex)
        {
            throw ex;
        }

    }


    //-- edit 20/06/2019 --
    //-- EDIT 10/09/2019 -- เพิ่ม stdID
    public DataTable SearchSiteTisi(String SiteID = "", String StdID = "", String orderSQL = "", String OtherCriteria = "")
    {
        String sql = ""; String criteria = "";
        try
        {
            if (OtherCriteria != "") { criteria = OtherCriteria; }
            AddCriteria(ref criteria, "SITE_ID", SiteID, DBUTIL.FieldTypes.ftNumeric);
            AddCriteria(ref criteria, "STD_ID", StdID, DBUTIL.FieldTypes.ftNumeric);

            sql = "SELECT * FROM O_SITE_TISI ";
            if (criteria != "") { sql += " WHERE " + criteria; }

            if (orderSQL != "")
            {
                sql += " ORDER BY " + orderSQL;
            }
            else
            {
                sql += " ORDER BY SITE_ID, STD_ID DESC";
            }

            return QueryData(sql);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    //-- Add by Turk 18/04/2562 --> MngSiteTisi --
    //-- edit 19/09/2019 -- เพิ่ม StdID
    public void MngSiteTisi(int op, String SiteID, String StdID, String C1Min = "", String C2Min = "", String C3Min = "", String IC4Min = "", String NC4Min = "", String IC5Min = "", String NC5Min = "", String C6Min = "", String N2Min = "",
                            String CO2Min = "", String H2SMin = "", String HGMin = "", String C1Max = "", String C2Max = "", String C3Max = "", String IC4Max = "", String NC4Max = "", String IC5Max = "",
                            String NC5Max = "", String C6Max = "", String N2Max = "", String CO2Max = "", String H2SMax = "", String HGMax = "")
    {
        string SQL, SQL1, SQL2, Criteria = "";
        try
        {
            SQL = ""; SQL1 = ""; SQL2 = "";
            if (op != DBUTIL.opINSERT)
            {
                AddCriteria(ref Criteria, "SITE_ID", SiteID, DBUTIL.FieldTypes.ftNumeric);
                AddCriteria(ref Criteria, "STD_ID", StdID, DBUTIL.FieldTypes.ftNumeric);
            }
            if (op != DBUTIL.opDELETE)
            {
                if (op == DBUTIL.opINSERT)
                {
                    AddSQL(op, ref SQL1, ref SQL2, "SITE_ID", SiteID, DBUTIL.FieldTypes.ftNumeric);
                    AddSQL(op, ref SQL1, ref SQL2, "STD_ID", StdID, DBUTIL.FieldTypes.ftNumeric);
                }
                else
                {
                    op = DBUTIL.opUPDATE;
                }
                AddSQL2(op, ref SQL1, ref SQL2, "C1_MIN", C1Min, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "C2_MIN", C2Min, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "C3_MIN", C3Min, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "IC4_MIN", IC4Min, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "NC4_MIN", NC4Min, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "IC5_MIN", IC5Min, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "NC5_MIN", NC5Min, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "C6_MIN", C6Min, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "N2_MIN", N2Min, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "CO2_MIN", CO2Min, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "H2S_MIN", H2SMin, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "HG_MIN", HGMin, DBUTIL.FieldTypes.ftText);

                AddSQL2(op, ref SQL1, ref SQL2, "C1_MAX", C1Max, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "C2_MAX", C2Max, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "C3_MAX", C3Max, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "IC4_MAX", IC4Max, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "NC4_MAX", NC4Max, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "IC5_MAX", IC5Max, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "NC5_MAX", NC5Max, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "C6_MAX", C6Max, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "N2_MAX", N2Max, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "CO2_MAX", CO2Max, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "H2S_MAX", H2SMax, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "HG_MAX", HGMax, DBUTIL.FieldTypes.ftText);

            }

            if (op != DBUTIL.opINSERT && Criteria == "")
            {
                throw new Exception("Insufficient data!");
            }
            else
            {
                SQL = CombineSQL(op, ref SQL1, ref SQL2, "O_SITE_TISI", Criteria);
                ExecuteSQL(SQL);
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }

    }




    //-- Add by Turk 18/04/2562 --> SearchDimH2S --
    //-- edit 21/06/2019 --
    public DataTable SearchDimH2S(String Name = "", String orderSQL = "", String OtherCriteria = "")
    {
        String sql = ""; String criteria = "";
        try
        {
            if (OtherCriteria != "") { criteria = OtherCriteria; }
            AddCriteria(ref criteria, "UPPER(H2S_NAME)", Name.ToUpper(), DBUTIL.FieldTypes.ftText);

            sql = "SELECT H2S_NAME, H2S_NAME AS NAME FROM O_DIM_H2S ";
            if (criteria != "") { sql += " WHERE " + criteria; }


            if (orderSQL != "")
            {
                sql += " ORDER BY " + orderSQL;
            }
            else
            {
                sql += " ORDER BY UPPER(H2S_NAME) ";
            }

            return QueryData(sql);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    //-- Add by Turk 18/04/2562 --> SearchDimHG --
    //-- edit 21/06/2019 --
    public DataTable SearchDimHG(String Name = "", String orderSQL = "", String OtherCriteria = "")
    {
        String sql = ""; String criteria = "";
        try
        {
            if (OtherCriteria != "") { criteria = OtherCriteria; }
            AddCriteria(ref criteria, "UPPER(HG_NAME)", Name.ToUpper(), DBUTIL.FieldTypes.ftText);

            sql = "SELECT HG_NAME, HG_NAME AS NAME FROM O_DIM_HG ";
            if (criteria != "") { sql += " WHERE " + criteria; }


            if (orderSQL != "")
            {
                sql += " ORDER BY " + orderSQL;
            }
            else
            {
                sql += " ORDER BY UPPER(HG_NAME) ";
            }

            return QueryData(sql);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    //-- Add by Turk 18/04/2562 --> SearchDimO2 --
    //-- EDIT 21/06/2019 --
    public DataTable SearchDimO2(String Name = "", String orderSQL = "", String OtherCriteria = "")
    {
        String sql = ""; String criteria = "";
        try
        {
            if (OtherCriteria != "") { criteria = OtherCriteria; }
            AddCriteria(ref criteria, "UPPER(O2_NAME)", Name.ToUpper(), DBUTIL.FieldTypes.ftText);

            sql = "SELECT O2_NAME, O2_NAME AS NAME FROM O_DIM_O2 ";
            if (criteria != "") { sql += " WHERE " + criteria; }


            if (orderSQL != "")
            {
                sql += " ORDER BY " + orderSQL;
            }
            else
            {
                sql += " ORDER BY UPPER(O2_NAME) ";
            }

            return QueryData(sql);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    //-- Add by Turk 18/04/2562 --> SearchDimHC --
    //-- EDIT 21/06/2019
    public DataTable SearchDimHC(String Name = "", String orderSQL = "", String OtherCriteria = "")
    {
        String sql = ""; String criteria = "";
        try
        {
            if (OtherCriteria != "") { criteria = OtherCriteria; }
            AddCriteria(ref criteria, "UPPER(HC_NAME)", Name.ToUpper(), DBUTIL.FieldTypes.ftText);

            sql = "SELECT HC_NAME, HC_NAME AS NAME FROM O_DIM_HC ";
            if (criteria != "") { sql += " WHERE " + criteria; }


            if (orderSQL != "")
            {
                sql += " ORDER BY " + orderSQL;
            }
            else
            {
                sql += " ORDER BY UPPER(HC_NAME) ";
            }

            return QueryData(sql);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


 
    //-- edit 26/06/2019 --
    public DataTable SearchDimSamplingPoint(String Name = "", String orderSQL = "", String OtherCriteria = "")
    {
        String sql = ""; String criteria = "";
        try
        {
            if (OtherCriteria != "") { criteria = OtherCriteria; }
            AddCriteria(ref criteria, "UPPER(FID)", Name.ToUpper(), DBUTIL.FieldTypes.ftText);

 
            sql = "SELECT DISTINCT FID, replace(replace(FID,'#',''),'\"','') AFID FROM  " +
                      " (SELECT H2S_NAME AS FID FROM O_DIM_H2S  " +
                      " UNION SELECT HC_NAME AS FID FROM O_DIM_HC  " +
                      " UNION SELECT HG_NAME AS FID FROM O_DIM_HG  " +
                      " UNION SELECT O2_NAME AS FID FROM O_DIM_O2) A  ";
                      

            if (criteria != "") { sql += " WHERE " + criteria; }


            if (orderSQL != "")
            {
                sql += " ORDER BY " + orderSQL;
            }
            else
            {
                sql += " ORDER BY UPPER(FID) ";
            }

            return QueryData(sql);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    //-- Add by Turk 18/04/2562 --> MngDimH2S --

    public void MngDimH2S(int op, string Name, string OldName = "")
    {
        string SQL, SQL1, SQL2, Criteria = "";
        try
        {
            SQL = ""; SQL1 = ""; SQL2 = "";
            if (op != DBUTIL.opINSERT)
            {
                Project.dal.AddCriteria(ref Criteria, "H2S_NAME", OldName, DBUTIL.FieldTypes.ftText);
            }
            if (op != DBUTIL.opDELETE)
            {
                if (op == DBUTIL.opINSERT)
                {
                    Project.dal.AddSQL(op, ref SQL1, ref SQL2, "H2S_NAME", Name, DBUTIL.FieldTypes.ftText);
                }
                else
                {
                    Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "H2S_NAME", Name, DBUTIL.FieldTypes.ftText);
                }
                
            }

            if (op != DBUTIL.opINSERT && Criteria == "")
            {
                throw new Exception("Insufficient data!");
            }
            else
            {
                SQL = CombineSQL(op, ref SQL1, ref SQL2, "O_DIM_H2S", Criteria);
                ExecuteSQL(SQL);
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    //-- Add by Turk 18/04/2562 --> MngDimHG --
    //-- edit 21/06/2019
    public void MngDimHG(int op, string Name, string OldName = "")
    {
        string SQL, SQL1, SQL2, Criteria = "";
        try
        {
            SQL = ""; SQL1 = ""; SQL2 = "";
            if (op != DBUTIL.opINSERT)
            {
                Project.dal.AddCriteria(ref Criteria, "HG_NAME", OldName, DBUTIL.FieldTypes.ftText);
            }
            if (op != DBUTIL.opDELETE)
            {
                if (op == DBUTIL.opINSERT)
                {
                    Project.dal.AddSQL(op, ref SQL1, ref SQL2, "HG_NAME", Name, DBUTIL.FieldTypes.ftText);
                }
                else
                {
                    Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "HG_NAME", Name, DBUTIL.FieldTypes.ftText);
                }
                
            }

            if (op != DBUTIL.opINSERT && Criteria == "")
            {
                throw new Exception("Insufficient data!");
            }
            else
            {
                SQL = CombineSQL(op, ref SQL1, ref SQL2, "O_DIM_HG", Criteria);
                ExecuteSQL(SQL);
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    //-- Add by Turk 18/04/2562 --> MngDimO2 --
    //-- edit 21/06/2019
    public void MngDimO2(int op, string Name, string OldName = "")
    {
        string SQL, SQL1, SQL2, Criteria = "";
        try
        {
            SQL = ""; SQL1 = ""; SQL2 = "";
            if (op != DBUTIL.opINSERT)
            {
                Project.dal.AddCriteria(ref Criteria, "O2_NAME", OldName, DBUTIL.FieldTypes.ftText);
            }
            if (op != DBUTIL.opDELETE)
            {
                if (op == DBUTIL.opINSERT)
                {
                    Project.dal.AddSQL(op, ref SQL1, ref SQL2, "O2_NAME", Name, DBUTIL.FieldTypes.ftText);
                }
                else
                {
                    Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "O2_NAME", Name, DBUTIL.FieldTypes.ftText);
                }
               
            }

            if (op != DBUTIL.opINSERT && Criteria == "")
            {
                throw new Exception("Insufficient data!");
            }
            else
            {
                SQL = CombineSQL(op, ref SQL1, ref SQL2, "O_DIM_O2", Criteria);
                ExecuteSQL(SQL);
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    //-- Add by Turk 18/04/2562 --> MngDimHC --
    //-- edit 21/06/2019 
    public void MngDimHC(int op, string Name, string OldName = "")
    {
        string SQL, SQL1, SQL2, Criteria = "";
        try
        {
            SQL = ""; SQL1 = ""; SQL2 = "";
            if (op != DBUTIL.opINSERT)
            {
                Project.dal.AddCriteria(ref Criteria, "HC_NAME", OldName, DBUTIL.FieldTypes.ftText);
            }
            if (op != DBUTIL.opDELETE)
            {
                if (op == DBUTIL.opINSERT)
                {
                    Project.dal.AddSQL(op, ref SQL1, ref SQL2, "HC_NAME", Name, DBUTIL.FieldTypes.ftText);
                }
                else
                {
                    Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "HC_NAME", Name, DBUTIL.FieldTypes.ftText);
                }
               
            }

            if (op != DBUTIL.opINSERT && Criteria == "")
            {
                throw new Exception("Insufficient data!");
            }
            else
            {
                SQL = CombineSQL(op, ref SQL1, ref SQL2, "O_DIM_HC", Criteria);
                ExecuteSQL(SQL);
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }



    //-- EDIT 19/07/2019 --  
    public DataTable SearchSpotFID( String FID = "", String rDate = "",  String orderSQL = "", String OtherCriteria = "")
    {
        String sql = ""; String criteria = "";
        try
        {
            if (OtherCriteria != "") { criteria = OtherCriteria; }
            AddCriteria(ref criteria, "UPPER(S.FID)", FID.ToUpper(), DBUTIL.FieldTypes.ftText);
            AddCriteria(ref criteria, "RDATE", Utility.AppDateValue(rDate), DBUTIL.FieldTypes.ftDate);
 

            //sql = "SELECT * FROM O_SPOT_UPDATE ";

            sql = "SELECT S.*, HS.SDATE AS H2S_DATE, HS.H2S, HG.SDATE AS HG_DATE, HG.HG, O2.SDATE AS O2_DATE, O2.O2, HC.SDATE AS HC_DATE, HC.HC " +
            " FROM O_SPOT_UPDATE S LEFT OUTER JOIN O_OGC_H2S HS ON S.H2S_NAME = HS.H2S_NAME AND TO_CHAR(S.RDATE,'MMYYYY')= TO_CHAR(HS.SDATE, 'MMYYYY') " +
            " LEFT OUTER JOIN O_OGC_HG HG ON S.HG_NAME = HG.HG_NAME AND TO_CHAR(S.RDATE,'MMYYYY')= TO_CHAR(HG.SDATE, 'MMYYYY') " +
            " LEFT OUTER JOIN O_OGC_O2 O2 ON S.O2_NAME = O2.O2_NAME AND TO_CHAR(S.RDATE,'MMYYYY')= TO_CHAR(O2.SDATE, 'MMYYYY') " +
            " LEFT OUTER JOIN O_OGC_HC HC ON S.HC_NAME = HC.HC_NAME AND TO_CHAR(S.RDATE,'MMYYYY')= TO_CHAR(HC.SDATE, 'MMYYYY')";

            if (criteria != "") { sql += " WHERE " + criteria; }

            if (orderSQL != "")
            {
                sql += " ORDER BY " + orderSQL;
            }
            else
            {
                sql += " ORDER BY UPPER(S.FID), RDATE ";
            }

            return QueryData(sql);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    //-- edit 19/07/2019 
    public void MngOSpotUpdate(int op, string FID, string rDate, string H2Sname = null, string HGname = null, string O2name = null, string HCname = null)
    {
        string SQL, SQL1, SQL2, Criteria = "";
        try
        {
            SQL = ""; SQL1 = ""; SQL2 = "";
            if (op != DBUTIL.opINSERT)
            {
                Project.dal.AddCriteria(ref Criteria, "FID", FID, DBUTIL.FieldTypes.ftText);
                Project.dal.AddCriteria(ref Criteria, "RDATE", Utility.AppDateValue(rDate), DBUTIL.FieldTypes.ftDate);
            }
            if (op != DBUTIL.opDELETE)
            {
                if (op == DBUTIL.opINSERT)
                {
                    Project.dal.AddSQL(op, ref SQL1, ref SQL2, "FID", FID, DBUTIL.FieldTypes.ftText);
                    Project.dal.AddSQL(op, ref SQL1, ref SQL2, "RDATE", Utility.AppDateValue(rDate), DBUTIL.FieldTypes.ftDate);
                }

                Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "H2S_NAME", H2Sname, DBUTIL.FieldTypes.ftText);
                Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "HG_NAME", HGname, DBUTIL.FieldTypes.ftText);
                Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "O2_NAME", O2name, DBUTIL.FieldTypes.ftText);
                Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "HC_NAME", HCname, DBUTIL.FieldTypes.ftText);

            }

            if (op != DBUTIL.opINSERT && Criteria == "")
            {
                throw new Exception("Insufficient data!");
            }
            else
            {
                SQL = CombineSQL(op, ref SQL1, ref SQL2, "O_SPOT_UPDATE", Criteria);
                ExecuteSQL(SQL);
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }



    //-- edit 26/06/2023 --
    public DataTable SearchDimAnalysisMethod(String anlmetID = "", String anlmetName = "", String orderSQL = "", String OtherCriteria = "")
    {
        String sql = ""; String criteria = "";
        try
        {
            if (OtherCriteria != "") { criteria = OtherCriteria; }
            AddCriteria(ref criteria, "ANLMET_ID", anlmetID, DBUTIL.FieldTypes.ftNumeric);
            AddCriteria(ref criteria, "ANLMET_NAME", anlmetName, DBUTIL.FieldTypes.ftText);

            sql = "SELECT * FROM O_DIM_ANALYSIS_METHOD ";
            if (criteria != "") { sql += " WHERE " + criteria; }


            if (orderSQL != "")
            {
                sql += " ORDER BY " + orderSQL;
            }
            else
            {
                sql += " ORDER BY ANLMET_ID";
            }

            return QueryData(sql);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    //-- edit 26/06/2023 ---   
    public void MngDimAnalysisMethod(int op, ref string anlmetID, string anlmetName)
    {
        string SQL, SQL1, SQL2, Criteria = "";
        try
        {
            SQL = ""; SQL1 = ""; SQL2 = "";
            if (op != DBUTIL.opINSERT)
            {
                Project.dal.AddCriteria(ref Criteria, "ANLMET_ID", anlmetID, DBUTIL.FieldTypes.ftNumeric);
            }
            if (op != DBUTIL.opDELETE)
            {
                if (op == DBUTIL.opINSERT)
                {
                    anlmetID = Utility.ToString(GenerateID("O_DIM_ANALYSIS_METHOD", "ANLMET_ID"));
                    Project.dal.AddSQL(op, ref SQL1, ref SQL2, "ANLMET_ID", anlmetID, DBUTIL.FieldTypes.ftNumeric);

                }
                else
                {
                    op = DBUTIL.opUPDATE;
                }

                Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "ANLMET_NAME", Utility.ToString(anlmetName).Trim(), DBUTIL.FieldTypes.ftText);
 
            }

            if (op != DBUTIL.opINSERT && Criteria == "")
            {
                throw new Exception("Insufficient data!");
            }
            else
            {
                SQL = Project.dal.CombineSQL(op, ref SQL1, ref SQL2, "O_DIM_ANALYSIS_METHOD", Criteria, timeStamp: true);
                ExecuteSQL(SQL);
            }


        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    //-- edit 26/06/2023 --
    public DataTable SearchDimAnalysisItem(String anlmetID = "", String seqNo = "", String orderSQL = "", String OtherCriteria = "")
    {
        String sql = ""; String criteria = "";
        try
        {
            if (OtherCriteria != "") { criteria = OtherCriteria; }
            AddCriteria(ref criteria, "ANLMET_ID", anlmetID, DBUTIL.FieldTypes.ftNumeric);
            AddCriteria(ref criteria, "SEQ_NO", seqNo, DBUTIL.FieldTypes.ftNumeric);

            sql = "SELECT * FROM O_DIM_ANALYSIS_ITEM ";
            if (criteria != "") { sql += " WHERE " + criteria; }


            if (orderSQL != "")
            {
                sql += " ORDER BY " + orderSQL;
            }
            else
            {
                sql += " ORDER BY ANLMET_ID, SEQ_NO";
            }

            return QueryData(sql);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }




    //-- edit 26/06/2023 ---   
    public void MngDimAnalysisItem(int op, string anlmetID, string seqNo, string seqNoEdit, string stdHead, string stdRef)
    {
        string SQL, SQL1, SQL2, Criteria = "";
        try
        {
            SQL = ""; SQL1 = ""; SQL2 = "";
            if (op != DBUTIL.opINSERT)
            {
                Project.dal.AddCriteria(ref Criteria, "ANLMET_ID", anlmetID, DBUTIL.FieldTypes.ftNumeric);
                Project.dal.AddCriteria(ref Criteria, "SEQ_NO", seqNo, DBUTIL.FieldTypes.ftNumeric);
            }
            if (op != DBUTIL.opDELETE)
            {
                if (op == DBUTIL.opINSERT)
                {
                    Project.dal.AddSQL(op, ref SQL1, ref SQL2, "ANLMET_ID", anlmetID, DBUTIL.FieldTypes.ftNumeric);
                   
                }
                else
                {
                    op = DBUTIL.opUPDATE;
                }

                Project.dal.AddSQL(op, ref SQL1, ref SQL2, "SEQ_NO", seqNoEdit, DBUTIL.FieldTypes.ftNumeric);
                Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "STD_HEAD", Utility.ToString(stdHead).Trim(), DBUTIL.FieldTypes.ftText);
                Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "STD_REF", Utility.ToString(stdRef).Trim(), DBUTIL.FieldTypes.ftText);

            }

            if (op != DBUTIL.opINSERT && Criteria == "")
            {
                throw new Exception("Insufficient data!");
            }
            else
            {
                SQL = Project.dal.CombineSQL(op, ref SQL1, ref SQL2, "O_DIM_ANALYSIS_ITEM", Criteria, timeStamp: false);
                ExecuteSQL(SQL);
            }


        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    //-- edit 04/08/2023 -- ดึงข้อมุล customer จาก gis จึงมี structure เหมือน gis.pipeline.station_location 
    public DataTable SearchCustomer(String custID = "", String custShort = "", String custName = "", String region = "", String orderSQL = "", String OtherCriteria = "")
    {
        String sql = ""; String criteria = "";
        try
        {
            if (OtherCriteria != "") { criteria = OtherCriteria; }
            AddCriteria(ref criteria, "PERMANENT_CODE", custID, DBUTIL.FieldTypes.ftText);
            AddCriteria(ref criteria, "NAME_ABBR", custShort, DBUTIL.FieldTypes.ftText);
            if (custName != "")
            {
                if (criteria != "") criteria += " AND ";
                criteria += "( UPPER(NAME_ABBR) LIKE '" +  custName.ToUpper() + "' OR UPPER(NAME_FULL) LIKE '" + custName.ToUpper() + "' )";
            }
            AddCriteria(ref criteria, "REGION", region , DBUTIL.FieldTypes.ftText);

            sql = "SELECT * FROM O_CUSTOMER ";
            if (criteria != "") { sql += " WHERE " + criteria; }


            if (orderSQL != "")
            {
                sql += " ORDER BY " + orderSQL;
            }
            else
            {
                sql += " ORDER BY PERMANENT_CODE, NAME_ABBR";
            }

            return QueryData(sql);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    //-- edit 03/07/2023 --
    public void MngCustomerGIS(int op, string custID, string custShort = "", string custTname = "", string region = "", string custType = "", string custSubType = "", string bvZone = "", string bvValve = "")
    {
        string SQL, SQL1, SQL2, Criteria = "";
        try
        {
            SQL = ""; SQL1 = ""; SQL2 = "";
            if (op != DBUTIL.opINSERT)
            {
                Project.dal.AddCriteria(ref Criteria, "PERMANENT_CODE", custID, DBUTIL.FieldTypes.ftText);
            }
            if (op != DBUTIL.opDELETE)
            {
                if (op == DBUTIL.opINSERT)
                {
                    Project.dal.AddSQL(op, ref SQL1, ref SQL2, "PERMANENT_CODE", custID, DBUTIL.FieldTypes.ftText);

                }
                else
                {
                    op = DBUTIL.opUPDATE;
                }

                Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "NAME_ABBR", Utility.ToString(custShort).Trim(), DBUTIL.FieldTypes.ftText);
                Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "NAME_FULL", Utility.ToString(custTname).Trim(), DBUTIL.FieldTypes.ftText);
                Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "REGION", Utility.ToString(region).Trim(), DBUTIL.FieldTypes.ftText);
                Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "TYPE", Utility.ToString(custType).Trim(), DBUTIL.FieldTypes.ftText);
                Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "SUB_TYPE", Utility.ToString(custSubType).Trim(), DBUTIL.FieldTypes.ftText);
                Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "BV_ZONE", Utility.ToString(bvZone).Trim(), DBUTIL.FieldTypes.ftText);
                Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "BV_VALVE", Utility.ToString(bvValve).Trim(), DBUTIL.FieldTypes.ftText);

            }

            if (op != DBUTIL.opINSERT && Criteria == "")
            {
                throw new Exception("Insufficient data!");
            }
            else
            {
                SQL = Project.dal.CombineSQL(op, ref SQL1, ref SQL2, "O_CUSTOMER", Criteria, timeStamp: false);
                ExecuteSQL(SQL);
            }


        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    //-- edit 03/07/2023 --
    public void MngCustomerGQMS(int op, string custID, string regionId = "", string QualityMain = "", string QualitySupport1 = "", string QualitySupport2 = "", string omaMain = "", string omaSupport1 = "", string h2s = "", string hg = "", string remark = "")
    {
        string SQL, SQL1, SQL2, Criteria = "";
        try
        {
            SQL = ""; SQL1 = ""; SQL2 = "";
            if (op != DBUTIL.opINSERT)
            {
                Project.dal.AddCriteria(ref Criteria, "PERMANENT_CODE", custID, DBUTIL.FieldTypes.ftText);
            }
            if (op != DBUTIL.opDELETE)
            {
                if (op == DBUTIL.opINSERT)
                {
                    Project.dal.AddSQL(op, ref SQL1, ref SQL2, "PERMANENT_CODE", custID, DBUTIL.FieldTypes.ftText);

                }
                else
                {
                    op = DBUTIL.opUPDATE;
                }

          
                Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "QUALITY_MAIN", Utility.ToString(QualityMain).Trim(), DBUTIL.FieldTypes.ftText);
                Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "QUALITY_SUPPORT1", Utility.ToString(QualitySupport1).Trim(), DBUTIL.FieldTypes.ftText);
                Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "QUALITY_SUPPORT2", Utility.ToString(QualitySupport2).Trim(), DBUTIL.FieldTypes.ftText);
                Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "OMA_MAIN", Utility.ToString(omaMain).Trim(), DBUTIL.FieldTypes.ftText);
                Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "OMA_SUPPORT1", Utility.ToString(omaSupport1).Trim(), DBUTIL.FieldTypes.ftText);
                Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "H2S", Utility.ToString(h2s).Trim(), DBUTIL.FieldTypes.ftText);
                Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "HG", Utility.ToString(hg).Trim(), DBUTIL.FieldTypes.ftText);
                Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "REMARK", Utility.ToString(remark).Trim(), DBUTIL.FieldTypes.ftText);

            }

            if (op != DBUTIL.opINSERT && Criteria == "")
            {
                throw new Exception("Insufficient data!");
            }
            else
            {
                SQL = Project.dal.CombineSQL(op, ref SQL1, ref SQL2, "O_CUSTOMER", Criteria, timeStamp: true);
                ExecuteSQL(SQL);
            }


        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    //-- edit 17/08/2023 -- ดึงข้อมุล customer จาก gis จึงมี structure เหมือน gis.pipeline.station_location 
    public DataTable SearchCustomerSubtype(String OtherCriteria = "")
    {
        String sql = ""; String criteria = "";
        try
        {
            if (OtherCriteria != "") { criteria = OtherCriteria; }


            sql = "SELECT DISTINCT SUB_TYPE  FROM O_CUSTOMER " +
                " WHERE NOT SUB_TYPE IS NULL ";
            if (criteria != "") { sql += " AND " + criteria; }
            sql += " ORDER BY SUB_TYPE";
           

            return QueryData(sql);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    //-- edit 17/08/2023 --
    public DataTable SearchCustomerRegion(String OtherCriteria = "")
    {
        String sql = ""; String criteria = "";
        try
        {
            if (OtherCriteria != "") { criteria = OtherCriteria; }


            sql = "SELECT DISTINCT REGION  FROM O_CUSTOMER " +
                " WHERE NOT REGION IS NULL ";
            if (criteria != "") { sql += " AND " + criteria; }
            sql += " ORDER BY REGION";


            return QueryData(sql);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    //-- edit 11/09/2023 --
    public DataTable SearchCustomerQualityMain(String OtherCriteria = "")
    {
        String sql = ""; String criteria = "";
        try
        {
            if (OtherCriteria != "") { criteria = OtherCriteria; }


            sql = "SELECT DISTINCT QUALITY_MAIN  FROM O_CUSTOMER " +
                " WHERE NOT QUALITY_MAIN IS NULL ";
            if (criteria != "") { sql += " AND " + criteria; }
            sql += " ORDER BY QUALITY_MAIN";


            return QueryData(sql);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    //-- edit 29/05/2024 ---
    public void MngTmpCustomerXLS(int op, object custID, object custShort = null, object custName = null, object region = null, object custType = null, object bvZone = null, object bvValve = null, object QualityMain = null, object QualitySupport1 = null, object QualitySupport2 = null, object omaMain = null, object omaSupport1 = null, object h2s = null, object hg = null, object remark = null)
    {
        string SQL, SQL1, SQL2, Criteria = "";
        try
        {
            SQL = ""; SQL1 = ""; SQL2 = "";
            if (op != DBUTIL.opINSERT)
            {
                Project.dal.AddCriteria(ref Criteria, "PERMANENT_CODE", custID, DBUTIL.FieldTypes.ftText);
            }
            if (op != DBUTIL.opDELETE)
            {
                if (op == DBUTIL.opINSERT)
                {
                    Project.dal.AddSQL(op, ref SQL1, ref SQL2, "PERMANENT_CODE", custID, DBUTIL.FieldTypes.ftText);

                }
                else
                {
                    op = DBUTIL.opUPDATE;
                }

                Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "NAME_ABBR", Utility.ToString(custShort).Trim(), DBUTIL.FieldTypes.ftText);
                Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "NAME_FULL", Utility.ToString(custName).Trim(), DBUTIL.FieldTypes.ftText);
                Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "REGION", Utility.ToString(region).Trim(), DBUTIL.FieldTypes.ftText);
                Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "TYPE", Utility.ToString(custType).Trim(), DBUTIL.FieldTypes.ftText);
                Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "BV_ZONE", Utility.ToString(bvZone).Trim(), DBUTIL.FieldTypes.ftText);
                Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "BV_VALVE", Utility.ToString(bvValve).Trim(), DBUTIL.FieldTypes.ftText);
                Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "QUALITY_MAIN", Utility.ToString(QualityMain).Trim(), DBUTIL.FieldTypes.ftText);
                Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "QUALITY_SUPPORT1", Utility.ToString(QualitySupport1).Trim(), DBUTIL.FieldTypes.ftText);
                Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "QUALITY_SUPPORT2", Utility.ToString(QualitySupport2).Trim(), DBUTIL.FieldTypes.ftText);
                Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "OMA_MAIN", Utility.ToString(omaMain).Trim(), DBUTIL.FieldTypes.ftText);
                Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "OMA_SUPPORT1", Utility.ToString(omaSupport1).Trim(), DBUTIL.FieldTypes.ftText);
                Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "H2S", Utility.ToString(h2s).Trim(), DBUTIL.FieldTypes.ftText);
                Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "HG", Utility.ToString(hg).Trim(), DBUTIL.FieldTypes.ftText);
                Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "REMARK", Utility.ToString(remark).Trim(), DBUTIL.FieldTypes.ftText);


            }
            
            if (op == DBUTIL.opUPDATE && Criteria == "") //-- edit 
            {
                throw new Exception("Insufficient data!");
            }
            else
            {
                SQL = Project.dal.CombineSQL(op, ref SQL1, ref SQL2, "O_TMP_CUSTOMER_XLS", Criteria, timeStamp: false);
                ExecuteSQL(SQL);
            }


        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    #endregion

    #region MailTemplate



    //edit 05/07/2018
    public DataTable SearchEmailTemplate(String TmpId, String MailTo, String MailCC, String MailBCC, String Subject, String ActiveFlag, String orderSQL = "", String OtherCriteria = "")
    {
        String sql = ""; String criteria = "";

        try
        {
            if (OtherCriteria != "") { criteria = OtherCriteria; }
            AddCriteria(ref criteria, "ETEMPLATE_ID", TmpId, DBUTIL.FieldTypes.ftNumeric);
            AddCriteria(ref criteria, "MAIL_TO", MailTo, DBUTIL.FieldTypes.ftText);
            AddCriteria(ref criteria, "MAIL_CC", MailCC, DBUTIL.FieldTypes.ftText);
            AddCriteria(ref criteria, "MAIL_BCC", MailBCC, DBUTIL.FieldTypes.ftText);
            AddCriteria(ref criteria, "UPPER(SUBJECT)", Subject.ToUpper(), DBUTIL.FieldTypes.ftText);
            
            AddCriteria(ref criteria, "ACTIVE_FLAG", ActiveFlag, DBUTIL.FieldTypes.ftText);


            sql = "SELECT  * FROM O_EMAIL_TEMPLATES ";

            if (criteria != "") { sql += " WHERE " + criteria; }

            if (orderSQL != "")
            {
                sql += " ORDER BY " + orderSQL;
            }
            else
            {

                sql += " ORDER BY ETEMPLATE_ID";
            }


            return QueryData(sql);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    //-- edit 05/07/2018
    public void MngEmailTemplate(int op, String ETmpId, String MailTo, String MailCC, String MailBCC, String Subject, String Message, String ActiveFlag)
    {
        string SQL, SQL1, SQL2, Criteria = "";
        try
        {
            SQL = ""; SQL1 = ""; SQL2 = "";
            if (op != DBUTIL.opINSERT)
            {
                AddCriteria(ref Criteria, "ETEMPLATE_ID", ETmpId, DBUTIL.FieldTypes.ftNumeric);

            }
            if (op != DBUTIL.opDELETE)
            {
                if (op == DBUTIL.opINSERT)
                {
                    if (ETmpId == "") { ETmpId = GenerateID("O_EMAIL_TEMPLATES", "ETEMPLATE_ID").ToString(); }
                    AddSQL(op, ref SQL1, ref SQL2, "ETEMPLATE_ID", ETmpId, DBUTIL.FieldTypes.ftNumeric);
                }
                else 
                {
                    op = DBUTIL.opUPDATE;
                }

                AddSQL2(op, ref SQL1, ref SQL2, "MAIL_TO", MailTo, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "MAIL_CC", MailCC, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "MAIL_BCC", MailBCC, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "SUBJECT", Subject, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "MESSAGE", Message, DBUTIL.FieldTypes.ftText);
                AddSQL2(op, ref SQL1, ref SQL2, "ACTIVE_FLAG", ActiveFlag, DBUTIL.FieldTypes.ftText);

            }

            if (op != DBUTIL.opINSERT && Criteria == "")
            {
                throw new Exception("Insufficient data!");
            }
            else
            {
                SQL = CombineSQL(op, ref SQL1, ref SQL2, "O_EMAIL_TEMPLATES", Criteria);
                ExecuteSQL(SQL);
            }


        }
        catch (Exception ex)
        {
            throw ex;
        }

    }

    #endregion
}
