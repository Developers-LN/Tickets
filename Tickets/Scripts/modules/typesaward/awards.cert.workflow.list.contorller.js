/**=========================================================
 * Module: AwardsCertWorkflowListController.js
 =========================================================*/

(function() {
    'use strict';

    angular
        .module('naut')
        .controller('AwardsCertWorkflowListController', AwardsCertWorkflowListController);

    AwardsCertWorkflowListController.$inject = ['$scope', '$rootScope', '$state'];
    function AwardsCertWorkflowListController($scope, $rootScope, $state) {

        this.getWorkflowList = function () {
            window.loading.show();
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: $rootScope.serverUrl + 'ticket/workflowApi/getWorkflowList?type=' + 6/*Certificación de premios*/,
                success: function (response) {
                    window.loading.hide();
                    if (response.result == true) {
                        $scope.workflowList = response.object.map(function (w) {
                            w.createDate = new Date(w.createDateLong);
                            return w;
                        });
                        $scope.$apply();
                        $rootScope.dataTable();
                    } else {
                        alertify.alert(response.message);
                    }
                }
            });
        }

        this.getWorkflowList();
    }
})();