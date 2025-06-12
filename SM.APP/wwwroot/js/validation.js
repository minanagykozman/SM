// Inside wwwroot/js/validation-init.js

$(function () {
    // Find all forms that have the class 'needs-validation'
    $('form.needs-validation').each(function () {
        var form = $(this);

        form.validate({
            // These are your GLOBAL settings
            errorClass: "text-danger",
            errorElement: "div",

            // This global setting makes validation work with Select2
            ignore: [],

            highlight: function (element, errorClass, validClass) {
                $(element).addClass('is-invalid');
                // Special handling for select2
                if ($(element).hasClass('form-select')) {
                    $(element).next('.select2-container').find('.select2-selection').addClass('is-invalid');
                }
            },
            unhighlight: function (element, errorClass, validClass) {
                $(element).removeClass('is-invalid');
                if ($(element).hasClass('form-select')) {
                    $(element).next('.select2-container').find('.select2-selection').removeClass('is-invalid');
                }
            },
            errorPlacement: function (error, element) {
                if (element.next('.select2-container').length) {
                    // Place error after the select2 container
                    error.insertAfter(element.next('.select2-container'));
                } else if (element.parent().hasClass('form-floating')) {
                    error.insertAfter(element.parent());
                } else {
                    error.insertAfter(element);
                }
            }
        });
    });

    // Global handler to make Select2 validation errors clear on change
    $('.form-select').on('change', function () {
        $(this).valid();
    });
});