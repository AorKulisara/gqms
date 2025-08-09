using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace PTT.GQMS.USL.Web.Forms
{
    //-- aor edit 15/05/2018 --
    public partial class ExportXLS : System.Web.UI.Page
    {
        public String Msg = "", ServerAction = "", PageAction = "";
        public String Act = "", Mode = "";
        public String pFileName = "export";


        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                ServerAction = Validation.GetParamStr("ServerAction", DefaultVal: "LOAD");
                Act = Validation.GetParamStr("ACT");
                Mode = Validation.GetParamStr("MODE");

                switch (ServerAction)
                {
                    case "LOAD":
                        if (Act != "") LoadData();
                        break;
                }
            }
            catch (Exception ex)
            {
                Msg = Utility.GetErrorMessage(ex);
            }



        }

        private void LoadData()
        {
            DataTable DT = null;
            String OtherCri = "";
            String fname = Utility.FormatDate(System.DateTime.Today, "YYYMMDD");
            try
            {

                //switch (Act)
                //{
                //    case "BANK":
                //        //-- กรณีไม่ใช้ ธต. ไม่ให้เห็นปุ่ม excel ให้เห็นแต่ pdf  ??????
                //        OtherCri = Utility.ToString(HttpContext.Current.Session["CRITERIA_EXPORT_BANK"]);
                //        DT = Project.dal.SearchIssuingBankCalc("", OtherCriteria: OtherCri);
                //        pFileName = "IssuingBankList" + fname;
                //        break;
                //    case "LC":
                //        OtherCri = Utility.ToString(HttpContext.Current.Session["CRITERIA_EXPORT_LC"]);
                //        DT = Project.dal.SearchLetterCreditALL("", OtherCriteria: OtherCri);
                //        pFileName = "LCList" + fname;
                //        break;
              
                //}


                if ( DT != null && DT.Rows.Count > 0 )
                {
                    gvColSetting(ref gvResult);
                    Utility.BindGVData(ref gvResult, DT, false);
                }
                
                switch (Mode)
                {
                    case "XLS":
                        PageAction = "$('.buttons-excel').click();";
                        break;
                    case "PDF":
                        PageAction = "$('.buttons-pdf').click();";
                        break;
                }

            }
            catch (Exception ex)
            {
                Msg = Utility.GetErrorMessage(ex);
            }
            finally
            {
                Utility.ClearObject(ref DT);
            }
        }


        //--- กำหนดคอลัมน์ที่ grid -------------------
        private void gvColSetting(ref GridView gv)
        {
            try
            {
                //-- clear grid column
                while (gv.Columns.Count > 1)
                {
                    gv.Columns.RemoveAt(1);
                };

               
                switch (Act)
                {
                    case "BANK":
                        AddColumn(ref gv, "SWIFT code", "SWIFT_CODE");
                        AddColumn(ref gv, "Short name", "BANK_SHORT");
                        AddColumn(ref gv, "Bank name", "BANK_NAME");
                        AddColumn(ref gv, "Branch", "BRANCH_NAME");
                        AddColumn(ref gv, "Country", "COUNTRY_NAME");
                        AddColumn(ref gv, "Credit limit (M.USD)", "HO_CREDIT_LIMIT_M", "NUMBER(3)");
                        break;
                    case "LC":
                    case "LC1":
                        AddColumn(ref gv, "Type", "LC_TYPE_NAME");
                        AddColumn(ref gv, "L/C no.", "LC_NO");
                        AddColumn(ref gv, "Issuing bank swift code", "ISSUING_SWIFT_CODE");
                        AddColumn(ref gv, "Issuing bank", "ISSUING_BANK_NAME");
                        AddColumn(ref gv, "Issuing bank branch", "ISSUING_BRANCH_NAME");
                        AddColumn(ref gv, "Unlisted Issuing bank", "UNLST_BANK_NAME");
                        AddColumn(ref gv, "Unlisted Issuing bank branch", "UNLST_BRANCH_NAME");
                        AddColumn(ref gv, "Confirming bank", "CONFIRM_BANK_NAME");
                        AddColumn(ref gv, "Confirming bank branch", "CONFIRM_BRANCH_NAME");
                        AddColumn(ref gv, "Beneficiary/Seller", "COMP_ID");
                        AddColumn(ref gv, "Applicant/Customer", "CUST_NAME");
                        AddColumn(ref gv, "Product", "PRODUCT");
                        AddColumn(ref gv, "L/C issuing date", "ISSUING_DATE", "DATE");
                        AddColumn(ref gv, "L/C Effective date", "EFF_DATE", "DATE");
                        AddColumn(ref gv, "L/C amount (USD)", "LC_AMT","NUMBER(2)");
                        AddColumn(ref gv, "L/C Amount (other currency)", "LC_OTH_AMT", "NUMBER(2)");
                        AddColumn(ref gv, "other currency", "OTH_CURRENCY");
                        AddColumn(ref gv, "Credit amount tolerance (+-%)", "AMT_TLR_PERCENT", "NUMBER(2)");
                        AddColumn(ref gv, "Estimated loading Qty ", "EST_QTY", "NUMBER(3)");
                        AddColumn(ref gv, "Estimated loading Qty (Min)", "EST_QTY_MIN", "NUMBER(3)");
                        AddColumn(ref gv, "Estimated loading Qty (Max)", "EST_QTY_MAX", "NUMBER(3)");
                        AddColumn(ref gv, "Quantity tolerance (+-%)", "QTY_TLR_PERCENT", "NUMBER(2)");
                        AddColumn(ref gv, "Quantity unit", "QTY_UNIT");
                        AddColumn(ref gv, "Payment Term", "PAYMENT_TERM_NAME");
                        AddColumn(ref gv, "Latest shipment date", "L_SHIPMENT_DATE","DATE");
                        AddColumn(ref gv, "L/C expiry date", "EXP_DATE", "DATE");
                        AddColumn(ref gv, "Period of presentation", "PERIOD_PRESENT_DESC");
                        AddColumn(ref gv, "Department/Shop", "DEPARTMENT_NAME");
                        AddColumn(ref gv, "Fin doc", "FIN_DOC");
                        //---
                        AddColumn(ref gv, "B/L date", "BL_DATE", "DATE");
                        AddColumn(ref gv, "Supplier name ", "SUPPLIER");
                        AddColumn(ref gv, "Vessel Name ", "VESSEL");
                        AddColumn(ref gv, "Due date", "DUE_DATE", "DATE");
                        AddColumn(ref gv, "Invoice date", "INVOICE_DATE", "DATE");
                        AddColumn(ref gv, "Invoice No.", "INVOICE_NO");
                        AddColumn(ref gv, "Amount (USD)", "INVOICE_AMT", "NUMBER(2)");
                        AddColumn(ref gv, "Amount (Other Currency)", "INVOICE_OTH_AMT", "NUMBER(2)");
                        AddColumn(ref gv, "Other Currency", "OTH_CURRENCY");
                        AddColumn(ref gv, "Actual loading Qty", "ACT_LOAD_QTY", "NUMBER(3)");
                        AddColumn(ref gv, "Presenting date", "PRESENT_DATE", "DATE");
                        AddColumn(ref gv, "Presenting Bank / Discounting Bank", "PRESENT_BANK_NAME");
                        AddColumn(ref gv, "Discounting Rate (%)", "DISCOUNT_RATE", "NUMBER(2)");
                        AddColumn(ref gv, "No. of Discounting days", "DISCOUNT_DAY", "NUMBER(0)");
                        AddColumn(ref gv, "Funding date", "FUND_DATE", "DATE");
                        AddColumn(ref gv, "B/E no. ", "BE_NO");
                        AddColumn(ref gv, "B/E date", "BE_DATE", "DATE");
                        AddColumn(ref gv, "Letter no.", "LETTER_NO");
                        AddColumn(ref gv, "LOI no. ", "LOI_NO");
                        AddColumn(ref gv, "AWB No. ", "AWB1_NO");
                        //--
                        AddColumn(ref gv, "Due accepted", "DUE_ACCEPTED");
                        AddColumn(ref gv, "Payment received Date", "PAY_REV_DATE", "DATE");
                        AddColumn(ref gv, "Received Amount", "REV_AMT", "NUMBER(2)");
                        AddColumn(ref gv, "Currency", "REV_CURRENCY");
                        AddColumn(ref gv, "Released LOI letter no", "RELES_LOI_NO");
                        AddColumn(ref gv, "Released LOI letter date", "RELES_LOI_DATE", "DATE");
                        AddColumn(ref gv, "Original BL", "BL_ORIGINAL", "NUMBER(0)");
                        AddColumn(ref gv, "Copy BL", "BL_COPY", "NUMBER(0)");
                        AddColumn(ref gv, "Original Cert of origin", "ORIGIN_ORIGINAL", "NUMBER(0)");
                        AddColumn(ref gv, "Copy Cert of origin", "ORIGIN_COPY", "NUMBER(0)");
                        AddColumn(ref gv, "Original Cert of quantity", "QUANT_ORIGINAL", "NUMBER(0)");
                        AddColumn(ref gv, "Copy Cert of quantity", "QUANT_COPY", "NUMBER(0)");
                        AddColumn(ref gv, "Original Cert of quality", "QUALITY_ORIGINAL", "NUMBER(0)");
                        AddColumn(ref gv, "Copy Cert of quality", "QUALITY_COPY", "NUMBER(0)");
                        AddColumn(ref gv, "Other1", "OTHER1");
                        AddColumn(ref gv, "Other2", "OTHER2");
                        AddColumn(ref gv, "AWB No.", "AWB2_NO");
                        AddColumn(ref gv, "Remark", "REMARK");
                        //--
                        AddColumn(ref gv, "L/C status", "LC_STATUS_DESC");
                        AddColumn(ref gv, "Last updated", "DATE_STATUS", "DATE");

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

        private void AddColumn(ref GridView gv, String HeadText, String Field, String colType="TEXT")
        {
            BoundField bfield = null;
            try
            {
                bfield = new BoundField();
                bfield.HeaderText = HeadText;
                bfield.DataField = Field;

                switch (colType)
                {
                    case "DATETIME": bfield.DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}";
                        bfield.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
                        break;
                    case "DATE": bfield.DataFormatString = "{0:dd/MM/yyyy}";
                        bfield.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
                        break;
                    case "NUMBER(0)": bfield.DataFormatString = "{0:#,##0}";
                        bfield.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
                        break;
                    case "NUMBER(1)": bfield.DataFormatString = "{0:#,##0.0}";
                        bfield.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
                        break;
                    case "NUMBER(2)": bfield.DataFormatString = "{0:#,##0.00}";
                        bfield.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
                        break;
                    case "NUMBER(3)": bfield.DataFormatString = "{0:#,##0.000}";
                        bfield.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
                        break;
                    default:
                        break;
                }



                gv.Columns.Add(bfield);
            }
            catch (Exception ex)
            {
                Utility.GetErrorMessage(ex);
            }

         
        }


        protected void gvResult_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.Header)
                {
                    e.Row.TableSection = TableRowSection.TableHeader;
                } else if (e.Row.RowType == DataControlRowType.Footer)
                {
                    e.Row.TableSection = TableRowSection.TableFooter;
                } else if ((e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Separator))
                {
                    DataRowView dr = (DataRowView)e.Row.DataItem;
                }

            }
            catch (Exception ex)
            {
                Msg = Utility.GetErrorMessage(ex);
            }
        }

 

    }
}