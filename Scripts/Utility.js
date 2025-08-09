/***** Control *****/
function GetCtrl(CtrlName) {
    return document.getElementById(CtrlName);
}
function GetCtrlValue(CtrlName) {
    var val = "";
    if (document.getElementById(CtrlName) != null) {
        val = document.getElementById(CtrlName).value;
    }
    return val;
}

function SetCtrlValue(CtrlName, Value) {
    if (document.getElementById(CtrlName) != null) {
        document.getElementById(CtrlName).value = Value;
    }
}

function GetOpenerCtrlValue(CtrlName) {
    var val = "";
    if (window.opener.document.getElementById(CtrlName) != null) {
        val = window.opener.document.getElementById(CtrlName).value;
    }
    return val;
}

function SetOpenerCtrlValue(CtrlName, Value) {
    if (window.opener.document.getElementById(CtrlName) != null) {
        window.opener.document.getElementById(CtrlName).value = Value;
    }
}

function SetOpenerInnerText(CtrlName, Value) {
    if (window.opener.document.getElementById(CtrlName) != null) {
        window.opener.document.getElementById(CtrlName).innerText = Value;
    }
}
function GetOpenerInnerText(CtrlName) {
    var val = "";
    if (window.opener.document.getElementById(CtrlName) != null) {
        val = window.opener.document.getElementById(CtrlName).innerText;
    }
    return val;
}

function SetInnerText(CtrlName, Value) {
    if (document.getElementById(CtrlName) != null) {
        document.getElementById(CtrlName).innerText = Value;
    }
}

function GetInnerText(CtrlName) {
    var val = "";
    if (document.getElementById(CtrlName) != null) {
        val = document.getElementById(CtrlName).innerText;
    }
    return val;
}

function GetParentCtrlValue(CtrlName) {
    var val = "";
    if (window.parent.document.getElementById(CtrlName) != null) {
        val = window.parent.document.getElementById(CtrlName).value;
    }
    return val;
}

function SetParentCtrlValue(CtrlName, Value) {
    if (window.parent.document.getElementById(CtrlName) != null) {
        window.parent.document.getElementById(CtrlName).value = Value;
    }
}

function SetParentInnerText(CtrlName, Value) {
    if (window.parent.document.getElementById(CtrlName) != null) {
        window.parent.document.getElementById(CtrlName).innerText = Value;
    }
}

function SetParentInnerText(CtrlName, Value) {
    if (window.parent.document.getElementById(CtrlName) != null) {
        window.parent.document.getElementById(CtrlName).innerText = Value;
    }
}

function GetInnerText(CtrlName) {
    var val = "";
    if (document.getElementById(CtrlName) != null) {
        val = document.getElementById(CtrlName).innerText;
    }
    return val;
}

function SubmitForm(FrmName) {
    document.forms[0].submit();
//    if (FrmName == null) {
//        document.forms[0].submit();
//    } else {
//        eval("document." + FrmName + ".submit();");
//    }
}

function SetMethodForm(FrmName, Method) {
    eval("document." + FrmName + ".method='" + Method + "';");
}

function SetActionForm(FrmName, Url) {
    eval("document." + FrmName + ".action='" + Url + "';");
}

function SetTargetForm(FrmName, Target) {
    eval("document." + FrmName + ".target='" + Target + "';");
}

function SubmitOpenerForm(FrmName) {
    eval("window.opener.document." + FrmName + ".submit();");
}

//--- ต้องส่งมาเป็น  .UniqueID  นะ
function GetValueRadioList(Name) {
    var ret = "";
    var rbs = document.getElementsByName(Name);
    for (var i = 0; i < rbs.length; i++) {
        if (rbs[i].checked) {
            ret = rbs[i].value;
        }
    }

    return ret;
}

function GetValueRadio(Name) {
    var checkList1 = document.getElementById(Name);
    var checkBoxList1 = checkList1.getElementsByTagName("input");
    var ret = "";
    if (checkBoxList1.checked) {
        ret = checkBoxList1.value;
    }
    return ret;
}


function GetValueCheckBoxList(Name) {
    var checkList1 = document.getElementById(Name);
    var checkBoxList1 = checkList1.getElementsByTagName("input");
    var checkBoxSelectedItems1 = new Array();
    var ret = "";
    for (var i = 0; i < checkBoxList1.length; i++) {
        if (checkBoxList1[i].checked) {
            checkBoxSelectedItems1.push(checkBoxList1[i].value);
            if (ret != "") ret = ret + ",";
            ret = ret + checkBoxList1[i].value;
        }
    }
    return ret;
}

function GetDescFromDropDown(FrmName, Name) {
    var sel = eval("document." + FrmName + "." + Name + ".selectedIndex");
    var txt = "";
    if (sel >= 0) { txt = eval("document." + FrmName + "." + Name + ".options[" + sel + "].text"); }
    return txt;
}



