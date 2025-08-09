//*********************************************************
//  Copyright (c), Thaiintersol co,ltd. All rights reserved.
//**********************************************************




#region .NET Framework Class Import
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;
using System.Security;
using System.Security.Principal;
using System.Security.Cryptography;
using System.Configuration;
using System.DirectoryServices;

#endregion

public class SecurityUtil
{
    #region Internal member valiables
    private string _appName;
    private string _appkey;
    private string _keyString;
    private byte[] _key;
    private string _LDAP_Path = "";
    private string _LDAP_FilterAttribute;
    #endregion

    private const string symmProvider = "TripleDESCryptoServiceProvider";
    private const string hashprovider = "MD5CryptoServiceProvider";

    private string SymDecrypt(string EncryptedString, string KeyString)
    {
        byte[] key;
        string DecryptedString = "";

        try
        {
            CryptoUtility.SymmetricCryptographer.Initiaize();
            key = Encoding.Unicode.GetBytes(KeyString);
            DecryptedString = CryptoUtility.SymmetricCryptographer.DecryptString(EncryptedString, key);
            CryptoUtility.SymmetricCryptographer.Unload(null, null);
            return DecryptedString;
        }
        catch (Exception ex)
        {
            CryptoUtility.SymmetricCryptographer.Unload(null, null);
            //throw new SharedException("Decryption error. " + ex.Message);
            throw (ex);
        }


    }

    public string DecryptData(string EncryptedText) {
        string key = "";
        try
        {
            key = ConfigurationManager.AppSettings["Securitykey"].ToString();
            return SymDecrypt(EncryptedText, key);
        }
        catch (Exception ex) {
            return "";
        }
    }

    public Boolean IsADAuthenticated(string Domain, string UserName, string Password) {
        DirectoryEntry entry;
        DirectorySearcher search;
        SearchResult result;
        object obj;
        string ret = ""; 
        try
        {
            if (Domain == "")
            {
                    return false;
            }
            else {
                if (_LDAP_Path == "") {
                    _LDAP_Path = "LDAP://" + Domain;
                }
                //-- aor edit 13/03/2017 -- ถ้ากรณีที่ไม่ได้ส่ง password มา ก็แสดงว่าให้ใช้ user ที่กำหนดใน web.config เชื่อมต่อ AD
                if (Password != "")
                     entry = new DirectoryEntry(_LDAP_Path, Domain + "\\" + UserName, Password);
                else
                     entry = new DirectoryEntry(_LDAP_Path, Domain + "\\" + Utility.ToString(ConfigurationManager.AppSettings["ADUser"]), Utility.ToString(ConfigurationManager.AppSettings["ADPWD"]));
                
                               
                obj = entry.NativeObject;
                search = new DirectorySearcher(entry);
                search.Filter = "(&(objectClass=user)(sAMAccountName=" + UserName + "))";
                search.PropertiesToLoad.Add("cn");
                search.CacheResults = true;
                result = search.FindOne();
                entry = null;

                if (result != null)
                {
                    _LDAP_Path = result.Path;
                    _LDAP_FilterAttribute = (string)result.Properties["cn"][0].ToString();
                    return true;
                }
                else {
                    return false;
                }

            }

        }
        catch(Exception ex) {
            ret = Utility.GetErrorMessage(ex, UsrMsg: "AD Authentication Error: Username=" + UserName + " Message=" + ex.Message);
            entry = null;
            return false;
        }
    }

