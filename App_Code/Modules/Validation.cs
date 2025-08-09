//*********************************************************
//  Copyright (c), Thaiintersol co,ltd. All rights reserved.
//**********************************************************




using System.Web;
using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;

public static class Validation
{
    public static string[] HazardStringList = { "\"\"", "<%", "%>", "{", "}", "/*", "*/", "</", "&quot", "&apos", "&amp", "&lt", "&gt", "onmouseover", "<script", "onclick", "prompt(", " or " };
    public static string[] HazardStringList2 = { "//", "<%", "%>", "{", "}", "/*", "*/", "</", "&quot", "&apos", "&amp", "&lt", "&gt", "onmouseover", "<script" };
    public static string[] HazardStringList3 = { "\"", "'", "//", "<%", "%>", "{", "}", "/*", "*/", "</", "&quot", "&apos", "&amp", "&lt", "&gt", "onmouseover", "<script", "|", "?", ";", "[", "]", "*", "%0" };

    public const string HazardCharacterList = "!><`&:$^'/\"\"=%*+-";
    public const string SpecialCharacterList = "!><`&:$^'/\"\"()=%*+-";

    public static ValidationException vx;

    #region Validate data

    public static bool IsAlphaNumeric(string Data)
    {
        Regex r = new Regex("^[a-zA-Z0-9]*$");
        return (r.IsMatch(Data));
    }

    public static bool HasHazardSpecialChar(string Data)
    {
        //char ch ;
        foreach (char ch in Data)
        {
            if (HazardCharacterList.IndexOf(ch) >= 0)
            {
                return true; ;
            }
        }
        return false; ;
    }

    public static bool HasSpecialChar(string Data)
    {
        foreach (char ch in Data)
        {
            if (SpecialCharacterList.IndexOf(ch) >= 0)
            {
                return true; ;
            }
        }
        return false; ;
    }

    public static bool HasHazardStr2(string Data)
    {
        foreach (string hz in HazardStringList2)
        {
            if (Data.IndexOf(hz) >= 0)
            {
                return true;
            }
        }
        return false;
    }

    public static bool HasHazardStr3(string Data)
    {
        foreach (string hz in HazardStringList3)
        {
            if (Data.IndexOf(hz) >= 0)
            {
                return true;
            }
        }
        return false;
    }

    public static bool IsValidStr2(string Data)
    {
        if (HasHazardStr2(Data) == false)
        {
            return true;
        }
        else
        {
            throw new System.Exception("Invalid string");
        }
    }
    public static bool IsValidStr2(string Data, bool IgnoreException)
    {
        if (HasHazardStr2(Data) == false)
        {
            return true;
        }
        else if (IgnoreException)
        {
            return false;
        }
        else
        {
            throw new System.Exception("Invalid string");
        }
    }

    public static bool IsValidStr3(string Data, bool IgnoreException)
    {
        Data = HttpContext.Current.Server.HtmlEncode(Data);
        if (!HasHazardStr3(Data))
        {
            return true;
        }
        else if (IgnoreException)
        {
            return false;
        }
        else
        {
            throw new System.Exception("Invalid string");
        }
    }

    public static bool HasHazardStr(string Data)
    {
        foreach (string hz in HazardStringList)
        {
            if (Data.IndexOf(hz) >= 0)
            {
                return true;
            }
        }
        return false;
    }

    public static bool IsValidStr(string Data, bool IgnoreException)
    {
        if (!HasHazardStr(Data))
        {
            return true;
        }
        else if (IgnoreException)
        {
            return false;
        }
        else
        {
            throw new System.Exception("Invalid string");
        }
    }

    public static bool IsValidNum(string Data, bool IgnoreException)
    {
        if (Utility.IsNumeric(Data))
        {
            return true;
        }
        else if (IgnoreException)
        {
            return false;
        }
        else
        {
            throw new System.Exception("Invalid number");
        }
    }

    public static bool IsValidDate(object Data, bool IgnoreException)
    {
        if (!Utility.IsNullDate(Convert.ToDateTime(Utility.AppDateValue(Data))))
        {
            return true;
        }
        else if (IgnoreException)
        {
            return false;
        }
        else
        {
            throw new System.Exception("Invalid date");
        }
    }

