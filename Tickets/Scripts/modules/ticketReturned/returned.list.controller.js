/**=========================================================
 * Module: ReturnedListController.js
 =========================================================*/

(function() {
    'use strict';

    angular
        .module('naut')
        .controller('ReturnedListController', ReturnedListController);

    ReturnedListController.$inject = ['$scope', '$state', '$rootScope'];
    function ReturnedListController($scope, $state, $rootScope) {
        $scope.raffleId = 0;
        $scope.groupClientId = 0;

        $rootScope.returnURL = '/#/ticket/Returneds';

        $scope.updateReturned = function () {
            window.loading.show();
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: $rootScope.serverUrl + 'ticket/ticketReturnedApi/getCreatedList?raffleId=' + $scope.raffleId + '&clientId=' + $scope.groupClientId,
                success: function (response) {
                    window.loading.hide();
                    $scope.returneds = response.object;
                    $rootScope.destroyDataTable();
                    $scope.$apply();
                    $rootScope.dataTable();
                }
            });
            
        }
        
        /*Mover Grupo*/
        $scope.showMoveGroupModal = function (returned) {
            window.loading.show();
            $.when(
                $.ajax($rootScope.serverUrl + 'ticket/ticketReturnedApi/getGroupListSelect?raffleId=' + returned.raffleId),
                $.ajax($rootScope.serverUrl + 'ticket/clientApi/getClientSelect?statu=2089')//Sorteos en planificacion
            ).then(function (groupResponse, clientsResponse) {
                window.loading.hide();
                $scope.groups = groupResponse[0].object;
                $scope.clientList = clientsResponse[0].object;
                $scope.currentReturned = returned;
                $('#showMoveGroupModal').modal('show');
                window.setTimeout(function () {
                    $scope.Group = returned.returnedGroup;
                    $scope.ClientId = returned.clientId;
                    $scope.$apply();
                }, 1);
            });
        }

        $scope.currentReturned = {};

        $scope.saveNewGroup = function () {
            var data = {
                group: $scope.Group,
                currentGroup: $scope.currentReturned.returnedGroup,
                clientId: $scope.ClientId,
                raffleId: $scope.currentReturned.raffleId
            }
            window.loading.show();
            $.ajax({
                type: 'POST',
                dataType: 'json',
                url: $rootScope.serverUrl + 'ticket/ticketReturnedApi/moveReturnedGroup',
                data: data,
                success: function (data) {
                    window.loading.hide();
                    if (data.result === false) {
                        alertify.alert(data.message);                        
                    } else {
                        alertify.success('Grupo movido correctamente');
                        $('#showMoveGroupModal').modal('hide');
                        $scope.updateReturned();
                    }
                }
            });
        }

        $('#showMoveGroupModal').on('hidden.bs.modal', function (e) {
            $scope.currentShowAwardTitle = '';
            $scope.currentShowAwardNumber = '';
            setTimeout(function () {
                $scope.$apply();
            }, 0);
        });

        $scope.deleteReturnedGroup = function (returned) {
            // confirm dialog
            alertify.confirm("&iquest;Desea borrar este grupo de devoluciones?", function (e) {
                if (e) {
                    $.ajax({
                        type: 'POST',
                        dataType: 'json',
                        url: $rootScope.serverUrl + 'ticket/ticketReturnedApi/deleteGroup',
                        data: returned,
                        success: function (data) {
                            if (data.result === true) {
                                $scope.updateReturned();
                                alertify.success(data.message);
                            }
                        }
                    });
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

        this.loadData();
    }
})();
