//*********************************************************
//  Copyright (c), Thaiintersol co,ltd. All rights reserved.
//**********************************************************




using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Net.Mail;

public class mdlMail
{

    //-- aor edit 01/06/2017 --
    //'Mail Mode
    //'0 > Do not send anything
    //'1 > Send EMail Only
    //'2 > Write Log Only
    //'3 > Both Send EMail and Write Log
    public static string SendMailData(string Subject, string Message, string Sender, string Receiver, string MailCC = "", string MailBCC = "", string[] Filename = null, string Criteria = "", string Mode = "")
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
    string FN = "";
    string FNlist = "";

    try
    {
        if (Project.gMailMode == "0")
        {
            ErrMsg = "Mail Mode is closed status.";
        }
        else
        {
            if (Project.gMailMode == "2")
            {
                //WriteTextFile
            }
            if (Project.gMailMode == "1" || Project.gMailMode == "3")
            {
                    MailFrom = Sender == "" ? Project.gSender_EMail : Sender;
                    if (Project.gTest_EMail != "")
                    {
                        TestMsg += " To : " + Receiver + "<br/>";
                        if (MailCC != "") { TestMsg += " Cc : " + MailCC + " <br/>"; }
                        if (MailBCC != "") { TestMsg += " Bcc : " + MailBCC + " <br/>"; }
                        Message = "This is for test system :  <br/>" + TestMsg + "<br/>" + Message;
                        Subject = "[TEST MAIL] " + Subject;
                        MailTo = Project.gTest_EMail;
                        MailCC = "";
                        MailBCC = "";
                    }
                    else
                    {
                        MailTo = Receiver;
                    }
 

                    objMail = new MailMessage();
                    if (Project.gEmail_ProgramName != "")
                    {
                        Frommail = new MailAddress(MailFrom, Project.gEmail_ProgramName);
                    }
                    else
                    {
                        Frommail = new MailAddress(MailFrom);
                    }
                    objMail.From = Frommail;
                    objMail.Subject = Subject;
                    objMail.SubjectEncoding = System.Text.Encoding.GetEncoding("windows-874");

                    //-- edit 27/08/2020 -- default font 
                    Message = "<html><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=windows-874\"></head><body><span style='font-size: 11.0pt; line-height: 100%; font-family: \"Calibri\",\"sans-serif\"'>" + Message + "</span></body></html>";
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
                                objMail.Bcc.Add(BCcmail);
                            }
                            catch { }
                        }
                    }

                    if (Filename != null && Filename.Length > 0)
                    {
                        for (i = 0; i < Filename.Length; i++)
                        {
                            FN = Filename[i].Trim();
                            if (i > 0) FNlist += ",";
                            if (Utility.IsFilePathExist(FN))
                            {
                                objMail.Attachments.Add(new System.Net.Mail.Attachment(FN));
                                FNlist += FN;
                            }
                            else
                            {
                                FNlist += FN + "!";
                            }
                        }
                    }

                    objMail.IsBodyHtml = true;
                    SMTPMail = new SmtpClient();
                    SMTPMail.Host = Project.gSMTP_Server;
                    if (Project.gSMTP_Port != "") SMTPMail.Port = Utility.ToInt(Project.gSMTP_Port);

                    if (Project.gSMTP_User != "")
                    {
                        SMTPMail.Credentials = new System.Net.NetworkCredential(Project.gSMTP_User, Project.gSMTP_Password);
                    }

                    SMTPMail.EnableSsl = false; //-- edit 27/08/2018 --

                    SMTPMail.Send(objMail);

                    //ให้เมื่อส่ง mail สำเร็จ ให้ส่ง ErrMsg เป็นช่องว่าง
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

    //-- edit 19/03/2025 ส่งเมล์แจ้ง error
    public static void SendMailError(string Subject, string Message )
    {
        string ErrMSG = "", MailTO = "";
        try
        {
            MailTO = Utility.ToString(ConfigurationManager.AppSettings["MailTo_ERR"]);
            ErrMSG = mdlMail.SendMailData(Subject, Message, "", MailTO);
        }
        catch (Exception ex)
        {
            throw ex;
        }

    }
}