    //--------------------------- ValidateStr
    public static string ValidateStr(string Data)
    {
        try
        {
            if (string.IsNullOrEmpty(Data) || Data.Trim() == "")
            {
                return "";
            } else if (IsValidStr(Data, false))
            {
                return Data.Trim();
            }
            else
            {
                throw new System.Exception("Invalid string");
            }
        }
        catch (Exception ex)
        {
            throw new System.Exception("Invalid string");
        }
    }

    public static string ValidateStr(string Data, bool AllowEmpty)
    {
        try
        {
            if (Data.Trim() == "")
            {
                if (AllowEmpty)
                {
                    return "";
                }
                else
                {
                    throw new System.Exception("String is required");
                }
            }
            if (IsValidStr(Data, false))
            {
                return Data;
            }
            else
            {
                throw new System.Exception("Invalid string");
            }
        }
        catch (Exception ex)
        {
            throw new System.Exception("Invalid string");
        }
    }

    public static string ValidateStr(string Data, bool AllowEmpty, object DefaultVal)
    {
        try
        {
            if (string.IsNullOrEmpty(Data) || Data.Trim() == "")
            {
                if (AllowEmpty)
                {
                    if (DefaultVal != null)
                    {
                        return DefaultVal.ToString();
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    throw new System.Exception("String is required");
                }
            } else if (IsValidStr(Data, false))
            {
                return Data;
            }
            else
            {
                throw new System.Exception("Invalid string");
            }
        }
        catch (Exception ex)
        {
            throw new System.Exception("Invalid string");
        }
    }
    //-------------------------- end ValidateStr

    public static string ValidateStr2(string Data, bool AllowEmpty, object DefaultVal)
    {
        try
        {
            if (Data.Trim() == "")
            {
                if (AllowEmpty)
                {
                    if (DefaultVal != null)
                    {
                        return DefaultVal.ToString();
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    throw new System.Exception("String is required");
                }
            }
            if (IsValidStr2(Data, false))
            {
                return Data;
            }
            else
            {
                throw new System.Exception("Invalid string");
            }
        }
        catch (Exception ex)
        {
            throw new System.Exception("Invalid string");
        }
    }

    public static string ValidateStr3(string Data, bool AllowEmpty, object DefaultVal)
    {
        try
        {
            if (Data.Trim() == "")
            {
                if (AllowEmpty)
                {
                    if (DefaultVal != null)
                    {
                        return DefaultVal.ToString();
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    throw new System.Exception("String is required");
                }
            }
            if (IsValidStr3(Data, false))
            {
                return Data;
            }
            else
            {
                throw new System.Exception("Invalid string");
            }
        }
        catch (Exception ex)
        {
            throw new System.Exception("Invalid string");
        }
    }

    public static object ValidateSearch(string Data, bool AllowEmpty, string DefaultVal, int MaxValue)
    {
        object Num;
        try
        {

            if (Data.Trim() == "")
            {
                if (AllowEmpty)
                {
                    return DefaultVal.ToString();
                }
                else
                {
                    throw new System.Exception("Number is required");
                }
            }
            if (!IsValidStr3(Data, false))
            {
                throw new System.Exception("Invalid search data string");
            }

            if (IsValidNum(Data, true))
            {
                Num = Convert.ToInt32(Data);
                if (MaxValue > 0)
                {
                    if ((int)Num > MaxValue)
                    {
                        throw new System.Exception("Invalid search data number");
                    }
                }
            }
            return Data;
        }
        catch (Exception ex)
        {
            throw new System.Exception("Invalid search data");
        }
    }

    public static string ValidateName(string Data, bool AllowEmpty = true, string DefaultVal = "")
    {
        try
        {
            if (string.IsNullOrEmpty(Data) || Data.Trim() == "")
            {
                if (AllowEmpty)
                {
                    if (DefaultVal != null)
                    {
                        return DefaultVal.ToString();
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    throw new System.Exception("Alphanumeric string is required");
                }
            }
            else if (IsAlphaNumeric(Data))
            {
                return Data;
            }
            else
            {
                throw new System.Exception("Invalid string");
            }
        }
        catch (Exception ex)
        {
            throw new System.Exception("Invalid string");
        }
    }


    public static long ValidateLong(string Data, bool AllowEmpty = true, long DefaultVal = 0)
    {
        try
        {
            if (Data.Trim() == "")
            {
                if (AllowEmpty)
                {
                    return DefaultVal;
                }
                else
                {
                    throw new System.Exception("Number is required");
                }
            }
            if (IsValidNum(Data, false))
            {
                return Convert.ToInt64(Data);
            }
            else
            {
                throw new System.Exception("Invalid number");
            }
        }
        catch (Exception ex)
        {
            throw new System.Exception("Invalid number");
        }
    }

    public static int ValidateInt(string Data, bool AllowEmpty = true, int DefaultVal = 0)
    {
        try
        {
            if (string.IsNullOrEmpty(Data) || Data.Trim() == "")
            {
                if (AllowEmpty)
                {
                    return DefaultVal;
                }
                else
                {
                    throw new System.Exception("Number is required");
                }
            }
            if (IsValidNum(Data, false))
            {
                return Convert.ToInt32(Data);
            }
            else
            {
                throw new System.Exception("Invalid number");
            }
        }
        catch (Exception ex)
        {
            throw new System.Exception("Invalid number");
        }
    }

    public static string ValidateIntStr(string Data, bool AllowEmpty = true, string DefaultVal = "")
    {
        try
        {
            if (string.IsNullOrEmpty(Data) || Data.Trim() == "")
            {
                if (AllowEmpty)
                {
                    return DefaultVal;
                }
                else
                {
                    throw new System.Exception("Number is required");
                }
            }
            if (IsValidNum(Data, false))
            {
                return Data;
            }
            else
            {
                throw new System.Exception("Invalid number");
            }
        }
        catch (Exception ex)
        {
            throw new System.Exception("Invalid number");
        }
    }

    public static Decimal ValidateNum(string Data, bool AllowEmpty, Decimal DefaultVal)
    {
        try
        {
            if (string.IsNullOrEmpty(Data) || Data.Trim() == "")
            {
                if (AllowEmpty)
                {
                    return DefaultVal;
                }
                else
                {
                    throw new System.Exception("Number is required");
                }
            }
            if (IsValidNum(Data, false))
            {
                return Convert.ToDecimal(Data);
            }
            else
            {
                throw new System.Exception("Invalid number");
            }
        }
        catch (Exception ex)
        {
            throw new System.Exception("Invalid number");
        }
    }

    public static object ValidateDate(string Data, bool AllowEmpty = true, object DefaultVal = null)
    {
        try
        {
            if (string.IsNullOrEmpty(Data) || Data.Trim() == "")
            {
                if (AllowEmpty)
                {
                    if (DefaultVal != null)
                    {
                        return DefaultVal;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    throw new System.Exception("Date is required");
                }
            }
            if (IsValidDate(Data, false))
            {
                return Utility.AppDateValue(Data);
            }
            else
            {
                throw new System.Exception("Invalid date");
            }
        }
        catch (Exception ex)
        {
            throw new System.Exception("Invalid date");
        }
    }

    public static string ValidateDateStr(string Data, bool AllowEmpty = true, string DefaultVal = "")
    {
        try
        {
            if (string.IsNullOrEmpty(Data) || Data.Trim() == "")
            {
                if (AllowEmpty)
                {
                    if (DefaultVal != null)
                    {
                        return DefaultVal;
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    throw new System.Exception("Date is required");
                }
            }
            if (IsValidDate(Data, false))
            {
                return Data;
            }
            else
            {
                throw new System.Exception("Invalid date");
            }
        }
        catch (Exception ex)
        {
            throw new System.Exception("Invalid date");
        }
    }
    
    public static object ValidateTime(string Data, bool AllowEmpty = true, object DefaultVal = null)
    {
        string format = "HH:mm:ss";
        try
        {
            System.Globalization.CultureInfo provider = System.Globalization.CultureInfo.InvariantCulture;
            DateTime dt = DateTime.ParseExact(Data, format, provider);
            if (Utility.ToString(dt) != "")
            {
                return Data;
            }
            else
            {
                return "";
                throw new System.Exception("Invalid Time");
            }
        }
        catch (Exception ex)
        {
            throw new System.Exception("Invalid Time. Please set format time HH:mm:ss");
        }
    }

    public static bool IsValidPassword(string Password)
    {
        return HasSpecialChar(Password);
    }


    //-- peach edit 10/10/2017  replace HazardChar
    public static string ReplaceHazardSpecialChar(string Data)
    {

        foreach (char ch in Data)
        {
            if (HazardCharacterList.IndexOf(ch) >= 0)
            {
                Data = Data.Replace(ch, '0');
            }
        }
        return Data;

    }


    //-- peach edit 15/02/2018 
    public static string ReplaceAlphaNumeric(string Data)
    {

        foreach (char ch in Data)
        {
            if ( ! IsAlphaNumeric(Utility.ToString(ch)) )
            {
                Data = Data.Replace(ch, '0');
            }
        }
        return Data;

    }


    #endregion


    #region Parameters

    public static string EncodeParam(string Data)
    {
        System.Text.ASCIIEncoding Encoder = new System.Text.ASCIIEncoding();
        try
        {
            if (Data != null && Data != "")
            {
                return Convert.ToBase64String(Encoder.GetBytes(Data.ToCharArray()));
            }
            else
            {
                return "";
            }
        }
        catch (Exception ex)
        {
            throw new System.Exception("Invalid parameter data");
        }
    }

    public static string DecodeParam(string EncodedData)
    {
        System.Text.ASCIIEncoding Encoder = new System.Text.ASCIIEncoding();
        try
        {
            if (EncodedData != null && EncodedData != "")
            {
                return Encoder.GetString(Convert.FromBase64String(EncodedData));
            }
            else
            {
                return "";
            }
            
        }
        catch (Exception ex)
        {
            throw new System.Exception("Invalid parameter data");
        }
    }

    public static long GetParamLong(string ParamName, Control hCtrl = null, long DefaultVal = 0, bool AllowEmpty = true, bool IsEncoded = false)
    {
        System.Web.UI.Page Page = (Page)HttpContext.Current.Handler;
        string ParamData = null;
        long value = 0;

        if (!((Page)HttpContext.Current.Handler).IsPostBack)
        {
            ParamData = Page.Request.QueryString[ParamName] + "";
            if (IsEncoded)
                ParamData = DecodeParam(ParamData);
        }
        else
        {
            if ((hCtrl == null))
            {
                ParamData = Page.Request.Form[ParamName];
            }
            else
            {
                ParamData = Utility.GetCtrl(hCtrl);
            }
        }

        value = ValidateLong(ParamData, AllowEmpty, DefaultVal);
        if ((hCtrl != null))
            Utility.SetCtrl(hCtrl, value.ToString());
        return value;
    }

    public static int GetParamInt(string ParamName, Control hCtrl = null, int DefaultVal = 0, bool AllowEmpty = true, bool IsEncoded = false)
    {
        System.Web.UI.Page Page = (Page)HttpContext.Current.Handler;
        string ParamData = null;
        int value = 0;

        if (!((Page)HttpContext.Current.Handler).IsPostBack)
        {
            ParamData = Page.Request.QueryString[ParamName] + "";
            if (IsEncoded)
                ParamData = DecodeParam(ParamData);
        }
        else
        {
            if ((hCtrl == null))
            {
                ParamData = Page.Request.Form[ParamName];
            }
            else
            {
                ParamData = Utility.GetCtrl(hCtrl);
            }
        }

        value = ValidateInt(ParamData, AllowEmpty, DefaultVal);
        if ((hCtrl != null))
            Utility.SetCtrl(hCtrl, value.ToString());
        return value;
    }

    public static string GetParamIntStr(string ParamName, Control hCtrl = null, string DefaultVal = "", bool AllowEmpty = true, bool IsEncoded = false)
    {
        System.Web.UI.Page Page = (Page)HttpContext.Current.Handler;
        string ParamData = null;
        string value = null;

        if (!Page.IsPostBack)
        {
            ParamData = Page.Request.QueryString[ParamName] + "";
            if (IsEncoded)
                ParamData = DecodeParam(ParamData);
        }
        else
        {
            if ((hCtrl == null))
            {
                ParamData = Page.Request.Form[ParamName];
            }
            else
            {
                ParamData = Utility.GetCtrl(hCtrl);
            }
        }

        value = ValidateIntStr(ParamData, AllowEmpty, DefaultVal);
        if ((hCtrl != null))
            Utility.SetCtrl(hCtrl, value);
        return value;
    }

    public static string GetParamStr(string ParamName, Control hCtrl = null, string DefaultVal = "", bool AllowEmpty = true, bool IsEncoded = false)
    {
        System.Web.UI.Page Page = (Page)HttpContext.Current.Handler;
        string ParamData = null;
        string value = null;

        if (!Page.IsPostBack)
        {
            ParamData = Page.Request.QueryString[ParamName] + "";
            if (IsEncoded)
                ParamData = DecodeParam(ParamData);
        }
        else
        {
            if ((hCtrl == null))
            {
                ParamData = Page.Request.Form[ParamName];
            }
            else
            {
                ParamData = Utility.GetCtrl(hCtrl);
            }
        }

        value = ValidateStr(ParamData, AllowEmpty, DefaultVal);
        if ((hCtrl != null))
            Utility.SetCtrl(hCtrl, value);
        return value;
    }

    public static string GetParamStr2(string ParamName, Control hCtrl = null, string DefaultVal = "", bool AllowEmpty = true, bool IsEncoded = false)
    {
        System.Web.UI.Page Page = (Page)HttpContext.Current.Handler;
        string ParamData = null;
        string value = null;

        if (!Page.IsPostBack)
        {
            ParamData = Page.Request.QueryString[ParamName] + "";
            if (IsEncoded)
                ParamData = DecodeParam(ParamData);
        }
        else
        {
            if ((hCtrl == null))
            {
                ParamData = Page.Request.Form[ParamName];
            }
            else
            {
                ParamData = Utility.GetCtrl(hCtrl);
            }
        }

        value = ValidateStr2(ParamData, AllowEmpty, DefaultVal);
        if ((hCtrl != null))
            Utility.SetCtrl(hCtrl, value);
        return value;
    }

#endregion

#region "Controls"

    public static int GetCtrlInt(Control Ctrl, int DefaultValue = 0)
    {
        string result = null;

        result = Utility.GetCtrl(Ctrl, DefaultValue.ToString());
        return ValidateInt(result);
    }

    public static long GetCtrlLong(ref Control Ctrl, long DefaultValue = 0)
    {
        string result = null;

        result = Utility.GetCtrl(Ctrl, DefaultValue.ToString());
        return ValidateLong(result);
    }

    public static Decimal GetCtrlDec(Control Ctrl, Decimal DefaultValue = 0)
    {
        string result = null;

        result = Utility.GetCtrl(Ctrl, DefaultValue.ToString());
        return ValidateNum(result, true, DefaultValue);
    }

    public static string GetCtrlIntStr(Control Ctrl, string DefaultValue = "")
    {
        string result = null;

        result = Utility.GetCtrl(Ctrl, DefaultValue);
        return ValidateIntStr(result);
    }

    public static string GetCtrlStr(Control Ctrl, string DefaultValue = null)
    {
        string result = null;

        result = Utility.GetCtrl(Ctrl, DefaultValue);
        return ValidateStr(result);
    }

    public static object GetCtrlDate(Control Ctrl, string DefaultValue = null)
    {
        string result = null;

        result = Utility.GetCtrl(Ctrl, DefaultValue);
        return ValidateDate(result);
    }

    public static string GetCtrlDateStr(Control Ctrl, string DefaultValue = null)
    {
        string result = null;

        result = Utility.GetCtrl(Ctrl, DefaultValue);
        return ValidateDateStr(result);
    }

    public static string GetCtrlText(ref ListControl Ctrl)
    {
        string result = "";

        try
        {
            if ((Ctrl.SelectedItem != null))
            {
                result = Ctrl.SelectedItem.Text;
            }

            return result;
        }
        catch (Exception ex)
        {
            result = "";
        }

        return result;
    }

#endregion



}