<%@ Page Title="" Language="C#" MasterPageFile="~/Includes/MainPage.Master" AutoEventWireup="true" CodeBehind="TemplateListOFF.aspx.cs" Inherits="PTT.GQMS.USL.Web.Reports.TemplateListOFF" MaintainScrollPositionOnPostback="True" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
      var NextPage = "TemplateOFF?UID=<%= HttpContext.Current.Session["UID"] %>";
      function DoAction(act, val) {
          var confirmed, isSubmitted, url, err;
          confirmed = true; isSubmitted = true; url = ""; err = "";

          switch (act) {
              case "ADD": url = NextPage; break;
              case "SELECT": url = NextPage + "&K=" + val; break;
          }

          if (err != "") {
              AlertModal(err);
          } else if (url != "") {
              LoadSpin();
              window.location.href = url;
          } else if (confirmed && isSubmitted) {
              SetCtrlValue("ServerAction", act);
              LoadSpin();
              SubmitForm();
          }
      }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HeadStyle" runat="server">
    <style>
        #gvResult a {
            color: #0C0057 !important;
        }
    </style>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <input type="hidden" id="ServerAction" name="ServerAction" />
    <input type="hidden" name="ItemIndex" id="ItemIndex" />
    <!-- Content Header (Page header) -->
    <section class="content-header">
        <ol class="breadcrumb">
            <li><i class="fa fa-home"></i></li>
            <li>Reports </li>
            <li class="active">Offshore Site Template</li>
          </ol>
    </section>

    <!-- Main content -->
    <section class="content">

        <div class="row">
            <div class="col-xs-12">

                <div class='box box-success'>
                    <div class="box-header">
                        <h3 class="box-title"><i class="fa fa-search"></i>Search</h3>
                    </div>
                    <div class="box-body">

                        <!-- right column -->
                        <div class="col-xs-4">
                            <table border="0" class="text-black" style="border-collapse: separate; border-spacing: 0px">
                                <tr>
                                    <td style="width: 150px; text-align: right;">Template name&nbsp;:&nbsp;</td>
                                    <td style="width: 150px; text-align: left;">
                                        <asp:TextBox ID="txtSearch" CssClass="form-control" runat="server" MaxLength="50"></asp:TextBox></td>

                                    <td style="width: 100px; padding-left: 10px;"></td>

                                </tr>
                            </table>

                        </div>

                        <div class="col-xs-6">
                              <table class="center-table" >
                            <tr>
                                <td style="width: 100px">
                                    <input class="btn btn-block btn-sametogetherproject" type="button" name="btnSearch" value="Search" onclick="javascript: DoAction('SEARCH','');" style="width: 80px" />
                                </td>
                                   <td style="width: 100px">
                                       <asp:Panel ID="pnlADD" runat="server">
                                    <input name="btnAdd" runat="server" class="btn btn-block btn-primary" type="button" id="btnAdd" value="Add" onclick="javascript: DoAction('ADD','');" style="width: 80px" />
                                           </asp:Panel>
                                </td>
                              </tr>
                        </table>
                        </div>

                    </div>
 

                </div>

                <div class="box">
                    <div class="box-body">
                        <asp:GridView ID="gvResult" runat="server" HorizontalAlign="Left" AutoGenerateColumns="False" DataKeyNames="" GridLines="Both" PageSize="30" OnRowDataBound="gvResult_RowDataBound" Width="450px">
                            <HeaderStyle CssClass="Table-head-success" HorizontalAlign="Center" />
                            <FooterStyle CssClass="" HorizontalAlign="Center" />
                            <RowStyle CssClass="itemRow1" HorizontalAlign="Left" VerticalAlign="Top" />
                            <AlternatingRowStyle CssClass="itemRow2" HorizontalAlign="Left" VerticalAlign="Top" />
                            <PagerStyle CssClass="pagination-ys cell-borderW1" />
                            <Columns>
                                <asp:BoundField HeaderText="ID" DataField="TID" SortExpression="TID">
                                    <HeaderStyle Width="10%" CssClass="cell-center" />
                                    <ItemStyle CssClass="cell-left cell-top cell-border" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="Template name" DataField="T_NAME" SortExpression="T_NAME">
                                    <HeaderStyle Width="90%" CssClass="cell-center" />
                                    <ItemStyle CssClass="cell-left cell-top cell-border" />
                                </asp:BoundField>
                            </Columns>
                        </asp:GridView>

                    </div>
                    <!-- /.box-body -->

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
