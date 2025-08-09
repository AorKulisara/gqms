<%@ Page Title="" Language="C#" MasterPageFile="~/Includes/MainPage.Master" AutoEventWireup="true" CodeBehind="UploadXLS.aspx.cs" Inherits="PTT.GQMS.USL.Web.Forms.UploadXLS" MaintainScrollPositionOnPostback="True" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

<script type="text/javascript">
    function DoAction(act, val) {
        var confirmed, isSubmitted, url, err;
        confirmed = true; isSubmitted = true; url = ""; err = "";
        switch (act) {
            case "IMPORT_XLS": isSubmitted = true; break;

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
            <li class="active">Upload excel (BTU)</li>
          </ol>
    </section>
    <!-- Main content -->
    <section class="content">
        <div class="row">

            <div class="col-xs-12">
                <div class='box box-danger'>
                    <div class="box-header">
                       <h3 class="box-title"><i class="fa fa-upload"></i>&nbsp;&nbsp;Upload excel (BTU data)</h3>
                    </div>
                    <div class="box-body">

                        <!-- left column -->
                        <div class="col-md-12">
                            <table border="0" class="text-black">
                                <tr>
                                    <td style="width: 150px; text-align: right; padding: 2px;">Select excel file&nbsp;:&nbsp;</td>
                                    <td style="width: 150px; text-align: left; padding: 2px;">
                                        <asp:FileUpload ID="FileImportData" AllowMultiple="true" runat="server" Width="650px" />

                                    </td>
                                </tr>
                                <tr><td></td><td>Note: The file name have to be the same as OGC Main Point</td></tr>

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

                    </div>
                    <!-- /.box-body -->



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
