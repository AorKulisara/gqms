<%@ Page Title="" Language="C#" MasterPageFile="~/Includes/MainPage.Master" AutoEventWireup="true" CodeBehind="MngMailTemplate.aspx.cs" Inherits="PTT.GQMS.USL.Web.Master.MngMailTemplate" ValidateRequest="false" MaintainScrollPositionOnPostback="True" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

    <script type="text/javascript">
        var LastPage = "";
        function DoAction(act, val) {
            var confirmed, isSubmitted, url, err;
            confirmed = true; isSubmitted = true; url = ""; err = "";

            switch (act) {
                case "SAVE": isSubmitted = true; err = CheckData(); break;
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

            if (GetCtrlValue("<%=txtSUBJECT.ClientID%>") == "") { t_err += "* Subject \r\n"; }


            var valEmailTo = document.getElementById("<%=txtMAIL_TO.ClientID%>").value;
            if (valEmailTo != "") {
                if (!validateEmail(valEmailTo)) {
                    t_err += "* The input is not a valid email address";
                }
            }
            var valEmailCC = document.getElementById("<%=txtMAIL_CC.ClientID%>").value;
            if (valEmailCC != "") {
                if (!validateEmail(valEmailCC)) {
                    t_err += "* The input is not a valid email CC address";
                }
            }

            var valEmailBCC = document.getElementById("<%=txtMAIL_BCC.ClientID%>").value;
            if (valEmailBCC != "") {
                if (!validateEmail(valEmailBCC)) {
                    t_err += "* The input is not a valid email BCC address";
                }
            }

            if (t_err != "") { t_err = "Require data !!!\r\n" + t_err; }
            return t_err;
        }

        function validateEmail(sEmail) {
            var filter = /^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$/;
            var valEmail = sEmail.split(/[;,]+/);
            for (var i in valEmail) {
                var email = valEmail[i];
                if (filter.test(email)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }

    </script>
    <script>
        $(document).ready(function () {
            $('input').iCheck({
                checkboxClass: 'icheckbox_square-blue',
                radioClass: 'iradio_square-blue',
                increaseArea: '20%' // optional
            });
        });
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HeadStyle" runat="server">
    <style>
        #success_message {
            display: none;
        }

        .TextColor {
            color: black;
        }
    </style>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <input type="hidden" id="ServerAction" name="ServerAction" />
    <asp:HiddenField ID="hidETEMPLATE_ID" runat="server" Value="1" />

    <!-- Content Header (Page header) -->
    <section class="content-header">

        <ol class="breadcrumb">
            <li><i class="fa fa-home"></i></li>
            <li>Master data</li>
            <li class="active">Mail template</li>
        </ol>
    </section>
    <!-- Main content -->
    <section class="content">
        <div class="row">
            <div class="col-xs-12">

                <div class='box box-violet'>
                    <div class="box-header">
                        <asp:Label ID="lblCaption" class="box-title" runat="server" Text="Mail template" Font-Size="16px"></asp:Label>
                    </div>
                    <div class="box-body">

                        <!-- right column -->
                        <div class="col-xs-12">

                            <table border="0" class="text-black"  style="padding: 1px">
 
                                <tr>
                                    <td style="text-align: right; height: 30px; width: 110px;">Alert Email&nbsp;:&nbsp;</td>
                                    <td style="text-align: left; height: 30px;">
                                        <asp:DropDownList ID="ddlALERT_EMAIL" runat="server" Width="400" AutoPostBack="True" OnSelectedIndexChanged="ddlALERT_EMAIL_SelectedIndexChanged">
                                            <asp:ListItem Text="Notice of incomplete data" Value="1"></asp:ListItem>
                                            <asp:ListItem Text="Notice of calibration data (OGC Data System)" Value="2"></asp:ListItem>
                                            <asp:ListItem Text="Notice of change customer data" Value="3"></asp:ListItem>
                                        </asp:DropDownList>
                                        
                                    </td>
                                </tr>
                                <tr>
                                    <td style="height: 30px; text-align: right;">Status&nbsp;:&nbsp;</td>
                                    <td style="text-align: left; height: 30px" colspan="3">
                                        <asp:RadioButtonList ID="rdbDisabled" runat="server" RepeatDirection="Horizontal" Font-Size="15px" CssClass="TextColor">
                                            <asp:ListItem Selected="True" Value="Y">&nbsp;Enable&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</asp:ListItem>
                                            <asp:ListItem Value="N">&nbsp;Disable</asp:ListItem>
                                        </asp:RadioButtonList>
                                    </td>
                                </tr>

                                <tr>
                                    <td style="height: 30px; text-align: right; width: 110px;">To &nbsp;:&nbsp;</td>
                                    <td style="text-align: left; height: 30px; width:400px;" colspan="3">
                                        <asp:TextBox ID="txtMAIL_TO" MaxLength="500" CssClass="form-control" runat="server"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="height: 30px; text-align: right;">CC &nbsp;:&nbsp;</td>
                                    <td style="text-align: left; height: 30px" colspan="3">
                                        <asp:TextBox ID="txtMAIL_CC" MaxLength="500" CssClass="form-control" runat="server"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="height: 30px; text-align: right;">BCC &nbsp;:&nbsp;</td>
                                    <td style="text-align: left; height: 30px" colspan="3">
                                        <asp:TextBox ID="txtMAIL_BCC" MaxLength="500" CssClass="form-control" runat="server"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="height: 30px; text-align: right;">Subject<span class="RequireField">*</span>:&nbsp;</td>
                                    <td style="text-align: left; height: 30px" colspan="3">
                                        <asp:TextBox ID="txtSUBJECT" MaxLength="500" CssClass="form-control" runat="server"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="height: 10px; text-align: right;"></td>
                                    <td style="text-align: left; height: 10px" colspan="3">
                                    </td>
                                </tr>
                                 <tr>
                                    <td style="height: 30px; text-align: right; vertical-align:top;">Message&nbsp;:&nbsp;</td>
                                    <td style="text-align: left;" colspan="3">
                                         <textarea id="editor1" name="editor1" rows="100" cols="100" runat="server"></textarea>
                                    </td>
                                </tr>

                            </table>



                        </div>

                        <div class="col-xs-12">
                           <table style="color: #000000">
                                <tr>
                                    <td colspan="7"><b>Parameter:</b></td>
                                </tr>
                                <tr>
                                    <td>{TABLE_INVALID_UNNORM}</td>
                                    <td style="width: 20px;text-align:center">=</td>
                                    <td>Tracking Table1</td>
                                    <td style="width: 20px">&nbsp;</td>
                                    <td>{TABLE_INVALID_TRANSFER}</td>
                                    <td style="width: 20px;text-align:center">=</td>
                                    <td>Tracking Table2</td>
                                </tr>
                                 <tr>
                                    <td>{TABLE_CALIBRATED}</td>
                                    <td style="width: 20px;text-align:center">=</td>
                                    <td>Tracking Site Calibrated Table</td>
                                    <td style="width: 20px">&nbsp;</td>
                                    <td>{TABLE_NOT_CALIBRATED}</td>
                                    <td style="width: 20px;text-align:center">=</td>
                                    <td>Tracking Site Not Calibrated Table</td>
                                </tr>
                                 <tr>
                                    <td>{TABLE_CHANGED_CUSTOMER}</td>
                                    <td style="width: 20px;text-align:center">=</td>
                                    <td>Tracking Changed Customer Data Table</td>
                                    <td style="width: 20px">&nbsp;</td>
                                    <td>{LINK_URL}</td>
                                    <td style="width: 20px;text-align:center">=</td>
                                    <td>Link to web page</td>
                                </tr>

                            </table>

                        </div>




                        <div class="col-xs-12 ">
                            <br />
                            <asp:Label ID="lblLastUpdated" runat="server" CssClass="lblDisplay"></asp:Label>
                        </div>

                    </div>
                    <div class=" box-footer">

                        <div class="col-xs-12">
                            <table style="margin-left: 30%;">
                                <tr>
                                    <td style="width: 100px">
                                        <asp:Panel ID="pnlSAVE" runat="server">
                                            <input name="btnSave" class="btn btn-block btn-sametogetherproject" type="button" id="btnSave" value="Save" onclick="javascript: DoAction('SAVE', '');" style="width: 80px" modal-confirm />
                                        </asp:Panel>
                                    </td>
                                </tr>
                            </table>
                        </div>


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
    <!-- CK Editor -->
    <script src="../ckeditor/ckeditor.js"></script>
    <script>
        $(function () {
            // Replace the <textarea id="editor1"> with a CKEditor
            // instance, using default configuration.
            CKEDITOR.replace('<%=editor1.ClientID %>');
            CKEDITOR.config.height = 300;
        });
    </script>

    <!--#include file="../Includes/JSfooter.html" -->
</asp:Content>
