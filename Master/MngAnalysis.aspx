<%@ Page Title="" Language="C#" MasterPageFile="~/Includes/MainPage.Master" AutoEventWireup="true" CodeBehind="MngAnalysis.aspx.cs" Inherits="PTT.GQMS.USL.Web.Master.MngAnalysis" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

        <script type="text/javascript">
            var NextPage1 = "MngAnalysisDetail?UID=<%= HttpContext.Current.Session["UID"] %>";

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
            <li class="active">Analysis method management </li>
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
                        <div class="col-md-7">
                            <table border="0" class="text-black">
                                <tr>
                                    <td style="width: 150px; text-align: right; padding: 2px;">Name&nbsp;:&nbsp;</td>
                                    <td style="text-align: left; padding: 2px;">
                                        <asp:TextBox ID="txtNAME" CssClass="form-control" runat="server" Width="300px" MaxLength="50" Text=""></asp:TextBox></td>
                                </tr>
                            </table>
                        </div>

                             <!-- right column -->
                       <div class="col-md-6">
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

     <asp:GridView ID="gvResult" runat="server" HorizontalAlign="Left" AutoGenerateColumns="False" DataKeyNames="ANLMET_ID" GridLines="Both"  
          OnRowDataBound="gvResult_RowDataBound" 
                            AllowSorting="True" OnSorting="gvResult_Sorting">
                            <HeaderStyle CssClass="Table-head-violet" HorizontalAlign="Center" />
                            <FooterStyle CssClass="" HorizontalAlign="Center" />
                            <RowStyle CssClass="itemRow1" HorizontalAlign="Left" VerticalAlign="Middle" />
                            <AlternatingRowStyle CssClass="itemRow2" HorizontalAlign="Left" VerticalAlign="Middle" />
                            <Columns>

                                <asp:BoundField HeaderText="Site description" DataField="ANLMET_NAME" SortExpression="ANLMET_NAME">
                                    <HeaderStyle CssClass="cell-center" Width="500px"/>
                                    <ItemStyle CssClass="cell-left cell-Middle cell-border" Width="500px"/>
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
