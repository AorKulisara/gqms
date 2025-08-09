<%@ Page Title="" Language="C#" MasterPageFile="~/Includes/MainPage.Master" AutoEventWireup="true" CodeBehind="Verify.aspx.cs" Inherits="PTT.GQMS.USL.Web.Forms.Verify" MaintainScrollPositionOnPostback="True" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">

    <!-- multiple Select2 -->
    <link href="../Content/select2.min.css" rel="stylesheet" />
    <script src="../Scripts/select2.full.min.js"></script>   

    <!-- DataTable -->
    <script type="text/javascript" src="../Scripts/DataTables/DataTables/js/jquery.dataTables.min.js"></script>
    <script type="text/javascript" src="../Scripts/DataTables/FixedColumns/js/dataTables.fixedColumns.min.js"></script>
    <script type="text/javascript" src="../Scripts/pdfmake/vfs_fonts.js"></script>

<script type="text/javascript">
            var NextPage1 = "OMAhour?UID=<%= HttpContext.Current.Session["UID"] %>";
            var NextPage2 = "FLOWhour?UID=<%= HttpContext.Current.Session["UID"] %>";
            var NextPage3 = "ImportXLS?UID=<%= HttpContext.Current.Session["UID"] %>";

            function DoAction(act, val, val2) {
                var confirmed, isSubmitted, url, err, url2;
                confirmed = true; isSubmitted = true; url = ""; url2 = ""; err = "";
                switch (act) {
                    case "SEARCH": break;
                    case "IMPORT_XLS": break; //url2 = NextPage3;
                    case "SAVE_XLS": break;                    
                    case "OMA": url = NextPage1+"&K=" + val +"&D=" + val2; break;
                    case "FLOW": url = NextPage2+"&K=" + val +"&D=" + val2; break;
                    case "CONFIRM_1": confirmed = ConfirmModal("Confirm to send data (daily) to NGBILL?");break;
                    case "CONFIRM_2":
                         //-- edit 28/08/2020 -- เมื่อกดปุ่ม confirm ตรวจสอบว่า ถ้ายังไม่มีข้อมูล As Found ให้แจ้งเตือน
                        var cMSG = "Confirm to send data (27 days) to NGBILL?"
                        if (GetCtrl("<%=lblFOUND_DATE.ClientID%>")) {
                            if (GetInnerText("<%=lblFOUND_DATE.ClientID%>") == "") cMSG += "\r\n<font color='red'>*** Please note that <u>No Calibration Data</u> ***</font>";
                        }
                        confirmed = ConfirmModal(cMSG); break; 
                    case "CONFIRM_3":
                        //-- edit 28/08/2020 -- เมื่อกดปุ่ม confirm ตรวจสอบว่า ถ้ายังไม่มีข้อมูล As Found ให้แจ้งเตือน
                        var cMSG = "Confirm to send data (end month) to NGBILL?"
                        if (GetCtrl("<%=lblFOUND_DATE.ClientID%>")) {
                            if (GetInnerText("<%=lblFOUND_DATE.ClientID%>") == "") cMSG += "\r\n<font color='red'>*** Please note that <u>No Calibration Data</u> ***</font>";
                        }
                        confirmed = ConfirmModal(cMSG); break; 
                    case "CONFIRM_4": confirmed = ConfirmModal("Confirm to send data (20 days) to NGBILL?"); break; 
                    case "EDIT":
                        var lst = GetCtrlValue("hidEditItemIndex");
                        lst += "," + val;
                        SetCtrlValue("hidEditItemIndex", lst);
                        break;
                    case "SAVE": break;
                    case "EDIT_SPOT": break;
                    case "SAVE_SPOT": break;
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
    //--การกำหนด fix column ทำให้อ่าน GetCtrl('chkAll').checked เป็นค่าตั้งแต่ page load เสมอ
    //--จะไม่สามารถกำหนด checkbox ที่เป็น fixed column ได้
<%--            function SelectAll() {
                SetCtrlValue("ServerAction", "SELECTALL");
                LoadSpin();
                SubmitForm();

            <%-- var i, chk, chkTmp,max;
            max = <%=chkCount%>;

            chk = GetCtrl('chkAll').checked;  
            for (i=0; i<max; i++) {
                chkTmp = GetCtrl("chkSelect" + i);
                if (chkTmp != null){
                    chkTmp.checked = chk;
                }
            }--%>


    //--------------------------------------------------------------------
    //-- เลือก OMA site
    function SelectOMA(idx, val1, val2) {
        //--- flowname ให้ชื่อเดียวกับ fid (Site)
        var flowname = GetCtrlValue("ddlOMA_" + idx);
        if (flowname == "NO") { //-- ไม่ใช้ข้อมูล ให้ clear data ---------------------------------
            if (GetCtrl("txtWC_" + idx)) SetCtrlValue("txtWC_" + idx, "");
        }
        else {
            var index = document.getElementById("ddlOMA_" + idx).selectedIndex;
            if (index == 0)
                SetCtrlValue("txtWC_" + idx, val1);
            else
                SetCtrlValue("txtWC_" + idx, val2);
        }

    }

    //--------------------------------------------------------------------
    //-- เลือก OGC site สำรอง
    function SelectOgc(idx, rdate) {
        //--- flowname ให้ชื่อเดียวกับ fid 
        var flowname = GetCtrlValue("ddlOGC_" + idx);
        var curFID = GetCtrlValue("<%=hidFID.ClientID%>");
       
        if (flowname == "NO") { //-- ไม่ใช้ข้อมูล ให้ clear data ---------------------------------
            //-- OGC column 3-21, sum column 22
            if (GetCtrl("txtC1_" + idx)) SetCtrlValue("txtC1_" + idx, "");
            if (GetCtrl("txtC2_" + idx)) SetCtrlValue("txtC2_" + idx, "");
            if (GetCtrl("txtC3_" + idx)) SetCtrlValue("txtC3_" + idx, "");
            if (GetCtrl("txtIC4_" + idx)) SetCtrlValue("txtIC4_" + idx, "");
            if (GetCtrl("txtNC4_" + idx)) SetCtrlValue("txtNC4_" + idx, "");
            if (GetCtrl("txtIC5_" + idx)) SetCtrlValue("txtIC5_" + idx, "");
            if (GetCtrl("txtNC5_" + idx)) SetCtrlValue("txtNC5_" + idx, "");
            if (GetCtrl("txtC6_" + idx)) SetCtrlValue("txtC6_" + idx, "");
            if (GetCtrl("txtCO2_" + idx)) SetCtrlValue("txtCO2_" + idx, "");
            if (GetCtrl("txtN2_" + idx)) SetCtrlValue("txtN2_" + idx, "");
            if (GetCtrl("txtH2S_" + idx)) SetCtrlValue("txtH2S_" + idx, "");
            if (GetCtrl("txtNHV_" + idx)) SetCtrlValue("txtNHV_" + idx, "");
            if (GetCtrl("txtGHV_" + idx)) SetCtrlValue("txtGHV_" + idx, "");
            if (GetCtrl("txtSG_" + idx)) SetCtrlValue("txtSG_" + idx, "");
            if (GetCtrl("txtWC_" + idx)) SetCtrlValue("txtWC_" + idx, "");
            if (GetCtrl("txtUNNORMMIN_" + idx)) SetCtrlValue("txtUNNORMMIN_" + idx, "");
            if (GetCtrl("txtUNNORMMAX_" + idx)) SetCtrlValue("txtUNNORMMAX_" + idx, "");
            if (GetCtrl("txtUNNORMALIZED_" + idx)) SetCtrlValue("txtUNNORMALIZED_" + idx, "");
            if (GetCtrl("txtWB_" + idx)) SetCtrlValue("txtWB_" + idx, "");

        }
        else {

            $.ajax({
                type: 'GET',
                url: "../Handlers/GetGQMS_DAILY_UPDATE.ashx?pName=" + encodeURIComponent(flowname) + "&curFID=" + encodeURIComponent(curFID) + "&pDATE=" + rdate + "&pRDM=" + Math.random().toString(36).substring(7),     //--- &pRDM ใส่เพื่อให้ query ใหม่ทุกครั้ง
                data: { get_param: 'value' },
                dataType: 'json',
                success: function (data) {
                    $.each(data, function (index, element) {

                        SetCtrlValue("txtC1_" + idx, FormatNum(element.C1, 3));
                        SetCtrlValue("txtC2_" + idx, FormatNum(element.C2, 3));
                        SetCtrlValue("txtC3_" + idx, FormatNum(element.C3, 3));
                        SetCtrlValue("txtIC4_" + idx, FormatNum(element.IC4, 3));
                        SetCtrlValue("txtNC4_" + idx, FormatNum(element.NC4, 3));
                        SetCtrlValue("txtIC5_" + idx, FormatNum(element.IC5, 3));
                        SetCtrlValue("txtNC5_" + idx, FormatNum(element.NC5, 3));
                        SetCtrlValue("txtC6_" + idx, FormatNum(element.C6, 3));
                        SetCtrlValue("txtCO2_" + idx, FormatNum(element.CO2, 3));
                        SetCtrlValue("txtN2_" + idx, FormatNum(element.N2, 3));
                        SetCtrlValue("txtH2S_" + idx, FormatNum(element.H2S, 3));
                        SetCtrlValue("txtNHV_" + idx, FormatNum(element.NHV, 3));
                        SetCtrlValue("txtGHV_" + idx, FormatNum(element.GHV, 3));
                        SetCtrlValue("txtSG_" + idx, FormatNum(element.SG, 4));
                        SetCtrlValue("txtWC_" + idx, FormatNum(element.WC, 2));
                        SetCtrlValue("txtUNNORMMIN_" + idx, FormatNum(element.UNNORMMIN, 3));
                        SetCtrlValue("txtUNNORMMAX_" + idx, FormatNum(element.UNNORMMAX, 3));
                        SetCtrlValue("txtUNNORMALIZED_" + idx, FormatNum(element.UNNORMALIZED, 3));
                        SetCtrlValue("txtWB_" + idx, FormatNum(element.WB, 3));
                    });
                }
            });

        }
    }


   //--------------------------------------------------------------------
    //-- เลือก SPOT site สำรอง
    function SelectSH2S() {
        var Sname = GetCtrlValue("ddlSH2S");
        var rMM = GetCtrlValue("<%=hidMM.ClientID%>");
        var rYY = GetCtrlValue("<%=hidYY.ClientID%>");

        //-- clear data
        if (GetCtrl("txtSH2S_DATE")) SetCtrlValue("txtSH2S_DATE", "");
        if (GetCtrl("txtSH2S")) SetCtrlValue("txtSH2S", "");

 
        $.ajax({
            type: 'GET',
            url: "../Handlers/GetSPOT_Data.ashx?pType=H2S&pName=" + encodeURIComponent(Sname) + "&pMM=" + rMM+ "&pYY=" + rYY + "&pRDM=" + Math.random().toString(36).substring(7),     //--- &pRDM ใส่เพื่อให้ query ใหม่ทุกครั้ง
            data: { get_param: 'value' },
            dataType: 'json',
            success: function (data) {
                $.each(data, function (index, element) {
                    SetCtrlValue("txtSH2S_DATE", FormatDate(element.SDATE,"dd/MM/yyyy"));
                    SetCtrlValue("txtSH2S", FormatNum(element.H2S, 2));
                });
            }
        });
    }

     //-- เลือก SPOT site สำรอง
    function SelectSHG() {
        var Sname = GetCtrlValue("ddlSHG");
        var rMM = GetCtrlValue("<%=hidMM.ClientID%>");
        var rYY = GetCtrlValue("<%=hidYY.ClientID%>");

        //-- clear data
        if (GetCtrl("txtSHG_DATE")) SetCtrlValue("txtSHG_DATE", "");
        if (GetCtrl("txtSHG")) SetCtrlValue("txtSHG", "");

 
        $.ajax({
            type: 'GET',
            url: "../Handlers/GetSPOT_Data.ashx?pType=HG&pName=" + encodeURIComponent(Sname) + "&pMM=" + rMM+ "&pYY=" + rYY + "&pRDM=" + Math.random().toString(36).substring(7),     //--- &pRDM ใส่เพื่อให้ query ใหม่ทุกครั้ง
            data: { get_param: 'value' },
            dataType: 'json',
            success: function (data) {
                $.each(data, function (index, element) {
                    SetCtrlValue("txtSHG_DATE", FormatDate(element.SDATE,"dd/MM/yyyy"));
                    SetCtrlValue("txtSHG", FormatNum(element.HG, 3));
                });
            }
        });
    }

   //-- เลือก SPOT site สำรอง
    function SelectSO2() {
        var Sname = GetCtrlValue("ddlSO2");
        var rMM = GetCtrlValue("<%=hidMM.ClientID%>");
        var rYY = GetCtrlValue("<%=hidYY.ClientID%>");

        //-- clear data
        if (GetCtrl("txtSO2_DATE")) SetCtrlValue("txtSO2_DATE", "");
        if (GetCtrl("txtSO2")) SetCtrlValue("txtSO2", "");

 
        $.ajax({
            type: 'GET',
            url: "../Handlers/GetSPOT_Data.ashx?pType=O2&pName=" + encodeURIComponent(Sname) + "&pMM=" + rMM+ "&pYY=" + rYY + "&pRDM=" + Math.random().toString(36).substring(7),     //--- &pRDM ใส่เพื่อให้ query ใหม่ทุกครั้ง
            data: { get_param: 'value' },
            dataType: 'json',
            success: function (data) {
                $.each(data, function (index, element) {
                    SetCtrlValue("txtSO2_DATE", FormatDate(element.SDATE,"dd/MM/yyyy"));
                    SetCtrlValue("txtSO2", FormatNum(element.O2, 3));
                });
            }
        });
    }

       //-- เลือก SPOT site สำรอง
    function SelectSHC() {
        var Sname = GetCtrlValue("ddlSHC");
        var rMM = GetCtrlValue("<%=hidMM.ClientID%>");
        var rYY = GetCtrlValue("<%=hidYY.ClientID%>");

        //-- clear data
        if (GetCtrl("txtSHC_DATE")) SetCtrlValue("txtSHC_DATE", "");
        if (GetCtrl("txtSHC")) SetCtrlValue("txtSHC", "");

 
        $.ajax({
            type: 'GET',
            url: "../Handlers/GetSPOT_Data.ashx?pType=HC&pName=" + encodeURIComponent(Sname) + "&pMM=" + rMM+ "&pYY=" + rYY + "&pRDM=" + Math.random().toString(36).substring(7),     //--- &pRDM ใส่เพื่อให้ query ใหม่ทุกครั้ง
            data: { get_param: 'value' },
            dataType: 'json',
            success: function (data) {
                $.each(data, function (index, element) {
                    SetCtrlValue("txtSHC_DATE", FormatDate(element.SDATE,"dd/MM/yyyy"));
                    SetCtrlValue("txtSHC", FormatNum(element.HC, 3));
                });
            }
        });
    }


</script>


   <script>
       // ------------------ DataTable --------------------------------------------------------------------      

       $(function () {

           $('#<%=gvResult.ClientID%>').DataTable({
           bInfo:          false,
           sort:           false,
           searching: false,
           paging:         false,
          // scrollY:        "480px",
           scrollX:        true,
           //scrollCollapse: true,
          //  fixedHeader:    true,
           fixedColumns:   {
               leftColumns: 3
               }
           });

          // $('#<%=gvResult.ClientID%>').columns.adjust().draw();

       });

       // ------------------ Copy & Paste --------------------------------------------------------------------      
       $(document).ready(function () {

           $('td input').bind('paste', null, function (e) {
               $this = $(this);
               setTimeout(function () {
                   var clipboardData = window.clipboardData.getData('Text'); //?? chrome ติดเรื่อง clipboard 
                   var rows = clipboardData.split(/\n/);
                   var r;
                   var input = $this;
                   var i;

                   var fullname1 = input.attr("name");
                   var ix = fullname1.indexOf("_");
                   var row1 = parseInt(Right(fullname1, fullname1.length - ix - 1));
                   var name1 = Left(fullname1, ix);


                   if (input.attr("name") == "undefined" || input.attr("name") == null)    //undefined or null 
                   {
                       r = 99;
                   }
                   else {
                       for (r = 0; r < rows.length - 1; r++) {
                           var columns = rows[r].split(/\s+/);
                           for (i = 0; i < columns.length - 1; i++) {
                               input.val(columns[i]);
                               //-- ถ้าหาเกิน input ที่มี ก็จะเกิด error
                               var ctl = input.attr("name");
                               if (ctl == "undefined" || ctl == null)    //undefined or null 
                               {
                                   i = 99;
                               }
                               else
                               {
                                   if (ctl.indexOf("txtWB") > -1) //-- last input
                                       i = 99;
                                   else
                                       input = input.parent().next().find('input[type=text]');
                               }
                           }
                           row1++;
                           var name2 = name1 + "_" + (row1);
                           var input = input.parent().parent().next().find("input[name='" + name2 + "']");
                       }
                   }


               }, 0);
           });


       });
       // ------------------ Select All --------------------------------------------------------------------      
       $(document).ready(function () {

           $('.check_All').click(function () {
               var c = this.checked;
               $(':checkbox').prop('checked', c);
               if (c)
                   SetCtrlValue("<%=hidSELECTALL.ClientID%>", "Y");
               else
                   SetCtrlValue("<%=hidSELECTALL.ClientID%>", "");
           });                      
       });
        // ---------------------------------------------------------------------------------------------    


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
    <input type="hidden" id="hidEditItemIndex" name="hidEditItemIndex" value="<%=hEditItemIndex %>" />

    <asp:HiddenField ID="hidSELECTALL" runat="server" />
    <asp:HiddenField ID="hidSITE_ID" runat="server" />
    <asp:HiddenField ID="hidFID" runat="server" />
    <asp:HiddenField ID="hidMM" runat="server" />
    <asp:HiddenField ID="hidYY" runat="server" />
<asp:HiddenField ID="hidgNgRptNo27" runat="server" />
<asp:HiddenField ID="hidgNgRptNoEND" runat="server" />
<asp:HiddenField ID="hidgNgRptNoDaily" runat="server" />
<asp:HiddenField ID="hidgNgRptNoDaily27" runat="server" />
    <asp:HiddenField ID="hidgNgRptNo20" runat="server" />
    <asp:HiddenField ID="hidgNgRptNoDaily20" runat="server" />
<asp:HiddenField ID="hidfromDate" runat="server" />
<asp:HiddenField ID="hidtoDate" runat="server" />
<asp:HiddenField ID="hidtoDate27" runat="server" />
<asp:HiddenField ID="hidfromDate27" runat="server" />
<asp:HiddenField ID="hidtoDate20" runat="server" />
<asp:HiddenField ID="hidfromDate20" runat="server" />

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

<asp:HiddenField ID="hidgC1_MIN" runat="server" /> <!-- EDIT 19/07/2019 -->
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

    <asp:HiddenField ID="hidsH2S_NAME" runat="server" />
    <asp:HiddenField ID="hidsHG_NAME" runat="server" />
    <asp:HiddenField ID="hidsO2_NAME" runat="server" />
    <asp:HiddenField ID="hidsHC_NAME" runat="server" />

    <!-- Content Header (Page header) -->
    <section class="content-header">
        <ol class="breadcrumb">
            <li><i class="fa fa-home"></i></li>
            <li class="active">Data verification (Onshore)</li>
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

<!-------- STANDARD GAS ----------------------------------------------------------------------------------->
<div class="col-md-6" runat="server" id="divSTANDARD" visible="false">
         <table border="0" style="border-spacing: 2px;border-collapse: separate; padding: 5px;" class="text-black">
                <tr>
                    <td style="text-align: right; width: 150px;">Cylinder Standard&nbsp;:</td>
                    <td style="text-align: left; width: 90px;">
                        <asp:Label ID="lblCYLINDER_NO" runat="server" Text=""></asp:Label>
                    </td>
                    <td style="text-align: right; width: 100px;">Order date&nbsp;:</td>
                    <td style="text-align: left; width: 80px;">
                        <asp:Label ID="lblORDER_DATE" runat="server" Text=""  CssClass="cell-center"  Width="80" Height="27"></asp:Label>
                    </td>
                    <td style="text-align: right; width: 100px;">Expiry date&nbsp;:</td>
                    <td style="text-align: left; width: 80px;">
                        <asp:Label ID="lblEXPIRE_DATE" runat="server" Text="" CssClass="cell-center cell-bg-nopass"  Width="80" Height="27"></asp:Label>
                    </td>
                </tr>

            </table>
</div>

<!----------------------------------------------------------------------------------------------------------------------->

<!------------------------------------------>

     <asp:GridView ID="gvResult" runat="server" HorizontalAlign="Left" AutoGenerateColumns="False" DataKeyNames="ADATE" GridLines="Both"  
         OnRowDataBound="gvResult_RowDataBound" OnRowCreated="gvResult_RowCreated" PageSize="10"  ShowFooter="True" Width="2240">
                            <HeaderStyle CssClass="" HorizontalAlign="Center" />
                            <FooterStyle CssClass="ItemFooter_green" HorizontalAlign="Center" />
                            <RowStyle CssClass="itemRow1" HorizontalAlign="Left" VerticalAlign="Middle" />
                            <PagerSettings Mode="NumericFirstLast" PageButtonCount="10" />
                            <PagerStyle CssClass="pagination-ys cell-borderW1" />
                            <AlternatingRowStyle CssClass="itemRow2" HorizontalAlign="Left" VerticalAlign="Middle" />
                            <Columns>

                                <asp:TemplateField HeaderText="">
                                    <HeaderStyle Width="30px" CssClass="Table-head-gray cell-center cell-middle" />
                                    <ItemStyle Width="30px" CssClass="cell-center cell-middle cell-border" />
                                    <ItemTemplate> 

                                    </ItemTemplate>
                                    <FooterTemplate></FooterTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Edit">
					                <HeaderStyle CssClass="Table-head-gray cell-center" Width="40px" />
                                    <ItemStyle CssClass="cell-center cell-top cell-border" Width="40px" />
                                     <ItemTemplate>
                                        <li class="fa fa-pencil-square-o fa-lg" style="color: #003399;" onclick="DoAction('EDIT',<%# Container.DataItemIndex %>)"></li>
                                    </ItemTemplate>
                                    <FooterStyle CssClass="cell-center cell-Middle cell-border" />
                                </asp:TemplateField>

                                <asp:BoundField HeaderText="TIME" DataField="ADATE" DataFormatString="{0:dd/MM/yyyy}" >
                                    <HeaderStyle CssClass="Table-head-gray cell-center" Width="90px"/>
                                    <ItemStyle CssClass="cell-center cell-Middle cell-border" Width="90px"/>
                                    <FooterStyle CssClass="cell-center cell-Middle cell-border" />
                                </asp:BoundField>

                                <asp:BoundField HeaderText="CH4" DataField="" >
                                    <HeaderStyle CssClass="Table-head-gray cell-center" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="70px"/>
                                    <FooterStyle CssClass="cell-right cell-Middle cell-border" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="C2H6" DataField="" >
                                    <HeaderStyle CssClass="Table-head-gray cell-center" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="70px"/>
                                    <FooterStyle CssClass="cell-right cell-Middle cell-border" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="C3H8" DataField="" >
                                    <HeaderStyle CssClass="Table-head-gray cell-center" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="70px"/>
                                    <FooterStyle CssClass="cell-right cell-Middle cell-border" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="IC4H10" DataField="" >
                                    <HeaderStyle CssClass="Table-head-gray cell-center" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="70px"/>
                                    <FooterStyle CssClass="cell-right cell-Middle cell-border" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="NC4H10" DataField="" >
                                    <HeaderStyle CssClass="Table-head-gray cell-center" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="70px"/>
                                    <FooterStyle CssClass="cell-right cell-Middle cell-border" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="IC5H12" DataField="" >
                                    <HeaderStyle CssClass="Table-head-gray cell-center" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="70px"/>
                                    <FooterStyle CssClass="cell-right cell-Middle cell-border" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="NC5H12" DataField="" >
                                    <HeaderStyle CssClass="Table-head-gray cell-center" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="70px"/>
                                    <FooterStyle CssClass="cell-right cell-Middle cell-border" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="C6H14" DataField="" >
                                    <HeaderStyle CssClass="Table-head-gray cell-center" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="70px"/>
                                    <FooterStyle CssClass="cell-right cell-Middle cell-border" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="CO2" DataField="" >
                                    <HeaderStyle CssClass="Table-head-gray cell-center" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="70px"/>
                                    <FooterStyle CssClass="cell-right cell-Middle cell-border" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="N2" DataField="" >
                                    <HeaderStyle CssClass="Table-head-gray cell-center" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="70px"/>
                                    <FooterStyle CssClass="cell-right cell-Middle cell-border" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="H2S" DataField="" >
                                    <HeaderStyle CssClass="Table-head-gray cell-center" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="70px"/>
                                    <FooterStyle CssClass="cell-right cell-Middle cell-border" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="NETHVDRY" DataField="" >
                                    <HeaderStyle CssClass="Table-head-gray cell-center" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="70px"/>
                                    <FooterStyle CssClass="cell-right cell-Middle cell-border" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="HVSAT" DataField="" >
                                    <HeaderStyle CssClass="Table-head-gray cell-center" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="70px"/>
                                    <FooterStyle CssClass="cell-right cell-Middle cell-border" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="SG" DataField="" >
                                    <HeaderStyle CssClass="Table-head-gray cell-center" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="70px"/>
                                    <FooterStyle CssClass="cell-right cell-Middle cell-border" />
                                </asp:BoundField>
                                  <asp:BoundField HeaderText="H2O" DataField="" >
                                    <HeaderStyle CssClass="Table-head-gray cell-center" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="70px"/>
                                    <FooterStyle CssClass="cell-right cell-Middle cell-border" />
                                </asp:BoundField>
                               
                                <asp:BoundField HeaderText="UNNORMMIN" DataField="" >
                                    <HeaderStyle CssClass="Table-head-gray cell-center" Width="90px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="90px"/>
                                    <FooterStyle CssClass="cell-right cell-Middle cell-border" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="UNNORMMAX" DataField="" >
                                    <HeaderStyle CssClass="Table-head-gray cell-center" Width="90px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="90px"/>
                                    <FooterStyle CssClass="cell-right cell-Middle cell-border" />
                                </asp:BoundField>
                                 <asp:BoundField HeaderText="UNNORM" DataField="" >
                                    <HeaderStyle CssClass="Table-head-gray cell-center" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="70px"/>
                                    <FooterStyle CssClass="cell-right cell-Middle cell-border" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="WI" DataField="" >
                                    <HeaderStyle CssClass="Table-head-gray cell-center" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="70px"/>
                                    <FooterStyle CssClass="cell-right cell-Middle cell-border" />
                                </asp:BoundField>
                              
                                <asp:BoundField HeaderText="SUM" DataField="" >
                                    <HeaderStyle CssClass="Table-head-gray cell-center" Width="70px"/>
                                    <ItemStyle CssClass="cell-bg-sum cell-right cell-Middle cell-border" Width="70px"/>
                                    <FooterStyle CssClass="cell-bg-sum cell-right cell-Middle cell-border" />
                                </asp:BoundField>

                                 <asp:BoundField HeaderText="DATE" DataField="" DataFormatString="{0:dd/MM/yyyy}" >
                                    <HeaderStyle CssClass="Table-head-orange cell-center" Width="70px"/>
                                    <ItemStyle CssClass="cell-center cell-Middle cell-border" Width="70px"/>
                                     <FooterStyle CssClass="cell-right cell-Middle cell-border" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="HVSAT" DataField=""  >
                                    <HeaderStyle CssClass="Table-head-orange cell-center" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="70px"/>
                                    <FooterStyle CssClass="cell-right cell-Middle cell-border" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="RUN" DataField="" DataFormatString="{0:#,##0}" >
                                    <HeaderStyle CssClass="Table-head-orange cell-center" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="70px"/>
                                    <FooterStyle CssClass="cell-right cell-Middle cell-border" />
                                </asp:BoundField>
                                
                                <asp:BoundField HeaderText="H2O" DataField=""  >
                                    <HeaderStyle CssClass="Table-head-primary cell-center" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="70px"/>
                                    <FooterStyle CssClass="cell-right cell-Middle cell-border" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="H2O" DataField=""  >
                                    <HeaderStyle CssClass="Table-head-primary cell-center" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="70px"/>
                                    <FooterStyle CssClass="cell-right cell-Middle cell-border" />
                                </asp:BoundField>
                                 <asp:TemplateField HeaderText="เลือก Site">
					                <HeaderStyle CssClass="Table-head-primary cell-center" Width="80px" />
                                    <ItemStyle CssClass="cell-center cell-top cell-border" Width="80px" />
                                    <FooterStyle CssClass="cell-right cell-Middle cell-border" />
                                </asp:TemplateField>

                                <asp:BoundField HeaderText="(Primary name)" DataField="" >
                                    <HeaderStyle CssClass="Table-head-success cell-center" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="70px"/>
                                    <FooterStyle CssClass="cell-right cell-Middle cell-border" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="(Seconde name)" DataField="" >
                                    <HeaderStyle CssClass="Table-head-success cell-center" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="70px"/>
                                    <FooterStyle CssClass="cell-right cell-Middle cell-border" />
                                </asp:BoundField>

                                 <asp:TemplateField HeaderText="สำรอง">
					                <HeaderStyle CssClass="Table-head-danger cell-center" Width="80px" />
                                    <ItemStyle CssClass="cell-center cell-top cell-border" Width="80px" />
                                     <FooterStyle CssClass="cell-right cell-Middle cell-border" />
                                </asp:TemplateField>
                                 

                            </Columns>
                        </asp:GridView>
                        
<!------------------------------------------>
                    </div>

                    <div class=" box-footer">
<!---------------------------------------------------------->
<asp:Panel ID="pnlCONFIRM" runat="server">
                        <table border="0" style="margin-left: 10%;">
                            <tr>
                                <td style="width: 280px">
                                    <asp:Panel ID="pnlComfirm1" runat="server" Visible="false">
                                    <input name="btnConfirm1" runat="server" class="btn btn-block btn-success" type="button" id="btnConfirm1" value="Confirm (daily)" onclick="javascript: DoAction('CONFIRM_1','');" style="width: 115px" modal-confirm /> 
                                    </asp:Panel>
                                    <asp:Panel ID="pnlComfirm12" runat="server" Visible="false">
                                    <input name="btnConfirm12" class="btn btn-block btn-success disabled" type="button" id="btnConfirm12" value="Confirm (daily)"  style="width: 115px" /> 
                                    </asp:Panel>
                                </td>
                                
                                <td style="width: 280px">
                                   <asp:Panel ID="pnlComfirm4" runat="server" Visible="false">
                                   <input name="btnConfirm4" runat="server" class="btn btn-block btn-info" type="button" id="btnConfirm4" value="Confirm (20)" onclick="javascript: DoAction('CONFIRM_4','');" style="width: 105px" modal-confirm /> 
                                   </asp:Panel>
                                    <asp:Panel ID="pnlComfirm42" runat="server" Visible="false">
                                        <input name="btnConfirm42" class="btn btn-block btn-primary disabled" type="button" id="btnConfirm42" value="Confirm (20)" style="width: 105px"/> 
                                    </asp:Panel>
                                </td>

                                <td style="width: 280px">
                                   <asp:Panel ID="pnlComfirm2" runat="server" Visible="false">
                                   <input name="btnConfirm2" runat="server" class="btn btn-block btn-primary" type="button" id="btnConfirm2" value="Confirm (27)" onclick="javascript: DoAction('CONFIRM_2','');" style="width: 105px" modal-confirm /> 
                                   </asp:Panel>
                                    <asp:Panel ID="pnlComfirm22" runat="server" Visible="false">
                                        <input name="btnConfirm22" class="btn btn-block btn-primary disabled" type="button" id="btnConfirm22" value="Confirm (27)" style="width: 105px"/> 
                                    </asp:Panel>
                                </td>
                                <td style="width: 280px">
                                   <asp:Panel ID="pnlComfirm3" runat="server" Visible="false">
                                   <input name="btnConfirm3" runat="server" class="btn btn-block btn-sametogetherproject" type="button" id="btnConfirm3" value="Confirm (end month)" onclick="javascript: DoAction('CONFIRM_3','');" style="width: 145px" modal-confirm /> 
                                   </asp:Panel>
                                     <asp:Panel ID="pnlComfirm32" runat="server" Visible="false">
                                        <input name="btnConfirm32" class="btn btn-block btn-sametogetherproject disabled" type="button" id="btnConfirm32" value="Confirm (end month)"  style="width: 145px" /> 
                                    </asp:Panel>
                                </td>
                            </tr>
                            <tr><td><asp:Label ID="lblComfirm1" runat="server" CssClass="lblDisplay"></asp:Label></td>
                                <td><asp:Label ID="lblComfirm4" runat="server" CssClass="lblDisplay"></asp:Label></td>
                                <td><asp:Label ID="lblComfirm2" runat="server" CssClass="lblDisplay"></asp:Label></td>
                                <td><asp:Label ID="lblComfirm3" runat="server" CssClass="lblDisplay"></asp:Label></td>
                            </tr>
                        </table>
</asp:Panel>
                
                      </div> 

                    <div class=" box-footer">
 <!---------------------------------------------------------->
<asp:Panel ID="pnlSPOT" runat="server">

       <asp:GridView ID="gvSpot" runat="server" HorizontalAlign="Left" AutoGenerateColumns="False" GridLines="Both"  
         OnRowDataBound="gvSpot_RowDataBound" PageSize="10" ShowHeader="false" ShowFooter="false">
                            <HeaderStyle CssClass="Table-head-gray" HorizontalAlign="Center" />
                            <FooterStyle CssClass="ItemFooter_green2" HorizontalAlign="Center" />
                            <RowStyle CssClass="itemRow1" HorizontalAlign="Left" VerticalAlign="Middle" />
                            <PagerSettings Mode="NumericFirstLast" PageButtonCount="10" />
                            <PagerStyle CssClass="pagination-ys cell-borderW1" />
                            <AlternatingRowStyle CssClass="itemRow2" HorizontalAlign="Left" VerticalAlign="Middle" />
                            <Columns>

                                 <asp:TemplateField HeaderText="Edit">
					                <HeaderStyle CssClass="Table-head-gray cell-center" Width="40px" />
                                    <ItemStyle CssClass="cell-center cell-top cell-border" Width="40px" />
                                     <ItemTemplate>
                                        <li class="fa fa-pencil-square-o fa-lg" style="color: #003399;" onclick="DoAction('EDIT_SPOT',0)"></li>
                                    </ItemTemplate>
                                    <FooterStyle CssClass="cell-center cell-Middle cell-border" />
                                </asp:TemplateField>

                                <asp:BoundField HeaderText="Sampling Point" DataField=""  >
                                    <HeaderStyle CssClass="cell-center Table-head-orange" Width="110px"/>
                                    <ItemStyle CssClass="cell-left cell-Middle cell-border" Width="110px" />
                                    <FooterStyle CssClass="cell-left cell-Middle cell-border" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="Date" DataField="" DataFormatString="{0:dd/MM/yyyy}" >
                                    <HeaderStyle CssClass="cell-center Table-head-orange" Width="90px"/>
                                    <ItemStyle CssClass="cell-center cell-Middle cell-border" Width="90px"/>
                                    <FooterStyle CssClass="cell-center cell-Middle cell-border" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="H2S" DataField=""  >
                                    <HeaderStyle CssClass="cell-center Table-head-orange" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="70px" />
                                    <FooterStyle CssClass="cell-right cell-Middle cell-border" />
                                </asp:BoundField>

                                <asp:BoundField HeaderText="Sampling Point" DataField=""  >
                                    <HeaderStyle CssClass="cell-center Table-head-success" Width="110px"/>
                                    <ItemStyle CssClass="cell-left cell-Middle cell-border" Width="110px" />
                                    <FooterStyle CssClass="cell-left cell-Middle cell-border" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="Date" DataField="" DataFormatString="{0:dd/MM/yyyy}" >
                                    <HeaderStyle CssClass="cell-center Table-head-success" Width="90px"/>
                                    <ItemStyle CssClass="cell-center cell-Middle cell-border" Width="90px"/>
                                    <FooterStyle CssClass="cell-center cell-Middle cell-border" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="Hg" DataField=""  >
                                    <HeaderStyle CssClass="cell-center Table-head-success" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="70px" />
                                    <FooterStyle CssClass="cell-right cell-Middle cell-border" />
                                </asp:BoundField>

                                <asp:BoundField HeaderText="Sampling Point" DataField=""  >
                                    <HeaderStyle CssClass="cell-center Table-head-primary" Width="110px"/>
                                    <ItemStyle CssClass="cell-left cell-Middle cell-border" Width="110px" />
                                    <FooterStyle CssClass="cell-left cell-Middle cell-border" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="Date" DataField="" DataFormatString="{0:dd/MM/yyyy}" >
                                    <HeaderStyle CssClass="cell-center Table-head-primary" Width="90px"/>
                                    <ItemStyle CssClass="cell-center cell-Middle cell-border" Width="90px"/>
                                    <FooterStyle CssClass="cell-center cell-Middle cell-border" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="O2" DataField=""  >
                                    <HeaderStyle CssClass="cell-center Table-head-primary" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="70px" />
                                    <FooterStyle CssClass="cell-right cell-Middle cell-border" />
                                </asp:BoundField>

                                <asp:BoundField HeaderText="Sampling Point" DataField=""  >
                                    <HeaderStyle CssClass="cell-center Table-head-danger" Width="110px"/>
                                    <ItemStyle CssClass="cell-left cell-Middle cell-border" Width="110px" />
                                    <FooterStyle CssClass="cell-left cell-Middle cell-border" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="Date" DataField="" DataFormatString="{0:dd/MM/yyyy}" >
                                    <HeaderStyle CssClass="cell-center Table-head-danger" Width="90px"/>
                                    <ItemStyle CssClass="cell-center cell-Middle cell-border" Width="90px"/>
                                    <FooterStyle CssClass="cell-center cell-Middle cell-border" />
                                </asp:BoundField>
                                <asp:BoundField HeaderText="Temp" DataField=""  >
                                    <HeaderStyle CssClass="cell-center Table-head-danger" Width="70px"/>
                                    <ItemStyle CssClass="cell-right cell-Middle cell-border" Width="70px" />
                                    <FooterStyle CssClass="cell-right cell-Middle cell-border" />
                                </asp:BoundField>
   
                                 
                            </Columns>
                        </asp:GridView>
               
                    
</asp:Panel>                    
                    
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
