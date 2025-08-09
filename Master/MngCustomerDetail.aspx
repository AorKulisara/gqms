<%@ Page Title="" Language="C#" MasterPageFile="~/Includes/MainPage.Master" AutoEventWireup="true" CodeBehind="MngCustomerDetail.aspx.cs" Inherits="PTT.GQMS.USL.Web.Master.MngCustomerDetail" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

        <!-- multiple Select2 -->
    <link href="../Content/select2.min.css" rel="stylesheet" />
    <script src="../Scripts/select2.full.min.js"></script>   

        <script type="text/javascript">
        var LastPage = "MngCustomer?UID=<%= HttpContext.Current.Session["UID"] %>";

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
 
            if (GetCtrlValue("<%=txtPERMANENT_CODE.ClientID %>") == "") { t_err += "* ID \r\n"; }
            if (GetCtrlValue("<%=txtNAME_ABBR.ClientID %>") == "") { t_err += "* Short Name \r\n"; }

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
    <asp:HiddenField ID="hidPERMANENT_CODE" runat="server" />


        <!-- Content Header (Page header) -->
    <section class="content-header">
         <ol class="breadcrumb">
            <li><i class="fa fa-home"></i></li>
            <li>Master data</li>

            <li onclick="javascript:GoMenu('MngSiteOFF');" style="cursor:pointer" >Customer data (GIS)</li>
            <li class="active">Customer detail</li>
        </ol>
    </section>

    <!-- Main content -->
    <section class="content">
        <div class="row">

            <div class="col-xs-12">

                <div class='box box-violet box-violet'>
                    <div class="box-header">
                        <h3 class="box-title">Customer detail</h3>
                    </div>
                    <div class="box-body ">

                    <div class="col-md-6">
                        <table border="0" class="text-black">
                            <tr>
                                <td style="text-align: right; height: 30px; width: 180px;">ID<span class="RequireField">*</span>:&nbsp;&nbsp;</td>
                                <td style="text-align: left; height: 30px;" >
                                    <asp:TextBox ID="txtPERMANENT_CODE" MaxLength="10" CssClass="form-control" runat="server" Width="200px" Text=""></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td style="text-align: right; height: 30px; width: 180px;">Short Name<span class="RequireField">*</span>:&nbsp;&nbsp;</td>
                                <td style="text-align: left; height: 30px;" >
                                    <asp:TextBox ID="txtNAME_ABBR" MaxLength="100" CssClass="form-control" runat="server" Width="400px" Text=""></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td style="text-align: right; height: 30px; width: 180px;">Customer Name :&nbsp;&nbsp;</td>
                                <td style="text-align: left; height: 30px;" >
                                    <asp:TextBox ID="txtNAME_FULL" MaxLength="1000" CssClass="form-control" runat="server" Width="400px" Text=""></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td style="text-align: right; height: 30px; width: 180px;">Customer Type :&nbsp;&nbsp;</td>
                                <td style="text-align: left; height: 30px;" >
                                    <asp:TextBox ID="txtSUB_TYPE" MaxLength="100" CssClass="form-control" runat="server" Width="400px" Text=""></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td style="text-align: right; height: 30px; width: 180px;">Region :&nbsp;&nbsp;</td>
                                <td style="text-align: left; height: 30px;" >
                                    <asp:TextBox ID="txtREGION" MaxLength="100" CssClass="form-control" runat="server" Width="400px" Text=""></asp:TextBox>
                                </td>
                            </tr>
                             <tr>
                                <td style="text-align: right; height: 30px; width: 180px;">BV Zoning :&nbsp;&nbsp;</td>
                                <td style="text-align: left; height: 30px;" >
                                    <asp:TextBox ID="txtBV_ZONE" MaxLength="100" CssClass="form-control" runat="server" Width="400px" Text=""></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td style="text-align: right; height: 30px; width: 180px;">Block Valve :&nbsp;&nbsp;</td>
                                <td style="text-align: left; height: 30px;" >
                                    <asp:TextBox ID="txtBV_VALVE" MaxLength="100" CssClass="form-control" runat="server" Width="400px" Text=""></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td style="text-align: right; height: 30px; width: 180px;">Status :&nbsp;&nbsp;</td>
                                <td style="text-align: left; height: 30px;" >
                                    <asp:TextBox ID="txtSTATUS_CL" MaxLength="100" CssClass="form-control" runat="server" Width="400px" Text=""></asp:TextBox>
                                </td>
                            </tr>
                        </table>
                    </div>
                     <div class="col-md-6">
                        <table border="0" class="text-black">
                                <tr>
                                    <td style="text-align: right; height: 30px; width: 180px;">OGC Main Point :&nbsp;&nbsp;</td>
                                    <td style="text-align: left; height: 30px;" >
                                         <asp:DropDownList ID="ddlQUALITY_MAIN" Width="400px" CssClass="form-control select2" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlQUALITY_MAIN_SelectedIndexChanged">
                                        </asp:DropDownList>
                                   </td>
                                </tr>
                            <tr>
                                    <td style="text-align: right; height: 30px; width: 180px;">OGC Support Point1 :&nbsp;&nbsp;</td>
                                    <td style="text-align: left; height: 30px;" >
                                        <asp:DropDownList ID="ddlQUALITY_SUPPORT1" Width="400px" CssClass="form-control select2" runat="server">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                            <tr>
                                    <td style="text-align: right; height: 30px; width: 180px;">OGC Support Point2 :&nbsp;&nbsp;</td>
                                    <td style="text-align: left; height: 30px;" >
                                        <asp:DropDownList ID="ddlQUALITY_SUPPORT2" Width="400px" CssClass="form-control select2" runat="server">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                            <tr>
                                    <td style="text-align: right; height: 30px; width: 180px;">OMA Main Point :&nbsp;&nbsp;</td>
                                    <td style="text-align: left; height: 30px;" >
                                        <asp:DropDownList ID="ddlOMA_MAIN" Width="400px" CssClass="form-control select2" runat="server">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                            <tr>
                                    <td style="text-align: right; height: 30px; width: 180px;">OMA Support Point1 :&nbsp;&nbsp;</td>
                                    <td style="text-align: left; height: 30px;" >
                                        <asp:DropDownList ID="ddlOMA_SUPPORT1" Width="400px" CssClass="form-control select2" runat="server">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                            <tr>
                                    <td style="text-align: right; height: 30px; width: 180px;">H2S Main Point :&nbsp;&nbsp;</td>
                                    <td style="text-align: left; height: 30px;" >
                                        <asp:DropDownList ID="ddlH2S" Width="400px" CssClass="form-control select2" runat="server">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                            <tr>
                                    <td style="text-align: right; height: 30px; width: 180px;">Hg Main Point :&nbsp;&nbsp;</td>
                                    <td style="text-align: left; height: 30px;" >
                                        <asp:DropDownList ID="ddlHG" Width="400px" CssClass="form-control select2" runat="server">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                            <tr>
                                    <td style="text-align: right; vertical-align:top; width: 180px;">Remark :&nbsp;&nbsp;</td>
                                    <td style="text-align: left;" >
                                        <asp:TextBox ID="txtREMARK" MaxLength="4000" CssClass="form-control" runat="server" Width="400px" Text="" TextMode="MultiLine" Rows="2" Height="45"></asp:TextBox>
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


</div>
</div>
<!---------------------------------------------------------->
<!---------------------------------------------------------->

        <div class="box-footer">
            <table border="0" style="margin-left: 35%;">
                <tr>
                    <td style="width: 100px">
                        <asp:Panel ID="pnlSAVE" runat="server">
                            <input name="btnSave" class="btn btn-block btn-sametogetherproject" type="button" id="btnSave" value="Save" onclick="javascript: DoAction('SAVE','');" style="width: 80px" modal-confirm />
                        </asp:Panel>
                    </td>
                        <td style="width: 100px">
                        <asp:Panel ID="pnlDELETE" runat="server" Visible="false">
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
    <!-- Page script -->
<script>
    $(function () {
        //Initialize Select2 Elements
        $('.select2').select2()

    })
</script>

<!--#include file="../Includes/JSfooter.html" -->
</asp:Content>
