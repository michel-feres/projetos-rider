// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Ajuste global para aceitar vírgula como separador decimal no client-side validation
// Substitui a validação de número do jQuery Validate para considerar vírgula e ponto
(function ($) {
    if ($ && $.validator && $.validator.methods) {
        $.validator.methods.number = function (value, element) {
            if (this.optional(element)) {
                return true;
            }
            // remover espaços e trocar vírgula por ponto antes de testar
            var val = String(value).replace(/\s/g, '').replace(',', '.');
            return !isNaN(Number(val));
        };
    }
})(jQuery);
