<%@ Page Title="" Language="C#" MasterPageFile="~/Includes/MainPage.Master" AutoEventWireup="true" CodeBehind="TemplateSPOT.aspx.cs" Inherits="PTT.GQMS.USL.Web.Reports.TemplateSPOT" MaintainScrollPositionOnPostback="True" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

    <!-- multiple Select2 -->
    <link href="../Content/select2.min.css" rel="stylesheet" />
    <script src="../Scripts/select2.full.min.js"></script>   

    <script   type="text/javascript">
        var LastPage = "TemplateListSPOT?UID=<%= HttpContext.Current.Session["UID"] %>";
        function DoAction(act, val) {
            var confirmed, isSubmitted, url, err;
            confirmed = true; isSubmitted = true; url = ""; err = "";

            switch (act) {
                case "BACK": isSubmitted = false; url = LastPage; break;
                case "SAVE": isSubmitted = true; err = CheckData(); break;
                case "DELETE": confirmed = ConfirmModal("Confirm to delete data?"); break;
                case "ADD_ITEM": isSubmitted = true; err = CheckDataAddItem(); break;
                case "EDIT_ITEM": isSubmitted = true; break;
                case "SAVE_ITEM": isSubmitted = true; err = CheckDataItem(); break;
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

            if (GetCtrlValue("<%=txtName.ClientID %>") == "") { t_err += "* Template name \r\n"; }

            if (t_err != "") { t_err = "Require data !!!\r\n" + t_err; }
            return t_err;
        }


        function CheckDataAddItem() {
            var t_err = "";
            if ($("#txtSEQ").val() == "") { t_err += "* Seq \r\n"; }
            if ($("#ddlFID").val() == "") { t_err += "* FID \r\n"; }

            if (t_err != "") { t_err = "Require data !!!\r\n" + t_err; }
            return t_err;
        }

        function CheckDataItem() {
            var t_err = "";
            if ($("#txtSEQEdit").val() == "") { t_err += "* Seq \r\n"; }

            if (t_err != "") { t_err = "Require data !!!\r\n" + t_err; }
            return t_err;
        }

</script>


</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HeadStyle" runat="server">
      
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <input type="hidden" id="ServerAction" name="ServerAction"/>
    <input type="hidden" name="ItemIndex" id="ItemIndex" />
    <asp:HiddenField ID="hidTID" runat="server" />

        <!-- Content Header (Page header) -->
        <section class="content-header">

          <ol class="breadcrumb">
            <li><i class="fa fa-home"></i></li>
            <li>Reports</li>
            <li onclick="javascript:GoMenu('TemplateListSPOT');" style="cursor:pointer" >Site template</li>
            <li class="active">Sampling point template detail</li>
          </ol>
        </section>
        <!-- Main content -->
        <section class="content">
          <div class="row">

            <div class="col-xs-12">



            <div class='box box-success'>
            <div class="box-header">
                <h3 class="box-title">Sampling point template detail</h3>
            </div>
            <div class="box-body">

          <!-- right column -->
<div class="col-xs-12">
  <table border="0" class="text-black">
	<tr>
		<td style=" height:35px; width: 150px; text-align: right;">Template name&nbsp;<span class="RequireField">*</span>&nbsp;:&nbsp;</td>
		<td style=" width: 600px; text-align: left;"><asp:TextBox ID="txtName" MaxLength="200" runat="server" CssClass="form-control " Width="500px" ReadOnly="True" Text="H2S sampling point" ></asp:TextBox></td>
	</tr>
 </table>
       <br />
</div> 

                <div class="col-xs-12">
              
               <asp:GridView ID="gvData" runat="server" HorizontalAlign="Left" AutoGenerateColumns="False" DataKeyNames="SEQ" AllowPaging="False" GridLines="Both" OnRowDataBound="gvData_RowDataBound" Width="600"  >
                            <HeaderStyle CssClass="Table-head-success" HorizontalAlign="Center" />
                            <FooterStyle CssClass="" HorizontalAlign="Center" />
                            <RowStyle CssClass="itemRow1" HorizontalAlign="Left" VerticalAlign="Middle" />
                            <AlternatingRowStyle CssClass="itemRow2" HorizontalAlign="Left" VerticalAlign="Middle" />
                            <Columns>
                                <asp:TemplateField HeaderText="Seq">
                                    <HeaderStyle Width="15%" CssClass="cell-center" />
                                    <ItemStyle CssClass="cell-center cell-Middle cell-border" />
                                    <FooterStyle CssClass="cell-center cell-Middle cell-border" />
                                    <ItemTemplate>
                                        <%#DataBinder.Eval(Container.DataItem, "SEQ")%>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <input type='text' class='form-control' name='txtSEQEdit' id='txtSEQEdit' value='<%# DataBinder.Eval(Container.DataItem, "SEQ") %>' maxlength='3' onkeypress='return isNumber(event)' />
                                    </EditItemTemplate>
                                    <FooterTemplate>
                                        
                                    </FooterTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Sampling point">
                                    <HeaderStyle Width="60%" CssClass="cell-center" />
                                    <ItemStyle CssClass="cell-left cell-Middle cell-border" />
                                    <FooterStyle CssClass="cell-left cell-Middle cell-border" />
                                    <ItemTemplate>
                                        <%#DataBinder.Eval(Container.DataItem, "FID")%>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <%#DataBinder.Eval(Container.DataItem, "FID")%>
                                    </EditItemTemplate>
                                    <FooterTemplate>
                                        
                                    </FooterTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="" >
                                    <HeaderStyle  CssClass="cell-center" Width="10%" />
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

                  <div class=" box-footer">

                      <div class="col-xs-12">
                           <table style=" margin-left:30%;" >
                                  <tr>
                                  <td style="width:100px">
				                    <input name="btnSave" class="btn btn-block btn-sametogetherproject" type="button" id="btnSave" value="Save" onclick="javascript: DoAction('SAVE','');" style="width:80px" modal-confirm/>
                                  </td>
                                   <td style="width:100px">
				                    <input name="btnDelete" class="btn btn-block btn-delete" type="button" id="btnDelete" value="Delete" onclick="javascript: DoAction('DELETE','');" style="width:80px" modal-confirm />
                                  </td>
                                  <td style="width:130px;">
                                    <button type="button" class="pull-left btn btn-block btn-sametogetherproject" id="btnBack" onclick="javascript: DoAction('BACK','');" style="width:80px">
                                    <i class="fa fa-chevron-left"></i>  Back</button>
                                  </td>
                                  </tr>
                                </table>
                      </div>
                </div>

           </div>

            </div><!-- /.col -->
          </div><!-- /.row -->
 
        </section><!-- /.content -->


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
