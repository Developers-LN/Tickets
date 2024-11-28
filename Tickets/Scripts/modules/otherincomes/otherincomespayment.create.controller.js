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

        this.validateOtherIncomePaymentData = function (otherIncomePayment) {
            var error = '', isReq = ' es un campo requerido. <br>';
            if (otherIncomePayment.TotalPayment === undefined) {
                error += 'El monto pagado' + isReq;
            }
            if (otherIncomePayment.OtherIncomeDetailDescription === undefined) {
                error += 'La descripcion de pago' + isReq;
            }
            if (otherIncomePayment.PaymentDate === undefined) {
                error += 'La fecha de pago' + isReq;
            }
            if (otherIncomePayment.OtherIncomeId === undefined) {
                error += 'La cuenta de otros ingresos' + isReq;
            }
            if (otherIncomePayment.BankAccountCatalogId === undefined) {
                error += 'La cuenta de banco' + isReq;
            }
            if (error !== '') {
                alertify.showError('Alerta', error);
            }
            return error === '';
        }

        this.clearOtherIncomePayment = function () {
            $scope.otherIncomePayment = {
                Id: 0,
                OtherIncomesGroupId: $stateParams.otherincomeGroupId,
                OtherIncomeId: undefined,
                OtherIncomesGroupDescription: undefined,
                OtherIncomeDetailDescription: undefined,
                TotalPayment: undefined,
                PaymentDate: undefined,
                BankAccountCatalogId: undefined
            };
        }

        $scope.saveOtherIncomePaymentForm = function () {
            try {
                $scope.otherIncomePayment.PaymentDate = $rootScope.parseDate($scope.otherIncomePayment.PaymentDate, $scope.otherIncomePayment.PaymentDate).toJSON();
            } catch (e) { }
            if (self.validateOtherIncomePaymentData($scope.otherIncomePayment) === false) {
                return;
            }
            window.loading.show();
            $.ajax({
                type: 'POST',
                dataType: 'json',
                url: 'OtherIncomes/CreatePayment',
                data: $scope.otherIncomePayment,
                success: function (data) {
                    window.loading.hide();
                    if (data.result == true) {
                        window.open('/Reports/OtherPaymentReceipt?paymentId=' + data.paymentId);
                        window.location.href = '#/others/otherIncomePaymentByGroup/' + data.groupId;
                        alertify.success('Cuenta guardada correctamente!');
                    } else {
                        alertify.alert(data.message);
                    }
                }
            });
        }

        this.loadOtherIncomePaymentData = function () {
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: 'OtherIncomes/GetOtherIncomeDetailData?otherincomeDetailId=' + $stateParams.otherincomeDetailId,
                success: function (data) {
                    $scope.otherIncomeList = data.otherIncome;
                    $scope.bankAccountList = data.bankAccount;
                    if ($stateParams.otherincomeDetailId == 0) {
                        self.clearOtherIncomePayment();
                    } else {
                        $scope.otherIncomePayment = {
                            Id: data.otherIncomeDetail.Id,
                            OtherIncomesGroupId: data.otherIncomeDetail.OtherIncomesGroupId,
                            OtherIncomeId: data.otherIncomeDetail.OtherIncomeId,
                            OtherIncomesGroupDescription: data.otherIncomeDetail.OtherIncomesGroupDescription,
                            OtherIncomeDetailDescription: data.otherIncomeDetail.OtherIncomeDetailDescription,
                            TotalPayment: data.otherIncomeDetail.TotalPayment,
                            PaymentDate: new Date(data.otherIncomeDetail.PaymentDate),
                            BankAccountCatalogId: data.otherIncomeDetail.BankAccountCatalogId
                        };
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
