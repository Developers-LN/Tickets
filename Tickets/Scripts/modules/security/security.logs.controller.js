/**=========================================================
 * Module: SystemLogsController.js
 =========================================================*/

(function () {
    angular
        .module('naut')
        .controller('SystemLogsController', SystemLogsController);
    
    SystemLogsController.$inject = ['$scope', '$rootScope', '$state'];
    function SystemLogsController($scope, $rootScope, $state) {
	    /*jshint validthis:true*/
	    this.getLogList = function () {
	        window.loading.show();
	        $rootScope.destroyDataTable();
		    $.ajax({
			    type: 'GET',
			    contentType: 'application/json; charset=utf-8',
			    url: 'Security/GetLogs',
			    success: function (response) {
			        window.loading.hide();
			        if (response.result == true) {
			            $scope.logs = response.logs;
			            $scope.$apply();
			            $rootScope.dataTable();
			        }
			    }
		    });
	    }
	    this.getLogList();
    }
})();
