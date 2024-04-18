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

        this.validateWinnerData = function (winner) {
            var error = '', isReq = ' es un campo requerido. <br>';
            if (winner.WinnerName === undefined) {
                error += 'Nombre' + isReq;
            }
            if (winner.DocumentType === undefined) {
                error += 'Tipo de documento' + isReq;
            }
            if (winner.DocumentNumber === undefined) {
                error += 'Número de documento' + isReq;
            }
            if (winner.GenderId === undefined) {
                error += 'Sexo' + isReq;
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
                    $scope.awardsHistory = data.awardsHistory;

                    if ($stateParams.winnerId == 0) {
                        self.clearWinner();
                    } else {
                        $scope.winner = data.winner;
                    }
                    if (!$scope.$$phase) {
                        window.setTimeout(function () {
                            $scope.$apply();
                            $rootScope.createSelect2();
                        }, 0);
                    }
                    $rootScope.destroyDataTable();
                    $scope.$apply();
                    $rootScope.dataTable();
                }
            });
        }

        $scope.typeDocument = function () {
            //CEDULA
            if ($scope.winner.DocumentType == 5913) {
                $('#documentNumber').mask("999-9999999-9");
                $scope.maxLenght = 13;
            }
            //RNC
            else if ($scope.winner.DocumentType == 5914) {
                $('#documentNumber').mask("999-99999-9");
                $scope.maxLenght = 11;
            }
            //PASAPORTE
            else {
                $('#documentNumber').unmask();
                $scope.maxLenght = 20;
            }
            $scope.winner.DocumentNumber = undefined;
        }

        this.loadWinnerData();

        $rootScope.createSelect2();
    }
})();
