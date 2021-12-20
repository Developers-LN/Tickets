/**=========================================================
 * Module: ElectronicSalesListController.js
 =========================================================*/

(function () {
    'use strict';

    angular
        .module('naut')
        .controller('ElectronicSalesListController', ElectronicSalesListController);

    ElectronicSalesListController.$inject = ['$scope', '$state', '$rootScope'];
    function ElectronicSalesListController($scope, $state, $rootScope) {
        $scope.raffleId = 0;
        $scope.groupClientId = 0;

        $rootScope.returnURL = '/#/ticket/electronicSales';

        $scope.updateReturned = function () {
            window.loading.show();
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: $rootScope.serverUrl + 'ticket/electronicSalesApi/getElectronicSalesList?raffleId=' + $scope.raffleId + '&clientId=' + $scope.groupClientId,
                success: function (response) {
                    window.loading.hide();
                    $scope.returneds = response.object;
                    $rootScope.destroyDataTable();
                    $scope.$apply();
                    $rootScope.dataTable();
                }
            });
        }

        $scope.validateElectronicSale = function (returned) {
            // confirm dialog
            alertify.confirm("&iquest;Desea validar esta venta electronica?", function (e) {
                if (e) {
                    window.loading.show();
                    $.ajax({
                        type: 'POST',
                        dataType: 'json',
                        url: 'ElectronicSales/ElectronicSalesValidate',
                        data: {
                            AllocationId: JSON.stringify(returned.allocationId)
                        },
                        success: function (data) {
                            if (data.result === true) {
                                window.loading.hide();
                                alertify.success(data.message);
                                //window.open('/Reports/ElectronicSaleDetails?allocationId=' + AllocationId);
                                $scope.updateReturned();
                            } else {
                                alertify.alert(data.message);
                            }
                        }
                    });
                }
            });
        }

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

        this.loadData();
    }
})();
