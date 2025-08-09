<%@ Page Title="" Language="C#" MasterPageFile="~/Includes/MainPage.Master" AutoEventWireup="true" CodeBehind="MngAnalysisDetail.aspx.cs" Inherits="PTT.GQMS.USL.Web.Master.MngAnalysisDetail" ValidateRequest="false" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
 
        <script type="text/javascript">
        var LastPage = "MngAnalysis?UID=<%= HttpContext.Current.Session["UID"] %>";

        function DoAction(act, val) {
            var confirmed, isSubmitted, url, err;
            confirmed = true; isSubmitted = true; url = ""; err = "";

            switch (act) {
                case "BACK": isSubmitted = false; url = LastPage; break;
                case "SAVE": isSubmitted = true; err = CheckData(); break;
                case "DELETE": confirmed = ConfirmModal("Confirm to delete data?"); break;
                case "EDIT_ITEM": break;
                case "ADD_ITEM": confirmed = true; err = CheckItemDataAdd(); break;
                case "SAVE_ITEM": confirmed = true; err = CheckItemData(); break;
                case "DELETE_ITEM": confirmed = ConfirmModal("Confirm to delete data?"); break;
                
            }

            if (err != "") {
                AlertModal(err);
            } else if (url != "") {
                LoadSpin();
                window.location.href = url;
            } else if (confirmed && isSubmitted) {
                SetCtrlValue("ServerAction", act);
                SetCtrlValue("ItemIndex", val);
                LoadSpin();
                SubmitForm();
            }
        }


        function CheckData() {
            var t_err = "";
 
            if (GetCtrlValue("<%=txtANLMET_NAME.ClientID %>") == "") { t_err += "* Analysis Method \r\n"; }

            if (t_err != "") { t_err = "Require data !!!\r\n" + t_err; }
            return t_err;
        }

        function CheckItemDataAdd() {
            var t_err = "";
            if ($("#txtSEQ_NO").val() == "") { t_err += "* Sequence \r\n"; }
            if ($("#txtSTD_HEAD").val() == "") { t_err += "* Name \r\n"; }
            if ($("#txtSTD_REF").val() == "") { t_err += "* Standard Reference \r\n"; }

            if (t_err != "") { t_err = "Require data !!!\r\n" + t_err; }
            return t_err;
        }


        function CheckItemData() {
            var t_err = "";
            if ($("#txtSEQ_NOEdit").val() == "") { t_err += "* Sequence \r\n"; }
            if ($("#txtSTD_HEADEdit").val() == "") { t_err += "* Name \r\n"; }
            if ($("#txtSTD_REFEdit").val() == "") { t_err += "* Standard Reference \r\n"; }

            if (t_err != "") { t_err = "Require data !!!\r\n" + t_err; }
            return t_err;
        }


        //-------------------------------------------------------------------------

    </script>


</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HeadStyle" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <input type="hidden" id="ServerAction" name="ServerAction" />
     <input type="hidden" name="ItemIndex" id="ItemIndex" />
    <asp:HiddenField ID="hidANLMET_ID" runat="server" />


        <!-- Content Header (Page header) -->
    <section class="content-header">
         <ol class="breadcrumb">
            <li><i class="fa fa-home"></i></li>
            <li>Master data</li>
            <li onclick="javascript:GoMenu('MngAnalysis');" style="cursor:pointer" >Analysis method management</li>
            <li class="active">Analysis method detail</li>
        </ol>
    </section>

    <!-- Main content -->
    <section class="content">
        <div class="row">

            <div class="col-xs-12">

                <div class='box box-violet box-violet'>
                    <div class="box-header">
                        <h3 class="box-title">Analysis method detail</h3>
                    </div>
                    <div class="box-body ">

                     <div class="col-md-8">
                        <table border="0" class="text-black">
                            <tr>
                                    <td style="text-align: right; height: 30px; width: 150px;">Analysis method<span class="RequireField">*</span>:&nbsp;&nbsp;</td>
                                    <td style="text-align: left; height: 30px;" >
                                        <asp:TextBox ID="txtANLMET_NAME" MaxLength="500" CssClass="form-control" runat="server" Width="400px" Text=""></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                    </div>

                  <div class="col-md-12">
                        <table border="0" class="text-black">
                             <tr>
                                    <td style="height: 10px;"></td>
                                </tr>
                        </table>
                    </div>

<!--------------------------------------------------------------------------------------------->

 <div class="col-xs-12 ">
     <asp:Label ID="lblLastUpdated" runat="server" CssClass="lblDisplay" Text=""></asp:Label>
</div>

<!--------------------------------------------------------------------------------------------->
<div class="col-xs-12 ">
       <table border="0" style="margin-left: 35%;">
                <tr>
                    <td style="width: 100px">
                        <asp:Panel ID="pnlSAVE" runat="server">
                            <input name="btnSave" class="btn btn-block btn-sametogetherproject" type="button" id="btnSave" value="Save" onclick="javascript: DoAction('SAVE','');" style="width: 80px" modal-confirm />
                        </asp:Panel>
                    </td>
                        <td style="width: 100px">
                        <asp:Panel ID="pnlDELETE" runat="server">
                        <input name="btnDelete" class="btn btn-block btn-delete" type="button" id="btnDelete" value="Delete" onclick="javascript: DoAction('DELETE','');" style="width: 80px" modal-confirm />
                        </asp:Panel>
                    </td>
                        <td style="width: 100px;">
                        <button type="button" class="pull-left btn btn-block btn-sametogetherproject" id="btnBack" onclick="javascript: DoAction('BACK','');" style="width: 80px">
                            <i class="fa fa-chevron-left"></i>Back</button>
                    </td>

                </tr>
            </table>
