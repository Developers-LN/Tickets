/**=========================================================
 * Module: ConfigController.js
 =========================================================*/

(function() {
    'use strict';

    angular
        .module('naut')
        .controller('ConfigController', ConfigController);
    
    ConfigController.$inject = ['$scope','$rootScope', '$state', 'configCatalog', 'configWorkflow'];

    function ConfigController($scope, $rootScope, $state, configCatalog, configWorkflow) {
        $scope.officeList = [];
        configCatalog.init($rootScope, $scope, $state);
        configWorkflow.init($rootScope, $scope, $state);
    }
})();