/***** Style *****/
var old_bgcolor;
var old_class;
var old_bgcolor1;
var old_class1;
function ShowBar(src) {
    if (!src.contains(event.fromElement)) {
        src.style.cursor = 'hand';
        old_class = src.className;
        src.className = 'TableSelect';
    }
}
function HideBar(src) {
    if (!src.contains(event.toElement)) {
        src.style.cursor = 'default';
        src.className = old_class;
    }
}
function ShowBar2(src) {
    if (!src.contains(event.fromElement)) {
        src.style.cursor = 'hand';
        old_class1 = src.className;
        src.className = 'TableSelectAltGreen';
    }
}
function HideBar2(src) {
    if (!src.contains(event.toElement)) {
        src.style.cursor = 'default';
        src.className = old_class1;
    }
}
function ShowBar1(src) {
    if (!src.contains(event.fromElement)) {
        src.style.cursor = 'hand';
        old_class1 = src.className;
        src.className = 'TableSelectAlt';
    }
}
function HideBar1(src) {
    if (!src.contains(event.toElement)) {
        src.style.cursor = 'default';
        src.className = old_class1;
    }
}


/***** Utility *****/
function ToNum(N) {
    if (N != '') {
        if (typeof (N) == 'string') {
            var convertedString = N.split(',');
            convertedString = convertedString.join('');
            return parseFloat(convertedString);
        } else {
            return N;
        }
    } else {
        return 0;
    }
}
function RoundNumber(rnum, rlength) {
    // The number of decimal places to round to
    if (rnum > 8191 && rnum < 10485) {
        rnum = rnum - 5000;
        var newnumber = Math.round(rnum * Math.pow(10, rlength)) / Math.pow(10, rlength);
        newnumber = newnumber + 5000;
    } else {
        var newnumber = Math.round(rnum * Math.pow(10, rlength)) / Math.pow(10, rlength);
    }
    return newnumber;
}

function FormatNumber(num, decimalNum, bolLeadingZero, bolParens, bolCommas)
/**********************************************************************
IN:
NUM - the number to format
decimalNum - the number of decimal places to format the number to
bolLeadingZero - true / false - display a leading zero for
numbers between -1 and 1
bolParens - true / false - use parenthesis around negative numbers
bolCommas - put commas as number separators.
 
RETVAL:
The formatted number!
**********************************************************************/
{
    if (isNaN(parseFloat(num))) return "0";

    var tmpNum = num;
    var iSign = num < 0 ? -1 : 1; 	// Get sign of number

    // Adjust number so only the specified number of numbers after
    // the decimal point are shown.
    tmpNum *= Math.pow(10, decimalNum);
    tmpNum = Math.round(Math.abs(tmpNum))
    tmpNum /= Math.pow(10, decimalNum);
    tmpNum *= iSign; 				// Readjust for sign


    // Create a string object to do our formatting on
    var tmpNumStr = new String(tmpNum);

    // See if we need to strip out the leading zero or not.
    if (!bolLeadingZero && num < 1 && num > -1 && num != 0)
        if (num > 0)
        tmpNumStr = tmpNumStr.substring(1, tmpNumStr.length);
    else
        tmpNumStr = "-" + tmpNumStr.substring(2, tmpNumStr.length);

    // See if we need to put in the commas
    if (bolCommas && (num >= 1000 || num <= -1000)) {
        var iStart = tmpNumStr.indexOf(".");
        if (iStart < 0)
            iStart = tmpNumStr.length;

        iStart -= 3;
        while (iStart >= 1) {
            tmpNumStr = tmpNumStr.substring(0, iStart) + "," + tmpNumStr.substring(iStart, tmpNumStr.length)
            iStart -= 3;
        }
    }

    // See if we need to use parenthesis
    if (bolParens && num < 0)
        tmpNumStr = "(" + tmpNumStr.substring(1, tmpNumStr.length) + ")";

    return tmpNumStr; 	// Return our formatted string!
}

function FormatNum(num, digits) {
    var result = "";

    if (num != null && num != "") {
        num = num.replace(/,/g, "");
        if (!isNaN(parseFloat(num))) {
            result = parseFloat(num).toFixed(digits).replace(/\d(?=(\d{3})+\.)/g, '$&,');
        }
    }

    return result;    
}


