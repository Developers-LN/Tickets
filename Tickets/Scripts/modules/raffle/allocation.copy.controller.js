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
        $scope.RaffleId = undefined;
        this.getRaffleList = function () {
            window.loading.show();
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: 'Raffle/GetList',
                success: function (data) {
                    window.loading.hide();
                    $scope.raffles = data.solteos.filter(function (raffle) {
                        if (raffle.Id != $stateParams.raffleId) {
                            return raffle;
                        }
                    });
                    $scope.$apply();
                    $rootScope.createSelect2();
                }
            });
        }
        $scope.copyAllocation = function () {
            if ($scope.RaffleId === undefined) {
                alertify.alert("seleccione un sorteo");
                return;
            }
            window.loading.show();
            $.ajax({
                type: 'POST',
                dataType: 'json',
                url: 'Raffle/CopyAllocation',
                data: { sourceId: $scope.RaffleId, targetId: $stateParams.raffleId },
                success: function (response) {
                    window.loading.hide();
                    if (response.result == true) {
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
