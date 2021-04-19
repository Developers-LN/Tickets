/**=========================================================
 * Module: AgencyListController.js
 =========================================================*/

(function() {
    'use strict';

    angular
        .module('naut')
        .controller('AgencyListController', AgencyListController);

    AgencyListController.$inject = ['$scope', '$rootScope', '$state'];
    function AgencyListController($scope, $rootScope, $state) {
        var self = this;
        this.getAgencyList = function () {
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: 'Agency/GetList',
                success: function (data) {
                    $scope.agencyList = data.agencys;
                    $scope.$apply();
                    $rootScope.dataTable();
                }
            });
        }
        $scope.deleteAgency = function (agency) {
            // confirm dialog
            alertify.confirm("&iquest;Desea borrar este agencia?", function (e) {
                if (e) {
                    $.ajax({
                        type: 'POST',
                        dataType: 'json',
                        url: 'Agency/Delete',
                        data: { agencyId: agency.Id },
                        success: function (data) {
                            if (data === true) {
                                $rootScope.destroyDataTable();
                                self.getAgencyList();
                                alertify.success('Agencia borrado correctamente!');
                            }
                        }
                    });
                }
            });
        }
        this.getAgencyList();
    }
})();
