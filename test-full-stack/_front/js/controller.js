'use strict';

/* Controllers */

var guestbookCtrl = angular.module('guestbook.Controllers', []);

guestbookCtrl.controller('MessageListController', ['$scope', 'rest', '$routeParams', '$location',
	function ($scope, rest, $routeParams, $location) {
	    $scope.title = "Message list";
		
		// это пример использования сервера rest для выполнения запроса к API;
		// параметром служит callback-функция, которая будет вызвана после получения ответа от сервера
	    rest.loadMessageList(function(data) { $scope.list = data.answer; });
	}
]);

guestbookCtrl.controller('MessageViewController', ['$scope', 'rest', '$routeParams', '$location', '$window',
	function ($scope, rest, $routeParams, $location, $window) {
	    $scope.title = "View message";
		var messageID = $routeParams.messageID;
	    rest.loadMessage(messageID, function(data) { $scope.message = data.answer; });
		
		$scope.newComment = { MessageID : messageID, };
		
		$scope.addComment = function() {
			rest.saveComment($scope.newComment, function(data) {
				rest.loadMessage(messageID, function(data) { $scope.message = data.answer; });
				$scope.newComment = { MessageID : messageID, };
				$window.scrollTo(0, 0);
			});
		};
	}
]);

guestbookCtrl.controller('MessageNewController', ['$scope', 'rest', '$routeParams', '$location', '$window',
	function ($scope, rest, $routeParams, $location, $window) {
	    $scope.title = "Add message";
		$scope.addMessage = function() {
			rest.saveMessage($scope.newMessage, function(data) { $window.scrollTo(0, 0); });
		};
	}
]);

guestbookCtrl.controller('MessageVoteController', ['$scope', 'rest', '$routeParams', '$location', '$window',
	function ($scope, rest, $routeParams, $location, $window) {
	    $scope.title = "Vote for the message";

		var messageID =  $routeParams.messageID;
		rest.loadMessage(messageID, function(data) { $scope.message = data.answer; });

		$scope.setVote = function(value) {
			$scope.newVote = { MessageID : messageID, Value : value };
		};
		$scope.addVote = function() {			
			rest.saveVote($scope.newVote, function(data) { $window.scrollTo(0, 0); });
		};
	}
]);

guestbookCtrl.controller('MessageEditController', ['$scope', 'rest', '$routeParams', '$location', '$window',
	function ($scope, rest, $routeParams, $location, $window) {
	    $scope.title = "Edit message";
		
		var messageID = $routeParams.messageID;
	    rest.loadMessage(messageID, function(data) { $scope.newMessage = data.answer; });
		
		$scope.addMessage = function() {
			rest.saveMessage($scope.newMessage, function(data) {
				rest.loadMessage(messageID, function(data) { $scope.newMessage = data.answer; });				
				$window.scrollTo(0, 0);
			});
		};
	}
]);

guestbookCtrl.controller('MessageDeleteController', ['$scope', 'rest', '$routeParams', '$location', '$window',
	function ($scope, rest, $routeParams, $location, $window) {
		$scope.title = "Delete message";
		
		var messageID = $routeParams.messageID;
		rest.loadMessage(messageID, function(data) { $scope.message = data.answer; });
		
		$scope.deleteMessage = function() {
			rest.deleteMessage(messageID, function(data) { $window.scrollTo(0, 0); });
		};
	}
]);

guestbookCtrl.controller('CommentViewController', ['$scope', 'rest', '$routeParams', '$location', '$window',
	function ($scope, rest, $routeParams, $location, $window) {
	    $scope.title = "View comment";
		var commentID = $routeParams.commentID;
	    rest.loadComment(commentID, function(data) { $scope.comment = data.answer; });
	}
]);

guestbookCtrl.controller('CommentEditController', ['$scope', 'rest', '$routeParams', '$location', '$window',
	function ($scope, rest, $routeParams, $location, $window) {
	    $scope.title = "Edit comment";
		
		var commentID = $routeParams.commentID;
	    rest.loadComment(commentID, function(data) { $scope.newComment = data.answer; });
		
		$scope.addComment = function() {
			rest.saveComment($scope.newComment, function(data) {
				rest.loadComment(commentID, function(data) { $scope.newComment = data.answer; });				
				$window.scrollTo(0, 0);
			});
		};
	}
]);