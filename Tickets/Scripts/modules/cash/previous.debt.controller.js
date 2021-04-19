/**=========================================================
 * Module: PreviousDebtController.js
 =========================================================*/

(function() {
    'use strict';

    angular
        .module('naut')
        .controller('PreviousDebtController', PreviousDebtController);

    PreviousDebtController.$inject = ['$scope', '$state', '$rootScope', '$stateParams'];
    function PreviousDebtController($scope, $state, $rootScope, $stateParams) {
        $scope.cash = {
            Id: 0,
            ClientId: undefined,
            PaimentValue: undefined,
            Note: undefined
        };

        $rootScope.createSelect2();

        $scope.clientList = function (clientId) {
            window.loading.show();
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: 'Cash/GetPreviousDebt?clientId=' + clientId,
                success: function (data) {
                    window.loading.hide();
                    if (clientId <= 0) {
                        $scope.clients = data;
                    }
                    
                    $rootScope.destroyDataTable();
                    $scope.$apply();
                    $rootScope.dataTable();
                }
            });
        }
        $scope.updatelist = function () {
            $scope.clientList($scope.clientId);
        }
        $scope.clientList(0);

        function validateData(cash) {
            var error = '', isReq = ' es un campo requerido. <br>';

            if (cash.ClientId === undefined) {
                error += 'El Cliente' + isReq;
            }
            if (cash.PaimentValue === undefined) {
                error += 'El Monto a Pagar' + isReq;
            }
            if (cash.Note === undefined) {
                error += 'El Concepto' + isReq;
            }
            if (error !== '') {
                alertify.showError('Alerta', error);
            }
            return error === '';
        }

        $scope.previousDebtPaymentSave = function () {
            if (validateData($scope.cash) == false) {
                return;
            }
            window.loading.show();
            $.ajax({
                type: 'POST',
                dataType: 'json',
                url: 'Client/PreviousDebtPaymentSave',
                data: $scope.cash,
                success: function (data) {
                    window.loading.hide();
                    if (data.result === false) {
                        alertify.alert(data.message);
                    } else {
                        alertify.success(data.message);
                        window.open('Reports/PreviousDebtPaymentReport?paymentId=' + data.payment.Id);
                        window.location.href = '#/dashboard';
                    }
                }
            });
        }

        $scope.getDebt = function () {
            $scope.clients.forEach(function (client) {
                if (client.Id == $scope.cash.ClientId) {
                    $scope.PreviousDebt = client.PreviousDebt;
                    $scope.PrevieusPayment = client.PrevieusPayment;
                }
            });
        }
    }
})();
