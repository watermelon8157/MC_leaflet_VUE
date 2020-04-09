
//浮點數到小數第二位
function roundNumber(rnum, rlength) { // Arguments: number to round, number of decimal places
    var newnumber = Math.round(rnum * Math.pow(10, rlength)) / Math.pow(10, rlength);
    return parseFloat(newnumber); // Output the result to the form field (change for your purposes)
}

function ScrollToTop(speed) {
    var s = (speed) ? speed : 200;
    $('html, body').animate({ scrollTop: 0 }, s, 'swing');
}

function SetErrIcon(jsonStr) {
    $('.error_control').removeClass('error_control');
    $('.err_float_msg').remove();
    $('.err_msg_icon').remove();
    var rc = typeof (jsonStr) == 'string' ? JSON.parse(jsonStr) : jsonStr;
    if (rc.length > 0) {
        for (i = 0; i < rc.length; i++) {
            var item = $('#' + rc[i].ID + '').addClass('error_control');
            var ih = item.outerHeight();
            var iw = item.width() - 2;
            var arrow = $('<span class="err_msg_arrow"></span>');
            var icon = $('<span class="err_msg_icon ui-icon-alert"></span>')
                .css({ 'margin-top': '3px', 'margin-left': '-' + iw + 'px' });
            var dd = $('<span class="err_float_msg display-none"></span>').html(rc[i].errStr)
                .css({ 'margin-top': '-' + ih + 'px', 'margin-left': '-' + iw + 'px' })
                .append(arrow);
            item.on('mouseenter', function () {
                //dd.removeClass('display-none');
                icon.addClass('display-none');
            }).after(icon);
            /*item.on('mouseenter', function () {
                //dd.removeClass('display-none');
                icon.addClass('display-none');
            }).on('mouseout', function () {
                dd.addClass('display-none');
            }).after(dd);*/
        }
    }
};

