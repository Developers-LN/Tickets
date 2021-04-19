/**=========================================================
 * Module: ClientController.js
 =========================================================*/

(function() {
    'use strict';

    angular
        .module('naut')
        .controller('ClientController', ClientController);

    ClientController.$inject = ['$scope', '$rootScope', '$state', 'clientWorkflow'];
    function ClientController($scope, $rootScope, $state, clientWorkflow) {
        clientWorkflow.init($rootScope, $scope, $state);
    }
})();
