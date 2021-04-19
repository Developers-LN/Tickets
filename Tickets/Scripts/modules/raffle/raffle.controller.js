/**=========================================================
 * Module: RaffleController.js
 =========================================================*/

(function() {
    'use strict';

    angular
        .module('naut')
        .controller('RaffleController', RaffleController);

    RaffleController.$inject = ['$scope', '$rootScope', '$state', 'raffleServices', 'raffleAwardServices'];
    function RaffleController($scope, $rootScope, $state, raffleServices, raffleAwardServices) {
        raffleServices.init($rootScope, $scope, $state);
        raffleAwardServices.init($rootScope, $scope, $state);
    }
})();
