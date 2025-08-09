using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace PTT.GQMS.USL.Web.Settings
{
    //--  edit 21/06/2019 --
    public partial class ChangeLog : System.Web.UI.Page
    {
        public String Msg = "", ServerAction = "", PageAction = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Security.CheckRole(Security.TaskEventLog, true);
                if (!this.IsPostBack)
                {
                    InitCtrl();
                    ServerAction = "LOAD";
                }
                else
                {
                    ServerAction = Utility.ToString(Request.Form["ServerAction"]);
                }

                switch (ServerAction)
                {
                    case "SEARCH": LoadData(); break;
                    case "LOAD": LoadData(); break;
                }
            }
            catch (Exception ex)
            {
                Msg = Utility.GetErrorMessage(ex);
            }
        }

        private void InitCtrl()
        {
            DataTable DT = new DataTable();
            String sql = "";
            try
            {
             
                Utility.SetCtrl(txtDateFrom, Utility.AppFormatDate(DateTime.Now), false);
                Utility.SetCtrl(txtDateTo, Utility.AppFormatDate(DateTime.Now), false);
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

        private void LoadData()
        {
            DataTable DT = null;
            string UserName = "";
            string Description = "";
            string DateFrom = null;
            string DateTo = null;
            string FID = null;
            try
            {
                UserName = Utility.FormatSearchData(Validation.ValidateStr(txtCode.Text));
                Description = Utility.FormatSearchData(Validation.ValidateStr(txtAction.Text));
                DateFrom = Validation.GetCtrlDateStr(txtDateFrom);
                DateTo = Validation.GetCtrlDateStr(txtDateTo);
                FID = Utility.FormatSearchData(Validation.ValidateStr(txtFID.Text));

                Session["S_DATA"] = null;
                //-- Add by Turk 18/04/2562 --> SearchChangeLog --
                DT = Project.dal.SearchChangeLog(UserName, Description, DateFrom, DateTo, FID);
                Session["S_DATA"] = DT;
                gvResult.PageIndex = 0;
                Utility.BindGVData(ref gvResult, (DataTable)Session["S_DATA"], false);

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

        protected void gvResult_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvResult.PageIndex = e.NewPageIndex;
            Utility.BindGVData(ref gvResult, (DataTable)Session["S_DATA"], false);
        }


        protected void gvResult_RowDataBound(object sender, GridViewRowEventArgs e)
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
    }
}