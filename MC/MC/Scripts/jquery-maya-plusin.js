
var msg_stat = {
    Danger: "注意",
    Info: "提示",
    Success: "成功",
    Warning: "警告"
};


// add remove method in Array
Array.prototype.remove = function () {
    var what, a = arguments, L = a.length, ax;
    while (L && this.length) {
        what = a[--L];
        while ((ax = this.indexOf(what)) !== -1) {
            this.splice(ax, 1);
        }
    }
    return this;
};


// JS 去除空白
function trim(str) {
    var start = -1, end = str.length;
    while (str.charCodeAt(--end) < 33);
    while (str.charCodeAt(++start) < 33);
    return str.slice(start, end + 1);
}

// 左邊補字
function padLeft(str, lenght, addStr) {
    if (str.toString().length >= lenght)
        return str;
    else
        return padLeft(addStr + str.toString(), lenght, addStr);
}

// 右邊補字
function padRight(str, lenght, addStr) {
    if (str.toString().length >= lenght)
        return str;
    else
        return padRight(str.toString() + addStr, lenght, addStr);
}

//複製到剪貼簿
function copyClip(meintext) {
    if (window.clipboardData) {
        window.clipboardData.setData("Text", meintext);
    }
    else if (window.netscape) {
        netscape.security.PrivilegeManager.enablePrivilege('UniversalXPConnect');
        var clip = Components.classes['@mozilla.org/widget/clipboard;1'].createInstance(Components.interfaces.nsIClipboard);
        if (!clip) return;
        var trans = Components.classes['@mozilla.org/widget/transferable;1'].createInstance(Components.interfaces.nsITransferable);
        if (!trans) return;
        trans.addDataFlavor('text/unicode');
        var str = new Object();
        var len = new Object();
        var str = Components.classes["@mozilla.org/supports-string;1"].createInstance(Components.interfaces.nsISupportsString);
        var copytext = meintext;
        str.data = copytext;
        trans.setTransferData("text/unicode", str, copytext.length * 2);
        var clipid = Components.interfaces.nsIClipboard;
        if (!clip) return false;
        clip.setData(trans, null, clipid.kGlobalClipboard);
    }
    return false;
}

// 顯示bootstrap警示
function showAlert(msg, stat, def_wait, position) {
    //alert(msg);//修改為alert 20160909
    //return;
    //closeAlert();

    //20180209 新增方法 snackbar
    def_wait = typeof def_wait !== 'undefined' ? def_wait : 2000;
    var _style = 'alert fade_in ';
    //switch (stat) {
    //    case msg_stat.Danger:
    //        _style += " alert-danger";
    //        msg = "<span class='glyphicon glyphicon-remove-sign'></span>&nbsp;" + msg;
    //        break;
    //    case msg_stat.Success:
    //        //def_wait = 1000;
    //        _style += " alert-success";
    //        msg = "<span class='glyphicon glyphicon-ok-sign'></span>&nbsp;" + msg;
    //        break;
    //    case msg_stat.Warning:
    //        _style += " alert-warning";
    //        msg = "<span class='glyphicon glyphicon-info-sign'></span>&nbsp;" + msg;
    //        break;
    //    case msg_stat.Info:
    //    default:
    //        //def_wait = 1000;
    //        _style += "alert-info";
    //        msg = "<span class='glyphicon glyphicon-exclamation-sign'></span>&nbsp;" + msg;
    //        break;
    //}
    ////$('#snackbar-container .snackbar').not('.snackbar-opened').remove();//清空已經顯示過的訊息 20180313
    //$.snackbar({
    //    content: msg,
    //    style: _style,
    //    timeout: def_wait,
    //    htmlAllowed: true
    //});

    //return false;

    switch (stat) {
        case msg_stat.Danger:
            _style += " alert-danger";
            msg = msg;
            break;
        case msg_stat.Success:
            //def_wait = 1000;
            _style += " alert-success";
     
            break;
        case msg_stat.Warning:
            _style += " alert-warning";
           
            break;
        case msg_stat.Info:
        default:
            //def_wait = 1000;
            _style += "alert-info";
           
            break;
    }
    msg = msg;
    if (!!msg) {
        msg = msg.replace('\n', '<br \>');
    }
    def_wait = typeof def_wait !== 'undefined' ? def_wait : 5000;
    position = typeof position !== 'undefined' ? position : 'bottom';

    switch (position) {
        case 'top':
            $("#msgbox").css("top", '55px');
            $("#msgbox").css("bottom", '');
            break;
        case 'bottom':
            $("#msgbox").css("top", '');
            $("#msgbox").css("bottom", '5px');
            break;
    }

    switch (stat) {
        case msg_stat.Danger:
            $("#msgbox").removeClass("alert-danger").removeClass("alert-info").removeClass("alert-success").removeClass("alert-warning").addClass("alert-danger");
            $("#msgbox strong").html("<span class='glyphicon glyphicon-remove-sign'></span>&nbsp;");
            break;
        case msg_stat.Info:
        default:
            def_wait = 1000;
            $("#msgbox").removeClass("alert-danger").removeClass("alert-info").removeClass("alert-success").removeClass("alert-warning").addClass("alert-info");
            $("#msgbox strong").html("<span class='glyphicon glyphicon-exclamation-sign'></span>&nbsp;");
            break;
        case msg_stat.Success:
            def_wait = 1000;
            $("#msgbox").removeClass("alert-danger").removeClass("alert-info").removeClass("alert-success").removeClass("alert-warning").addClass("alert-success");
            $("#msgbox strong").html("<span class='glyphicon glyphicon-ok-sign'></span>&nbsp;");
            break;
        case msg_stat.Warning:
            $("#msgbox").removeClass("alert-danger").removeClass("alert-info").removeClass("alert-success").removeClass("alert-warning").addClass("alert-warning");
            $("#msgbox strong").html("<span class='glyphicon glyphicon-info-sign'></span>&nbsp;");
            break;
    }

    $("#msgbox_text").html(msg);

    $("#msgbox").fadeIn({
        duration: 500,
        easing: "easeOutCirc",
        complete: function () {
            setTimeout(
              function () {
                  closeAlert();
              }, def_wait);
        }
    });

}

