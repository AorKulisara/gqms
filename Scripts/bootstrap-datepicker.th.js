/**
 * Thai translation for bootstrap-datepicker
 * Suchau Jiraprapot <seroz24@gmail.com>
 */
 
;(function($){
	// en-th - (rare use) english language with thai-year
	$.fn.datepicker.dates['en-th'] = 
	// en-en.th - english language with smart thai-year input (2540-2569) conversion 
	$.fn.datepicker.dates['en-en.th'] = 
							$.fn.datepicker.dates['en'];
	
	// th-th - thai language with thai-year
	$.fn.datepicker.dates['th-th'] =
	$.fn.datepicker.dates['th'] = {
		format: 'dd/mm/yyyy',
		days: ["�ҷԵ��", "�ѹ���", "�ѧ���", "�ظ", "�����", "�ء��", "�����", "�ҷԵ��"],
		daysShort: ["��", "�", "�", "�", "��", "�", "�", "��"],
		daysMin: ["��", "�", "�", "�", "��", "�", "�", "��"],
		months: ["���Ҥ�", "����Ҿѹ��", "�չҤ�", "����¹", "����Ҥ�", "�Զع�¹", "�á�Ҥ�", "�ԧ�Ҥ�", "�ѹ��¹", "���Ҥ�", "��Ȩԡ�¹", "�ѹ�Ҥ�"],
		monthsShort: ["�.�.", "�.�.", "��.�.", "��.�.", "�.�.", "��.�.", "�.�.", "�.�.", "�.�.", "�.�.", "�.�.", "�.�."],
		today: "�ѹ���"
	};
}(jQuery));