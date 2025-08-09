//*********************************************************
//  Copyright (c), Thaiintersol co,ltd. All rights reserved.
//**********************************************************


using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Collections;
using System.Data;
using System.Web.UI;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;


public partial class BLL
{

    public BLL()
    {

    }

    #region Loggings

        public static void InsertAudit(string Category, string Action, string User, string RefID1=null, string RefID2=null)
    {
        if ( User == "") { User= Utility.ToString(HttpContext.Current.Session["USER_NAME"]); }
        Project.dal.InsertAuditLog(Category, Action, User);
    }

    #endregion


    #region EXCEL


    public static void Export(string fileName, GridView gv)
    {
        try
        {
        HttpContext.Current.Response.Clear();
        HttpContext.Current.Response.AddHeader("content-disposition", string.Format("attachment; filename={0}", fileName));
        HttpContext.Current.Response.ContentType = "application/ms-excel";
        HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.Unicode;
        HttpContext.Current.Response.BinaryWrite(System.Text.Encoding.Unicode.GetPreamble());

        using (StringWriter sw = new StringWriter())
        {
            using (HtmlTextWriter htw = new HtmlTextWriter(sw))
            {
                //  Create a form to contain the grid
                Table table = new Table();

                //  add the header row to the table
                if (gv.HeaderRow != null)
                {
                    PrepareControlForExport(gv.HeaderRow);
                    table.Rows.Add(gv.HeaderRow);
                    table.Rows[0].Style.Add("background", "#F0F0F0");
                }

                //  add each of the data rows to the table
                foreach (GridViewRow row in gv.Rows)
                {
                    PrepareControlForExport(row);
                    table.Rows.Add(row);
                }

                //  add the footer row to the table
                if (gv.FooterRow != null)
                {
                    PrepareControlForExport(gv.FooterRow);
                    table.Rows.Add(gv.FooterRow);
                }

                //  render the table into the htmlwriter
                table.RenderControl(htw);

                String css = "<style type='text/css'>.xls_n0 { mso-number-format:'\\#\\,\\#\\#0'; }.xls_n1 { mso-number-format:'\\#\\,\\#\\#0\\.0'; }" +
                            " .xls_n2 { mso-number-format:'\\#\\,\\#\\#0\\.00'; } .xls_n3 { mso-number-format:'\\#\\,\\#\\#0\\.000'; } " +
                            " .xls_dd { mso-number-format:'dd\\.mm\\.yyyy'; } .xls_dt { mso-number-format:'dd\\.mm\\.yyyy\\ hh\\:mm'; } " +
                            " td{ mso-number-format:'\\@'; border: solid thin black; text-align: center;}</style>";

                HttpContext.Current.Response.Write(css);

                    //  render the htmlwriter into the response
                    //HttpContext.Current.Response.Write(sw.ToString());
                    //HttpContext.Current.Response.End();

                    try
                    {
                        //Write HTTP output
                        HttpContext.Current.Response.Write(sw.ToString());
                    }
                    catch (Exception exc) { }
                    finally
                    {
                        try
                        {
                            //stop processing the script and return the current result
                            HttpContext.Current.Response.End();
                        }
                        catch (Exception ex) { }
                        finally
                        {
                            //Sends the response buffer
                            HttpContext.Current.Response.Flush();
                            // Prevents any other content from being sent to the browser
                            HttpContext.Current.Response.SuppressContent = true;
                            //Directs the thread to finish, bypassing additional processing
                            HttpContext.Current.ApplicationInstance.CompleteRequest();
                            //Suspends the current thread
                            //   Thread.Sleep(1);

                        }
                    }





                }
        }

        }
        catch (Exception ex)
        {
             
        }
    }

