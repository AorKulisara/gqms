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
using System.Net.Mail;
using System.Collections.Generic;
using System.Net;

/// <summary>
/// Summary description for Project
/// </summary>
public class Project
{
    #region Declarations

    public static BLL bll = new BLL();
    public static DAL dal = new DAL();
    public static DAL_HS dalhs = new DAL_HS();
    public static DAL_BU dalbu = new DAL_BU();
    public static DAL_WH1 dalwh1 = new DAL_WH1();
    public static DAL_WH2 dalwh2 = new DAL_WH2();

    //public static CRReportComponent RPT = new CRReportComponent();

    public static string gBannedDuration = "10"; //minutes
    public static string gSMTP_Server = "";
    public static string gSMTP_User = "";
    public static string gSMTP_Password = "";
    public static string gSMTP_Port = ""; //-- edit 05/06/2017
    public static string gSender_EMail = "";
    public static string gMailMode = "";
    public static string gEmail_ProgramName = "";
    public static string gTest_EMail = "";
    public static string gTest_BCC_EMail = "";
    public static string gWebUrl = "";
   
    public static string gPwdExpireAlert = "";
    public static string gFilePath = "";
    public static string gFileAttach = "";
    public static string gFileType = "";
    public static string gExcelPath = "";
    public static string gExcelFileType = "";
    public static string gXLSDateFormat = "";
    public static string gXLSDateFormatOff = "";//-- edit 26/11/2019 
    public static string gImgType = "";
    public static string gReportFilePath = "";
    public static string gReportViewerPath = "";
    public static string gPISPhotoURL = "";

    public static string gAD_DOMAIN = ""; //-- edit 09/06/2017 
    public static string gStandardExpireAlert = ""; //-- edit 28/08/2020


    // ******* URL ********
    public static string MAIN_URL = "";
    public static string gAttachURL = "";
    public static string gRefreshPage = "";


    // ***** Operation  *****
    public const String gOperationPK = "*";
    public const String gOperationInsertOnly = "+";
    public const String gOperationInsertAndUpdate = "=";
    public const String gOperationConstValue = ">";
    public const String gOperationSeparateField = ",";
    public const String gOperationAgentFieldList = "@";
    #endregion



    #region Constants
    //-- edit 06/07/2018 --------------------------
    // ****** Lookup Constants ******
    public const string lkRPT_TYPE1 = "DAILY";  //Daily report transaction
    public const string lkRPT_TYPE2 = "27DAY";  //27 Days report transaction
    public const string lkRPT_TYPE3 = "ENDMTH";  //End month report transction
    public const string lkRPT_TYPE4 = "DAILY27";  //Daily 27 report transaction
    public const string lkRPT_TYPE5 = "DAILY20";  //Daily(20)
    public const string lkRPT_TYPE6 = "20DAY";  //20 Days

    // ****** Log Category Constants ******
    public const string catLogOn = "LOGON";
    public const string catErrorLog = "ERROR";
    public const string catBannedLog = "BANNED";
    public const string catAutoJob = "AUTOJOB";
    public const string catTransfer1 = "TRANSFER_OGC";
    public const string catTransfer2 = "TRANSFER_FLOW";
    public const string catTransfer3 = "TRANSFER_MOISTURE";
    public const string catTransfer4 = "TRANSFER_CUSTOMER"; //-- EDIT 11/08/2023 --
    public const string catConfDAILY = "CONFIRM_DAILY";
    public const string catConf27DAY = "CONFIRM_27DAY";
    public const string catConf20DAY = "CONFIRM_20DAY";
    public const string catConfENDMTH = "CONFIRM_ENDMTH";
    public const string catDBsyncs = "DB_SYNCS";
    public const string catUserLog = "USER_CHANGED_LOG"; //-- edit 09/05/2025 --


    public enum RoleTypes
    {
        rtAdministrators = 1,
        rtPurchasingGroups = 2,
        rtApprovers = 3,
        rtAnnouncementSellers = 4,
        rtGeneralUsers = 5,
    }

