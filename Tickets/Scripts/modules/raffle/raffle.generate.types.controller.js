/**=========================================================
 * Module: RaffleGeneratedtypesController.js
 =========================================================*/

(function() {
    'use strict';

    angular
        .module('naut')
        .controller('RaffleGeneratedtypesController', RaffleGeneratedtypesController);

    RaffleGeneratedtypesController.$inject = ['$scope', '$rootScope', '$state'];
    function RaffleGeneratedtypesController($scope, $rootScope, $state) {
        this.getSolteoList = function (statu) {
            $rootScope.systemLoading = true;
            var url = $rootScope.serverUrl + 'ticket/raffleApi/getRaffleList?statu=' + statu
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

        this.getSolteoList(70);
        $scope.downloadRaffleReport = function (raffleId) {
            window.loading.show();
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: 'Reports/PrintImage?raffleId='+raffleId,
                success: function (data) {
                    window.loading.hide();
                    if (data.result === true) {
                        window.location.href = '/generalRaffle/' + data.fileName;
                    } else {
                        alertify.alert(data.message);
                    }
                    $scope.$apply();
                }
            });
        }
    }
})();
