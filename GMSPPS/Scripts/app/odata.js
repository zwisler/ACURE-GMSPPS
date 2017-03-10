angular.module('OData', ['ngResource'])
.factory('OData', ['$resource', function ($resource) {
	return function (url, paramDefaults, actions) {
		var oDataParamDefaults = {};
		if (paramDefaults) for (var i in paramDefaults) oDataParamDefaults[i] = paramDefaults[i];
		var oDataActions = {
			'query': { method: 'GET', isArray: false },
			'get': { method: 'GET' },
			'post': { method: 'POST' },
			'remove': { method: 'DELETE', url: url + '(:id)' },
			'patch': { method: 'PATCH', url: url + '(:id)' }
		}
		if (actions) for (var i in actions) oDataActions[i] = actions[i];

		var OData = $resource(url, oDataParamDefaults, oDataActions);
		return OData;
	};
}]);