/**=========================================================
 * Module: about.us.controller.js
 =========================================================*/

(function() {
    'use strict';

    angular
        .module('naut')
        .controller('AboutUsController', AboutUsController);
    
    AboutUsController.$inject = ['$scope'];
    function AboutUsController($scope) {
     
      this.infoAbout = function () {
          window.loading.show();
          $.ajax({
              type: 'GET',
              contentType: 'application/json; charset=utf-8',
              url: 'Dashboard/infoAboutUs',
              success: function (data) {
                  window.loading.hide();
                  $scope.info = data;
                  $scope.$apply();
              }
          });
      }
      this.infoAbout();

    }
})();
