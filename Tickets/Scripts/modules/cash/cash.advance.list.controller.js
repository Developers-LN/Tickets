/**=========================================================
 * Module: CashAdvanceListController.js
 =========================================================*/

(function () {
    'use strict';

    angular
        .module('naut')
        .controller('CashAdvanceListController', CashAdvanceListController);

    CashAdvanceListController.$inject = ['$scope', '$state', '$rootScope'];
    function CashAdvanceListController($scope, $state, $rootScope) {
        $scope.ClientId = 0;
        $scope.updateCashAdvance = function (clientId) {
            window.loading.show();
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: 'Cash/GetCashAdvancesList?clientId=' + clientId,
                success: function (data) {
                    window.loading.hide();
                    $scope.cashAdvances = data.creditNotes;
                    if ($scope.ClientId === 0) {
                        $scope.clients = data.clients;
                    }

                    $rootScope.destroyDataTable();
                    $scope.$apply();
                    $rootScope.createSelect2();
                    $rootScope.dataTable();
                }
            });
        }

        $scope.updateCashAdvance(0);

        $scope.changeCashAdvance = function () {
            $scope.updateCashAdvance($scope.ClientId);
        }
    }
})();