    /// <summary>
    /// Replace any of the contained controls with literals
    /// </summary>
    /// <param name="control"></param>
    private static void PrepareControlForExport(Control control)
    {
        for (int i = 0; i < control.Controls.Count; i++)
        {
            Control current = control.Controls[i];
            if (current is LinkButton)
            {
                control.Controls.Remove(current);
                control.Controls.AddAt(i, new LiteralControl((current as LinkButton).Text));
            }
            else if (current is ImageButton)
            {
                control.Controls.Remove(current);
                control.Controls.AddAt(i, new LiteralControl((current as ImageButton).AlternateText));
            }
            else if (current is HyperLink)
            {
                control.Controls.Remove(current);
                control.Controls.AddAt(i, new LiteralControl((current as HyperLink).Text));
            }
            else if (current is DropDownList)
            {
                control.Controls.Remove(current);
                control.Controls.AddAt(i, new LiteralControl((current as DropDownList).SelectedItem.Text));
            }
            else if (current is CheckBox)
            {
                control.Controls.Remove(current);
                control.Controls.AddAt(i, new LiteralControl((current as CheckBox).Checked ? "True" : "False"));
            }

            if (current.HasControls())
            {
                PrepareControlForExport(current);
            }
        }
    }

    public static void ExportExcel(Control c, string filename)
    {
        try
        {

            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=" + filename);
            HttpContext.Current.Response.ContentType = "application/ms-excel";
            HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.Unicode;
            HttpContext.Current.Response.BinaryWrite(System.Text.Encoding.Unicode.GetPreamble());

            System.IO.StringWriter sw = new System.IO.StringWriter();
            System.Web.UI.HtmlTextWriter hw = new HtmlTextWriter(sw);

            c.RenderControl(hw);

            //HttpContext.Current.Response.Write(sw.ToString());
            //HttpContext.Current.Response.End();

            try
            {
                //Write HTTP output
                HttpContext.Current.Response.Write(sw.ToString());
            }
            catch (Exception exc) { }
            finally
            {
                try
                {
                    //stop processing the script and return the current result
                    HttpContext.Current.Response.End();
                }
                catch (Exception ex) { }
                finally
                {
                    //Sends the response buffer
                    HttpContext.Current.Response.Flush();
                    // Prevents any other content from being sent to the browser
                    HttpContext.Current.Response.SuppressContent = true;
                    //Directs the thread to finish, bypassing additional processing
                    HttpContext.Current.ApplicationInstance.CompleteRequest();
                    //Suspends the current thread
                 //   Thread.Sleep(1);

                }
            }

        }
        catch (Exception ex)
        {
            
        }

    }


    //-- aor edit 06/06/2017 --
    public static void AttachExportExcel(Control c, string filename)
    {
        try
        {

            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=" + filename);
            HttpContext.Current.Response.ContentType = "application/ms-excel";
            HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.Unicode;
            HttpContext.Current.Response.BinaryWrite(System.Text.Encoding.Unicode.GetPreamble());

            System.IO.StringWriter sw = new System.IO.StringWriter();
            System.Web.UI.HtmlTextWriter hw = new HtmlTextWriter(sw);

            c.RenderControl(hw);

            Utility.WriteFile(filename, sw.ToString());

            HttpContext.Current.Response.End();



        }
        catch (Exception ex)
        {

        }

    }


    #endregion




    #region ControlLoadSave
    //-- กำหนด auto load data --
    //-- aor edit 09/05/2017 --
    //==== การตั้งชื่อ control สำหรับ auto load/save ============================================
    // dropdown month = ddm
    // text year =  txy
    // text date = dat, lda
    // text date time = dtt, ldt
    // number = tn0,tn1,tn2,tn3, ln0,ln1,ln2,ln3
    // text = txt, lxt
    // hidden = hdf
    public static void ControlLoadData( ref ContentPlaceHolder cph, DataRow DR, Boolean doreadOnly = false , Boolean readOnly=false , string noCOL = "" )
    {
        string ctlType;
        string ctlName;
        try {

            foreach (Control ctlForm in cph.Controls)
            {
        
                if (ctlForm.ID != null )
                {


                 if (ctlForm.ID.Length > 4) 
                {
                    ctlType = Utility.Left(ctlForm.ID, 3);
                    ctlName = Utility.Mid(ctlForm.ID, 3);
                    if (noCOL.IndexOf("," + ctlName + ",") < 0 || noCOL=="")
                    {
                        GenerateLoadData(DR, ctlForm, ctlType, ctlName, doreadOnly, readOnly);
                    } 

                } 
                }

            }
        }
        catch (Exception ex)
        {
            Utility.GetErrorMessage(ex);
        }
        finally
        {
             
        }
    }


