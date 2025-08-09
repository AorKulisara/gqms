using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
namespace PTT.GQMS.USL.Web.Settings
{
    //-- edit 05/07/2018 --
    public partial class MngUserList : System.Web.UI.Page
    {
        public String Msg = "", ServerAction = "", PageAction = "";
        public bool canEdit;
        public bool canDelete;
        public bool canAdd;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Security.CheckRole(Security.TaskUser, true);
                SetCtrl();
                ServerAction = Validation.GetParamStr("ServerAction");
                if (!this.IsPostBack)
                {
                    InitCtrl();
                    if (ServerAction == "") { ServerAction = "LOAD"; }
                }

                switch (ServerAction)
                {
                    case "SEARCH":
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
            try
            {
                DT = Project.dal.SearchRoleData("","","");
                Utility.LoadList(ref cboRole, DT, "ROLE_NAME", "ROLE_ID",true,"");
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


        private void SetCtrl()
        {
            try
            {
                canAdd = Security.CanDo(Security.TaskUser, Security.actAdd);
                canEdit = canAdd;
                canDelete = canAdd;

                pnlADD.Visible = (canAdd) ? true : false;
            }
            catch (Exception ex)
            {
                Msg = Utility.GetErrorMessage(ex);
            }
        }

        private void LoadData()
        {
            DataTable DT = new DataTable();
            String UserName = "";
            try
            {
                UserName = Utility.FormatSearchData(Validation.GetCtrlStr(txtUserName));
                Session["S_DATA"] = null;
                DT = Project.dal.SearchUserList(UserName,RoleID:Validation.GetCtrlIntStr(cboRole));
                Session["S_DATA"] = DT;
                gvResult.PageIndex = 0;
                Utility.BindGVData(ref gvResult, (DataTable)Session["S_DATA"],false);
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


                DataRowView dr = (DataRowView)e.Row.DataItem;

                if ((e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Separator) && dr["USER_NAME"].ToString() != "")
                {
                    e.Row.Attributes.Add("onclick", "javascript:DoAction('SELECT','" + Validation.EncodeParam(Utility.ToString(dr["USER_NAME"]) )+ "');");
                }

            }
            catch (Exception ex)
            {

            }

        }

    }
}