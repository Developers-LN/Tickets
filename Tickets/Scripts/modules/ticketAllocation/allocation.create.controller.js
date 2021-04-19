/**=========================================================
 * Module: AllocationCreateController.js
 =========================================================*/

(function() {
    'use strict';

    angular
        .module('naut')
        .controller('AllocationCreateController', AllocationCreateController);

    AllocationCreateController.$inject = ['$scope', '$state', '$rootScope', '$stateParams'];
    function AllocationCreateController($scope, $state, $rootScope, $stateParams) {
        var self = this;
        $scope.total = {
            number: 0,
            fraction: 0,
            total: 0
        };

        this.currentTarget = null;

        $scope.clearTicketNumber = function () {
            $scope.ticketNumber = {
                numberFrom: undefined,
                numberTo: undefined,
                number: undefined
            }
        }

        this.validateNumber = function (ticket) {
            var error = '', isReq = ' es un campo requerido. <br/>';
            if ($('#ticketAllocateNumber')[0].checked === true) {
                if (ticket.number === undefined) {
                    error += 'El numero' + isReq;
                }
                $scope.ticket.ticketAllocationNumbers.forEach(function (number) {
                    if (number == ticket.number || number.number == ticket.number) {
                        var numberString = '';
                        for (var i = number.fractionFrom; i <= number.fractionTo; i++) {
                            if ( i >= $scope.ticket.fractionFrom && i <= $scope.ticket.fractionTo) {
                                numberString += i;
                                if (i != $scope.ticket.fractionTo) {
                                    numberString += ', ';
                                }
                            }
                        }
                        if (numberString != '') {
                            error += 'Las fracciones ' + numberString + ' del numero ' + ticket.number + ' ya esta agregado. <br/>';
                        }
                    }
                });
            } else {
                if (ticket.numberFrom === undefined) {
                    error += 'El numero desde' + isReq;
                }
                if (ticket.numberTo === undefined) {
                    error += 'El numero hasta' + isReq;
                }
                var e = '';
                for (var number = ticket.numberFrom; number <= ticket.numberTo; number++) {
                    if ($scope.ticket.ticketAllocationNumbers.some(function (n) {
                        return n === number
                    }) === true) {
                        e += number;
                        if (number < ticket.numberTo) {
                            e += ', ';
                        }
                    }
                }
                if (e !== '') {
                    error += 'Los n&uacute;mero ( ' + e + ' ) ya estan agregado. <br/>';
                }
            }

            if (error !== '') {
                alertify.showError('Alerta', error);
            }
            return error === '';
        }

        $scope.deleteNumber = function (number) {
            // confirm dialog
            alertify.confirm("&iquest;Desea borrar este Numero?", function (e) {
                if (e) {
                    $scope.ticket.ticketAllocationNumbers = $scope.ticket.ticketAllocationNumbers.filter(function (item) {
                        return item !== number;
                    });
                    $rootScope.destroyDataTable();
                    setTimeout(function () {
                        $scope.$apply();
                        $rootScope.dataTable();
                        alertify.success('Numero borrado correctamente!');
                    }, 0);
                }
            });
        }

        this.validateData = function (ticket) {
            if ($stateParams.allocationId != 0) {
                return true;
            }

            var error = '', isReq = ' es un campo requerido. <br>';
            if (ticket.raffleId === undefined) {
                error += 'El Sorteo' + isReq;
            }
            if (ticket.clientId === undefined) {
                error += 'El cliente' + isReq;
            }
            if (ticket.fractionFrom === undefined) {
                error += 'El fracci&#243;n desde' + isReq;
            }
            if (ticket.fractionTo === undefined) {
                error += 'El fracci&#243;n hasta' + isReq;
            }
            if (error !== '') {
                alertify.showError('Alerta', error);
            }
            return error === '';
        }

        this.loadAllocationData = function () {
            window.loading.show();
            $.when(
                $.ajax($rootScope.serverUrl + 'ticket/ticketAllocationApi/getTicketAllocation?id=' + $stateParams.allocationId),
                $.ajax($rootScope.serverUrl + 'ticket/catalogApi/getAllocationTypeSelect'),/*Clientes aprobados*/
                $.ajax($rootScope.serverUrl + 'ticket/clientApi/getClientSelect?statu=2089'),/*Clientes aprobados*/
                $.ajax($rootScope.serverUrl + 'ticket/raffleApi/getRaffleSelect?statu=68')//Sorteos en planificacion
            ).then(function (allocationResponse, typeResponse, clientResponse, raffleResponse) {
                window.loading.hide();
                if (typeResponse[1] == 'success') {
                    $scope.types = typeResponse[0].object;
                }
                if (clientResponse[1] == 'success') {
                    $scope.clients = clientResponse[0].object;
                }
                if (raffleResponse[1] == 'success') {
                    $scope.raffles = raffleResponse[0].object;
                }

                if (allocationResponse[1] == 'success') {
                    $scope.ticket = allocationResponse[0].object;
                    if ($scope.ticket.id > 0) {
                        $scope.production = $scope.ticket.production;
                        $scope.fractionPrice = $scope.ticket.fractionPrice;
                        if ($scope.ticket.ticketAllocationNumbers.length > 0) {
                            $scope.ticket.fractionFrom = $scope.ticket.ticketAllocationNumbers[0].fractionFrom;
                            $scope.ticket.fractionTo = $scope.ticket.ticketAllocationNumbers[0].fractionTo;
                        }
                    }
                }
                window.setTimeout(function () {
                    $scope.$apply();
                    $rootScope.createSelect2();
                }, 0);
            });
        }

        this.loadtotal = function () {
            if ($scope.ticket.raffleId <= 0 || $scope.ticket.clientId <= 0) {
                return;
            }
            $scope.total.fraction = $scope.ticket.ticketAllocationNumbers.length * ($scope.ticket.fractionTo - $scope.ticket.fractionFrom + 1);
            $scope.total.total = $scope.total.fraction * $scope.fractionPrice;
            window.setTimeout(function () {
                $scope.$apply();
            }, 0);
        }

        $scope.changeValues = function () {
            if ($scope.ticket.typeId == 5822 /*Tipo quinielas*/) {
                $scope.fractionLabel = "Cantidad";
            } else {
                $scope.fractionLabel = "Cantidad de Fracciones";
            }
            if ($scope.ticket.raffleId <= 0 || $scope.ticket.clientId <= 0 || $scope.ticket.typeId <= 0) {
                return;
            }
            var id = 0;
            
            $scope.raffles.forEach(function (raffle) {
                if (raffle.value == $scope.ticket.raffleId) {
                    if ($scope.ticket.typeId == 5821) {/*Asignacion de billetes*/
                        id = raffle.ticketProspectId;
                    } else {
                        id = raffle.poolProspectId;
                    }
                }
            });
            window.loading.show();
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: 'ticket/prospectApi/getProspect?id=' + id,
                success: function (response) {
                    window.loading.hide();
                    if (response.result == true) {
                        $scope.prospect = response.object;

                        $scope.clients.forEach(function (client) {
                            if (client.value == $scope.ticket.clientId) {
                                $scope.prospect.prospectPrices.forEach(function (price) {
                                    if (price.priceId == client.priceId) {
                                        $scope.fractionPrice = price.factionPrice;
                                    }
                                });
                            }
                        });
                        $scope.ticket.fractionFrom = 1;
                        $scope.ticket.fractionTo = $scope.prospect.leafNumber * $scope.prospect.leafFraction;
                        $scope.maxFraction = $scope.prospect.leafNumber * $scope.prospect.leafFraction;
                        $scope.production = $scope.prospect.production;
                        $scope.ticket.ticketAllocationNumbers = [];
                        self.loadtotal();

                        $scope.$apply();
                    } else {
                        alertify.alert(response.message);
                    }
                }
            });
        }

        $scope.returnTo = function () {
            if (Number($stateParams.allocationId) > 0) {
                window.location.href = '/#/ticket/allocationDetails/' + $stateParams.allocationId;
            } else {
                window.location.href = '/#/ticket/allocations';
            }
        }

        $scope.verifyKeydown = function (e) {
            if (e.which == 13) {
                self.currentTarget = e.currentTarget;
                $scope.addNumber();
            }
        }

        $scope.addNumber = function () {
            if (self.validateNumber($scope.ticketNumber) === false) {
                return;
            }
            var numbers = [];
            if ($('#ticketAllocateNumber')[0].checked === false) {
                for (var number = $scope.ticketNumber.numberFrom; number <= $scope.ticketNumber.numberTo; number++) {
                    numbers.push({
                        id: 0,
                        number: number,
                        fractionFrom: $scope.ticket.fractionFrom,
                        fractionTo: $scope.ticket.fractionTo
                    });
                }
            } else {
                numbers.push({
                    id: 0,
                    number: $scope.ticketNumber.number,
                    fractionFrom: $scope.ticket.fractionFrom,
                    fractionTo: $scope.ticket.fractionTo
                });
            }
            self.verifyTicketAllocation(numbers);
        }

        this.verifyTicketAllocation = function (numbers) {
            if (self.validateData($scope.ticket) === false) {
                return;
            }
            var ticket = {
                id: $scope.ticket.id,
                raffleId: $scope.ticket.raffleId,
                ticketAllocationNumbers: numbers,
                typeId: $scope.ticket.typeId
            };

            window.loading.show();
            $.ajax({
                type: 'POST',
                dataType: 'json',
                url: $rootScope.serverUrl + 'ticket/ticketAllocationApi/validateAllocation',
                data: ticket,
                success: function (response) {
                    window.loading.hide();
                    if (response.result === false) {
                        alertify.showError('Alerta', response.message);
                    } else {
                        $scope.ticket.ticketAllocationNumbers = $scope.ticket.ticketAllocationNumbers.concat(numbers);
                        self.loadtotal();

                        $rootScope.destroyDataTable();
                        setTimeout(function () {
                            $scope.clearTicketNumber();
                            $scope.$apply();
                            $rootScope.dataTable();
                            alertify.success(response.message);
                            window.setTimeout(function(){
                                if (self.currentTarget !== null) {
                                    self.currentTarget.focus();
                                }
                            },0);
                        }, 0);
                    }
                }
            });
        }

        $scope.saveTicketForm = function () {

            if (self.validateData($scope.ticket) === false) {
                return;
            }
            window.loading.show();
            $.ajax({
                type: 'POST',
                dataType: 'json',
                url: $rootScope.serverUrl + 'ticket/ticketAllocationApi/saveAllocation',
                data: $scope.ticket,
                success: function (response) {
                    window.loading.hide();
                    if (response.result === false) {
                        alertify.alert(response.message);
                    } else {
                        alertify.success(response.message);
                        $scope.returnTo();
                    }
                }
            });
        }

        $("input[name=numbers]:radio").change(function (val) {
            if (val.currentTarget.value == 'number') {
                $('#numberContainer').removeClass('hide');
                $('#rangeNumberContainer').addClass('hide');
            } else {
                $('#numberContainer').addClass('hide');
                $('#rangeNumberContainer').removeClass('hide');
            }
        });

        $scope.maxFraction = 0;
        $scope.production = 0;
        self.loadAllocationData();

        $scope.clearTicketNumber();

        $rootScope.dataTable();

        $scope.fractionLabel = "Cantidad de Fracciones";
    }
})();
