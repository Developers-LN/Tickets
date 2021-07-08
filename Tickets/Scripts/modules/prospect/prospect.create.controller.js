/**=========================================================
 * Module: ProspectCreateController.js
 =========================================================*/

(function() {
    'use strict';

    angular
        .module('naut')
        .controller('ProspectCreateController', ProspectCreateController);

    ProspectCreateController.$inject = ['$scope', '$rootScope', '$state', 'editableOptions', '$sce', 'editableThemes'];
    function ProspectCreateController($scope, $rootScope, $state, editableOptions, $sce, editableThemes) {
        editableOptions.theme = 'bs3';
        editableThemes.bs3.inputClass = 'font-control input-sm';
        editableThemes.bs3.buttonsClass = 'btn-sm';
        editableThemes.bs3.submitTpl = '<button type="submit" class="btn btn-success"><span class="fa fa-check"></span></button>';
        editableThemes.bs3.cancelTpl = '<button type="button" class="btn btn-default" ng-click="$form.$cancel()">' +
                                         '<span class="fa fa-times text-muted"></span>' +
                                       '</button>';
        var self = this;
        /*jshint validthis:true*/
        this.validateProspectData = function(prospect) {
            var error = '', isReq = ' es un campo requerido. <br>';
            if (prospect.Name == '') {
                error += 'Nombre' + isReq;
            }
            if (self.validateNumenber(prospect.Price) === false) {
                error += 'Precio de venta' + isReq;
            }
            if (prospect.GroupId == '') {
                error += 'Grupo' + isReq;
            }
            if (self.validateNumenber(prospect.Production) === false) {
                error += 'Producci&oacute;n' + isReq;
            }
            if (self.validateNumenber(prospect.LeafNumber) === false) {
                error += 'Hojas por número' + isReq;
            }
            if (self.validateNumenber(prospect.LeafFraction) === false) {
                error += 'Fracci&oacute;n por hojas' + isReq;
            }
            /*if (self.validateNumenber(prospect.ExpirateDate) === false) {
	            error += 'Fecha de Expiraci&oacute;n' + isReq;
	        }*/
            if (self.validateNumenber(prospect.MaxReturnTickets) === false) {
                error += 'Porcentaje de devoluciones' + isReq;
            }
            if (self.validateNumenber(prospect.PercentageWinners) === false) {
                error += 'Porcentage de n&uacut;meros ganadores' + isReq;
            }
            if (self.validateNumenber(prospect.ImpresionType) === false) {
                error += 'Formato de Impresi&oacute;n' + isReq;
            }
            if (prospect.Statu == '') {
                error += 'Estado' + isReq;
            }
            if (error !== '') {
                alertify.showError('Alerta', error);
            }
            return error === '';
        }

        this.validatePrice = function(price) {
            var error = '', isReq = ' es un campo requerido. <br>';
            if (price.PriceId === undefined) {
                error += 'Tipo de Premio' + isReq;
            }
            if (price.TicketPrice === undefined) {
                error += 'Precio Billete' + isReq;
            }
            if (price.SeriePrice === undefined) {
                error += 'Precio Serie' + isReq;
            }
            if (price.FactionPrice === undefined) {
                error += 'Precio Fraccion' + isReq;
            }
            if (error !== '') {
                alertify.showError('Alerta', error);
            }
            return error === '';
        }

        $scope.saveProspectForm = function () {
            $scope.prospect.ExpirateDate = $rootScope.parseDate($scope.prospect.ExpirateDate, new Date()).toJSON();
            if (self.validateProspectData($scope.prospect) === false) {
                return;
            }
            self.saveFormProspect();
        }
        this.saveFormProspect = function () {
            window.loading.show();
            $.ajax({
                type: 'POST',
                dataType: 'json',
                url: 'Prospect/Create',
                data: $scope.prospect,
                success: function (data) {
                    if (data.Result === true) {
                        alertify.success(data.Message);
                        window.loading.hide();
                        $state.go('app.prospectlist');
                    } else {
                        alertify.alert(data.Message);
                        window.loading.hide();
                    }
                }
            });
        }

        this.validateAwardData = function (award) {
            var error = '', isReq = ' es un campo requerido. <br>';
            if (award.Name == '') {
                error += 'Nombre' + isReq;
            }

            if (self.validateNumenber(award.OrderAward) === false) {
                error += 'Orden' + isReq;
            }
            if (self.validateNumenber(award.Quantity) === false) {
                error += 'Cantidad' + isReq;
            }

            if (!award.ByFraction || award.ByFraction == '') {
                error += 'Por Fracci&#243;n' + isReq;
            }
            if (!award.TypesAwardId || award.TypesAwardId == '') {
                error += 'Tipo de premio' + isReq;
            }
            if (self.validateNumenber(award.Value) === false) {
                error += 'Valor' + isReq;
            }
            var editing = $scope.editingAward || {};
            if ($scope.prospect.Awards.some(function (item) { return item.OrderAward === award.OrderAward && item.$$hashKey !== editing.$$hashKey; }) == true) {
                error += 'El orden del premio esta duplicado.' + isReq;
            }
            if (error !== '') {
                alertify.showError('Alerta', error);
            }
            return error === '';
        }
        this.validateNumenber = function(number) {
            if (!number || Number(number) <= 0 || number == '') {
                return false
            }
            return true;
        }

        this.filterPriceList = function (selectedId) {
            if (!$scope.typePrices) {
                return;s
            }
            var selectedId = selectedId || 0;
            $scope.typePricesFiltered = $scope.typePrices.filter(function (item) {
                if ($scope.prospect.Prospect_Price.length === 0) {
                    return true;
                }
                return $scope.prospect.Prospect_Price.some(function (price) {
                    return Number(price.PriceId) === Number(item.value)
	                && Number(selectedId) !== Number(item.value);
                }) === false;
            });
        }

        this.geAlltProspectList = function () {
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: 'Prospect/Prospects',
                success: function (data) {
                    $scope.allProspectList = data.prospects;
                    $scope.prospectList = data.prospects;
                    $scope.groups = data.groups;
                    $scope.printTechniques = data.printTechniques;
                    $scope.prospectStatus = data.prospectStatus;
                    $scope.awardTypes = data.awardTypes;
                    $scope.typePrices = data.typePrices;
                    $scope.$apply();
                    $rootScope.dataTable();
                }
            });
        }
        $scope.clearPrice = function (price) {
            $scope.price = {
                Id: price ? price.Id : 0,
                PriceId: price ? price.PriceId : undefined,
                FactionPrice: price ? price.FactionPrice : undefined,
                TicketPrice: price ? price.TicketPrice : undefined,
                SeriePrice: price ? price.SeriePrice : undefined
            };
            if (price) {
                self.filterPriceList(price.PriceId);
            } else {
                $scope.editingPrice = null;
                self.filterPriceList();
            }
        }
        this.clearProspect = function (prospect) {
            prospect = prospect || {};
            return {
                Id: prospect.Id || 0,
                Name: prospect.Name || '',
                Description: prospect.Description || '',
                GroupId: prospect.GroupId || '',
                Production: prospect.Production || '',
                LeafNumber: prospect.LeafNumber || '',
                LeafFraction: prospect.LeafFraction || '',
                ExpirateDate: prospect.ExpirateDate || '',
                MaxReturnTickets: prospect.MaxReturnTickets || '',
                PercentageWinners: prospect.PercentageWinners || '',
                ImpresionType: prospect.ImpresionType || '',
                groupDescription: prospect.groupDescription || '',
                impresionTypeDescription: prospect.impresionTypeDescription || '',
                statuDescription: prospect.statuDescription || '',
                Statu: prospect.Statu || '',
                Price: prospect.Price || '',
                Awards: [],
                Prospect_Price: []
            };
        }
        this.getDescriptionById = function (id, list) {
            var detail = 'Selecciones';
            if (!list) {
                return detail;
            }
            for (var i = 0; i < list.length; i++) {
                if (list[i].value == id) {
                    detail = list[i].text;
                }
            }
            return detail;
        }
        this.clearAward = function (award) {
            award = award || {};
            return {
                Id: award.Id || null,
                Name: award.Name || '',
                Description: award.Description || '',
                SourceAward: award.SourceAward || '',
                SourceAwardDescription: award.SourceAwardDescription || '',
                OrderAward: award.OrderAward || '',
                Quantity: award.Quantity || '',
                Terminal: award.Terminal || '',
                ByFraction: award.ByFraction || '',
                ByFractionDescription: award.ByFractionDescription || '',
                Value: award.Value || '',
                TotalValue: award.TotalValue || '',
                TypesAwardId: award.TypesAwardId || ''
            };
        }
        $scope.showGroupDescription = function () {
            return self.getDescriptionById($scope.prospect.GroupId, $scope.groups);
        }
        this.loadProspectCreate = function () {
            if ($scope.prospect.Id > 0) {
                $scope.activeStep = 2;
                $($scope.prospect.Awards).each(function (i) {
                    if ($scope.prospect.Awards[i].SourceAward > 0) {
                        for (var x = 0; x < $scope.prospect.Awards.length; x++) {
                            if ($scope.prospect.Awards[x].Id === $scope.prospect.Awards[i].SourceAward) {
                                $scope.prospect.Awards[i].SourceAward = $scope.prospect.Awards[x].OrderAward;
                            }
                        }
                    }
                });

                $scope.prospect.ExpirateDate = new Date($scope.prospect.ExpirateDate);
            } else {
                $scope.activeStep = 1;
            }
            if ($rootScope.viewMode === true) {
                $scope.viewMode === $rootScope.viewMode;
                $scope.activeStep = 5;
            }
            $scope.award = self.clearAward();
            $scope.selectedTypeRadio = 'new';
            $("input[name=typeRadio]:radio").change(function (val) {
                self.changeProspectTypeRadio(val.currentTarget);
            });
            $('#myModal').on('hidden.bs.modal', function (e) {
                setTimeout(function () {
                    self.clearAward();
                }, 0);
            });
        }
        this.changeProspectTypeRadio = function (radioGroup) {
            $scope.selectedTypeRadio = radioGroup.value;
            if (radioGroup.value === 'exist') {
                $('#existenProspectDropDown').removeClass('hide');
            } else {
                $('#existenProspectDropDown').addClass('hide');
            }
        }
        $scope.deleteAward = function (award) {
            // confirm dialog
            alertify.confirm("&iquest;Desea borrar este premio?", function (e) {
                if (e) {
                    $rootScope.destroyDataTable('awardDatatable');
                    setTimeout(function () {
                        $scope.prospect.Awards = jQuery.grep($scope.prospect.Awards, function (item) {
                            return item.$$hashKey !== award.$$hashKey;
                        });
                        $scope.$apply();
                        alertify.success('Sucursal borrada correctamente!');
                    }, 0);
                }
            });
        }
        $scope.saveAwardForm = function () {
            if (self.validateAwardData($scope.award) === true) {
                if ($scope.editingAward === null) {
                    $rootScope.destroyDataTable('awardDatatable');
                }
                if ($scope.editingAward !== null) {
                    $($scope.prospect.Awards).each(function (i) {
                        if ($scope.prospect.Awards[i].$$hashKey === $scope.editingAward.$$hashKey) {
                            $scope.award.SourceAwardDescription = $('#sourceAwardDropdown option:selected').text();
                            $scope.award.ByFractionDescription = $('#byFractionDropdown option:selected').text();
                            $scope.award.TypesAwardDesc = $('#typesAwardIdDropdown option:selected').text();
                            $scope.prospect.Awards[i] = $scope.award;
                        }
                    });
                } else {
                    $scope.award.SourceAwardDescription = $('#sourceAwardDropdown option:selected').text();
                    $scope.award.ByFractionDescription = $('#byFractionDropdown option:selected').text();
                    $scope.award.TypesAwardDesc = $('#typesAwardIdDropdown option:selected').text();
                    $scope.prospect.Awards.push($scope.award);
                }

                setTimeout(function () {
                    alertify.success('Premio guardado correctamente!');
                    $scope.$apply();
                }, 0);
                $('#myModal').modal('hide');
            }
        }
        $scope.goStepTow = function () {
            if ($scope.selectedTypeRadio === 'new') {
                $scope.prospect = self.clearProspect();
            } else {
                var prospectId = $('#existenProspect').val();
                if (!prospectId) {
                    alertify.showError('Alerta', 'Seleccione un prospecto existente.');
                    return;
                }
                $($scope.allProspectList).each(function (i, item) {
                    if (item.Id === Number(prospectId)) {
                        $scope.prospect = item;
                        $scope.prospect.Id = 0;
                        $scope.prospect.Statu = '';
                        for (var i = 0; i < $scope.prospect.Awards.length; i++) {
                            if ($scope.prospect.Awards[i].SourceAward > 0) {
                                for (var x = 0; x < $scope.prospect.Awards.length; x++) {
                                    if ($scope.prospect.Awards[x].Id === $scope.prospect.Awards[i].SourceAward) {
                                        $scope.prospect.Awards[i].SourceAward = $scope.prospect.Awards[x].OrderAward;
                                    }
                                }
                            }
                        }
                        for (var i = 0; i < $scope.prospect.Awards.length; i++) {
                            $scope.prospect.Awards[i].Id = 0;
                        }
                        for (var i = 0; i < $scope.prospect.Prospect_Price.length; i++) {
                            $scope.prospect.Prospect_Price[i].Id = 0;
                        }
                        $scope.prospect.ExpirateDate = new Date($scope.prospect.ExpirateDate);
                    }
                });
            }
            $scope.activeStep = 2;
        }
        $scope.goStepThree = function () {
            if (self.validateProspectData($scope.prospect) === false) {
                return;
            }
            if ($scope.prospect.LeafFraction != 10) {
                alertify.alert('Debe de revisarse el dise&ntilde;o de impresi&oacute;n con el Administrador del Sistema. <br>');
            }
            $scope.prospect.groupDescription = $('#existenProspect option:selected').text();
            $scope.prospect.impresionTypeDescription = $('#impresionTypeDescription option:selected').text();
            $scope.prospect.statuDescription = $('#statuDescription option:selected').text();
            self.filterPriceList();
            $scope.activeStep = 3;
        }
        $scope.goStepFour = function () {
            if (self.validateProspectData($scope.prospect) === false) {
                return;
            }
            $scope.prospect.groupDescription = $('#existenProspect option:selected').text();
            $scope.prospect.impresionTypeDescription = $('#impresionTypeDescription option:selected').text();
            $scope.prospect.statuDescription = $('#statuDescription option:selected').text();
            $scope.activeStep = 4;
        }
        $scope.goStepFive = function () {
            $scope.activeStep = 5;
            $scope.showGroupDescription();
        }

        $scope.showPrintTechniquesDescription = function () {
            return self.getDescriptionById($scope.prospect.ImpresionType, $scope.printTechniques);
        }
        $scope.showStatuDescription = function () {
            return self.getDescriptionById($scope.prospect.Statu, $scope.prospectStatus);
        }
        $scope.topGoToStep = function (step) {
            if (step < $scope.activeStep && $scope.viewMode !== true) {
                $scope.activeStep = step;
            }
        }
        $scope.getProspectList = function () {
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: 'Prospect/Prospects',
                success: function (data) {
                    $scope.prospectList = data.prospects;
                    $scope.$apply();
                    $rootScope.dataTable();
                }
            });
        }

        $scope.editAward = function (award) {
            if (award) {
                $scope.editingAward = award;
            } else {
                $scope.editingAward = null;
            }
            $scope.disabledTerminal = true;
            $scope.disabledByFraction = true;
            $scope.disabledByQuantity = true;
            $scope.award = self.clearAward(award);
            $('#myModal').modal('show');
        }
        $scope.addPrice = function () {
            if (self.validatePrice($scope.price) === false) {
                return;
            }
            if ($scope.editingPrice === null) {
                $rootScope.destroyDataTable('priceDatatable');
            }
            if ($scope.editingPrice !== null) {
                $($scope.prospect.Prospect_Price).each(function (i) {
                    if ($scope.editingPrice !== null) {
                        if ($scope.prospect.Prospect_Price[i].$$hashKey === $scope.editingPrice.$$hashKey) {
                            $scope.price.PriceDesc = $('#priceTypeDropDown option:selected').text();
                            $scope.prospect.Prospect_Price[i] = $scope.price;
                            $scope.editingPrice = null;
                        }
                    }
                });
            } else {
                $scope.price.PriceDesc = $('#priceTypeDropDown option:selected').text();
                $scope.prospect.Prospect_Price.push($scope.price);
            }
            setTimeout(function () {
                self.filterPriceList();
                $scope.clearPrice();
                $scope.$apply();
                alertify.success('Premio guardado correctamente!');
            }, 0);
        }
        $scope.showExpirateDate = function () {
            var data = $scope.prospect.ExpirateDate;
            return data;
        }
        $scope.editPrice = function (price) {
            if (price) {
                $scope.editingPrice = price;
            } else {
                $scope.editingPrice = null;
            }
            self.filterPriceList($scope.editingPrice.PriceId);
            $scope.clearPrice(price);
        }

        $scope.deletePrice = function (price) {
            // confirm dialog
            alertify.confirm("&iquest;Desea borrar este Precio?", function (e) {
                if (e) {
                    $rootScope.destroyDataTable('priceDatatable');
                    setTimeout(function () {
                        $scope.prospect.Prospect_Price = jQuery.grep($scope.prospect.Prospect_Price, function (item) {
                            return item.$$hashKey !== price.$$hashKey;
                        });
                        self.filterPriceList();
                        $scope.$apply();
                        alertify.success('Precio borrado correctamente!');
                    }, 0);
                }
            });
        }

        $scope.returnBack = function () {
            $state.go($rootScope.returnBack);
        }
        $scope.editingPrice = null;
        $scope.convertToHTML = function () {
            if ($scope.typePrices) {
                var text = '';
                $scope.typePrices.forEach(function (p) {
                    text += p.text + ' : ' + p.description + ', ';
                });
                return $sce.trustAsHtml(text);
            }
        }

        $scope.showTotal = function () {
            var total = 0;
            $scope.prospect.Awards.forEach(function (award) {
                total += award.Value * award.Quantity;
            });
            return total;
        }

        $scope.prospect = $rootScope.prospect || self.clearProspect();
        $scope.prospectList = $rootScope.prospectList;
        $scope.clearPrice();
        $scope.changeTicketPrice = function () {
            $scope.price.SeriePrice = $scope.price.TicketPrice / $scope.prospect.LeafNumber;
            $scope.price.FactionPrice = $scope.price.SeriePrice / $scope.prospect.LeafFraction;
        }
        self.geAlltProspectList();
        self.loadProspectCreate();
        if (!$rootScope.returnBack) {
            $rootScope.returnBack = 'app.prospectlist';
        }

        $scope.updateAwardTypeValue = function () {
            $scope.disabledTerminal = true;
            $scope.disabledByFraction = true;
            $scope.disabledByFraction = false;
            $scope.award.ByFraction = 15;
            switch ( Number($scope.award.TypesAwardId))
            {
                case 7: //Aproximate
                    $scope.award.Terminal = 0;
                    $scope.award.Quantity = 2;
                    $scope.disabledTerminal = false;
                    $scope.disabledByQuantity = false;
                    break;
                case 6:
                    $scope.award.ByFraction = 14;
                    $scope.award.Terminal = 0;
                    $scope.disabledTerminal = false;
                    break;
                case 8:
                    $scope.award.Quantity = 99;
                    $scope.disabledByQuantity = false;
                    $scope.award.Terminal = 0;
                    $scope.disabledTerminal = false;
                    break;
            }
        }
    }
})();
