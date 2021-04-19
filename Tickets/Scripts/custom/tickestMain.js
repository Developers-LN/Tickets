$(function () {
    $(document).ready(function () {
        $('.number-fracction-barcode').each(function (i, ticket) {
            $(ticket.nextElementSibling).JsBarcode(ticket.value, {width:1, height:15});
        });
    });
});