/**=========================================================
 * Module: ClientListController.js
 =========================================================*/

(function() {
    'use strict';

    angular
        .module('naut')
        .controller('ClientListController', ClientListController);

    ClientListController.$inject = ['$scope', '$rootScope', '$state'];
    function ClientListController($scope, $rootScope, $state) {
        var self = this;
        this.getClientList = function () {
            window.loading.show();
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: 'Client/GetList',
                success: function (data) {
                    window.loading.hide();
                    $scope.clientList = data.clients;
                    
                    $rootScope.destroyDataTable();
                    $scope.$apply();
                    $rootScope.dataTable();
                }
            });
        }

        $scope.deleteClient = function (client) {
            // confirm dialog
            alertify.confirm("&iquest;Desea borrar este cliente?", function (e) {
                if (e) {
                    $.ajax({
                        type: 'POST',
                        dataType: 'json',
                        url: 'Client/Delete',
                        data: { clientId: client.Id },
                        success: function (data) {
                            if (data === true) {
                                self.getClientList();
                                alertify.success('Cliente borrado correctamente!');
                            }
                        }
                    });
                }
            });
        }

        $scope.startWorkflowProspect = function (client) {
            // confirm dialog
            alertify.confirm("&iquest;Desea enviar este cliente a un proceso de aprobaci&#243;n?", function (e) {
                if (e) {
                    $.ajax({
                        type: 'POST',
                        dataType: 'json',
                        url: 'Client/SendClientProccess',
                        data: { clientId: client.Id },
                        success: function (data) {
                            if (data.result === true) {
                                self.getClientList();
                                alertify.success('El cliente fue enviado al flujo de aprobaci&#243;n correctamente!');
                            } else {
                                alertify.alert(data.message);
                            }
                        }
                    });
                }
            });
        }
        this.getClientList();
    }
})();
