/**=========================================================
 * Module: RaffleGeneratedController.js
 =========================================================*/

(function() {
    'use strict';

    angular
        .module('naut')
        .controller('RaffleGeneratedController', RaffleGeneratedController);

    RaffleGeneratedController.$inject = ['$scope', '$rootScope', '$state'];
    function RaffleGeneratedController($scope, $rootScope, $state) {

        this.getSolteoList = function () {
            window.loading.show();
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: $rootScope.serverUrl + 'ticket/raffleApi/getGeneratedRaffles',
                success: function (response) {
                    window.loading.hide();
                    $scope.raffles = response.object.map(function (raffle) {
                        raffle.raffleDate = new Date(raffle.raffleDate);
                        raffle.startReturnDate = new Date(raffle.startReturnDate);
                        raffle.endReturnDate = new Date(raffle.endReturnDate);
                        return raffle;
                    });
                    $scope.$apply();
                    $rootScope.dataTable();
                }
            });
        }

        this.getTaxReceiptList = function () {
            window.loading.show();
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: 'TaxReceipt/GetList',
                success: function (response) {
                    $scope.taxreceipts = response.taxreceipts;
                }
            });
        }

        $scope.downloadRaffleReport = function (raffleId) {
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: 'Reports/PrintImage?raffleId='+raffleId,
                success: function (data) {
                    if (data.result === true) {
                        window.location.href = '/generalRaffle/' + data.fileName;
                    } else {
                        alertify.alert(data.message);
                    }
                    $scope.$apply();
                }
            });
        }

        this.getTaxReceiptList();
        this.getSolteoList();
    }
})();
