/**=========================================================
 * Module: RaffleListController.js
 =========================================================*/

(function() {
    'use strict';

    angular
        .module('naut')
        .controller('RaffleListController', RaffleListController);

    RaffleListController.$inject = ['$scope', '$rootScope'];
    function RaffleListController($scope, $rootScope) {
        /*jshint validthis:true*/
        var self = this;
        $scope.deleteRaffle = function (raffle) {
            // confirm dialog
            alertify.confirm("&iquest;Desea borrar este sorteo?", function (e) {
                if (e) {
                    window.loading.show();
                    $.ajax({
                        type: 'POST',
                        dataType: 'json',
                        url: $rootScope.serverUrl + 'ticket/raffleApi/suspend',
                        data: raffle,
                        success: function (response) {
                            window.loading.hide();
                            if (response.result == true) {
                                $rootScope.destroyDataTable();
                                self.getRaffleList();
                                alertify.success(response.message);
                            } else {
                                alertify.success(response.message);
                            }
                        }
                    });
                }
            });
        }

        this.getRaffleList = function () {
            window.loading.show();
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: $rootScope.serverUrl + 'ticket/raffleApi/getRaffleListSP',
                success: function (response) {
                    window.loading.hide();
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

        self.getRaffleList();
    }
})();
