/**=========================================================
 * Module: OpenReturnedController.js
 =========================================================*/

(function() {
    'use strict';

    angular
        .module('naut')
        .controller('OpenReturnedController', OpenReturnedController);

    OpenReturnedController.$inject = ['$scope', '$rootScope', '$state'];
    function OpenReturnedController($scope, $rootScope, $state) {
        /*jshint validthis:true*/
        var self = this;

        this.clearData = function () {
            return {
                RaffleId: undefined,
                Date: undefined,
                Time: undefined,
                Note: undefined,
                EndReturnedDate: undefined
            };
        }

        this.getData = function () {
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: 'Raffle/GetOpenReturned',
                success: function (data) {
                    $scope.raffles = data.raffles;
                    $scope.$apply();
                    $rootScope.createSelect2();
                }
            });
        }


        this.validateData = function(open) {
            var error = '', isReq = ' es un campo requerido. <br>';
            if (open.RaffleId === undefined) {
                error += 'Sorteo' + isReq;
            }
            if (open.EndReturnedDate === undefined) {
                error += 'Fecha fin devoluci&oacute;n' + isReq;
            }
            if (open.Note === undefined) {
                error += 'Observaci&oacute;n' + isReq;
            }
            if (error !== '') {
                alertify.showError('Alerta', error);
            }
            return error === '';
        }

        $scope.saveForm = function () {
            try {
                if ($scope.open.Date === undefined || $scope.open.Time === undefined) {
                    $scope.open.EndReturnedDate = undefined;
                } else {
                    $scope.open.EndReturnedDate = $rootScope.parseDate($scope.open.Date, $scope.open.Time).toJSON();
                }
            } catch (e) { }
            if (self.validateData($scope.open) === false) {
                return;
            }
            window.loading.show();
            $.ajax({
                type: 'POST',
                dataType: 'json',
                url: 'Raffle/OpenReturned',
                data: $scope.open,
                success: function (data) {
                    window.loading.hide();
                    if (data.result === true) {
                        alertify.success(data.message);
                        $state.go('app.dashboard');
                    } else {
                        alertify.alert(data.message);
                    }
                }
            });
        }

        self.getData();
        $scope.open = self.clearData();
    }
})();
