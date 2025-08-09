<%@ Page Title="" Language="C#" MasterPageFile="~/Includes/MainPage.Master" AutoEventWireup="true" CodeBehind="UploadH2S.aspx.cs" Inherits="PTT.GQMS.USL.Web.Forms.UploadH2S" MaintainScrollPositionOnPostback="True" %>


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
            <li class="active">Upload excel (SPOT H2S)</li>
          </ol>
    </section>
    <!-- Main content -->
    <section class="content">
        <div class="row">

            <div class="col-xs-12">
                <div class='box box-danger'>
                    <div class="box-header">
                       <h3 class="box-title"><i class="fa fa-upload"></i>&nbsp;&nbsp;Upload excel (SPOT H2S data)</h3>
                    </div>
                    <div class="box-body">

                        <!-- left column -->
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
          OnRowDataBound="gvResult_RowDataBound" AllowSorting="false"  >
                            <HeaderStyle CssClass="Table-head-gray" HorizontalAlign="Center" />
                            <FooterStyle CssClass="" HorizontalAlign="Center" />
                            <RowStyle CssClass="itemRow1" HorizontalAlign="Left" VerticalAlign="Middle" />
                            <AlternatingRowStyle CssClass="itemRow2" HorizontalAlign="Left" VerticalAlign="Middle" />
                            <Columns>
                                 <asp:BoundField HeaderText="No." DataField="ROW_NO" DataFormatString="{0:#,##0}" >
                                    <HeaderStyle CssClass="cell-center" Width="40px"/>
                                    <ItemStyle CssClass="cell-center cell-Middle cell-border" Width="100px"/>
                                </asp:BoundField>
                                <asp:BoundField HeaderText="Sampling Point" DataField="H2S_NAME" >
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

                                <asp:BoundField HeaderText="Total Sulfer" DataField="SULFUR"  DataFormatString="{0:#,##0.00}" >
                                    <HeaderStyle CssClass="cell-center" Width="100px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="100px"/>
                                </asp:BoundField>
                                <asp:BoundField HeaderText="H2S" DataField="H2S" DataFormatString="{0:#,##0.00}" >
                                    <HeaderStyle CssClass="cell-center" Width="100px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="100px"/>
                                </asp:BoundField>
                                 <asp:BoundField HeaderText="COS" DataField="COS" DataFormatString="{0:#,##0.00}" >
                                    <HeaderStyle CssClass="cell-center" Width="100px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="100px"/>
                                </asp:BoundField>
                                <asp:BoundField HeaderText="CH3SH" DataField="CH3SH" DataFormatString="{0:#,##0.00}" >
                                    <HeaderStyle CssClass="cell-center" Width="100px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="100px"/>
                                </asp:BoundField>
                                <asp:BoundField HeaderText="C2H5SH" DataField="C2H5SH" DataFormatString="{0:#,##0.00}" >
                                    <HeaderStyle CssClass="cell-center" Width="100px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="100px"/>
                                </asp:BoundField>
                                <asp:BoundField HeaderText="DMS" DataField="DMS" DataFormatString="{0:#,##0.00}" >
                                    <HeaderStyle CssClass="cell-center" Width="100px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="100px"/>
                                </asp:BoundField>
                                <asp:BoundField HeaderText="T-ButylSH" DataField="LSH"  DataFormatString="{0:#,##0.00}" >
                                    <HeaderStyle CssClass="cell-center" Width="100px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="100px"/>
                                </asp:BoundField>
                                <asp:BoundField HeaderText="C3H7SH" DataField="C3H7SH"  DataFormatString="{0:#,##0.00}" >
                                    <HeaderStyle CssClass="cell-center" Width="100px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="100px"/>
                                </asp:BoundField>

                            </Columns>
                        </asp:GridView>

                    </div>
                    <div class="box-body">
                        <asp:Label ID="lblRemark" runat="server" CssClass="text-black" Text="หมายเหตุ: H2S อยู่ในหน่วย PPM" Visible="False"></asp:Label>

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
