/**=========================================================
 * Module: IdentifyAwardController.js
 =========================================================*/

(function () {
    'use strict';

    angular
        .module('naut')
        .controller('IdentifyAwardController', IdentifyAwardController);

    IdentifyAwardController.$inject = ['$scope', '$state', '$rootScope', '$stateParams'];
    function IdentifyAwardController($scope, $state, $rootScope, $stateParams) {
        var self = this;

        $('#phoneNumber').mask("(999) 999-9999");

        $scope.totalFraction = 0;
        $scope.totalValue = 0;
        $scope.totalNumber = 0;
        $scope.totalGeneral = 0;
        $scope.pricePerFraction = 0;

        $scope.WinnerDocument = "";
        $scope.WinnerName = "";
        $scope.WinnerPhone = "";
        $scope.AddWinner = 0;
        $scope.GenderId = 0;

        $scope.BachId = 0;

        this.clearIdentify = function () {
            $scope.identifyBach = {
                Id: 0,
                RaffleId: undefined,
                ClientId: undefined,
                WinnerId: undefined,
                DocumentType: undefined,
                DocumentNumber: undefined,
                WinnerName: undefined,
                GenderId: undefined,
                Notes: undefined,
                WinnerPhone: undefined,
                IdentifyNumbers: [],
                Type: $rootScope.moduleCanDelete == '' ? 4007 : 4006
            };
        }

        self.seach = function (array, id) {
            var i
            for (i = 0; i < array.length; i++) {
                if (array[i].Id == id) {
                    return array[i];
                }
            }
            return -1;
        }

        self.getPrice = function (array, id) {
            var i;
            for (i = 0; i < array.length; i++) {
                if (array[i].PriceId == id) {
                    return array[i];
                }
            }
            return -1;
        }

        $scope.GetFractionPrice = function () {
            if ($scope.identifyBach.RaffleId != undefined && $scope.identifyBach.ClientId != undefined
                && $scope.identifyBach.RaffleId != 0 && $scope.identifyBach.ClientId != 0) {

                var currentClient = self.seach($scope.clients, $scope.identifyBach.ClientId);
                var currentSorteo = self.seach($scope.raffles, $scope.identifyBach.RaffleId);
                var currentPrice = self.getPrice(currentSorteo.Prices, currentClient.PriceId);
                $scope.clientFractionPrice = currentPrice;
                $scope.identifyBach.IdentifyNumbers.currentPrice = currentPrice;
            }
        }

        this.clearNumber = function (number) {
            $scope.editingNumber = number ? number : null;
            $scope.number = {
                Id: number ? number.Id : 0,
                NumberId: number ? number.NumberId : undefined,
                NumberDesc: '',
                FractionFrom: number ? number.FractionFrom : undefined,
                FractionTo: number ? number.FractionTo : undefined,
                IdentityAwards: [],
                Type: $rootScope.moduleCanDelete == '' ? 4007 : 4006,
            };
        }

        this.ticketReaderString = '';

        $scope.verifyTicketNumber = function (event) {
            if (eval.ctrlKey == true) {
                e.stopPropagation();
                e.preventDefault();
                e.returnValue = false;
                e.cancelBubble = true;
                return false;
            }
            if (event.keyCode === 13) {
                var codeRead = self.ticketReaderString;
                try {
                    var codeReadSplit = codeRead.split('-');
                    self.ticketReaderString = '';
                    if (codeReadSplit.length < 3) {
                        return;
                    }
                    var number = {
                        Id: 0,
                        NumberId: Number(codeReadSplit[1]),
                        NumberDesc: '',
                        FractionFrom: Number(codeReadSplit[2]),
                        FractionTo: Number(codeReadSplit[3]),
                        Type: $rootScope.moduleCanDelete == '' ? 4007 : 4006,
                        IdentityAwards: [],
                    };
                    number.RaffleId = Number(codeReadSplit[0]);

                    var error = '';
                    error += self.validateNumberInList(number);

                    if (error !== '') {
                        self.clearNumber();
                        alertify.showError('Alerta', error);
                        return;
                    }
                    $scope.number = number;
                    $scope.$apply();
                    $scope.editingNumber = null;
                    window.loading.show();
                    $.ajax({
                        type: 'POST',
                        dataType: 'json',
                        url: 'TicketAllocation/ValidateNumberAward',
                        data: { awardTicket: number, clientId: 0 },
                        success: function (data) {
                            window.loading.hide();
                            if (data.result === true) {
                                error += self.validateNumberInList(number);
                                if (error !== '') {
                                    self.clearNumber();
                                    alertify.showError('Alerta', error);
                                    return;
                                }
                                self.saveAwardNumber(number);
                            } else {
                                if (data.message) {
                                    alertify.showError('Alerta', message);
                                } else {
                                    var message = '';
                                    data.messages.forEach(function (msg) {
                                        message += msg + ' <br/>'
                                    });
                                    alertify.showError('Alerta', message);
                                }
                                window.loading.hide();
                            }
                        }
                    });
                } catch (e) {
                }
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

        this.loadIdentifyData = function () {
            window.loading.show();
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: 'TicketAllocation/GetIdentifyData?identifyId=' + $stateParams.identifyId + '&typeIdentify=' + 5898,
                success: function (data) {
                    $scope.clients = data.clients;
                    $scope.raffles = data.raffles;
                    $scope.winners = data.winners;
                    $scope.documentTypes = data.documentTypes;
                    $scope.genders = data.genders;

                    if (data.identifyBach !== null) {
                        $scope.BachId = data.identifyBach.Id;
                        $scope.identifyBach = data.identifyBach;
                        $scope.IdentityAwards = [];
                        $scope.identifyBach.IdentifyNumbers.forEach(function (i) {
                            $scope.totalFraction += i.Fractions;
                            $scope.totalValue += i.Total;
                            awardDetail(i.NumberId, i.RaffleId, i.FractionFrom, i.FractionTo);
                        });

                        $scope.totalNumber = data.identifyBach.IdentifyNumbers.length;
                    } else {
                        self.clearIdentify();
                    }
                    window.loading.hide();
                    $scope.$apply();
                    $rootScope.createSelect2();
                }
            });
        }

        this.validateBach = function (identifyBach) {
            var error = '', isReq = ' es un campo requerido. <br>';
            if (identifyBach.RaffleId === undefined) {
                error += 'Sorteo' + isReq;
            }
            if (identifyBach.ClientId === undefined) {
                error += 'Cliente' + isReq;
            }
            if (identifyBach.WinnerId == 0 || identifyBach.WinnerId == undefined && $scope.AddWinner != 0) {
                if (identifyBach.DocumentNumber === undefined) {
                    error += 'Cédula' + isReq;
                }
                if (identifyBach.WinnerName === undefined) {
                    error += 'Nombre' + isReq;
                }
                if (identifyBach.GenderId === undefined) {
                    error += 'Sexo' + isReq;
                }
                if (identifyBach.WinnerPhone === undefined) {
                    error += 'Teléfono' + isReq;
                }
            }
            if (identifyBach.WinnerId == 0 || identifyBach.WinnerId == undefined && $scope.AddWinner == 0) {
                error += 'Ganador' + isReq;
            }
            if (error !== '') {
                alertify.showError('Alerta', error);
            }
            return error === '';
        }

        this.validateNumber = function (number) {
            var error = '', isReq = ' es un campo requerido. <br>';
            if (number.NumberId === undefined) {
                error += 'Numero' + isReq;
            }
            if (number.FractionFrom === undefined) {
                error += 'Fraccion Desde' + isReq;
            }
            if (number.FractionTo === undefined) {
                error += 'Fraccion Hasta' + isReq;
            }
            if (error !== '') {
                alertify.showError('Alerta', error);
            }
            return error === '';
        }

        this.validateNumberInList = function (number) {
            var message = '';
            for (var fraction = number.FractionFrom; fraction <= number.FractionTo; fraction++) {
                if ($scope.identifyBach.IdentifyNumbers.some(function (n) {
                    return fraction >= n.FractionFrom && fraction <= n.FractionTo && (n.NumberId == number.NumberId || n.NumberDesc == number.NumberId);
                }) === true) {
                    message += fraction + ', ';
                }
            }
            if (message == '') {
                return '';
            } else {
                return 'La fracci&oacute;n ' + message + ' del billete ' + number.NumberId + ' esta agregada en el listado. <br/>';
            }
        }

        $scope.editNumber = function (number) {
            if (number) {
                $scope.editingNumber = number;
            } else {
                $scope.editingNumber = null;
            }
            self.clearNumber(number);
        }

        $scope.deleteNumber = function (number) {
            // confirm dialog
            alertify.confirm("&iquest;Desea borrar este Numero?", function (e) {
                if (e) {
                    setTimeout(function () {
                        for (var i = 0; i < $scope.IdentityAwards.length; i++) {
                            if (parseInt(number.NumberId) == $scope.IdentityAwards[i].AwardNumber && number.FractionFrom == $scope.IdentityAwards[i].FractionFrom && number.FractionTo == $scope.IdentityAwards[i].FractionTo) {
                                $scope.totalValue -= $scope.IdentityAwards[i].TotalValue;
                                $scope.totalFraction -= $scope.IdentityAwards[i].Fractions;
                                $scope.IdentityAwards.splice(i, 1);
                                i--;
                            }
                        }

                        $scope.identifyBach.IdentifyNumbers = $scope.identifyBach.IdentifyNumbers.filter(function (item) {
                            return item.$$hashKey !== number.$$hashKey;
                        });

                        $scope.$apply();
                        showTotalValue();
                        showFractionTotal();
                        alertify.success('Numero borrado correctamente!');
                    }, 0);
                }
            });
        }

        $scope.deleteIdentifyNumber = function (number) {
            // confirm dialog
            alertify.confirm("&iquest;Desea borrar este Numero?", function (e) {
                if (e) {
                    $.ajax({
                        type: 'POST',
                        dataType: 'json',
                        url: 'TicketAllocation/IdentifyNumberDelete',
                        data: number,
                        success: function (data) {
                            if (data.result === true) {

                                setTimeout(function () {
                                    $scope.identifyBach.IdentifyNumbers = $scope.identifyBach.IdentifyNumbers.filter(function (item) {
                                        return (item.Id == number.Id && item.NumberDesc == number.NumberDesc && item.FractionFrom == number.FractionFrom && item.FractionTo == number.FractionTo) == false;
                                    });
                                    $scope.$apply();
                                    alertify.success(data.message);
                                }, 0);
                            } else {
                                console.log(data.message);
                            }
                        }
                    });
                }
            });
        }

        $scope.addNumber = function () {
            if (self.validateNumber($scope.number) === false) {
                return;
            }
            var error = '';
            error += self.validateNumberInList($scope.number);

            if (error !== '') {
                self.clearNumber();
                alertify.showError('Alerta', error);
                return;
            }
            $scope.number.RaffleId = $scope.identifyBach.RaffleId;

            window.loading.show();
            $.ajax({
                type: 'POST',
                dataType: 'json',
                url: 'TicketAllocation/ValidateNumberAward?number=' + $scope.number + '&ClientId=' + $scope.identifyBach.ClientId,
                data: $scope.number,
                success: function (data) {
                    window.loading.hide();
                    if (data.result === true) {
                        self.saveAwardNumber($scope.number);

                        $scope.IdentityAwards = [];
                        $scope.identifyBach.IdentifyNumbers.forEach(function (i) {
                            awardDetail(i.NumberId, i.RaffleId, i.FractionFrom, i.FractionTo);
                        });

                        $scope.totalNumber = $scope.identifyBach.IdentifyNumbers.length;
                        $scope.$apply();
                        showTotalValue();
                        showFractionTotal();
                    } else {
                        if (data.message) {
                            alertify.showError('Alerta', message);
                        } else {
                            var message = '';
                            data.messages.forEach(function (msg) {
                                message += msg + ' <br/>'
                            });
                            alertify.showError('Alerta', message);
                        }
                        window.loading.hide();
                    }
                }
            });
        }

        this.saveAwardNumber = function (currentNumber) {

            var error = '';
            error += self.validateNumberInList(currentNumber);

            if (error !== '') {
                self.clearNumber();
                alertify.showError('Alerta', error);
                return;
            }

            if ($scope.editingNumber !== null) {
                $scope.identifyBach.IdentifyNumbers.forEach(function (number, i) {
                    if ($scope.editingNumber !== null) {
                        if (number.$$hashKey === $scope.editingNumber.$$hashKey) {
                            $scope.number.NumberDesc = $scope.number.NumberId;
                            $scope.identifyBach.IdentifyNumbers[i] = $scope.number;
                            $scope.editingNumber = null;
                        }
                    }
                });
            } else {
                currentNumber.NumberDesc = currentNumber.NumberId;
                $scope.identifyBach.IdentifyNumbers.push(currentNumber);
            }
            setTimeout(function () {
                $scope.identifyBach.IdentifyNumbers = $scope.identifyBach.IdentifyNumbers.filter(function (n) {
                    return $scope.identifyBach.IdentifyNumbers.some(function (i) {
                        return i.FractionFrom == n.FractionFrom && i.FractionTo == n.FractionTo && i.$$hashKey != n.$$hashKey && (i.NumberId == n.NumberId || i.NumberDesc == n.NumberId);
                    }) == false;
                });
                self.clearNumber();
                $scope.$apply();
                alertify.success('Premio guardado correctamente!');
                self.ticketReaderString = '';
            }, 0);
        }

        $scope.cancelNumber = function () {
            self.clearNumber();
        }

        $scope.saveForm = function () {
            window.loading.show();
            if (self.validateBach($scope.identifyBach) === false) {
                window.loading.hide();
                return;
            }
            $.ajax({
                type: 'POST',
                dataType: 'json',
                url: 'TicketAllocation/IdentifyAwardLight',
                data: $scope.identifyBach,
                success: function (data) {
                    if (data.result === true) {
                        alertify.success(data.message);
                        window.location.href = '/#/ticket/identifybachdetail/' + data.bachId;
                        window.loading.hide();
                    } else {
                        alertify.alert(data.message);
                        window.loading.hide();
                    }
                }
            });
        }

        $scope.verifyAwardNumber = function () {
            window.loading.show();
            $.ajax({
                type: 'POST',
                dataType: 'json',
                url: 'TypesAward/AwardNumber',
                data: $scope.awardNumber,
                success: function (data) {
                    window.loading.hide();
                    $scope.$apply();
                }
            });
        }

        this.loadIdentifyData();

        this.clearNumber();

        this.barcodeReader = function (e) {
            if (e.which == 17) {
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

        function showFractionTotal() {
            try {
                var totalFractions = 0;
                $scope.identifyBach.IdentifyNumbers.forEach(function (i) {
                    totalFractions += (i.FractionTo - i.FractionFrom + 1);
                });
                $scope.totalFraction = totalFractions;
                return totalFractions;
            } catch (e) {
                return 0;
            }
        }

        function showTotalValue() {
            var totalc = 0;
            $scope.IdentityAwards.forEach(function (i) {
                totalc += i.TotalValue;
            });
            $scope.totalValue = totalc;
            return totalc;
        }

        $scope.editingNumber = null;
        function awardDetail(number, raffleId, fractionFrom, fractionTo) {
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: $rootScope.serverUrl + 'ticket/ticketAllocationApi/awardNumberDetails?number=' + number + '&raffleId=' + raffleId + '&fractionFrom=' + fractionFrom + '&fractionTo=' + fractionTo,
                success: function (response) {
                    $scope.numberDetails = response.object;

                    var t = new GetAwardsObj($scope.numberDetails);
                    for (var i = 0; i < t.length; i++) {
                        $scope.IdentityAwards.push(t[i]);
                        $scope.totalValue += t[i].TotalValue;
                    }
                    $scope.$apply();
                }
            });
        }

        $scope.AddWinnerBTN = function () {
            if ($scope.AddWinner == 0) {
                $scope.AddWinner = 1;
                $scope.identifyBach.DocumentNumber = undefined;
                $scope.identifyBach.DocumentType = undefined;
                $scope.identifyBach.WinnerName = undefined;
                $scope.identifyBach.WinnerPhone = undefined;
                $scope.identifyBach.GenderId = undefined
            }
            else {
                $scope.AddWinner = 0;
            }
        }

        function GetAwardsObj(numberDetails) {

            var nArray = [];
            for (var i = 0; i < numberDetails.length; i++) {

                nArray.push({
                    Id: numberDetails[i].Id,
                    AwardNumber: numberDetails[i].AwardNumber,
                    FractionFrom: numberDetails[i].FractionFrom,
                    FractionTo: numberDetails[i].FractionTo,
                    Fractions: numberDetails[i].Fractions,
                    AwardName: numberDetails[i].AwardName,
                    AwardValue: numberDetails[i].AwardValue,
                    TotalValue: numberDetails[i].TotalValue
                });
            }
            return nArray;
        }

        $scope.typeDocument = function () {
            //CEDULA
            if ($scope.identifyBach.DocumentType == 5913) {
                $('#documentNumber').mask("999-9999999-9");
                $scope.maxLenght = 13;
            }
            //RNC
            else if ($scope.identifyBach.DocumentType == 5914) {
                $('#documentNumber').mask("999-99999-9");
                $scope.maxLenght = 11;
            }
            //PASAPORTE
            else {
                $('#documentNumber').unmask();
                $scope.maxLenght = 20;
            }
            $scope.identifyBach.DocumentNumber = undefined;
        }
    }
})();
