/**=========================================================
 * Module: VentaLoteria365Controller.js
 =========================================================*/

(function () {
    'use strict';

    angular
        .module('naut')
        .controller('VentaLoteria365Controller', VentaLoteria365Controller);

    VentaLoteria365Controller.$inject = ['$scope', '$state', '$rootScope', '$stateParams'];
    function VentaLoteria365Controller($scope, $state, $rootScope, $stateParams) {
        $scope.venta = {
            RaffleId: undefined,
            ClientId: undefined
        };

        this.loadData = function () {
            $rootScope.systemLoading = true;
            $.when(
                $.ajax($rootScope.serverUrl + 'ticket/clientApi/getClientSelect?statu=2089'),/*Clientes aprobados*/
                $.ajax($rootScope.serverUrl + 'ticket/raffleApi/getRaffleSelect?statu=0')//todos
            ).then(function (clientResponse, raffleResponse) {
                $rootScope.systemLoading = false;
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

        function validateData(venta) {
            var error = '', isReq = ' es un campo requerido. <br>';

            if (venta.RaffleId === undefined) {
                error += 'El sorteo' + isReq;
            }
            if (venta.ClientId === undefined) {
                error += 'El cliente' + isReq;
            }
            if (error !== '') {
                alertify.showError('Alerta', error);
            }
            return error === '';
        }
        $scope.printReport = function () {
            if (validateData($scope.venta) === false) {
                return;
            }
            window.open('Reports/VentasLoteria365?clientId=' + $scope.venta.ClientId + '&raffleId=' + $scope.venta.RaffleId);
        }
    }
})();