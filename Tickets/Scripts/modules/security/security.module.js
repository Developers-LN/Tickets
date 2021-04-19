/**=========================================================
 * Module: securityModule
 =========================================================*/

(function () {
	'use strict';

	angular
        .module('naut')
        .service('securityModule', securityModule);
	/* @ngInject */
	function securityModule() {
	    /*jshint validthis:true*/
	    function validateModuleData(module) {
	        var error = '', isReq = ' es un campo requerido. <br>';
	        if (module.Name == '') {
	            error += 'El nombre del rol' + isReq;
	        }
	        if (error !== '') {
	            alertify.showError('Alerta', error);
	        }
	        return error === '';
	    }
	    this.getModuleList = function () {
	        $.ajax({
	            type: 'GET',
	            contentType: 'application/json; charset=utf-8',
	            url: 'Security/GetModuleList',
	            success: function (data) {
	                self.$scope.moduleList = data;
	                self.$scope.$apply();
	                self.$rootScope.dataTable();
	            }
	        });
	    }
	    this.editModule = function (module) {
	        self.$rootScope.module = module;
	        self.$state.go('app.securityModuleCreate');
	    }
	    this.clearModule = function () {
	        return {
	            Id: 0,
	            Name: '',
	            Description: '',
	            CanView: false,
	            CanAdd: false,
	            CanEdit: false,
	            CanSearch: false,
	            CanDelete: false
	        };
	    }

	    this.saveModuleForm = function () {
	        if (validateModuleData(self.$scope.module) === false) {
	            return;
	        }
	        $.ajax({
	            type: 'POST',
	            dataType: 'json',
	            url: 'Security/ModuleCreate',
	            data: self.$scope.module,
	            success: function (data) {
	                if (data === true) {
	                    alertify.success('Modulo guardado correctamente!');
	                    self.$state.go('app.securityModuleList');
	                }
	            }
	        });
	    }

	    this.deleteModule = function () {
	        // confirm dialog
	        alertify.confirm("&iquest;Desea borrar este modulo?", function (e) {
	            if (e) {
	                $.ajax({
	                    type: 'POST',
	                    dataType: 'json',
	                    contentType: 'application/json; charset=utf-8',
	                    url: 'Security/ModuleDelete',
	                    data: self.$scope.office,
	                    success: function (data) {
	                        if (data === true) {
	                            alertify.success('Modulo borrado correctamente!');
	                            self.$state.go('app.configOfficeList');
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
	        $scope.module = $rootScope.module || self.clearModule();

	        switch ($state.current.name) {
	            case 'app.securityModuleList':
	                self.getModuleList();
	                $scope.editModule = self.editModule;
	                break;
	            case 'app.securityModuleCreate':
	                $scope.saveModuleForm = self.saveModuleForm;
	                break;
	        }
	    }
	}
	securityModule.$inject = [];
})();
