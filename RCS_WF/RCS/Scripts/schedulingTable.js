(function ($) {

    $.widget('ui.schedulingForm', {
        version: "@VERSION",
        options: {
            url: undefined,
            editable: true,
            indicator: true,
            year: undefined,
            month: undefined,
            default_shift_type: 'D',
            day_work_max: 1,
            start_day: 1,
            tableid : 'schedulingTable',
            areaid: 'area_block',
            extra_col: undefined,
            /*
            [
                { type: 'text', key: 'real_off', name: '實休' },
                { type: 'text', key: 'vo_month', name: 'VO月休' },
                { type: 'text', key: 'vo_left', name: 'VO餘' },
                { type: 'text', key: 'overtime', name: '加班' },
                { type: 'text', key: 'pre_off', name: '借休' },
                { type: 'text', key: 'last_hr', name: '上月時數' },
                { type: 'text', key: 'this_hr', name: '本月時數' }
            ]
            */
            toolbar_pos: { top: 1, left: 2 },
            firefunc: $.noop
        },
        _create: function () {//page load, called once only
            this.today = new Date().setHours(0, 0, 0, 0);
            this.wkday = new Array("日", "一", "二", "三", "四", "五", "六");
        },
        _init: function () {//called everytime
            var that = this;
            this.$schedulingTable = $(this.element).addClass("schedulingTable")
                .find('#' + this.options.tableid).html('');

            if (this.options.year == undefined) {
                this.options.year = new Date().getFullYear();
            }
            if (this.options.month == undefined) {
                this.options.month = new Date().getMonth() + 1; //getMonth取得index,所以要+1
            }
            this.colncnt = 0;
            this.totalcoln = [];
            this.$table = $('<table width="100%" border="0" cellspacing="0" cellpadding="0"><thead></thead><tbody></tbody></table>')
                .data({
                    'userdaydata': new Object(),
                    'shiftsumdata': new Object(),
                    'daysumdata': { holiday: new Object(), workday: new Object() }
                });
            if (that.options.url != undefined) {
                $.getJSON(that.options.url, function (json) {
                    that._initTable(json);
                });
            };
        },
        _destory: function () {
            $(this.element).html('');
        },
        _initTable: function (schdata) {
            var myui = this;
            var userjson = schdata.schedul_data; //{day_data[], op_id, op_name, remark, s_id}
            var areajson = schdata.area_data; //{color, name}
            var shiftjson = schdata.shift_data; //{name,shortcut,work_type}
            //區域
            this._setupArea(areajson);

            this.$schedulingTable.data('schdata', schdata);
            //thead
            this._buildthead();
            //tbody
            this._buildtbody(userjson, shiftjson);
            //組成Table
            this.$schedulingTable.append(this.$table).append(this.$toolbar);
            //將資料填入畫面
            this._bindDBData();
            //註冊事件
            this._bindevent(areajson);

            this._trigger('firefunc', null, schdata);
        },
        _setupArea: function (areajson) {
            var myui = this;
            if (this.options.areaid != undefined && areajson != undefined) {
                var areablock = this._Areablock().html('');
                $.each(areajson, function (index, val) {
                    var display = myui.options.editable ? '' : 'style="display: none;"';
                    var id = 'area_' + index;
                    var checked = index == 0 ? 'checked="' + this.name + '"' : '';
                    var radio = $('<input type="radio" name="location" id="' + id + '" value="' + this.name + '" ' + checked + display + ';"/>').data('areadata', val);
                    var radioDiv = $('<div class="col-md-2"></div>').append(radio)
                        .append('<span style="color:' + this.color + ';font-size:2em;">■</span><label for="' + id + '" data-area-name-id="' + this.name + '">' + this.name + '</label>');
                    areablock.append(radioDiv);
                });

            }
        },
        _buildthead: function () {
            var myui = this;
            var start_day = this.options.start_day;
            var last_new_date = new Date(this.options.year, this.options.month - 1, 0);
            var new_date = new Date(this.options.year, this.options.month, 0);
            var last_end_day = last_new_date.getDate();
            var end_day = new_date.getDate();

            var table_str = "";
            //第一列
            table_str += '<tr><td class="htd min_w100">姓名</td>';
            if (start_day > 1) { //排班表開始日如不為1則從上個月開始計算
                var month = this.options.month - 2;
                var year = this.options.year
                if (month == -1) {
                    year = this.options.year - 1;
                    month = 11;
                }
                for (var i = start_day; i <= last_end_day ; i++) {
                    table_str += myui._buildDaytd(year, month, i);
                }
                last_end_day = start_day - 1;
                end_day = start_day - 1;
            }
            for (var i = 1; i <= end_day ; i++) {
                table_str += myui._buildDaytd(this.options.year, this.options.month - 1, i);
            }
            table_str += '<td class="htd w50">休假(天)</td><td class="htd w50">工作(天)</td>';
            this.totalcoln.push({ day: 'holiday', count: false, type: 'label' });
            this.totalcoln.push({ day: 'workday', count: false, type: 'label' });
            if (this.options.extra_col != undefined) {
                $.each(this.options.extra_col, function (index, val) {
                    table_str += '<td class="htd w50">' + val.name + '</td>';
                    myui.totalcoln.push({ day: val.key, count: false, type: val.type });
                });
            }
            table_str += '</tr>';
            this.colncnt += 2;
            ////第二列
            //table_str += '<tr><td class="htd"></td>';
            //for (var i = 1; i <= end_day ; i++) {
            //    var d = new Date(year, month - 1, i);
            //    var day_no = d.getDay();
            //    var week_color = day_no === 0 || day_no === 6 ? ' week_color' : '';

            //    table_str += '<td class="htd w50' + week_color + '">' + wkday[day_no] + '</td>';
            //}
            //table_str += '<td class="htd"></td><td class="htd"></td>';
            table_str += '</tr>';

            myui.$table.find('thead').append(table_str);

            //return table_str;
        },
        _buildDaytd: function (y, m, d) {
            var nowd = new Date(y, m, d);
            var day_no = nowd.getDay();
            var week_color = day_no == 0 || day_no == 6 ? 'week_color' : '';
            var today_class = nowd.setHours(0, 0, 0, 0) === this.today ? ' today_color' : '';
            var formatedate = nowd.getFullYear() + '/' + (nowd.getMonth() + 1) + '/' + d;
            this.colncnt++;
            var $td = '<td class="htd w50' + today_class + '">' + d +
                '<br/><span class="' + week_color + '">' + this.wkday[day_no] + '<span></td>';

            this.totalcoln.push({ day: formatedate, count: true, type: 'shift' });
            return $td;
        },
        _buildtbody: function (userjson, shiftjson) {
            var myui = this;
            $.each(userjson, function (index, user) {
                var $tr = $('<tr data-calc-user="' + user.op_id + '"></tr>').append('<td class="btd">' + user.op_name + '</td>'); //人員
                myui.getUserData()[user.op_id] = user; //將員工資料塞到tr
                myui.getDaySumData().holiday[user.op_id] = { sum: 0 };//初始化請假及工作天日數
                myui.getDaySumData().workday[user.op_id] = { sum: 0 };

                var editmode = user.day_data != null, created = false;
                $.each(myui.totalcoln, function (index, val) {
                    var show_tag = "";
                    if (!editmode) {
                        if (created == false) {
                            created = true;
                            user.day_data = new Object();
                        }
                        user.day_data[val.day] = { day: val.day, stype: '', area: '', wtype: '' };
                    }
                    else {
                        var dbdata = user.day_data[val.day];
                        if (dbdata == undefined) {
                            user.day_data[val.day] = { day: val.day, stype: '', area: '', wtype: '' };
                        }
                    }
                    var textbox = '';
                    if (val.type === 'text') {
                        textbox = '<input type="text" class="extra_text" maxlength="6"></input>';
                        show_tag = ''
                    }
                    var usertd = $('<td class="btd td_pb">' + show_tag + textbox + '</td>')
                        .data({ setdate: this.day, user_id: user.op_id, type: val.type, shift_id: '', area_id: '', work_type: '' });
                    if (val.day == 'holiday') {
                        myui.getDaySumData().holiday[user.op_id].target = usertd; //先把之後要更新的td記錄起來
                    }
                    if (val.day == 'workday') {
                        myui.getDaySumData().workday[user.op_id].target = usertd; //先把之後要更新的td記錄起來
                    }

                    $tr.append(usertd);
                });

                myui.$table.find('tbody').append($tr);
            });
            myui.$table.find('tbody').append('<tr class="split_color"><td colspan="' + (myui.totalcoln.length + 1) + '"></tr>');

            this.$toolbar = $('<div class="toolbar_container" ></div>');
            var schedualitem = $('<div class="schedualitem"></div>');
            var show = 0;
            $.each(shiftjson, function (index, shiftitem) {
                //班別統計區塊
                myui.getShiftSumData()[shiftitem.shortcut] = new Object();//初始化班別日期統計天數
                
                var $tr = $('<tr><td class="btd" data-shift-id="' + shiftitem.shortcut + '">' + shiftitem.name + '(人)</td></tr>');//.append(colnstr);
                $.each(myui.totalcoln, function (cidx, daycoln) {
                    var coln = $('<td class="btd"></td>');
                    $tr.append(coln);
                    if (daycoln.count) {
                        myui.getShiftSumData()[shiftitem.shortcut][daycoln.day] = { sum: 0, target: coln };//初始化班別日期統計天數
                    }
                });

                //Tool Bar
                var shift = $('<div class="pb_item pb_b" style="white-space: nowrap;"></div>').html(this.name)
                    .data('shiftdata', this);
                if (this.shortcut == myui.options.default_shift_type) {
                    shift.addClass('target_shift');
                }
                myui.$toolbar.append(shift);
                myui.$table.find('tbody').append($tr);
            });
            this.$toolbar.append(schedualitem);

        },
        _setShiftData: function (shift) {
            var area = this.getSelectedArea();
            var targettd = this.getSelectedtd().html('');
            var tddata = this.getSelectedDate();
            var d = { stype: shift.shortcut, area: area.name, wtype: shift.work_type };

            if (shift != undefined) {
                this._appendShiftTotd(targettd, area, shift, tddata);
            }
            this._calculate();
            var h = this._updateDBDayData(d); //更新資料
            this._setSelectedAreaRadio();
        },
        _clearShiftData: function () {
            var area = this.getSelectedArea();
            var targettd = this.getSelectedtd().html('');
            var tddata = this.getSelectedDate();

            var d = { stype: '', area: '', wtype: '' };
            var shiftItems = this.getShiftSumData();
            var dayItems = this.getDaySumData();
            var olddbdata = this._updateDBDayData();

            shiftItems[olddbdata.stype][tddata.setdate].sum--;
            if (olddbdata.wtype == 'W') {
                dayItems.workday[tddata.user_id].sum--;
            }
            if (olddbdata.wtype == 'R') {
                dayItems.holiday[tddata.user_id].sum--;
            }

            this._calculate();
            var h = this._updateDBDayData(d); //更新資料
            this._setSelectedAreaRadio();
        },
        _appendShiftTotd: function (targettd, area, shift, tddata) {
            if (!tddata.dodel) {//刪除時不加入內容
                var showdiv = $('<div style="background-color:' + area.color + ';" class="pb_gray pb_item">' + shift.shortcut +
                    '<div class="edit_toolbar"><div class="del" title="刪除"></div></div></div>');
                //註冊刪除事件
                this._on(showdiv, {
                    'click.del': function () {
                        this.getSelectedtd().data('dodel', true);
                        this._clearShiftData();
                    }
                });
                var shiftItems = this.getShiftSumData();
                var dayItems = this.getDaySumData();
                targettd.append(showdiv);
                tddata.shift_id = shift.shortcut;
                tddata.area_id = area.name;
                tddata.work_type = shift.work_type;
                var olddbdata = this._updateDBDayData();
                var cnt = 1; //刪除時減1
                var shiftdata = this.getShiftData();
                //變更班別時先把原本的班別數量-1
                if (olddbdata != undefined && olddbdata.stype != '') {
                    shiftItems[olddbdata.stype][tddata.setdate].sum--;
                    if (olddbdata.wtype == 'W') {
                        dayItems.workday[tddata.user_id].sum--;
                    }
                    if (olddbdata.wtype == 'R') {
                        dayItems.holiday[tddata.user_id].sum--;
                    }
                }
                //總計
                shiftItems[shift.shortcut][tddata.setdate].sum += cnt;
                if (shift.work_type == 'W') {
                    dayItems.workday[tddata.user_id].sum += cnt;
                }
                if (shift.work_type == 'R') {
                    dayItems.holiday[tddata.user_id].sum += cnt;
                }
            }
            else {
                this.getSelectedtd().data('dodel', false);
            }
        },
        _setSelectedAreaRadio: function () {
            if (this.options.indicator) {
                var hoverdate = this._updateDBDayData();
                if (hoverdate.area != undefined && hoverdate.area != '') {
                    this._Areablock().find('[data-area-name-id="' + hoverdate.area + '"]').addClass('target_area');
                }
                if (hoverdate.stype != undefined && hoverdate.stype != '') {
                    this.$table.find('[data-shift-id]').removeClass('target_area')
                        .end().find('[data-shift-id="' + hoverdate.stype + '"]').addClass('target_area');
                }
            }
        },
        clearSelected: function () {
            if (this.$toolbar != undefined) this.$toolbar.hide();
            if (this.$table != undefined) {
                this.$table.removeData('selectedtd')
                    .find('tr').removeClass('trSelected')
                    .find('td.target_area').removeClass('target_area')
                    .end()
                    .find('td').removeClass('tdSelected')
                    .find('[type="text"]').blur();
            }

            this._Areablock().find('.target_area').removeClass('target_area');
        },
        _showHoverItem: function (target) {
            var myui = this;
            var $me = $(target);
            var d = $me.data();
            var area = this._Areablock();
            this.$table.data('selectedtd', $me); //將目前選到的td記錄下來,選班別時帶入班別
            if (d != undefined) {
                if (d.setdate != undefined && d.type == 'shift') {
                    this.$toolbar.css({
                        'display': 'block',
                        'top': $me.offset().top + $me.height() + myui.options.toolbar_pos.top + 'px',
                        'left': $me.offset().left + $me.width() / 2 + myui.options.toolbar_pos.left + 'px'
                    });
                    $me.addClass('tdSelected').parent().addClass('trSelected');
                    this.$toolbar.show();
                    this._setSelectedAreaRadio();
                }
            }
            $me.find('.edit_toolbar').show();
        },
        _bindevent: function (areajson) {
            var myui = this;
            if (this.options.editable) {
                //移入/開td顯示班別列
                this.$table.on('mouseleave', myui.clearSelected())
                    .find('td')
                    .on({
                        'mouseenter': function () {
                            myui.clearSelected();
                            myui._showHoverItem(this);
                        },
                        'mouseleave': function () {
                            $(this).find('.edit_toolbar').hide();
                        },
                        'click': function () {
                            var d = $(this).data();
                            if (d.setdate != undefined && d.type == 'shift') {
                                var nowshift = myui.getNowShift();
                                myui._setShiftData(nowshift);
                            }
                        }
                    })
                    .end()
                    .find('input[type="text"]').on('keyup', function () {
                        var d = { stype: '', area: $(this).val(), wtype: '' };
                        myui._updateDBDayData(d); //輸入時更新DBdata
                    });

                //排班event
                var shiftlist = this.$toolbar.find('.pb_item');
                shiftlist.on('click', function () {
                    myui.getSelectedtd().data('dodel', false);
                    var $me = $(this);
                    var shiftdata = $me.data('shiftdata');
                    shiftlist.removeClass('target_shift');
                    $me.addClass('target_shift');
                    myui._setShiftData(shiftdata);
                    myui.$table.data('nowshiftdata', shiftdata);
                    myui.getSelectedtd().find('.edit_toolbar').hide();
                });

            }
        },
        _bindDBData: function () {
            var myui = this;
            var userdata = this.getUserData();
            this.$table.find('td').each(function () {
                var $td = $(this);
                var d = $td.data();
                if (d != undefined && d.setdate != undefined && d.user_id != undefined) {
                    var day_data = userdata[d.user_id].day_data;
                    var shift = day_data[d.setdate];
                    var area = shift != undefined ?
                        myui._Areablock().find('[value="' + shift.area + '"]').data('areadata') : undefined;
                    
                    if (d.type == 'shift' && shift != undefined && area != undefined) {
                        //d.dodel = false;
                        shift = myui.getNowShift(shift.stype);
                        myui._appendShiftTotd($td, area, shift, d);
                        $td.find('.edit_toolbar').hide();
                    }
                    if (d.type == 'text' && shift != undefined) {
                        var textbox = $td.find('input').val(shift.area);
                        if (!myui.options.editable) {
                            textbox.attr('disabled', '')
                        };
                    }
                }
            });
            this._calculate();
        },
        _calculate: function () {
            var shiftItems = this.getShiftSumData();
            var dayItems = this.getDaySumData();
            $.each(shiftItems, function (shiftid, sumitem) {
                $.each(sumitem, function () {
                    this.target.html(this.sum);
                })
            });

            $.each(dayItems.holiday, function () {
                this.target.html(this.sum);
            });
            $.each(dayItems.workday, function () {
                this.target.html(this.sum);
            });
        },
        _updateDBDayData: function (data) {
            var targetuser = this.getSelectedDate();
            if (targetuser != undefined) {
                var dbuser = this.getUserData(targetuser.user_id);
                if (data != undefined) {
                    dbuser.day_data[targetuser.setdate] = data;
                }
                return dbuser.day_data[targetuser.setdate];
            }
            return undefined;
        },
        _Areablock: function () {
            return $(this.element).find('#' + this.options.areaid);
        },
        getSelectedArea: function () {
            if (this.options.areaid != undefined) {
                return $(this._Areablock().find('[name="location"]:checked')).data('areadata');
            }
        },
        getUserData: function (userid) {
            var userdata = this.$table.data('userdaydata');
            if (userid != undefined) {
                return userdata[userid];
            }
            return userdata;
        },
        getShiftSumData: function (day) {
            var shiftsumdata = this.$table.data('shiftsumdata');
            if (day != undefined) {
                return shiftsumdata[day];
            }
            return shiftsumdata;
        },
        getDaySumData: function (userid) {
            var daysumdata = this.$table.data('daysumdata');
            if (userid != undefined) {
                return daysumdata[userid];
            }
            return daysumdata;
        },
        getNowShift: function (shiftid) {
            var nowshift = this.$table.data('nowshiftdata');
            var myui = this;
            if (nowshift == undefined) {
                var shift = this.getShiftData();
                if (shiftid == undefined) {
                    shiftid = myui.options.default_shift_type
                }
                nowshift = shift.filter(function (shift) { return shift.shortcut == shiftid });
                if (nowshift != undefined && nowshift.length > 0) { nowshift = nowshift[0]; }
            }
            return nowshift;
        },
        getSelectedtd: function () {
            return this.$table.data('selectedtd');
        },
        getSelectedDate: function () {
            if (this.getSelectedtd() != undefined) {
                return this.getSelectedtd().data();
            }
            return undefined;
        },
        getSaveData: function () {
            return {
                year: this.options.year,
                month: this.options.month,
                schedul_data: this.$schedulingTable.data('schdata').schedul_data
            };
        },
        getShiftData: function () {
            return this.$schedulingTable.data('schdata').shift_data;
        }
    });
})(jQuery);