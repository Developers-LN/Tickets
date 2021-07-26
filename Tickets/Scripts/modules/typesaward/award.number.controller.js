/**=========================================================
 * Module: AwardNumberController.js
 =========================================================*/

(function () {
    'use strict';

    angular
        .module('naut')
        .controller('AwardNumberController', AwardNumberController);

    AwardNumberController.$inject = ['$scope', '$state', '$rootScope', '$stateParams'];
    function AwardNumberController($scope, $state, $rootScope, $stateParams) {
        $scope.findedAward = false;
        $scope.awardNumber = {
            RaffleId: '',
            Number: ''
        };
        $scope.numberFound = false;
        var self = this;

        function validateData(awardNumber) {
            var error = '', isReq = ' es un campo requerido. <br>';

            if (validateNumenber(awardNumber.RaffleId) === false) {
                error += 'El Sorteo' + isReq;
            }
            if (validateNumenber(awardNumber.Number) === false) {
                error += 'El Billete' + isReq;
            }
            if (error !== '') {
                alertify.showError('Alerta', error);
            }
            return error === '';
        }

        function validateNumenber(number) {
            if (Number(number) < 0 || number === '') {
                return false;
            }
            return true;
        }

        this.loadIdentifyData = function (numberId) {
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: $rootScope.serverUrl + 'ticket/ticketAllocationApi/getTicketNumberDetails?numberId=' + numberId,
                success: function (response) {
                    response.object.Transactions = response.object.Transactions.map(function (transaction) {
                        transaction.Date = new Date(transaction.Date);
                        return transaction;
                    });
                    var returnedFractions = 0;
                    response.object.returnes.forEach(function (returned) {
                        returnedFractions += (returned.FractionTo - returned.FractionFrom + 1);
                    });
                    $scope.number = response.object;
                    $scope.numberFound = true;
                    $scope.caducado = response.object.Caducado;
                    $scope.showTotals();
                    $rootScope.destroyDataTable();
                    $scope.$apply();
                    $rootScope.dataTable();
                }
            });
        }

        $.ajax({
            type: 'GET',
            contentType: 'application/json; charset=utf-8',
            url: 'TypesAward/GetAwardNumberData',
            success: function (data) {
                window.loading.hide();
                if (data !== null) {
                    $scope.raffles = data.raffles;
                }
                window.setTimeout(function () {
                    $scope.$apply();
                    $rootScope.createSelect2();
                }, 0);
            }
        });

        $scope.Keyup = function (e) {
            if (e.which == 13) {
                $scope.searchNumber();
            }
        }

        $scope.totalFraction = 0;
        $scope.totalValue = 0;
        $scope.totalGeneral = 0;

        $scope.showTotals = function () {
            $scope.totalFraction = 0;
            $scope.totalValue = 0;
            $scope.totalGeneral = 0;
            $scope.number.awards.forEach(function (award) {
                var fractions = award.FractionTo - award.FractionFrom + 1;
                $scope.totalFraction += fractions;
                $scope.totalValue += award.AwardValue / award.MaxFraction;
                $scope.totalGeneral += ((award.FractionTo - award.FractionFrom) + 1) * (award.AwardValue / award.MaxFraction);
            });
        }

        $scope.paymentCount = 0;
        $scope.searchNumber = function () {
            //$scope.findedAward = true;
            //$scope.numberFound = false;
            if (validateData($scope.awardNumber) === false) {
                return;
            }
            window.loading.show();
            $scope.findedAward = true;
            $scope.numberFound = false;
            $.ajax({
                type: 'POST',
                dataType: 'json',
                url: 'TypesAward/AwardNumber',
                data: $scope.awardNumber,
                success: function (data) {
                    window.loading.hide();
                    if (data.result === true) {
                        self.loadIdentifyData(data.numberId);
                    } else {
                        alertify.alert(data.message);
                        $scope.findedAward = false;
                    }
                    $scope.$apply();
                }
            });
        }
    }
})();
