/**=========================================================
 * Module: CheckPaymentController.js
 =========================================================*/

(function () {
    'use strict';

    angular
        .module('naut')
        .controller('CheckPaymentController', CheckPaymentController);

    CheckPaymentController.$inject = ['$scope', '$state', '$rootScope', '$stateParams'];
    function CheckPaymentController($scope, $state, $rootScope, $stateParams) {
        $scope.payment = {
            Id: 0,
            CashId: '',
            ClientId: '',
            IdentifyBachId: $stateParams.identifyBachId,
            Value: undefined,
            Note: undefined,
            PaymentType: undefined
        };
        function validateData(payment) {
            var error = '', isReq = ' es un campo requerido. <br>';
            if (payment.Value === undefined) {
                error += 'El Monto' + isReq;
            }
            if (payment.Note === undefined) {
                error += 'La Nota' + isReq;
            }
            if (error !== '') {
                alertify.showError('Alerta', error);
            }
            return error === '';
        }
        function validateNumenber(number) {
            if (Number(number) < 0 || number === '') {
                return false
            }
            return true;
        }
        this.loadPaymentData = function () {
            window.loading.show();
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: 'Cash/VerifyCashOpen',
                success: function (data) {
                    if (data.result !== true) {
                        window.location.href = '#/ticket/identifybachdetail/' + $stateParams.identifyBachId;
                        alertify.confirm(data.message, function (ok) {
                            if (ok === true) {
                                if (data.result === 1) {
                                    $state.go('app.cashOpen');
                                } else if (data.result === 2) {
                                }
                            }
                        }, function () { });
                    } else {
                        $.when($.ajax('Cash/GetPaymentData'), $.ajax('TicketAllocation/GetIdentifyData?identifyId=' + $stateParams.identifyBachId))
                            .done(function (paymentData, identifyBach) {
                                if (paymentData[1] == 'success') {
                                    if (paymentData[0].result === false) {
                                        alertify.alert(paymentData[0].message);
                                        window.location.href = '#/ticket/identifybachtopaydetail/' + $stateParams.identifyBachId;
                                    } else {
                                        if (identifyBach[1] == 'success') {
                                            var identifyNumbers = [];
                                            identifyBach[0].identifyBach.IdentifyNumbers.forEach(function (identifyNumber) {
                                                identifyNumber.RaffleAwards.forEach(function (award) {
                                                    identifyNumbers.push({
                                                        FractionFrom: identifyNumber.FractionFrom,
                                                        FractionTo: identifyNumber.FractionTo,
                                                        Id: identifyNumber.Id,
                                                        IdentifyBachId: identifyNumber.IdentifyBachId,
                                                        NumberDesc: identifyNumber.NumberDesc,
                                                        NumberId: identifyNumber.NumberId,
                                                        RaffleAward: award,
                                                        TicketNumber: identifyNumber.TicketNumber,
                                                    });
                                                });
                                            });
                                            identifyBach[0].identifyBach.IdentifyNumbers = identifyNumbers;
                                            $scope.identifyBach = identifyBach[0].identifyBach;
                                            $scope.showTotal();
                                        }
                                        $scope.clients = paymentData[0].clients;
                                        window.loading.hide();
                                        $scope.$apply();
                                    }
                                }
                            });
                    }
                }
            });
        }
        $scope.totalpayment = 0;
        $scope.totalGeneral = 0;

        $scope.totalRestant = 0;
        $scope.totalPercent = 0;
        $scope.totalAwards = 0;

        $scope.showTotal = function () {
            var totalFraction = 0, totalNumber = 0, totalValue = 0, cashValue = 0.0, natureValue = 0.0;
            $scope.identifyBach.IdentifyNumbers.forEach(function (number) {
                var fractions = (number.RaffleAward.FractionTo - number.RaffleAward.FractionFrom + 1);
                totalFraction += fractions;
                totalNumber += 1;
                totalValue += number.RaffleAward.AwardValue;
                $scope.totalAwards += ((fractions * number.RaffleAward.AwardValue) - (fractions * number.RaffleAward.AwardValue) * (number.RaffleAward.LawDiscount / 100));
                if (number.RaffleAward.TypesAwardId != 16 && number.RaffleAward.TypesAwardId != 17) {
                    cashValue += ((fractions * number.RaffleAward.AwardValue) - (fractions * number.RaffleAward.AwardValue) * (number.RaffleAward.LawDiscount / 100));
                }
                else {
                    natureValue += ((fractions * number.RaffleAward.AwardValue) - (fractions * number.RaffleAward.AwardValue) * (number.RaffleAward.LawDiscount / 100));
                }
            });

            $scope.identifyBach.IdentifyBachPayments.forEach(function (payment) {
                $scope.totalpayment += payment.Value;
            });
            $scope.identifyBach.NoteCredits.forEach(function (note) {
                $scope.totalpayment += note.TotalCash;
            });

            $scope.totalpayment = $scope.totalpayment;

            $scope.totalGeneral = $scope.totalAwards + ($scope.totalAwards * $scope.identifyBach.percent / 100);

            $scope.totalRestant = $scope.totalGeneral - $scope.totalpayment;
            $scope.payment.Value = $scope.totalRestant - natureValue;
        }

        $scope.saveForm = function () {
            if (validateData($scope.payment) === false) {
                return;
            }
            window.loading.show();
            $.ajax({
                type: 'POST',
                dataType: 'json',
                url: 'Cash/CheckPayment',
                data: $scope.payment,
                success: function (data) {
                    window.loading.hide();
                    if (data.result === true) {
                        alertify.success(data.message);
                        window.location.href = '#/ticket/identifybachtopaydetail/' + $stateParams.identifyBachId;
                    } else {
                        alertify.alert(data.message);
                    }
                }
            });
        }
        this.loadPaymentData();
    }
})();
