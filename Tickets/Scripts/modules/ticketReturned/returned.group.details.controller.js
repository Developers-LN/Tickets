/**=========================================================
 * Module: Returned.Group.Details.controller.js
 =========================================================*/

(function() {
    'use strict';

    angular
        .module('naut')
        .controller('ReturnedGroupDetailsController', ReturnedGroupDetailsController);

    ReturnedGroupDetailsController.$inject = ['$scope', '$state', '$rootScope', '$stateParams'];
    function ReturnedGroupDetailsController($scope, $state, $rootScope, $stateParams) {
        $scope.editMode = false;
        if ($rootScope.returnURL == '/#/ticket/Returneds' || !$rootScope.returnURL) {
            $scope.editMode = true;
        }
        $scope.returnBack = function () {
            if ($rootScope.returnURL == '/#/ticket/Returneds' || !$rootScope.returnURL) {
                window.location.href = '/#/ticket/Returneds';
            } else {
                window.location.href = '/#/ticket/returnedListQuery';
            }
        }
        var self = this;
        $scope.fractionTotal = 0;
        this.loadReturneds = function () {
            window.loading.show();
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: $rootScope.serverUrl + 'ticket/ticketReturnedApi/details?group=' + $stateParams.group + '&raffleId=' + $stateParams.raffleId,
                success: function (response) {
                    window.loading.hide();
                    if (response.result == true) {
                        $scope.returns = response.object.map(function (r) {
                            $scope.fractionTotal += r.fractionQuantity;
                            r.createDate = new Date(r.createDate);
                            return r;
                        });
                        $rootScope.destroyDataTable();
                        window.setTimeout(function () {
                            $scope.$apply();
                            $rootScope.dataTable();
                        }, 0);
                    } else {
                        alertify.alert(response.message);
                    }
                }
            });
        }

        var self = this;
        $scope.currentReturned = {};

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
                 $scope.$apply();
                 window.setTimeout(function () {
                     $scope.Group = returned.returnedGroup;
                     $scope.ClientId = returned.clientId;
                     $scope.$apply();
                 }, 0);
             });
        }

        $scope.saveNewGroup = function () {
           
            var data = {
                group: $scope.Group,
                returnedId: $scope.currentReturned.id,
                clientId: $scope.ClientId
            };

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
                        self.loadReturneds();
                    }
                }
            });
        }

        $('#showMoveGroupModal').on('hidden.bs.modal', function (e) {
            $scope.currentShowAwardTitle = '';
            $scope.currentShowAwardNumber = '';
            $scope.Group = undefined;
            $scope.ClientId = undefined;
            setTimeout(function () {
                $scope.$apply();
            }, 0);
        });

        $scope.deleteReturned = function (returned) {
            // confirm dialog
            alertify.confirm("&iquest;Desea borrar esta de devolucion?", function (e) {
                if (e) {
                    $.ajax({
                        type: 'POST',
                        dataType: 'json',
                        url: 'ticket/ticketReturnedApi/deleteNumber',
                        data: returned,
                        success: function (data) {
                            if (data.result === true) {
                                self.loadReturneds();
                                alertify.success(data.message);
                            } else {
                                alertify.alert(data.message);
                            }
                        }
                    });
                }
            });
        }

        this.loadReturneds();
    }
})();