(function ($) {
    $.widget('ui.SetTopAnchor', {
        version: "@VERSION",
        options: {
            showheight: 200,
            compareself: false, //decide to show top div when scroll down window/this.element to showheight
            topcss: 'to-top',
            speed: 200,
            topself: false,
            scrollbody: true
        },
        _create: function () {
            var t = this;
            var el = $(this.element);
            var topdiv = $('<div class=\"' + this.options.topcss + '\">TOP</div>').hide();
            t._on(topdiv, {
                'click': 'toTop'
            });
            el.prepend(topdiv);
            var target = this.options.compareself ? el : window;
            $(target).scroll(function () {
                t.show(target, topdiv);
            });
        },
        show: function (target, div) {
            var h = $(target).scrollTop();
            if (h >= this.options.showheight) {
                if (div.is(':hidden')) div.show();
            } else {
                if (div.is(':visible')) div.hide();
            };

        },
        toTop: function () {
            var t = this.options.topself;
            if (t) {
                $(this.element[0]).animate({ scrollTop: 0 }, this.options.seepd, 'swing');
            }
            if (this.options.scrollbody)
            {
                $('html, body').animate({ scrollTop: 0 }, this.options.seepd, 'swing');
            }
        }
    });

    $.widget('ui.ammTabs', {
        version: "@VERSION",
        options: {
            items: "li",
            tabElement: "ul",
            windowItems: 'div',
            cssSelected: 'tab-selected tab-selected-gradient',
            cssDefault: 'tab-default radius-5-top',
            cssWinDefault: 'tab-window-default',
            cssWinSelected: 'tab-window-selected',
            clickbutton: '',
            opentab: 0,
            firefunc: $.noop  //callback: use $.noop to be default function, this won't do anything when trigger
        },
        _create: function () {
            var that = this;
            var ot = this.options.opentab;
            var btn = this.options.clickbutton;
            var otab = ot != '' ? ot : 0;
            var tabs_div = this.element.children(this.options.windowItems);
            var tabs_ul = this.element.children(this.options.tabElement);
            var tabs_li = tabs_ul.children(this.options.items);
            if (tabs_div && tabs_div.length > 0) tabs_div.addClass(this.options.cssWinDefault);
            if (tabs_li && tabs_li.length > 0) {
                var winsele = this.options.cssWinSelected;
                var tabsele = this.options.cssSelected;
                $(tabs_li).addClass(this.options.cssDefault)
                    .each(function () {
                        var $this_li = $(this);
                        if ($this_li.attr('disabled') == 'disabled') {
                            $this_li.css('cursor', 'not-allowed');
                        }
                        else {
                            $this_li.css('cursor', '');
                        }
                        $this_li.on('click', function () {
                            if ($this_li.attr('disabled') != 'disabled' && $this_li.data('is_opened') != '1') {
                                $(tabs_li).data('is_opened', '0');
                                $this_li.data('is_opened', '1');
                                tabs_div.hide().removeClass(winsele);
                                var div_id = '#div-' + this.id;
                                $(div_id).show().addClass(winsele);
                                tabs_li.removeClass(tabsele);
                                $(this).addClass(tabsele);
                                if (btn != '') {
                                    $(btn).click();
                                };
                                that._trigger('firefunc'); //_trigger to fire function, if exist
                            }
                        });
                    });
                    
                $(tabs_li[otab]).addClass(this.options.cssSelected);
                $(tabs_div[otab]).show().addClass(this.options.cssWinSelected);
            }
        },
        enable_tab: function (tabIdx) {
            var tabs_ul = this.element.children(this.options.tabElement);
            var tabs_li = tabs_ul.children(this.options.items);
            $(tabs_li[tabIdx]).removeAttr('disabled').css('cursor', '');
        },
        disable_tab: function (tabIdx) {
            var tabs_ul = this.element.children(this.options.tabElement);
            var tabs_li = tabs_ul.children(this.options.items);
            $(tabs_li[tabIdx]).attr('disabled', '').css('cursor', 'not-allowed');
        }
    });


    $.widget('ui.selectfilter', {
        version: "@VERSION",
        options: {
            color: "#05acf0",
            position: 'right',
            margintop: 1,
            emptytext: ''
        },
        _create: function () {
            var t = this,
                $select = $(this.element),
                $options = $select.find('option'),
                initmargin = 'margin-' + this.options.position;

            if ($select.is(':disabled') === false && $options.length > 0)
            {
                var w = $select.outerWidth();
                $select.data('options', $options);

                var margin = parseInt($select.css(initmargin).replace('px', '')) + 12; //取得要撐開的margin寬度
                //floating filter textbox
                var filtertxt = this._filter(w);
                //simulate margin div
                var margindiv = $('<div style="display: inline-block; vertical-align: middle; width: ' + margin + 'px;"/>');
                $select.css({
                    'width': w + 'px',
                    initmargin: '0' //set select margin = 0
                });

                if (this.options.position === 'left') { //add margin div
                    $select.before(margindiv)
                        .after(filtertxt); //add filter textbox
                } else {
                    $select.after(margindiv)
                        .after(filtertxt); //add filter textbox
                }
              
                //set filter event
                t._on(filtertxt, {
                    'keyup': function () { //search
                        var rule = filtertxt.val();
                        $select.empty().scrollTop(0);
                        if (rule != '') {
                            var $filter = $options.filter(':contains("' + rule + '")');
                            if (t.options.emptytext != ''){
                                $select.append($('<option></option>').attr('value', '').text(t.options.emptytext))
                            }
                            $select.append($filter).selecFirst();
                        }
                        else {
                            $select.append($options).selecFirst();
                        }
                    }
                });
            }
        },
        _setcolor: function () {
            if (this.options.color.indexOf('#') < 0) {
                return '#' + this.options.color;
            } else {
                return this.options.color;
            }
        },
        _filter: function (w) {
            var filtercolor = this._setcolor(),
                border = 'border: 1px solid ' + filtercolor + '; border-' + this.options.position + ': 10px solid ' + filtercolor + '; ',
                radius = 'border-bottom-' + this.options.position + '-radius: 10px; border-top-' + this.options.position + '-radius: 10px;',
                fmove = { 'width': '40px' },
                bmove = { 'width': '2px' },
                style = 'margin-top: ' + this.options.margintop + 'px; width: 2px; ' + border + radius;

            if (this.options.position === 'left') {
                $.extend(fmove, { 'margin-left': '-' + (w + 51) + 'px' });
                $.extend(bmove, { 'margin-left': '-' + (w + 11) + 'px' });
                style += 'margin-left: -' + (w + 11) + 'px;';
            }
            var filtertxt = $('<input type="text" title="filter text" style="outline: none; position: absolute;' + style + '"/>');
          
            filtertxt.on({
                'focus': function () {
                    $(this).animate(fmove, 500);
                },
                'blur': function () {
                    $(this).animate(bmove, 500);
                }
            });

            return filtertxt;
        }
    });


    $.widget('ui.checkGroupMode', {
        version: "@VERSION",
        options: {
            mode: 'single', //checkSiblings //uncheckSiblings
            exceptCss: '.no_check'
        },
        _create: function () {
            var that = this,
                $group = $(this.element),
                except = that._exceptCss(),
                chks = $group.find('input:checkbox').not(except), //checkbox
                sel = $group.find('select').not(except), // select
                txt = $group.find('input:text').not(except), //textbox
                fst = chks.first(), // 1st checkbox
                fst_siblings = fst.siblings(),
                oth = chks.not(fst).not(except); // other checkbox

            var checkother = this.options.mode == 'checkSiblings';
            var ismulti = (this.options.mode === 'checkSiblings' || this.options.mode === 'uncheckSiblings');


            fst.on('click', function () { // set first checkbox event
                var clearoth = function (c) {
                    if (c) {
                        that._clearText(txt.not(fst_siblings));
                        that._selectFirst(sel.not(fst_siblings));
                    }
                };
                if (this.checked) {
                    if (ismulti) {
                        that._setCheckbox(oth, checkother);
                        clearoth(!checkother);
                    }
                    else {
                        that._setCheckbox(oth, false);
                        clearoth(true);
                    }
                }
                else {
                    that._clearText(fst_siblings);
                    that._selectFirst(fst_siblings);
                }
            });

            oth.on('click', function () { // set other checkbox event
                var $t = $(this).siblings();
                var clearfst = function () {
                    that._setCheckbox(fst, false);
                    that._clearText(fst_siblings);
                    that._selectFirst(fst_siblings);
                };
                if (this.checked) {
                    if (!ismulti) {
                        that._setCheckbox(chks.not(this), false);
                        that._clearText(txt.not($t));
                        that._selectFirst(sel.not($t));
                    } else if (!checkother) {
                        clearfst();
                    }
                } else {
                    if (ismulti && checkother) {
                        clearfst();
                    }
                    that._clearText($t);
                    that._selectFirst($t);
                }
            });

            var input_check = function (item) { //select & textbox function
                var the = $(item);
                var the_chk = the.siblings(); // get checkbox with select or textbox
                var hastext = the.val() != '';
                that._setCheckbox(the_chk, hastext);
                if (hastext && !ismulti) { // uncheck other checkbox
                    that._setCheckbox(chks.not(the_chk), false);
                }
            };
            sel.on('change', function () { // set select event
                input_check(this);
            });
            txt.on('keyup', function () { // set textbox event
                input_check(this);
            });

        },
        _exceptCss: function () {
            if (this.options.exceptCss.substring(0 ,1) != '.') {
                return '.' + this.options.exceptCss;
            } else {
                return this.options.exceptCss;
            }
        },
        _selectFirst: function (t, disabled) {
            return t.each(function () {
                var selidx = $(this).data('default-val')
                if (disabled != undefined) {
                    if (disabled) {
                        this.setAttribute('disabled', '');
                        this.selectedIndex = !selidx == undefined ? 0 : selidx;
                    }
                    else {
                        this.removeAttribute('disabled');
                    }
                }
                else {
                    this.selectedIndex = selidx == undefined ? 0 : selidx;
                }
            });
        },
        _setCheckbox: function (t, isCheck) {
            return t.each(function () {
                this.checked = isCheck;
            });
        },
        _clearText: function (t) {
            return t.each(function () {
                $(this).val('');
            });
        }
    });
}(jQuery));

