/**=========================================================
 * Module: invoiceTicketController.js
 =========================================================*/

(function () {
    'use strict';

    angular
        .module('naut')
        .controller('invoiceTicketController', invoiceTicketController);

    invoiceTicketController.$inject = ['$scope', '$state', '$rootScope', '$stateParams'];
    function invoiceTicketController($scope, $state, $rootScope, $stateParams) {
        var self = this;
        $scope.allocationList = [];

        this.validateData = function (invoice) {
            var error = '', isReq = ' es un campo requerido. <br>';
            if (invoice.raffleId == undefined) {
                error += 'El Sorteo' + isReq;
            }
            if (invoice.clientId == undefined) {
                error += 'El Cliente' + isReq;
            }
            if (invoice.condition == undefined) {
                error += 'El Condicion' + isReq;
            }
            if (invoice.invoiceDate == undefined) {
                error += 'La Fecha' + isReq;
            }
            if (invoice.ticketAllocations.length == 0) {
                error += 'Seleccione una o mas asignaciones';
            }
            if (error !== '') {
                alertify.showError('Alerta', error);
            }
            return error === '';
        }

        $scope.clientDiscount = 0.00;

        this.loadData = function () {
            window.loading.show();
            $.when(
                $.ajax($rootScope.serverUrl + 'ticket/ticketInvoiceApi/getInvoice?id=' + $stateParams.id),
                $.ajax($rootScope.serverUrl + 'ticket/clientApi/getClientSelect?statu=2089'),/*Clientes aprobados*/
                $.ajax($rootScope.serverUrl + 'ticket/raffleApi/getRaffleSelect?statu=0'),//Sorteos en planificacion
                $.ajax($rootScope.serverUrl + 'ticket/catalogApi/getInvoiceConditionSelect'),
                $.ajax($rootScope.serverUrl + 'ticket/catalogApi/getXpriedInvoiceTimeSelect'),
                $.ajax($rootScope.serverUrl + 'ticket/catalogApi/getPaymentTypeSelect'),
                $.ajax($rootScope.serverUrl + 'ticket/catalogApi/getAllocationTypeSelect')
            ).then(function (invoiceResponse, clientResponse, raffleResponse, invoiceConditionResponse, xpriedInvoiceTimeResponse, paymentTypeResponse) {
                window.loading.hide();

                $scope.clients = clientResponse[0].object;
                $scope.raffles = raffleResponse[0].object;
                $scope.conditions = invoiceConditionResponse[0].object;
                $scope.xpireInvoiceTimes = xpriedInvoiceTimeResponse[0].object;
                $scope.paymentTypes = paymentTypeResponse[0].object;

                invoiceResponse[0].object.invoiceDate = new Date(invoiceResponse[0].object.invoiceDateLong);
                $scope.invoice = invoiceResponse[0].object;
                xpriedInvoiceTimeResponse[0].object = xpriedInvoiceTimeResponse[0].object.map(function (xpired) {
                    xpired.description = Number(xpired.description);
                    return xpired;
                });
                if ($scope.invoice.id == 0) {
                    $scope.invoice.invoiceExpredDay = xpriedInvoiceTimeResponse[0].object[0].description;
                }
                if (invoiceResponse[0].object.id > 0) {
                    $scope.updateAllocations();
                }
                window.setTimeout(function () {
                    $scope.$apply();
                    $rootScope.createSelect2();
                }, 0);
            });
        }

        this.getAllocationsByClient = function (ticketProspectId, poolProspectId, priceId) {
            if ($scope.invoice.raffleId === 0 || $scope.invoice.clientId === 0) {
                return;
            }

            window.loading.show();
            $.when(
                $.ajax($rootScope.serverUrl + 'ticket/ticketAllocationApi/getReviewAllocations?raffleId=' + $scope.invoice.raffleId + '&clientId=' + $scope.invoice.clientId),
                $.ajax($rootScope.serverUrl + 'ticket/prospectApi/getProspectPrice?prospectId=' + ticketProspectId + '&priceId=' + priceId)/*,
                $.ajax($rootScope.serverUrl + 'ticket/prospectApi/getProspectPrice?prospectId=' + poolProspectId + '&priceId=' + priceId)*/
            ).then(function (allocationResponse, ticketPriceResponse/*, poolPriceResponse*/) {
                window.loading.hide();
                if (ticketPriceResponse[1] == 'success') {
                    $scope.invoice.ticketPrice = ticketPriceResponse[0].object.factionPrice;
                }

                /*if (poolPriceResponse[1] == 'success') {
                    if (poolPriceResponse[0].object) {
                        $scope.invoice.poolPrice = poolPriceResponse[0].object.factionPrice;
                    } else {
                        $scope.invoice.poolPrice = 15.6;
                    }
                }*/

                if (allocationResponse[1] == 'success') {
                    if ($scope.invoice.id > 0) {
                        $scope.invoice.ticketAllocations.map(function (a) {
                            a.price = $scope.invoice.ticketPrice;
                            /*if (a.typeId == 5821) {
                                a.price = $scope.invoice.ticketPrice;
                            } else {
                                a.price = $scope.invoice.poolPrice;
                            }*/
                        });
                        allocationResponse[0].object = $scope.invoice.ticketAllocations;
                    }
                    $scope.ticketAllocations = allocationResponse[0].object/*.filter(function (a) {
                        return a.typeId == 5821;
                    })*/;
                    /*
                    $scope.poolAllocations = allocationResponse[0].object.filter(function (a) {
                        return a.typeId == 5822;
                    });*/
                }
                window.setTimeout(function () {
                    $scope.$apply();
                }, 0);
            });
        }

        $scope.showTotalPrice = function () {
            if (!$scope.invoice) {
                return;
            }
            var total = 0, totalTicket = 0, totalFraccion = 0, totalDiscount = 0, subTotal = 0;
            $scope.invoice.ticketAllocations.forEach(function (allocation) {
                totalTicket += allocation.numberCount;
                totalFraccion += allocation.fractionCount;
                total += allocation.fractionCount * allocation.price;
            });
            subTotal = total;
            totalDiscount = (total * $scope.clientDiscount) / 100;
            total = subTotal - totalDiscount;
            return {
                subTotal: $rootScope.parseMoney(subTotal),
                total: $rootScope.parseMoney(total),
                totalDiscount: $rootScope.parseMoney(totalDiscount),
                ticket: $rootScope.parseNumber(totalTicket),
                fraction: $rootScope.parseNumber(totalFraccion)
            };
        }

        $scope.showSupTotalPrice = function (allocation) {
            if (!$scope.invoice) {
                return;
            }
            var total = allocation.fractionCount * allocation.price;
            return $rootScope.parseMoney(total);
        }

        $scope.updateAllocations = function () {
            var raffleId = $scope.invoice.raffleId;
            var clientId = $scope.invoice.clientId;
            var clientPriceId = 0;
            $scope.clients.forEach(function (client) {
                if (client.value == clientId) {
                    clientPriceId = client.priceId;
                    $scope.clientDiscount = client.discount;
                }
            });
            var ticketProspectId = 0;
            var poolProspectId = 0;
            $scope.raffles.forEach(function (raffle, i) {
                if (raffle.value === Number(raffleId)) {
                    ticketProspectId = raffle.ticketProspectId;
                    poolProspectId = raffle.poolProspectId;
                }
            });

            self.getAllocationsByClient(ticketProspectId, poolProspectId, clientPriceId);
        }

        $scope.updateAllocationNumbers = function () {
            if ($scope.invoice.id > 0) {
                return;
            }
            $scope.invoice.ticketAllocations = [];
            $scope.ticketAllocations.forEach(function (allocation) {
                if ($('#allocation-' + allocation.id)[0].checked === true) {
                    allocation.price = $scope.invoice.ticketPrice;
                    $scope.invoice.ticketAllocations = $scope.invoice.ticketAllocations.concat(allocation);
                }
            });
            /*
            $scope.poolAllocations.forEach(function (allocation) {
                if ($('#allocation-' + allocation.id)[0].checked === true) {
                    allocation.price = $scope.invoice.poolPrice;
                    $scope.invoice.ticketAllocations = $scope.invoice.ticketAllocations.concat(allocation);
                }
            });
            */
            $rootScope.destroyDataTable();
            window.setTimeout(function () {
                $scope.$apply();
                $rootScope.dataTable();
            }, 0);
        }

        $scope.saveForm = function () {
            try {
                $scope.invoice.invoiceDate = $rootScope.parseDate($scope.invoice.invoiceDate, $scope.invoice.invoiceDate).toJSON();
            } catch (e) { }
            if (self.validateData($scope.invoice) === false) {
                return;
            }
            var t = $scope.invoice;
            window.loading.show();
            $.ajax({
                type: 'POST',
                dataType: 'json',
                url: $rootScope.serverUrl + 'ticket/ticketInvoiceApi/save',
                data: $scope.invoice,
                success: function (response) {
                    window.loading.hide();
                    if (response.result === true) {
                        window.open('/Reports/InvoiceDetail?invoiceId=' + response.object.id);
                        alertify.success(response.message);
                        $state.go('app.ticketInvoiceList');
                    } else {
                        alertify.alert(response.message);
                    }
                    window.setTimeout(function () {
                        document.getElementById("save").attr('disabled', true);
                    }, 0);
                }
            });
        }

        this.loadData();
    }
})();
