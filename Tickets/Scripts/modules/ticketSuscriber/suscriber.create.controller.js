/**=========================================================
 * Module: SuscriberCreateController.js
 =========================================================*/

(function() {
    'use strict';

    angular
        .module('naut')
        .controller('SuscriberCreateController', SuscriberCreateController);

    SuscriberCreateController.$inject = ['$scope', '$state', '$rootScope', '$stateParams'];
    function SuscriberCreateController($scope, $state, $rootScope, $stateParams) {
        var self = this;
        this.validateData = function (ticket) {
            if ($stateParams.suscriberId != 0) {
                return true;
            }
            var error = '', isReq = ' es un campo requerido. <br>';
            if (!ticket.clientId) {
                error += 'El cliente' + isReq;
            }
            if (error !== '') {
                alertify.showError('Alerta', error);
            }
            return error === '';
        }

        $scope.returnTo = function () {
            if (Number($stateParams.suscriberId) > 0) {
                window.location.href = '/#/ticket/suscriberDetails/' + $stateParams.suscriberId;
            } else {
                window.location.href = '/#/ticket/suscribers';
            }
        }

        $scope.verifyKeydown = function (e) {
            if (e.which === 13) {
                $scope.addNumber();
            }
        }

        this.validateNumber = function (ticket) {
            var error = '', isReq = ' es un campo requerido. <br/>';
            if ($('#ticketAllocateNumber')[0].checked === true) {
                if (ticket.number === undefined) {
                    error += 'El numero' + isReq;
                }
                if ($scope.suscriber.ticketSuscriberNumbers.some(function (number) {
                    return number.number === ticket.number
                }) === true) {
                    error += 'El numero ' + ticket.number + ' ya esta agregado. <br/>';
                }
            } else {
                if (ticket.numberFrom === undefined) {
                    error += 'El numero desde' + isReq;
                }
                if (ticket.numberTo === undefined) {
                    error += 'El numero hasta' + isReq;
                }
                var e = '';
                for (var number = ticket.numberFrom; number <= ticket.numberTo; number++) {
                    if ($scope.suscriber.ticketSuscriberNumbers.some(function (n) {
                        return n.number === number
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
                    $scope.suscriber.ticketSuscriberNumbers = $scope.suscriber.ticketSuscriberNumbers.filter(function (item) {
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

        $scope.addNumber = function () {
            if (self.validateNumber($scope.ticketNumber) === false) {
                return;
            }
            var numbers = [];
            if ($('#ticketAllocateNumber')[0].checked === false) {
                for (var number = $scope.ticketNumber.numberFrom; number <= $scope.ticketNumber.numberTo; number++) {
                    numbers.push({id: 0, number: number });
                }
            } else {
                numbers.push({id: 0, number: $scope.ticketNumber.number });
            }
            self.verifyTicketAllocation(numbers);
        }

        this.verifyTicketAllocation = function (numbers) {
            if (self.validateData($scope.suscriber) === false) {
                return;
            }
            var ticket = {
                ticketSuscriberNumbers: numbers
            }
            window.loading.show();
            $.ajax({
                type: 'POST',
                dataType: 'json',
                url: $rootScope.serverUrl + 'ticket/ticketSuscriberApi/verify',
                data: ticket,
                success: function (response) {
                    window.loading.hide();
                    if (response.result === false) {
                        alertify.showError('Alerta', response.message);
                    } else {
                        $scope.suscriber.ticketSuscriberNumbers = $scope.suscriber.ticketSuscriberNumbers.concat(numbers);

                        $rootScope.destroyDataTable();
                        setTimeout(function () {
                            self.clearNumber();
                            $scope.$apply();
                            $rootScope.dataTable();
                            alertify.success(response.message);
                        }, 0);
                    }
                }
            });
        }

        this.clearNumber = function () {
            $scope.ticketNumber.number = undefined;
            $scope.ticketNumber.numberFrom = undefined;
            $scope.ticketNumber.numberTo = undefined;
            
        }

        $scope.saveTicketForm = function () {
            if (self.validateData($scope.suscriber) === false) {
                return;
            }
            $.ajax({
                type: 'POST',
                dataType: 'json',
                url: $rootScope.serverUrl + 'ticket/ticketSuscriberApi/save',
                data: $scope.suscriber,
                success: function (response) {
                    if (response.result === false) {
                        alertify.alert(response.message);
                    }else{
                        alertify.success(response.message);
                        $scope.returnTo();
                    }
                }
            });
        }

        $('#ticketAllocateNumber').click(function () {
            $('#numberContainer').removeClass('hide');
            $('#rangeNumberContainer').addClass('hide');
        });

        $('#ticketAllocateRangeNumber').click(function () {
            $('#numberContainer').addClass('hide');
            $('#rangeNumberContainer').removeClass('hide');
        });

        this.loadData = function () {
            window.loading.show();
            $.when(
                $.ajax($rootScope.serverUrl + 'ticket/ticketSuscriberApi/get?id=' + $stateParams.id),
                $.ajax($rootScope.serverUrl + 'ticket/clientApi/getClientSelect?statu=2089')/*Clientes aprobados*/
            ).then(function (suscriberResponse, clientResponse) {
                window.loading.hide();
                if (clientResponse[1] == 'success') {
                    $scope.clients = clientResponse[0].object;
                }

                if (suscriberResponse[1] == 'success') {
                    $scope.suscriber = suscriberResponse[0].object;
                }
                window.setTimeout(function () {
                    $scope.$apply();
                    $rootScope.createSelect2();
                }, 0);
            });
        }
        this.loadData();
    }
})();
