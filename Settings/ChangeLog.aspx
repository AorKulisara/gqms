<%@ Page Title="" Language="C#" MasterPageFile="~/Includes/MainPage.Master" AutoEventWireup="true" CodeBehind="ChangeLog.aspx.cs" Inherits="PTT.GQMS.USL.Web.Settings.ChangeLog" ValidateRequest="False" MaintainScrollPositionOnPostback="True" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">


    <script   type="text/JavaScript">
    function DoAction(type) {
        var confirmed, isSubmitted, url, err;
        confirmed = true; isSubmitted = true; url = ""; err = "";

        switch (type) {
   
        }

        if (err != "") {
            AlertModal(err);
        } else if (url != "") {
            window.location.href = url;
        } else if (confirmed && isSubmitted) {
            SetCtrlValue("ServerAction", type);
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
        
        <!-- Content Header (Page header) -->
        <section class="content-header">
          <ol class="breadcrumb">
            <li><i class="fa fa-home"></i></li>
            <li> Security</li>
            <li class="active">Gas Quality Data Changed Log</li>
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
  <table border="0" class="text-black" style="border-collapse: separate; border-spacing: 0px">
				                     <tr>
						                                     <td style="width: 100px; height: 27px;text-align: right; padding: 2px;">From date&nbsp;:&nbsp;</td>
                                                                <td style="width: 120px;" >

                                                                             <div id="datepicker1" class="input-group date" style="vertical-align: baseline">
                                                                                <asp:TextBox ID="txtDateFrom" runat="server" class="form-control timepicker" Width="100px" data-date-language="en-US" maxlength='10'  ></asp:TextBox>
                                                                                 <div class="input-group-addon">
                                                                                        <i id="i1" class="fa fa-calendar"></i>
                                                                                 </div>
                                                                             </div>   
                                                                            <script>
                                                                                $(function () {
                                                                                    $("input[id$=txtDateFrom]").datepicker({
                                                                                        format: 'dd/mm/yyyy'
                                                                                    }).on('changeDate', function (selected) {
                                                                                        if ($(this).val() == "") {
                                                                                            $('input[id$=txtDateFrom]').datepicker('setEndDate', null);
                                                                                            $('input[id$=txtDateTo]').datepicker('setStartDate', null);
                                                                                            $('input[id$=txtDateTo]').datepicker('setEndDate', null);
                                                                                            $('input[id$=txtDateTo]').val("");
                                                                                        } else {
                                                                                            var dateFrom = new Date(selected.date.valueOf());
                                                                                            $('input[id$=txtDateTo]').datepicker('setStartDate', dateFrom);
                                                                                        }
                                                                                    });
                                                                                    //.keydown(false);
                                                                                    
                                                                                });
                                                                                $('#datepicker1').datepicker({
                                                                                    autoclose: true
                                                                                })
	                                                                         </script>
                                                                                         
                                                                </td> 
                                                                <td style="width: 40px; height: 27px;text-align: right; padding: 2px;">To&nbsp;:&nbsp;</td>
                                                        <td  style="width: 120px;">
                                                        <div id="$('#datepicker2').datepicker({
                                                                autoclose: true
                                                            })" class="input-group date" style="vertical-align: baseline">
                                                            <asp:TextBox ID="txtDateTo" runat="server" class="form-control timepicker" Width="100px" data-date-language="en-US" maxlength='10'   ></asp:TextBox>
                                                            <div class="input-group-addon">
                                                                            <i id="i2" class="fa fa-calendar"></i>
                                                                        </div>
                                                        </div>
                                                        <script>
                                                            $(function () {
                                                                $('input[id$=txtDateTo]').datepicker({
                                                                    format: 'dd/mm/yyyy'
                                                                }).on('changeDate', function (selected) {
                                                                    if ($(this).val() == "") {
                                                                        $('input[id$=txtDateFrom]').datepicker('setEndDate', null);
                                                                    } else {
                                                                        var dateTo = new Date(selected.date.valueOf());
                                                                        $('input[id$=txtDateFrom]').datepicker('setEndDate', dateTo);
                                                                    }
                                                                });
                                                                //.keydown(false);
                                                            });
                                                            $('#datepicker2').datepicker({
                                                                autoclose: true
                                                            })
                                                        	</script>
                                                                            
                                                        </td>


					                                    </tr>					                                
				                                    </table>
    
</div> 

<div class="col-md-5">
  <table border="0" class="text-black" style="border-collapse: separate; border-spacing: 0px">
				                    					                                    <tr>
						                                    <td style="width: 100px; height: 27px;text-align: right; padding: 2px;" >User name&nbsp;:&nbsp;</td>
						                                    <td style="width: 220px;"><asp:TextBox id="txtCode" CssClass="form-control" runat="server" MaxLength="20"></asp:TextBox></td>
						                                   
						                                </tr>
						                                
				                                    </table>
    
</div> 
                
