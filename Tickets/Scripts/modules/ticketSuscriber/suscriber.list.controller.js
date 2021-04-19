/**=========================================================
 * Module: SuscriberListController.js
 =========================================================*/

(function() {
    'use strict';

    angular
        .module('naut')
        .controller('SuscriberListController', SuscriberListController);

    SuscriberListController.$inject = ['$scope', '$state', '$rootScope'];
    function SuscriberListController($scope, $state, $rootScope) {
        var self = this;

        $scope.deleteTicket = function (ticket) {
            // confirm dialog
            alertify.confirm("&iquest;Desea borrar este numero abonado?", function (e) {
                if (e) {
                    window.loading.show();
                    $.ajax({
                        type: 'POST',
                        dataType: 'json',
                        url: $rootScope.serverUrl + 'ticket/ticketSuscriberApi/delete',
                        data: ticket,
                        success: function (response) {
                            window.loading.hide();
                            if (response.result === true) {
                                $rootScope.destroyDataTable();
                                self.getList($scope.ClientId);
                                alertify.success(response.message);
                            } else {
                                alertify.alert(esponse.message);
                            }

                        }
                    });
                }
            });
        }

        this.getList = function (clientId) {
            window.loading.show();
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: $rootScope.serverUrl + 'ticket/ticketSuscriberApi/getList?clientId=' + clientId,
                success: function (response) {
                    window.loading.hide();
                    if (response.result === false) {
                        alertify.alert(response.message);
                    } else {
                        $scope.ticketList = response.object.map(function (ticket) {
                            ticket.CreateDate = new Date(ticket.CreateDate);
                            return ticket;
                        });
                        $rootScope.destroyDataTable();
                        $scope.$apply();
                        $rootScope.dataTable();
                    }
                }
            });
        }

        this.loadData = function () {
            window.loading.show();
            $.when(
                $.ajax($rootScope.serverUrl + 'ticket/clientApi/getClientSelect?statu=2089')
            ).then(function (clientResponse) {
                window.loading.hide();
                if (clientResponse.result == true) {
                    $scope.clients = clientResponse.object;
                }
                window.setTimeout(function () {
                    $scope.$apply();
                    $rootScope.createSelect2();
                }, 0);
            });
        }

        this.loadData();

        $scope.updateSuscriber = function () {
            self.getList($scope.ClientId);
        }
    }
})();
