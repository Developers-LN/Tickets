window.loading = (function () {
    var pleaseWaitDiv = $('<div id="pleaseWaitDialog" class="modal fade bs-example-modal-sm" tabindex="-1" role="dialog" aria-labelledby="mySmallModalLabel" aria-hidden="true" data-backdrop="static" data-keyboard="false"><div class="modal-dialog modal-sm"><div class="modal-content"><div class="modal-body"><div class="progress progress-striped active"><div class="progress-bar progress-bar-info" role="" aria-valuenow="100" aria-valuemin="0" aria-valuemax="100" style="width: 100%"><span class="sr-only">100% Complete</span></div></div></div></div></div></div>');
    return {
        show: function () {
            if (this.isShow === false) {
                this.isShow = true;
                pleaseWaitDiv.modal();
            }
        },
        hide: function () {
            if (this.isShow === true) {
                this.isShow = false;
                pleaseWaitDiv.modal('hide');
            }
            $('div.modal-backdrop').remove();
        },
        isShow: false
    };
})();