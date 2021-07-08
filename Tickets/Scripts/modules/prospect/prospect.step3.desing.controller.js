/**=========================================================
 * Module: ProspectDesingController.js
 =========================================================*/

(function () {
    'use strict';

    angular
        .module('naut')
        .controller('ProspectDesingController', ProspectDesingController);

    ProspectDesingController.$inject = ['$scope', '$rootScope'];
    function ProspectDesingController($scope, $rootScope) {
        var self = this;
        this.loadDesingData = function () {
            window.loading.show();
            $.when(
                $.ajax($rootScope.serverUrl + 'ticket/catalogApi/getProspectGroupSelect'),
                $.ajax($rootScope.serverUrl + 'ticket/catalogApi/getProspectStatuSelect'),
                $.ajax($rootScope.serverUrl + 'ticket/catalogApi/getProspectPrintSelect')
            ).then(function (groupResponse, statuResponse, printResponse) {
                window.loading.hide();
                if (groupResponse[1] == 'success') {
                    $scope.groups = groupResponse[0].object;
                }
                if (statuResponse[1] == 'success') {
                    $scope.status = statuResponse[0].object.filter(function (statu) {
                        return (statu.id != 22 && statu.id != 23);
                    });
                }
                if (printResponse[1] == 'success') {
                    $scope.prints = printResponse[0].object;
                }
                $scope.prospect = $rootScope.prospect;
                window.setTimeout(function () {
                    $scope.$apply();
                }, 100);
            });
        }
        if ($rootScope.prospect) {
            self.loadDesingData();
        }
        $rootScope.$watch('activeStep', function () {
            if ($rootScope.activeStep == 2) {
                self.loadDesingData();
            }
        });

        $scope.goToNextStep = function () {
            if ($rootScope.prospectValidate($scope.prospect) == false) {
                return;
            }
            $rootScope.prospect = $scope.prospect;
            $rootScope.goStep4();
        }
    }
})();
