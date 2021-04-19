/**=========================================================
 * Module: employeeServices
 =========================================================*/

(function () {
	'use strict';

	angular
        .module('naut')
        .service('employeeServices', employeeServices);
	/* @ngInject */
	function employeeServices() {
	    /*jshint validthis:true*/
	    function validateEmpleyeeData(employee) {
	        var error = '', isReq = ' es un campo requerido. <br>';
	        if (employee.Name == '') {
	            error += 'Nombre' + isReq;
	        }
	        if (employee.LastName == '') {
	            error += 'Apellido' + isReq;
	        }
	        if (employee.DocumentNumber == '') {
	            error += 'Cedula' + isReq;
	        }
	        if (!employee.MaritalStatus || employee.MaritalStatus == '') {
	            error += 'Estado civil' + isReq;
	        }
	        if (!employee.Gender || employee.Gender == '') {
	            error += 'Sexo' + isReq;
	        }
	       
	        if (!employee.Province || employee.Province == '') {
	            error += 'Provincia' + isReq;
	        }
	        if (!employee.Town || employee.Town == '') {
	            error += 'Municipio' + isReq;
	        }
	        if (employee.Addres == '') {
	            error += 'Direccion' + isReq;
	        }
	        if (employee.Phone == '') {
	            error += 'Telefono' + isReq;
	        }
	        if (employee.Email == '') {
	            error += 'E-Mail' + isReq;
	        }

	        if (!employee.Department || employee.Department == '') {
	            error += 'Departamento' + isReq;
	        }
	        if (!employee.Office || employee.Office == '') {
	            error += 'Puesto' + isReq;
	        }
	        if (!employee.GroupId || employee.GroupId == '') {
	            error += 'Grupo' + isReq;
	        }
	        if (!employee.Statu || employee.Statu == '') {
	            error += 'Estatus' + isReq;
	        }
	        if (error !== '') {
	            alertify.showError('Alerta', error);
	        }
	        return error === '';
	    }

	    function validateNumenber(number) {
	        if (!number || Number(number) <= 0 || number == '') {
                return false
	        }
	        return true;
	    }

	    this.getEmployeeList = function () {
	        window.loading.show();
	        $.ajax({
	            type: 'GET',
	            contentType: 'application/json; charset=utf-8',
	            url: 'Employee/GetList',
	            success: function (data) {
	                window.loading.hide();
	                if (data.result == true) {
	                    self.$scope.employeeList = data.employees;
	                    self.$scope.provinces = data.provinces;
	                    if (self.$scope.employee.Id > 0) {
	                        self.updateTown();
	                        self.updateDistritalTown();
	                    }
	                    self.$scope.$apply();
	                    self.$rootScope.dataTable();
	                } else {
	                    alertify.alert(data.message);
	                }
	            }
	        });
	    }
	    this.updateTown = function ( ) {
	        self.$scope.towns = [];
	        var selectedProvinceId = Number(self.$scope.employee.Province);
	        $(self.$scope.provinces).each(function (i) {
	            if (self.$scope.provinces[i].Id == selectedProvinceId) {
	                self.$scope.towns = self.$scope.provinces[i].Towns;
                    return
	            }
	        });
	    }
	    this.updateDistritalTown = function ( ) {
	        var selectedProvinceId = Number(self.$scope.employee.Province);
	        var selectedTownId = Number(self.$scope.employee.Town);
	        $(self.$scope.provinces).each(function (i) {
	            if (self.$scope.provinces[i].Id == selectedProvinceId) {
	                $(self.$scope.provinces[i].Towns).each(function (t) {
	                    if (self.$scope.provinces[i].Towns[t].Id == selectedTownId) {
	                        self.$scope.distTowns = self.$scope.provinces[i].Towns[t].DistTowns;
	                    }
	                    return;
	                });
	            }
	        });
	    }

	    this.editEmployees = function (employee) {
	        self.$rootScope.employee = employee;
	        self.$state.go('app.employeeCreate');
	    }
	    this.clearEmployee = function () {
	        return {
	            Id : 0,
	            Name: '',
	            LastName: '',
	            DocumentNumber: '',
	            MaritalStatus: '',
	            Gender: '',
	            Birthday: '',
	            Province: '',
	            Section: '',
	            Town: '',
	            Addres: '',
	            Phone: '',
	            Email: '',
	            Department: '',
	            Office: '',
	            GroupId: 40,
	            Comment: '',
	            Statu: ''
	        };
	    }

	    this.saveEmployeeForm = function () {
	        self.$scope.employee.Birthday = $('.datatime-picker')[0].value;
	        if (validateEmpleyeeData(self.$scope.employee) === false) {
	            return;
	        }
	        $.ajax({
	            type: 'POST',
	            dataType: 'json',
	            url: 'Employee/Create',
	            data: self.$scope.employee,
	            success: function (data) {
	                if (data === true) {
	                    alertify.success('Empleado guardada correctamente!');
	                    self.$state.go('app.employeeList');
	                } else {
	                    alertify.alert(data.message);
	                }
	            }
	        });
	    }

	    this.deleteEmployee = function (employee) {
	        // confirm dialog
	        alertify.confirm("&iquest;Desea borrar este empleado?", function (e) {
	            if (e) {
	                $.ajax({
	                    type: 'POST',
	                    dataType: 'json',
	                    url: 'Employee/Delete',
	                    data: { employeeId: employee.Id },
	                    success: function (data) {
	                        if (data === true) {
	                            self.$rootScope.destroyDataTable();
	                            self.getEmployeeList();
	                            alertify.success('Empleado borrado correctamente!');
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
	        $scope.employee = $rootScope.employee || self.clearEmployee();

	        switch ($state.current.name) {
	            case 'app.employeeList':
	                self.getEmployeeList();
	                $scope.editEmployees = self.editEmployees;
	                $scope.deleteEmployee = self.deleteEmployee;
	                break;
	            case 'app.employeeCreate':
	                self.getEmployeeList();
	                $scope.updateTown = self.updateTown;
	                $scope.updateDistritalTown = self.updateDistritalTown;
	                $('.datatime-picker').datepicker({ });
	                $scope.saveEmployeeForm = self.saveEmployeeForm;
	                break;
	        }
	    }
	}
	employeeServices.$inject = [];
})();
