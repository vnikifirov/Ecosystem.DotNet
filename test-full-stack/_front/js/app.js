'use strict';

/* App Module */

var arora = angular.module('guestbook', [
  'ngRoute',
  'guestbook.Controllers',
  'guestbook.Services',
  'guestbook.Filters',
  'ui.bootstrap',
  'ui.bootstrap.datetimepicker',
]);

arora.config(['$routeProvider',
  function ($routeProvider) {
      var path = window.cje.partial;
      $routeProvider.when('/home', { templateUrl: path + 'messageList.html', controller: 'MessageListController' });

      $routeProvider.when('/message/view/:messageID', { templateUrl: path + 'messageView.html', controller: 'MessageViewController' });
      
      $routeProvider.when('/message/add', { templateUrl: path + 'messageNew.html', controller: 'MessageNewController' });

      $routeProvider.when('/message/edit/:messageID', { templateUrl: path + 'messageNew.html', controller: 'MessageEditController' });

      $routeProvider.when('/message/delete/:messageID', { templateUrl: path + 'messageDelete.html', controller: 'MessageDeleteController' });
	  
      $routeProvider.when('/comment/view/:commentID', { templateUrl: path + 'commentView.html', controller: 'CommentViewController' });

      $routeProvider.when('/comment/edit/:commentID', { templateUrl: path + 'commentEdit.html', controller: 'CommentEditController' });

      $routeProvider.when('/message/vote/:messageID', { templateUrl: path + 'messageVote.html', controller: 'MessageVoteController' });

      $routeProvider.otherwise({ redirectTo: '/home' });
	  
  }])
.run(['$rootScope', function($rootScope){
	$rootScope.userID = window.cje.userID;

	moment.locale("ru");
	
	$rootScope.AddLoader = function() {
		if (!$rootScope.loader_obj) $rootScope.loader_obj = {};
		var key = 'l' + (Math.random() * 10000);
		$rootScope.loader_obj[key] = true;
		$rootScope.loading = true;
		return key;
	};
	$rootScope.RemoveLoader = function(key) {
		if (!$rootScope.loader_obj) $rootScope.loader_obj = {};
		delete $rootScope.loader_obj[key];
		$rootScope.loading = Object.keys($rootScope.loader_obj).length > 0;
	};
}]);

if (!Object.keys) {
    Object.keys = function (obj) {
        var keys = [],
            k;
        for (k in obj) {
            if (Object.prototype.hasOwnProperty.call(obj, k)) {
                keys.push(k);
            }
        }
        return keys;
    };
}