(function () {
	'use strict';

	angular
        .module('naut')
        .controller('SystemEmailController', SystemEmailController);

	SystemEmailController.$inject = ['$scope', '$rootScope', '$state'];
	function SystemEmailController($scope, $rootScope, $state) {
		var self = this;
		//;
		//$scope.recipients = "";
		$scope.message = "";
		$scope.selectedSubject = 0;
	
		self.getEmailModuleList = function()
		{
			$.ajax({
				type: 'GET',
				dataType: 'json',
				url: 'Config/GetReportMessage',
				success: function (data) {
				   
				    $scope.Modules = data.ReportByEmail;
				    $scope.nameSelected = 0;
				 $('#selectEmailMultiple').select2(
				   {
				    data: data.EmployeesEmails,
				     multiple: true,
				    });
				   $scope.recipients = [];
				    self.getNames(data.ReportByEmail);
					window.loading.hide();
				}
			});
		}

		self.getNames = function (data) {

		    var i, names = [];
		    for (i = 0; i < data.length; i++) {
		        names[i] = { Id: data[i].Id, Name: data[i].Name };

		    }
		    names.unshift({ Id: 0, Name: "Seleccione un modulo" });
		    $scope.Names = names;
		    $scope.nameSelected = 0;
		   
		}
		
		$scope.update = function () {
		    if ($scope.nameSelected > 0) {
		        var i = $scope.nameSelected - 1;

		        $scope.ModulesName = $scope.Modules[i].ModuleName;
		        // console.log($scope.Modules[i].Recipients);
		        $scope.subject = $scope.Modules[i].Subject;
		        if ($scope.Modules[i].Recipients != null && $scope.Modules[i].Recipients != undefined) {
		            var value = $.map($scope.Modules[i].Recipients.split(","), function (value) {
		                return parseInt(value);
		            });
		            // console.log('from update',value);
		            $scope.recipients = value;
		        }
		        $('#selectEmailMultiple').select2().val(value).trigger('change');
		        $scope.message = $scope.Modules[i].Message;

		    } else {
		        $scope.nameSelected = $scope.Names[0].Id;
		        $("#selectEmailMultiple").select2('val', 'All');
		        $scope.recipients = "";
		        $scope.subject = "";
		        $scope.message = "";
		        $scope.ModulesName = "";
		    }
		}

		$scope.saveChanges = function()
		{
	
			if ($scope.nameSelected != 0) {
			    var selectedEmails = $("#selectEmailMultiple").val();
			    
				var email = {
					Id: $scope.nameSelected,
					Name: $scope.Names[$scope.nameSelected].Name,
					ModuleName:  $scope.ModulesName ,
					Recipients: selectedEmails != null ?selectedEmails.toString() : null,
					Subject: $scope.subject,
                    Message: $scope.message
				};
			
				$.ajax({
					type: 'POST',
					dataType: 'json',
					url: 'Config/SaveReportMessage',
					data:email,
					success: function (data) {
						
						window.loading.hide();
					}
				});
				self.clearSelection();
				self.getEmailModuleList();
				alertify.success('Modulo guardado correctamente!');
			}
		}
		self.clearSelection = function () {
		    $scope.Module = [];
		    $scope.nameSelected = $scope.Names[0].Id;
		    //console.log("log:", $scope.Names[0].Name);
		    $("#selectEmailMultiple").select2('val', 'All');
		    $scope.recipients = "";
		    $scope.subject = "";
		    $scope.message = "";
		    $scope.ModulesName = "";
		} 
		self.getEmailModuleList();
	}
})();
