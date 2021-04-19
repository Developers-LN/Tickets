/**=========================================================
 * Module: typesAwardServices
 =========================================================*/

(function () {
	'use strict';

	angular
        .module('naut')
        .service('typesAwardServices', typesAwardServices);
	/* @ngInject */
	function typesAwardServices() {
	    /*jshint validthis:true*/
	    function validateData(typesAward) {
	        var error = '', isReq = ' es un campo requerido. <br>';
	        if (typesAward.Name == '') {
	            error += 'Nombre' + isReq;
	        }
	        if (!typesAward.GroupId || typesAward.GroupId == '') {
	            error += 'Grupo' + isReq;
	        }
	        if (!typesAward.Creation || typesAward.Creation == '') {
	            error += 'Creacion' + isReq;
	        }
	        if (error !== '') {
	            alertify.showError('Alerta', error);
	        }
	        return error === '';
	    }

	    this.getList = function () {
	        $.ajax({
	            type: 'GET',
	            contentType: 'application/json; charset=utf-8',
	            url: 'TypesAward/GetList',
	            success: function (data) {
	                self.$scope.typesAwardList = data.typesAwards;
	                self.$scope.$apply();
	                self.$rootScope.dataTable();
	            }
	        });
	    }
	    this.editTypesAward = function (typesAward) {
	        self.$rootScope.typesAward = typesAward;
	        self.$state.go('app.typesawardcreate');
	    }
	    this.clearTypesAward = function () {
	        return {
	            Id : 0,
	            Name: '',
	            Description: '',
	            Creation: '',
	            GroupId: ''
	        };
	    }

	    this.saveTypesAwardForm = function () {
	        if (validateData(self.$scope.typesAward) === false) {
	            return;
	        }
	        $.ajax({
	            type: 'POST',
	            dataType: 'json',
	            url: 'TypesAward/Create',
	            data: self.$scope.typesAward,
	            success: function (data) {
	                if (data === true) {
	                    alertify.success('Tipo de Premio guardada correctamente!');
	                    self.$state.go('app.typesawardlist');
	                } else {
	                    alertify.alert(data.message);
	                }
	            }
	        });
	    }

	    this.deleteTypesAward = function (typesAward) {
	        // confirm dialog
	        alertify.confirm("&iquest;Desea borrar este tipo de premio?", function (e) {
	            if (e) {
	                $.ajax({
	                    type: 'POST',
	                    dataType: 'json',
	                    url: 'TypesAward/Delete',
	                    data: { typesAwardId: typesAward.Id },
	                    success: function (data) {
	                        if (data === true) {
	                            self.$rootScope.destroyDataTable();
	                            self.getList();
	                            alertify.success('Tipo de premio borrado correctamente!');
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
	        $scope.typesAward = $rootScope.typesAward || self.clearTypesAward();

	        switch ($state.current.name) {
	            case 'app.typesawardlist':
	                self.getList();
	                $scope.editTypesAward = self.editTypesAward;
	                $scope.deleteTypesAward = self.deleteTypesAward;
	                break;
	            case 'app.typesawardcreate':
	                $scope.saveTypesAwardForm = self.saveTypesAwardForm;
	                break;
	        }
	    }
	}
	typesAwardServices.$inject = [];
})();