    #endregion

	public Project()
	{
        //
        // TODO: Add constructor logic here
        //
    }

    #region Configurations

    public static string ReadConfiguration(){
        SecurityUtil Encrypt = new SecurityUtil();
        string Result = "";

        try
        {
            gSMTP_Server = Utility.ToString(ConfigurationManager.AppSettings["SMTP_Server"]);
            gSMTP_User = Utility.ToString(ConfigurationManager.AppSettings["SMTP_User"]);
            gSMTP_Port = Utility.ToString(ConfigurationManager.AppSettings["SMTP_Port"]);
            gSMTP_Password = Utility.ToString(ConfigurationManager.AppSettings["SMTP_Password"]);
            gSender_EMail = Utility.ToString(ConfigurationManager.AppSettings["Sender_EMail"]);
            gMailMode = Utility.ToString(ConfigurationManager.AppSettings["MailMode"]);
            gEmail_ProgramName = Utility.ToString(ConfigurationManager.AppSettings["Sender_Name"]);
 
            gFileType = Utility.ToString(ConfigurationManager.AppSettings["FileType"]);
            gImgType = Utility.ToString(ConfigurationManager.AppSettings["ImgType"]);
            gWebUrl = Utility.ToString(ConfigurationManager.AppSettings["Web_Url"]);
            gPISPhotoURL = Utility.ToString(ConfigurationManager.AppSettings["PISPhotoURL"]);

            gFilePath = Utility.ToString(ConfigurationManager.AppSettings["FilePath"]);
            gExcelPath = Utility.ToString(ConfigurationManager.AppSettings["ExcelPath"]);
            gExcelFileType = Utility.ToString(ConfigurationManager.AppSettings["ExcelFileType"]);
            gXLSDateFormat = Utility.ToString(ConfigurationManager.AppSettings["XLSDateFormat"]);
            gXLSDateFormatOff = Utility.ToString(ConfigurationManager.AppSettings["XLSDateFormatOff"]);

            gReportFilePath = Utility.ToString(ConfigurationManager.AppSettings["ReportFilePath"]);
            gReportViewerPath = Utility.ToString(ConfigurationManager.AppSettings["ReportViewerPath"]);
            gFileAttach = Utility.ToString(ConfigurationManager.AppSettings["FileAttach"]);

            gAD_DOMAIN = Utility.ToString(ConfigurationManager.AppSettings["ADDomain"]); //-- EDIT 09/06/2017
            gStandardExpireAlert = Utility.ToString(ConfigurationManager.AppSettings["StandardExpireAlert"]); //-- edit 28/08/2020

            //gTest_EMail = Utility.ToString(ConfigurationManager.AppSettings["TEST_EMail"]);
            //-- AOR EDIT 21/06/2017 -- ใช้ใน SYS_CONFIGS เนื่องจากแก้ไขใน Web.config ไม่สะดวก
            gTest_EMail = dal.GetSysConfigValue("TEST_EMAIL");
        }
        catch (System.Data.OleDb.OleDbException oex)
        {
            Result = "มีปัญหาในการอ่านข้อมูลจากฐานข้อมูล กรุณาติดต่อผู้ดูแลระบบ";
        }
        catch (Exception ex)
        {
            Result = ex.Message;
        }

        return (Result);
    }

    #endregion

    #region User Authorization

    private static string GetIP()
    {
        try
        {
            string strHostName = "";
            strHostName = System.Net.Dns.GetHostName();
            IPHostEntry ipEntry = System.Net.Dns.GetHostEntry(strHostName);
            IPAddress[] addr = ipEntry.AddressList;
            return addr[addr.Length - 2].ToString();
        }
        catch
        {
            return HttpContext.Current.Request.ServerVariables["LOCAL_ADDR"].ToString();
        }
    }

