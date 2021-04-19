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
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: $rootScope.serverUrl + 'ticket/raffleApi/getGeneratedRaffles',
                success: function (response) {
                    $scope.raffles = response.object.map(function (raffle) {
                        raffle.raffleDate = new Date(raffle.raffleDateLong);
                        raffle.startReturnDate = new Date(raffle.startReturnDateLong);
                        raffle.endReturnDate = new Date(raffle.endReturnDateLong);
                        return raffle;
                    });
                    $scope.$apply();
                    $rootScope.dataTable();
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

        this.getSolteoList();
    }
})();
