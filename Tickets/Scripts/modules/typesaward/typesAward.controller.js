/**=========================================================
 * Module: TypesAwardController.js
 =========================================================*/

(function() {
    'use strict';

    angular
        .module('naut')
        .controller('TypesAwardController', TypesAwardController);
    
    TypesAwardController.$inject = ['$scope', '$rootScope', '$state', 'typesAwardServices'];
    function TypesAwardController($scope, $rootScope, $state, typesAwardServices) {
        typesAwardServices.init($rootScope, $scope, $state);
    }
})();
