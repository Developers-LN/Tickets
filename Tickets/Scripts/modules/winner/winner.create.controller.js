/**=========================================================
 * Module: WinnerCreateController.js
 =========================================================*/

(function () {
    'use strict';

    angular
        .module('naut')
        .controller('WinnerCreateController', WinnerCreateController);

    WinnerCreateController.$inject = ['$scope', '$rootScope', '$state', '$stateParams'];
    function WinnerCreateController($scope, $rootScope, $state, $stateParams) {
        var self = this;

        $('#phoneNumber').mask("(999) 999-9999");

        $scope.WinnerDocument = "";
        $scope.WinnerName = "";
        $scope.WinnerPhone = "";
        $scope.AddWinner = 0;
        $scope.GenderId = 0;

        this.validateWinnerData = function (winner) {
            var error = '', isReq = ' es un campo requerido. <br>';
            if (winner.Name === undefined) {
                error += 'Nombre' + isReq;
            }
            if (winner.DocumentType === undefined) {
                error += 'Tipo de documento' + isReq;
            }
            if (winner.DocumentNumber === undefined) {
                error += 'Número de documento' + isReq;
            }
            if (winner.WinnerName === undefined) {
                error += 'Nombre' + isReq;
            }
            if (winner.GenderId === undefined) {
                error += 'Género' + isReq;
            }
            if (winner.Phone === undefined) {
                error += 'Teléfono' + isReq;
            }
            if (error !== '') {
                alertify.showError('Alerta', error);
            }
            return error === '';
        }

        this.clearWinner = function () {
            $scope.winner = {
                Id: 0,
                DocumentType: undefined,
                DocumentNumber: undefined,
                WinnerName: undefined,
                Phone: undefined,
                GenderId: undefined,
                Notes: undefined
            };
        }

        $scope.saveWinnerForm = function () {
            if (self.validateWinnerData($scope.winner) === false) {
                return;
            }
            window.loading.show();
            $.ajax({
                type: 'POST',
                dataType: 'json',
                url: 'Winner/Create',
                data: $scope.winner,
                success: function (data) {
                    window.loading.hide();
                    if (data === true) {
                        alertify.success('Ganador guardado correctamente!');
                        $state.go('app.winnersList');
                    } else {
                        alertify.alert(data.message);
                    }
                }
            });
        }

        this.loadWinnerData = function () {
            window.loading.show();
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: 'Winner/GetWinnerData?winnerId=' + $stateParams.winnerId,
                success: function (data) {
                    window.loading.hide();
                    $scope.documentTypes = data.documentType;
                    $scope.genders = data.genders;

                    if ($stateParams.winnerId == 0) {
                        self.clearWinner();
                    } else {
                        $scope.winner = data.winner;
                    }
                    if (!$scope.$$phase) {
                        window.setTimeout(function () {
                            $scope.$apply();
                        }, 0);
                    }
                }
            });
        }

        this.loadWinnerData();

        $rootScope.createSelect2();
    }
})();
