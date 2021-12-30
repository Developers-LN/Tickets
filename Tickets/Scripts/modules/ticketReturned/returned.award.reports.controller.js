/**=========================================================
 * Module: ReturnedAwardReportsController.js
 =========================================================*/

(function () {
    'use strict';

    angular
        .module('naut')
        .controller('ReturnedAwardReportsController', ReturnedAwardReportsController);

    ReturnedAwardReportsController.$inject = ['$scope', '$state', '$rootScope', '$stateParams'];
    function ReturnedAwardReportsController($scope, $state, $rootScope, $stateParams) {
        $scope.returned = {
            RaffleId: undefined,
            GroupId: ''
        };

        $scope.loadRaffles = function () {
            $rootScope.systemLoading = true;
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: 'TicketReturned/ReturnedAwardReportData',
                success: function (data) {
                    $rootScope.systemLoading = false;
                    $scope.raffles = data.raffles;
                    $scope.groups = data.groups;
                    $scope.$apply();
                }
            });
        };

        $scope.getGroup = function () {
            window.loading.show();
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: 'TicketReturned/ReturnedAwardReportData?raffleId=' + $scope.returned.RaffleId,
                success: function (data) {
                    window.loading.hide();
                    $scope.groups = data.groups;
                    $scope.$apply();
                }
            });
        }
        $scope.loadRaffles();

        $rootScope.createSelect2();

        $scope.printReport = function () {
            window.open('Reports/ReturnedGroupAwards?raffleId=' + $scope.returned.RaffleId + '&groupId=' + $scope.returned.GroupId);
        };
    }
})();
