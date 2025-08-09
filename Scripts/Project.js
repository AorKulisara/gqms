

function JSFooter(Msg, PageAction){
    if (Msg != ""){
        alert(Msg);
    } else {
        eval(PageAction);
    }
}
function ShowReport(ReportName, ParamData, FileType){
    var url = "";
   
	if (typeof FileType == 'undefined') { FileType = ""; }
	if (typeof ParamData == 'undefined' || ParamData == "") {
	    if (ReportName != "") {
	        url = "../RPT/ReportViewer.aspx?RptName=" + ReportName + "&FileType=" + FileType;
	        window.open(url, "", "directories=no,location=no,menubar=no,scrollbars=yes,toolbar=no,width=950,height=680,resizable=yes");
	    }
	} else {
        if (ReportName != "" && ParamData != "") {
            url = "../RPT/ReportViewer.aspx?RptName=" + ReportName + "&ParamData=" + ParamData + "&FileType=" + FileType;
		    window.open(url, "", "directories=no,location=no,menubar=no,scrollbars=yes,toolbar=no,width=950,height=680,resizable=yes");
	    }
	}
	
}

function ShowExcel(ReportFile, ParamData, Criteria, FileType) {
    var url = "";
    if (ReportFile != "" && ParamData != "") {
        url = "../RPT/ReportViewer.aspx?RptName=" + ReportFile + "&ParamData=" + ParamData + "&Criteria=" + Criteria + "&FileType=" + FileType;
        window.open(url, "", "directories=no,location=no,menubar=no,scrollbars=yes,toolbar=no,width=950,height=680,resizable=yes");
    }
}


//-- เปลี่ยนจาก alert() เป็น MsgModal()
function Result(Key, GoPage) {

    switch (Key) {
        case "ST": MsgModal("Submitted."); break;
        case "CM": MsgModal("WDR completed."); break;        
        case "SD": MsgModal("Save completed."); window.opener.RefreshPage(); window.close(); break;
        case "AE": MsgModal("Approve completed."); break;
        case "RJ": MsgModal("Reject completed."); break;
        case "S": MsgModal("Save completed."); break;
        case "S1": MsgModalclosePage("Save completed."); break;
        case "S2": MsgModalgoPage("Save completed.",GoPage); break;
        case "S3": MsgModal("Successful in saving data."); window.opener.RefreshPage(); break;
        case "D": MsgModal("Delete completed."); break;           
        case "D1": MsgModalclosePage("Delete completed."); break;
        case "D2": MsgModalgoPage("Delete completed.", GoPage); break;
        case "C": MsgModal("Cannot delete this data."); break;
        case "N": window.location.href = GoPage; break;
        case "N_FRAME": parent.window.location.href = GoPage; break;
        case "N_POPUP": opener.window.location.href = GoPage; break;
        case "E": MsgModalgoPage("Cannot use this page.","../Includes/Main.aspx"); break;
        case "E1": window.close(); break;
        case "E2": MsgModalclosePage("Cannot use this page."); break;
        case "FU": MsgModal("File deleted."); break;
        case "FE": MsgModal("Cannot delete this file."); break;
        case "UU": MsgModal("File transfer completed."); break;
        case "UE": MsgModal("Cannot transfer this file."); break;
        case "FI": MsgModal("Invalid file type."); break;
        case "USD": MsgModal("This data is already exist."); break;
        case "EE": MsgModal("Cannot export this data."); break;
        case "UC": MsgModal("Successful in upload file."); break;
        case "DFE": MsgModal("Unable to delete this file."); break;
        case "DFC": MsgModal("Successful in deleting file."); break;
        case "EC":
            //MsgModal("Export completed."); //-- edit 25/07/2023
            if (GoPage != "" && GoPage != "undefined") {
                GoPage = sURL(GoPage); 
                window.open(GoPage);
            }
            break;
    }
}

var oldly_class;
function ShowPhoto(srcOld, CtrlImageName, FileFullName) {
    if (!srcOld.contains(event.fromElement)) {
        srcOld.style.cursor = 'hand';
        oldly_class = srcOld.className;
        srcOld.className = 'TableSelectBar';
    }
    document.getElementById(CtrlImageName).src = FileFullName;
    document.getElementById(CtrlImageName).style.display = 'block';
}
function HidePhoto(srcOld, CtrlImageName) {
    if (!srcOld.contains(event.toElement)) {
        srcOld.style.cursor = 'default';
        srcOld.className = oldly_class;
    }
    document.getElementById(CtrlImageName).style.display = 'none';
}

function AttachFile(FullFileNameWithPath) {
    window.open(FullFileNameWithPath, '_blank', 'width=700, height=1200');
}

function OpenDocumentData(ParamStr) {
    var settingsStr = 'directories=no,location=no,menubar=no,resizable=yes,status=yes,toolbar=no,scrollbars=no,top=100,left=100,width=1000,height=550';
    window.open('DocumentDetailPopUp.aspx?' + ParamStr, '_blank', settingsStr);
}