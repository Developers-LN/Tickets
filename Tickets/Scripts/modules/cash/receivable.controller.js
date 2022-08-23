/**=========================================================
 * Module: ReceivableController.js
 =========================================================*/

(function () {
    'use strict';

    angular
        .module('naut')
        .controller('ReceivableController', ReceivableController);

    ReceivableController.$inject = ['$scope', '$state', '$rootScope', '$stateParams'];
    function ReceivableController($scope, $state, $rootScope, $stateParams) {

        function validateData(receivable) {
            var error = '', isReq = ' es un campo requerido. <br>';
            var isReq2 = ' son requeridos. <br>';
            if (receivable.ReceiptDate === undefined) {
                error += 'La Fecha del Recibo' + isReq;
            }
            if (receivable.ReceiptType === undefined) {
                error += 'El Tipo de Pago' + isReq;
            }
            if (receivable.ReceiptType == 5857 && receivable.Recibo === undefined) {
                error += 'El Numero de recibo' + isReq;
            }
            if (receivable.clientType == 5862 && receivable.Cedula === undefined && receivable.Nombre === undefined && receivable.Observaciones === undefined) {
                error += 'Los datos del colaborador' + isReq2;
            }
            if (receivable.TotalCash === undefined) {
                error += 'Verifique el monto a pagar' + isReq;
            }
            if ($scope.receivable.TotalCash > $scope.payment.totalRestant) {
                error += 'No se puede pagar mas de lo que debe' + isReq;
            }
            if ($scope.receivable.IncludeCashAdvance === 1 && $scope.receivable.CashAdvance === undefined && $scope.receivable.CashAdvanceNote === undefined) {
                error += 'Los datos del avance de efectivo' + isReq2;
            }
            if (error !== '') {
                alertify.showError('Alerta', error);
            }
            return error === '';
        }

        var self = this;

        $scope.receivable = {
            Id: 0,
            InvoiceId: undefined,
            ReceiptDate: new Date(),
            ReceiptType: undefined,
            NoteCreditReceiptPayments: [],
            TotalCash: undefined,
            Recibo: undefined,
            Cedula: undefined,
            Nombre: undefined,
            Codigo: undefined,
            Telefono: undefined,
            Observaciones: undefined,
            clientType: undefined,
            Notas: undefined,
            CashAdvance: undefined,
            IncludeCashAdvance: undefined,
            CashAdvanceNote: undefined
        };

        $scope.totalCreditNoteValue = 0;
        this.getReceivableData = function () {
            window.loading.show();
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: 'Cash/VerifyCashOpen',
                success: function (data) {
                    if (data.result !== true) {
                        window.location.href = '#/ticket/identifybachdetail/' + $stateParams.identifyBachId;
                        alertify.confirm(data.message, function (ok) {
                            window.loading.hide();
                            if (ok === true) {
                                if (data.result === 1) {
                                    $state.go('app.cashOpen');
                                } else if (data.result === 2) {
                                }
                            }
                        }, function () { });
                    } else {
                        $.ajax({
                            type: 'GET',
                            contentType: 'application/json; charset=utf-8',
                            url: 'Cash/GetReceivableData?invoiceId=' + $stateParams.invoiceId,
                            success: function (data) {
                                window.loading.hide();
                                $scope.clientName = data.clientName;
                                $scope.clientId = data.clientId;
                                $scope.receivable.clientType = data.clientType;
                                $scope.paymentTypes = data.paymentTypes.filter(function (p) {
                                    if (p.Id == 2075 && $rootScope.moduleCanDelete == '') {
                                        return p;
                                    }
                                    if (p.Id != 2075 && $rootScope.moduleCanSearch == '') {
                                        return p;
                                    }
                                });
                                $scope.cashAdvances = data.cashAdvances;
                                $scope.creditNotes = data.creditNotes;
                                $scope.payment = data.payment;
                                $scope.returneds = data.returneds;
                                //$rootScope.dataTable();
                                $scope.invoiceId = $stateParams.invoiceId;
                                $scope.raffleId = data.raffleId;

                                $scope.paymentsHistory = data.paymentsHistory
                                $scope.raffleName = data.raffleName;
                                $scope.invoiceDate = data.invoiceDate;
                                $scope.invoiceDiscount = data.invoiceDiscount;

                                $rootScope.destroyDataTable();
                                $scope.$apply();
                                $rootScope.dataTable();
                                rCheck($scope.creditNotes);
                                rCheck($scope.cashAdvances);
                            }
                        });
                    }
                }
            });
        }

        $scope.fullCredit = false;

        $scope.isreturn = function (cn) {
            if (cn.Concepts.includes("Nota de credito por devoluci\u00F3n de billetes del Sorteo")) {
                return true;
            } else {
                return false;
            }
        };

        var rCheck = function (c) {
            angular.forEach(c, function (cn, i) {
                if (c[i].Concepts.includes("Nota de credito por devoluci\u00F3n de billetes del Sorteo")) {
                    event.target.checked = true;
                    $scope.selectCreditNote(event, c[i]);
                } else {

                    event.target.checked = null;
                }
            });
        }

        $scope.selectCreditNote = function (e, creditNote) {
            if (e.target.checked == true) {
                //last change
                if ($scope.totalCreditNoteValue <= $scope.payment.totalRestant && $scope.fullCredit == false) {
                    $scope.totalCreditNoteValue += creditNote.TotalRest;

                    if ($scope.totalCreditNoteValue >= $scope.payment.totalRestant) {
                        $scope.fullCredit = true;
                    }

                    $scope.receivable.NoteCreditReceiptPayments.push({ NoteCreditId: creditNote.Id });

                } else {
                    e.target.checked = false;
                    if ($scope.totalCreditNoteValue < $scope.payment.totalRestant) {
                        $scope.fullCredit = false;
                    }
                }
            } else {
                var index = 0;
                $scope.receivable.NoteCreditReceiptPayments.findIndex(function (e, i, a) {
                    if (e.NoteCreditId == creditNote.Id) {
                        index = i;
                        return i;
                    }
                });

                if (index > -1) {
                    $scope.receivable.NoteCreditReceiptPayments.splice(index, 1);
                }

                $scope.totalCreditNoteValue -= creditNote.TotalRest;
                $scope.receivable.NoteCreditReceiptPayments.filter(function (r) {
                    return r.NoteCreditId != creditNote.Id;
                });

                if ($scope.totalCreditNoteValue < $scope.payment.totalRestant) {
                    $scope.fullCredit = false;
                }
            }
            $scope.receivable.TotalCash = $scope.totalCreditNoteValue;

            if ($scope.receivable.TotalCash >= ($scope.payment.totalRestant /*- $scope.payment.discount*/)) {
                $scope.receivable.TotalCash = ($scope.payment.totalRestant /*- $scope.payment.discount*/);
            }
            window.setTimeout(function () {
                $scope.$apply();
            }, 0);
        }

        $scope.saveForm = function () {
            try {
                $scope.receivable.ReceiptDate = $rootScope.parseDate($scope.receivable.ReceiptDate, $scope.receivable.ReceiptDate).toJSON();
                $scope.receivable.InvoiceId = $stateParams.invoiceId;
                if ($scope.receivable.includeCashAdvance === undefined || $scope.receivable.includeCashAdvance === 0) {
                    $scope.receivable.IncludeCashAdvance = 0;
                    $scope.receivable.CashAdvance = 0;
                    $scope.receivable.CashAdvanceNote = "N/A";
                }
            } catch (e) { }
            if (validateData($scope.receivable) === false) {
                return;
            }
            $.ajax({
                type: 'POST',
                dataType: 'json',
                url: 'Cash/Receivable',
                //data: $scope.receivable,
                data:
                {
                    receiptPayment: $scope.receivable,
                    includeCashAdvance: $scope.receivable.IncludeCashAdvance,
                    totalCashAdvance: $scope.receivable.CashAdvance,
                    noteCashAdvance: $scope.receivable.CashAdvanceNote
                },
                success: function (data) {
                    if (data.result === true) {
                        if (data.clientType == 5862) {
                            window.open('/Reports/InvoicePaymentInfo?paymentId=' + data.Pago);
                        }
                        else {
                            window.open('/Reports/InvoicePayment?paymentId=' + data.Pago);
                        }
                        alertify.success(data.message);
                        $state.go('app.cashReceivableList');
                    } else {
                        alertify.alert(data.message);
                    }
                }
            });
        }
        this.getReceivableData();
    }
})();
