/**=========================================================
 * Module: ProspectAwardController.js
 =========================================================*/

(function () {
    'use strict';

    angular
        .module('naut')
        .controller('ProspectAwardController', ProspectAwardController);

    ProspectAwardController.$inject = ['$scope', '$rootScope'];
    function ProspectAwardController($scope, $rootScope) {
        var self = this;
        $rootScope.$watch('activeStep', function () {
            if ($rootScope.activeStep == 4) {
                $scope.awards = $rootScope.prospect.awards;
                $scope.prospect = $rootScope.prospect;
                window.loading.show();
                $.when(
                    $.ajax($rootScope.serverUrl + 'ticket/catalogApi/getByFractionSelect'),
                    $.ajax($rootScope.serverUrl + 'ticket/prospectApi/getTypeAwardtSelect')
                ).then(function (byFractionResponse, awardTypeResponse) {
                    window.loading.hide();
                    if (byFractionResponse[1] == 'success') {
                        $scope.byFractions = byFractionResponse[0].object;
                    }
                    if (awardTypeResponse[1] == 'success') {
                        $scope.awardTypes = awardTypeResponse[0].object;
                    }

                    window.setTimeout(function () {
                        $scope.$apply();
                    }, 100);
                });
            }
        });
        $scope.goToNextStep = function () {
            $rootScope.prospect.awards = $scope.awards;
            $rootScope.goStep6($rootScope.prospect);
        }
        $scope.goToBackStep = function () {
            $rootScope.prospect.awards = $scope.awards;
            $rootScope.goStep4($rootScope.prospect);
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
            $scope.updateAwardTypeValue();
            $('#myModal').modal('show');
        }

        this.validateAwardData = function (award) {
            var error = '', isReq = ' es un campo requerido. <br>';
            if (award.name == '') {
                error += 'Nombre' + isReq;
            }
            if (award.name != award.nameAux && award.nameAux != '') {
                $($scope.awards).each(function (i) {
                    if ($scope.awards[i].sourceAwardDescription == award.nameAux) {
                        $scope.awards[i].sourceAwardDescription = award.name;
                    }
                });
                $scope.award.nameAux = $scope.award.name;
            }
            else {
                $scope.award.nameAux = $scope.award.name;
            }
            if (award.orderAward == undefined) {
                error += 'Orden' + isReq;
            }
            if (award.quantity == undefined) {
                error += 'Cantidad' + isReq;
            }
            if (award.byFraction == undefined) {
                error += 'Por Fracci&#243;n' + isReq;
            }
            if (award.typesAwardId == undefined) {
                error += 'Tipo de premio' + isReq;
            }
            if (award.value == undefined) {
                error += 'Valor' + isReq;
            }
            if (award.sourceAwardDescription != "") {
                if (award.sourceAwardDescription == award.name) {
                    error += 'El premio no puede ser derivado de si mismo';
                }
                else {
                    let index = $scope.awards.findIndex(i => i.name == award.sourceAwardDescription);
                    let sourceAwardOrder = $scope.awards[index].orderAward;
                    if (award.orderAward < sourceAwardOrder) {
                        error += 'El premio de origen no puede estar despues de este premio';
                    }
                }
            }
            var editing = $scope.editingAward || {};
            if ($scope.awards.some(function (item) { return item.orderAward === award.orderAward && item.$$hashKey !== editing.$$hashKey; }) == true) {
                error += 'El orden del premio esta duplicado.' + isReq;
            }
            if (error !== '') {
                alertify.showError('Alerta', error);
            }
            return error === '';
        }

        $scope.showTotal = function () {
            var total = 0;
            $scope.prospect.awards.forEach(function (award) {
                total += award.value * award.quantity;
            });
            return total;
        }

        $scope.deleteAward = function (award) {
            // confirm dialog
            var dependentsName = '';
            var dependents = $scope.awards.filter(function (a) {
                if (a.sourceAward == award.id) {
                    dependentsName += a.name + "<br/>";
                    return true;
                }
                return false;
            });
            alertify.confirm("&iquest;Desea borrar este premio?", function (e) {
                if (e) {
                    if (dependents.length > 0) {
                        alertify.confirm('Este premio tiene como dependientes:</br/>' + dependentsName + ' &iquest;Desea borrar este premio y sus dependientes?', function (e) {
                            if (e) {
                                dependents.forEach(function (da) {
                                    $scope.awards = jQuery.grep($scope.awards, function (item) {
                                        return item.$$hashKey !== da.$$hashKey;
                                    });
                                });
                                $rootScope.destroyDataTable('awardDatatable');
                                setTimeout(function () {
                                    $scope.awards = jQuery.grep($scope.awards, function (item) {
                                        return item.$$hashKey !== award.$$hashKey;
                                    });
                                    $scope.$apply();
                                    alertify.success('Sucursal borrada correctamente!');
                                }, 0);
                            }
                        });
                    } else {
                        $rootScope.destroyDataTable('awardDatatable');
                        setTimeout(function () {
                            $scope.awards = jQuery.grep($scope.awards, function (item) {
                                return item.$$hashKey !== award.$$hashKey;
                            });
                            $scope.$apply();
                            alertify.success('Sucursal borrada correctamente!');
                        }, 0);
                    }
                }
            });
        }

        $scope.saveAwardForm = function () {
            $scope.award.sourceAwardDescription = $('#sourceAwardDropdown option:selected').text();
            if (self.validateAwardData($scope.award) === true) {
                if ($scope.editingAward === null) {
                    $rootScope.destroyDataTable('awardDatatable');
                }
                if ($scope.editingAward !== null) {
                    $($scope.awards).each(function (i) {
                        if ($scope.awards[i].$$hashKey === $scope.editingAward.$$hashKey) {
                            $scope.award.sourceAwardDescription = $('#sourceAwardDropdown option:selected').text();
                            $scope.award.byFractionDescription = $('#byFractionDropdown option:selected').text();
                            $scope.award.typesAwardDesc = $('#typesAwardIdDropdown option:selected').text();
                            $scope.award.totalValue = $scope.award.value * $scope.award.quantity;
                            $scope.awards[i] = $scope.award;
                        }
                    });
                } else {
                    $scope.award.sourceAwardDescription = $('#sourceAwardDropdown option:selected').text();
                    $scope.award.byFractionDescription = $('#byFractionDropdown option:selected').text();
                    $scope.award.typesAwardDesc = $('#typesAwardIdDropdown option:selected').text();
                    $scope.awards.push($scope.award);
                }

                setTimeout(function () {
                    alertify.success('Premio guardado correctamente!');
                    $scope.$apply();
                }, 0);
                $('#myModal').modal('hide');
            }
        }

        this.clearAward = function (award) {
            award = award || {};
            return {
                id: award.id || null,
                name: award.name || '',
                nameAux: award.name || '',
                description: award.description || '',
                sourceAward: award.sourceAward || '',
                sourceAwardDescription: award.sourceAwardDescription || '',
                orderAward: award.orderAward || '',
                quantity: award.quantity || '',
                terminal: award.terminal || '',
                byFraction: award.byFraction || '',
                byFractionDescription: award.byFractionDescription || '',
                value: award.value || '',
                totalValue: award.totalValue || '',
                typesAwardId: award.typesAwardId || ''
            };
        }

        $scope.updateAwardTypeValue = function () {
            $scope.disabledTerminal = true;
            $scope.disabledByFraction = true;
            $scope.disabledByFraction = false;
            $scope.award.byFraction = 15;
            switch (Number($scope.award.typesAwardId)) {
                case 7: //Aproximate
                    $scope.award.terminal = 0;
                    $scope.award.quantity = 2;
                    $scope.disabledTerminal = false;
                    $scope.disabledByQuantity = false;
                    break;
                case 6:
                    $scope.award.byFraction = 14;
                    $scope.award.terminal = 0;
                    $scope.disabledTerminal = false;
                    break;
                case 8:
                    $scope.award.quantity = 99;
                    $scope.disabledByQuantity = false;
                    $scope.award.terminal = 0;
                    $scope.disabledTerminal = false;
                    break;
            }
        }
    }
})();