    //-- aor edit 30/01/2018 --
    //logon by windows account ไม่ต้องเปลี่ยนรหัสผ่าน
    public static string LoadUserData(string UserName, Boolean IsAuthenticated = false)
    {
        DataTable DT= null;
        DataRow DR;
        DateTime CurrentDate = DateTime.Now;
        String ret = "";
        String Msg = "";
        String Code = "";
        String roleID = "";
        String UnitCode = "";
        Boolean UserAllowFlag = false;
        
        try
        {

            if (UserName != "")
            {

                //-- ดึงข้อมูลในระบบจาก sys_users
                DR = dal.GetUserData(UserName); //-- ดึงข้อมูลผู้ใช้ในระบบ
                if (DR != null )
                {
                    if (Utility.ToString(DR["DISABLED_FLAG"]) == "Y")
                    { Msg = "User Name : " + UserName + " is prohibited please contact system administrator !"; }
                    else
                    {
                        Msg = "";

                        HttpContext.Current.Session["USER_NAME"] = Utility.ToString(DR["USER_NAME"]);
                        HttpContext.Current.Session["USER_DESC"] = Utility.ToString(DR["USER_DESC"]);
                        HttpContext.Current.Session["EMPLOYEE_ID"] = Utility.ToString(DR["EMPLOYEE_ID"]);
                        HttpContext.Current.Session["ROLE_ID"] = Utility.ToString(DR["ROLE_ID"]);
                        HttpContext.Current.Session["RIGHTS"] = Utility.ToString(DR["RIGHTS"]);
                        HttpContext.Current.Session["POSITION_NAME"] = Utility.ToString(DR["POSITION_NAME"]);
                        HttpContext.Current.Session["UNIT_NAME"] = Utility.ToString(DR["UNIT_NAME"]);
                        HttpContext.Current.Session["USER_EMAIL"] = Utility.ToString(DR["USER_EMAIL"]);
                        UserAllowFlag = true;
                    }

                }
                else
                {
                    Msg = "User Name : " + UserName + " is prohibited please contact system administrator !";
                }
    



                //--มีการแสดงข้อมูล Last login เช่น IP, Date/Time เพื่อให้ผู้ใช้งานเห็นข้อมูลการเข้าระบบครั้งล่าสุด
                if (UserAllowFlag == true )
                {
                    HttpContext.Current.Session["AVATAR_ICON"] = gPISPhotoURL + UserName + ".jpg";
                    HttpContext.Current.Session["UID"] = Validation.ReplaceAlphaNumeric(Security.Encrypted(UserName + Utility.AppFormatDateTime(System.DateTime.Now), UserName));

                    DataTable DTL = Project.dal.SearchLastLogOn(UserName, "", null);
                    if (DTL.Rows.Count > 0)
                    {
                        HttpContext.Current.Session["LAST_LOGIN"] = "&nbsp;&nbsp;&nbsp;Last login: " + Utility.AppFormatDateTime(DTL.Rows[0]["TRANS_DATE"]) + "<br/>&nbsp;&nbsp;&nbsp;IP: " + Utility.ToString(DTL.Rows[0]["IP_ADDRESS"]);
                    }
                    else
                    {
                        HttpContext.Current.Session["LAST_LOGIN"] = "";
                    }
                }


            }

        }
        catch (Exception ex)
        {
			ret = Utility.GetErrorMessage(ex, UsrMsg: "LoadUser Error: User Name=" + UserName + " Message=" + ex.Message);
            throw ex;
        }
        finally
        {
            Utility.ClearObject(ref DT);

        }
        return Msg;
    }



    #endregion

    #region EMail

