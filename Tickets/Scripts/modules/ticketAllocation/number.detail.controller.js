/**=========================================================
 * Module: NumberDetailController.js
 =========================================================*/

(function() {
    'use strict';

    angular
        .module('naut')
        .controller('NumberDetailController', NumberDetailController);

    NumberDetailController.$inject = ['$scope', '$state', '$rootScope', '$stateParams'];
    function NumberDetailController($scope, $state, $rootScope, $stateParams) {
        var self = this;
        if ($rootScope.returnUrl) {
            $scope.returnUrl = $rootScope.returnUrl;
        } else {
            $scope.returnUrl = '/#/dashboard'
        }

        this.loadIdentifyData = function () {
            window.loading.show();
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: 'ticket/ticketAllocationApi/getTicketNumberDetails?numberId=' + $stateParams.numberId,
                success: function (data) {
                    var number = data.object;
                    number.Transactions = number.Transactions.map(function (transaction) {
                        transaction.Date = new Date(transaction.Date);
                        return transaction;
                    });
                    $scope.number = number;
                    window.loading.hide();
                    $rootScope.destroyDataTable();
                    $scope.$apply();
                    $rootScope.dataTable();
                }
            });
        }
        this.loadIdentifyData();
    }
})();
