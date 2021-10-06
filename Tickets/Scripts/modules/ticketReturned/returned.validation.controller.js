/**=========================================================
 * Module: ReturnedValidationController.js
 =========================================================*/

(function() {
    'use strict';

    angular
        .module('naut')
        .controller('ReturnedValidationController', ReturnedValidationController);

    ReturnedValidationController.$inject = ['$scope', '$state', '$rootScope'];
    function ReturnedValidationController($scope, $state, $rootScope) {
        $scope.raffleId = 0;
        $scope.clientId = 0;
        $rootScope.returnURL = '/#/ticket/returnedValidation';

        $scope.updateReturned = function () {
            window.loading.show();
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: $rootScope.serverUrl + 'ticket/ticketReturnedApi/getListtByValidation?raffleId=' + $scope.raffleId + '&clientId=' + $scope.clientId,
                success: function (response) {
                    window.loading.hide();
                    $scope.returnedValidates = response.object.validates.map(function (returned) {
                        returned.groupList = returned.returnedSubGroups.split(', ');
                        return returned;
                    });

                    $scope.returnedNonValidates = response.object.nonValidates.map(function (returned) {
                        returned.groupList = returned.returnedSubGroups.split(', ');
                        return returned;
                    });

                    $rootScope.destroyDataTable();
                    $scope.$apply();
                    $rootScope.dataTable();
                }
            });

        }
        $rootScope.dataTable();

        $scope.validateReturn = function (returned) {
            // confirm dialog
            alertify.confirm("&iquest;Desea validar esta devolucion?", function (e) {
                if (e) {
                    window.loading.show();
                    $.ajax({
                        type: 'POST',
                        dataType: 'json',
                        url: 'Cash/CreditNoteGroupApply',
                        data: { 
                            group: JSON.stringify(returned.groupList),
                            raffleId: returned.raffleId,
                            clientId: returned.clientId
                        },
                        success: function (data) {
                            if (data.result === true) {
                                window.loading.hide();
                                alertify.success(data.message);
                                window.open('/Reports/ReturnedDeatils?raffleId=' + returned.raffleId + '&clientId=' + returned.clientId + '&statu=4002');
                                $scope.updateReturned();
                            } else {
                                alertify.alert(data.message);
                            }
                        }
                    });
                }
            });
        }
        $scope.ShowHideMessage = 'Ocultar';
        $scope.show = true;
        $scope.ShowHideContent = function () {
            if ($scope.show == true) {
                $scope.ShowHideMessage = 'mostrar';
                $scope.show = false;
            } else {
                $scope.ShowHideMessage = 'Ocultar';
                $scope.show = true;
            }
        }

        $scope.ShowHideMessage2 = 'Ocultar';
        $scope.show2 = true;
        $scope.ShowHideContent2 = function () {
            if ($scope.show2 == true) {
                $scope.ShowHideMessage2 = 'mostrar';
                $scope.show2 = false;
            } else {
                $scope.ShowHideMessage2 = 'Ocultar';
                $scope.show2 = true;
            }
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

        this.loadData();
    }
})();