function isDate(DateToCheck) {
    if (DateToCheck == "") { return true; }
    var m_strDate = FormatDate(DateToCheck);
    if (m_strDate == "") {
        return false;
    }

    var m_arrDate = m_strDate.split("."); //-- edit 13/05/2017 -- "/" ->"."
    var m_DAY = m_arrDate[0];
    var m_MONTH = m_arrDate[1];
    var m_YEAR = m_arrDate[2];
    if (m_YEAR.length > 4) { return false; }
    if (m_YEAR < 1000) { return false; }
    var d = new Date();
    m_YEAR = parseInt(m_YEAR);
    if (m_YEAR > 2500) {
        m_YEAR = m_YEAR - 543;
    }
    m_strDate = m_MONTH + "/" + m_DAY + "/" + m_YEAR;
    var testDate = new Date(m_strDate);
    if (testDate.getMonth() + 1 == m_MONTH) {
        return true;
    } else {
        return false;
    }
}
function FormatDate(DateToFormat, FormatAs) {
    if (DateToFormat == "") { return ""; }
    if (!FormatAs) { FormatAs = "dd/MM/yyyy"; }
    var strReturnDate;
    FormatAs = FormatAs.toLowerCase();
    DateToFormat = DateToFormat.toLowerCase();
    var arrDate;
    var arrMonths = new Array("January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December");
    var strMONTH;
    var Separator;
    while (DateToFormat.indexOf("st") > -1) {
        DateToFormat = DateToFormat.replace("st", "");
    }
    while (DateToFormat.indexOf("nd") > -1) {
        DateToFormat = DateToFormat.replace("nd", "");
    }
    while (DateToFormat.indexOf("rd") > -1) {
        DateToFormat = DateToFormat.replace("rd", "");
    }
    while (DateToFormat.indexOf("th") > -1) {
        DateToFormat = DateToFormat.replace("th", "");
    }
    if (DateToFormat.indexOf(".") > -1) {
        Separator = ".";
    }
    if (DateToFormat.indexOf("-") > -1) {
        Separator = "-";
    }
    if (DateToFormat.indexOf("/") > -1) {
        Separator = "/";
    }
    if (DateToFormat.indexOf(" ") > -1) {
        Separator = " ";
    }
    arrDate = DateToFormat.split(Separator);
    DateToFormat = "";
    for (var iSD = 0; iSD < arrDate.length; iSD++) {
        if (arrDate[iSD] != "") {
            DateToFormat += arrDate[iSD] + Separator;
        }
    }
    DateToFormat = DateToFormat.substring(0, DateToFormat.length - 1);
    arrDate = DateToFormat.split(Separator);
    if (arrDate.length < 3) {
        return "";
    }
    var DAY = arrDate[0];
    var MONTH = arrDate[1];
    var YEAR = arrDate[2];
    if (parseFloat(arrDate[1]) > 12) {
        DAY = arrDate[1];
        MONTH = arrDate[0];
    }
    if (parseFloat(DAY) && DAY.toString().length == 4) {
        YEAR = arrDate[0];
        DAY = arrDate[2];
        MONTH = arrDate[1];
    }
    for (var iSD = 0; iSD < arrMonths.length; iSD++) {
        var ShortMonth = arrMonths[iSD].substring(0, 3).toLowerCase();
        var MonthPosition = DateToFormat.indexOf(ShortMonth);
        if (MonthPosition > -1) {
            MONTH = iSD + 1;
            if (MonthPosition == 0) {
                DAY = arrDate[1];
                YEAR = arrDate[2];
            }
            break;
        }
    }
    var strTemp = YEAR.toString();
    if (strTemp.length == 2) {
        if (parseFloat(YEAR) > 40) {
            YEAR = "19" + YEAR;
        } else {
            YEAR = "20" + YEAR;
        }
    }
    if (parseInt(MONTH) < 10 && MONTH.toString().length < 2) {
        MONTH = "0" + MONTH;
    }
    if (parseInt(DAY) < 10 && DAY.toString().length < 2) {
        DAY = "0" + DAY;
    }
    //-- edit 19/07/2019 --
    if (DAY.toString().length > 2) {
        DAY = Left(DAY, 2);
    }

    switch (FormatAs) {
        case "dd.mm.yyyy":
            return DAY + "." + MONTH + "." + YEAR;
        case "dd/mm/yyyy":
            return DAY + "/" + MONTH + "/" + YEAR;
        case "mm/dd/yyyy":
            return MONTH + "/" + DAY + "/" + YEAR;
        case "dd/mmm/yyyy":
            return DAY + " " + arrMonths[MONTH - 1].substring(0, 3) + " " + YEAR;
        case "mmm/dd/yyyy":
            return arrMonths[MONTH - 1].substring(0, 3) + " " + DAY + " " + YEAR;
        case "dd/mmmm/yyyy":
            return DAY + " " + arrMonths[MONTH - 1] + " " + YEAR;
        case "mmmm/dd/yyyy":
            return arrMonths[MONTH - 1] + " " + DAY + " " + YEAR;
        case "yyyymmdd":
            return YEAR + "" + MONTH + "" + DAY;
    }
    return DAY + "." + strMONTH + "." + YEAR; ;
}

function CheckDateRange(DateStart, DateEnd) {
    var Check = true;
    if (DateStart != "" && DateEnd != "") {
        var DStart = FormatDate(DateStart, "yyyymmdd");
        var DEnd = FormatDate(DateEnd, "yyyymmdd");
        if (DStart > DEnd) {
            Check = false;
        }
    }
    return Check
}

