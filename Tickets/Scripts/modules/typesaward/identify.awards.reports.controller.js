/**=========================================================
 * Module: AccountReceivableReportsController.js
 =========================================================*/

(function() {
    'use strict';

    angular
        .module('naut')
        .controller('IdentifyAwardsReportsController', IdentifyAwardsReportsController);

    IdentifyAwardsReportsController.$inject = ['$scope', '$state', '$rootScope', '$stateParams'];
    function IdentifyAwardsReportsController($scope, $state, $rootScope, $stateParams) {
        $scope.cash = {
            EndDate: undefined,
            StartDate: undefined,
            RaffleId: 0,
            ClientId: 0
        };
        $scope.required = false;

        $scope.update = function () {
            $scope.required = false;
            $scope.cash.RaffleId != "0" ? ($("#startDate").prop('disabled', true), $("#endDate").prop('disabled', true)) : ($("#startDate").prop('disabled', false), $("#endDate").prop('disabled', false));
            $('#startDate').val(undefined);
            $scope.cash.StartDate = undefined;
            $('#endDate').val(undefined);
            $scope.cash.EndDate = undefined;
        }

        $("#startDate").on("change", function (e) {
            $('#endDate').prop('min', e.target.value);
        });

        $("#reset-date").click(function () {
            $('#startDate').val(undefined);
            $scope.cash.StartDate = undefined;
        })
        $("#reset-date1").click(function () {
            $('#endDate').val(undefined);
            $scope.cash.EndDate = undefined;
        })

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
                    $rootScope.createSelect2();
                    $rootScope.dataTable();
                }
            });
        }

        $scope.loadRaffles();

        function validateData(cash) {
            var error = '', isReq = ' es un campo requerido. <br>';

            if (cash.StartDate !== undefined && $scope.cash.EndDate === undefined) {
                $scope.required = true;
                error += 'La Fecha Fin' + isReq;
            }
            if (cash.StartDate === undefined && $scope.cash.EndDate !== undefined) {
                $scope.required = true;
                error += 'La Fecha de Inicio' + isReq;
            }
            if (error !== '') {
                alertify.showError('Alerta', error);
            }
            return error === '';
        }
        $scope.printReport = function () {
            if (validateData($scope.cash) === false) {
                return;
            }
            try {
                $scope.cash.EndDate = $rootScope.parseDate($scope.cash.EndDate, $scope.cash.EndDate).toJSON();
                $scope.cash.StartDate = $rootScope.parseDate($scope.cash.StartDate, $scope.cash.StartDate).toJSON();
            } catch (e) {
            }
            window.open('Reports/IdentifyAwardsReport?startDate=' + $scope.cash.StartDate + '&endDate=' + $scope.cash.EndDate + '&clientId=' + $scope.cash.ClientId + '&raffleId=' + $scope.cash.RaffleId);
        }
        $scope.printReportResumen = function () {
            if (validateData($scope.cash) === false) {
                return;
            }
            try {
                $scope.cash.EndDate = $rootScope.parseDate($scope.cash.EndDate, $scope.cash.EndDate).toJSON();
                $scope.cash.StartDate = $rootScope.parseDate($scope.cash.StartDate, $scope.cash.StartDate).toJSON();
            } catch (e) {
            }
            window.open('Reports/IdentifyAwardsReportResumen?startDate=' + $scope.cash.StartDate + '&endDate=' + $scope.cash.EndDate + '&clientId=' + $scope.cash.ClientId + '&raffleId=' + $scope.cash.RaffleId);
        }
    }

})();
