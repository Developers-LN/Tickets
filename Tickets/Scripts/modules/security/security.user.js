/**=========================================================
 * Module: securityUser
 =========================================================*/

(function () {
	'use strict';

	angular
        .module('naut')
        .service('securityUser', securityUser);
	/* @ngInject */
	function securityUser() {
	    /*jshint validthis:true*/
	    function verifyPassowrd(user){
	        var error =  '';
	        if (user.Password === user.PasswordRepeat) {
	            if (!user.Password) {
	                error += 'La contrase&ntilde;a no es valida. <br>';
	            } else {
	                var strongRegex = new RegExp("^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.{8,})");
	                if (strongRegex.test(user.Password) === false) {
	                    error += 'La contrase&ntilde;a debe contener: mas de 8 caracteres, 1 una mayoscula, 1 numero. <br/>';
	                }
	            }
	        } else {
	            error += 'La contrase&ntilde;a no son iguales. <br>';
	        }
	        return error;
	    }
	    function validateUserData(user) {
	        var error = '', isReq = ' es un campo requerido. <br>';
	        if (user.Name == '') {
	            error += 'El nombre de usuario' + isReq;
	        }
	        if ($.inArray(user.Name, self.$scope.userNameList) >= 0 && user.Id === 0) {
	            error += 'Existe un usuario reguistrado con el nombre ' + user.Name + '. <br>';
	        }
	        if( !(user.Id > 0 && user.Password === '' && user.PasswordRepeat === '')){
	            error += verifyPassowrd(user);
	        }
	        if (!user.Statu || user.Statu== '' ) {
	            error += 'El estado' + isReq;
	        }
	        if (error !== '') {
	            alertify.showError('Alerta', error);
	        }
	        return error === '';
	}

	    this.getUserList = function () {
	        $.ajax({
	            type: 'GET',
	            contentType: 'application/json; charset=utf-8',
	            url: 'Security/GetUserList',
				success: function (data) {
	                self.$scope.userList = data.users;
	                self.$scope.employes = data.employes;
	                self.$scope.userNameList = [];
	                $(data).each(function (i, e) {
	                    self.$scope.userNameList.push(e.Name);
	                });
	                if (self.$state.current.name === 'app.securityUserList') {
	                    self.$scope.$apply();
	                    self.$rootScope.dataTable();
	                }
	            }
	        });
	    }
	    this.editUser = function (user) {
	        self.$rootScope.createUser = user;
	        self.$state.go('app.securityUserCreate');
	    }
	    this.clearUser = function () {
	        return {
	            Id: 0,
	            Name: '',
	            Password: '',
	            PasswordRepeat: '',
	            Statu: ''
	        };
	    }
	    this.getRolList = function () {
	        self.$scope.user.Id = self.$scope.user.Id || 0;
	        if (self.$scope.user.Id === 0) {
	            alertify.alert('Seleccione un usuario!');
	            self.$state.go('app.securityUserList');
	            return;
	        }
	        $.ajax({
	            type: 'GET',
	            contentType: 'application/json; charset=utf-8',
	            url: 'Security/GetRolListForUser?userId=' + self.$scope.user.Id,
	            success: function (data) {
	                self.$scope.rolList = data;
	                self.$scope.$apply();
	                self.$rootScope.dataTable();
	            }
	        });
	    }
	    this.manageRols = function (user) {
	        self.$rootScope.user = user;
	        self.$state.go('app.securityRolListInUser');
	    }
	    this.saveUserForm = function () {
	        if (validateUserData(self.$scope.user) === false) {
	            return;
	        }
	        $.ajax({
	            type: 'POST',
	            dataType: 'json',
	            url: 'Security/UserCreate',
	            data: self.$scope.user,
	            success: function (data) {
	                if (data === true) {
	                    alertify.success('Usuario guardado correctamente!');
	                    self.$state.go('app.securityUserList');
	                }
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
	                var addButton = $('#AddElementButton' + rolId);
	                var removeButton = $('#RemoveElementButton' + rolId);
	                var elementStatuLabel = $('#ElementStatuLabel' + rolId);
	                if (data === true) {
	                    addButton.addClass('hide');
	                    removeButton.removeClass('hide');
	                    alertify.log('Rol agregado correctamente!');
	                    elementStatuLabel[0].innerHTML = 'Asignado';
	                } else {
	                    addButton.removeClass('hide');
	                    removeButton.addClass('hide');
	                    alertify.log('Rol Removido correctamente!');
	                    elementStatuLabel[0].innerHTML = 'No asignado';
	                }
	            }
	        });
	    }
	    var self = this;

	    this.init = function ($rootScope, $scope, $state) {
	        self.$rootScope = $rootScope;
	        self.$scope = $scope;
	        self.$state = $state;

	        $scope.passwordRegex = new RegExp("^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.{8,})");
	        $scope.user = $rootScope.createUser || self.clearUser();
	        switch ($state.current.name) {
	            case 'app.securityUserList':
	                self.getUserList( );
	                $scope.editUser = self.editUser;
	                $scope.manageRols = self.manageRols;
	                break;
	            case 'app.securityUserCreate':
	                self.getUserList();
	                if ($scope.user.Id > 0) {
	                    $('#UserName').attr('disabled', 'disabled');
	                } else {
	                    $('#UserName').removeAttr('disabled', 'disabled');
	                }
	                $scope.saveUserForm = self.saveUserForm;
	                break;
	            case 'app.securityRolListInUser':
	                $scope.user = $rootScope.user;
	                self.getRolList();
	                $scope.manageRolStatu = self.manageRolStatu;
	                break;
	        }
	    };
	}
	securityUser.$inject = [];
})();