    public static string SendEmail(string Subject, string Message, string Sender, string Receiver, string MailCC, string MailBCC, string[] Filename = null)
    {
        SmtpClient SMTPMail = null;
        MailMessage objMail = null;
        MailAddress CcMail = null;
        MailAddress Tomail = null;
        MailAddress Frommail = null;

        string MailFrom = "";
        string MailTo = "";
        string ErrMsg = "";
        string[] OneMail;
        string[] OnemailTo;
        MailAddress BCcmail = null;
        string[] OneBccmail;
        int i;
        string TestMsg = "";
        try
        {
            if (gMailMode == "0")
            {
                ErrMsg = "Mail Mode is closed status.";
            }
            else
            {
                if (gMailMode == "2")
                {
                    //WriteTextFile
                }
                if (gMailMode == "1" || gMailMode == "3")
                {
                    if (gTest_EMail != "")
                    {
                        TestMsg += " To : " + Receiver + "<br/>";
                        if (MailCC != "") { TestMsg += " Cc : " + MailCC + " <br/>"; }
                        if (MailBCC != "") { TestMsg += " Bcc : " + MailBCC + " <br/>"; }
                        Message = "This is for test system :  <br/>" + TestMsg + "<br/>" + Message;
                        Subject = Subject + " (This Mail For Test WOT System) ";

                    }

                    MailFrom = Sender == "" ? gSender_EMail : Sender;
                    MailTo = gTest_EMail == "" ? Receiver : gTest_EMail;
                    MailCC = gTest_EMail == "" ? MailCC : gTest_EMail;
                    MailBCC = gTest_EMail == "" ? MailBCC : gTest_EMail;

                    objMail = new MailMessage();
                    if (gEmail_ProgramName != "")
                    {
                        Frommail = new MailAddress(MailFrom, gSender_EMail);
                    }
                    else
                    {
                        Frommail = new MailAddress(MailFrom);
                    }
                    objMail.From = Frommail;
                    objMail.Subject = Subject;
                    objMail.SubjectEncoding = System.Text.Encoding.GetEncoding("windows-874");
                    objMail.Body = Message;
                    if (MailTo != "")
                    {
                        OnemailTo = MailTo.Split(';');
                        for (i = 0; i < OnemailTo.Length; i++)
                        {
                            try
                            {
                                Tomail = new MailAddress(OnemailTo[i]);
                                objMail.To.Add(Tomail);
                            }
                            catch { }
                        }
                    }
                    if (MailCC != "")
                    {
                        OneMail = MailCC.Split(';');
                        for (i = 0; i < OneMail.Length; i++)
                        {
                            try
                            {
                                CcMail = new MailAddress(OneMail[i]);
                                objMail.CC.Add(CcMail);
                            }
                            catch { }
                        }
                    }
                    if (MailBCC != "")
                    {
                        OneBccmail = MailBCC.Split(';');
                        for (i = 0; i < OneBccmail.Length; i++)
                        {
                            try
                            {
                                BCcmail = new MailAddress(OneBccmail[i]);
                                objMail.Bcc.Add(CcMail);
                            }
                            catch { }
                        }
                    }

                    objMail.IsBodyHtml = true;
                    SMTPMail = new SmtpClient();
                    SMTPMail.Host = gSMTP_Server;
                    //SMTPMail.Port = 25;
                  
                    SMTPMail.Send(objMail);

                    ErrMsg = "";

                }
            }
        }
        catch (Exception ex)
        {
            ErrMsg = ex.Message;
        }
        return ErrMsg;
    }

    #endregion


    #region Utilities

