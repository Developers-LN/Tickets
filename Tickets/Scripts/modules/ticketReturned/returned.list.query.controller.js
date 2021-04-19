/**=========================================================
 * Module:returned.list.query.controller.js
 =========================================================*/

(function() {
    'use strict';

    angular
        .module('naut')
        .controller('ReturnedListQueryController', ReturnedListQueryController);

    ReturnedListQueryController.$inject = ['$scope', '$state', '$rootScope'];
    function ReturnedListQueryController($scope, $state, $rootScope) {
        $scope.raffleId = 0;
        $scope.clientId = 0;
        $rootScope.returnURL = '/#/ticket/returnedListQuery';

        this.loadData = function () {
            window.loading.show();
            $.when(
                $.ajax($rootScope.serverUrl + 'ticket/clientApi/getClientSelect?statu=2089'),/*Clientes aprobados*/
                $.ajax($rootScope.serverUrl + 'ticket/raffleApi/getRaffleForReturnedSelect')//Sorteos en planificacion
            ).then(function (clientResponse, raffleResponse) {
                window.loading.hide();
                if (clientResponse[1] == 'success') {
                    $scope.clients = clientResponse[0].object;
                }
                if (raffleResponse[1] == 'success') {
                    $scope.raffles = raffleResponse[0].object;
                }
                window.setTimeout(function () {
                    $scope.$apply();
                    $rootScope.createSelect2();
                }, 0);
            });
        }

        $scope.updateReturned = function () {
            window.loading.show();
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: $rootScope.serverUrl + 'ticket/ticketReturnedApi/geListtByClient?raffleId=' + $scope.raffleId + '&clientId=' + $scope.clientId,
                success: function (response) {
                    window.loading.hide();
                    $scope.returneds = response.object.map(function (returned) {
                        returned.Group = returned.returnedSubGroups.split(', ');
                        return returned;
                    });

                    $rootScope.destroyDataTable();
                    $scope.$apply();
                    $rootScope.dataTable();
                }
            });

        }
        
        this.loadData();
    }
})();
