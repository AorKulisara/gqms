/**
 * @license Copyright (c) 2003-2017, CKSource - Frederico Knabben. All rights reserved.
 
* For licensing, see LICENSE.md or http://ckeditor.com/license
 
*/

CKEDITOR.editorConfig = function( config ) {
	// Define changes to default configuration here. For example:
	// config.language = 'fr';
	// config.uiColor = '#AADC6E';
};




CKEDITOR.editorConfig = function (config) {
    config.toolbarGroups = [
		{ name: 'clipboard', groups: ['clipboard', 'undo'] },
		{ name: 'editing', groups: ['find', 'selection', 'spellchecker', 'editing'] },
		{ name: 'forms', groups: ['forms'] },
		{ name: 'basicstyles', groups: ['basicstyles', 'cleanup'] },
		{ name: 'paragraph', groups: ['list', 'indent', 'blocks', 'align', 'bidi', 'paragraph'] },
		{ name: 'links', groups: ['links'] },
		{ name: 'insert', groups: ['insert'] },

		{ name: 'styles', groups: ['styles'] },
		{ name: 'colors', groups: ['colors'] },
		{ name: 'tools', groups: ['tools'] },
		{ name: 'document', groups: ['mode', 'document', 'doctools'] },
		{ name: 'others', groups: ['others'] },
		{ name: 'about', groups: ['about'] }
    ];

    config.removeButtons = 'Save,NewPage,Preview,Print,Templates,Replace,Find,Scayt,Form,Checkbox,Radio,TextField,Select,Textarea,Button,HiddenField,ImageButton,BidiLtr,BidiRtl,Language,Link,Unlink,Anchor,Flash,PageBreak,Iframe,ShowBlocks,About,SelectAll,Cut,Paste,Copy,PasteText,PasteFromWord,CopyFormatting,RemoveFormat,Smiley,SpecialChar,HorizontalRule,Image,Table,Blockquote,CreateDiv,Styles,Format';
};