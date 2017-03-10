var myApp = angular.module('app', ['SignalR']);

myApp.directive('resize', ['$window', function ($window) {
    return function (scope, element) {
        var w = angular.element($window);
        scope.getWindowDimensions = function () {
            return { 'h': w.height(), 'w': w.width() };
        };
        scope.$watch(scope.getWindowDimensions, function (newValue, oldValue) {
            scope.windowHeight = newValue.h;
            scope.windowWidth = newValue.w;

            scope.style = function () {
                return {
                    'height': (newValue.h - 100) + 'px',
                    //'width': (newValue.w - 100) + 'px' 
                };
            };

        }, true);

        w.bind('resize', function () {
            scope.$apply();
        });
    }
}]);




myApp.factory('Mission',['$rootScope','Hub', '$timeout', function($rootScope, Hub, $timeout){

    //declaring the hub connection
    var hub = new Hub('missionhub', {

        //client side methods
        listeners:{
            'addPin': function (LAT, LON, Name, Status){
                //var employee = find(id);
                //employee.Locked = true;
                $rootScope.$apply();
            },
            'unlockEmployee': function (id) {
                var employee = find(id);
                employee.Locked = false;
                $rootScope.$apply();
            }
        },

        //server side methods
        methods: ['subscribe','unsubscribe'],

        //query params sent on initial connection
        queryParams:{
            'token': 'exampletoken'
        },

        //handle connection error
        errorHandler: function(error){
            console.error(error);
        },

        //specify a non default root
        //rootPath: '/api

        stateChanged: function(state){
            switch (state.newState) {
                case $.signalR.connectionState.connecting:
                    //your code here
                    break;
                case $.signalR.connectionState.connected:
                    //your code here
                    break;
                case $.signalR.connectionState.reconnecting:
                    //your code here
                    break;
                case $.signalR.connectionState.disconnected:
                    //your code here
                    break;
            }
        }
    });

    var subscribe = function (mission) {
        hub.subscribe(mission.ID); //Calling a server method
    };
    var unsubscribe = function (mission) {
        hub.unsubscribe(mission.ID); //Calling a server method
    }

    return {
        subscribe: subscribe,
        subscribe: unsubscribe
    };
}]);


