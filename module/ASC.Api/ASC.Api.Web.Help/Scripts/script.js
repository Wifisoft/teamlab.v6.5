$(function() {
    $("#method_nav").treeview({
        collapsed: true,
        animated: "medium",
        unique: false,
        persist: "location"
    });
    $("#method_nav").show();
    $(".tip").twipsy({ live: false, placement: 'below', offset: 10 });
});

