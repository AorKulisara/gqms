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


public partial class DAL_WH1
{
    private String _dbProvider, _dbDataSource, _dbName, _dbUserName, _dbPassword;
    private String _ConnectionString;
    private DBUTIL DB = new DBUTIL();
    public String ConnectionStringWH1;

    public DAL_WH1()
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
            _dbProvider = Utility.ToString(ConfigurationManager.AppSettings["DB_ProviderWH1"]);
            _dbDataSource = Utility.ToString(ConfigurationManager.AppSettings["DB_DataSourceWH1"]);
            _dbName = Utility.ToString(ConfigurationManager.AppSettings["DB_NameWH1"]);
            _dbUserName = Utility.ToString(ConfigurationManager.AppSettings["DB_UserNameWH1"]);
            _dbPassword = Utility.ToString(ConfigurationManager.AppSettings["DB_PasswordWH1"]);

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

            ConnectionStringWH1 = _ConnectionString;
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


}