// 關閉bootstrap警示
function closeAlert() {
    $("#msgbox").fadeOut({
        druation: 1000,
        easing: "easeInCirc"
    });
}

var myControl = {
    create: function (tp_inst, obj, unit, val, min, max, step) {
        $('<input class="ui-timepicker-input" value="' + val + '" style="width:50%">')
        .appendTo(obj)
        .spinner({
            min: min,
            max: max,
            step: step,
            change: function (e, ui) {
                // key events
                // don't call if api was used and not key press
                if (e.originalEvent !== undefined)
                    tp_inst._onTimeChange();
                tp_inst._onSelectHandler();
            },
            spin: function (e, ui) { // spin events
                tp_inst.control.value(tp_inst, obj, unit, ui.value);
                tp_inst._onTimeChange();
                tp_inst._onSelectHandler();
            }
        });
        return obj;
    },
    options: function (tp_inst, obj, unit, opts, val) {
        if (typeof (opts) == 'string' && val !== undefined)
            return obj.find('.ui-timepicker-input').spinner(opts, val);
        return obj.find('.ui-timepicker-input').spinner(opts);
    },
    value: function (tp_inst, obj, unit, val) {
        if (val !== undefined)
            return obj.find('.ui-timepicker-input').spinner('value', val);
        return obj.find('.ui-timepicker-input').spinner('value');
    }
};

//置中指定的 DOM 元素
(function (a, b) {
    jQuery.fn.center = function (c) {
        return this.each(function () {
            if (c == true) {
                $(this).css("position", "fixed"); $(this).css("top", "50%");
                var d = Math.round(Math.max(0, ($(this).outerHeight() / 2))) * -1; $(this).css("margin-top", d + "px");
                $(this).css("left", "50%"); var e = Math.round(Math.max(0, ($(this).outerWidth() / 2))) * -1;
                $(this).css("margin-left", e + "px");
            }
            else {
                $(this).css("position", "absolute");
                $(this).css("top", Math.max(0, (($(a).outerHeight() / 2) - $(this).outerHeight() / 2)));
                $(this).css("left", Math.max(0, (($(a).outerWidth() / 2) - $(this).outerWidth() / 2)));
            }
        })
    };
})(window);

$.fn.serializeObject = function () {
    var o = {};
    var a = this.serializeArray();
    $.each(a, function () {
        if (o[this.name] !== undefined) {
            if (!o[this.name].push) {
                o[this.name] = [o[this.name]];
            }
            o[this.name].push(this.value || '');
        } else {
            o[this.name] = this.value || '';
        }
    });
    return o;
};

//階層參數反解
//EX: {"ptinfo.loc": "K"} 轉 {"ptinfo": {"loc": "K"}}
//可使用於bootstrap-table-flatJSON.js 參數反解
function convert_flat_all(obj) {
    var res_all = [];
    for (var i = 0; i < obj.length; i++) {
        res_all.push(convert_flat(obj[i]));
    }
    return res_all;
}

function convert_flat(obj) {
    var res = {}, i, j, splits, ref, key;
    for (i in obj) {
        if (obj.hasOwnProperty(i)) {
            splits = i.split('.');
            ref = res;
            for (j = 0; j < splits.length; j++) {
                key = splits[j];
                if (j == splits.length - 1) {
                    ref[key] = obj[i];
                } else {
                    //check is array
                    if (isNaN(splits[j + 1])) {
                        ref = ref[key] = ref[key] || {};
                    } else {
                        ref = ref[key] = ref[key] || [];
                    }
                }
            }
        };
    }
    return res;
}

