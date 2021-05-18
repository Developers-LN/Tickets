/**=========================================================
 * Module: CashReportsController.js
 =========================================================*/

(function() {
    'use strict';

    angular
        .module('naut')
        .controller('CashReportsController', CashReportsController);

    CashReportsController.$inject = ['$scope', '$state', '$rootScope', '$stateParams'];
    function CashReportsController($scope, $state, $rootScope, $stateParams) {
        $scope.cash = {
            userId: undefined,
            raffleId: undefined
        };

        $scope.userList = function (userId, raffleId) {
            window.loading.show();
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: 'Cash/CashReportData?userId=' + userId + "&raffleId=" + raffleId,
                success: function (data) {
                    window.loading.hide();
                    if (userId <= 0 || raffleId <= 0) {
                        $scope.users = data.users;
                        $scope.raffles = data.raffles;
                    }
                    $rootScope.createSelect2();
                    $rootScope.destroyDataTable();
                    $scope.$apply();
                    $rootScope.dataTable();
                }
            });
        }
        $scope.updatelist = function () {
            $scope.userList($scope.userId);
        }

        $scope.userList(0);

        $scope.PrintReport = function () {
            window.open("Reports/GetCashReport?userId=" + $scope.cash.userId + "&raffleId=" + $scope.cash.raffleId);
        }
    }
})();