    //-- edit 11/11/2016 --
    public void QueryAD(string Domain, string UserName, string Password, ref string UserDesc, ref string PositionName, ref string Department, ref string Mail, ref string TelNo, string SearchDisplayName="") {
        DirectoryEntry entry;
        DirectorySearcher search;
        SearchResult result;
        object obj;

        try
        {
            if (Domain == "")
            {
            }
            else {
                if (_LDAP_Path == "") { _LDAP_Path = "LDAP://" + Domain; }
                //-- aor edit 13/03/2017 -- ถ้ากรณีที่ไม่ได้ส่ง password มา ก็แสดงว่าให้ใช้ user ที่กำหนดใน web.config เชื่อมต่อ AD
                if (Password != "")
                    entry = new DirectoryEntry(_LDAP_Path, Domain + "\\" + UserName, Password);
                else
                    entry = new DirectoryEntry(_LDAP_Path, Domain + "\\" + Utility.ToString(ConfigurationManager.AppSettings["ADUser"]), Utility.ToString(ConfigurationManager.AppSettings["ADPWD"]));

                obj = entry.NativeObject;
                search = new DirectorySearcher(entry);
                if (SearchDisplayName == "")
                    search.Filter = "(&(objectClass=user)(sAMAccountName=" + UserName + "))";
                else
                    search.Filter = "(&(objectClass=user)(DisplayName=*" + SearchDisplayName + "))";

                search.PropertiesToLoad.Add("cn");
                search.PropertiesToLoad.Add("DisplayName");
                search.PropertiesToLoad.Add("Mail");
                search.PropertiesToLoad.Add("Title");
                search.PropertiesToLoad.Add("Department");
                search.PropertiesToLoad.Add("TelephoneNumber");
                search.CacheResults = true;
                entry = null;

                foreach (SearchResult s in search.FindAll())
                {
                    UserDesc = GetProperty(s, "DisplayName").ToString();
                    Mail = GetProperty(s, "Mail").ToString();
                    PositionName = GetProperty(s, "Title").ToString();
                    Department = GetProperty(s, "Department").ToString();
                    TelNo = GetProperty(s, "TelephoneNumber").ToString();
                }
            }
        }
        catch (Exception ex)
        {
            Utility.GetErrorMessage(ex, UsrMsg: "AD Authentication Error: Username=" + UserName + " Message=" + ex.Message);
            HttpContext.Current.Session["AD_ERROR"] = _LDAP_Path +"::" + ex.Message; //-- aor edit 09/06/2017 --
            entry = null;
        }
   }

    //-- edit 09/06/2017 --
    public void QueryADTest(string Domain, string UserName, string ADUser, string Password, ref string UserDesc, ref string PositionName, ref string Department, ref string Mail, ref string TelNo, string SearchDisplayName = "")
    {
        DirectoryEntry entry;
        DirectorySearcher search;
        SearchResult result;
        object obj;

        try
        {
            if (Domain == "")
            {
            }
            else {
                _LDAP_Path = "LDAP://" + Domain;
                //-- aor edit 13/03/2017 -- ถ้ากรณีที่ไม่ได้ส่ง password มา ก็แสดงว่าให้ใช้ user ที่กำหนดใน web.config เชื่อมต่อ AD
                //if (Password != "")
                //    entry = new DirectoryEntry(_LDAP_Path, Domain + "\\" + ADUser, Password);  //UserName-->ADUser
                //else
                //    entry = new DirectoryEntry(_LDAP_Path, Domain + "\\" + Utility.ToString(ConfigurationManager.AppSettings["ADUser"]), Utility.ToString(ConfigurationManager.AppSettings["ADPWD"]));

                if ( Password == "")
                {
                    ADUser = Utility.ToString(ConfigurationManager.AppSettings["ADUser"]);
                    Password = Utility.ToString(ConfigurationManager.AppSettings["ADPWD"]);
                }
                entry = new DirectoryEntry(_LDAP_Path, Domain + "\\" + ADUser, Password);  //UserName-->ADUser


                obj = entry.NativeObject;
                search = new DirectorySearcher(entry);
                if (SearchDisplayName == "")
                    search.Filter = "(&(objectClass=user)(sAMAccountName=" + UserName + "))";
                else
                    search.Filter = "(&(objectClass=user)(DisplayName=*" + SearchDisplayName + "))";

                search.PropertiesToLoad.Add("cn");
                search.PropertiesToLoad.Add("DisplayName");
                search.PropertiesToLoad.Add("Mail");
                search.PropertiesToLoad.Add("Title");
                search.PropertiesToLoad.Add("Department");
                search.PropertiesToLoad.Add("TelephoneNumber");
                search.CacheResults = true;
                entry = null;

                foreach (SearchResult s in search.FindAll())
                {
                    UserDesc = GetProperty(s, "DisplayName").ToString();
                    Mail = GetProperty(s, "Mail").ToString();
                    PositionName = GetProperty(s, "Title").ToString();
                    Department = GetProperty(s, "Department").ToString();
                    TelNo = GetProperty(s, "TelephoneNumber").ToString();
                }
            }
        }
        catch (Exception ex)
        {
            Utility.GetErrorMessage(ex, UsrMsg: "AD Authentication Error: Username=" + UserName + " Message=" + ex.Message);
            HttpContext.Current.Session["AD_ERROR"] = _LDAP_Path + "," + Domain  + "\\" + ADUser+"," + Password + "::" + ex.Message; //-- aor edit 09/06/2017 --
            entry = null;
        }
    }



    public string GetProperty(SearchResult result, string PropertyName) {
        string value = "";
        try
        {
            if (result != null) {
                if (result.Properties[PropertyName].Count > 0) {
                    value = result.Properties[PropertyName][0].ToString();
                }
            }
        }
        catch { }
        return value;
    }
}
