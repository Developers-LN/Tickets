/**=========================================================
 * Module: ReturnedAwardController.js
 =========================================================*/

(function () {
    'use strict';

    angular
        .module('naut')
        .controller('ReturnedAwardController', ReturnedAwardController);

    ReturnedAwardController.$inject = ['$scope', '$state', '$rootScope', '$http'];
    function ReturnedAwardController($scope, $state, $rootScope, $http) {
        $rootScope.returnUrl = "/#/ticket/Returned/awards";

        this.getData = function () {
            $http.get($rootScope.serverUrl + 'ticket/raffleApi/getGeneratedRaffles').then(function (response) {
                if (response !== null) {
                    $scope.raffles = response.data.object;
                }
                window.setTimeout(function () {
                    $scope.$apply();
                    $rootScope.createSelect2();
                }, 0);
            });
        }

        $scope.updateRaffles = function (raffleId) {
            window.loading.show();
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: $rootScope.serverUrl + '/TicketReturned/GetReturnedAwardList?raffleId=' + raffleId,
                success: function (data) {
                    window.loading.hide();
                    if (raffleId == 0) {
                        $scope.raffles = data.raffles;
                    }
                    if (data.returneds) {
                        $scope.returneds = data.returneds;
                    }
                    $rootScope.destroyDataTable();
                    $scope.$apply();
                    $rootScope.dataTable();
                }
            });
        }
        $scope.updateDelivered = function () {
            $scope.updateRaffles($scope.RaffleId);
        }
        this.getData();

        $scope.awardCertification = function (returned) {
            // confirm dialog
            alertify.confirm("&iquest;Desea certificar este premio?", function (e) {
                if (e) {
                    $.ajax({
                        type: 'POST',
                        dataType: 'json',
                        url: '/TicketAllocation/CertificationAwardData',
                        data: {
                            //iNumberId: returned.NumberId, 
                            number: returned.Number,
                            //fractionFrom: 0,
                            //fractionTo: 0,
                            raffleAwardId: returned.RaffleAwardId,
                            fractions: returned.Fractions
                        },
                        success: function (data) {
                            if (data.result === true) {
                                $scope.updateRaffles($scope.RaffleId);
                                alertify.success(data.message);
                                window.open('/Reports/NumberCertification?CertificationNumberId=' + data.certificateObject.Id);
                            }
                            else {
                                alertify.alert(data.message);
                            }
                        }
                    });
                }
            });
        }
    }
})();
