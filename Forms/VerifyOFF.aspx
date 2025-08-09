<%@ Page Title="" Language="C#" MasterPageFile="~/Includes/MainPage.Master" AutoEventWireup="true" CodeBehind="VerifyOFF.aspx.cs" Inherits="PTT.GQMS.USL.Web.Forms.VerifyOFF" MaintainScrollPositionOnPostback="True" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

    <!-- multiple Select2 -->
    <link href="../Content/select2.min.css" rel="stylesheet" />
    <script src="../Scripts/select2.full.min.js"></script>   

    <!-- DataTable -->
    <script type="text/javascript" src="../Scripts/DataTables/DataTables/js/jquery.dataTables.min.js"></script>
    <script type="text/javascript" src="../Scripts/DataTables/FixedColumns/js/dataTables.fixedColumns.min.js"></script>
    <script type="text/javascript" src="../Scripts/pdfmake/vfs_fonts.js"></script>

<script type="text/javascript">

            function DoAction(act, val, val2) {
                var confirmed, isSubmitted, url, err, url2;
                confirmed = true; isSubmitted = true; url = ""; url2 = ""; err = "";
                switch (act) {
                    case "SEARCH": break;
                    case "IMPORT_XLS": break; 
                    case "SAVE_XLS": break;                    
                }

                if (err != "") {
                    AlertModal(err);
                } else if (url != "") {
                    window.open(url, target = act+val+val2);
                } else if (url2 != "") {
                    LoadSpin();
                    window.location.href = url2;
                } else if (confirmed && isSubmitted) {
                    SetCtrlValue("ServerAction", act);
                    LoadSpin();
                    SubmitForm();
                }
            }


    //--------------------------------------------------------------------

</script>


   <script>
  


    </script>



</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HeadStyle" runat="server">
    <style>
        .itemdate{
            background-color: #99ff99;
        }
    </style>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <input type="hidden" id="ServerAction" name="ServerAction" />

    <asp:HiddenField ID="hidSITE_ID" runat="server" />
    <asp:HiddenField ID="hidFID" runat="server" />
    <asp:HiddenField ID="hidMM" runat="server" />
    <asp:HiddenField ID="hidYY" runat="server" />
    <asp:HiddenField ID="hidfromDate" runat="server" />
    <asp:HiddenField ID="hidtoDate" runat="server" />

    <!-- Content Header (Page header) -->
    <section class="content-header">
        <ol class="breadcrumb">
            <li><i class="fa fa-home"></i></li>
            <li class="active">Data verification (Offshore)</li>
          </ol>
    </section>
    <!-- Main content -->
    <section class="content">
        <div class="row">

            <div class="col-xs-12">
                <div class='box box-warning'>

                    <div class="box-body">

                        <!-- left column -->
                        <div class="col-md-4">
                            <table border="0" class="text-black">
                                <tr>
                                    <td style="width: 150px; text-align: right; padding: 2px;">Select OGC Main Point&nbsp;:&nbsp;</td>
                                    <td style="width: 150px; text-align: left; padding: 2px;">
                                     <asp:DropDownList ID="ddlFID" runat="server" Width="200px" CssClass="form-control select2">
                                     </asp:DropDownList>
                               </td>
                             </tr>
                            </table>
                        </div>
                         <!-- right column -->
                        <div class="col-md-8">
                            <table border="0" class="text-black">
                                <tr>
                                    <td style="width: 150px; text-align: right; padding: 2px;">Month / Year&nbsp;:&nbsp;</td>
                                    <td style="width: 100px; text-align: left; padding: 2px;">
                                        <asp:DropDownList ID="ddlMONTH" runat="server" Width="100px" CssClass="form-control">
                                           
							            </asp:DropDownList>
                                        </td>
                                   <td style="width: 110px; text-align: left; padding: 2px;">
                                        <asp:DropDownList ID="ddlYEAR" runat="server" Width="80px" CssClass="form-control">
							            </asp:DropDownList>
                                    </td>
                                    <td style="width: 110px; text-align: center; padding: 2px;">
                                        <input class="btn btn-block btn-sametogetherproject" type="button" name="btnSearch" value="View" onclick="javascript: DoAction('SEARCH','');" style="width: 80px"  />
                                    </td>
                                     <td style="width: 5px">
                                </td>
                                     <td style="width: 120px">
                                    <asp:Panel ID="pnlIMPORT" runat="server">
                                    <button type="button" class="pull-left btn btn-block btn-primary" id="btnIMP" onclick="javascript: DoAction('IMPORT_XLS','');" >
                                    <i class="fa fa-sign-in"></i>  &nbsp;Import Excel</button>
                                    </asp:Panel>
                                </td>
                                </tr>
                            </table>
                        </div>



                        <asp:Panel ID="pnlFILE" runat="server" Visible="false">

                 <div class="col-md-12">
                            <table border="0" class="text-black">
                                <tr>
                                    <td style="width: 150px; text-align: right; padding: 2px;">Select excel file&nbsp;:&nbsp;</td>
                                    <td style="width: 150px; text-align: left; padding: 2px;">
                                        <asp:FileUpload ID="FileImportData" runat="server" Width="400px" />
                                    </td>
                                    <td style="width: 5px;"></td>
                                    <td style="width: 90px">
                                    <button type="button" class="pull-left btn btn-block btn-primary" id="btnXLS" onclick="javascript: DoAction('SAVE_XLS','');" >
                                    Save</button>
                                </td>
                                </tr>
                            </table>
                        </div>

                        </asp:Panel>


                        <div class="col-md-12">&nbsp;</div>

