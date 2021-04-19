/**=========================================================
 * Module: AllocationReviewController.js
 =========================================================*/

(function() {
    'use strict';

    angular
        .module('naut')
        .controller('AllocationReviewController', AllocationReviewController);

    AllocationReviewController.$inject = ['$scope', '$state', '$rootScope'];
    function AllocationReviewController($scope, $state, $rootScope) {
        var self = this;
        $scope.markAsReview = function (allocation) {
            // confirm dialog
            alertify.confirm("&iquest;Desea marcar como revisada esta asignacion?", function (e) {
                if (e) {
                    window.loading.show();
                    $.ajax({
                        type: 'POST',
                        dataType: 'json',
                        url: $rootScope.serverUrl + 'ticket/ticketPrintApi/ticketAllocationReview',
                        data: allocation,
                        success: function (data) {
                            window.loading.hide();
                            if (data.result == true) {
                                alertify.success(data.message);
                                $scope.updateAllocation();
                            }
                        }
                    });
                }
            }); 
        }
        
        $scope.updateAllocation = function () {
            if ($scope.RaffleId == 0) {
                return;
            }

            window.loading.show();
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: $rootScope.serverUrl + 'ticket/ticketAllocationApi/getPrintedAllocations?raffleId=' + $scope.RaffleId + '&clientId=' + $scope.ClientId,
                success: function (response) {
                    window.loading.hide();
                    if (response.result == true) {
                        $scope.allocations = response.object;
                        $rootScope.destroyDataTable();
                        $scope.$apply();
                        $rootScope.dataTable();
                    } else {
                        alertify.alert(response.message);
                    }
                }
            });
        }

        this.loadData = function () {
            window.loading.show();
            $.when(
                $.ajax($rootScope.serverUrl + 'ticket/clientApi/getClientSelect?statu=2089'),/*Clientes aprobados*/
                $.ajax($rootScope.serverUrl + 'ticket/raffleApi/getRaffleSelect?statu=68')//Sorteos en planificacion
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

        $scope.RaffleId = 0;
        $scope.ClientId = 0;
        self.loadData();
    }
})();
