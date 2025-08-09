<%@ Page Title="" Language="C#" MasterPageFile="~/Includes/MainPage.Master" AutoEventWireup="true" CodeBehind="rptMonthly.aspx.cs" Inherits="PTT.GQMS.USL.Web.Reports.rptMonthly" MaintainScrollPositionOnPostback="True" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

    <!-- multiple Select2 -->
    <link href="../Content/select2.min.css" rel="stylesheet" />
    <script src="../Scripts/select2.full.min.js"></script>   

    <!-- DataTable -->
    <script type="text/javascript" src="../Scripts/DataTables/DataTables/js/jquery.dataTables.min.js"></script>
    <script type="text/javascript" src="../Scripts/DataTables/FixedColumns/js/dataTables.fixedColumns.min.js"></script>
    <script type="text/javascript" src="../Scripts/pdfmake/vfs_fonts.js"></script>


        <script type="text/javascript">
            var NextPage = "ExcelMonthly?UID=<%= HttpContext.Current.Session["UID"] %>";

            function DoAction(act, val) {
                var confirmed, isSubmitted, url, err, url2;
                confirmed = true; isSubmitted = true; url = ""; url2 = ""; err = "";
                switch (act) {
                    case "SEARCH": break;
                    case "SAVE": break;
                    case "EXPORT_XLS":
                        //'#' => '%23',
                        var ff =GetCtrlValue("<%=hidFID.ClientID%>");
                        ff = ff.replace("#","%23");
                        url=NextPage+"&K="+GetCtrlValue("<%=hidSITE_ID.ClientID%>")+"&F="+ff+"&T="+GetCtrlValue("<%=ddlRPT_TYPE.ClientID%>")+"&MM="+GetCtrlValue("<%=hidMM.ClientID%>")+"&YY="+GetCtrlValue("<%=hidYY.ClientID%>"); 
                        break;

                        
                }

                if (err != "") {
                    AlertModal(err);
                } else if (url != "") {
                    window.open(url, target = val);
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
    //--การกำหนด fix column ทำให้อ่าน GetCtrl('chkAll').checked เป็นค่าตั้งแต่ page load เสมอ
    //--จะไม่สามารถกำหนด checkbox ที่เป็น fixed column ได้
            function SelectAll() {
            var i, chk, chkTmp,max;
            max = <%=chkCount%>;

            chk = GetCtrl('chkAll').checked;  
            for (i=0; i<max; i++) {
                chkTmp = GetCtrl("chkSelect" + i);
                if (chkTmp != null){
                    chkTmp.checked = chk;
                }
            } 


            }

    </script>

 
   <script>
     // ------------------ DataTable --------------------------------------------------------------------      

<%--       $(function () {

        $('#<%=gvResult.ClientID%>').DataTable({
           bInfo:          false,
           sort:           false,
           searching:      false,
          // scrollY:        "480px",
           scrollX:        true,
           scrollCollapse: true,
           paging:         false,
           fixedHeader:    true,
           fixedColumns:   {
               leftColumns: 2
           }
          });
       });--%>

       // -----------------------------------------------------------------------------------------------    


    </script>
 
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HeadStyle" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <input type="hidden" id="ServerAction" name="ServerAction" />

    <asp:HiddenField ID="hidSITE_ID" runat="server" />
    <asp:HiddenField ID="hidFID" runat="server" />
    <asp:HiddenField ID="hidMM" runat="server" />
    <asp:HiddenField ID="hidYY" runat="server" />
<asp:HiddenField ID="hidfromDate" runat="server" />
<asp:HiddenField ID="hidtoDate" runat="server" />

<asp:HiddenField ID="hidgISO_FLAG" runat="server" />
<asp:HiddenField ID="hidgorderYMD" runat="server" />
<asp:HiddenField ID="hidgexpireYMD" runat="server" />
<asp:HiddenField ID="hidgC1" runat="server" />
<asp:HiddenField ID="hidgC2" runat="server" />
<asp:HiddenField ID="hidgC3" runat="server" />
<asp:HiddenField ID="hidgIC4" runat="server" />
<asp:HiddenField ID="hidgNC4" runat="server" />
<asp:HiddenField ID="hidgIC5" runat="server" />
<asp:HiddenField ID="hidgNC5" runat="server" />
<asp:HiddenField ID="hidgC6" runat="server" />
<asp:HiddenField ID="hidgN2" runat="server" />
<asp:HiddenField ID="hidgCO2" runat="server" />
<asp:HiddenField ID="hidgH2S" runat="server" />

<asp:HiddenField ID="hidgHG" runat="server" /><!-- EDIT 22/07/2019 -->

    <asp:HiddenField ID="hidgC1_MIN" runat="server" /> 
<asp:HiddenField ID="hidgC2_MIN" runat="server" />
<asp:HiddenField ID="hidgC3_MIN" runat="server" />
<asp:HiddenField ID="hidgIC4_MIN" runat="server" />
<asp:HiddenField ID="hidgNC4_MIN" runat="server" />
<asp:HiddenField ID="hidgIC5_MIN" runat="server" />
<asp:HiddenField ID="hidgNC5_MIN" runat="server" />
<asp:HiddenField ID="hidgC6_MIN" runat="server" />
<asp:HiddenField ID="hidgN2_MIN" runat="server" />
<asp:HiddenField ID="hidgCO2_MIN" runat="server" />
<asp:HiddenField ID="hidgH2S_MIN" runat="server" />
    <asp:HiddenField ID="hidgHG_MIN" runat="server" />

<asp:HiddenField ID="hidgC1_MAX" runat="server" />
<asp:HiddenField ID="hidgC2_MAX" runat="server" />
<asp:HiddenField ID="hidgC3_MAX" runat="server" />
<asp:HiddenField ID="hidgIC4_MAX" runat="server" />
<asp:HiddenField ID="hidgNC4_MAX" runat="server" />
<asp:HiddenField ID="hidgIC5_MAX" runat="server" />
<asp:HiddenField ID="hidgNC5_MAX" runat="server" />
<asp:HiddenField ID="hidgC6_MAX" runat="server" />
<asp:HiddenField ID="hidgN2_MAX" runat="server" />
<asp:HiddenField ID="hidgCO2_MAX" runat="server" />
<asp:HiddenField ID="hidgH2S_MAX" runat="server" />
    <asp:HiddenField ID="hidgHG_MAX" runat="server" />


    <asp:HiddenField ID="hidREPORT_ID" runat="server" />
    <asp:HiddenField ID="hidISO_ACCREDIT" runat="server" />
    <asp:HiddenField ID="hidH2S_FLAG" runat="server" />
    <asp:HiddenField ID="hidISO_MINMAX" runat="server" />


    <!-- Content Header (Page header) -->
    <section class="content-header">
        <ol class="breadcrumb">
            <li><i class="fa fa-home"></i></li>
            <li>Reports </li>
            <li class="active">Onshore Monthly Report</li>
          </ol>
    </section>
    <!-- Main content -->
    <section class="content">
        <div class="row">

            <div class="col-xs-12">
                <div class='box box-success'>
                    <div class="box-header">
                       <h3 class="box-title"><i class="fa fa-file-text "></i> Onshore Monthly Report</h3>
                    </div>
                    <div class="box-body">

                        <!-- left column -->
                        <div class="col-md-5">
                            <table border="0" class="text-black">
                                <tr>
                                    <td style="width: 180px; text-align: right; padding: 2px;">Select OGC Main Point&nbsp;:&nbsp;</td>
                                    <td style="width: 150px; text-align: left; padding: 2px;">
                                     <asp:DropDownList ID="ddlFID" runat="server" Width="200px" CssClass="form-control select2">
                                     </asp:DropDownList>
                               </td>
                             </tr>
                            </table>
                        </div>
                         <!-- right column -->
                        <div class="col-md-7">
                            <table border="0" class="text-black">
                                <tr>
                                    <td style="width: 150px; text-align: right; padding: 2px;">Type&nbsp;:&nbsp;</td>
                                    <td style="width: 100px; text-align: left; padding: 2px;">
                                        <asp:DropDownList ID="ddlRPT_TYPE" runat="server" Width="150px" CssClass="form-control">
                                            <asp:ListItem Value="20DAY" Text="Report 21-20"></asp:ListItem>
                                            <asp:ListItem Value="27DAY" Text="Report 28-27"></asp:ListItem>
                                            <asp:ListItem Value="ENDMTH" Text="Report end month" Selected></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                    <td style="width: 110px; text-align: right; padding: 2px;">Month / Year&nbsp;:&nbsp;</td>
                                    <td style="width: 100px; text-align: left; padding: 2px;">
                                         <asp:DropDownList ID="ddlMONTH" runat="server" Width="100px" CssClass="form-control">
							            </asp:DropDownList>
                                        </td>
                                   <td style="width: 120px; text-align: left; padding: 2px;">
                                        <asp:DropDownList ID="ddlYEAR" runat="server" Width="80px" CssClass="form-control">
							            </asp:DropDownList>
                                    </td>
                                    <td style="width: 110px; text-align: center; padding: 2px;">
                                        <input class="btn btn-block btn-sametogetherproject" type="button" name="btnSearch" value="View" onclick="javascript: DoAction('SEARCH','');" style="width: 80px"  />
                                    </td>
                                </tr>
                             
                            </table>
                        </div>

                    </div>
                    
                </div>

<asp:Panel ID="pnlREPORT" runat="server" Visible="false">
                <div class="box">
                    <div class="box-body">


<!------------------------------------------>


     <asp:GridView ID="gvResult" runat="server" HorizontalAlign="Left" AutoGenerateColumns="False" DataKeyNames="ADATE" GridLines="Both"  
         OnRowDataBound="gvResult_RowDataBound" PageSize="10"  ShowFooter="True">
                            <HeaderStyle CssClass="Table-head-gray" HorizontalAlign="Center" />
                            <FooterStyle CssClass="ItemFooter_green" HorizontalAlign="Center" />
                            <RowStyle CssClass="itemRow1" HorizontalAlign="Left" VerticalAlign="Middle" />
                            <PagerSettings Mode="NumericFirstLast" PageButtonCount="10" />
                            <PagerStyle CssClass="pagination-ys cell-borderW1" />
                            <AlternatingRowStyle CssClass="itemRow2" HorizontalAlign="Left" VerticalAlign="Middle" />
                            <Columns>

                                <asp:TemplateField HeaderText="">
                                    <HeaderStyle Width="30px" CssClass="cell-center cell-middle" />
                                    <ItemStyle Width="30px" CssClass="cell-center cell-middle cell-border" />
                                    <ItemTemplate> 

                                    </ItemTemplate>
                                    <FooterTemplate></FooterTemplate>
                                </asp:TemplateField>

                                <asp:BoundField HeaderText="TIME" DataField="ADATE" DataFormatString="{0:dd/MM/yyyy}" >
                                    <HeaderStyle CssClass="cell-center" Width="90px"/>
                                    <ItemStyle CssClass="cell-center cell-Middle cell-border" Width="90px"/>
                                    <FooterStyle CssClass="cell-center cell-Middle cell-border" />
                                </asp:BoundField>

                                <asp:BoundField HeaderText="CH4" DataField=""  HtmlEncode="false" >
                                    <HeaderStyle CssClass="cell-center" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="70px"/>
                                    <FooterStyle CssClass="cell-right cell-Middle cell-border" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="C2H6" DataField=""  HtmlEncode="false" >
                                    <HeaderStyle CssClass="cell-center" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="70px"/>
                                    <FooterStyle CssClass="cell-right cell-Middle cell-border" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="C3H8" DataField="" HtmlEncode="false" >
                                    <HeaderStyle CssClass="cell-center" Width="70px" />
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="70px"/>
                                    <FooterStyle CssClass="cell-right cell-Middle cell-border" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="IC4H10" DataField=""  HtmlEncode="false" >
                                    <HeaderStyle CssClass="cell-center" Width="90px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="70px"/>
                                    <FooterStyle CssClass="cell-right cell-Middle cell-border" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="NC4H10" DataField=""  HtmlEncode="false" >
                                    <HeaderStyle CssClass="cell-center" Width="90px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="70px"/>
                                    <FooterStyle CssClass="cell-right cell-Middle cell-border" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="IC5H12" DataField=""  HtmlEncode="false" >
                                    <HeaderStyle CssClass="cell-center" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="70px"/>
                                    <FooterStyle CssClass="cell-right cell-Middle cell-border" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="NC5H12" DataField=""  HtmlEncode="false" >
                                    <HeaderStyle CssClass="cell-center" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="70px"/>
                                    <FooterStyle CssClass="cell-right cell-Middle cell-border" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="C6H14" DataField=""  HtmlEncode="false" >
                                    <HeaderStyle CssClass="cell-center" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="70px"/>
                                    <FooterStyle CssClass="cell-right cell-Middle cell-border" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="CO2" DataField=""  HtmlEncode="false" >
                                    <HeaderStyle CssClass="cell-center" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="70px"/>
                                    <FooterStyle CssClass="cell-right cell-Middle cell-border" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="N2" DataField=""  HtmlEncode="false" >
                                    <HeaderStyle CssClass="cell-center" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="70px"/>
                                    <FooterStyle CssClass="cell-right cell-Middle cell-border" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="H2S" DataField=""  HtmlEncode="false" >
                                    <HeaderStyle CssClass="cell-center" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="70px"/>
                                    <FooterStyle CssClass="cell-right cell-Middle cell-border" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="NETHVDRY" DataField=""  HtmlEncode="false" >
                                    <HeaderStyle CssClass="cell-center" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="70px"/>
                                    <FooterStyle CssClass="cell-right cell-Middle cell-border" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="HVSAT" DataField=""  HtmlEncode="false" >
                                    <HeaderStyle CssClass="cell-center" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="70px"/>
                                    <FooterStyle CssClass="cell-right cell-Middle cell-border" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="SG" DataField=""  HtmlEncode="false" >
                                    <HeaderStyle CssClass="cell-center" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="70px"/>
                                    <FooterStyle CssClass="cell-right cell-Middle cell-border" />
                                </asp:BoundField>
                                  <asp:BoundField HeaderText="H2O" DataField=""  HtmlEncode="false" >
                                    <HeaderStyle CssClass="cell-center" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="70px"/>
                                    <FooterStyle CssClass="cell-right cell-Middle cell-border" />
                                </asp:BoundField>
                               
                                <asp:BoundField HeaderText="UNNORMMIN" DataField=""  HtmlEncode="false" >
                                    <HeaderStyle CssClass="cell-center" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="70px"/>
                                    <FooterStyle CssClass="cell-right cell-Middle cell-border" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="UNNORMMAX" DataField=""  HtmlEncode="false" >
                                    <HeaderStyle CssClass="cell-center" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="70px"/>
                                    <FooterStyle CssClass="cell-right cell-Middle cell-border" />
                                </asp:BoundField>
                              
                                <asp:BoundField HeaderText="RUN" DataField="" DataFormatString="{0:#,##0}" >
                                    <HeaderStyle CssClass="Table-head-orange cell-center" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="70px"/>
                                    <FooterStyle CssClass="cell-right cell-Middle cell-border" />
                                </asp:BoundField>
                                
                                <asp:BoundField HeaderText="FLOW" DataField=""  HtmlEncode="false" >
                                    <HeaderStyle CssClass="Table-head-success cell-center" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="70px"/>
                                    <FooterStyle CssClass="cell-right cell-Middle cell-border" />
                                </asp:BoundField>                               

                            </Columns>
                        </asp:GridView>


<!------------------------------------------>


                        <div class="col-md-12">
                            <table border="0" class="text-black">
                                <tr><td colspan="5" style="height: 5px;"></td></tr>
                                <tr>
                                    <td style="text-align: right; height: 50px; width: 200px; vertical-align: top;">Remarks &nbsp;:&nbsp;<br />(Auto)&nbsp;&nbsp;&nbsp;</td>
                                    <td style="text-align: left; height: 50px;">
                                        <asp:TextBox ID="txtREMARK" MaxLength="4000" CssClass="form-control" runat="server" Width="580"  TextMode="MultiLine" Rows="3" Text="" Height="100"></asp:TextBox>
                                    </td>
                                    <td></td>
                                    <td style="text-align: right; height: 50px; width: 140px; vertical-align: top;">Remarks &nbsp;:&nbsp;<br />(Additional)&nbsp;&nbsp;&nbsp;</td>
                                    <td style="text-align: left; height: 50px;">
                                        <asp:TextBox ID="txtREMARK_ADD" MaxLength="4000" CssClass="form-control" runat="server" Width="580"  TextMode="MultiLine" Rows="3" Text="" Height="100"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr><td colspan="5" style="height: 5px;"></td></tr>
                                <tr>
                                    <td style="text-align: right; height: 30px; width: 200px; vertical-align: top;">Signature Require&nbsp;:&nbsp;</td>
                                    <td style="text-align: left; height: 30px;" >
                                          <asp:RadioButton ID="rblSIGN_Y" runat="server" Text="&nbsp;Yes" GroupName="S" />
                                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                        <asp:RadioButton ID="rblSIGN_N" runat="server" Text="&nbsp;No"  Checked="True"  GroupName="S" />
                                    </td>
                                   
                                    <td colspan="3"></td>
                                </tr>
                                <tr><td colspan="5" style="height: 5px;"></td></tr>
                            </table>
                        </div>

                        <div class="col-md-5">
                            <table border="0" class="text-black">
                                <tr>
                                    <td style="width: 200px; text-align: right; padding: 2px;">Report by&nbsp;:&nbsp;</td>
                                    <td style="width: 150px; text-align: left; padding: 2px;">
                                        <asp:DropDownList ID="ddlREPORT_BY" runat="server" Width="200px" CssClass="form-control">
							            </asp:DropDownList>
                                    </td>
                                </tr>
                            </table>
                        </div>

                       <div class="col-md-5">
                            <table border="0" class="text-black">
                                <tr>
                                    <td style="width: 100px; text-align: right; padding: 2px;">Approved by&nbsp;:&nbsp;</td>
                                    <td style="width: 150px; text-align: left; padding: 2px;">
                                        <asp:DropDownList ID="ddlAPPROVE_BY" runat="server" Width="200px" CssClass="form-control">
							            </asp:DropDownList>
                                    </td>
                                </tr>
                            </table>
                        </div>

                    </div>
                    <!-- /.box-body -->
                      

<!---------------------------------------------------------->
<!---------------------------------------------------------->

          <div class="box-footer">

                        <table border="0" style="margin-left: 35%;">
                            <tr>
                                <td style="width: 100px">
                                    <input name="btnSave" class="btn btn-block btn-sametogetherproject" type="button" id="btnSave" value="Save" onclick="javascript: DoAction('SAVE','');" style="width: 80px" />
                                </td>
                                <td style="width: 120px">
                                     <button type="button" class="pull-left btn btn-block btn-sametogetherproject" id="btnXLS" onclick="javascript: DoAction('EXPORT_XLS','');" >
                                    <i class="fa fa-file-excel-o"></i> &nbsp;Export Excel</button>
                                </td>
                            </tr>
                        </table>
                    </div>
                    

                </div>
                <!-- /.box -->
</asp:Panel>




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
