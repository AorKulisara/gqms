using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;


namespace PTT.GQMS.USL.Web.Master
{
    //-- edit 21/06/2019 

    public partial class MngSiteOFFDetail : System.Web.UI.Page
    {
        public string ServerAction;
        public string Msg = "", PageAction = "";
        public String Key;

        public bool canAdd = true;
        public bool canEdit = true;
        public bool canDelete = true;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Security.CheckRole(Security.TaskMDSite, true);

                SetCtrl();
                if (!this.IsPostBack)
                {
                    HttpContext.Current.Session["CheckRefresh"] = Server.UrlDecode(System.DateTime.Now.ToString()); //Prevent duplicate insert on page refresh

                    InitCtrl();

                    Key = Validation.GetParamStr("K", IsEncoded: true);
                    if (Utility.IsNumeric(Key))
                    {
                        Utility.SetCtrl(hidSITE_ID, Key);
                        ServerAction = "LOAD";
                    }
                    else {

                        ServerAction = "ADD";
                    }
                }
                else
                {
                    ServerAction = Validation.GetParamStr("ServerAction");
                }



                switch (ServerAction)
                {
                    case "ADD": if (canAdd) { AddData(); } break;
                    case "LOAD":
                        LoadData();
                        break;
                    case "SAVE":
                        if (canAdd || canEdit)
                        {
                            if (HttpContext.Current.Session["CheckRefresh"].ToString() == ViewState["CheckRefresh"].ToString()) //Prevent duplicate insert on page refresh
                            {
                                SaveData();
                                HttpContext.Current.Session["CheckRefresh"] = Server.UrlDecode(System.DateTime.Now.ToString()); //give a new value to session
                            }
                        }
                        break;
                    case "DELETE":
                        if (canDelete)
                        {
                            DeleteData();
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Msg = Utility.GetErrorMessage(ex);
            }

        }

        //Prevent duplicate insert on page refresh
        protected void Page_PreRender(object sender, EventArgs e)
        {
            ViewState["CheckRefresh"] = HttpContext.Current.Session["CheckRefresh"];
        }

        private void SetCtrl()
        {
            try
            {
                //-- กำหนดให้มี 2 คอลัมน์คือ Read และ Add/Edit/Delete 
                canEdit = Security.CanDo(Security.TaskMDSite, Security.actAdd);
                canDelete = canEdit;
                canAdd = canEdit;


                pnlDELETE.Visible = (canDelete) ? true : false;
                pnlSAVE.Visible = (canEdit) ? true : false;


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


        private void AddData()
        {
            DataTable DT = null;
            try
            {
                Utility.SetCtrl(hidSITE_ID, "");

                Utility.SetCtrl(txtFID, "");
                Utility.SetCtrl(txtSITE_NAME, "");
                // -- Add by Turk 18/04/2562 --> txtCOMPANY --
                Utility.SetCtrl(txtCOMPANY, "");

                Utility.SetCtrl(lblLastUpdated, "");

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
            DataRow DR = null;
 
            try
            {
                if (Utility.GetCtrl(hidSITE_ID ) != "")
                {
                    // -- Edit by Turk 18/04/2562 --> SearchOffshoreFID, txtCOMPANY --
                    DT = Project.dal.SearchOffshoreFID(Validation.GetCtrlInt(hidSITE_ID).ToString());
                    if (DT != null && DT.Rows.Count > 0)
                    {
                        DR = Utility.GetDR(ref DT);

                        Utility.SetCtrl(txtFID, Utility.ToString(DR["FID"]));
                        Utility.SetCtrl(txtSITE_NAME, Utility.ToString(DR["SITE_NAME"]));
                        Utility.SetCtrl(txtCOMPANY, Utility.ToString(DR["COMPANY"]));

                        Utility.ShowLastUpdate(lblLastUpdated, Utility.ToString(DR["CREATED_BY"]), DR["CREATED_DATE"], Utility.ToString(DR["MODIFIED_BY"]), DR["MODIFIED_DATE"]);

                    }
                    else
                    {
                       
                        Msg = "";
                        PageAction = "Result('N', LastPage);";
                    }
                }
                else
                {
                    AddData();
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

        private void SaveData()
        {
            DataTable DT = new DataTable();
            int OPs;
            String rFID = "", rSiteName = "", rCompany = "";
            try
            {
                Key = Utility.GetCtrl(hidSITE_ID);
                if (Key == "")
                {
                    OPs = DBUTIL.opINSERT;
                }
                else
                {
                    OPs = DBUTIL.opUPDATE;
                }

                rFID = Validation.GetCtrlStr(txtFID).Trim();
                rSiteName = Validation.GetCtrlStr(txtSITE_NAME);
                // -- Add by Turk 18/04/2562 --> rCompany --
                rCompany = Validation.GetCtrlStr(txtCOMPANY);
 

                DT = Project.dal.SearchOffshoreFID("", rFID);
                if (DT.Rows.Count > 0 && OPs == DBUTIL.opINSERT)
                {
                    Msg = "Site already exist!";
                }
                else
                {
                    // -- Add by Turk 19/04/2562 --> MngSiteOFFFID --
                    Project.dal.MngSiteOFFFID(OPs, ref Key, rFID, rSiteName, rCompany);
                    Utility.SetCtrl(hidSITE_ID, Key);

                }
                if (Msg == "")
                {

                    LoadData();
                    Msg = ""; PageAction = "Result('S','');";
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

  
        private void DeleteData()
        {
            try
            {
                Key = Utility.GetCtrl(hidSITE_ID);
                if (Key == "")
                {
                    Msg = ""; PageAction = "Result('C');";
                }
                else
                {
 
                    Project.dal.MngSiteOFFFID(DBUTIL.opDELETE, ref Key);
                    Msg = ""; PageAction = "Result('D2', LastPage);";

                }

            }
            catch (Exception ex)
            {
                Msg = Utility.GetErrorMessage(ex);
            }
        }

    }
}