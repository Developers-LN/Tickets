/**=========================================================
 * Module: AgencyCreateController.js
 =========================================================*/

(function() {
    'use strict';

    angular
        .module('naut')
        .controller('AgencyCreateController', AgencyCreateController);

    AgencyCreateController.$inject = ['$scope', '$rootScope', '$state', '$stateParams'];
    function AgencyCreateController($scope, $rootScope, $state, $stateParams) {
        var self = this;
        this.validateAgencyData = function(agency) {
            var error = '', isReq = ' es un campo requerido. <br>';
            if (agency.Name === undefined) {
                error += 'Nombre' + isReq;
            }
            if (agency.Description === undefined) {
                error += 'Descripcion' + isReq;
            }
            if (agency.GroupId === undefined) {
                error += 'Grupo' + isReq;
            }
            if (agency.Province === undefined) {
                error += 'Provincia' + isReq;
            }
            if (agency.Town === undefined) {
                error += 'Municipio' + isReq;
            }
            if (agency.Email === undefined) {
                error += 'E-Mail' + isReq;
            }
            if (agency.Phone === undefined) {
                error += 'Telefono' + isReq;
            }
            if (agency.IntDate === undefined) {
                error += 'Fecha de Entrada' + isReq;
            }

            if (agency.Statu === undefined) {
                error += 'Estatus' + isReq;
            }
            if (error !== '') {
                alertify.showError('Alerta', error);
            }
            return error === '';
        }

        this.clearAgency = function () {
            $scope.agency = {
                Id: 0,
                Name: undefined,
                Description: undefined,
                EmployeeId: undefined,
                Province: undefined,
                Section: undefined,
                Town: undefined,
                Addres: undefined,
                Phone: undefined,
                Email: undefined,
                Fax: undefined,
                GroupId: undefined,
                IntDate: undefined,
                Statu: undefined
            };
        }

        $scope.updateTown = function (provinceId) {
            $scope.towns = [];
            var selectedProvinceId = provinceId || Number($scope.agency.Province);
            $scope.provinces.forEach(function (province) {
                if (province.Id == selectedProvinceId) {
                    $scope.towns = province.Towns;
                    $("#town-dropdown").select2("val", 0);
                    $("#section-dropdown").select2("val", 0);
                }
            });
        }

        $scope.updateDistritalTown = function (provinceId, townId) {
            var selectedProvinceId = provinceId || Number($scope.agency.Province);
            var selectedTownId = townId || Number($scope.agency.Town);
            $scope.provinces.forEach(function (province) {
                if (province.Id == selectedProvinceId) {
                    province.Towns.forEach(function (town) {
                        if (town.Id == selectedTownId) {
                            $scope.distTowns = town.DistTowns;
                            $("#section-dropdown").select2("val", 0);
                        }
                    });
                }
            });
        }

        $scope.saveAgencyForm = function () {
            try{
                $scope.agency.IntDate = $rootScope.parseDate($scope.agency.IntDate, $scope.agency.IntDate).toJSON();
            }catch(e){}
            if (self.validateAgencyData($scope.agency) === false) {
                return;
            }
            $.ajax({
                type: 'POST',
                dataType: 'json',
                url: 'Agency/Create',
                data: $scope.agency,
                success: function (data) {
                    if (data === true) {
                        alertify.success('Agencia guardada correctamente!');
                        $state.go('app.agencyList');
                    } else {
                        alertify.alert(data.message);
                    }
                }
            });
        }

        this.loadAgencyData = function () {
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: 'Agency/GetAgencyData?agencyId=' + $stateParams.agencyId,
                success: function (data) {
                    $scope.provinces = data.provinces;
                    $scope.groups = data.groups;
                    $scope.generalStatus = data.generalStatus;

                    $scope.employees = data.employees;
                    $rootScope.createSelect2();
                    if ($stateParams.agencyId == 0) {
                        self.clearAgency();
                    } else {
                        data.agency.IntDate = new Date(data.agency.IntDate);

                        $scope.updateTown(data.agency.Province);
                        $scope.updateDistritalTown(data.agency.Province, data.agency.Town);

                        $("#province-dropdown").select2("val", data.agency.Province);
                        $("#town-dropdown").select2("val", data.agency.Town);
                        $("#section-dropdown").select2("val", data.agency.Section);

                        $scope.agency = data.agency;
                    }
                    $scope.$apply();
                    window.setTimeout(function () {
                        $rootScope.createSelect2();
                    }, 0);
                }
            });
        }

        self.loadAgencyData();
    }
})();
