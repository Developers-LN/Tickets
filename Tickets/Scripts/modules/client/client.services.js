/**=========================================================
 * Module: clientServices
 =========================================================*/

(function () {
	'use strict';

	angular
        .module('naut')
        .service('clientServices', clientServices);
	/* @ngInject */
	function clientServices() {
	    /*jshint validthis:true*/
	    
	    this.startWorkflowProspect = function (client) {
	        // confirm dialog
	        alertify.confirm("&iquest;Desea enviar este cliente a un proceso de aprobaci&#243;n?", function (e) {
	            if (e) {
	                $.ajax({
	                    type: 'POST',
	                    dataType: 'json',
	                    url: 'Client/SendClientProccess',
	                    data: { clientId: client.Id },
	                    success: function (data) {
	                        if (data.result === true) {
	                            self.$rootScope.destroyDataTable();
	                            self.getClientList();
	                            alertify.success('El cliente fue enviado al flujo de aprobaci&#243;n correctamente!');
	                        } else {
	                            alertify.alert(data.message);
	                        }
	                    }
	                });
	            }
	        });
	    }


	    this.clearClient = function () {
	        return {
	            Id : 0,
	            Addres : '',
	            Birthday: '',
	            PriceId: '',
	            CheckIn : '',
	            ClientType : '',
	            Comment : '',
	            CreateDate : '',
	            CreateUser : '',
	            CreditLimit : '',
	            DocumentNumber : '',
	            Email : '',
	            Fax : '',
	            Gender : '',
	            GroupId : '',
	            MaritalStatus : '',
	            Name : '',
	            Phone : '',
	            Province : '',
	            RNC : '',
	            Section : '',
	            Statu : '',
	            Town : '',
	            Tradename: ''
	        };
	    }

	    this.saveClientForm = function () {
	        self.$scope.client.Birthday = $('.datatime-picker')[0].value;
	        if (validateClientData(self.$scope.client) === false) {
	            return;
	        }
	        $.ajax({
	            type: 'POST',
	            dataType: 'json',
	            url: 'Client/Create',
	            data: self.$scope.client,
	            success: function (data) {
	                if (data === true) {
	                    alertify.success('Cliente guardada correctamente!');
	                    self.$state.go('app.clientList');
	                } else {
	                    alertify.alert(data.message);
	                }
	            }
	        });
	    }

	    var self = this;
	    this.init = function ($rootScope, $scope, $state) {
	        self.$rootScope = $rootScope;
	        self.$scope = $scope;
	        self.$state = $state;
	        $scope.client = $rootScope.client || self.clearClient();

	        switch ($state.current.name) {
	            case 'app.clientList':
	                self.getClientList();
	                $scope.startWorkflowProspect = self.startWorkflowProspect;
	                $scope.editClient = self.editClient;
	                $scope.deleteClient = self.deleteClient;
	                break;
	            case 'app.clientCreate':
	                self.getClientList();
	                $scope.updateTown = self.updateTown;
	                $scope.updateDistritalTown = self.updateDistritalTown;
	                $('.datatime-picker').datepicker({ });
	                $scope.saveClientForm = self.saveClientForm;
	                break;
	        }
	    }
	}
	clientServices.$inject = [];
})();
