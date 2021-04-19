/**=========================================================
 * Module: clientWorkflow
 =========================================================*/

(function () {
	'use strict';

	angular
        .module('naut')
        .service('clientWorkflow', clientWorkflow);
	/* @ngInject */
	function clientWorkflow() {
	    /*jshint validthis:true*/
	    function validateProccessData(proccess) {
	        var error = '', isReq = ' es un campo requerido. <br>';
	        if (proccess.Comment == '') {
	            error += 'Comentario' + isReq;
	        }
	        if (error !== '') {
	            alertify.showError('Alerta', error);
	        }
	        return error === '';
	    }
        
	    this.getWorkflowList = function () {
	        window.loading.show();
	        $.ajax({
	            type: 'GET',
	            contentType: 'application/json; charset=utf-8',
	            url: 'Client/GetWorkflowList',
	            success: function (data) {
	                window.loading.hide();
	                self.$scope.workflowList = data;
	                self.$scope.$apply();
	                self.$rootScope.dataTable();
	            }
	        });
	    }

	    this.clearProccess = function () {
	        return {
                Id: 0,
	            Comment: ''
	        };
	    }

	    this.approvedClientWorkflow = function (workflow) {
	        self.$rootScope.workflow = workflow;
	        self.$state.go('app.clientApprovedClientProcess');
	    }

	    this.saveProccessForm = function () {
	        if (validateProccessData(self.$scope.proccess) === false) {
	            return;
	        }
	        window.loading.show();
	        $.ajax({
	            type: 'POST',
	            dataType: 'json',
	            url: 'Client/ApprovedClientProcess',
	            data: self.$scope.proccess,
	            success: function (data) {
	                window.loading.hide();
	                if (data.result === true) {
	                    if (self.$scope.proccess.Statu == 24) {
	                        alertify.success('Proceso de aprobaci&#243;n de Prospecto aprobado correctamente!');
	                    } else {
	                        alertify.success('Proceso de aprobaci&#243;n de Prospecto rechazado correctamente!');
	                    }
	                    self.$state.go('app.clientWorkflowList');
	                } else {
	                    alertify.alert(data.message);
	                }
	            }
	        });
	    }

	    this.rejectClientProcess = function () {
	        alertify.confirm("&iquest;Desea rechazar este proceso?", function (e) {
	            if (e) {
	                self.$scope.proccess.Statu = 25;
	                self.$scope.proccess.WorkFlowId = self.$scope.workflow.Id;
	                self.saveProccessForm();
	            }
	        });
	    }

	    this.approvedClientProcess = function () {
	        alertify.confirm("&iquest;Desea aprobar este proceso?", function (e) {
	            if (e) {
	                self.$scope.proccess.Statu = 24;
	                self.$scope.proccess.WorkFlowId = self.$scope.workflow.Id;
	                self.saveProccessForm();
	            }
	        });
	    }

	    var self = this;
	    this.init = function ($rootScope, $scope, $state) {
	        self.$rootScope = $rootScope;
	        self.$scope = $scope;
	        self.$state = $state;
	        $scope.workflow = $rootScope.workflow;
	        switch ($state.current.name) {
	            case 'app.clientWorkflowList':
	                self.getWorkflowList();
	                $scope.approvedClientWorkflow = self.approvedClientWorkflow;
	                break;
	            case 'app.clientApprovedClientProcess':
	                $scope.workflow.proccess = $scope.workflow.proccess.map(function (proccess) {
	                    proccess.CreateDate = new Date(proccess.CreateDate);
	                    return proccess;
	                });

	                $scope.proccess = self.clearProccess();
	                $scope.approvedClientProcess = self.approvedClientProcess;
	                $scope.rejectClientProcess = self.rejectClientProcess;
	                break;
	        }
	    }
	}
	clientWorkflow.$inject = [];
})();
