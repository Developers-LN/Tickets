/**=========================================================
 * Module: PayedAwardPeriodReportsController.js
 =========================================================*/

(function () {
    'use strict';

    angular
        .module('naut')
        .controller('PayedAwardPeriodReportsController', PayedAwardPeriodReportsController);

    PayedAwardPeriodReportsController.$inject = ['$scope', '$state', '$rootScope', '$stateParams'];
    function PayedAwardPeriodReportsController($scope, $state, $rootScope, $stateParams) {
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

                    window.setTimeout(function () {
                        $scope.$apply();
                        $rootScope.createSelect2();
                    }, 0);
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
            if (cash.StartDate === undefined && cash.EndDate === undefined && cash.RaffleId === 0) {
                $scope.required = true;
                error += 'No ha introducido ningun dato';
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
            }
            catch (e) { }

            window.open('Reports/PayedAwardByPeriod?startDate=' + $scope.cash.StartDate + '&endDate=' + $scope.cash.EndDate + '&clientId=' + $scope.cash.ClientId + '&raffleId=' + $scope.cash.RaffleId);
        }

        //NUEVO CODIGO PARA GENERAR EXCEL DE LAS VENTAS Y CUENTAS POR COBRAR
        var url = 'http://' + location.host + '/';
        $scope.GenerateExcelReport = function () {
            if (validateData($scope.cash) === false) {
                return;
            }
            alertify.confirm("&iquest;Desea descargar el archivo Excel?", function (e) {
                if (e) {
                    try {
                        $scope.cash.EndDate = $rootScope.parseDate($scope.cash.EndDate, $scope.cash.EndDate).toJSON();
                        $scope.cash.StartDate = $rootScope.parseDate($scope.cash.StartDate, $scope.cash.StartDate).toJSON();
                    }
                    catch (err) { }

                    window.open('Integration/ExportToExcel?FechaInicio=' + $scope.cash.StartDate + '&FechaFin=' + $scope.cash.EndDate + '&raffleId=' + $scope.cash.RaffleId);
                }
            });
        }
    }
})();
