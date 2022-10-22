/**=========================================================
 * Module: ElectronicSalesDetailsController.js
 =========================================================*/

(function () {
    'use strict';

    angular
        .module('naut')
        .controller('ElectronicSalesDetailsController', ElectronicSalesDetailsController);

    ElectronicSalesDetailsController.$inject = ['$scope', '$state', '$rootScope', '$stateParams'];
    function ElectronicSalesDetailsController($scope, $state, $rootScope, $stateParams) {
        var self = this;
        $scope.totals = {
            number: 0,
            fraction: 0,
            total: 0
        };

        $rootScope.returnUrl = "/#/ticket/electronicSalesGroupDetails/" + $stateParams.allocationId;

        $scope.globalReturnURL = $rootScope.globalReturnURL;

        this.loadAllocation = function () {
            window.loading.show();
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: 'ElectronicSales/getElectronicSalesDetails?allocationId=' + $stateParams.allocationId,
                success: function (response) {
                    window.loading.hide();

                    $scope.allocationId = response.allocationId;
                    $scope.allocateDate = response.allocateDate;
                    $scope.clientDiscount = response.clientDiscount;
                    $scope.clientType = response.clientType;
                    $scope.clientName = response.clientName;
                    $scope.clientId = response.clientId;
                    $scope.raffleId = response.raffleId;
                    $scope.amount = response.amount;
                    $scope.raffleName = response.raffleName;
                    $scope.electronicSalesByDate = response.electronicSalesByDate;

                    //self.calculateTotal();
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

        $scope.printReport = function (allocationId, date) {
            try {
                date = $rootScope.parseDate(date, allocationId).toJSON();
            }
            catch (e) { }

            window.open('Reports/DataByDay?allocationId=' + allocationId + '&dateSale=' + date);
        }

        /*$scope.showSubTotal = function (number) {
            return (number.fractionTo - number.fractionFrom + 1) * $scope.allocation.fractionPrice;
        }*/

        /*this.calculateTotal = function () {
            $scope.totals.number = $scope.allocation.ticketAllocationNumbers.length;
            $scope.totals.fraction = 0;
            $scope.allocation.ticketAllocationNumbers.forEach(function (n) {
                $scope.totals.fraction += (n.fractionTo - n.fractionFrom + 1);
            });
            $scope.totals.total = $scope.totals.fraction * $scope.allocation.fractionPrice;
        }*/

        this.loadAllocation();
    }
})();
