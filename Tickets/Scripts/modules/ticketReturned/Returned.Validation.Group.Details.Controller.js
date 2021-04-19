/**=========================================================
 * Module: returned.Validation.Group.Details.Controller.js
 =========================================================*/

(function() {
    'use strict';

    angular
        .module('naut')
        .controller('ReturnedValidationGroupDetailsController', ReturnedValidationGroupDetailsController);

    ReturnedValidationGroupDetailsController.$inject = ['$scope', '$state', '$rootScope', '$stateParams'];
    function ReturnedValidationGroupDetailsController($scope, $state, $rootScope, $stateParams) {
        $scope.editMode = false;
        if ($rootScope.returnURL == '/#/ticket/Returneds' || !$rootScope.returnURL) {
            $scope.editMode = true;
        }
        $scope.returnBack = function () {
            if ($rootScope.returnURL == '/#/ticket/Returneds' || !$rootScope.returnURL) {
                window.location.href = '/#/ticket/Returneds';
            } else {
                window.location.href = $rootScope.returnURL;
            }
        }

        var self = this;
        this.loadAllocation = function (allocationId) {
            window.loading.show();
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: 'Ticket/ReturnedDetailsGroup?group=' + $stateParams.group + '&raffleId=' + $stateParams.raffleId,
                success: function (data) {
                    window.loading.hide();

                    $scope.returns = data.map(function (r) {
                        $scope.fractionTotal += r.FractionTo - r.FractionFrom + 1;
                        r.CreateDate = new Date(r.CreateDate);
                        return r;
                    });

                    $rootScope.destroyDataTable();
                    $scope.$apply();
                    $rootScope.dataTable();


                }
            });
        }
        $scope.fractionTotal = 0;
        this.loadAllocation();
    }
    
    
})();
