/**=========================================================
 * Module: OtherIncomesPaymentListController.js
 =========================================================*/

(function () {
    'use strict';

    angular
        .module('naut')
        .controller('OtherIncomesPaymentListController', OtherIncomesPaymentListController);

    OtherIncomesPaymentListController.$inject = ['$scope', '$rootScope', '$state'];
    function OtherIncomesPaymentListController($scope, $rootScope, $state) {
        var self = this;
        this.getOtherIncomesList = function () {
            window.loading.show();
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: 'OtherIncomes/GetOtherIncomeGroupsList',
                success: function (data) {
                    window.loading.hide();
                    $scope.otherIncomesGroupList = data.otherIncomesGroup;

                    $scope.$apply();
                    $rootScope.createSelect2();
                }
            });
        }

        $scope.changeValues = function () {
            if ($scope.otherIncomeGroupId == 0) {
                return;
            }

            window.loading.show();
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: 'OtherIncomes/GetOtherIncomePaymentList?otherIncomeGroupId=' + $scope.otherIncomeGroupId,
                success: function (response) {
                    window.loading.hide();
                    console.log(response);
                    $scope.otherIncomesPayments = response.otherIncomesPayments.map(function (payment) {
                        payment.PaymentDate = new Date(payment.PaymentDate);
                        return payment;
                    });
                    $rootScope.destroyDataTable();
                    $scope.$apply();
                    $rootScope.dataTable();
                }
            });
        }

        $scope.deleteAccount = function (client) {
            // confirm dialog
            alertify.confirm("&iquest;Desea borrar este cliente?", function (e) {
                if (e) {
                    $.ajax({
                        type: 'POST',
                        dataType: 'json',
                        url: 'Client/Delete',
                        data: { clientId: client.Id },
                        success: function (data) {
                            if (data === true) {
                                self.getOtherIncomesList();
                                alertify.success('Cliente borrado correctamente!');
                            }
                        }
                    });
                }
            });
        }

        $scope.startWorkflowProspect = function (client) {
            // confirm dialog
            alertify.confirm("&iquest;Desea enviar este cliente a un proceso de aprobaci&#243;n?", function (e) {
                if (e) {
                    $.ajax({
                        type: 'POST',
                        dataType: 'json',
                        url: 'Client/SendClientProccess',
                        data: { clientId: client.Id },
                        success: function (data) {
                            if (data.result === true) {
                                self.getClientList();
                                alertify.success('El cliente fue enviado al flujo de aprobaci&#243;n correctamente!');
                            } else {
                                alertify.alert(data.message);
                            }
                        }
                    });
                }
            });
        }

        $scope.otherIncomeId = 0;
        this.getOtherIncomesList();
    }
})();
