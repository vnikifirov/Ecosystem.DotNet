'use strict';

/* Services */

angular.module('guestbook.Services', [])
	.service('rest', ['$http', '$rootScope', function ($http, $rootScope) {
		
		//
		//	Здесь описаны вспомогательные функции, обеспечивающие http-запросы к серверу;
		//	также эти функции обеспечивают значение $rootScope.loading = true во время, когда хотя бы один запрос выполняется,
		//	и значение $rootScope.loading = false во время, когда все запросы выполнены,
		//	это удобно для отображения на страницах индикатора загрузки или блокировки некоторых кнопок (например, кнопки "сохранить" 
		//	на странице редактирования объекта на то время, пока объект ещё не загружен и поля формы не заполнены старыми значениями).
		//
		//	Параметры p_get и p_post представляют собой объекты вида {key1:value1,key2:value2}, которые будут переданы в качестве GET и POST параметров запроса.
		//
		//	Примеры обёрток для этих функций представлены ниже.
		//
		
		this.get = function(url, p_get, cb) {
			var lkey = $rootScope.AddLoader();
			var p = [];
			for (var key in p_get) p.push(encodeURIComponent(key)+"="+encodeURIComponent(p_get[key]));
	        return $http.get(url+(p.length == 0 ? "" : "?"+p.join("&")))
				.success(function (data, status, headers) {
					if (angular.isFunction(cb)) cb(data);
				    $rootScope.RemoveLoader(lkey);
				})
				.error(function (data, status, headers) {
				    window.alert("error get "+url+" !");
				    $rootScope.RemoveLoader(lkey);
				});
		};
		
		this.post = function(url, p_get, p_post, cb) {
			var lkey = $rootScope.AddLoader();
			var p = [];
			for (var key in p_get) p.push(encodeURIComponent(key)+"="+encodeURIComponent(p_get[key]));
			return $http.post(url+(p.length == 0 ? "" : "?"+p.join("&")), $.param(p_post),{headers:{'Content-Type': 'application/x-www-form-urlencoded'}})
				.success(function (data, status, headers) {
					if (angular.isFunction(cb)) cb(data);
				    $rootScope.RemoveLoader(lkey);
				})
				.error(function (data, status, headers) {
				    window.alert("error post "+url+" !");
				    $rootScope.RemoveLoader(lkey);
				});
		};
		
		this.upload = function(url, p_get, p_formdata, cb) {
			var lkey = $rootScope.AddLoader();
			var p = [];
			for (var key in p_get) p.push(encodeURIComponent(key)+"="+encodeURIComponent(p_get[key]));
			return $http.post(url+(p.length == 0 ? "" : "?"+p.join("&")), p_formdata, {transformRequest:angular.identity, headers:{'Content-Type': undefined}})
				.success(function (data, status, headers) {
					if (angular.isFunction(cb)) cb(data);
				    $rootScope.RemoveLoader(lkey);
				})
				.error(function (data, status, headers) {
				    window.alert("error upload "+url+" !");
				    $rootScope.RemoveLoader(lkey);
				});
		};
		
		this.delete = function(url, p_get, cb) {
			var lkey = $rootScope.AddLoader();
			var p = [];
			for (var key in p_get) p.push(encodeURIComponent(key)+"="+encodeURIComponent(p_get[key]));
	        return $http.delete(url+(p.length == 0 ? "" : "?"+p.join("&")))
				.success(function (data, status, headers) {
					if (angular.isFunction(cb)) cb(data);
				    $rootScope.RemoveLoader(lkey);
				})
				.error(function (data, status, headers) {
				    window.alert("error get "+url+" !");
				    $rootScope.RemoveLoader(lkey);
				});
		};
	
		/* ================================================================================ */
		/* ================================================================================ */
		/* ================================================================================ */
		
		//
		//	Отсюда начинаются методы, последний параметр которых принимает callback-функцию;
		//	она будет вызвана после получения ответа с сервера
		//
	
	    this.loadMessageList = function (cb) {
			this.get("/api/message", {}, cb);
	    };
	    this.loadMessage = function (id, cb) {
			return this.get("/api/message", {id:id}, cb);
	    };
		this.saveMessage = function(message, cd) {
			return this.post("/api/message", {}, message, cd)
		};
		this.deleteMessage = function(id, cd) {
			return this.delete("/api/message", {id:id}, cd)
		};
		this.loadComment = function (id, cb) {
			return this.get("/api/comment", {id:id}, cb);
	    };
		this.saveComment = function (comment, cb) {
			return this.post("/api/comment", {}, comment, cb);
	    };
		this.saveVote = function (vote, cb) {
			return this.post("/api/rating", {}, vote, cb);
	    };
	}]);
