/**=========================================================
 * Module: ReprintListController.js
 =========================================================*/

(function () {
    'use strict';

    angular
        .module('naut')
        .controller('ReprintListController', ReprintListController);

    ReprintListController.$inject = ['$scope', '$state', '$rootScope'];
    function ReprintListController($scope, $state, $rootScope) {
        var self = this;

        this.getRaffleList = function () {
            window.loading.show();
            $.when(
                $.ajax($rootScope.serverUrl + 'ticket/raffleApi/getRaffleSelect?statu=68')//Sorteos en planificacion
            ).then(function (raffleResponse) {
                window.loading.hide();
                if (raffleResponse.result == true) {
                    $scope.raffles = raffleResponse.object;
                }
                $rootScope.destroyDataTable();
                $scope.$apply();
                $rootScope.dataTable();
                window.setTimeout(function () {
                    $scope.$apply();
                    $rootScope.createSelect2();
                }, 0);
            });
        };

        this.getReprintList = function () {
            window.loading.show();
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: $rootScope.serverUrl + 'ticket/ticketPrintApi/getReprintList?raffleId=' + $scope.RaffleId,
                success: function (response) {
                    if (response.result == true) {
                        $scope.ticketReprints = response.object;
                    }

                    $rootScope.destroyDataTable();
                    $scope.$apply();
                    $rootScope.dataTable();
                    window.loading.hide();
                }
            });
        }

        $scope.updateReprint = function () {
            self.getReprintList();
        }

        $scope.startWorkflowProspect = function (reprint) {
            // confirm dialog
            alertify.confirm("&iquest;Desea enviar esta reimpresion a un proceso de aprobaci&#243;n?", function (e) {
                if (e) {
                    window.loading.show();
                    $.ajax({
                        type: 'POST',
                        dataType: 'json',
                        url: $rootScope.serverUrl + 'ticket/workflowApi/sendReprintToWorkflow',
                        data: reprint,
                        success: function (response) {
                            window.loading.hide();
                            if (response.result === true) {
                                alertify.success(response.message);
                                self.getReprintList();
                            } else {
                                alertify.alert(response.message);
                            }
                        }
                    });
                }
            });
        }

        $scope.deleteTicket = function (reprint) {
            // confirm dialog
            alertify.confirm("&iquest;Desea borrar esta reimpresi&oacute;n?", function (e) {
                if (e) {
                    window.loading.show();
                    $.ajax({
                        type: 'POST',
                        dataType: 'json',
                        url: $rootScope.serverUrl + 'ticket/ticketPrintApi/delete',
                        data: reprint,
                        success: function (response) {
                            window.loading.hide();
                            if (response.result === true) {
                                self.getReprintList();
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
        this.getRaffleList();
    }
})();
