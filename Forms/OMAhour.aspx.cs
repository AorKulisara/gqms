using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace PTT.GQMS.USL.Web.Forms
{
    //-- edit 12/07/2018 ---

    public partial class OMAhour : System.Web.UI.Page
    {
        public String Msg = "", ServerAction = "", PageAction = "";
        public String gUNIT = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Security.CheckRole(Security.TaskVerify, true);

                if (!this.IsPostBack)
                {

                    InitCtrl();
                    ServerAction = Validation.GetParamStr("ServerAction", DefaultVal: "LOAD");

                    Utility.SetCtrl(hidOMA_NAME, Validation.GetParamStr("K", IsEncoded: true)); //-- OMA NAME
                    Utility.SetCtrl(hidDATE, Validation.GetParamStr("D", IsEncoded: true)); //-- DATE

                    lblNAME.Text = " - " + hidOMA_NAME.Value;
                }
                else
                {
                    ServerAction = Validation.GetParamStr("ServerAction");

                }

                switch (ServerAction)
                {
                    case "LOAD":
                    case "SEARCH": LoadData(); break;

                }
            }
            catch (Exception ex)
            {
                Msg = Utility.GetErrorMessage(ex);
            }
        }

        private void InitCtrl()
        {
            try
            {
             

            }
            catch (Exception ex)
            {
                Msg = Utility.GetErrorMessage(ex);
            }
            finally
            {

            }
        }

        private void LoadData()
        {
            DataTable DT = null;

            try
            {
                if (Utility.GetCtrl(hidOMA_NAME) != "" && Utility.GetCtrl(hidDATE) != "")
                {

                    DT = Project.dal.SearchVwArchHourMoisture(Validation.GetCtrlStr(hidOMA_NAME), Validation.GetCtrlDateStr(hidDATE), Validation.GetCtrlDateStr(hidDATE));

                    if (DT.Rows.Count > 0) gUNIT = Utility.ToString(DT.Rows[0]["UNIT"]);

                    Utility.BindGVData(ref gvResult, DT, false);

                    //-- ให้มีการแจ้ง alert เงื่อนไข คือ 0, -1 หรือว่าค่าซ้ำกันเกิน 3 ชั่วโมง หรือว่าเกิน 7 lb 
                    //--- เมื่อ databound เรียบร้อยแล้ว ต้องมาดูว่ามี ค่าซ้ำกันเกิน 3 ชั่วโมงหรือไม่
                    String PrevData1 = "", PrevData2 = "", PrevData3 = "", CurrentData;
                    foreach (GridViewRow rowData in gvResult.Rows)
                    {
                        if (rowData.RowType == DataControlRowType.DataRow)
                        {

                            CurrentData = gvResult.Rows[rowData.RowIndex].Cells[1].Text;
                            if (rowData.RowIndex < 3) //-- edit 03/07/2023 <= 2
                            {   //-- edit 03/07/2023 -- ให้พิจารณาเฉพาะวันที่กำหนด ไม่ต้องดูย้อนหลังของวันก่อนหน้า 
                                ////-- ค้นหาข้อมูลของวันก่อนหน้า 
                                //String fDate = gvResult.Rows[rowData.RowIndex].Cells[0].Text;  //--- DATE 
                                //if (Project.dal.GetAlertVwArchHourMoisture(Validation.GetCtrlStr(hidOMA_NAME), fDate, fDate) == "Y")
                                //{
                                //    if (rowData.RowIndex - 2 >= 0) gvResult.Rows[rowData.RowIndex - 2].Cells[1].CssClass = "cell-right cell-Middle cell-border txt-warning";
                                //    if (rowData.RowIndex - 1 >= 0) gvResult.Rows[rowData.RowIndex - 1].Cells[1].CssClass = "cell-right cell-Middle cell-border txt-warning";
                                //    gvResult.Rows[rowData.RowIndex].Cells[1].CssClass = "cell-right cell-Middle cell-border txt-warning";
                                //}
                            }
                            else //-- edit 03/07/2023 > 2
                           if (rowData.RowIndex >= 3 && Utility.ToNum(CurrentData)>0 && CurrentData == PrevData3 && CurrentData == PrevData2 && CurrentData == PrevData1)
                            { 
                                gvResult.Rows[rowData.RowIndex-3].Cells[1].CssClass = "cell-right cell-Middle cell-border txt-warning"; //-- alert แต่ใช้คำนวน average
                                gvResult.Rows[rowData.RowIndex-2].Cells[1].CssClass = "cell-right cell-Middle cell-border txt-warning"; //-- alert แต่ใช้คำนวน average
                                gvResult.Rows[rowData.RowIndex-1].Cells[1].CssClass = "cell-right cell-Middle cell-border txt-warning"; //-- alert แต่ใช้คำนวน average
                                gvResult.Rows[rowData.RowIndex].Cells[1].CssClass = "cell-right cell-Middle cell-border txt-warning"; //-- alert แต่ใช้คำนวน average
                            }

                            PrevData1 = PrevData2; PrevData2 = PrevData3; PrevData3 = CurrentData;

                        }
                    }


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


        protected void gvResult_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {

                if ( e.Row.RowType == DataControlRowType.Header)
                {
                    if (gUNIT != "") e.Row.Cells[1].Text = "H2O (" + gUNIT + ")";
                }
                else
                {
                    if ((e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Separator))
                    {
                        DataRowView dr = (DataRowView)e.Row.DataItem;

                        //-- ให้มีการแจ้ง alert เงื่อนไข คือ 0, -1 หรือว่าค่าซ้ำกันเกิน 3 ชั่วโมง หรือว่าเกิน 7 lb 
                        if (Utility.ToNum(dr["VALUE"]) <= 0)
                        {
                            //-- alert
                            e.Row.Cells[1].CssClass = "cell-right cell-Middle cell-border txt-warning"; //กำหนดสืพื้น เพื่อให้ทราบว่าไม่ใช้คำนวณ average
                        }
                        else
                        {
                            if (Utility.ToNum(dr["VALUE"]) > 7) e.Row.Cells[1].CssClass = "cell-right cell-Middle cell-border txt-warning"; //-- alert แต่ใช้คำนวน average

                        }

                    }
                    else
                    if (e.Row.RowType == DataControlRowType.Footer)  //-- คำนวณค่าเฉลี่ย โดยตัด 0 กับ -1 ออกก่อน
                    {
                        e.Row.Cells[0].Text = "Average";
                        String avg = Project.dal.GetAverageVwArchHourMoisture(Validation.GetCtrlStr(hidOMA_NAME), Validation.GetCtrlDateStr(hidDATE), Validation.GetCtrlDateStr(hidDATE));
                        e.Row.Cells[1].Text = Utility.FormatNum(avg, 6);
                    }
                }
                


            }
            catch (Exception ex)
            {
                Msg = Utility.GetErrorMessage(ex);
            }
            finally
            {
                
            }

        }
 

    }
}