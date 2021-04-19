/**=========================================================
 * Module: AllocationPrintController.js
 =========================================================*/

(function() {
    'use strict';

    angular
        .module('naut')
        .controller('AllocationPrintController', AllocationPrintController);

    AllocationPrintController.$inject = ['$scope', '$state', '$rootScope'];
    function AllocationPrintController($scope, $state, $rootScope) {
        var self = this;

        this.loadData = function () {
            window.loading.show();
            $.when(
                $.ajax($rootScope.serverUrl + 'ticket/clientApi/getClientSelect?statu=2089'),/*Clientes aprobados*/
                $.ajax($rootScope.serverUrl + 'ticket/raffleApi/getRaffleSelect?statu=68')//Sorteos en planificacion
            ).then(function (clientResponse, raffleResponse) {
                window.loading.hide();
                if (clientResponse[1] == 'success') {
                    $scope.clients = clientResponse[0].object;
                }
                if (raffleResponse[1] == 'success') {
                    $scope.raffles = raffleResponse[0].object;
                }
                window.setTimeout(function () {
                    $scope.$apply();
                    $rootScope.createSelect2();
                }, 0);
            });
        }

        $scope.changeValues = function () {
            if ($scope.RaffleId == 0) {
                return;
            }

            window.loading.show();
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: $rootScope.serverUrl + 'ticket/ticketAllocationApi/getTicketAllocationList?raffleId=' + $scope.RaffleId + '&clientId=' + $scope.ClientId,
                success: function (response) {
                    window.loading.hide();
                    if (response.result == true) {
                        $scope.allocations = response.object;
                        $rootScope.destroyDataTable();
                        $scope.$apply();
                        $rootScope.dataTable();
                    } else {
                        alertify.alert(response.message);
                    }
                }
            });
        }

        $scope.updateAllocation = function () {
            self.changeValues($scope.RaffleId, $scope.ClientId);
        }

        $scope.RaffleId = 0;
        $scope.ClientId = 0;

        this.loadData();
    }
})();
