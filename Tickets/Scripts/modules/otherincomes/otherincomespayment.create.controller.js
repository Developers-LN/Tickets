/**=========================================================
 * Module: OtherIncomesPaymentCreateController.js
 =========================================================*/

(function () {
    'use strict';

    angular
        .module('naut')
        .controller('OtherIncomesPaymentCreateController', OtherIncomesPaymentCreateController);

    OtherIncomesPaymentCreateController.$inject = ['$scope', '$rootScope', '$state', '$stateParams'];
    function OtherIncomesPaymentCreateController($scope, $rootScope, $state, $stateParams) {
        var self = this;

        this.validateOtherIncomePaymentData = function (otherIncomePayement) {
            var error = '', isReq = ' es un campo requerido. <br>';
            if (otherIncomePayement.Total === undefined) {
                error += 'El monto pagado' + isReq;
            }
            if (otherIncomePayement.Description === undefined) {
                error += 'La descripcion' + isReq;
            }
            if (otherIncomePayement.PaymentDate === undefined) {
                error += 'La fecha de pago' + isReq;
            }
            if (otherIncomePayement.OtherIncomeId === undefined) {
                error += 'La cuenta de otros ingresos' + isReq;
            }
            if (error !== '') {
                alertify.showError('Alerta', error);
            }
            return error === '';
        }

        this.clearOtherIncomePayment = function () {
            $scope.otherIncomePayement = {
                Id: 0,
                Total: undefined,
                Description: undefined,
                CreateDate: undefined,
                CreateUser: undefined,
                PaymentDate: undefined,
                OtherIncomeId: undefined,
                SequenceNumber: undefined
            };
        }

        $scope.saveOtherIncomePaymentForm = function () {
            try {
                $scope.otherIncomePayement.PaymentDate = $rootScope.parseDate($scope.otherIncomePayement.PaymentDate, $scope.otherIncomePayement.PaymentDate).toJSON();
            } catch (e) { }
            if (self.validateOtherIncomePaymentData($scope.otherIncomePayement) === false) {
                return;
            }
            window.loading.show();
            $.ajax({
                type: 'POST',
                dataType: 'json',
                url: 'OtherIncomes/CreatePayment',
                data: $scope.otherIncomePayement,
                success: function (data) {
                    window.loading.hide();
                    console.log(data);
                    if (data.result == true) {
                        window.open('/Reports/OtherPaymentReceipt?paymentId=' + data.paymentId);
                        alertify.success('Cuenta guardada correctamente!');
                        $state.go('app.otherincomesPaymentsList');
                    } else {
                        alertify.alert(data.message);
                    }
                }
            });
        }

        this.loadOtherIncomePaymentData = function () {
            window.loading.show();
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: 'OtherIncomes/GetOtherIncomeList',
                success: function (data) {
                    window.loading.hide();
                    $scope.otherIncomeList = data.otherIncome;

                    if ($stateParams.otherincomePaymentId == 0) {
                        self.clearOtherIncomePayment();
                    } else {
                        $scope.otherIncomePayement = data.otherIncomePayement;
                    }
                    if (!$scope.$$phase) {
                        window.setTimeout(function () {
                            $scope.$apply();
                            $rootScope.createSelect2();
                        }, 0);
                    }
                }
            });
        }

        this.loadOtherIncomePaymentData();
    }
})();
