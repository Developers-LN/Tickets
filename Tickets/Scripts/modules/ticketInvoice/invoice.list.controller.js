/**=========================================================
 * Module: InvoiceListController.js
 =========================================================*/

(function() {
    'use strict';

    angular
        .module('naut')
        .controller('InvoiceListController', InvoiceListController);

    InvoiceListController.$inject = ['$scope', '$state', '$rootScope'];
    function InvoiceListController($scope, $state, $rootScope) {
        var self = this;
        $scope.ClientId = 0;
        $scope.RaffleId = 0;
        $scope.updateInvoices = function () {
            if ($scope.RaffleId == 0) {
                return;
            }
            window.loading.show();
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: $rootScope.serverUrl + 'ticket/ticketInvoiceApi/getInvoiceList?raffleId=' + $scope.RaffleId + '&clientId=' + $scope.ClientId,
                success: function (response) {
                    window.loading.hide();
                    if (response.result == true) {
                        $scope.invoices = response.object;
                    } else {
                        alertify.alert(response.message);
                    }
                    $rootScope.destroyDataTable();
                    $scope.$apply();
                    $rootScope.dataTable();
                }
            });
        }

        this.showAllocations = function (ticketAllocations) {
            var string = '';
            ticketAllocations.forEach(function (allocation, i) {
                string += allocation.sequenceNumberTicketAllocation;
                if ((i + 1) < ticketAllocations.length) {
                    string += ', '
                }
            });
            return string;
        }

        this.loadData = function () {
            window.loading.show();
            $.when(
                $.ajax($rootScope.serverUrl + 'ticket/clientApi/getClientSelect?statu=2089'),/*Clientes aprobados*/
                $.ajax($rootScope.serverUrl + 'ticket/raffleApi/getRaffleSelect?statu=0')//Sorteos en planificacion
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

        $scope.showInvoiceDetails = function (invoice) {
            window.loading.show();
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: $rootScope.serverUrl + 'ticket/ticketInvoiceApi/getInvoice?id=' + invoice.id,
                success: function (response) {
                    window.loading.hide();
                    if (response.result == true) {
                        response.object.ticketAllocations = self.showAllocations(response.object.ticketAllocations);
                        $scope.invoices = $scope.invoices.map(function (i) {
                            if (i.id == response.object.id) {
                                return response.object;
                            }
                            return i;
                        });
                    } else {
                        alertify.alert(response.message);
                    }
                    $rootScope.destroyDataTable();
                    $scope.$apply();
                    $rootScope.dataTable();
                }
            });
        }

        this.loadData();
    }
})();
