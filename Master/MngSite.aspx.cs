using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace PTT.GQMS.USL.Web.Master
{
    //-- edit 06/07/2018 ---

    public partial class MngSite : System.Web.UI.Page
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
                DT = Project.dal.SearchDimRegion();
                Utility.LoadList(ref ddlREGION_ID, DT, "REGION_NAME", "REGION_ID", true, "All");
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
            String FID = "", siteName = "", regionID = "", isoFlag = "", h2sFlag = "";

            try
            {
                FID = Utility.FormatSearchData(Validation.GetCtrlStr(txtFID));
                siteName = Utility.FormatSearchData(Validation.GetCtrlStr(txtSITE_NAME));
                regionID = Validation.GetCtrlStr(ddlREGION_ID);
                isoFlag = Validation.GetCtrlStr(ddlISO_FLAG);
                h2sFlag = Validation.GetCtrlStr(ddlH2S_FLAG);

                //DT = Project.dal.SearchSiteFID("", FID, siteName, regionID, isoFlag, h2sFlag);
                //-- EDIT 30/03/2022 --
                DT = Project.dal.SearchSiteFID_REPORT("", FID, siteName, regionID, isoFlag, h2sFlag);

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