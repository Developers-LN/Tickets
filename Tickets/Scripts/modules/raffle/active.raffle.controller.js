/**=========================================================
 * Module: ActiveRaffleController.js
 =========================================================*/

(function() {
    'use strict';

    angular
        .module('naut')
        .controller('ActiveRaffleController', ActiveRaffleController);

    ActiveRaffleController.$inject = ['$scope', '$rootScope', '$state'];
    function ActiveRaffleController($scope, $rootScope, $state) {
        var self = this;
        this.loadActiveRaffle = function () {
            window.loading.show();
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: $rootScope.serverUrl + 'ticket/raffleApi/getActive',
                success: function (response) {
                    window.loading.hide();
                    if (response.result === false) {
                        alertify.alert(response.message);
                        $state.go('app.dashboard');
                    }
                    else
                    {
                        $scope.raffle = response.object;
                    }
                    $scope.$apply();
                }
            });
        
        }
        this.loadActiveRaffle();
    }
})();
