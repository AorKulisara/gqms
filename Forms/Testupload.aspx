<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Testupload.aspx.cs" Inherits="PTT.OGC.USL.Web.Testupload" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">

    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <meta http-equiv="X-UA-Compatitble" content="IE=edge,Chrome=1" />
    <title>PTT OGC</title>
    <meta content='width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no' name='viewport' />
    <!-- Font Awesome Icons -->
    <link href="../Content/font-awesome.min.css" rel="stylesheet" type="text/css" />

    <!-- jQuery 1.11.3 and 2.1.4 -->
    <script src="../Scripts/jquery.min.js" type="text/javascript"></script>
    <!--<script src="../Scripts/jQuery/jQuery-2.1.4.min.js" type="text/javascript"></script>-->
    <!-- Bootstrap 3.3.2 -->
    <link href="../Content/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/bootstrap.min.js" type="text/javascript"></script>
    <!-- AdminLTE Template -->
    <link href="../Content/AdminLTE.min.css" rel="stylesheet" type="text/css" />
    <link href="../Content/custom-style.min.css" rel="stylesheet" type="text/css" />
    <script src="../Scripts/app.min.js" type="text/javascript"></script>

    <!-- MyTheme & Script-->
    <link href="../Content/appStyle.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="../Scripts/Utility.js"></script>
    <script type="text/javascript" src="../Scripts/Project.js"></script>
    <link href="../Content/AutoCompleteTheme.css" rel="stylesheet" type="text/css" />
    <!-- Icon ระบบ -->
    <link rel="shortcut icon" href="../favicon.ico" type="image/x-icon" />

    <!--Jquery UI & Theme-->
    <script type="text/javascript" src="../Scripts/jquery-ui.min.js"></script>
    <link href="../Content/jquery-ui.min.css" rel="stylesheet" type="text/css" />

    <!--Datepicker Bootstrap-->
    <link href="../Content/datepicker.css" rel="stylesheet" />
    <script src="../Scripts/bootstrap-datepicker.js" type="text/javascript"></script>
    <script src="../Scripts/bootstrap-datepicker-thai.js" type="text/javascript"></script>
    <script src="../Scripts/bootstrap-datepicker.th.js" type="text/javascript"></script>


    <!-- iCheck -->
    <link href="../Content/icheck_square/_all.css" rel="stylesheet"/>
    <script src="../Scripts/icheck.js" type="text/javascript"></script>


    <script type="text/javascript">
    function DoAction(act, val) {
        var confirmed, isSubmitted, url, err;
        confirmed = true; isSubmitted = true; url = ""; err = "";
        switch (act) {
            case "IMPORT_XLS":  isSubmitted = true; break;

        }

        if (err != "") {
            AlertModal(err);
        } else if (url != "") {
            window.open(url, target = val );
        } else if (confirmed && isSubmitted) {
            SetCtrlValue("ServerAction", act);
            form1.submit();
            
           // LoadSpin();
          //  SubmitForm();
        }
    }

</script>


</head>
<body>
    <form id="form1" runat="server">
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
    </form>
</body>
</html>
