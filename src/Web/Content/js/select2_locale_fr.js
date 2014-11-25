/**
 * Select2 French translation
 */
(function ($) {
    "use strict";

    $.extend($.fn.select2.defaults, {
        formatNoMatches: function () { return "Pas de résultat"; },
        formatInputTooShort: function (input, min) { var n = min - input.length; return "SVP ajouter " + n + " caractère" + (n == 1? "" : "s"); },
        formatInputTooLong: function (input, max) { var n = input.length - max; return "SVP enlever " + n + " caractère" + (n == 1? "" : "s"); },
        formatSelectionTooBig: function (limit) { return "Vous pouvez seulement sélectionner " + limit + " élément" + (limit == 1 ? "" : "s"); },
        formatLoadMore: function (pageNumber) { return "Chargement..."; },
        formatSearching: function () { return "Recherche..."; }
    });
})(jQuery);