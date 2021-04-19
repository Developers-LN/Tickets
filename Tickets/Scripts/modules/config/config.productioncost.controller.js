(function () {
   'use strict';

    angular
        .module('naut')
        .controller('SystemProductionCost', SystemProductionCost);

    SystemProductionCost.$inject = ['$scope', '$rootScope', '$state'];
    function SystemProductionCost($scope, $rootScope, $state) {
        var self = this;
        //array for add cost to visible list
        $scope.ProductionCost = [];

        $scope.StaticProductionCostObject = {
            Id: 0,
            RaffleId: 0,
            Detalle: "",
            Cantidad: "",
            Monto: "",
            Status:true,
            Clear: function () {
                this.Id = 0;
               // this.RaffleId = 0; // if not necesary clear raffleid
                this.Detalle = "";
                this.Cantidad = "";
                this.Monto = "";
            },
            Verify: function()
            {
                if (this.RaffleId != 0 && this.Detalle != "" && this.Cantidad != 0 && this.Monto != 0) {
                    return true;
                } else {
                    return false;
                }
            }

        };
      
        function GetProductionCost(id, raffleId, details, count, amount)
        {
            var obj = {
                Id : id,
                RaffleId: parseInt(raffleId),
                Detalle: details,
                Cantidad: count,
                Monto: amount,
                Created: "",
                Status:true
            }
            return obj;
        }
        $scope.deleteArrayItems = [];
        $scope.deleteNumber = function (arrayItem) {
            // confirm dialog
            if (arrayItem.Id > 0)
            {
                arrayItem.Status = false;
                $scope.deleteArrayItems.push(arrayItem);
            }
            alertify.confirm("&iquest;Desea borrar este Numero?", function (e) {
                if (e) {
                    $scope.ProductionCost = $scope.ProductionCost.filter(function (item) {
                        return item !== arrayItem;
                    });
                    $rootScope.destroyDataTable();
                    setTimeout(function () {
                        $scope.$apply();
                        $rootScope.dataTable();
                        alertify.success('Numero borrado correctamente!');
                    }, 0);
                }
            });
        }
        $scope.added = 0;
        $scope.agregarToList = function()
        {
            //console.log("Value of verify:", $scope.StaticProductionCostObject.Verify());
            //console.log("hi:", $scope.StaticProductionCostObject)
            if ($scope.StaticProductionCostObject.Verify()) {
                $scope.added++;
               // console.log("value of StaticProductionCostObject", $scope.StaticProductionCostObject);
                var localProductionCost = new GetProductionCost(null,
                    
                    $scope.StaticProductionCostObject.RaffleId, $scope.StaticProductionCostObject.Detalle,
                    $scope.StaticProductionCostObject.Cantidad, $scope.StaticProductionCostObject.Monto);
                localProductionCost.Id = null;
                $scope.ProductionCost.push(localProductionCost);
                //console.log("localProductionCost", localProductionCost);
                //console.log("total values in productionCost:", $scope.ProductionCost);
                $scope.StaticProductionCostObject.Clear();
            }
        }

        self.sorteoList = function () {
            window.loading.show();
            $.ajax({
                type: 'GET',
                dataType: 'json',
                url: 'Config/GetSorteoList',
                success: function (data) {
                
                   $('#raffleS').select2(
				   {
				       placeHolder: "Seleccione un sorteo",
				       data: data,
                       allowClear:true
				   });
                }
            });
            window.loading.hide();
            
        }

        $scope.guardar = function()
        {
            if($scope.ProductionCost.length > 0)
            {
                self.saveData();
            }
        }

        self.saveData = function()
        {
            //se concatenan los array para agregar los borrados de la lista.
            $scope.ProductionCost = $scope.ProductionCost.concat($scope.deleteArrayItems);
            window.loading.show();
            $.ajax({
                type: 'POST',
               // dataType: 'json',
                contentType: 'application/json',
                url: 'Config/SaveProductionCost',
                data: JSON.stringify($scope.ProductionCost),
                success: function (data1) {
                    window.loading.hide();
                    //mensaje indicando alguna accion!
                    //la tabla queda vacia
                    $scope.deleteArrayItems = [];
                    $scope.ProductionCost = [];
                    $scope.$apply();
                    alertify.success('Datos guardados correctamente!');
                }
            });
           
        }
        $scope.noUpdate = 0;
        $scope.UpdateData = function()
        { 
            if ($scope.ProductionCost.length > 0 && $scope.noUpdate > 0 && $scope.added > 0) {
                //mensaje a mostrar indicando que puede perder data
                //si el usuario lo permite cargar data
                alertify.confirm("&iquest;Si cambia de sorteo se perdera el trabajo realizado, Desea proceder ?", function (e) {
                    if (e) {
                        self.loadProductCost();
                        $scope.arrayItem = [];
                        $scope.deleteArrayItems = [];
                        $scope.added = 0;
                     }
                    });
            } else {
                //no hay perdida de data, se cambia de raffleId 
                self.loadProductCost();
                $scope.noUpdate++;
            }
        }

        self.loadProductCost = function()
        {
            window.loading.show();
            $.ajax({
                type: 'GET',
                dataType: 'json',
                url: "Config/GetProductionCost?raffleId=" + $scope.StaticProductionCostObject.RaffleId,
                success: function (data) {
                    $scope.ProductionCost = data;
                    $scope.$apply();
                    //for (var i = 0; i < data.length; i++) {
                    //    $scope.ProductionCost.push(data[i]);
                    //}
                    $scope.$apply();
                }
            });
            window.loading.hide();

        }
    
       
        self.sorteoList();
   }
})();
