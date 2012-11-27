
(function ($) {
  window.photoDetails = (function () {
    var
      originalTitle = '',
      $imgHelper = null
      $helperContainer = null;

    var init = function () {
      originalTitle = ' ' + document.title;
      $imgHelper = $(document.createElement('div')).css({width : '1px', height : '1px', overflow : 'hidden', position : 'absolute', left : '-1px', top : '-1px', zIndex : '-666'}).appendTo(document.body);
      $helperContainer = $(document.createElement('div')).css({background : '#FFF', height : '1px', lineHeight : '1px', overflow : 'hidden'}).insertBefore('#photo_image_container');

      var
        maxWidth = $helperContainer.width() - 20;

        $('#photo_image_container').css({visibility : 'visible'}).find('div.container:first').width(maxWidth).height(maxWidth);
    };

    var editImage = function () {
      var
        link = $('#editImage').attr('href');

      if (link) {
        document.location.href = link;
      }
    };

    var removeImage = function (param) {
      if (typeof param === 'undefined') {
        if (confirm(ASC.Photos.Resources.confirmRemoveMessage)) {
          var imgId = $('#hiddenContainer input').slice(0, 1).val();
          if (imgId) {
            $('#photo_image_container').find('div.container:first').addClass('loading');
            PhotoDetails.DeleteImage(imgId, arguments.callee);
          }
        }
      } else if (typeof param === 'object' && param.hasOwnProperty('value')) {
        param = param.value;
        var
          imgId = param.rs1,
          newPos = param.rs2,
          link = param.rs3;

        if (newPos >= 0) {
          var
            $currentImg = $('#thumbnail-' + imgId),
            $toShow = $currentImg.nextAll('a:hidden:first');

          if ($toShow.length === 0) {
            $toShow = $currentImg.prevAll('a:hidden:first');
          }
          $currentImg.remove();
          $toShow.show();
          History.move(link);
        } else {
          document.location.href = link;
        }
      }
    };

    var rotateImage = function (param) {
      if (typeof param === 'boolean') {
        var imgId = $('#hiddenContainer input').slice(0, 1).val();
        if (imgId) {
          $('#photo_image_container').find('div.container:first').addClass('loading');
          PhotoDetails.ImageRotate(imgId, param, arguments.callee);
        }
      } else if (typeof param === 'object' && param.hasOwnProperty('value')) {
        param = param.value;

        var
          imgId = param.rs1,
          imgName = param.rs2,
          imgSrc = param.rs3,
          thumbSrc = param.rs4,
          imgWidth = param.rs5,
          imgHeight = param.rs6;

        $('#photo_image_container').find('div.container:first').addClass('loading');

        $('#thumbnail-' + imgId).find('img:first').attr({'src' : thumbSrc, 'alt' : imgName});

        $imgHelper.empty();
        var $newImage = $(document.createElement('img')).appendTo($imgHelper);
        $newImage.attr({'src' : imgSrc, 'alt' : imgName, 'truewidth' : imgWidth, 'trueheight' : imgHeight}).load(photoDetails.resizeImage);
      }
    };

    var resizeImage = function (evt) {
      var
        $img = $imgHelper.find('img'),
        imgSrc = $img.attr('src'),
        imgName = $img.attr('alt'),
        imgWidth = $img.attr('truewidth'),
        imgHeight = $img.attr('trueheight');

      imgWidth = isFinite(+imgWidth) ? +imgWidth : $img.width();
      imgHeight = isFinite(+imgHeight) ? +imgHeight : $img.height();

      if (imgWidth > imgHeight) {
        var
          maxWidth = $helperContainer.width() - 10 * 2; 

        if (maxWidth < imgWidth) {
          var mn = maxWidth / imgWidth;
          imgHeight = Math.floor(imgHeight * mn);
          imgWidth = Math.floor(imgWidth * mn);
        }
      } else {
        var
          maxHeight = $helperContainer.width() - 10 * 2;

        if (maxHeight < imgHeight) {
          var mn = maxHeight / imgHeight;
          imgWidth = Math.floor(imgWidth * mn);
          imgHeight = Math.floor(imgHeight * mn);
        }
      }

      $(document.createElement('img'))
        .appendTo($('#hrefContainer').empty())
        .attr({'src' : imgSrc, 'alt' : imgName})
        .width(imgWidth).height(imgHeight)
        .css({marginLeft : Math.round(-imgWidth / 2) + 'px', marginTop : Math.round(-imgHeight / 2) + 'px'});

      $('#photo_image_container').find('div.container:first').removeClass('loading');
    };

    var updateImage = function (param) {
      if (!param || typeof param !== 'object' || !param.hasOwnProperty('value') || !param.value) {
        return undefined;
      }

      param = param.value;

      var
        status = param.status,
        imgId = param.rs1,
        imgName = param.rs2,
        imgSrc = param.rs3,
        imgDescription = param.rs4,
        imgWidth = param.rs5,
        imgHeight = param.rs6,
        views = param.rs7,
        comments = param.rs8,
        editLink = param.rs9,
        ssLink = param.rs10,
        prevImageId = param.rs11,
        nextImageId = param.rs12,
        commentsHtml = param.rs13;

      if (status.toLowerCase() === 'error') {
        $('#prevImage').removeClass('active');
        $('#nextImage').removeClass('active');
        $('#subMenu').css({visibility : 'hidden'});
        $('#commentsContainer').removeClass('loading');
        $('#photo_image_container').find('div.container:first').removeClass('loading');
        $('span.containerBreadCrumbLink:first').html(imgName).parent().next().html(imgName);
        $('#hrefContainer').attr({'href' : imgSrc}).empty();
        $(document.createElement('img')).attr({'src' : imgSrc, 'alt' : imgName}).appendTo('#hrefContainer');
        return undefined;
      }

      $('#hrefContainer').attr({'href' : imgSrc});

      $(document.createElement('img'))
        .appendTo($imgHelper.empty())
        .load(photoDetails.resizeImage)
        .attr({'src' : imgSrc, 'alt' : imgName, 'truewidth' : imgWidth, 'trueheight' : imgHeight});

      document.title = imgName + originalTitle;

      $('#hdnObjectID').val(imgId);

      $('#hiddenContainer input').slice(1, 2).val(imgName);

      $('span.containerBreadCrumbLink:first').html(imgName).parent().next().html(imgName);

      $('#editImage').attr({'href' : editLink});
      $('#startSlideShow').attr({'href' : ssLink});
      $('#viewsCount').html(views);
      $('#commentsCount').html(comments);
      $('#subMenu').css({visibility : 'visible'});

      if (prevImageId) {
        $('#prevImage').addClass('active').attr({'href' : '#' + prevImageId});
      } else {
        $('#prevImage').removeClass('active').attr({'href' : '#'});
      }
      if (nextImageId) {
        $('#nextImage').addClass('active').attr({'href' : '#' + nextImageId});
      } else {
        $('#nextImage').removeClass('active').attr({'href' : '#'});
      }

      $('#commentsContainer').removeClass('loading');
      var commentsContainer = document.getElementById('commentsContainer');
      if (commentsContainer && typeof commentsContainer === 'object') {
        commentsContainer.innerHTML = commentsHtml;
        if (typeof InitEditor === 'function') {
          InitEditor();
        }
      }
    };

    var prevImage = function () {
      if (!$('#prevImage').hasClass('active')) {
        return undefined;
      }
      var link = $('#prevImage').attr('href');
      if (link.length === 0) {
        return undefined;
      }
      link = link.charAt(0) === '#' ? link.substring(1) : link;
      if (link) {
        History.move(link);
      }
    };

    var nextImage = function () {
      if (!$('#nextImage').hasClass('active')) {
        return undefined;
      }
      var link = $('#nextImage').attr('href');
      if (link.length === 0) {
        return undefined;
      }
      link = link.charAt(0) === '#' ? link.substring(1) : link;
      if (link) {
        History.move(link);
      }
    };

    var shiftToLeft = function () {
      if ($('#prevThumb').hasClass('active')) {
        var
          trueSrc = '',
          $img = null,
          $toHide = $('#photoThumbnails').children('a:visible:last'),
          $toShow = $toHide.prevAll('a:hidden:first');

        if ($toHide.length > 0 && $toShow.length > 0) {
          $toHide.hide();
          $img = $toShow.show().find('img:first');
          trueSrc = $img.attr('truesrc');
          if (trueSrc) {
            $img.attr({'src' : trueSrc, 'truesrc' : ''});
          }
          $('#nextThumb').addClass('active');
        }
        if ($('#photoThumbnails').children('a:first').is(':visible')) {
          $('#prevThumb').removeClass('active');
        }
      }
    };

    var shiftToRight = function () {
      if ($('#nextThumb').hasClass('active')) {
        var
          trueSrc = '',
          $img = null,
          $toHide = $('#photoThumbnails').children('a:visible:first'),
          $toShow = $toHide.nextAll('a:hidden:first');

        if ($toHide.length !== 0 && $toShow.length !== 0) {
          $toHide.hide();
          $img = $toShow.show().find('img:first');
          trueSrc = $img.attr('truesrc');
          if (trueSrc) {
            $img.attr({'src' : trueSrc, 'truesrc' : ''});
          }
          $('#prevThumb').addClass('active');
        }
        if ($('#photoThumbnails').children('a:last').is(':visible')) {
          $('#nextThumb').removeClass('active');
        }
      }
    };

    var changeImage = function (imgId) {
      imgId = imgId.toString();
      if (imgId.length === 0) {
        return undefined;
      }
      var pos = -1;
      if ((pos = imgId.indexOf('#')) === -1) {
        return undefined;
      }
      imgId = imgId.substring(pos + 1); // '#'.length
      imgId = imgId.charAt(0) === '#' ? imgId.substring(1) : imgId;
      if (imgId) {
        History.move(imgId);
      }
    };

    var onUpdateAnchor = function (anchor) {
      if (typeof anchor !== 'string' || anchor.length === 0) {
        anchor = $('#hiddenContainer input').slice(3, 4).val();
      }

      if (typeof anchor !== 'string' || anchor.length === 0) {
        return undefined;
      }

      var imgId = anchor.charAt(0) === '#' ? anchor.substring(1) : anchor;
      if (imgId.length === 0) {
        return undefined;
      }

      $('#commentsContainer').html('').addClass('loading');

      $('#prevThumb').removeClass('active');
      $('#nextThumb').removeClass('active');
      $('#photoThumbnails').show().find('a').removeClass('selected').hide();
      var
        trueSrc = '',
        maxVisibleImg = 8,  
        $thumbsBar = $('#thumbsBar'),
        $currentImg = $('#thumbnail-' + imgId).addClass('selected').show(),
        $img = null,
        $leftImage = null,
        $rightImage = null;

      if ($currentImg.length === 0) {
        $currentImg = $('#photoThumbnails').find('a.thumb:first').show();
      }
      if ($currentImg.length !== 0) {
        $img = $currentImg.find('img:first');
        trueSrc = $img.attr('truesrc');
        if (trueSrc) {
          $img.attr({src : trueSrc, 'truesrc' : ''});
        }
        $('#prevThumb').addClass('active');
        $('#nextThumb').addClass('active');
        $leftImage = $currentImg;
        $rightImage = $currentImg;
        var correctHeight = $thumbsBar.height();
        while (maxVisibleImg > 0 && ($leftImage !== null || $rightImage !== null)) {
          if ($leftImage !== null) {
            $leftImage = $leftImage.prev();
            if ($leftImage.length === 0) {
              $leftImage = null;
            }
          }
          if ($rightImage !== null) {
            $rightImage = $rightImage.next();
            if ($rightImage.length === 0) {
              $rightImage = null;
            }
          }
          if ($leftImage !== null) {
            $img = $leftImage.show().find('img:first');
            trueSrc = $img.attr('truesrc');
            if (trueSrc) {
              $img.attr({src : trueSrc, 'truesrc' : ''});
            }
            maxVisibleImg--;
          }
          if ($rightImage !== null) {
            $img = $rightImage.show().find('img:first');
            trueSrc = $img.attr('truesrc');
            if (trueSrc) {
              $img.attr({src : trueSrc, 'truesrc' : ''});
            }
            maxVisibleImg--;
          }
        }
        if ($leftImage === null || $leftImage.prev().length === 0) {
          $('#prevThumb').removeClass('active');
        }
        if ($rightImage === null || $rightImage.next().length === 0) {
          $('#nextThumb').removeClass('active');
        }
      }

      $('#photo_image_container').find('div.container:first').addClass('loading');
      $('#hiddenContainer input').slice(0, 1).val(imgId);
      AjaxPro.onLoading = function () {};
      PhotoDetails.GetImage(imgId, photoDetails.updateImage);
    };

    return {
      init        : init,
      editImage   : editImage,
      removeImage : removeImage,
      rotateImage : rotateImage,
      prevImage   : prevImage,
      nextImage   : nextImage,
      resizeImage : resizeImage,
      updateImage : updateImage,

      shiftToLeft   : shiftToLeft,
      shiftToRight  : shiftToRight,
      changeImage   : changeImage,

      onUpdateAnchor  : onUpdateAnchor
    };
  })();

  History.bind('onupdate', photoDetails.onUpdateAnchor);

  $(document).ready(function () {
    History.init();
    photoDetails.init();
    var hash = document.location.hash;
    if (hash === '' || hash === '#') {
      photoDetails.onUpdateAnchor('');
    }

    $('#thumbsBar')
      .bind('selectstart', function () {
        return false;
      })
      .mousedown(function () {
        return false;
      });
  });
})(jQuery);
