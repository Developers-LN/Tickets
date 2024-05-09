/**=========================================================
 * Module: OtherIncomesGroupHistoryController.js
 =========================================================*/

(function () {
    'use strict';

    angular
        .module('naut')
        .controller('OtherIncomesGroupHistoryController', OtherIncomesGroupHistoryController);

    OtherIncomesGroupHistoryController.$inject = ['$scope', '$state', '$rootScope', '$stateParams'];
    function OtherIncomesGroupHistoryController($scope, $state, $rootScope, $stateParams) {

        this.getPaymentHistory = function () {
            window.loading.show();
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: 'OtherIncomes/GetPaymentGroupHistory?otherIncomeGroupId=' + $stateParams.otherIncomeGroupId,
                success: function (data) {
                    window.loading.hide();
                    $scope.Id = data.Id;
                    $scope.Description = data.Description;
                    $scope.CreateDate = data.CreateDate;
                    $scope.Status = data.Status;
                    $scope.statusDesc = data.statusDesc;
                    $scope.payments = data.payments;

                    $rootScope.destroyDataTable();
                    $scope.$apply();
                    $rootScope.dataTable();
                }
            });
        }

        this.getPaymentHistory();
    }
})();
