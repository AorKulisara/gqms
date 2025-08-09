//*********************************************************
//  Copyright (c), Thaiintersol co,ltd. All rights reserved.
//**********************************************************




using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

public class Security
{
    //Work Process
    public const int TaskUpload = 1;     //Upload excel (BTU)
    public const int TaskVerify = 2;     //Data verification
    public const int TaskRptMonthly = 5;   //Report: Monthly report
    public const int TaskRptSite = 6;   //Report: Site compare report
    public const int TaskRptBTU = 7;            //Report: BTU compare report

    //Master Data
    public const int TaskMDSite = 10;     //Master data: Site management
    public const int TaskMDTag = 11;       //Master data: Tag mapping
    public const int TaskMDMail = 12;       //Master data: Mail template
    public const int TaskMDTransfer = 13;    //Master data: Transfer data
    public const int TaskMDCustomer = 14;    //Master data: Customer data

    //Security
    public const int TaskRole=30;           //Security: Role Management
    public const int TaskUser=31;           //Security: User Management
    public const int TaskEventLog=32;       //Security: Event Log
    public const int TaskExceptionLog=33;   //Security: Exception Log




    //---==========================================================

    public const int actView = 1;
    public const int actAdd = 2;
    public const int actEdit = 4;
    public const int actDelete = 8;
    public const int actApprove = 16;


    private static String chrLst = " 0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_@#$%^&*()-+/=.,!";
    private static int chrCnt = 79;
    private static int maxKeyLen = 20;

    public Security()
    {

    }

    private static String Key2Chr(Int32 num)
    {
        return chrLst.Substring((num % chrCnt), 1);
    }

    private static Int32 Key2Num(String key)
    {
        return chrLst.IndexOf(key);
    }

    public static String DecodeKey(String txt1, String txt2)
    {
        Int32 x, num, i;
        String s, t;
        String c = " ";
        String ret = "";
        String ChkRight;
        try
        {
            s = ""; t = "";
            if (txt2.Length != maxKeyLen + 3)
            {
                ret = "";
            }
            else
            {
                s = Utility.Right(c.PadRight(maxKeyLen - 1, ' ') + txt1 + txt1 + txt1 + txt1, maxKeyLen);
                x = Key2Num(txt2.Substring(maxKeyLen + 1, 1)) * chrCnt + Key2Num(txt2.Substring(maxKeyLen, 1));
                for (i = maxKeyLen; i >= 1; i--)
                {
                    num = (chrCnt + (Key2Num(txt2.Substring(i - 1, 1)) + 55 + Key2Num(s.Substring(i - 1, 1)) - x) % chrCnt) % chrCnt;
                    t = Key2Chr(num) + t;
                    x = x - 55 - Key2Num(s.Substring(i - 1, 1)) + num;
                }
                ChkRight = Utility.Right(txt2, 1);
                ret = Utility.Right(t, (chrCnt * 100 + Key2Num(ChkRight) - x) % chrCnt);
            }
        }
        catch
        {
            ret = "";
        }

        return ret;
    }

    public static String EncodeKey(String txt1, String txt2)
    {
        String txt;
        Int32 i, x;
        String s = "";

        try
        {
            txt1 = txt1.Trim();
            txt2 = txt2.Trim();

            x = 55;
            for (i = 1; i <= 10; i++)
            {
                if (i > (10 - txt1.Length))
                {
                    x = (x + i) ^ (int)System.Convert.ToChar(txt1.Substring(10 - i, 1));
                }
                else
                {
                    x = x ^ i;
                }
                if (i <= txt2.Length)
                {
                    x = x ^ (int)System.Convert.ToChar(txt2.Substring(i - 1, 1));
                }
                else
                {
                    x = x ^ (i * 3);
                }
                x = x & 127;
                if (x == 124)
                {
                    x = 125;
                }
                else if (x == 39)
                {
                    x = 40;
                }
                else if (x < 32)
                {
                    x += 32;
                }
                s += Convert.ToChar(x);
            }
        }
        catch (Exception ex)
        {
            txt = ex.Message;
            s = "";
        }

        return s;
    }

