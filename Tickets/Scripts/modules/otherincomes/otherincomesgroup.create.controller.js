/**=========================================================
 * Module: OtherIncomesGroupCreateController.js
 =========================================================*/

(function () {
    'use strict';

    angular
        .module('naut')
        .controller('OtherIncomesGroupCreateController', OtherIncomesGroupCreateController);

    OtherIncomesGroupCreateController.$inject = ['$scope', '$rootScope', '$state', '$stateParams'];
    function OtherIncomesGroupCreateController($scope, $rootScope, $state, $stateParams) {
        var self = this;

        this.validateOtherIncomeData = function (otherIncomeGroup) {
            var error = '', isReq = ' es un campo requerido. <br>';
            if (otherIncomeGroup.Description === undefined) {
                error += 'La descripción' + isReq;
            }
            if (error !== '') {
                alertify.showError('Alerta', error);
            }
            return error === '';
        }

        this.clearOtherIncome = function () {
            $scope.otherIncomeGroup = {
                Id: 0,
                Description: undefined,
                Status: undefined
            };
        }

        $scope.saveOtherIncomeGroupPaymentForm = function () {
            if (self.validateOtherIncomeData($scope.otherIncomeGroup) === false) {
                return;
            }
            window.loading.show();
            $.ajax({
                type: 'POST',
                dataType: 'json',
                url: 'OtherIncomes/CreateGroup',
                data: $scope.otherIncomeGroup,
                success: function (data) {
                    window.loading.hide();
                    if (data.Value === true) {
                        alertify.success('Cuenta guardada correctamente!');
                        window.location.href = '#/others/otherIncomePaymentByGroup/' + data.Results.Id;
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
                url: 'OtherIncomes/GetOtherGroupData?otherIncomeGroupId=' + $stateParams.otherIncomeGroupId,
                success: function (data) {
                    window.loading.hide();
                    $scope.origins = data.origins;
                    $scope.otherIncomeTypes = data.otherIncomeTypes;
                    $scope.periodicities = data.periodicities;
                    $scope.status = data.status;

                    if ($stateParams.otherincomeId == 0) {
                        self.clearOtherIncome();
                    } else {
                        $scope.otherIncomeGroup = data.otherIncomeGroup;
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
