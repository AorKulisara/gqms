<%@ Page Title="" Language="C#" MasterPageFile="~/Includes/MainPage.Master" AutoEventWireup="true" CodeBehind="rptCompare.aspx.cs" Inherits="PTT.GQMS.USL.Web.Reports.rptCompare" MaintainScrollPositionOnPostback="True" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <!-- multiple Select2 -->
    <link href="../Content/select2.min.css" rel="stylesheet" />
    <script src="../Scripts/select2.full.min.js"></script>    
    
            <script type="text/javascript">
                var NextPage = "TemplateDetail?UID=<%= HttpContext.Current.Session["UID"] %>";
                var NextPage2 = "ExcelCompare?UID=<%= HttpContext.Current.Session["UID"] %>";
                var NextPage3 = "ChartCompare?UID=<%= HttpContext.Current.Session["UID"] %>";

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

                         //url=NextPage2+"&T="+sl+"&F="+sf+"&C="+GetCtrlValue("<%=ddlCOMP.ClientID%>")+"&MM="+GetCtrlValue("<%=ddlMONTH.ClientID%>")+"&YY="+GetCtrlValue("<%=ddlYEAR.ClientID%>"); 
                        url=NextPage2+"&T="+sl+"&F="+sf+"&C="+GetCtrlValue("<%=ddlCOMP.ClientID%>")+"&DF="+GetCtrlValue("<%=txtDateFrom.ClientID%>")+"&DT="+GetCtrlValue("<%=txtDateTo.ClientID%>"); 
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
                        //url=NextPage3+"&T="+sl+"&F="+sf+"&C="+GetCtrlValue("<%=ddlCOMP.ClientID%>")+"&MM="+GetCtrlValue("<%=ddlMONTH.ClientID%>")+"&YY="+GetCtrlValue("<%=ddlYEAR.ClientID%>"); 
                        url=NextPage3+"&T="+sl+"&F="+sf+"&C="+GetCtrlValue("<%=ddlCOMP.ClientID%>")+"&DF="+GetCtrlValue("<%=txtDateFrom.ClientID%>")+"&DT="+GetCtrlValue("<%=txtDateTo.ClientID%>");
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
            <li class="active">Onshore Site Compare Report</li>
          </ol>
    </section>
    <!-- Main content -->
    <section class="content">
        <div class="row">

            <div class="col-xs-12">
                <div class='box box-success'>
                    <div class="box-header">
                       <h3 class="box-title"><i class="fa fa-file-text "></i> Onshore Site Compare Report</h3>
                    </div>
                    <div class="box-body">

                        <!-- left column -->
                        <div class="col-md-6">
                            <table border="0" class="text-black">
                                <tr>
                                    <td style="width: 181px; text-align: right; padding: 2px;">From date&nbsp;:&nbsp;</td>
                                                                <td style="width: 120px;" >

                                                                             <div id="datepicker1" class="input-group date" style="vertical-align: baseline">
                                                                                <asp:TextBox ID="txtDateFrom" runat="server" class="form-control timepicker" Width="100px" data-date-language="en-US" maxlength='10'  ></asp:TextBox>
                                                                                 <div class="input-group-addon">
                                                                                        <i id="i1" class="fa fa-calendar"></i>
                                                                                 </div>
                                                                             </div>   
                                                                            <script>
                                                                                $(function () {
                                                                                    $("input[id$=txtDateFrom]").datepicker({
                                                                                        format: 'dd/mm/yyyy'
                                                                                    }).on('changeDate', function (selected) {
                                                                                        if ($(this).val() == "") {
                                                                                            $('input[id$=txtDateFrom]').datepicker('setEndDate', null);
                                                                                            $('input[id$=txtDateTo]').datepicker('setStartDate', null);
                                                                                            $('input[id$=txtDateTo]').datepicker('setEndDate', null);
                                                                                            $('input[id$=txtDateTo]').val("");
                                                                                        } else {
                                                                                            var dateFrom = new Date(selected.date.valueOf());
                                                                                            $('input[id$=txtDateTo]').datepicker('setStartDate', dateFrom);
                                                                                        }
                                                                                    });
                                                                                    //.keydown(false);
                                                                                    
                                                                                });
                                                                                $('#datepicker1').datepicker({
                                                                                    autoclose: true
                                                                                })
	                                                                         </script>
                                                                     </td> 
                                                                <td style="width: 40px; height: 27px;text-align: right; padding: 2px;">To&nbsp;:&nbsp;</td>
                                                        <td  style="width: 120px;">
                                                        <div id="$('#datepicker2').datepicker({
                                                                autoclose: true
                                                            })" class="input-group date" style="vertical-align: baseline">
                                                            <asp:TextBox ID="txtDateTo" runat="server" class="form-control timepicker" Width="100px" data-date-language="en-US" maxlength='10'   ></asp:TextBox>
                                                            <div class="input-group-addon">
                                                                            <i id="i2" class="fa fa-calendar"></i>
                                                                        </div>
                                                        </div>
                                                            <script>
                                                            $(function () {
                                                                $('input[id$=txtDateTo]').datepicker({
                                                                    format: 'dd/mm/yyyy'
                                                                }).on('changeDate', function (selected) {
                                                                    if ($(this).val() == "") {
                                                                        $('input[id$=txtDateFrom]').datepicker('setEndDate', null);
                                                                    } else {
                                                                        var dateTo = new Date(selected.date.valueOf());
                                                                        $('input[id$=txtDateFrom]').datepicker('setEndDate', dateTo);
                                                                    }
                                                                });
                                                                //.keydown(false);
                                                            });
                                                            $('#datepicker2').datepicker({
                                                                autoclose: true
                                                            })
                                                        	</script>
                                                                            
                                                        </td>
                                </tr>
                             
                            </table>
                        </div>
  <!--                      <div class="col-md-6">
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
-->

                         <!-- right column -->
                        <div class="col-md-6">
                            <table border="0" class="text-black">
                                <tr>
                                    <td style="width: 150px; text-align: right; padding: 2px;">Select Composition&nbsp;:&nbsp;</td>
                                    <td style="width: 100px; text-align: left; padding: 2px;">
                                        <asp:DropDownList ID="ddlCOMP" runat="server" Width="150px" CssClass="form-control select2">
                                            <asp:ListItem Value="C1" Text="CH4"></asp:ListItem>
                                            <asp:ListItem Value="C2" Text="C2H6"></asp:ListItem>
                                            <asp:ListItem Value="C3" Text="C3H8"></asp:ListItem>
                                            <asp:ListItem Value="IC4" Text="IC4H10"></asp:ListItem>
                                            <asp:ListItem Value="NC4" Text="NC4H10"></asp:ListItem>
                                            <asp:ListItem Value="IC5" Text="IC5H12"></asp:ListItem>
                                            <asp:ListItem Value="NC5" Text="NC5H12"></asp:ListItem>
                                            <asp:ListItem Value="C6" Text="C6H14"></asp:ListItem>
                                            <asp:ListItem Value="C7" Text="C7H16"></asp:ListItem>
                                            <asp:ListItem Value="CO2" Text="CO2"></asp:ListItem>
                                            <asp:ListItem Value="N2" Text="N2"></asp:ListItem>
                                            <asp:ListItem Value="H2S" Text="H2S"></asp:ListItem>
                                            <asp:ListItem Value="NHV" Text="NETHVDRY"></asp:ListItem>
                                            <asp:ListItem Value="GHV" Text="GHVSAT"></asp:ListItem>
                                            <asp:ListItem Value="SG" Text="SG"></asp:ListItem>
                                            <asp:ListItem Value="WC" Text="H2O"></asp:ListItem>
                                            <asp:ListItem Value="UNNORMMIN" Text="UNNORMMIN"></asp:ListItem>
                                            <asp:ListItem Value="UNNORMMAX" Text="UNNORMMAX"></asp:ListItem>
                                            <asp:ListItem Value="UNNORMALIZED" Text="UNNORM"></asp:ListItem>
                                            <asp:ListItem Value="WB" Text="WI"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                
                            </table>
                        </div>

                        <!-- left column -->
                        <div class="col-md-12">
                            <table border="0" class="text-black">
                                <tr>
                                    <td style="width: 180px; text-align: right; padding: 2px;">Select Template&nbsp;:&nbsp;</td>
                                    <td style="width: 100px; text-align: left; padding: 2px;">
                                         <asp:ListBox ID="ddlTEMPLATE" runat="server" Width="500px" SelectionMode="Multiple" CssClass="form-control select2">
                                        </asp:ListBox>
                                    </td>
                                    <td style="width: 140px; text-align: right; padding: 2px;">
                                       
                                    </td>
                                </tr>
                                <tr>
                                    <td style="width: 180px; text-align: right; padding: 2px;">(OR) Select OGC Main Point&nbsp;:&nbsp;</td>
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
