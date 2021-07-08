/**=========================================================
 * Module: ProspectPriceController.js
 =========================================================*/

(function () {
    'use strict';

    angular
        .module('naut')
        .controller('ProspectPriceController', ProspectPriceController);

    ProspectPriceController.$inject = ['$scope', '$rootScope', '$sce'];
    function ProspectPriceController($scope, $rootScope, $sce) {
        var self = this;
        $rootScope.$watch('activeStep', function () {
            if ($rootScope.activeStep == 3) {
                $rootScope.destroyDataTable('priceDatatable');
                $scope.prospectPrices = $rootScope.prospect.prospectPrices;
                window.loading.show();
                $.ajax({
                    type: 'GET',
                    contentType: 'application/json; charset=utf-8',
                    url: 'ticket/catalogApi/getPriceTypeSelect',
                    success: function (response) {
                        window.loading.hide();
                        $scope.typePrices = response.object;
                        $scope.typePricesFiltered = response.object;
                        self.filterPriceList();
                        window.setTimeout(function () {
                            $scope.$apply();
                        }, 100);
                    }
                });
            }
        });

        this.validatePrice = function (price) {
            var error = '', isReq = ' es un campo requerido. <br>';
            if (price.priceId === undefined) {
                error += 'Tipo de Premio' + isReq;
            }
            if (price.ticketPrice === undefined) {
                error += 'Precio Billete' + isReq;
            }
            if (price.seriePrice === undefined) {
                error += 'Precio Serie' + isReq;
            }
            if (price.factionPrice === undefined) {
                error += 'Precio Fraccion' + isReq;
            }
            if (error !== '') {
                alertify.showError('Alerta', error);
            }
            return error === '';
        }

        $scope.goToNextStep = function () {
            $rootScope.prospect.prospectPrices = $scope.prospectPrices;
            $rootScope.goStep5($rootScope.prospect);
        }
        $scope.goToPrevioStep = function () {
            $rootScope.prospect.prospectPrices = $scope.prospectPrices;
            $rootScope.goStep3($rootScope.prospect);
        }

        $scope.editingPrice = null;
        $scope.editPrice = function (price) {
            if (price) {
                $scope.editingPrice = price;
            } else {
                $scope.editingPrice = null;
            }
            self.filterPriceList($scope.editingPrice.priceId);
            $scope.clearPrice(price);
        }

        $scope.clearPrice = function (price) {
            $scope.price = {
                id: price ? price.id : 0,
                priceId: price ? price.priceId : undefined,
                factionPrice: price ? price.factionPrice : undefined,
                ticketPrice: price ? price.ticketPrice : undefined,
                seriePrice: price ? price.seriePrice : undefined
            };
            if (price) {
                self.filterPriceList(price.priceId);
            } else {
                $scope.editingPrice = null;
                self.filterPriceList();
            }
        }

        $scope.deletePrice = function (price) {
            // confirm dialog
            alertify.confirm("&iquest;Desea borrar este Precio?", function (e) {
                if (e) {
                    $rootScope.destroyDataTable('priceDatatable');
                    setTimeout(function () {
                        $scope.prospectPrices = jQuery.grep($scope.prospectPrices, function (item) {
                            return item.$$hashKey !== price.$$hashKey;
                        });
                        self.filterPriceList();
                        $scope.$apply();
                        alertify.success('Precio borrado correctamente!');
                    }, 0);
                }
            });
        }

        $scope.addPrice = function () {
            if (self.validatePrice($scope.price) === false) {
                return;
            }
            if ($scope.editingPrice === null) {
                $rootScope.destroyDataTable('priceDatatable');
            }
            if ($scope.editingPrice !== null) {
                $($scope.prospectPrices).each(function (i) {
                    if ($scope.editingPrice !== null) {
                        if ($scope.prospectPrices[i].$$hashKey === $scope.editingPrice.$$hashKey) {
                            $scope.price.priceDesc = $('#priceTypeDropDown option:selected').text();
                            $scope.prospectPrices[i] = $scope.price;
                            $scope.editingPrice = null;
                        }
                    }
                });
            } else {
                $scope.price.priceDesc = $('#priceTypeDropDown option:selected').text();
                $scope.prospectPrices.push($scope.price);
            }
            setTimeout(function () {
                self.filterPriceList();
                $scope.clearPrice();
                $scope.$apply();
                alertify.success('Premio guardado correctamente!');
            }, 0);
        }

        this.filterPriceList = function (selectedId) {
            if (!$scope.typePrices) {
                return;
            }
            var selectedId = selectedId || 0;
            $scope.typePricesFiltered = $scope.typePrices.filter(function (item) {
                if ($scope.prospectPrices.length === 0) {
                    return true;
                }
                return $scope.prospectPrices.some(function (price) {
                    return Number(price.priceId) === Number(item.id)
                        && Number(selectedId) !== Number(item.id);
                }) === false;
            });
        }
        $scope.changeTicketPrice = function () {
            $scope.price.seriePrice = $scope.price.ticketPrice / $rootScope.prospect.leafNumber;
            $scope.price.factionPrice = $scope.price.seriePrice / $rootScope.prospect.leafFraction;
        }
        $scope.convertToHTML = function () {
            if ($scope.typePrices) {
                var text = '';
                $scope.typePrices.forEach(function (p) {
                    text += p.text + ' : ' + p.description + ', ';
                });
                return $sce.trustAsHtml(text);
            }
        }
    }
})();
