//2016/10/05 新增 onlySetThis (class) 判斷只有有此classh才要帶入，預設都要帶入資料
function fillJsonDom(formid, jsonItem,onlySetThis) {
    var temp_form = $('#' + formid);
    var block = temp_form.find('.form-block');
    var control = temp_form.find('.form-control');
    var filterdata = function (data, id_value) {
        if (data != undefined && data != null) {
            return data.filter(
                function (data) { return data.id == id_value; }
                )[0];
        }
        return undefined;
    };
    var showdisabled = function (ctrl) {
        //2016/09/22 新增 data-hide-disable == true 才可隱藏 disabled == true 的項目判斷
        if (ctrl.data('hide-disable') != undefined && ctrl.data('hide-disable') == true) {
            if (ctrl.is(':disabled')) {
                if (ctrl.is(':checkbox, :radio, select') && !ctrl.is(':checked')) {
                    var $p = ctrl.parent();
                    var $lbl = $p.find('label[for="' + ctrl[0].id + '"]');
                    if (($lbl == undefined || $lbl.length == 0) && $p.is('label')) {
                        $lbl = $p;
                    }
                    ctrl.hide();
                    $lbl.hide();
                }
                if (ctrl.is(':text, textarea') && $.trim(ctrl.val()) == '') {
                    ctrl.hide();
                }
            }
            if (ctrl.is('select') || ctrl.attr('type') == 'select') { //2016/09/12 調整選單判斷
                ctrl.find("option").each(function () {
                    var opt = $(this);
                    if (opt.is(':disabled') && !opt.prop('selected')) {
                        opt.wrap("<span style='display:none'></span>");
                        opt.hide();
                    }
                });
            }
            if (ctrl.is('table')) {
                var ddd = ctrl.bootstrapTable('getData');
                if (!$.isArray(ddd)) {
                    ctrl.hide();
                }
            }
        }        
    };
    var setdata = function (ctrl, d, getsub) {
        var $this = $(ctrl);
        var item_id = ctrl.id;
        if(!item_id)
            item_id = ctrl.name;
        var sub_d = (getsub) ? filterdata(d, item_id) : d;
        if (sub_d != undefined) {
            if ($this.is(':checkbox')) {
                ctrl.checked = (getsub) ? sub_d.chkd : d;
            } else if ($this.is(':radio')) {
                $this.attr('checked', (getsub) ? sub_d.val : d);
            } else if ($this.is('select') || $this.attr('type') == 'select') { //2016/09/12 調整選單判斷
                $this.val((getsub) ? sub_d.val : d);
            } else if ($this.is(':text, textarea, :hidden')) {
                $this.val((getsub) ? sub_d.txt : d);
            }
        }
        if ($this.is('table') && d != undefined) {
            $this.bootstrapTable({ data: d });
        }
        showdisabled($this);
    };
    control.each(function () {
        var cate_name = $(this).data('rcs-cate');
        var setThis = true;
        if (onlySetThis != null && onlySetThis != undefined && onlySetThis != '' && !$(this).hasClass(onlySetThis))
            setThis = false;
        if (cate_name != undefined && cate_name != '' && setThis) {
            setdata(this, jsonItem[cate_name]);
        }
    });
    block.each(function () {
        var cate_name = $(this).data('rcs-cate');
        var setThis = true;
        if (onlySetThis != null && onlySetThis != undefined && onlySetThis != '' && !$(this).hasClass(onlySetThis))
            setThis = false;
        if (cate_name != undefined && cate_name != '' && setThis) {
            var jsondata = jsonItem[cate_name];
            $(this).find('textarea, input, select, table')
                .each(function () {
                    setdata(this, jsondata, true);
                });
        }
    });
}
function getJsonObject(form_id) {
    var JsonObj = Object();
    var form_obj = $('#' + form_id);
    var block = form_obj.find('.form-block');
    var control = form_obj.find('.form-control');
    control.each(function () {
        var $ctrl = $(this);
        var cate_name = $ctrl.data('rcs-cate');
        if (!!cate_name) {
            if ($ctrl.is(':text') || $ctrl.is('textarea') || $ctrl.is(':hidden') || $ctrl.attr('type') == 'select') {
                JsonObj[cate_name] = $ctrl.val();
            } else if ($ctrl.is('select')) {
                JsonObj[cate_name] = $ctrl.find('option:selected').val();
            }
        }
    });
    block.each(function () {
        var $block = $(this);
        var cate_name = $block.data('rcs-cate');
        var items = $block.find('textarea, input, select');
        var DataJsonObj = [];
        items.each(function () {
            var itemJsonObj = Object();
            var $this = $(this);
            var is_checked = false;
            if ($this.is(':checkbox') || $this.is(':radio')) {
                is_checked = this.checked;
            }
            //itemJsonObj['cate'] = cate_name;
            itemJsonObj['id'] = $this.attr('id');
            //itemJsonObj['type'] = $this.attr('type');
            itemJsonObj['val'] = $.trim($(this).val());
            if ($this.is(':text') || $this.is('textarea') || $this.is('hidden')|| $this.attr('type') == 'select') {
                itemJsonObj['txt'] = $this.val();
            } else if ($this.is('select')) { //2016/09/07 加上選單判斷
                var tmpObj = $('#' + $(this).attr('id') + ' option:selected');
                if ($(tmpObj).attr('disabled')) {                  
                    $(tmpObj).attr('disabled', false);
                    itemJsonObj['txt'] = $.trim($(this).val()) != '' ? $(tmpObj).text() : '';
                    itemJsonObj['val'] = $(this).val();
                    $(tmpObj).attr('disabled', true);
                } else {
                    itemJsonObj['txt'] = $.trim($(this).val()) != '' ? $(tmpObj).text() : '';
                    itemJsonObj['val'] = $(this).val();
                }               
            }
            else {
                var $rcs_text = $this.data('rcs-text');
                try {
                    itemJsonObj['txt'] = eval($rcs_text)
                } catch (e) {
                    itemJsonObj['txt'] = $rcs_text ? $this.data('rcs-text') : '';
                }
            }         
            itemJsonObj['chkd'] = is_checked;
            if (Object.keys(JsonObj).indexOf(cate_name) >= 0)
                JsonObj[cate_name].push(itemJsonObj);
            else
                DataJsonObj.push(itemJsonObj);
        });
        if (Object.keys(JsonObj).indexOf(cate_name) < 0)
            JsonObj[cate_name] = DataJsonObj;
    });
    return JsonObj;
}
function setDisabledSelect(pName, pArray) {
    $("select[name=" + pName + "]").each(function () {
        $(this).find("option").each(function () {
            var t = $(this).val();
            if (pArray.indexOf(t) >= 0) $(this).attr('disabled', true);
        });
    });
}