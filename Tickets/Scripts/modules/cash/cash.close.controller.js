/**=========================================================
 * Module: CashCloseController.js
 =========================================================*/

(function() {
    'use strict';

    angular
        .module('naut')
        .controller('CashCloseController', CashCloseController);

    CashCloseController.$inject = ['$scope', '$state', '$rootScope', '$stateParams'];
    function CashCloseController($scope, $state, $rootScope, $stateParams) {
        $scope.cashClose = {
            Id: 0,
            ToktalCashCalculated: '',
            TotalCheckCalculated: '',
            TotalCardCalculated: '',
            ToktalCashRegistered: '',
            TotalCheckRegistered: '',
            TotalCardRegistered: ''
        };
        function validateData(cashClose) {
            var error = '', isReq = ' es un campo requerido. <br>';

            if (validateNumenber(cashClose.ToktalCashRegistered) === false) {
                error += 'Total en Efectivo' + isReq;
            }
            if (validateNumenber(cashClose.TotalCheckRegistered) === false) {
                error += 'Total en Cheques' + isReq;
            }
            if (validateNumenber(cashClose.TotalCardRegistered) === false) {
                error += 'Total en Credito' + isReq;
            }
            if (error !== '') {
                alertify.showError('Alerta', error);
            }
            return error === '';
        }
        function validateNumenber(number) {
            if (Number(number) < 0 || number === '') {
                return false
            }
            return true;
        }
        
        $.ajax({
            type: 'GET',
            contentType: 'application/json; charset=utf-8',
            url: 'Cash/VerifyCashOpen',
            success: function (data) {
                if (data.result !== true) {
                    $state.go('app.dashboard');
                    alertify.confirm(data.message, function (ok) {
                        if (ok === true) {
                            if (data.result === 1) {
                                $state.go('app.cashOpen');
                            } else if (data.result === 2) {
                            }
                        }
                    }, function () { });
                } else {
                    $.ajax({
                        type: 'GET',
                        contentType: 'application/json; charset=utf-8',
                        url: 'Cash/GetCashCloseData',
                        success: function (data) {
                            if (data.result === false) {
                                alertify.alert(data.message);
                                $scope.goBack();
                            } else {
                                $scope.cashClose.ToktalCashCalculated = data.totalCash;
                                $scope.cashClose.TotalCheckCalculated = data.totalCheck;
                                $scope.cashClose.TotalCardCalculated = data.totalCard;
                                $scope.$apply();
                            }
                        }
                    });
                }
            }
        });
        $scope.goBack = function(){
            $state.go('app.dashboard');
        }
        $scope.saveForm = function () {
            if (validateData($scope.cashClose) === false) {
                return;
            }
            $.ajax({
                type: 'POST',
                dataType: 'json',
                url: 'Cash/CashClose',
                data: $scope.cashClose,
                success: function (data) {
                    if (data.result === true) {
                        alertify.success(data.message);
                        $scope.goBack();
                    } else {
                        alertify.alert(data.message);
                    }
                }
            });
        }
    }
})();
