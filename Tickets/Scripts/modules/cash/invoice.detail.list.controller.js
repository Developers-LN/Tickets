/**=========================================================
 * Module: InvoiceDetailListController.js
 =========================================================*/

(function() {
    'use strict';

    angular
        .module('naut')
        .controller('InvoiceDetailListController', InvoiceDetailListController);

    InvoiceDetailListController.$inject = ['$scope', '$state', '$rootScope'];
    function InvoiceDetailListController($scope, $state, $rootScope) {
        $scope.RaffleId = 0;
        $scope.ClientId = 0;
        $scope.raffles = [];
        $scope.clients = [];
        $scope.updateInvoiceDetails = function () {
            window.loading.show();
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: 'Cash/CashReportListData?raffleId=' + $scope.RaffleId + '&clientId=' + $scope.ClientId,
                success: function (data) {
                    window.loading.hide();
                    if ($scope.RaffleId == 0 && $scope.ClientId == 0) {
                        $scope.raffles = data.raffles;
                        $scope.clients = data.clients;
                    }
                    $scope.invoiceDetails = data.invoiceDetails.map(function (invoiceDetail) {
                        invoiceDetail.StartDate = new Date(invoiceDetail.StartDate);
                        invoiceDetail.EndDate = new Date(invoiceDetail.EndDate);
                        return invoiceDetail;
                    });

                    $rootScope.destroyDataTable();
                    $scope.$apply();
                    $rootScope.dataTable();
                    
                }
            });
        }
        $rootScope.createSelect2();
        $scope.updateInvoiceDetails();
    }
})();