myApp.controller('MissionController', [
  '$scope',
  '$rootScope',
  '$http',
  'Hub',
  function ($scope, $rootScope, $http, Hub) {
      //nicht doppet laden!!!
     
          //map erzeugen
          var map = new Microsoft.Maps.Map(document.getElementById('BMap'), {
              credentials: 'AnRLhFi7j3gEJfOcoNDwIYaZMTZmM4ex30Nlg9zZ_uhPfcBgirbw0S8qDFlSVqjX',
              mapTypeId: 'a'
          });
      //Helpers
          var find = function (name) {
              for (var i = 0; i < $scope.subscribers.length; i++) {
                  if ($scope.subscribers[i].name == name) return $scope.subscribers[i];
              }
              return null;
          };
          var mymission = {};
          $scope.subscribers = [];
          var Suscriber = function (Name, Status) {             
              var suscriber = {
                  name: Name,
                  state: Status,
                  displayMode: function () {
                      switch (this.state) {
                          case 1:
                              //your code here
                              return 'receive-template';
                              break;
                          case 2:
                              //your code here
                              return 'accepted-template';
                              break;
                          case 3:
                              //your code here
                              return 'rejected-template';
                              break;
                          case 4:
                              //your code here
                              return 'started-template';
                              break;
                          case 5:
                              //your code here
                              return 'ready-template';
                              break;

                      }
                  }
              };
              return suscriber
          }
          //$scope.mission = Mission;
         // var mymission = new Mission();
          //Hub setup
          var hub = new Hub("missionhub", {
              listeners: {
                  'ClientAkk': function (Name, Status, ID, CostMid) {
                      var employee = find(Name);
                      if (employee != null)
                      {
                          employee.state = Status
                      }
                      else {
                          var mysuscriber = new Suscriber(Name, Status);
                          $scope.subscribers.push(mysuscriber);
                      }
                     
                      
                      //var find = false;
                      //var curendlocation = new Microsoft.Maps.Location(LAT, LON);
                      //var pushpinOptions = {
                      //    typeName: 'Tracker'
                      //};
                      //for (var i = map.entities.getLength() - 1; i >= 0; i--) {
                      //    var pushpin = map.entities.get(i);
                      //    if (pushpin instanceof Microsoft.Maps.Pushpin && pushpin.Title == Name) {
                      //        map.entities.removeAt(i);
                      //        // .setLocation(loc);
                      //    };

                      //}
                      //var pushpin = new Microsoft.Maps.Pushpin(curendlocation, pushpinOptions);

                      //pushpin.Title = Name;

                      //var pushpinOver = Microsoft.Maps.Events.addHandler(pushpin, 'mouseover', pinMouseOver);
                      //var ppClick = Microsoft.Maps.Events.addHandler(pushpin, 'click', pushpinClick);
                      //var pushpinmouseout = Microsoft.Maps.Events.addHandler(pushpin, 'mouseout', pinmouseout);
                      //map.entities.push(pushpin);
                      // Chats.add(userName, chatMessage);
                        $rootScope.$apply();
                  },
                  'addPin': function (LAT, LON, Name, Status) {
                      //var employee = find(id);
                      //employee.Locked = true;
                      $rootScope.$apply();
                  }

              },
              methods: ['subscribe'],
              errorHandler: function (error) {
                  console.error(error);
              },
              hubDisconnected: function () {
                  if (hub.connection.lastError) {
                      hub.connection.start();
                  }
              },
              transport: 'webSockets',
              stateChanged: function(state){
                  switch (state.newState) {
                      case $.signalR.connectionState.connecting:
                          //your code here
                          break;
                      case $.signalR.connectionState.connected:
                          //Suscribe @HUB
                          hub.subscribe(mymission.ID);
                          // add a Puspin to Map
                          var curendlocation = new Microsoft.Maps.Location(mymission.LAT, mymission.LON);
                          var pushpinOptions = { typeName: 'Tracker' };
                          var pushpin = new Microsoft.Maps.Pushpin(curendlocation, pushpinOptions);
                          pushpin.Title = mymission.Title;
                          map.entities.push(pushpin);
                          ///Center Map to Location
                          map.setView({ zoom: 13, center: curendlocation })

                          break;
                      case $.signalR.connectionState.reconnecting:
                          //your code here
                          break;
                      case $.signalR.connectionState.disconnected:
                          //your code here
                          break;
                  }
              },
              logging: true
          });
          


          // Am Siganl Hub Anmelden (init des View)
     
          $scope.initme = function (mission) {
             

             
              mymission = mission;
             // hub.subscribe(mission.ID); //Calling a server method
             
                var curendlocation = new Microsoft.Maps.Location(mission.LAT, mission.LON);
                 var pushpinOptions = {
                     typeName: 'Tracker'
                 };
                 for (var i = map.entities.getLength() - 1; i >= 0; i--) {
                     var pushpin = map.entities.get(i);
                     if (pushpin instanceof Microsoft.Maps.Pushpin && pushpin.Title == mission.Title) {
                         map.entities.removeAt(i);
                         // .setLocation(loc);
                     };
   
                 }
                 var pushpin = new Microsoft.Maps.Pushpin(curendlocation, pushpinOptions);
   
                 pushpin.Title = mission.Title;
   
                 var pushpinOver = Microsoft.Maps.Events.addHandler(pushpin, 'mouseover', pinMouseOver);
                 
                 var pushpinmouseout = Microsoft.Maps.Events.addHandler(pushpin, 'mouseout', pinmouseout);
                 map.entities.push(pushpin);
                 
                 // Chats.add(userName, chatMessage);
              //  $rootScope.$apply();
                 map.setView({ zoom: 13, center: curendlocation })
            } ;
   
   
   
             // Aktuelle Position verwenden
             //var geoLocationProvider = new Microsoft.Maps.GeoLocationProvider(map);
             //geoLocationProvider.getCurrentPosition({
             //    showAccuracyCircle: false
             //}, {
             //    successCallback: function (object) {
             //        curendlocation = object.center.Location;
             //        var curendlocation = new Microsoft.Maps.Location(object.center);
             //    }
             //});
             //event viewchangestart
             mapviewchangeStart = Microsoft.Maps.Events.addHandler(map, 'viewchangestart', function (e) {
                 onViewChangeStart(e);
             });
             function onViewChangeStart(e) {
                 var latlon = map.getCenter();
                 //var output = document.getElementById("output");
                 var zoom = map.getZoom()
                 if (zoom > 10) {
                     $scope.zoom = true;
                 } else {
                     $scope.zoom = false;
                 }
             }
             // event MouseOver des Pin Tooltip + Infobox einblenden
             function pinMouseOver(e) {
                 var DataName = e.target.getTypeName();
                 var ID = e.target.ID;
                 var Title = e.target.Title;
                 var count = map.entities.getLength();
                 for (var i = 0; i < count; i++) {
                     var test = map.entities.get(i);
                     if (test.ID == ID && (test._typeName != 'Pers' || test._typeName != 'Off')) { test.setOptions({ visible: true }); }
                 }
                 $('.' + DataName).children().attr('title', Title);
             }
             // event Mouseout des Pin  Infobox ausbelnden
             function pinmouseout(e) {
                 var DataName = e.target.getTypeName();
                 var ID = e.target.ID;
                 var Title = e.target.Title;
                 var count = map.entities.getLength();
                 for (var i = 0; i < count; i++) {
                     var test = map.entities.get(i);
                     if (test.ID == ID && test._typeName == 'Infobox') {
                         test.setOptions({ visible: false });
                     }
                 }
             }
         
         
 


  }]); // ende Controller

