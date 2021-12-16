/**=========================================================
 * Module: IdentifyAwardListToPayController.js
 =========================================================*/

(function () {
    'use strict';

    angular
        .module('naut')
        .controller('IdentifyAwardListToPayController', IdentifyAwardListController);

    IdentifyAwardListController.$inject = ['$scope', '$state', '$rootScope'];

    function IdentifyAwardListController($scope, $state, $rootScope) {
        var self = this;

        self.loadIdentifys = function (raffleId, clientId) {
            window.loading.show();
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: 'TicketAllocation/GetIdentifyListToPay?raffleId=' + raffleId + '&clientId=' + clientId,
                success: function (data) {
                    if (raffleId > 0 || clientId > 0) {
                        $scope.identifyBachs = data.identifyBachs;
                    }
                    if (raffleId <= 0 && clientId <= 0) {
                        $scope.raffles = data.raffles;
                        $scope.clients = data.clients;
                    }
                    $rootScope.destroyDataTable();
                    $scope.$apply();
                    $rootScope.dataTable();
                    window.loading.hide();
                }
            });
        }
        self.loadIdentifys(0, 0);
        var self = this;
        $scope.RaffleId = 0;
        $scope.ClientId = 0;

        $scope.updateDelivered = function () {
            self.loadIdentifys($scope.RaffleId, $scope.ClientId);
        }
        $scope.updateDelivered1 = function () {
            self.loadIdentifys1();
        }

        $rootScope.createSelect2();

        $scope.deleteIdentifyBach = function (identifyBach) {
            // confirm dialog
            alertify.confirm("&iquest;Desea borrar este Numero?", function (e) {
                if (e) {
                    window.loading.show();
                    $.ajax({
                        type: 'POST',
                        dataType: 'json',
                        url: 'TicketAllocation/IdentifyBachDelete',
                        data: identifyBach,
                        success: function (data) {
                            window.loading.hide();
                            if (data.result === true) {
                                alertify.success(data.message);
                                self.loadIdentifys($scope.RaffleId, $scope.ClientId);
                            } else {
                                console.log(data.message);
                            }
                        }
                    });
                }
            });
        }
        self.GetBatchNumbers = function () {

            $.ajax({
                type: 'get',
                dataType: 'json',
                url: 'TicketAllocation/GetBatchList',

                success: function (data) {
                    $scope.batchNumbers = data;
                }
            });
        }
        self.GetBatchNumbers();

        self.loadIdentifys1 = function () {
            window.loading.show();
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: 'TicketAllocation/GetIdentifyListByBatch?batchNumber=' + $scope.batchId,
                success: function (data) {
                    $scope.identifyBachs = data;
                    $rootScope.destroyDataTable();
                    $scope.$apply();
                    $rootScope.dataTable();
                    window.loading.hide();
                }
            });
        }

        self.callJQuery = function () {
            $('#sorteo_lotes').hide();
            $(document).ready(function () {
                $('#SByBatch').click(function () {

                    $('#sorteo_lotes').fadeToggle();
                    $('#sorteo_filters').fadeToggle();
                    var value = $('#SByBatch').text();
                    if (value.includes('lotes')) {
                        $('#SByBatch').text('Busqueda estandar');
                    } else {
                        $('#SByBatch').text('Busqueda por lotes');
                    }
                });
            });
        }
        self.callJQuery();
    }
})();
