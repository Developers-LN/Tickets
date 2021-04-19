/**=========================================================
 * Module: ChangePasswordController.js
 =========================================================*/

(function() {
    'use strict';
    var app = angular.module('naut');
    app.directive("passwordVerify", function () {
        return {
            require: "ngModel",
            scope: {
                passwordVerify: '='
            },
            link: function (scope, element, attrs, ctrl) {
                scope.$watch(function () {
                    var combined;

                    if (scope.$parent.user.Password || ctrl.$viewValue) {
                        combined = scope.$parent.user.Password + '_' + ctrl.$viewValue;
                    }
                    return combined;
                }, function (value) {
                    if (value) {
                        ctrl.$parsers.unshift(function (viewValue) {
                            var origin = scope.$parent.user.Password;
                            if (origin !== viewValue) {
                                ctrl.$setValidity("passwordVerify", false);
                                return undefined;
                            } else {
                                ctrl.$setValidity("passwordVerify", true);
                                return viewValue;
                            }
                        });
                    }
                });
            }
        };
    });

    angular
        .module('naut')
        .controller('ChangePasswordController', ChangePasswordController);
    
    ChangePasswordController.$inject = ['$scope', '$rootScope', '$state'];
    function ChangePasswordController($scope, $rootScope, $state) {
        var self = this;
        $scope.user = {
            CurrentPassword: undefined,
            Password: undefined,
            PasswordRepeat: undefined
        };

        this.validate = function (user) {
            var error = '', isReq = ' es un campo requerido. <br/>';

            if (user.CurrentPassword === undefined) {
                error += 'Antigua Contrase&ntilde;a' + isReq;
            }

            if (user.Password === undefined) {
                error += 'Nueva Contrase&ntilde;a' + isReq;
            }

            if (user.PasswordRepeat === undefined) {
                error += 'Repetir Nueva Contrase&ntilde;a' + isReq;
            }

            if (user.Password !== user.PasswordRepeat) {
                error += 'La nueva contrase&ntilde;a no coinciden. <br/>';
            } else {
                var strongRegex = new RegExp("^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.{8,})");
                if (strongRegex.test(user.Password) === false) {
                    error += 'La contrase&ntilde;a debe contener: mas de 8 caracteres, 1 una mayoscula, 1 numero. <br/>';
                }
            }

            if (error !== '') {
                alertify.showError('Alerta', error);
            }
            return error === '';
        }

        this.changePasswordForm = function () {
            $.ajax({
                type: 'POST',
                dataType: 'json',
                url: 'Security/ChangePassword',
                data: {Password: $scope.user.Password},
                success: function (data) {
                    if (data.result === true) {
                        alertify.success('Contrase&ntilde;a cambiada correctamente!');
                        $state.go('app.dashboard');
                    } else {
                        alertify.success(data.message);
                    }
                    window.loading.hide();
                }
            });
        }

        $scope.passwordRegex = new RegExp("^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.{8,})");
        this.verifyPassword = function () {
            window.loading.show();
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: 'Security/VerifyPassword?password=' + $scope.user.CurrentPassword,
                success: function (data) {
                    if (data.result === true) {
                        self.changePasswordForm();
                    } else {
                        alertify.alert('La contrase&ntilde;a actual no es correcta');
                        window.loading.hide();
                    }
                }
            });
        }
        $scope.chagePassword = function () {
            if (self.validate($scope.user) === false) {
                return;
            }
            self.verifyPassword();
        }
    }
})();
