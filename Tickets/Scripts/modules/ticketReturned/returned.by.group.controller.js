/**=========================================================
 * Module: ReturnetByGroupController.js
 =========================================================*/

(function () {
    'use strict';

    angular
        .module('naut')
        .controller('ReturnetByGroupController', ReturnetByGroupController);

    ReturnetByGroupController.$inject = ['$scope', '$state', '$rootScope'];
    function ReturnetByGroupController($scope, $state, $rootScope) {
        var self = this;

        this.loadData = function () {
            window.loading.show();
            $.when(
                $.ajax($rootScope.serverUrl + 'ticket/clientApi/getClientSelect?statu=2089'),/*Clientes aprobados*/
                $.ajax($rootScope.serverUrl + 'ticket/raffleApi/getRaffleSelect?statu=0')//Sorteos en planificacion
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
                url: 'ticket/ticketAllocationApi/getReturnedByGroup?raffleId=' + $scope.RaffleId,
                success: function (response) {
                    window.loading.hide();
                    if (response.result == true) {
                        $scope.retunedGroups = response.object;
                        $rootScope.destroyDataTable();
                        $scope.$apply();
                        $rootScope.dataTable();
                    } else {
                        alertify.alert(response.message);
                    }
                }
            });
        }

        $scope.printReportDeliver = function () {
            window.open('Reports/DeliveredAllocation?clientId=' + $scope.ClientId + '&raffleId=' + $scope.RaffleId);
        }

        $scope.RaffleId = 0;
        $scope.ClientId = 0;
        this.loadData();
    }
})();
