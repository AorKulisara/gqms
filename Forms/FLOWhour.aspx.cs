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
    public partial class FLOWhour : System.Web.UI.Page
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

                    Utility.SetCtrl(hidFLOW_NAME, Validation.GetParamStr("K", IsEncoded: true)); //-- FLOW NAME
                    Utility.SetCtrl(hidDATE, Validation.GetParamStr("D", IsEncoded: true)); //-- DATE

                    lblNAME.Text = " - " + hidFLOW_NAME.Value;

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
                if (Utility.GetCtrl(hidFLOW_NAME) != "" && Utility.GetCtrl(hidDATE) != "")
                {
                    DT = Project.dal.SearchVwArchHourFlowrate(Validation.GetCtrlStr(hidFLOW_NAME), Validation.GetCtrlDateStr(hidDATE), Validation.GetCtrlDateStr(hidDATE));
                    if (DT.Rows.Count > 0) gUNIT = Utility.ToString(DT.Rows[0]["UNIT"]);

                    Utility.BindGVData(ref gvResult, DT, false);

                    //-- ให้มีการแจ้ง alert เงื่อนไข คือ ถ้าเป็น 0 ติดต่อกันเกิน >= 6 ชั่วโมง ก็จะไม่ใช้ flow นี้ (มี Alert)
                    //--- เมื่อ databound เรียบร้อยแล้ว ต้องมาดูว่ามี ถ้าเป็น 0 ติดต่อกันเกิน >= 6 ชั่วโมง ก็จะไม่ใช้ flow นี้ (มี Alert)
                    Double PrevData1 = -999, PrevData2 = -999, PrevData3 = -999, PrevData4 = -999, PrevData5 = -999, CurrentData= -999;
                    foreach (GridViewRow rowData in gvResult.Rows)
                    {
                        if (rowData.RowType == DataControlRowType.DataRow)
                        {
                            CurrentData = Utility.ToDouble(gvResult.Rows[rowData.RowIndex].Cells[1].Text);

                            if (rowData.RowIndex < 5 ) //-- edit 03/07/2023 <= 5)
                            {
                                //-- edit 03/07/2023 -- ให้พิจารณาเฉพาะวันที่กำหนด ไม่ต้องดูย้อนหลังของวันก่อนหน้า 
                                ////-- ค้นหาข้อมูลของวันก่อนหน้า 
                                //String fDate = gvResult.Rows[rowData.RowIndex].Cells[0].Text;  //--- DATE 
                                //if ( Project.dal.GetAlertVwArchHourFlowrate(Validation.GetCtrlStr(hidFLOW_NAME),fDate, fDate) == "Y" )
                                //{
                                //    if (rowData.RowIndex - 4 >= 0 ) gvResult.Rows[rowData.RowIndex - 4].Cells[1].CssClass = "cell-right cell-Middle cell-border txt-warning";
                                //    if (rowData.RowIndex - 3 >= 0) gvResult.Rows[rowData.RowIndex - 3].Cells[1].CssClass = "cell-right cell-Middle cell-border txt-warning";
                                //    if (rowData.RowIndex - 2 >= 0) gvResult.Rows[rowData.RowIndex - 2].Cells[1].CssClass = "cell-right cell-Middle cell-border txt-warning";
                                //    if (rowData.RowIndex - 1 >= 0) gvResult.Rows[rowData.RowIndex - 1].Cells[1].CssClass = "cell-right cell-Middle cell-border txt-warning";
                                //    gvResult.Rows[rowData.RowIndex].Cells[1].CssClass = "cell-right cell-Middle cell-border txt-warning";
                                //}
                            }
                            else //-- edit 03/07/2023 rowData.RowIndex > 5
                            if (rowData.RowIndex >= 5 && CurrentData == 0 && PrevData5 == 0 && PrevData4 == 0 && PrevData3 == 0 && PrevData2 == 0 && PrevData1 == 0)
                            {
                                gvResult.Rows[rowData.RowIndex - 5].Cells[1].CssClass = "cell-right cell-Middle cell-border txt-warning"; 
                                gvResult.Rows[rowData.RowIndex - 4].Cells[1].CssClass = "cell-right cell-Middle cell-border txt-warning"; 
                                gvResult.Rows[rowData.RowIndex - 3].Cells[1].CssClass = "cell-right cell-Middle cell-border txt-warning"; 
                                gvResult.Rows[rowData.RowIndex - 2].Cells[1].CssClass = "cell-right cell-Middle cell-border txt-warning"; 
                                gvResult.Rows[rowData.RowIndex - 1].Cells[1].CssClass = "cell-right cell-Middle cell-border txt-warning"; 
                                gvResult.Rows[rowData.RowIndex].Cells[1].CssClass = "cell-right cell-Middle cell-border txt-warning"; 
                            }

                            PrevData1 = PrevData2; PrevData2 = PrevData3; PrevData3 = PrevData4;
                            PrevData4 = PrevData5; PrevData5 = CurrentData;

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

                if (e.Row.RowType == DataControlRowType.Header)
                {
                    if (gUNIT != "") e.Row.Cells[1].Text = "Flow (" + gUNIT + ")";
                }
                else
                {
                    if ((e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Separator))
                    {
                        DataRowView dr = (DataRowView)e.Row.DataItem;

                        //-- ให้มีการแจ้ง alert เงื่อนไข คือ < 0  ถ้าเป็น 0 ติดต่อกันเกิน >= 6 ชั่วโมง ก็จะไม่ใช้ flow นี้ (มี Alert)
                        if (Utility.ToNum(dr["VALUE"]) < 0)
                        {
                            //-- alert
                            e.Row.Cells[1].CssClass = "cell-right cell-Middle cell-border txt-warning"; //กำหนดสืพื้น 
                        }

                    }
                    else
                    if (e.Row.RowType == DataControlRowType.Footer)  
                    {
                        e.Row.Cells[0].Text = "Total";
                        String sum = Project.dal.GetSumVwArchHourFlowrate(Validation.GetCtrlStr(hidFLOW_NAME), Validation.GetCtrlDateStr(hidDATE), Validation.GetCtrlDateStr(hidDATE));
                        e.Row.Cells[1].Text = Utility.FormatNum(sum, 3);
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