/**
 * @license Copyright (c) 2003-2015, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.md or http://ckeditor.com/license
 */
CKEDITOR.timestamp = '20160321-01';
CKEDITOR.editorConfig = function (config) {
    config.language = 'zh';
    config.uiColor = "#ececec";
    config.disableNativeSpellChecker = true;
    config.magicline_color = '#cccccc';
    config.toolbar_complete = [
        ['NewPage', 'Preview', '-', 'Cut', 'Copy', 'Paste', 'PasteFromWord', '-', 'Undo', 'Redo', '-', 'Find', 'Replace', '-', 'SelectAll', 'RemoveFormat'],
        ['Bold', 'Italic', 'Underline', 'Strike', '-', 'Subscript', 'Superscript'],
        ['NumberedList', 'BulletedList', '-', 'Outdent', 'Indent', 'Blockquote'],
        ['Maximize'],
        ['Source'],
        ['JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock','-','Link', 'Unlink', 'Anchor'],
        ['Image', 'Youtube','Table', 'HorizontalRule', 'Smiley', 'SpecialChar', 'PageBreak'],
        //['Styles', 'Format'], 
        ['Font'], 
        ['FontSize'],
        ['TextColor', 'BGColor']
    ];
    config.toolbar_simple = [
        ['Bold', 'Italic', 'Underline'],
        ['FontSize'], ['TextColor', 'BGColor'], ['Table']
    ];
    config.toolbar = "complete";
    config.toolbarLocation = 'top';
    config.height = '150';
    config.extraPlugins = 'image,imagebrowser,youtube';
    config.removePlugins = 'elementspath';
    config.filebrowserImageUploadUrl = "../Main/UploadPicture";
    config.resize_enabled = false;
    config.imageBrowser_listUrl = "../../../../../Main/GetPictreList?" + Math.floor((Math.random() * 10) + 1).toString();
    config.youtube_width = '480';
    config.youtube_height = '349';
    config.youtube_related = true;
    config.youtube_older = false;
    config.youtube_privacy = false;
    config.htmlEncodeOutput = false;
    config.entities = false;
};

// fixed IE 11 browser dropdowns false
$.fn.modal.Constructor.prototype.enforceFocus = function() {
     modal_this = this;
     $(document).on('focusin.modal', function (e) {
         // Fix for CKEditor + Bootstrap IE issue with dropdowns on the toolbar
         // Adding additional condition '$(e.target.parentNode).hasClass('cke_contents cke_reset')' to
         // avoid setting focus back on the modal window.
         if (modal_this.$element[0] !== e.target && !modal_this.$element.has(e.target).length
             && $(e.target.parentNode).hasClass('cke_contents cke_reset')) {
             modal_this.$element.focus();
         }
     });
 };