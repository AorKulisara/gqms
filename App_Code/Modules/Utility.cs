//*********************************************************
//  Copyright (c), Thaiintersol co,ltd. All rights reserved.
//**********************************************************




using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

//*** Addition Component ***//
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

/// <summary>
/// Summary description for Utility
/// </summary>
public class Utility
{
    #region Declare Variable
    public enum Privilege { actRead = 1, actModify = 2, actConfirm = 4 };
    #endregion

    public Utility()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    static string Replace(string s, string OldChar, string newChar)
    {
        s = s.Replace(OldChar, newChar);
        return s;
    }

    public static int DateDiff(DateTime date1, DateTime date2)
    {
        int diff = -1;

        try
        {
            if (date1 != null && date2 != null)
            {
                TimeSpan ts = (date2 - date1);
                if (ts.Days >= 0) diff = ts.Days;
            }
        }
        catch
        {
        }
        return (diff);
    }


    #region Get Text
    public static Object CDB(Object val)
    {
        if (val == null || val == "")
        {
            return (DBNull.Value);
        }
        else
        {
            return (val);
        }
    }

    public static String Trim(Object val)
    {
        if (val == null)
        {
            return "";
        }
        else
        {
            return (val + "").ToString().Trim();
        }
    }

    public static String ToString(Object val)
    {
        if (val == null)
        {
            return "";
        }
        else
        {
            return (val + "").ToString();
        }
    }
    public static String FormatRetrieveData(Object val)
    {
        return FormatRetrieveData(val, true);
    }
    public static String FormatRetrieveData(Object val, Boolean isReplaceAll)
    {
        String ret = (val + "").ToString();

        ret = ret.Replace(">", "");
        ret = ret.Replace("<", "");
        ret = ret.Replace("..", "");
        ret = ret.Replace("--", "");
        ret = ret.Replace("`", "");
        ret = ret.Replace("'", "");
        ret = ret.Replace("|", "");
        ret = ret.Replace("&", "");
        ret = ret.Replace(":", "");
        ret = ret.Replace(";", "");
        ret = ret.Replace("$", "");
        ret = ret.Replace("@", "");
        ret = ret.Replace(",", "");
        ret = ret.Replace("\'", "");
        ret = ret.Replace("\"", "");
        ret = ret.Replace("/", "");
        ret = ret.Replace("+", "");
        ret = ret.Replace("(", "");
        ret = ret.Replace(")", "");
        ret = ret.Replace("=", "");
        ret = ret.Replace("<>", "");
        ret = ret.Replace("()", "");
        ret = ret.Replace("+", "");
        ret = ret.Replace("#", "");
        ret = ret.Replace(" ", "");
        if (isReplaceAll)
        {
            ret = ret.Replace("%", "");
            ret = ret.Replace("*", "");
        }

        return ret;
    }
    public static String FormatSearchData(Object searchText)
    {
        return FormatSearchData(searchText, false);
    }

    public static String FormatSearchData(Object searchText, Boolean IsRetrieveData = true)
    {
        return FormatSearchData(searchText, false, IsRetrieveData);
    }
    public static String FormatSearchData(Object searchText, Boolean searchBeginning, Boolean IsRetrieveData = true)
    {
        String ret = "";

        searchText = ToString(searchText);
        if (searchText.ToString() != "")
        {
            if (IsRetrieveData)
            {
                searchText = FormatRetrieveData(searchText, false);
            }
            searchText = searchText.ToString().Replace("*", "%");
            if (!(searchText.ToString().IndexOf("%") > 0))
            {
                if (searchBeginning)
                {
                    searchText = searchText + "%";
                }
                else
                {
                    searchText = "%" + searchText + "%";
                }
            }
            ret = ToString(searchText);
        }

        return ret;
    }
    public static String FormatSaveData(String data)
    {
        return FormatSaveData(data, "NULL");
    }
    public static String FormatSaveData(String data, String defaultValue)
    {
        String ret = "";

        if (data == "")
        {
            if (defaultValue == "")
            {
                data = "NULL";
            }
            else
            {
                data = defaultValue;
            }
        }
        else
        {
            ret = data;
        }

        return ret;
    }

    #endregion

    #region ClearObject
    public static void ClearObject(ref DataSet obj)
    {
        try
        {
            obj.Dispose();
        }
        catch { }
        finally { obj = null; }
    }
    public static void ClearObject(ref DataTable obj)
    {
        try
        {
            obj.Dispose();
        }
        catch { }
        finally { obj = null; }
    }
    public static void ClearObject(ref DataView obj)
    {
        try
        {
            obj.Dispose();
        }
        catch { }
        finally { obj = null; }
    }
    public static void ClearObject(ref StreamWriter obj)
    {
        try
        {
            if (obj != null) { obj.Close(); }
            if (obj != null) { obj.Dispose(); }
        }
        catch { }
        finally { obj = null; }
    }
    public static void ClearObject(ref FileStream obj)
    {
        try
        {
            if (obj != null) { obj.Close(); }
            if (obj != null) { obj.Dispose(); }
        }
        catch { }
        finally { obj = null; }
    }
    public static void ClearObject(ref TableRow obj)
    {
        try
        {
            if (obj != null) { obj.Dispose(); }
        }
        catch { }
        finally { obj = null; }
    }
    public static void ClearObject(ref TableCell obj)
    {
        try
        {
            if (obj != null) { obj.Dispose(); }
        }
        catch { }
        finally { obj = null; }
    }
    #endregion

    #region GetDT
    public static DataTable GetDT(ref DataSet ds)
    {
        DataTable dt = null;

        try
        {
            if (ds != null && ds.Tables.Count > 0)
            {
                dt = ds.Tables[0];
            }
        }
        catch { }

        return dt;
    }
    #endregion

    #region GetDR
    public static DataRow GetDR(ref DataSet ds)
    {
        DataRow dr = null;

        try
        {
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                dr = ds.Tables[0].Rows[0];
            }
        }
        catch { }

