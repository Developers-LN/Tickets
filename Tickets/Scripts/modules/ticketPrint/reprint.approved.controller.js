/**=========================================================
 * Module: ReprintApproveeController.js
 =========================================================*/

(function() {
    'use strict';

    angular
        .module('naut')
        .controller('ReprintApproveeController', ReprintApproveeController);

    ReprintApproveeController.$inject = ['$scope', '$rootScope', '$state', '$stateParams'];
    function ReprintApproveeController($scope, $rootScope, $state, $stateParams) {
        var self = this;
        this.validateProccessData = function (proccess) {
            var error = '', isReq = ' es un campo requerido. <br>';
            if (proccess.comment === undefined) {
                error += 'Nota' + isReq;
            }
            if (error !== '') {
                alertify.showError('Alerta', error);
            }
            return error === '';
        }

        this.saveProccessForm = function () {
            window.loading.show();
            $.ajax({
                type: 'POST',
                dataType: 'json',
                url: $rootScope.serverUrl + 'ticket/workflowApi/saveWorkflowProccess',
                data: $scope.proccess,
                success: function (response) {
                    window.loading.hide();
                    if (response.result === true) {
                        alertify.success(response.message);

                        $rootScope.loadNotifications();
                        $state.go('app.reprintWorkflows');
                    } else {
                        alertify.alert(response.message);
                    }
                }
            });
        }

        $scope.clearProccess = function () {
            return {
                id: 0,
                comment: undefined,
                statu: 0,
                workFlowId: 0
            };
        }

        $scope.approvedProspectProcess = function (statu) {
            if (self.validateProccessData($scope.proccess) === false) {
                return;
            }
            alertify.confirm("&iquest;Desea aprobar este proceso?", function (e) {
                if (e) {
                    $scope.proccess.statu = statu;
                    $scope.proccess.workFlowId = $scope.workflow.id;
                    self.saveProccessForm();
                }
            });
        }

        $scope.rejectProspectProcess = function (statu) {
            if (self.validateProccessData($scope.proccess) === false) {
                return;
            }
            alertify.confirm("&iquest;Desea rechazar este proceso?", function (e) {
                if (e) {
                    $scope.proccess.statu = statu;
                    $scope.proccess.workFlowId = $scope.workflow.id;
                    self.saveProccessForm();
                }
            })
        }

        window.setTimeout(function () {
            $('.slimScrollDiv').slimScroll();
        }, 0);

        $scope.proccess = $scope.clearProccess();

        this.loadWorkflow = function () {
            window.loading.show();
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: $rootScope.serverUrl + 'ticket/workflowApi/getWorkflow?id=' + $stateParams.workflowId,
                success: function (response) {
                    window.loading.hide();
                    if (response.result == true) {
                        $scope.workflow = response.object;
                        $scope.$apply();
                        $rootScope.dataTable();
                    } else {
                        alertify.alert(response.message);
                    }
                }
            });
        }
        this.loadWorkflow();
    }
})();