$.fn.clearText = function () {
    return this.each(function () {
        $(this).val('');
    });
}

$.fn.clearSelect = function () {
    return this.each(function () {
        if (this.tagName == 'SELECT')
            this.options.length = 0;
    });
}

$.fn.fillSelect = function (data) {
    return this.clearSelect().each(function () {
        if (this.tagName == 'SELECT') {
            var dropdownList = this;
            $.each(data, function (index, optionData) {
                var option = new Option(optionData.Text, optionData.Value);
                dropdownList.add(option, null);
            });
            if (data.length == 2) {
                dropdownList.selectedIndex = 1;
            }
        }
    });
}
$.fn.chkAllBox = function (chked) {
    return this.each(function () {
        this.checked = chked;
    });
}
$.fn.selecFirst = function (disabled) {
    return this.each(function () {
        var selidx = $(this).data('idx')
        if (disabled != undefined) {
            if (disabled)
            {
                this.setAttribute('disabled', '');
                this.selectedIndex = !selidx == undefined ? 0 : selidx;
            }
            else
            {
                this.removeAttribute('disabled');
            }
        }
        else {
            this.selectedIndex = selidx == undefined ? 0 : selidx;
        }
    });
}

function BlockScreen() {
    var ly1 = $('<div id="divloading1" class="loading_bg"></div>');
    var ly2 = $('<div id="divloading2" class="loading_pic"></div>');
    $(ly1).appendTo('body');
    $(ly2).appendTo('body');

    $('html').css({ 'height': '100%', 'overflow': 'hidden', 'margin': '0px', 'padding': '0px' })
    $('body').css({ 'height': '100%', 'overflow': 'auto', 'margin-right': '0px', 'margin-bottom': '0px', 'margin-top': '0px', 'padding': '0px' });
    var w = $("#divloading2").width() / 2;
    var fw = jQuery.boxModel && document.documentElement.clientWidth || document.body.clientWidth + 'px';
    var fh = Math.max(document.body.scrollHeight, document.body.offsetHeight) - (jQuery.boxModel ? 0 : 4) + 34 + 'px';

    var ftop = (document.documentElement.clientHeight || document.body.clientHeight) / 2 + 'px';
    var fleft = (document.documentElement.clientWidth || document.body.clientWidth) / 2 - w + 'px';

    $('#divloading1').css({ 'opacity': '0.6', 'width': fw, 'height': fh, 'z-index': '900' }).show();
    $('#divloading2').css({ 'top': ftop, 'left': fleft, 'z-index': '950' }).show();
}

