using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Web.UI.HtmlControls;

namespace PTT.GQMS.USL.Web.Master
{
    //-- edit 05/07/2018
    public partial class MngMailTemplate : System.Web.UI.Page
    {
        public string ServerAction = "";
        public string Msg = "", PageAction = "";
        public bool canAdd = true;
        public bool canEdit = true;
        public bool canDelete = true;
        public String uUpdate2;
        public object dUpdate2;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Security.CheckRole(Security.TaskMDMail, true);

                SetCtrl();
                if (!this.IsPostBack)
                {
                    HttpContext.Current.Session["CheckRefresh"] = Server.UrlDecode(System.DateTime.Now.ToString()); //Prevent duplicate insert on page refresh

                    InitCtrl();
                    if (ServerAction == "") { ServerAction = "LOAD"; }
                }
                else
                {
                    ServerAction = Validation.GetParamStr("ServerAction");
                }



                switch (ServerAction)
                {
                    case "LOAD": LoadData(); break;
                    case "SAVE":
                        if (HttpContext.Current.Session["CheckRefresh"].ToString() == ViewState["CheckRefresh"].ToString()) //Prevent duplicate insert on page refresh
                        {
                            SaveData();
                            HttpContext.Current.Session["CheckRefresh"] = Server.UrlDecode(System.DateTime.Now.ToString()); //give a new value to session
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
                canEdit = Security.CanDo(Security.TaskMDMail, Security.actAdd);
                canDelete = canEdit;
                canAdd = canEdit;

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
            try
            {
                
                Utility.SetCtrl(rdbDisabled, "Y");
                Utility.SetCtrl(txtMAIL_TO, "");
                Utility.SetCtrl(txtMAIL_CC, "");
                Utility.SetCtrl(txtMAIL_BCC, "");
                Utility.SetCtrl(txtSUBJECT, "");
                Utility.SetCtrl(editor1, "");
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
            DataRow DR = null;

            try
            {
                Utility.SetCtrl(hidETEMPLATE_ID, Utility.GetCtrl(ddlALERT_EMAIL));

                DT = Project.dal.SearchEmailTemplate(Utility.GetCtrl(hidETEMPLATE_ID) , "", "", "", "", "");
                if (DT.Rows.Count != 0)
                {
                    DR = Utility.GetDR(ref DT);
                    Utility.SetCtrl(rdbDisabled, Utility.ToString(DR["ACTIVE_FLAG"]), false);
                    Utility.SetCtrl(txtMAIL_TO, Utility.ToString(DR["MAIL_TO"]), false);
                    Utility.SetCtrl(txtMAIL_CC, Utility.ToString(DR["MAIL_CC"]), false);
                    Utility.SetCtrl(txtMAIL_BCC, Utility.ToString(DR["MAIL_BCC"]), false);
                    Utility.SetCtrl(txtSUBJECT, Utility.ToString(DR["SUBJECT"]), false);
                    Utility.SetCtrl(editor1, Utility.ToString(DR["MESSAGE"]), false);

                    Utility.ShowLastUpdate(lblLastUpdated, Utility.ToString(DR["CREATED_BY"]), DR["CREATED_DATE"], Utility.ToString(DR["MODIFIED_BY"]), DR["MODIFIED_DATE"]);

                }
                else
                {
                    AddData();
                    Msg = "";
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

        protected void ddlALERT_EMAIL_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void SaveData()
        {
            DataTable DT = null;
            String Status = "";
            String MailTo = "";
            String MailCC = "";
            String MailBCC = "";
            String Subject = "";
            String Message = "";
            int OPs;

            try
            {
                Utility.SetCtrl(hidETEMPLATE_ID, Utility.GetCtrl(ddlALERT_EMAIL));

                Status = Utility.GetCtrl(rdbDisabled);
                MailTo = Utility.GetCtrl(txtMAIL_TO);
                MailCC = Utility.GetCtrl(txtMAIL_CC);
                MailBCC = Utility.GetCtrl(txtMAIL_BCC);
                Subject = Utility.GetCtrl(txtSUBJECT);
                Message = Utility.GetCtrl(editor1);
               
                DT = Project.dal.SearchEmailTemplate(Utility.GetCtrl(hidETEMPLATE_ID), "", "", "", "", "", "", "");
                if (DT.Rows.Count == 0)
                {
                    OPs = DBUTIL.opINSERT;
                }
                else
                {
                    OPs = DBUTIL.opUPDATE;
                }
                Project.dal.MngEmailTemplate(OPs, Utility.GetCtrl(hidETEMPLATE_ID), MailTo, MailCC, MailBCC, Subject, Message, Status);
                Msg = "Save completed.";
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



    }
}