using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace PTT.GQMS.USL.Web.Master
{
    //-- edit 21/06/2019 ---

    public partial class MngSiteOFF : System.Web.UI.Page
    {
        public String Msg = "", ServerAction = "", PageAction = "";
        public bool canEdit;
        public bool canDelete;
        public bool canAdd;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Security.CheckRole(Security.TaskMDSite, true);
                SetCtrl();

                if (!this.IsPostBack)
                {
                    InitCtrl();
                    ServerAction = Validation.GetParamStr("ServerAction", DefaultVal: "LOAD");
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

        private void SetCtrl()
        {
            try
            {
                canAdd = Security.CanDo(Security.TaskMDSite, Security.actAdd);
                canEdit = canAdd;
                canDelete = canAdd;

                pnlADD.Visible = (canAdd) ? true : false;

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
            DataTable DT;
            String FID= "", siteName = "", Company = "";

            try
            {
                FID = Utility.FormatSearchData(Validation.GetCtrlStr(txtFID));
                siteName = Utility.FormatSearchData(Validation.GetCtrlStr(txtSITE_NAME));
                Company = Utility.FormatSearchData(Validation.GetCtrlStr(txtCOMPANY));
                
                //-- Add by Turk 18/04/2562 --> SearchOffshoreFID --
                DT = Project.dal.SearchOffshoreFID("", FID, siteName, Company, orderSQL: "FID");

                Session["DT"] = DT;
                Utility.BindGVData(ref gvResult, (DataTable)Session["DT"], false);    

            }
            catch (Exception ex)
            {
                Msg = Utility.GetErrorMessage(ex);
            }
        }


        protected void gvResult_Sorting(object sender, GridViewSortEventArgs e)
        {
            DataTable DT = (DataTable)Session["DT"];

            if (DT != null)
            {
                DataView DV = new DataView(DT);
                if (Utility.ToString(this.ViewState["sortExpression"]) == "" || Utility.ToString(this.ViewState["sortExpression"]) == "DESC")
                {
                    this.ViewState["sortExpression"] = "ASC";
                }
                else
                {
                    this.ViewState["sortExpression"] = "DESC";
                }

                DV.Sort = e.SortExpression + " " + Utility.ToString(this.ViewState["sortExpression"]);
                Session["DT"] = DV.ToTable();
                gvResult.PageIndex = 0;
                Utility.BindGVData(ref gvResult, (DataTable)Session["DT"], false);

            }


        }


        protected void gvResult_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {

                if ((e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Separator))
                {
                    DataRowView dr = (DataRowView)e.Row.DataItem;

                    e.Row.Attributes.Add("onclick", "javascript:DoAction('SELECT','" + Validation.EncodeParam(Utility.ToString(dr["SITE_ID"])) + "');");
                }

            }
            catch (Exception ex)
            {
                Msg = Utility.GetErrorMessage(ex);
            }
        }



    }
}