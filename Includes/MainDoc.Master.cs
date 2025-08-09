using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

namespace PTT.GQMS.USL.Web.Includes
{
    //-- aor edit 05/05/2017 --
    public partial class MainDoc : System.Web.UI.MasterPage
    {
        public static string Msg = "";
        public String PageAction;
        protected void Page_Load(object sender, EventArgs e)
        {

            //-- aor edit 25/04/2018 --
            HttpContext.Current.Session["CURRENT_PAGE"] = Utility.ToString(Path.GetFileName(Request.PhysicalPath));

        }
    }
}