function MM_findObj(n, d) { //v4.01
    var p, i, x; if (!d) d = document; if ((p = n.indexOf("?")) > 0 && parent.frames.length) {
        d = parent.frames[n.substring(p + 1)].document; n = n.substring(0, p);
    }
    if (!(x = d[n]) && d.all) x = d.all[n]; for (i = 0; !x && i < d.forms.length; i++) x = d.forms[i][n];
    for (i = 0; !x && d.layers && i < d.layers.length; i++) x = MM_findObj(n, d.layers[i].document);
    if (!x && d.getElementById) x = d.getElementById(n); return x;
}

function MM_preloadImages() { //v3.0
    var d = document; if (d.images) {
        if (!d.MM_p) d.MM_p = new Array();
        var i, j = d.MM_p.length, a = MM_preloadImages.arguments; for (i = 0; i < a.length; i++)
            if (a[i].indexOf("#") != 0) { d.MM_p[j] = new Image; d.MM_p[j++].src = a[i]; } 
    }
}

function MM_swapImgRestore() { //v3.0
    var i, x, a = document.MM_sr; for (i = 0; a && i < a.length && (x = a[i]) && x.oSrc; i++) x.src = x.oSrc;
}

function MM_swapImage() { //v3.0
    var i, j = 0, x, a = MM_swapImage.arguments; document.MM_sr = new Array; for (i = 0; i < (a.length - 2); i += 3)
        if ((x = MM_findObj(a[i])) != null) { document.MM_sr[j++] = x; if (!x.oSrc) x.oSrc = x.src; x.src = a[i + 2]; }
}

//function  IsNumeric( strValue ) {
//  var objRegExp  =  /^\d{1,3}(,\d\d\d)*\.\d\d$|^\d+\.\d\d$/;

//  //check for numeric characters
//  return objRegExp.test(strValue);
//}
function IsNumeric(sText) {
    var ValidChars = "0123456789.,";
    var IsNumber = true;
    var Char;
    for (i = 0; i < sText.length && IsNumber == true; i++) {
        Char = sText.charAt(i);
        if (ValidChars.indexOf(Char) == -1) {
            IsNumber = false;
        }
    }
    return IsNumber;
}
function IsEnglish(sText) {
    var ValidChars = " 0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ.,*&#@?><{}/-+$%()_=|[]:;'";
    var IsEnglishs = true;
    var Char;
    for (i = 0; i < sText.length && IsEnglishs == true; i++) {
        Char = sText.charAt(i);
        if (ValidChars.indexOf(Char) == -1) {
            IsEnglishs = false;
        }
    }
    return IsEnglishs;
}

function IsTime(sText) {
    var ValidChars = "0123456789:-";
    var IsTimes = true;
    var Char;
    for (i = 0; i < sText.length && IsTimes == true; i++) {
        Char = sText.charAt(i);
        if (ValidChars.indexOf(Char) == -1) {
            IsTimes = false;
        }
    }
    return IsTimes;
}

function IsNumberData(sText) {
    var ValidChars = "0123456789";
    var IsNumType = true;
    var Char;
    for (i = 0; i < sText.length && IsNumType == true; i++) {
        Char = sText.charAt(i);
        if (ValidChars.indexOf(Char) == -1) {
            IsNumType = false;
        }
    }
    return IsNumType;
}

function IsEMailList(MailList) {
    var IsEmail = true;
    var MailArr;
    MailList = MailList.replace(",", ";");
    MailArr = MailList.split(";");
    for (i = 0; i < MailArr.length && IsEmail == true; i++) {
        var EMailid = MailArr[i];
        if (IsEmailCheck(EMailid) == true) {
            IsEmail = true;
        } else {
            IsEmail = false;
        }
    }
    return IsEmail;
    //if (MailList.indexOf("@")==-1){
    //	return false;
    //} 
    //else { return true; }
}

function IsEmailCheck(emailStr) {
    // checks if the e-mail address is valid
    var Isemail = true;

    var r1 = new RegExp("(@.*@)|(\\.\\.)|(@\\.)|(^\\.)");
    var r2 = new RegExp("^.+\\@(\\[?)[a-zA-Z0-9\\-\\.]+\\.([a-zA-Z]{2,3}|[0-9]{1,3})(\\]?)$");
    var isOK = !r1.test(emailStr) && r2.test(emailStr);
    if (emailStr.length < 1) isOK = true;
    //var emailPat = /^(\".*\"|[A-Za-z0-9]\w*)@(\[\d{1,3}(\.\d{1,3}){3}]|[A-Za-z0-9]\w*(\.[A-Za-z0-9]\w*)+)$/;
    //var matchArray = emailStr.match(emailPat);
    //	if (matchArray == null) {
    //alert("Your email address seems incorrect.  Please try again (check the '@' and '.'s in the email address)");
    //		Isemail= false;
    return isOK;
}

