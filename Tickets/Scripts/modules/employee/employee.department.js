/**=========================================================
 * Module: departmentServices
 =========================================================*/

(function () {
	'use strict';

	angular
        .module('naut')
        .service('departmentServices', departmentServices);
	/* @ngInject */
	function departmentServices() {
	    /*jshint validthis:true*/
	    function validateDepartmentData(department) {
	        var error = '', isReq = ' es un campo requerido. <br>';
	        if (department.NameDetail == '') {
	            error += 'Nombre' + isReq;
	        }
	        if (error !== '') {
	            alertify.showError('Alerta', error);
	        }
	        return error === '';
	    }
	    this.getDepartmentList = function () {
	        $.ajax({
	            type: 'GET',
	            contentType: 'application/json; charset=utf-8',
	            url: 'Employee/GetDepartmentList',
	            success: function (data) {
	                self.$scope.departmentList = data;
	                self.$scope.$apply();
	                self.$rootScope.dataTable();
	            }
	        });
	    }

	    this.editDepartment = function (department) {
	        self.$rootScope.department = department;
	        self.$state.go('app.departmentCreate');
	    }

	    this.clearDepartment = function () {
	        return {
	            Id: 0,
	            IdGroup: '',
	            NameGroup: '',
	            IdDetail: '',
	            NameDetail: '',
	            Description: '',
	            Statu: ''
	        };
	    }

	    this.saveDepartmentForm = function () {
	        if (validateDepartmentData(self.$scope.department) === false) {
	            return;
	        }
	        $.ajax({
	            type: 'POST',
	            dataType: 'json',
	            url: 'Employee/DepartmentCreate',
	            data: self.$scope.department,
	            success: function (data) {
	                if (data === true) {
	                    alertify.success('Departamento guardada correctamente!');
	                    self.$state.go('app.departmentList');
	                }
	            }
	        });
	    }

	    this.deleteDepartment = function (department) {
	        // confirm dialog
	        alertify.confirm("&iquest;Desea borrar este departamento?", function (e) {
	            if (e) {
	                $.ajax({
	                    type: 'POST',
	                    dataType: 'json',
	                    url: 'Employee/DepartmentDelete',
	                    data: { departmentId: department.Id },
	                    success: function (data) {
	                        if (data === true) {
	                            self.$rootScope.destroyDataTable();
	                            self.getDepartmentList();
	                            alertify.success('Departamento borrado correctamente!');
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
	        $scope.department = $rootScope.department || self.clearDepartment();

	        switch ($state.current.name) {
	            case 'app.departmentList':
	                self.getDepartmentList();
	                $scope.editDepartment = self.editDepartment;
	                $scope.deleteDepartment = self.deleteDepartment;
	                break;
	            case 'app.departmentCreate':
	                $scope.saveDepartmentForm = self.saveDepartmentForm;
	                break;
	        }
	    }
	}
	departmentServices.$inject = [];
})();