    public static void ControlLoadData(ref Panel cph, DataRow DR, Boolean doreadOnly = false, Boolean readOnly = false, string noCOL = "")
    {
        string ctlType;
        string ctlName;
        try
        {

            foreach (Control ctlForm in cph.Controls)
            {

                if (ctlForm.ID != null)
                {


                    if (ctlForm.ID.Length > 4)
                    {
                        ctlType = Utility.Left(ctlForm.ID, 3);
                        ctlName = Utility.Mid(ctlForm.ID, 3);
                        if (noCOL.IndexOf("," + ctlName + ",") < 0 || noCOL == "")
                        {
                            GenerateLoadData(DR, ctlForm, ctlType, ctlName, doreadOnly, readOnly);
                        }

                    }
                }

            }
        }
        catch (Exception ex)
        {
            Utility.GetErrorMessage(ex);
        }
        finally
        {

        }
    }


    public static void GenerateLoadData(DataRow DR, Control ctlForm , string ctlType, string ctlName,  Boolean doreadOnly = false, Boolean readOnly = false)
    {
        try
        {
            switch (ctlType)
            {
                case "txt":
                    TextBox tb = (TextBox)ctlForm;
                    try
                    {
                        Utility.SetCtrl(tb, Utility.ToString(DR[ctlName]));
                        if (doreadOnly && readOnly) tb.ReadOnly = true;
                    }
                    catch (Exception ex)
                    {
                        Utility.GetErrorMessage(ex);
                    }
                    break;
                case "lxt":
                    Label lb = (Label)ctlForm;
                    try
                    {
                        Utility.SetCtrl(lb, Utility.ToString(DR[ctlName]));
                    }
                    catch (Exception ex)
                    {
                        Utility.GetErrorMessage(ex);
                    }
                    break;
                case "dat":
                    TextBox td = (TextBox)ctlForm;
                    try
                    {
                        Utility.SetCtrl(td, Utility.AppFormatDate(DR[ctlName]));
                        if (doreadOnly && readOnly) { td.ReadOnly = true; td.Enabled = false; }
                    }
                    catch (Exception ex)
                    {
                        Utility.GetErrorMessage(ex);
                    }
                    break;
                case "lda":
                    Label lb2 = (Label)ctlForm;
                    try
                    {
                        Utility.SetCtrl(lb2, Utility.AppFormatDate(DR[ctlName]));
                    }
                    catch (Exception ex)
                    {
                        Utility.GetErrorMessage(ex);
                    }
                    break;
                case "dtt":
                    TextBox tt = (TextBox)ctlForm;
                    try
                    {
                        Utility.SetCtrl(tt, Utility.AppFormatDateTime(DR[ctlName]));
                        if (doreadOnly && readOnly) tt.ReadOnly = true;
                    }
                    catch (Exception ex)
                    {
                        Utility.GetErrorMessage(ex);
                    }
                    break;
                case "ldt":
                    Label lb3 = (Label)ctlForm;
                    try
                    {
                        Utility.SetCtrl(lb3, Utility.AppFormatDateTime(DR[ctlName]));
                    }
                    catch (Exception ex)
                    {
                        Utility.GetErrorMessage(ex);
                    }
                    break;
                case "ddl":
                    DropDownList dl = (DropDownList)ctlForm;
                    try
                    {
                        Utility.SetCtrl(dl, Utility.ToString(DR[ctlName]));
                        if (doreadOnly && readOnly) dl.Enabled = false;

                    }
                    catch (Exception ex)
                    {
                        Utility.GetErrorMessage(ex);
                    }
                    break;
                case "tn0":
                    TextBox tno = (TextBox)ctlForm;
                    try
                    {
                        Utility.SetCtrl(tno, Utility.FormatNum(DR[ctlName], 0));
                        if (doreadOnly && readOnly) tno.ReadOnly = true;
                    }
                    catch (Exception ex)
                    {
                        Utility.GetErrorMessage(ex);
                    }
                    break;
                case "ln0":
                    Label lb4 = (Label)ctlForm;
                    try
                    {
                        Utility.SetCtrl(lb4, Utility.FormatNum(DR[ctlName], 0));
                    }
                    catch (Exception ex)
                    {
                        Utility.GetErrorMessage(ex);
                    }
                    break;
                case "tn1":
                    TextBox tn1 = (TextBox)ctlForm;
                    try
                    {
                        Utility.SetCtrl(tn1, Utility.FormatNum(DR[ctlName], 1));
                        if (doreadOnly && readOnly) tn1.ReadOnly = true;
                    }
                    catch (Exception ex)
                    {
                        Utility.GetErrorMessage(ex);
                    }
                    break;
                case "ln1":
                    Label lb5 = (Label)ctlForm;
                    try
                    {
                        Utility.SetCtrl(lb5, Utility.FormatNum(DR[ctlName], 1));
                    }
                    catch (Exception ex)
                    {
                        Utility.GetErrorMessage(ex);
                    }
                    break;
                case "tn2":
                    TextBox tn2 = (TextBox)ctlForm;
                    try
                    {
                        Utility.SetCtrl(tn2, Utility.FormatNum(DR[ctlName], 2));
                        if (doreadOnly && readOnly) tn2.ReadOnly = true;
                    }
                    catch (Exception ex)
                    {
                        Utility.GetErrorMessage(ex);
                    }
                    break;
                case "ln2":
                    Label lb6 = (Label)ctlForm;
                    try
                    {
                        Utility.SetCtrl(lb6, Utility.FormatNum(DR[ctlName], 2));
                    }
                    catch (Exception ex)
                    {
                        Utility.GetErrorMessage(ex);
                    }
                    break;
                case "tn3":
                    TextBox tn3 = (TextBox)ctlForm;
                    try
                    {
                        Utility.SetCtrl(tn3, Utility.FormatNum(DR[ctlName], 3));
                        if (doreadOnly && readOnly) tn3.ReadOnly = true;
                    }
                    catch (Exception ex)
                    {
                        Utility.GetErrorMessage(ex);
                    }
                    break;
                case "ln3":
                    Label lb7 = (Label)ctlForm;
                    try
                    {
                        Utility.SetCtrl(lb7, Utility.FormatNum(DR[ctlName], 3));
                    }
                    catch (Exception ex)
                    {
                        Utility.GetErrorMessage(ex);
                    }
                    break;
                case "hdf":
                    HiddenField hd = (HiddenField)ctlForm;
                    try
                    {
                        Utility.SetCtrl(hd, Utility.ToString(DR[ctlName]));
                    }
                    catch (Exception ex)
                    {
                        Utility.GetErrorMessage(ex);
                    }
                    break;
                case "cbl":
                    CheckBoxList cb = (CheckBoxList)ctlForm;
                    try
                    {
                        Utility.SetCheckBoxList(ref cb, Utility.ToString(DR[ctlName]));
                    }
                    catch (Exception ex)
                    {
                        Utility.GetErrorMessage(ex);
                    }
                    break;
                case "rdl":
                    RadioButtonList rd = (RadioButtonList)ctlForm;
                    try
                    {
                        Utility.SetListValue(ref rd, Utility.ToString(DR[ctlName]));

                    }
                    catch (Exception ex)
                    {
                        Utility.GetErrorMessage(ex);
                    }
                    break;
            }

        }
        catch (Exception ex)
        {
            Utility.GetErrorMessage(ex);
        }
        finally
        {

        }
    }



