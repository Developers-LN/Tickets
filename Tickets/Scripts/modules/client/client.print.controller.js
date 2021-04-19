/**=========================================================
 * Module: ClientPrintController.js
 =========================================================*/

(function() {
    'use strict';

    angular
        .module('naut')
        .controller('ClientPrintController', ClientPrintController);

    ClientPrintController.$inject = ['$scope', '$rootScope', '$state'];
    function ClientPrintController($scope, $rootScope, $state) {
        this.getClientList = function () {
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: 'Client/GetList',
                success: function (data) {
                    $scope.clientList = data.clients;
                    $scope.raffles = data.raffles;
                    $scope.$apply();
                }
                
            });
        }
        $rootScope.createSelect2();
        this.getClientList();
    }
})();
