<%@ Page Title="" Language="C#" MasterPageFile="~/Includes/MainPage.Master" AutoEventWireup="true" CodeBehind="MngRegion.aspx.cs" Inherits="PTT.GQMS.USL.Web.Master.MngRegion" %>
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
            if ($("#txtREGION_ID").val() == "") { t_err += "* Region ID \r\n"; }
            if ($("#txtREGION_NAME").val() == "") { t_err += "* Region Name \r\n"; }
            if ($("#txtREGION_ADDR").val() == "") { t_err += "* Location \r\n"; }

            if (t_err != "") { t_err = "Require data !!!\r\n" + t_err; }
            return t_err;
        }


        function CheckData() {
            var t_err = "";

            if ($("#txtREGION_NAMEEdit").val() == "") { t_err += "* Region Name \r\n"; }
            if ($("#txtREGION_ADDREdit").val() == "") { t_err += "* Location \r\n"; }

            if (t_err != "") { t_err = "Require data !!!\r\n" + t_err; }
            return t_err;
        }


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
            <li class="active">Region management</li>
        </ol>
    </section>

    <!-- Main content -->
    <section class="content">
        <div class="row">
            <div class="col-xs-12">
                <div class="box box-violet">
 
                    <!-- /.box-header -->
                    <div class="box-body">

                        <asp:GridView ID="gvData" runat="server" HorizontalAlign="Left" AutoGenerateColumns="False" DataKeyNames="REGION_ID" AllowPaging="False" GridLines="Both" OnRowDataBound="gvData_RowDataBound" Width="1300px"  >
                            <HeaderStyle CssClass="Table-head-violet" HorizontalAlign="Center" />
                            <FooterStyle CssClass="" HorizontalAlign="Center" />
                            <RowStyle CssClass="itemRow1" HorizontalAlign="Left" VerticalAlign="Middle" />
                            <AlternatingRowStyle CssClass="itemRow2" HorizontalAlign="Left" VerticalAlign="Middle" />
                            <Columns>
                                 <asp:TemplateField HeaderText="Region ID">
                                    <HeaderStyle Width="10%" CssClass="cell-center" />
                                    <ItemStyle CssClass="cell-left cell-Middle cell-border" />
                                    <FooterStyle CssClass="cell-left cell-Middle cell-border" />
                                    <ItemTemplate>
                                        <%#DataBinder.Eval(Container.DataItem, "REGION_ID")%>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <%#DataBinder.Eval(Container.DataItem, "REGION_ID")%>
                                    </EditItemTemplate>
                                    <FooterTemplate>
                                        <input type='text' class='form-control' name='txtREGION_ID' id='txtREGION_ID' size='10' maxlength='10' />
                                    </FooterTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Region Name">
                                    <HeaderStyle Width="10%" CssClass="cell-center" />
                                    <ItemStyle CssClass="cell-left cell-Middle cell-border" />
                                    <FooterStyle CssClass="cell-left cell-Middle cell-border" />
                                    <ItemTemplate>
                                        <%#DataBinder.Eval(Container.DataItem, "REGION_NAME")%>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <input type="text" class='form-control' name='txtREGION_NAMEEdit' id='txtREGION_NAMEEdit' value='<%# DataBinder.Eval(Container.DataItem, "REGION_NAME") %>' size='50' maxlength='100' />
                                    </EditItemTemplate>
                                    <FooterTemplate>
                                        <input type='text' class='form-control' name='txtREGION_NAME' id='txtREGION_NAME' size='50' maxlength='100' />
                                    </FooterTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Region Name">
                                    <HeaderStyle Width="25%" CssClass="cell-center" />
                                    <ItemStyle CssClass="cell-left cell-Middle cell-border" />
                                    <FooterStyle CssClass="cell-left cell-Middle cell-border" />
                                    <ItemTemplate>
                                        <%#DataBinder.Eval(Container.DataItem, "REGION_FULL")%>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <input type="text" class='form-control' name='txtREGION_FULLEdit' id='txtREGION_FULLEdit' value='<%# DataBinder.Eval(Container.DataItem, "REGION_FULL") %>' size='50' maxlength='500' />
                                    </EditItemTemplate>
                                    <FooterTemplate>
                                        <input type='text' class='form-control' name='txtREGION_FULL' id='txtREGION_FULL' size='50' maxlength='500' />
                                    </FooterTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Location">
                                    <HeaderStyle Width="50%" CssClass="cell-center" />
                                    <ItemStyle CssClass="cell-left cell-Middle cell-border" />
                                    <FooterStyle CssClass="cell-left cell-Middle cell-border" />
                                    <ItemTemplate>
                                        <%#DataBinder.Eval(Container.DataItem, "REGION_ADDR")%>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <input type="text" class='form-control' name='txtREGION_ADDREdit' id='txtREGION_ADDREdit' value='<%# DataBinder.Eval(Container.DataItem, "REGION_ADDR") %>' size='100' maxlength='2000' />
                                    </EditItemTemplate>
                                    <FooterTemplate>
                                        <input type='text' class='form-control' name='txtREGION_ADDR' id='txtREGION_ADDR' size='100' maxlength='2000' />
                                    </FooterTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="" >
                                    <HeaderStyle  CssClass="cell-center" Width="5%" />
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
