<%@ Page Title="" Language="C#" MasterPageFile="~/Includes/MainPage.Master" AutoEventWireup="true" CodeBehind="MngSite.aspx.cs" Inherits="PTT.GQMS.USL.Web.Master.MngSite" ValidateRequest="False" MaintainScrollPositionOnPostback="True" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

        <script type="text/javascript">
            var NextPage1 = "MngSiteDetail?UID=<%= HttpContext.Current.Session["UID"] %>";

            function DoAction(act, val) {
                var confirmed, isSubmitted, url, err, url2;
                confirmed = true; isSubmitted = true; url = ""; url2 = ""; err = "";
                switch (act) {
                    case "ADD": url2 = NextPage1;  break;
                    case "SELECT": url2 = NextPage1 + "&K=" + val + ""; break;
                    case "SEARCH": break;
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
            <li>Master data</li>
            <li class="active">Onshore Site management </li>
        </ol>
    </section>
    <!-- Main content -->
    <section class="content">

     <div class="row">

            <div class="col-xs-12">
                <div class='box box-violet'>
                    <div class="box-header">
                       <h3 class="box-title"><i class="fa fa-search"></i> Search</h3>
                    </div>
                    <div class="box-body">

                        <!-- left column -->
                        <div class="col-md-6">
                            <table border="0" class="text-black">
                                <tr>
                                    <td style="width: 150px; text-align: right; padding: 2px;">OGC Main Point&nbsp;:&nbsp;</td>
                                    <td style="width: 290px; text-align: left; padding: 2px;">
                                        <asp:TextBox ID="txtFID" CssClass="form-control" runat="server" Width="200px" MaxLength="50" Text=""></asp:TextBox></td>
                                </tr>
                            </table>
                        </div>
                         <!-- right column -->
                      
                        <div class="col-md-6">
                            <table border="0" class="text-black">
                                <tr>
                                    <td style="width: 150px; text-align: right; padding: 2px;">Site description&nbsp;:&nbsp;</td>
                                    <td style="width: 290px; text-align: left; padding: 2px;">
                                        <asp:TextBox ID="txtSITE_NAME" CssClass="form-control" runat="server" Width="200px" MaxLength="50" Text=""></asp:TextBox></td>
                                </tr>
                            </table>
                        </div>

                        <!-- left column -->
                        <div class="col-md-6">
                            <table border="0" class="text-black">
                                <tr>
                                    <td style="width: 150px; text-align: right; padding: 2px;">Region&nbsp;:&nbsp;</td>
                                    <td style="width: 290px; text-align: left; padding: 2px;">
                                         <asp:DropDownList ID="ddlREGION_ID" runat="server" Width="200px" CssClass="form-control">
							            </asp:DropDownList></td>
                                </tr>
                            </table>
                        </div>

                             <!-- right column -->
                       <div class="col-md-6">
                            <table border="0" class="text-black">
                                <tr>
                                    <td style="width: 150px; text-align: right; padding: 2px;">ISO&nbsp;:&nbsp;</td>
                                    <td style="width: 100px; text-align: left; padding: 2px;">
                                        <asp:DropDownList ID="ddlISO_FLAG" runat="server" Width="100px" CssClass="form-control">
                                            <asp:ListItem Value="" Text="All"></asp:ListItem>
                                            <asp:ListItem Value="N" Text="No"></asp:ListItem>
                                            <asp:ListItem Value="Y" Text="Yes"></asp:ListItem>
							            </asp:DropDownList>
                                    </td>
                                    <td style="width: 70px; text-align: right; padding: 2px;">H2S&nbsp;:&nbsp;</td>
                                    <td style="width: 120px; text-align: left; padding: 2px;">
                                        <asp:DropDownList ID="ddlH2S_FLAG" runat="server" Width="100px" CssClass="form-control">
                                            <asp:ListItem Value="" Text="All"></asp:ListItem>
                                            <asp:ListItem Value="N" Text="No"></asp:ListItem>
                                            <asp:ListItem Value="Y" Text="Yes"></asp:ListItem>
							            </asp:DropDownList>
                                    </td>
                                </tr>
                            </table>
                        </div>



                    </div>

                    <div class=" box-footer">

                        <table style="margin-left: 30%;">
                            <tr>
                                <td style="width: 90px">
                                    <input class="btn btn-block btn-sametogetherproject" type="button" name="btnSearch" value="Search" onclick="javascript: DoAction('SEARCH','');" style="width: 80px"  />
                                </td>
                                   <td style="width: 100px">
                                    <asp:Panel ID="pnlADD" runat="server">
                                        <input name="btnAdd" runat="server" class="btn btn-block btn-primary" type="button" id="btnAdd" value="Add" onclick="javascript: DoAction('ADD');" style="width: 80px" />
                                    </asp:Panel>                                    
                                </td>
                                <td style="width: 25px">
                                </td>
                            </tr>
                        </table>
                    </div>

                    
                </div>


                <div class="box">
                    <div class="box-body" style="overflow-x:auto;">

     <asp:GridView ID="gvResult" runat="server" HorizontalAlign="Left" AutoGenerateColumns="False" DataKeyNames="FID" GridLines="Both"  
          OnRowDataBound="gvResult_RowDataBound" 
                            AllowSorting="True" OnSorting="gvResult_Sorting">
                            <HeaderStyle CssClass="Table-head-violet" HorizontalAlign="Center" />
                            <FooterStyle CssClass="" HorizontalAlign="Center" />
                            <RowStyle CssClass="itemRow1" HorizontalAlign="Left" VerticalAlign="Middle" />
                            <AlternatingRowStyle CssClass="itemRow2" HorizontalAlign="Left" VerticalAlign="Middle" />
                            <Columns>
                                <asp:BoundField HeaderText="Region" DataField="REGION_NAME" SortExpression="REGION_NAME">
                                    <HeaderStyle CssClass="cell-center bg-green" Width="90px"/>
                                    <ItemStyle CssClass="cell-center cell-Middle cell-border" Width="90px"/>
                                </asp:BoundField>

                                <asp:BoundField HeaderText="OGC Main Point" DataField="FID" SortExpression="FID">
                                    <HeaderStyle CssClass="cell-center bg-green" Width="200px"/>
                                    <ItemStyle CssClass="cell-left cell-Middle cell-border" Width="200px"/>
                                </asp:BoundField>
 
                                <asp:BoundField HeaderText="Site description" DataField="SITE_NAME" SortExpression="SITE_NAME">
                                    <HeaderStyle CssClass="cell-center bg-green" Width="300px"/>
                                    <ItemStyle CssClass="cell-left cell-Middle cell-border" Width="300px"/>
                                </asp:BoundField>
                                <asp:BoundField HeaderText="ISO17025" DataField="ISO_FLAG_DESC" SortExpression="ISO_FLAG_DESC">
                                    <HeaderStyle CssClass="cell-center bg-green" Width="90px"/>
                                    <ItemStyle CssClass="cell-center cell-Middle cell-border" Width="90px"/>
                                </asp:BoundField>
                             <asp:BoundField HeaderText="H2S" DataField="H2S_FLAG_DESC" SortExpression="H2S_FLAG_DESC">
                                    <HeaderStyle CssClass="cell-center bg-green" Width="90px"/>
                                    <ItemStyle CssClass="cell-center cell-Middle cell-border" Width="90px"/>
                             </asp:BoundField>
                             <asp:BoundField HeaderText="Mail alert" DataField="ALERT_FLAG_DESC" SortExpression="ALERT_FLAG_DESC">
                                    <HeaderStyle CssClass="cell-center bg-green" Width="90px"/>
                                    <ItemStyle CssClass="cell-center cell-Middle cell-border" Width="90px"/>
                             </asp:BoundField>

                             <asp:BoundField HeaderText="Daily(20)" DataField="RPT_DAILY20" SortExpression="RPT_DAILY20">
                                    <HeaderStyle CssClass="cell-center bg-orange" Width="100px"/>
                                    <ItemStyle CssClass="cell-left cell-Middle cell-border" Width="100px"/>
                             </asp:BoundField>
                                <asp:BoundField HeaderText="20 Days" DataField="RPT_20DAY" SortExpression="RPT_20DAY">
                                    <HeaderStyle CssClass="cell-center bg-orange" Width="100px"/>
                                    <ItemStyle CssClass="cell-left cell-Middle cell-border" Width="100px"/>
                             </asp:BoundField>
                                <asp:BoundField HeaderText="Daily(27)" DataField="RPT_DAILY27" SortExpression="RPT_DAILY27">
                                    <HeaderStyle CssClass="cell-center bg-orange" Width="100px"/>
                                    <ItemStyle CssClass="cell-left cell-Middle cell-border" Width="100px"/>
                             </asp:BoundField>
                                <asp:BoundField HeaderText="27 Days" DataField="RPT_27DAY" SortExpression="RPT_27DAY">
                                    <HeaderStyle CssClass="cell-center bg-orange" Width="100px"/>
                                    <ItemStyle CssClass="cell-left cell-Middle cell-border" Width="100px"/>
                             </asp:BoundField>
                             <asp:BoundField HeaderText="Daily" DataField="RPT_DAILY" SortExpression="RPT_DAILY">
                                    <HeaderStyle CssClass="cell-center bg-orange" Width="100px"/>
                                    <ItemStyle CssClass="cell-left cell-Middle cell-border" Width="100px"/>
                             </asp:BoundField>
                             <asp:BoundField HeaderText="End month" DataField="RPT_ENDMTH" SortExpression="RPT_ENDMTH">
                                    <HeaderStyle CssClass="cell-center bg-orange" Width="100px"/>
                                    <ItemStyle CssClass="cell-left cell-Middle cell-border" Width="100px"/>
                             </asp:BoundField>

                             <asp:BoundField HeaderText="OMA Main Point" DataField="OMA_NAME1" SortExpression="OMA_NAME1">
                                    <HeaderStyle CssClass="cell-center" Width="100px"/>
                                    <ItemStyle CssClass="cell-left cell-Middle cell-border" Width="100px"/>
                             </asp:BoundField>
                             <asp:BoundField HeaderText="OGC Support Point 1" DataField="OGC_NAME1" SortExpression="OGC_NAME1">
                                    <HeaderStyle CssClass="cell-center" Width="100px"/>
                                    <ItemStyle CssClass="cell-left cell-Middle cell-border" Width="100px"/>
                             </asp:BoundField>
                             <asp:BoundField HeaderText="H2S Main Point" DataField="H2S_NAME1" SortExpression="H2S_NAME1">
                                    <HeaderStyle CssClass="cell-center" Width="100px"/>
                                    <ItemStyle CssClass="cell-left cell-Middle cell-border" Width="100px"/>
                             </asp:BoundField>
                             <asp:BoundField HeaderText="HG Main Point" DataField="HG_NAME1" SortExpression="HG_NAME1">
                                    <HeaderStyle CssClass="cell-center" Width="100px"/>
                                    <ItemStyle CssClass="cell-left cell-Middle cell-border" Width="100px"/>
                             </asp:BoundField>
                            <asp:BoundField HeaderText="O2 Main Point" DataField="O2_NAME1" SortExpression="O2_NAME1">
                                    <HeaderStyle CssClass="cell-center" Width="100px"/>
                                    <ItemStyle CssClass="cell-left cell-Middle cell-border" Width="100px"/>
                             </asp:BoundField>
                             <asp:BoundField HeaderText="HC Main Point" DataField="HC_NAME1" SortExpression="HC_NAME1">
                                    <HeaderStyle CssClass="cell-center" Width="100px"/>
                                    <ItemStyle CssClass="cell-left cell-Middle cell-border" Width="100px"/>
                             </asp:BoundField>


                            </Columns>
                        </asp:GridView>


                    </div>
                    <!-- /.box-body -->


<div class=" box-footer">

</div>


                </div>
                <!-- /.box -->
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
                <!--#include file="../Includes/JSfooter.html" -->
</asp:Content>
