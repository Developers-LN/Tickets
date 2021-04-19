/**=========================================================
 * Module: SuscriberDetailsController.js
 =========================================================*/

(function() {
    'use strict';

    angular
        .module('naut')
        .controller('SuscriberDetailsController', SuscriberDetailsController);

    SuscriberDetailsController.$inject = ['$scope', '$state', '$rootScope', '$stateParams'];
    function SuscriberDetailsController($scope, $state, $rootScope, $stateParams) {
        var self = this;
        this.loadAllocation = function (allocationId) {
            window.loading.show();
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: $rootScope.serverUrl + 'ticket/ticketSuscriberApi/get?id=' + $stateParams.id,
                success: function (response) {
                    window.loading.hide();
                    response.object.CreateDate = new Date(response.object.CreateDate);
                    $scope.ticket = response.object;
                    $rootScope.destroyDataTable();
                    $scope.$apply();
                    $rootScope.dataTable();
                }
            });
        }

        $scope.deleteNumber = function (number) {
            // confirm dialog
            alertify.confirm("&iquest;Desea borrar este billete abonado?", function (e) {
                if (e) {
                    window.loading.show();
                    $.ajax({
                        type: 'POST',
                        dataType: 'json',
                        url: $rootScope.serverUrl + 'ticket/ticketSuscriberApi/deleteNumber',
                        data: number,
                        success: function (response) {
                            window.loading.hide();
                            if (response.result == true) {
                                alertify.success(response.message);
                                self.loadAllocation();
                            } else{
                                alertify.alert(response.message);
                            }

                        }
                    });
                }
            });
        }

        this.loadAllocation();
    }
})();