    public static DBUTIL.FieldTypes GetFieldType(object value)
    {
        DBUTIL.FieldTypes type;
        switch (value.GetType().ToString().ToLower())
        {
            case "system.int16":
            case "system.int32":
            case "system.int64":
            case "system.decimal":
                type = DBUTIL.FieldTypes.ftNumeric;
                break;
            case "system.datetime":
                type = DBUTIL.FieldTypes.ftDateTime;
                break;
            case "system.byte[]":
                type = DBUTIL.FieldTypes.ftBinary;
                break;
            default:
                type = DBUTIL.FieldTypes.ftText;
                break;
        }
        return type;
    }	
    public static String FormatData(string data, int digit = 0, string DefaultShow = "")
    {
        string fmt = "";
        decimal Num;
        int ShowDigit = 0;
        try
        {

            //digit = ทศนิยม
            //DefaultShow = จำนวนที่โชว์ เช่น  2   ถ้าค่าน้อยกว่า 10  จะใส่เลข 0 นำหน้าให้
            if (fmt == "") { fmt = "#,##0"; }
            if (DefaultShow == "") { ShowDigit = 0; } else { ShowDigit = Utility.ToInt(DefaultShow); }
            if (!string.IsNullOrEmpty(data))
            {
                Num = Utility.ToNum(data);
                if (digit > 0)
                {
                    fmt += "." + new String('0', digit);
                    return string.Format(fmt, Num);
                }
                else
                {
                    if (Num.ToString().IndexOf(".") > -1)
                    {
                        fmt += "." + new String('0', digit);
                        return string.Format(fmt, Num);
                    }
                    else
                    {
                        if (Num.ToString().Length < ShowDigit)
                        {
                            return "0" + Num.ToString();
                        }
                        else
                        {
                            return Num.ToString();
                        }
                    }
                }

            }
        }
        catch
        {

        }


        return fmt;
    }

    public static void GetStartEndDate(String year, String month, ref DateTime sDate, ref DateTime eDate)
    {
        Int32 yy, mm, sMM, eMM;

        yy = Utility.ToInt(year);
        mm = Utility.ToInt(month);
        if (yy != 0)
        {
            if (yy > 2500) { yy -= 543; }
            if (mm >= 1 && mm <= 12)
            {
                sMM = mm; eMM = mm;
            }
            else
            {
                sMM = 1; eMM = 12;
            }
            sDate = new DateTime(yy, sMM, 1);
            eDate = new DateTime(yy, eMM, DateTime.DaysInMonth(yy, eMM));
        }
    }

    #endregion


    #region List Controls

    public static void LoadRoleList(ref DropDownList C, bool IncBlank = true, string IncDesc = "", string IncValue = "")
    {
        string DescField = "ROLE_NAME";
        string ValueField = "ROLE_ID";
        DataTable DT = new DataTable();
        DataRow DR = null;
        try
        {

            C.Items.Clear();
            DT = Project.dal.QueryData("SELECT * FROM SYS_ROLES");

            if (IncBlank)
            {
                DR = DT.NewRow();
                DR[DescField] = (IncDesc != "") ? IncDesc : DBNull.Value.ToString();
                //DR[ValueField] = (IncValue != "") ? Utility.ToInt(IncValue) : Utility.ToInt(DBNull.Value.ToString());
                DT.Rows.InsertAt(DR, 0);

            }

            C.DataTextField = DescField;
            C.DataValueField = ValueField;
            C.DataSource = DT;
            C.DataBind();
        }
        catch
        {
            if ((C != null))
            {
                C.Items.Clear();
            }
        }
    }


     public static void LoadHour(ref DropDownList C, bool IncBlank = true, string IncDesc = "", string IncValue = "")
    {
        try
        {
            Utility.LoadHourCombo(ref C, IncBlank, "", "");
        }
        catch
        {
            if ((C != null))
            {
                C.Items.Clear();
            }
        }
    }
   
    public static void LoadMinute(ref DropDownList C, bool IncBlank = true, string IncDesc = "", string IncValue = "")
    {
        try
        {
            Utility.LoadMinuteCombo(ref C, IncBlank, "", "");
        }
        catch
        {
            if ((C != null))
            {
                C.Items.Clear();
            }
        }
    }

   
    

    #endregion

    //==========================================================================
    //-- ตัวแปรสำหรับระบบ OGC
    #region Composition

