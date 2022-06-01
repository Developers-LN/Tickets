/**=========================================================
 * Module: RaffleCreateController.js
 =========================================================*/

(function () {
    'use strict';

    angular
        .module('naut')
        .controller('RaffleCreateController', RaffleCreateController);

    RaffleCreateController.$inject = ['$scope', '$rootScope', '$state', '$stateParams'];
    function RaffleCreateController($scope, $rootScope, $state, $stateParams) {
        /*jshint validthis:true*/
        var self = this;
        this.getRaffleData = function () {
            $.when(
                $.ajax($rootScope.serverUrl + 'ticket/raffleApi/getRaffle?id=' + $stateParams.raffleId),
                $.ajax($rootScope.serverUrl + 'ticket/catalogApi/getRaffleStatuSelect'),
                $.ajax($rootScope.serverUrl + 'ticket/catalogApi/getRaffleCommoditySelect'),
                $.ajax($rootScope.serverUrl + 'ticket/prospectApi/getTicketProspectSelect?statu=' + 23/*Aprobado*/),
                $.ajax($rootScope.serverUrl + 'ticket/prospectApi/getPoolsProspectSelect?statu=' + 23/*Aprobado*/)
            ).then(function (raffleResponse, statuResponse, commodityResponse, ticketProspectResponse, poolProspectResponse) {
                window.loading.hide();
                if (statuResponse[1] == 'success') {
                    $scope.raffleStatus = statuResponse[0].object;
                }
                if (commodityResponse[1] == 'success') {
                    $scope.commoditys = commodityResponse[0].object;
                }
                if (ticketProspectResponse[1] == 'success') {
                    $scope.ticketProspects = ticketProspectResponse[0].object;
                }
                if (poolProspectResponse[1] == 'success') {
                    $scope.poolProspects = poolProspectResponse[0].object;
                }
                if (raffleResponse[1] == 'success') {
                    $scope.raffle = raffleResponse[0].object;
                    if ($stateParams.raffleId > 0) {
                        $scope.raffle.raffleDate = new Date($scope.raffle.raffleDateLong);
                        $scope.raffle.startReturnDate = new Date($scope.raffle.startReturnDateLong);
                        $scope.raffle.endReturnDate = new Date($scope.raffle.endReturnDateLong);
                        $scope.raffle.endAllocationDate = new Date($scope.raffle.endAllocationDateLong);

                        $scope.raffle.startDate = $scope.raffle.startReturnDate;
                        $scope.raffle.startTime = $scope.raffle.startReturnDate;

                        $scope.raffle.endDate = $scope.raffle.endReturnDate;
                        $scope.raffle.endTime = $scope.raffle.endReturnDate;

                        $scope.raffle.endAllocationDateOnly = $scope.raffle.endAllocationDate;
                        $scope.raffle.endAllocationTime = $scope.raffle.endAllocationDate;
                        $scope.changeRaffleDate();
                        $scope.prospectChange();
                    }
                    $scope.$apply();
                }
                window.setTimeout(function () {
                    $scope.$apply();
                }, 100);
            });
        }

        $scope.changeRaffleDate = function () {
            try {
                var date = $scope.raffle.raffleDate;

                var maxdate = new Date(date);
                maxdate.setDate(date.getDate());
                maxdate.setHours(23);
                $scope.maxReturnedDate = maxdate;
            } catch (e) {
                $scope.maxReturnedDate = new Date();
            }
        }

        $scope.prospectChange = function () {
            $scope.ticketProspects.forEach(function (prospect) {
                if (prospect.value == $scope.raffle.ticketProspectId) {
                    $scope.expirateDate = new Date(prospect.expirateDate);
                }
            });
        }

        this.validate = function (raffle) {
            var error = '', isReq = ' es un campo requerido. <br>';
            if (raffle.name == undefined) {
                error += 'Nombre' + isReq;
            }
            if (raffle.raffleDate == undefined) {
                error += 'Fecha del sorteo' + isReq;
            }
            if (raffle.ticketProspectId == undefined || raffle.ticketProspectId <= 0) {
                error += 'Prospecto de billetes' + isReq;
            }
            if (raffle.commodityId == undefined || raffle.commodityId <= 0) {
                error += 'Mercanc&iacute;a' + isReq;
            }
            if (raffle.startReturnDate == undefined) {
                error += 'Fecha inicio devolucion' + isReq;
            }
            if (raffle.endReturnDate == undefined) {
                error += 'Fecha fin devolucion' + isReq;
            }
            if (raffle.endAllocationDate == undefined) {
                error += 'Fecha fin de asignaci&oacute;n' + isReq;
            }
            if (raffle.statu == undefined || raffle.statu <= 0) {
                error += 'Status' + isReq;
            }
            if (error !== '') {
                alertify.showError('Alerta', error);
            }
            return error === '';
        }

        $scope.saveRaffleForm = function () {
            try {
                if ($scope.raffle.startDate === undefined || $scope.raffle.startTime === undefined) {
                    $scope.raffle.startReturnDate = undefined;
                } else {
                    $scope.raffle.startReturnDate = $rootScope.parseDate($scope.raffle.startDate, $scope.raffle.startTime).toJSON();
                }
                if ($scope.raffle.endDate === undefined || $scope.raffle.endTime === undefined) {
                    $scope.raffle.endReturnDate = undefined;
                } else {
                    $scope.raffle.endReturnDate = $rootScope.parseDate($scope.raffle.endDate, $scope.raffle.endTime).toJSON();
                }
                if ($scope.raffle.raffleDate === undefined) {
                    $scope.raffle.raffleDate = undefined;
                } else {
                    $scope.raffle.raffleDate = $rootScope.parseDate($scope.raffle.raffleDate, $scope.raffle.raffleDate).toJSON();
                }
                if ($scope.raffle.endAllocationDateOnly === undefined || $scope.raffle.endAllocationTime === undefined) {
                    $scope.raffle.endAllocationDate = undefined;
                } else {
                    $scope.raffle.endAllocationDate = $rootScope.parseDate($scope.raffle.endAllocationDateOnly, $scope.raffle.endAllocationTime).toJSON();
                }
            } catch (e) { }
            if (self.validate($scope.raffle) === false) {
                return;
            }
            window.loading.show();
            $.ajax({
                type: 'POST',
                dataType: 'json',
                url: $rootScope.serverUrl + 'ticket/raffleApi/saveRaffle',
                data: $scope.raffle,
                success: function (data) {
                    window.loading.hide();
                    if (data.result === true) {
                        alertify.success(data.message);
                        $state.go('app.solteoList');
                    } else {
                        alertify.alert(data.message);
                    }
                }
            });
        }

        self.getRaffleData();
    }
})();
