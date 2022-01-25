/**=========================================================
 * Module: ReturnedController.js
 =========================================================*/

(function () {
    'use strict';

    angular
        .module('naut')
        .controller('ReturnedController', ReturnedController);

    ReturnedController.$inject = ['$scope', '$state', '$rootScope', '$stateParams'];
    function ReturnedController($scope, $state, $rootScope, $stateParams) {
        $scope.prospectFraction = 0;
        var self = this;

        this.currentTarget = null;
        this.clearTicket = function (ticketNumber) {
            $scope.editingTicketNumber = ticketNumber ? ticketNumber : null;
            $scope.ticketNumber = {
                clientId: ticketNumber ? ticketNumber.clientId : undefined,
                fractionFrom: ticketNumber ? ticketNumber.fractionFrom : undefined,
                fractionTo: ticketNumber ? ticketNumber.fractionTo : undefined,
                numberId: ticketNumber ? ticketNumber.numberId : undefined,
                $$hashKey: ticketNumber ? ticketNumber.$$hashKey : undefined
            };
        }

        $scope.fractionTotal = function () {
            var fraction = 0;
            $scope.returned.ticketReturnedNumbers.forEach(function (t) {
                fraction += t.fractionTo - t.fractionFrom + 1;

            });
            return fraction;
        }

        this.validateTicketNumber = function (ticketNumber) {
            var error = '', isReq = ' es un campo requerido. <br>';
            if (ticketNumber.clientId === undefined) {
                error += 'El Cliente' + isReq;
            }
            if (ticketNumber.numberId === undefined) {
                error += 'El Billete' + isReq;
            }
            if (ticketNumber.fractionFrom === undefined) {
                error += 'El Fraccion Desde' + isReq;
            }
            if (ticketNumber.fractionTo === undefined) {
                error += 'El Fraccion Hasta' + isReq;
            }
            error += self.validateNumberInList(ticketNumber);

            if (error !== '') {
                alertify.showError('Alerta', error);
            }
            return error === '';
        }

        this.validateData = function (returned) {
            var error = '', isReq = ' es un campo requerido. <br>';
            if (returned.raffleId === undefined) {
                error += 'El Sorteo' + isReq;
            }
            if (returned.returnedGroup === undefined) {
                error += 'El Grupo' + isReq;
            } else if (returned.returnedGroup.length > 10) {
                error += 'El Grupo contiene mas de 10 caracteres. <br>';
            }
            if (returned.ticketReturnedNumbers.length == 0) {
                error += 'No ha ingresado ningun billete a devolver!';
            }
            if (error !== '') {
                alertify.showError('Alerta', error);
            }
            return error === '';
        }

        this.validateNumberInList = function (number) {
            var message = '';
            $scope.returned.ticketReturnedNumbers = $scope.returned.ticketReturnedNumbers || [];

            for (var fraction = number.fractionFrom; fraction <= number.fractionTo; fraction++) {
                if ($scope.returned.ticketReturnedNumbers.some(function (n) {
                    return fraction >= n.fractionFrom && fraction <= n.fractionTo && n.numberId == number.numberId && n.$$hashKey != number.$$hashKey;
                }) === true) {
                    message += 'La fracci&oacute;n ' + fraction + ' del billete ' + number.numberId + ' esta agregada en el listado.' + ' <br/>';
                }
            }
            return message;
        }

        $scope.changeRaffle = function () {
            $scope.raffles.forEach(function (raffle) {
                if (raffle.value == $scope.returned.raffleId) {
                    $scope.maxFraction = raffle.rractionCount;
                    $scope.production = raffle.production;
                }
            });
        }

        this.loadData = function () {
            window.loading.show();
            $.when(
                $.ajax($rootScope.serverUrl + 'ticket/clientApi/getClientSelect?statu=2089'),/*Clientes aprobados*/
                $.ajax($rootScope.serverUrl + 'ticket/raffleApi/getRaffleForReturnedSelect')//Sorteos en planificacion
            ).then(function (clientResponse, raffleResponse) {
                window.loading.hide();
                if (clientResponse[1] == 'success') {
                    $scope.clients = clientResponse[0].object;
                }
                if (raffleResponse[1] == 'success') {
                    $scope.raffles = raffleResponse[0].object;
                }
                window.setTimeout(function () {
                    $scope.$apply();
                    $rootScope.createSelect2();
                }, 0);
            });
        }

        $scope.saveForm = function () {
            if (self.validateData($scope.returned) === false) {
                return;
            }
            window.loading.show();
            $.ajax({
                type: 'POST',
                dataType: 'json',
                url: 'ticket/ticketReturnedApi/save',
                data: $scope.returned,
                success: function (data) {
                    window.loading.hide();
                    if (data.result === true) {
                        window.setTimeout(function () {
                            $scope.returned.ticketReturnedNumbers = [];
                            alertify.alert(data.message);
                            $scope.$apply();
                        }, 0);
                    } else {
                        if (data.object) {
                            var messageString = '';
                            data.object.forEach(function (message) {
                                messageString += message + " <br/>";
                            });
                            alertify.showError('Alerta', messageString);
                        } else {
                            alertify.showError('Alerta', data.message);
                        }
                    }
                }
            });
        }

        $scope.goBack = function () {
            $scope.ticketNumber = {};
            window.document.body.removeEventListener('keydown', self.barcodeReader, false);
            window.location.href = '/#/ticket/Returneds';
        }

        $scope.editNumber = function (ticketNumber) {
            if (ticketNumber) {
                $scope.editingTicketNumber = ticketNumber;
            } else {
                $scope.editingTicketNumber = null;
            }
            self.clearTicket(ticketNumber);
        }

        $scope.deleteNumber = function (ticketNumber) {
            // confirm dialog
            alertify.confirm("&iquest;Desea borrar este Numero?", function (e) {
                if (e) {
                    setTimeout(function () {
                        $scope.returned.ticketReturnedNumbers = $scope.returned.ticketReturnedNumbers.filter(function (item) {
                            return item.$$hashKey !== ticketNumber.$$hashKey;
                        });
                        $scope.$apply();
                        alertify.success('Numero borrado correctamente!');
                    }, 0);
                }
            });
        }

        this.ticketReaderString = '';

        this.barcodeReader = function (e) {
            if (e.which == 17 || e.which == 74) {
                e.stopPropagation();
                e.preventDefault();
                e.returnValue = false;
                e.cancelBubble = true;
                return false;
            } else {
                $scope.verifyTicketNumber(e);
            }
        }

        try {
            window.document.body.removeEventListener('keydown', self.barcodeReader, false);
        } catch (e) { }

        $rootScope.barcodeReader = self.barcodeReader;
        window.document.body.addEventListener('keydown', self.barcodeReader, false);

        $scope.verifyTicketNumber = function (event) {
            if (event.keyCode === 13 && self.ticketReaderString !== '') {
                var codeRead = self.ticketReaderString;
                self.ticketReaderString = '';
                try {
                    if (codeRead.length < 16) {
                        alertify.showError('Alerta', 'La lectura del codigo de barra es erronea.');
                        return;
                    }

                    var codeReadSplit = codeRead.split('-');
                    codeReadSplit = codeReadSplit.filter(function (s) {
                        if (s !== '') {
                            return s;
                        }
                    });
                    var raffleId = codeReadSplit[0].replace(/[^0-9\.]+/g, '');

                    if (raffleId !== $scope.returned.raffleId) {
                        alertify.showError('Alerta', 'El billete leido no es del sorteo seleccionado.');
                        return;
                    }

                    var currentTicketNumber = {
                        clientId: $scope.ticketNumber.clientId,
                        fractionFrom: Number(codeReadSplit[2].replace(/[^0-9\.]+/g, '')),
                        fractionTo: Number(codeReadSplit[3].replace(/[^0-9\.]+/g, '')),
                        numberId: Number(codeReadSplit[1].replace(/[^0-9\.]+/g, ''))
                    };

                    var returnedTicket = {
                        id: 0,
                        raffleId: raffleId,
                        returnedGroup: $scope.returned.returnedGroup,
                        ticketReturnedNumbers: [currentTicketNumber]
                    };

                    var error = '';
                    error += self.validateNumberInList(currentTicketNumber);

                    if (error !== '') {
                        self.clearTicket();
                        $scope.ticketNumber.clientId = currentTicketNumber.clientId;
                        alertify.showError('Alerta', error);
                        return;
                    }
                    if (verify(returnedTicket)) {
                        return;
                    }

                    $("#ticketAllocationNumber").val('');
                    self.ticketReaderString = '';
                    currentTicketNumber.clientDesc = $('#cliientIdDropdown option:selected').text();;
                    $scope.returned.ticketReturnedNumbers.push(currentTicketNumber);

                    setTimeout(function () {
                        self.clearTicket();
                        $scope.ticketNumber.clientId = currentTicketNumber.clientId;
                        $scope.ticketNumber.clientDesc = currentTicketNumber.clientDesc;
                        $scope.$apply();
                        alertify.success('Billete agregado correctamente!', 1);
                    }, 0);
                } catch (e) { }
            } else {
                if (event.keyCode === 189 || event.keyCode === 219) {
                    self.ticketReaderString += '-';
                } else {
                    var char = String.fromCharCode(event.keyCode);
                    var reg = /^\d+$/;
                    if (reg.test(char)) {
                        self.ticketReaderString += char;
                    }
                }
            }
        }

        $scope.addNumber = function () {
            if (self.validateTicketNumber($scope.ticketNumber) === false) {
                return;
            }

            var returnedTicket = {
                id: 0,
                raffleId: $scope.returned.raffleId,
                returnedGroup: $scope.returned.returnedGroup,
                ticketReturnedNumbers: [$scope.ticketNumber],
                ClientId: $scope.ticketNumber.clientId
            };
            window.loading.show();
            $.ajax({
                type: 'POST',
                dataType: 'json',
                url: 'ticket/ticketReturnedApi/verify',
                data: returnedTicket,
                success: function (data) {
                    window.loading.hide();
                    if (data.result === true) {
                        var clientId = $scope.ticketNumber.clientId;
                        if ($scope.editingTicketNumber !== null) {
                            $scope.returned.ticketReturnedNumbers.forEach(function (number, i) {
                                if ($scope.editingTicketNumber !== null) {
                                    if (number.$$hashKey === $scope.editingTicketNumber.$$hashKey) {
                                        $scope.ticketNumber.clientDesc = $('#cliientIdDropdown').select2('data')[0].text;
                                        $scope.returned.ticketReturnedNumbers[i] = $scope.ticketNumber;
                                        $scope.editingTicketNumber = null;
                                    }
                                }
                            });
                        } else {
                            $scope.ticketNumber.clientDesc = $('#cliientIdDropdown').select2('data')[0].text;
                            $scope.returned.ticketReturnedNumbers.push($scope.ticketNumber);
                        }

                        setTimeout(function () {
                            self.clearTicket();
                            $scope.ticketNumber.clientId = clientId;
                            $scope.$apply();
                            alertify.success('Billete guardado correctamente!');
                        }, 0);
                    } else {
                        if (data.message != '') {
                            alertify.showError('Alerta', data.message);
                        } else {
                            var message = "";
                            data.object.forEach(function (msg) {
                                message += msg + ' <br/>'
                            });
                            alertify.showError('Alerta', message);
                        }
                    }
                }
            });
        }

        $scope.returned = {
            raffleId: undefined,
            returnedGroup: undefined,
            ticketReturnedNumbers: []
        };

        this.loadData();
        this.clearTicket();
    }

    function verify(c) {
        var state = false;
        $.ajax({
            type: 'POST',
            dataType: 'json',
            url: 'ticket/ticketReturnedApi/verify',
            data: c,
            success: function (data) {
                window.loading.hide();
                if (data.result === true) {
                    state = true;
                    var clientId = $scope.ticketNumber.clientId;
                    if ($scope.editingTicketNumber !== null) {
                        $scope.returned.ticketReturnedNumbers.forEach(function (number, i) {
                            if ($scope.editingTicketNumber !== null) {
                                if (number.$$hashKey === $scope.editingTicketNumber.$$hashKey) {
                                    $scope.ticketNumber.clientDesc = $('#cliientIdDropdown').select2('data')[0].text;
                                    $scope.returned.ticketReturnedNumbers[i] = $scope.ticketNumber;
                                    $scope.editingTicketNumber = null;
                                }
                            }
                        });
                    } else {
                        $scope.ticketNumber.clientDesc = $('#cliientIdDropdown').select2('data')[0].text;
                        $scope.returned.ticketReturnedNumbers.push($scope.ticketNumber);
                    }

                    setTimeout(function () {
                        self.clearTicket();
                        $scope.ticketNumber.clientId = clientId;
                        $scope.$apply();
                        alertify.success('Billete guardado correctamente!');
                    }, 0);
                } else {
                    state = false;
                    if (data.message != '') {
                        alertify.showError('Alerta', data.message);
                    } else {
                        var message = "";
                        data.object.forEach(function (msg) {
                            message += msg + ' <br/>'
                        });
                        alertify.showError('Alerta', message);
                    }
                }
            }
        });
        return state;
    }
})();
