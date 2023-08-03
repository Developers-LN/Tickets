/**=========================================================
 * Module: AllocationDetailsController.js
 =========================================================*/

(function() {
    'use strict';

    angular
        .module('naut')
        .controller('AllocationDetailsController', AllocationDetailsController);

    AllocationDetailsController.$inject = ['$scope', '$state', '$rootScope', '$stateParams'];
    function AllocationDetailsController($scope, $state, $rootScope, $stateParams) {
        var self = this;
        $scope.totals = {
            number: 0,
            fraction: 0,
            total: 0
        };

        $rootScope.returnUrl = "/#/ticket/allocationDetails/" + $stateParams.allocationId;

        $scope.globalReturnURL = $rootScope.globalReturnURL;

        this.loadAllocation = function (allocationId) {
            window.loading.show();
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: $rootScope.serverUrl + 'ticket/ticketAllocationApi/getTicketAllocation?id=' + $stateParams.allocationId,
                success: function (response) {
                    window.loading.hide();
                    if(response.result == true){
                        $scope.allocation = response.object;
                    }
                    self.calculateTotal();
                    $rootScope.destroyDataTable();
                    $scope.$apply();
                    $rootScope.dataTable();
                }
            });
        }

        $scope.deleteNumber = function (number) {
            // confirm dialog
            alertify.confirm("&iquest;Desea borrar esta billetes?", function (e) {
                if (e) {
                    window.loading.show();
                    $.ajax({
                        type: 'POST',
                        dataType: 'json',
                        url: $rootScope.serverUrl + 'ticket/ticketAllocationApi/deleteAllocationNumber',
                        data: number,
                        success: function (response) {
                            window.loading.hide();
                            if (response.result == false) {
                                alertify.alert(response.message);
                            } else {
                                self.loadAllocation();
                                alertify.alert(response.message);
                            }
                        }
                    });
                }
            });
        }

        $scope.unConsignateNumber = function (number) {
            // confirm dialog
            alertify.confirm("&iquest;Desea borrar esta billetes?", function (e) {
                if (e) {
                    window.loading.show();
                    $.ajax({
                        type: 'POST',
                        dataType: 'json',
                        url: $rootScope.serverUrl + 'ticket/ticketAllocationApi/unConsignateNumber',
                        data: number,
                        success: function (response) {
                            window.loading.hide();
                            if (response.result == false) {
                                alertify.alert(response.message);
                            } else {
                                self.loadAllocation();
                                alertify.alert(response.message);
                            }
                        }
                    });
                }
            });
        }

        $scope.consignarAsignacion = function (id) {
            // confirm dialog
            alertify.confirm("&iquest;Desea realizar la consignacion de esta asignacion?", function (e) {
                if (e) {
                    window.loading.show();
                    $.ajax({
                        type: 'POST',
                        dataType: 'json',
                        url: $rootScope.serverUrl + 'ticket/ticketAllocationApi/consingAllocation?id=' + id,
                        success: function (response) {
                            window.loading.hide();
                            if (response.result == false) {
                                alertify.alert(response.message);
                            } else {
                                self.loadAllocation();
                                alertify.alert(response.message);
                            }
                        }
                    });
                }
            });
        }

        $scope.anularAsignacion = function (id) {
            // confirm dialog
            alertify.confirm("&iquest;Desea realizar el anulado de esta asignacion?", function (e) {
                if (e) {
                    window.loading.show();
                    $.ajax({
                        type: 'POST',
                        dataType: 'json',
                        url: $rootScope.serverUrl + 'ticket/ticketAllocationApi/removeAllocation?id=' + id,
                        success: function (response) {
                            window.loading.hide();
                            if (response.result == false) {
                                alertify.alert(response.message);
                            } else {
                                self.loadAllocation();
                                alertify.alert(response.message);
                            }
                        }
                    });
                }
            });
        }

        $scope.deliverAllocation = function (id) {
            // confirm dialog
            alertify.confirm("&iquest;Desea realizar la entrega de esta asignacion?", function (e) {
                if (e) {
                    window.loading.show();
                    $.ajax({
                        type: 'POST',
                        dataType: 'json',
                        url: $rootScope.serverUrl + 'ticket/ticketAllocationApi/deliverAllocation?id=' + id,
                        success: function (response) {
                            window.loading.hide();
                            if (response.result == false) {
                                alertify.alert(response.message);
                            } else {
                                self.loadAllocation();
                                alertify.alert(response.message);
                            }
                        }
                    });
                }
            });
        }

        $scope.showSubTotal = function (number) {
            return (number.fractionTo - number.fractionFrom + 1) * $scope.allocation.fractionPrice;
        }

        this.calculateTotal = function () {
            $scope.totals.number = $scope.allocation.ticketAllocationNumbers.length;
            $scope.totals.fraction = 0;
            $scope.allocation.ticketAllocationNumbers.forEach(function(n){
                $scope.totals.fraction += (n.fractionTo - n.fractionFrom + 1);
            });
            $scope.totals.total = $scope.totals.fraction * $scope.allocation.fractionPrice;
        }

        this.loadAllocation();
    }
})();
