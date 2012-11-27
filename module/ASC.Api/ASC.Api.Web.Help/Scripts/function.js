$(function() {
    $(".prettyprint code").each(function(e, t) {
        var type = "";
        switch ($(t).attr("data-mediatype")) {
            case "application/json":
                $(t).html($(t).text().replace(/\n/g, '<br/>'));
                type = "language-js";
                break;
            case "text/xml":
                $(t).html($(t).html().replace(/\n/g, '<br/>'));
                type = "language-xml";
                break;
            default:
        }
        $(t).addClass(type);

    });
    $("a.param-name").hover(function(e) {
        $($(this).attr("href")).toggleClass('highlite');
    });
    prettyPrint();
    $('.tabs li:first').each(function(e, t) {
        $(t).addClass('active');
    });
    $('.tab-content div:first').each(function(e, t) {
        $(t).addClass('active');
    });
    $('.tabs').tabs();
});