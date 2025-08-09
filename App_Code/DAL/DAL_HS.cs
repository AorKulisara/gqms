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


public partial class DAL_HS
{
    private String _dbProvider, _dbDataSource, _dbName, _dbUserName, _dbPassword;
    private String _ConnectionString;
    private DBUTIL DB = new DBUTIL();

    public DAL_HS()
    {
        ReadConfigurations();
        DB.DB_Provider = _dbProvider;
        DB.ConnectStr = _ConnectionString;
        DB.DB_Type = DB.GetDbTypes(_ConnectionString);
    }

    private void ReadConfigurations()
    {
        SecurityUtil Encrypt = new SecurityUtil();
        try
        {
            _dbProvider = Utility.ToString(ConfigurationManager.AppSettings["DB_ProviderHS"]);
            _dbDataSource = Utility.ToString(ConfigurationManager.AppSettings["DB_DataSourceHS"]);
            _dbName = Utility.ToString(ConfigurationManager.AppSettings["DB_NameHS"]);
            _dbUserName = Utility.ToString(ConfigurationManager.AppSettings["DB_UserNameHS"]);
            _dbPassword = Utility.ToString(ConfigurationManager.AppSettings["DB_PasswordHS"]);

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

    public DataTable QueryData(string SQL, OleDbConnection Conn = null, OleDbTransaction Trans = null)
    {
        return DB.QueryData(SQL, Conn, Trans); ;
    }

    public int ExecuteSQL(string SQL, OleDbConnection Conn = null, OleDbTransaction Trans = null)
    {
        return DB.ExecuteSQL(SQL, Conn, Trans);
    }

    // ================================================================================================================================
    #region PMISHS_DIM


    //-- aor edit 10/07/2018 --
    public DataTable SearchDimFlowRate(String Name = "", String TagName = "", String orderSQL = "", String OtherCriteria = "")
    {
        String sql = ""; String criteria = "";
        try
        {
            if (OtherCriteria != "") { criteria = OtherCriteria; }
            Project.dal.AddCriteria(ref criteria, "UPPER(NAME)", Name.ToUpper(), DBUTIL.FieldTypes.ftText);
            Project.dal.AddCriteria(ref criteria, "UPPER(TAGNAME)", TagName.ToUpper(), DBUTIL.FieldTypes.ftText);
                    

            sql = "SELECT NAME, TAGNAME, TAGDESC, UNIT, NAME || ' (' || TAGNAME || ')' AS NAME_TAGNAME " +
                " FROM CHONBURI.DIM_FLOWRATE ";
            if (criteria != "") { sql += " WHERE " + criteria; }


            if (orderSQL != "")
            {
                sql += " ORDER BY " + orderSQL;
            }
            else
            {
                sql += " ORDER BY NAME";
            }

            return QueryData(sql);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    //-- aor edit 10/07/2018 --
    public void MngDimFlowRate(int op, string Name, string TagName, string TagDesc, string Unit)
    {
        string SQL, SQL1, SQL2, Criteria = "";
        try
        {
            SQL = ""; SQL1 = ""; SQL2 = "";
            if (op != DBUTIL.opINSERT)
            {
                Project.dal.AddCriteria(ref Criteria, "TAGNAME", TagName, DBUTIL.FieldTypes.ftText);
            }
            if (op != DBUTIL.opDELETE)
            {
                if (op == DBUTIL.opINSERT)
                {
                    
                    Project.dal.AddSQL(op, ref SQL1, ref SQL2, "TAGNAME", Utility.ToString(TagName).Trim(), DBUTIL.FieldTypes.ftText);

                }
                else {
                    op = DBUTIL.opUPDATE;
                }

                Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "NAME", Utility.ToString(Name).Trim(), DBUTIL.FieldTypes.ftText);
                Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "TAGDESC", TagDesc, DBUTIL.FieldTypes.ftText);
                Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "UNIT", Unit, DBUTIL.FieldTypes.ftText);

            }

            if (op != DBUTIL.opINSERT && Criteria == "")
            {
                throw new Exception("Insufficient data!");
            }
            else {
                SQL = Project.dal.CombineSQL(op, ref SQL1, ref SQL2, "CHONBURI.DIM_FLOWRATE", Criteria, timeStamp: false);
                ExecuteSQL(SQL);
            }


        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    //-- aor edit 10/07/2018 --
    public DataTable SearchDimMoisture(String Name = "", String TagName = "", String orderSQL = "", String OtherCriteria = "")
    {
        String sql = ""; String criteria = "";
        try
        {
            if (OtherCriteria != "") { criteria = OtherCriteria; }
            Project.dal.AddCriteria(ref criteria, "UPPER(NAME)", Name.ToUpper(), DBUTIL.FieldTypes.ftText);
            Project.dal.AddCriteria(ref criteria, "UPPER(TAGNAME)", TagName.ToUpper(), DBUTIL.FieldTypes.ftText);


            sql = "SELECT NAME, TAGNAME, TAGDESC, UNIT, NAME || ' (' || TAGNAME || ')' AS NAME_TAGNAME " +
                " FROM CHONBURI.DIM_MOISTURE ";
            if (criteria != "") { sql += " WHERE " + criteria; }


            if (orderSQL != "")
            {
                sql += " ORDER BY " + orderSQL;
            }
            else
            {
                sql += " ORDER BY NAME";
            }

            return QueryData(sql);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    //-- aor edit 10/07/2018 --
    public void MngDimMoisture(int op, string Name, string TagName, string TagDesc, string Unit)
    {
        string SQL, SQL1, SQL2, Criteria = "";
        try
        {
            SQL = ""; SQL1 = ""; SQL2 = "";
            if (op != DBUTIL.opINSERT)
            {
                Project.dal.AddCriteria(ref Criteria, "TAGNAME", TagName, DBUTIL.FieldTypes.ftText);
            }
            if (op != DBUTIL.opDELETE)
            {
                if (op == DBUTIL.opINSERT)
                {
                    Project.dal.AddSQL(op, ref SQL1, ref SQL2, "TAGNAME", Utility.ToString(TagName).Trim(), DBUTIL.FieldTypes.ftText);

                }
                else {
                    op = DBUTIL.opUPDATE;
                }

                Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "NAME", Utility.ToString(Name).Trim(), DBUTIL.FieldTypes.ftText);
                Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "TAGDESC", TagDesc, DBUTIL.FieldTypes.ftText);
                Project.dal.AddSQL2(op, ref SQL1, ref SQL2, "UNIT", Unit, DBUTIL.FieldTypes.ftText);

            }

            if (op != DBUTIL.opINSERT && Criteria == "")
            {
                throw new Exception("Insufficient data!");
            }
            else {
                SQL = Project.dal.CombineSQL(op, ref SQL1, ref SQL2, "CHONBURI.DIM_MOISTURE", Criteria, timeStamp: false);
                ExecuteSQL(SQL);
            }


        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    #endregion

    // ================================================================================================================================
    #region PMISHS_CHONBURI_VIEW


    //-- aor edit 25/07/2018 --
    //-- FromYMD = YYYYMMDD
    public DataTable SearchHSVwArchDayFlowrate(String TagName = "", String FromYMD = "", String orderSQL = "", String OtherCriteria = "")
    {
        String sql = ""; String criteria = "";
        try
        {
            if (OtherCriteria != "") { criteria = OtherCriteria; }
            Project.dal.AddCriteria(ref criteria, "TAGNAME", TagName, DBUTIL.FieldTypes.ftText);
            if ( FromYMD != "")
            {
                if (criteria != "") criteria += " AND ";
                criteria += " \"DATE\" >= TO_DATE('" + FromYMD +"','YYYYMMDD') ";
            }

            sql = "SELECT DISTINCT * FROM CHONBURI.VW_ARCH_DAY_FLOWRATE ";
            if (criteria != "") { sql += " WHERE " + criteria; }


            if (orderSQL != "")
            {
                sql += " ORDER BY " + orderSQL;
            }
            else
            {
                sql += " ORDER BY \"DATE\" , TAGNAME ";
            }

            return QueryData(sql);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    //-- aor edit 25/07/2018 --
    //-- FromYMD = YYYYMMDD
    public DataTable SearchHSVwArchHourFlowrate(String TagName = "", String FromYMD = "", String orderSQL = "", String OtherCriteria = "")
    {
        String sql = ""; String criteria = "";
        try
        {
            if (OtherCriteria != "") { criteria = OtherCriteria; }
            Project.dal.AddCriteria(ref criteria, "TAGNAME", TagName, DBUTIL.FieldTypes.ftText);
            if (FromYMD != "")
            {
                if (criteria != "") criteria += " AND ";
                criteria += " \"DATE\" >=TO_DATE('" + FromYMD + "','YYYYMMDD') ";
            }

            sql = "SELECT DISTINCT * FROM CHONBURI.VW_ARCH_HOUR_FLOWRATE ";
            if (criteria != "") { sql += " WHERE " + criteria; }


            if (orderSQL != "")
            {
                sql += " ORDER BY " + orderSQL;
            }
            else
            {
                sql += " ORDER BY \"DATE\" , TAGNAME ";
            }

            return QueryData(sql);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    //-- aor edit 25/07/2018 --
    //-- FromYMD = YYYYMMDD
    public DataTable SearchHSVwArchDayMoisture(String TagName = "", String FromYMD = "", String orderSQL = "", String OtherCriteria = "")
    {
        String sql = ""; String criteria = "";
        try
        {
            if (OtherCriteria != "") { criteria = OtherCriteria; }
            Project.dal.AddCriteria(ref criteria, "TAGNAME", TagName, DBUTIL.FieldTypes.ftText);
            if (FromYMD != "")
            {
                if (criteria != "") criteria += " AND ";
                criteria += " \"DATE\" >= TO_DATE('" + FromYMD + "','YYYYMMDD') ";
            }

            sql = "SELECT DISTINCT * FROM CHONBURI.VW_ARCH_DAY_MOISTURE ";
            if (criteria != "") { sql += " WHERE " + criteria; }

            if (orderSQL != "")
            {
                sql += " ORDER BY " + orderSQL;
            }
            else
            {
                sql += " ORDER BY \"DATE\" , TAGNAME ";
            }

            return QueryData(sql);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    //-- aor edit 25/07/2018 --
    //-- FromYMD = YYYYMMDD
    public DataTable SearchHSVwArchHourMoisture(String TagName = "", String FromYMD = "", String orderSQL = "", String OtherCriteria = "")
    {
        String sql = ""; String criteria = "";
        try
        {
            if (OtherCriteria != "") { criteria = OtherCriteria; }
            Project.dal.AddCriteria(ref criteria, "TAGNAME", TagName, DBUTIL.FieldTypes.ftText);
            if (FromYMD != "")
            {
                if (criteria != "") criteria += " AND ";
                criteria += " \"DATE\" >= TO_DATE('" + FromYMD + "','YYYYMMDD') ";
            }

            sql = "SELECT DISTINCT * FROM CHONBURI.VW_ARCH_HOUR_MOISTURE ";
            if (criteria != "") { sql += " WHERE " + criteria; }

            if (orderSQL != "")
            {
                sql += " ORDER BY " + orderSQL;
            }
            else
            {
                sql += " ORDER BY \"DATE\", TAGNAME ";
            }

            return QueryData(sql);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    #endregion


    // ================================================================================================================================
    #region PMISHS_SCADAHIST_VIEW

    //-- aor edit 26/07/2018 --
    //-- FromYMD = YYYYMMDD
    public DataTable SearchHSVwOGCDaily(String GcName = "", String FromYMD = "", String orderSQL = "", String OtherCriteria = "")
    {
        String sql = ""; String criteria = "";
        try
        {
            if (OtherCriteria != "") { criteria = OtherCriteria; }
            Project.dal.AddCriteria(ref criteria, "GCNAME", GcName, DBUTIL.FieldTypes.ftText);
            if (FromYMD != "")
            {
                if (criteria != "") criteria += " AND ";
                criteria += " TIME > TO_DATE('" + FromYMD + "','YYYYMMDD') ";  //-- ให้ค้นหาวันใหม่เลย เนื่องจากถ้าค้นหาวันเก่าด้วย ต้องบันทึกจะต้องทำ _BACKUP record ซึ่งจะเกิดทุกวัน 
            }

            sql = "SELECT TIME, GCNAME, MAX(N2) N2, MAX(CO2) CO2, MAX(C1) C1, MAX(C2) C2, MAX(C3) C3, MAX(IC4) IC4, MAX(NC4) NC4, MAX(IC5) IC5, MAX(NC5) NC5, MAX(C6) C6, MAX(UNO) UNO, " +
            " MAX(SG) SG, MAX(GHVDRY) GHVDRY, MAX(GHVSAT) GHVSAT, MAX(NHVDRY) NHVDRY, MAX(WI) WI, MAX(H2S) H2S, MAX(UNNORMMIN) UNNORMMIN, MAX(UNNORMMAX) UNNORMMAX " +
            " FROM SCADAHIST.VW_OGC_DAILY ";
            if (criteria != "") { sql += " WHERE " + criteria; }
            sql += " GROUP BY TIME, GCNAME ";

            if (orderSQL != "")
            {
                sql += " ORDER BY " + orderSQL;
            }
            else
            {
                sql += " ORDER BY TIME, GCNAME ";
            }

            return QueryData(sql);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    //-- EDIT 22/08/2022 ---
    public DataTable SearchFID_InvalidFreeze(String FromYMD , String orderSQL = "", String OtherCriteria = "")
    {
        String sql = ""; String criteria = "";
        try
        {
            if (OtherCriteria != "") { criteria = OtherCriteria; }
            if (FromYMD != "")
            {

                sql = " SELECT TO_CHAR(D.TIME,'YYYYMMDD') TIME_YMD, D.TIME, D.GCNAME, S.TIME FROM " +
                " (SELECT TIME, GCNAME, MAX(C1) C1,  MAX(GHVDRY) GHVDRY, MAX(GHVSAT) GHVSAT " +
                "  FROM SCADAHIST.VW_OGC_DAILY  WHERE TIME > TO_DATE('" + FromYMD + "','YYYYMMDD') " +
                "   GROUP BY TIME, GCNAME ) D  " +
                " INNER JOIN    " +
                "  (SELECT TIME, GCNAME, MAX(C1) C1,  MAX(GHVDRY) GHVDRY, MAX(GHVSAT) GHVSAT " +
                "  FROM SCADAHIST.VW_OGC_DAILY WHERE TIME >= TO_DATE('" + FromYMD + "','YYYYMMDD') " +
                "   GROUP BY TIME, GCNAME ) S " +
                "  ON D.GCNAME=S.GCNAME AND D.TIME-1=S.TIME " +
                "  WHERE D.C1=S.C1 AND D.GHVDRY=S.GHVDRY AND D.GHVSAT=S.GHVSAT  " +
                "  AND D.C1>0 AND D.GHVDRY>0 AND D.GHVSAT>0 ";

                if (criteria != "") { sql += " AND " + criteria; }

                if (orderSQL != "")
                {
                    sql += " ORDER BY " + orderSQL;
                }
                else
                {
                    sql += " ORDER BY D.GCNAME, D.TIME ";
                }

            }

            return QueryData(sql);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    #endregion

}
