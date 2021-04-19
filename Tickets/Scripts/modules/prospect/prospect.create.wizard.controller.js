/**=========================================================
 * Module: ProspectCreateWizardController.js
 =========================================================*/

(function() {
    'use strict';

    angular
        .module('naut')
        .controller('ProspectCreateWizardController', ProspectCreateWizardController);

    ProspectCreateWizardController.$inject = ['$scope', '$rootScope', '$stateParams'];
    function ProspectCreateWizardController($scope, $rootScope, $stateParams) {
        $rootScope.prospectType = 0;
        $rootScope.activeStep = 0;
        $rootScope.prospect = undefined;

        $rootScope.goStep1 = function () {
            $rootScope.activeStep = 1;
        }
        $rootScope.goStep2 = function (type) {
            $rootScope.activeStep = 1;
        }

        $rootScope.goStep3 = function (prospect) {
            $rootScope.prospect = prospect;
            $rootScope.activeStep = 2;
            window.setTimeout(function () {
                $rootScope.$apply();
            }, 0);
        }

        $rootScope.goStep4 = function () {
            $rootScope.activeStep = 3;
        }

        $rootScope.goStep5 = function () {
            $rootScope.activeStep = 4;
        }

        $rootScope.goStep6 = function () {
            $rootScope.activeStep = 5;
        }

        $rootScope.loadProspect = function (id, edit) {
            window.loading.show();
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: 'ticket/prospectApi/getProspect?id=' + id,
                success: function (response) {
                    window.loading.hide();
                    if (response.result == true) {
                        response.object.expirateDate = new Date(response.object.expirateDateLong);

                        for (var i = 0; i < response.object.awards.length; i++) {
                            if (response.object.awards[i].sourceAward > 0) {
                                for (var x = 0; x < response.object.awards.length; x++) {
                                    if (response.object.awards[x].id === response.object.awards[i].sourceAward) {
                                        response.object.awards[i].sourceAward = response.object.awards[x].orderAward;
                                    }
                                }
                            }
                        }

                        if (edit == false) {
                            //response.object.prospectType = $rootScope.prospectType;
                            response.object.id = 0;
                            response.object.statu = undefined;

                            for (var i = 0; i < response.object.awards.length; i++) {
                                response.object.awards[i].id = 0;
                            }
                            for (var i = 0; i < response.object.prospectPrices.length; i++) {
                                response.object.prospectPrices[i].id = 0;
                            }
                        }

                        $rootScope.goStep3(response.object);
                    } else {
                        alertify.alert(response.message);
                    }
                }
            });
        }

        $rootScope.prospectValidate = function (prospect) {
            var error = '', isReq = ' es un campo requerido. <br>';
            if (prospect.name == undefined) {
                error += 'Nombre' + isReq;
            }
            if (prospect.price == undefined || prospect.price <= 0) {
                error += 'Precio de venta' + isReq;
            }
            /*if (prospect.groupId == undefined || prospect.groupId <= 0) {
                error += 'Grupo' + isReq;
            }*/
            if (prospect.production == undefined || prospect.production  <= 0) {
                error += 'Producci&oacute;n' + isReq;
            }
            if (prospect.leafNumber == undefined || prospect.leafNumber  <= 0) {
                error += 'Hojas por número' + isReq;
            }
            if (prospect.leafFraction == undefined || prospect.leafFraction <= 0) {
                error += 'Fracci&oacute;n por hojas' + isReq;
            }
            if (prospect.maxReturnTickets == undefined) {
                error += 'Porcentaje de devoluciones' + isReq;
            }
            if (prospect.impresionType == undefined || prospect.impresionType <= 0) {
                error += 'Formato de Impresi&oacute;n' + isReq;
            }
            if (prospect.statu == undefined || prospect.statu  <= 0) {
                error += 'Estado' + isReq;
            }
            if (error !== '') {
                alertify.showError('Alerta', error);
            }
            return error === '';
        }

        $rootScope.saveProspectForm = function (prospect) {
            try {
                prospect.expirateDate = $rootScope.parseDate(prospect.expirateDate, new Date()).toJSON();
            } catch (e) { }
            if ($rootScope.prospectValidate(prospect) === false) {
                return;
            }
            prospect.awards = prospect.awards;
            prospect.prospectPrices = prospect.prospectPrices;
            window.loading.show();
            $.ajax({
                type: 'POST',
                dataType: 'json',
                url: $rootScope.serverUrl + 'ticket/prospectApi/saveProspect',
                data: prospect,
                success: function (data) {
                    window.loading.hide();
                    if (data.result === true) {
                        alertify.success(data.message);
                        document.location.href = '#/prospect/prospects';
                    } else {
                        alertify.alert(data.message);
                        window.loading.hide();
                    }
                }
            });
        }

        if ($stateParams.id == 0) {
            $rootScope.goStep1();
        } else {
            $rootScope.loadProspect($stateParams.id, true);
        }
    }
})();
