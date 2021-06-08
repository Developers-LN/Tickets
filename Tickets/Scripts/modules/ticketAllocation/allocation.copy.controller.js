/**=========================================================
 * Module: AllocationCopyController.js
 =========================================================*/

(function () {
    'use strict';

    angular
        .module('naut')
        .controller('AllocationCopyController', AllocationCopyController);

    AllocationCopyController.$inject = ['$scope', '$rootScope', '$state', '$stateParams'];
    function AllocationCopyController($scope, $rootScope, $state, $stateParams) {
        /*jshint validthis:true*/
        var self = this;
        $scope.copyAllocation = {
            sourceTicketRaffleId: 0,
            sourcePoolRaffleId: 0,
            targetRaffleId: 0
        };
        this.getRaffleList = function () {
            window.loading.show();
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: $rootScope.serverUrl + 'ticket/raffleApi/GetRaffleList?statu=' + 0,
                success: function (response) {
                    window.loading.hide();
                    $scope.raffles = response.object.filter(function (raffle) {
                        if (raffle.id != $stateParams.raffleId) {
                            return raffle;
                        }
                    });
                    $scope.$apply();
                    $rootScope.createSelect2();
                }
            });
        }

        $scope.copyAllocation = function () {
            if ($scope.copyAllocation.sourcePoolRaffleId == 0 && $scope.copyAllocation.sourceTicketRaffleId == 0) {
                alertify.alert("seleccione un sorteo para billetes o de Quinielas.");
                return;
            }
            $scope.copyAllocation.targetRaffleId = $stateParams.raffleId;
            window.loading.show();
            $.ajax({
                type: 'POST',
                dataType: 'json',
                url: $rootScope.serverUrl + 'ticket/ticketAllocationApi/copyAllocations',
                data: $scope.copyAllocation,
                success: function (response) {
                    window.loading.hide();
                    if (response.result === true) {
                        alertify.success(response.message);
                        $state.go('app.solteoList');
                    } else {
                        alertify.success(response.message);
                    }
                }
            });
        }

        self.getRaffleList();
    }
})();
