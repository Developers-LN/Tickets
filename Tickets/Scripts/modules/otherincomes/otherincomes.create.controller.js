/**=========================================================
 * Module: OtherIncomesCreateController.js
 =========================================================*/

(function () {
    'use strict';

    angular
        .module('naut')
        .controller('OtherIncomesCreateController', OtherIncomesCreateController);

    OtherIncomesCreateController.$inject = ['$scope', '$rootScope', '$state', '$stateParams'];
    function OtherIncomesCreateController($scope, $rootScope, $state, $stateParams) {
        var self = this;

        this.validateOtherIncomeData = function (otherIncome) {
            var error = '', isReq = ' es un campo requerido. <br>';
            if (otherIncome.NoCatalogAccount === undefined) {
                error += 'El número de cuenta' + isReq;
            }
            if (otherIncome.AccountName === undefined) {
                error += 'El nombre de la cuenta' + isReq;
            }
            if (otherIncome.Origin === undefined) {
                error += 'El origen' + isReq;
            }
            if (otherIncome.AccountType === undefined) {
                error += 'El tipo de cuenta' + isReq;
            }
            if (otherIncome.Description === undefined) {
                error += 'La descripción' + isReq;
            }
            if (otherIncome.Periodicity === undefined) {
                error += 'La periodicidad de cobro' + isReq;
            }
            if (error !== '') {
                alertify.showError('Alerta', error);
            }
            return error === '';
        }

        this.clearOtherIncome = function () {
            $scope.otherIncome = {
                Id: 0,
                NoCatalogAccount: undefined,
                AccountName: undefined,
                Origin: undefined,
                AccountType: undefined,
                Description: undefined,
                Periodicity: undefined,
                CreateDate: undefined,
                CreateUser: undefined,
                Status: undefined
            };
        }

        $scope.saveOtherIncomeForm = function () {
            if (self.validateOtherIncomeData($scope.otherIncome) === false) {
                return;
            }
            window.loading.show();
            $.ajax({
                type: 'POST',
                dataType: 'json',
                url: 'OtherIncomes/Create',
                data: $scope.otherIncome,
                success: function (data) {
                    window.loading.hide();
                    if (data === true) {
                        alertify.success('Cuenta guardada correctamente!');
                        $state.go('app.otherincomesList');
                    } else {
                        alertify.alert(data.message);
                    }
                }
            });
        }

        this.loadOtherIncomeData = function () {
            window.loading.show();
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: 'OtherIncomes/GetOtherIncomeData?otherincomeId=' + $stateParams.otherincomeId,
                success: function (data) {
                    window.loading.hide();
                    $scope.origins = data.origins;
                    $scope.otherIncomeTypes = data.otherIncomeTypes;
                    $scope.periodicities = data.periodicities;
                    $scope.status = data.status;
                    $scope.otherIncome = data.otherIncome;

                    if ($stateParams.otherincomeId == 0) {
                        self.clearOtherIncome();
                    } else {
                        $scope.otherIncome = data.otherIncome;
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

        this.loadOtherIncomeData();
    }
})();
