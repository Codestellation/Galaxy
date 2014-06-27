function validateWithBootstrap() {
    $.validator.setDefaults({
        errorElement: "span",
        errorClass: "help-block",
        highlight: function(element, errorClass, validClass) {
            var formgroup = $(element).closest('.form-group');
            formgroup.removeClass('has-success');
            formgroup.addClass('has-error');
        },
        unhighlight: function(element, errorClass, validClass) {
            var formgroup = $(element).closest('.form-group');
            formgroup.removeClass('has-error');
            formgroup.addClass('has-success');
        },
        errorPlacement: function(error, element) {
            if (element.parent('.input-group').length || element.prop('type') === 'checkbox' || element.prop('type') === 'radio') {
                error.insertAfter(element.parent());
            } else {
                error.insertAfter(element);
            }
        }
    });
}