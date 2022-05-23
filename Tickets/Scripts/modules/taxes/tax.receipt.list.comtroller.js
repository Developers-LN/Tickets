/**=========================================================
 * Module: TaxReceiptController.js
 =========================================================*/

(function () {
    'use strict';

    angular
        .module('naut')
        .controller('TaxReceiptListController', TaxReceiptListController);

    TaxReceiptListController.$inject = ['$scope', '$state', '$rootScope'];
    function TaxReceiptListController($scope, $state, $rootScope) {
        var self = this;
        this.getTaxReceipt = function () {
            window.loading.show();
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: 'TaxReceipt/GetTaxReceiptList',
                success: function (data) {
                    window.loading.hide();
                    $scope.taxReceiptList = data.taxReceipts;
                    $rootScope.destroyDataTable();
                    $scope.$apply();
                    $rootScope.createSelect2();
                    $rootScope.dataTable();
                }
            });
        }

        self.getTaxReceipt();
    }
})();
