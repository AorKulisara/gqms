<%@ Page Title="" Language="C#" MasterPageFile="~/Includes/MainPage.Master" AutoEventWireup="true" CodeBehind="MngSiteDetail.aspx.cs" Inherits="PTT.GQMS.USL.Web.Master.MngSiteDetail" MaintainScrollPositionOnPostback="True" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <!-- multiple Select2 -->
    <link href="../Content/select2.min.css" rel="stylesheet" />
    <script src="../Scripts/select2.full.min.js"></script>   

       <!-- datatable popup search -------->
    <link rel="stylesheet" type="text/css" href="../Scripts/DataTables/css/jquery.dataTables.min.css" />
    <script type="text/javascript" src="../Scripts/DataTables/js/jquery.dataTables.min.js"></script>


        <script type="text/javascript">
        var LastPage = "MngSite?UID=<%= HttpContext.Current.Session["UID"] %>";

        function DoAction(act, val) {
            var confirmed, isSubmitted, url, err;
            confirmed = true; isSubmitted = true; url = ""; err = "";

            switch (act) {
                case "BACK": isSubmitted = false; url = LastPage; break;
                case "SAVE": isSubmitted = true; err = CheckData(); break;
                case "DELETE": confirmed = ConfirmModal("Confirm to delete data?"); break;
                case "VIEW_STD": break;
                case "DELETE_STD": break;
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


      function Calc_SUM()
        {
          var summ = 0;
          if (GetCtrl("<%=txtSUM.ClientID %>") )
          {
              if (GetCtrlValue("<%=txtC1.ClientID %>") != "")  summ = summ + To_Num(GetCtrlValue("<%=txtC1.ClientID %>"));
              if (GetCtrlValue("<%=txtC2.ClientID %>") != "")  summ = summ + To_Num(GetCtrlValue("<%=txtC2.ClientID %>"));
              if (GetCtrlValue("<%=txtC3.ClientID %>") != "")  summ = summ + To_Num(GetCtrlValue("<%=txtC3.ClientID %>"));
              if (GetCtrlValue("<%=txtIC4.ClientID %>") != "") summ = summ + To_Num(GetCtrlValue("<%=txtIC4.ClientID %>"));
              if (GetCtrlValue("<%=txtNC4.ClientID %>") != "")  summ = summ + To_Num(GetCtrlValue("<%=txtNC4.ClientID %>"));
              if (GetCtrlValue("<%=txtIC5.ClientID %>") != "") summ = summ + To_Num(GetCtrlValue("<%=txtIC5.ClientID %>"));
              if (GetCtrlValue("<%=txtNC5.ClientID %>") != "") summ = summ + To_Num(GetCtrlValue("<%=txtNC5.ClientID %>"));
              if (GetCtrlValue("<%=txtC6.ClientID %>") != "") summ = summ + To_Num(GetCtrlValue("<%=txtC6.ClientID %>"));
              if (GetCtrlValue("<%=txtCO2.ClientID %>") != "") summ = summ + To_Num(GetCtrlValue("<%=txtCO2.ClientID %>"));
              if (GetCtrlValue("<%=txtN2.ClientID %>") != "") summ = summ + To_Num(GetCtrlValue("<%=txtN2.ClientID %>"));
              //-- EDIT 20/06/2019 summary ไม่รวม h2s, hg
              //if (GetCtrlValue("<%=txtH2S.ClientID %>") != "") summ = summ + To_Num(GetCtrlValue("<%=txtH2S.ClientID %>"));
              //if (GetCtrlValue("<%=txtHG.ClientID %>") != "")  summ = summ + To_Num(GetCtrlValue("<%=txtHG.ClientID %>"));
              summ = FormatNum(summ + "", 4);
              SetCtrlValue("<%=txtSUM.ClientID %>", summ);

          }
        } 

         //-- EDIT 20/06/2019 
         function Calc_SUMMIN()
        {
          var summ = 0;
          if (GetCtrl("<%=txtSUM_MIN.ClientID %>") )
          {
              if (GetCtrlValue("<%=txtC1_MIN.ClientID %>") != "")  summ = summ + To_Num(GetCtrlValue("<%=txtC1_MIN.ClientID %>"));
              if (GetCtrlValue("<%=txtC2_MIN.ClientID %>") != "")  summ = summ + To_Num(GetCtrlValue("<%=txtC2_MIN.ClientID %>"));
              if (GetCtrlValue("<%=txtC3_MIN.ClientID %>") != "")  summ = summ + To_Num(GetCtrlValue("<%=txtC3_MIN.ClientID %>"));
              if (GetCtrlValue("<%=txtIC4_MIN.ClientID %>") != "") summ = summ + To_Num(GetCtrlValue("<%=txtIC4_MIN.ClientID %>"));
              if (GetCtrlValue("<%=txtNC4_MIN.ClientID %>") != "")  summ = summ + To_Num(GetCtrlValue("<%=txtNC4_MIN.ClientID %>"));
              if (GetCtrlValue("<%=txtIC5_MIN.ClientID %>") != "") summ = summ + To_Num(GetCtrlValue("<%=txtIC5_MIN.ClientID %>"));
              if (GetCtrlValue("<%=txtNC5_MIN.ClientID %>") != "") summ = summ + To_Num(GetCtrlValue("<%=txtNC5_MIN.ClientID %>"));
              if (GetCtrlValue("<%=txtC6_MIN.ClientID %>") != "") summ = summ + To_Num(GetCtrlValue("<%=txtC6_MIN.ClientID %>"));
              if (GetCtrlValue("<%=txtCO2_MIN.ClientID %>") != "") summ = summ + To_Num(GetCtrlValue("<%=txtCO2_MIN.ClientID %>"));
              if (GetCtrlValue("<%=txtN2_MIN.ClientID %>") != "") summ = summ + To_Num(GetCtrlValue("<%=txtN2_MIN.ClientID %>"));
              //-- EDIT 20/06/2019 summary ไม่รวม h2s, hg
              //if (GetCtrlValue("<%=txtH2S_MIN.ClientID %>") != "") summ = summ + To_Num(GetCtrlValue("<%=txtH2S_MIN.ClientID %>"));
              //if (GetCtrlValue("<%=txtHG_MIN.ClientID %>") != "")  summ = summ + To_Num(GetCtrlValue("<%=txtHG_MIN.ClientID %>"));
              summ = FormatNum(summ + "", 4);
              SetCtrlValue("<%=txtSUM_MIN.ClientID %>", summ);

          }
            } 


               //-- EDIT 20/06/2019 
         function Calc_SUMMAX()
        {
          var summ = 0;
          if (GetCtrl("<%=txtSUM_MAX.ClientID %>") )
          {
              if (GetCtrlValue("<%=txtC1_MAX.ClientID %>") != "")  summ = summ + To_Num(GetCtrlValue("<%=txtC1_MAX.ClientID %>"));
              if (GetCtrlValue("<%=txtC2_MAX.ClientID %>") != "")  summ = summ + To_Num(GetCtrlValue("<%=txtC2_MAX.ClientID %>"));
              if (GetCtrlValue("<%=txtC3_MAX.ClientID %>") != "")  summ = summ + To_Num(GetCtrlValue("<%=txtC3_MAX.ClientID %>"));
              if (GetCtrlValue("<%=txtIC4_MAX.ClientID %>") != "") summ = summ + To_Num(GetCtrlValue("<%=txtIC4_MAX.ClientID %>"));
              if (GetCtrlValue("<%=txtNC4_MAX.ClientID %>") != "")  summ = summ + To_Num(GetCtrlValue("<%=txtNC4_MAX.ClientID %>"));
              if (GetCtrlValue("<%=txtIC5_MAX.ClientID %>") != "") summ = summ + To_Num(GetCtrlValue("<%=txtIC5_MAX.ClientID %>"));
              if (GetCtrlValue("<%=txtNC5_MAX.ClientID %>") != "") summ = summ + To_Num(GetCtrlValue("<%=txtNC5_MAX.ClientID %>"));
              if (GetCtrlValue("<%=txtC6_MAX.ClientID %>") != "") summ = summ + To_Num(GetCtrlValue("<%=txtC6_MAX.ClientID %>"));
              if (GetCtrlValue("<%=txtCO2_MAX.ClientID %>") != "") summ = summ + To_Num(GetCtrlValue("<%=txtCO2_MAX.ClientID %>"));
              if (GetCtrlValue("<%=txtN2_MAX.ClientID %>") != "") summ = summ + To_Num(GetCtrlValue("<%=txtN2_MAX.ClientID %>"));
              //-- EDIT 20/06/2019 summary ไม่รวม h2s, hg
              //if (GetCtrlValue("<%=txtH2S_MAX.ClientID %>") != "") summ = summ + To_Num(GetCtrlValue("<%=txtH2S_MAX.ClientID %>"));
              //if (GetCtrlValue("<%=txtHG_MAX.ClientID %>") != "")  summ = summ + To_Num(GetCtrlValue("<%=txtHG_MAX.ClientID %>"));
              summ = FormatNum(summ + "", 4);
              SetCtrlValue("<%=txtSUM_MAX.ClientID %>", summ);

          }
        } 
           

        //-------------------------------------------------------------------------

    </script>


</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HeadStyle" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <input type="hidden" id="ServerAction" name="ServerAction" />
    <asp:HiddenField ID="hidSITE_ID" runat="server" />
    <asp:HiddenField ID="hidRPT_ID1" runat="server" />
    <asp:HiddenField ID="hidRPT_ID2" runat="server" />
    <asp:HiddenField ID="hidRPT_ID3" runat="server" />
    <asp:HiddenField ID="hidRPT_ID4" runat="server" />
    <asp:HiddenField ID="hidRPT_ID5" runat="server" />
    <asp:HiddenField ID="hidRPT_ID6" runat="server" />

    <asp:HiddenField ID="hidSGCSITE_ID" runat="server" />
    <asp:HiddenField ID="hidSTD_ID" runat="server" />
    <asp:HiddenField ID="hidTISISITE_ID" runat="server" />

    <asp:HiddenField ID="hidSELECT_STD" runat="server" />
    <asp:HiddenField ID="hidDELETE_STD" runat="server" />
    

        <!-- Content Header (Page header) -->
    <section class="content-header">
         <ol class="breadcrumb">
            <li><i class="fa fa-home"></i></li>
            <li>Master data</li>
             <%-- Edit by Turk 18/04/2562 --> Onshore Site management --%>
            <li onclick="javascript:GoMenu('MngSite');" style="cursor:pointer" >Onshore Site management</li>
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
                                    <td style="text-align: right; height: 30px; width: 150px;">Region&nbsp;:&nbsp;&nbsp;</td>
                                    <td style="text-align: left; height: 30px;">
                                        <asp:DropDownList ID="ddlREGION_ID" runat="server" Width="100px" CssClass="form-control">
							            </asp:DropDownList>
                                    </td>
                                    <td style="text-align: right; height: 30px; width: 140px;">Auto email alert&nbsp;:&nbsp;&nbsp;</td>
                                    <td style="text-align: left; height: 30px;">
                                         <asp:DropDownList ID="ddlALERT_FLAG" runat="server" Width="100px" CssClass="form-control">
                                            <asp:ListItem Value="N" Text="No"></asp:ListItem>
                                             <asp:ListItem Value="Y" Text="Yes"></asp:ListItem>
                                            
							            </asp:DropDownList>
                                    </td>

                                </tr>
                        </table>
                    </div>

                     <div class="col-md-6">
                        <table border="0" class="text-black">
                             <tr>
                                    <td style="text-align: right; height: 30px; width: 150px;">ISO17025&nbsp;:&nbsp;&nbsp;</td>
                                    <td style="text-align: left; height: 30px;">
                                         <asp:DropDownList ID="ddlISO_FLAG" runat="server" Width="100px" CssClass="form-control" AutoPostBack="True" OnSelectedIndexChanged="ddlISO_FLAG_SelectedIndexChanged">
                                            <asp:ListItem Value="N" Text="No"></asp:ListItem>
                                            <asp:ListItem Value="Y" Text="Yes"></asp:ListItem>
							            </asp:DropDownList>
                                    </td>
                                    <td style="text-align: right; height: 30px; width: 60px;">H2S&nbsp;:&nbsp;&nbsp;</td>
                                    <td style="text-align: left; height: 30px;">
                                         <asp:DropDownList ID="ddlH2S_FLAG" runat="server" Width="100px" CssClass="form-control">
                                            <asp:ListItem Value="N" Text="No"></asp:ListItem>
                                            <asp:ListItem Value="Y" Text="Yes"></asp:ListItem>
							            </asp:DropDownList>
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
<div class="col-md-12">

 <div class="box box-orange box-solid">
            <div class="box-header">
              <h3 class="box-title"><i class="fa fa-list-alt"></i> Report</h3>

              <div class="box-tools pull-right">
                <button type="button" class="btn btn-box-tool" data-widget="collapse"><i class="fa fa-minus"></i>
                </button>
              </div>
            </div>
            <!-- /.box-header -->
            <div class="box-body">
                <table border="0" class="text-black">
                <tr style="vertical-align:top;">
                    <td style="text-align: right; height: 30px; width: 150px;">&nbsp;</td>
                    <td style="text-align: left; height: 30px; width: 130px;">
                        <b>NGBilling report no.</b>
                    </td>
                    <td style="text-align: left; height: 30px; width: 130px;">
                        <b>QC Lab report no.</b>
                    </td>
                    <td style="text-align: left; height: 30px;">
                        <b>QC Lab report name</b>
                    </td>
                </tr>

                <tr style="vertical-align:top;">
                    <td style="text-align: right; height: 30px; width: 150px;">Daily(20)&nbsp;:&nbsp;&nbsp;</td>
                    <td style="text-align: left; height: 30px; width: 130px;">
                        <asp:TextBox ID="txtNGBILL_RPT_NO5" MaxLength="50" CssClass="form-control" runat="server" Width="120px" Text=""></asp:TextBox>
                    </td>
                    <td style="text-align: left; height: 30px; width: 130px;">
                        <asp:TextBox ID="txtGC_RPT_NO5" MaxLength="50" CssClass="form-control" runat="server" Width="120px" Text=""></asp:TextBox>
                    </td>
                    <td style="text-align: left; height: 30px;">
                        <asp:TextBox ID="txtGC_RPT_NAME5" MaxLength="500" CssClass="form-control" runat="server" Width="650px" Text="" ></asp:TextBox>
                    </td>
                </tr>
                <tr style="vertical-align:top;">
                    <td style="text-align: right; height: 30px; width: 150px;">20 Days&nbsp;:&nbsp;&nbsp;</td>
                    <td style="text-align: left; height: 30px; width: 130px;">
                        <asp:TextBox ID="txtNGBILL_RPT_NO6" MaxLength="50" CssClass="form-control" runat="server" Width="120px" Text=""></asp:TextBox>
                    </td>
                    <td style="text-align: left; height: 30px; width: 130px;">
                        <asp:TextBox ID="txtGC_RPT_NO6" MaxLength="50" CssClass="form-control" runat="server" Width="120px" Text=""></asp:TextBox>
                    </td>
                    <td style="text-align: left; height: 30px;">
                        <asp:TextBox ID="txtGC_RPT_NAME6" MaxLength="500" CssClass="form-control" runat="server" Width="650px" Text="" ></asp:TextBox>
                    </td>
                </tr>


                <tr style="vertical-align:top;">
                    <td style="text-align: right; height: 30px; width: 150px;">Daily(27)&nbsp;:&nbsp;&nbsp;</td>
                    <td style="text-align: left; height: 30px; width: 130px;">
                        <asp:TextBox ID="txtNGBILL_RPT_NO4" MaxLength="50" CssClass="form-control" runat="server" Width="120px" Text=""></asp:TextBox>
                    </td>
                    <td style="text-align: left; height: 30px; width: 130px;">
                        <asp:TextBox ID="txtGC_RPT_NO4" MaxLength="50" CssClass="form-control" runat="server" Width="120px" Text=""></asp:TextBox>
                    </td>
                    <td style="text-align: left; height: 30px;">
                        <asp:TextBox ID="txtGC_RPT_NAME4" MaxLength="500" CssClass="form-control" runat="server" Width="650px" Text="" ></asp:TextBox>
                    </td>
                </tr>
                <tr style="vertical-align:top;">
                    <td style="text-align: right; height: 30px; width: 150px;">27 Days&nbsp;:&nbsp;&nbsp;</td>
                    <td style="text-align: left; height: 30px; width: 130px;">
                        <asp:TextBox ID="txtNGBILL_RPT_NO2" MaxLength="50" CssClass="form-control" runat="server" Width="120px" Text=""></asp:TextBox>
                    </td>
                    <td style="text-align: left; height: 30px; width: 130px;">
                        <asp:TextBox ID="txtGC_RPT_NO2" MaxLength="50" CssClass="form-control" runat="server" Width="120px" Text=""></asp:TextBox>
                    </td>
                    <td style="text-align: left; height: 30px;">
                        <asp:TextBox ID="txtGC_RPT_NAME2" MaxLength="500" CssClass="form-control" runat="server" Width="650px" Text="" ></asp:TextBox>
                    </td>
                </tr>
                <tr style="vertical-align:top;">
                    <td style="text-align: right; height: 30px; width: 150px;">Daily&nbsp;:&nbsp;&nbsp;</td>
                    <td style="text-align: left; height: 30px; width: 130px;">
                        <asp:TextBox ID="txtNGBILL_RPT_NO1" MaxLength="50" CssClass="form-control" runat="server" Width="120px" Text=""></asp:TextBox>
                    </td>
                    <td style="text-align: left; height: 30px; width: 130px;">
                        <asp:TextBox ID="txtGC_RPT_NO1" MaxLength="50" CssClass="form-control" runat="server" Width="120px" Text=""></asp:TextBox>
                    </td>
                    <td style="text-align: left; height: 30px;">
                        <asp:TextBox ID="txtGC_RPT_NAME1" MaxLength="500" CssClass="form-control" runat="server" Width="650px" Text="" ></asp:TextBox>
                    </td>
                </tr>
                <tr style="vertical-align:top;">
                    <td style="text-align: right; height: 30px; width: 150px;">End month&nbsp;:&nbsp;&nbsp;</td>
                    <td style="text-align: left; height: 30px; width: 130px;">
                        <asp:TextBox ID="txtNGBILL_RPT_NO3" MaxLength="50" CssClass="form-control" runat="server" Width="120px" Text=""></asp:TextBox>
                    </td>
                    <td style="text-align: left; height: 30px; width: 130px;">
                        <asp:TextBox ID="txtGC_RPT_NO3" MaxLength="50" CssClass="form-control" runat="server" Width="120px" Text=""></asp:TextBox>
                    </td>
                    <td style="text-align: left; height: 30px;">
                        <asp:TextBox ID="txtGC_RPT_NAME3" MaxLength="500" CssClass="form-control" runat="server" Width="650px" Text="" ></asp:TextBox>
                    </td>
                </tr>
                
     </table>
           </div>

     <div class="box-body">
  
         <table border="0" class="text-black">
                <tr style="vertical-align:top;">
                    <td style="text-align: right; height: 30px; width: 150px;">&nbsp;</td>
                    <td style="text-align: left; height: 30px; width: 130px;">
                        <b>Analysis Method :</b>
                    </td>
                    <td style="text-align: left; height: 30px;">
                         <asp:DropDownList ID="ddlANLMET_ID" runat="server"  CssClass="form-control">
						 </asp:DropDownList>
                    </td>
                </tr>
         </table>
    </div>


</div>


    
</div> 

<!--------------------------------------------------------------------------------------------->
<asp:Panel ID="pnlISO" runat="server" Visible="true">
<div class="col-md-6">

 <div class="box box-success box-solid">
            <div class="box-header">
              <h3 class="box-title"><i class="fa fa-list-alt"></i> Standard gas composition</h3>

              <div class="box-tools pull-right">
                <button type="button" class="btn btn-box-tool" data-widget="collapse"><i class="fa fa-minus"></i>
                </button>
              </div>
            </div>
            <!-- /.box-header -->
            <div class="box-body">
                    <table border="0" class="text-black">
                    <tr>
                        <td style="text-align: right; height: 30px; width: 120px;">Cylinder No.&nbsp;:&nbsp;&nbsp;</td>
                        <td style="text-align: left; height: 30px;">
                            <asp:TextBox ID="txtCYLINDER_NO" MaxLength="50" CssClass="form-control" runat="server" Width="400px" > </asp:TextBox>
                        </td>
                        <td style="width: 45px; padding-left:15px;">
                                        <button id="btnFind" runat="server" type="button" class="pull-left btn btn-block btn-sametogetherproject"  onclick="JavaScript: openModalSTD(); return false;" style="width: 36px; padding:2px">
                                        <i class="fa fa-list-ol fa-lg"></i></button>
                         </td>
                    </tr>
                    </table>

                             <table border="0" class="text-black">
                                <tr>
                                    <td style="text-align: right; height: 30px; width: 120px;">Order date&nbsp;:&nbsp;&nbsp;
                                        <asp:HiddenField ID="hidORDER_DATE" runat="server" />
                                    </td>
                                    <td style="text-align: left; height: 30px;">
                                        <div id="datepicker1" class="input-group date" style="vertical-align: baseline">
                                            <asp:TextBox ID="datORDER_DATE" runat="server" CssClass="form-control" Width="100px" data-date-language="en-US" MaxLength="10" data-inputmask="'alias': 'dd/mm/yyyy'" ></asp:TextBox>
                                            <div id="divORDER_DATE" runat="server" class="input-group-addon"><i id="i1" class="fa fa-calendar"></i>
                                                 <script>
                                                    $(function () {
                                                        $("input[id$=datORDER_DATE]").datepicker({
                                                            format: 'dd/mm/yyyy'
                                                        });
                                                   });
                                                    $('#datepicker1').datepicker({
                                                        autoclose: true
                                                    })
                                                </script>
                                            </div>
                                        </div>
                                       
                                    </td>
                                    <td style="text-align: right; height: 30px; width: 90px;">Expire date&nbsp;:&nbsp;&nbsp;</td>
                                    <td style="text-align: left; height: 30px;">
                                        <div id="datepicker2" class="input-group date" style="vertical-align: baseline">
                                            <asp:TextBox ID="datEXPIRE_DATE" runat="server" CssClass="form-control" Width="100px" data-date-language="en-US" MaxLength="10" data-inputmask="'alias': 'dd/mm/yyyy'" ></asp:TextBox>
                                            <div id="div1" runat="server" class="input-group-addon"><i id="i1" class="fa fa-calendar"></i>
                                                 <script>
                                                    $(function () {
                                                        $("input[id$=datEXPIRE_DATE]").datepicker({
                                                            format: 'dd/mm/yyyy'
                                                        });
                                                   });
                                                    $('#datepicker2').datepicker({
                                                        autoclose: true
                                                    })
                                                </script>
                                            </div>
                                        </div>
                                       
                                    </td>
                                </tr>
                                 </table>

                              <table border="0" class="text-black">
                                  <tr><td></td><td></td></tr>
                                <tr>
                                    <td style="text-align: right; vertical-align:top; height: 30px; width: 120px;">&nbsp;&nbsp;</td>
                                    <td style="text-align: left; height: 60px;">
                                        <table cellspacing="0" align="Left" rules="all" border="0" style="border-collapse:collapse;">
		                                <tr>
                                        <th  class="cell-center cell-borderW1" scope="col"  style="width: 200px; border:0px;">&nbsp;</th>
                                        <th  class="cell-center cell-borderW1" scope="col"  style="width: 200px; border:0px;">&nbsp;</th>
                                        <th  class="Table-head-success cell-center" scope="col" colspan="2"  style="width: 400px;">ISO17025</th> 
		                                </tr>
                                            <tr align="center" class="Table-head-gray">
                                                <th class="cell-center" scope="col" style="width: 200px;">Component</th>
                                                <th class="cell-center" scope="col" style="width: 200px;">Certified Concentration (Mole%)</th>
                                                <%-- Add by Turk 11/04/2562 --> Min Max --%>
                                                <th class="cell-center" scope="col" style="width: 200px;">Min</th>
                                                <th class="cell-center" scope="col" style="width: 200px;">Max</th>
                                            </tr>
                                            <tr><td class="cell-left cell-Middle cell-border">CH4
                                                </td>
                                                <td class="cell-center cell-Middle cell-border"><asp:TextBox ID="txtC1" MaxLength="15" CssClass="form-control" runat="server" Width="180px" ReadOnly="true" ></asp:TextBox>
                                                </td>
                                                <%-- Add by Turk 11/04/2562 --> Min Max --%>
                                                <td class="cell-left cell-Middle cell-border"><asp:TextBox ID="txtC1_MIN" MaxLength="15" CssClass="form-control" runat="server" Width="160px" onkeypress="return isNumber(event)" onblur="Calc_SUMMIN();"  ></asp:TextBox>
                                                </td>
                                                <td class="cell-left cell-Middle cell-border"><asp:TextBox ID="txtC1_MAX" MaxLength="15" CssClass="form-control" runat="server" Width="160px" onkeypress="return isNumber(event)" onblur="Calc_SUMMAX();"  ></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr><td class="cell-left cell-Middle cell-border">C2H6
                                                </td>
                                                <td class="cell-center cell-Middle cell-border"><asp:TextBox ID="txtC2" MaxLength="15" CssClass="form-control" runat="server" Width="180px" ReadOnly="true" ></asp:TextBox>
                                                </td>
                                                <%-- Add by Turk 11/04/2562 --> Min Max --%>
                                                <td class="cell-left cell-Middle cell-border"><asp:TextBox ID="txtC2_MIN" MaxLength="15" CssClass="form-control" runat="server" Width="160px" onkeypress="return isNumber(event)" onblur="Calc_SUMMIN();"  ></asp:TextBox>
                                                </td>
                                                <td class="cell-left cell-Middle cell-border"><asp:TextBox ID="txtC2_MAX" MaxLength="15" CssClass="form-control" runat="server" Width="160px" onkeypress="return isNumber(event)" onblur="Calc_SUMMAX();"  ></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr><td class="cell-left cell-Middle cell-border">C3H8
                                                </td>
                                                <td class="cell-center cell-Middle cell-border"><asp:TextBox ID="txtC3" MaxLength="15" CssClass="form-control" runat="server" Width="180px" ReadOnly="true" ></asp:TextBox>
                                                </td>
                                                <%-- Add by Turk 11/04/2562 --> Min Max --%>
                                                <td class="cell-left cell-Middle cell-border"><asp:TextBox ID="txtC3_MIN" MaxLength="15" CssClass="form-control" runat="server" Width="160px" onkeypress="return isNumber(event)" onblur="Calc_SUMMIN();"  ></asp:TextBox>
                                                </td>
                                                <td class="cell-left cell-Middle cell-border"><asp:TextBox ID="txtC3_MAX" MaxLength="15" CssClass="form-control" runat="server" Width="160px" onkeypress="return isNumber(event)" onblur="Calc_SUMMAX();"  ></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr><td class="cell-left cell-Middle cell-border">IC4H10
                                                </td>
                                                <td class="cell-center cell-Middle cell-border"><asp:TextBox ID="txtIC4" MaxLength="15" CssClass="form-control" runat="server" Width="180px" ReadOnly="true" ></asp:TextBox>
                                                </td>
                                                <%-- Add by Turk 11/04/2562 --> Min Max --%>
                                                <td class="cell-left cell-Middle cell-border"><asp:TextBox ID="txtIC4_MIN" MaxLength="15" CssClass="form-control" runat="server" Width="160px" onkeypress="return isNumber(event)" onblur="Calc_SUMMIN();"  ></asp:TextBox>
                                                </td>
                                                <td class="cell-left cell-Middle cell-border"><asp:TextBox ID="txtIC4_MAX" MaxLength="15" CssClass="form-control" runat="server" Width="160px" onkeypress="return isNumber(event)" onblur="Calc_SUMMAX();"  ></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr><td class="cell-left cell-Middle cell-border">NC4H10
                                                </td>
                                                <td class="cell-center cell-Middle cell-border"><asp:TextBox ID="txtNC4" MaxLength="15" CssClass="form-control" runat="server" Width="180px" ReadOnly="true" ></asp:TextBox>
                                                </td>
                                                <%-- Add by Turk 11/04/2562 --> Min Max --%>
                                                <td class="cell-left cell-Middle cell-border"><asp:TextBox ID="txtNC4_MIN" MaxLength="15" CssClass="form-control" runat="server" Width="160px" onkeypress="return isNumber(event)" onblur="Calc_SUMMIN();"  ></asp:TextBox>
                                                </td>
                                                <td class="cell-left cell-Middle cell-border"><asp:TextBox ID="txtNC4_MAX" MaxLength="15" CssClass="form-control" runat="server" Width="160px" onkeypress="return isNumber(event)" onblur="Calc_SUMMAX();"  ></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr><td class="cell-left cell-Middle cell-border">IC5H12
                                                </td>
                                                <td class="cell-center cell-Middle cell-border"><asp:TextBox ID="txtIC5" MaxLength="15" CssClass="form-control" runat="server" Width="180px" ReadOnly="true" ></asp:TextBox>
                                                </td>
                                                <%-- Add by Turk 11/04/2562 --> Min Max --%>
                                                <td class="cell-left cell-Middle cell-border"><asp:TextBox ID="txtIC5_MIN" MaxLength="15" CssClass="form-control" runat="server" Width="160px" onkeypress="return isNumber(event)" onblur="Calc_SUMMIN();"  ></asp:TextBox>
                                                </td>
                                                <td class="cell-left cell-Middle cell-border"><asp:TextBox ID="txtIC5_MAX" MaxLength="15" CssClass="form-control" runat="server" Width="160px" onkeypress="return isNumber(event)" onblur="Calc_SUMMAX();"  ></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr><td class="cell-left cell-Middle cell-border">NC5H12
                                                </td>
                                                <td class="cell-center cell-Middle cell-border"><asp:TextBox ID="txtNC5" MaxLength="15" CssClass="form-control" runat="server" Width="180px" ReadOnly="true" ></asp:TextBox>
                                                </td>
                                                <%-- Add by Turk 11/04/2562 --> Min Max --%>
                                                <td class="cell-left cell-Middle cell-border"><asp:TextBox ID="txtNC5_MIN" MaxLength="15" CssClass="form-control" runat="server" Width="160px" onkeypress="return isNumber(event)" onblur="Calc_SUMMIN();"  ></asp:TextBox>
                                                </td>
                                                <td class="cell-left cell-Middle cell-border"><asp:TextBox ID="txtNC5_MAX" MaxLength="15" CssClass="form-control" runat="server" Width="160px" onkeypress="return isNumber(event)" onblur="Calc_SUMMAX();"  ></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr><td class="cell-left cell-Middle cell-border">C6H14
                                                </td>
                                                <td class="cell-center cell-Middle cell-border"><asp:TextBox ID="txtC6" MaxLength="15" CssClass="form-control" runat="server" Width="180px" ReadOnly="true" ></asp:TextBox>
                                                </td>
                                                <%-- Add by Turk 11/04/2562 --> Min Max --%>
                                                <td class="cell-left cell-Middle cell-border"><asp:TextBox ID="txtC6_MIN" MaxLength="15" CssClass="form-control" runat="server" Width="160px" onkeypress="return isNumber(event)" onblur="Calc_SUMMIN();"  ></asp:TextBox>
                                                </td>
                                                <td class="cell-left cell-Middle cell-border"><asp:TextBox ID="txtC6_MAX" MaxLength="15" CssClass="form-control" runat="server" Width="160px" onkeypress="return isNumber(event)" onblur="Calc_SUMMAX();"  ></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr><td class="cell-left cell-Middle cell-border">CO2
                                                </td>
                                                <td class="cell-center cell-Middle cell-border"><asp:TextBox ID="txtCO2" MaxLength="15" CssClass="form-control" runat="server" Width="180px" ReadOnly="true" ></asp:TextBox>
                                                </td>
                                                <%-- Add by Turk 11/04/2562 --> Min Max --%>
                                                <td class="cell-left cell-Middle cell-border"><asp:TextBox ID="txtCO2_MIN" MaxLength="15" CssClass="form-control" runat="server" Width="160px" onkeypress="return isNumber(event)" onblur="Calc_SUMMIN();"  ></asp:TextBox>
                                                </td>
                                                <td class="cell-left cell-Middle cell-border"><asp:TextBox ID="txtCO2_MAX" MaxLength="15" CssClass="form-control" runat="server" Width="160px" onkeypress="return isNumber(event)" onblur="Calc_SUMMAX();"  ></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr><td class="cell-left cell-Middle cell-border">N2
                                                </td>
                                                <td class="cell-center cell-Middle cell-border"><asp:TextBox ID="txtN2" MaxLength="15" CssClass="form-control" runat="server" Width="180px" ReadOnly="true" ></asp:TextBox>
                                                </td>
                                                <%-- Add by Turk 11/04/2562 --> Min Max --%>
                                                <td class="cell-left cell-Middle cell-border"><asp:TextBox ID="txtN2_MIN" MaxLength="15" CssClass="form-control" runat="server" Width="160px" onkeypress="return isNumber(event)" onblur="Calc_SUMMIN();"  ></asp:TextBox>
                                                </td>
                                                <td class="cell-left cell-Middle cell-border"><asp:TextBox ID="txtN2_MAX" MaxLength="15" CssClass="form-control" runat="server" Width="160px" onkeypress="return isNumber(event)" onblur="Calc_SUMMAX();"  ></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr><td class="cell-left cell-Middle cell-border">H2S
                                                </td>
                                                <td class="cell-center cell-Middle cell-border"><asp:TextBox ID="txtH2S" MaxLength="15" CssClass="form-control" runat="server" Width="180px" ReadOnly="true" ></asp:TextBox>
                                                </td>
                                                <%-- Add by Turk 11/04/2562 --> Min Max --%>
                                                <td class="cell-left cell-Middle cell-border"><asp:TextBox ID="txtH2S_MIN" MaxLength="15" CssClass="form-control" runat="server" Width="160px" onkeypress="return isNumber(event)" onblur="Calc_SUMMIN();"  ></asp:TextBox>
                                                </td>
                                                <td class="cell-left cell-Middle cell-border"><asp:TextBox ID="txtH2S_MAX" MaxLength="15" CssClass="form-control" runat="server" Width="160px" onkeypress="return isNumber(event)" onblur="Calc_SUMMAX();"  ></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr><td class="cell-left cell-Middle cell-border">Hg
                                                </td>
                                                <td class="cell-center cell-Middle cell-border"><asp:TextBox ID="txtHG" MaxLength="15" CssClass="form-control" runat="server" Width="180px" ReadOnly="true" ></asp:TextBox>
                                                </td>
                                                <%-- Add by Turk 11/04/2562 --> Min Max --%>
                                                <td class="cell-left cell-Middle cell-border"><asp:TextBox ID="txtHG_MIN" MaxLength="15" CssClass="form-control" runat="server" Width="160px" onkeypress="return isNumber(event)" onblur="Calc_SUMMIN();"  ></asp:TextBox>
                                                </td>
                                                <td class="cell-left cell-Middle cell-border"><asp:TextBox ID="txtHG_MAX" MaxLength="15" CssClass="form-control" runat="server" Width="160px" onkeypress="return isNumber(event)" onblur="Calc_SUMMAX();"  ></asp:TextBox>
                                                </td>
                                            </tr>
                                            <tr class="ItemFooter">
                                                <td class="cell-left cell-Middle cell-border">Summary </td>
                                                <td class="cell-center cell-Middle cell-border">
                                                    <asp:TextBox ID="txtSUM" runat="server" CssClass="form-control" MaxLength="15" ReadOnly="True" Width="180px"></asp:TextBox>
                                                </td>
                                                <%-- Add by Turk 11/04/2562 --> Min Max --%>
                                                <td class="cell-left cell-Middle cell-border">
                                                    <asp:TextBox ID="txtSUM_MIN" runat="server" CssClass="form-control" MaxLength="15" ReadOnly="True" Width="160px"></asp:TextBox>
                                                </td>
                                                <td class="cell-left cell-Middle cell-border">
                                                    <asp:TextBox ID="txtSUM_MAX" runat="server" CssClass="form-control" MaxLength="15" ReadOnly="True" Width="160px"></asp:TextBox>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                      
                                </table>
           </div>
</div>

</div>
 </asp:Panel>

<!--------------------------------------------------------------------------------------------->

                        <div class="col-md-6">

 <div class="box box-violet box-solid">
            <div class="box-header">
              <h3 class="box-title"><i class="fa fa-list-alt"></i> Chromat run</h3>

              <div class="box-tools pull-right">
                <button type="button" class="btn btn-box-tool" data-widget="collapse"><i class="fa fa-minus"></i>
                </button>
              </div>
            </div>
            <!-- /.box-header -->
            <div class="box-body">
                    <table border="0" class="text-black">
                <tr>
                    <td style="text-align: right; height: 30px; width: 150px;">Total run&nbsp;:&nbsp;&nbsp;</td>
                    <td style="text-align: left; height: 30px;">
                        <asp:TextBox ID="txtTOTAL_RUN" MaxLength="10" CssClass="form-control" runat="server" Width="180px" onkeypress="return isNumber(event)" ></asp:TextBox>
                    </td>
                </tr>
        <tr>
                    <td style="text-align: right; height: 30px; width: 150px;">Tolerance&nbsp;:&nbsp;&nbsp;</td>
                    <td style="text-align: left; height: 30px;">
                        <asp:TextBox ID="txtTOLERANCE_RUN" MaxLength="5" CssClass="form-control" runat="server" Width="180px" onkeypress="return isNumber(event)" ></asp:TextBox>
                    </td>
                </tr>
     </table>
           </div>
</div>

</div>
<!--------------------------------------------------------------------------------------------->

<div class="col-md-6">
 <div class="box box-violet box-solid">
            <div class="box-header">
              <h3 class="box-title"><i class="fa fa-list-alt"></i> OMA (Moisture)</h3>

              <div class="box-tools pull-right">
                <button type="button" class="btn btn-box-tool" data-widget="collapse"><i class="fa fa-minus"></i>
                </button>
              </div>
            </div>
            <!-- /.box-header -->
            <div class="box-body">
                    <table border="0" class="text-black">
                <tr>
                    <td style="text-align: right; height: 30px; width: 150px;">OMA Main Point&nbsp;:&nbsp;&nbsp;</td>
                    <td style="text-align: left; height: 30px;">
                        <asp:DropDownList ID="ddlOMA_NAME1" Width="400px" CssClass="form-control select2" runat="server">
                        </asp:DropDownList>
                    </td>
                </tr>
        <tr>
                    <td style="text-align: right; height: 30px; width: 150px;">OMA Support Point1&nbsp;:&nbsp;&nbsp;</td>
                    <td style="text-align: left; height: 30px;">
                        <asp:DropDownList ID="ddlOMA_NAME2" Width="400px" CssClass="form-control select2" runat="server">
                        </asp:DropDownList>
                    </td>
                </tr>
     </table>
           </div>
</div>

</div>
<!--------------------------------------------------------------------------------------------->

<div class="col-md-6">

 <div class="box box-violet box-solid">
            <div class="box-header">
              <h3 class="box-title"><i class="fa fa-list-alt"></i> Flow </h3>

              <div class="box-tools pull-right">
                <button type="button" class="btn btn-box-tool" data-widget="collapse"><i class="fa fa-minus"></i>
                </button>
              </div>
            </div>
            <!-- /.box-header -->
            <div class="box-body">
                <table border="0" class="text-black">
                <tr>
                    <td style="text-align: right; height: 30px; width: 150px;">Primary flow&nbsp;:&nbsp;&nbsp;</td>
                    <td style="text-align: left; height: 30px;">
                        <asp:DropDownList ID="ddlFLOW_NAME1" Width="400px" CssClass="form-control select2" runat="server">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td style="text-align: right; height: 30px; width: 150px;">Secondary flow&nbsp;:&nbsp;&nbsp;</td>
                    <td style="text-align: left; height: 30px;">
                        <asp:DropDownList ID="ddlFLOW_NAME2" Width="400px" CssClass="form-control select2" runat="server">
                        </asp:DropDownList>
                    </td>
                </tr>

     </table>
           </div>
</div>

</div> 

<!--------------------------------------------------------------------------------------------->

<div class="col-md-6">

 <div class="box box-violet box-solid">
            <div class="box-header">
              <h3 class="box-title"><i class="fa fa-list-alt"></i> OGC (สำรอง) </h3>

              <div class="box-tools pull-right">
                <button type="button" class="btn btn-box-tool" data-widget="collapse"><i class="fa fa-minus"></i>
                </button>
              </div>
            </div>
            <!-- /.box-header -->
            <div class="box-body">
                <table border="0" class="text-black">
                <tr>
                    <td style="text-align: right; height: 30px; width: 150px;">OGC Support Point 1&nbsp;:&nbsp;&nbsp;</td>
                    <td style="text-align: left; height: 30px;">
                        <asp:DropDownList ID="ddlOGC_NAME1" Width="400px" CssClass="form-control select2" runat="server">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td style="text-align: right; height: 30px; width: 150px;">OGC Support Point 2&nbsp;:&nbsp;&nbsp;</td>
                    <td style="text-align: left; height: 30px;">
                        <asp:DropDownList ID="ddlOGC_NAME2" Width="400px" CssClass="form-control select2" runat="server">
                        </asp:DropDownList>
                    </td>
                </tr>
 	<tr>
                   <td style="text-align: right; height: 30px; width: 150px;">OGC Support Point 3&nbsp;:&nbsp;&nbsp;</td>
                    <td style="text-align: left; height: 30px;">
                        <asp:DropDownList ID="ddlOGC_NAME3" Width="400px" CssClass="form-control select2" runat="server">
                        </asp:DropDownList>
                    </td>
                </tr>

     </table>
           </div>
</div>

</div> 


<!---------------------------------------------------------->
<%-- Add by Turk 11/04/2562 --> H2S --%>
<div class="col-md-6"></div>
<div class="col-md-6">

 <div class="box box-violet box-solid">
            <div class="box-header">
              <h3 class="box-title"><i class="fa fa-list-alt"></i> (SPOT) H2S </h3>

              <div class="box-tools pull-right">
                <button type="button" class="btn btn-box-tool" data-widget="collapse"><i class="fa fa-minus"></i>
                </button>
              </div>
            </div>
            <!-- /.box-header -->
            <div class="box-body">
                <table border="0" class="text-black">
                <tr>
                    <td style="text-align: right; height: 30px; width: 150px;">H2S Main Point&nbsp;:&nbsp;&nbsp;</td>
                    <td style="text-align: left; height: 30px;">
                        <asp:DropDownList ID="ddlH2S_NAME1" Width="400px" CssClass="form-control select2" runat="server">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td style="text-align: right; height: 30px; width: 150px;">H2S Support Point 1&nbsp;:&nbsp;&nbsp;</td>
                    <td style="text-align: left; height: 30px;">
                        <asp:DropDownList ID="ddlH2S_NAME2" Width="400px" CssClass="form-control select2" runat="server">
                        </asp:DropDownList>
                    </td>
                </tr>

     </table>
           </div>
</div>

</div> 

<!--------------------------------------------------------------------------------------------->
<%-- Add by Turk 11/04/2562 --> HG --%>
<div class="col-md-6"></div>
<div class="col-md-6">

 <div class="box box-violet box-solid">
            <div class="box-header">
              <h3 class="box-title"><i class="fa fa-list-alt"></i> (SPOT) HG </h3>

              <div class="box-tools pull-right">
                <button type="button" class="btn btn-box-tool" data-widget="collapse"><i class="fa fa-minus"></i>
                </button>
              </div>
            </div>
            <!-- /.box-header -->
            <div class="box-body">
                <table border="0" class="text-black">
                <tr>
                    <td style="text-align: right; height: 30px; width: 150px;">HG Main Point&nbsp;:&nbsp;&nbsp;</td>
                    <td style="text-align: left; height: 30px;">
                        <asp:DropDownList ID="ddlHG_NAME1" Width="400px" CssClass="form-control select2" runat="server">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td style="text-align: right; height: 30px; width: 150px;">HG Support Point 1&nbsp;:&nbsp;&nbsp;</td>
                    <td style="text-align: left; height: 30px;">
                        <asp:DropDownList ID="ddlHG_NAME2" Width="400px" CssClass="form-control select2" runat="server">
                        </asp:DropDownList>
                    </td>
                </tr>

     </table>
           </div>
</div>

</div> 

<!--------------------------------------------------------------------------------------------->
<%-- Add by Turk 11/04/2562 --> O2 --%>
<div class="col-md-6"></div>
<div class="col-md-6">

 <div class="box box-violet box-solid">
            <div class="box-header">
              <h3 class="box-title"><i class="fa fa-list-alt"></i> (SPOT) O2 </h3>

              <div class="box-tools pull-right">
                <button type="button" class="btn btn-box-tool" data-widget="collapse"><i class="fa fa-minus"></i>
                </button>
              </div>
            </div>
            <!-- /.box-header -->
            <div class="box-body">
                <table border="0" class="text-black">
                <tr>
                    <td style="text-align: right; height: 30px; width: 150px;">O2 Main Point&nbsp;:&nbsp;&nbsp;</td>
                    <td style="text-align: left; height: 30px;">
                        <asp:DropDownList ID="ddlO2_NAME1" Width="400px" CssClass="form-control select2" runat="server">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td style="text-align: right; height: 30px; width: 150px;">O2 Support Point 1&nbsp;:&nbsp;&nbsp;</td>
                    <td style="text-align: left; height: 30px;">
                        <asp:DropDownList ID="ddlO2_NAME2" Width="400px" CssClass="form-control select2" runat="server">
                        </asp:DropDownList>
                    </td>
                </tr>

     </table>
           </div>
</div>

</div> 

<!--------------------------------------------------------------------------------------------->
<%-- Add by Turk 11/04/2562 --> HC --%>
<div class="col-md-6"></div>
<div class="col-md-6">

 <div class="box box-violet box-solid">
            <div class="box-header">
              <h3 class="box-title"><i class="fa fa-list-alt"></i> (SPOT) H-C dew point </h3>

              <div class="box-tools pull-right">
                <button type="button" class="btn btn-box-tool" data-widget="collapse"><i class="fa fa-minus"></i>
                </button>
              </div>
            </div>
            <!-- /.box-header -->
            <div class="box-body">
                <table border="0" class="text-black">
                <tr>
                    <td style="text-align: right; height: 30px; width: 150px;">HC Main Point&nbsp;:&nbsp;&nbsp;</td>
                    <td style="text-align: left; height: 30px;">
                        <asp:DropDownList ID="ddlHC_NAME1" Width="400px" CssClass="form-control select2" runat="server">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td style="text-align: right; height: 30px; width: 150px;">HC Support Point 1&nbsp;:&nbsp;&nbsp;</td>
                    <td style="text-align: left; height: 30px;">
                        <asp:DropDownList ID="ddlHC_NAME2" Width="400px" CssClass="form-control select2" runat="server">
                        </asp:DropDownList>
                    </td>
                </tr>

     </table>
           </div>
</div>

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

  
    <!-- add modal standard ########################################################################################### -->

    <div class="modal fade" id="mdlSearchSTD" role="dialog">
        <div class="modal-dialog" style="width:600px;">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h4 class="modal-title">Standard Gas History</h4>
                </div>

                <div class="modal-body" style="padding: 20px;">
                    <div class="row form-group">
                        <table id="tblSTDList" style="width:100%" class="table-striped compact">
                            <thead>
                                <tr>
                                    <th class="Table-head-primary cell-center" style="width:5%" >View</th>
                                    <th class="Table-head-primary cell-center" style="width:50%" >Cylinder No.</th>
                                    <th class="Table-head-primary cell-center" style="width:20%" >Order date</th>
                                    <th class="Table-head-primary cell-center" style="width:20%" >Expire data</th>
                                    <th class="Table-head-primary cell-center" style="width:5%" >Delete</th>
                                 </tr>
                            </thead>
                            <tbody>

                            </tbody>
                        </table>

                        <textarea id="jsonSTD" name="jsonSTD" readonly="readonly" class="hidden"></textarea>
                </div>

                <div class="modal-footer">
                        <div class="col-md-12">
                            <button type="button" id="btnMdlCloseSTD" runat="server" class="btn btn-default" data-dismiss="modal">Close</button>
                        </div>
                </div>
            </div> <!--main content-->
        </div><!-- dialog-->
    </div>
</div>
    
    <!-- end modal standard ########################################################################################### -->


</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="FootContent" runat="server">
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="FootScript" runat="server">
<!-- Page script -->
<script>
    var tbl;

    $(function () {
        //Initialize Select2 Elements
        $('.select2').select2()

    })

    //-- calcularate summary gas composition 
    Calc_SUM();
    Calc_SUMMIN(); Calc_SUMMAX();


    $('#tblSTDList').hide();

    //--- popup standard history ----------------------------------------------------------------------------------
    function openModalSTD() {

        $('#mdlSearchSTD').modal({ backdrop: 'static', keyboard: false });
        $('#jsonSTD').val('');
        $('#tblSTDList').show();

        if (tbl !== undefined) {
            tbl.clear().draw();
            tbl.destroy();
        } 

        tbl = $('#tblSTDList').DataTable({
            dom: "Bftip",
            searching: false, 
            paging: false,
            pageLength: 20,
			ordering: false,				
            destroy: true,
                
        });

        SearchSTD();

    }

    function SearchSTD() {

        var siteid = GetCtrlValue("<%=hidSITE_ID.ClientID %>");

        var delstdid = GetCtrlValue("<%=hidDELETE_STD.ClientID %>");  //-- get delete std id
        

        tbl = $('#tblSTDList').DataTable({
                dom: "Bftip",
            searching: false,
                 paging: false,
                pageLength: 20,
                ordering: false,
                destroy: true,
                ajax: {
                    url: "../Handlers/GetSTD.ashx",
                    type: "POST",
                    data: {
                        "siteid": siteid, "delstdid": delstdid
                    }
                },
                columns: [
                    {
                        data: null,
                        render: function (data, type, row, meta) {
                            return "<li class=\"fa fa-eye fa-15x\" style=\"color: #2f5491;cursor: pointer;\" data-dismiss=\"modal\" onclick=\"SelectSTD('" + meta.row + "')\"></li>";
                        },
                        className: "text-center"
                    },
                    { data: 'CYLINDER_NO'},
                    { data: 'ORDER_SHOW' },
                    { data: 'EXPIRE_SHOW' },
                    {
                        data: null,
                        render: function (data, type, row, meta) {
                            return "<li class=\"fa  fa-times fa-15x\" style=\"color: #912f2f;cursor: pointer;\" data-dismiss=\"modal\" onclick=\"DeleteSTD('" + meta.row + "')\"></li>";
                        },
                        className: "text-center"
                    },
                ]
            });
        dat = tbl.ajax.json();

 
        SetCtrlValue("<%=hidDELETE_STD.ClientID %>", ""); //-- clear delete std id
    }
 

    function SelectSTD(rowidx) {
        if (tbl !== undefined) {

             var data = tbl.rows(rowidx).data();

            SetCtrlValue("<%=hidSELECT_STD.ClientID %>", data[0]['STD_ID']);

            DoAction('VIEW_STD', '');

        }
    }

    function DeleteSTD(rowidx) {

        var confirmed = confirm("Confirm to delete standard gas?");

        if (confirmed) {

           if (tbl !== undefined) {

            var data = tbl.rows(rowidx).data();
 
            SetCtrlValue("<%=hidDELETE_STD.ClientID %>", data[0]['STD_ID']);

            SearchSTD();


        }
        }


    }



</script>

<!--#include file="../Includes/JSfooter.html" -->
</asp:Content>
