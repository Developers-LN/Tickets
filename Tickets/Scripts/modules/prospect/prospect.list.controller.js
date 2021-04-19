/**=========================================================
 * Module: ProspectListController.js
 =========================================================*/

(function() {
    'use strict';

    angular
        .module('naut')
        .controller('ProspectListController', ProspectListController);

    ProspectListController.$inject = ['$scope', '$rootScope', '$state'];
    function ProspectListController($scope, $rootScope, $state) {
        var self = this;
        this.getProspectList = function () {
            window.loading.show();
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: $rootScope.serverUrl + 'ticket/prospectApi/getProspects',
                success: function (response) {
                    window.loading.hide();
                    if (response.result == true) {
                        $scope.prospects = response.object;
                        $scope.$apply();
                        $rootScope.dataTable();
                    } else {
                        alertify.alert(response.message);
                    }
                }
            });
        }

        $scope.startWorkflowProspect = function (prospect) {
            // confirm dialog
            alertify.confirm("&iquest;Desea enviar este prospecto a un proceso de aprobaci&#243;n?", function (e) {
                if (e) {
                    window.loading.show();
                    $.ajax({
                        type: 'POST',
                        dataType: 'json',
                        url: $rootScope.serverUrl + 'ticket/workflowApi/sendProspectToWorkflow',
                        data: prospect,
                        success: function (response) {
                            window.loading.hide();
                            if (response.result === true) {
                                $rootScope.destroyDataTable();
                                self.getProspectList();
                                alertify.success(response.message);
                            } else {
                                alertify.alert(data.message);
                            }
                        }
                    });
                }
            });
        }

        this.getProspectList();
    }
})();
