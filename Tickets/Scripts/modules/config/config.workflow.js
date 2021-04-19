/**=========================================================
 * Module: configWorkflow
 =========================================================*/

(function () {
	'use strict';

	angular
        .module('naut')
        .service('configWorkflow', configWorkflow);
	/* @ngInject */
	function configWorkflow() {
	    /*jshint validthis:true*/
	    function validateWorkflowTypeData(workflowType) {
	        var error = '', isReq = ' es un campo requerido. <br>';
	        if (workflowType.Name == '') {
	            error += 'El nombre' + isReq;
	        }
	        if (workflowType.Statu == '') {
	            error += 'El estado' + isReq;
	        }
	        if (error !== '') {
	            alertify.showError('Alerta', error);
	        }
	        return error === '';
	    }
	    function validateWorkflowTypeUserData(workflowTypeUser) {
	        var error = '', isReq = ' es un campo requerido. <br>';
	        if (workflowTypeUser.UserId == '') {
	            error += 'El usuario' + isReq;
	        }
	        if (workflowTypeUser.OrderApproval == '' || !workflowTypeUser.OrderApproval) {
	            error += 'El orden de aproaaci&#243;n' + isReq;
	        }
	        if (workflowTypeUser.TypeApproval == '') {
	            error += 'El tipo de aprobaci&#243;n' + isReq;
	        }
	        if (error !== '') {
	            alertify.showError('Alerta', error);
	        }
	        return error === '';
	    }
	    this.getWorkflowTypeList = function () {
	        $.ajax({
	            type: 'GET',
	            contentType: 'application/json; charset=utf-8',
	            url: 'Config/GetWorkflowTypeList',
	            success: function (data) {
	                self.$scope.workflowTypeList = data;
	                self.$scope.$apply();
	                self.$rootScope.dataTable();
	            }
	        });
	    }
	    this.getWorkflowTypeUserList = function () {
	        if (self.$scope.workflowType.Id === 0) {
	            return;
	        }
	        $.ajax({
	            type: 'GET',
	            contentType: 'application/json; charset=utf-8',
	            url: 'Config/GetWorkflowTypeUserList?workflowTypeId=' + self.$scope.workflowType.Id,
	            success: function (data) {
	                self.$scope.workflowTypeUserList = data;
	                self.$scope.$apply();
	                self.$rootScope.dataTable();
	            }
	        });
	    }

	    this.editWorkflowType = function (workflowType) {
	        self.$rootScope.workflowType = workflowType;
	        self.$state.go('app.configWorkflowTypeCreate');
	    }
	    this.editWorkflowTypeUser = function (workflowTypeUser) {
	        self.$rootScope.workflowTypeUser = workflowTypeUser;
	        self.$state.go('app.configWorkflowTypeUserCreate');
	    }
	    this.managerWorkflowUser = function (workflowType) {
	        self.$rootScope.workflowType = workflowType;
	        self.$state.go('app.configWorkflowTypeUserList');
	    }
	    this.clearWorkflowTypeUser = function () {
	        return {
	            Id: 0,
	            WorkflowTypeId: self.$scope.workflowType.Id,
	            OrderApproval: '',
	            TypeApproval: '',
	            UserId: ''
	        };
	    }
	    this.clearWorkflowType = function () {
	        return {
	            Id: 0,
	            Name: '',
	            Description: '',
                Statu: ''
	        };
	    }

	    this.saveWorkflowTypeForm = function () {
	        if (validateWorkflowTypeData(self.$scope.workflowType) === false) {
	            return;
	        }
	        $.ajax({
	            type: 'POST',
	            dataType: 'json',
	            url: 'Config/WorkflowTypeCreate',
	            data: self.$scope.workflowType,
	            success: function (data) {
	                if (data === true) {
	                    alertify.success('Tipo de flujo de trabajo guardado correctamente!');
	                    self.$state.go('app.configWorkflowTypeList');
	                }
	            }
	        });
	    }

	    this.saveWorkflowTypeUserForm = function () {
	        if (validateWorkflowTypeUserData(self.$scope.workflowTypeUser) === false) {
	            return;
	        }
	        $.ajax({
	            type: 'POST',
	            dataType: 'json',
	            url: 'Config/WorkflowTypeUserCreate',
	            data: self.$scope.workflowTypeUser,
	            success: function (data) {
	                if (data === true) {
	                    alertify.success('Usuario guardado correctamente!');
	                    self.$state.go('app.configWorkflowTypeUserList');
	                }
	            }
	        });
	    }

	    this.deleteWorkflowType = function (workflowType) {
	        // confirm dialog
	        alertify.confirm("&iquest;Desea borrar este tipo de flujo de trabajo?", function (e) {
	            if (e) {
	                $.ajax({
	                    type: 'POST',
	                    dataType: 'json',
	                    url: 'Config/WorkflowTypeDelete',
	                    data: { workflowTypeId: workflowType.Id },
	                    success: function (data) {
	                        if (data === true) {
	                            self.$rootScope.destroyDataTable();
	                            self.getWorkflowTypeList();
	                            alertify.success('Tipo de flujo de trabajo borrado correctamente!');
	                        }
	                    }
	                });
	            }
	        });
	    }
	    this.deleteWorkflowTypeUser = function (workflowTypeUser) {
	        // confirm dialog
	        alertify.confirm("&iquest;Desea borrar este Usuario?", function (e) {
	            if (e) {
	                $.ajax({
	                    type: 'POST',
	                    dataType: 'json',
	                    url: 'Config/WorkflowTypeUserDelete',
	                    data: { workflowTypeUserId: workflowTypeUser.Id },
	                    success: function (data) {
	                        if (data === true) {
	                            self.$rootScope.destroyDataTable();
	                            self.getWorkflowTypeUserList();
	                            alertify.success('Usuario borrado correctamente!');
	                        }
	                    }
	                });
	            }
	        });
	    }

	    var self = this;
	    this.init = function ($rootScope, $scope, $state) {
	        self.$rootScope = $rootScope;
	        self.$scope = $scope;
	        self.$state = $state;

	        switch ($state.current.name) {
	            case 'app.configWorkflowTypeList':
	                $scope.workflowType = $rootScope.workflowType || self.clearWorkflowType();
	                self.getWorkflowTypeList();
	                $scope.managerWorkflowUser = self.managerWorkflowUser;
	                $scope.editWorkflowType = self.editWorkflowType;
	                $scope.deleteWorkflowType = self.deleteWorkflowType;
	                break;
	            case 'app.configWorkflowTypeCreate':
	                $scope.workflowType = $rootScope.workflowType || self.clearWorkflowType();
	                $scope.saveWorkflowTypeForm = self.saveWorkflowTypeForm;
	                break;
	            case 'app.configWorkflowTypeUserList':
	                if (!$rootScope.workflowType) {
	                    self.$state.go('app.configWorkflowTypeList');
	                    return;
	                }
	                $scope.workflowTypeUser = $rootScope.workflowTypeUser || self.clearWorkflowTypeUser();
	                $scope.workflowType = $rootScope.workflowType
	                $scope.editWorkflowTypeUser = self.editWorkflowTypeUser;
	                $scope.deleteWorkflowTypeUser = self.deleteWorkflowTypeUser;
	                self.getWorkflowTypeUserList();
                    break;
	            case 'app.configWorkflowTypeUserCreate':
	                $scope.workflowTypeUser = $rootScope.workflowTypeUser || self.clearWorkflowTypeUser();
	                if (!$rootScope.workflowType) {
	                    self.$state.go('app.configWorkflowTypeList');
	                    return;
	                }
	                $scope.saveWorkflowTypeUserForm = self.saveWorkflowTypeUserForm;
                    break;
	        }
	    }
	}
	configWorkflow.$inject = [];
})();
