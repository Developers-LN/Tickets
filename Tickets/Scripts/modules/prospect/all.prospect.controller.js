/**=========================================================
 * Module: AllProspectsController.js
 =========================================================*/

(function() {
    'use strict';

    angular
        .module('naut')
        .controller('AllProspectsController', AllProspectsController);

    AllProspectsController.$inject = ['$scope', '$rootScope', '$state'];
    function AllProspectsController($scope, $rootScope, $state) {
        $rootScope.systemLoading = true;
        $.ajax({
            type: 'GET',
            contentType: 'application/json; charset=utf-8',
            url: 'ticket/prospectApi/getProspects?statu=0',
            success: function (data) {
                $rootScope.systemLoading = false;
                $scope.prospects = data.object;
                $scope.$apply();
                $rootScope.dataTable();
            }
        });
    }
})();
