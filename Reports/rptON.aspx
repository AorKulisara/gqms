<%@ Page Title="" Language="C#" MasterPageFile="~/Includes/MainPage.Master" AutoEventWireup="true" CodeBehind="rptON.aspx.cs" Inherits="PTT.GQMS.USL.Web.Reports.rptON" MaintainScrollPositionOnPostback="True" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <!-- multiple Select2 -->
    <link href="../Content/select2.min.css" rel="stylesheet" />
    <script src="../Scripts/select2.full.min.js"></script>   

    <!-- DataTable -->
    <script type="text/javascript" src="../Scripts/DataTables/DataTables/js/jquery.dataTables.min.js"></script>
    <script type="text/javascript" src="../Scripts/DataTables/FixedColumns/js/dataTables.fixedColumns.min.js"></script>
    <script type="text/javascript" src="../Scripts/pdfmake/vfs_fonts.js"></script>

    <script type="text/javascript">
        var NextPage = "TemplateDetail?UID=<%= HttpContext.Current.Session["UID"] %>";
        var NextPage2 = "ExcelON?UID=<%= HttpContext.Current.Session["UID"] %>";

        function DoAction(act, val) {
            var confirmed, isSubmitted, url, err, url2;
            confirmed = true; isSubmitted = true; url = ""; url2 = ""; err = "";
            switch (act) {
                case "NEW": url2 = NextPage; break;
                case "SEARCH": break;
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
                          url=NextPage2+"&T="+sl+"&F="+sf+"&C="+GetCtrlValue("<%=ddlRPT_TYPE.ClientID%>")+"&MM1="+GetCtrlValue("<%=ddlMONTH.ClientID%>")+"&YY1="+GetCtrlValue("<%=ddlYEAR.ClientID%>")+"&MM2="+GetCtrlValue("<%=ddlMONTHTO.ClientID%>")+"&YY2="+GetCtrlValue("<%=ddlYEARTO.ClientID%>");

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
            <li class="active">Onshore Summary</li>
          </ol>
    </section>
    <!-- Main content -->
    <section class="content">
        <div class="row">

            <div class="col-xs-12">
                <div class='box box-success'>
                    <div class="box-header">
                       <h3 class="box-title"><i class="fa fa-file-text "></i> Onshore Summary</h3>
                    </div>

                      <div class="box-body">

                        <!-- left column -->
                        <div class="col-md-6">
                            <table border="0" class="text-black">
                                <tr>
                                    <td style="width: 165px; text-align: right; padding: 2px;">Month / Year&nbsp;:&nbsp;</td>
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
                                    <td style="width: 150px; text-align: right; padding: 2px;">Select Period&nbsp;:&nbsp;</td>
                                    <td style="width: 100px; text-align: left; padding: 2px;">
                                        <asp:DropDownList ID="ddlRPT_TYPE" runat="server" Width="150px" CssClass="form-control">
                                            <asp:ListItem Value="1DAY" Text="รายงาน 1 วัน"></asp:ListItem>
                                            <asp:ListItem Value="10DAY" Text="รายงาน 10 วัน"></asp:ListItem>
                                            <asp:ListItem Value="15DAY" Text="รายงาน 15 วัน"></asp:ListItem>
                                            <asp:ListItem Value="ENDMTH" Text="รายเดือน"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                
                            </table>
                        </div>

                        <!-- left column -->
                        <div class="col-md-12" style="margin-left: -16px;">
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
                                <td style="width: 90px">
                                    <button type="button" class="pull-left btn btn-block btn-sametogetherproject" id="btnSEARCH" onclick="javascript: DoAction('SEARCH','');" >
                                    &nbsp;View</button>
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

                <div class="box">
                    <div class="box-body">
                        <asp:GridView ID="gvResult" runat="server" HorizontalAlign="Left" AutoGenerateColumns="False" OnRowDataBound="gvResult_RowDataBound"  >
                            <HeaderStyle CssClass="Table-head-gray" HorizontalAlign="Center" />
                            <FooterStyle CssClass="ItemFooter_green" HorizontalAlign="Center" />
                            <RowStyle CssClass="itemRow11" HorizontalAlign="Left" VerticalAlign="Middle" />
                            <PagerSettings Mode="NumericFirstLast" PageButtonCount="10" />
                            <PagerStyle CssClass="pagination-ys cell-borderW1" />
                            <AlternatingRowStyle CssClass="itemRow11" HorizontalAlign="Left" VerticalAlign="Middle" />

                            <Columns>
                                <asp:BoundField HeaderText="Location" DataField="FID" >
                                    <HeaderStyle CssClass="cell-center" Width="90px"/>
                                    <ItemStyle CssClass="cell-center cell-Middle cell-border11" Width="90px"/>
                                </asp:BoundField>
                                 <asp:BoundField HeaderText="Year" DataField="YY" >
                                    <HeaderStyle CssClass="cell-center" Width="50px"/>
                                    <ItemStyle CssClass="cell-center cell-Middle cell-border11" Width="50px"/>
                                </asp:BoundField>
                                <asp:BoundField HeaderText="Month" DataField="MM" >
                                    <HeaderStyle CssClass="cell-center" Width="60px"/>
                                    <ItemStyle CssClass="cell-center cell-Middle cell-border11" Width="60px"/>
                                </asp:BoundField>
                                <asp:BoundField HeaderText="Day" DataField="DDAY" >
                                    <HeaderStyle CssClass="cell-center" Width="70px"/>
                                    <ItemStyle CssClass="cell-center cell-Middle cell-border11" Width="70px"/>
                                </asp:BoundField>
                                <asp:BoundField HeaderText="CH4" DataField="" >
                                    <HeaderStyle CssClass="cell-center" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border11" Width="70px"/>
                                </asp:BoundField>
                                <asp:BoundField HeaderText="C2H6" DataField="" >
                                    <HeaderStyle CssClass="cell-center" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border11" Width="70px"/>
                                </asp:BoundField>
                                <asp:BoundField HeaderText="C3H8" DataField="" >
                                    <HeaderStyle CssClass="cell-center" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border11" Width="70px"/>
                                </asp:BoundField>
                                <asp:BoundField HeaderText="IC4H10" DataField="" >
                                    <HeaderStyle CssClass="cell-center" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border11" Width="70px"/>
                                </asp:BoundField>
                                <asp:BoundField HeaderText="NC4H10" DataField="" >
                                    <HeaderStyle CssClass="cell-center" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border11" Width="70px"/>
                                </asp:BoundField>
                                <asp:BoundField HeaderText="IC5H12" DataField="" >
                                    <HeaderStyle CssClass="cell-center" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border11" Width="70px"/>
                                </asp:BoundField>
                                <asp:BoundField HeaderText="NC5H12" DataField="" >
                                    <HeaderStyle CssClass="cell-center" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border11" Width="70px"/>
                                </asp:BoundField>
                                <asp:BoundField HeaderText="C6H14" DataField="" >
                                    <HeaderStyle CssClass="cell-center" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border11" Width="70px"/>
                                </asp:BoundField>
                                <asp:BoundField HeaderText="CO2" DataField="" >
                                    <HeaderStyle CssClass="cell-center" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border11" Width="70px"/>
                                </asp:BoundField>
                                <asp:BoundField HeaderText="N2" DataField="" >
                                    <HeaderStyle CssClass="cell-center" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border11" Width="70px"/>
                                </asp:BoundField>
                                 <asp:BoundField HeaderText="Hg" DataField="" >
                                    <HeaderStyle CssClass="cell-center" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border11" Width="70px"/>
                                </asp:BoundField>
                                <asp:BoundField HeaderText="H2S" DataField="" >
                                    <HeaderStyle CssClass="cell-center" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border11" Width="70px"/>
                                </asp:BoundField>
                                <asp:BoundField HeaderText="GHVsat" DataField="" >
                                    <HeaderStyle CssClass="cell-center" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border11" Width="70px"/>
                                </asp:BoundField>
                                <asp:BoundField HeaderText="SG" DataField="" >
                                    <HeaderStyle CssClass="cell-center" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border11" Width="70px"/>
                                </asp:BoundField>
                                  <asp:BoundField HeaderText="WI" DataField="" >
                                    <HeaderStyle CssClass="cell-center" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border11" Width="70px"/>
                                </asp:BoundField>
                                  <asp:BoundField HeaderText="H2O" DataField="" >
                                    <HeaderStyle CssClass="cell-center" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border11" Width="70px"/>
                                </asp:BoundField>
                                <asp:BoundField HeaderText="C2+" DataField="" >
                                    <HeaderStyle CssClass="cell-center" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border11" Width="70px"/>
                                </asp:BoundField>
                                <asp:BoundField HeaderText="CO2+N2" DataField="" >
                                    <HeaderStyle CssClass="cell-center" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border11" Width="70px"/>
                                </asp:BoundField>                        
                            </Columns>

                        </asp:GridView>
                    </div>
                </div>
                <!-- /.box-body -->
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
