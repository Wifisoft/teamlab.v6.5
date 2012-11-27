(function(jq, win, doc, body) {
    jq.fn.helper = function(options) {
        var options = jQuery.extend({
            addTop: -20,
            addLeft: -29,
            position: "absolute",
            fixWinSize: false,
            popup: false,
            BlockHelperID: '', //obligatory  parameter
            enableAutoHide: true,
            close: false
        }, options);

        return this.each(function() {
            var w = jq(window);
            var scrWidth = w.width();
            var scrHeight = w.height();
            var addTop = options.addTop;
            var addLeft = options.addLeft;
            var topPadding = w.scrollTop();
            var leftPadding = w.scrollLeft();

            if (options.position == "fixed") {
                addTop -= topPadding;
                addLeft -= leftPadding;
            }


            var $helpBlock = jq('#' + options.BlockHelperID);
            var elem = jq(this);
            var elemPos = elem.offset();
            var elemPosLeft = elemPos.left;
            var elemPosTop = elemPos.top - $helpBlock.outerHeight();

            if (options.popup) {
                elemPosTop = elem.position().top - $helpBlock.outerHeight();
                elemPosLeft = elem.position().left;
            }


            if (options.close) {
                if (jq('#closeHelpBlock').length == 0) {
                    $helpBlock.prepend('<div id="closeHelpBlock" class="closeBlock"></div>');
                    jq('#closeHelpBlock').click(function() {
                        $helpBlock.hide();
                    });
                }
            }

            jq('#' + options.BlockHelperID + ' ' + '#cornerHelpBlock').remove();

            if (options.fixWinSize && (elemPosLeft + addLeft + $helpBlock.outerWidth()) > (leftPadding + scrWidth)) {
                elemPosLeft = Math.max(0, leftPadding + scrWidth - $helpBlock.outerWidth()) - addLeft;
            }

            if ((elemPosTop + addTop < 0) || ((options.fixWinSize) && (elemPosTop > topPadding) &&
               ((elemPos.top + $helpBlock.outerHeight() + jq(this).outerHeight()) > (topPadding + scrHeight)))) {

                if ((elemPosLeft + addLeft + $helpBlock.outerWidth()) > jq(document).width()) {
                    elemPosLeft = elemPosLeft - addLeft - $helpBlock.outerWidth() + 40; // 40 for correct display of the direction corner
                    $helpBlock.prepend('<div id="cornerHelpBlock" class="pos_bottom_left"></div>');
                } else {
                    $helpBlock.prepend('<div id="cornerHelpBlock" class="pos_bottom"></div>');
                }
                elemPosTop = elemPos.top + jq(this).outerHeight();
                addTop = -addTop;

            } else {

                if ((elemPosLeft + addLeft + $helpBlock.outerWidth()) > jq(document).width()) {
                    elemPosLeft = elemPosLeft - addLeft - $helpBlock.outerWidth() + 40; // 40 for correct display of the direction corner
                    $helpBlock.append('<div id="cornerHelpBlock" class="pos_top_left"></div>');
                } else {
                    $helpBlock.append('<div id="cornerHelpBlock" class="pos_top"></div>');
                }
            }


            if (options.enableAutoHide) {

                jq(document).click(function(e) {
                    if (!jq(e.target).parents().andSelf().is(elem)) {
                        $helpBlock.hide();
                    }
                });
                //                elem.click(function(e) {
                //                    e.stopPropagation();
                //                });
            }

            $helpBlock.css(
          {
              "top": elemPosTop + addTop,
              "left": elemPosLeft + addLeft,
              "position": options.position
          });

            if ($helpBlock.css('display') == "none") {
                $helpBlock.show();
            } else {
                $helpBlock.hide();
            }
        });
    };

})(jQuery, window, document, document.body);




  