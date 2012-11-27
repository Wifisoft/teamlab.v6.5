var Search = new function() {

    this.Toggle = function(element, th) {
        var elem = jq('#' + element)
        if (elem.css('display') == 'none') {
            elem.css('display', 'block');
            jq('#' + th).attr("src", SkinManager.GetImage('collapse_down_dark.png'));
        }
        else {
            elem.css('display', 'none');
            jq('#' + th).attr("src", SkinManager.GetImage('collapse_right_dark.png'));
        }
    }
}