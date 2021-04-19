/**=========================================================
 * Module: CreationTypeController.js
 =========================================================*/

(function() {
    'use strict';

    angular
        .module('naut')
        .controller('CreationTypeController', CreationTypeController);

    CreationTypeController.$inject = ['$scope', '$rootScope'];
    function CreationTypeController($scope, $rootScope) {
        var self = this;

        this.loadExistentProspect = function(){
            window.loading.show();
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: 'ticket/prospectApi/getProspectSelect',
                success: function (response) {
                    window.loading.hide();
                    if (response.result == true) {
                        $scope.prospects = response.object;
                        $scope.$apply();
                    } else {
                        alertify.alert(response.message);
                    }
                }
            });
        }

        $scope.sourceProspectId = 0;

        this.changeProspectTypeRadio = function (radioGroup) {
            $scope.selectedTypeRadio = radioGroup.value;
            if (radioGroup.value === 'exist') {
                $('#existenProspectDropDown').removeClass('hide');
            } else {
                $scope.sourceProspectId = 0;
                $('#existenProspectDropDown').addClass('hide');
            }
        }

        $scope.selectedTypeRadio = 'new';
        $("input[name=typeRadio]:radio").change(function (val) {
            self.changeProspectTypeRadio(val.currentTarget);
        });

        $scope.goToNextStep = function () {
            $rootScope.loadProspect($scope.sourceProspectId, false);
        }
        this.loadExistentProspect();
    }
})();