</div>
<!--------------------------------------------------------------------------------------------->
</div>
</div>
<!---------------------------------------------------------->
<!---------------------------------------------------------->

        <div class="box-footer">
         <asp:GridView ID="gvData" runat="server" HorizontalAlign="Left" AutoGenerateColumns="False" DataKeyNames="SEQ_NO" AllowPaging="False" GridLines="Both" OnRowDataBound="gvData_RowDataBound" Width="1300px"  >
                            <HeaderStyle CssClass="Table-head-violet" HorizontalAlign="Center" />
                            <FooterStyle CssClass="" HorizontalAlign="Center" />
                            <RowStyle CssClass="itemRow1" HorizontalAlign="Left" VerticalAlign="Middle" />
                            <AlternatingRowStyle CssClass="itemRow2" HorizontalAlign="Left" VerticalAlign="Middle" />
                            <Columns>
                                 <asp:TemplateField HeaderText="Sequence">
                                    <HeaderStyle Width="7%" CssClass="cell-center" />
                                    <ItemStyle CssClass="cell-center cell-Middle cell-border" />
                                    <FooterStyle CssClass="cell-center cell-Middle cell-border" />
                                    <ItemTemplate>
                                        <%#DataBinder.Eval(Container.DataItem, "SEQ_NO")%>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <input type="text" class='form-control' name='txtSEQ_NOEdit' id='txtSEQ_NOEdit' value='<%# DataBinder.Eval(Container.DataItem, "SEQ_NO") %>' size='10' maxlength='2' />
                                    </EditItemTemplate>
                                    <FooterTemplate>
                                        <input type='text' class='form-control' name='txtSEQ_NO' id='txtSEQ_NO' size='10' maxlength='2' onkeypress="return isNumber(event)" />
                                    </FooterTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Name">
                                    <HeaderStyle Width="35%" CssClass="cell-center" />
                                    <ItemStyle CssClass="cell-left cell-Middle cell-border" />
                                    <FooterStyle CssClass="cell-left cell-Middle cell-border" />
                                    <ItemTemplate>
                                        <%#DataBinder.Eval(Container.DataItem, "STD_HEAD")%>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <input type="text" class='form-control' name='txtSTD_HEADEdit' id='txtSTD_HEADEdit' value='<%# DataBinder.Eval(Container.DataItem, "STD_HEAD") %>' size='50' maxlength='1000' />
                                    </EditItemTemplate>
                                    <FooterTemplate>
                                        <input type='text' class='form-control' name='txtSTD_HEAD' id='txtSTD_HEAD' size='50' maxlength='1000' />
                                    </FooterTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Standard reference">
                                    <HeaderStyle Width="48%" CssClass="cell-center" />
                                    <ItemStyle CssClass="cell-left cell-Middle cell-border" />
                                    <FooterStyle CssClass="cell-left cell-Middle cell-border" />
                                    <ItemTemplate>
                                        <%#DataBinder.Eval(Container.DataItem, "STD_REF")%>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <input type="text" class='form-control' name='txtSTD_REFEdit' id='txtSTD_REFEdit' value='<%# DataBinder.Eval(Container.DataItem, "STD_REF") %>' size='50' maxlength='1000' />
                                    </EditItemTemplate>
                                    <FooterTemplate>
                                        <input type='text' class='form-control' name='txtSTD_REF' id='txtSTD_REF' size='50' maxlength='1000' />
                                    </FooterTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="" >
                                    <HeaderStyle  CssClass="cell-center" Width="5%" />
                                    <ItemStyle CssClass="cell-center cell-Middle cell-border" />
                                    <FooterStyle CssClass="cell-center cell-Middle cell-border" />
                                    <ItemTemplate>
                                        <li class="fa fa-pencil-square-o fa-2x" style="color: #666666;" onclick="DoAction('EDIT_ITEM',<%# Container.DataItemIndex %>)"></li>
                                        &nbsp;<li class="fa fa-times fa-2x" style="color: #993333" modal-confirm onclick="DoAction('DELETE_ITEM',<%# Container.DataItemIndex %>)"></li>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <li class="fa fa-save fa-2x" style="color: #666666;" modal-confirm onclick="DoAction('SAVE_ITEM',<%# Container.DataItemIndex %>)"></li>
                                        &nbsp;<li class="fa fa-undo fa-2x" style="color: #993333" onclick="DoAction('CANCEL_ITEM',0)"></li>
                                    </EditItemTemplate>
                                    <FooterTemplate>
                                        <a href="Javascript:DoAction('ADD_ITEM',0)">
                                            <li class="fa fa-plus-square fa-2x" style="color: #009933;" modal-confirm></li>
                                        </a>
                                    </FooterTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
        </div>


                </div>

                <!-- /.box -->

        </div>
        <!-- /.row -->


        <!--======================================================================================================  -->

    </section>
    <!-- /.content -->



</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="FootScript" runat="server">
 
<!--#include file="../Includes/JSfooter.html" -->
</asp:Content>
