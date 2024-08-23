/**=========================================================
 * Module: RoutesConfig.js
 =========================================================*/

(function () {
    'use strict';

    angular
        .module('naut')
        .config(routesConfig);

    routesConfig.$inject = ['$locationProvider', '$stateProvider', '$urlRouterProvider', 'RouteProvider'];
    function routesConfig($locationProvider, $stateProvider, $urlRouterProvider, Route) {
        // use the HTML5 History API
        $locationProvider.html5Mode(false);

        // Default route
        $urlRouterProvider.otherwise('/dashboard');

        // Application Routes States
        $stateProvider
            .state('app', {
                abstract: true,
                controller: "CoreController",
                resolve: {
                    _assets: Route.require('icons', 'toaster', 'animate')
                }
            })
            .state('app.dashboard', {
                url: '/dashboard',
                templateUrl: Route.base('Dashboard/Index'),
                resolve: {}
            })
            .state('app.aboutUs', {
                url: '/dashboard/aboutUs',
                templateUrl: Route.base('Dashboard/AboutUs'),
                resolve: {}
            })
            /*PROSPECT*/
            .state('app.prospectlist', {
                url: '/prospect/prospects',
                templateUrl: Route.base('Prospect/List'),
                resolve: {}
            })
            .state('app.prospectSuspends', {
                url: '/prospect/suspends',
                templateUrl: Route.base('Prospect/SuspendedList'),
                resolve: {}
            })
            .state('app.prospectDetails', {
                url: '/prospect/details:/prospectId',
                templateUrl: Route.base('Prospect/Details'),
                resolve: {}
            })
            .state('app.prospectcreate', {
                url: '/prospect/prospect',
                templateUrl: Route.base('Prospect/Create'),
                resolve: {}
            })
            .state('app.prospectCreateWizard', {
                url: '/prospect/create/wizard/:id',
                templateUrl: Route.base('Prospect/ProspectCreateWizard'),
                resolve: {}
            })
            .state('app.prospectViewer', {
                url: '/prospect/viewer/:prospectId',
                templateUrl: Route.base('Prospect/_ProspectVerifyPartial'),
                resolve: {}
            })
            .state('app.prospectWorkflowList', {
                url: '/prospect/workflows',
                templateUrl: Route.base('Prospect/WorkflowList'),
                resolve: {}
            })
            .state('app.checkPaymentflowList', {
                url: '/identifybachWorkFlow/workflows',
                templateUrl: Route.base('IdentifyBach/WorkflowList'),
                resolve: {}
            })
            .state('app.prospectApprovedProspectProcess', {
                url: '/prospect/proccess/:id',
                templateUrl: Route.base('Prospect/ApprovedProcess'),
                resolve: {}
            })
            .state('app.prospectAllProspect', {
                url: '/prospect/prospect/all',
                templateUrl: Route.base('Prospect/AllProspects'),
                resolve: {}
            })
            //Types Award
            .state('app.typesawardcreate', {
                url: '/typesAward/typesAward',
                templateUrl: Route.base('TypesAward/Create'),
                resolve: {}
            })
            .state('app.typesawardlist', {
                url: '/typesAward/typesAwards',
                templateUrl: Route.base('TypesAward/List'),
                resolve: {}
            })
            .state('app.typesawardAwardNumber', {
                url: '/typesAward/awardNumber',
                templateUrl: Route.base('TypesAward/AwardNumber'),
                resolve: {}
            })
            .state('app.typesawardPrintList', {
                url: '/typesAward/printList',
                templateUrl: Route.base('TypesAward/PrintList'),
                resolve: {}
            })
            .state('app.ReturnedTraceability', {
                url: '/typesAward/returnedTraceability',
                templateUrl: Route.base('TypesAward/ReturnedTraceability'),
                resolve: {}
            })
            .state('app.IdentifyAwardsReports', {
                url: '/typesAward/identifyAwardsReports',
                templateUrl: Route.base('TypesAward/IdentifyAwardsReports'),
                resolve: {}
            })
            .state('app.IdentifyAwardsReportsResumen', {
                url: '/typesAward/identifyAwardsReportsResumen',
                templateUrl: Route.base('TypesAward/IdentifyAwardsReportsResumen'),
                resolve: {}
            })
            .state('app.awardsCertifications', {
                url: '/ticket/awardsCertifications',
                templateUrl: Route.base('TypesAward/AwardsCertifications'),
                resolve: {}
            })
            .state('app.awardsCertWorkflowList', {
                url: '/ticket/awardsCertificationsWorkflowList',
                templateUrl: Route.base('TypesAward/AwardsCertWorkflowList'),
                resolve: {}
            })
            .state('app.approvedAwardsCertProcess', {
                url: '/ticket/award/approve/:workflowId',
                templateUrl: Route.base('TypesAward/ApprovedAwardsCertProcess')
            })
            .state('app.VentaLoteria365', {
                url: '/ticket/VentaLoteria365',
                templateUrl: Route.base('VentaLoteria365/VentaLoteria365'),
                resolve: {}
            })
            /*    SECURITY    */
            //Users
            .state('app.securityUserList', {
                url: '/security/users',
                templateUrl: Route.base('Security/UserList'),
                resolve: {}
            })
            .state('app.securityUserCreate', {
                url: '/security/user',
                templateUrl: Route.base('Security/UserCreate'),
                resolve: {}
            })
            .state('app.securityRolListInUser', {
                url: '/security/user/rols',
                templateUrl: Route.base('Security/RolListInUser'),
                resolve: {}
            })
            .state('app.securityLogin', {
                url: '/login',
                templateUrl: Route.base('Security/Login'),
                resolve: {}
            })
            .state('app.securityChangePassword', {
                url: '/security/changepassword',
                templateUrl: Route.base('Security/ChangePassword'),
                resolve: {}
            })
            //Roles
            .state('app.securityRolList', {
                url: '/security/rols',
                templateUrl: Route.base('Security/RolList'),
                resolve: {}
            })
            .state('app.securityRolCreate', {
                url: '/security/rol',
                templateUrl: Route.base('Security/RolCreate'),
                resolve: {}
            })
            .state('app.securityUserListInRol', {
                url: '/security/rol/users',
                templateUrl: Route.base('Security/UserListInRol'),
                resolve: {}
            })
            .state('app.securityModuleListInRol', {
                url: '/security/rol/modules',
                templateUrl: Route.base('Security/ModuleListInRol'),
                resolve: {}
            })
            .state('app.securityAccessDenied', {
                url: '/general/accessDenied',
                templateUrl: Route.base('Security/AccessDenied'),
                resolve: {}
            })
            .state('app.securityOfficeListInRol', {
                url: '/security/rol/offices',
                templateUrl: Route.base('Security/OfficeListInRol'),
                resolve: {}
            })
            //Auditory
            .state('app.securityLogs', {
                url: '/security/logs',
                templateUrl: Route.base('Security/Logs'),
                resolve: {}
            })
            //Modules
            .state('app.securityModuleList', {
                url: '/security/modules',
                templateUrl: Route.base('Security/ModuleList'),
                resolve: {}
            })
            .state('app.securityModuleCreate', {
                url: '/security/module',
                templateUrl: Route.base('Security/ModuleCreate'),
                resolve: {}
            })
            //Catalog
            .state('app.configCatalogCreate', {
                url: '/config/catalog',
                templateUrl: Route.base('Config/CatalogCreate'),
                resolve: {}
            })
            .state('app.configCatalogList', {
                url: '/config/catalogs',
                templateUrl: Route.base('Config/CatalogList'),
                resolve: {}
            })
            //WorkflowType
            .state('app.configWorkflowTypeList', {
                url: '/config/workflow/types',
                templateUrl: Route.base('Config/WorkflowTypeList'),
                resolve: {}
            })
            .state('app.configWorkflowTypeCreate', {
                url: '/config/workflow/type',
                templateUrl: Route.base('Config/WorkflowTypeCreate'),
                resolve: {}
            })
            //WorkflowTypeUser
            .state('app.configWorkflowTypeUserList', {
                url: '/config/workflow/users',
                templateUrl: Route.base('Config/WorkflowTypeUserList'),
                resolve: {}
            })
            .state('app.configWorkflowTypeUserCreate', {
                url: '/config/workflow/user',
                templateUrl: Route.base('Config/WorkflowTypeUserCreate'),
                resolve: {}
            })
            .state('app.configSystem', {
                url: '/config/system',
                templateUrl: Route.base('Config/SystemConfig'),
                resolve: {}
            })
            .state('app.emailConfig', {
                url: '/config/email',
                templateUrl: Route.base('Config/ConfigEmail'),
                resolve: {}
            })
            .state('app.configProductionCost', {
                url: '/config/costoProduccion',
                templateUrl: Route.base('Config/CostosProduccion'),
                resolve: {}
            })
            /*CLIENT*/
            .state('app.clientList', {
                url: '/client/clients',
                templateUrl: Route.base('Client/List'),
                resolve: {}
            })
            .state('app.otherincomesList', {
                url: '/others/otherIncomesList',
                templateUrl: Route.base('OtherIncomes/OtherIncomesList'),
                resolve: {}
            })
            .state('app.otherIncomeAccount', {
                url: '/others/otherIncomes/:otherincomeId',
                templateUrl: Route.base('OtherIncomes/Create'),
                resolve: {}
            })
            .state('app.otherIncomesPaymentHistory', {
                url: '/others/otherIncomesPaymentHistory/:otherincomeId',
                templateUrl: Route.base('OtherIncomes/OtherIncomesPaymentHistory'),
                resolve: {}
            })
            .state('app.otherIncomePaymentByGroup', {
                url: '/others/otherIncomePaymentByGroup/:otherIncomeGroupId',
                templateUrl: Route.base('OtherIncomes/OtherIncomePaymentByGroup'),
                resolve: {}
            })
            .state('app.otherincomesPaymentsList', {
                url: '/others/otherIncomesPayment',
                templateUrl: Route.base('OtherIncomes/OtherIncomesPaymentList'),
                resolve: {}
            })
            .state('app.otherIncomeAccountPayment', {
                url: '/others/otherIncomesPayment/:otherIncomeGroupId',
                templateUrl: Route.base('OtherIncomes/CreatePayment'),
                resolve: {}
            })
            .state('app.otherIncomeGroupPayment', {
                url: '/others/otherIncomeGroup/:otherIncomeGroupId',
                templateUrl: Route.base('OtherIncomes/CreateGroupPayment'),
                resolve: {}
            })
            .state('app.winnersList', {
                url: '/winner/winners',
                templateUrl: Route.base('Winner/List'),
                resolve: {}
            })
            .state('app.winnerCreate', {
                url: '/winner/winner/:winnerId',
                templateUrl: Route.base('Winner/Create'),
                resolve: {}
            })
            .state('app.ClientGeneralReport', {
                url: '/client/clientGeneralReport',
                templateUrl: Route.base('Client/ClientGeneralReport')
            })
            .state('app.clientCreate', {
                url: '/client/client/:clientId',
                templateUrl: Route.base('Client/Create'),
                resolve: {}
            })
            .state('app.clientListPrint', {
                url: '/client/listPrint',
                templateUrl: Route.base('Client/ListPrint'),
                resolve: {}
            })
            .state('app.clientWorkflowList', {
                url: '/client/workflows',
                templateUrl: Route.base('Client/WorkflowList'),
                resolve: {}
            })
            .state('app.clientApprovedClientProcess', {
                url: '/client/workflow/approveProcess',
                templateUrl: Route.base('Client/ApprovedClientProcess'),
                resolve: {}
            })
            /*RAFFLE*/
            .state('app.solteoList', {
                url: '/raffle/raffles',
                templateUrl: Route.base('Raffle/List'),
                resolve: {}
            })
            .state('app.activeRaffle', {
                url: '/raffle/ActiveRaffle',
                templateUrl: Route.base('Raffle/ActiveRaffle'),
                resolve: {}
            })
            .state('app.generatedRaffleList', {
                url: '/raffle/generateds',
                templateUrl: Route.base('Raffle/GeneratedList'),
                resolve: {}
            })
            .state('app.solteoCreate', {
                url: '/raffle/raffle/:raffleId',
                templateUrl: Route.base('Raffle/Create'),
                resolve: {}
            })
            .state('app.raffleAwardNumber', {
                url: '/raffle/awardNumber',
                templateUrl: Route.base('Raffle/RaffleAwardNumber'),
                resolve: {}
            })
            .state('app.raffleVirtualRaffle', {
                url: '/raffle/virtual/:raffleId',
                templateUrl: Route.base('Raffle/VirtualRaffle')
            })
            .state('app.printReportList', {
                url: '/raffle/print/printReports',
                templateUrl: Route.base('Raffle/PrintReportList')
            })
            //NUEVO REPORTE DE BILLETES DE CADA SORTEO
            .state('app.printTicketReport', {
                url: '/raffle/print/printTicketReport',
                templateUrl: Route.base('Raffle/PrintTicketReport')
            })
            //NUEVO REPORTE DE PREMIOS PAGABLES
            .state('app.printPayableAwards', {
                url: '/raffle/print/printPayableAwards',
                templateUrl: Route.base('Raffle/PrintPayableAwards')
            })
            //NUEVO REPORTE DE SORTEO
            .state('app.printRaffleReport', {
                url: '/raffle/print/printRaffleReport',
                templateUrl: Route.base('Raffle/PrintRaffleReport')
            })
            .state('app.inspectorReports', {
                url: '/raffle/print/inspectorReports',
                templateUrl: Route.base('Raffle/InspectorReports')
            })
            .state('app.creditReports', {
                url: '/client/print/creditReports',
                templateUrl: Route.base('Client/CreditReports')
            })
            .state('app.creditReportsByRaffle', {
                url: '/raffle/print/creditReportsByRaffle',
                templateUrl: Route.base('Raffle/CreditReportsByRaffle')
            })
            .state('app.rafflePrintGeneraed', {
                url: '/raffle/print/generated',
                templateUrl: Route.base('Raffle/PrintGeneratedList')
            })
            .state('app.PrintGeneratedTypes', {
                url: '/raffle/print/generatedTypes',
                templateUrl: Route.base('Raffle/PrintGeneratedTypes')
            })
            .state('app.raffleCopyAllocation', {
                url: '/raffle/copy/:raffleId',
                templateUrl: Route.base('TicketAllocation/CopyAllocation')
            })
            .state('app.raffleOpenReturned', {
                url: '/raffle/openAllocation',
                templateUrl: Route.base('Raffle/OpenReturned')
            })
            .state('app.ticketReturnQuery', {
                url: '/ticket/returnedListQuery',
                templateUrl: Route.base('TicketReturned/ReturnedListQuery')
            })
            .state('app.ticketReturnValidation', {
                url: '/ticket/returnedValidation',
                templateUrl: Route.base('TicketReturned/ReturnedValidation')
            })
            .state('app.ticketReturnGroupDetails', {
                url: '/ticket/returnedGroupDetails/:group/:raffleId',
                templateUrl: Route.base('TicketReturned/ReturnedGroupDetails')
            })
            .state('app.ticketReturnGroupClientDetails', {
                url: '/ticket/returnedGroupClientDetails/:group/:clientId/:raffleId',
                templateUrl: Route.base('TicketReturned/ReturnedGroupClientDetails')
            })
            .state('app.ticketElectronicSalesDetails', {
                url: '/ticket/electronicSalesGroupDetails/:allocationId',
                templateUrl: Route.base('ElectronicSales/ElectronicSalesGroupDetails')
            })
            .state('app.ticketReturnValidationGroupDetails', {
                url: '/ticket/returnedValidationGroupDetails/:group/:raffleId',
                templateUrl: Route.base('TicketReturned/ReturnedValidationGroupDetails')
            })
            /*Reprint*/
            .state('app.reprintTicketList', {
                url: '/ticket/reprintList',
                templateUrl: Route.base('TicketPrint/TicketReprintList')
            })
            .state('app.reprintTicketCreate', {
                url: '/ticket/reprintCreate/:reprintId/:allocationId',
                templateUrl: Route.base('TicketPrint/TicketReprintCreate')
            })

            .state('app.reprintWorkflows', {
                url: '/ticket/reprintWorkflows',
                templateUrl: Route.base('TicketPrint/ReprintWorkflowList')
            })
            .state('app.reprintTicketApproved', {
                url: '/ticket/reprint/approve/:workflowId',
                templateUrl: Route.base('TicketPrint/ApprovedReprintProcess')
            })
            .state('app.reprintTicketRepornt', {
                url: '/ticket/reprintRepornt',
                templateUrl: Route.base('TicketPrint/TicketReprint')
            })


            /*EMPLOYEE*/
            .state('app.employeeList', {
                url: '/employe/employes',
                templateUrl: Route.base('Employee/List'),
                resolve: {}
            })
            .state('app.employeeCreate', {
                url: '/employe/employe',
                templateUrl: Route.base('Employee/Create'),
                resolve: {}
            })
            //Department
            .state('app.departmentList', {
                url: 'employe/departments',
                templateUrl: Route.base('Employee/DepartmentList'),
                resolve: {}
            })
            .state('app.departmentCreate', {
                url: 'employe/department',
                templateUrl: Route.base('Employee/DepartmentCreate'),
                resolve: {}
            })
            /*EMPLOYEE*/
            .state('app.agencyList', {
                url: '/agency/agencys',
                templateUrl: Route.base('Agency/List'),
                resolve: {}
            })
            .state('app.agencyCreate', {
                url: '/agency/agency/:agencyId',
                templateUrl: Route.base('Agency/Create'),
                resolve: {}
            })
            /*TICKET ALLOCATION*/
            //Review
            .state('app.ticketReviewList', {
                url: '/ticket/review/allocation',
                templateUrl: Route.base('TicketPrint/TicketReviewList'),
                resolve: {}
            })
            /**/
            //Asignación
            .state('app.ticketAllocationList', {
                url: '/ticket/allocations',
                templateUrl: Route.base('TicketAllocation/TicketAllocationList'),
                resolve: {}
            })
            .state('app.ticketAllocationConsignatedList', {
                url: '/ticket/ConsignmentAllocations',
                templateUrl: Route.base('TicketAllocation/TicketAllocationConsignmentList'),
                resolve: {}
            })
            .state('app.ticketAllocationDeliverList', {
                url: '/ticket/deliverAllocations',
                templateUrl: Route.base('TicketAllocation/TicketAllocationToDeliverList'),
                resolve: {}
            })
            .state('app.electronicSales', {
                url: '/ticket/electronicSales',
                templateUrl: Route.base('ElectronicSales/ElectronicSalesList'),
                resolve: {}
            })
            .state('app.ticketPrintList', {
                url: '/ticket/ticketPrint',
                templateUrl: Route.base('TicketPrint/TicketPrint'),
                resolve: {}
            })
            .state('app.allocationSummaryReports', {
                url: '/ticket/allocationSummaryReports',
                templateUrl: Route.base('TicketAllocation/AllocationSummaryReports'),
                resolve: {}
            })
            .state('app.allocationPrintList', {
                url: '/ticket/allocationPrintList',
                templateUrl: Route.base('TicketAllocation/AllocationPrintList'),
                resolve: {}
            })
            .state('app.ticketAllocationCreate', {
                url: '/ticket/allocation/:allocationId',
                templateUrl: Route.base('TicketAllocation/TicketAllocationCreate'),
                resolve: {}
            })
            .state('app.ticketAllocationDetails', {
                url: '/ticket/allocationDetails/:allocationId',
                templateUrl: Route.base('TicketAllocation/AllocationDetails'),
                resolve: {}
            })
            .state('app.ticketAllocationConsignedDetails', {
                url: '/ticket/allocationConsignedDetails/:allocationId',
                templateUrl: Route.base('TicketAllocation/AllocationConsignedDetails'),
                resolve: {}
            })
            .state('app.ticketAllocationDeliverDetails', {
                url: '/ticket/allocationDeliverDetails/:allocationId',
                templateUrl: Route.base('TicketAllocation/AllocationDeliverDetails'),
                resolve: {}
            })
            .state('app.invoicePaymentHistory', {
                url: '/cash/invoicePaymentHistory/:invoiceId',
                templateUrl: Route.base('Cash/InvoicePaymentHistory'),
                resolve: {}
            })
            .state('app.AwardsHistory', {
                url: '/winner/awardsHistory/:winnerId',
                templateUrl: Route.base('Winner/AwardsHistory'),
                resolve: {}
            })
            .state('app.electronicSalesDetails', {
                url: '/electronic/electronicSales/:allocationId',
                templateUrl: Route.base('ElectronicSales/ElectronicSalesDetails'),
                resolve: {}
            })
            .state('app.ticketNumberDetail', {
                url: '/ticket/numberdetail/:numberId',
                templateUrl: Route.base('TicketAllocation/NumberDetail'),
                resolve: {}
            })
            //IdentifyBach
            .state('app.ticketIdentifyBachList', {
                url: '/ticket/identifybachlist',
                templateUrl: Route.base('TicketAllocation/IdentifyAwardList'),
                resolve: {}
            })
            .state('app.sellerAwardsList', {
                url: '/ticket/sellerawardlist',
                templateUrl: Route.base('TicketAllocation/SellerAwardList'),
                resolve: {}
            })
            .state('app.ticketIdentifyBachListToPay', {
                url: '/ticket/identifybachlisttopay',
                templateUrl: Route.base('TicketAllocation/IdentifyAwardListToPay'),
                resolve: {}
            })
            .state('app.ticketIdentifyBach', {
                url: '/ticket/identifybach/:identifyId',
                templateUrl: Route.base('TicketAllocation/IdentifyAward'),
                resolve: {}
            })
            .state('app.ticketIdentifyBachSeller', {
                url: '/ticket/identifybachseller/:identifyId',
                templateUrl: Route.base('TicketAllocation/IdentifyAwardSeller'),
                resolve: {}
            })
            .state('app.ticketIdentifyBachDetail', {
                url: '/ticket/identifybachdetail/:identifyId',
                templateUrl: Route.base('TicketAllocation/IdentifyAwardDetail'),
                resolve: {}
            })
            .state('app.ticketIdentifyBachSellerDetail', {
                url: '/ticket/identifybachsellerdetail/:identifyId',
                templateUrl: Route.base('TicketAllocation/IdentifySellerAwardDetail'),
                resolve: {}
            })
            .state('app.ticketIdentifyBachToPayDetail', {
                url: '/ticket/identifybachtopaydetail/:identifyId',
                templateUrl: Route.base('TicketAllocation/IdentifyAwardToPayDetail'),
                resolve: {}
            })
            //REVISAR ESTA PARTE PARA DARLE UTILIDAD
            .state('app.ticketIdentifyBachSellerToPayDetail', {
                url: '/ticket/identifybachsellertopaydetail/:identifyId',
                templateUrl: Route.base('TicketAllocation/IdentifyAwardSellerToPayDetail'),
                resolve: {}
            })
            .state('app.ticketAllocationsPrint', {
                url: '/ticket/allocationPrint',
                templateUrl: Route.base('TicketAllocation/AllocationListPrint'),
                resolve: {}
            })
            //Returned
            .state('app.ticketReturned', {
                url: '/ticket/returned',
                templateUrl: Route.base('TicketReturned/Returned')
            })
            .state('app.ticketReturnedList', {
                url: '/ticket/Returneds',
                templateUrl: Route.base('TicketReturned/ReturnedList'),
                resolve: {}
            })
            .state('app.ticketReturnedAward', {
                url: '/ticket/Returned/awards',
                templateUrl: Route.base('TicketReturned/ReturnedAwardList'),
                resolve: {}
            })
            .state('app.ticketReturnedAwardPrint', {
                url: '/ticket/returnedPrint',
                templateUrl: Route.base('TicketReturned/PrintReturnedList'),
                resolve: {}
            })
            .state('app.ticketReturnedByGroup', {
                url: '/ticket/returnedByGroup',
                templateUrl: Route.base('TicketReturned/PrintReturnedByGroup'),
                resolve: {}
            })
            .state('app.returnedAwardReports', {
                url: '/ticket/returnedAwardReports',
                templateUrl: Route.base('TicketReturned/ReturnedAwardReports'),
                resolve: {}
            })
            .state('app.ticketReassign', {
                url: '/ticket/reassign/:idAssign',
                templateUrl: Route.base('TicketAllocation/TicketReassign'),
                resolve: {}
            })
            //Abonado
            .state('app.ticketSuscriberList', {
                url: '/ticket/suscribers',
                templateUrl: Route.base('TicketSuscriber/TicketSuscriberList'),
                resolve: {}
            })
            .state('app.ticketSuscriberCreate', {
                url: '/ticket/suscriber/:id',
                templateUrl: Route.base('TicketSuscriber/TicketSuscriberCreate'),
                resolve: {}
            })
            .state('app.ticketSuscriberDetails', {
                url: '/ticket/suscriberDetails/:id',
                templateUrl: Route.base('TicketSuscriber/TicketSuscriberDetails'),
                resolve: {}
            })
            .state('app.ticketInvoicePrint', {
                url: '/ticket/invoicePrint',
                templateUrl: Route.base('TicketInvoice/InvoiceListPrint')
            })
            .state('app.ticketInvoiceList', {
                url: '/ticket/invoices',
                templateUrl: Route.base('TicketInvoice/InvoiceList')
            })
            .state('app.ticketInvoiceSuspendList', {
                url: '/ticket/invoices/suspend',
                templateUrl: Route.base('TicketInvoice/InvoiceListForSuspend')
            })
            .state('app.ticketInvoice', {
                url: '/ticket/invoice/:id',
                templateUrl: Route.base('TicketInvoice/Invoice')
            })
            /*    CASH    */
            .state('app.cashOpen', {
                url: '/cash/open',
                templateUrl: Route.base('Cash/OpenCash')
            })
            .state('app.cashReports', {
                url: '/cash/cashReports',
                templateUrl: Route.base('Cash/CashReports')
            })
            .state('app.dashboardRaffleReport', {
                url: '/raffle/dashboardRaffleReport',
                templateUrl: Route.base('Raffle/DashboardRaffleReport')
            })
            .state('app.AccountsReceivablesReports', {
                url: '/cash/accountsReceivablesReports',
                templateUrl: Route.base('Cash/AccountsReceivablesReports')
            })
            .state('app.AccountsReceivablesReportsByPeriod', {
                url: '/cash/accountsReceivablesReportsByPeriod',
                templateUrl: Route.base('Cash/AccountsReceivablesReportsByPeriod')
            })
            .state('app.payedAwardsByPeriod', {
                url: '/cash/payedAwardByPeriod',
                templateUrl: Route.base('Cash/PayedAwardByPeriod')
            })
            .state('app.electronicAwardPayed', {
                url: '/cash/ElectronicAwardPayed',
                templateUrl: Route.base('Cash/ElectronicAwardPayed')
            })
            .state('app.InvoiceByPeriod', {
                url: '/cash/invoiceByPeriod',
                templateUrl: Route.base('Cash/InvoiceByPeriod')
            })
            .state('app.AccountsReceivablesReportsExcel', {
                url: '/cash/accountsReceivablesReportsExcel',
                templateUrl: Route.base('Cash/AccountsReceivablesReportsExcel')
            })
            .state('app.PayedBachByPeriodExcel', {
                url: '/cash/payedBachByPeriodExcel',
                templateUrl: Route.base('Cash/PayedBachByPeriodExcel')
            })
            .state('app.AccountsReceivablesCloseReportsExcel', {
                url: '/cash/accountsReceivableClosesReportsExcel',
                templateUrl: Route.base('Cash/AccountsReceivablesCloseReportsExcel')
            })
            .state('app.PayableAwardsReportsExcel', {
                url: '/cash/payableAwardsReportsExcel',
                templateUrl: Route.base('Cash/PayableAwardsReportsExcel')
            })
            .state('app.clientGeneralBalance', {
                url: '/client/generalBalance',
                templateUrl: Route.base('Cash/ClientGeneralBalance')
            })
            .state('app.AccountsPaymentsReports', {
                url: '/cash/accountsPaymentsReports',
                templateUrl: Route.base('Cash/AccountsPaymentsReports')
            })
            .state('app.cashClose', {
                url: '/cash/close',
                templateUrl: Route.base('Cash/CashClose')
            })
            .state('app.cashReceivableList', {
                url: '/cash/receivables',
                templateUrl: Route.base('Cash/ReceivableList')
            })
            .state('app.cashReceivableCreate', {
                url: '/cash/receivable/:invoiceId',
                templateUrl: Route.base('Cash/Receivable')
            })
            .state('app.cashPayment', {
                url: '/cash/payment/:identifyBachId',
                templateUrl: Route.base('Cash/Payment')
            })
            .state('app.invoiceDetailCashReport', {
                url: '/report/invoiceDetail',
                templateUrl: Route.base('Cash/InvoiceDetailCashReports')
            })
            .state('app.invoiceDetailCashList', {
                url: '/report/invoiceDetailList',
                templateUrl: Route.base('Cash/InvoiceDetailList')
            })
            //nuevo reporte
            .state('app.ProfitabilityReport', {
                url: '/raffle/ProfitabilityRaffle',
                templateUrl: Route.base('raffle/ProfitabilityRaffle')
            })
            .state('app.cashNoteCreditList', {
                url: '/cash/noteCredits',
                templateUrl: Route.base('Cash/CreditNoteList')
            })
            .state('app.cashNoteTaxReceiptCreditList', {
                url: '/cash/noteCreditsTaxReceipt',
                templateUrl: Route.base('Cash/CreditNoteListTaxReceipt')
            })
            .state('app.cashAdvance', {
                url: '/cash/CashAdvances',
                templateUrl: Route.base('Cash/CashAdvanceList')
            })
            .state('app.positiveBalance', {
                url: '/cash/positiveBalance',
                templateUrl: Route.base('Cash/PositiveBalanceList')
            })
            .state('app.PositiveBalanceCreate', {
                url: '/cash/positiveBalance/:noteCreditId/:raffleAwardId',
                templateUrl: Route.base('Cash/PositiveBalance')
            })
            .state('app.taxReceiptList', {
                url: '/taxReceipt/taxReceiptList',
                templateUrl: Route.base('TaxReceipt/TaxReceiptList')
            })
            .state('app.cashNoteCreditCreate', {
                url: '/cash/noteCredit/:noteCreditId/:raffleAwardId',
                templateUrl: Route.base('Cash/CreditNote')
            })
            .state('app.cashNoteCreditTaxReceiptCreate', {
                url: '/cash/noteCreditTaxReceipt/:noteCreditId/:raffleAwardId',
                templateUrl: Route.base('Cash/CreditNoteTaxReceipts')
            })
            .state('app.cashAdvanceCreate', {
                url: '/cash/cashAdvance/:noteCreditId/:raffleAwardId',
                templateUrl: Route.base('Cash/CashAdvance')
            })
            .state('app.taxReceiptCreate', {
                url: '/taxReceipt/taxReceipt',
                templateUrl: Route.base('TaxReceipt/TaxReceipt')
            })
            .state('app.previousDebt', {
                url: '/cash/previousDebt',
                templateUrl: Route.base('Cash/PreviousDebt')
            })
            .state('app.electronicTicketXml', {
                url: '/integration/electronicTicketXml',
                templateUrl: Route.base('Integration/ElectronicTicketXml'),
                resolve: {}
            });
    }
})();