function IsPhoneData(sText) {
    var ValidChars = "#0123456789,-#*+() ต่อ ext";
    var IsPhoneType = true;
    var Char;
    for (i = 0; i < sText.length && IsPhoneType == true; i++) {
        Char = sText.charAt(i);
        if (ValidChars.indexOf(Char) == -1) {
            IsPhoneType = false;
        }
    }
    return IsPhoneType;
}

function IsValidTime(value) {
    var colonCount = 0;
    var hasMeridian = false;
    for (var i = 0; i < value.length; i++) {
        var ch = value.substring(i, i + 1);
        if ((ch < '0') || (ch > '9')) {
            if ((ch != ':') && (ch != ' ') && (ch != 'a') && (ch != 'A') && (ch != 'p') && (ch != 'P') && (ch != 'm') && (ch != 'M')) {
                return false;
            }
        }
        if (ch == ':') { colonCount++; }
        if ((ch == 'p') || (ch == 'P') || (ch == 'a') || (ch == 'A')) { hasMeridian = true; }
    }
    if ((colonCount < 1) || (colonCount > 2)) { return false; }
    var hh = value.substring(0, value.indexOf(":"));
    if ((parseFloat(hh) < 0) || (parseFloat(hh) > 23)) { return false; }
    if (hasMeridian) {
        if ((parseFloat(hh) < 1) || (parseFloat(hh) > 12)) { return false; }
    }
    if (colonCount == 2) {
        var mm = value.substring(value.indexOf(":") + 1, value.lastIndexOf(":"));
    } else {
        var mm = value.substring(value.indexOf(":") + 1, value.length);
    }
    if ((parseFloat(mm) < 0) || (parseFloat(mm) > 59)) { return false; }
    if (colonCount == 2) {
        var ss = value.substring(value.lastIndexOf(":") + 1, value.length);
    } else {
        var ss = "00";
    }
    if ((parseFloat(ss) < 0) || (parseFloat(ss) > 59)) { return false; }
    return true;
}

function Mid(str, start, len)
/***
IN: str - the string we are LEFTing
start - our string's starting position (0 based!!)
len - how many characters from start we want to get

RETVAL: The substring from start to start+len
***/
{
    // Make sure start and len are within proper bounds
    if (start < 0 || len < 0) return "";

    var iEnd, iLen = String(str).length;
    if (start + len > iLen)
        iEnd = iLen;
    else
        iEnd = start + len;

    return String(str).substring(start, iEnd);
}

function InStr(strSearch, charSearchFor)
/*
InStr(strSearch, charSearchFor) : Returns the first location a substring (charSearchFor) found in the string strSearch. (If the character is not found, -1 is returned.)
*/
{
    for (i = 0; i < strSearch.length; i++) {
        if (charSearchFor == Mid(strSearch, i, 1)) {
            return i;
        }
    }
    return -1;
}

function Asc(CHARACTER) {
    return CHARACTER.charCodeAt(0);
}

function Right(STRING, CHARACTER_COUNT) {
    return STRING.substring((STRING.length - CHARACTER_COUNT), STRING.length);
}


var CharList = " 0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_@#$%^&*()-+/=.,";
var CharCount = 79;
var MaxKeyLen = 20;

function Key2Char(num) {
    return CharList.charAt(num % CharCount);
}

function Key2Num(Key) {
    return CharList.indexOf(Key);
}

function EncodeKey(SecretKey, Data) {
    var X, X0;
    var S, t, EncodedKey;
    var Valid = true;
    var I;
    var EncKey;
    var ch;

    S = SecretKey + Data;
    X0 = 12;
    for (I = 0; I < S.length; I++) {
        X0 = X0 + S.charCodeAt(I);
        if (Key2Num(S.charAt(I)) == -1) Valid = false;
    }
    X0 = X0 % CharCount;
    X = X0;
    if (!Valid) {
        return "";
    }

    EncodedKey = Key2Char(X0);
    S = "          " + SecretKey + SecretKey + SecretKey + SecretKey;
    S = S.substr(S.length - MaxKeyLen, MaxKeyLen)
    t = "          " + Data + Data + Data + Data;
    t = t.substr(t.length - MaxKeyLen, MaxKeyLen);
    for (I = 0; I < MaxKeyLen; I++) {
        X = X + 55 + Key2Num(S.charAt(I)) - Key2Num(t.charAt(I));
        EncodedKey = EncodedKey + Key2Char(X);
    }
    EncodedKey = EncodedKey + Key2Char(Math.floor(X / CharCount)) + Key2Char(X0 + Data.length);
    return EncodedKey;
}


