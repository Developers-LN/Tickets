/**=========================================================
 * Module: ClientPrintController.js
 =========================================================*/

(function () {
    'use strict';

    angular
        .module('naut')
        .controller('ClientPrintController', ClientPrintController);

    ClientPrintController.$inject = ['$scope', '$rootScope', '$state'];
    function ClientPrintController($scope, $rootScope, $state) {
        this.getClientList = function () {
            window.loading.show();
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: 'Client/GetList',
                success: function (data) {
                    window.loading.hide();
                    $scope.clientList = data.clients;
                    $scope.raffles = data.raffles;
                    $scope.$apply();
                    $rootScope.dataTable();
                }

            });
        }

        //NUEVO CODIGO PARA GENERAR XML DE LAS ASIGNACIONES
        var url = 'http://' + location.host + '/';
        $scope.xmlAwardDownload = function (ClientId, RaffleId) {
            if (RaffleId == undefined || RaffleId == null) {
                alertify.alert('Seleccione un sorteo');
                return;
            }
            else {
                alertify.confirm("&iquest;Desea descargar el archivo XML de los numeros ganadores?", function (e) {
                    if (e) {
                        window.loading.show();
                        $.ajax({
                            type: 'GET',
                            contentType: 'application/json; charset=utf-8',
                            url: 'Integration/RaffleAwards',
                            data: { ClientId: ClientId, RaffleId: RaffleId },
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
        }

        $scope.xlsxAwardDownload = function (ClientId, RaffleId) {
            if (RaffleId == undefined || RaffleId == null) {
                alertify.alert('Seleccione un sorteo');
                return;
            }
            else {
                alertify.confirm("&iquest;Desea descargar el archivo Excel de los numeros ganadores?", function (e) {
                    if (e) {
                        window.loading.show();
                        $.ajax({
                            type: 'GET',
                            contentType: 'application/json; charset=utf-8',
                            url: 'Integration/PayableAwardsByClientToExcel',
                            data: { ClientId: ClientId, RaffleId: RaffleId },
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
        }

        $rootScope.createSelect2();
        this.getClientList();
    }
})();
