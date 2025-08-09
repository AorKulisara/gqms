<%@ Page Title="" Language="C#" MasterPageFile="~/Includes/MainPage.Master" AutoEventWireup="true" CodeBehind="MngMapping.aspx.cs" Inherits="PTT.GQMS.USL.Web.Master.MngMapping" MaintainScrollPositionOnPostback="True" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
      <script type="text/javascript">
        function DoAction(act, val, val2, val3, val4) {
            var err, url1, url2, confirmed;
            err = ""; url1 = ""; url2 = ""; url3 = ""; confirmed = true;
            switch (act) {
                case "ADD": confirmed = true; err = CheckDataAdd(); break;
                case "SAVE": confirmed = true; err = CheckData(); break;
                case "DELETE": confirmed = ConfirmModal("Confirm to delete data?"); break;
            }

            if (err != "") {
                AlertModal(err);
            } else if (url1 != "") {
                window.location.href = url1;
            } else if (url2 != "") {
                window.open(url2, "", settings);
            } else if (url3 != "") {
                window.open(url3, "", settings2);
            } else if (confirmed && act != "") {
                SetCtrlValue("ServerAction", act);
                SetCtrlValue("ItemIndex", val);
                LoadSpin();
                SubmitForm();
            }
        }

        function CheckDataAdd() {
            var t_err = "";
            if ($("#txtLK_ITEM_ID").val() == "") { t_err += "* Item ID \r\n"; }
            if ($("#txtLK_VALUE").val() == "") { t_err += "* Display Name \r\n"; }

            if (t_err != "") { t_err = "Require data !!!\r\n" + t_err; }
            return t_err;
        }


        function CheckData() {
            var t_err = "";

            if ($("#txtLK_VALUEEdit").val() == "") { t_err += "* Display Name \r\n"; }

            if (t_err != "") { t_err = "Require data !!!\r\n" + t_err; }
            return t_err;
        }

        $(function () {
            var MyLength = [{ "selector": "[id*=txtLK_VALUE]", "length": "100" }]
            $.each(MyLength, function (key, val) {
                $(val["selector"]).attr("maxlength", val["length"]).keypress(function (e) {
                    if ($(this).val().length >= To_Num(val["length"])) {
                        e.preventDefault();
                    }
                }).blur(function (e) {
                    $(this).val($(this).val().substr(0, To_Num(val["length"])));
                });
            });
        });


    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HeadStyle" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">

    <input type="hidden" id="ServerAction" name="ServerAction" />
    <input type="hidden" name="ItemIndex" id="ItemIndex" />
    <!-- Content Header (Page header) -->
    <section class="content-header">

        <ol class="breadcrumb">
            <li><i class="fa fa-home"></i></li>
            <li>Master data</li>
            <li class="active">Tag mapping </li>
        </ol>
    </section>

    <!-- Main content -->
    <section class="content">
        <div class="row">
            <div class="col-xs-12">
                <div class="box box-violet">
                    <div class="box-header">
                        <table>
                            <tr>
                                <td>
                                    <asp:Label ID="lblTitle" class="box-title" runat="server" Text="Tag mapping : " Font-Size="16px"></asp:Label>&nbsp;
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddlLK_GRP_ID" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlLK_GRP_ID_SelectedIndexChanged" CssClass="form-control" Width="300px">
                                        <asp:ListItem Text="CHONBURI_DIM_FLOWRATE" Value="F"></asp:ListItem>
                                        <asp:ListItem Text="CHONBURI_DIM_MOISTURE" Value="M"></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <!-- /.box-header -->
                    <div class="box-body">

                        <asp:GridView ID="gvData" runat="server" HorizontalAlign="Left" AutoGenerateColumns="False" DataKeyNames="NAME" AllowPaging="False" GridLines="Both" OnRowDataBound="gvData_RowDataBound" Width="900px"  >
                            <HeaderStyle CssClass="Table-head-violet" HorizontalAlign="Center" />
                            <FooterStyle CssClass="" HorizontalAlign="Center" />
                            <RowStyle CssClass="itemRow1" HorizontalAlign="Left" VerticalAlign="Middle" />
                            <AlternatingRowStyle CssClass="itemRow2" HorizontalAlign="Left" VerticalAlign="Middle" />
                            <Columns>
                                 <asp:TemplateField HeaderText="Tag name">
                                    <HeaderStyle Width="20%" CssClass="cell-center" />
                                    <ItemStyle CssClass="cell-left cell-Middle cell-border" />
                                    <FooterStyle CssClass="cell-left cell-Middle cell-border" />
                                    <ItemTemplate>
                                        <%#DataBinder.Eval(Container.DataItem, "TAGNAME")%>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <%#DataBinder.Eval(Container.DataItem, "TAGNAME")%>
                                    </EditItemTemplate>
                                    <FooterTemplate>
                                        <input type='text' class='form-control' name='txtTAGNAME' id='txtTAGNAME' size='15' maxlength='16' />
                                    </FooterTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Name">
                                    <HeaderStyle Width="20%" CssClass="cell-center" />
                                    <ItemStyle CssClass="cell-left cell-Middle cell-border" />
                                    <FooterStyle CssClass="cell-left cell-Middle cell-border" />
                                    <ItemTemplate>
                                        <%#DataBinder.Eval(Container.DataItem, "NAME")%>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <input type="text" class='form-control' name='txtNAMEEdit' id='txtNAMEEdit' value='<%# DataBinder.Eval(Container.DataItem, "NAME") %>' size='15' maxlength='20' />
                                    </EditItemTemplate>
                                    <FooterTemplate>
                                        <input type='text' class='form-control' name='txtNAME' id='txtNAME' size='15' maxlength='20' />
                                    </FooterTemplate>
                                </asp:TemplateField>

                               

                                <asp:TemplateField HeaderText="Description">
                                    <HeaderStyle Width="40%" CssClass="cell-center" />
                                    <ItemStyle CssClass="cell-left cell-Middle cell-border" />
                                    <FooterStyle CssClass="cell-left cell-Middle cell-border" />
                                    <ItemTemplate>
                                        <%#DataBinder.Eval(Container.DataItem, "TAGDESC")%>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <input type="text" class='form-control' name='txtTAGDESCEdit' id='txtTAGDESCEdit' value='<%# DataBinder.Eval(Container.DataItem, "TAGDESC") %>' size='25' maxlength='60' />
                                    </EditItemTemplate>
                                    <FooterTemplate>
                                        <input type='text' class='form-control' name='txtTAGDESC' id='txtTAGDESC' size='25' maxlength='60' />
                                    </FooterTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Unit">
                                    <HeaderStyle Width="10%" CssClass="cell-center" />
                                    <ItemStyle CssClass="cell-left cell-Middle cell-border" />
                                    <FooterStyle CssClass="cell-left cell-Middle cell-border" />
                                    <ItemTemplate>
                                        <%#DataBinder.Eval(Container.DataItem, "UNIT")%>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <input type="text" class='form-control' name='txtUNITEdit' id='txtUNITEdit' value='<%# DataBinder.Eval(Container.DataItem, "UNIT") %>' size='10' maxlength='15' />
                                    </EditItemTemplate>
                                    <FooterTemplate>
                                        <input type='text' class='form-control' name='txtUNIT' id='txtUNIT' size='10' maxlength='15' />
                                    </FooterTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="" >
                                    <HeaderStyle  CssClass="cell-center" Width="10%" />
                                    <ItemStyle CssClass="cell-center cell-Middle cell-border" />
                                    <FooterStyle CssClass="cell-center cell-Middle cell-border" />
                                    <ItemTemplate>
                                        <li class="fa fa-pencil-square-o fa-2x" style="color: #666666;" onclick="DoAction('EDIT',<%# Container.DataItemIndex %>)"></li>
                                        &nbsp;<li class="fa fa-times fa-2x" style="color: #993333" modal-confirm onclick="DoAction('DELETE',<%# Container.DataItemIndex %>)"></li>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <li class="fa fa-save fa-2x" style="color: #666666;" modal-confirm onclick="DoAction('SAVE',<%# Container.DataItemIndex %>)"></li>
                                        &nbsp;<li class="fa fa-undo fa-2x" style="color: #993333" onclick="DoAction('CANCEL',0)"></li>
                                    </EditItemTemplate>
                                    <FooterTemplate>
                                        <a href="Javascript:DoAction('ADD',0)">
                                            <li class="fa fa-plus-square fa-2x" style="color: #009933;" modal-confirm></li>
                                        </a>
                                    </FooterTemplate>
                                </asp:TemplateField>
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
