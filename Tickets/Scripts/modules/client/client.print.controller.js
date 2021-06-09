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
            window.loading.show();
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: 'Client/GetList',
                success: function (data) {
                    window.loading.hide();
                    $scope.clientList = data.clients;
                    $scope.raffles = data.raffles;
                    $scope.$apply();
                    $rootScope.dataTable();
                }
                
            });
        }

        $rootScope.createSelect2();
        this.getClientList();
    }
})();
