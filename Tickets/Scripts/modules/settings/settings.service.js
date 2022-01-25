/**=========================================================
 * Module: SettingsService
 =========================================================*/

(function () {
    'use strict';

    angular
        .module('naut')
        .service('settings', settings);
    /* @ngInject */
    function settings($rootScope, $localStorage, $translate, $state) {
        /*jshint validthis:true*/
        var self = this;

        self.init = init;
        self.loadAndWatch = loadAndWatch;
        self.availableThemes = availableThemes;
        self.setTheme = setTheme;

        /////////////////

        self.themes = [
            { name: 'primary', sidebar: 'bg-white', sidebarHeader: 'bg-primary bg-light', brand: 'bg-primary', topbar: 'bg-primary' },
            { name: 'purple', sidebar: 'bg-white', sidebarHeader: 'bg-purple bg-light', brand: 'bg-purple', topbar: 'bg-purple' },
            { name: 'success', sidebar: 'bg-white', sidebarHeader: 'bg-success bg-light', brand: 'bg-success', topbar: 'bg-success' },
            { name: 'warning', sidebar: 'bg-white', sidebarHeader: 'bg-warning bg-light', brand: 'bg-warning', topbar: 'bg-warning' },
            { name: 'info', sidebar: 'bg-white', sidebarHeader: 'bg-info bg-light', brand: 'bg-info', topbar: 'bg-info' },
            { name: 'danger', sidebar: 'bg-white', sidebarHeader: 'bg-danger bg-light', brand: 'bg-danger', topbar: 'bg-danger' },
            { name: 'pink', sidebar: 'bg-white', sidebarHeader: 'bg-pink bg-light', brand: 'bg-pink', topbar: 'bg-pink' },
            { name: 'amber', sidebar: 'bg-white', sidebarHeader: 'bg-amber bg-light', brand: 'bg-amber', topbar: 'bg-amber' },
        ];
        $rootScope.notifications = [];
        function init() {
            // Global settings
            $rootScope.app = {
                name: 'Tickets',
                title: 'Inicio',
                description: 'Tickets de Loteria',
                year: new Date().getFullYear(),
                views: {
                    animation: 'ng-fadeInLeft2'
                },
                layout: {
                    isFixed: true,
                    isBoxed: false,
                    isDocked: false
                },
                sidebar: {
                    isOffscreen: false
                },
                footer: {
                    hidden: false
                },
                themeId: 0,
                // default theme
                theme: {
                    name: 'primary',
                    sidebar: 'bg-white',
                    sidebarHeader: 'bg-primary bg-light',
                    brand: 'bg-primary',
                    topbar: 'bg-primary'
                }
            };
        }
        $rootScope.dataTable = function (paging) {
            try {
                $('.dataTableGrid').each(function (i, d) {
                    $(d).DataTable({
                        destroy: true,
                        'oLanguage': {
                            'sUrl': 'Vendor/datatables/media/res/dt_ES.txt'
                        },
                        pageLength: 25,
                        paging: paging
                    });
                });
            } catch (e) { }
        }
        $rootScope.parseMoney = function (number) {
            return $('<span />').html(number).formatCurrency({ region: 'es-DO' }).text();
        }
        $rootScope.parseNumber = function (number) {
            return $rootScope.parseMoney(number).substring(3, $rootScope.parseMoney(number).length - 3);
        }
        $rootScope.destroyDataTable = function (datatableId) {
            try {
                if (datatableId) {
                    var dataTable = $('#' + datatableId).dataTable();
                    dataTable.fnClearTable();
                    dataTable.fnDraw();
                    dataTable.fnDestroy();
                    return;
                }
                $('.dataTableGrid').each(function (i, d) {
                    var dataTable = $(d).dataTable();
                    dataTable.fnClearTable();
                    dataTable.fnDraw();
                    dataTable.fnDestroy();
                });
            } catch (e) { }
        }
        function azt(number) {
            if ((number + '').length == 1) {
                return '0' + number;
            } else {
                return '' + number;
            }
        }
        $rootScope.parseDate = function (date, time) {
            return new Date(date.getFullYear(), azt(date.getMonth()), azt(date.getDate()), azt(time.getHours()), azt(time.getMinutes()), azt(time.getSeconds()));
        }

        $rootScope.verifyCanView = function (pageId) {
            return $rootScope.ticketsModuleList.some(function (page) {
                return page.name === pageId && page.view === true;
            });
        }
        $rootScope.addZeroToNumber = function (lenght, number) {
            lenght = (lenght + '').length;
            var stringNumber = number + '';
            while (stringNumber.length < lenght) {
                stringNumber = "0" + stringNumber;
            }
            return stringNumber;
        }
        $rootScope.verifyCanViewMenu = function (menu) {
            if (menu.children[1]) {
                var show = false;
                var subMenus = menu.children[1].children;
                for (var c = 0; c < subMenus.length; c++) {
                    if ($(subMenus[c]).hasClass('ng-hide') === false) {
                        show = true;
                    }
                }
                if (show === false) {
                    $(menu).addClass('ng-hide');
                }
            }
        }
        $rootScope.logOff = function () {

            // confirm dialog
            alertify.confirm("&iquest;Desea salir del sistema?", function (e) {
                if (e) {
                    $.ajax({
                        type: 'POST',
                        dataType: 'json',
                        url: 'Security/LogOff',
                        data: {},
                        success: function (data) {
                            window.location.href = '/Security/Login';
                        }
                    });
                }
            });
        }
        function loadAndWatch() {
            // Load current settings from local storage
            if (angular.isDefined($localStorage.settings))
                $rootScope.app = $localStorage.settings;
            else
                $localStorage.settings = $rootScope.app;

            $rootScope.$watch('app.layout', function () {
                $localStorage.settings = $rootScope.app;
            }, true);
        }

        function availableThemes() {
            return self.themes;
        }

        function setTheme(idx) {
            $rootScope.app.theme = this.themes[idx];
        }
        $rootScope.createSelect2 = function () {
            $('.dropdown-select2').select2();
        };
        $rootScope.verifyModuleAccess = function (module) {
            $rootScope.moduleCanEdit = module.edit === true ? '' : 'hide';
            $rootScope.moduleCanDelete = module.delete === true ? '' : 'hide';
            $rootScope.moduleCanAdd = module.add === true ? '' : 'hide';
            $rootScope.moduleCanEdit = module.edit === true ? '' : 'hide';
            $rootScope.moduleCanSearch = module.search === true ? '' : 'hide';
        }

        $rootScope.verifyUserLogin = function () {
            $.ajax({
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                url: 'Security/VerifyUserLogin',
                async: false,
                success: function (data) {
                    data = JSON.parse(data);
                    if (data !== false) {
                        $rootScope.authenticated = true;
                        $rootScope.ticketsModuleList = $rootScope.skipedModules.concat(data.modules);
                        $rootScope.user = { name: data.name };
                        $rootScope.loadNotifications();
                    } else {
                        window.location.href = '/Security/Login';
                    }
                },
                error: function (error) {
                    window.alert(error);
                }
            });
        }

        $rootScope.loadNotifications = function () {
            $.when(
                //$.ajax($rootScope.serverUrl + 'ticket/workflowApi/getWorkflowList?type=' + 3),//Tipo de Workflow para clientes
                $.ajax($rootScope.serverUrl + 'ticket/workflowApi/getWorkflowList?type=' + 1),//Tipo de Workflow para prospecto
                $.ajax($rootScope.serverUrl + 'ticket/workflowApi/getWorkflowList?type=' + 4),//Tipo de Workflow para reimprecion
                $.ajax("Cash/GetCreditNoteReturneds")
            ).done(function (prospectResponse, reprintResponse, creditNote) {
                if (creditNote[1] == 'success') {
                    creditNote[0].forEach(function (n) {
                        $rootScope.notifications.push({
                            title: 'Nota de Credito',
                            descripcion: 'Al cliente ' + n.clientname + ' se le agrego una nota de cr\u00e9dito por devoluci\u00f3n de billete del sorteo #' + n.raffleId,
                            link: '#/cash/receivables'
                        });
                    });
                }
                /*if (clientResponse[1] == 'success') {
                    clientResponse[0].forEach(function (n) {
                    $rootScope.notifications.push({
                        title: 'Aprobacion de Cliente',
                        descripcion: n.client.name,
                        link: '#/client/workflows'
                    });
                });
            }*/
                if (prospectResponse[1] == 'success') {
                    prospectResponse[0].object.forEach(function (n) {
                        $rootScope.notifications.push({
                            title: 'Aprobacion de Prospecto',
                            descripcion: n.prospect.name,
                            link: '#/prospect/proccess/' + n.id
                        });
                    });
                }
                if (reprintResponse[1] == 'success') {
                    reprintResponse[0].object.forEach(function (n) {
                        $rootScope.notifications.push({
                            title: 'Aprobacion de Reimpresion',
                            descripcion: n.reprint.note,
                            link: '#/ticket/reprint/approve/' + n.id
                        });
                    });
                }
            });
        }
    }
    settings.$inject = ['$rootScope', '$localStorage', '$translate', '$state'];
})();
