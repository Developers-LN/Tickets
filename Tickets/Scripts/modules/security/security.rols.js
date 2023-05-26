/**=========================================================
 * Module: securityRol
 =========================================================*/

(function () {
	'use strict';

	angular
		.module('naut')
		.service('securityRol', securityRol);
	/* @ngInject */
	function securityRol() {
		/*jshint validthis:true*/

		/*Start RoleList*/
		this.getRolList = function () {
			$.ajax({
				type: 'GET',
				contentType: 'application/json; charset=utf-8',
				url: 'Security/GetRolList',
				success: function (data) {
					self.$scope.rolList = data;
					self.$scope.$apply();
					self.$rootScope.dataTable();
				}
			});
		}
		this.editRol = function (rol) {
			self.$rootScope.rol = rol;
			self.$state.go('app.securityRolCreate');
		}
		this.manageOffice = function (rol) {
			self.$rootScope.rol = rol;
			self.$state.go('app.securityOfficeListInRol');
		}
		this.manageUsers = function (rol) {
			self.$rootScope.rol = rol;
			self.$state.go('app.securityUserListInRol');
		}
		this.manageModule = function (rol) {
			self.$rootScope.rol = rol;
			self.$state.go('app.securityModuleListInRol');
		}
		/*End RolList*/

		/*Start Create/Edit Rol*/
		function validateRolData(rol) {
			var error = '', isReq = ' es un campo requerido. <br>';
			if (rol.RoleName == '') {
				error += 'El nombre del rol' + isReq;
			}
			if (error !== '') {
				alertify.showError('Alerta', error);
			}
			return error === '';
		}
		this.clearRol = function () {
			return {
				RoleId: 0,
				RoleName: '',
				Description: ''
			};
		}
		this.saveRolForm = function () {
			if (validateRolData(self.$scope.rol) === false) {
				return;
			}
			$.ajax({
				type: 'POST',
				dataType: 'json',
				url: 'Security/RolCreate',
				data: self.$scope.rol,
				success: function (data) {
					if (data === true) {
						alertify.success('Rol guardado correctamente!');
						self.$state.go('app.securityRolList');
					}
				}
			});
		}
		/*End Create/Edit Rol*/

		/*Start Manage UserList*/
		this.getUserList = function () {
			if (self.$scope.rol.RoleId === 0) {
				return;
			}
			$.ajax({
				type: 'GET',
				contentType: 'application/json; charset=utf-8',
				url: 'Security/GetUserListForRol?rolId=' + self.$scope.rol.RoleId,
				success: function (data) {
					self.$scope.userList = data;
					self.$scope.$apply();
					self.$rootScope.dataTable();
				}
			});
		}
		this.manageRolStatu = function (rolId, userId, statu) {
			var rolStatu = {
				RolId: rolId,
				UserId: userId,
				Statu: statu
			};
			$.ajax({
				type: 'POST',
				dataType: 'json',
				url: 'Security/ManageRolInUserStatu',
				data: rolStatu,
				success: function (data) {
					var addButton = $('#AddElementButton' + userId);
					var removeButton = $('#RemoveElementButton' + userId);
					var elementStatuLabel = $('#ElementStatuLabel' + userId);
					if (data === true) {
						addButton.addClass('hide');
						removeButton.removeClass('hide');
						alertify.log('Usuario asignado correctamente!');
						elementStatuLabel[0].innerHTML = 'Asignado';
					} else {
						addButton.removeClass('hide');
						removeButton.addClass('hide');
						alertify.log('Usuario desasigando correctamente!');
						elementStatuLabel[0].innerHTML = 'No asignado';
					}
				}
			});
		}
		/*End Manage UserList*/

		/*Start Manage ModuleList*/
		this.getModuleList = function () {
			if (self.$scope.rol.RoleId === 0) {
				return;
			}
			$.ajax({
				type: 'GET',
				contentType: 'application/json; charset=utf-8',
				url: 'Security/GetModuleListInRol?rolId=' + self.$scope.rol.RoleId,
				success: function (data) {
					self.$scope.modueList = data;
					self.$scope.$apply();
					self.$rootScope.dataTable(false);
				}
			});
		}
		this.manageModuleStatu = function (rolId) {
			var table = $('.dataTableGrid').DataTable();
			table.search('')
				.columns().search('')
				.draw();
			var moduleList = [];
			$(self.$scope.modueList).each(function (i, module) {
				moduleList.push({
					RolId: rolId,
					ModuleId: module.Id,
					CanView: $('#RolModuleViewCheckBox' + module.Id)[0].checked,
					CanEdit: $('#RolModuleEditCheckBox' + module.Id)[0].checked,
					CanDelete: $('#RolModuleDeleteCheckBox' + module.Id)[0].checked,
					CanAdd: $('#RolModuleAddCheckBox' + module.Id)[0].checked,
					CanSearch: $('#RolModuleSearchCheckBox' + module.Id)[0].checked
				});
			});

			$.ajax({
				type: 'POST',
				dataType: 'json',
				contentType: 'application/json; charset=utf-8',
				url: 'Security/SaveModuleForRol',
				data: JSON.stringify({ 'models': moduleList }),
				success: function (data) {
					self.$state.go('app.securityRolList');
					alertify.log('Permisos guardado correctamente!');
				}
			});
		}
		/*End Manage ModuleList*/

		this.updateModuleStatu = function (rolId, moduleId) {
			var table = $('.dataTableGrid').DataTable();
			table.search('')
				.columns().search('')
				.draw();
			var moduleList = [];
			$(self.$scope.modueList).each(function (i, module) {
				moduleList.push({
					RolId: rolId,
					ModuleId: module.Id,
					CanView: $('#RolModuleViewCheckBox' + module.Id)[0].checked,
					CanEdit: $('#RolModuleEditCheckBox' + module.Id)[0].checked,
					CanDelete: $('#RolModuleDeleteCheckBox' + module.Id)[0].checked,
					CanAdd: $('#RolModuleAddCheckBox' + module.Id)[0].checked,
					CanSearch: $('#RolModuleSearchCheckBox' + module.Id)[0].checked
				});
			});

			moduleList = moduleList.filter(function (module) {
				return (module.ModuleId == moduleId) == true;
			});

			$.ajax({
				type: 'POST',
				dataType: 'json',
				contentType: 'application/json; charset=utf-8',
				url: 'Security/SaveModuleForRol',
				data: JSON.stringify({ 'models': moduleList }),
				success: function (data) {
					/*self.$state.go('app.securityRolList');*/
					alertify.log('Permisos guardado correctamente!');
				}
			});
		}

		/*Start Manage OfficeList*/
		this.getOfficeList = function () {
			if (self.$scope.rol.RoleId === 0) {
				return;
			}
			$.ajax({
				type: 'GET',
				contentType: 'application/json; charset=utf-8',
				url: 'Security/GetOfficeListInRol?rolId=' + self.$scope.rol.RoleId,
				success: function (data) {
					self.$scope.officeList = data;
					self.$scope.$apply();
					self.$rootScope.dataTable();
				}
			});
		}
		this.manageOfficeStatu = function (rolId, officeId, statu) {
			var rolStatu = {
				RolId: rolId,
				OfficeId: officeId,
				Statu: statu
			};
			$.ajax({
				type: 'POST',
				dataType: 'json',
				url: 'Security/ManageOfficeInInRolStatu',
				data: rolStatu,
				success: function (data) {
					var addButton = $('#AddElementButton' + officeId);
					var removeButton = $('#RemoveElementButton' + officeId);
					var elementStatuLabel = $('#ElementStatuLabel' + officeId);
					if (data === true) {
						addButton.addClass('hide');
						removeButton.removeClass('hide');
						alertify.log('Sucursal asignada correctamente!');
						elementStatuLabel[0].innerHTML = 'Asignada';
					} else {
						addButton.removeClass('hide');
						removeButton.addClass('hide');
						alertify.log('Sucursal desasignada correctamente!');
						elementStatuLabel[0].innerHTML = 'No asignada';
					}
				}
			});
		}
		/*End Manage OfficeList*/

		var self = this;
		this.init = function ($rootScope, $scope, $state) {
			self.$rootScope = $rootScope;
			self.$scope = $scope;
			self.$state = $state;

			$scope.rol = $rootScope.rol || self.clearRol();
			switch ($state.current.name) {
				case 'app.securityRolList':
					self.getRolList();
					$scope.editRol = self.editRol;
					$scope.manageUsers = self.manageUsers;
					$scope.manageModule = self.manageModule;
					$scope.manageOffice = self.manageOffice;
					break;
				case 'app.securityRolCreate':
					$scope.saveRolForm = self.saveRolForm;
					break;
				case 'app.securityUserListInRol':
					self.getUserList();
					$scope.manageRolStatu = self.manageRolStatu;
					break;
				case 'app.securityModuleListInRol':
					self.getModuleList();
					$scope.manageModuleStatu = self.manageModuleStatu;
					$scope.updateModuleStatu = self.updateModuleStatu;
					break;
				case 'app.securityOfficeListInRol':
					self.getOfficeList();
					$scope.manageOfficeStatu = self.manageOfficeStatu;
					break;
			}
		}
	}
	securityRol.$inject = [];
})();
