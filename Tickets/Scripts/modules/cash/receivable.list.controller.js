/**=========================================================
 * Module: ReceivableListController.js
 =========================================================*/

(function () {
    'use strict';

    angular
        .module('naut')
        .controller('ReceivableListController', ReceivableListController);

    ReceivableListController.$inject = ['$scope', '$state', '$rootScope'];
    function ReceivableListController($scope, $state, $rootScope) {
        $scope.RaffleId = 0;
        $scope.ClientId = 0;

        $rootScope.createSelect2();

        $scope.updateRaffles = function (raffleId, clientId) {
            window.loading.show();
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: 'Cash/GetReceivableList?raffleId=' + raffleId + '&clientId=' + clientId,
                success: function (data) {
                    window.loading.hide();

                    $scope.invoices = data.invoices.map(function (invoice) {
                        //invoice.InvoiceDate = new Date(invoice.InvoiceDate);
                        //invoice.xpiredDate = new Date(invoice.xpiredDate);
                        return invoice;
                    });
                    if (raffleId <= 0) {
                        $scope.raffles = data.raffles;
                    }
                    if (clientId <= 0) {
                        $scope.clients = data.clients;
                    }
                    $scope.showTotals();
                    $rootScope.destroyDataTable();
                    $scope.$apply();
                    $rootScope.dataTable();
                }
            });
        }
        $scope.updateReceivable = function () {
            $scope.updateRaffles($scope.RaffleId, $scope.ClientId);
        }
        $scope.totalBruton = 0;
        $scope.totalDiscount = 0;
        $scope.totalNeto = 0;
        $scope.totalRest = 0;
        $scope.showTotals = function () {
            $scope.totalBruton = 0;
            $scope.totalDiscount = 0;
            $scope.totalNeto = 0;
            $scope.totalRest = 0;
            $scope.invoices.forEach(function (invoice) {
                if (invoice.PaymentStatu != 2084) {
                    $scope.totalBruton += invoice.totalInvoice;
                    $scope.totalDiscount += invoice.discount;
                    $scope.totalNeto += invoice.totalQuantity;
                    $scope.totalRest += invoice.totalRestant;
                }
            });
        }

        $scope.saveReceivablePayment = function (invoiceId) {
            window.loading.show();
            $.ajax({
                type: 'POST',
                dataType: 'json',
                url: 'Cash/ReceivablePayment',
                data: { invoiceId: invoiceId },
                success: function (data) {
                    window.loading.hide();
                    if (data.result === true) {
                        alertify.success(data.message);
                        window.open('Reports/ReceivableReport?invoiceId=' + invoiceId);
                    } else {
                        alertify.alert(data.message);
                    }
                }
            });
        }
        $scope.updateRaffles(0, 0);
    }
})();
