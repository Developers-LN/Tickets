/**=========================================================
 * Module: IdentifyAwardDetailController.js
 =========================================================*/

(function () {
    'use strict';

    angular
        .module('naut')
        .controller('IdentifyAwardDetailController', IdentifyAwardDetailController);

    IdentifyAwardDetailController.$inject = ['$scope', '$state', '$rootScope', '$stateParams'];
    function IdentifyAwardDetailController($scope, $state, $rootScope, $stateParams) {
        var self = this;

        $scope.totalFraction = 0;
        $scope.totalValue = 0;
        $scope.totalNumber = 0;
        $scope.totalGeneral = 0;
        $scope.pricePerFraction = 0;

        $scope.showPayments = true;

        $rootScope.returnUrl = "/#/ticket/identifybachdetail/" + $stateParams.identifyId;
        $scope.totalpayment = 0;

        this.loadIdentifyData = function () {
            window.loading.show();
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: 'TicketAllocation/GetIdentifyDetilsData?identifyId=' + $stateParams.identifyId,
                success: function (data) {
                    var identifyNumbers = [];
                    data.identifyBach.IdentifyNumbers.forEach(function (identifyNumber) {
                        identifyNumber.RaffleAwards.forEach(function (award) {
                            identifyNumbers.push({
                                FractionFrom: award.FractionFrom,
                                FractionTo: award.FractionTo,
                                Id: identifyNumber.Id,
                                IdentifyBachId: identifyNumber.IdentifyBachId,
                                NumberDesc: identifyNumber.NumberDesc,
                                NumberId: identifyNumber.NumberId,
                                RaffleAward: award,
                                TicketNumber: identifyNumber.TicketNumber,
                            });
                        });
                    });
                    data.identifyBach.IdentifyNumbers = identifyNumbers;
                    $scope.identifyBach = data.identifyBach;

                    $scope.pricePerFraction = data.pricePerFraction;

                    $scope.showTotal();
                    window.loading.hide();
                    $rootScope.destroyDataTable();
                    $scope.$apply();
                    $rootScope.dataTable();
                }
            });
        }

        $scope.showSubTotal = function (number) {
            var subTotal = 0;
            subTotal = ((number.FractionTo - number.FractionFrom + 1) * number.RaffleAward.AwardValue);
            return subTotal;
        }

        $scope.approvePay = function (id) {
            // confirm dialog
            alertify.confirm("&iquest;Desea aprobar el pago de este lote?", function (e) {
                if (e) {
                    window.loading.show();
                    $.ajax({
                        type: 'POST',
                        dataType: 'json',
                        url: $rootScope.serverUrl + 'ticket/ticketAllocationApi/approveBachPay?id=' + id,
                        success: function (response) {
                            window.loading.hide();
                            if (response.result == false) {
                                alertify.alert(response.message);
                            } else {
                                window.open('Reports/ApprovedBach?bachId=' + id);
                                self.loadIdentifyData();
                                alertify.alert(response.message);
                            }
                        }
                    });
                }
            });
        }

        $scope.showTotal = function () {
            $scope.identifyBach.IdentifyNumbers.forEach(function (number) {
                var fractions = (number.RaffleAward.FractionTo - number.RaffleAward.FractionFrom + 1);
                $scope.totalFraction += fractions;
                $scope.totalNumber += 1;
                $scope.totalValue += (number.RaffleAward.AwardValue / fractions);
                $scope.totalToPay += (number.RaffleAward.AwardValue - (number.RaffleAward.AwardValue * (number.RaffleAward.LawDiscount / 100))) / fractions;
                $scope.totalGeneral += ((fractions * number.RaffleAward.AwardValue) - (fractions * number.RaffleAward.AwardValue) * (number.RaffleAward.LawDiscount / 100));
            });

            $scope.identifyBach.IdentifyBachPayments.forEach(function (payment) {
                $scope.totalpayment += payment.Value;
            });
            $scope.identifyBach.NoteCredits.forEach(function (note) {
                $scope.totalpayment += note.TotalCash;
            });
        }
        this.loadIdentifyData();
    }
})();
