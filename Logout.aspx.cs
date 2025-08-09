using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PTT.GQMS.USL.Web
{
    public partial class Logout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Session.Clear();
                //Response.Redirect("Default.aspx?MODE=SIGNOFF");
            }
            catch (Exception ex)
            {
                Session.Clear();
            }
        }
    }
}