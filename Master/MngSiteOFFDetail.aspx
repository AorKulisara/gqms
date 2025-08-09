<%@ Page Title="" Language="C#" MasterPageFile="~/Includes/MainPage.Master" AutoEventWireup="true" CodeBehind="MngSiteOFFDetail.aspx.cs" Inherits="PTT.GQMS.USL.Web.Master.MngSiteOFFDetail" MaintainScrollPositionOnPostback="True" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

        <script type="text/javascript">
        var LastPage = "MngSiteOFF?UID=<%= HttpContext.Current.Session["UID"] %>";

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
 
            if (GetCtrlValue("<%=txtFID.ClientID %>") == "") { t_err += "* OGC Main Point \r\n"; }
            if (GetCtrlValue("<%=txtSITE_NAME.ClientID %>") == "") { t_err += "* Site description \r\n"; }

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
    <asp:HiddenField ID="hidSITE_ID" runat="server" />


        <!-- Content Header (Page header) -->
    <section class="content-header">
         <ol class="breadcrumb">
            <li><i class="fa fa-home"></i></li>
            <li>Master data</li>

            <li onclick="javascript:GoMenu('MngSiteOFF');" style="cursor:pointer" >Offshore Site management</li>
            <li class="active">Site detail</li>
        </ol>
    </section>

    <!-- Main content -->
    <section class="content">
        <div class="row">

            <div class="col-xs-12">

                <div class='box box-violet box-violet'>
                    <div class="box-header">
                        <h3 class="box-title">Site detail</h3>
                    </div>
                    <div class="box-body ">

                    <div class="col-md-6">
                        <table border="0" class="text-black">
                            <tr>
                                <td style="text-align: right; height: 30px; width: 150px;">OGC Main Point<span class="RequireField">*</span>:&nbsp;&nbsp;</td>
                                <td style="text-align: left; height: 30px;" >
                                    <asp:TextBox ID="txtFID" MaxLength="100" CssClass="form-control" runat="server" Width="400px" Text=""></asp:TextBox>
                                </td>
                            </tr>
                            </table>
                    </div>
                     <div class="col-md-6">
                        <table border="0" class="text-black">

                            <tr>
                                    <td style="text-align: right; height: 30px; width: 150px;">Site description<span class="RequireField">*</span>:&nbsp;&nbsp;</td>
                                    <td style="text-align: left; height: 30px;" >
                                        <asp:TextBox ID="txtSITE_NAME" MaxLength="200" CssClass="form-control" runat="server" Width="400px" Text=""></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                    </div>

                  <div class="col-md-6">
                        <table border="0" class="text-black">
                             <tr>
                                 <%-- Edit by Turk 11/04/2562 --> Company --%>
                                 <td style="text-align: right; height: 30px; width: 150px;">Company&nbsp;:&nbsp;</td>
                                <td style="text-align: left; height: 30px;" >
                                    <asp:TextBox ID="txtCOMPANY" MaxLength="100" CssClass="form-control" runat="server" Width="400px" Text=""></asp:TextBox>
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
