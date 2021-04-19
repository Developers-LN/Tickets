	var enterKeyCode = 13;
	var self = this;
	window.TextboxKeyPress = function ($event) {
	    if ($event.keyCode === enterKeyCode) {
	        window.loginFormSubmit();
	    }
	}

	window.loginFormSubmit = function () {
	    var user = {
	        Password: $('#Password').val(),
	        Name: $('#UserName').val()
	    };
	    if (this.validate(user) === false) {
	        return;
	    }
	    window.loading.show();
	    $.ajax({
	        type: 'POST',
	        dataType: 'json',
	        url: '/Security/Login',
	        data: user,
	        success: function (_data) {
	            console.log(_data);
	            data = JSON.parse(_data.Data);

	            if (data.result === true) {
	                alertify.log('Iniciar sesion correctamente!');
	                if (_data.Url == undefined)
	                {
	                    window.location.href = '/#/dashboard';
	                } else {
	                    window.location.href = _data.Url;
	                }
	                
	            } else {
	                alertify.showError('Alerta', 'Su nombre de usuario o contrase&nacute;a no son correctos');
	                window.loading.hide();
	            }
	        }
	    });
	}

	this.validate = function (user) {
	    var error = '', isReq = ' es un campo requerido. <br>';
	    if (user.Name == '') {
	        error += 'El nombre del usuario' + isReq;
	    }
	    if (user.Password == '') {
	        error += 'La contrase&ntilde;a' + isReq;
	    }
	    if (error !== '') {
	        alertify.showError('Alerta', error);
	    }
	    return error === '';
	}
