using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace PTT.GQMS.USL.Web.Reports
{
    //-- aor edit 27/06/2019 ---
    public partial class rptSPOT : System.Web.UI.Page
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
                Utility.LoadMonthCombo(ref ddlMONTH, false, "", "EN", "");
                Utility.LoadYearCombo(ref ddlYEAR, "2018");

                Utility.LoadMonthCombo(ref ddlMONTHTO, false, "", "EN", "");
                Utility.LoadYearCombo(ref ddlYEARTO, "2018");

                DateTime today = System.DateTime.Today;
                if (today.Day < 6) //-- กรณีที่เป็นวันที่ 1,2,3,4,5 ของเดือน  ให้ระบบแสดงเดือนย้อนหลังก่อน
                {  //ให้แสดงเดือนย้อนหลัง
                    if (today.Month == 1)
                    {
                        Utility.SetCtrl(ddlMONTH, "12");
                        Utility.SetCtrl(ddlYEAR, (today.Year - 1).ToString());

                        Utility.SetCtrl(ddlMONTHTO, "12");
                        Utility.SetCtrl(ddlYEARTO, (today.Year - 1).ToString());
                    }
                    else
                    {
                        Utility.SetCtrl(ddlMONTH, (today.Month - 1).ToString());
                        Utility.SetCtrl(ddlYEAR, today.Year.ToString());

                        Utility.SetCtrl(ddlMONTHTO, (today.Month - 1).ToString());
                        Utility.SetCtrl(ddlYEARTO, today.Year.ToString());
                    }
                }
                else
                {
                    Utility.SetCtrl(ddlMONTH, today.Month.ToString());
                    Utility.SetCtrl(ddlYEAR, today.Year.ToString());

                    Utility.SetCtrl(ddlMONTHTO, today.Month.ToString());
                    Utility.SetCtrl(ddlYEARTO, today.Year.ToString());
                }


                DT = Project.dal.SearchDimSamplingPoint();
                Utility.LoadList(ref ddlFID, DT, "FID", "AFID", false, "");

                DT = Project.dal.SearchRptFidTemplate("3","", "");
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