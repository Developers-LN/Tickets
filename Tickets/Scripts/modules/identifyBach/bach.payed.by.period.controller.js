/**=========================================================
 * Module: PayedBachByPeriodReportsController.js
 =========================================================*/

(function () {
    'use strict';

    angular
        .module('naut')
        .controller('PayedBachByPeriodReportsController', PayedBachByPeriodReportsController);

    PayedBachByPeriodReportsController.$inject = ['$scope', '$state', '$rootScope', '$stateParams'];
    function PayedBachByPeriodReportsController($scope, $state, $rootScope, $stateParams) {
        $scope.bach = {
            EndDate: undefined,
            StartDate: undefined,
            RaffleId: 0,
            ClientId: 0
        };

        $scope.required = false;

        $scope.update = function () {
            $scope.required = false;
            $scope.bach.RaffleId != "0" ? ($("#startDate").prop('disabled', true), $("#endDate").prop('disabled', true)) : ($("#startDate").prop('disabled', false), $("#endDate").prop('disabled', false));
            $('#startDate').val(undefined);
            $scope.bach.StartDate = undefined;
            $('#endDate').val(undefined);
            $scope.bach.EndDate = undefined;
        }
        $("#startDate").on("change", function (e) {
            $('#endDate').prop('min', e.target.value);
        });

        $("#reset-date").click(function () {
            $('#startDate').val(undefined);
            $scope.bach.StartDate = undefined;
        })
        $("#reset-date1").click(function () {
            $('#endDate').val(undefined);
            $scope.bach.EndDate = undefined;
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

        function validateData(bach) {
            var error = '', isReq = ' es un campo requerido. <br>';

            if (bach.StartDate !== undefined && $scope.bach.EndDate === undefined) {
                $scope.required = true;
                error += 'La Fecha Fin' + isReq;
            }
            if (bach.StartDate === undefined && $scope.bach.EndDate !== undefined) {
                $scope.required = true;
                error += 'La Fecha de Inicio' + isReq;
            }
            if (bach.StartDate === undefined && bach.EndDate === undefined && bach.RaffleId === 0) {
                $scope.required = true;
                error += 'No ha introducido ningun dato';
            }
            if (error !== '') {
                alertify.showError('Alerta', error);
            }
            return error === '';
        }

        $scope.printReport = function () {
            if (validateData($scope.bach) === false) {
                return;
            }
            try {
                $scope.bach.EndDate = $rootScope.parseDate($scope.bach.EndDate, $scope.bach.EndDate).toJSON();
                $scope.bach.StartDate = $rootScope.parseDate($scope.bach.StartDate, $scope.bach.StartDate).toJSON();
            }
            catch (e) { }

            window.open('Reports/AccountsReceivables?startDate=' + $scope.bach.StartDate + '&endDate=' + $scope.bach.EndDate + '&clientId=' + $scope.bach.ClientId + '&raffleId=' + $scope.bach.RaffleId);
        }

        //NUEVO CODIGO PARA GENERAR EXCEL DE LAS VENTAS Y CUENTAS POR COBRAR
        var url = 'http://' + location.host + '/';
        $scope.GenerateExcelReport = function () {
            if (validateData($scope.bach) === false) {
                return;
            }
            alertify.confirm("&iquest;Desea descargar el archivo Excel?", function (e) {
                if (e) {
                    try {
                        $scope.bach.EndDate = $rootScope.parseDate($scope.bach.EndDate, $scope.bach.EndDate).toJSON();
                        $scope.bach.StartDate = $rootScope.parseDate($scope.bach.StartDate, $scope.bach.StartDate).toJSON();
                    }
                    catch (err) { }

                    window.open('Integration/BachPayedByPeriod?FechaInicio=' + $scope.bach.StartDate + '&FechaFin=' + $scope.bach.EndDate + '&raffleId=' + $scope.bach.RaffleId);
                }
            });
        }
    }
})();
