(function () {
    'use strict';

    angular
        .module('naut')
        .controller('RaffleProfitability', RaffleProfitability);

    RaffleProfitability.$inject = ['$scope', '$rootScope', '$state','$window'];
    function RaffleProfitability($scope, $rootScope, $state, $window) {
        var self = this;

        $scope.selectedRaffle = 0;
      
        self.GetRaffle = function() {
            window.loading.show();
            $.ajax({
                type: 'GET',
                dataType: 'json',
                url: 'Config/GetSorteoList',
                success: function (data1) {
                    window.loading.hide();
                    $scope.raffleList = data1;
                }
            });
        }
        $rootScope.createSelect2();
        self.GetRaffle();

        $scope.GoTo = function(){
            $window.open('Reports/ProfitabilityReport?raffleId=' + $scope.selectedRaffle, '_blank');
            
        }
    }
})();
