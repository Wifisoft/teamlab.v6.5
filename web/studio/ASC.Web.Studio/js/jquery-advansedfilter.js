
(function ($, win, doc, body) {
  var
    defaultAnykeyTimeout = 500,
    currentHash = '',
    cmplClassName = 'advansed-filter-complete',
    localStorage = window.localStorage || {},
    key = location.protocol + '//' + location.hostname + (location.port ? ':' + location.port : '') + location.pathname + location.search,
    templates = {
      filterContainer : 'template-filter-container',
      filterItem      : 'template-filter-item'
    },
    isFirstUpdateLocaleStorage = true,
    lazyTrigger = false,
    filterContainer = null,
    filterInputKeyupHandler = 0,
    filterInputKeyupObject = null,
    filterInputKeyupTimeout = 0,
    Resources = {},
    filterValues = [
      {type : 'sorter', id : 'sorter',  hashmask : 'sorter/{0}'},
      {type : 'text',   id : 'text',    hashmask : 'text/{0}'}
    ],
    sorterValues = [];

  key = encodeURIComponent(key.charAt(key.length - 1) === '/' ? key + 'default.aspx' : key);

  function isArray (o) {
    return o ? o.constructor.toString().indexOf("Array") != -1 : false;
  }

  function converText (str, toText) {
    str = typeof str === 'string' ? str : '';
    if (!str) {
      return '';
    }

    if (toText === true) {
      var
        symbols = [
          ['&lt;',  '<'],
          ['&gt;',  '>'],
          ['&and;', '\\^'],
          ['&sim;', '~'],
          ['&amp;', '&']
        ];

      var symInd = symbols.length;
      while (symInd--) {
        str = str.replace(new RegExp(symbols[symInd][1], 'g'), symbols[symInd][0]);
      }
      return str;
    }

    var o = document.createElement('textarea');
    o.innerHTML = str;
    return o.value;
  }

  function format () {
    if (arguments.length === 0) {
      return '';
    }

    var pos = -1, str = arguments[0] || '', cnd = '', ind = -1, cnds = str.match(/{(\d+)}/g), cndsInd = cnds ? cnds.length : 0;
    while (cndsInd--) {
      pos = -1;
      cnd = cnds[cndsInd];
      ind = cnd.replace(/[{}]+/g, '');
      while ((pos = str.indexOf(cnd, pos + 1)) !== -1) {
        str = str.substring(0, pos) + (arguments[+ind + 1] || '') + str.substring(pos + cnd.length);
      }
    }
    return str;
  };

  function getFiltersHash (filtervalues) {
    var
      hash = [],
      filtervalue = null,
      filtervaluehash = '';

    for (var i = 0, n = filtervalues ? filtervalues.length : 0; i < n; i++) {
      filtervalue = filtervalues[i];
      switch (filtervalue.type) {
        case 'sorter' :
          filtervaluehash = {
            id : filtervalue.id,
            type : filtervalue.type,
            params : {
              id : filtervalue.params.id,
              def : filtervalue.params.def,
              dsc : filtervalue.params.dsc,
              sortOrder : filtervalue.params.sortOrder
            }
          };
          break;
        case 'daterange' :
          filtervaluehash = {
            id : filtervalue.id,
            type : filtervalue.type,
            params : {
              from : Teamlab.serializeTimestamp(new Date(filtervalue.params.from)),
              to : Teamlab.serializeTimestamp(new Date(filtervalue.params.to))
            }
          };
          break;
        default :
          filtervaluehash = {
            id : filtervalue.id,
            type : filtervalue.type,
            params : filtervalue.params
          };
          break;
      }

      filtervaluehash.params = $.base64.encode($.toJSON(filtervaluehash.params));
      hash.push($.toJSON(filtervaluehash));
    }

    return $.base64.encode(hash.join(';'));
  }

  function updateLocalStorageFilters (opts, filtervalues) {
    if (isFirstUpdateLocaleStorage === true && (filtervalues.length === 0 || (filtervalues.length === 1 && filtervalues[0].id === 'sorter'))) {
      isFirstUpdateLocaleStorage = false;
      return undefined;
    }

    opts = opts && typeof opts === 'object' ? opts : {};
    isFirstUpdateLocaleStorage = false;

    var newhash = getFiltersHash(filtervalues);

    if (opts.inhash === true) {
      try {ASC.Controls.AnchorController.safemove(newhash)} catch (err) {}
    }
    localStorage[key] = newhash;
    $.cookies.set(key, newhash, {path : location.pathname});
  }

  function getLocalStorageFilters (opts) {
    var values = null;

    if (localStorage[key]) {
      values = $.base64.decode(localStorage[key]);
    }
    if (opts && typeof opts === 'object' && opts.inhash === true) {
      currentHash = location.hash;
      currentHash = currentHash && typeof currentHash === 'string' && currentHash.charAt(0) === '#' ? currentHash.substring(1) : currentHash;
      if (currentHash.length === 0) {
        currentHash = $.cookies.get(key);
        currentHash = decodeURIComponent(currentHash);
        currentHash = currentHash && typeof currentHash === 'string' && currentHash.charAt(0) === '#' ? currentHash.substring(1) : currentHash;
      }
      values = $.base64.decode(currentHash);
      if (currentHash && currentHash !== 'null') {
        try {ASC.Controls.AnchorController.safemove(currentHash)} catch (err) {}
        $.cookies.set(key, currentHash, {path : location.pathname});
      }
    }

    var
      params = null,
      filtervalue = null,
      filtervalues = [];

    values = typeof values === 'string' && values.length > 0 ? values.split(';') : [];
    for (var i = 0, n = values.length; i < n; i++) {
      try {
        filtervalue = $.parseJSON(values[i]);
        filtervalue.params = $.parseJSON($.base64.decode(filtervalue.params));
      } catch (err) {
        filtervalue = null;
      }
      if (filtervalue) {
        switch (filtervalue.type) {
          case 'sorter' :
            filtervalues.push({id : filtervalue.id, selected : true, params : filtervalue.params});
            break;
          case 'daterange' :
            filtervalues.push({id : filtervalue.id, params : {from : Teamlab.serializeDate(filtervalue.params.from), to : Teamlab.serializeDate(filtervalue.params.to)}});
            break;
          default :
            filtervalues.push({id : filtervalue.id, params : filtervalue.params});
            break;
        }
      }
    }

    return filtervalues || [];
  }

  function getContainerHash ($container) {
    return getFiltersHash($container.data('filtervalues') || []);
  }

  function getContainerFilters ($container) {
    return $container.data('filtervalues') || [];
  }

  function getFilterValue (filtervalues, type) {
    var filtervaluesInd = filtervalues.length;
    while (filtervaluesInd--) {
      if (filtervalues[filtervaluesInd].type === type) {
        return filtervalues[filtervaluesInd];
      }
    }
    return null;
  }

  function extendItemValue (dst, src) {
    for (var srcInd in src) {
      if (src.hasOwnProperty(srcInd) && !dst.hasOwnProperty(srcInd)) {
        dst[srcInd] = src[srcInd];
      }
    }
  }

  function extendItemValues (dst, src) {
    var
      rslt = [],
      dstInd = 0, srcInd = 0;

    dstInd = dst.length;
    while (dstInd--) {
      srcInd = src.length;
      while (srcInd--) {
        if (dst[dstInd] && src[srcInd] && dst[dstInd].id === src[srcInd].id) {
          extendItemValue(dst[dstInd], src[srcInd]);
          break;
        }
      }
      rslt.unshift(dst[dstInd]);
    }

    srcInd = src.length;
    while (srcInd--) {
      dstInd = dst.length;
      while (dstInd--) {
        if (src[srcInd].id === dst[dstInd].id) {
          break;
        }
      }
      rslt.unshift(src[srcInd]);
    }

    return rslt;
  }

  function getGroupSelectorName (id) {
    var
      groups = groupSelector ? groupSelector.Groups || [] : [],
      groupsInd = 0;
    groupsInd = groups ? groups.length : 0;
    while (groupsInd--) {
      if (groups[groupsInd].Id == id) {
        return groups[groupsInd].Name;
      }
    }
    return id;
  }

  function getUserSelectorName (id, name) {
    var
      users = null,
      usersInd = 0,
      groups = userSelector ? userSelector.Groups || [] : [],
      groupsInd = 0;

    groupsInd = groups ? groups.length : 0;

    while (groupsInd--) {
      users = groups[groupsInd].Users;
      usersInd = users ? users.length : 0;
      while (usersInd--) {
        if (users[usersInd].ID == id) {
          return users[usersInd].Name;
        }
      }
    }

    users = userSelector ? userSelector.LockedUsers || [] : [];
    usersInd = users ? users.length : 0;
    while (usersInd--) {
        if (users[usersInd].ID == id) {
            return users[usersInd].Name;
        }
    }

    return typeof name === 'string' && name ? name : id;
  }

  function addFilterToGroup (groups, filtervalue) {
    if (!filtervalue.title) {
      return undefined;
    }

    var groupsInd = 0;
    groupsInd = groups ? groups.length : 0;
    while (groupsInd--) {
      if (groups[groupsInd].title === filtervalue.group) {
        groups[groupsInd].items.push(filtervalue);
        break;
      }
    }
    if (groupsInd === -1) {
      groups.push({title : filtervalue.group || '', items : [filtervalue]});
    }
  }

  function createDatepicker ($o, $container, $filteritem, filtervalue) {
    return $o
      .datepicker({
        onSelect    : (function ($container, $filteritem, filtervalue, callback) {
          return function (datetext, inst) {
            callback(this, $container, $filteritem, filtervalue, datetext, inst);
          }
        })($container, $filteritem, filtervalue, onUserFilterDateSelectValue)
      })
      .click(function (evt) {
        evt.stopPropagation();
      });
  }

  function createComboboxOptions (options) {
    var option = null, html = [];

    for (var i = 0, n = options.length; i < n; i++) {
      option = options[i];
      html = html.concat(['<option', ' class="' + option.classname + '"' + ' value="' + option.value + '"', option.def === true ? ' selected="selected"' : '', '>', converText(option.title, true), '</option>']);
    }

    return html.join('');
  }

  function createFilterItem (filtervalue) {
    var
      html = [],
      o = null, $o = null;

    $o = jQuery('#' + templates.filterItem).tmpl(filtervalue);
    if ($o && $o.length > 0) {
      return $o;
    }

    html = [
      '<span class="title">' + converText(filtervalue.filtertitle || filtervalue.title, true) + '</span>',
      '<span class="selector-wrapper">',
        '<span class="daterange-selector from-daterange-selector">',
          '<span class="label">' + (Resources.LblFrom || 'From') + '</span>',
          '<input class="textEditCalendar advansed-filter-dateselector-date dateselector-from-date" type="text" />',
        '</span>',
        '<span class="daterange-selector to-daterange-selector">',
          '<span class="label">' + (Resources.LblTo || 'To') + '</span>',
          '<input class="textEditCalendar advansed-filter-dateselector-date dateselector-to-date" type="text" />',
        '</span>',
        (filtervalue.options ? 
          [
            '<span class="combobox-selector">',
              '<select class="advansed-filter-combobox">',
                createComboboxOptions(filtervalue.options),
              '</select>',
            '</span>',
          ].join('') : 
          ''
        ),
        '<span class="group-selector">',
          //'<span class="custom-value"><span class="value"></span>&nbsp;<small>▼</small></span>',
          '<span class="custom-value"><span class="value"></span></span>',
          //'<span class="default-value"><span class="value">' + (Resources.LblSelect || 'Select') + '</span>&nbsp;<small>▼</small></span>',
          '<span class="default-value"><span class="value">' + (Resources.LblSelect || 'Select') + '</span></span>',
        '</span>',
        '<span class="person-selector">',
          //'<span class="custom-value"><span class="value"></span>&nbsp;<small>▼</small></span>',
          '<span class="custom-value"><span class="value"></span></span>',
          //'<span class="default-value"><span class="value">' + (Resources.LblSelect || 'Select') + '</span>&nbsp;<small>▼</small></span>',
          '<span class="default-value"><span class="value">' + (Resources.LblSelect || 'Select') + '</span></span>',
        '</span>',
      '</span>',
      '<span class="btn-delete">×</span>'
    ];

    o = doc.createElement('div');
    o.className = 'default-value';
    o.innerHTML = html.join('');
    return o;
  }

  function createAdvansedFilterGroup (opts, itemgroups, type) {
    var
      colcount = 1,
      html = [],
      items = null,
      itemvalues = null,
      title = '',
      itemgroupsInd = 0, itemgroupsLen = 0,
      itemvaluesInd = 0, itemvaluesLen = 0;

    colcount = opts && typeof opts === 'object' && opts.hasOwnProperty('colcount') && isFinite(+opts.colcount) ? +opts.colcount : colcount;
    html.push('<li class="item-list-top ' + type + '-list-top"></li>');
    for (itemgroupsInd = 0, itemgroupsLen = itemgroups.length; itemgroupsInd < itemgroupsLen; itemgroupsInd++) {
      if (type === 'filter' && colcount > 1) {
        if (itemgroupsInd < colcount) {
          html.push('<li class="item-group-col"><ul class="group-items">');
          for (var i = itemgroupsInd; i < itemgroupsLen; i += colcount) {
            itemvalues = itemgroups[i].items;
            items = [];
            for (itemvaluesInd = 0, itemvaluesLen = itemvalues.length; itemvaluesInd < itemvaluesLen; itemvaluesInd++) {
              items.push(createAdvansedFilterItem(itemvalues[itemvaluesInd], type));
            }
            title = converText(itemgroups[i].title, true);

            html = html.concat([
              '<li',
                ' class="item-group ' + type + '-group',
                  itemgroups[i].title ? '' : ' none-title',
                '"',
              '>',
                '<span class="title" title="' + title + '">',
                  title,
                '</span>',
                '<ul class="filter-items">',
                  items.join(''),
                '</ul>',
              '</li>'
            ]);
          }
          html.push('</ul><div class="clear"></div></li>');
        }
        //html = html.concat([
        //  '<li',
        //    ' class="item-group ' + type + '-group',
        //      //itemgroupsInd === 0 ? ' first-group' : '',
        //      itemgroups[itemgroupsInd].title ? '' : ' none-title',
        //    '"',
        //  '>',
        //    '<span class="title" title="' + title + '">',
        //      title,
        //    '</span>',
        //    '<ul class="filter-items">',
        //      items.join(''),
        //    '</ul>',
        //  '</li>'
        //]);
      } else {
        itemvalues = itemgroups[itemgroupsInd].items;
        items = [];
        for (itemvaluesInd = 0, itemvaluesLen = itemvalues.length; itemvaluesInd < itemvaluesLen; itemvaluesInd++) {
          items.push(createAdvansedFilterItem(itemvalues[itemvaluesInd], type));
        }

        title = converText(itemgroups[itemgroupsInd].title, true);
        html = html.concat([
          '<li',
            ' class="item-group ' + type + '-group',
              //itemgroupsInd === 0 ? ' first-group' : '',
              itemgroups[itemgroupsInd].title ? '' : ' none-title',
            '"',
          '>',
            '<span class="title" title="' + title + '">',
              title,
            '</span>',
          '</li>',
          items.join('')
          ]
        );
      }
      //if (itemgroupsInd > 0 && itemgroupsLen > colcount && itemgroupsInd !== (itemgroupsLen - 1) && (itemgroupsInd + 1) % colcount === 0) {
      //  html.push('<li class="item-list-separator ' + type + '-list-separator"></li>')
      //}
    }
    html.push('<li class="item-list-separator ' + type + '-list-separator"></li>');
    return html.join('');
  }

  function createAdvansedFilterItem (itemvalue, type) {
    return [
      '<li',
        ' class="item-item ' + type + '-item',
          itemvalue.def === true ? ' selected' : '',
          itemvalue.def === true ? (itemvalue.dsc = !!itemvalue.dsc) === true ? ' dsc-sort' : ' asc-sort' : '',
          itemvalue.dsc === true || itemvalue.sortOrder === 'descending' ? ' dsc-sort-default' : '',
        '"',
        ' title="' + converText(itemvalue.title, true) + '"',
        ' data-id="' + itemvalue.id + '"',
        itemvalue.type ? ' data-type="' + itemvalue.type + '"' : '',
        itemvalue.hashmask ? ' data-anchor="' + itemvalue.hashmask + '"' : '',
      '>',
        '<span class="inner-state"></span>',
        '<span class="inner-text">',
          converText(itemvalue.title, true),
          //itemvalue.title,
        '</span>',
      '</li>'
    ].join('');
  }

  function createAdvansedFilter (filtervalues, sortervalues) {
    var
      html = null,
      filters = [],
      sorters = [],
      o = null, $o = null;

    $o = jQuery('#' + templates.filterContainer).tmpl({filtervalues : filtervalues, sortervalues : sortervalues});
    if ($o && $o.length > 0) {
      return $o;
    }

    for (var i = 0, n = filtervalues.length; i < n; i++) {
      filters.push(createAdvansedFilterItem(filtervalues[i], 'filter'));
    }

    if (filters.length > 0) {
      filters = [
        '<div class="advansed-item-list advansed-filter-list">',
          '<ul class="item-list filter-list">',
            '<li class="item-list-top filter-list-top"></li>',
            filters.join(''),
          '</ul>',
        '</div>'
      ];
    }

    for (var i = 0, n = sortervalues.length; i < n; i++) {
      sorters.push(createAdvansedFilterItem(sortervalues[i], 'sorter'));
    }

    if (sorters.length > 0) {
      sorters = [
        '<div class="advansed-item-list advansed-sorter-list">',
          '<ul class="item-list sorter-list">',
            '<li class="item-list-top sorter-list-top"></li>',
            sorters.join(''),
          '</ul>',
        '</div>'
      ];
    }

    html = [
      '<div class="advansed-filter empty-filter-list">',
        '<label class="advansed-filter-state btn-start-filter"></label>',
        '<label class="advansed-filter-sort btn-show-sorters"></label>',
        sorters.join(''),
        filters.join(''),
        '<div class="advansed-filter-container">',
          '<div class="advansed-filter-button btn-show-filters"><div class="inner-text"><span class="text"><span>' + (Resources.LblFilter || 'Filter') + '</span></span></div></div>',
          '<div class="advansed-filter-filters empty-list">',
            '<div class="btn-show-hidden-filters">' + (Resources.BtnHiddenFilter || '...') + '</div>',
            '<div class="hidden-filters-container">',
              '<div class="control-top hidden-filters-container-top"></div>',
            '</div>',
          '</div>',
          '<div class="advansed-filter-input"><input class="advansed-filter advansed-filter-input advansed-filter-complete" type="text" /></div>',
        '</div>',
      '</div>'
    ];

    o = doc.createElement('div');
    o.className = 'clearFix';
    o.innerHTML = html.join('');
    return o;
  }

  function updateAdvansedFilter (opts, $container, filtervalues, sortervalues) {
    var
      o = null,
      wasAdded = false,
      groups = [],
      groupsInd = 0,
      filtervalues = [].concat(filtervalues),
      sortervalues = [].concat(sortervalues),
      filtervaluesInd = 0, filtervaluesLen = 0,
      filterid = '',
      $filter = null,
      $filters = $container.find('li.filter-item'),
      filtersInd = 0, filtersLen = 0;

    for (filtersInd = 0, filtersLen = $filters.length; filtersInd < filtersLen; filtersInd++) {
      $filter = $($filters[filtersInd]);
      filterid = $filter.attr('data-id');
      wasAdded = false;
      for (filtervaluesInd = 0, filtervaluesLen = filtervalues ? filtervalues.length : 0; filtervaluesInd < filtervaluesLen; filtervaluesInd++) {
        if (filtervalues[filtervaluesInd].id === filterid) {
          addFilterToGroup(groups, filtervalues[filtervaluesInd]);
          filtervalues.splice(filtervaluesInd, 1);
          wasAdded = true;
          break;
        }
      }
      if (wasAdded === false) {
        addFilterToGroup(groups, {type : $filter.attr('data-type'), title : $filter.text(), group : '', hashmask : $filter.attr('data-anchor')});
      }
    }

    for (filtervaluesInd = 0, filtervaluesLen = filtervalues ? filtervalues.length : 0; filtervaluesInd < filtervaluesLen; filtervaluesInd++) {
      addFilterToGroup(groups, filtervalues[filtervaluesInd]);
    }

    groupsInd = groups.length;
    while (groupsInd--) {
      if (groups[groupsInd].title === '') {
        groups.push(groups[groupsInd]);
        groups.splice(groupsInd, 1);
        break;
      }
    }

    o = $container.find('ul.filter-list:first').removeClass('multi-column').empty()[0] || null;
    if (o) {
      if (opts.colcount > 1) {
        o.className += ' multi-column multi-column-' + opts.colcount;
      }
      o.innerHTML = createAdvansedFilterGroup(opts, groups, 'filter');
    }

    o = $container.find('ul.sorter-list:first').removeClass('multi-column').empty()[0] || null;
    if (o) {
      o.innerHTML = createAdvansedFilterGroup(opts, [{title : '', items : sortervalues}], 'sorter');
    }

    return o;
  }

  function callSetFilterTrigger ($container, filtervalue, needResetAllEvent) {
    var
      filtervalue = filtervalue || null,
      opts = $container.data('filteroptions'),
      filtervalues = $container.data('filtervalues'),
      filtervaluesInd = filtervalues ? filtervalues.length : 0,
      selectedfilters = [];
    while (filtervaluesInd--) {
      if (filtervalues[filtervaluesInd].isset === true) {
        selectedfilters.unshift(filtervalues[filtervaluesInd]);
      }
      if (filtervalue === null && filtervalues[filtervaluesInd].id === 'sorter') {
        filtervalue = filtervalues[filtervaluesInd];
      }
    }

    if (selectedfilters.length > 1) {
      $container.addClass('has-filters');
    } else {
      $container.removeClass('has-filters');
    }

    updateLocalStorageFilters(opts, selectedfilters);
    if (needResetAllEvent === true) {
      $container.trigger('resetallfilters', [$container, filtervalue.id, selectedfilters]);
    }
    $container.trigger('setfilter', [$container, filtervalue, filtervalue.params, selectedfilters]);
    $container.trigger('updatefilter', [$container, filtervalue.id, selectedfilters]);
  }

  function callResetFilterTrigger ($container, filtervalue, needResetAllEvent) {
    var
      filtervalue = filtervalue || null,
      opts = $container.data('filteroptions'),
      filtervalues = $container.data('filtervalues'),
      filtervaluesInd = filtervalues ? filtervalues.length : 0,
      selectedfilters = [];
    while (filtervaluesInd--) {
      if (filtervalues[filtervaluesInd].isset === true) {
        selectedfilters.unshift(filtervalues[filtervaluesInd]);
      }
      if (filtervalue === null && filtervalues[filtervaluesInd].id === 'sorter') {
        filtervalue = filtervalues[filtervaluesInd];
      }
    }

    if (selectedfilters.length > 1) {
      $container.addClass('has-filters');
    } else {
      $container.removeClass('has-filters');
    }

    updateLocalStorageFilters(opts, selectedfilters);
    if (needResetAllEvent === true) {
      $container.trigger('resetallfilters', [$container, filtervalue.id, selectedfilters]);
    }
    $container.trigger('resetfilter', [$container, filtervalue, selectedfilters]);
    $container.trigger('updatefilter', [$container, filtervalue.id, selectedfilters]);
  }

  function setFilterItem ($container, $filteritem, filtervalue, params, nonetrigger) {
    onBodyClick(true);

    updateTextFilter($container, true);

    if ($container.length > 0 && filtervalue && filtervalue.id === 'sorter') {
      var $sorter = $container.find('li.sorter-item[data-id="' + params.id + '"]:first');
//      if (typeof params.dsc !== 'boolean') {
//        params.dsc = true;
//      } else {
//        params.dsc = !params.dsc;
//      }
//      if (!filtervalue.params || filtervalue.params.id !== params.id) {
//        params.dsc = true;
//      }

      resizeUserSorterContainer($container, params.title);

      var
        $sortercontainer = $container.find('div.advansed-filter-sort-container:first');

      var classname = params.dsc === true ? 'dsc-sort' : 'asc-sort';
      $sortercontainer
        .removeClass('asc-sort')
        .removeClass('dsc-sort')
        .addClass(classname);
      $sorter
        .addClass('selected')
        .removeClass('asc-sort')
        .removeClass('dsc-sort')
        .addClass(classname)
        .siblings().removeClass('selected').removeClass('asc-sort').removeClass('dsc-sort');
    }

    if ($container.length > 0 && filtervalue) {
      var paramsitem = null;
      var paramsid = $filteritem && $filteritem.length > 0 ? $filteritem.attr('data-paramsid') || null : null;
      if (!filtervalue.params) {
        filtervalue.params = filtervalue.multiselect === true ? [] : {};
      }

      if (filtervalue.multiselect === true && isArray(filtervalue.params) && paramsid) {
        var
          items = filtervalue.params,
          itemsInd = items ? items.length : 0;
        while (itemsInd--) {
          if (paramsid == items[itemsInd].__id) {
            paramsitem = items[itemsInd];
            break;
          }
        }
        if (!paramsitem) {
          paramsitem = {__id : paramsid};
          filtervalue.params.push(paramsitem);
        }
      } else {
        paramsitem = filtervalue.params;
      }

      for (var fld in params) {
        if (params.hasOwnProperty(fld)) {
          paramsitem[fld] = params[fld];
        }
      }

      filtervalue.isset = true;
      resizeUserFilterContainer($container, true);
      if (nonetrigger !== true && lazyTrigger === false) {
        callSetFilterTrigger($container, filtervalue);
      }
    }
  }

  function unsetFilterItem ($container, $filteritem, filtervalue, nonetrigger) {
    onBodyClick(true);

    updateTextFilter($container, true);

    if ($container.length > 0 && filtervalue) {
      var noParams = false;
      var paramsid = $filteritem && $filteritem.length > 0 ? $filteritem.attr('data-paramsid') || null : null;
      if (filtervalue.multiselect === true && isArray(filtervalue.params) && paramsid) {
        var
          items = filtervalue.params,
          itemsInd = items ? items.length : 0;
        while (itemsInd--) {
          if (paramsid == items[itemsInd].__id) {
            items.splice(itemsInd, 1);
            break;
          }
        }
        if (items.length === 0) {
          noParams = true;
        }
      } else {
        noParams = true;
      }
      if (noParams === true) {
        filtervalue.params = null;
        filtervalue.isset = false;
      }
      if (nonetrigger !== true && lazyTrigger === false) {
        callResetFilterTrigger($container, filtervalue);
      }
    }
  }

  function updateFiltersList ($items) {
    var
      hasitems = false,
      itemsInd = 0,
      $item = null,
      $group = null;

    if ($items.filter('.item-group').length === 0) {
      return undefined;
    }

    itemsInd = $items.length;
    while (itemsInd--) {
      $item = $($items[itemsInd]);
      if ($item.hasClass('item-group')) {
        $item.removeClass('hidden-item');
        if (hasitems === false) {
          $item.addClass('hidden-item');
        }
        hasitems = false;
        continue;
      }
      if ($item.hasClass('hidden-item')) {
        continue;
      }
      hasitems = true;
    }

    //$items.filter('.item-group').removeClass('first-group').not('.disabled-item').filter(':first').addClass('first-group');
  }

  function updateTextFilter ($container, nonetrigger) {
    var
      $this = $container.find('input.advansed-filter-input:first'),
      value = $this.val(),
      filtervalue = null,
      filtervalues = $container.data('filtervalues'),
      filtervaluesInd = 0;

    if (!filtervalues || filtervalues.length === 0) {
      return undefined;
    }

    filtervaluesInd = filtervalues.length;
    while (filtervaluesInd--) {
      if (filtervalues[filtervaluesInd].id === 'text') {
        break;
      }
    }

    if (filtervaluesInd !== -1) {
      filtervalue = filtervalues[filtervaluesInd];
      if (typeof value === 'string' && value.length > 0) {
        var params = {value : value};
        if (!filtervalue.params) {
          filtervalue.params = {};
        }
        for (var fld in params) {
          if (params.hasOwnProperty(fld)) {
            filtervalue.params[fld] = params[fld];
          }
        }
        filtervalue.isset = true;
      } else {
        filtervalue.params = null;
        filtervalue.isset = false;
      }
    }
  }

  /* <flag> */

  function onUserFilterFlagSelectValue ($container, $filteritem, filtervalue, nonetrigger) {
    if ($container.length > 0 && filtervalue) {
      setFilterItem($container, $filteritem, filtervalue, {}, nonetrigger);
    }
  }

  function compareUserFilterParamsFlag ($container, containerfiltervalue, filtervalue) {
    return true;
  }

  function customizeUserFilterFlag ($container, $filteritem, filtervalue) {
    return filtervalue.hasOwnProperty('defaultparams') ? filtervalue.defaultparams : {};
  }

  function destroyUserFilterFlag ($container, $filteritem, filtervalue) {
    
  }

  /* </flag> */
  /* <group> */

  function setUserFilterGroupValue ($container, $filteritem, params) {
    if (params && params.hasOwnProperty('name')) {
      $filteritem.removeClass('default-value').find('span.group-selector:first span.custom-value:first').attr('title', params.name).find('span.value:first').text(params.name);
    }
  }

  function onUserFilterGroupSelectValue ($container, $filteritem, filtervalue, group, nonetrigger) {
    if ($container.length > 0 && $filteritem.length > 0) {
      setUserFilterGroupValue($container, $filteritem, {id : group.Id, name : converText(group.Name)});
    }

    if ($container.length > 0 && filtervalue) {
      setFilterItem($container, $filteritem, filtervalue, {id : group.Id, value : group.Id, name : converText(group.Name)}, nonetrigger);
    }
  }

  function showUserFilterGroup ($container, $filteritem) {
    if ($container.hasClass('showed-groupselector')) {
      return undefined;
    }

    onBodyClick();
    jQuery(document.body).unbind('click', onBodyClick);
    
    $container.addClass('showed-groupselector').find('div.advansed-filter-groupselector-container:first').show();
    resizeControlContainer($container, $filteritem, $container.find('div.advansed-filter-groupselector-container:first'));

    setTimeout(function () {
      jQuery(document.body).one('click', onBodyClick);
    }, 1);
  }

  function destroyUserFilterGroup ($container, $filteritem, filtervalue) {
    
  }

  function compareUserFilterParamsGroup ($container, containerfiltervalue, filtervalue) {
    return containerfiltervalue.id === filtervalue.id;
  }

  function customizeUserFilterGroup ($container, $filteritem, filtervalue) {
    if (groupSelector && groupSelector._____init !== true) {
      groupSelector._____init = true;
      try {groupSelector.Open()} catch (err) {}
    }
    groupSelector.AdditionalFunction = (function ($container, $filteritem, filtervalue, callback) {
      return function (group) {
        callback($container, $filteritem, filtervalue, group);
      }
    })($container, $filteritem, filtervalue, onUserFilterGroupSelectValue);
  }

  /* </group> */
  /* <person> */

  function setUserFilterPersonValue ($container, $filteritem, params) {
    if (params && params.hasOwnProperty('name')) {
      $filteritem.removeClass('default-value').find('span.person-selector:first span.custom-value:first').attr('title', params.name).find('span.value:first').text(params.name);
    }
  }

  function onUserFilterPersonSelectValue ($container, $filteritem, filtervalue, userid, username, nonetrigger) {
    if ($container.length > 0 && $filteritem.length > 0) {
      setUserFilterPersonValue($container, $filteritem, {id : userid, name : converText(username)});
    }

    if ($container.length > 0 && filtervalue) {
      setFilterItem($container, $filteritem, filtervalue, {id : userid, value : userid, name : converText(username)}, nonetrigger);
    }
  }

  function compareUserFilterParamsPerson ($container, containerfiltervalue, filtervalue) {
    return containerfiltervalue.id === filtervalue.id;
  }

  function customizeUserFilterPerson ($container, $filteritem, filtervalue) {
    if (userSelector && userSelector._____init !== true) {
      userSelector._____init = true;
      try {userSelector.RenderItems()} catch (err) {}
    }
    userSelector.AdditionalFunction = (function ($container, $filteritem, filtervalue, callback) {
      return function (userid, username) {
        callback($container, $filteritem, filtervalue, userid, username);
      }
    })($container, $filteritem, filtervalue, onUserFilterPersonSelectValue);
  }

  function showUserFilterPerson ($container, $filteritem) {
    if ($container.hasClass('showed-userselector')) {
      return undefined;
    }

    if (userSelector) {
      userSelector.ClearFilter();
    }

    onBodyClick();
    jQuery(document.body).unbind('click', onBodyClick);

    $container.addClass('showed-userselector').find('div.advansed-filter-userselector-container:first').show();
    resizeControlContainer($container, $filteritem, $container.find('div.advansed-filter-userselector-container:first'));

    setTimeout(function () {
      jQuery(document.body).one('click', onBodyClick);
    }, 1);
  }

  function destroyUserFilterPerson ($container, $filteritem, filtervalue) {
    
  }

  /* </person> */
  /* <date> */

  function onUserFilterDateSelectValue (target, $container, $filteritem, filtervalue, datetext, inst, nonetrigger) {
    var
      $target = jQuery(target),
      date = inst.input.datepicker('getDate'),
      $dateselector = $target.parents('span.advansed-filter-dateselector-date:first');

    if ($container.length > 0 && filtervalue) {
      $dateselector.find('span.btn-show-datepicker-title:first').text(datetext);
      if (date) {
        setFilterItem($container, $filteritem, filtervalue, $dateselector.hasClass('dateselector-from-date') ? {from : date.getTime()} : {to : date.getTime()}, nonetrigger);
      }
    }
  }

  function compareUserFilterParamsDate ($container, containerfiltervalue, filtervalue) {
    return containerfiltervalue.from == filtervalue.from && containerfiltervalue.to == filtervalue.to;
  }

  function customizeUserFilterDate ($container, $filteritem, filtervalue) {
    var
      $datepicker = null,
      tmpDate = new Date(),
      defaultFromDate,
      defaultToDate;

    defaultFromDate = new Date(tmpDate.getFullYear(), tmpDate.getMonth() - 6, tmpDate.getDate(), 0, 0, 0, 0);
    defaultToDate = new Date(tmpDate.getFullYear(), tmpDate.getMonth(), tmpDate.getDate(), 0, 0, 0, 0);

    $datepicker = createDatepicker($filteritem.find('span.from-daterange-selector:first span.datepicker-container:first'), $container, $filteritem, filtervalue);
    $datepicker.datepicker('setDate', defaultFromDate);
    $filteritem.find('span.from-daterange-selector:first span.btn-show-datepicker-title:first').text($datepicker.datepicker('getDisplayDate'));

    $datepicker = createDatepicker($filteritem.find('span.to-daterange-selector:first span.datepicker-container:first'), $container, $filteritem, filtervalue);
    $datepicker.datepicker('setDate', defaultToDate);
    $filteritem.find('span.to-daterange-selector:first span.btn-show-datepicker-title:first').text($datepicker.datepicker('getDisplayDate'));

    return {from : defaultFromDate.getTime(), to : defaultToDate.getTime()};
  }

  function showUserFilterDate (evt, $container, $filteritem, $dateselector) {
    if ($dateselector.hasClass('showed-datepicker')) {
      return undefined;
    }

    onBodyClick(evt);
    jQuery(document.body).unbind('click', onBodyClick);

    var $datepicker = $dateselector.addClass('showed-datepicker').find('span.advansed-filter-datepicker-container:first').css('display', 'block');

    $datepicker.removeClass('reverse-position');
    if ($filteritem.parents('div.hidden-filters-container:first').length === 0) {
      if ($container.width() - $filteritem[0].offsetLeft - $filteritem.parent()[0].offsetLeft - $filteritem.width() < $datepicker.width()) {
        $datepicker.addClass('reverse-position');
      }
    }

    setTimeout(function () {
      jQuery(document.body).one('click', onBodyClick);
    }, 1);
  }

  function destroyUserFilterDate ($container, $filteritem, filtervalue) {
    
  }

  /* </date> */
  /* <combobox> */

  function onUserFilterComboboxSelectValue (target, $container, $filteritem, filtervalue, nonetrigger) {
    var
      $target = jQuery(target),
      value = $target.val();

    if ($container.length > 0 && filtervalue) {
      if (isArray(value)) {
        var
          values = value,
          valuesInd = value.length;
        while (valuesInd--) {
          if (values[valuesInd] == -1) {
            values.splice(valuesInd, 1);
            break;
          }
        }
        value = values;
      }
      setFilterItem($container, $filteritem, filtervalue, {value : value, title : $target.find('option[value="' + value + '"]:first').text()}, nonetrigger);
    }
  }

  function compareUserFilterParamsCombobox ($container, containerfiltervalue, filtervalue) {
    if (containerfiltervalue.value == filtervalue.value) {
      return true;
    }
    if (isArray(containerfiltervalue.value) && isArray(filtervalue.value) && containerfiltervalue.value.length === filtervalue.value.length) {
      return [].concat(containerfiltervalue.value).sort().join('') == [].concat(filtervalue.value).sort().join('');
    }
    return false;
  }

  function customizeUserFilterCombobox ($container, $filteritem, filtervalue) {
    var
      $select = $filteritem.find('select.advansed-filter-combobox:first'),
      value = $select.val();

    if (isArray(value)) {
      var
        values = value,
        valuesInd = value.length;
      while (valuesInd--) {
        if (values[valuesInd] == -1) {
          values.splice(valuesInd, 1);
          break;
        }
      }
      value = values;
    }

    value = value == '-1' ? null : value;
    value = isArray(value) && (value.length === 0 || (value.length === 1 && value[0] == '-1')) ? null : value;

    $select
      .advansedFilterCustomCombobox()
      .change((function ($container, $filteritem, filtervalue, callback) {
        return function (evt) {
          callback(this, $container, $filteritem, filtervalue);
        }
      })($container, $filteritem, filtervalue, onUserFilterComboboxSelectValue));

    // d'ohhh
    return !value ? null : {value : value, title : $select.find('option[value="' + value + '"]:first').text()};
  }

  function destroyUserFilterCombobox ($container, $filteritem, filtervalue) {
    
  }

  /* </combobox> */

  function compareUserFilterParams ($container, filtervalue, params, nonetrigger) {
    var fn = null;
    switch (filtervalue.type) {
      case 'flag'       : fn = compareUserFilterParamsFlag;       break;
      case 'group'      : fn = compareUserFilterParamsGroup;      break;
      case 'person'     : fn = compareUserFilterParamsPerson;     break;
      case 'daterange'  : fn = compareUserFilterParamsDate;       break;
      case 'combobox'   : fn = compareUserFilterParamsCombobox;   break;
    }

    if (fn !== null) {
      if (filtervalue.hasOwnProperty('params')) {
        return fn($container, filtervalue.params, params, nonetrigger);
      }
    }
    return false;
  }

  function customizeUserFilter ($container, $filteritem, filtervalue, nonetrigger) {
    var fn = null;
    switch (filtervalue.type) {
      case 'flag'       : fn = customizeUserFilterFlag;       break;
      case 'group'      : fn = customizeUserFilterGroup;      break;
      case 'person'     : fn = customizeUserFilterPerson;     break;
      case 'daterange'  : fn = customizeUserFilterDate;       break;
      case 'combobox'   : fn = customizeUserFilterCombobox;   break;
    }

    if (fn !== null) {
      var o = fn($container, $filteritem, filtervalue, nonetrigger);
      return filtervalue.hasOwnProperty('bydefault') && filtervalue.bydefault && typeof filtervalue.bydefault === 'object' ? filtervalue.bydefault : o;
    }
  }

  function destroyUserFilter ($container, $filteritem, filtervalue) {
    var fn = null;
    switch (filtervalue.type) {
      case 'flag'       : fn = destroyUserFilterFlag;     break;
      case 'group'      : fn = destroyUserFilterGroup;    break;
      case 'person'     : fn = destroyUserFilterPerson;   break;
      case 'daterange'  : fn = destroyUserFilterDate;     break;
      case 'combobox'   : fn = destroyUserFilterCombobox; break;
    }

    if (fn !== null) {
      fn($container, $filteritem, filtervalue);
    }
  }

  function processUserFilter ($container, $filteritem, filtervalue, params, nonetrigger) {
    switch (filtervalue.type) {
      case 'flag' :
        onUserFilterFlagSelectValue($container, $filteritem, filtervalue, nonetrigger);
        break;
      case 'group' :
        onUserFilterGroupSelectValue($container, $filteritem, filtervalue, {Id : params.id, Name : getGroupSelectorName(params.id)}, nonetrigger);
        break;
      case 'person' :
        onUserFilterPersonSelectValue($container, $filteritem, filtervalue, params.id, getUserSelectorName(params.id, params.name), nonetrigger);
        break;
      case 'daterange' :
        var date = null, inst = null, $datepicker = null;

        if (params.hasOwnProperty('from')) {
          $datepicker = $filteritem.find('span.from-daterange-selector:first span.datepicker-container:first');
          date = new Date(+params.from);
          inst = $datepicker.datepicker('getInstance');

          if ($datepicker.length > 0 && inst && date instanceof Date) {
            $datepicker.datepicker('setDate', date);
            onUserFilterDateSelectValue($datepicker, $container, $filteritem, filtervalue, $datepicker.datepicker('getDisplayDate'), inst, nonetrigger);
          }
        }

        if (params.hasOwnProperty('to')) {
          $datepicker = $filteritem.find('span.to-daterange-selector:first span.datepicker-container:first');
          date = new Date(+params.to);
          inst = $datepicker.datepicker('getInstance');

          if ($datepicker.length > 0 && inst && date instanceof Date) {
            $datepicker.datepicker('setDate', date);
            onUserFilterDateSelectValue($datepicker, $container, $filteritem, filtervalue, $datepicker.datepicker('getDisplayDate'), inst, nonetrigger);
          }
        }
        break;
      case 'combobox' :
        $filteritem.find('select').val(params.value).change();
        break;
      default :
        filtervalue.hasOwnProperty('process') && typeof filtervalue.process === 'function' ? filtervalue.process($container, $filteritem, filtervalue, params, nonetrigger) : null;
        break;
    }
  }

  function showUserFilterByOption ($container, filterid, nonetrigger) {
    var
      $selectedfilters = $container.find('div.advansed-filter-filters:first div.filter-item'),
      $filteritems = $container.find('li.item-item.filter-item'),
      $filter = null;

    $filteritems.filter('[data-id="' + filterid + '"]').removeClass('hidden-item');
    updateFiltersList($container.find('ul.filter-list:first li').not('li.item-list-top'));
  }

  function hideUserFilterByOption ($container, filterid, nonetrigger) {
    var
      $selectedfilters = $container.find('div.advansed-filter-filters:first div.filter-item'),
      $filteritems = $container.find('li.item-item.filter-item'),
      $filter = null;

    $filteritems.filter('[data-id="' + filterid + '"]').addClass('hidden-item');
    updateFiltersList($container.find('ul.filter-list:first li').not('li.item-list-top'));

    if (($filter = $selectedfilters.filter('[data-id="' + filterid + '"]')).length > 0) {
      removeUserFilterByObject($container, $filter, nonetrigger);
      return true;
    }
  }

  function enableUserFilterByOption ($container, filterid, nonetrigger) {
    var
      $selectedfilters = $container.find('div.advansed-filter-filters:first div.filter-item'),
      $filteritems = $container.find('li.item-item.filter-item'),
      $filter = null;

    $filteritems.filter('[data-id="' + filterid + '"]').removeClass('disabled-item');
    updateFiltersList($container.find('ul.filter-list:first li').not('li.item-list-top'));
  }

  function disableUserFilterByOption ($container, filterid, nonetrigger) {
    var
      $selectedfilters = $container.find('div.advansed-filter-filters:first div.filter-item'),
      $filteritems = $container.find('li.item-item.filter-item'),
      $filter = null;

    $filteritems.filter('[data-id="' + filterid + '"]').addClass('disabled-item');
    updateFiltersList($container.find('ul.filter-list:first li').not('li.item-list-top'));

    if (($filter = $selectedfilters.filter('[data-id="' + filterid + '"]')).length > 0) {
      removeUserFilterByObject($container, $filter, nonetrigger);
      return true;
    }
  }

  function resetUserFilterByOption ($container, filterid, nonetrigger) {
    var
      $selectedfilters = $container.find('div.advansed-filter-filters:first div.filter-item'),
      $filter = null;
    if (($filter = $selectedfilters.filter('[data-id="' + filterid + '"]')).length > 0) {
      removeUserFilterByObject($container, $filter, nonetrigger);
      return true;
    }
  }

  function addUserFilter ($container, filtervalue, params, nonetrigger) {
    var
      $filteritem = null,
      $filters = $container.find('div.advansed-filter-filters:first');
    if ($filters.length === 0) {
      return undefined;
    }

    if (filtervalue.id === 'text') {
      if (params && typeof params === 'object' && params.hasOwnProperty('value')) {
        var $input = $container.find('input.advansed-filter-input:first');
        if ($input.val() != params.value) {
          $input.val(params.value);
          updateTextFilter($container, nonetrigger);
          return true;
        }
      };
      return false;
    }

    if (($filteritem = $filters.find('div.filter-item[data-id="' + filtervalue.id + '"]:first')).length > 0 && filtervalue.multiselect !== true) {
      if (!params || compareUserFilterParams($container, filtervalue, params, nonetrigger)) {
        return false;
      }
      removeUserFilterByObject($container, $filteritem, true);
    }

    var groupby = filtervalue.hasOwnProperty('groupby') && typeof filtervalue.groupby === 'string' && filtervalue.groupby.length > 0 ? filtervalue.groupby : null;
    if (groupby !== null) {
      var $groupfilter = $filters.find('div.filter-item[data-group="' + groupby + '"]:first');
      if ($container.length > 0 && $groupfilter.length > 0) {
        removeUserFilterByObject($container, $groupfilter, true);
      }
    }

    var customdata = $container.data('customdata');
    if (customdata) {
      var maxfilters = customdata.hasOwnProperty('maxfilters') ? customdata.maxfilters : -1;
      if (maxfilters !== -1 && $filters.find('div.filter-item').length >= maxfilters) {
        var $firstfilter = $filters.find('div.filter-item:first');
        if ($container.length > 0 && $firstfilter.length > 0) {
          removeUserFilterByObject($container, $firstfilter, true);
        }
      }
    }

    var paramsitems = isArray(params) ? params : [params];
    for (var i = 0, n = paramsitems.length; i < n; i++) {
      params = paramsitems[i];

      $filteritem = $((filtervalue.hasOwnProperty('create') && typeof filtervalue.create === 'function' ? filtervalue.create : createFilterItem)(filtervalue));

      if ($filteritem.length > 0) {
        $filters.append($filteritem);
        if (filtervalue) {
          var paramsid = params && typeof params === 'object' && params.hasOwnProperty('__id') ? params.__id : Math.floor(Math.random() * 1000000);
          $filteritem.attr('data-id', filtervalue.id).attr('data-paramsid', paramsid).attr('data-type', filtervalue.type).addClass('filter-item').addClass('filter-item-' + filtervalue.type);
          if (groupby !== null) {
            $filteritem.attr('data-group', groupby);
          }

          var defaultparams = (filtervalue.hasOwnProperty('customize') && typeof filtervalue.customize === 'function' ? filtervalue.customize : customizeUserFilter)($container, $filteritem, filtervalue, nonetrigger);
          defaultparams = defaultparams && typeof defaultparams === 'object' ? defaultparams : null;
          defaultparams = filtervalue.hasOwnProperty('value') ? filtervalue.value : defaultparams;
          defaultparams ? defaultparams.__id = paramsid : null;
          params = params && typeof params === 'object' ? params : null;
          params ? params.__id = paramsid : null;
          if (defaultparams && !params) {
            setFilterItem($container, $filteritem, filtervalue, defaultparams, nonetrigger);
            processUserFilter($container, $filteritem, filtervalue, defaultparams, true);
          }
          if (params) {
            setFilterItem($container, $filteritem, filtervalue, params, nonetrigger);
            processUserFilter($container, $filteritem, filtervalue, params, nonetrigger);
          }
        }
      }
    }

    resizeUserFilterContainer($container);
    return true;
  }

  function removeUserFilter ($container, filtervalue, $filteritem, nonetrigger) {
    var $filters = $container.find('div.advansed-filter-filters:first');
    if ($filters.length === 0) {
      return undefined;
    }

    if ($filteritem.length > 0) {
      if (filtervalue) {
        (filtervalue.hasOwnProperty('destroy') && typeof filtervalue.destroy === 'function' ? filtervalue.destroy : destroyUserFilter)($container, $filteritem, filtervalue);
        unsetFilterItem($container, $filteritem, filtervalue, nonetrigger);
      }
      $filteritem.find('div.advansed-filter-control').appendTo($container);
      $filteritem.remove();
    }

    resizeUserFilterContainer($container);
  }

  function removeUserFilterByObject ($container, $filteritem, nonetrigger) {
    var
      id = $filteritem.attr('data-id'),
      filtervalues = $container.data('filtervalues'),
      filtervaluesInd = 0;
    filtervaluesInd = filtervalues ? filtervalues.length : 0;
    while (filtervaluesInd--) {
      if (filtervalues[filtervaluesInd].id === id) {
        break;
      }
    }
    removeUserFilter($container, filtervaluesInd !== -1 ? filtervalues[filtervaluesInd] : null, $filteritem, nonetrigger);
  }

  function enableUserSorter ($container, sorterid, nonetrigger) {
    var
      $sorteritems = $container.find('li.item-item.sorter-item');

    $sorteritems.filter('[data-id="' + sorterid + '"]').removeClass('hidden-item');
  }

  function disableUserSorter ($container, sorterid, nonetrigger) {
    var
      $sorteritems = $container.find('li.item-item.sorter-item'),
      $sorter = null;

    $sorter = $sorteritems.filter('[data-id="' + sorterid + '"]').addClass('hidden-item');

    if ($sorter.hasClass('selected')) {
      $sorter = $sorteritems.not('.hidden-item').filter(':first');
      if ($sorter.length > 0) {
        sorterid = $sorter.attr('data-id');
        if (sorterid) {
          setUserSorter($container, sorterid, {dsc : false}, nonetrigger);
          return true;
        }
      }
    }
  }

  function setUserSorter ($container, sorterid, params, nonetrigger) {
    var
      filtervalues = $container.data('filtervalues') || [],
      filtervaluesInd = -1,
      sortervalues = $container.data('sortervalues') || [],
      sortervaluesInd = -1;

    if (sortervalues.length > 0) {
      sortervaluesInd = sortervalues.length;
      while (sortervaluesInd--) {
        if (sortervalues[sortervaluesInd].id === sorterid) {
          break;
        }
      }
    }

    if (filtervalues.length > 0) {
      filtervaluesInd = filtervalues.length;
      while (filtervaluesInd--) {
        if (filtervalues[filtervaluesInd].id === 'sorter') {
          break;
        }
      }
    }

    if (filtervaluesInd !== -1 && sortervaluesInd !== -1) {
      if (
        filtervalues[filtervaluesInd].params &&
        filtervalues[filtervaluesInd].params.id === sortervalues[sortervaluesInd].id &&
        filtervalues[filtervaluesInd].params.dsc === params.dsc
      ) {
        return false;
      }
      sortervalues[sortervaluesInd].dsc = params.dsc;
      sortervalues[sortervaluesInd].sortOrder = params.dsc === true ? 'descending' : 'ascending';
      setFilterItem($container, null, filtervalues[filtervaluesInd], sortervalues[sortervaluesInd], nonetrigger);
      return true;
    }
    return false;
  }

  function needHideFilters ($container, $filters, $firstfilter, $lastfilter) {
    var
      minInputWidth = 100;

//if ($firstfilter.length > 0 && $lastfilter.length > 0) {
//  console.log([
//    '($firstfilter[0].offsetTop !== $lastfilter[0].offsetTop) : ',
//    ($firstfilter[0].offsetTop !== $lastfilter[0].offsetTop),
//    '\n',
//    '($firstfilter[0].offsetHeight !== $lastfilter[0].offsetHeight) : ',
//    ($firstfilter[0].offsetHeight !== $lastfilter[0].offsetHeight),
//    '\n',
//    '($container[0].offsetWidth < $filters[0].offsetWidth + $filters[0].offsetLeft + minInputWidth) : ',
//    ($container[0].offsetWidth < $filters[0].offsetWidth + $filters[0].offsetLeft + minInputWidth),
//    '\n',
//    '($firstfilter.find(\'span.selector-wrapper:first\')[0].offsetHeight !== $lastfilter.find(\'span.selector-wrapper:first\')[0].offsetHeight) : ',
//    ($firstfilter.find('span.selector-wrapper:first')[0].offsetHeight !== $lastfilter.find('span.selector-wrapper:first')[0].offsetHeight)
//  ].join(''));
//}

    return $firstfilter.length === 0 || $lastfilter.length === 0 ? false : 
      ($firstfilter[0].offsetTop !== $lastfilter[0].offsetTop) ||
      ($firstfilter[0].offsetHeight !== $lastfilter[0].offsetHeight) ||
      ($container[0].offsetWidth < $filters[0].offsetWidth + $filters[0].offsetLeft + minInputWidth) ||
      ($firstfilter.find('span.selector-wrapper:first')[0].offsetHeight !== $lastfilter.find('span.selector-wrapper:first')[0].offsetHeight);
  }
 
  function resizeFilterGroupByHeight (opts, $container) {
    if (opts && typeof opts === 'object' && opts.colcount > 1) {
      var rowcount = 0, maxitemmetric = 0, itemmetric = 0,
          $grouplist = $container.find('ul.item-list.filter-list.multi-column:first'),
          $group = null,
          $cols = $grouplist.find('li.item-group-col'),
          $colsInd = 0,
          $groups = null;

      rowcount = Math.ceil($grouplist.find('li.item-group.filter-group').length / opts.colcount);
      while (rowcount--) {
        maxitemmetric = 0;
        $groups = $();
        $colsInd = $cols.length;
        while ($colsInd--) {
          $group = $($($cols[$colsInd]).find('li.item-group.filter-group')[rowcount]);
          if ($group.length > 0) {
            $groups = $groups.add($group);
            itemmetric = $group.height();
            maxitemmetric < itemmetric ? maxitemmetric = itemmetric : null;
          }
        }
        $groups.height(maxitemmetric + 'px');
      }
    }
  }

  function resizeUserSorterContainer ($container, title) {
    var
      sortercontainerWidth = 0,
      $filtercontainer = $container.find('div.advansed-filter-container:first'),
      $sortercontainer = $container.find('div.advansed-filter-sort-container:first');
    if ($sortercontainer.length > 0) {
      $sortercontainer.addClass('sorter-isset');
      if (title) {
        $sortercontainer.find('span.value:first').text(title);
      }
      sortercontainerWidth = $container.hasClass('disable-sorter-block') ? 0 : $sortercontainer.width();
      $filtercontainer.css('margin-right', (sortercontainerWidth > 0 ? sortercontainerWidth + 38 : 0) + 22 + 'px');
      $container.find('div.advansed-filter-helper:first').css('margin-right', (sortercontainerWidth > 0 ? sortercontainerWidth + 38 : 0) + 22 + 'px');
      //$container.find('label.advansed-filter-state:first').css('left', $filtercontainer.width() + 8 + 'px');
      $container.find('label.advansed-filter-state:first').css('left', 'auto').css('right', (sortercontainerWidth > 0 ? sortercontainerWidth + 38 : 0) + 'px');

      // if filter width change
      resizeUserFilterContainer($container);
    }
  }

  function resizeUserFilterContainer ($container, debug) {
    var
      containerWidth = $container.width(),
      $input = $container.find('div.advansed-filter-input:first'),
      $button = $container.find('div.advansed-filter-button:first'),
      $filters = $container.find('div.advansed-filter-filters:first'),
      $hiddenfilteritems = $filters.find('div.hidden-filters-container:first').find('div.filter-item');

    if ($input.length === 0 || $filters.length === 0 || containerWidth === 0) {
      return undefined;
    }

    if ($filters.find('div.filter-item').length === 0) {
      $filters.addClass('empty-list');
      $container.addClass('empty-filter-list');
    } else {
      $filters.removeClass('empty-list');
      $container.removeClass('empty-filter-list');
    }

    //$filters.removeClass('has-hidden-filters').find('div.filter-item').removeClass('hidden-filter').appendTo($filters);
    $filters.removeClass('has-hidden-filters').append($hiddenfilteritems.removeClass('hidden-filter'));

    var
      titlewidth = 0,
      maxwidth = 0,
      $selectorwrapper = null,
      $el = null,
      ind = 0,
      opts = $container.data('filteroptions'),
      $advansedcontainer = $container.find('div.advansed-filter-container:first'),
      $hiddenfilterscontainer = $filters.find('div.hidden-filters-container:first'),
      $firstfilter = $filters.find('div.filter-item:first'),
      $lastfilter = $filters.find('div.filter-item:last'),
      //$selectedfilters = $filters.children('div.filter-item').not('.is-rendered'),
      //selectedfiltersInd = $selectedfilters.length,
      $selectedfilters = $filters.find('div.filter-item'),
      needhidefilters = false;

    ind = $selectedfilters.length;
    while (ind--) {
      $el = jQuery($selectedfilters[ind]);
      $selectorwrapper = $el.find('span.selector-wrapper:first');
      titlewidth = $el.find('span.title:first').width() + 8; //8 - padding
      $el.width(titlewidth + $selectorwrapper.width());
      $selectorwrapper.css('left', titlewidth + 'px');
    }

    needhidefilters = needHideFilters($advansedcontainer, $filters, $firstfilter, $lastfilter);
    if ($firstfilter.length && needhidefilters) {
      $filters.addClass('has-hidden-filters');
      while ($firstfilter.length > 0 && needHideFilters($advansedcontainer, $filters, $firstfilter, $lastfilter)) {
        $el = $firstfilter;
        $firstfilter = $firstfilter.addClass('hidden-filter').next();
        $el.appendTo($hiddenfilterscontainer);
      }
    }

    if ($filters.find('div.hidden-filter').length > 0) {
      $filters.addClass('has-hidden-filters');
    } else {
      $filters.removeClass('has-hidden-filters');
    }

    var
      $hiddenfilters = $hiddenfilterscontainer.find('div.filter-item'),
      hiddenfiltersInd = $hiddenfilters.length,
      zindex = 0;
    while (hiddenfiltersInd--) {
      $hiddenfilters[hiddenfiltersInd].style.zIndex = ++zindex;
    }

    if ($filters.find('div.filter-item').length > 0) {
      $container.addClass('has-rendered-filters');
    } else {
      $container.removeClass('has-rendered-filters');
    }

    if (opts && typeof opts === 'object' && opts.colcount > 1) {
      var $advansedfilterlist = $container.find('ul.item-list.filter-list.multi-column:first');

      if ($advansedfilterlist.addClass('show-item-list').height() > 0 && !$advansedfilterlist.hasClass('is-render')) {
        resizeFilterGroupByHeight(opts, $container);

        var colwidth = 0, colswidth = 0,
            $advansedfilterlistcols = $advansedfilterlist.find('li.item-group-col');

        $advansedfilterlist.width('auto')
        var $advansedfilterlistcolsInd = $advansedfilterlistcols.length;
        while ($advansedfilterlistcolsInd--) {
          colwidth = $advansedfilterlistcols[$advansedfilterlistcolsInd].offsetWidth;
          $advansedfilterlistcols[$advansedfilterlistcolsInd].style.width = colwidth + 'px';
          colswidth += colwidth;
        }
        $advansedfilterlist.addClass('is-render').width(colswidth + 20 + 'px'); // 20 - padding
      }
      $advansedfilterlist.removeClass('show-item-list');
    }

    var offsetLeft = $filters.width() + 2;
    //if (/* !$filters.hasClass('is-render') && */$button.length > 0) {
    //  //$filters.addClass('is-render');
    //  //$filters.css('left', $button.width() + 1 + 3 + 'px');
    //  $button.css('left', offsetLeft + 'px');
    //  $container.find('div.advansed-filter-list:first').css('left', offsetLeft + 'px');
    //}
    $button.css('left', offsetLeft + 'px');
    $container.find('div.advansed-filter-list:first').css('left', offsetLeft + 'px');
    $input[0].style.marginLeft = $button[0].offsetWidth + offsetLeft + 'px';
  }

  function resizeControlContainer ($container, $filteritem, $control) {
    $control
      .addClass('reset-position')
      .parents('div.advansed-filter-control:first')
      .appendTo($filteritem.parents('div.hidden-filters-container:first').length === 0 ? $filteritem : $filteritem.parents('div.advansed-filter-filters:first').find('div.btn-show-hidden-filters:first'));

    if ($filteritem.parents('div.hidden-filters-container:first').length > 0) {
      return undefined;
    }

    if ($container.width() - $filteritem[0].offsetLeft - $filteritem.parent()[0].offsetLeft - $filteritem.width() < $control.width()) {
      var
        offset = $control.width() - ($container.width() - $filteritem[0].offsetLeft - $filteritem.parent()[0].offsetLeft - $filteritem.width()) + 40,
        offsetcontroltop = $control.find('div.control-top:first')[0].offsetLeft || 0,
        margincontainer = parseFloat($control.css('margin-left'));

      $control.removeClass('reset-position').css('margin-left', -offset + 'px')
        .find('div.control-top:first').css('left', offsetcontroltop + offset - (isFinite(margincontainer) ? Math.abs(margincontainer) : 0) + 'px');
    }
  }

  function resizeContainer ($container) {
    var
      $label = $container.find('label.advansed-filter-label:first'),
      $input = $container.find('input.advansed-filter-input:first'),
      $filtercontainer = $container.find('div.advansed-filter-container:first'),
      $filterlist = $container.find('div.advansed-filter-list:first'),
      id = $input.attr('id') || Math.floor(Math.random() * 1000000);

    $input.attr('id', id);
    $label.attr('for', id);

    var labelwidth = $label.innerWidth();
    $filtercontainer.css('margin-left', labelwidth + 'px');
    $filterlist.css('margin-left', labelwidth + 'px');
  }

  function toggleSorterBlock ($container, value) {
    value === true ? $container.removeClass('disable-sorter-block') : $container.addClass('disable-sorter-block');
  }

  function addReadyEvent (fn, args) {
    if (typeof fn === 'function') {
      jQuery((function (fn, args) {return function () {setTimeout(function () {fn.apply(window, args)}, 1)}})(fn, args));
    }
  }

  function setEvents ($container, opts) {
    $container = $container.hasClass('advansed-filter') ? $container : $container.find('div.advansed-filter:first');
    if ($container.length === 0) {
      return undefined;
    }

    if (opts.hasOwnProperty('anykey') && opts.anykey === true) {
      var timeout = opts.hasOwnProperty('anykeytimeout') ? opts.anykeytimeout : defaultAnykeyTimeout;
      timeout = isFinite(+timeout) ? +timeout : defaultAnykeyTimeout;
      if (timeout > 0) {
        filterInputKeyupTimeout = timeout;
        $container.find('input.advansed-filter-input:first').unbind('keyup', onFilterInputKeyupHelper).bind('keyup', onFilterInputKeyupHelper);
      } else {
        $container.find('input.advansed-filter-input:first').unbind('keyup', onFilterInputKeyup).bind('keyup', onFilterInputKeyup);
      }
    } else {
      $container.find('input.advansed-filter-input:first').unbind('keyup', onFilterInputEnter).bind('keyup', onFilterInputEnter);
    }
    $container.find('input.advansed-filter-input:first').unbind('keyup', onKeyUp).bind('keyup', onKeyUp);

    $container.find('.btn-start-filter:first').unbind('click', onStartFilter).bind('click', onStartFilter);
    $container.find('label.btn-reset-filter:first').unbind('click', onResetFilter).bind('click', onResetFilter);
    $container.find('ul.filter-list:first').unbind('click', onSelectFilter).bind('click', onSelectFilter);
    $container.find('ul.sorter-list:first').unbind('click', onSelectSorter).bind('click', onSelectSorter);
    $container.find('div.btn-show-filters:first').unbind('click', onShowFilters).bind('click', onShowFilters);
    $container.find('label.btn-show-sorters:first').unbind('click', onShowSorters).bind('click', onShowSorters);
    $container.find('span.btn-toggle-sorter:first').unbind('click', onToggleSorter).bind('click', onToggleSorter);
    $container.find('div.advansed-filter-sort-container:first span.title:first').unbind('click', onShowSorters).bind('click', onShowSorters);
    $container.find('div.advansed-filter-filters:first').unbind('click', onUserFilterClick).bind('click', onUserFilterClick);
    $container.find('div.advansed-filter-userselector:first').unbind('click', onUserSelectorClick).bind('click', onUserSelectorClick);
    $container.find('div.advansed-filter-groupselector:first').unbind('click', onGroupSelectorClick).bind('click', onGroupSelectorClick);
  }

  /* <callbacks> */

  function onBodyClick (p1) {
    var $target = p1 && typeof p1 === 'object' ? jQuery(p1.target) : null;

    if (
      ($target && $target.is('span.btn-show-datepicker') && $target.parents('div.hidden-filters-container:first').length > 0) ||
      ($target && $target.is('span.combobox-title') && $target.parents('div.hidden-filters-container:first').length > 0) ||
      ($target && $target.is('span.combobox-title-inner-text') && $target.parents('div.hidden-filters-container:first').length > 0)
    ) {
      jQuery(document.body).unbind('click', arguments.callee);
      jQuery(document.body).one('click', arguments.callee);

      jQuery('div.advansed-filter').find('span.advansed-filter-dateselector-date').removeClass('showed-datepicker').find('span.advansed-filter-datepicker-container').hide();
      return undefined;
    }

    if (p1 === true) {
      jQuery(document.body).trigger('click');
    }

    jQuery('div.advansed-filter').removeClass('showed-filters').find('ul.filter-list:first').hide();
    jQuery('div.advansed-filter').removeClass('showed-sorters').find('ul.sorter-list:first').hide();
    jQuery('div.advansed-filter').removeClass('showed-hidden-filters').find('div.hidden-filters-container:first').hide();
    jQuery('div.advansed-filter').removeClass('showed-userselector').find('div.advansed-filter-userselector-container:first').hide();
    jQuery('div.advansed-filter').removeClass('showed-groupselector').find('div.advansed-filter-groupselector-container:first').hide();

    jQuery('div.advansed-filter').find('span.advansed-filter-dateselector-date').removeClass('showed-datepicker').find('span.advansed-filter-datepicker-container').hide();
  }

  function onKeyUp (evt) {
    if (this.value === '') {
      this.parentNode.className = this.parentNode.className.replace(' has-value', '');
    } else {
      if (this.parentNode.className.indexOf(' has-value') === -1) {
        this.parentNode.className = this.parentNode.className + ' has-value';
      }
    }
  }

  function onShowFilters (evt) {
    var $filter = jQuery(this).parents('div.advansed-filter:first');
    if ($filter.hasClass('showed-filters')) {
      return undefined;
    }

    onBodyClick(evt);
    jQuery(document.body).unbind('click', onBodyClick);
    $filter.addClass('showed-filters').find('ul.filter-list:first').show();
    setTimeout(function () {
      jQuery(document.body).one('click', onBodyClick);
    }, 1);
  }

  function onShowSorters (evt) {
    var $container = jQuery(this).parents('div.advansed-filter:first');
    if ($container.hasClass('disable-sorter-block') || $container.hasClass('showed-sorters')) {
      return undefined;
    }

    onBodyClick(evt);
    jQuery(document.body).unbind('click', onBodyClick);
    $container.addClass('showed-sorters').find('ul.sorter-list:first').show();
    setTimeout(function () {
      jQuery(document.body).one('click', onBodyClick);
    }, 1);
  }

  function onToggleSorter (evt) {
    var $container = jQuery(this).parents('div.advansed-filter:first');
    if ($container.hasClass('disable-sorter-block')) {
      return undefined;
    }

    var $selsorter = $container.find('li.sorter-item.selected:first');

    if ($container.length === 0 || $selsorter.length === 0) {
      return undefined;
    }

    var sorterid = $selsorter.attr('data-id') || null;
    if (sorterid) {
      setUserSorter($container, sorterid, {dsc : $selsorter.hasClass('asc-sort')});
    }
  }

  function onSelectFilter (evt) {
    var $selfilter = jQuery(evt.target);
    if (!$selfilter.hasClass('filter-item')) {
      $selfilter = $selfilter.parents('li.filter-item:first');
    }
    var $container = $selfilter.parents('div.advansed-filter:first');
    if ($container.length === 0 || $selfilter.length === 0 || $selfilter.hasClass('hidden-item') || $selfilter.hasClass('disabled-item')) {
      return undefined;
    }

    var
      filtervalues = $container.data('filtervalues'),
      filtervaluesInd = 0,
      filterid = $selfilter.attr('data-id') || '';

    filtervaluesInd = filtervalues ? filtervalues.length : 0;
    while (filtervaluesInd--) {
      if (filtervalues[filtervaluesInd].id === filterid) {
        break;
      }
    }

    if (filtervaluesInd !== -1) {
      addUserFilter($container, filtervalues[filtervaluesInd]);
    }
  }

  function onSelectSorter (evt) {
    var $selsorter = jQuery(evt.target);
    if (!$selsorter.hasClass('sorter-item')) {
      $selsorter = $selsorter.parents('li.sorter-item:first');
    }
    var $container = $selsorter.parents('div.advansed-filter:first');
    if ($container.length === 0 || $selsorter.length === 0) {
      return undefined;
    }

    var sorterid = $selsorter.attr('data-id') || null;
    if (sorterid) {
      var dsc = $selsorter.hasClass('asc-sort');
      if (!$selsorter.hasClass('asc-sort') && !$selsorter.hasClass('dsc-sort') && $selsorter.hasClass('dsc-sort-default')) {
        dsc = true;
      }
      setUserSorter($container, sorterid, {dsc : dsc});
    }
  }

  function onUserFilterClick (evt) {
    var
      $container = null,
      $filteritem = null,
      $target = jQuery(evt.target);

    $container = $target.parents('div.advansed-filter:first');
    $filteritem = $target.hasClass('filter-item') ? $target : $target.parents('div.filter-item:first');

    if ($target.hasClass('btn-show-hidden-filters')) {
      var $filter = jQuery(this).parents('div.advansed-filter:first');
      if ($filter.hasClass('showed-hidden-filters')) {
        return undefined;
      }

      onBodyClick(evt);
      jQuery(document.body).unbind('click', onBodyClick);
      $filter.addClass('showed-hidden-filters').find('div.hidden-filters-container:first').show();
      setTimeout(function () {
        jQuery(document.body).one('click', onBodyClick);
      }, 1);
      return undefined;
    }

    if ($target.hasClass('btn-show-datepicker')) {
      var $dateselector = $target.parents('span.advansed-filter-dateselector-date:first');
      showUserFilterDate(evt, $container, $filteritem, $dateselector);
      return undefined;
    }

    if ($container.length === 0 || $filteritem.length === 0) {
      return undefined;
    }

    if ($target.hasClass('btn-delete')) {
      var
        id = $filteritem.attr('data-id'),
        filtervalues = $container.data('filtervalues'),
        filtervaluesInd = 0;
      filtervaluesInd = filtervalues ? filtervalues.length : 0;
      while (filtervaluesInd--) {
        if (filtervalues[filtervaluesInd].id === id) {
          break;
        }
      }
      removeUserFilter($container, filtervaluesInd !== -1 ? filtervalues[filtervaluesInd] : null, $filteritem);
      return undefined;
    }

    if ($target.hasClass('group-selector') || $target.parents('span.group-selector:first').length > 0) {
      showUserFilterGroup($container, $filteritem);
    }

    if ($target.hasClass('person-selector') || $target.parents('span.person-selector:first').length > 0) {
      showUserFilterPerson($container, $filteritem);
    }
  }

  function onUserSelectorClick (evt) {
    var
      $container = null,
      $filteritem = null,
      $target = jQuery(evt.target);

    $container = $target.parents('div.advansed-filter:first');

    if ($container.length === 0) {
      return undefined;
    }

    evt.stopPropagation();
    //if ($target.is('#userSelector') || $target.hasClass('adv-userselector-deps') || $target.parents('div.adv-userselector-deps:first').length > 0) {
    //  evt.stopPropagation();
    //}
  }

  function onGroupSelectorClick (evt) {
    var
      $container = null,
      $filteritem = null,
      $target = jQuery(evt.target);

    $container = $target.parents('div.advansed-filter:first');

    if ($container.length === 0) {
      return undefined;
    }

    evt.stopPropagation();
    //if ($target.is('.groupSelectorContainer', $container) || $target.is('.filterBox') || $target.parents('div.filterBox:first').length > 0) {
    //  evt.stopPropagation();
    //}
  }

  function onFilterInputKeyup (evt) {
    var
      $this = evt && typeof evt === 'object' ? jQuery(evt.target) : filterInputKeyupObject ? jQuery(filterInputKeyupObject) : jQuery(),
      value = $this.val(),
      $container = $this.parents('div.advansed-filter:first'),
      filtervalues = $container.data('filtervalues'),
      filtervaluesInd = 0;

    if (!filtervalues || filtervalues.length === 0) {
      return undefined;
    }

    filtervaluesInd = filtervalues.length;
    while (filtervaluesInd--) {
      if (filtervalues[filtervaluesInd].id === 'text') {
        break;
      }
    }

    if (filtervaluesInd !== -1) {
      wasUpdated = true;
      wasUpdated = filtervalues[filtervaluesInd].params && value == filtervalues[filtervaluesInd].params.value ? false : wasUpdated;
      wasUpdated = !filtervalues[filtervaluesInd].params && value == '' ? false : wasUpdated;
      if (wasUpdated) {
        if (typeof value === 'string' && value.length > 0) {
          setFilterItem($container, null, filtervalues[filtervaluesInd], {value : value});
        } else {
          unsetFilterItem($container, null, filtervalues[filtervaluesInd]);
        }
      }
    }
  }

  function onFilterInputKeyupHelper (evt) {
    clearTimeout(filterInputKeyupHandler);
    if (filterInputKeyupTimeout > 0) {
      filterInputKeyupObject = evt.target;
      filterInputKeyupHandler = setTimeout(onFilterInputKeyup, filterInputKeyupTimeout);
    }
  }

  function onFilterInputEnter (evt) {
    switch (evt.keyCode) {
      case 13 :
        return onFilterInputKeyup(evt);
    }
  }

  function onStartFilter (evt) {
    var
      $this = jQuery(this),
      $container = $this.parents('div.advansed-filter:first'),
      value = $container.find('input.advansed-filter-input:first').val(),
      filtervalues = $container.data('filtervalues'),
      filtervaluesInd = 0;

    if (!filtervalues || filtervalues.length === 0) {
      return undefined;
    }

    filtervaluesInd = filtervalues.length;
    while (filtervaluesInd--) {
      if (filtervalues[filtervaluesInd].id === 'text') {
        break;
      }
    }

    if (filtervaluesInd !== -1) {
      if (typeof value === 'string' && value.length > 0) {
        setFilterItem($container, null, filtervalues[filtervaluesInd], {value : value});
      } else {
        unsetFilterItem($container, null, filtervalues[filtervaluesInd]);
      }
    }
  }

  function onResetFilter (evt) {
    var
      $container = jQuery(this).parents('div.advansed-filter:first'),
      filtervalues = $container.data('filtervalues'),
      sortervalues = $container.data('sortervalues'),
      filtervaluesInd = 0,
      filtervalue = null,
      sorterfilter = null,
      $filters = $container.find('div.filter-item'),
      $filter = null,
      filtersInd = 0,
      wasRemover = false;

    lazyTrigger = true;
    $container.removeClass('has-filters')
      .find('div.advansed-filter-input:first').removeClass('has-value')
        .find('input.advansed-filter-input:first').val('');
    filtervaluesInd = filtervalues ? filtervalues.length : 0;
    while (filtervaluesInd--) {
      filtervalue = filtervalues[filtervaluesInd];
      if (filtervalue.id === 'text' && filtervalue.isset === true) {
        wasRemover = true;
        unsetFilterItem($container, null, filtervalue);
      }
      if (filtervalue.id === 'sorter') {
        sorterfilter = filtervalue;
      }
    }

    filtersInd = $filters.length;
    while (filtersInd--) {
      wasRemover = true;
      $filter = jQuery($filters[filtersInd]);
      removeUserFilterByObject($container, $filter, true);
    }

    // TODO: add reset sorter

    lazyTrigger = false;
    if (wasRemover === true) {
      callSetFilterTrigger($container, null, true);
    }
  }

  function onChangeHash (opts, $container, hash) {
    if (currentHash !== hash) {
      readLastState(opts, $container, undefined, true);
    }
  }
  /* </callbacks> */

  function readLastState (opts, $container, nonetrigger, clearAndRestore) {
    var
      containerfiltervalues = $container.length > 0 ? $container.data('filtervalues') || [] : [],
      filtervalues = getLocalStorageFilters(opts, $container);

    if (clearAndRestore === true) {
      var
        containerfiltervalueId = null,
        containerfiltervalue = null,
        containerfiltervaluesInd = 0,
        filtervaluesInd = 0;
      containerfiltervaluesInd = containerfiltervalues.length;
      while (containerfiltervaluesInd--) {
        containerfiltervalue = containerfiltervalues[containerfiltervaluesInd];
        containerfiltervalueId = containerfiltervalue.id;
        filtervaluesInd = filtervalues.length;
        while (filtervaluesInd--) {
          if (filtervalues[filtervaluesInd].id == containerfiltervalueId) {
            break;
          }
        }
        if (filtervaluesInd === -1) {
          filtervalues.push({
            id : containerfiltervalue.id,
            type : containerfiltervalue.type,
            reset : true
          });
        }
      }
    }

    if (filtervalues.length > 0) {
      initAdvansedFilter(null, $container, filtervalues, null, nonetrigger);
    }
  }

  function initAdvansedFilter (opts, $this, filtervalues, sortervalues, nonetrigger) {
    var
      wasCallTrigger = false,
      changeSorter = false,
      wasAdded = false,
      $container = $this.filter(':first'),
      containerfiltervalues = $container.length > 0 ? $container.data('filtervalues') || [] : [],
      containersortervalues = $container.length > 0 ? $container.data('sortervalues') || [] : [],
      containerfiltervaluesInd = 0,
      containersortervaluesInd = 0,
      filtervalue = null,
      sortervalue = null;

    if (opts) {
      $container[opts.hasButton === false ? 'addClass' : 'removeClass']('no-button');
    }

    // d'ohhh
    if (sortervalues === null) {
      sortervalues = [];
      var filtervaluesInd = filtervalues.length;
      while (filtervaluesInd--) {
        if (filtervalues[filtervaluesInd].id === 'sorter' && filtervalues[filtervaluesInd].params) {
          filtervalue = filtervalues[filtervaluesInd];
          sortervalue = {id : filtervalue.params.id, title : filtervalue.params.title, dsc : filtervalue.params.dsc, sortOrder : filtervalue.params.sortOrder, selected : filtervalue.selected === true};
          sortervalues.push(sortervalue);
          break;
        }
      }
    }

    opts = opts || {};
    lazyTrigger = true;
    wasCallTrigger = false;

    changeSorter = false;
    for (var i = 0, n = sortervalues.length; i < n; i++) {
      sortervalue = sortervalues[i];
      if (sortervalue.visible === true) {
        enableUserSorter($container, sortervalue.id, true);
      }
      if (sortervalue.visible === false) {
        if (disableUserSorter($container, sortervalue.id, true)) {
          changeSorter = true;
        }
      }
      if (sortervalue.selected === true) {
        //containersortervaluesInd = containersortervalues.length;
        //while (containersortervaluesInd--) {
        //  if (sortervalue.id == containersortervalues[containersortervaluesInd].id) {
        //    break;
        //  }
        //}
        //if (containersortervaluesInd !== -1) {
        //  extendItemValue(sortervalue, containersortervalues[containerfiltervaluesInd]);
        //}
        if (setUserSorter($container, sortervalue.id, {dsc : sortervalue.dsc === true || sortervalue.sortOrder === 'descending'}, true)) {
          changeSorter = true;
        }
      }
      if (sortervalue.def === true) {
        setUserSorter($container, sortervalue.id, {dsc : sortervalue.dsc === true || sortervalue.sortOrder === 'descending'}, true);
        break;
      }
    }

    wasAdded = false;
    for (var i = 0, n = filtervalues.length; i < n; i++) {
      filtervalue = filtervalues[i];
      if (filtervalue.visible === true) {
        showUserFilterByOption($container, filtervalue.id, true);
      }
      if (filtervalue.visible === false) {
        if (hideUserFilterByOption($container, filtervalue.id, true)) {
          wasAdded = true;
        }
      }
      if (filtervalue.enable === true) {
        enableUserFilterByOption($container, filtervalue.id, true);
      }
      if (filtervalue.enable === false) {
        if (disableUserFilterByOption($container, filtervalue.id, true)) {
          wasAdded = true;
        }
      }
      if (filtervalue.type === 'combobox' && filtervalue.hasOwnProperty('options')) {
        containerfiltervaluesInd = containerfiltervalues.length;
        while (containerfiltervaluesInd--) {
          if (filtervalue.id == containerfiltervalues[containerfiltervaluesInd].id) {
            break;
          }
        }
        if (containerfiltervaluesInd !== -1) {
          var containerfiltervalue = containerfiltervalues[containerfiltervaluesInd];
          containerfiltervalue.options = filtervalue.options.slice(0);
          // d'ohhh
          var defaulttitle = null;
          defaulttitle = typeof filtervalue.defaulttitle === 'string' && filtervalue.defaulttitle.length > 0 ? filtervalue.defaulttitle : defaulttitle;
          defaulttitle = typeof containerfiltervalue.defaulttitle === 'string' && containerfiltervalue.defaulttitle.length > 0 ? containerfiltervalue.defaulttitle : defaulttitle;
          if (defaulttitle) {
            containerfiltervalue.options.unshift({value : '-1', classname : 'default-value', title : defaulttitle, def : true});
          }
        }
      }
      if (filtervalue.visible !== false && filtervalue.hasOwnProperty('params') && filtervalue.params) {
        containerfiltervaluesInd = containerfiltervalues.length;
        while (containerfiltervaluesInd--) {
          if (filtervalue.id !== 'sorter' && filtervalue.id == containerfiltervalues[containerfiltervaluesInd].id) {
            break;
          }
        }
        if (containerfiltervaluesInd !== -1) {
          if (addUserFilter($container, containerfiltervalues[containerfiltervaluesInd], filtervalue.params, true)) {
            wasAdded = true;
          }
        }
      }
      if (filtervalue.reset === true || (filtervalue.hasOwnProperty('params') && filtervalue.params === null)) {
        if (resetUserFilterByOption($container, filtervalue.id, true)) {
          wasAdded = true;
        }
      }
    }
    if (opts.store === true) {
      if (!$container.hasClass('is-init')) {
        readLastState(opts, $container, true);
        if (opts.inhash === true) {
          try {
            ASC.Controls.AnchorController.bind((function (opts, $container) {
              return function (hash) {
                onChangeHash(opts, $container, hash);
              };
            })(opts, $container));
          } catch (err) {}
        }
      }
    }
    lazyTrigger = false;

    if (!$container.hasClass('is-init')) {
      if (nonetrigger !== true) {
        resizeContainer($container);
        $container.addClass('is-init');
        callSetFilterTrigger($container);
        wasCallTrigger = true;
      }
    }

    if (nonetrigger !== true && wasCallTrigger !== true && (wasAdded === true || changeSorter === true)) {
      wasCallTrigger = true;
      callSetFilterTrigger($container);
    }
  }

  function setAdvansedFilter ($this, id, params) {
    var
      $container = $this.filter(':first'),
      containerfiltervalues = $container.length > 0 ? $container.data('filtervalues') || [] : [],
      containerfiltervaluesInd = 0;

    containerfiltervaluesInd = containerfiltervalues.length;
    while (containerfiltervaluesInd--) {
      if (id == containerfiltervalues[containerfiltervaluesInd].id) {
        break;
      }
    }
    if (containerfiltervaluesInd !== -1) {
      setFilterItem($container, null, containerfiltervalues[containerfiltervaluesInd], params);
    }
  }

  function getAdvansedFilter ($this) {
    var
      $container = $this.filter(':first'),
      $filters = null,
      filterid = null,
      filtervalue = null,
      filtervalues = $container.data('filtervalues'),
      filtervaluesInd = 0,
      selectedfilters = [];

    if (!filtervalues || filtervalues.length === 0) {
      return selectedfilters;
    }

    updateTextFilter($container, true);

    filtervaluesInd = filtervalues.length;
    while (filtervaluesInd--) {
      if (filtervalues[filtervaluesInd].isset === true) {
        selectedfilters.unshift(filtervalues[filtervaluesInd]);
      }
    }

    return selectedfilters;
  };

  function clearAdvansedFilter ($this) {
    var
      wasRemover = false,
      $container = $this.filter(':first'),
      filtervalues = $container.length > 0 ? $container.data('filtervalues') || [] : [],
      $filters = $container.find('div.advansed-filter-filters:first div.filter-item'),
      $filter = null,
      filtersInd = 0,
      filtervaluesInd = 0;

    lazyTrigger = true;
    $container.removeClass('has-filters')
      .find('div.advansed-filter-input:first').removeClass('has-value')
        .find('input.advansed-filter-input:first').val('');
    filtervaluesInd = filtervalues ? filtervalues.length : 0;
    while (filtervaluesInd--) {
      filtervalue = filtervalues[filtervaluesInd];
      if (filtervalue.id === 'text' && filtervalue.isset === true) {
        wasRemover = true;
        $container.find('input.advansed-filter-input:first').val('');
        unsetFilterItem($container, null, filtervalue);
      }
    }
    filtersInd = $filters.length;
    while (filtersInd--) {
      wasRemover = true;
      $filter = jQuery($filters[filtersInd]);
      removeUserFilterByObject($container, $filter, true);
    }
    lazyTrigger = false;
    if (wasRemover === true) {
      callSetFilterTrigger($container);
    }
  }

  function updateEmptyInputs ($els, filtervalues, sortervalues, opts, customdata) {
    var
      needIndex = opts.hasOwnProperty('zindex') ? opts.zindex === true : false,
      $template = null,
      $containers = $(),
      $container = null,
      elsInd = 0,
      $el = null;

    $template = $(createAdvansedFilter(filtervalues, sortervalues));
    updateAdvansedFilter(opts, $template, filtervalues, sortervalues);
    setEvents($template, opts);

    customdata = customdata && typeof customdata === 'object' ? customdata : null;
    elsInd = $els.length;
    while (elsInd--) {
      $el = $($els[elsInd]).val('');
      if (($container = $el.is('div.advansed-filter') ? $el : $el.parents('div.advansed-filter:first')).length > 0) {
        if (needIndex === true) {
          $container.css('z-index', 100 - elsInd);
        }
        continue;
      }
      $container = $template.clone(true);
      $container.insertBefore($el).find('input.advansed-filter-complete:first').attr('id', $el.attr('id') || null);
      $el.remove();
      $container = $container.hasClass('advansed-filter') ? $container : $container.find('div.advansed-filter:first');

      if (needIndex === true) {
        $container.css('z-index', 100 - elsInd);
      }

      $container.addClass('has-events').data('filtervalues', filtervalues).data('sortervalues', sortervalues).data('customdata', customdata).data('filteroptions', opts);

      if (opts.hasOwnProperty('help')) {
        $container.addClass('has-help').find('div.advansed-filter-helper:first').html(opts.help);
      }

      if (opts.hasOwnProperty('hint')) {
        var popupId = Math.floor(Math.random() * 1000000);
        $container.find('div.advansed-filter-hint-popup:first').attr(id, popupId).html(opts.hint);
        $container.addClass('has-hint').find('label.advansed-filter-hint:first').helper({BlockHelperID : popupId});
      }

      $containers = $containers.add($container);
    }
    return $containers;
  }

  function updateControlInputs ($els, filtervalues, sortervalues, opts, customdata) {
    var
      needIndex = opts.hasOwnProperty('zindex') ? opts.zindex === true : false,
      $containers = $(),
      $container = null,
      elsInd = 0,
      $el = null;

    customdata = customdata && typeof customdata === 'object' ? customdata : null;
    elsInd = $els.length;
    while (elsInd--) {
      $el = $($els[elsInd]).val('');
      if (($container = $el.is('div.advansed-filter') ? $el : $el.parents('div.advansed-filter:first')).length === 0) {
        continue;
      }

      if (!$container.hasClass('has-events')) {
        $container.find('input.advansed-filter:first').val('').attr('maxlength', opts.hasOwnProperty('maxlength') ? opts.maxlength : null);

        updateAdvansedFilter(opts, $container, filtervalues, sortervalues);
        setEvents($container, opts);

        if (needIndex === true) {
          $container.css('z-index', 100 - elsInd);
        }

        $container.addClass('has-events').data('filtervalues', filtervalues).data('sortervalues', sortervalues).data('customdata', customdata).data('filteroptions', opts);
        if (opts.hasOwnProperty('help')) {
          $container.addClass('has-help').find('div.advansed-filter-helper:first').html(opts.help);
        }

        if (opts.hasOwnProperty('hint')) {
          var popupId = Math.floor(Math.random() * 1000000);
          $container.find('div.advansed-filter-hint-popup:first').attr('id', popupId).html(opts.hint);
          $container.addClass('has-hint').find('label.advansed-filter-hint:first').attr('data-popupid', popupId).click(function () {
            jQuery(this).helper({BlockHelperID : jQuery(this).attr('data-popupid')});

            // check position
            var $hint = jQuery('#' + jQuery(this).attr('data-popupid'));
            var top = $hint.offset().top;
            if ($hint.offset().top > $hint.height()) {
              $hint.removeClass('valign-bottom').addClass('valign-top')
                .parents('div.advansed-filter-hint-popup-helper:first').removeClass('valign-bottom').addClass('valign-top')
                .find('div.cornerHelpBlock:first').removeClass('pos_bottom').addClass('pos_top');
            } else {
              $hint.removeClass('valign-top').addClass('valign-bottom')
                .parents('div.advansed-filter-hint-popup-helper:first').removeClass('valign-top').addClass('valign-bottom')
                .find('div.cornerHelpBlock:first').removeClass('pos_top').addClass('pos_bottom');
            }
          });
        }
      }
      $containers = $containers.add($container);
    }
    return $containers;
  }

  $.fn.advansedFilter = $.fn.advansedfilter = function (opts) {
    if (arguments.length === 0) {
      return getAdvansedFilter($(this));
    }

    //if (arguments.length === 2) {
    //  return setAdvansedFilter($(this), arguments[0], arguments[1]);
    //}

    if (opts === null) {
      return clearAdvansedFilter($(this));
    }

    if (opts && typeof opts === 'string') {
      var
        cmd = opts,
        $container = $(this).filter(':first');

      switch (cmd) {
        case 'sort' :
          toggleSorterBlock($container, arguments.length > 1 ? arguments[1] === true : true);

          resizeUserFilterContainer($container);
          resizeUserSorterContainer($container);
          resizeContainer($container);
          return undefined;
        case 'filters' :
          return getContainerFilters($container);
        case 'storage' :
          return getLocalStorageFilters();
        case 'resize' :
          resizeUserSorterContainer($container);
          resizeContainer($container);
          resizeUserFilterContainer($container);
          return undefined;
        default :
          return setAdvansedFilter($container, arguments[0], arguments[1]);
      }
      return undefined;
    }

    var
      customdata = null,
      colcount = 1,
      opts = opts && typeof opts === 'object' ? opts : {},
      filtervalues = opts.hasOwnProperty('filters') ? opts.filters : [],
      sortervalues = opts.hasOwnProperty('sorters') ? opts.sorters : [],
      resources = opts.hasOwnProperty('resources') ? opts.resources : {},
      maxfilters = opts.hasOwnProperty('maxfilters') ? isFinite(+opts.maxfilters) && +opts.maxfilters >= -1 ? +opts.maxfilters : -1 : -1,
      $containers = null,
      $this = $(this);

    if ($this.length === 0) {
      return $this;
    }

    for (var fld in resources) {
      if (resources.hasOwnProperty(fld)) {
        Resources[fld] = resources[fld];
      }
    }

    opts.colcount = opts.hasOwnProperty('colcount') && isFinite(+opts.colcount) ? +opts.colcount : colcount;
    opts.colcount = opts.colcount > 4 ? 4 : opts.colcount;

    customdata = {maxfilters : maxfilters};
    filtervalues = extendItemValues(filtervalues, filterValues);
    sortervalues = extendItemValues(sortervalues, sorterValues);

    if (($containers = updateControlInputs($this, filtervalues, sortervalues, opts, customdata)).length === $this.length) {
      addReadyEvent(initAdvansedFilter, [opts, $containers, filtervalues, sortervalues, opts.nonetrigger]);
      return $containers;
    }

    $containers = updateEmptyInputs($this, filtervalues, sortervalues, opts, customdata);
    addReadyEvent(initAdvansedFilter, [opts, $containers, filtervalues, sortervalues, opts.nonetrigger]);
    return $containers;
  };
})(jQuery, window, document, document.body);