function expand(ID) {
    document.getElementById('item' + ID).style.display = 'block';
    document.getElementById('expander' + ID).innerHTML = '<a class="expander" href="javascript:collapse(' + ID + ')" title="Collapse">[-]</a>';
}
function collapse(ID) {
    document.getElementById('item' + ID).style.display = 'none';
    document.getElementById('expander' + ID).innerHTML = '<a class="expander" href="javascript:expand(' + ID + ')" title="Expand">[+]</a>';
}

//---------------------------------
// กำหนดให้เปลี่ยนสีพื้น text box ตอน Click ที่ textbox นั้นๆ

var highlightcolor = "#f0fff0"

var ns6 = document.getElementById && !document.all
var previous = ''
var eventobj

//Regular expression to highlight only form elements
//var intended=/INPUT|TEXTAREA|SELECT|OPTION/
var intended = /INPUT|TEXTAREA|OPTION/

//Function to check whether element clicked is form element
function checkel(which) {
    if (which.style && intended.test(which.tagName)) {
        if (ns6 && eventobj.nodeType == 3)
            eventobj = eventobj.parentNode.parentNode
        return true
    }
    else
        return false
}

//Function to highlight form element
function highlight(e) {
    eventobj = ns6 ? e.target : event.srcElement
    if (previous != '') {
        if (checkel(previous))
            previous.style.backgroundColor = ''
        previous = eventobj
        if (checkel(eventobj))
            eventobj.style.backgroundColor = highlightcolor
    }
    else {
        if (checkel(eventobj))
            eventobj.style.backgroundColor = highlightcolor
        previous = eventobj
    }
}

//---------------------------------
function Left(str, n)
/***
IN: str - the string we are LEFTing
n - the number of characters we want to return

RETVAL: n characters from the left side of the string
***/
{
    if (n <= 0)     // Invalid bound, return blank string
        return "";
    else if (n > String(str).length)   // Invalid bound, return
        return str;                // entire string
    else // Valid bound, return appropriate substring
        return String(str).substring(0, n);
}

function getMonth(MON) {
    var Month = 0;
    switch (MON) {
        case "Jan": Month = 1; break;
        case "Feb": Month = 2; break;
        case "Mar": Month = 3; break;
        case "Apr": Month = 4; break;
        case "May": Month = 5; break;
        case "Jun": Month = 6; break;
        case "Jul": Month = 7; break;
        case "Aug": Month = 8; break;
        case "Sep": Month = 9; break;
        case "Oct": Month = 10; break;
        case "Nov": Month = 11; break;
        case "Dec": Month = 12; break;
    }
    return Month;
}

function imposeMaxLength(Object, MaxLen) {
    if (Object.value.length > MaxLen) {
        Object.value = Object.value.substr(0, MaxLen);
    }
}

function CheckRange(FVal, TVal) {
    var Check = false;
    if (FVal != "" && TVal != "") {
        if ((IsNumberData(FVal) || IsNumeric(FVal)) && (IsNumberData(FVal) || IsNumeric(FVal))) {
            if (parseFloat(FVal) > parseFloat(TVal)) {
                Check = true;
            }
        }
    }
    return Check;
}

//Validate Password 
function ValidateEnglishLower(sText) {
    var ValidChars = "abcdefghijklmnopqrstuvwxyz";
    var ValidateEng = false;
    var Char;
    for (i = 0; i < sText.length && ValidateEng == false; i++) {
        Char = sText.charAt(i);
        if (ValidChars.indexOf(Char) > -1) {
            ValidateEng = true;
        }
    }
    return ValidateEng;
}
function ValidateEnglishUpper(sText) {
    var ValidChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    var ValidateEng = false;
    var Char;
    for (i = 0; i < sText.length && ValidateEng == false; i++) {
        Char = sText.charAt(i);
        if (ValidChars.indexOf(Char) > -1) {
            ValidateEng = true;
        }
    }
    return ValidateEng;
}

function ValidateNumber(sText) {
    var ValidChars = "0123456789";
    var ValidateNum = false;
    var Char;
    for (i = 0; i < sText.length && ValidateNum == false; i++) {
        Char = sText.charAt(i);
        if (ValidChars.indexOf(Char) > -1) {
            ValidateNum = true;
        }
    }
    return ValidateNum;
}

function ValidateSpecChar(sText) {
    var ValidChars = ".#@?/-+_|[]";
    var ValidateSpecChars = false;
    var Char;
    for (i = 0; i < sText.length && ValidateSpecChars == false; i++) {
        Char = sText.charAt(i);
        if (ValidChars.indexOf(Char) > -1) {
            ValidateSpecChars = true;
        }
    }
    return ValidateSpecChars;
}

