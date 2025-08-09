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

    public partial class MngUserDetail : System.Web.UI.Page
    {
        public String ServerAction;
        public String Msg = "";
        public String PageAction = "";
        public String OP;
        public bool canEdit;
        public bool canDelete;
        public bool canAdd;
        public String SignPath = "Signature/";
    

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {

                Security.CheckRole(Security.TaskUser, true);
                SetCtrl();
                if (!this.IsPostBack)
                {
                    Utility.SetCtrl(txtUserName, Validation.GetParamStr("K", IsEncoded:true));
                    InitCtrl();
                    if (Utility.GetCtrl(txtUserName) != "") { ServerAction = "LOAD"; }
                    else { ServerAction = "ADD"; }
                }
                else
                {
                    OP = Validation.GetParamStr("OP");
                    ServerAction = Validation.GetParamStr("ServerAction");
                }

                switch (ServerAction)
                {
                    case "ADD": if (canAdd) { AddData(); } break;
                    case "LOAD": LoadData(); break;
                    case "SAVE": if (canAdd || canEdit) { SaveData(); } break;
                    case "DELETE": if (canDelete) { DeleteData(); } break;

                    case "DELETE_IMG": if (canEdit) { DeleteImage(); } break;

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

                DT = Project.dal.SearchRoleData("", "", "");
                Utility.LoadList(ref ddlRole, DT, "ROLE_NAME", "ROLE_ID", false, "");
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
                canEdit = Security.CanDo(Security.TaskUser, Security.actAdd);
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

        private void AddData()
        {
            try
            {
                OP = DBUTIL.opINSERT.ToString();
                Utility.SetCtrl(txtUserName, "");
                txtUserName.Enabled = true;
                txtUserName.CssClass = "form-control";

                Utility.SetCtrl(txtUserDesc, "");
                Utility.SetCtrl(txtPosition, "");
                Utility.SetCtrl(txtUnit, "");
                Utility.SetCtrl(txtEmail, "");
                Utility.SetListValue(ref ddlRole, "");
                Utility.SetListValue(ref rdbDisabled, "N");
                Utility.SetCtrl(lblLastUpdated, "");
                ImgAvatar.Visible = false;
                divImg.Visible = ImgAvatar.Visible;

            }
            catch (Exception ex)
            {
                Msg = Utility.GetErrorMessage(ex);
            }
        }

        private void LoadData()
        {
            DataTable DT = null;
            DataRow DR = null;
            try
            {

                DT = Project.dal.SearchUserData(Validation.GetCtrlStr(txtUserName).ToUpper(),"");
                DR = Utility.GetDR(ref DT);
                if (DR == null)
                {
                    Msg = ""; PageAction = "Result('N', LastPage);";
                }
                else
                {
                    OP = DBUTIL.opUPDATE.ToString();
                    Utility.SetCtrl(txtUserName, Utility.ToString(DR["USER_NAME"]), true);
                    txtUserName.Enabled = false; //--  enable=false ป้องกันไม่ให้เปลี่ยน username

                    Utility.SetCtrl(txtUserDesc, Utility.ToString(DR["USER_DESC"]));
                    Utility.SetCtrl(txtPosition, Utility.ToString(DR["POSITION_NAME"]));
                    Utility.SetCtrl(txtUnit, Utility.ToString(DR["UNIT_NAME"]));
                    Utility.SetCtrl(txtEmail, Utility.ToString(DR["USER_EMAIL"]));
                    Utility.SetListValue(ref ddlRole, Utility.ToString(DR["ROLE_ID"]));
                    string disableFlag = Utility.ToString(DR["DISABLED_FLAG"]);
                    if (disableFlag == "") { disableFlag = "N"; }
                    Utility.SetListValue(ref rdbDisabled, disableFlag);
                    Utility.SetCtrl(hidSIGN_FILENAME, Utility.ToString(DR["SIGN_FILENAME"]));


                    if (Utility.ToString(DR["SIGN_FILENAME"]) != "")
                    {
                        ImgAvatar.ImageUrl = Project.gFilePath + SignPath + Utility.ToString(DR["SIGN_FILENAME"]) + "?" + Utility.FormatDate(System.DateTime.Now, "HHMISS");                      

                        ImgAvatar.Visible = true;
                    }
                    else
                    {
                        ImgAvatar.Visible = false;
                    }
                    divImg.Visible = ImgAvatar.Visible;

                    Utility.ShowLastUpdate(lblLastUpdated, Utility.ToString(DR["CREATED_BY"]), DR["CREATED_DATE"], Utility.ToString(DR["MODIFIED_BY"]), DR["MODIFIED_DATE"]);


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
            DataTable DT = null;
            DataRow DR = null;
            int top;
            String Flag = "";
            String Key, Desc,  Role = "",Position , Unit, Email, EmpID;
            String SignFile = "";
                //-- edit 12/05/2025 -- write change log
            String OldDesc = "", OldPosition = "", OldUnit = "", OldEmail = "", OldRole = "", OldRoleName = "", OldFlag = "";

            try
            {
                Key = Validation.ValidateName(Utility.GetCtrl(txtUserName).ToUpper().Trim());
                if (Utility.ToInt(OP) == DBUTIL.opINSERT)
                {
                    top = DBUTIL.opINSERT;
                    DT = Project.dal.SearchUserData(Key, "");
                    DR = Utility.GetDR(ref DT);
                    if (DR != null)
                    {
                        Msg = "Username already exist!";
                    }
                }
                else
                {
                    top = DBUTIL.opUPDATE;

                    //-- edit 12/05/2025 -- write change log
                    DT = Project.dal.SearchUserData(Validation.GetCtrlStr(txtUserName).ToUpper(), "");
                    DR = Utility.GetDR(ref DT);
                    if (DR != null)
                    {
                        OldDesc = Utility.ToString(DR["USER_DESC"]);
                        OldPosition = Utility.ToString(DR["POSITION_NAME"]);
                        OldUnit = Utility.ToString(DR["UNIT_NAME"]);
                        OldEmail = Utility.ToString(DR["USER_EMAIL"]);
                        OldRole = Utility.ToString(DR["ROLE_ID"]);
                        OldRoleName = Utility.ToString(DR["ROLE_NAME"]);
                        OldFlag = Utility.ToString(DR["DISABLED_FLAG"]);
                    }

                }

                if (Msg == "")
                {
                    Desc = Utility.GetCtrl(txtUserDesc);
                    EmpID = Key;
                    Position = Utility.GetCtrl(txtPosition);
                    Unit = Utility.GetCtrl(txtUnit);
                    Email = Utility.GetCtrl(txtEmail);
                    Role = Utility.GetCtrl(ddlRole);
                    Flag = Utility.GetCtrl(rdbDisabled);

                    if (flUpload.HasFile == true)
                    {
                        SignFile = UploadUserFile(ref flUpload);
                    }
                    else
                    {
                        SignFile = null;
                    }


                    if ( Msg == "")
                    {
                        Project.dal.MngUserData(top, Key, Desc, Role, EmpID, Position, Unit, Email, Flag, SignFile);

                        //--  edit 12/05/2025 -- write log
                        if (top == DBUTIL.opINSERT)
                        {
                            BLL.InsertAudit(Project.catUserLog, "Add Username: " + Utility.GetCtrl(txtUserName).ToUpper(), "");
                        }
                        else
                        {  
                            String chgMsg = "";
                            if (Desc != OldDesc) chgMsg += " Full name " + " Old:" + OldDesc + " ___New:" + Desc + " <br />";
                            if (Position != OldPosition) chgMsg += " Position " + " Old:" + OldPosition + " ___New:" + Position + " <br />";
                            if (Unit != OldUnit) chgMsg += " Department " + " Old:" + OldUnit + " ___New:" + Unit + " <br />";
                            if (Email != OldEmail) chgMsg += " E-Mail " + " Old:" + OldEmail + " ___New:" + Email + " <br />";
                            if (Role != OldRole) chgMsg += " Role name " + " Old:" + OldRoleName + " ___New:" + ddlRole.SelectedItem.Text + " <br />";
                            if (Flag != OldFlag)
                            {
                                if (OldFlag == "N")
                                    chgMsg += " Status " + " Old:Disable" ;
                                else
                                    chgMsg += " Status " + " Old:Enable";
                                if (Flag == "N")
                                    chgMsg += " ___New:Disable";
                                else
                                    chgMsg += " ___New:Enable";
                            }

                            BLL.InsertAudit(Project.catUserLog, "Change Username: " + Utility.GetCtrl(txtUserName) + " <br />" + chgMsg , "");

                        }
                        

                        LoadData();
                        Msg = ""; PageAction = "Result('S','');";
                    }
                     
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
                if (Utility.ToInt(OP) == DBUTIL.opINSERT)
                {
                    Msg = "";
                    PageAction = "Result('C');";
                }
                else
                {
                    DeleteUserFile(Utility.GetCtrl(hidSIGN_FILENAME));

                    Project.dal.MngUserData(DBUTIL.opDELETE, Utility.GetCtrl(txtUserName) );
                    //--  edit 12/05/2025 -- write log
                    BLL.InsertAudit(Project.catUserLog, "Delete Username: " + Utility.GetCtrl(txtUserName), "");

                    Msg = ""; PageAction = "Result('D2', LastPage);";
                }

            }
            catch (Exception ex)
            {
                Msg = Utility.GetErrorMessage(ex);
            }
            finally
            {
                
            }
        }


        private void DeleteImage()
        {
            try
            {
                if (Utility.GetCtrl(hidSIGN_FILENAME) != "")
                {
                    DeleteUserFile(Utility.GetCtrl(hidSIGN_FILENAME));

                    Project.dal.MngUserData(DBUTIL.opUPDATE, Utility.GetCtrl(txtUserName), SignFilename: "NULL");

                    LoadData();
                }

            }
            catch (Exception ex)
            {
                Msg = Utility.GetErrorMessage(ex);
            }
            finally
            {

            }
        }

        //=====================================================================================
        private string UploadUserFile(ref FileUpload FileUpload)
        {
   
            String FullFileName = "", FileName = "", FileType = "";
            try
            {
                if (FileUpload.HasFile == false) { Msg = ""; PageAction = "Result('UE');"; }
                else
                {
                    FileType = Utility.GetFileType(FileUpload.FileName).ToLower();
                    if (FileType != "" && Project.gImgType.Contains(FileType))
                    {
                        if (FileUpload.PostedFile.ContentLength > 1097152) { Msg = "Unable to attach file ! Because this file is over 1 MB."; }
                        else
                        {
                         
                            FileName = Utility.GetCtrl(txtUserName).ToUpper().Trim() + FileType;
                            FullFileName = Server.MapPath(Project.gFilePath + SignPath + FileName);
                            //FileUpload.SaveAs(FullFileName);
                            //Utility.SaveThumbnail(FileUpload.FileBytes, FullFileName, 0, 70); //-- กำหนด image height = 70 px
                            Utility.SaveThumbnail(FileUpload.FileBytes, FullFileName, 0, 0); //-- กำหนด image height 


                            // if (Msg == "") { Msg = ""; PageAction = "Result('UC');"; }
                        }
                    }
                    else { Msg = "Unable to attach file ! Because this file is not type of image."; }
                }
                return FileName;
            }
            catch (Exception ex)
            {

                Msg = Utility.GetErrorMessage(ex);
                return "";
            }
        }

        private void DeleteUserFile(string FileName, bool ClearCtrl = true)
        {
            String FullFileName = "";
            try
            {
                if (FileName != "")
                {
                    FullFileName = Server.MapPath(Project.gFilePath + SignPath + FileName);
                    Utility.DeleteFileP(FullFileName);
                    
                }
            }
            catch (Exception ex)
            { Msg = Utility.GetErrorMessage(ex); }
        }



    }
}