/**=========================================================
 * Module: SecurityController.js
 =========================================================*/

(function () {
    'use strict';

    angular
        .module('naut')
        .controller('SecurityController', SecurityController);

    SecurityController.$inject = ['$scope', '$rootScope', '$state', 'securityUser', 'securityRol', 'securityModule'];
    function SecurityController($scope, $rootScope, $state, securityUser, securityRol, securityModule) {
        $scope.userList = [];
        $scope.rolList = [];
        $scope.moduleList = [];

        securityUser.init($rootScope, $scope, $state);
        securityRol.init($rootScope, $scope, $state);
        securityModule.init($rootScope, $scope, $state);

        $rootScope.createSelect2();
    }
})();
