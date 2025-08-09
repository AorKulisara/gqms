using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Data;

namespace PTT.GQMS.USL.Web.Includes
{
    //-- aor edit 25/05/2018 --
    public partial class MainPage : System.Web.UI.MasterPage
    {
        public static string Msg = "";
        public String PageAction;
        public String gMasterFileAttach = Project.gFileAttach; 
        public String MenuAction;
        
        protected void Page_Load(object sender, EventArgs e)
        {          
            try
            {
 
                    //-- ต้อง set menu ทุกครั้ง เนื่องจากบางที page index แล้วเห็นเมนูที่ไม่มีสิทธิ์
                    SetInVisibleMenu();
    


                //-- aor edit 25/04/2018 --
                HttpContext.Current.Session["CURRENT_PAGE"] = Utility.ToString(Path.GetFileName(Request.PhysicalPath));

            }
            catch (Exception ex)
            {
                Msg = Utility.GetErrorMessage(ex);
            }
            
        }

        //-- แสดงเมนูตามสิทธิ์
        private void SetInVisibleMenu()
        {
            //---- Upload Excel ---
            if (Security.CheckRole(Security.TaskUpload, false) == false)
            {
                MenuAction += " $(\"#Menu1\").css(\"display\", \"none\") " + Environment.NewLine;
            }

            //---- Verification ---
            if (Security.CheckRole(Security.TaskVerify, false) == false)
            {
                MenuAction += " $(\"#Menu2\").css(\"display\", \"none\") " + Environment.NewLine;
            }

            //---- Report ---
            if (Security.CheckRole(Security.TaskRptMonthly, false) == false && Security.CheckRole(Security.TaskRptSite, false) == false && Security.CheckRole(Security.TaskRptBTU, false) == false )
            {
                MenuAction += " $(\"#Menu3\").css(\"display\", \"none\") " + Environment.NewLine;

            }
            else
            {
                if (Security.CheckRole(Security.TaskRptMonthly, false) == false)
                {
                    MenuAction += " $(\"#Menu3-1\").css(\"display\", \"none\") " + Environment.NewLine;
                }
                if (Security.CheckRole(Security.TaskRptSite, false) == false)
                {
                    MenuAction += " $(\"#Menu3-2\").css(\"display\", \"none\") " + Environment.NewLine;
                }
                if (Security.CheckRole(Security.TaskRptBTU, false) == false)
                {
                    MenuAction += " $(\"#Menu3-3\").css(\"display\", \"none\") " + Environment.NewLine;
                }
                if (Security.CheckRole(Security.TaskRptSite, false) == false && Security.CheckRole(Security.TaskRptBTU, false) == false)
                {
                    MenuAction += " $(\"#Menu3-4\").css(\"display\", \"none\") " + Environment.NewLine;
                }
 
            }


            //---- Master Data ---
            if (Security.CheckRole(Security.TaskMDSite, false) == false && Security.CheckRole(Security.TaskMDTag, false) == false && Security.CheckRole(Security.TaskMDMail, false) == false && Security.CheckRole(Security.TaskMDTransfer, false) == false 
                && Security.CheckRole(Security.TaskMDCustomer, false) == false)
            {
                MenuAction += " $(\"#Menu4\").css(\"display\", \"none\") " + Environment.NewLine;

            }
            else
            {
                if (Security.CheckRole(Security.TaskMDSite, false) == false)
                {
                    MenuAction += " $(\"#Menu4-1\").css(\"display\", \"none\") " + Environment.NewLine;
                    MenuAction += " $(\"#Menu4-2\").css(\"display\", \"none\") " + Environment.NewLine;
                    MenuAction += " $(\"#Menu4-7\").css(\"display\", \"none\") " + Environment.NewLine;
                    MenuAction += " $(\"#Menu4-8\").css(\"display\", \"none\") " + Environment.NewLine;
                    
                }
                if (Security.CheckRole(Security.TaskMDTag , false) == false)
                {
                    MenuAction += " $(\"#Menu4-3\").css(\"display\", \"none\") " + Environment.NewLine;
                    MenuAction += " $(\"#Menu4-6\").css(\"display\", \"none\") " + Environment.NewLine;
                }
                if (Security.CheckRole(Security.TaskMDMail, false) == false)
                {
                    MenuAction += " $(\"#Menu4-4\").css(\"display\", \"none\") " + Environment.NewLine;
                }
                if (Security.CheckRole(Security.TaskMDTransfer, false) == false)
                {
                    MenuAction += " $(\"#Menu4-5\").css(\"display\", \"none\") " + Environment.NewLine;
                }

                if (Security.CheckRole(Security.TaskMDCustomer, false) == false)
                {
                    MenuAction += " $(\"#Menu4-9\").css(\"display\", \"none\") " + Environment.NewLine;
                }
            }


            //---- Security ---
            if (Security.CheckRole(Security.TaskRole, false) == false && Security.CheckRole(Security.TaskUser, false) == false && Security.CheckRole(Security.TaskEventLog, false) == false && Security.CheckRole(Security.TaskExceptionLog, false) == false)
            {
                MenuAction += " $(\"#Menu5\").css(\"display\", \"none\") " + Environment.NewLine;
            }
            else
            {
                if (Security.CheckRole(Security.TaskRole, false) == false)
                {
                    MenuAction += " $(\"#Menu5-1\").css(\"display\", \"none\") " + Environment.NewLine;
                }
                if (Security.CheckRole(Security.TaskUser, false) == false)
                {
                    MenuAction += " $(\"#Menu5-2\").css(\"display\", \"none\") " + Environment.NewLine;
                }
                if (Security.CheckRole(Security.TaskEventLog, false) == false)
                {
                    MenuAction += " $(\"#Menu5-3\").css(\"display\", \"none\") " + Environment.NewLine;
                }
                if (Security.CheckRole(Security.TaskExceptionLog, false) == false)
                {
                    MenuAction += " $(\"#Menu5-4\").css(\"display\", \"none\") " + Environment.NewLine;
                }
            }

        }



    }
}