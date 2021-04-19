/**=========================================================
 * Module: raffleServices
 =========================================================*/

(function () {
	'use strict';

	angular
        .module('naut')
        .service('raffleServices', raffleServices);
	/* @ngInject */
	function raffleServices() {
	    /*jshint validthis:true*/
	    function validateSolteoData(solteo) {
	        var error = '', isReq = ' es un campo requerido. <br>';
	        if (solteo.Name === undefined) {
	            error += 'Nombre' + isReq;
	        }
	        if (solteo.DateSolteo === undefined) {
	            error += 'Fecha del sorteo' + isReq;
	        }
	        if (solteo.ProspectId === undefined) {
	            error += 'Prospecto' + isReq;
	        }
	        if (solteo.Commodity === undefined) {
	            error += 'Mercanc&iacute;a' + isReq;
	        }
	        if (solteo.StartReturnDate === undefined) {
	            error += 'Fecha inicio devolucion' + isReq;
	        }
	        if (solteo.EndReturnDate === undefined) {
	            error += 'Fecha fin devolucion' + isReq;
	        }
	        if (solteo.Statu === undefined) {
	            error += 'Status' + isReq;
	        }
	        if (error !== '') {
	            alertify.showError('Alerta', error);
	        }
	        return error === '';
	    }

	    function validateNumenber(number) {
	        if (!number || Number(number) <= 0 || number == '') {
                return false
	        }
	        return true;
	    }

	    this.getSolteoList = function (statu) {
	        $.ajax({
	            type: 'GET',
	            contentType: 'application/json; charset=utf-8',
	            url: 'Raffle/GetList?statu=' + statu,
	            success: function (data) {
	                self.$scope.raffleList = data.solteos.map(function (raffle) {
	                    raffle.DateSolteo = new Date(raffle.DateSolteo);
	                    raffle.StartReturnDate = new Date(raffle.StartReturnDate);
	                    raffle.EndReturnDate = new Date(raffle.EndReturnDate);
	                    return raffle;
	                });
	                self.$scope.$apply();
	                self.$rootScope.dataTable();
	            }
	        });
	    }

	    this.getRaffleData = function () {
	        $.ajax({
	            type: 'GET',
	            contentType: 'application/json; charset=utf-8',
	            url: 'Raffle/GetRaffleData',
	            success: function (data) {
	                self.$scope.raffleStatus = data.raffleStatus;
	                self.$scope.prospects = data.prospects;
	                self.$scope.solteoCommoditys = data.solteoCommoditys;
	                if (self.$scope.solteo.Id > 0) {
	                    self.$scope.solteo.DateSolteo = new Date(self.$scope.solteo.DateSolteoO);
	                    self.$scope.solteo.StartReturnDate = new Date(self.$scope.solteo.StartReturnDate);
	                    self.$scope.solteo.EndReturnDate = new Date(self.$scope.solteo.EndReturnDate);

	                    self.$scope.solteo.StartDate = self.$scope.solteo.StartReturnDate;
	                    self.$scope.solteo.StartTime = self.$scope.solteo.StartReturnDate;

	                    self.$scope.solteo.EndDate = self.$scope.solteo.EndReturnDate;
	                    self.$scope.solteo.EndTime = self.$scope.solteo.EndReturnDate;
	                }
	                self.$scope.$apply();
	            }
	        });
	    }

	    this.editSolteo = function (solteo) {
	        self.$rootScope.solteo = solteo;
	        self.$state.go('app.solteoCreate');
	    }
	    this.clearSolteo = function () {
	        return {
	            Id: 0,
	            Name: undefined,
	            ProspectId: '',
	            DateSolteo: undefined,
	            Commodity: undefined,
	            Note: '',
	            StartReturnDate: undefined,
	            EndReturnDate: undefined,
	            StartDate: undefined,
	            EndDate: undefined,
	            StartTime: undefined,
	            EndTime: undefined,
	            Statu: undefined,
	        };
	    }

	    this.saveSolteoForm = function () {
	        try{
	            self.$scope.solteo.StartReturnDate = self.$rootScope.parseDate(self.$scope.solteo.StartDate, self.$scope.solteo.StartTime).toJSON();

	            self.$scope.solteo.EndReturnDate = self.$rootScope.parseDate(self.$scope.solteo.EndDate, self.$scope.solteo.EndTime).toJSON();

	            self.$scope.solteo.DateSolteo = self.$rootScope.parseDate(self.$scope.solteo.DateSolteo, self.$scope.solteo.DateSolteo).toJSON();
	        } catch (e) { }
	        if (validateSolteoData(self.$scope.solteo) === false) {
	            return;
	        }
	        window.loading.show();
	        $.ajax({
	            type: 'POST',
	            dataType: 'json',
	            url: 'Raffle/Create',
	            data: self.$scope.solteo,
	            success: function (data) {
	                if (data.result === true) {
	                    alertify.success(data.message);
	                    window.loading.hide();
	                    self.$state.go('app.solteoList');
	                } else {
	                    alertify.alert(data.message);
	                    window.loading.hide();
	                }
	            }
	        });
	    }

	    this.deleteSolteo = function (solteo) {
	        // confirm dialog
	        alertify.confirm("&iquest;Desea borrar este sorteo?", function (e) {
	            if (e) {
	                $.ajax({
	                    type: 'POST',
	                    dataType: 'json',
	                    url: 'Raffle/Delete',
	                    data: { solteoId: solteo.Id },
	                    success: function (data) {
	                        if (data === true) {
	                            self.$rootScope.destroyDataTable();
	                            self.getSolteoList();
	                            alertify.success('Sorteo borrado correctamente!');
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
	        switch ($state.current.name) {
	            case 'app.solteoList':
	                self.getSolteoList();
	                $scope.editSolteo = self.editSolteo;
	                $scope.deleteSolteo = self.deleteSolteo;
	                break;
	            case 'app.solteoCreate':
	                $scope.solteo = $rootScope.solteo || self.clearSolteo();
	                $scope.solteo.DateSolteoO = $scope.solteo.DateSolteo;
	                self.getRaffleData();
	                $scope.saveSolteoForm = self.saveSolteoForm;
	                break;
	        }
	    }
	}
	raffleServices.$inject = [];
})();