<!-------- ASFOUND, ASLEFT, FINAL CAL ----------------------------------------------------------------------------------->
<div class="col-md-6" runat="server" id="divASFOUND" visible="false">
         <table border="0" style="border-spacing: 2px;border-collapse: separate; padding: 5px;" class="text-black">
                <tr>
                    <td style="text-align: right; width: 90px;">As Found&nbsp;:</td>
                    <td style="text-align: left; width: 80px;">
                        <asp:Label ID="lblFOUND_DATE" runat="server" Text=""></asp:Label>
                    </td>
                    <td style="text-align: left; width: 60px;">
                        <asp:Label ID="lblFOUND_STATUS" runat="server" Text="" CssClass="cell-center"  Width="60" Height="27"></asp:Label>
                    </td>
                    <td style="text-align: right; width: 130px;">Final Calibrate&nbsp;:</td>
                    <td style="text-align: left; width: 80px;">
                        <asp:Label ID="lblCAL_DATE" runat="server" Text=""></asp:Label>
                    </td>
                    <td style="text-align: left; width: 60px;">
                        <asp:Label ID="lblCAL_STATUS" runat="server" Text="" CssClass="cell-center"  Width="60" Height="27"></asp:Label>
                    </td>
                    <td style="text-align: right; width: 90px;">As Left&nbsp;:</td>
                    <td style="text-align: left; width: 80px;">
                        <asp:Label ID="lblLEFT_DATE" runat="server" Text=""></asp:Label>
                    </td>
                    <td style="text-align: left; width: 60px;">
                        <asp:Label ID="lblLEFT_STATUS" runat="server" Text="" CssClass="cell-center"  Width="60" Height="27"></asp:Label>
                    </td>
                </tr>

            </table>
</div>


<!----------------------------------------------------------------------------------------------------------------------->

