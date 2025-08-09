<%@ Page Title="" Language="C#" MasterPageFile="~/Includes/MainPage.Master" AutoEventWireup="true" CodeBehind="MngUserDetail.aspx.cs" Inherits="PTT.GQMS.USL.Web.Settings.MngUserDetail" ValidateRequest="False" MaintainScrollPositionOnPostback="True" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript">
        var LastPage = "MngUserList?UID=<%= HttpContext.Current.Session["UID"] %>";
        function DoAction(ServerAction) {
            var confirmed, isSubmitted, url, err;

            confirmed = true; isSubmitted = true; url = ""; err = "";

            switch (ServerAction) {
                case "BACK": isSubmitted = false; url = LastPage; break;
                case "SAVE": isSubmitted = true; err = CheckData(); break;
                case "DELETE": confirmed = ConfirmModal("Confirm to delete data?"); break;
                case "UPLOAD": break;

                case "DELETE_IMG": confirmed = ConfirmModal("Confirm to delete signature?"); break;
            }

            if (err != "") {
                AlertModal(err);
            } else if (url != "") {
                LoadSpin();
                window.location.href = url;
            } else if (isSubmitted && confirmed) {
                SetCtrlValue("U", EncodeKey('Username', GetCtrlValue("<%=txtUserName.ClientID %>")));
                SetCtrlValue("ServerAction", ServerAction);
                SubmitForm();
            }
        }
        function CheckData() { 
            var t_err = "";
            if (GetCtrlValue("<%=txtUserName.ClientID %>") == "") { t_err += "* Username \r\n"; }
                else {
                    if (HaveSpecChar(GetCtrlValue("<%=txtUserName.ClientID %>"))) { t_err += "* username: Only numbers and characters (including capital) are allowed.\r\n   No special characteres are allowed\r\n"; }

                }

                if (GetCtrlValue("<%=txtUserDesc.ClientID %>") == "") { t_err += "* Full name \r\n"; }

                var valEmail = document.getElementById("<%=txtEmail.ClientID%>").value;
                if (valEmail != "") {
                    if (!validateEmail(valEmail)) {
                        t_err += "* The input is not a valid email address";
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

    <style>
        .btn-file {
            position: relative;
            overflow: hidden;
        }

            .btn-file input[type=file] {
                position: absolute;
                top: 0;
                right: 0;
                min-width: 100%;
                min-height: 100%;
                font-size: 100px;
                text-align: right;
                filter: alpha(opacity=0);
                opacity: 0;
                outline: none;
                background: white;
                cursor: inherit;
                display: block;
            }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HeadStyle" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <input type="hidden" id="ServerAction" name="ServerAction" />
    <input type="hidden" id="U" name="U" />
    <input type="hidden" id="P" name="P" />
    <input type="hidden" id="OP" name="OP" value="<%=OP%>" />
    <asp:HiddenField ID="hidSIGN_FILENAME" runat="server" />


    <!-- Content Header (Page header) -->
    <section class="content-header">

        <ol class="breadcrumb">
            <li><i class="fa fa-home"></i></li>
            <li>Security</li>
            <li onclick="javascript:GoMenu('MngUserList');" style="cursor: pointer">User management</li>
            <li class="active">User detail</li>
        </ol>
    </section>
    <!-- Main content -->
    <section class="content">
        <div class="row">

            <div class="col-xs-12">

                <div class='box box-primary-light'>
                    <div class="box-header">
                        <h3 class="box-title">User Detail</h3>
                    </div>
                    <div class="box-body ">

                        <!-- right column -->
                        <div class="col-xs-12">
                            <table border="0" class="text-black" id="tableuser">
                                <tr>
                                    <td colspan="4" style="height: 10px;"></td>
                                </tr>
                                <tr>
                                    <td style="text-align: right; height: 30px; width: 250px;">Username (Employee ID)<span class="RequireField">*</span>&nbsp;:&nbsp;</td>
                                    <td style="text-align: left; height: 30px;">
                                        <asp:TextBox ID="txtUserName" CssClass="form-control" ReadOnly="True" runat="server" Width="100px" MaxLength="10"></asp:TextBox>
                                    </td>
                                    <td style="text-align: center; height: 30px; width: 480px;" colspan="2">
                                         &nbsp;
                                    </td>
                                </tr>
                                <tr>
                                    <td style="height: 30px; text-align: right;">Full name&nbsp;<span class="RequireField">*</span>&nbsp;:&nbsp;</td>
                                    <td style="text-align: left; height: 30px" colspan="3">
                                        <asp:TextBox ID="txtUserDesc" CssClass="form-control" runat="server" Width="300px" MaxLength="200"></asp:TextBox></td>
                                </tr>

                                <tr>
                                    <td style="height: 30px; text-align: right;">Position&nbsp;:&nbsp;</td>
                                    <td style="text-align: left; height: 30px" colspan="3">
                                        <asp:TextBox ID="txtPosition" CssClass="form-control" runat="server" Width="300px" MaxLength="200"></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <td style="height: 30px; text-align: right;">Department&nbsp;:&nbsp;</td>
                                    <td style="text-align: left; height: 30px" colspan="3">
                                        <asp:TextBox ID="txtUnit" CssClass="form-control" runat="server" Width="300px" MaxLength="350"></asp:TextBox></td>
                                </tr>
                                <tr>
                                    <td style="height: 30px; text-align: right;">E-Mail&nbsp;:&nbsp;</td>
                                    <td style="height: 30px; text-align: left;" colspan="3">
                                        <asp:TextBox ID="txtEmail" CssClass="form-control" runat="server" Width="300px" MaxLength="250"></asp:TextBox></td>
                                </tr>
                                  <tr>
                                    <td style="height: 30px; text-align: right;">Role name&nbsp;:&nbsp;</td>
                                    <td style="text-align: left; height: 30px" colspan="3">
                                        <asp:DropDownList ID="ddlRole" CssClass="form-control" Width="200px" runat="server"></asp:DropDownList></td>
                                </tr>
                                <tr>
                                    <td style="text-align: right; height: 30px; width: 200px;">Signature&nbsp;:&nbsp;&nbsp;</td>
                                    <td style="text-align: left; height: 30px;">
                                        <asp:FileUpload ID="flUpload" runat="server" Width="300px" />  
                                    </td>
                                    <td style="width: 20px;">
                                        </td>
                                    <td>

                                    </td>
                                </tr>
                                <tr><td></td>
                                    <td colspan="3">
                                        (Maximum file size 1 MB | File format:.jpg  .png)                                        
                                    </td>
                                </tr>
                                <tr><td></td>
                                    <td colspan="3">
                                        <asp:Image ID="ImgAvatar" runat="server" BorderStyle="Solid" BorderWidth="1" />                                        
                                        <div id="divImg" runat="server" visible ="false"  class="btn-group btn-group-sm" style="vertical-align:bottom;">
                                            <li class="fa fa-times fa-2x" style="color: #993333" onclick="DoAction('DELETE_IMG')" modal-confirm ></li>
                                        </div>
                                    </td>
                                </tr>

                              
                    
                                <tr>
                                    <td style="height: 15px;" colspan="4"></td>
                                </tr>
                                <tr>
                                    <td style="height: 30px; text-align: right;">Status&nbsp;:&nbsp;</td>
                                    <td style="text-align: left;" colspan="3">
                                        <asp:RadioButtonList ID="rdbDisabled" runat="server" RepeatDirection="Horizontal" class="radio">
                                            <asp:ListItem Selected="True" Value="N">Enable&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</asp:ListItem>
                                            <asp:ListItem Value="Y">Disable</asp:ListItem>
                                        </asp:RadioButtonList></td>
                                </tr>

                            </table>
                            <style>
                                input[type=radio] {
                                    margin-left: 1px !important;
                                }

                                #tableuser td:nth-child(1) {
                                    padding-right: 3px;
                                }
                            </style>
                        </div>

                        <div class="col-xs-12 " style="height: 35px">
                            <asp:Label ID="lblLastUpdated" runat="server" CssClass="lblDisplay"></asp:Label>

                        </div>

                    </div>
                    <div class="box-footer">
                        <table style="margin-left: 30%;">
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