        return dr;
    }

    public static DataRow GetDR(ref DataTable dt)
    {
        DataRow dr = null;

        try
        {
            if (dt != null && dt.Rows.Count > 0)
            {
                if (dt.Columns.Count == 1 && dt.Rows.Count == 1 && ToString(dt.Rows[0][0]) == "")
                {
                    dr = null;
                }
                else
                {
                    dr = dt.Rows[0];
                }
            }
        }
        catch { }

        return dr;
    }
    #endregion

    #region GetDRV
    public static DataRowView GetDRV(ref DataView dv)
    {
        DataRowView drv = null;

        try
        {
            if (dv.Count > 0)
            {
                drv = dv[0];
            }
        }
        catch { }

        return drv;
    }
    #endregion

    #region GetErrorMessage
    public static String GetErrorMessage(Exception ex, bool WriteLogs = true, string UsrMsg = "")
    {
        String Msg = null;
        String UserName = null;
        Int32 debugLevel;

        try
        {
            debugLevel = Utility.ToInt(ConfigurationManager.AppSettings["DebugLevel"]);

            if (UsrMsg == "")
            {
                if (ex != null)
                {
                    switch (debugLevel)
                    {
                        case 2: Msg = ex.ToString(); break;
                        default: Msg = ex.Message; break;
                    }
                }
            }
            else
            {
                Msg = UsrMsg;
            }

            if (Msg != "")
            {
                Msg = Msg.Replace("'", "");
                Msg = Msg.Replace("\r\n", "\\r\\n").Replace("\r", "\\r").Replace("\n", "\\n").Replace("'", "\\'");
                try
                {
                    if (WriteLogs)
                    {

                        UserName = Utility.ToString(HttpContext.Current.Session["USER_NAME"]);

                        //-- AOR EDIT 25/04/2018 --
                        String UrlPage = Utility.ToString(HttpContext.Current.Session["CURRENT_PAGE"]);
                        if (UrlPage != "") Msg = UrlPage + "/" + Msg;

                        BLL.InsertAudit(Project.catErrorLog, Msg, UserName, "", "");                       
                    }
                }
                catch (Exception tex)
                {

                }
            }
        }
        catch { }
        return (Msg);
    }

    public static void MsgAlert(Object obj, bool IsException = false)
    {
        Exception exParam;
        try
        {
            // ไม่สนใจ Thread was being Aborted.
            if (!obj.GetType().IsAssignableFrom(typeof(System.Threading.ThreadAbortException)))
            {
                if (IsException == false)
                {
                    PTT.GQMS.USL.Web.Includes.MainPage.Msg = ToString(obj).Replace("\r\n", @"\r\n");
                    
                }
                else
                {
                    exParam = (Exception)obj;
                    PTT.GQMS.USL.Web.Includes.MainPage.Msg = GetErrorMessage(exParam).Replace("\r\n", @"\r\n");
                }
            }
        }
        catch (Exception ex) { throw ex; }
    }


    public static void WriteErrorMessage(Exception ex, string UsrMsg = "", string userName = "")
    {
        String Msg = null;
        Int32 debugLevel;

        try
        {
            debugLevel = Utility.ToInt(ConfigurationManager.AppSettings["DebugLevel"]);

            if (UsrMsg == "")
            {
                if (ex != null)
                {
                    Msg = ex.Message;
                }
            }
            else
            {
                Msg = UsrMsg;
            }

            if (Msg != "")
            {
                Msg = Msg.Replace("'", "");
                Msg = Msg.Replace("\r\n", "\\r\\n").Replace("\r", "\\r").Replace("\n", "\\n").Replace("'", "\\'");
                try
                {
                    if (userName == "")
                    {
                        userName = Utility.ToString(System.Web.HttpContext.Current.Session["USER_NAME"]);
                      
                        String UrlPage = Utility.ToString(HttpContext.Current.Session["CURRENT_PAGE"]);
                        if (UrlPage != "") Msg = UrlPage + "/" + Msg;
                    }

                    BLL.InsertAudit(Project.catErrorLog, Msg, userName, "", "");
                }
                catch (Exception tex)
                {

                }
            }
        }
        catch { }
     
    }



    #endregion


    #region New Date Utility

    //-- edit 02/07/2019 -
    public static int LastDayofMonth(int Year, int Month)
    {
        int d = 30;
        switch (Month)
        {
            case 1: d = 31; break;
            case 2: d = DateTime.DaysInMonth(Year, Month); break;
            case 3: d = 31; break;
            case 4: d = 30; break;
            case 5: d = 31; break;
            case 6: d = 30; break;
            case 7: d = 31; break;
            case 8: d = 31; break;
            case 9: d = 30; break;
            case 10: d = 31; break;
            case 11: d = 30; break;
            case 12: d = 31; break;
        }
        return d;
    }


    public static String EnMonth(Int32 m)
    {
        String EM = "";
        switch (m)
        {
            case 1: EM = "January"; break;
            case 2: EM = "February"; break;
            case 3: EM = "March"; break;
            case 4: EM = "April"; break;
            case 5: EM = "May"; break;
            case 6: EM = "June"; break;
            case 7: EM = "July"; break;
            case 8: EM = "August"; break;
            case 9: EM = "September"; break;
            case 10: EM = "October"; break;
            case 11: EM = "November"; break;
            case 12: EM = "December"; break;
        }
        return EM;
    }

    //-- AOR EDIT 24/05/2017 เปลี่ยนเป็น upper case
    public static String EnMonthAbbr(Int32 m)
    {
        String EM = "";
        switch (m)
        {
            case 1: EM = "JAN"; break;
            case 2: EM = "FEB"; break;
            case 3: EM = "MAR"; break;
            case 4: EM = "APR"; break;
            case 5: EM = "MAY"; break;
            case 6: EM = "JUN"; break;
            case 7: EM = "JUL"; break;
            case 8: EM = "AUG"; break;
            case 9: EM = "SEP"; break;
            case 10: EM = "OCT"; break;
            case 11: EM = "NOV"; break;
            case 12: EM = "DEC"; break;
        }
        return EM;
    }

    //-- edit 25/06/2019 -
    public static String EnMonthAbbrVal(String EM)
    {
        String m = "";
        switch (EM)
        {
            case "JAN": m = "01"; break;
            case "FEB": m = "02"; break;
            case "MAR": m = "03"; break;
            case "APR": m = "04"; break;
            case "MAY": m = "05"; break;
            case "JUN": m = "06"; break;
            case "JUL": m = "07"; break;
            case "AUG": m = "08"; break;
            case "SEP": m = "09"; break;
            case "OCT": m = "10"; break;
            case "NOV": m = "11"; break;
            case "DEC": m = "12"; break;
        }
        return m;
    }

    public static String ThMonth(Int32 m)
    {
        String TH = "";
        switch (m)
        {
            case 1: TH = "มกราคม"; break;
            case 2: TH = "กุมภาพันธ์"; break;
            case 3: TH = "มีนาคม"; break;
            case 4: TH = "เมษายน"; break;
            case 5: TH = "พฤษภาคม"; break;
            case 6: TH = "มิถุนายน"; break;
            case 7: TH = "กรกฎาคม"; break;
            case 8: TH = "สิงหาคม"; break;
            case 9: TH = "กันยายน"; break;
            case 10: TH = "ตุลาคม"; break;
            case 11: TH = "พฤศจิกายน"; break;
            case 12: TH = "ธันวาคม"; break;
        }
        return TH;
    }

    private static String ThMonthAbbr(Int32 m)
    {
        String TH = "";
        switch (m)
        {
            case 1: TH = "ม.ค."; break;
            case 2: TH = "ก.พ."; break;
            case 3: TH = "มี.ค."; break;
            case 4: TH = "เม.ย."; break;
            case 5: TH = "พ.ค."; break;
            case 6: TH = "มิ.ย."; break;
            case 7: TH = "ก.ค."; break;
            case 8: TH = "ส.ค."; break;
            case 9: TH = "ก.ย."; break;
            case 10: TH = "ต.ค."; break;
            case 11: TH = "พ.ย."; break;
            case 12: TH = "ธ.ค."; break;
        }
        return TH;
    }

    //-- edit 25/06/2019 -
    public static String ThMonthAbbrVal(String TH)
    {
        String m = "";
        switch (TH)
        {
            case "ม.ค.": m = "01"; break;
            case "ก.พ.": m = "02"; break;
            case "มี.ค.": m = "03"; break;
            case "เม.ย.": m = "04"; break;
            case "พ.ค.": m = "05"; break;
            case "มิ.ย.": m = "06"; break;
            case "ก.ค.": m = "07"; break;
            case "ส.ค.": m = "08"; break;
            case "ก.ย.": m = "09"; break;
            case "ต.ค.": m = "10"; break;
            case "พ.ย.": m = "11"; break;
            case "ธ.ค.": m = "12"; break;
        }
        return m;
    }

    public static String ShowENMonth(Int32 MM)
    {
        return EnMonth(MM);
    }

    public static String ShowENMonthAbbr(Int32 MM)
    {
        return EnMonthAbbr(MM);
    }

    public static String ShowTHMonth(Int32 MM)
    {
        return ThMonth(MM);
    }

    public static String ShowTHMonthAbbr(Int32 MM)
    {
        return ThMonthAbbr(MM);
    }

    public static Object ThaiDateValue(String S)
    {
        Int32 D, M, Y;
        Int32 I, J;
        Object HH, MI, SS;
        String Delim, TS;
        Object TDVal;

        Delim = "/";
        S = S.Trim();
        I = S.IndexOf(Delim);
        if (S.Length < 6 || I == -1)
        {
            return null;
        }
        else
        {
            D = ToInt(Left(S, I));
            J = S.IndexOf(Delim, I + 2);
            if (J == 0) { J = I; }
            if ((I > 0) && (J - I + 1 > 0))
            {
                M = Int32.Parse(Mid(S, I + 1, J - I - 1));
            }
            else
            {
                M = 0;
            }
            S = Mid(S, J + 1);
            if (IsNumeric(S))
            {
                Y = ToInt(S);
            }
            else
            {
                Y = 0;
            }
            I = S.IndexOf(" ");
            if (S.IndexOf(":") > -1)
            {
                TS = Mid(S, I + 1).Trim();
            }
            else
            {
                TS = "";
            }
            if (Y == 0 && TS != "")
            {
                S = S.Replace(" " + TS, "");
                if (IsNumeric(S))
                {
                    Y = ToInt(S);
                }
                else
                {
                    Y = 0;
                }
            }
            if (Y < 20)
            {
                Y += 2543;
            }
            else if (Y < 100)
            {
                Y += 2500;
            }
            else if (Y < 1900)
            {
                Y = 0;
            }
            else if (Y < 2400)
            {
                Y += 543;
            }
            if ((D > 0) && (D < 32) && (M > 0) && (M < 13) && (Y > 2400))
            {
                if (DateTime.Now.Year < 2500) { Y -= 543; }
                if (TS.Trim() != "")
                {
                    if (Y > 2500) { Y -= 543; }
                    TDVal = TS.Split(':');
                    HH = ((String[])TDVal)[0];
                    MI = ((String[])TDVal)[1];
                    SS = ((String[])TDVal)[2];
                    TDVal = null;
                    TDVal = new DateTime(Y, M, D, Convert.ToInt32(HH), Convert.ToInt32(MI), Convert.ToInt32(SS));
                }
                else
                {
                    TDVal = new DateTime(Y, M, D);
                }
            }
            else
            {
                TDVal = null;
            }
            return TDVal;
        }
    }

    public static bool IsNullDate(DateTime D)
    {
        return (Convert.ToDouble(D.ToOADate()) <= 2);
    }

    public static string CRDate(object DateVal)
    {
        System.Globalization.CultureInfo EN = System.Globalization.CultureInfo.CreateSpecificCulture("en-CA");
        object D;
        string strDate = "";

        if (DateVal != "")
        {
            D = Utility.AppDateValue(DateVal);
            if (System.Configuration.ConfigurationManager.AppSettings["CRDateType"].ToString() == "TH")
            {
                strDate = String.Format(EN, "{0:bbbb-MM-dd}", D);
            }
            else
            {
                strDate = String.Format(EN, "{0:yyyy-MM-dd}", D);
            }
        }

        return (strDate);

    }

    public static string CRDateTime(object DateVal)
    {
        System.Globalization.CultureInfo EN = System.Globalization.CultureInfo.CreateSpecificCulture("en-CA");
        object D;
        string strDate = "";

        if (DateVal != "")
        {
            D = Utility.AppDateValue(DateVal);
            if (System.Configuration.ConfigurationManager.AppSettings["CRDateType"].ToString() == "TH")
            {
                strDate = String.Format(EN, "{0:bbbb-MM-dd HH:mm:ss}", D);
            }
            else
            {
                strDate = String.Format(EN, "{0:yyyy-MM-dd HH:mm:ss}", D);
            }
        }

        return (strDate);

    }


    public static Object AppDateValue(object S)
    {
        //-- edit 14/03/2017 --
        //System.Globalization.CultureInfo EN = new System.Globalization.CultureInfo("th-TH");
        System.Globalization.CultureInfo EN = new System.Globalization.CultureInfo("en-US");

        object result = null;

        try
        {
            if (S.ToString() == "")
            {
                result = null;
            }
            else if (S.ToString().IndexOf("/") >= 0)
            {
                if (S.ToString().Length <= 10)
                {
                    result = DateTime.ParseExact(S.ToString(), @"dd/MM/yyyy", EN);
                }
                else
                {
                    //-- "dd/MM/yyyy HH:mm
                    if (S.ToString().Length == 16) S = S + ":00";
                    result = DateTime.ParseExact(S.ToString(), @"dd/MM/yyyy HH:mm:ss", EN);
                }
            }
            else
            {
                result = DateTime.Parse(S.ToString());
            }
        }
        catch (Exception ex)
        {

            result = null;
        }

        try
        {
            if (result == null)
            {
                result = Convert.ToDateTime(S, EN);
            }
        }
        catch (Exception ex)
        {
            result = null;
        }

        return (result);
    }

    public static DateTime AppDateValue(object D, string fmt)
    {   //-- edit 14/03/2017 --
        //System.Globalization.CultureInfo Culture = new System.Globalization.CultureInfo("th-TH");
        System.Globalization.CultureInfo Culture = new System.Globalization.CultureInfo("en-US");


        DateTime DT = new DateTime();
        String S = "";
        try
        {
            if (D + "" != "")
            {
                S = D.ToString();
                if (fmt == "")
                {
                    if (S.Length > 10)
                    {
                        DT = DateTime.ParseExact(S, "dd/MM/yyyy HH:mm:ss", Culture);
                    }
                    else
                    {
                        DT = DateTime.ParseExact(S, "dd/MM/yyyy", Culture);
                    }
                }
                else
                {
                    DT = DateTime.ParseExact(S, fmt, Culture);
                }
            }
        }
        catch (Exception ex)
        {
            try
            {
                DT = DateTime.Parse(S);
            }
            catch (Exception ex2)
            {
                DT = DT;
            }
        }

        return DT;
    }

    public static String FormatDate(DateTime d, String fmt)
    {
        Int32 DD, MM, YY, HH, MI, SS;
        if (d != null)
        {
            fmt = fmt.ToUpper();
            DD = d.Day;
            MM = d.Month;
            YY = d.Year;
            MI = d.Minute;
            SS = d.Second;
            HH = d.Hour;

            if (YY > 2400) { YY = YY - 543; }
            if (fmt.IndexOf("DD") > -1)
            {
                if (DD > 9)
                {
                    fmt = fmt.Replace("DD", DD.ToString());
                }
                else
                {
                    fmt = fmt.Replace("DD", "0" + DD.ToString());
                }
            }
            if (fmt.IndexOf("D") > -1) //-- edit 22/07/2019 --
            {
               fmt = fmt.Replace("D", DD.ToString());
            }
            if (fmt.IndexOf("MM") > -1)
            {
                if (MM > 9)
                {
                    fmt = fmt.Replace("MM", MM.ToString());
                }
                else
                {
                    fmt = fmt.Replace("MM", "0" + MM.ToString());
                }
            }

            if (fmt.IndexOf("MONTH") > -1)
            {
                fmt = fmt.Replace("MONTH", EnMonth(MM));
            }
            else if (fmt.IndexOf("MON") > -1) { fmt = fmt.Replace("MON", EnMonthAbbr(MM).ToUpper()); }

            if (fmt.IndexOf("YYYY") > -1) { fmt = fmt.Replace("YYYY", YY.ToString()); }
            if (fmt.IndexOf("YY") > -1) { fmt = fmt.Replace("YY", Right(YY.ToString(), 2)); }


            if (fmt.IndexOf("วว") > -1)
            {
                if (DD > 9)
                {
                    fmt = fmt.Replace("วว", DD.ToString());
                }
                else { fmt = fmt.Replace("วว", "0" + DD.ToString()); }
            }

            if (fmt.IndexOf("ดดดด") > -1)
            {
                fmt = fmt.Replace("ดดดด", ThMonth(MM));
            }
            else if (fmt.IndexOf("ดดด") > -1) { fmt = fmt.Replace("ดดด", ThMonthAbbr(MM)); }

            if (fmt.IndexOf("BBBB") > -1)
            {
                fmt = fmt.Replace("BBBB", Convert.ToString(YY + 543));
            }
            else if (fmt.IndexOf("BB") > -1) { fmt = fmt.Replace("BB", Right(Convert.ToString(YY + 543), 2)); }
            else if (fmt.IndexOf("ปปปป") > -1) { fmt = fmt.Replace("ปปปป", Convert.ToString(YY + 543)); }
            else if (fmt.IndexOf("ปป") > -1) { fmt = fmt.Replace("ปป", Right(Convert.ToString(YY + 543), 2)); }

            if (fmt.IndexOf("HH") > -1)
            {
                if (HH > 9)
                {
                    fmt = fmt.Replace("HH", HH.ToString());
                }
                else
                {
                    fmt = fmt.Replace("HH", "0" + HH.ToString());
                }
            }

            if (fmt.IndexOf("MI") > -1)
            {
                if (MI > 9)
                {
                    fmt = fmt.Replace("MI", MI.ToString());
                }
                else
                {
                    fmt = fmt.Replace("MI", "0" + MI.ToString());
                }
            }

            if (fmt.IndexOf("SS") > -1)
            {
                if (SS > 9)
                {
                    fmt = fmt.Replace("SS", SS.ToString());
                }
                else
                {
                    fmt = fmt.Replace("SS", "0" + SS.ToString());
                }
            }

        }
        return fmt;
    }

    public static String AppFormatTime(object D)
    {
        return FormatDate(AppDateValue(D, ""), "HH:MI");
    }


    public static String AppFormatDateTime(object D, string LangType = "EN")
    {
        string result = "";
        System.Globalization.CultureInfo Culture;

        try
        {
            if (ToString(D) != "")
            {
                if (LangType == "EN")
                {
                    Culture = new System.Globalization.CultureInfo("en-US");
                }
                else
                {
                    Culture = new System.Globalization.CultureInfo("th-TH");
                }
                //-- edit 11/05/2018 -- แสดงแค่นาที 
                //result = string.Format(Culture, @"{0:dd/MM/yyyy HH:mm:ss}", D);
                result = string.Format(Culture, @"{0:dd/MM/yyyy HH:mm}", D);
            }
        }
        catch (Exception ex)
        {
            result = "";
        }

        return result;
    }


    // Default Date เป็น EN  dd/MM/yyyy
    public static String AppFormatDate(object D, string LangType = "EN") 
    {
        string result = "";
        //CultureInfo.InvariantCulture > คือ Default EN
        System.Globalization.CultureInfo Culture;
        try
        {
            if (ToString(D) != "")
            {
                if (LangType == "EN")
                {
                    Culture = new System.Globalization.CultureInfo("en-US");
                }
                else
                {
                    Culture = new System.Globalization.CultureInfo("th-TH");
                }
                result = string.Format(Culture, @"{0:dd/MM/yyyy}", D);
            }
        }
        catch (Exception ex)
        {
            result = "";
        }

        return result;

    }

    public static String FormatAmt(string D)
    {
        string result = "";

        if (D + "" != "")
        {
            try
            {
                decimal temp = decimal.Parse(D);
                result = String.Format("{0:#,##0.00}", temp);
                //result = result.Remove(0, 1);
            }
            catch (Exception ex)
            {
                result = "";
            }
        }

        return result;

    }

    //-- aor edit 05/05/2017
    public static String FormatNum(Object val, int dec = 0)
    {
        string result = "";
        string D = "";
        if (val != null)
        {
            D = (val + "").ToString();
            if (D + "" != "")
            {


                try
                {
                    if (IsNumeric(D))
                    {
                        decimal temp = decimal.Parse(D);
                        switch (dec)
                        {
                            case 0: result = String.Format("{0:#,##0}", temp); break;
                            case 1: result = String.Format("{0:#,##0.0}", temp); break;
                            case 2: result = String.Format("{0:#,##0.00}", temp); break;
                            case 3: result = String.Format("{0:#,##0.000}", temp); break;
                            case 4: result = String.Format("{0:#,##0.0000}", temp); break;
                            case 5: result = String.Format("{0:#,##0.00000}", temp); break;
                            case 6: result = String.Format("{0:#,##0.000000}", temp); break;
                            case 7: result = String.Format("{0:#,##0.0000000}", temp); break;
                        }
                    }
                    else
                    {
                        result = D;  //-- EDIT 11/07/2018 --- ระบบนี้ค่าก๊าซบางครั้งอาจไม่ใช่ตัวเลข แต่ต้องแสดงให้เห็น
                    }

                }
                catch (Exception ex)
                {
                    result = "";
                }
            }
        }




        return result;

    }

    //-- aor edit 05/05/2017
    public static String FormatNumNoComma(Object val, int dec = 0)
    {
        string result = "";
        string D = "";
        if (val != null)
        {
            D = (val + "").ToString();
            if (D + "" != "")
            {


                try
                {
                    if (IsNumeric(D))
                    {
                        //-- EDIT 26/02/2022 ตัวเลขขนาดเล็กจะใช้ไม่ได้  0.000000483333167267119
                        //decimal temp = decimal.Parse(D);
                        decimal temp ;
                        D = D.Replace(",", ""); //-- edit 01/03/2022 comma ทำให้แปลงค่าไม่ได้
                        Boolean c = decimal.TryParse(D, System.Globalization.NumberStyles.Float, null, out temp);
                        switch (dec)
                        {
                            case 0: result = String.Format("{0:#0}", temp); break;
                            case 1: result = String.Format("{0:#0.0}", temp); break;
                            case 2: result = String.Format("{0:#0.00}", temp); break;
                            case 3: result = String.Format("{0:#0.000}", temp); break;
                            case 4: result = String.Format("{0:#0.0000}", temp); break;
                            case 5: result = String.Format("{0:#0.00000}", temp); break;
                            case 6: result = String.Format("{0:#0.000000}", temp); break;
                            case 7: result = String.Format("{0:#0.0000000}", temp); break;
                        }
                    }
                    else
                    {
                        //-- EDIT 25/06/2019 ---  <0.40
                        if (D.IndexOf("<") > -1 || D.IndexOf(">") > -1 || D.IndexOf("<=") > -1 || D.IndexOf(">=") > -1)
                            result = D;
                        else
                            result = "";  //-- EDIT 11/07/2018 --- ระบบนี้ค่าก๊าซบางครั้งอาจไม่ใช่ตัวเลข แต่ต้องแสดงให้เห็น
                    }

                }
                catch (Exception ex)
                {
                    result = "";
                }
            }
        }




        return result;

    }


    //-- aor edit 27/06/2019
    //ตรวจสอบว่าเป็นตัวเลขด้วย
    public static String FormatCheckNumNoComma(Object val, int dec = 0)
    {
        string result = "";
        string D = "";
        if (val != null)
        {
            D = (val + "").ToString();
            if (D + "" != "")
            {
                //-- EDIT 25/06/2019 ---  <0.40 กรณีมีเครื่องหมาย ให้เอาเครื่องหมายออก เพื่อจะได้ plot graph ได้
                if (D.IndexOf("<") > -1 || D.IndexOf(">") > -1 || D.IndexOf("<=") > -1 || D.IndexOf(">=") > -1)
                {
                    D = D.Replace("<", "").Replace(">", "").Replace("=", "");
                }

                try
                {
                    if (IsNumeric(D))
                    {
                        decimal temp = decimal.Parse(D);
                        switch (dec)
                        {
                            case 0: result = String.Format("{0:#0}", temp); break;
                            case 1: result = String.Format("{0:#0.0}", temp); break;
                            case 2: result = String.Format("{0:#0.00}", temp); break;
                            case 3: result = String.Format("{0:#0.000}", temp); break;
                            case 4: result = String.Format("{0:#0.0000}", temp); break;
                            case 5: result = String.Format("{0:#0.00000}", temp); break;
                            case 6: result = String.Format("{0:#0.000000}", temp); break;
                            case 7: result = String.Format("{0:#0.0000000}", temp); break;
                        }
                    }
                    else
                    {
                       result = "";  //-- EDIT 11/07/2018 --- ระบบนี้ค่าก๊าซบางครั้งอาจไม่ใช่ตัวเลข แต่ต้องแสดงให้เห็น

                       
                    }

                }
                catch (Exception ex)
                {
                    result = "";
                }
            }
        }




        return result;

    }



    public static String TimeHH(String txt, String code)
    {
        String ret = "";
        if (txt == "")
        {
            ret = "";
        }
        else
        {
            int i = txt.IndexOf(code);
            ret = Left(txt, i);
        }
        return ret;
    }


    public static String TimeMM(String txt, String code)
    {
        String ret = "";
        if (txt == "")
        {
            ret = "";
        }
        else
        {
            int i = txt.IndexOf(code);
            ret = Right(txt, i);
        }
        return ret;
    }

    public static void SetHHMM(DropDownList ddlHH, DropDownList ddlMM, string time, char Separator = ':')
    {
        try
        {
            if (time != "")
            {
                string[] item = time.Split(Separator);
                if (item.Length >= 2)
                {
                    SetListValue(ref ddlHH, item[0]);
                    if (item[1].Length > 2) item[1] = item[1].Substring(0, 2);
                    SetListValue(ref ddlMM, item[1]);
                }
            }
        }
        catch (Exception ex)
        {
            if (ddlHH.Items.Count > 0) ddlHH.SelectedIndex = 0;
            if (ddlMM.Items.Count > 0) ddlMM.SelectedIndex = 0;
        }
    }

    static public bool CheckTime(TimeSpan startTime, TimeSpan endTime)
    {
        if (endTime >= startTime)
        {
            return true;
        }
        else
        {
            return false;
        }

    }



    #endregion

    #region String
    public static String Left(String txt, Int32 length)
    {
        String ret = "";
        if (length > 0)
        {
            if (length == 0 || txt.Length == 0)
            {
                ret = "";
            }
            else if (txt.Length <= length)
            {
                ret = txt;
            }
            else
            {
                ret = txt.Substring(0, length);
            }
        }
        return ret;
    }

    public static String Right(String txt, Int32 length)
    {
        String ret = "";
        if (length > 0)
        {
            if (length == 0 || txt.Length == 0)
            {
                ret = "";
            }
            else if (txt.Length <= length)
            {
                ret = txt;
            }
            else
            {
                ret = txt.Substring(txt.Length - length, length);
            }
        }
        return ret;
    }

    public static string Mid(string txt, int start, int end)
    {
        return txt.Substring(start, end);
    }

    public static string Mid(string txt, int start)
    {
        return txt.Substring(start, txt.Length - start);
    }

    public static string Chr(int n)
    {
        return Utility.ToString((char)n);
    }


    public static string UCase(string txt)
    {
        String ret = "";
        if (txt != "")
        {
            ret = txt.ToUpper();
        }
        return ret;
    }

    public static string LCase(string txt)
    {
        String ret = "";
        if (txt != "")
        {
            ret = txt.ToLower();
        }
        return ret;
    }
    #endregion

    #region Number
    public static int RandomNumber(int min, int max)
    {
        Random random = new Random();
        return random.Next(min, max);
    }

    public static Decimal ToNum(Object N)
    {
        Decimal num;
        try
        {
            num = Convert.ToDecimal(N);
        }
        catch
        {
            num = 0;
        }
        return num;
    }

    public static Int32 ToInt(Object N)
    {
        Int32 num;

        try
        {
            num = Convert.ToInt32(N);
        }
        catch
        {
            num = 0;
        }
        return num;
    }

    public static Int64 ToLong(Object N)
    {
        Int64 num;

        try
        {
            num = Convert.ToInt64(N);
        }
        catch
        {
            num = 0;
        }
        return num;
    }

    public static Double ToDouble(Object N)
    {
        Double num;

        try
        {
            num = Convert.ToDouble(N);
        }
        catch
        {
            num = 0;
        }
        return num;
    }


    public static String GetFileType(String FileName)
    {
        try
        {
            if (FileName != "")
            {
                //FileName = Mid(FileName, 1, FileName.LastIndexOf("\\") + 1);
                FileName = Mid(FileName, FileName.LastIndexOf("."));
                return FileName;
            }
            else
            {
                return "";
            }
        }
        catch
        {
            return "";
        }
    }

    public static String GetFileName(String FileName)
    {
        try
        {
            if (FileName != "")
            {
                int idx = FileName.LastIndexOf("\\");
                //if (idx < 0) idx = 0;
                //return FileName.Substring(idx);
                //-- aor edit 18/07/2018
                return FileName.Substring(idx+1);
            }
            else
            {
                return "";
            }
        }
        catch
        {
            return "";
        }
    }

    public static Boolean IsDate(object Expression)
    {
        bool isDate;
        DateTime retDate;

        if (Expression != null)
        {
            isDate = (AppDateValue(Expression) != null);
        }
        else
        {
            isDate = false;
        }
        try
        {

        }
        catch (Exception ex)
        {
            isDate = false;
        }

        return isDate;
    }

    //-- edit 17/07/2018 ---
    public static Boolean IsExcelDate(object Expression)
    {
        bool isDate;
        if (Expression != null && Utility.ToString(Expression).Length >8 )
        {

            String Value = Expression.ToString();
            String ChkDate = "";
            //-- ตรวจสอบเรื่องวันที่ dd/mm/yyyy, mm/dd/yyyy
            //-- ระบบนี้เป็น ค.ศ. แต่บางครั้ง excel อ่านมาเป็น พ.ศ.
            if (Value.Split('/').Length < 2)
                isDate = false;
            else
            {
                String yyyy = Value.Split('/')[2];
                if (yyyy.Length > 4) yyyy = Utility.Left(yyyy, 4);
                if (Utility.ToNum(yyyy) > 2500)
                {
                    yyyy = Utility.ToString(Utility.ToNum(yyyy) - 543);
                }

                if (Project.gXLSDateFormat == "mm/dd/yyyy")
                {
                    ChkDate = Value.Split('/')[1].PadLeft(2, '0') + "/" + Value.Split('/')[0].PadLeft(2, '0') + "/" + yyyy;
                }
                else {
                    ChkDate = Value.Split('/')[0].PadLeft(2, '0') + "/" + Value.Split('/')[1].PadLeft(2, '0') + "/" + yyyy;
                }

                isDate = (AppDateValue(ChkDate) != null);
            }




        }
        else
        {
            isDate = false;
        }
        try
        {

        }
        catch (Exception ex)
        {
            isDate = false;
        }

        return isDate;
    }

    public static Boolean IsDBNull(dynamic value)
    {
        return (value == DBNull.Value);
    }

    public static Boolean IsNumeric(object Expression)
    {
        bool isNum;
        double retNum;

        if (Expression != null)
        {
            isNum = Double.TryParse(Convert.ToString(Expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
        }
        else
        {
            isNum = false;
        }
        return isNum;
    }

    public static Boolean IsOdd(int value)
    {
        return value % 2 != 0;
    }

    public static Boolean IsEven(int value)
    {
        return value % 2 == 0;
    }
    #endregion

    #region Arrays

    public static bool FindValueFromArray(object[] Values, object valueToSearch)
    {
        bool retVal = false;
        Array myArray = (Array)Values;
        if (Array.IndexOf(myArray, valueToSearch) != -1)
        {
            retVal = true;
        }
        return retVal;
    }

    #endregion

    #region Init Controls

    public static void LoadHourCombo(ref DropDownList cbo, Boolean incBlank, String defaultHour, String blankText)
    {
        Int32 i;

        cbo.Items.Clear();
        if (incBlank) { cbo.Items.Add(new ListItem(blankText, "")); }

        for (i = 0; i <= 23; i++)
        {
            cbo.Items.Add(new ListItem(Project.FormatData(i.ToString(), DefaultShow: "2"), i.ToString()));
        }

        if (defaultHour != "")
        {
            cbo.SelectedValue = defaultHour;
        }
        else
        {
            cbo.SelectedIndex = -1;
        }
    }

    public static void LoadMinuteCombo(ref DropDownList cbo, Boolean incBlank, String defaultHour, String blankText)
    {
        Int32 i;

        cbo.Items.Clear();
        if (incBlank) { cbo.Items.Add(new ListItem(blankText, "")); }

        for (i = 0; i <= 59; i++)
        {
            cbo.Items.Add(new ListItem(Project.FormatData(i.ToString(), DefaultShow: "2"), i.ToString()));
        }

        if (defaultHour != "")
        {
            cbo.SelectedValue = defaultHour;
        }
        else
        {
            cbo.SelectedIndex = -1;
        }
    }

    public static void LoadMonthCombo(ref DropDownList cbo)
    {
        LoadMonthCombo(ref cbo, false, "", "EN", "");
    }

    public static void LoadMonthCombo(ref DropDownList cbo, Boolean incBlank, String defaultMonth, String langType, String blankText)
    {
        Int32 i;

        cbo.Items.Clear();
        if (incBlank) { cbo.Items.Add(new ListItem(blankText, "")); }
        if (langType == "TH")
        {
            for (i = 1; i <= 12; i++)
            {
                cbo.Items.Add(new ListItem(ThMonth(i), i.ToString()));
            }
        }
        else
        {
            for (i = 1; i <= 12; i++)
            {
                cbo.Items.Add(new ListItem(EnMonth(i), i.ToString()));
            }
        }
        if (defaultMonth != "")
        {
            cbo.SelectedValue = defaultMonth;
        }
        else
        {
            cbo.SelectedIndex = -1;
        }
    }

    public static void LoadYearCombo(ref DropDownList cbo)
    {
        LoadYearCombo(ref cbo, false, "","", "EN", "");
    }

    //-- edit 24/08/2018 --
    public static void LoadYearCombo(ref DropDownList cbo, String startYear)
    {
        LoadYearCombo(ref cbo, false, startYear , "", "EN", "");
    }

    public static void LoadYearCombo(ref DropDownList cbo, Boolean incBlank, String startYear, String defaultYear, String langType, String blankText)
    {
        Int32 i;
        Int32 num = 10; //ย้อนหลัง 10 ปี
        Int32 sYear, bYear, eYear;

        cbo.Items.Clear();
        if (incBlank) { cbo.Items.Add(new ListItem(blankText, "")); }
        sYear = System.DateTime.Now.Year;
        if (langType == "TH")
        {
            if (sYear < 2500) { sYear += 543; }
        }
        else
        {
            if (sYear > 2500) { sYear -= 543; }
        }
        if (startYear != "")
            bYear = Utility.ToInt(startYear);
        else
            bYear = sYear - num;

        eYear = sYear;

        i = eYear;
        do
        {
            if (langType == "TH")
            {
                cbo.Items.Add(new ListItem((i + 543).ToString(), i.ToString()));
            }
            else
            {
                cbo.Items.Add(new ListItem((i).ToString(), i.ToString()));
            }
            i -= 1;
        } while (i >= bYear);

        if (defaultYear != "")
        {
            cbo.SelectedValue = defaultYear;
        }
        else
        {
            cbo.SelectedIndex = -1;
        }
    }

    public static void LoadList(ref ListBox cbo, DataTable dt, String descField, String valueField)
    {
        LoadList(ref cbo, dt, descField, valueField, false, "");
    }

    public static void LoadList(ref ListBox cbo, DataTable dt, String descField, String valueField, Boolean incBlank, String blankText, Boolean IsReadOnly = false)
    {
        DataRow dr = null;

        try
        {
            cbo.Items.Clear();
            if (dt != null)
            {
                if (incBlank)
                {
                    dr = dt.NewRow();
                    dr[descField] = blankText;
                    dt.Rows.InsertAt(dr, 0);
                }

                cbo.DataTextField = descField;
                cbo.DataValueField = valueField;
                cbo.DataSource = dt;
                cbo.DataBind();


                if (IsReadOnly == true)
                {
                    cbo.Enabled = false;
                    if (cbo.CssClass.IndexOf("txtReadOnly") < 0) cbo.CssClass += " txtReadOnly";
                }
            }
            else
            {
                cbo.DataSource = null;
                cbo.DataBind();
            }
        }
        catch
        {
        }
    }

    /// //////////////////////////////

    public static void LoadList(ref DropDownList cbo, DataTable dt, String descField, String valueField)
    {
        LoadList(ref cbo, dt, descField, valueField, false, "");
    }

    public static void LoadList(ref DropDownList cbo, DataTable dt, String descField, String valueField, Boolean incBlank, String blankText, Boolean IsReadOnly = false)
    {
        DataRow dr = null;

        try
        {
            cbo.Items.Clear();
            if (dt != null)
            {
                if (incBlank)
                {
                    //-- edit 19/07/2019 --- บางครั้งต้องการแสดงเป็น none แต่ value ให้เป็น "" แต่บังเอิญใช้ field เดียวกัน
                    if ( descField == valueField && blankText != "")
                    {
                        dt.Columns.Add(descField + "D", typeof(String));
                        foreach (DataRow dr1 in dt.Rows)
                        {
                            dr1[descField + "D"] = dr1[descField];
                        }
                        descField = descField + "D";
                    }

                    dr = dt.NewRow();
                    dr[descField] = blankText;
                    dt.Rows.InsertAt(dr, 0);
                }

                cbo.DataTextField = descField;
                cbo.DataValueField = valueField;
                cbo.DataSource = dt;
                cbo.DataBind();


                if (IsReadOnly == true)
                {
                    cbo.Enabled = false;
                    if (cbo.CssClass.IndexOf("txtReadOnly") < 0) cbo.CssClass += " txtReadOnly";
                }
            }
            else
            {
                cbo.DataSource = null;
                cbo.DataBind();
            }
        }
        catch
        {
        }
    }

    public static void LoadList(ref RadioButtonList cbo, DataTable dt, String descField, String valueField, Boolean incBlank, String blankText)
    {
        DataRow dr = null;

        try
        {
            cbo.Items.Clear();
            if (dt != null)
            {
                if (incBlank)
                {
                    dr = dt.NewRow();
                    dr[descField] = blankText;
                    dt.Rows.InsertAt(dr, 0);
                }

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    ListItem li = new ListItem(dt.Rows[i][descField].ToString(), dt.Rows[i][valueField].ToString());
                    cbo.Items.Add(li);
                }
            }
        }
        catch
        {
        }
    }

    public static void LoadList(ref CheckBoxList cbo, DataTable dt, String descField, String valueField, Boolean incBlank, String blankText)
    {
        DataRow dr = null;

        try
        {

            if (dt != null)
            {
                if (incBlank)
                {
                    dr = dt.NewRow();
                    dr[descField] = blankText;
                    dt.Rows.InsertAt(dr, 0);
                }

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    ListItem li = new ListItem(dt.Rows[i][descField].ToString(), dt.Rows[i][valueField].ToString());
                    cbo.Items.Add(li);
                }
            }
        }
        catch
        {
        }
    }

    public static void LoadList(ref BulletedList cbo, DataTable dt, String descField, String valueField, Boolean incBlank, String blankText)
    {
        DataRow dr = null;

        try
        {

            if (dt != null)
            {
                if (incBlank)
                {
                    dr = dt.NewRow();
                    dr[descField] = blankText;
                    dt.Rows.InsertAt(dr, 0);
                }

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    ListItem li = new ListItem(dt.Rows[i][descField].ToString(), dt.Rows[i][valueField].ToString());
                    cbo.Items.Add(li);
                }

            }
        }
        catch
        {
        }
    }



    public static void BindGVData(ref GridView GV, object dataSource, bool AddBlankRow)
    {
        DataSet DS = null;
        DataTable DT = null;
        DataTable DT2 = null;
        DataView DV;
        try
        {
            if (GV != null && dataSource != null)
            {
                switch (dataSource.GetType().ToString().ToUpper())
                {
                    case "SYSTEM.DATA.DATASET":
                        DS = (DataSet)dataSource;
                        if (DS.Tables.Count > 0)
                        {
                            DT = DS.Tables[0];
                        }
                        if (AddBlankRow == true && DT.Rows.Count == 0)
                        {
                            DT2 = DT.Copy();
                            DT2.Rows.Add(DT2.NewRow());
                            GV.DataSource = DT2;
                        }
                        GV.DataSource = DT;
                        GV.DataBind();
                        break;
                    case "SYSTEM.DATA.DATATABLE":
                        DT = (DataTable)dataSource;
                        if (AddBlankRow == true && DT.Rows.Count == 0)
                        {
                            DT2 = DT.Copy();
                            DT2.Rows.Add(DT2.NewRow());
                            GV.DataSource = DT2;
                            GV.DataBind();
                            GV.Rows[0].Visible = false;
                        }
                        else
                        {
                            GV.DataSource = DT;
                            GV.DataBind();
                        }
                        break;
                    case "SYSTEM.DATA.DATAVIEW":
                        DV = (DataView)dataSource;
                        if (AddBlankRow == true && DV.Count == 0)
                        {
                            DV.AddNew();
                        }

                        GV.DataSource = DV;
                        GV.DataBind();
                        break;
                }
                if (GV.FooterRow != null && GV.ShowFooter == true)
                {
                    //GV.FooterRow.Visible = !IsReadOnly;
                }
            }
            //else if (AddBlankRow) {
            //    GV.ShowFooter = true;
            //    GV.DataSource = dataSource;
            //    GV.DataBind();
            //}
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public static string FormatLongText(string Text, int MaxLen, string act = "")
    {
        string result = Text;

        if (Text.Length > MaxLen)
        {
            result = "<a title='" + Text + "'";
            if (act != "") result += @" onclick=""" + act + @"""";
            result += ">" + Text.Substring(0, MaxLen) + "...</a>";
        }

        return (result);
    }

    #endregion

    #region Controls

    public static string GetCtrl(Control ctrl, string DefaultValue = "", bool IsHTMLEncode = false)
    {
        string result = "";
        Label lb = null;
        TextBox txt = null;
        DropDownList cbo = null;
        CheckBox chk = null;
        HiddenField hdn = null;
        RadioButton rb = null;
        RadioButtonList rbl = null;
        HtmlInputText htmlText = null;
        HtmlInputPassword htmlPass = null;
        HtmlTextArea htmlTxtArea = null;
        HtmlSelect htmlSelect = null;
        try
        {
            result = DefaultValue;

            switch (ctrl.GetType().Name.ToUpper())
            {
                case "CHECKBOX":
                    chk = (CheckBox)ctrl;
                    result = (chk.Checked) ? chk.Text : "";
                    break;
                case "DROPDOWNLIST":
                    cbo = (DropDownList)ctrl;
                    result = cbo.SelectedValue;
                    break;
                case "LABEL":
                    lb = (Label)ctrl;
                    result = lb.Text;
                    break;
                case "TEXTBOX":
                    txt = (TextBox)ctrl;
                    result = txt.Text;
                    break;
                case "HIDDENFIELD":
                    hdn = (HiddenField)ctrl;
                    result = hdn.Value;
                    break;
                case "RADIOBUTTONLIST":
                    rbl = (RadioButtonList)ctrl;
                    result = rbl.SelectedValue;
                    break;
                case "HTMLINPUTTEXT":
                    htmlText = (HtmlInputText)ctrl;
                    result = htmlText.Value;
                    break;
                case "HTMLINPUTPASSWORD":
                    htmlPass = (HtmlInputPassword)ctrl;
                    result = htmlPass.Value;
                    break;
                case "HTMLTEXTAREA":
                    htmlTxtArea = (HtmlTextArea)ctrl;
                    result = htmlTxtArea.Value;
                    break;
                case "HTMLSELECT":
                    htmlSelect = (HtmlSelect)ctrl;
                    result = htmlSelect.Value[htmlSelect.SelectedIndex].ToString();
                    break;
            }

            //-- aor edit 05/07/2017 --
            result = result.Trim();
        }
        catch
        {
            result = DefaultValue;
        }

        if (IsHTMLEncode)
        {
            result = HttpContext.Current.Server.HtmlEncode(result);
        }

        return result;
    }

    public static string GetListText(ListControl ctrl)
    {
        if (ctrl.SelectedItem != null)
        {
            return (ctrl.SelectedItem.Text);
        }
        else
        {
            return ("");
        }
    }

    public static void SetCtrl(Control ctrl, string value, bool IsReadOnly = false, bool IsHTMLEncode = false)
    {
        Label lb = null;
        TextBox txt = null;
        HtmlInputText htmlTxt = null;
        HtmlTextArea htmlTxtArea = null;
        DropDownList cbo = null;
        RadioButton rdb = null;
        RadioButtonList rbl = null;
        CheckBox chk = null;
        CheckBoxList chl = null;
        HiddenField hdn = null;
        String ctrlType;

        try
        {
            if (!string.IsNullOrEmpty(value))
            {
                if (IsHTMLEncode) { value = HttpContext.Current.Server.HtmlEncode(value); }
            }
            else
            {
                value = "";
            }


            ctrlType = ctrl.GetType().Name.ToUpper();
            if (IsReadOnly)
            {
                switch (ctrlType)
                {
                    case "LABEL":
                        lb = (Label)ctrl;
                        lb.Text = value;
                        if (lb.CssClass.IndexOf("txtReadOnly") < 0) lb.CssClass += " txtReadOnly";
                        break;
                    case "TEXTBOX":
                        txt = (TextBox)ctrl;
                        txt.Text = value;
                        txt.Enabled = false;
                        if (txt.CssClass.IndexOf("txtReadOnly") < 0) txt.CssClass += " txtReadOnly";
                        // txt.Attributes.Add("ReadOnly", "readonly");
                        break;
                    case "HTMLINPUTTEXT":
                        htmlTxt = (HtmlInputText)ctrl;
                        htmlTxt.Value = value;
                        htmlTxt.Disabled = true;
                        SetAttributeCtrl(htmlTxt, "class", "form-control txtReadOnly");
                        break;
                    case "DROPDOWNLIST":
                        cbo = (DropDownList)ctrl;
                        cbo.Enabled = false;
                        if (cbo.CssClass.IndexOf("txtReadOnly") < 0) cbo.CssClass += " txtReadOnly";
                        SetListValue(ref cbo, value.ToString());
                        break;
                    case "RADIOBUTTON":
                        rdb = (RadioButton)ctrl;
                        rdb.Enabled = false;
                        //if (rdb.CssClass.IndexOf("txtReadOnly") < 0) rdb.CssClass += " txtReadOnly";
                        rdb.Checked = (value.ToLower() == "true");
                        break;
                    case "RADIOBUTTONLIST":
                        rbl = (RadioButtonList)ctrl;
                        rbl.Enabled = false;
                        //if (rbl.CssClass.IndexOf("txtReadOnly") < 0) rbl.CssClass += " txtReadOnly";
                        SetListValue(ref rbl, value.ToString());
                        break;
                    case "CHECKBOX":
                        chk = (CheckBox)ctrl;
                        chk.Enabled = false;
                        //if (chk.CssClass.IndexOf("txtReadOnly") < 0) chk.CssClass += " txtReadOnly";
                        chk.Checked = (value.ToLower() == "true");
                        break;
                    case "CHECKBOXLIST":
                        chl = (CheckBoxList)ctrl;
                        chl.Enabled = false;
                        //if (chl.CssClass.IndexOf("txtReadOnly") < 0) chl.CssClass += " txtReadOnly";
                        SetCheckBoxList(ref chl, value);
                        break;
                    case "HIDDENFIELD":
                        hdn = (HiddenField)ctrl;
                        hdn.Value = value;
                        break;
                    case "HTMLTEXTAREA":
                        htmlTxtArea = (HtmlTextArea)ctrl;
                        htmlTxtArea.Value = value;

                        break;
                }
            }
            else
            {
                switch (ctrlType)
                {
                    case "LABEL":
                        lb = (Label)ctrl;
                        lb.Text = value.ToString();
                        lb.CssClass = lb.CssClass.Replace(" txtReadOnly", "").Replace("txtReadOnly", "");
                        break;
                    case "TEXTBOX":
                        txt = (TextBox)ctrl;
                        txt.Text = value.ToString();
                        txt.CssClass = txt.CssClass.Replace(" txtReadOnly", "").Replace("txtReadOnly", "");
                        txt.ReadOnly = false;
                        txt.Enabled = true;
                        break;
                    case "HTMLINPUTTEXT":
                        htmlTxt = (HtmlInputText)ctrl;
                        htmlTxt.Value = value;
                        htmlTxt.Disabled = false;
                        break;
                    case "DROPDOWNLIST":
                        cbo = (DropDownList)ctrl;
                        cbo.Enabled = true;
                        cbo.CssClass = cbo.CssClass.Replace(" txtReadOnly", "").Replace("txtReadOnly", "");
                        SetListValue(ref cbo, value.ToString());
                        break;
                    case "RADIOBUTTON":
                        rdb = (RadioButton)ctrl;
                        rdb.Enabled = true;
                        rdb.CssClass = rdb.CssClass.Replace(" txtReadOnly", "").Replace("txtReadOnly", "");
                        rdb.Checked = (value.ToLower() == "true");
                        break;
                    case "RADIOBUTTONLIST":
                        rbl = (RadioButtonList)ctrl;
                        rbl.Enabled = true;
                        rbl.CssClass = rbl.CssClass.Replace(" txtReadOnly", "").Replace("txtReadOnly", "");
                        SetListValue(ref rbl, value.ToString());
                        break;
                    case "CHECKBOX":
                        chk = (CheckBox)ctrl;
                        chk.Enabled = true;
                        chk.CssClass = chk.CssClass.Replace(" txtReadOnly", "").Replace("txtReadOnly", "");
                        chk.Checked = (value.ToLower() == "true");
                        break;
                    case "CHECKBOXLIST":
                        chl = (CheckBoxList)ctrl;
                        chl.Enabled = true;
                        chl.CssClass = chl.CssClass.Replace(" txtReadOnly", "").Replace("txtReadOnly", "");
                        SetCheckBoxList(ref chl, value);
                        break;
                    case "HIDDENFIELD":
                        hdn = (HiddenField)ctrl;
                        hdn.Value = value;
                        break;
                    case "HTMLTEXTAREA":
                        htmlTxtArea = (HtmlTextArea)ctrl;
                        htmlTxtArea.Value = value;
                        htmlTxtArea.Disabled = false;
                        break;
                }

            }

        }
        catch (Exception ex)
        {

        }

    }

    //-- aor edit 19/03/2018 ---
    public static void SetCtrlReadOnly(Control ctrl)
    {
        TextBox txt = null;
        HtmlInputText htmlTxt = null;
        DropDownList cbo = null;
        RadioButton rdb = null;
        RadioButtonList rbl = null;
        CheckBox chk = null;
        CheckBoxList chl = null;
        String ctrlType;

        try
        {

            ctrlType = ctrl.GetType().Name.ToUpper();

            switch (ctrlType)
            {
                case "TEXTBOX":
                    txt = (TextBox)ctrl;
                    txt.Enabled = false;
                    if (txt.CssClass.IndexOf("txtReadOnly") < 0) txt.CssClass += " txtReadOnly";
                    break;
                case "HTMLINPUTTEXT":
                    htmlTxt = (HtmlInputText)ctrl;
                    htmlTxt.Disabled = true;
                    SetAttributeCtrl(htmlTxt, "class", "form-control txtReadOnly");
                    break;
                case "DROPDOWNLIST":
                    cbo = (DropDownList)ctrl;
                    cbo.Enabled = false;
                    if (cbo.CssClass.IndexOf("txtReadOnly") < 0) cbo.CssClass += " txtReadOnly";
                    break;
                case "RADIOBUTTON":
                    rdb = (RadioButton)ctrl;
                    rdb.Enabled = false;
                    //if (rdb.CssClass.IndexOf("txtReadOnly") < 0) rdb.CssClass += " txtReadOnly";
                    break;
                case "RADIOBUTTONLIST":
                    rbl = (RadioButtonList)ctrl;
                    rbl.Enabled = false;
                    //if (rbl.CssClass.IndexOf("txtReadOnly") < 0) rbl.CssClass += " txtReadOnly";
                    break;
                case "CHECKBOX":
                    chk = (CheckBox)ctrl;
                    chk.Enabled = false;
                    //if (chk.CssClass.IndexOf("txtReadOnly") < 0) chk.CssClass += " txtReadOnly";
                    break;
                case "CHECKBOXLIST":
                    chl = (CheckBoxList)ctrl;
                    chl.Enabled = false;
                    //if (chl.CssClass.IndexOf("txtReadOnly") < 0) chl.CssClass += " txtReadOnly";
                    break;

            }
 
 

        }
        catch (Exception ex)
        {

        }

    }

    //-- EDIT 03/07/2019 ---
    public static void SetListValue(ref ListBox ctrl, String selectedValue, bool IsReadOnly = false)
    {
        try
        {
            //ctrl.SelectedValue = selectedValue;
            // Ignore case sensitive
            ctrl.ClearSelection();
            string searchVal = selectedValue.Trim().ToLower().Replace(" ", "");

            foreach (ListItem li in ctrl.Items)
            {
                if (searchVal.IndexOf(li.Value.Trim().ToLower().Replace(" ", "")) > -1)
                {
                    li.Selected = true;

                }
            }


            if (IsReadOnly)
            {
                ctrl.Enabled = false;
                ctrl.CssClass += " txtReadOnly";
                // SetListValue(ref ctrl, selectedValue.ToString());
            }
        }
        catch
        {
            ctrl.SelectedIndex = -1;
        }
    }


    public static void SetListValue(ref DropDownList ctrl, String selectedValue, bool IsReadOnly = false)
    {
        try
        {
            ctrl.SelectedValue = selectedValue;
            if (IsReadOnly)
            {
                ctrl.Enabled = false;
                ctrl.CssClass += " txtReadOnly";
                SetListValue(ref ctrl, selectedValue.ToString());
            }
        }
        catch
        {
            ctrl.SelectedIndex = -1;
        }
    }

    public static void SetListValue(ref RadioButtonList ctrl, String selectedValue)
    {
        try
        {
            ctrl.SelectedValue = selectedValue;
        }
        catch
        {
            ctrl.SelectedIndex = -1;
        }
    }

    public static void SetCheckBoxList(ref CheckBoxList ctrl, String pValue, String DefaultSymbol = ",")
    {
        String[] valueList;
        try
        {
            valueList = pValue.Split(DefaultSymbol[0]);

            foreach (ListItem li in ctrl.Items)
            {
                li.Selected = (valueList.Contains(li.Value));
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public void SetDateCtrl(ref TextBox Ctrl, object value, bool IsReadOnly = false)
    {
        if (IsReadOnly)
        {
            Ctrl.Attributes.Add("ReadOnly", "true");
            Ctrl.CssClass = "txtReadOnly";
            Ctrl.Text = Utility.ToString(value);
        }
    }


    public static void SetCtrlHistory(Control ctrl, string value, bool IsReadOnly = false, bool IsHTMLEncode = false)
    {
        Label lb = null;
        TextBox txt = null;
        DropDownList cbo = null;
        RadioButton rdb = null; RadioButtonList rbl = null;
        CheckBox chk = null; CheckBoxList chl = null;
        String ctrlType;

        try
        {
            if (!string.IsNullOrEmpty(value))
            {
                if (IsHTMLEncode) { value = HttpContext.Current.Server.HtmlEncode(value); }
            }
            else
            {
                value = "";
            }


            ctrlType = ctrl.GetType().Name.ToUpper();
            if (IsReadOnly)
            {
                switch (ctrlType)
                {
                    case "LABEL":
                        lb = (Label)ctrl;

                        lb.CssClass = "EditCtrl";
                        break;
                    case "TEXTBOX":
                        txt = (TextBox)ctrl;
                        txt.CssClass = "EditCtrl";
                        txt.ReadOnly = true;
                        break;
                    case "DROPDOWNLIST":
                        cbo = (DropDownList)ctrl;
                        cbo.Enabled = false;
                        cbo.CssClass = "EditCtrl";
                        SetListValue(ref cbo, value.ToString());
                        break;
                    case "RADIOBUTTON":
                        rdb = (RadioButton)ctrl;
                        rdb.Enabled = false;
                        rdb.CssClass = "EditCtrl";
                        break;
                    case "RADIOBUTTONLIST":
                        rbl = (RadioButtonList)ctrl;
                        rbl.Enabled = false;
                        rbl.CssClass = "EditCtrl";
                        SetListValue(ref rbl, value.ToString());
                        break;
                    case "CHECKBOX":
                        chk = (CheckBox)ctrl;
                        chk.Enabled = false;
                        chk.Attributes.Add("class", "EditCtrl");
                        break;
                    case "CHECKBOXLIST":
                        chl = (CheckBoxList)ctrl;
                        chl.Enabled = false;
                        chl.Attributes.Add("class", "EditCtrl");
                        break;
                }
            }
            else
            {
                switch (ctrlType)
                {
                    case "LABEL":
                        lb = (Label)ctrl;
                        lb.Text = value.ToString();
                        lb.CssClass = "";
                        break;
                    case "TEXTBOX":
                        txt = (TextBox)ctrl;
                        txt.Text = value.ToString();
                        txt.CssClass = "";
                        txt.ReadOnly = false;
                        break;
                    case "DROPDOWNLIST":
                        cbo = (DropDownList)ctrl;
                        cbo.Enabled = true;
                        cbo.CssClass = "";
                        SetListValue(ref cbo, value.ToString());
                        break;
                    case "RADIOBUTTON":
                        rdb = (RadioButton)ctrl;
                        rdb.Enabled = true;
                        rdb.CssClass = "";
                        break;
                    case "RADIOBUTTONLIST":
                        rbl = (RadioButtonList)ctrl;
                        rbl.Enabled = true;
                        rbl.CssClass = "";
                        break;
                    case "CHECKBOX":
                        chk = (CheckBox)ctrl;
                        chk.Enabled = true;
                        chk.Attributes.Add("class", "");
                        break;
                    case "CHECKBOXLIST":
                        chl = (CheckBoxList)ctrl;
                        chl.Enabled = true;
                        chl.Attributes.Add("class", "");
                        break;
                }

            }

        }
        catch
        {
        }

    }

    //-- aor edit 06/12/2016 --
    public static void ShowLastUpdate(Control ctrl, string uCreate, object dCreate, string uUpdate, object dUpdate)
    {
        String show = "";
        try
        {
            show = "Created Date: " + Utility.AppFormatDateTime(dCreate) + "    By: " + uCreate ;
            if ( uUpdate != "")
            {
                show += "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Updated Date: " + Utility.AppFormatDateTime(dUpdate) + "    By: " + uUpdate;
            }
            SetCtrl(ctrl, show);

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    //-- aor edit 30/01/2017 --
    public static void ShowLastUpdate(Control ctrl, string uCreate, object dCreate, string uUpdate1, object dUpdate1, string uUpdate2, object dUpdate2)
    {
        String show = "";
        try
        {
            show = "Created Date: " + Utility.AppFormatDateTime(dCreate) + "    By: " + uCreate;

            if (dUpdate1 is DateTime && dUpdate2 is DateTime)
            {
                if ((DateTime)dUpdate1 > (DateTime)dUpdate2)
                {
                    show += "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Updated Date: " + Utility.AppFormatDateTime(dUpdate1) + "    By: " + uUpdate1;
                }
                else
                {
                    show += "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Updated Date: " + Utility.AppFormatDateTime(dUpdate2) + "    By: " + uUpdate2;
                }
            }
            else
            {
                if (dUpdate1 is DateTime )
                {
                    show += "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Updated Date: " + Utility.AppFormatDateTime(dUpdate1) + "    By: " + uUpdate1;                   
                }
            }

            SetCtrl(ctrl, show);

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    //-- edit 03/07/2019 --
    public static string GetListBoxValue(ListBox ctrl, String DefaultSymbol = ",")
    {
        String[] valueList;
        int i = 0;
        String valuesReturn = "";
        try
        {
            valueList = new string[ctrl.Items.Count];
            foreach (ListItem li in ctrl.Items)
            {
                if (li.Selected == true)
                {
                    valueList[i] = Validation.ValidateStr(li.Value);
                    i++;
                }
            }
            Array.Resize(ref valueList, i);
            valuesReturn = String.Join(DefaultSymbol, valueList);
            return valuesReturn;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }




    public static string GetCheckBoxListValue(CheckBoxList ctrl, String DefaultSymbol = ",")
    {
        String[] valueList;
        int i = 0;
        String valuesReturn = "";
        try
        {
            valueList = new string[ctrl.Items.Count];
            foreach (ListItem li in ctrl.Items)
            {
                if (li.Selected == true)
                {
                    valueList[i] = Validation.ValidateStr(li.Value);
                    i++;
                }
            }
            Array.Resize(ref valueList, i);
            valuesReturn = String.Join(DefaultSymbol, valueList);
            return valuesReturn;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public static string GetCheckBoxListText(CheckBoxList ctrl, String DefaultSymbol = ",")
    {
        String[] valueList;
        int i = 0;
        String valuesReturn = "";
        try
        {
            valueList = new string[ctrl.Items.Count];
            foreach (ListItem li in ctrl.Items)
            {
                if (li.Selected == true)
                {
                    valueList[i] = Validation.ValidateStr(li.Text);
                    i++;
                }
            }
            Array.Resize(ref valueList, i);
            valuesReturn = String.Join(DefaultSymbol, valueList);
            return valuesReturn;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    public static String FormatCurrency(Object val)
    {
        String ret = (val + "").ToString();

        ret = ret.Replace(",", "");

        return ret;
    }
    #endregion

    #region Lock Control

    public static void LockCtrl(Control ctrl)
    {
        Label lb = null;
        TextBox txt = null;
        DropDownList cbo = null;
        RadioButton rdb = null; RadioButtonList rbl = null;
        CheckBox chk = null; CheckBoxList chl = null;
        HtmlInputText htmltxt = null;

        try
        {
            switch (ctrl.GetType().Name.ToUpper())
            {
                case "LABEL":
                    lb = (Label)ctrl;
                    //if (lb.CssClass.IndexOf("txtReadOnly")<0) lb.CssClass += " txtReadOnly";
                    break;
                case "HTMLINPUTTEXT":
                    htmltxt = (HtmlInputText)ctrl;
                    htmltxt.Disabled = true;
                    break;
                case "TEXTBOX":
                    txt = (TextBox)ctrl;
                    if (txt.CssClass.IndexOf("txtReadOnly") < 0) txt.CssClass += " txtReadOnly";
                    //txt.Attributes.Add("ReadOnly", "readonly");
                    txt.Enabled = false;
                    break;
                case "DROPDOWNLIST":
                    cbo = (DropDownList)ctrl;
                    cbo.Enabled = false;
                    if (cbo.CssClass.IndexOf("txtReadOnly") < 0) cbo.CssClass += " txtReadOnly";
                    break;
                case "RADIOBUTTON":
                    rdb = (RadioButton)ctrl;
                    rdb.Enabled = false;
                    if (rdb.CssClass.IndexOf("LockCtrl") < 0) rdb.CssClass += " LockCtrl";
                    break;
                case "RADIOBUTTONLIST":
                    rbl = (RadioButtonList)ctrl;
                    rbl.Enabled = false;
                    if (rbl.CssClass.IndexOf("LockCtrl") < 0) rbl.CssClass += " LockCtrl";
                    break;
                case "CHECKBOX":
                    chk = (CheckBox)ctrl;
                    chk.Enabled = false;
                    if (chk.CssClass.IndexOf("LockCtrl") < 0) chk.CssClass += " LockCtrl";
                    break;
                case "CHECKBOXLIST":
                    chl = (CheckBoxList)ctrl;
                    chl.Enabled = false;
                    if (chl.CssClass.IndexOf("LockCtrl") < 0) chl.CssClass += " LockCtrl";
                    break;
                case "BUTTON":
                    ctrl.Visible = false;
                    break;
            }

        }
        catch (Exception ex)
        {
            var msg = ex.Message;
        }

    }

    public static void LockControls(ControlCollection ctrls)
    {
        foreach (Control ctrl in ctrls)
        {
            LockCtrl(ctrl);
        }
    }

    public static void UnlockCtrl(Control ctrl)
    {
        Label lb = null;
        TextBox txt = null;
        DropDownList cbo = null;
        RadioButton rdb = null; RadioButtonList rbl = null;
        CheckBox chk = null; CheckBoxList chl = null;

        try
        {
            switch (ctrl.GetType().Name.ToUpper())
            {
                case "LABEL":
                    lb = (Label)ctrl;
                    lb.CssClass = lb.CssClass.Replace(" txtReadOnly", "").Replace("txtReadOnly", "");
                    break;
                case "TEXTBOX":
                    txt = (TextBox)ctrl;
                    txt.CssClass = txt.CssClass.Replace(" txtReadOnly", "").Replace("txtReadOnly", "");
                    txt.Enabled = true;
                    txt.ReadOnly = false;
                    break;
                case "DROPDOWNLIST":
                    cbo = (DropDownList)ctrl;
                    cbo.Enabled = true;
                    cbo.CssClass = cbo.CssClass.Replace(" txtReadOnly", "").Replace("txtReadOnly", "");
                    break;
                case "RADIOBUTTON":
                    rdb = (RadioButton)ctrl;
                    rdb.Enabled = true;
                    rdb.CssClass = rdb.CssClass.Replace(" LockCtrl", "").Replace("LockCtrl", "");
                    break;
                case "RADIOBUTTONLIST":
                    rbl = (RadioButtonList)ctrl;
                    rbl.Enabled = true;
                    rbl.CssClass = rbl.CssClass.Replace(" LockCtrl", "").Replace("LockCtrl", "");
                    break;
                case "CHECKBOX":
                    chk = (CheckBox)ctrl;
                    chk.Enabled = true;
                    chk.CssClass = chk.CssClass.Replace(" LockCtrl", "").Replace("LockCtrl", "");
                    //chk.Attributes.Add("class", "");
                    break;
                case "CHECKBOXLIST":
                    chl = (CheckBoxList)ctrl;
                    chl.Enabled = true;
                    chl.CssClass = chl.CssClass.Replace(" LockCtrl", "").Replace("LockCtrl", "");
                    //chl.Attributes.Add("class", "");
                    break;
                case "BUTTON":
                    ctrl.Visible = true;
                    break;
            }

        }
        catch
        {
        }

    }


    public static void UnlockControls(ControlCollection ctrls)
    {
        foreach (Control ctrl in ctrls)
        {
            UnlockCtrl(ctrl);
        }
    }

    #endregion

    #region Path and file
    public static String GetValidFileName(String fileName)
    {
        try
        {
            String ret = Regex.Replace(fileName.Trim(), "[^A-Za-z0-9ก-๙_. ]+", "");
            ret = ret.Replace(" ", "_");
            return Regex.Replace(ret, "_+", "_");
        }
        catch (Exception ex)
        {
            throw (ex);
        }
    }


    public static String CombineFilePathWithFileName(String FilePath, String FileName)
    {
        return System.IO.Path.Combine(GetFullPath(FilePath), FileName);
    }
    #endregion

    #region Directory


    public static String GetFullPath(String directoryName) // Return with MapPath of Project
    {
        return HttpContext.Current.Server.MapPath(directoryName);
    }

    public static Boolean IsDirectoryExist(String directoryName)
    {
        return Directory.Exists(HttpContext.Current.Server.MapPath(directoryName));
    }
    public static void CreateNewDirectory(String directoryName)
    {
        if (IsDirectoryExist(directoryName)) { DeleteDirectory(directoryName); }
        Directory.CreateDirectory(HttpContext.Current.Server.MapPath(directoryName));
    }
    public static void DeleteDirectory(String directoryName)
    {
        if (IsDirectoryExist(directoryName))
        {
            Directory.Delete(HttpContext.Current.Server.MapPath(directoryName), true);
        }
    }
    #endregion

    #region File


    public static Boolean IsFileExist(String fileName)
    {
        return File.Exists(HttpContext.Current.Server.MapPath(fileName));
    }

    public static Boolean IsFilePathExist(String filePath)
    {
        return File.Exists(filePath);
    }


    public static void CreateNewFile(String fileName)
    {
        CreateNewFile(fileName, false);
    }
    public static void CreateNewFile(String fileName, Boolean isCreateNew)
    {
        if (isCreateNew && IsFileExist(fileName)) { DeleteFile(fileName); }
        File.Create(HttpContext.Current.Server.MapPath(fileName));
    }
    public static void DeleteFile(String fileName)
    {
        if (IsFileExist(fileName))
        {
            File.Delete(HttpContext.Current.Server.MapPath(fileName));
        }
    }

    public static void DeleteFileP(String fileName)
    {
        File.Delete(fileName);
    }

    public static void DeleteFilePath(String filePath)
    {
        if (IsFilePathExist(filePath))
        {
            File.Delete(filePath);
        }
    }
    public static Boolean WriteFile(String filename, Object data)
    {
        return WriteFile(filename, data, false);
    }

    public static Boolean WriteFile(String filename, Object data, Boolean isAppend)
    {
        StreamWriter sw = null;
        String fName;

        try
        {
            fName = HttpContext.Current.Server.MapPath(filename);
            sw = new StreamWriter(fName, isAppend);
            sw.Write(data);

            return true;
        }
        catch
        {
            return false;
        }
        finally
        {
            Utility.ClearObject(ref sw);
        }
    }

    public static byte[] FileToByteArray(string file)
    {
        byte[] byteArray = null;
        try
        {
            byteArray = System.IO.File.ReadAllBytes(file);
        }
        catch { }
        return byteArray;
    }

    public static System.Drawing.Image byteArrayToImage(byte[] byteArrayIn)
    {
        MemoryStream ms = new MemoryStream(byteArrayIn);
        System.Drawing.Image returnImage = System.Drawing.Image.FromStream(ms);
        return returnImage;
    }


    public static void SaveThumbnail(byte[] f, string fileName)
    {
        try
        {
            using (System.Drawing.Image TargetImage = Utility.byteArrayToImage(f))
            {
                using (System.Drawing.Image Thumbnail = TargetImage.GetThumbnailImage(100, 100, null, IntPtr.Zero))
                {
                    Thumbnail.Save(System.Web.Hosting.HostingEnvironment.MapPath("~/Files/Pictures/Thumbnails/") + fileName, System.Drawing.Imaging.ImageFormat.Gif);
                    Thumbnail.Dispose();
                }
                TargetImage.Dispose();
            }

        }
        catch (Exception ex)
        {
        }
    }

 
    public static void SaveThumbnail(byte[] f, string newfileNamePath, int width, int height)
    {
        try
        {
            using (System.Drawing.Image TargetImage = Utility.byteArrayToImage(f))
            {
                //-- edit 05/07/2018
                // Compute thumbnail size.
                int originalWidth = TargetImage.Width;
                int originalHeight = TargetImage.Height;
                int pct = 1;
                if (width == 0 && height == 0)
                {
                    width = originalWidth;
                    height = originalHeight;
                }
                else
                {
                    if (width == 0)
                    {
                        pct = (height * 100) / originalHeight;
                        width = (originalWidth / 100) * pct;
                    }
                    else
                    {
                        if (height == 0)
                        {
                            pct = (width * 100) / originalWidth;
                            height = (originalHeight / 100) * pct;
                        }
                    }
                }

               

                using (System.Drawing.Image Thumbnail = TargetImage.GetThumbnailImage( width, height, null, IntPtr.Zero))
                {
                    System.Drawing.Imaging.ImageFormat imageFormat = System.Drawing.Imaging.ImageFormat.Png; 
                    switch ( GetFileType(newfileNamePath) )
                    {
                        case "gif": imageFormat = System.Drawing.Imaging.ImageFormat.Gif;   break;
                        case "bmp": imageFormat = System.Drawing.Imaging.ImageFormat.Bmp; break;
                        case "jpeg":
                        case "jpg": imageFormat = System.Drawing.Imaging.ImageFormat.Jpeg; break;
                        case "png": imageFormat = System.Drawing.Imaging.ImageFormat.Png; break;
                    }
                    Thumbnail.Save(newfileNamePath, imageFormat);
                    Thumbnail.Dispose();
                }
                TargetImage.Dispose();
            }

        }
        catch (Exception ex)
        {
        }
    }

    public static byte[] ReadFileData(string Filename)
    {
        System.IO.FileStream fs = new System.IO.FileStream(Filename, System.IO.FileMode.Open, System.IO.FileAccess.Read);
        byte[] Buffer = null;
        try
        {
            if (fs != null)
            {
                Buffer = new byte[fs.Length];
                fs.Read(Buffer, 0, (int)fs.Length);
                fs.Close();
                ClearObject(ref fs);
            }
            return Buffer;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public static byte[] ReadFile(string FileName)
    {
        FileStream fs = null;
        long lngLen = 0;
        try
        {
            // Read file and return contents
            fs = File.Open(FileName, FileMode.Open, FileAccess.Read);
            lngLen = fs.Length;

            byte[] Buffer = new byte[Convert.ToInt32(lngLen - 1) + 1];
            fs.Read(Buffer, 0, Convert.ToInt32(lngLen));

            return Buffer;
        }
        catch (Exception ex)
        {

            throw ex;

        }
        finally
        {
            if ((fs != null))
            {
                fs.Close();
                fs.Dispose();
                fs = null;
            }
        }
    }



    public static object checkSession(object ss, string typeVar)
    {
        try
        {
            if (typeVar == "BOOLEAN")
            {
                if (ss == null)
                {
                    return false;
                }
                else
                {
                    return ss;
                }
            }
            else
            {
                if (ss == null)
                {
                    return null;
                }
                else
                {
                    return ss;
                }
            }

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    #endregion



    public static void CustomGridHeader(ref GridViewRow TopRow, int Type, String CSS, int Span, String Text)
    {
        TableCell TCell = new TableCell();
        try
        {
            if (TopRow != null)
            {
                switch (Type)
                {
                    case 1: // RowSpan
                        TCell.Text = Text;
                        TCell.Wrap = true;
                        TCell.RowSpan = Span;
                        TCell.CssClass = CSS;
                        TCell.VerticalAlign = VerticalAlign.Middle;
                        break;
                    case 2: //Column Span
                        TCell.Text = Text;
                        TCell.ColumnSpan = Span;
                        TCell.CssClass = CSS;
                        TCell.VerticalAlign = VerticalAlign.Middle;
                        break;
                    case 3: // Column , Row   span
                        TCell.Text = Text;
                        TCell.RowSpan = Span;
                        TCell.ColumnSpan = Span;
                        TCell.CssClass = CSS;
                        TCell.VerticalAlign = VerticalAlign.Middle;
                        break;
                }
                TopRow.Cells.Add(TCell);
                Utility.ClearObject(ref TCell);
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    public static string GetAttributeCtrl(Control ctrl, string AttributeName, string DefaultValue = "", bool IsHTMLEncode = false)
    {
        string result = "";
        Label lb = null;
        TextBox txt = null;
        DropDownList cbo = null;
        String ctrlType;
        CheckBox chk = null;
        HiddenField hdn = null;
        RadioButton rb = null;
        RadioButtonList rbl = null;
        HtmlInputText htmlText = null;
        HtmlInputPassword htmlPass = null;
        try
        {
            result = DefaultValue;

            switch (ctrl.GetType().Name.ToUpper())
            {
                case "CHECKBOX":
                    chk = (CheckBox)ctrl;
                    result = (chk.Checked) ? ToString(chk.Attributes[AttributeName]) : "";
                    break;
                case "DROPDOWNLIST":
                    cbo = (DropDownList)ctrl;
                    result = ToString(cbo.Attributes[AttributeName]);
                    break;
                case "LABEL":
                    lb = (Label)ctrl;
                    result = ToString(lb.Attributes[AttributeName]);
                    break;
                case "TEXTBOX":
                    txt = (TextBox)ctrl;
                    result = ToString(txt.Attributes[AttributeName]);
                    break;
                case "RADIOBUTTONLIST":
                    rbl = (RadioButtonList)ctrl;
                    result = ToString(rbl.Attributes[AttributeName]);
                    break;
                case "HTMLINPUTTEXT":
                    htmlText = (HtmlInputText)ctrl;
                    result = ToString(htmlText.Attributes[AttributeName]);
                    break;
                case "HTMLINPUTPASSWORD":
                    htmlPass = (HtmlInputPassword)ctrl;
                    result = ToString(htmlPass.Attributes[AttributeName]);
                    break;
            }
        }
        catch
        {
            result = DefaultValue;
        }

        if (IsHTMLEncode)
        {
            result = HttpContext.Current.Server.HtmlEncode(result);
        }

        return result;
    }

    public static void SetAttributeCtrl(Control ctrl, string AttributeName, string value, bool IsHTMLEncode = false)
    {
        Label lb = null;
        TextBox txt = null;

        HtmlInputText htmlTxt = null;
        DropDownList cbo = null;
        RadioButton rdb = null;
        RadioButtonList rbl = null;
        CheckBox chk = null;
        CheckBoxList chl = null;
        String ctrlType;

        try
        {
            if (!string.IsNullOrEmpty(value))
            {
                if (IsHTMLEncode) { value = HttpContext.Current.Server.HtmlEncode(value); }
            }
            else
            {
                value = "";
            }

            ctrlType = ctrl.GetType().Name.ToUpper();
            switch (ctrlType)
            {
                case "LABEL":
                    lb = (Label)ctrl;
                    lb.Attributes.Add(AttributeName, value);
                    break;
                case "TEXTBOX":
                    txt = (TextBox)ctrl;
                    txt.Attributes.Add(AttributeName, value);
                    break;
                case "HTMLINPUTTEXT":
                    htmlTxt = (HtmlInputText)ctrl;
                    htmlTxt.Attributes.Add(AttributeName, value);
                    break;
                case "DROPDOWNLIST":
                    cbo = (DropDownList)ctrl;
                    cbo.Attributes.Add(AttributeName, value);
                    break;
                case "RADIOBUTTON":
                    rdb = (RadioButton)ctrl;
                    rdb.Attributes.Add(AttributeName, value);
                    break;
                case "RADIOBUTTONLIST":
                    rbl = (RadioButtonList)ctrl;
                    rbl.Attributes.Add(AttributeName, value);
                    break;
                case "CHECKBOX":
                    chk = (CheckBox)ctrl;
                    chk.Attributes.Add(AttributeName, value);
                    break;
                case "CHECKBOXLIST":
                    chl = (CheckBoxList)ctrl;
                    if (chl.CssClass.IndexOf("txtReadOnly") < 0) chl.CssClass += " txtReadOnly";
                    break;
            }

        }
        catch (Exception ex)
        {

        }

    }

    public static void AddEnCodeColumn(ref DataTable DT, String SelectedColumnName, String NewEncodeColumnName)
    {
        try
        {
            if (DT != null)
            {
                if (DT.Rows.Count > 0 && SelectedColumnName != "" && NewEncodeColumnName != "")
                {
                    DT.Columns.Add(NewEncodeColumnName, Type.GetType("System.String"));

                    for (int i = 0; i <= DT.Rows.Count - 1; i++)
                    {
                        DT.Rows[i][NewEncodeColumnName] = Validation.EncodeParam(ToString(DT.Rows[i][SelectedColumnName]));
                    }
                    DT.AcceptChanges();
                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public static String GetCurrentPageWithQueryString(String FullUrl)
    {
        try
        {
            if (FullUrl != "")
            {
                var uriBuilder = new UriBuilder(FullUrl);
                return ToString(uriBuilder.Uri).Substring(ToString(uriBuilder.Uri).LastIndexOf("/") + 1);
            }
            else
            {
                return "";
            }
        }
        catch (Exception ex)
        {
            return "";
        }
    }

    public static String GetCurrentQueryString(String FullUrl)
    {
        try
        {
            if (FullUrl != "")
            {
                var uriBuilder = new UriBuilder(FullUrl);
                return ToString(uriBuilder.Query);
            }
            else
            {
                return "";
            }
        }
        catch (Exception ex)
        {
            return "";
        }
    }

    public static void AddQueryStringToUrl(ref String Url, String QueryStringName, String QueryStringValue, bool EncodeValue = true, bool IsFullUrl = false)
    {
        try
        {
            if (Url != null && Url != "" && QueryStringName != null && QueryStringName != "")
            {
                var uriBuilder = new UriBuilder(Url);
                var query = HttpUtility.ParseQueryString(uriBuilder.Query);
                query[QueryStringName] = (EncodeValue == true) ? Validation.EncodeParam(QueryStringValue) : QueryStringValue;
                uriBuilder.Query = ToString(query);
                if (IsFullUrl == false)
                {
                    Url = ToString(uriBuilder.Host) + ToString(uriBuilder.Query);
                }
                else
                {
                    Url = ToString(uriBuilder);
                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public static void RemoveQueryStringFromUrl(ref String Url, String QueryStringName, bool IsFullUrl = false)
    {
        try
        {
            if (Url != null && Url != "" && QueryStringName != null && QueryStringName != "")
            {
                var uriBuilder = new UriBuilder(Url);
                var query = HttpUtility.ParseQueryString(uriBuilder.Query);
                query.Remove(QueryStringName);
                uriBuilder.Query = ToString(query);
                if (IsFullUrl == false)
                {
                    Url = ToString(uriBuilder.Host) + ToString(uriBuilder.Query);
                }
                else
                {
                    Url = ToString(uriBuilder);
                }
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }



  


}
