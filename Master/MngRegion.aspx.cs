using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace PTT.GQMS.USL.Web.Master
{
    //-- EDIT 26/06/2023 --

    public partial class MngRegion : System.Web.UI.Page
    {
        public String Msg, ServerAction, PageAction;
        public int ItemIndex;
        public bool canAdd = true;
        public bool canEdit = true;
        public bool canDelete = true;
        public String editableFlag = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Security.CheckRole(Security.TaskMDSite, true);
                SetCtrl();
                if (!this.IsPostBack)
                {
                    ServerAction = "LOAD";

                }
                else
                {
                    ServerAction = Validation.GetParamStr("ServerAction");
                    ItemIndex = Validation.GetParamInt("ItemIndex");

                }

                switch (ServerAction)
                {
                    case "LOAD": LoadData(); break;
                    case "EDIT":
                        DoItemCommand(ServerAction, ItemIndex); gvData.EditIndex = ItemIndex; BindData();
                        gvData.FooterRow.Visible = false; break;
                    case "ADD": DoItemCommand(ServerAction); LoadData(); break;
                    case "SAVE":
                        DoItemCommand(ServerAction, ItemIndex); gvData.EditIndex = -1; LoadData();
                        if (canAdd) gvData.FooterRow.Visible = true;
                        break;
                    case "CANCEL":
                        gvData.EditIndex = -1; BindData();
                        if (canAdd) gvData.FooterRow.Visible = true;
                        break;
                    case "DELETE": DoItemCommand(ServerAction, ItemIndex); LoadData(); break;

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
                canEdit = canAdd; canDelete = canAdd;

                if (canEdit || canDelete)
                {
                    gvData.Columns[4].Visible = true;
                }
                else
                {
                    gvData.Columns[4].Visible = false;
                }
                if (canAdd)
                {
                    gvData.ShowFooter = true;
                }
                else
                {
                    gvData.ShowFooter = false;
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
            try
            {

                DT = Project.dal.SearchDimRegion();
                Session["DT"] = DT;
                BindData();
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



        private void BindData()
        {
            DataTable DT = (DataTable)Session["DT"];
            Utility.BindGVData(ref gvData, Session["DT"], (DT.Rows.Count == 0));


            if (canEdit || canDelete)
            {
                    gvData.Columns[4].Visible = true;
            }

            if (canAdd)
            {
                    gvData.FooterRow.Visible = true;
            }

        }

        protected void gvData_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            try
            {



            }
            catch (Exception ex)
            {

            }

        }


        private void DoItemCommand(string action, int ItemIndex = 0)
        {
            DataTable DT = (DataTable)Session["DT"];
            string regionID = "";
            try
            {
               

                if (action != "ADD")
                {
                    regionID = DT.Rows[ItemIndex]["REGION_ID"].ToString();
                }
                else if (action == "ADD")
                {
                    regionID = Validation.ValidateStr(Request.Form["txtREGION_ID"]);
                }

                if (regionID != "")
                {
                    switch (action)
                    {
                        case "ADD":
                            DT = Project.dal.SearchDimRegion(regionID);
                            if (DT.Rows.Count != 0)
                            {
                                Msg = "Duplicated Region ID, Please verify the correctness ";
                            }
                            else
                            {
                                Project.dal.MngDimRegion(DBUTIL.opINSERT, regionID, Validation.ValidateStr(Request.Form["txtREGION_NAME"]), Validation.ValidateStr(Request.Form["txtREGION_FULL"]), Validation.ValidateStr(Request.Form["txtREGION_ADDR"]));

                                Msg = "Save completed.";
                            }
                            break;
                        case "SAVE":
                            Project.dal.MngDimRegion(DBUTIL.opUPDATE, regionID, Validation.ValidateStr(Request.Form["txtREGION_NAMEEdit"]), Validation.ValidateStr(Request.Form["txtREGION_FULLEdit"]), Validation.ValidateStr(Request.Form["txtREGION_ADDREdit"]));
                            Msg = "Save completed.";
                            break;
                        case "EDIT": break;
                        case "DELETE":
                            //-- ตรวจสอบว่ามีใช้หรือไม่
                            DT = Project.dal.SearchSiteFID(RegionID: regionID);
                            if (DT.Rows.Count != 0)
                            {
                                Msg = "Cannot delete this because it is being used by FID.";
                            }
                            else
                            {
                                Project.dal.MngDimRegion(DBUTIL.opDELETE, regionID, "","","");
                                Msg = "Delete completed.";
                            } 
                            break;
                        case "CANCEL": break;
                    }
                }
                else
                {
                    Msg = "Invalid Region ID.";
                }
 

            }
            catch (Exception ex)
            {
                Msg = Utility.GetErrorMessage(ex);
            }
        }





    }
}