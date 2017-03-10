angular.module('app',['SignalR','OData'])
.factory('Mission', ['$rootScope', 'Hub', 'OData', function ($rootScope, Hub, OData) {
	var Missions = this;

	//Mission ViewModel
	var Mission = function (mission) {
		if (!mission) mission = {};
		var Mission = {
		    ID: mission.ID || null,
		    Title: mission.Title || 'New',
		    Text: mission.Text || 'New',
		    LAT:ProviderLogo || 47,
		    LON: mission.LON || 13,
		    ProviderLogo:  mission.ProviderLogo || 'New',
		    ProviderName:  mission.ProviderName || 'New',
		    CostumMissionID:  mission.CostumMissionID || 'New'
		};
		return Mission;
	}

	//Hub setup
	var hub = new Hub('mission', {
		listeners: {
			'newConnection': function (id) {
				Missions.connected.push(id);
				$rootScope.$apply();
			},
			'removeConnection': function (id) {
				Missions.connected.splice(Missions.connected.indexOf(id), 1);
				$rootScope.$apply();
			},
			'lockMission': function (id) {
				var mission = find(id);
				mission.Locked = true;
				$rootScope.$apply();
			},
			'unlockMission': function (id) {
				var mission = find(id);
				mission.Locked = false;
				$rootScope.$apply();
			},
			'updatedMission': function (id, key, value) {
				var mission = find(id);
				mission[key] = value;
				$rootScope.$apply();
			},
			'addMission': function (mission) {
				Missions.all.push(new Mission(mission));
				$rootScope.$apply();
			},
			'removeMission': function (id) {
				var mission = find(id);
				Missions.all.splice(Missions.all.indexOf(mission), 1);
				$rootScope.$apply();
			}
		},
		methods: ['lock', 'unlock'],
		errorHandler: function (error) {
			console.error(error);
		}
	});

	//Web API setup
	var webApi = OData('/odata/Missions', { id: '@Id' });

	//Helpers
	var find = function (id) {
		for (var i = 0; i < Missions.all.length; i++) {
			if (Missions.all[i].Id == id) return Missions.all[i];
		}
		return null;
	};

	//Variables
	Missions.all = [];
	Missions.connected = [];
	Missions.loading = true;

	//Methods
	Missions.add = function () {
		webApi.post({ Name: 'New', Email: 'New', Salary: 1 });
	}
	Missions.edit = function (mission) {
		mission.Edit = true;
		hub.lock(mission.Id);
	}
	Missions.delete = function (mission) {
		webApi.remove({ id: mission.Id });
	}
	Missions.patch = function (mission, key) {
		var payload = {};
		payload[key] = mission[key];
		webApi.patch({ id: mission.Id }, payload);
	}
	Missions.done = function (mission) {
		mission.Edit = false;
		hub.unlock(mission.Id);
	}

	//Load
	Missions.all = webApi.query(function (data) {
		var missions = [];
		angular.forEach(data.value, function (mission) {
			missions.push(new Mission(mission));
		});
		Missions.all = missions;
		Missions.loading = false;
	});
	return Missions;
}])
.controller('signalrGrid', ['$scope','Mission', function($scope, Mission){
	$scope.missions = Mission;
}])