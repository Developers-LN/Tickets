/**=========================================================
 * Module: RaffleForInspectorReporController.js
 =========================================================*/

(function () {
    'use strict';

    angular
        .module('naut')
        .controller('RaffleForInspectorReporController', RaffleForInspectorReporController);

    RaffleForInspectorReporController.$inject = ['$scope', '$rootScope', '$state'];
    function RaffleForInspectorReporController($scope, $rootScope, $state) {
        this.getSolteoList = function (statu) {
            $rootScope.systemLoading = true;
            var url = $rootScope.serverUrl + 'ticket/raffleApi/getRaffleList?statu=0'
            window.loading.show();
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: url,
                success: function (response) {
                    window.loading.hide();
                    $rootScope.systemLoading = false;
                    $scope.raffles = response.object;
                    window.setTimeout(function () {
                        $scope.$apply();
                        $rootScope.dataTable();
                    }, 0);
                }
            });
        }

        this.getSolteoList();

    }
})();
