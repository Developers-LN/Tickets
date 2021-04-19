/**=========================================================
 * Module: WorkflowReprintController.js
 =========================================================*/

(function() {
    'use strict';

    angular
        .module('naut')
        .controller('WorkflowReprintController', WorkflowReprintController);

    WorkflowReprintController.$inject = ['$scope', '$rootScope', '$state'];
    function WorkflowReprintController($scope, $rootScope, $state) {
        this.getWorkflowList = function () {
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: $rootScope.serverUrl + 'ticket/workflowApi/getWorkflowList?type='+ 4,/*tipo de flujo de reimpresion*/
                success: function (response) {
                    if (response.result == true) {
                        $scope.workflowList = response.object;
                        $scope.$apply();
                        $rootScope.dataTable();
                    } else {
                        alertify.alert(response.message)
                    }
                }
            });
        }

        this.getWorkflowList();
    }
})();
