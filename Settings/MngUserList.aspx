<%@ Page Title="" Language="C#" MasterPageFile="~/Includes/MainPage.Master" AutoEventWireup="true" CodeBehind="MngUserList.aspx.cs" Inherits="PTT.GQMS.USL.Web.Settings.MngUserList" ValidateRequest="False" MaintainScrollPositionOnPostback="True" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        var NextPage = "MngUserDetail?UID=<%= HttpContext.Current.Session["UID"] %>";
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
    <input type="hidden" name="ItemIndex" />

    <!-- Content Header (Page header) -->
    <section class="content-header">
        <ol class="breadcrumb">
            <li><i class="fa fa-home"></i></li>
            <li>Security</li>
            <li class="active">User management</li>
        </ol>
    </section>
    <!-- Main content -->
    <section class="content">
        <div class="row">

            <div class="col-xs-12">



                <div class='box box-primary-light'>
                    <div class="box-header">
                        <h3 class="box-title"><i class="fa fa-search"></i> Search</h3>
                    </div>
                    <div class="box-body">

                        <!-- right column -->
                        <div class="col-md-5">
                            <table border="0" class="text-black">
                                <tr>
                                    <td style="width: 100px; text-align: right; padding: 2px;">Role name&nbsp;:&nbsp;</td>
                                    <td style="width: 150px; text-align: left; padding: 2px;">
                                        <asp:DropDownList ID="cboRole" Width="200px" CssClass="form-control" runat="server"></asp:DropDownList></td>

                                </tr>
                            </table>

                        </div>
                        <div class="col-md-5">
                            <table border="0" class="text-black">
                                <tr>
                                    <td style="width: 100px; text-align: right; padding: 2px;">User name&nbsp;:&nbsp;</td>
                                    <td style="width: 150px; text-align: left; padding: 2px;">
                                        <asp:TextBox ID="txtUserName" CssClass="form-control" runat="server" MaxLength="50"></asp:TextBox></td>
                                </tr>
                            </table>

                        </div>

                    </div>

                    <div class=" box-footer">

                        <table style="margin-left: 30%;">
                            <tr>
                                <td style="width: 100px">
                                    <input class="btn btn-block btn-sametogetherproject" type="button" name="btnSearch" value="Search" onclick="javascript: DoAction('SEARCH');" style="width: 80px" modal-confirm />
                                </td>
                                <td style="width: 100px">
                                    <asp:Panel ID="pnlADD" runat="server">
                                    <input name="btnAdd" runat="server" class="btn btn-block btn-primary" type="button" id="btnAdd" value="Add" onclick="javascript: DoAction('ADD');" style="width: 80px" />
                                    </asp:Panel>
                                </td>
                            </tr>
                        </table>
                    </div>


                </div>


                <div class="box">
                    <div class="box-body">
                  <asp:GridView ID="gvResult" runat="server" HorizontalAlign="Left" AutoGenerateColumns="False" DataKeyNames="" AllowPaging="True" GridLines="Both" PageSize="20" OnPageIndexChanging="gvResult_PageIndexChanging" OnRowDataBound="gvResult_RowDataBound"
                   Width="100%"  >
			                            <HeaderStyle CssClass="Table-head-primary-light" HorizontalAlign="Center" />
				                        <FooterStyle CssClass="" HorizontalAlign="Center" />
				                        <RowStyle CssClass="itemRow1" HorizontalAlign="Left" VerticalAlign="Top" />
				                        <AlternatingRowStyle CssClass="itemRow2" HorizontalAlign="Left" VerticalAlign="Top" />
                                        <PagerSettings Mode="NumericFirstLast" PageButtonCount="10" />
                                        <PagerStyle CssClass="pagination-ys cell-borderW1" />
			                            <Columns>
				                            <asp:BoundField HeaderText="Role name" DataField="ROLE_NAME" SortExpression="ROLE_NAME">
					                            <HeaderStyle Width="25%" CssClass="Table-head-primary-light cell-center" />
					                            <ItemStyle CssClass="cell-center cell-top cell-border" />
				                            </asp:BoundField>
				                            <asp:BoundField HeaderText="User name" DataField="USER_NAME" SortExpression="USER_NAME">
					                            <HeaderStyle Width="25%" CssClass="Table-head-primary-light cell-center" />
					                            <ItemStyle CssClass="cell-center cell-top cell-border" />
				                            </asp:BoundField>
				                            <asp:BoundField HeaderText="Description" DataField="USER_DESC" SortExpression="USER_DESC">
					                            <HeaderStyle Width="40%" CssClass="Table-head-primary-light cell-center" />
					                            <ItemStyle CssClass="cell-left cell-top cell-border" />
				                            </asp:BoundField>
                                            <asp:BoundField HeaderText="Status" DataField="USER_STATUS" SortExpression="USER_STATUS">
					                            <HeaderStyle Width="10%" CssClass="Table-head-primary-light cell-center" />
					                            <ItemStyle CssClass="cell-center cell-top cell-border" />
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