    //-- กำหนด auto save data --
    //-- aor edit 09/05/2017 --
    public static void ControlSaveData(ref ContentPlaceHolder cph, string saveCOL, int OPs, ref string SQL1, ref string SQL2)
    {
        string ctlType;
        string ctlName;
        try
        {
            foreach (Control ctlForm in cph.Controls)
            {
                if (ctlForm.ID != null && ctlForm.ID.Length > 4)
                {
                    ctlType = Utility.Left(ctlForm.ID, 3);
                    ctlName = Utility.Mid(ctlForm.ID, 3);


                    if (saveCOL.IndexOf("," + ctlName + ",") > -1)
                    {
                        GenerateSaveData(OPs, ref SQL1, ref SQL2, ctlForm, ctlType, ctlName);
                    }

                }
            }
        }
        catch (Exception ex)
        {
            Utility.GetErrorMessage(ex);
        }
        finally
        {

        }

    }


    public static void ControlSaveData(ref Panel cph, string saveCOL, int OPs, ref string SQL1, ref string SQL2)
    {
        string ctlType;
        string ctlName;
        try
        {
            foreach (Control ctlForm in cph.Controls)
            {
                if (ctlForm.ID != null && ctlForm.ID.Length > 4)
                {
                    ctlType = Utility.Left(ctlForm.ID, 3);
                    ctlName = Utility.Mid(ctlForm.ID, 3);

                    if (saveCOL.IndexOf("," + ctlName + ",") > -1)
                    {
                        GenerateSaveData( OPs, ref SQL1, ref SQL2, ctlForm,  ctlType,  ctlName);
                    }

                }
            }
        }
        catch (Exception ex)
        {
            Utility.GetErrorMessage(ex);
        }
        finally
        {

        }

    }


