
(function ($) {
  window.slideShowManager = (function () {
    var
      $cachedContainer = null,
      timerInterval = 3000,
      showedTimer = null,
      cachedIndexes = {},
      imageId = -1,
      albumId = -1,
      maxImgHeight = -1,
      lastAction = '',
      isRunning = false;

    var init = function (image, album, param) {
      $helperContainer = $(document.createElement('div')).css({background : '#000', height : '1px', lineHeight : '1px', overflow : 'hidden', display : 'none'}).appendTo(document.body);
      $cachedContainer = $(document.createElement('div')).css({width : '100%', height : '100%', position : 'absolute', left : '0', top : '0', zIndex : '-1', visibility : 'hidden', overflow : 'hidden'}).appendTo(document.body);
      maxImgHeight = (param && typeof param === 'object' && param.maxImgHeight) ? param.maxImgHeight : -1;
      imageId = image;
      albumId = album;

      isRunning = true;
      lastAction = 'StartSlideShow';
      $('#runSlideShow').removeClass('stoped');
      getImage(imageId);
    };

    var getPrevImage = function () {
      lastAction = 'GetPrevImage';
      getImage(--imageId);
    };

    var getNextImage = function () {
      lastAction = 'GetNextImage';
      getImage(++imageId);
    };

    var toggle = function () {
      (isRunning) ? stop() : start();
    };

    var start = function () { 
      isRunning = true;
      lastAction = 'StartSlideShow';
      $('#runSlideShow').removeClass('stoped');
      showedTimer = setTimeout(getNextImage, timerInterval);
    };

    var stop = function () { 
      isRunning = false;
      clearTimeout(showedTimer);
      lastAction = 'StopSlideShow';
      $('#runSlideShow').addClass('stoped');
    };

    var prevImage = function () {
      stop();
      getPrevImage();
    };

    var nextImage = function () {
      stop();
      getNextImage();
    };

    var updateImage = function (image, params) {
      if (!image || typeof image !== 'object') {
        switch (lastAction.toLowerCase()) {
          case 'getprevimage' :
            getPrevImage();
            break;
          case 'getnextimage' :
            getNextImage();
            break;
        }
        return undefined;
      }

      if (!isRunning && lastAction.toLowerCase() === 'stopslideshow') {
        return undefined;
      }

      var
        $image = $(image),
        width = $image.width(),
        height = $image.height(),
        maxWidth = $helperContainer.css({display : 'block'}).width();
        $helperContainer.css({display : 'none'});
      if (maxWidth < width) {
        var mn = maxWidth / width;
        height = Math.floor(height * mn);
        width = Math.floor(width * mn);
      }

      $('#imageContainer').width(width).height(height).css({opacity : '0'}).attr({src : image.src, alt : image.alt}).animate({opacity: 1}, 500);
      $('#photoName').html(params.alt);

      if (isRunning) {
        showedTimer = setTimeout(getNextImage, timerInterval);
      }
    };

    var loadImage = function (src, alt, handler) {
      if (typeof src !== 'string' || src.length === 0 || typeof alt !== 'string' || typeof handler !== 'function') {
        return undefined;
      }

      var loadedTimer = setTimeout((function (handler) { return function () { handler.apply(null, [null]); }; })(handler), timerInterval * 10); 

      $i = $(document.createElement('img'))
        .load((function (handler, loadedTimer, params) {
          return function () {
            clearTimeout(loadedTimer);
            handler.apply(null, [this, params]);
          };
        })(handler, loadedTimer, {alt : alt, src : src}))
        .appendTo($cachedContainer.empty())
        .attr('src', src);
    };

    var getImage = function (param) {
      if (typeof param === 'undefined') {
        return undefined;
      }
      if (typeof param === 'number') {
        if (typeof cachedIndexes !== 'object') {
          cachedIndexes = {};
        }
        if (cachedIndexes.hasOwnProperty(param)) {
          imageId = param;
          loadImage(cachedIndexes[param].src, cachedIndexes[param].alt, updateImage);
          return undefined;
        }
        SlideShow.GetImage(albumId, param, slideShowManager.getImage);
      } else if (typeof param === 'object' && param.hasOwnProperty('value')) {
        param = param.value;
        var index = param.rs1;
        if (typeof cachedIndexes !== 'object') {
          cachedIndexes = {};
        }
        cachedIndexes[index] = {
          src : param.rs2,
          alt : param.rs3
        };
        imageId = index;
        loadImage(cachedIndexes[index].src, cachedIndexes[index].alt, updateImage);
      }
    };

    var close = function () {
      window.close();
    };

    return {
      init      : init,
      getImage  : getImage,
      prevImage : prevImage,
      nextImage : nextImage,
      start     : start,
      stop      : stop,
      toggle    : toggle,
      close     : close
    };
  })();

  $(document).ready(function () {
    slideShowManager.init(0, $('#imgAlbumId').val(), {maxImgHeight : $('#imgMaxHeight').val()});

    $(document).keyup(function (evt) {
      evt.keyCode = evt.keyCode || evt.charCode;
      switch (evt.keyCode) {
        case 32:
          slideShowManager.toggle();
          break;
        case 33:
        case 37:
        case 105:
          slideShowManager.prevImage();
          break;
        case 34:
        case 39:
        case 99:
          slideShowManager.nextImage();
          break;
      }
    });
  });
})(jQuery);
