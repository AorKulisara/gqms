<%@ Page Title="" Language="C#" MasterPageFile="~/Includes/MainPage.Master" AutoEventWireup="true" CodeBehind="rptBTU.aspx.cs" Inherits="PTT.GQMS.USL.Web.Reports.rptBTU" MaintainScrollPositionOnPostback="True" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <!-- multiple Select2 -->
    <link href="../Content/select2.min.css" rel="stylesheet" />
    <script src="../Scripts/select2.full.min.js"></script>    
    
            <script type="text/javascript">
                var NextPage = "TemplateDetail?UID=<%= HttpContext.Current.Session["UID"] %>";
                var NextPage2 = "ExcelBTU?UID=<%= HttpContext.Current.Session["UID"] %>";
                var NextPage3 = "ChartBTU?UID=<%= HttpContext.Current.Session["UID"] %>";

            function DoAction(act, val) {
                var confirmed, isSubmitted, url, err, url2;
                confirmed = true; isSubmitted = true; url = ""; url2 = ""; err = "";
                switch (act) {
                    case "NEW": url2 = NextPage; break;
                    case "EXPORT_XLS":
                        var tt = document.getElementById("<%=ddlTEMPLATE.ClientID%>");
                        var sl = "";
                        for (var i = 0; i < tt.options.length; i++) {
                            if (tt.options[i].selected == true) {
                                 sl += tt.options[i].value +",";
                            }
                        }

                        var ff = document.getElementById("<%=ddlFID.ClientID%>");
                        var sf = "";
                        for (var i = 0; i < ff.options.length; i++) {
                            if (ff.options[i].selected == true) {
                                 sf += ff.options[i].value +",";
                            }
                        }

                         url=NextPage2+"&T="+sl+"&F="+sf+"&MM="+GetCtrlValue("<%=ddlMONTH.ClientID%>")+"&YY="+GetCtrlValue("<%=ddlYEAR.ClientID%>"); 
                        break;
                    case "GRAPH": 
                        var tt = document.getElementById("<%=ddlTEMPLATE.ClientID%>");
                        var sl = "";
                        for (var i = 0; i < tt.options.length; i++) {
                            if (tt.options[i].selected == true) {
                                 sl += tt.options[i].value +",";
                            }
                        }

                        var ff = document.getElementById("<%=ddlFID.ClientID%>");
                        var sf = "";
                        for (var i = 0; i < ff.options.length; i++) {
                            if (ff.options[i].selected == true) {
                                 sf += ff.options[i].value +",";
                            }
                        }

                         url=NextPage3+"&T="+sl+"&F="+sf+"&MM="+GetCtrlValue("<%=ddlMONTH.ClientID%>")+"&YY="+GetCtrlValue("<%=ddlYEAR.ClientID%>"); 
                        break;
                }

                if (err != "") {
                    AlertModal(err);
                } else if (url != "") {
                    window.open(url, target = val);
                } else if (url2 != "") {
                    LoadSpin();
                    window.location.href = url2;
                } else if (confirmed && isSubmitted) {
                    SetCtrlValue("ServerAction", act);
                    LoadSpin();
                    SubmitForm();
                }
            }


    </script>

 
 
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HeadStyle" runat="server">

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <input type="hidden" id="ServerAction" name="ServerAction" />



    <!-- Content Header (Page header) -->
    <section class="content-header">
        <ol class="breadcrumb">
            <li><i class="fa fa-home"></i></li>
            <li>Reports </li>
            <li class="active">BTU compare report</li>
          </ol>
    </section>
    <!-- Main content -->
    <section class="content">
        <div class="row">

            <div class="col-xs-12">
                <div class='box box-success'>
                    <div class="box-header">
                       <h3 class="box-title"><i class="fa fa-file-text "></i> BTU compare report</h3>
                    </div>
                    <div class="box-body">

                        <!-- left column -->
                        <div class="col-md-6">
                            <table border="0" class="text-black">
                                <tr>
                                    <td style="width: 150px; text-align: right; padding: 2px;">Select Month / Year&nbsp;:&nbsp;</td>
                                    <td style="width: 100px; text-align: left; padding: 2px;">
                                         <asp:DropDownList ID="ddlMONTH" runat="server" Width="100px" CssClass="form-control">

							            </asp:DropDownList>
                                        </td>
                                   <td style="width: 120px; text-align: left; padding: 2px;">
                                        <asp:DropDownList ID="ddlYEAR" runat="server" Width="80px" CssClass="form-control">
							            </asp:DropDownList>
                                    </td>
                                </tr>
                             
                            </table>
                        </div>

                        <!-- left column -->
                        <div class="col-md-12">
                            <table border="0" class="text-black">
                                <tr>
                                    <td style="width: 150px; text-align: right; padding: 2px;">Select Template&nbsp;:&nbsp;</td>
                                    <td style="width: 100px; text-align: left; padding: 2px;">
                                         <asp:ListBox ID="ddlTEMPLATE" runat="server" Width="500px" SelectionMode="Multiple" CssClass="form-control select2">
                                        </asp:ListBox>
                                    </td>
                                    <td style="width: 140px; text-align: right; padding: 2px;">
                                        
                                    </td>
                                </tr>
                                <tr>
                                    <td style="width: 150px; text-align: right; padding: 2px;">(OR) Select FID&nbsp;:&nbsp;</td>
                                    <td style="width: 100px; text-align: left; padding: 2px;">
                                         <asp:ListBox ID="ddlFID" runat="server" Width="500px" SelectionMode="Multiple" CssClass="form-control select2">
                                         </asp:ListBox>
                                    </td>
                                    <td style="width: 140px; text-align: right; padding: 2px;">
                                        
                                    </td>
                                </tr>
                                
                            </table>
                        </div>

                    </div>
                    
             <!-- /.box-body -->

<!---------------------------------------------------------->
                    <div class="box-footer">

                        <table border="0" style="margin-left: 35%;">
                            <tr>
                                <td style="width: 120px">
                                    <button type="button" class="pull-left btn btn-block btn-sametogetherproject" id="btnGRAPH" onclick="javascript: DoAction('GRAPH','');" >
                                    <i class="fa fa-line-chart"></i> &nbsp;Graph</button>
                                </td>
                                <td style="width: 20px">
                                </td>
                                <td style="width: 120px">
                                    <button type="button" class="pull-left btn btn-block btn-sametogetherproject" id="btnXLS" onclick="javascript: DoAction('EXPORT_XLS','');" >
                                    <i class="fa fa-file-excel-o"></i> &nbsp;Export Excel</button>
                                </td>
                                <td style="width: 20px">
                                </td>
                                <td style="width: 120px">
                                    <input name="btnNew" class="btn btn-block btn-success" type="button" id="btnNew" value="New template" onclick="javascript: DoAction('NEW','','');" style="width:110px; height:30px;" />
                                    </td>
                            </tr>
                        </table>
                    </div>

                </div>

            </div>
            <!-- /.col -->
        </div>
        <!-- /.row -->

    </section>
    <!-- /.content -->




</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="FootScript" runat="server">
    <!-- Page script -->
<script>
    $(function () {
        //Initialize Select2 Elements
        $('.select2').select2()

    })
</script>

                <!--#include file="../Includes/JSfooter.html" -->
</asp:Content>
