using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PTT.GQMS.USL.Web.Includes
{   
    //-- aor created 30/01/2018 -- 
    public partial class Main : System.Web.UI.Page
    {
        public String Msg = "", ServerAction = "", PageAction = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
  
            }
            catch (Exception ex)
            {
                Msg = Utility.GetErrorMessage(ex);
            }
        }
    }
}