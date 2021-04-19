/**=========================================================
 * Module: RoutesRun
 =========================================================*/

(function() {
    'use strict';

    angular
        .module('naut')
        .run(appRun);
    /* @ngInject */   
    function appRun($rootScope, $window, $state) {

      // Hook not found
      $rootScope.$on('$stateNotFound',
        function(event, unfoundState, fromState, fromParams) {
            console.log(unfoundState.to); // "lazy.state"
            console.log(unfoundState.toParams); // {a:1, b:2}
            console.log(unfoundState.options); // {inherit:false} + default options
        });
      $rootScope.skipedModules =  [{
          name: 'app.securityLogin',
          view: true
      }, {
          name: 'app.securityAccessDenied',
          view: true
      }, {
          name: 'app.dashboard',
          view: true
      }, {
          name: 'app.securityChangePassword',
          view: true
      }];

      $rootScope.ticketsModuleList = $rootScope.skipedModules;
      $rootScope.systemLoading = true;
      $rootScope.$on('$stateChangeStart', function (event, toState, toParams, fromState, fromParams) {
          $rootScope.systemLoading = true;
          $rootScope.app.title = '';
          if (fromState.name == '') {
              $rootScope.verifyUserLogin();
          }
          var result = false;
          $($rootScope.ticketsModuleList).each(function (i, item) {
              if (toState.name === item.name) {
                  $rootScope.verifyModuleAccess(item);
                  result = item.view;
              }
          });
          if (result === false) {
              event.preventDefault();
              $state.go('app.securityAccessDenied');
          }
      });
      // Hook success
      $rootScope.$on('$stateChangeSuccess',
        function (event, toState, toParams, fromState, fromParams) {
            if($rootScope.barcodeReader != undefined){
                window.document.body.removeEventListener('keydown', $rootScope.barcodeReader, false);
            }
          // success here
            // display new view from top 
            window.setTimeout(function () {
                $('#mainMenu').children().each(function (i, menu) {
                    $rootScope.verifyCanViewMenu(menu);

                    $rootScope.systemLoading = false;
                });
            }, 0);
          $window.scrollTo(0, 0);
        });

    }
    appRun.$inject = ['$rootScope', '$window', '$state'];

})();