    // 8/10/2015 Miew Edit รองรับ Scan Security
    public static Boolean CheckPath(string RedirectUrl = "../ErrorPage.aspx")
    {
        bool Valid = false;
        try
        {
            Valid = Validation.IsValidStr(Utility.ToString(HttpContext.Current.Request.ServerVariables["PATH_INFO"]), true);

        }
        catch (Exception ex)
        {
            Valid = false;
        }
        if (!Valid && RedirectUrl != "")
        {
            HttpContext.Current.Session.Clear();
            HttpContext.Current.Session["RETRY"] = 99;
        }
        return (Valid);
    }

    public static Boolean CheckRole(Int32 taskID)
    {
        Int32 currentPriviledge = -1;

        // 8/10/2015 Miew Edit รองรับ Scan Security
        CheckPath();

        if (Utility.ToString(HttpContext.Current.Session["USER_NAME"]) == "")
        {
            HttpContext.Current.Response.Write("<script language=\"javascript\">if (parent != null || parent != undefined){ parent.location.href='../NoRight.aspx';} else if (opener != null || opener != undefined){opener.location.href='../NoRight.aspx'; this.close();} else {window.location.href='../NoRight.aspx';}</script>");
        }
        else if (!IsAuthorized(taskID, actView, ref currentPriviledge))
        {
            HttpContext.Current.Response.Write("<script language=\"javascript\">" +
            "alert('Unable to access this page ! Please contact system administrator if you require to access.');" +
            "if (opener){window.opener.location.href='" + HttpContext.Current.Request.UrlReferrer.ToString() + "'; this.close();}else if(parent)" +
            "{window.parent.location.href='" + HttpContext.Current.Request.UrlReferrer.ToString() + "';} else" +
            "{window.location.href='" + HttpContext.Current.Request.UrlReferrer.ToString() + "';}</script>");
        }
        return (currentPriviledge > 0);
    }


    public static Boolean CheckRole(Int32 taskID, Boolean isGoNoRight)
    {
        Int32 currentPriviledge = -1;
        string UserName = "";
        string UrlRefer = "";

        // 8/10/2015 Miew Edit รองรับ Scan Security
        CheckPath();

        if (!IsAuthorized(taskID, actView, ref currentPriviledge) && isGoNoRight)
        {
            
            UrlRefer = Utility.ToString(HttpContext.Current.Request.UrlReferrer);
            UserName = Utility.ToString(HttpContext.Current.Session["USER_NAME"]) + "";

            if (UserName == "")
            {
                HttpContext.Current.Response.Write("<script language=\"javascript\">" +
                   "alert('Unable to access this page ! Please contact system administrator if you require to access.');" +
                   "if (opener){window.opener.location.href='../NoRight.aspx'; this.close();}else if(parent)" +
                   "{window.parent.location.href='../NoRight.aspx';} else" +
                   "{window.location.href='../NoRight.aspx';}</script>");
            }
            else
            {

                if (!string.IsNullOrEmpty(UrlRefer))
                {
                    HttpContext.Current.Response.Write("<script language=\"javascript\">" +
                    "alert('Unable to access this page ! Please contact system administrator if you require to access.');" +
                    "if (opener){window.opener.location.href='" + UrlRefer + "'; this.close();}else if(parent)" +
                    "{window.parent.location.href='" + UrlRefer + "';} else" +
                    "{window.location.href='" + UrlRefer + "';}</script>");
                }
                else
                {
                    HttpContext.Current.Response.Write("<script language=\"javascript\">" +
                      "alert('Unable to access this page ! Please contact system administrator if you require to access.');" +
                      "if (opener){window.opener.location.href='../NoRight.aspx'; this.close();}else if(parent)" +
                      "{window.parent.location.href='../NoRight.aspx';} else" +
                      "{window.location.href='../NoRight.aspx';}</script>");
                }
            }
        }
        return (currentPriviledge > 0);
    }



