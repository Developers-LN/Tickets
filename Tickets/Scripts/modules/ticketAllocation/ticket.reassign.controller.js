/**=========================================================
 * Module: TicketReassignController.js
 =========================================================*/

(function() {
    'use strict';

    angular
        .module('naut')
        .controller('TicketReassignController', TicketReassignController);

    TicketReassignController.$inject = ['$scope', '$state', '$rootScope', '$stateParams'];
    function TicketReassignController($scope, $state, $rootScope, $stateParams) {
        var self = this;
        $scope.reassign = {
            clientId: undefined,
            allocationId: $stateParams.idAssign
        };

        $scope.reassignTicket = function () {
            if ($scope.reassign.clientId === undefined) {
                alertify.alert("Tiene que seleccionar el Client");
                return;
            }
            window.loading.show();
            $.ajax({
                type: 'POST',
                dataType: 'json',
                url: $rootScope.serverUrl + 'ticket/ticketAllocationApi/ticketAllocationReassing',
                data: $scope.reassign,
                success: function (response) {
                    window.loading.hide();
                    if (response.result === true) {
                        alertify.success(response.message);
                        window.location.href = "/#/ticket/allocations";
                    } else {
                        alertify.alert(response.message);
                    }
                }
            });
        }

        this.loadClient = function () {
            window.loading.show();
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: $rootScope.serverUrl + 'ticket/clientApi/getClientSelect?statu=2089',
                success: function (clientResponse) {
                    window.loading.hide();
                    if (clientResponse.result === false) {
                        alertify.alert(clientResponse.message);
                    } else {
                        $scope.clients = clientResponse.object;
                        $scope.$apply();
                        $rootScope.createSelect2();
                    }

                }
            });
        }

        this.loadClient();
    }
})();
