<%@ Page Title="" Language="C#" MasterPageFile="~/Includes/MainPage.Master" AutoEventWireup="true" CodeBehind="MngCustomer.aspx.cs" Inherits="PTT.GQMS.USL.Web.Master.MngCustomer"  %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <!-- multiple Select2 -->
    <link href="../Content/select2.min.css" rel="stylesheet" />
    <script src="../Scripts/select2.full.min.js"></script>   

        <script type="text/javascript">
            var NextPage1 = "MngCustomerDetail?UID=<%= HttpContext.Current.Session["UID"] %>";

            function DoAction(act, val) {
                var confirmed, isSubmitted, url, err, url2;
                confirmed = true; isSubmitted = true; url = ""; url2 = ""; err = "";
                switch (act) {
                    case "ADD": url2 = NextPage1;  break;
                    case "SELECT": url2 = NextPage1 + "&K=" + val + ""; break;
                    case "SEARCH": break;
                    case "EXPORT_XLS":
                        break;
                    case "IMPORT_XLS":  //-- edit 29/05/2024
                        break;
                    case "SAVE_XLS": break;   
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
            <li>Master data</li>
            <li class="active">Customer data (GIS) </li>
        </ol>
    </section>
    <!-- Main content -->
    <section class="content">

     <div class="row">

            <div class="col-xs-12">
                <div class='box box-violet'>
                    <div class="box-header">
                       <h3 class="box-title"><i class="fa fa-search"></i> Search</h3>
                    </div>
                    <div class="box-body">

                        <!-- left column -->
                        <div class="col-md-6">
                            <table border="0" class="text-black">
                                <tr>
                                    <td style="width: 150px; text-align: right; padding: 2px;">Customer name&nbsp;:&nbsp;</td>
                                    <td style="width: 290px; text-align: left; padding: 2px;">
                                        <asp:TextBox ID="txtNAME" CssClass="form-control" runat="server" Width="500px" MaxLength="150" Text=""></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <td style="width: 150px; text-align: right; padding: 2px;">Type&nbsp;:&nbsp;</td>
                                    <td style="width: 290px; text-align: left; padding: 2px;">
                                        <asp:ListBox ID="ddlSUB_TYPE" runat="server" Width="500px" SelectionMode="Multiple" CssClass="form-control select2">
                                        </asp:ListBox>
                                    </td>
                                </tr>
                            </table>
                        </div>
                         <!-- right column -->
                        <div class="col-md-6">
                            <table border="0" class="text-black">
                                <tr>
                                    <td style="width: 150px; text-align: right; padding: 2px;">Region&nbsp;:&nbsp;</td>
                                    <td style="width: 290px; text-align: left; padding: 2px;">
                                        <asp:ListBox ID="ddlREGION" runat="server" Width="500px" SelectionMode="Multiple" CssClass="form-control select2">
                                        </asp:ListBox>
                                   </td>
                                </tr>
                                <tr>
                                    <td style="width: 150px; text-align: right; padding: 2px;">OGC Main Point&nbsp;:&nbsp;</td>
                                    <td style="width: 290px; text-align: left; padding: 2px;">
                                        <asp:ListBox ID="ddlQUALITY_MAIN" runat="server" Width="500px" SelectionMode="Multiple" CssClass="form-control select2">
                                        </asp:ListBox>
                                   </td>
                                </tr>
                            </table>
                        </div>
                        <!-- left column -->
                        <div class="col-md-6">
                            <table border="0" class="text-black">
                                <tr>
                                    <td style="width: 151px; text-align: right; padding: 2px;">Created Date&nbsp;:&nbsp;</td>
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

                         <!-- right column -->
                        <div class="col-md-6">
                            <table border="0" class="text-black">
                                <tr>
                                    <td style="width: 151px; text-align: right; padding: 2px;">Updated Date&nbsp;:&nbsp;</td>
                                                                <td style="width: 120px;" >

                                                                             <div id="datepicker3" class="input-group date" style="vertical-align: baseline">
                                                                                <asp:TextBox ID="txtDateFrom2" runat="server" class="form-control timepicker" Width="100px" data-date-language="en-US" maxlength='10'  ></asp:TextBox>
                                                                                 <div class="input-group-addon">
                                                                                        <i id="i3" class="fa fa-calendar"></i>
                                                                                 </div>
                                                                             </div>   
                                                                            <script>
                                                                                $(function () {
                                                                                    $("input[id$=txtDateFrom2]").datepicker({
                                                                                        format: 'dd/mm/yyyy'
                                                                                    }).on('changeDate', function (selected) {
                                                                                        if ($(this).val() == "") {
                                                                                            $('input[id$=txtDateFrom2]').datepicker('setEndDate', null);
                                                                                            $('input[id$=txtDateTo2]').datepicker('setStartDate', null);
                                                                                            $('input[id$=txtDateTo2]').datepicker('setEndDate', null);
                                                                                            $('input[id$=txtDateTo2]').val("");
                                                                                        } else {
                                                                                            var dateFrom = new Date(selected.date.valueOf());
                                                                                            $('input[id$=txtDateTo2]').datepicker('setStartDate', dateFrom);
                                                                                        }
                                                                                    });
                                                                                    //.keydown(false);
                                                                                    
                                                                                });
                                                                                $('#datepicker3').datepicker({
                                                                                    autoclose: true
                                                                                })
	                                                                         </script>
                                                                     </td> 
                                                                <td style="width: 40px; height: 27px;text-align: right; padding: 2px;">To&nbsp;:&nbsp;</td>
                                                        <td  style="width: 120px;">
                                                        <div id="$('#datepicker4').datepicker({
                                                                autoclose: true
                                                            })" class="input-group date" style="vertical-align: baseline">
                                                            <asp:TextBox ID="txtDateTo2" runat="server" class="form-control timepicker" Width="100px" data-date-language="en-US" maxlength='10'   ></asp:TextBox>
                                                            <div class="input-group-addon">
                                                                            <i id="i4" class="fa fa-calendar"></i>
                                                                        </div>
                                                        </div>
                                                            <script>
                                                            $(function () {
                                                                $('input[id$=txtDateTo2]').datepicker({
                                                                    format: 'dd/mm/yyyy'
                                                                }).on('changeDate', function (selected) {
                                                                    if ($(this).val() == "") {
                                                                        $('input[id$=txtDateFrom2]').datepicker('setEndDate', null);
                                                                    } else {
                                                                        var dateTo = new Date(selected.date.valueOf());
                                                                        $('input[id$=txtDateFrom2]').datepicker('setEndDate', dateTo);
                                                                    }
                                                                });
                                                                //.keydown(false);
                                                            });
                                                            $('#datepicker4').datepicker({
                                                                autoclose: true
                                                            })
                                                        	</script>
                                                                            
                                                        </td>
                                </tr>
                             
                            </table>
                        </div>
                    </div>

                    <div class=" box-footer">

                        <table style="margin-left: 30%;">
                            <tr>
                                <td style="width: 90px">
                                    <input class="btn btn-block btn-sametogetherproject" type="button" name="btnSearch" value="Search" onclick="javascript: DoAction('SEARCH','');" style="width: 80px"  />
                                </td>
                                 <td style="width: 120px">
                                    <button type="button" class="pull-left btn btn-block btn-sametogetherproject" id="btnXLS" onclick="javascript: DoAction('EXPORT_XLS','');" >
                                    <i class="fa fa-file-excel-o"></i> &nbsp;Export Excel</button>
                                </td>
                                <td style="width: 20px">
                                </td>
                                     <td style="width: 120px">
                                    <asp:Panel ID="pnlIMPORT" runat="server">
                                    <button type="button" class="pull-left btn btn-block btn-primary" id="btnIMP" onclick="javascript: DoAction('IMPORT_XLS','');" >
                                    <i class="fa fa-sign-in"></i>  &nbsp;Import Excel</button>
                                    </asp:Panel>
                                </td>
                                  <td style="width: 20px">
                                </td>
                                <td style="width: 100px">
                                    <asp:Panel ID="pnlADD" runat="server" Visible="false">
                                        <input name="btnAdd" runat="server" class="btn btn-block btn-primary" type="button" id="btnAdd" value="Add" onclick="javascript: DoAction('ADD','');" style="width: 80px" />
                                    </asp:Panel>                                    
                                </td>
                            </tr>
                        </table>

             <asp:Panel ID="pnlFILE" runat="server" Visible="false">
                 <div class="col-md-12">
                            <table border="0" class="text-black">
                                <tr><td colspan="4">&nbsp;</td></tr>
                                <tr>
                                    <td style="width: 150px; text-align: right; padding: 2px;">Select excel file&nbsp;:&nbsp;</td>
                                    <td style="width: 150px; text-align: left; padding: 2px;">
                                        <asp:FileUpload ID="FileImportData" runat="server" Width="400px" />
                                    </td>
                                    <td style="width: 5px;"></td>
                                    <td style="width: 90px">
                                    <button type="button" class="pull-left btn btn-block btn-primary" id="btnXLS2" onclick="javascript: DoAction('SAVE_XLS','');" >
                                    Save</button>
                                </td>
                                </tr>
                            </table>
                        </div>

                        </asp:Panel>

                    </div>


                    
                </div>


                <div class="box">
                    <div class="box-body" style="overflow-x:auto;">

     <asp:GridView ID="gvResult" runat="server" HorizontalAlign="Left" AutoGenerateColumns="False" DataKeyNames="PERMANENT_CODE" GridLines="Both"  
          OnRowDataBound="gvResult_RowDataBound" 
                            AllowSorting="True" OnSorting="gvResult_Sorting">
                            <HeaderStyle CssClass="Table-head-violet" HorizontalAlign="Center" />
                            <FooterStyle CssClass="" HorizontalAlign="Center" />
                            <RowStyle CssClass="itemRow1" HorizontalAlign="Left" VerticalAlign="Middle" />
                            <AlternatingRowStyle CssClass="itemRow2" HorizontalAlign="Left" VerticalAlign="Middle" />
                            <Columns>

                                <asp:BoundField HeaderText="ID" DataField="PERMANENT_CODE" SortExpression="PERMANENT_CODE">
                                    <HeaderStyle CssClass="cell-center bg-green" Width="100px"/>
                                    <ItemStyle CssClass="cell-left cell-Middle cell-border" Width="100px"/>
                                </asp:BoundField>
                                <asp:BoundField HeaderText="Short Name" DataField="NAME_ABBR" SortExpression="NAME_ABBR">
                                    <HeaderStyle CssClass="cell-center bg-green" Width="80px"/>
                                    <ItemStyle CssClass="cell-left cell-Middle cell-border" Width="80px"/>
                                </asp:BoundField>
                                 <asp:BoundField HeaderText="Customer Name" DataField="NAME_FULL" SortExpression="NAME_FULL">
                                    <HeaderStyle CssClass="cell-center bg-green" Width="340px"/>
                                    <ItemStyle CssClass="cell-left cell-Middle cell-border" Width="340px"/>
                                </asp:BoundField>
                                <asp:BoundField HeaderText="Type" DataField="SUB_TYPE" SortExpression="SUB_TYPE">
                                    <HeaderStyle CssClass="cell-center bg-green" Width="90px"/>
                                    <ItemStyle CssClass="cell-left cell-Middle cell-border" Width="90px"/>
                                </asp:BoundField>
                                <asp:BoundField HeaderText="Region" DataField="REGION" SortExpression="REGION">
                                    <HeaderStyle CssClass="cell-center bg-green" Width="90px"/>
                                    <ItemStyle CssClass="cell-left cell-Middle cell-border" Width="90px"/>
                                </asp:BoundField>
                                <asp:BoundField HeaderText="BV Zoning" DataField="BV_ZONE" SortExpression="BV_ZONE">
                                    <HeaderStyle CssClass="cell-center bg-green" Width="100px"/>
                                    <ItemStyle CssClass="cell-left cell-Middle cell-border" Width="100px"/>
                                </asp:BoundField>
                                 <asp:BoundField HeaderText="Block Valve" DataField="BV_VALVE" SortExpression="BV_VALVE">
                                    <HeaderStyle CssClass="cell-center bg-green" Width="100px"/>
                                    <ItemStyle CssClass="cell-left cell-Middle cell-border" Width="100px"/>
                                </asp:BoundField>
                                <asp:BoundField HeaderText="OGC Main Point" DataField="QUALITY_MAIN" SortExpression="QUALITY_MAIN">
                                    <HeaderStyle CssClass="cell-center" Width="120px"/>
                                    <ItemStyle CssClass="cell-left cell-Middle cell-border" Width="120px"/>
                                </asp:BoundField>
                                <asp:BoundField HeaderText="OGC Support Point1" DataField="QUALITY_SUPPORT1" SortExpression="QUALITY_SUPPORT1">
                                    <HeaderStyle CssClass="cell-center" Width="120px"/>
                                    <ItemStyle CssClass="cell-left cell-Middle cell-border" Width="120px"/>
                                </asp:BoundField>
                                <asp:BoundField HeaderText="OGC Support Point2" DataField="QUALITY_SUPPORT2" SortExpression="QUALITY_SUPPORT2">
                                    <HeaderStyle CssClass="cell-center" Width="120px"/>
                                    <ItemStyle CssClass="cell-left cell-Middle cell-border" Width="120px"/>
                                </asp:BoundField>
                                <asp:BoundField HeaderText="OMA Main Point" DataField="OMA_MAIN" SortExpression="OMA_MAIN">
                                    <HeaderStyle CssClass="cell-center" Width="120px"/>
                                    <ItemStyle CssClass="cell-left cell-Middle cell-border" Width="120px"/>
                                </asp:BoundField>
                                 <asp:BoundField HeaderText="OMA Support Point1" DataField="OMA_SUPPORT1" SortExpression="OMA_SUPPORT1">
                                    <HeaderStyle CssClass="cell-center" Width="120px"/>
                                    <ItemStyle CssClass="cell-left cell-Middle cell-border" Width="120px"/>
                                </asp:BoundField>
                                <asp:BoundField HeaderText="H2S Main Point" DataField="H2S" SortExpression="H2S">
                                    <HeaderStyle CssClass="cell-center" Width="100px"/>
                                    <ItemStyle CssClass="cell-left cell-Middle cell-border" Width="100px"/>
                                </asp:BoundField>
                                <asp:BoundField HeaderText="Hg Main Point" DataField="HG" SortExpression="HG">
                                    <HeaderStyle CssClass="cell-center" Width="100px"/>
                                    <ItemStyle CssClass="cell-left cell-Middle cell-border" Width="100px"/>
                                </asp:BoundField>
                                <asp:BoundField HeaderText="Remark" DataField="REMARK" SortExpression="REMARK">
                                    <HeaderStyle CssClass="cell-center" Width="250px"/>
                                    <ItemStyle CssClass="cell-left cell-Middle cell-border" Width="250px"/>
                                </asp:BoundField>

                                <asp:BoundField HeaderText="Created Date" DataField="CREATED_DATE" SortExpression="CREATED_DATE"  DataFormatString="{0:dd/MM/yyyy HH:mm}">
                                    <HeaderStyle CssClass="cell-center bg-gray-active" Width="100px"/>
                                    <ItemStyle CssClass="cell-left cell-Middle cell-border" Width="100px"/>
                                </asp:BoundField>
                                <asp:BoundField HeaderText="Updated Date" DataField="MODIFIED_DATE" SortExpression="MODIFIED_DATE"  DataFormatString="{0:dd/MM/yyyy HH:mm}">
                                    <HeaderStyle CssClass="cell-center bg-gray-active" Width="100px"/>
                                    <ItemStyle CssClass="cell-left cell-Middle cell-border" Width="100px"/>
                                </asp:BoundField>
                                  


                            </Columns>
                        </asp:GridView>


                    </div>
                    <!-- /.box-body -->


<div class=" box-footer">
    
</div>


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
    <!-- Page script -->
    <script>
        $(function () {
            //Initialize Select2 Elements
            $('.select2').select2()

        })
    </script>
                <!--#include file="../Includes/JSfooter.html" -->
</asp:Content>