    public static Boolean CanDo(int taskID, int action)
    {
        int currentPriviledge = 0;
        return IsAuthorized(taskID, action, ref currentPriviledge);
    }

    public static Boolean CanDo(int taskID, int action, ref int currentPriviledge) {
        return IsAuthorized(taskID, action, ref currentPriviledge);
    }

    public static Boolean IsAuthorized(Int32 taskID, Int32 action, ref Int32 currentPriviledge)
    {
        return IsAuthorized(taskID, action, ref currentPriviledge, null);
    }

    public static Boolean IsAuthorized(Int32 taskID, Int32 action, ref Int32 currentPriviledge, String permissions)
    {
       String rights = "";
       Int32 p = 0;
       try
       {
            if (permissions != null)
            {
                rights = permissions;
            }
            else
            {
                rights = Utility.ToString(HttpContext.Current.Session["RIGHTS"]);
            }

            p = Convert.ToInt32(System.Text.Encoding.ASCII.GetBytes(rights.Substring(taskID - 1, 1) + "@")[0]) - 64;
            currentPriviledge = p;
            return ((action & p) != 0);
       }
        catch (Exception ex)
        {
            return false;
        }
    }

    public static String Encrypted(object Key1, object Key2) {
        int X;
        string S = "";
        Key1 = Key1.ToString().Trim();
        Key2 = Key2.ToString().Trim();
        X = 55;
        for (int i = 0; i < 10; i++) {
            if (i > 10 - Key1.ToString().Length)
            {
                X = (X + i) ^ (int)Convert.ToChar(Key1.ToString().Substring(10 - i, 1));
            }
            else {
                X = X ^ i;
            }
            if (i < Key2.ToString().Length)
            {
                X = X ^ (int)Convert.ToChar(Key2.ToString().Substring(i, 1));
            }
            else {
                X = X ^ (i * 3);
            }
            X = X & 127;
            if (X == 124)
            {
                X = 125;
            }
            else if(X<32) {
                X = X + 32;
            }
            if (X == 39) {
                X = 40;
            }
            S = S + Convert.ToChar(X);
        }
        return S;
    }



    public static Boolean IsAuthenticated()
    {
        string UserName = "";
        int I;

        UserName = HttpContext.Current.Session["USER_NAME"] + "";
        if (UserName != "")
        {
            return true;
        }
        else
        {
            try
            {
                if (HttpContext.Current.Request.Cookies["UserData"] != null)
                {
                    UserName = HttpContext.Current.Request.Cookies["UserData"].Values["UID"].ToString();
                }
            }
            catch { }
        }

        if (UserName == "" && HttpContext.Current.User.Identity.IsAuthenticated)
        {
            UserName = HttpContext.Current.User.Identity.Name;
            I = UserName.IndexOf("\\");
            if (I >= 0) { UserName = UserName.Substring(I + 1); }
        }

        Project.LoadUserData(UserName, true);

        return HttpContext.Current.Session["USER_NAME"] + "" != "";

    }


    public static Boolean IsBannedRequest(string UserName)
    {
        DAL DAL = new DAL();
        DataTable DT = null;
        DataRow DR = null;
        double m;
        TimeSpan t;
        string ret = "";
        try
        {
            DT = DAL.SearchAuditLog("", "", Project.catBannedLog, "", UserName, "TRANS_DATE DESC", " TRANS_DATE>SYSDATE-1 ");
            DR = Utility.GetDR(ref DT);
            if (DR != null)
            {
                t = DateTime.Now - (DateTime)DR["TRANS_DATE"];
                m = t.TotalMinutes;
                if (m < Utility.ToInt(Project.gBannedDuration))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        catch (Exception ex)
        {
            ret = Utility.GetErrorMessage(ex, UsrMsg: "BannedRequest Error: Username=" + UserName + " Message=" + ex.Message);
            return false;
        }
        finally
        {
            Utility.ClearObject(ref DT);
        }
    }




}