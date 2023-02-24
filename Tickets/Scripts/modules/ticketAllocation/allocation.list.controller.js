/**=========================================================
 * Module: AllocationListController.js
 =========================================================*/

(function() {
    'use strict';

    angular
        .module('naut')
        .controller('AllocationListController', AllocationListController);

    AllocationListController.$inject = ['$scope', '$state', '$rootScope'];
    function AllocationListController($scope, $state, $rootScope) {
        var self = this;
        $scope.editTicket = function (ticket) {
            $rootScope.ticket = ticket;
            $state.go('app.ticketAllocationCreate');
        }

        $rootScope.globalReturnURL = '/#/ticket/allocations';

        this.loadData = function () {
            window.loading.show();
            $.when(
                $.ajax($rootScope.serverUrl + 'ticket/clientApi/getClientSelect?statu=2089'),/*Clientes aprobados*/
                $.ajax($rootScope.serverUrl + 'ticket/raffleApi/getRaffleSelect?statu=0'),//Sorteos en planificacion
                $.ajax($rootScope.serverUrl + 'ticket/raffleApi/getRaffleConsignation')
            ).then(function (clientResponse, raffleResponse, raffleResponseConsignation) {
                window.loading.hide();
                if (clientResponse[1] == 'success') {
                    $scope.clients = clientResponse[0].object;
                }
                if (raffleResponse[1] == 'success') {
                    $scope.raffles = raffleResponse[0].object;
                }
                if (raffleResponseConsignation[1] == 'success') {
                    $scope.rafflesConsignation = raffleResponseConsignation[0].object;
                }
                window.setTimeout(function () {
                    $scope.$apply();
                    $rootScope.createSelect2();
                }, 0);
            });
        }

        //NUEVO CODIGO PARA GENERAR XML DE LAS ASIGNACIONES
        var url = 'http://' + location.host + '/';
        $scope.xmlAllocationNumberDownload = function (id) {
            alertify.confirm("&iquest;Desea descargar el archivo XML de la Asignacion?", function (e) {
                if (e) {
                    window.loading.show();
                    $.ajax({
                        type: 'GET',
                        contentType: 'application/json; charset=utf-8',
                        url: 'Integration/AllocationNumbers?id=' + id,
                        success: function (data) {
                            window.loading.hide();
                            if (data.result == true) {
                                alertify.success(data.message);
                                var link = document.createElement("a");
                                link.download = name;
                                $(link).attr('download');
                                link.href = url + data.path;
                                link.click();
                            }
                            else {
                                alertify.alert(data.message);
                            }
                            $scope.$apply();
                        }
                    });
                }
            });
        }

        $scope.changeValues = function () {
            if ($scope.RaffleId == 0) {
                return;
            }

            window.loading.show();
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: 'ticket/ticketAllocationApi/getTicketAllocationList?raffleId=' + $scope.RaffleId +'&clientId='+ $scope.ClientId,
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

        $scope.changeValuesConsigned = function () {
            if ($scope.RaffleId == 0) {
                return;
            }

            window.loading.show();
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: 'ticket/ticketAllocationApi/getTicketAllocationConsignateList?raffleId=' + $scope.RaffleId + '&clientId=' + $scope.ClientId,
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

        $scope.sentAllocationToPrint = function (allocation) {
            // confirm dialog
            alertify.confirm("&iquest;Desea enviar esta asignacion a imprecion?", function (e) {
                if (e) {
                    window.loading.show();
                    $.ajax({
                        type: 'POST',
                        dataType: 'json',
                        url: $rootScope.serverUrl + 'ticket/ticketAllocationApi/startAllocationPrint',
                        data: allocation,
                        success: function (response) {
                            window.loading.hide();
                            if (response.result === false) {
                                alertify.alert(response.message);
                            } else {
                                alertify.success(response.message);
                                $scope.changeValues();
                            }
                        }
                    });
                }
            });
        }

        $scope.deleteAllocation = function (allocation) {
            // confirm dialog
            alertify.confirm("&iquest;Desea borrar esta asignacion de billetes?", function (e) {
                if (e) {
                    window.loading.show();
                    $.ajax({
                        type: 'POST',
                        dataType: 'json',
                        url: $rootScope.serverUrl + 'ticket/ticketAllocationApi/deleteAllocation',
                        data: allocation,
                        success: function (response) {
                            window.loading.hide();
                            if (response.result === true) {
                                $rootScope.destroyDataTable();
                                $scope.changeValues();
                                alertify.success(response.message);
                            } else {
                                alertify.success(response.message);
                            }
                        }
                    });
                }
            });
        }

        $scope.RaffleId = 0;
        $scope.ClientId = 0;
        this.loadData();
    }
})();
