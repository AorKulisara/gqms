<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExportXLS.aspx.cs" Inherits="PTT.GQMS.USL.Web.Forms.ExportXLS" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <meta http-equiv="X-UA-Compatitble" content="IE=edge,Chrome=1" />
    <title>PTT OGC</title>
    <meta content='width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no' name='viewport' />

    <!-- jQuery 1.11.3 and 2.1.4 -->
    <script src="../Scripts/jquery.min.js" type="text/javascript"></script>

    <!-- DataTable -->
    <link rel="stylesheet" type="text/css" href="../Scripts/DataTables/DataTables/css/jquery.dataTables.min.css" />
    <link rel="stylesheet" type="text/css" href="../Scripts/DataTables/Buttons/css/buttons.dataTables.min.css" />

    <script type="text/javascript" src="../Scripts/DataTables/DataTables/js/jquery.dataTables.min.js"></script>
    <script type="text/javascript" src="../Scripts/DataTables/Buttons/js/dataTables.buttons.min.js"></script>
    <script type="text/javascript" src="../Scripts/DataTables/JSZip/jszip.min.js"></script>
    <script type="text/javascript" src="../Scripts/DataTables/Buttons/js/buttons.html5.min.js"></script>
    <script type="text/javascript" src="../Scripts/pdfmake/pdfmake.min.js"></script>
    <script type="text/javascript" src="../Scripts/pdfmake/vfs_fonts.js"></script>


<script type="text/javascript">
// ------------------ DataTable -------------------        

  $(function () {

        $('#<%=gvResult.ClientID%>').DataTable({
            dom: 'Bfrtip',
            searching: false,
            sort: false,
            paging: false,
            buttons: [
                {
                    extend: 'excelHtml5',
                    title: '',
                    filename: "<%= pFileName%>",
                },
                {
                    extend: 'pdfHtml5',
                    footer: true,
                    //exportOptions: {
                   //     modifier: {
                   //         page: 'current'
                   //     },
                   // },
                    header: true,
                    title: '',
                    filename: "<%= pFileName%>",
                    //orientation: 'landscape'
                    //,customize: function (doc) {
                    //    doc.defaultStyle.fontSize = 10;
                    //    doc.content.splice(1, 0, {
                    //        margin: [0, -50, 0, 5],
                    //        alignment: 'left',
                    //        image: imgs
                    //    });
                    //} 
                }
            ]
        });

        $('.buttons-excel').hide();
        $('.buttons-pdf').hide();

        <% if (PageAction != "") Response.Write(PageAction); %>
    });

        // -------------------------------------------------        
</script>


<style type="text/css">
html,body{min-height:100%}
.layout-boxed html, .layout-boxed body{height: 100%;background-color: #FFFFFF;}
body{-webkit-font-smoothing:antialiased;-moz-osx-font-smoothing:grayscale;font-family:'Source Sans Pro','Helvetica Neue',Helvetica,Arial,sans-serif;font-weight:400;
     overflow-x:auto;overflow-y:auto;font-size:11px;}

    table.dataTable thead th, table.dataTable thead td {
        padding: 1px 10px ! important;
    }

    table.dataTable tbody th, table.dataTable tbody td
    {
	    padding:	0px, 0px ! important;
    }

</style>


</head>
<body>
    <form id="frm" runat="server">
        <asp:GridView ID="gvResult" runat="server" HorizontalAlign="Left" AutoGenerateColumns="False" DataKeyNames="" GridLines="Both" OnRowDataBound="gvResult_RowDataBound" >
            <HeaderStyle CssClass="Table-head-gray" HorizontalAlign="Center" />
            <FooterStyle CssClass="" HorizontalAlign="Center" />
            <RowStyle CssClass="itemRow1" HorizontalAlign="Left" VerticalAlign="Top" />
            <AlternatingRowStyle CssClass="itemRow1" HorizontalAlign="Left" VerticalAlign="Top" />
            <Columns>

            </Columns>
        </asp:GridView>
    </form>
</body>

    <script type="text/javascript" >
    Minimize();
    setTimeout("CloseWindow()", 15000);//milliseconds

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
