<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NoRight.aspx.cs" Inherits="PTT.GQMS.USL.Web.NoRight" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Refresh" content="2;URL=Default.aspx" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>

            <asp:Label ID="Label1" Style="z-index: 101; left: 40px; position: absolute; top: 24px" runat="server"
                Width="760px" Font-Bold="True" Font-Size="Large" ForeColor="MidnightBlue">Access denied or session timeout!</asp:Label>
        </div>
    </form>
</body>
</html>
