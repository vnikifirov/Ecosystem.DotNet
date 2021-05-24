'use strict';

/* Directives */

angular.module('guestbook.Filters', [])

.filter('moment', function() {
	return function(input,format) {
		var val = input*1000;
		return moment(isNaN(val)?input:val).format(format);  
	}
})
;