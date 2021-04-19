/**=========================================================
 * Module: raffleAwardServices
 =========================================================*/

(function () {
	'use strict';

	angular
        .module('naut')
        .service('raffleAwardServices', raffleAwardServices);
	/* @ngInject */
	function raffleAwardServices() {
	    /*jshint validthis:true*/
	    function validateData(raffleAward) {
	        var error = '', isReq = ' es un campo requerido. <br>';

	        if (!raffleAward.AwardId || raffleAward.AwardId === '') {
	            error += 'Premio' + isReq;
	        }
	        if (!raffleAward.ControlNumber || raffleAward.ControlNumber === '') {
	            error += 'Numero ganador' + isReq;
	        }
	        if (error !== '') {
	            alertify.showError('Alerta', error);
	        }
	        return error === '';
	    }

	    this.getRaffleAwardDetails = function () {
	        $.ajax({
	            type: 'GET',
	            contentType: 'application/json; charset=utf-8',
	            url: 'Raffle/RaffleAwardDetails',
	            success: function (data) {
	                self.$scope.raffle = data;
	                filterAwards();
	                self.clearRaffleAward();
	                self.$scope.$apply();
	                self.$rootScope.dataTable();
	            }
	        });
	    }

	    this.clearRaffleAward = function () {
	        self.$scope.isEditing = false;
	        self.$scope.raffleAward = {
                Id: 0,
	            RaffleId: self.$scope.raffle.RaffleId,
	            AwardId: 0,
	            ControlNumber: 0,
	            Fraction: ''
	        };
	        filterAwards();
	    }

	    this.addRaffleAward = function () {
	        if (validateData(self.$scope.raffleAward) === true) {
	            self.$scope.raffleAward.AwardName = $('#AwardId option:selected').text();
	            if (self.$scope.isEditing === true) {
	                $(self.$scope.raffle.raffleAwards).each(function (i) {
	                    if (self.$scope.raffle.raffleAwards[i].$$hashKey === self.$scope.raffleAward.$$hashKey) {
	                        self.$scope.raffle.raffleAwards[i] = self.$scope.raffleAward;
	                        self.clearRaffleAward();
	                    }
	                });
	            } else {
	                self.$rootScope.destroyDataTable();
	                setTimeout(function () {
	                    self.$scope.raffle.raffleAwards.push(self.$scope.raffleAward);
	                    self.clearRaffleAward();
	                    self.$scope.$apply();
	                    self.$rootScope.dataTable();
	                }, 0);
	            }
	            alertify.success('Premio guardado correctamente!');
	        }
	    }
	    function filterAwards(selectedId) {
	        selectedId = selectedId || -1;
	        self.$scope.awardList = $(self.$scope.raffle.awardList).filter(function (i) {
	            if (self.$scope.raffle.raffleAwards.length === 0) {
	                return true;
	            }
	            return self.$scope.raffle.raffleAwards.some(function (raffleAward) {
	                return Number(raffleAward.AwardId) === Number(self.$scope.raffle.awardList[i].Id) && Number(selectedId) !==  Number(self.$scope.raffle.awardList[i].Id);
	            }) === false;
	        });
	    }
	    this.editRaffleAward = function (raffleAward) {
	        self.$scope.isEditing = true;
	        self.$scope.raffleAward = {
	            Id: raffleAward.Id,
	            $$hashKey: raffleAward.$$hashKey,
	            RaffleId: self.$scope.raffle.RaffleId,
	            AwardId: raffleAward.AwardId,
	            ControlNumber: raffleAward.ControlNumber,
	            Fraction: raffleAward.Fraction
	        };

	        filterAwards(raffleAward.AwardId);
	    }

	    this.deleteRaffleAward = function (raffleAward) {
	        alertify.confirm("&iquest;Desea borrar este premio?", function (e) {
	            if (e) {
	                self.$rootScope.destroyDataTable();
	                $(self.$scope.raffle.raffleAwards).each(function (i) {
	                    if (self.$scope.raffle.raffleAwards[i].$$hashKey === raffleAward.$$hashKey) {
	                        setTimeout(function () {
	                            self.$scope.raffle.raffleAwards.splice(i, 1);
	                            filterAwards();
	                            self.$scope.$apply();
	                            self.$rootScope.dataTable();
	                        }, 0);
	                    }
	                });
	            }
	        });
	    }

	    this.saveRaffleAwardForm = function () {
	        $.ajax({
	            type: 'POST',
	            dataType: 'json',
	            contentType: 'application/json; charset=utf-8',
	            url: 'Raffle/RaffleAwardNumber',
	            data: JSON.stringify({ 'raffleAwards': self.$scope.raffle.raffleAwards }),
	            success: function (data) {
	                if (data.Result === true) {
	                    alertify.success(data.Message);
	                    self.$state.go('app.dashboard');
	                } else {
	                    alertify.alert(data.Message);
	                }
	            }
	        });
	    }

	    var self = this;
	    this.init = function ($rootScope, $scope, $state) {
	        switch ($state.current.name) {
	            case 'app.raffleAwardNumber':
	                self.$rootScope = $rootScope;
	                self.$scope = $scope;
	                self.$state = $state;
	                self.getRaffleAwardDetails();
	                $scope.addRaffleAward = self.addRaffleAward;
	                $scope.clearRaffleAward = self.clearRaffleAward;
	                $scope.saveRaffleAwardForm = self.saveRaffleAwardForm;
	                $scope.editRaffleAward = self.editRaffleAward;
	                $scope.deleteRaffleAward = self.deleteRaffleAward;
	                break;
	        }
	    }
	}
	raffleAwardServices.$inject = [];
})();