function ValidatePassword(Password) {
    var ValidatePasswords = false;
    if (ValidateEnglishLower(Password) && ValidateEnglishUpper(Password) && ValidateNumber(Password) && ValidateSpecChar(Password)) {
        ValidatePasswords = true;
    }
    return ValidatePasswords;
}
var UserSpecialChar = "!><`'|&:;$@,^\"/{}[]?+-()=#%*";
var Password = "!>,<`&:$^\'\"()=%*:{}";
function HaveSpecChar(sText, Type) {
    var ValidChars = UserSpecialChar;
    if (Type == "PASSWORD") { ValidChars = Password; }
    var ValidateSpecChars = false;
    var Char;
    for (i = 0; i < sText.length && ValidateSpecChars == false; i++) {
        Char = sText.charAt(i);
        if (ValidChars.indexOf(Char) > -1) {
            ValidateSpecChars = true;
        }
    }
    return ValidateSpecChars;
}

function openWindowWithPost(url, name, setValue, keys, values) {
    var newWindow = window.open("../Includes/PopupPage.htm", name, setValue);
    if (!newWindow) {
        newWindow = window.open("Includes/PopupPage.htm", name, setValue);
        if (!newWindow) {
            return false;
        }
    }
    var html = "";
    html += "<html><head></head><body><form id='formid' method='post' action='" + url + "'>";
    if (keys && values && (keys.length == values.length))
        for (var i = 0; i < keys.length; i++)
        html += "<input type='hidden' name='" + keys[i] + "' value='" + values[i] + "'/>";
    html += "</form><sc" + "ript type='text/javascript'>document.getElementById(\"formid\").submit()</sc" + "ript></body></html>";
    newWindow.document.write(html);
    return newWindow;
}

function openLinkWithPost(url, name, setValue, keys, values) {
    var newWindow = window.open("../Includes/PopupPage.htm", "_self");
    var html = "";
    html += "<html><head></head><body><form id='formid' method='post' action='" + url + "'>";
    if (keys && values && (keys.length == values.length))
        for (var i = 0; i < keys.length; i++)
        html += "<input type='hidden' name='" + keys[i] + "' value='" + values[i] + "'/>";
    html += "</form><sc" + "ript type='text/javascript'>document.getElementById(\"formid\").submit()</sc" + "ript></body></html>";
    newWindow.document.write(html);

    return newWindow;
}

function openTabWithPost(url, name, setValue, keys, values) {
    var newWindow = window.open("../Includes/PopupPage.htm", name);
    var html = "";
    html += "<html><head></head><body><form id='formid' method='post' action='" + url + "'>";
    if (keys && values && (keys.length == values.length))
        for (var i = 0; i < keys.length; i++)
        html += "<input type='hidden' name='" + keys[i] + "' value='" + values[i] + "'/>";
    html += "</form><sc" + "ript type='text/javascript'>document.getElementById(\"formid\").submit()</sc" + "ript></body></html>";
    newWindow.document.write(html);

    return newWindow;
}

function GetDate(ctrlName, DateSel) {
    document.getElementById(ctrlName).value = DateSel;
    //alert(document.getElementById("ctl00_CTSubForm_txtExpectDate").value)
    SetCtrlValue(ctrlName, DateSel);
    
}

function Calendar_Open(ctrlname) {
    var d;
    d = GetCtrlValue(ctrlname);
    window.open("../Includes/Calendar.aspx?CtrlName=" + ctrlname + "&CurrentDate=" + d + "&Lang=EN", "", "height=290,width=310");
}



// Set Inner Text IE & FireFox
function setTextContent(element, text) {
    while (element.firstChild !== null)
        element.removeChild(element.firstChild); // remove all existing content 
    element.appendChild(document.createTextNode(text));
}

// onkeypress="EnterFindData(event,'FormName', 'ServerAction')"
function EnterFindData(evt, FormName, ServerAction) {
    var charCode = (evt.which) ? evt.which : event.keyCode
    if (charCode == 13) {
        SetCtrlValue("ServerAction", ServerAction);
        SubmitForm(FormName);
    }
}
var MM = "";
var YY = "";
// Return Value MM = Parameter Month For CrystalReport
//              YY = Parameter Year For CrystalReport
// Ex. ShowReport("Receive_TaxCard.rpt", "MM," + MM + ",YY," + YY);
function GetMMYY(ctrlNameDate) {
    var txtDate;
    var Year;
    var valDate = new Array();
    txtDate = GetCtrlValue(ctrlNameDate);
    if (txtDate != "") {
        valDate = txtDate.split('/');
        MM = valDate[1];
        if (ToNum(valDate[2]) < 2400) {
            Year = ToNum(valDate[2]) + 543;
        }
        else {
            Year = ToNum(valDate[2]);
        }
        YY = Year.toString();
        //        if (valDate[2].length == 4) {
        //            if (ToNum(valDate[2]) > 2400) {
        //                Year = ToNum(valDate[2]) - 543;
        //            }
        //            YY = Year.toString().substring(2);
        //        }
    }
}

