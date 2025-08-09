using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

using System.IO;
using System.Drawing;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace PTT.GQMS.USL.Web.Master
{
    //-- edit 21/07/2023 --

    public partial class ExcelCustomer : System.Web.UI.Page
    {
        public String Msg = "", ServerAction = "", PageAction = "";

        public String rOpenFile = "CUSTOMER.xlsx";

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {

                if (!this.IsPostBack)
                {
                    WriteXSL();
               
                }

            }
            catch (Exception ex)
            {
                Msg = Utility.GetErrorMessage(ex);
            }

        }


        private void WriteXSL()
        {
            DataTable DT = null;
            int col = 1, rw = 2; //start column , row
            try
            {
                string srcFileName = "CUSTOMER_TEMPLATE.xlsx";
                string destFileName = "CUSTOMER_" + Utility.FormatDate(System.DateTime.Today,"MMYYYY") + ".xlsx";
                string srcFullFileName = HttpContext.Current.Server.MapPath(Project.gExcelPath + srcFileName);
                string destFullFileName = HttpContext.Current.Server.MapPath(Project.gExcelPath + "Export/" + destFileName);

                FileInfo srcFile = new FileInfo(srcFullFileName);
                FileInfo destFile = new FileInfo(destFullFileName);
                ExcelPackage p = new ExcelPackage(srcFile);
                ExcelWorkbook myWorkbook = p.Workbook;
                //--Should return the sheet name
                ExcelWorksheet myWorksheet = myWorkbook.Worksheets.FirstOrDefault();


                //===== data =====================================================
                String custName = "", OtherCri = "";
                custName = Utility.ToString(Session["custName"]);
                OtherCri = Utility.ToString(Session["OtherCri"]);

                //-- ค้นหา region จากต้นทาง จึงไม่ใช้ region_id
                DT = Project.dal.SearchCustomer("", "", custName, "", OtherCriteria: OtherCri);
                foreach (DataRow DR in DT.Rows)
                {
                    //ID	Short Name	Customer Name	Customer Type	Region	BV Zoning	Block Valve	OGC Main Point	OGC Support Point1	OGC Support Point2	OMA Main Point	OMA Support Point1	H2S Main Point	Hg  Main Point	Remark
                    col = 1;
                    WriteValue(ref myWorksheet, rw, col, DR["PERMANENT_CODE"]); col++;
                    WriteValue(ref myWorksheet, rw, col, DR["NAME_ABBR"]); col++;
                    WriteValue(ref myWorksheet, rw, col, DR["NAME_FULL"]); col++;
                    WriteValue(ref myWorksheet, rw, col, DR["SUB_TYPE"]); col++;
                    WriteValue(ref myWorksheet, rw, col, DR["REGION"]); col++;
                    WriteValue(ref myWorksheet, rw, col, DR["BV_ZONE"]); col++;
                    WriteValue(ref myWorksheet, rw, col, DR["BV_VALVE"]); col++;
                    WriteValue(ref myWorksheet, rw, col, DR["STATUS_CL"]); col++;
                    WriteValue(ref myWorksheet, rw, col, DR["QUALITY_MAIN"]); col++;
                    WriteValue(ref myWorksheet, rw, col, DR["QUALITY_SUPPORT1"]); col++;
                    WriteValue(ref myWorksheet, rw, col, DR["QUALITY_SUPPORT2"]); col++;
                    WriteValue(ref myWorksheet, rw, col, DR["OMA_MAIN"]); col++;
                    WriteValue(ref myWorksheet, rw, col, DR["OMA_SUPPORT1"]); col++;
                    WriteValue(ref myWorksheet, rw, col, DR["H2S"]); col++;
                    WriteValue(ref myWorksheet, rw, col, DR["HG"]); col++;
                    WriteValue(ref myWorksheet, rw, col, DR["REMARK"]); col++;
                    //-- edit 29/05/2024 
                    WriteValue(ref myWorksheet, rw, col, Utility.AppFormatDateTime(DR["CREATED_DATE"])); col++;
                    WriteValue(ref myWorksheet, rw, col, Utility.AppFormatDateTime(DR["MODIFIED_DATE"])); col++;

                    rw++;
                }



                p.SaveAs(destFile);

                rOpenFile = destFileName;

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

        //-- write composition cell
        private void WriteValue(ref ExcelWorksheet WS, int sRow, int sCol, Object gDR)
        {
            ExcelRange cell;
            try
            {

                cell = WS.Cells[sRow, sCol];
                if (Utility.IsNumeric(gDR))
                    cell.Value = Utility.ToNum(gDR);
                else
                    cell.Value = Utility.ToString(gDR);

            }
            catch (Exception ex)
            {
                Msg = Utility.GetErrorMessage(ex);
            }
            finally
            {

            }

        }


        private string BgColor(int indx)
        {
            String bgColor = "";
            try
            {
                switch (indx % 10)
                {
                    case 0: bgColor = "#afd6ff"; break; //ฟ้า
                    case 1: bgColor = "#60ff7a"; break; //เขียว
                    case 2: bgColor = "#ffefaa"; break; //ส้ม
                    case 3: bgColor = "#ffa393"; break; //แดง
                    case 4: bgColor = "#ff99e3"; break; //ม่วง
                    case 5: bgColor = "#a6a3ff"; break; //น้ำเงิน
                    case 6: bgColor = "#ffc27a"; break; //ส้ม
                    case 7: bgColor = "#fffc68"; break; //เหลือง
                    case 8: bgColor = "#9cfcb9"; break; //เขียว
                    case 9: bgColor = "#ffa8ef"; break; //ปานเย็น
                }

                return bgColor;
            }
            catch (Exception ex)
            {
                Msg = Utility.GetErrorMessage(ex);
                return "";
            }



        }


    }
}