<div class="col-md-5">
  <table border="0" class="text-black" style="border-collapse: separate; border-spacing: 0px">
        <tr>
						                                    <td style="width: 100px; height: 27px;text-align: right; padding: 2px;">FID&nbsp;:&nbsp;</td>
						                                    <td  style="width: 220px;">
							                                    <asp:TextBox id="txtFID" CssClass="form-control" MaxLength="200" runat="server" ></asp:TextBox>
							                                </td>
						                                </tr>
						                               
				                                    </table>
    
</div> 

<div class="col-md-5">
  <table border="0" class="text-black" style="border-collapse: separate; border-spacing: 0px">
						                                <tr>
							                                <td style="width: 100px; height: 27px;text-align: right; padding: 2px;">Data&nbsp;:&nbsp;</td>
							                                <td   style="width: 220px;"><asp:TextBox id="txtAction" CssClass="form-control" MaxLength="200" runat="server" ></asp:TextBox></td>
					                                    </tr>
				                                    </table>
    
</div> 
<div class="col-md-5">
    &nbsp;
</div> 
            </div>
              <div class=" box-footer">

                               <input  class="btn btn-block btn-sametogetherproject" type="button" name="btnSearch" value="Search" onclick="javascript: DoAction('SEARCH');" style="width:80px;margin-left:40%;" modal-confirm />
                </div>

           </div>

           <!-- /.box -->

              <div class="box">
                <div class="box-body">
                      <asp:GridView ID="gvResult" runat="server" HorizontalAlign="Left" AutoGenerateColumns="False" DataKeyNames="" AllowPaging="True" GridLines="Both" PageSize="30" OnPageIndexChanging="gvResult_PageIndexChanging" OnRowDataBound="gvResult_RowDataBound" Width="100%">
					                    <HeaderStyle CssClass="Table-head-primary-light" HorizontalAlign="Center" />
				                        <FooterStyle CssClass="" HorizontalAlign="Center" />
				                        <RowStyle CssClass="itemRow1" HorizontalAlign="Left" VerticalAlign="Top" />
				                        <AlternatingRowStyle CssClass="itemRow2" HorizontalAlign="Left" VerticalAlign="Top" />
                                        <PagerStyle CssClass="pagination-ys cell-borderW1" />

					                    <Columns>

						                    <asp:BoundField DataField="TRANS_DATE" HeaderText="Changed Date" DataFormatString="{0:dd/MM/yyyy HH:mm}" HtmlEncode="false">
							                    <HeaderStyle Width="15%" CssClass="cell-center"></HeaderStyle>
							                    <ItemStyle CssClass="cell-center cell-top cell-border"></ItemStyle>
						                    </asp:BoundField>
                                            <asp:BoundField DataField="USER_NAME" HeaderText="User name">
							                    <HeaderStyle Width="12%" CssClass="cell-center"></HeaderStyle>
							                    <ItemStyle CssClass="cell-center cell-top cell-border"></ItemStyle>
						                    </asp:BoundField>
                                            <asp:BoundField DataField="FID" HeaderText="FID / Customer">
							                    <HeaderStyle Width="15%" CssClass="cell-center"></HeaderStyle>
							                    <ItemStyle CssClass="cell-center cell-top cell-border"></ItemStyle>
						                    </asp:BoundField>
						                <asp:BoundField DataField="RDATE" HeaderText="Data Date" DataFormatString="{0:dd/MM/yyyy}" HtmlEncode="false">
							                    <HeaderStyle Width="10%" CssClass="cell-center"></HeaderStyle>
							                    <ItemStyle CssClass="cell-center cell-top cell-border"></ItemStyle>
						                    </asp:BoundField>
                                            <asp:BoundField DataField="FIELD_NAME" HeaderText="Data">
                                            <HeaderStyle Width="18%" CssClass="cell-center"></HeaderStyle>
							                    <ItemStyle CssClass="cell-left cell-top cell-border"></ItemStyle>
						                    </asp:BoundField>
                                            <asp:BoundField DataField="OLD_DATA" HeaderText="Old">
                                            <HeaderStyle Width="15%" CssClass="cell-center"></HeaderStyle>
							                    <ItemStyle CssClass="cell-left cell-top cell-border"></ItemStyle>
						                    </asp:BoundField>
                                            <asp:BoundField DataField="NEW_DATA" HeaderText="New">
                                            <HeaderStyle Width="15%" CssClass="cell-center"></HeaderStyle>
							                    <ItemStyle CssClass="cell-left cell-top cell-border"></ItemStyle>
						                    </asp:BoundField>
					                    </Columns>
				                    </asp:GridView>

                </div>
        </div>

           <!-- /.box -->
            </div><!-- /.col -->
          </div><!-- /.row -->
 
        </section><!-- /.content -->

</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="FootScript" runat="server">


            <!--#include file="../Includes/JSfooter.html" -->
</asp:Content>
