/// <reference path="jquery.js" />
/// <reference path="jquery.validate.js" />
/// <reference path="bootstrap.js" />
/// <reference path="bootstrap-datepicker.js" />
/// <reference path="select2.js" />

$.validator.setDefaults({
    highlight: function (element) {
        $(element).closest(".control-group").addClass("error");
    },
    unhighlight: function (element) {
        $(element).closest(".control-group").removeClass("error");
    }
});

var supports_history_api = function () {
    return !!(window.history && history.pushState);
};

var is_touch_device = function() {
    return !!('ontouchstart' in window) ? 1 : 0;
};

var processPage = function () {
    $('a[data-link-bubble]').each(function () {
        var link = $(this);
        var bubbleTarget = link.closest(link.attr('data-link-bubble'));
        bubbleTarget.data('href', link.attr('href'));
        bubbleTarget.on('click', function () {
            window.location.href = $(this).data('href');
        });
        bubbleTarget.css('cursor', 'pointer');
        bubbleTarget.on('mouseover', function () {
            window.status = $(this).data('href');
        });
    });

    $('input[data-date-format]').each(function () {
        $(this).datepicker();
    });

    $('input[data-select-on-focus]').on('focus', function () {
        $(this).select();
    });

    $('input[data-select-on-focus]').on('mouseup', function (event) {
        event.preventDefault();
    });

    $(".input-validation-error").closest(".control-group").addClass("error");

    $("input[data-autocomplete-source]").each(function() {
        var targetSource = $(this).data("autocomplete-source");
        $(this).attr("autocomplete", "off");
        $(this).typeahead({
            source: function (query, typeahead) {
                return $.ajax({
                    type: "POST",
                    url: targetSource,
                    data: { q: query },
                    success: function(data) {
                        return typeahead(data);
                    },
                    dataType: "json"
                });
            }
        });
    });

    $('select[data-select2-enabled]').select2();

    //Put dropdown select item description to target
    $('select[data-description-target]').each(function () {
        showDomainValueDescription(this);
    });
};

$(function () {
    $.ajaxSetup({
        cache: false,
        traditional: true
    });

    $("#container").on('click', 'a[data-submit-form]', function () {
        $(this).closest("form").submit();
    });

    $("#container").on('change', 'select[data-submit-form]', function () {
        $(this).closest("form").submit();
    });

    $("#container").on('change', 'input[data-submit-form]', function () {
        $(this).closest("form").submit();
    });

    //Toggle (Display/hide) target data row
    $("#container").on('click', 'a[data-toggle-target-row]', function () {
        var target = $(this).attr('data-toggle-target-row');
        var curState = $(target).css("display");

        if (curState == "none")
            $(target).css("display", ""); //Empty value, fix for IE7 (Orignally table-row)
        else
            $(target).css("display", "none");
    });

    //Toggle (Display/hide) target
    $("#container").on('click', 'a[data-toggle-target]', function () {
        var target = $(this).attr('data-toggle-target');
        $(target).toggle();
    });

    $("#container").on('change', 'select[data-description-target]', function () {
        showDomainValueDescription(this);
    });

    if (supports_history_api()) {
        $("#container").on('click', 'div.pagination a[data-ajax-target]', function (e) {
            var url = $(this).attr('href');
            var ajaxTarget = $(this).attr('data-ajax-target');
            history.pushState(null, null, url);
            $.ajax({
                url: url,
                type: 'GET',
                success: function (data) {
                    $(ajaxTarget).html(data);
                }
            });
            e.preventDefault();
        });
    }

    processPage();

    if (!is_touch_device()) {
        $("#container").tooltip({
            selector: "a[rel=tooltip]"
        });
    }

    $(document).ajaxStart(function() {
        $("#ajaxLoader").show();
    });
    
    $(document).ajaxComplete(function () {
        $("#ajaxLoader").hide();
        processPage();
        $("form").removeData("validator");
        $("form").removeData("unobtrusiveValidation");
        $.validator.unobtrusive.parse("form");
    });
});

//Put dropdown select item description to target
var showDomainValueDescription = function (ddl) {
    var target = $(ddl).attr('data-description-target');
    var description = $(ddl).children('option:selected').attr('data-description');

    $(target).text(description);
};

var delay = (function () {
    var timer = 0;
    return function (callback, ms) {
        clearTimeout(timer);
        timer = setTimeout(callback, ms);
    };
})();