/**=========================================================
 * Module: RaffleVirtualController.js
 =========================================================*/

(function() {
    'use strict';

    angular
        .module('naut')
        .controller('RaffleVirtualController', RaffleVirtualController);

    RaffleVirtualController.$inject = ['$scope', '$rootScope', '$state', '$stateParams'];
    function RaffleVirtualController($scope, $rootScope, $state, $stateParams) {
        /*jshint validthis:true*/
        var self = this;
        $scope.ShowAwardNumberList = [];

        this.validateDigitedAward = function(awards) {
            var error = '', isReq = ' es un campo requerido. <br>';
            $(awards).each(function (i, award) {
                if (Number(award.ControlNumber) > $scope.raffleDetails.ticketProspect.production) {
                    error += 'Numero ganador a ' + award.AwardName + ' ' + isReq;
                }
                if (award.AwardName.indexOf('FRACCION') >= 0) {
                    if (Number(award.Fraction) <= 0 || Number(award.Fraction) > $scope.maxFraction) {
                        error += 'Error en la fracci&oacute;n ganadora';
                    }
                }
            });
            if (error !== '') {
                alertify.showError('Alerta', error);
            }
            return error === '';
        }

        $scope.isNumber = function (e) {
            var char = String.fromCharCode(e.keyCode);
            var valid = /^[0-9]+$/.test(char);
            if (!valid) {
                e.preventDefault();
            }
            return valid;
        }

        $scope.usePatentValue = function (sourceId, currentId) {
            if (currentId !== null) {
                var number = $('#raffleAwardNumber-' + sourceId).val();
                $('#raffleAwardNumber-' + currentId).val (number);
            }
        }

        $scope.addZero = function (sourceId, currentId) {
            if (currentId !== null) {
                var number = $rootScope.addZeroToNumber($scope.raffleDetails.ticketProspect.production - 1, $('#raffleAwardNumber-' + sourceId).val());
                $('#raffleAwardNumber-' + sourceId).val(number);
            }
        }
       
        $('#showNumberModal').keydown(function (e) {
            if (e.which == 13) {
                $scope.getNextAwrad(1);
            }
        });

        this.getRaffleAwardDetails = function () {
            window.loading.show();
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: $rootScope.serverUrl + 'ticket/raffleApi/getRaffleDetails?id=' + $stateParams.raffleId,
                success: function (response) {
                    window.loading.hide();
                    if (response.result === false) {
                        alertify.alert(response.message);
                    } else {
                        self.showRaffleDetails(response.object);
                    }
                }
            });
        }

        this.showRaffleDetails = function (data) {
            $scope.raffleName = data.raffleDetails.name;
            $scope.raffleDetails = data.raffleDetails;
            $scope.maxLenthDigited = (($scope.raffleDetails.ticketProspect.production - 1) + '').length;
            $scope.digitedAwards = data.digitedAwards.map(function (d) {
                if (d.HasAward === true) {
                    d.ControlNumber = $rootScope.addZeroToNumber($scope.raffleDetails.ticketProspect.production - 1, d.ControlNumber);
                }
                return d;
            });
            $scope.awardTypes = data.awardTypes;
            $scope.maxFraction = data.maxFraction;

            $scope.raffleAwardListByAward = data.raffleAwardListByAward;
            $scope.restantAwardCount = data.restantAwardCount;
            if (data.digitedAwards.length > 0) {
                $scope.digitedAwardSuccess = !data.digitedAwards[0].HasRaffleAward;

            } else {
                $scope.digitedAwardSuccess = false;
            }
            $scope.$apply();
            window.setTimeout(function () {
                data.raffleAwardListByAward.forEach(function (typeAward) {
                    $('#number-grid-' + typeAward.typeAwardId).empty();
                    var quantity = $(data.restantAwardCount).filter(function (i, item) { return item.typeAwardId == typeAward.typeAwardId; })[0].quantity;
                    $('#pendientAward-' + typeAward.typeAwardId).text(quantity);
                });
                if (data.raffleAwardListByAward.length > 0) {
                    $scope.currentAwardType = data.raffleAwardListByAward[0];
                    self.showAwardNumbers($scope.currentAwardType.typeAwardId);
                    $scope.select(2);
                }
                $scope.$apply();
                window.loading.hide();
            }, 0);
        }

        this.showAwardNumbers = function (typeAwardId) {
            $('#number-grid-' + typeAwardId).empty();
            var awardTypes = $scope.raffleAwardListByAward.filter(function (a) {
                $('#number-grid-' + a.typeAwardId).empty();
                return a.typeAwardId === typeAwardId;
            });
            if (awardTypes.length <= 0) {
                return;
            }
            $scope.currentAwardType = awardTypes[0];
            
             $scope.currentAwardType.awardList.forEach(function (awardList) {
                var htmlNumberGrd = '<span class="col-lg-3"><span class="number-title">' + awardList.awardName + '</span><br/>';
                awardList.awardList.forEach(function (citem) {
                    htmlNumberGrd += '<span class="number-container">' + citem.number + '</span><br/>'
            });
                htmlNumberGrd += '</span>';
                $(htmlNumberGrd).appendTo('#number-grid-' + typeAwardId);
            });
            
           
        }

        this.currentAwardShow = {
            numberIndex: 0,
            number: '',
            awardName: '',
            typeIndex: 0
        };

        $scope.saveRaffleAward = function () {
            var awards = $scope.digitedAwards.map(function (award) {
                return {
                    ControlNumber: $('#raffleAwardNumber-' + award.Id).val(),
                    Fraction: $('#raffleAwardFraction-' + award.Id).val(),
                    RaffleId: award.RaffleId,
                    AwardId: award.Id,
                    AwardName: award.AwardName
                };
            });

            if (self.validateDigitedAward(awards) === true) {
                // confirm dialog
                alertify.confirm("&iquest;Desea generar el sorteo virtual?", function (e) {
                    if (e) {
                        window.loading.show();
                        $.ajax({
                            type: 'POST',
                            dataType: 'json',
                            contentType: 'application/json; charset=utf-8',
                            url: 'Raffle/RaffleGenerate',
                            data: JSON.stringify({ 'raffleAwards': awards }),
                            success: function (data) {
                                window.loading.hide();
                                if (data.Result == true) {
                                    self.getRaffleAwardDetails();
                                    alertify.success(data.Message);
                                } else {
                                    alertify.alert(data.Message);
                                }
                            }
                        });
                    }
                });
            }
        }

       
        $scope.showLastBall = function () {
            $scope.select(10);
            $scope.getNextAwrad(1);
        }

        $scope.getNextAwrad = function (direction) {
            var awardList = $scope.currentAwardType.awardList;
            self.currentAwardShow.numberIndex += direction;
            if (self.currentAwardShow.numberIndex < 0) {
                self.currentAwardShow.typeIndex--;
                if (self.currentAwardShow.typeIndex < 0) {
                    if ($scope.currentAwardType.typeAwardId == 3) {
                        self.currentAwardShow.typeIndex = 0;
                        self.currentAwardShow.numberIndex = -1;
                        $scope.getNextAwrad(1);
                        alertify.alert('Estas en el primer numero');
                    } else if ($scope.currentAwardType.typeAwardId == 4) {
                        $scope.select(3);
                        
                        self.currentAwardShow.typeIndex = $scope.currentAwardType.awardList.length - 1;
                        self.currentAwardShow.numberIndex = $scope.currentAwardType.awardList[self.currentAwardShow.typeIndex].awardList.length;
                        $scope.getNextAwrad(-1);
                    } if ($scope.currentAwardType.typeAwardId == 2) {
                        $scope.select(4);
                        self.currentAwardShow.typeIndex = $scope.currentAwardType.awardList.length - 1;
                        self.currentAwardShow.numberIndex = $scope.currentAwardType.awardList[self.currentAwardShow.typeIndex].awardList.length;
                        $scope.getNextAwrad(-1);
                    } else if ($scope.currentAwardType.typeAwardId == 10) {
                        $scope.select(2);
                        self.currentAwardShow.typeIndex = $scope.currentAwardType.awardList.length - 1;
                        self.currentAwardShow.numberIndex = $scope.currentAwardType.awardList[self.currentAwardShow.typeIndex].awardList.length;
                        $scope.getNextAwrad(-1);
                    }
                    return;
                }
                self.currentAwardShow.numberIndex = awardList[self.currentAwardShow.typeIndex].awardList.length - 1;
            }

            if (self.currentAwardShow.numberIndex >= awardList[self.currentAwardShow.typeIndex].awardList.length) {
                self.currentAwardShow.typeIndex++;
                self.currentAwardShow.numberIndex = 0;
            }
            if (self.currentAwardShow.typeIndex >= awardList.length) {
                if ($scope.currentAwardType.typeAwardId == 3) {
                    $scope.select(4);
                    $scope.getNextAwrad(1);
                    return;
                } else if ($scope.currentAwardType.typeAwardId == 4) {
                    $scope.select(2);
                    $scope.getNextAwrad(1);
                    return;
                } if ($scope.currentAwardType.typeAwardId == 2) {
                    $scope.select(10);
                    $scope.getNextAwrad(1);
                    return;
                }else if($scope.currentAwardType.typeAwardId == 10) {
                    alertify.alert('Se han mostrado todo los numeros');
                    self.currentAwardShow.typeIndex += -1;
                    self.currentAwardShow.numberIndex = awardList[self.currentAwardShow.typeIndex].awardList.length - 1;
                    return;
                }
            }

            self.currentAwardShow.awardName = awardList[self.currentAwardShow.typeIndex].awardName + '( ' + (self.currentAwardShow.numberIndex + 1) + ' de ' + awardList[self.currentAwardShow.typeIndex].awardList.length + ' )';
            self.currentAwardShow.number = awardList[self.currentAwardShow.typeIndex].awardList[self.currentAwardShow.numberIndex].number;

            $('#awardNumber-' + $scope.currentAwardType.typeAwardId).text(self.currentAwardShow.number);
            $('#awardName-' + $scope.currentAwardType.typeAwardId).text(self.currentAwardShow.awardName);
            $scope.currentShowAwardTitle = self.currentAwardShow.awardName;
            $scope.currentShowAwardNumber = self.currentAwardShow.number;

            $scope.lastShowAwardNumber = '';
            $scope.nextShowAwardNumber = '';
            if (self.currentAwardShow.numberIndex > 0) {
                $scope.lastShowAwardNumber = awardList[self.currentAwardShow.typeIndex].awardList[self.currentAwardShow.numberIndex - 1].number;
            }
            if ((self.currentAwardShow.numberIndex +1) < awardList[self.currentAwardShow.typeIndex].awardList.length ) {
                $scope.nextShowAwardNumber = awardList[self.currentAwardShow.typeIndex].awardList[self.currentAwardShow.numberIndex + 1].number;

            }
            
            if (direction > 0) {
                $scope.ShowAwardNumberList.push({
                    typeIndex: self.currentAwardShow.typeIndex,
                    numberIndex: self.currentAwardShow.numberIndex,
                    number: self.currentAwardShow.number
                });
                var d = $('.award-number-grid');
                d.scrollTop(d.prop("scrollHeight"));
            } else {
                var currentIndexAward = $scope.ShowAwardNumberList.length - 1;
                $scope.ShowAwardNumberList = $scope.ShowAwardNumberList.slice(0, currentIndexAward)
            }
            window.setTimeout(function () {
                $scope.$apply();
            }, 0);
        }

        $scope.select = function (awardTypeId) {
            self.currentAwardShow.typeIndex = 0;
            self.currentAwardShow.numberIndex = -1;
            self.showAwardNumbers(awardTypeId);
        };

        $scope.shwoAwardModal = function () {
            $scope.getNextAwrad(1);
            $scope.ShowAwardNumberList = [];
            $('#showNumberModal').modal('show');
        }

        $('#showNumberModal').on('hidden.bs.modal', function (e) {
            $scope.currentShowAwardTitle = '';
            $scope.currentShowAwardNumber = '';
            setTimeout(function () {
                $scope.$apply();
            }, 0);
        });


        self.getRaffleAwardDetails();
        $scope.currentShowAwardTitle = '';
        $scope.currentShowAwardNumber = '';
        $scope.lastShowAwardNumber = '';
        $scope.nextShowAwardNumber = '';
        
    }
})();