<!------------------------------------------>

     <asp:GridView ID="gvResult" runat="server" HorizontalAlign="Left" AutoGenerateColumns="False" GridLines="Both"  
         OnRowDataBound="gvResult_RowDataBound" OnRowCreated="gvResult_RowCreated" PageSize="10" ShowHeader="false" ShowFooter="false">
                            <HeaderStyle CssClass="Table-head-gray" HorizontalAlign="Center" />
                            <FooterStyle CssClass="ItemFooter_green2" HorizontalAlign="Center" />
                            <RowStyle CssClass="itemRow1" HorizontalAlign="Left" VerticalAlign="Middle" />
                            <PagerSettings Mode="NumericFirstLast" PageButtonCount="10" />
                            <PagerStyle CssClass="pagination-ys cell-borderW1" />
                            <AlternatingRowStyle CssClass="itemRow2" HorizontalAlign="Left" VerticalAlign="Middle" />
                            <Columns>

                                <asp:BoundField HeaderText="Date" DataField="ADATE" DataFormatString="{0:dd/MM/yyyy}" >
                                    <HeaderStyle CssClass="cell-center" Width="100px"/>
                                    <ItemStyle CssClass="cell-center cell-Middle cell-border table itemdate" Width="90px"/>
                                    <FooterStyle CssClass="cell-center cell-Middle cell-border" />
                                </asp:BoundField>

                                <asp:BoundField HeaderText="CH4" DataField="C1"  >
                                    <HeaderStyle CssClass="cell-center" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="70px" />
                                    <FooterStyle CssClass="cell-right cell-Middle cell-border" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="C2H6" DataField="C2"  >
                                    <HeaderStyle CssClass="cell-center" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="70px" />
                                    <FooterStyle CssClass="cell-right cell-Middle cell-border" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="C3H8" DataField="C3"  >
                                    <HeaderStyle CssClass="cell-center" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="70px" />
                                    <FooterStyle CssClass="cell-right cell-Middle cell-border" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="IC4H10" DataField="IC4"  >
                                    <HeaderStyle CssClass="cell-center" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="70px" />
                                    <FooterStyle CssClass="cell-right cell-Middle cell-border" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="NC4H10" DataField="NC4"  >
                                    <HeaderStyle CssClass="cell-center" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="70px" />
                                    <FooterStyle CssClass="cell-right cell-Middle cell-border" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="IC5H12" DataField="IC5"  >
                                    <HeaderStyle CssClass="cell-center" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="70px" />
                                    <FooterStyle CssClass="cell-right cell-Middle cell-border" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="NC5H12" DataField="NC5"  >
                                    <HeaderStyle CssClass="cell-center" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="70px" />
                                    <FooterStyle CssClass="cell-right cell-Middle cell-border" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="C6H14" DataField="C6"  >
                                    <HeaderStyle CssClass="cell-center" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="70px" />
                                    <FooterStyle CssClass="cell-right cell-Middle cell-border" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="C7H16" DataField="C7"  >
                                    <HeaderStyle CssClass="cell-center" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="70px" />
                                    <FooterStyle CssClass="cell-right cell-Middle cell-border" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="CO2" DataField="CO2"  >
                                    <HeaderStyle CssClass="cell-center" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="70px" />
                                    <FooterStyle CssClass="cell-right cell-Middle cell-border" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="N2" DataField="N2"  >
                                    <HeaderStyle CssClass="cell-center" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="70px" />
                                    <FooterStyle CssClass="cell-right cell-Middle cell-border" />
                                </asp:BoundField>
                                 <asp:BoundField HeaderText="SUM" DataField="SUM_COMPO"  >
                                    <HeaderStyle CssClass="cell-center " Width="70px"/>
                                    <ItemStyle CssClass="cell-bg-sum cell-right cell-Middle cell-border" Width="70px" />
                                    <FooterStyle CssClass="cell-bg-sum cell-right cell-Middle cell-border" />
                                </asp:BoundField>

                                <asp:BoundField HeaderText="GHVSAT" DataField="GHV"  >
                                    <HeaderStyle CssClass="cell-center" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="70px" />
                                    <FooterStyle CssClass="cell-right cell-Middle cell-border" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="SG" DataField="SG"  >
                                    <HeaderStyle CssClass="cell-center" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="70px" />
                                    <FooterStyle CssClass="cell-right cell-Middle cell-border" />
                                </asp:BoundField>

                                <asp:BoundField HeaderText="H2O" DataField="H2O"  >
                                    <HeaderStyle CssClass="cell-center Table-head-primary" Width="90px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="70px" />
                                    <FooterStyle CssClass="cell-right cell-Middle cell-border" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="Hg" DataField="HG"  >
                                    <HeaderStyle CssClass="cell-center Table-head-success" Width="90px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="70px" />
                                    <FooterStyle CssClass="cell-right cell-Middle cell-border" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="H2S" DataField="H2S"  >
                                    <HeaderStyle CssClass="cell-center Table-head-orange" Width="90px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="70px" />
                                    <FooterStyle CssClass="cell-right cell-Middle cell-border" />
                                </asp:BoundField>
                                 
                            </Columns>
                        </asp:GridView>
                    </div>
                    
<!---------------------------------------------------------->

                    <div class=" box-footer">
                    </div>
                </div>

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
<!-- Page script -->
<script>
    $(function () {
        //Initialize Select2 Elements
        $('.select2').select2()

    })
</script>

<!--#include file="../Includes/JSfooter.html" -->

</asp:Content>