//階層參數反解 方法二
function convert_flat_obj(obj) {
    var result = {};
    eachKeyValue(obj, function (namespace, value) {
        var parts = namespace.split(".");
        var last = parts.pop();
        var node = result;
        parts.forEach(function (key) {
            node = node[key] = node[key] || {};
        });
        node[last] = value;
    });
    return result;
}
function eachKeyValue(obj, fun) {
    for (var i in obj) {
        if (obj.hasOwnProperty(i)) {
            fun(i, obj[i]);
        }
    }
}

// 比對欄位是否相同
function chk_compare(obj_id, ref_obj_id) {
    if ($(obj_id).is(":visible")) {
        if ($(obj_id).val() !== $(ref_obj_id).val()) {
            $(ref_obj_id).addClass("error").change(function () {
                $(ref_obj_id).removeClass("error").unbind("change");
            });
            return false;
        } else
            return true;
    } else
        return true;
}

// 判斷是否為空的
function chk_empty(obj_id) {
    var chk_flag = true;
    if ($(obj_id).length > 0) {
        $(obj_id).each(function () {

            switch ($(this).attr("type").toLowerCase()) {
                case "checkbox":
                    if (!$(this).prop("chkecked")) {
                        $(this).addClass("text_empty").change(function () {
                            $(this).removeClass("text_empty").unbind("change");
                        });
                        chk_flag = false;
                    }
                    break;
                case "text":
                    if ($(this).val() === "") {
                        $(this).addClass("text_empty").change(function () {
                            $(this).removeClass("text_empty").unbind("change");
                        });
                        chk_flag = false;
                    }
                    break;
            }
        });
    }
    return chk_flag;
}


//用POST傳遞encodeURIComponent參數，另開新頁
function openPostWindow(url, post_json, en_sw) {
    en_sw = typeof en_sw !== 'undefined' ? en_sw : false;
    var new_win = window.open();
    var tempForm = new_win.document.createElement("form");
    tempForm.method = "post";
    tempForm.action = url;
    for (var data_name in post_json) {
        var hideInput = document.createElement("input");
        hideInput.type = "hidden";
        hideInput.name = data_name;
        if (en_sw) {
            hideInput.value = encodeURIComponent(post_json[data_name]);
        } else {
            hideInput.value = post_json[data_name];
        }
        tempForm.appendChild(hideInput);
    }
    new_win.document.write(tempForm.outerHTML);
    tempForm.submit();
}

function replace_all(strOrg, strFind, strReplace) {
    var index = 0;
    while (strOrg.indexOf(strFind, index) != -1) {
        strOrg = strOrg.replace(strFind, strReplace);
        index = strOrg.indexOf(strFind, index);
    }
    return strOrg
}

//datetimepicker 函式
function maya_datetimepicker(selector) {
    $(selector).each(function () {
        var dt_option = {
            format: 'YYYY-MM-DD HH:mm',
            locale: moment.locale('zh-tw'),
            dayViewHeaderFormat: 'YYYY MMM',
            sideBySide: false,
            ignoreReadonly: true,
            widgetParent: null,
            toolbarPlacement: 'bottom',
            showClose: true,
            showTodayButton: true,
            showClear: true,
            keepOpen: false,
            useCurrent: false,
            debug: false,
            widgetPositioning: {
                horizontal: 'auto',
                vertical: 'auto'
            }
        };
        if (typeof $(this).attr("dt-sidebyside") != 'undefined') {
            dt_option.sideBySide = Boolean($(this).attr("dt-sidebyside"));
        }
        if (typeof $(this).attr("dt-widgetparent") != 'undefined') {
            dt_option.widgetParent = $(eval($(this).attr("dt-widgetparent")));
        }
        switch ($(this).attr("dt-widgetpositioning")) {
            case "left":
                dt_option.widgetPositioning.horizontal = 'left';
                break;
            case "right":
                dt_option.widgetPositioning.horizontal = 'right';
                break;
            case "top":
                dt_option.widgetPositioning.vertical = 'top';
                break;
            case "bottom":
                dt_option.widgetPositioning.vertical = 'bottom';
                break;
        }

        switch ($(this).attr("dt-format")) {
            case "dt":
                dt_option.format = "YYYY-MM-DD HH:mm";
                break;
            case "t":
                dt_option.format = "HH:mm";
                break;
            case "d":
                dt_option.format = "YYYY-MM-DD";
                break;
        }

        $(this).datetimepicker(dt_option);

    });
}