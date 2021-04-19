/**=========================================================
 * Module: TicketReprintController.js
 =========================================================*/

(function() {
    'use strict';

    angular
        .module('naut')
        .controller('TicketReprintController', TicketReprintController);

    TicketReprintController.$inject = ['$scope', '$state', '$rootScope'];
    function TicketReprintController($scope, $state, $rootScope) {
        var self = this;

        this.loadData = function () {
            window.loading.show();
            $.when(
                $.ajax($rootScope.serverUrl + 'ticket/clientApi/getClientSelect?statu=2089'),/*Clientes aprobados*/
                $.ajax($rootScope.serverUrl + 'ticket/raffleApi/getRaffleSelect?statu=68'),//Sorteos en planificacion
                $.ajax($rootScope.serverUrl + 'ticket/catalogApi/getReprintDesing')
            ).then(function (clientResponse, raffleResponse, ticketDesingResponse) {
                window.loading.hide();
                if (clientResponse[1] == 'success') {
                    $scope.clients = clientResponse[0].object;
                }
                if (raffleResponse[1] == 'success') {
                    $scope.raffles = raffleResponse[0].object;
                }
                if (ticketDesingResponse[1] == 'success') {
                    $scope.ticketDesing = ticketDesingResponse[0].object;
                }
                window.setTimeout(function () {
                    $scope.$apply();
                    $rootScope.createSelect2();
                }, 0);
            });
        }

        this.getList = function () {
            window.loading.show();
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: $rootScope.serverUrl + 'ticket/ticketPrintApi/getReprintAppruved?raffleId=' + $scope.RaffleId,
                success: function (response) {
                    if (response.result === false) {
                        alertify.alert(response.message);
                        $state.go('app.dashboard');
                    } else {
                        $scope.reprints = response.object.map(function (ticket) {
                            ticket.CreateDate = new Date(ticket.CreateDate);
                            return ticket;
                        });
                        $rootScope.destroyDataTable();
                        $scope.$apply();
                        $rootScope.dataTable();
                    }
                    window.loading.hide();
                }
            });
        }

        $scope.updateAllocation = function () {
            self.getList( );
        }

        $scope.reprintTicket = function (reprint) {
            window.open($rootScope.serverUrl + $scope.ticketDesing + reprint.id)
            self.getList();
        }

        $scope.RaffleId = 0;

        self.loadData();
    }
})();
