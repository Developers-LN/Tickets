/**=========================================================
 * Module: NoteCreaditTaxReceiptController.js
 =========================================================*/

(function () {
    'use strict';

    angular
        .module('naut')
        .controller('NoteCreaditTaxReceiptController', NoteCreaditTaxReceiptController);

    NoteCreaditTaxReceiptController.$inject = ['$scope', '$state', '$rootScope', '$stateParams'];
    function NoteCreaditTaxReceiptController($scope, $state, $rootScope, $stateParams) {
        var self = this;
        $scope.identifyBachId = $stateParams.raffleAwardId;
        $scope.creditNote = {
            Id: 0,
            ClientId: undefined,
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
            $.when(
                $.ajax('Cash/GetCreditNoteTaxReceiptData?creditNoteId=' + $stateParams.noteCreditId + '&identifyBachId=' + $stateParams.raffleAwardId),
                $.ajax('TicketAllocation/GetIdentifyData?identifyId=' + $stateParams.raffleAwardId)).done(function (creditNoteData, identifyBachData) {
                    if (creditNoteData[1] === 'success') {
                        if (creditNoteData[0].result === false) {
                            alertify.alert(creditNoteData[0].message);
                            $scope.goBack();
                        } else {
                            if (creditNoteData[0].creditNote.Id) {
                                creditNoteData[0].creditNote.NoteDate = new Date(creditNoteData[0].creditNote.NoteDate);
                                $scope.creditNote = creditNoteData[0].creditNote;
                                $scope.defaultValue = $scope.creditNote.ClientId;
                            } else if (creditNoteData[0].creditNote.Id === 0) {
                                $scope.creditNote.IdentifyBaches.push(creditNoteData[0].creditNote.RaffleAward);
                                $scope.creditNote.ClientId = Number(creditNoteData[0].ClientId);
                            }
                            $scope.clients = creditNoteData[0].clients;
                            if (identifyBachData[1] === 'success' && $stateParams.raffleAwardId != 0) {
                                var identifyNumbers = [];
                                identifyBachData[0].identifyBach.IdentifyNumbers.forEach(function (identifyNumber) {
                                    identifyNumber.RaffleAwards.forEach(function (award) {
                                        identifyNumbers.push({
                                            FractionFrom: identifyNumber.FractionFrom,
                                            FractionTo: identifyNumber.FractionTo,
                                            Id: identifyNumber.Id,
                                            IdentifyBachId: identifyNumber.IdentifyBachId,
                                            NumberDesc: identifyNumber.NumberDesc,
                                            NumberId: identifyNumber.NumberId,
                                            RaffleAward: award
                                        });
                                    });
                                });
                                identifyBachData[0].identifyBach.IdentifyNumbers = identifyNumbers;
                                $scope.identifyBach = identifyBachData[0].identifyBach;
                                $scope.creditNote.ClientId = $scope.identifyBach.ClientId;
                                self.showTotal();
                            } else {
                                $scope.totalRestant = 100000000;
                            }
                            window.loading.hide();
                            window.setTimeout(function () {
                                $scope.$apply();
                                $rootScope.createSelect2();
                            }, 0);
                            $rootScope.dataTable();
                        }
                    }
                });
        }

        $scope.goBack = function () {
            if ($scope.raffleAwardId > 0) {
                window.location.href = '#/ticket/identifybachdetail/' + $scope.raffleAwardId;
            } else {
                $state.go('app.cashNoteTaxReceiptCreditList');
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
                url: 'Cash/CreditNoteTaxReceipt',
                data: $scope.creditNote,
                success: function (data) {
                    window.loading.hide();
                    if (data.result === true) {
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
