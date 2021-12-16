/**=========================================================
 * Module: DeliveryController.js
 =========================================================*/

(function () {
    'use strict';

    angular
        .module('naut')
        .controller('CashAdvanceController', CashAdvanceController);

    CashAdvanceController.$inject = ['$scope', '$state', '$rootScope', '$stateParams'];
    function CashAdvanceController($scope, $state, $rootScope, $stateParams) {
        var self = this;
        $scope.identifyBachId = $stateParams.raffleAwardId;
        $scope.creditNote = {
            Id: 0,
            ClientId: undefined,
            RaffleId: undefined,
            TotalCash: undefined,
            Concepts: undefined,
            IdentifyBaches: [{ Id: $stateParams.raffleAwardId }]
        };
        $scope.raffleAwardId = $stateParams.raffleAwardId;
        function validateData(creditNote) {
            var error = '', isReq = ' es un campo requerido. <br>';

            if (creditNote.ClientId === undefined) {
                error += 'El Cliente' + isReq;
            }
            if (creditNote.RaffleId === undefined) {
                error += 'El sorteo' + isReq;
            }
            if (creditNote.TotalCash === undefined) {
                error += 'El Monto Total' + isReq;
            }
            if (creditNote.Concepts === undefined) {
                error += 'El Concepto' + isReq;
            }
            if (error !== '') {
                alertify.showError('Alerta', error);
            }
            return error === '';
        }

        $scope.totalpayment = 0;
        $scope.totalGeneral = 0;

        $scope.totalRestant = 0;
        $scope.totalPercent = 0;
        $scope.totalAwards = 0;

        this.showTotal = function () {
            var totalFraction = 0, totalNumber = 0, totalValue = 0;
            $scope.identifyBach.IdentifyNumbers.forEach(function (number) {
                var fractions = (number.RaffleAward.FractionTo - number.RaffleAward.FractionFrom + 1);
                totalFraction += fractions;
                totalNumber += 1;
                $scope.totalAwards += ((fractions * number.RaffleAward.AwardValue) - (fractions * number.RaffleAward.AwardValue) * (number.RaffleAward.LawDiscount / 100));;
            });

            $scope.identifyBach.IdentifyBachPayments.forEach(function (payment) {
                $scope.totalpayment += payment.Value;
            });
            $scope.identifyBach.NoteCredits.forEach(function (note) {
                $scope.totalpayment += note.TotalCash;
            });

            $scope.totalGeneral = $scope.totalAwards + ($scope.totalAwards * $scope.identifyBach.percent / 100);

            $scope.totalRestant = $scope.totalGeneral - $scope.totalpayment;
            $scope.creditNote.TotalCash = $scope.totalRestant;
        }

        this.loadCreaditNote = function () {
            window.loading.show();
            $.when($.ajax('Cash/GetCashAdvanceInfo')).done(function (cashAdvanceInfo) {
                $scope.clients = cashAdvanceInfo.clients;
                $scope.raffles = cashAdvanceInfo.raffles;
                $scope.totalRestant = 100000000;
                window.loading.hide();
                window.setTimeout(function () {
                    $scope.$apply();
                    $rootScope.createSelect2();
                }, 0);
                $rootScope.dataTable();
            });
        }

        $scope.goBack = function () {
            if ($scope.raffleAwardId > 0) {
                window.location.href = '#/ticket/identifybachdetail/' + $scope.raffleAwardId;
            } else {
                $state.go('app.cashAdvance');
            }
        }

        $scope.saveForm = function () {
            try {
                $scope.creditNote.NoteDate = $rootScope.parseDate($scope.creditNote.NoteDate, $scope.creditNote.NoteDate).toJSON();
            } catch (e) { }
            if (validateData($scope.creditNote) === false) {
                return;
            }
            window.loading.show();
            $.ajax({
                type: 'POST',
                dataType: 'json',
                url: 'Cash/CashAdvance',
                data: $scope.creditNote,
                success: function (data) {
                    window.loading.hide();
                    if (data.result === true) {
                        window.open('/Reports/CashAdvanceReport?cashAdvance=' + data.cashAdvance);
                        alertify.success(data.message);
                        $scope.goBack();
                    } else {
                        alertify.alert(data.message);
                    }
                }
            });
        }
        this.loadCreaditNote();
    }
})();
