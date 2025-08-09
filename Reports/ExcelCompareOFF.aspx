<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExcelCompareOFF.aspx.cs" Inherits="PTT.GQMS.USL.Web.Reports.ExcelCompareOFF" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>OGC</title>
</head>
<body>
    <form id="frm" runat="server">
    <div>
    
    </div>
    </form>
</body>

<script type="text/javascript" >
    window.location.href = "../dat/Excel/Export/<%=rOpenFile%>?pRDM=" + Math.random().toString(36).substring(7),     //--- &pRDM ใส่เพื่อให้เปิดไฟล์ใหม่ทุกครั้ง
    Minimize();
    setTimeout("CloseWindow()", 10000);//milliseconds

function Minimize() {
        window.innerWidth = 100;
        window.innerHeight = 100;
        window.screenX = screen.width;
        window.screenY = screen.height;
        alwaysLowered = true;
    }


function CloseWindow()
{
    window.opener='X'; 
    window.open('','_parent',''); 
    window.close();
}
</script>

</html>
