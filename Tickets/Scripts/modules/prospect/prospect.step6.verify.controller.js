/**=========================================================
 * Module: CreationTypeController.js
 =========================================================*/

(function () {
    'use strict';

    angular
        .module('naut')
        .controller('ProspectVerifyController', ProspectVerifyController);

    ProspectVerifyController.$inject = ['$scope', '$rootScope', '$state', 'editableOptions', '$sce', 'editableThemes', '$stateParams'];
    function ProspectVerifyController($scope, $rootScope, $state, editableOptions, $sce, editableThemes, $stateParams) {
        editableOptions.theme = 'bs3';
        editableThemes.bs3.inputClass = 'font-control input-sm';
        editableThemes.bs3.buttonsClass = 'btn-sm';
        editableThemes.bs3.submitTpl = '<button type="submit" class="btn btn-success"><span class="fa fa-check"></span></button>';
        editableThemes.bs3.cancelTpl = '<button type="button" class="btn btn-default" ng-click="$form.$cancel()">' +
            '<span class="fa fa-times text-muted"></span>' +
            '</button>';
        var self = this;
        this.loadProspectData = function (prospect) {
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
                    $scope.status = statuResponse[0].object;
                }
                if (printResponse[1] == 'success') {
                    $scope.prints = printResponse[0].object;
                }
                $scope.prospect = prospect;
                window.setTimeout(function () {
                    $scope.$apply();
                }, 100);
            });
            window.setTimeout(function () {
                $scope.$apply();
            }, 100);
        }
        $scope.viewMode = false;
        $rootScope.$watch('activeStep', function () {
            if ($rootScope.activeStep == 5) {
                self.loadProspectData($rootScope.prospect);
            }
        });
        if ($stateParams.prospectId > 0) {
            $scope.viewMode = true;
            window.loading.show();
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: 'ticket/prospectApi/getProspect?id=' + $stateParams.prospectId,
                success: function (response) {
                    window.loading.hide();
                    if (response.result == true) {
                        self.loadProspectData(response.object);
                    } else {
                        alertify.alert(response.message);
                    }
                }
            });
        }

        this.getDescriptionById = function (id, list) {
            var detail = 'Selecciones';
            if (!list) {
                return detail;
            }
            for (var i = 0; i < list.length; i++) {
                if (list[i].id == id) {
                    detail = list[i].nameDetail;
                }
            }
            return detail;
        }

        $scope.showGroupDescription = function () {
            try {
                return self.getDescriptionById($scope.prospect.groupId, $scope.groups);
            } catch (e) { }
        }

        $scope.showPrintTechniquesDescription = function () {
            try {
                return self.getDescriptionById($scope.prospect.impresionType, $scope.prints);
            } catch (e) { }
        }

        $scope.showStatuDescription = function () {
            try {
                return self.getDescriptionById($scope.prospect.statu, $scope.status);
            } catch (e) { }
        }

        $scope.topGoToStep = function (step) {
            if (step < $scope.activeStep && $scope.viewMode !== true) {
                $scope.activeStep = step;
            }
        }

        $scope.getProspectList = function () {
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: 'Prospect/Prospects',
                success: function (data) {
                    $scope.prospectList = data.prospects;
                    $scope.$apply();
                    $rootScope.dataTable();
                }
            });
        }

        $scope.showExpirateDate = function () {
            var data = $scope.prospect.ExpirateDate;
            return data;
        }

        $scope.showTotal = function () {
            var total = 0;
            $scope.prospect.awards.forEach(function (award) {
                total += award.value * award.quantity;
            });
            return total;
        }

        $scope.goToBackStep = function () {
            $rootScope.goStep5($rootScope.prospect);
        }
        $scope.saveProspect = function () {
            $rootScope.saveProspectForm($scope.prospect);
        }
    }
})();
