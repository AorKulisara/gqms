using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace PTT.GQMS.USL.Web.Reports
{
    //-- aor edit 28/06/2019 ---
    public partial class rptCompareOFF : System.Web.UI.Page
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

                DT = Project.dal.SearchSiteOffshoreFID(orderSQL: " FID ");
                Utility.LoadList(ref ddlFID, DT, "FID", "SITE_ID", false, "");

                DateTime today = System.DateTime.Today;
                DateTime tmpday = today;
                String FDate = "", TDate = "";
                if (today.Day < 6) //-- กรณีที่เป็นวันที่ 1,2,3,4,5 ของเดือน  ให้ระบบแสดงเดือนย้อนหลังก่อน
                {  //ให้แสดงเดือนย้อนหลัง
                    if (today.Month == 1)
                    {
                        FDate = "01/12/" + (today.Year - 1).ToString();
                        TDate = "31/12/" + (today.Year - 1).ToString();
                    }
                    else
                    {
                        FDate = "01/" + (today.Month - 1).ToString().PadLeft(2, '0') + "/" + (today.Year).ToString();
                        tmpday = Convert.ToDateTime(Utility.AppDateValue(FDate)).AddMonths(1).AddDays(-1);
                        TDate = Utility.AppFormatDate(tmpday);
                    }
                }
                else
                {
                    FDate = "01/" + (today.Month).ToString().PadLeft(2, '0') + "/" + (today.Year).ToString();
                    tmpday = Convert.ToDateTime(Utility.AppDateValue(FDate)).AddMonths(1).AddDays(-1);
                    TDate = Utility.AppFormatDate(tmpday);
                }
                Utility.SetCtrl(txtDateFrom, FDate, false);
                Utility.SetCtrl(txtDateTo, TDate, false);


                DT = Project.dal.SearchRptFidTemplate("2", "", "");
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