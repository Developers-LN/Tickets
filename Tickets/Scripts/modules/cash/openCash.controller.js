/**=========================================================
 * Module: OpenCashController.js
 =========================================================*/

(function() {
    'use strict';

    angular
        .module('naut')
        .controller('OpenCashController', OpenCashController);

    OpenCashController.$inject = ['$scope', '$state'];
    function OpenCashController($scope, $state) {
        function validateData(cash) {
            var error = '', isReq = ' es un campo requerido. <br>';
            if (cash.Name == '') {
                error += 'Monto de Entrada' + isReq;
            }
            if (error !== '') {
                alertify.showError('Alerta', error);
            }
            return error === '';
        }

        $.ajax({
            type: 'GET',
            contentType: 'application/json; charset=utf-8',
            url: 'Cash/VerifyCashClose',
            success: function (data) {
                if (data.result === false) {
                    alertify.alert(data.message);
                    $state.go('app.dashboard');
                }
            }
        });

        $scope.saveCashForm = function () {
            if (validateData($scope.cash) === false) {
                return;
            }
            $.ajax({
                type: 'POST',
                dataType: 'json',
                url: 'Cash/OpenCash',
                data: $scope.cash,
                success: function (data) {
                    if (data.result === true) {
                        alertify.success(data.message);
                        $state.go('app.dashboard');
                    } else {
                        alertify.error(data.message);
                    }
                }
            });
        }
    }
})();
