/**=========================================================
 * Module: ReprintCreateController.js
 =========================================================*/

(function() {
    'use strict';

    angular
        .module('naut')
        .controller('ReprintCreateController', ReprintCreateController);

    ReprintCreateController.$inject = ['$scope', '$state', '$rootScope', '$stateParams'];
    function ReprintCreateController($scope, $state, $rootScope, $stateParams) {
        var self = this;
        this.validateData = function (reprint) {
            var error = '', isReq = ' es un campo requerido. <br>';
            if (reprint.raffleId == undefined) {
                error += 'El Sorteo' + isReq;
            }
            if (reprint.clientId == undefined) {
                error += 'El Cliente' + isReq;
            }
            if (reprint.note == undefined) {
                error += 'El Observaci&oacute;n' + isReq;
            }
            if ($scope.reprint.ticketReprintNumbers.some(function (n) {
                return n.selected == true;
            }) == false) {
                error += 'Tienes que seleccionar al menos 1 numero.';
            }
            if (error !== '') {
                alertify.showError('Alerta', error);
            }
            return error === '';
        }

        $scope.returnTo = function () {
            if (Number($stateParams.reprintId) > 0) {
                window.location.href = '/#/ticket/suscriberDetails/' + $stateParams.reprintId;
            } else {
                if ($stateParams.allocationId > 0) {
                    window.location.href = '/#/ticket/review/allocation';
                } else {
                    window.location.href = '/#/ticket/reprintList';
                }
            }
        }

        this.getAllocationsByClient = function (raffleId, clientId) {
            if (raffleId === undefined || clientId === undefined) {
                return;
            }
            window.loading.show();
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: $rootScope.serverUrl + 'ticket/ticketAllocationApi/ticketAllocationListForPrint?raffleId=' + raffleId + '&clientId=' + clientId,
                success: function (response) {
                    window.loading.hide();
                    if (response.result == true) {
                        $scope.allocations = response.object;
                    } else {
                        alertify.alert(response.message);
                    }
                    window.setTimeout(function () {
                        $scope.$apply();
                    }, 0);
                }
            });
        }

        $scope.updateAllocationNumbers = function () {
            var selectedNumbers = $scope.reprint.ticketReprintNumbers;
            $scope.reprint.ticketReprintNumbers = [];
            $scope.allocations.forEach(function (allocation) {
                if ($('#allocation-' + allocation.id)[0].checked === true) {
                    allocation.ticketAllocationNumbers.forEach(function (number) {
                        number.selected = selectedNumbers.some(function (n) {
                            if (n.id == number.id) {
                                return n.selected;
                            }
                            return false;
                        });
                        $scope.reprint.ticketReprintNumbers.push(number);
                    });
                }
            });

            $rootScope.destroyDataTable();
            window.setTimeout(function () {
                $scope.$apply();
                $rootScope.dataTable();
            }, 0);
        }

        $scope.selectAllNumber = function (e) {
            $scope.reprint.ticketReprintNumbers.forEach(function (n, i) {
                $scope.reprint.ticketReprintNumbers[i].selected = e.target.checked;
            });
        }

        $scope.updateAllocations = function () {
            var raffleId = $scope.reprint.raffleId;
            var clientId = $scope.reprint.clientId;
            self.getAllocationsByClient(raffleId, clientId);
        }

        $scope.selectNumber = function (e, number) {
            $scope.reprint.ticketReprintNumbers.forEach(function (n, i) {
                if (n.id == number.id) {
                    $scope.reprint.ticketReprintNumbers[i].selected = e.target.checked;
                }
            });
        }

        $scope.saveTicketForm = function () {
            if (self.validateData($scope.reprint) === false) {
                return;
            }
            var numbers = [];
            $scope.reprint.ticketReprintNumbers = $scope.reprint.ticketReprintNumbers.filter(function (t) {
                numbers.push(t);
                return t.selected == true;
            });
            window.loading.show();
            $.ajax({
                type: 'POST',
                dataType: 'json',
                url: $rootScope.serverUrl + 'ticket/ticketPrintApi/save',
                data: $scope.reprint,
                success: function (response) {
                    window.loading.hide();
                    if (response.result === false) {
                        $scope.reprint.ticketReprintNumbers = numbers;
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

        this.getCreateData = function () {
            window.loading.show();
            $.when(
                $.ajax($rootScope.serverUrl + 'ticket/clientApi/getClientSelect?statu=2089'),/*Clientes aprobados*/
                $.ajax($rootScope.serverUrl + 'ticket/raffleApi/getRaffleSelect?statu=68'),//Sorteos en planificacion
                $.ajax($rootScope.serverUrl + 'ticket/ticketPrintApi/getReprint?id=' + $stateParams.reprintId)
            ).then(function (clientResponse, raffleResponse, reprintResponse) {
                window.loading.hide();
                if (clientResponse[1] == 'success') {
                    $scope.clients = clientResponse[0].object;
                }
                if (raffleResponse[1] == 'success') {
                    $scope.raffles = raffleResponse[0].object;
                }
                if (reprintResponse[1] == 'success') {
                    $scope.reprint = reprintResponse[0].object;
                    if ($stateParams.reprintId > 0) {
                        $scope.changeRaffle();
                    }
                    $rootScope.createSelect2();
                    $rootScope.dataTable();
                    $scope.$apply();
                }
                window.setTimeout(function () {
                    $scope.$apply();
                    $rootScope.createSelect2();
                }, 0);
            });
        }

        this.getAllocationDetails = function () {
            window.loading.show();
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: $rootScope.serverUrl + 'ticket/ticketAllocationApi/getTicketAllocation?id=' + $stateParams.allocationId,
                success: function (data) {
                    window.loading.hide();
                    //data.allocation.raffleDate = new Date(data.allocation.raffleDate);
                    $scope.allocationId = Number($stateParams.allocationId);
                    $scope.allocation = data.object;

                    $scope.reprint = {
                        id: 0,
                        raffleId: $scope.allocation.raffleId,
                        note: undefined,
                        clientId: $scope.allocation.clientId,
                        ticketReprintNumbers: [],
                        isPrint: true
                    };

                    $scope.allocation.ticketAllocationNumbers.forEach(function (number) {
                        number.selected = false;
                        $scope.reprint.ticketReprintNumbers.push(number);
                    });

                    //$rootScope.destroyDataTable();
                    window.setTimeout(function () {
                        $scope.$apply();
                        $rootScope.dataTable();
                    }, 0);
                }
            });
        }

        if ($stateParams.allocationId > 0) {
            self.getAllocationDetails();
        } else {
            $scope.allocationId = 0;
            self.getCreateData();
        }
    }
})();
