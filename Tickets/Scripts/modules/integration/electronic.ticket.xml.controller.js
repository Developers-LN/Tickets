/**=========================================================
 * Module: ElectronicTicketXmlController.js
 =========================================================*/

(function() {
    'use strict';

    angular
        .module('naut')
        .controller('ElectronicTicketXmlController', ElectronicTicketXmlController);

    ElectronicTicketXmlController.$inject = ['$scope', '$rootScope', '$state', 'Upload'];
    function ElectronicTicketXmlController($scope, $rootScope, $state, Upload) {
        this.getSolteoList = function (statu) {
            window.loading.show();
            var url = $rootScope.serverUrl + 'ticket/raffleApi/getRaffleSelect'
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: url,
                success: function (response) {
                    window.loading.hide();
                    $scope.raffles = response.object.map(function (raffle) {
                        raffle.StartReturnDate = new Date(raffle.StartReturnDate);
                        raffle.EndReturnDate = new Date(raffle.EndReturnDate);
                        return raffle;
                    });
                    $scope.$apply();
                    $rootScope.dataTable();
                }
            });
        }

        this.getSolteoList(70);
        $('#invoiceFileButton').click(function () {
            $('#xmlInvoice').trigger('click');
            
        });
        $scope.downloadRaffleReport = function (raffleId) {
            window.loading.show();
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: 'Reports/PrintImage?raffleId=' + raffleId,
                success: function (data) {
                    window.loading.hide();
                    if (data.result === true) {
                        window.location.href = '/generalRaffle/' + data.fileName;
                    } else {
                        alertify.alert(data.message);
                    }
                    $scope.$apply();
                }
            });
        }

        var url = 'http://' + location.host + '/';
        $scope.xmlAllocationDownload = function (raffleId) {
            // confirm dialog
            alertify.confirm("&iquest;Desea descargar el archivo XML de Asignaciones?", function (e) {
                if (e) {
                    window.loading.show();
                    $.ajax({
                        type: 'GET',
                        contentType: 'application/json; charset=utf-8',
                        url: 'Integration/AllocationNumberToXML?raffleId=' + raffleId,
                        success: function (data) {
                            window.loading.hide();
                            if (data.result == true) {
                                alertify.success(data.message);

                                var link = document.createElement("a");
                                link.download = name;
                                $(link).attr('download');
                                link.href = url + data.path;
                                link.click();
                            }
                            else {
                                alertify.alert(data.message);
                            }
                            $scope.$apply();
                        }
                    });
                }
            });
        }

        $scope.xmlAwardDownload = function (raffleId) {
            // confirm dialog
            alertify.confirm("&iquest;Desea descargar el archivo XML de Numeros Premiados?", function (e) {
                if (e) {
                    window.loading.show();
                    $.ajax({
                        type: 'GET',
                        contentType: 'application/json; charset=utf-8',
                        url: 'Integration/AwardNumberToXML?raffleId=' + raffleId,
                        success: function (data) {
                            window.loading.hide();
                            if (data.result == true) {
                                alertify.success(data.message);
                                var link = document.createElement("a");
                                link.download = name;
                                $(link).attr('download');
                                link.href = url + data.path;
                                link.click();
                            }
                            else {
                                alertify.alert(data.message);
                            }
                            $scope.$apply();
                        }
                    });
                }
            });
        }

        $scope.showUploadXmlInvoice = function (raffleId) {
            $scope.invoice.raffleId = raffleId;
            $('#showUploadXmlInvoice').modal('show');
        }

      

        $scope.invoice = {
            raffleId: 0,
            xmlInvoice: '',
            sourceFileName: ''
        };

        $('#xmlInvoice').change(function (e) {
            if (!e.currentTarget.files[0]) {
                $scope.invoice.xmlInvoice = '';
                $scope.invoice.sourceFileName = '';
                return;
            }

            var file = e.currentTarget.files[0];
            window.loading.show();
            self.uploadImage = Upload.upload({
                url: $rootScope.serverUrl + 'Controllers/FileUpload.ashx',
                data: { pathName: 'xmlInvoice ' },
                file: file, // or list of files ($files) for html5 only
            }).progress(function (evt) {
                $scope.fileProgress = parseInt(100.0 * evt.loaded / evt.total);
            }).success(function (data, status, headers, config) {
                window.loading.hide();
                if (data.result == true) {
                    $scope.invoice.xmlInvoice = data.fileUrl;
                    $scope.invoice.sourceFileName = data.sourceFileName;
                    alertify.success('Documento cargada correctamente');
                } else {
                    alertify.error(data.message);
                }
            }).error(function (err) {
                window.loading.hide();
                $scope.sourceFileName = '';
                alertify.alert('Error cargando el documento');
            });
        });

        $scope.clearXmlInvoiceLabel = function () {
            $scope.invoice.xmlInvoice = '';
            $scope.invoice.sourceFileName = '';
            $('#xmlInvoice').val('');
            window.setTimeout(function () {
                $scope.$apply();
            }, 0);
        }
        $('#showUploadXmlInvoice').on('hidden.bs.modal', function (e) {
            $scope.invoice = {
                raffleId: 0,
                xmlInvoice: '',
                sourceFileName: ''
            };
        });
        $scope.createInvioceXml = function () {
            if ($scope.invoice.xmlInvoice == '') {
                alertify.alert('Seleccione un archivo.');
                return;
            }
            window.loading.show();
            $.ajax({
                type: 'POST',
                dataType: 'json',
                url: 'Integration/CreateInvoiceXML',
                data: { raffleId: $scope.invoice.raffleId, path: $scope.invoice.xmlInvoice },
                success: function (data) {
                    window.loading.hide();
                    if (data.result === true) {
                        alertify.success(data.message);
                        $('#showUploadXmlInvoice').modal('hide');
                    } else {
                        $('#showUploadXmlInvoice').modal('hide');
                        alertify.alert(data.message);
                        
                    }
                }
            });
        }

        $scope.showUploadXmlPayed = function (raffleId) {
            $scope.Payed.raffleId = raffleId;
            $('#showUploadXmlPayed').modal('show');
        }

        $('#payedFileButton').click(function () {
            $('#xmlPayed').trigger('click');
        });

        $scope.Payed = {
            raffleId: 0,
            xmlPayed: '',
            sourceFileName: ''
        };

        $('#xmlPayed').change(function (e) {
            if (!e.currentTarget.files[0]) {
                $scope.Payed.xmlPayed = '';
                $scope.Payed.sourceFileName = '';
                return;
            }

            var file = e.currentTarget.files[0];
            window.loading.show();
            self.uploadImage = Upload.upload({
                url: $rootScope.serverUrl + 'Controllers/FileUpload.ashx',
                data: { pathName: 'xmlPayed ' },
                file: file, // or list of files ($files) for html5 only
            }).progress(function (evt) {
                $scope.fileProgress = parseInt(100.0 * evt.loaded / evt.total);
            }).success(function (data, status, headers, config) {
                window.loading.hide();
                if (data.result == true) {
                    $scope.Payed.xmlPayed = data.fileUrl;
                    $scope.Payed.sourceFileName = data.sourceFileName;
                    alertify.success('Documento cargado correctamente');
                } else {
                    alertify.error(data.message);
                }
            }).error(function (err) {
                window.loading.hide();
                $scope.Payed.sourceFileName = '';
                alertify.alert('Error cargando documento');
            });
        });

        $scope.clearXmlPayedLabel = function () {
            $scope.Payed.xmlPayed = '';
            $scope.Payed.sourceFileName = '';
            $('#xmlPayed').val('');
            window.setTimeout(function () {
                $scope.$apply();
            }, 0);
        }

        $scope.createBachXml = function () {
            if ($scope.Payed.xmlPayed == '') {
                alertify.alert('Seleccione un archivo.');
                return;
            }
            window.loading.show();
            $.ajax({
                type: 'POST',
                dataType: 'json',
                url: 'Integration/CreateBachXML',
                data: { raffleId: $scope.Payed.raffleId, path: $scope.Payed.xmlPayed },
                success: function (data) {
                    window.loading.hide();
                    if (data.result === true) {
                        alertify.success(data.message);
                        $('#showUploadXmlPayed').modal('hide');
                    } else {
                        var message = '';
                        data.messages.forEach(function (m) {
                            message += m + '<br/>';
                        });
                        alertify.alert(message);
                    }
                }
            });
        }
        $('#showUploadXmlPayed').on('hidden.bs.modal', function (e) {
            $scope.invoice = {
                raffleId: 0,
                xmlPayed: '',
                sourceFileName: ''
            };
        });
    }
})();
