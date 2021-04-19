/**=========================================================
 * Module: RafflePrintController.js
 =========================================================*/

(function() {
    'use strict';

    angular
        .module('naut')
        .controller('RafflePrintController', RafflePrintController);

    RafflePrintController.$inject = ['$scope', '$rootScope', '$state'];
    function RafflePrintController($scope, $rootScope, $state) {
        this.getSolteoList = function (statu) {
            $rootScope.systemLoading = true;
            var url = $rootScope.serverUrl + 'ticket/raffleApi/getRaffleList?statu=0'
            
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: url,
                success: function (response) {
                    $rootScope.systemLoading = false;
                    $scope.raffles = response.object;
                    $scope.$apply();
                    $rootScope.dataTable();
                }
            });
        }

        this.getSolteoList();
        
    }
})();