function unBlockScreen() {
    $('#divloading1').remove();
    $('#divloading2').remove();
    clearBlockStyle();
}

//Created by Ammenze 2013/12/25
function BlockScreenDiv(div) {
    var ly1 = $('<div id="divloading1" class="loading_bg"></div>');
    var d = '#' + div;
    var ly2 = $(d);
    $(ly1).appendTo('body');
    $(ly2).appendTo('body');

    $('html').css({ 'height': '100%', 'overflow': 'hidden', 'margin': '0px', 'padding': '0px' })
    $('body').css({ 'height': '100%', 'overflow': 'auto', 'margin-right': '0px', 'margin-bottom': '0px', 'margin-top': '0px', 'padding': '0px' });
    var w = $(d).width() / 2;
    var h = $(d).height() / 2;
    var fw = jQuery.boxModel && document.documentElement.clientWidth || document.body.clientWidth + 'px';
    var fh = Math.max(document.body.scrollHeight, document.body.offsetHeight) - (jQuery.boxModel ? 0 : 4) + 34 + 'px';

    var ftop = (document.documentElement.clientHeight || document.body.clientHeight) / 2 - h + 'px';
    var fleft = (document.documentElement.clientWidth || document.body.clientWidth) / 2 - w + 'px';

    $('#divloading1').css({ 'opacity': '0.6', 'width': fw, 'height': fh, 'z-index': '900' }).show();
    $(d).css({ 'top': ftop, 'left': fleft, 'z-index': '950' }).show();
}
function clearBlockStyle() {
    $('html').css({ 'height': '', 'overflow': '', 'margin': '', 'padding': '' })
    $('body').css({ 'height': '', 'overflow': '', 'margin-right': '', 'margin-bottom': '', 'margin-top': '', 'padding': '' });
}
//Created by Ammenze 2013/12/25
function unBlockScreenDiv(div) {
    $('#divloading1').remove();
    $('#' + div).hide();
    clearBlockStyle();
}
