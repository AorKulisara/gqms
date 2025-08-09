<%@ Page Title="" Language="C#" MasterPageFile="~/Includes/MainPage.Master" AutoEventWireup="true" CodeBehind="UploadO2.aspx.cs" Inherits="PTT.GQMS.USL.Web.Forms.UploadO2" MaintainScrollPositionOnPostback="True" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

<script type="text/javascript">
    function DoAction(act, val) {
        var confirmed, isSubmitted, url, err;
        confirmed = true; isSubmitted = true; url = ""; err = "";
        switch (act) {
            case "IMPORT_XLS": isSubmitted = true; break;
            case "SEARCH": break;
        }

        if (err != "") {
            AlertModal(err);
        } else if (url != "") {
            window.open(url, target = val );
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
            <li class="active">Upload excel (SPOT O2)</li>
          </ol>
    </section>
    <!-- Main content -->
    <section class="content">
        <div class="row">

            <div class="col-xs-12">
                <div class='box box-danger'>
                    <div class="box-header">
                       <h3 class="box-title"><i class="fa fa-upload"></i>&nbsp;&nbsp;Upload excel (SPOT O2 data)</h3>
                    </div>
                    <div class="box-body">

                        <!-- left column -->
                        <%-- Add by Turk 11/04/2562 --> Dropdown Month --%>
                        <div class="col-md-12">
                            <table border="0" class="text-black">
                                <tr>
                                    <td style="width: 150px; text-align: right; padding: 2px;">Month&nbsp;:&nbsp;</td>
                                    <td style="width: 125px; text-align: left; padding: 2px;">
                                        <asp:DropDownList ID="ddlMONTH" runat="server" CssClass="form-control" Width="120px">
                                            
                                        </asp:DropDownList>
                                    </td>
                                    <td style="width: 120px; text-align: left; padding: 2px;">
                                        <asp:DropDownList ID="ddlYEAR" runat="server" Width="80px" CssClass="form-control"></asp:DropDownList>
                                    </td>
                                </tr>
                            </table>
                        </div>

                        <!-- left column -->
                        <div class="col-md-12">
                            <table border="0" class="text-black">
                                <tr>
                                    <td style="width: 150px; text-align: right; padding: 2px;">Select excel file&nbsp;:&nbsp;</td>
                                    <td style="width: 150px; text-align: left; padding: 2px;">
                                        <asp:FileUpload ID="FileImportData" AllowMultiple="False" runat="server" Width="650px" />

                                    </td>
                                </tr>
                                <tr><td></td><td>Note: The sheet name have to be the same as abbreviation month. ex: Jan, Feb,.... </td></tr>


                            </table>
                        </div>


                    </div>

                    <div class=" box-footer">

                        <table style="margin-left: 30%;">
                            <tr>
                                <td style="width: 50px">
                                    &nbsp;
                                </td>
                                <td style="width: 120px">
                                    <button type="button" class="pull-left btn btn-block btn-sametogetherproject" id="btnXLS" onclick="javascript: DoAction('IMPORT_XLS','');" >
                                    <i class="fa fa-upload"></i> &nbsp;Upload</button>
                                </td>
                                 <td style="width: 15px">
                                </td>
                                <td style="width: 110px; text-align: center; padding: 2px;">
                                        <input class="btn btn-block btn-sametogetherproject" type="button" name="btnSearch" value="View" onclick="javascript: DoAction('SEARCH','');" style="width: 80px"  />
                                    </td>
                                  <td  style="text-align: left; width:100px;">
                                  </td>
                            </tr>
                        </table>
                    </div>

                    
                </div>


                <div class="box">
                    <div class="box-body" style="overflow-x:auto;">
<asp:Label ID="lblSuccess" runat="server" CssClass="text-black" Text=""></asp:Label><br />
<asp:Label ID="lblAlert" runat="server" CssClass="text-danger" Text=""></asp:Label><br />

                             <asp:GridView ID="gvResult" runat="server" HorizontalAlign="Left" AutoGenerateColumns="False" DataKeyNames="" GridLines="Both"  
          OnRowDataBound="gvResult_RowDataBound" AllowSorting="false" >
                            <HeaderStyle CssClass="Table-head-gray" HorizontalAlign="Center" />
                            <FooterStyle CssClass="" HorizontalAlign="Center" />
                            <RowStyle CssClass="itemRow1" HorizontalAlign="Left" VerticalAlign="Middle" />
                            <AlternatingRowStyle CssClass="itemRow2" HorizontalAlign="Left" VerticalAlign="Middle" />
                            <Columns>
                                
                                 <asp:BoundField HeaderText="No." DataField="ROW_NO" DataFormatString="{0:#,##0}" >
                                    <HeaderStyle CssClass="cell-center" Width="40px"/>
                                    <ItemStyle CssClass="cell-center cell-Middle cell-border" Width="100px"/>
                                </asp:BoundField>
                                <asp:BoundField HeaderText="Sampling Point" DataField="O2_NAME" >
                                    <HeaderStyle CssClass="cell-center" Width="200px"/>
                                    <ItemStyle CssClass="cell-left cell-Middle cell-border" Width="200px"/>
                                </asp:BoundField>

                                <asp:BoundField HeaderText="Date" DataField="SDATE"  DataFormatString="{0:dd/MM/yyyy HH:mm}" HtmlEncode="false">
                                    <HeaderStyle CssClass="cell-center" Width="150px"/>
                                    <ItemStyle CssClass="cell-center cell-Middle cell-border" Width="150px"/>
                                </asp:BoundField>
 
                                  <asp:BoundField HeaderText="Sample No." DataField="SAMPLE_NO" >
                                    <HeaderStyle CssClass="cell-center" Width="180px"/>
                                    <ItemStyle CssClass="cell-center cell-Middle cell-border" Width="180px"/>
                                </asp:BoundField>

                                <asp:BoundField HeaderText="O2 (%mole)" DataField="O2" DataFormatString="{0:#,##0.000}" >
                                    <HeaderStyle CssClass="cell-center" Width="100px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="100px"/>
                                </asp:BoundField>
                       

                            </Columns>
                        </asp:GridView>

                    </div>
                    <!-- /.box-body -->



                </div>
                <!-- /.box -->


                
                <!------------------------------------------------>
 
          
                


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