myApp.controller('ProviderController', [
  '$scope',
  '$rootScope',
  '$http',
  'Hub',
  function ($scope, $rootScope, $http, Hub) {
      //nicht doppet laden!!!
     
         
      //Helpers
          var find = function (name) {
              for (var i = 0; i < $scope.subscribers.length; i++) {
                  if ($scope.subscribers[i].name == name) return $scope.subscribers[i];
              }
              return null;
          };
          var mymission = {};
          $scope.subscribers = [];
          var Suscriber = function (Name, Status) {             
              var suscriber = {
                  name: Name,
                  state: Status,
                  displayMode: function () {
                      switch (this.state) {
                          case 1:
                              //your code here
                              return 'receive-template';
                              break;
                          case 2:
                              //your code here
                              return 'accepted-template';
                              break;
                          case 3:
                              //your code here
                              return 'rejected-template';
                              break;
                          case 4:
                              //your code here
                              return 'started-template';
                              break;
                          case 5:
                              //your code here
                              return 'ready-template';
                              break;

                      }
                  }
              };
              return suscriber
          }
          //$scope.mission = Mission;
         // var mymission = new Mission();
          //Hub setup
          var hub = new Hub("providerhub", {
              listeners: {
                  'StartMission': function (ID, URL) {
                      window.location.replace(URL);
                      var employee = find(Name);
                      if (employee != null)
                      {
                          employee.state = Status
                      }
                      else {
                          var mysuscriber = new Suscriber(Name, Status);
                          $scope.subscribers.push(mysuscriber);
                      }
                     
                      
                      //var find = false;
                      //var curendlocation = new Microsoft.Maps.Location(LAT, LON);
                      //var pushpinOptions = {
                      //    typeName: 'Tracker'
                      //};
                      //for (var i = map.entities.getLength() - 1; i >= 0; i--) {
                      //    var pushpin = map.entities.get(i);
                      //    if (pushpin instanceof Microsoft.Maps.Pushpin && pushpin.Title == Name) {
                      //        map.entities.removeAt(i);
                      //        // .setLocation(loc);
                      //    };

                      //}
                      //var pushpin = new Microsoft.Maps.Pushpin(curendlocation, pushpinOptions);

                      //pushpin.Title = Name;

                      //var pushpinOver = Microsoft.Maps.Events.addHandler(pushpin, 'mouseover', pinMouseOver);
                      //var ppClick = Microsoft.Maps.Events.addHandler(pushpin, 'click', pushpinClick);
                      //var pushpinmouseout = Microsoft.Maps.Events.addHandler(pushpin, 'mouseout', pinmouseout);
                      //map.entities.push(pushpin);
                      // Chats.add(userName, chatMessage);
                        $rootScope.$apply();
                  },
                  'addPin': function (LAT, LON, Name, Status) {
                      //var employee = find(id);
                      //employee.Locked = true;
                      $rootScope.$apply();
                  }

              },
              methods: ['subscribe'],
              errorHandler: function (error) {
                  console.error(error);
              },
              hubDisconnected: function () {
                  if (hub.connection.lastError) {
                      hub.connection.start();
                  }
              },
              transport: 'webSockets',
              stateChanged: function(state){
                  switch (state.newState) {
                      case $.signalR.connectionState.connecting:
                          //your code here
                          break;
                      case $.signalR.connectionState.connected:
                          //Suscribe @HUB
                          hub.subscribe(myprovider.Name);
                          // add a Puspin to Map
                          
                          break;
                      case $.signalR.connectionState.reconnecting:
                          //your code here
                          break;
                      case $.signalR.connectionState.disconnected:
                          //your code here
                          break;
                  }
              },
              logging: true
          });
          


          // Am Siganl Hub Anmelden (init des View)
     
          $scope.initme = function (provider) {
             

             
              myprovider = provider;
             // hub.subscribe(mission.ID); //Calling a server method
             
               
   
                 
                
                 
                 // Chats.add(userName, chatMessage);
              //  $rootScope.$apply();
                 
            } ;
   
   
   
             // Aktuelle Position verwenden
             //var geoLocationProvider = new Microsoft.Maps.GeoLocationProvider(map);
             //geoLocationProvider.getCurrentPosition({
             //    showAccuracyCircle: false
             //}, {
             //    successCallback: function (object) {
             //        curendlocation = object.center.Location;
             //        var curendlocation = new Microsoft.Maps.Location(object.center);
             //    }
             //});
             //event viewchangestart
            
         
         
 


  }]); // ende Controller


