/**=========================================================
 * Module: DeliveryListController.js
 =========================================================*/

(function () {
    'use strict';

    angular
        .module('naut')
        .controller('NoteCreaditTaxReceiptListController', NoteCreaditTaxReceiptListController);

    NoteCreaditTaxReceiptListController.$inject = ['$scope', '$state', '$rootScope'];
    function NoteCreaditTaxReceiptListController($scope, $state, $rootScope) {
        $scope.ClientId = 0;
        $scope.updateCreditNote = function (clientId) {
            window.loading.show();
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: 'Cash/GetCreditNoteTaxReceiptList?clientId=' + clientId,
                success: function (data) {
                    window.loading.hide();
                    $scope.creditNotes = data.creditNotes;
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

        $scope.updateCreditNote(0);

        $scope.changeCreditNote = function () {
            $scope.updateCreditNote($scope.ClientId);
        }
    }
})();