function ClearDropDown(CtrlName) {
    var obj = document.getElementById(CtrlName);
    if (obj != null) {
        var cntDDL = obj.options.length;
        for (l = 0; l < cntDDL; l++) {
            //Note: Always remove(0) and NOT remove(i)
            obj.remove(0);
        }
    }
}

function lstDropdownSelected(CtrlName, valAtLeast) {
    var obj = document.getElementById(CtrlName);
    var selected = false;
    var cntAtLeast = 0;
    if (obj != null) {
        var cntDDL = obj.options.length;
        for (l = 0; l < cntDDL; l++) {
            //Note: Always remove(0) and NOT remove(i)
            if (obj.options[l].checked) {
                cntAtLeast = cntAtLeast + 1;
                if (cntAtLeast == valAtLeast) {
                    selected = true;
                    break;
                }
            }
        }
    }
    return selected;
}

function CheckSelected(CtrlName, Cnt, valAtLeast) {
    var isSelected = false;
    var cName;
    var cntAtLeast = 0;
    for (g = 0; g < Cnt; g++) {
        cName = CtrlName + "_" + g;
        if (GetCtrl(cName) != null) {
            if (GetCtrl(cName).checked) {
                cntAtLeast++;
                if (cntAtLeast == valAtLeast) {
                    isSelected = true;
                    break;
                }
            }
        }
    }
    return isSelected;
}

function GenerateDate(numMonth, DateFrom, DateTo) {
    var currentTime = new Date();
    var dayNow = currentTime.getDate();
    var monthNow = currentTime.getMonth() + 1;
    var yearNow = currentTime.getFullYear();

    if (parseInt(monthNow) < 1) {
        yearNow = parseInt(yearNow) - 1;
        monthNow = parseInt(monthNow) + 12
    }

    if ((monthNow == 4 || monthNow == 6 || monthNow == 9 || monthNow == 11) && dayNow == 31) { dayNow = 30; }
    else if (monthNow == 2 && dayNow > 28) {
        if (isleap(yearNow) == true) { dayNow = 29; }
        else { dayNow = 28; }
    }

    var retValueNow = dayNow + "/" + monthNow + "/" + yearNow;
    SetCtrlValue(DateTo, retValueNow);

    var day = currentTime.getDate();
    var month = (currentTime.getMonth() + 1) - parseInt(numMonth);
    var year = currentTime.getFullYear();
    if (parseInt(month) < 1) {
        year = parseInt(year) - 1;
        month = parseInt(month) + 12
    }

    if ((month == 4 || month == 6 || month == 9 || month == 11) && day == 31) { day = 30; }
    else if (month == 2 && day > 28) {
        if (isleap(year) == true) { day = 29; }
        else { day = 28; }
    }


    var retValue = day + "/" + month + "/" + year;

    SetCtrlValue(DateFrom, retValue);

}

function isleap(yr) {

    if ((parseInt(yr) % 4) == 0) {
        if (parseInt(yr) % 100 == 0) {
            if (parseInt(yr) % 400 != 0) {
                return "false";
            }
            if (parseInt(yr) % 400 == 0) {
                return "true";
            }
        }
        if (parseInt(yr) % 100 != 0) {
            return "true";
        }
    }
    if ((parseInt(yr) % 4) != 0) {
        return "false";
    }
}

function To_Num(N) {
    if (N != '') {
        if (typeof (N) == 'string') {
            var convertedString = N.split(',');
            convertedString = convertedString.join('');
            //-- aor edit 06/01/2014 --
            if (isNaN(parseFloat(convertedString))) {
                return "invalid";
            }
            else {
                return parseFloat(convertedString);
            }

        } else {
            return N;
        }
    } else {
        return 0;
    }
}

function LinkPage(pPageName, pQStr) {
    var PageName = "";
    var QStr = "";
    PageName = pPageName.toString();
    QStr = pQStr.toString();
    if (PageName != "") {
        if (QStr != "") {
            PageName += "?" + QStr;
        }
        window.location.href = PageName;
    }
}

function LockBackSpaceAndDelete(e) { // ใช้ใน event ของ element เช่น onkeyup="return LockBackSpaceAndDelete()"
    var code;
    if (!e) var e = window.event; // some browsers don't pass e, so get it from the window
    if (e.keyCode) code = e.keyCode; // some browsers use e.keyCode
    else if (e.which) code = e.which;  // others use e.which

    if (code == 8 || code == 46)
        return false;
}


//---------------------------------------------------------
//-- allow enter number ---
function isNumber(evt) {
    evt = (evt) ? evt : window.event;

    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode == 44 || charCode == 46 || (charCode >= 47 && charCode <= 57))
         return true;
    else
        return false;
}

//-- allow enter only ---
function isNumberOnly(evt) {
    evt = (evt) ? evt : window.event;
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode >= 47 && charCode <= 57)
        return true;
    else
        return false;
}


//---------------------------------------------------------