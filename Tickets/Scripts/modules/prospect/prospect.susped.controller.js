/**=========================================================
 * Module: ProspectSuspendController.js
 =========================================================*/

(function() {
    'use strict';

    angular
        .module('naut')
        .controller('ProspectSuspendController', ProspectSuspendController);

    ProspectSuspendController.$inject = ['$scope', '$rootScope', '$state'];
    function ProspectSuspendController($scope, $rootScope, $state) {
        this.getProspectList = function () {
            window.loading.show();
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: 'ticket/prospectApi/getProspects?statu=2074',
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
        $scope.viewProspect = function (prospect) {
            $rootScope.prospect = prospect;
            $rootScope.prospectList = $scope.prospectList;
            $rootScope.viewMode = true;
            $rootScope.returnBack = 'app.prospectSuspends';
            $state.go('app.prospectcreate');
        }
        this.getProspectList();
    }
})();
