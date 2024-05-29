/**=========================================================
 * Module: InvoiceDetailCashReportsController.js
 =========================================================*/

(function() {
    'use strict';

    angular
        .module('naut')
        .controller('InvoiceDetailCashReportsController', InvoiceDetailCashReportsController);

    InvoiceDetailCashReportsController.$inject = ['$scope', '$state', '$rootScope', '$stateParams'];
    function InvoiceDetailCashReportsController($scope, $state, $rootScope, $stateParams) {
        $scope.cash = {
            EndDate: undefined,
            StartDate: undefined,
            RaffleId: undefined,
            ClientId: undefined,
            PayingFund: undefined
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
                    $scope.payingFunds = data.payingFund;
                    $scope.raffles = data.raffles;

                    $scope.$apply();
                    $rootScope.createSelect2();
                    $rootScope.dataTable();
                }
            });
        }

        $scope.loadRaffles();

        function validateData(invoiceDetail) {
            var error = '', isReq = ' es un campo requerido. <br>';

            if (invoiceDetail.RaffleId === undefined) {
                error += 'El Sorteo' + isReq;
            }
            if (invoiceDetail.ClientId === undefined) {
                error += 'El Cliente' + isReq;
            }
            if (invoiceDetail.StartDate === undefined) {
                error += 'La Fecha de Inicio' + isReq;
            }
            if (invoiceDetail.EndDate === undefined) {
                error += 'La Fecha Fin' + isReq;
            }
            if (error !== '') {
                alertify.showError('Alerta', error);
            }
            return error === '';
        }

        $scope.printReport = function () {
            try {
                $scope.cash.EndDate = $rootScope.parseDate($scope.cash.EndDate, $scope.cash.EndDate).toJSON();
                $scope.cash.StartDate = $rootScope.parseDate($scope.cash.StartDate, $scope.cash.StartDate).toJSON();
            } catch (e) {
            }

            if (validateData($scope.cash) === false) {
                return;
            }
            window.loading.show();
            $.ajax({
                type: 'POST',
                dataType: 'json',
                url: 'Cash/InvoiceDetailCreate',
                data: $scope.cash,
                success: function (data) {
                    window.loading.hide();
                    if (data.result === true) {
                        alertify.success(data.message);
                        window.open("Reports/InvoiceDeatilCashReport?invoiceDetailId=" + data.invoiceDetailsObject.Id);
                        window.location.href = '#/report/invoiceDetailList';
                    } else {
                        alertify.alert(data.message);
                    }
                }
            });
        }
    }
})();
