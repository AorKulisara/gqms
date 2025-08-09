<%@ Page Title="" Language="C#" MasterPageFile="~/Includes/MainPage.Master" AutoEventWireup="true" CodeBehind="MngRoleDetail.aspx.cs" Inherits="PTT.GQMS.USL.Web.Settings.MngRoleDetail" ValidateRequest="False" MaintainScrollPositionOnPostback="True" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script   type="text/javascript">
        var LastPage = "MngRoleList?UID=<%= HttpContext.Current.Session["UID"] %>";
        function DoAction(act, val) {
            var confirmed, isSubmitted, url, err;
            confirmed = true; isSubmitted = true; url = ""; err = "";

            switch (act) {
                case "BACK": isSubmitted = false; url = LastPage; break;
                case "SAVE": isSubmitted = true; err = CheckData(); break;
                case "DELETE": confirmed = ConfirmModal("Confirm to delete data?"); break;
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
        function CheckData() {
            var t_err = "";


            if (GetCtrlValue("<%=txtName.ClientID %>") == "") { t_err += "* Role Name \r\n"; }
            if (GetCtrlValue("<%=txtDesc.ClientID %>") == "") { t_err += "* Description \r\n"; }


            if (t_err != "") { t_err = "Require data !!!\r\n" + t_err; }
            return t_err;
        }

</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HeadStyle" runat="server">
    <style>
        .center-table
{
  margin: 0 auto !important;
  float: none !important;
}
    #gvResult a {
  color: #0C0057 !important;
}


    </style>
      
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <input type="hidden" id="ServerAction" name="ServerAction"/>
    <input type="hidden" id="OP" name="OP" value="<%=OP %>"  />
    <input type="hidden" id="KEY" name="KEY" value="<%=Key %>"  />
        <!-- Content Header (Page header) -->
        <section class="content-header">

          <ol class="breadcrumb">
            <li><i class="fa fa-home"></i></li>
            <li>Security</li>
            <li onclick="javascript:GoMenu('MngRoleList');" style="cursor:pointer" >Role management</li>
            <li class="active">Role detail</li>
          </ol>
        </section>
        <!-- Main content -->
        <section class="content">
          <div class="row">

            <div class="col-xs-12">



            <div class='box box-primary-light'>
            <div class="box-header">
                <h3 class="box-title">Role Detail</h3>
            </div>
            <div class="box-body">

          <!-- right column -->
<div class="col-xs-12">
  <table border="0" class="text-black">
	<tr>
		<td style=" height:35px; width: 150px; text-align: right;">Role name&nbsp;<span class="RequireField">*</span>&nbsp;:&nbsp;</td>
		<td style=" width: 600px; text-align: left;"><asp:TextBox ID="txtName" MaxLength="200" runat="server" CssClass="form-control txtReadOnly" Width="200px" ReadOnly="True" ></asp:TextBox></td>
	</tr>
	<tr>
		<td style=" height:35px; text-align: right;" >Description&nbsp;<span class="RequireField">*</span>&nbsp;:&nbsp;</td>
		<td style=" text-align: left;" ><asp:TextBox ID="txtDesc" MaxLength="300" CssClass="form-control" runat="server" Width="400px"></asp:TextBox></td>
	</tr>
 </table>
       <br />
</div> 

                                <div class="col-xs-12">
              <asp:GridView ID="gvResult" runat="server" HorizontalAlign="Left" AutoGenerateColumns="False" CellPadding="4" DataKeyNames="TASK_ID" GridLines="Both" OnRowDataBound="gvResult_RowDataBound">
				                            <HeaderStyle CssClass="Table-head-primary-light" HorizontalAlign="Center" />
				                            <FooterStyle CssClass="" HorizontalAlign="Center" />
				                            <RowStyle CssClass="itemRow1" HorizontalAlign="Center" VerticalAlign="Top" />
				                            <AlternatingRowStyle CssClass="itemRow2" HorizontalAlign="Center" VerticalAlign="Top" />
				                            <PagerStyle Font-Bold="True" Font-Underline="False" HorizontalAlign="Right" CssClass="page" />
				                            <Columns>
					                            <asp:BoundField HeaderText="Screen" DataField="TASK_DESC">
						                            <HeaderStyle Width="500px" CssClass="cell-center" />
						                            <ItemStyle  CssClass=" cell-left cell-top cell-border" />
					                            </asp:BoundField>
					                            <asp:TemplateField HeaderText="Read">
					                                <HeaderStyle Width="100px" CssClass="cell-center" />
                                                    <ItemStyle  CssClass="cell-center cell-top cell-border" />
					                            </asp:TemplateField>
					                            <asp:TemplateField HeaderText="Add/Edit/Delete">
					                                <HeaderStyle Width="150px"  CssClass="cell-center" />
                                                    <ItemStyle  CssClass="cell-center cell-top cell-border" />
					                            </asp:TemplateField>
				                            </Columns>
				                        </asp:GridView>

                                     
                </div>


                                        <div class="col-xs-12">
                                            <asp:Label ID="lblLastUpdated" runat="server" CssClass="lblDisplay" ></asp:Label>

                      </div>
  
            </div>

                  <div class=" box-footer">

                      <div class="col-xs-12">
                           <table style=" margin-left:30%;" >
                                  <tr>
                                  <td style="width:100px">
                                      <asp:Panel ID="pnlSAVE" runat="server">
				                    <input name="btnSave" class="btn btn-block btn-sametogetherproject" type="button" id="btnSave" value="Save" onclick="javascript: DoAction('SAVE','');" style="width:80px" modal-confirm/>
				                    </asp:Panel>
                                  </td>
                                   <td style="width:100px">
                                       <asp:Panel ID="pnlDELETE" runat="server">
				                    <input name="btnDelete" class="btn btn-block btn-delete" type="button" id="btnDelete" value="Delete" onclick="javascript: DoAction('DELETE','');" style="width:80px" modal-confirm />
				                     </asp:Panel>
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
            <!--#include file="../Includes/JSfooter.html" -->
</asp:Content>
