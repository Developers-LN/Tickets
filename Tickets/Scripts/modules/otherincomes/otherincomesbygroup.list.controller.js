/**=========================================================
 * Module: OtherIncomesGroupHistoryController.js
 =========================================================*/

(function () {
    'use strict';

    angular
        .module('naut')
        .controller('OtherIncomesGroupHistoryController', OtherIncomesGroupHistoryController);

    OtherIncomesGroupHistoryController.$inject = ['$scope', '$state', '$rootScope', '$stateParams'];
    function OtherIncomesGroupHistoryController($scope, $state, $rootScope, $stateParams) {

        this.getPaymentHistory = function () {
            window.loading.show();
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: 'OtherIncomes/GetPaymentGroupHistory?otherIncomeGroupId=' + $stateParams.otherIncomeGroupId,
                success: function (data) {
                    window.loading.hide();
                    $scope.Id = data.Id;
                    $scope.sequenceNumber = data.sequenceNumber;
                    $scope.Description = data.Description;
                    $scope.CreateDate = data.CreateDate;
                    $scope.Status = data.Status;
                    $scope.statusDesc = data.statusDesc;
                    $scope.payments = data.payments;

                    $rootScope.destroyDataTable();
                    $scope.$apply();
                    $rootScope.dataTable();
                }
            });
        }

        $scope.approvegroup = function(id){
            // confirm dialog
            alertify.confirm("&iquest;Desea realizar la aprobación de este grupo de ingresos?", function (e) {
                if (e) {
                    window.loading.show();
                    $.ajax({
                        type: 'POST',
                        dataType: 'json',
                        url: 'OtherIncomes/approveGroup?id=' + id,
                        success: function (response) {
                            window.loading.hide();
                            if (response.result == false) {
                                alertify.alert(response.message);
                            } else {
                                window.location.reload();
                                alertify.alert(response.message);
                            }
                        }
                    });
                }
            });
        }

        this.getPaymentHistory();
    }
})();
