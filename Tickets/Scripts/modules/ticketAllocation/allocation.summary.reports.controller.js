/**=========================================================
 * Module: AllocationSummaryReportsController.js
 =========================================================*/

(function() {
    'use strict';

    angular
        .module('naut')
        .controller('AllocationSummaryReportsController', AllocationSummaryReportsController);

    AllocationSummaryReportsController.$inject = ['$scope', '$state', '$rootScope', '$stateParams'];
    function AllocationSummaryReportsController($scope, $state, $rootScope, $stateParams) {
        $scope.allocation = {
            RaffleId: undefined,
            ClientId: undefined
        };

        this.loadData = function () {
            $rootScope.systemLoading = true;
            $.when(
                $.ajax($rootScope.serverUrl + 'ticket/clientApi/getClientSelect?statu=2089'),/*Clientes aprobados*/
                $.ajax($rootScope.serverUrl + 'ticket/raffleApi/getRaffleSelect?statu=0')//todos
            ).then(function (clientResponse, raffleResponse) {
                $rootScope.systemLoading = false;
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

        this.loadData();

        function validateData(allocation) {
            var error = '', isReq = ' es un campo requerido. <br>';

            if (allocation.RaffleId === undefined) {
                error += 'El Sorteo' + isReq;
            }
            if (allocation.ClientId === undefined) {
                error += 'El Cliente' + isReq;
            }
            if (error !== '') {
                alertify.showError('Alerta', error);
            }
            return error === '';
        }

        $scope.printReport = function () {
            if (validateData($scope.allocation) === false) {
                return;
            }
            window.open('Reports/AllocationSummaryby?clientId=' + $scope.allocation.ClientId + '&raffleId=' + $scope.allocation.RaffleId);
        }
    }
})();
