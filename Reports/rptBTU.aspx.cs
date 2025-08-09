using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace PTT.GQMS.USL.Web.Reports
{
    //-- aor edit 22/08/2018 ---
    public partial class rptBTU : System.Web.UI.Page
    {
        public String Msg = "", ServerAction = "", PageAction = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            Security.CheckRole(Security.TaskRptSite, true);

            if (!this.IsPostBack)
            {

                InitCtrl();


            }


        }

        private void InitCtrl()
        {
            DataTable DT = new DataTable();
            try
            {
                DT = Project.dal.SearchSiteFID(orderSQL: " FID ");
                Utility.LoadList(ref ddlFID, DT, "FID", "SITE_ID", false, "");

                Utility.LoadMonthCombo(ref ddlMONTH, false, "", "EN", "");
                Utility.LoadYearCombo(ref ddlYEAR, "2015");


                DateTime today = System.DateTime.Today;
                if (today.Day < 6) //-- กรณีที่เป็นวันที่ 1,2,3,4,5 ของเดือน  ให้ระบบแสดงเดือนย้อนหลังก่อน
                {  //ให้แสดงเดือนย้อนหลัง
                    if (today.Month == 1)
                    {
                        Utility.SetCtrl(ddlMONTH, "12");
                        Utility.SetCtrl(ddlYEAR, (today.Year - 1).ToString());
                    }
                    else
                    {
                        Utility.SetCtrl(ddlMONTH, (today.Month - 1).ToString());
                        Utility.SetCtrl(ddlYEAR, today.Year.ToString());
                    }
                }
                else
                {
                    Utility.SetCtrl(ddlMONTH, today.Month.ToString());
                    Utility.SetCtrl(ddlYEAR, today.Year.ToString());
                }

                DT = Project.dal.SearchRptFidTemplate("1","", "");
                Utility.LoadList(ref ddlTEMPLATE, DT, "T_NAME", "TID");


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