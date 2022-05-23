/**=========================================================
 * Module: DeliveryController.js
 =========================================================*/

(function () {
    'use strict';

    angular
        .module('naut')
        .controller('TaxReceiptController', TaxReceiptController);

    TaxReceiptController.$inject = ['$scope', '$state', '$rootScope', '$stateParams'];
    function TaxReceiptController($scope, $state, $rootScope, $stateParams) {
        var self = this;
        $scope.taxReceipt = {
            Id: 0,
            Type: undefined,
            Notas: undefined,
            SequenceFrom: undefined,
            SequenceTo: undefined,
            DueDate: undefined
        };

        function validateData(taxReceipt) {
            var error = '', isReq = ' es un campo requerido. <br>';

            if (taxReceipt.Type === undefined) {
                error += 'El tipo de comprobante fiscal' + isReq;
            }
            if (taxReceipt.SequenceFrom === undefined) {
                error += 'La secuecia desde' + isReq;
            }
            if (taxReceipt.SequenceTo === undefined) {
                error += 'La secuencia hasta' + isReq;
            }
            if (taxReceipt.DueDate === undefined) {
                error += 'La fecha de caducidad' + isReq;
            }
            if ((taxReceipt.SequenceFrom <= $scope.minSequence || taxReceipt.SequenceFrom <= $scope.maxSequence)) {
                error += 'La secuencia desde debe ser mayor a ' + $scope.maxSequence;
            }
            if ((taxReceipt.SequenceTo <= $scope.minSequence || taxReceipt.SequenceTo <= $scope.maxSequence)) {
                error += 'La secuencia hasta debe ser mayor a ' + $scope.maxSequence;
            }
            if (error !== '') {
                alertify.showError('Alerta', error);
            }
            return error === '';
        }

        this.loadtaxReceiptInfo = function () {
            window.loading.show();
            $.when($.ajax('TaxReceipt/GetTaxReceiptInfo')).done(function (taxReceiptInfo) {
                $scope.taxTypes = taxReceiptInfo.taxType;
                $scope.minSequence = taxReceiptInfo.Min;
                $scope.maxSequence = taxReceiptInfo.Max;
                window.loading.hide();
                window.setTimeout(function () {
                    $scope.$apply();
                    $rootScope.createSelect2();
                }, 0);
                $rootScope.dataTable();
            });
        }

        $scope.goBack = function () {
            window.location.href = '#/taxReceipt/taxReceiptList';
        }

        $scope.saveForm = function () {
            try {
                $scope.taxReceipt.dueDate = $rootScope.parseDate($scope.taxReceipt.dueDate, $scope.taxReceipt.dueDate).toJSON();
            } catch (e) { }
            if (validateData($scope.taxReceipt) === false) {
                return;
            }
            window.loading.show();

            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: 'TaxReceipt/CheckSequenceRange',
                data: { 'From': $scope.taxReceipt.SequenceFrom, 'To': $scope.taxReceipt.SequenceTo },
                success: function (data) {
                    window.loading.hide();
                    if (data.result === false) {
                        $.ajax({
                            type: 'POST',
                            dataType: 'json',
                            url: 'TaxReceipt/TaxReceipt',
                            data: $scope.taxReceipt,
                            success: function (data) {
                                window.loading.hide();
                                if (data.result === true) {
                                    alertify.success(data.message);
                                    $scope.goBack();
                                } else {
                                    alertify.alert(data.message);
                                }
                            }
                        });
                    }
                    else {
                        alertify.alert(data.message);
                    }
                }
            });
        }
        this.loadtaxReceiptInfo();
    }
})();
