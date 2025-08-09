<%@ Page Title="" Language="C#" MasterPageFile="~/Includes/MainPage.Master" AutoEventWireup="true" CodeBehind="rptSPOT.aspx.cs" Inherits="PTT.GQMS.USL.Web.Reports.rptSPOT" MaintainScrollPositionOnPostback="True" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <!-- multiple Select2 -->
    <link href="../Content/select2.min.css" rel="stylesheet" />
    <script src="../Scripts/select2.full.min.js"></script>    
    
            <script type="text/javascript">
                var NextPage = "TemplateSPOT?UID=<%= HttpContext.Current.Session["UID"] %>";
                var NextPage2 = "ExcelSPOT?UID=<%= HttpContext.Current.Session["UID"] %>";
                var NextPage3 = "ChartSPOT?UID=<%= HttpContext.Current.Session["UID"] %>";

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
                          url=NextPage2+"&T="+sl+"&F="+sf+"&C="+GetCtrlValue("<%=ddlCOMP.ClientID%>")+"&MM1="+GetCtrlValue("<%=ddlMONTH.ClientID%>")+"&YY1="+GetCtrlValue("<%=ddlYEAR.ClientID%>")+"&MM2="+GetCtrlValue("<%=ddlMONTHTO.ClientID%>")+"&YY2="+GetCtrlValue("<%=ddlYEARTO.ClientID%>");

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

                        url=NextPage3+"&T="+sl+"&F="+sf+"&C="+GetCtrlValue("<%=ddlCOMP.ClientID%>")+"&MM1="+GetCtrlValue("<%=ddlMONTH.ClientID%>")+"&YY1="+GetCtrlValue("<%=ddlYEAR.ClientID%>")+"&MM2="+GetCtrlValue("<%=ddlMONTHTO.ClientID%>")+"&YY2="+GetCtrlValue("<%=ddlYEARTO.ClientID%>");

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
            <li class="active">Onshore Contaminate Report</li>
          </ol>
    </section>
    <!-- Main content -->
    <section class="content">
        <div class="row">

            <div class="col-xs-12">
                <div class='box box-success'>
                    <div class="box-header">
  
                       <h3 class="box-title"><i class="fa fa-file-text "></i> Graph</h3>
                    </div>
                    <div class="box-body">

                        <!-- left column -->
                        <div class="col-md-6">
                            <table border="0" class="text-black">
                                <tr>
                                    <td style="width: 135px; text-align: right; padding: 2px;">Month / Year&nbsp;:&nbsp;</td>
                                    <td style="width: 100px; text-align: left; padding: 2px;">
                                        <asp:DropDownList ID="ddlMONTH" runat="server" Width="100px" CssClass="form-control"></asp:DropDownList>
                                    </td>
                                    <td style="width: 85px; text-align: left; padding: 2px;">
                                        <asp:DropDownList ID="ddlYEAR" runat="server" Width="80px" CssClass="form-control"></asp:DropDownList>
                                    </td>
                                    <td>&nbsp;-&nbsp;</td>
                                    <td style="width: 100px; text-align: left; padding: 2px;">
                                        <asp:DropDownList ID="ddlMONTHTO" runat="server" Width="100px" CssClass="form-control"></asp:DropDownList>
                                    </td>
                                    <td style="width: 120px; text-align: left; padding: 2px;">
                                        <asp:DropDownList ID="ddlYEARTO" runat="server" Width="80px" CssClass="form-control"></asp:DropDownList>
                                    </td>
                                </tr>
                            </table>
                        </div>


                         <!-- right column -->
                        <div class="col-md-6">
                            <table border="0" class="text-black">
                                <tr>
                                    <td style="width: 150px; text-align: right; padding: 2px;">Select Value&nbsp;:&nbsp;</td>
                                    <td style="width: 100px; text-align: left; padding: 2px;">
                                        <asp:DropDownList ID="ddlCOMP" runat="server" Width="150px" CssClass="form-control select2">
                                            <asp:ListItem Value="SULFUR" Text="H2S - Total Sulfur"></asp:ListItem>
                                            <asp:ListItem Value="H2S" Text="H2S - H2S"></asp:ListItem>
                                            <asp:ListItem Value="COS" Text="H2S - COS"></asp:ListItem>
                                            <asp:ListItem Value="CH3SH" Text="H2S - CH3SH"></asp:ListItem>
                                            <asp:ListItem Value="C2H5SH" Text="H2S - C2H5SH"></asp:ListItem>
                                            <asp:ListItem Value="DMS" Text="H2S - DMS"></asp:ListItem>
                                            <asp:ListItem Value="LSH" Text="H2S - T-bulylSH"></asp:ListItem>
                                            <asp:ListItem Value="C3H7SH" Text="H2S - C3H7SH"></asp:ListItem>
                                            <asp:ListItem Value="HG" Text="HG"></asp:ListItem>
                                            <asp:ListItem Value="VOL" Text="HG - Vol"></asp:ListItem>
                                            <asp:ListItem Value="O2" Text="O2"></asp:ListItem>
                                            <asp:ListItem Value="HC" Text="HC - Temp"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                
                            </table>
                        </div>

                        <!-- left column -->
                        <div class="col-md-12" style="margin-left: -16px;">
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
                                    <td style="width: 150px; text-align: right; padding: 2px;">(OR) Sampling point&nbsp;:&nbsp;</td>
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
