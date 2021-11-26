/**=========================================================
 * Module: ClientCreateController.js
 =========================================================*/

(function() {
    'use strict';

    angular
        .module('naut')
        .controller('ClientCreateController', ClientCreateController);

    ClientCreateController.$inject = ['$scope', '$rootScope', '$state', '$stateParams'];
    function ClientCreateController($scope, $rootScope, $state, $stateParams) {
        var self = this;

        this.validateClientData = function (client) {
            var error = '', isReq = ' es un campo requerido. <br>';
            if (client.Name === undefined) {
                error += 'Nombre' + isReq;
            }
            if (client.DocumentNumber === undefined) {
                error += 'Cedula' + isReq;
            }
            if (client.PriceId === undefined) {
                error += 'Precio para el cliente' + isReq;
            }
            if (client.MaritalStatus === undefined) {
                error += 'Estado civil' + isReq;
            }/*
            if (client.Gender === undefined) {
                error += 'Sexo' + isReq;
            }*/
            if (client.Province === undefined) {
                error += 'Provincia' + isReq;
            }
            if (client.Town === undefined) {
                error += 'Municipio' + isReq;
            }
            if (client.Addres === undefined) {
                error += 'Direccion' + isReq;
            }
            if (client.Phone === undefined) {
                error += 'Telefono' + isReq;
            }
            if (client.Email === undefined) {
                error += 'E-Mail' + isReq;
            }
            if (client.CreditLimit === undefined) {
                error += 'Limite de Credito' + isReq;
            }
            if (client.AmountDeposit === undefined) {
                error += 'Monto de Fianza' + isReq;
            }
            if (client.Discount === undefined) {
                error += 'Descuento' + isReq;
            }
            if (client.ClientType === undefined) {
                error += 'Tipo de Cliente' + isReq;
            }
            if (client.GroupId === undefined) {
                error += 'Grupo' + isReq;
            }
            if (client.PreviousDebt === undefined ) {
                error += 'Deuda Anterior' + isReq;
            }
            if (error !== '') {
                alertify.showError('Alerta', error);
            }
            return error === '';
        }

        $scope.updateTown = function (provinceId) {
            $scope.towns = [];
            var selectedProvinceId = provinceId || Number($scope.client.Province);
            $scope.provinces.forEach(function (province) {
                if (province.Id == selectedProvinceId) {
                    $scope.towns = province.Towns;
                    $("#town-dropdown").select2("val", 0);
                    $("#section-dropdown").select2("val", 0);
                }
            });
        }
        $scope.updateDistritalTown = function (provinceId, townId) {
            var selectedProvinceId = provinceId || Number($scope.client.Province);
            var selectedTownId = townId || Number($scope.client.Town);
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

        this.clearClient = function () {
            $scope.client = {
                Id: 0,
                Addres: undefined,
                Birthday: undefined,
                PriceId: undefined,
                CheckIn: undefined,
                ClientType: undefined,
                Comment: undefined,
                CreateDate: undefined,
                CreateUser: undefined,
                CreditLimit: undefined,
                DocumentNumber: undefined,
                Discount: undefined,
                Email: undefined,
                Fax: '',
                Gender: undefined,
                GroupId: undefined,
                MaritalStatus: undefined,
                Name: undefined,
                Phone: undefined,
                Province: undefined,
                RNC: undefined,
                Section: undefined,
                Town: undefined,
                Tradename: undefined,
                AmountDeposit: undefined,
                AdminDocument: '',
                DepositDocument: '',
                PreviousDebt: 0
            };
        }

        $scope.saveClientForm = function () {
            try {
                $scope.client.Birthday = $rootScope.parseDate($scope.client.Birthday, $scope.client.Birthday).toJSON();
            } catch (e) { }
            if (self.validateClientData($scope.client) === false) {
                return;
            }
            window.loading.show();
            $.ajax({
                type: 'POST',
                dataType: 'json',
                url: 'Client/Create',
                data: $scope.client,
                success: function (data) {
                    window.loading.hide();
                    if (data === true) {
                        alertify.success('Cliente guardado correctamente!');
                        $state.go('app.clientList');
                    } else {
                        alertify.alert(data.message);
                    }
                }
            });
        }

        this.loadClientData = function () {
            window.loading.show();
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: 'Client/GetClientData?clientId=' + $stateParams.clientId,
                success: function (data) {
                    window.loading.hide();
                    $scope.provinces = data.provinces;
                    $scope.matritalStates = data.matritalStates;
                    $scope.prices = data.prices;
                    $scope.genders = data.genders;

                    $scope.clientTypes = data.clientTypes;
                    $scope.invoiceStatus = data.invoiceStatus;
                    $scope.clientGroups = data.clientGroups;

                    if ($stateParams.clientId == 0) {
                        self.clearClient();
                    } else {
                        data.client.Birthday = new Date(data.client.Birthday);
                        $scope.updateTown(data.client.Province);
                        $scope.updateDistritalTown(data.client.Province, data.client.Town);

                        $("#province-dropdown").select2("val", data.client.Province);
                        $("#town-dropdown").select2("val", data.client.Town);
                        $("#section-dropdown").select2("val", data.client.Section);
                        $scope.client = data.client;

                        if ($scope.client.AdminDocument != '') {
                            $scope.adminDocumentLabel = 'Documento Administrativo';
                        }
                        if ($scope.client.DepositDocument != '') {
                            $scope.filancyDocumentLabel = 'Documento de Fianza';
                        }
                    }
                    if (!$scope.$$phase) {
                        window.setTimeout(function () {
                            $scope.$apply();
                        }, 0);
                    }
                }
            });
        }

        this.loadClientData();
        $scope.uploadDocument = function (type) {
            if (type == 'admin') {
                $('#adminDocumentFile').trigger('click');
            } else {
                $('#financyDocumentFile').trigger('click');
            }
        }

        $scope.clearAdminDocumentLabel = function () {
            $scope.client.AdminDocument = '';
            $scope.adminDocumentLabel = '';
            $('#adminDocumentFile').val('');
            if (!$scope.$$phase) {
                window.setTimeout(function () {
                    $scope.$apply();
                }, 0);
            }
        }

        $('#adminDocumentFile').change(function (e) {
            if (e.originalEvent.srcElement.files.length === 0) {
                $scope.clearAdminDocumentLabel();
                return;
            }
            self.loadFileData(e, 'admin');
        });
        this.loadFileData = function (e, type) {
            for (var i = 0; i < e.originalEvent.srcElement.files.length; i++) {
                var file = e.originalEvent.srcElement.files[i];
                var reader = new FileReader();
                reader.onloadend = function () {
                    if (type == 'admin') {
                        $scope.client.AdminDocument = reader.result;
                        $scope.adminDocumentLabel = file.name;
                    } else {
                        $scope.client.DepositDocument = reader.result;
                        $scope.filancyDocumentLabel = file.name;
                    }
                    if (!$scope.$$phase) {
                        window.setTimeout(function () {
                            $scope.$apply();
                        }, 0);
                    }
                }
                reader.readAsDataURL(file);
            }
        }
        $scope.filancyDocumentLabel = '';
        $scope.adminDocumentLabel = '';
        $scope.clearFinancyDocumentLabel = function () {
            $scope.client.DepositDocument = '';
            $scope.filancyDocumentLabel = '';
            $('#financyDocumentFile').val('');
            if (!$scope.$$phase) {
                window.setTimeout(function () {
                    $scope.$apply();
                }, 0);
            }
        }
        $('#financyDocumentFile').change(function (e) {
            if (e.originalEvent.srcElement.files.length === 0) {
                $scope.clearFinancyDocumentLabel();
                return;
            }
            self.loadFileData(e, 'financy');
        });

        $rootScope.createSelect2();
    }
})();
