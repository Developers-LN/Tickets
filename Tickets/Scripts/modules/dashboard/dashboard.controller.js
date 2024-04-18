/**=========================================================
 * Module: DashboardController.js
 =========================================================*/

(function() {
    'use strict';

    angular
        .module('naut')
        .controller('DashboardController', DashboardController);
    
    DashboardController.$inject = ['$scope', '$timeout', '$rootScope'];
    function DashboardController($scope, $timeout, $rootScope) {
        var self = this;
        $scope.RaffleId = 0;
        this.loadData = function () {
            window.loading.show();
            $.when(
                $.ajax($rootScope.serverUrl + "ticket/raffleApi/getRaffleSelect"),
                $.ajax($rootScope.serverUrl + "ticket/clientApi/getClientSelect")
                )
            .then(function (raffleResponse, clientsR) {
                window.loading.hide();
                if (raffleResponse[1] == 'success') {
                    $scope.raffles = raffleResponse[0].object;
                    if ($scope.raffles.length > 0) {
                        $scope.RaffleId = $scope.raffles[0].value;
                        $scope.AllocateRaffleId = $scope.raffles[0].value;;
                    }
                }
                if (clientsR[1] == 'success') {
                    $scope.clients = clientsR[0].object;
                }

                $scope.updateProduction();
                $scope.updateAllocate();
                $scope.$apply();
                $rootScope.createSelect2();
            }, function (error) {
                window.loading.show();
                console.log('Error: ' + error);
            });
        }
        $scope.productionIsLoaing = false;
        $scope.productions = [];
        $scope.updateProduction = function () {
            if ($scope.RaffleId != 0) {
                if (window.productionDough) {
                    window.productionDough.destroy();
                }
                $scope.productionIsLoaing = true;
                $scope.productions = [];
                $.ajax({
                    type: 'GET',
                    contentType: 'application/json; charset=utf-8',
                    url: 'Dashboard/GetRaffleProduction?raffleId=' + $scope.RaffleId,
                    success: function (data) {
                        $scope.productions = data.productions;
                        $scope.productionIsLoaing = false;
                        var ctx = document.getElementById("chart-area").getContext("2d");
                        window.productionDough = new Chart(ctx).Doughnut(data.productions, { responsive: true });

                        $scope.$apply();
                    }
                });
            }
        }

        $scope.allocationsIsLoaing = false;
        $scope.allocations = [];
        $scope.AllocateRaffleId = 0;
        $scope.updateAllocate = function () {
            if ($scope.AllocateRaffleId != 0) {
                if (window.allocationDough) {
                    window.allocationDough.destroy();
                }
                $scope.allocationsIsLoaing = true;
                $scope.allocations = [];
                $.ajax({
                    type: 'GET',
                    contentType: 'application/json; charset=utf-8',
                    url: 'Dashboard/GetRaffleAllocation?raffleId=' + $scope.AllocateRaffleId,
                    success: function (data) {
                        $scope.allocations = data.allocations.map(function (allocation, i) {
                            var number = i * 10 + (20);
                            allocation.color = data.colors[number];
                            return allocation;
                        });
                        $scope.allocationsIsLoaing = false;
                        var ctx = document.getElementById("chart-area-allocations").getContext("2d");
                        window.allocationDough = new Chart(ctx).Pie($scope.allocations, { responsive: true });

                        $scope.$apply();
                    }
                });
            }
        }

        self.loadData();

        /*this.testWebService = function () {
            var identify = {
                raffleId: 3753,
                clientId: 1,
                raffleDate: '02-01-2016',
                userName: 'admin',
                password: 'Concentra4168',
                ticketNumbers: [{
                    tiketNumber: '89551',
                    fractionFrom: 1,
                    fractionTo: 20
                },
                {
                    tiketNumber: '89552',
                    fractionFrom: 1,
                    fractionTo: 20
                },
                {
                    tiketNumber: '89553',
                    fractionFrom: 1,
                    fractionTo: 20
                },
                {
                    tiketNumber: '89554',
                    fractionFrom: 1,
                    fractionTo: 20
                },
                {
                    tiketNumber: '89555',
                    fractionFrom: 1,
                    fractionTo: 20
                }]
            };
            $.ajax({
                type: 'POST',
                dataType: 'json',
                url: 'integration/electronicTicketApi/identifyticket',
                data: identify,
                success: function (data) {
                    if (data.result === true) {
                        console.log(data);
                    } else {
                        console.log(data);
                    }
                }
            });
        }
        this.testWebService();*/
    }
})();
