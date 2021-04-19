/**=========================================================
 * Module: configCatalog
 =========================================================*/

(function () {
	'use strict';

	angular
        .module('naut')
        .service('configCatalog', configCatalog);
	/* @ngInject */
	function configCatalog() {
	    /*jshint validthis:true*/
	    function validateCatalogData(catalog) {
	        var error = '', isReq = ' es un campo requerido. <br>';
	        if (catalog.NameGroup == '') {
	            error += 'El grupo' + isReq;
	        }
	        if (catalog.NameDetail == '') {
	            error += 'El detalle' + isReq;
	        }
	        if (error !== '') {
	            alertify.showError('Alerta', error);
	        }
	        return error === '';
	    }
	    this.getCatalogList = function () {
	        $.ajax({
	            type: 'GET',
	            contentType: 'application/json; charset=utf-8',
	            url: 'Config/GetCatalogList',
	            success: function (data) {
	                self.$scope.catalogList = data;
	                self.$scope.$apply();
	                self.$rootScope.dataTable();
	            }
	        });
	    }

	    this.editCatalog = function (catalog) {
	        self.$rootScope.catalog = catalog;
	        self.$state.go('app.configCatalogCreate');
	    }

	    this.clearCatalog = function () {
	        return {
	            Id: 0,
	            IdGroup: '',
	            NameGroup: '',
	            IdDetail: '',
	            NameDetail: '',
	            Description: '',
	            Description2: '',
                Statu: ''
	        };
	    }

	    this.saveCatalogForm = function () {
	        if (validateCatalogData(self.$scope.catalog) === false) {
	            return;
	        }
	        $.ajax({
	            type: 'POST',
	            dataType: 'json',
	            url: 'Config/CatalogCreate',
	            data: self.$scope.catalog,
	            success: function (data) {
	                if (data === true) {
	                    alertify.success('Catalogo guardada correctamente!');
	                    self.$state.go('app.configCatalogList');
	                }
	            }
	        });
	    }

	    this.deleteCatalog = function (catalog) {
	        // confirm dialog
	        alertify.confirm("&iquest;Desea borrar este catalogo?", function (e) {
	            if (e) {
	                $.ajax({
	                    type: 'POST',
	                    dataType: 'json',
	                    url: 'Config/CatalogDelete',
	                    data: { catalogId: catalog.Id },
	                    success: function (data) {
	                        if (data === true) {
	                            self.$rootScope.destroyDataTable();
	                            self.getCatalogList();
	                            alertify.success('Catalogo borrado correctamente!');
	                        }
	                    }
	                });
	            }
	        });
	    }

	    var self = this;
	    this.init = function ($rootScope, $scope, $state) {
	        self.$rootScope = $rootScope;
	        self.$scope = $scope;
	        self.$state = $state;
	        $scope.catalog = $rootScope.catalog || self.clearCatalog();

	        switch ($state.current.name) {
	            case 'app.configCatalogList':
	                self.getCatalogList();
	                $scope.editCatalog = self.editCatalog;
	                $scope.deleteCatalog = self.deleteCatalog;
	                break;
	            case 'app.configCatalogCreate':
	                if ($scope.catalog.Id > 0) {
	                    $('#NameGroup').attr('disabled', 'disabled');
	                } else {
	                    $('#NameGroup').removeAttr('disabled', 'disabled');
	                }
	                $scope.saveCatalogForm = self.saveCatalogForm;
	                break;
	        }
	    }
	}
	configCatalog.$inject = [];
})();
