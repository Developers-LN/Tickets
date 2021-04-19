/**=========================================================
 * Module: EmployeeController.js
 =========================================================*/

(function() {
    'use strict';

    angular
        .module('naut')
        .controller('EmployeeController', EmployeeController);

    EmployeeController.$inject = ['$scope', '$rootScope', '$state', 'employeeServices', 'departmentServices'];
    function EmployeeController($scope, $rootScope, $state, employeeServices, departmentServices) {
        employeeServices.init($rootScope, $scope, $state);
        departmentServices.init($rootScope, $scope, $state);
    }
})();
