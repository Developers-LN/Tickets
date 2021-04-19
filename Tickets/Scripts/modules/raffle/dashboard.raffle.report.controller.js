/**=========================================================
 * Module: DashboardRaffleReportController.js
 =========================================================*/

(function() {
    'use strict';

    angular
        .module('naut')
        .controller('DashboardRaffleReportController', DashboardRaffleReportController);

    DashboardRaffleReportController.$inject = ['$scope', '$state', '$rootScope', '$stateParams'];
    function DashboardRaffleReportController($scope, $state, $rootScope, $stateParams) {
        $scope.sendData = {
            EndDate: undefined,
            StartDate: undefined,
            RaffleId: 0,
            ClientId: 0
        };

        $scope.loadRaffles = function () {
            window.loading.show();
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: 'Cash/CashReportData',
                success: function (data) {
                    window.loading.hide();
                    $scope.clients = data.clients;
                    $scope.raffles = data.raffles;
                    $scope.$apply();
                    $rootScope.dataTable();
                }
            });
        }

        $scope.loadRaffles();

        function validateData(send) {
            var error = '', isReq = ' es un campo requerido. <br>';
            
            if (send.StartDate === undefined) {
                error += 'La Fecha de Inicio' + isReq;
            }
            if (send.EndDate === undefined) {
                error += 'La Fecha Fin' + isReq;
            }
            if (error !== '') {
                alertify.showError('Alerta', error);
            }
            return error === '';
        }

        $scope.printReport = function () {
            try {
                $scope.sendData.EndDate = $rootScope.parseDate($scope.sendData.EndDate, $scope.sendData.EndDate).toJSON();
                $scope.sendData.StartDate = $rootScope.parseDate($scope.sendData.StartDate, $scope.sendData.StartDate).toJSON();
            } catch (e) {
            }

            if (validateData($scope.sendData) === false) {
                return;
            }
            window.open('Reports/RaffleDashboardReport?startDate=' + $scope.sendData.StartDate + '&endDate=' + $scope.sendData.EndDate + '&clientId=' + $scope.sendData.ClientId + '&raffleId=' + $scope.sendData.RaffleId);
        }
    }
})();