myApp.controller('BingController', [
  '$scope',
  '$http',
  'Hub',
  function ($scope, $http, Hub) {
      //nicht doppet laden!!!
      if ($scope.init != true) {
          $scope.init = true;
          //map erzeugen
          var map = new Microsoft.Maps.Map(document.getElementById('BMap'), {
              credentials: 'AnRLhFi7j3gEJfOcoNDwIYaZMTZmM4ex30Nlg9zZ_uhPfcBgirbw0S8qDFlSVqjX',
              mapTypeId: 'a'
          });

          //Hub setup
          var hub = new Hub("chatHub", {
              listeners: {
                  'addPin': function (LAT, LON, Name, Status) {
                      //var find = false;
                      var curendlocation = new Microsoft.Maps.Location(LAT, LON);
                      var pushpinOptions = {
                          typeName: 'Tracker'
                      };
                      for (var i = map.entities.getLength() - 1; i >= 0; i--) {
                          var pushpin = map.entities.get(i);
                          if (pushpin instanceof Microsoft.Maps.Pushpin && pushpin.Title == Name) {
                              map.entities.removeAt(i);
                             // .setLocation(loc);
                          };

                      }
                      var pushpin = new Microsoft.Maps.Pushpin(curendlocation, pushpinOptions);
                      
                      pushpin.Title = Name ;
                     
                      var pushpinOver = Microsoft.Maps.Events.addHandler(pushpin, 'mouseover', pinMouseOver);
                      var ppClick = Microsoft.Maps.Events.addHandler(pushpin, 'click', pushpinClick);
                      var pushpinmouseout = Microsoft.Maps.Events.addHandler(pushpin, 'mouseout', pinmouseout);
                      map.entities.push(pushpin);
                     // Chats.add(userName, chatMessage);
                    //  $rootScope.$apply();
                  }
              },
              methods: ['send'],
              errorHandler: function (error) {
                  console.error(error);
              },
              hubDisconnected: function () {
                  if (hub.connection.lastError) {
                      hub.connection.start();
                  }
              },
              transport: 'webSockets',
              logging: true
          });






          // Aktuelle Position verwenden
          var geoLocationProvider = new Microsoft.Maps.GeoLocationProvider(map);
          geoLocationProvider.getCurrentPosition({
              showAccuracyCircle: false
          }, {
              successCallback: function (object) {
                  curendlocation = object.center.Location;
                  var curendlocation = new Microsoft.Maps.Location(object.center);
              }
          });
          //event viewchangestart
          mapviewchangeStart = Microsoft.Maps.Events.addHandler(map, 'viewchangestart', function (e) {
              onViewChangeStart(e);
          });
          function onViewChangeStart(e) {
              var latlon = map.getCenter();
              //var output = document.getElementById("output");
              var zoom = map.getZoom()
              if (zoom > 10) {
                  $scope.zoom = true;
              } else {
                  $scope.zoom = false;
              }
          }
          // event MouseOver des Pin Tooltip + Infobox einblenden
          function pinMouseOver(e) {
              var DataName = e.target.getTypeName();
              var ID = e.target.ID;
              var Title = e.target.Title;
              var count = map.entities.getLength();
              for (var i = 0; i < count; i++) {
                  var test = map.entities.get(i);
                  if (test.ID == ID && (test._typeName != 'Pers' || test._typeName != 'Off')) { test.setOptions({ visible: true }); }
              }
              $('.' + DataName).children().attr('title', Title);
          }
          // event Mouseout des Pin  Infobox ausbelnden
          function pinmouseout(e) {
              var DataName = e.target.getTypeName();
              var ID = e.target.ID;
              var Title = e.target.Title;
              var count = map.entities.getLength();
              for (var i = 0; i < count; i++) {
                  var test = map.entities.get(i);
                  if (test.ID == ID && test._typeName == 'Infobox') {
                      test.setOptions({ visible: false });
                  }
              }
          }
          // event es wurde auf eien Pin geklickt
          function pushpinClick(e) {
              var DataName = e.target.getTypeName();
              var ID = e.target.ID;
              if (DataName == 'Pers')
                  var Url = "http://www.doit4you.net/Person.aspx?id=" + ID;
              if (DataName == 'Off')
                  var Url = "http://www.doit4you.net/Offert.aspx?id=" + ID;
              window.location.assign(Url);
          }
          // lade die Wort Liste für Auto Comlide

          $http.get('http://www.doit4you.net/Services/C1_net-Web-C1_Service.svc/json/GetC1_KEY_WORT').success(function (data, status) {
              var jsondata = data.GetC1_KEY_WORTResult.RootResults;
              var woerter = [
              ];
              for (var i = 0; i < jsondata.length; i++) {
                  woerter.push(jsondata[i].WORT);
              }
              $scope.states = woerter;
          }); // /> swervice Aufruf
          // get top 10 Pers 
          //$http.get('http://www.doit4you.net/Services/C1_net-Web-C1_Service.svc/json/Get_TOP10_PERSJ').success(function (data, status) {
          //    // this callback will be called asynchronously
          //    // when the response is available
          //    var jsondata = data.Get_Top10_PERSJResult.RootResults;
          //    var jsondata2 = data.Get_TOP10_PERSJResult;
          //    map.entities.clear();
          //    for (var i = 0; i < jsondata.length; i++) {
          //        var latitude = jsondata[i].LAT;
          //        var longitude = jsondata[i].LON;
          //        var ID = jsondata[i].ID;
          //        var Name = jsondata[i].Vorname.concat(' ', jsondata[i].Nachname);
          //        var curendlocation = new Microsoft.Maps.Location(latitude, longitude);
          //        var pushpin = new Microsoft.Maps.Pushpin(curendlocation, { typeName: 'Pers' });
          //        pushpin.Title = Name;
          //        pushpin.ID = ID;
          //        var pushpinOver = Microsoft.Maps.Events.addHandler(pushpin, 'mouseover', pinMouseOver);
          //        var ppClick = Microsoft.Maps.Events.addHandler(pushpin, 'click', pushpinClick);
          //        var pushpinmouseout = Microsoft.Maps.Events.addHandler(pushpin, 'mouseout', pinmouseout);

          //        //Pin Anzeigen
          //        map.entities.push(pushpin);
          //        //Info Box 
          //        var infoboxOptions = {
          //            width: 200,
          //            height: 100,
          //            visible: false,
          //            showCloseButton: true,
          //            zIndex: ID,
          //            offset: new Microsoft.Maps.Point(20, 20),
          //            showPointer: true
          //        };
          //        var defaultInfobox = new Microsoft.Maps.Infobox(curendlocation, infoboxOptions);
          //        map.entities.insert(defaultInfobox, ID);
          //        defaultInfobox.ID = ID;
          //        defaultInfobox.setHtmlContent('<div   id="infoboxText" style="background-color:White;  border-width:1px; border-radius: 3%; border-color:DarkOrange; height:125px; width:150px"><a id="infoboxDescription" style="position:absolute; top:20px; left:5px; width:200px;"><img src="http://www.doit4you.net/ImageHandler.ashx?id=' + ID + '" style="height:100px; border-radius: 50%" /></a><div  style="text-align: center; " > <a  style="color:#0088B2;font-weight:bold; ">' + Name + '</a></div></div>');
          //        var content = defaultInfobox.getHtmlContent();
          //    }
          //}); // /> swervice Aufruf Top10 geladen
          //auto Coplett example Geo Lace exampel
          $scope.getLocation = function (val) {
              return $http.get('http://maps.googleapis.com/maps/api/geocode/json', {
                  params: {
                      address: val,
                      sensor: false
                  }
              }).then(function (res) {
                  var addresses = [
                  ];
                  angular.forEach(res.data.results, function (item) {
                      addresses.push(item.formatted_address);
                  });
                  return addresses;
              });
          };
          // suchen Button wurde gedrückt
          $scope.Suchen = function (SuchString) {
              var text = $scope.SuchString;
              map.entities.clear();
              //array für Box
              var locs = [
              ];
              map.setView({
                  zoom: 13,
                  center: latlon
              });
              var latlon = map.getCenter();
              //Suche Personen
              var Url = 'http://www.doit4you.net/Services/C1_net-Web-C1_Service.svc/json/GetPerson_SearchJ?Suchbegriff=' + SuchString;
              $http.get(Url).success(function (data, status) {
                  var count = data.GetPerson_SearchJResult.TotalCount;
                  var length = data.GetPerson_SearchJResult.RootResults.length;
                  if (count + length > 0) {
                      var jsondata = data.GetPerson_SearchJResult.RootResults;
                      for (var i = 0; i < jsondata.length; i++) {
                          var latitude = jsondata[i].LAT;
                          var longitude = jsondata[i].LON;
                          if (latitude != 0) {
                              var ID = jsondata[i].ID;
                              var Name = jsondata[i].Vorname.concat(' ', jsondata[i].Nachname);
                              var curendlocation = new Microsoft.Maps.Location(latitude, longitude);
                              var pushpin = new Microsoft.Maps.Pushpin(curendlocation, { typeName: 'Pers' });
                              pushpin.Title = Name;
                              pushpin.ID = ID;
                              var pushpinOver = Microsoft.Maps.Events.addHandler(pushpin, 'mouseover', pinMouseOver);
                              var ppClick = Microsoft.Maps.Events.addHandler(pushpin, 'click', pushpinClick);
                              var pushpinmouseout = Microsoft.Maps.Events.addHandler(pushpin, 'mouseout', pinmouseout);
                              //var pushpinOver= Microsoft.Maps.Events.addHandler(pushpin, 'mouseover', pinMouseOver);  
                              map.entities.push(pushpin);
                              // Box erweitern
                              locs.push(new Microsoft.Maps.Location(latitude, longitude));
                              var infoboxOptions = {
                                  width: 200,
                                  height: 100,
                                  visible: false,
                                  showCloseButton: true,
                                  zIndex: ID,
                                  offset: new Microsoft.Maps.Point(20, 20),
                                  showPointer: true
                              };
                              var defaultInfobox = new Microsoft.Maps.Infobox(curendlocation, infoboxOptions);
                              map.entities.insert(defaultInfobox, ID);
                              defaultInfobox.ID = ID;
                              defaultInfobox.setHtmlContent('<div id="infoboxText" style="background-color:White;  border-radius: 3%; border-width:1px; border-color:DarkOrange; height:123px; width:150px"><a id="infoboxDescription" style="position:absolute; top:20px; left:5px; width:200px;"><img src="http://www.doit4you.net/ImageHandler.ashx?id=' + ID + '" style="height:100px;" /></a><div  style="text-align: center; " > <a  style="color:#0088B2;font-weight:bold; ">' + Name + '</a></div></div>');
                              var content = defaultInfobox.getHtmlContent();
                          }
                      }
                      // ende for 
                      //Box anzeigen

                      if (locs.length > 0) {
                          var bounds = Microsoft.Maps.LocationRect.fromLocations(locs);
                          map.setView({
                              bounds: bounds,
                              padding: 100
                          });
                      }
                  }
                  //ende if

              }); // /> swervice Aufruf  Personen
              //Suche Offers
              var Url = 'http://www.doit4you.net/Services/C1_net-Web-C1_Service.svc/json/Get_Offer_SearchJ?Suchbegriff=' + SuchString;
              $http.get(Url).success(function (data, status) {
                  var count = data.Get_Offer_SearchJResult.TotalCount;
                  var length = data.Get_Offer_SearchJResult.RootResults.length;
                  if (count + length > 0) {
                      var jsondata = data.Get_Offer_SearchJResult.RootResults;
                      for (var i = 0; i < jsondata.length; i++) {
                          var latitude = jsondata[i].LAT;
                          var longitude = jsondata[i].LON;
                          if (latitude != 0) {
                              var ID = jsondata[i].ID;
                              var Name = jsondata[i].Name;
                              var Text = jsondata[i].Text;
                              var curendlocation = new Microsoft.Maps.Location(latitude, longitude);
                              var pushpinOptions = {
                                  typeName: 'Off'
                              };
                              var pushpin = new Microsoft.Maps.Pushpin(curendlocation, pushpinOptions);
                              pushpin.Title = Name;
                              pushpin.ID = ID;
                              var pushpinOver = Microsoft.Maps.Events.addHandler(pushpin, 'mouseover', pinMouseOver);
                              var ppClick = Microsoft.Maps.Events.addHandler(pushpin, 'click', pushpinClick);
                              var pushpinmouseout = Microsoft.Maps.Events.addHandler(pushpin, 'mouseout', pinmouseout);
                              map.entities.push(pushpin);
                              // Box erweitern
                              locs.push(new Microsoft.Maps.Location(latitude, longitude));
                              var infoboxOptions = {
                                  width: 200,
                                  height: 100,
                                  visible: false,
                                  showCloseButton: true,
                                  zIndex: ID,
                                  offset: new Microsoft.Maps.Point(20, 20),
                                  showPointer: true
                              };
                              var defaultInfobox = new Microsoft.Maps.Infobox(curendlocation, infoboxOptions);
                              map.entities.insert(defaultInfobox, ID);
                              defaultInfobox.ID = ID;
                              defaultInfobox.setHtmlContent('<div id="infoboxText" style="background-color:White; border-style:double;border-width:1px; border-color:Blue; height:250px; width:200px"><a id="infoboxDescription" style="position:absolute; top:30px; left:10px; width:220px;"><img src="http://www.doit4you.net/OffPic.ashx?id=' + ID + '" style="height:60px;" /></a><div  style="text-align: center; " ><a  style="color:#0088B2;font-weight:bold;">' + Name + '</a></div><a style="position:absolute; left:10px; top:100px;font-size:12px;"> ' + Text + '</a></div>');
                              var content = defaultInfobox.getHtmlContent();
                          }
                      }
                      // ende for 
                      //Box anzeigen

                      if (locs.length > 0) {
                          var bounds = Microsoft.Maps.LocationRect.fromLocations(locs);
                          map.setView({
                              bounds: bounds,
                              padding: 100
                          });
                      }
                  }
                  //ende if

              }); // /> swervice Aufruf  Offers
          }; // ende Suche


      }
      // ende Controller Funktion

  }
]); // ende Controller