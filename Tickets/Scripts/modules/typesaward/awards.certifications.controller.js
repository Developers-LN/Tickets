/**=========================================================
 * Module: AwardsCertificationsController.js
 =========================================================*/

(function() {
    'use strict';

    angular
        .module('naut')
        .controller('AwardsCertificationsController', AwardsCertificationsController);

    AwardsCertificationsController.$inject = ['$scope', '$state', '$rootScope', '$http'];
    function AwardsCertificationsController($scope, $state, $rootScope, $http) {
        $rootScope.returnUrl = "/#";
        $rootScope.createSelect2();
        this.getData = function () {
            $http.get($rootScope.serverUrl + 'ticket/raffleApi/getGeneratedRaffles')
            .then(function (response) {
                $scope.raffles = response.data.object;
            });
        }

        $scope.updateRaffles = function (raffleId) {
            window.loading.show();
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: $rootScope.serverUrl + '/TypesAward/GetAwardsList?raffleId=' + raffleId,
                success: function (data) {
                    window.loading.hide();
                    if (raffleId == 0) {
                        $scope.raffles = data.raffles;
                    }
                    if (data.awards) {
                        $scope.awards = data.awards;
                    }
                    $rootScope.destroyDataTable();
                    $scope.$apply();
                    $rootScope.dataTable();
                }
            });
        }
        $scope.updateDelivered = function () {
            $scope.updateRaffles($scope.RaffleId);
        }
        this.getData();

        $scope.startWorkflowProspect = function (award) {
            // confirm dialog
            alertify.confirm("&iquest;Desea enviar este premio a un proceso de aprobaci&#243;n?", function (e) {
                if (e) {
                    window.loading.show();
                    $.ajax({
                        type: 'POST',
                        dataType: 'json',
                        url: $rootScope.serverUrl + 'ticket/workflowApi/sendAwardCertificationToWorkflow',
                        data: {
                            Id : award.INumberId,
                            NumberId: award.NumberId,
                            FractionFrom: award.FractionFrom,
                            FractionTo: award.FractionTo,
                            IdentifyBachId: award.IdentifyBachId,
                            Status : award.Status
                        },
                        success: function (response) {
                            window.loading.hide();
                            if (response.result === true) {
                                alertify.success(response.message);
                                $scope.updateRaffles($scope.RaffleId);
                            } else {
                                alertify.alert(response.message);
                            }
                        }
                    });
                }
            });
        }


        $scope.awardCertification = function (award) {
            // confirm dialog
            alertify.confirm("&iquest;Desea certificar este premio?", function (e) {
                if (e) {
                    $.ajax({
                        type: 'POST',
                        dataType: 'json',
                        url: 'TicketAllocation/CertificationAwardData',
                        data: {
                            iNumberId: award.INumberId,
                            number: award.Number,
                            FractionFrom: award.FractionFrom,
                            FractionTo: award.FractionTo,
                            raffleAwardId: award.RaffleAwardId,
                            fractions: award.Fractions
                        },
                        success: function (data) {
                            if (data.result === true) {
                                $scope.updateRaffles($scope.RaffleId);
                                alertify.success(data.message);
                                window.open('/Reports/AwardCertification?CertificationNumberId=' + data.certificateObject.Id);
                            }
                            else
                            {
                                alertify.alert(data.message);
                            }
                        }
                    });
                }
            });
        }
    }
})();
