/**=========================================================
 * Module: SystemConfigController.js
 =========================================================*/

(function() {
    'use strict';

    angular
        .module('naut')
        .controller('SystemConfigController', SystemConfigController);

    SystemConfigController.$inject = ['$scope', '$rootScope', '$state'];
    function SystemConfigController($scope, $rootScope, $state) {
        var self = this;
        this.validate = function (config) {
            var error = '', isReq = ' es un campo requerido. <br>';
            if (config.LoteryAdmin === undefined) {
                error += 'Nombre del Administrador' + isReq;
            }
            if (config.MaxReturnTickets === undefined) {
                error += 'Porcentaje de devoluciones' + isReq;
            }
            if (config.RaffleXpiredTime === undefined) {
                error += 'Tiempo de caducidad del sorteo' + isReq;
            }
            if (config.LawDiscountPercentMayor === undefined) {
                error += 'Tiempo de caducidad del sorteo' + isReq;
            }
            if (error !== '') {
                alertify.showError('Alerta', error);
            }
            return error === '';
        };
        $scope.config = {
            id: 0,
            MaxReturnTickets: undefined,
            LoteryAdmin: undefined,
            TicketDesign: undefined,
            RaffleXpiredTime: undefined,
            LawDiscountPercentMayor: undefined
        };
        $scope.saveConfig = function () {
            if (self.validate($scope.config) === true) {
                window.loading.show()
                $.ajax({
                    type: 'POST',
                    dataType: 'json',
                    url: 'Config/SystemConfig',
                    data: $scope.config,
                    success: function (data) {
                        window.loading.hide();
                        if (data.result === true) {
                            alertify.success(data.message);
                        }
                    }
                });
            }
        };
        this.getSystemConfigData= function () {
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: 'Config/GetSystemConfigData',
                success: function (data) {
                    $scope.ticketDesings = data.ticketDesings;
                    $scope.xpiredTimes = data.xpiredTimes;
                    $scope.$apply();
                    window.setTimeout(function () {
                        if (data.config.Id > 0) {
                            $scope.config = data.config;
                        }
                        $scope.$apply();
                    }, 0);
                }
            });
        }
        this.getSystemConfigData();
    }
})();
