using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace PTT.GQMS.USL.Web.Settings
{
    //-- edit 05/07/2018 
    public partial class MngRoleDetail : System.Web.UI.Page
    {
        public string ServerAction;
        public string Msg = "", PageAction = "";
        public String tRights = "";
        public String OP;
        public String Key;

        public bool canAdd =true;
        public bool canEdit = true;
        public bool canDelete = true;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Security.CheckRole(Security.TaskRole, true);
                SetCtrl();
                if (!this.IsPostBack)
                {
                    Key = Validation.GetParamStr("K", IsEncoded:true);
                    if (Utility.IsNumeric(Key))
                    {
                        ServerAction = "LOAD";
                    }else{

                        ServerAction = "ADD";
                    }
                }
                else
                {
                    OP = Validation.GetParamIntStr("OP");
                    Key = Validation.GetParamStr("KEY");
                    ServerAction = Validation.GetParamStr("ServerAction");
                }

                

                switch (ServerAction)
                {
                    case "ADD": if (canAdd) { AddData(); } break;
                    case "LOAD": LoadData(); break;
                    case "SAVE":  if (canAdd || canEdit) { SaveData(); } 
                                break;
                    case "DELETE": if (canDelete) { DeleteData(); } break;
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
                //-- กำหนดให้มี 2 คอลัมน์คือ Read และ Add/Edit/Delete 
                canEdit = Security.CanDo(Security.TaskRole, Security.actAdd);
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
            DataTable DT = null;
            try
            {
                OP = DBUTIL.opINSERT.ToString();

                Utility.SetCtrl(txtName, "");
                Utility.SetCtrl(txtDesc, "");
                Utility.SetCtrl(lblLastUpdated, "");

                DT = Project.dal.SearchTaskList(orderSQL: "SEQ_NO, TASK_ID");
                Session["SYS_TASKS"] = DT;
                Utility.BindGVData(ref gvResult, DT,false);
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
            DataRow DR = null;
            DataTable DT = null;
            DataTable DT1 = null;
            try
            {
                if (Key == "") { Key = Validation.GetParamIntStr("KEY"); } 
                    DT = Project.dal.SearchRoleData(Key, "", ""); 
                    if (DT != null && DT.Rows.Count > 0) 
                    {
                        OP = DBUTIL.opUPDATE.ToString(); 
                        DR = Utility.GetDR(ref DT);
                        Key = Utility.ToString(DR["ROLE_ID"]);
                        Utility.SetCtrl(txtName, DR["ROLE_NAME"].ToString(),IsReadOnly: true );
                        Utility.SetCtrl(txtDesc, DR["ROLE_DESC"].ToString());

                        tRights = Utility.ToString(DR["RIGHTS"]);

                        DT1 = Project.dal.SearchTaskList(orderSQL: "SEQ_NO, TASK_ID"); 
                        Session["SYS_TASKS"] = DT1; 
                        Utility.BindGVData(ref gvResult, DT1, false);

                        Utility.ShowLastUpdate(lblLastUpdated, Utility.ToString(DR["CREATED_BY"]), DR["CREATED_DATE"], Utility.ToString(DR["MODIFIED_BY"]), DR["MODIFIED_DATE"]);

                }
                else
                    {
                        AddData();
                        Msg = "";
                        PageAction = "Result('N', LastPage);";
                    }
            }
            catch (Exception ex)
            {
                Msg = Utility.GetErrorMessage(ex);
            }
            finally
            {
                Utility.ClearObject(ref DT);
                Utility.ClearObject(ref DT1);
            }
        }

        private void SaveData()
        {
            DataTable DT = new DataTable();
            int OPs;
            String rName, rDesc, rRight;
            try
            {
                if (Utility.ToNum(OP) == DBUTIL.opINSERT)
                {
                    OPs = DBUTIL.opINSERT;
                }
                else
                {
                    OPs = DBUTIL.opUPDATE;
                }
                rName = Validation.GetCtrlStr(txtName);
                rDesc = Validation.GetCtrlStr(txtDesc);

                DT = Project.dal.SearchRoleData("",RoleName:rName,RoleDesc:"");
                if (DT.Rows.Count > 0 && OPs == DBUTIL.opINSERT)
                {
                    Msg = "Role name already exist!";
                }
                else
                {
                    rRight = GenRights();
                    Project.dal.MngRoleData(op: OPs, RoleID: ref Key, RoleName: rName, RoleDesc: rDesc, Rights: rRight);
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
                if (Utility.ToNum(OP) == DBUTIL.opINSERT)
                {
                    Msg = "";
                    PageAction = "Result('C');";
                }
                else
                {
                    if (Key == "1")
                    {
                        Msg = "";
                        PageAction = "Result('C');";  //-- ไม่ให้ลบ admin role
                    }
                    else
                    {
                        Project.dal.MngRoleData(DBUTIL.opDELETE, ref Key, "", "", "");
                        Msg = ""; PageAction = "Result('D2', LastPage);";  //'MngRoleList?UID=" + HttpContext.Current.Session["UID"] +"');";
                    }

                }

            }
            catch (Exception ex)
            {
                Msg = Utility.GetErrorMessage(ex);
            }
        }



        public string GenRights()
        {
            int[] SumOfValue;
            int IndexOfValue = 0;
            string StrRight = "";
            DataTable DT = null;

            try
            {
                DT = (DataTable)Session["SYS_TASKS"];
                if (DT != null && DT.Rows.Count > 0)
                {
                    SumOfValue = new int[50]; //-- กำหนดจำนวน task สูงสุด 49 
                    foreach (object Ctrl in Request.Form)
                    {
                        string c = Convert.ToString(Ctrl);
                        if (c.IndexOf("chk") >= 0)
                        {
                            IndexOfValue = Utility.ToInt(c.Substring(1 + c.IndexOf('_'), (c.Length - (c.IndexOf('_') + 1))));
                            SumOfValue[IndexOfValue] = SumOfValue[IndexOfValue] + Utility.ToInt("0" + Request.Form[Ctrl.ToString()].ToString());
                        }
                    }
                    for (int i = 1; i < SumOfValue.Length; i++)
                    {
                        StrRight += Convert.ToChar(SumOfValue[i] + 64);
                    }

                }    

            }
            catch (Exception ex)
            {
                StrRight = "";
            }
            return StrRight;
        }

        protected void gvResult_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            string tKey = "";
            string Status1 = "";
            string Status2 = "";
            int act = 0;

            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    tKey = gvResult.DataKeys[e.Row.RowIndex].Value.ToString();

                    try
                    {
                        act = (int)Convert.ToChar(tRights.Substring(Convert.ToInt32(tKey) - 1, 1)) - 64;
                    }
                    catch { }

                    if (Convert.ToBoolean(act & Security.actView))
                    {
                        Status1 = "checked";
                    }
                    else
                    {
                        Status1 = "";
                    }

                    if (Convert.ToBoolean(act & Security.actAdd))
                    {
                        Status2 = "checked";
                    }
                    else
                    {
                        Status2 = "";
                    }

                    e.Row.Cells[1].Text = "<input type='checkbox' value='" + Security.actView + "' name='chkRead_" + tKey + "'" + Status1 + " >";
                    e.Row.Cells[2].Text = "<input type='checkbox' value='" + Security.actAdd + "' name='chkAdd_" + tKey + "'" + Status2 + " >";

                }
            }
            catch (Exception ex)
            {
                Msg = Utility.GetErrorMessage(ex);
            }

        }

    }
}