    public static void GenerateSaveData(int OPs, ref string SQL1, ref string SQL2, Control ctlForm,  string ctlType, string ctlName)
    {
        try
        {
                switch (ctlType)
                {
                    case "txt":
                        TextBox tb = (TextBox)ctlForm;
                        Project.dal.AddSQL(OPs, ref SQL1, ref SQL2, ctlName, Validation.GetCtrlStr(tb), DBUTIL.FieldTypes.ftText);
                        break;
                    case "dat":
                        TextBox td = (TextBox)ctlForm;
                        Project.dal.AddSQL(OPs, ref SQL1, ref SQL2, ctlName, Utility.AppDateValue(Validation.GetCtrlStr(td)), DBUTIL.FieldTypes.ftDate);
                        break;
                    case "dtt":
                        TextBox tt = (TextBox)ctlForm;
                        Project.dal.AddSQL(OPs, ref SQL1, ref SQL2, ctlName, Utility.AppDateValue(Validation.GetCtrlStr(tt)), DBUTIL.FieldTypes.ftDateTime);
                        break;
                    case "ddl":
                        DropDownList dl = (DropDownList)ctlForm;
                        Project.dal.AddSQL(OPs, ref SQL1, ref SQL2, ctlName, Validation.GetCtrlStr(dl), DBUTIL.FieldTypes.ftText);
                        break;
                    case "tn0":
                    case "tn1":
                    case "tn2":
                    case "tn3":
                        TextBox tn3 = (TextBox)ctlForm;
                        if (tn3.Text.Trim() == "")
                            Project.dal.AddSQL(OPs, ref SQL1, ref SQL2, ctlName, "", DBUTIL.FieldTypes.ftNumeric);
                        else
                            Project.dal.AddSQL(OPs, ref SQL1, ref SQL2, ctlName, Validation.GetCtrlDec(tn3), DBUTIL.FieldTypes.ftNumeric);
                        break;
                    case "hdf":
                        HiddenField hd = (HiddenField)ctlForm;
                        Project.dal.AddSQL(OPs, ref SQL1, ref SQL2, ctlName, Validation.GetCtrlStr(hd), DBUTIL.FieldTypes.ftText);
                        break;
                    case "cbl":
                        CheckBoxList cb = (CheckBoxList)ctlForm;
                        String cbv = Utility.GetCheckBoxListValue(cb);
                        Project.dal.AddSQL(OPs, ref SQL1, ref SQL2, ctlName, cbv, DBUTIL.FieldTypes.ftText);
                        break;
                    case "rdl":
                        RadioButtonList rd = (RadioButtonList)ctlForm;
                        Project.dal.AddSQL(OPs, ref SQL1, ref SQL2, ctlName, Validation.GetCtrlStr(rd), DBUTIL.FieldTypes.ftText);
                        break;

                }
                    
        }
        catch (Exception ex)
        {
            Utility.GetErrorMessage(ex);
        }
        finally
        {

        }

    }



    #endregion


    #region File
    public static Boolean IsFileExist(String fileName)
    {
        return File.Exists(HttpContext.Current.Server.MapPath(fileName));
    }

    public static void DeleteFile(String fileName)
    {
        if (IsFileExist(fileName))
        {
            File.Delete(HttpContext.Current.Server.MapPath(fileName));
        }
    }

    #endregion

}
