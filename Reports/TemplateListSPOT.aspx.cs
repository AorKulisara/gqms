using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace PTT.GQMS.USL.Web.Reports
{
    //-- EDIT 27/06/2019 ---
    public partial class TemplateListSPOT : System.Web.UI.Page
    {
        public string ServerAction;
        public string Msg = "", PageAction = "";
        public bool canAdd;
        public bool canEdit;
        public bool canDelete;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Security.CheckRole(Security.TaskRptSite, true);

                ServerAction = Validation.GetParamStr("ServerAction");
                SetCtrl();

                if (!this.IsPostBack)
                {
                    if (ServerAction == "") { ServerAction = "SEARCH"; }
                }



                switch (ServerAction)
                {
                    case "SEARCH": LoadData(); break;
                }
            }
            catch (Exception ex)
            {
                Msg = Utility.GetErrorMessage(ex);
            }
        }


        private void SetCtrl()
        {
            try
            {

                canAdd = true;
                canEdit = canAdd;
                canDelete = canEdit;

                pnlADD.Visible = (canAdd) ? true : false;

            }
            catch (Exception ex)
            {
                Msg = Utility.GetErrorMessage(ex);
            }
        }

        private void LoadData()
        {
            DataTable DT = null;
            String sText = "";
            try
            {
                sText = Utility.FormatSearchData(Validation.GetCtrlStr(txtSearch));
                DT = Project.dal.SearchRptFidTemplate("3","", sText, orderSQL: " TID");
                Utility.BindGVData(ref gvResult, DT, false);
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
            if ((e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Separator))
            {

                DataRowView dr = (DataRowView)e.Row.DataItem;
                e.Row.Attributes.Add("onclick", "javascript:DoAction('SELECT','" + Validation.EncodeParam(Utility.ToString(dr["TID"])) + "');");



            }
        }



    }
}