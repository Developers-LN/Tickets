/**=========================================================
 * Module: CashReportsController.js
 =========================================================*/

(function() {
    'use strict';

    angular
        .module('naut')
        .controller('CashReportsController', CashReportsController);

    CashReportsController.$inject = ['$scope', '$state', '$rootScope', '$stateParams'];
    function CashReportsController($scope, $state, $rootScope, $stateParams) {
        $scope.cash = {
            userId: undefined,
            /*raffleId: undefined,*/
            fechaPago: undefined
        };

        function validateData(cash) {
            var error = '', isReq = ' es un campo requerido. <br>';

            if (cash.userId === undefined) {
                error += 'El usuario' + isReq;
            }
            if (cash.fechaPago === undefined) {
                error += 'la fecha' + isReq;
            }
            if (error !== '') {
                alertify.showError('Alerta', error);
            }
            return error === '';
        }

        $scope.userList = function (userId/*, raffleId*/) {
            window.loading.show();
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: 'Cash/CashReportData',
                success: function (data) {
                    window.loading.hide();
                    if (userId <= 0 /*|| raffleId <= 0*/) {
                        $scope.users = data.users;
                        /*$scope.raffles = data.raffles;*/
                    }
                    $rootScope.createSelect2();
                    $rootScope.destroyDataTable();
                    $scope.$apply();
                    $rootScope.dataTable();
                }
            });
        }
        $scope.updatelist = function () {
            $scope.userList($scope.userId);
        }

        $scope.userList(0);

        $scope.PrintReport = function () {
            function zero(n) {
                return (n > 9 ? '' : '0') + n;
            }
            if (validateData($scope.cash) === false) {
                return;
            }
            var FechaSelect = $scope.cash.fechaPago;
            var FechaConvert = FechaSelect.getFullYear() + "-" + zero(FechaSelect.getMonth() + 1) + "-" + zero(FechaSelect.getDate());
            window.open("Reports/GetCashReport?userId=" + $scope.cash.userId + "&Fecha=" + FechaConvert);
        }
    }
})();