    public static String CompositionName(string fieldName)
    {
        String NM = "";
        switch (fieldName)
        {
            case "C1": NM = "CH4"; break;
            case "C2": NM = "C2H6"; break;
            case "C3": NM = "C3H8"; break;
            case "IC4": NM = "IC4H10"; break;
            case "NC4": NM = "NC4H10"; break;
            case "IC5": NM = "IC5H12"; break;
            case "NC5": NM = "NC5H12"; break;
            case "C6": NM = "C6H14"; break;
            case "CO2": NM = "CO2"; break;
            case "N2": NM = "N2"; break;
            case "H2S": NM = "H2S"; break;
            case "NHV": NM = "NETHVDRY"; break;
            //case "GHV": NM = "HVSAT"; break;
            case "GHV": NM = "GHVSAT"; break;
            case "SG": NM = "SG"; break;
            case "WC": NM = "H2O"; break;
            case "UNNORMMIN": NM = "UNNORMMIN"; break;
            case "UNNORMMAX": NM = "UNNORMMAX"; break;
            case "UNNORMALIZED": NM = "UNNORM"; break;
            case "WB": NM = "WI"; break;
            case "BTU": NM = "BTU"; break;
            //-- edit 28/06/2019 --
            case "C7": NM = "C7H16"; break;
            case "H2O": NM = "H2O"; break;
            case "HG": NM = "HG"; break;
        }
        return NM;
    }

    public static String CompositionUnit(string fieldName)
    {
        String NM = "";
        switch (fieldName)
        {
            case "C1": NM = "mole %"; break;
            case "C2": NM = "mole %"; break;
            case "C3": NM = "mole %"; break;
            case "IC4": NM = "mole %"; break;
            case "NC4": NM = "mole %"; break;
            case "IC5": NM = "mole %"; break;
            case "NC5": NM = "mole %"; break;
            case "C6": NM = "mole %"; break;
            case "CO2": NM = "mole %"; break;
            case "N2": NM = "mole %"; break;
            //case "H2S": NM = "mole %"; break;
            case "H2S": NM = "ppm"; break;
            case "NHV": NM = "Btu/scf"; break;
            case "GHV": NM = "Btu/scf"; break;
            case "SG": NM = ""; break;
            case "WC": NM = "Lb/MMscf"; break;
            case "UNNORMMIN": NM = "mole %"; break;
            case "UNNORMMAX": NM = "mole %"; break;
            case "UNNORMALIZED": NM = "mole %"; break;
            case "WB": NM = "Btu/MMscf"; break;
            case "BTU": NM = "Btu/scf"; break;
            //-- edit 28/06/2019 --
            case "C7": NM = "mole %"; break;
            case "H2O": NM = "Lb/MMscf"; break;
            case "HG": NM = "ug/m3"; break;
        }
        return NM;
    }


    //-- edit 27/06/2019 --
    public static String SpotName(string fieldName)
    {
        String NM = "";
        switch (fieldName)
        {          
            case "SULFUR": NM = "H2S - Total Sulfur"; break;
            case "H2S": NM = "H2S - H2S"; break;
            case "COS": NM = "H2S - COS"; break;
            case "CH3SH": NM = "H2S - CH3SH"; break;
            case "C2H5SH": NM = "H2S - C2H5SH"; break;
            case "DMS": NM = "H2S - DMS"; break;
            case "LSH": NM = "H2S - T-bulylSH"; break;
            case "C3H7SH": NM = "H2S - C3H7SH"; break;
            case "HG": NM = "HG"; break;
            case "VOL": NM = "HG - Vol"; break;
            case "O2": NM = "O2"; break;
            case "HC": NM = "HC - Temp"; break;

        }
        return NM;
    }

    //-- edit 27/06/2019 --
    public static String SpotUnit(string fieldName)
    {
        String NM = "";
        switch (fieldName)
        {
            case "HC": NM = "&deg;C"; break;  //Temp.
            case "O2": NM = "mole %"; break;
            case "HG": NM = " ug / cu.m."; break;
            case "VOL": NM = "Lit."; break;
            default: NM = "ppm."; break;
        }
        return NM;
    }


    #endregion
    //==========================================================================
}
