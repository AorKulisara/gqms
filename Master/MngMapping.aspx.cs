using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace PTT.GQMS.USL.Web.Master
{
    //-- edit 10/07/2018 --

    public partial class MngMapping : System.Web.UI.Page
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
                Security.CheckRole(Security.TaskMDTag, true);
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
                canAdd = Security.CanDo(Security.TaskMDTag, Security.actAdd);
                canEdit = canAdd; canDelete = canAdd;

                if (canEdit || canDelete)
                {
                    gvData.Columns[3].Visible = true;
                }
                else
                {
                    gvData.Columns[3].Visible = false;
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
                //--F = CHONBURI_DIM_FLOWRATE 
                //-- M = CHONBURI_DIM_MOISTURE 
                switch ( ddlLK_GRP_ID.SelectedValue)
                {
                    case "F":
                        DT = Project.dalhs.SearchDimFlowRate(orderSQL: " TAGNAME " );
                        break;
                    case "M":
                        DT = Project.dalhs.SearchDimMoisture(orderSQL: " TAGNAME ");
                        break;
                }

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


        protected void ddlLK_GRP_ID_SelectedIndexChanged(object sender, EventArgs e)
        {
                LoadData();
        }


        private void BindData()
        {
            DataTable DT = (DataTable)Session["DT"];
            Utility.BindGVData(ref gvData, Session["DT"], (DT.Rows.Count == 0));


            if (canEdit || canDelete)
            {
                if (editableFlag == "N")
                {
                    gvData.Columns[3].Visible = false;
                }
                else
                {
                    gvData.Columns[3].Visible = true;
                }

            }

            if (canAdd)
            {
                if (editableFlag == "N")
                {
                    gvData.FooterRow.Visible = false;
                }
                else
                {
                    gvData.FooterRow.Visible = true;
                }
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
            string itemTagName = "";
            try
            {
                //--F = CHONBURI_DIM_FLOWRATE  //-- M = CHONBURI_DIM_MOISTURE 

                if (action != "ADD")
                {
                    itemTagName = DT.Rows[ItemIndex]["TAGNAME"].ToString();
                }
                else if (action == "ADD")
                {
                    itemTagName = Validation.ValidateStr(Request.Form["txtTAGNAME"]);
                }

                switch (action)
                {
                    case "ADD":                       
                        switch (ddlLK_GRP_ID.SelectedValue) //--F = CHONBURI_DIM_FLOWRATE  //-- M = CHONBURI_DIM_MOISTURE 
                        {
                            case "F":
                                DT = Project.dalhs.SearchDimFlowRate("",itemTagName);
                                break;
                            case "M":
                                DT = Project.dalhs.SearchDimMoisture("", itemTagName);
                                break;
                        }
                        if (DT.Rows.Count != 0)
                        {
                            Msg = "Duplicated Tag Name, Please verify the correctness ";
                        }
                        else
                        {
                            switch (ddlLK_GRP_ID.SelectedValue) //--F = CHONBURI_DIM_FLOWRATE  //-- M = CHONBURI_DIM_MOISTURE 
                            {
                                case "F":
                                    Project.dalhs.MngDimFlowRate(DBUTIL.opINSERT, Validation.ValidateStr(Request.Form["txtNAME"]), itemTagName,  Validation.ValidateStr(Request.Form["txtTAGDESC"]), Validation.ValidateStr(Request.Form["txtUNIT"]));
                                    break;
                                case "M":
                                    Project.dalhs.MngDimMoisture(DBUTIL.opINSERT, Validation.ValidateStr(Request.Form["txtNAME"]), itemTagName,  Validation.ValidateStr(Request.Form["txtTAGDESC"]), Validation.ValidateStr(Request.Form["txtUNIT"]));
                                    break;
                            }


                            Msg = "Save completed.";
                        }
                        break;
                    case "SAVE":
                        switch (ddlLK_GRP_ID.SelectedValue) //--F = CHONBURI_DIM_FLOWRATE  //-- M = CHONBURI_DIM_MOISTURE 
                        {
                            case "F":
                                Project.dalhs.MngDimFlowRate(DBUTIL.opUPDATE, Validation.ValidateStr(Request.Form["txtNAMEEdit"]), itemTagName,  Validation.ValidateStr(Request.Form["txtTAGDESCEdit"]), Validation.ValidateStr(Request.Form["txtUNITEdit"]));
                                break;
                            case "M":
                                Project.dalhs.MngDimMoisture(DBUTIL.opUPDATE, Validation.ValidateStr(Request.Form["txtNAMEEdit"]), itemTagName, Validation.ValidateStr(Request.Form["txtTAGDESCEdit"]), Validation.ValidateStr(Request.Form["txtUNITEdit"]));
                                break;
                        }


                        Msg = "Save completed.";
                        break;
                    case "EDIT": break;
                    case "DELETE":

                        switch (ddlLK_GRP_ID.SelectedValue) //--F = CHONBURI_DIM_FLOWRATE  //-- M = CHONBURI_DIM_MOISTURE 
                        {
                            case "F":
                                Project.dalhs.MngDimFlowRate(DBUTIL.opDELETE, "", itemTagName, "", "");
                                break;
                            case "M":
                                Project.dalhs.MngDimMoisture(DBUTIL.opDELETE, "", itemTagName, "", "");
                                break;
                        }

                        Msg = "Delete completed.";
                        break;
                    case "CANCEL": break;
                }
            }
            catch (Exception ex)
            {
                Msg = Utility.GetErrorMessage(ex);
            }
        }





    }
}