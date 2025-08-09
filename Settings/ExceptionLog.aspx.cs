using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace PTT.GQMS.USL.Web.Settings
{
    //-- aor edit 30/01/2018 --
    public partial class ExceptionLog : System.Web.UI.Page
    {
        public String Msg = "", ServerAction = "", PageAction = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Security.CheckRole(Security.TaskExceptionLog, true);
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
            try
            {
                Utility.SetCtrl(txtDateFrom, Utility.AppFormatDate(DateTime.Now), false);
                Utility.SetCtrl(txtDateTo, Utility.AppFormatDate(DateTime.Now), false);
            }
            catch (Exception ex)
            {
                Msg = Utility.GetErrorMessage(ex);
            }
        }

        private void LoadData()
        {
            DataTable DT = null;
            string UserName = "";
            string DateFrom = null;
            string DateTo = null;
            string OtherCri = "";
            try
            {
                UserName = Utility.FormatSearchData(Validation.ValidateStr(txtCode.Text));
                DateFrom = Validation.GetCtrlDateStr(txtDateFrom);
                DateTo = Validation.GetCtrlDateStr(txtDateTo);

                OtherCri = " CATEGORY LIKE 'ERROR%' ";

                Session["S_DATA"] = null;
                DT = Project.dal.SearchAuditLog(DateFrom, DateTo,"", "", UserName, OtherCriteria:OtherCri );
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


        }

    }
}