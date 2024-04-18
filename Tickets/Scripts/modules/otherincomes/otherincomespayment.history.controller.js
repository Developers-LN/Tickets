/**=========================================================
 * Module: OtherIncomesHistoryController.js
 =========================================================*/

(function () {
    'use strict';

    angular
        .module('naut')
        .controller('OtherIncomesHistoryController', OtherIncomesHistoryController);

    OtherIncomesHistoryController.$inject = ['$scope', '$state', '$rootScope', '$stateParams'];
    function OtherIncomesHistoryController($scope, $state, $rootScope, $stateParams) {

        this.getPaymentHistory = function () {
            window.loading.show();
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: 'OtherIncomes/GetPaymentHistory?otherIncomeId=' + $stateParams.otherincomeId,
                success: function (data) {
                    window.loading.hide();
                    $scope.paymentHistory = data.paymentHistory;
                    $scope.NoCatalogAccount = data.NoCatalogAccount;
                    $scope.AccountName = data.AccountName;
                    $scope.origin = data.origin;
                    $scope.accountType = data.accountType;
                    $scope.periodicity = data.periodicity;
                    $scope.status = data.status;
                    $scope.Description = data.Description;

                    $rootScope.destroyDataTable();
                    $scope.$apply();
                    $rootScope.dataTable();
                }
            });
        }

        this.getPaymentHistory();
    }
})();
