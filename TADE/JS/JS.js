var ASYNC = true;
var loadingMessages = new Array("Loading . . .");

function initLoadingMsg() {
	$("#lblLoading").text(loadingMessages[Math.floor(Math.random() * loadingMessages.length)]);
}

function loading()
{
	initLoadingMsg();
	$("#loading").fadeIn();
}

function loaded() {
	$("#loading").fadeOut();
}

function loadingFast() {
	initLoadingMsg();
	$("#loading").fadeIn(0.1);
}

function loadedFast() {
	$("#loading").fadeOut(0.1);
}

function loadingShow() {
	initLoadingMsg();
	//$("#loading").fadeIn(0.1);
	$("#loading").show();
}

function loadingHide()
{
	$("#loading").hide();
}

function fadeBg()
{
	$("#bgImage").fadeTo(3000, 0.2);
}

function niceAlert(msg)
{
	$("#alertMessage").text(msg);
	$("#alert").dialog({
		modal: true,
		dialogClass: "no-close",
		autoOpen: true,
		show: { effect: "shake", duration: 100 },
		hide: { effect: "slide", duration: 100 },
		buttons:
		[
			{
				text: "OK",
				click: function()
				{
					$(this).dialog("close");
				}
			}
		]
	});
}

function callServerSide(url, data, success, error, allowCache)
{
	allowCache = (typeof allowCache === "undefined") ? false : allowCache;
	$.ajax({
		type: "POST",
		url: url,
		data: data,
		async: ASYNC,
		cache: allowCache,
		contentType: "application/json; charset=utf-8",
		dataType: "json",
		success: success,
		error: error
	});
}

function callServerSideGet(url, success, error, allowCache)
{
	allowCache = (typeof allowCache === "undefined") ? false : allowCache;
	$.ajax({
		type: "GET",
		url: url,
		async: ASYNC,
		cache: allowCache,
		contentType: "application/json; charset=utf-8",
		dataType: "json",
		success: success,
		error: error
	});
}

function setCookie(cname, cvalue, exdays) {
	var d = new Date();
	d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
	var expires = "expires=" + d.toUTCString();
	document.cookie = cname + "=" + cvalue + "; " + expires;
}

function getCookie(cname) {
	var name = cname + "=";
	var ca = document.cookie.split(';');
	for (var i = 0; i < ca.length; i++) {
		var c = ca[i];
		while (c.charAt(0) == ' ') c = c.substring(1);
		if (c.indexOf(name) == 0) return c.substring(name.length, c.length);
	}
	return "";
}

function checkCookie() {
	var user = getCookie("username");
	if (user != "") {
		alert("Welcome again " + user);
	} else {
		user = prompt("Please enter your name:", "");
		if (user != "" && user != null) {
			setCookie("username", user, 365);
		}
	}
}

//function convertTableToCSV($table) {

//	var $rows = $table.find('tr:has(td)');

//	// Temporary delimiter characters unlikely to be typed by keyboard
//	// This is to avoid accidentally splitting the actual contents
//    var tmpColDelim = String.fromCharCode(11); // vertical tab character
//    var tmpRowDelim = String.fromCharCode(0); // null character

//	// actual delimiter characters for CSV format
//    var colDelim = '","'
//    var rowDelim = '"\r\n"';

//	// Grab text from table into CSV formatted string
//    var csv = '"' + $rows.map(function (i, row) {
//          var $row = $(row),
//              $cols = $row.find('td');

//          return $cols.map(function (j, col) {
//            	var $col = $(col),
//                    text = $col.text();

//                return text.replace(/"/g, '""'); // escape double quotes

//          }).get().join(tmpColDelim);

//    }).get().join(tmpRowDelim)
//            .split(tmpRowDelim).join(rowDelim)
//            .split(tmpColDelim).join(colDelim) + '"';

//	// Data URI
//    var csvData = 'data:application/csv;charset=utf-8,' + encodeURIComponent(csv);

//    return csvData;
//}

function convertTableToCSV($table) {

    $rows = $table.find('tr');

    var csvData = "";

    for (var i = 0; i < $rows.length; i++) {
        var $cells = $($rows[i]).children('th,td'); //header or content cells

        for (var y = 0; y < $cells.length; y++) {
            if (y > 0) {
                csvData += ",";
            }
            var txt = ($($cells[y]).text()).toString().trim();
            if (txt.indexOf(',') >= 0 || txt.indexOf('\"') >= 0 || txt.indexOf('\n') >= 0) {
                txt = "\"" + txt.replace(/\"/g, "\"\"") + "\"";
            }
            csvData += txt;
        }
        csvData += '\n';
    }

    // Data URI
    var csv = 'data:text/csv;charset=utf-8,' + encodeURIComponent(csvData);

    return csv;
}


function AddDateTimeToFilename(filename) {
	//getting values of current time for generating the file name
	var dt = new Date();
	var day = dt.getDate();
	var month = dt.getMonth() + 1;
	var year = dt.getFullYear();
	var hour = dt.getHours();
	var mins = dt.getMinutes();
	var postfix = "_" + year + month + day + "_" + hour + mins;
	//creating a temporary HTML link element (they support setting file names)
	var newFilename = filename + postfix;
	return newFilename;
}
