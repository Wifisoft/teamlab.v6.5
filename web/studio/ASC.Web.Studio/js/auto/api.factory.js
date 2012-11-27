
;window.ServiceFactory = (function () {
  var
    isInit = false,
    onlyFactory = false,
    defaultAvatar = '',
    defaultAvatarMedium = '',
    defaultAvatarSmall = '',
    formatDatetime = null,
    formatDate = null,
    formatTime = null,
    formats = {datetime : formatDatetime, date : formatDate, time : formatTime},
    monthNames = [],
    monthShortNames = [],
    dayNames = [],
    dayShortNames = [],
    nameCollections = {days : dayNames, shortdays : dayShortNames, months : monthNames, shortmonths : monthShortNames},
    portalUtcOffsetTotalMinutes = 0,
    portalUtcOffset = '',
    portalTimeZoneName = '',
    myProfile = null,
    portalSettings = {},
    portalQuotas = {},
    supportedImgs = [],
    supportedDocs = [],
    supportedTypes = [],
    searchEntityTypes = [
      {id : -1, label : 'unknown'},
      {id : 0,  label : 'project'},
      {id : 1,  label : 'milestone'},
      {id : 2,  label : 'task'},
      {id : 3,  label : 'subtask'},
      {id : 4,  label : 'team'},
      {id : 5,  label : 'comment'},
      {id : 6,  label : 'discussion'},
      {id : 7,  label : 'file'},
      {id : 8,  label : 'timespend'},
      {id : 9,  label : 'activity'}
    ],
    folderTypes = [
      {id : 0, name : 'DEFAULT',  label : 'folder-default'},
      {id : 1, name : 'COMMON',   label : 'folder-common'},
      {id : 2, name : 'BUNCH',    label : 'folder-bunch'},
      {id : 3, name : 'TRASH',    label : 'folder-trash'},
      {id : 5, name : 'USER',     label : 'folder-user'},
      {id : 6, name : 'SHARE',    label : 'folder-shared'}
    ],
    // mail - 0, tel - 1, link - 2
    contactTitles = {
      mail        : {name : 'mail',         type : 0, title : 'Email'},
      facebook    : {name : 'facebook',     type : 2, title : 'FaceBook'},
      myspace     : {name : 'myspace',      type : 2, title : 'MySpace'},
      livejournal : {name : 'livejournal',  type : 2, title : 'LiveJournal'},
      twitter     : {name : 'twitter',      type : 2, title : 'twitter'},
      yahoo       : {name : 'yahoo',        type : 2, title : 'YAHOO'},
      jabber      : {name : 'jabber',       type : 2, title : 'Jabber'},
      blogger     : {name : 'blogger',      type : 2, title : 'blogger'},
      skype       : {name : 'skype',        type : 2, title : 'skype'},
      msn         : {name : 'msn',          type : 2, title : 'msn'},
      aim         : {name : 'aim',          type : 2, title : 'aim'},
      icq         : {name : 'icq',          type : 2, title : 'icq'},
      gmail       : {name : 'gmail',        type : 0, title : 'Google Mail'},
      gbuzz       : {name : 'gbuzz',        type : 2, title : 'Google Buzz'},
      gtalk       : {name : 'gtalk',        type : 2, title : 'Google Talk'},
      phone       : {name : 'phone',        type : 1, title : 'Tel'},
      mobphone    : {name : 'mobphone',     type : 1, title : 'Mobile'}
    },
    contactTypes = {
      phone : {id : 0, title : 'Phone', categories : {work : {id : 1, title : 'Work'}, home : {id : 0, title : 'Home'}, mobile : {id : 2, title : 'Mobile'}, fax : {id : 3, title : 'Fax'}, direct : {id : 4, title : 'Direct'}, other : {id : 5, title : 'Other'}}},
      email : {id : 1, title : 'Email', categories : {work : {id : 1, title : 'Work'}, home : {id : 0, title : 'Home'}, other : {id : 2, title : 'Other'}}},
      website : {id : 2, title : 'Website', categories : {work : {id : 1, title : 'Work'}, home : {id : 0, title : 'Home'}, other : {id : 2, title : 'Other'}}},
      skype : {id : 3, title : 'Skype', categories : {work : {id : 1, title : 'Work'}, home : {id : 0, title : 'Home'}, other : {id : 2, title : 'Other'}}},
      twitter : {id : 4, title : 'Twitter', categories : {work : {id : 1, title : 'Work'}, home : {id : 0, title : 'Home'}, other : {id : 2, title : 'Other'}}},
      linkedin : {id : 5, title : 'LinkedIn', categories : {work : {id : 1, title : 'Work'}, home : {id : 0, title : 'Home'}, other : {id : 2, title : 'Other'}}},
      facebook : {id : 6, title : 'Facebook', categories : {work : {id : 1, title : 'Work'}, home : {id : 0, title : 'Home'}, other : {id : 2, title : 'Other'}}},
      address : {id : 7, title : 'Address', categories : {work : {id : 5, title : 'Work'}, home : {id : 0, title : 'Home'}, postal : {id : 1, title : 'Postal'}, office : {id : 2, title : 'Office'}, billing : {id : 3, title : 'Billing'}, other : {id : 4, title : 'Other'}}},
      livejournal : {id : 8, title : 'LiveJournal', categories : {work : {id : 1, title : 'Work'}, home : {id : 0, title : 'Home'}, other : {id : 2, title : 'Other'}}},
      myspace : {id : 9, title : 'MySpace', categories : {work : {id : 1, title : 'Work'}, home : {id : 0, title : 'Home'}, other : {id : 2, title : 'Other'}}},
      gmail : {id : 10, title : 'GMail', categories : {work : {id : 1, title : 'Work'}, home : {id : 0, title : 'Home'}, other : {id : 2, title : 'Other'}}},
      blogger : {id : 11, title : 'Blogger', categories : {work : {id : 1, title : 'Work'}, home : {id : 0, title : 'Home'}, other : {id : 2, title : 'Other'}}},
      yahoo : {id : 12, title : 'Yahoo', categories : {work : {id : 1, title : 'Work'}, home : {id : 0, title : 'Home'}, other : {id : 2, title : 'Other'}}},
      msn : {id : 13, title : 'MSN', categories : {work : {id : 1, title : 'Work'}, home : {id : 0, title : 'Home'}, other : {id : 2, title : 'Other'}}},
      icq : {id : 14, title : 'ICQ', categories : {work : {id : 1, title : 'Work'}, home : {id : 0, title : 'Home'}, other : {id : 2, title : 'Other'}}},
      jabber : {id : 15, title : 'Jabber', categories : {work : {id : 1, title : 'Work'}, home : {id : 0, title : 'Home'}, other : {id : 2, title : 'Other'}}},
      aim : {id : 16, title : 'AIM', categories : {work : {id : 1, title : 'Work'}, home : {id : 0, title : 'Home'}, other : {id : 2, title : 'Other'}}}
    },
    extTypes = [
      {name : 'archive',  exts : ['.zip', '.rar', '.ace', '.arc', '.arj', '.cab', '.enc', '.jar', '.lha', '.lzh', '.pak', '.pk3', '.tar', '.tgz', '.uue', '.xxe', '.zoo', '.bh', '.gz', '.ha']},
      {name : 'image',    exts : ['.bmp', '.cod', '.gif', '.ief', '.jpe', '.jpg', '.tif', '.cmx', '.ico', '.pnm', '.pbm', '.ppm', '.psd', '.rgb', '.xbm', '.xpm', '.xwd', '.png', '.ai', '.jpeg']},
      {name : 'sound',    exts : ['.mp3', '.wav', '.pcm', '.3gp', '.fla', '.cda', '.ogg', '.aiff', '.flac']},
      {name : 'ebook',    exts : ['.fb2', '.ibk', '.prc', '.epub']},
      {name : 'html',     exts : ['.htm', '.mht', '.html']},
      {name : 'djvu',     exts : ['.djvu']},
      {name : 'svg',      exts : ['.svg']},
      {name : 'doc',      exts : ['.doc', '.docx']},
      {name : 'xls',      exts : ['.xls', '.xlsx']},
      {name : 'pps',      exts : ['.pps', '.ppsx']},
      {name : 'ppt',      exts : ['.ppt', '.pptx']},
      {name : 'odp',      exts : ['.odp']},
      {name : 'ods',      exts : ['.ods']},
      {name : 'odt',      exts : ['.odt']},
      {name : 'pdf',      exts : ['.pdf']},
      {name : 'rtf',      exts : ['.rtf']},
      {name : 'txt',      exts : ['.txt']},
      {name : 'iaf',      exts : ['.iaf']},
      {name : 'csv',      exts : ['.csv']},
      {name : 'xml',      exts : ['.xml']},
      {name : 'xps',      exts : ['.xps']},
      {name : 'avi',      exts : ['.avi']},
      {name : 'flv',      exts : ['.flv', '.fla']},
      {name : 'm2ts',     exts : ['.m2ts']},
      {name : 'mkv',      exts : ['.mkv']},
      {name : 'mov',      exts : ['.mov']},
      {name : 'mp4',      exts : ['.mp4']},
      {name : 'mpg',      exts : ['.mpg']},
      {name : 'vob',      exts : ['.vob']}
    ],
    milestoneStatuses = {
      open            : {id : 0, name : 'open'},
      closed          : {id : 1, name : 'closed'}
    },
    taskStatuses = {
      notaccept       : {id : 0, name : 'notaccept'},
      open            : {id : 1, name : 'open'},
      closed          : {id : 2, name : 'closed'},
      disable         : {id : 3, name : 'disable'},
      unclassified    : {id : 4, name : 'unclassified'},
      notinmilestone  : {id : 5, name : 'notinmilestone'}
    },
    fileShareTypes = {
      None      : '0',
      ReadWrite : '1',
      Read      : '2',
      Restrict  : '3'
    },
    apiAnchors = [
      {handler : 'cmt-blog', re : /blog\.json/, method : 'post'},
      {handler : 'cmt-blog', re : /blog\/[\w\d-]+\.json/},
      {handler : 'cmt-topic', re : /forum\/topic\/[\w\d-]+\.json/},
      {handler : 'cmt-event', re : /event\/[\w\d-]+\.json/},
      {handler : 'cmt-event', re : /event\.json/, method : 'post'},
      {handler : 'cmt-bookmark', re : /bookmark\/[\w\d-]+\.json/},
      {handler : 'cmt-bookmark', re : /bookmark\.json/, method : 'post'},
      {handler : 'cmt-blogs', re : /blog\.json/, method : 'get'},
      {handler : 'cmt-blogs', re : /blog\/@search\/.+\.json/, method : 'get'},
      {handler : 'cmt-topics', re : /forum\/topic\/recent\.json/},
      {handler : 'cmt-topics', re : /forum\/@search\/.+\.json/, method : 'get'},
      {handler : 'cmt-categories', re : /forum\.json/},
      {handler : 'cmt-events', re : /event\.json/, method : 'get'},
      {handler : 'cmt-events', re : /event\/@search\/.+\.json/, method : 'get'},
      {handler : 'cmt-bookmarks', re : /bookmark\/top\/recent\.json/, method : 'get'},
      {handler : 'cmt-bookmarks', re : /bookmark\/@search\/.+\.json/, method : 'get'},

      {handler : 'prj-task', re : /project\/[\w\d-]+\/task\.json/, method : 'post'},
      {handler : 'prj-task', re : /project\/task\/[\w\d-]+\.json/},
      {handler : 'prj-task', re : /project\/task\/[\w\d-]+\/[\w\d-]+\.json/},
      {handler : 'prj-task', re : /project\/task\/[\w\d-]+\/status\.json/},
      {handler : 'prj-task', re : /project\/task\/[\w\d-]+\/[\w\d-]+\/status\.json/},
      {handler : 'prj-tasks', re : /project\/[\w\d-]+\/task\.json/, method : 'get'},
      {handler : 'prj-tasks', re : /project\/[\w\d-]+\/task\/[@all|@self]+\.json/},
      {handler : 'prj-tasks', re : /project\/[\w\d-]+\/task\/filter\.json/},
      {handler : 'prj-tasks', re : /project\/task\/filter\.json/},
      {handler : 'prj-tasks', re : /project\/task\/@self\.json/},
      {handler : 'prj-milestone', re : /project\/milestone\/[\w\d-]+\.json/},
      {handler : 'prj-milestone', re : /project\/[\w\d-]+\/milestone\.json/, method : 'post'},
      {handler : 'prj-milestones', re : /project\/[\w\d-]+\/milestone\.json/, method : 'get'},
      {handler : 'prj-milestones', re : /project\/milestone\/[\w\d-]+\/[\w\d-]+\/[\w\d-]+\.json/},
      {handler : 'prj-milestones', re : /project\/milestone\/[\w\d-]+\/[\w\d-]+\.json/},
      {handler : 'prj-milestones', re : /project\/milestone\/late\.json/},
      {handler : 'prj-milestones', re : /project\/milestone\.json/},
      {handler : 'prj-milestones', re : /project\/milestone\/filter\.json/},
      {handler : 'prj-milestone', re : /project\/milestone\/[\w\d-]+\/status\.json/},
      {handler : 'prj-discussion', re : /project\/message\/[\w\d-]+\.json/},
      {handler : 'prj-discussion', re : /project\/[\w\d-]+\/message\.json/, method : 'post'},
      {handler : 'prj-discussions', re : /project\/message\.json/},
      {handler : 'prj-discussions', re : /project\/[\w\d-]+\/message\.json/, method : 'get'},
      {handler : 'prj-discussions', re : /project\/message\/filter\.json/},
      {handler : 'prj-projectrequest', re : /project\/request\.json/},
      {handler : 'prj-projectrequest', re : /project\/request\/[\w\d-]+\.json/},
      {handler : 'prj-project', re: /project\.json/, method: 'post' },
      {handler : 'prj-projects', re : /project[\/]*[@self|@follow]*\.json/},
      {handler : 'prj-projects', re : /project\/filter\.json/},
      {handler : 'prj-searchentries', re : /project\/@search\/.+\.json/},
      {handler : 'prj-projectperson', re : /project\/[\w\d-]+\/team\.json/, method : 'post'},
      {handler : 'prj-projectperson', re : /project\/[\w\d-]+\/team\.json/, method : 'delete'},
      {handler : 'prj-projectpersons', re : /project\/[\w\d-]+\/team\.json/, method : 'get'},
      {handler : 'prj-activities', re : /project\/activities\/filter\.json/},

      {handler : 'doc-folder', re : /files\/[^\/]+$/},
      {handler : 'doc-folder', re : /files\/[^\/]+\/[text|html|file]+\.json/},
      {handler : 'doc-file', re : /files\/file\/[^\/]+$/},
      {handler : 'doc-file', re : /files\/[^\/]+\/upload\.xml/},
      {handler : 'doc-file', re : /files\/[^\/]+\/upload\.json/},

      {handler : 'doc-folder', re : /files\/[@%\w\d-_]+\.json/},
      {handler : 'doc-folder', re : /files\/[@%\w\d-_]+\/[text|html|file]+\.json/},
      {handler : 'doc-folder', re : /project\/[\w\d-]+\/files\.json/},
      {handler : 'doc-files', re : /project\/task\/[\w\d-]+\/files\.json/},
      {handler : 'doc-files', re : /project\/[\w\d-]+\/entityfiles\.json/},
      {handler : 'doc-files', re : /crm\/[\w\d-]+\/[\w\d-]+\/files\.json/, method : 'get'},
      {handler : 'doc-file', re : /files\/file\/[@%\w\d-_]+\.json/},
      {handler : 'doc-file', re : /files\/[@%\w\d-_]+\/upload\.xml/},
      {handler : 'doc-file', re : /files\/[@%\w\d-_]+\/upload\.json/},
      {handler : 'doc-file', re : /crm\/files\/[\w\d-]+\.json/},
      {handler : 'doc-file', re : /crm\/[case|contact|opportunity]+\/[\w\d-]+\/files\/upload\.xml/},
      {handler : 'doc-file', re : /crm\/[case|contact|opportunity]+\/[\w\d-]+\/files\/upload\.json/},
      {handler : 'doc-file', re : /crm\/[case|contact|opportunity]+\/[\w\d-]+\/files\/text\.json/},

      {handler : 'crm-address', re : /crm\/contact\/[\w\d-]+\/data\.json/},
      {handler : 'crm-address', re : /crm\/contact\/[\w\d-]+\/batch\.json/},
      {handler : 'crm-contact', re : /crm\/contact\/[\w\d-]+\.json/},
      {handler : 'crm-contact', re : /crm\/contact\/person\/[\w\d-]+\.json/, method : 'put'},
      {handler : 'crm-contact', re : /crm\/contact\/company\/[\w\d-]+\/person\.json/, method : 'post'},
      {handler : 'crm-contact', re : /crm\/contact\/company\/[\w\d-]+\/person\.json/, method : 'delete'},
      {handler : 'crm-contact', re : /crm\/[case|opportunity]+\/[\w\d-]+\/contact\.json/, method : 'post'},
      {handler : 'crm-contact', re : /crm\/[case|opportunity]+\/[\w\d-]+\/contact\/[\w\d-]+\.json/, method : 'post'},
      {handler : 'crm-contact', re : /crm\/[case|opportunity]+\/[\w\d-]+\/contact\/[\w\d-]+\.json/},
      {handler : 'crm-task', re : /crm\/task\.json/},
      {handler : 'crm-task', re : /crm\/task\/[\w\d-]+\.json/},
      {handler : 'crm-task', re : /crm\/task\/[\w\d-]+\/close\.json/},
      {handler : 'crm-task', re : /crm\/task\/[\w\d-]+\/reopen\.json/},
      {handler : 'crm-opportunity', re : /crm\/opportunity\/[\w\d-]+\.json/},
      {handler : 'crm-dealmilestone', re : /crm\/opportunity\/stage\/[\w\d-]+\.json/},
      {handler : 'crm-dealmilestone', re : /crm\/opportunity\/stage\.json/, method : 'post'},
      {handler : 'crm-dealmilestones', re : /crm\/opportunity\/stage\.json/, method : 'get'},
      {handler : 'crm-contactstatus', re : /crm\/contact\/type\/[\w\d-]+\.json/},
      {handler : 'crm-contactstatus', re : /crm\/contact\/type\.json/, method : 'post'},
      {handler : 'crm-contactstatuses', re : /crm\/contact\/type\.json/, method : 'get'},
      {handler : 'crm-customfield', re : /crm\/[contact|person|company|opportunity|case]+\/customfield\/[\w\d-]+\.json/},
      {handler : 'crm-customfield', re : /crm\/[contact|person|company|opportunity|case]+\/customfield\.json/},
      {handler : 'crm-customfields', re : /crm\/[contact|person|company|opportunity|case]+\/customfield\/definitions\.json/},
      {handler : 'crm-tag', re : /crm\/[case|contact|opportunity]+\/tag\.json/},
      {handler : 'crm-tag', re : /crm\/[case|contact|opportunity]+\/taglist\.json/},
      {handler : 'crm-tag', re : /crm\/[case|contact|opportunity]+\/[\w\d-]+\/tag\.json/},
      {handler : 'crm-tags', re : /crm\/[case|contact|opportunity]+\/tag\/unused\.json/},
      {handler : 'crm-tags', re : /crm\/[case|contact|opportunity]+\/tag\/[\w\d-]+\.json/},
      {handler : 'crm-tags', re : /crm\/[case|contact|opportunity]+\/tag\.json/},
      {handler : 'crm-cases', re : /crm\/case\/filter\.json/},
      {handler : 'crm-contacts', re : /crm\/contact\.json/},
      {handler : 'crm-contacts', re : /crm\/contact\/filter\.json/},
      {handler : 'crm-contacts', re : /crm\/contact\/company\/[\w\d-]+\/person\.json/, method : 'get'},
      {handler : 'crm-contacts', re : /crm\/[case|opportunity]+\/[\w\d-]+\/contact\.json/, method : 'get'},
      {handler : 'crm-contacts', re : /crm\/contact\/access\.json/},
      {handler : 'crm-contacts', re : /crm\/contact\/[\w\d-]+\/access\.json/},
      //{handler : 'crm-customfields', re : /crm\/contact\/[\w\d-]+\/access\.json/},
      {handler : 'crm-tasks', re : /crm\/task\/filter\.json/},
      {handler : 'crm-opportunities', re : /crm\/opportunity\/filter\.json/},
      {handler : 'crm-contacttasks', re : /crm\/contact\/task\/near\.json/},
      {handler : 'crm-taskcategory', re : /crm\/task\/category\/[\w\d-]+\.json/},
      {handler : 'crm-taskcategory', re : /crm\/task\/category\.json/, method : 'post'},
      {handler : 'crm-taskcategories', re : /crm\/task\/category\.json/, method : 'get'},

      {handler : 'crm-historyevent', re : /crm\/history\.json/},
      {handler : 'crm-historyevent', re : /crm\/history\/[\w\d-]+.json/},
      {handler : 'crm-historyevent', re : /crm\/[contact|opportunity|case]+\/[\w\d-]+\/files\.json/, method : 'post'},
      {handler : 'crm-historyevents', re : /crm\/history\/filter\.json/},
      {handler : 'crm-historycategory', re : /crm\/history\/category\/[\w\d-]+\.json/},
      {handler : 'crm-historycategory', re : /crm\/history\/category\.json/, method : 'post'},
      {handler : 'crm-historycategories', re : /crm\/history\/category\.json/, method : 'get'},

      {handler : 'crm-rootfolder', re : /crm\/files\/root\.json/},

      {handler : 'crm-tasktemplatecontainer', re : /crm\/[contact|person|company|opportunity|case]+\/tasktemplatecontainer\.json/},
      {handler : 'crm-tasktemplatecontainer', re : /crm\/tasktemplatecontainer\/[\w\d-]+\.json/},
      {handler : 'crm-tasktemplate', re : /crm\/tasktemplatecontainer\/[\w\d-]+\/tasktemplate\.json/},
      {handler : 'crm-tasktemplate', re : /crm\/tasktemplatecontainer\/tasktemplate\/[\w\d-]+\.json/},

      {handler : 'authentication', re : /authentication\.json/},
      {handler : 'settings', re : /settings\.json/},
      {handler : 'security', re : /settings\/security\.json/},
      {handler : 'administrator', re : /settings\/security\/administrator\.json/},
      {handler : 'quotas', re : /settings\/quota\.json/},

      {handler : 'profile', re : /people\/[\w\d-]+\.json/},
      {handler : 'isme', re : /people\/@self\.json/},
      {handler : 'profiles', re : /people\.json/},
      {handler : 'profiles', re : /people\/@search\/[\w\d-]+\.json/},
      {handler : 'group', re : /group\/[\w\d-]+\.json/},
      {handler : 'groups', re : /group\.json/},
      {handler : 'comment', re : /comment\.json/, method : 'post'},
      {handler : 'comments', re : /comment\.json/, method : 'get'}
    ];

  function isArray (o) {
   return o ? o.constructor.toString().indexOf("Array") != -1 : false;
  }

  function converText (str, toText) {
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

  function extend (src, dsc) {
    for (var fld in dsc) {
      if (dsc.hasOwnProperty(fld)) {
        src[fld] = dsc[fld];
      }
    }
    return src;
  };

  function clone (o) {
    if (!o || typeof o !== 'object') {
      return o;
    }

    var p, v, c = typeof o.pop === 'function' ? [] : {};
    for (p in o) {
      if (o.hasOwnProperty(p)) {
        v = o[p];
        if (v && typeof v === 'object') {
          c[p] = clone(v);
        } else {
          c[p] = v;
        }
      }
    }
    return c;
  }

  function collection (items, hCreate, hExtend) {
    var
      collection = [],
      itemsInd = items ? items.length : 0;

    while (itemsInd--) {
      collection.unshift(extend(hCreate ? hCreate(items[itemsInd]) : {}, hExtend(items[itemsInd])));
    }

    return collection;
  }

  function leftPad (n, m) {
    var p = '000000000';
    n = '' + n;
    if (!m) {
      return n.length === 1 ? '0' + n : n;
    }
    return n.length === m ? n : p.substring(0, m - n.length) + n;
  }

  function getResponse (response) {
    var o = null;
    if (typeof response === 'string') {
      try {
        o = jQuery.parseJSON(converText(response));
      } catch (err) {
        o = null;
      }
      if (!o || typeof o !== 'object') {
        try {
          o = jQuery.parseJSON(converText(jQuery.base64.decode(response)));
        } catch (err) {
          o = null;
        }
      }
    }
    return o;
  }

  function fixUrl (url) {  
    if (!url) {
      return '';
    }
    if (url.indexOf('://') === -1) {
      if (url.charAt(0) !== '/') {
        url = '/' + url;
      }
      url = [location.protocol, '//', location.hostname, location.port ? ':' + location.port : '', url].join('');      
    }
    return url;
  }

  function getFileType (ext) {
    var
      exts = null,
      extsInd = 0,
      types = extTypes,
      typesInd = 0;

    typesInd = types.length;
    while (typesInd--) {
      exts = types[typesInd].exts;
      extsInd = exts.length;
      while (extsInd--) {
        if (exts[extsInd] == ext) {
          return types[typesInd].name;
        }
      }
    }
    return 'unknown';
  }

  function isSupportedFileType (ext) {   
    var
      types = supportedTypes,
      typesInd = 0;

    typesInd = types.length;
    while (typesInd--) {
      if (ext == types[typesInd]) {
        return true;
      }
    }
    return false;
  }

  function getRootFolderTypeById (id) {
    var
      types = folderTypes,
      typesInd = 0;

    typesInd = types.length;
    while (typesInd--) {
      if (types[typesInd].id == id) {
        return types[typesInd].label;
      }
    }
    return types.length > 0 ? types[0].label : '';
  }


  function getSearchEntityTypeById (id) {
    var
      types = searchEntityTypes,
      typesInd = 0;

    typesInd = types.length;
    while (typesInd--) {
      if (types[typesInd].id == id) {
        return types[typesInd].label;
      }
    }
    return types.length > 0 ? types[0].label : id;
  }

  function getTaskStatusName (id) {
    var
      statuses = taskStatuses,
      statusesInd = 0;

    for (statusesInd in statuses) {
      if (statuses.hasOwnProperty(statusesInd)) {
        if (statuses[statusesInd].id == id) {
          return statuses[statusesInd].name;
        }
      }
    }
    return '';
  }

  function getContacts (items) {
    var contact = null, item = null, strcontacts = {mailboxes : [], telephones : [], links : []};
    if (!items) {
      return strcontacts;
    }

    for (var i = 0, n = items.length; i < n; i++) {
      item = items[i];
      contact = {
        type  : contactTitles.hasOwnProperty(item.type) ? contactTitles[item.type].type : -1,
        name  : item.type,
        title : item.value,
        label : contactTitles.hasOwnProperty(item.type) ? contactTitles[item.type].title : item.type,
        istop : false
      };

      switch (contact.name) {
        case contactTitles.twitter.name:
          contact.val = contact.title.indexOf('twitter.com') === -1 ? 'http://twitter.com/' + contact.title : contact.title;
          contact.title = contact.title.indexOf('twitter.com') !== -1 ? contact.title.substring(contact.title.lastIndexOf('/')) : contact.title;
          break;
        case contactTitles.facebook.name:
          contact.val = contact.title.indexOf('facebook.com') === -1 ? 'http://facebook.com/' + contact.title : contact.title;
          contact.title = contact.title.indexOf('facebook.com') !== -1 ? contact.title.substring(contact.title.lastIndexOf('/') + 1) : contact.title;
          break;
        case contactTitles.skype.name:
          contact.istop = true;
          contact.val = 'skype:' + contact.title + '?call';
        case contactTitles.jabber.name:
        case contactTitles.msn.name:
        case contactTitles.aim.name:
          break;
        case contactTitles.icq.name:
          contact.val = 'http://www.icq.com/people/' + contact.title;
          break;
        case contactTitles.yahoo.name:
        case contactTitles.gmail.name:
        case contactTitles.gtalk.name:
          contact.istop = true;
          contact.val = 'mailto:' + contact.title;
          break;
        case contactTitles.blogger.name:
          contact.val = contact.title.indexOf('blogger.com') === -1 ? 'http://' + contact.title + '.blogger.com/' : contact.title;
          break;
        case contactTitles.myspace.name:
          contact.val = contact.title.indexOf('myspace.com') === -1 ? 'http://myspace.com/' + contact.title : contact.title;
          contact.title = contact.title.indexOf('myspace.com') !== -1 ? contact.title.substring(contact.title.lastIndexOf('/') + 1) : contact.title;
          break;
        case contactTitles.livejournal.name:
          contact.val = contact.title.indexOf('livejournal.com') === -1 ? 'http://' + contact.title + '.livejournal.com/' : contact.title;
          break;
        default:
          contact.val = contact.title;
          break;
      }

      switch (contact.type) {
        // mails
        case 0 : strcontacts.mailboxes.push(contact); break;
        // tels
        case 1 : strcontacts.telephones.push(contact); break;
        // links
        case 2 : strcontacts.links.push(contact); break;
        // other
        case -1 : strcontacts.links.push(contact); break;
      }
    }

    return strcontacts;
  }

  var init = function (opts) {
    if (isInit === true) {
      return undefined;
    }
    isInit = true;

    opts = opts || {};
    if (opts.hasOwnProperty('portaldatetime') && opts.portaldatetime && typeof opts.portaldatetime === 'object') {
      portalUtcOffsetTotalMinutes = opts.portaldatetime.hasOwnProperty('utcoffsettotalminutes') ? opts.portaldatetime.utcoffsettotalminutes : portalUtcOffsetTotalMinutes;
      portalUtcOffset = portalUtcOffsetTotalMinutes != 0 ? (portalUtcOffsetTotalMinutes > 0 ? '+' : '-') + leftPad(Math.floor(Math.abs(portalUtcOffsetTotalMinutes) / 60)) + ':' + leftPad(Math.abs(portalUtcOffsetTotalMinutes) % 60) : portalUtcOffset;
      portalTimeZoneName = opts.portaldatetime.hasOwnProperty('displayname') ? opts.portaldatetime.displayname : portalTimeZoneName;
    }
    if (opts.hasOwnProperty('names') && opts.names && typeof opts.names === 'object') {
      monthNames = opts.names.hasOwnProperty('months') ? opts.names.months.split(',') : monthNames;
      monthShortNames = opts.names.hasOwnProperty('shortmonths') ? opts.names.shortmonths.split(',') : monthShortNames;
      dayNames = opts.names.hasOwnProperty('days') ? opts.names.days.split(',') : dayNames;
      dayShortNames = opts.names.hasOwnProperty('shortdays') ? opts.names.shortdays.split(',') : dayShortNames;

      nameCollections.days = dayNames;
      nameCollections.shortdays = dayShortNames;
      nameCollections.months = monthNames;
      nameCollections.shortmonths = monthShortNames;
    }

    if (opts.hasOwnProperty('avatars') && opts.avatars && typeof opts.avatars === 'object') {
      defaultAvatar = opts.avatars.hasOwnProperty('large') ? opts.avatars.large : defaultAvatar;
      defaultAvatarMedium = opts.avatars.hasOwnProperty('medium') ? opts.avatars.medium : defaultAvatarMedium;
      defaultAvatarSmall = opts.avatars.hasOwnProperty('small') ? opts.avatars.small : defaultAvatarSmall;
    }

    if (opts.hasOwnProperty('formats') && opts.formats && typeof opts.formats === 'object') {
      formatDatetime = opts.formats.hasOwnProperty('datetime') ? opts.formats.datetime : formatDatetime;
      formatDate = opts.formats.hasOwnProperty('date') ? opts.formats.date : formatDate;
      formatTime = opts.formats.hasOwnProperty('time') ? opts.formats.time : formatTime;
      if (formatTime && formatDate) {
        formatDatetime = formatTime + ' ' + formatDate;
      }

      formats.datetime = formatDatetime;
      formats.date = formatDate;
      formats.time = formatTime;
    }

    if (opts.hasOwnProperty('supportedfiles') && opts.supportedfiles && typeof opts.supportedfiles === 'object') {
      var imgs = opts.supportedfiles.hasOwnProperty('imgs') ? opts.supportedfiles.imgs || [] : [];
      supportedImgs = imgs && typeof imgs === 'string' ? imgs.split('|') : [];
      var docs = opts.supportedfiles.hasOwnProperty('docs') ? opts.supportedfiles.docs || [] : [];
      supportedDocs = docs && typeof docs === 'string' ? docs.split('|') : [];
      supportedTypes = [].concat(supportedImgs, supportedDocs);
    }

    if (opts.hasOwnProperty('responses') && opts.responses && typeof opts.responses === 'object') {
      var
        response = null,
        responses = opts.responses;
      for (var fld in responses) {
        response = responses[fld];
        if (response && (typeof response === 'object' || typeof response === 'string')) {
          response = getResponse(response);
          if (response) {
            response = response.hasOwnProperty('response') ? response.response : response;
            create(fld, 'get', response);
          }
        }
      }
    }

    if (opts.hasOwnProperty('contacttitles') && opts.contacttitles && typeof opts.contacttitles === 'object') {
      var contacttitles = opts.contacttitles;
      for (var fld in contacttitles) {
      if (contacttitles.hasOwnProperty(fld)) {
        if (contactTitles.hasOwnProperty(fld)) {
          contactTitles[fld].title = contacttitles[fld];
        }
      }
    }

    }
  };

  var fixData = function (data) {
    if (!data || typeof data !== 'object') {
      return data;
    }

    var value = null;
    for (var fld in data) {
      if (data.hasOwnProperty(fld)) {
        value = data[fld];
        switch (typeof value) {
          case 'string' :
            switch (fld.toLowerCase()) {
            case 'infotype' :
              if (contactTypes.hasOwnProperty(value)) {
                if (data.hasOwnProperty('category') && typeof data.category === 'string' && contactTypes[value].categories.hasOwnProperty(data.category)) {
                  data.category = contactTypes[value].categories[data.category].id;
                }
                value = contactTypes[value].id;
              }
              break;
            }
            break;
        }
        data[fld] = value;
      }
    }

    return data;
  };

  var serializeDate = (function () {
    if (new Date(Date.parse('1970-01-01T00:00:00.000Z')).getTime() === new Date(Date.parse('1970-01-01T00:00:00.000Z')).getTime() && false) {
      return function (d, toLocalTime) {
        if (!d) {
          return null;
        }
        var date = null, timestamp = d && typeof d === 'object' && d.hasOwnProperty('utc') && d.utc || d;
        var offset = d && typeof d === 'object' && d.hasOwnProperty('offset') && isFinite(+d.offset) && +d.offset || 0;
        if (typeof timestamp !== 'string') {
          return null;
        }

        date = new Date(Date.parse(timestamp));

        if (toLocalTime !== true && date instanceof Date) {
          date.setMinutes(date.getMinutes() - new Date().getTimezoneOffset());
        }

        return date instanceof Date ? date : null;
      };
    }
    return function (d, toLocalTime) {
      if (!d) {
        return null;
      }
      var date = null, timestamp = d && typeof d === 'object' && d.hasOwnProperty('utc') && d.utc || d;
      var offset = d && typeof d === 'object' && d.hasOwnProperty('offset') && isFinite(+d.offset) && +d.offset || 0;
      if (typeof timestamp !== 'string') {
          return null;
      }

      offset = 0;
      if (timestamp.indexOf('Z') === -1) {
        offset = timestamp.substring(timestamp.length - 5).split(':');
        offset = (+offset[0] * 60 + +offset[1]) * (timestamp.charAt(timestamp.length - 6, 1) === '+' ? 1 : -1);
      }
      date = timestamp.split('.')[0].split('T');
      date[0] = date[0].split('-');
      date[1] = date[1].split(':');

      date = new Date(
        Date.UTC(
          +date[0][0],
          +date[0][1] - 1,
          +date[0][2],
          +date[1][0],
          +date[1][1] + new Date().getTimezoneOffset(),
          +date[1][2],
          0
        )
      );

      if (toLocalTime !== true && date instanceof Date) {
        //date.setMinutes(date.getMinutes() + new Date().getTimezoneOffset());
        //date.setMinutes(date.getMinutes() + portalUtcOffsetTotalMinutes);
        //date.setMinutes(date.getMinutes() + offset);
      }

      return date instanceof Date ? date : null;
    };
  })();

  var serializeTimestamp = function (d, safeurl) {
    var timestamp = d instanceof Date ? '' + [d.getFullYear(), leftPad((d.getMonth() + 1)), leftPad(d.getDate())].join('-') + 'T' + [leftPad(d.getHours()), leftPad(d.getMinutes()), leftPad(d.getSeconds())].join(':') : '';
    return timestamp;

    var timestamp = d instanceof Date ? '' + [d.getFullYear(), leftPad((d.getMonth() + 1)), leftPad(d.getDate())].join('-') + 'T' + [leftPad(d.getHours()), leftPad(d.getMinutes()), leftPad(d.getSeconds())].join(':') + '.' + leftPad(d.getMilliseconds(), 7) + portalUtcOffset : '';
    //return safeurl === true ? timestamp.replace(/:/g, '-') : timestamp;
    return timestamp;

    return d instanceof Date ? '' + [d.getFullYear(), leftPad((d.getUTCMonth() + 1)), leftPad(d.getUTCDate())].join('-') + 'T' + [leftPad(d.getUTCHours()), leftPad(d.getUTCMinutes()), leftPad(d.getUTCSeconds())].join(':') + '.' + leftPad(d.getMilliseconds(), 7) + 'Z' : '';
    return JSON.stringify(d);
  };

  function formatingDateTerm (date, term, dayshortnames, daynames, monthshortnames, monthnames) {
    switch (term) {
      case 'd'    : return date.getDate();
      case 'dd'   : return leftPad(date.getDate());
      case 'ddd'  : return dayshortnames[date.getDay()];
      case 'dddd' : return daynames[date.getDay()];
      case 'h'    :
        var hours = date.getHours();
        return hours > 12 ? hours - 12 : hours === 0 ? 12 : hours;
      case 'hh'   :
        var hours = date.getHours();
        return leftPad(hours > 12 ? hours - 12 : hours === 0 ? 12 : hours);
        break;
      case 'H'    : return date.getHours();
      case 'HH'   : return leftPad(date.getHours());
      case 'm'    : return date.getMinutes();
      case 'mm'   : return leftPad(date.getMinutes());
      case 'M'    : return date.getMonth() + 1;
      case 'MM'   : return leftPad(date.getMonth() + 1);
      case 'MMM'  : return dayshortnames[date.getMonth()];
      case 'MMMM' : return monthnames[date.getMonth()];
      case 's'    : return date.getSeconds();
      case 'ss'   : return leftPad(date.getSeconds());
      case 't'    : return date.getHours() < 12 ? 'A' : 'P';
      case 'tt'   : return date.getHours() < 12 ? 'AM' : 'PM';
      case 'y'    : return date.getYear() - 100;
      case 'yy'   : return (date.getYear() % 100 < 10 ? '0' : '') + date.getYear() % 100;
      case 'yyy'  : return (date.getYear() % 100 < 10 ? '0' : '') + date.getYear() % 100;
      case 'yyyy' : return date.getFullYear();
    }
    return '';
  }

  var formattingDate = function (date, format, dayshortnames, daynames, monthshortnames, monthnames) {
    if (!(date instanceof Date) || date.getTime() < 0) {
      return '';
    }
    if (typeof format !== 'string' || format.length === 0) {
      return '';
    }

    var
      hours = date.getHours(),
      amhours = hours > 12 ? hours - 12 : hours === 0 ? 12 : hours,
      output = '',
      term = '',
      islit = false;
    format = format.split('');
    for (var i = 0, n = format.length; i < n; i++) {
      islit = false;
      switch (format[i]) {
        case 'd' :
        case 'h' :
        case 'H' :
        case 'm' :
        case 'M' :
        case 's' :
        case 't' :
        case 'y' :
          term += format[i];
          break;
        default :
          islit = true;
      }
      if (islit) {
        output += formatingDateTerm(date, term, dayshortnames, daynames, monthshortnames, monthnames);
        output += format[i];
        term = '';
      }
    }
    if (term) {
      output += formatingDateTerm(date, term, dayshortnames, daynames, monthshortnames, monthnames);
    }
    return output;
  };

  var getDisplayTime = function (date) {
    var displaydate = date ? date.toLocaleTimeString() : '';
    if (date && formatTime) {
      displaydate = formattingDate(date, formatTime, dayShortNames, dayNames, monthShortNames, monthNames);
    }
    return displaydate;
  };

  var getDisplayDate = function (date) {
    var displaydate = date ? date.toLocaleDateString() : '';
    if (date && formatDate) {
      displaydate = formattingDate(date, formatDate, dayShortNames, dayNames, monthShortNames, monthNames);
    }
    return displaydate;
  };

  var getDisplayDatetime = function (date) {
    var displaydate = date ? date.toLocaleTimeString() + ' ' + date.toLocaleDateString() : '';
    if (date && formatDatetime) {
      displaydate = formattingDate(date, formatDatetime, dayShortNames, dayNames, monthShortNames, monthNames);
    }
    return displaydate;
  };

  function createGroup (o) {
    if (!o) {
      return null;
    }

    var group = {
      type : 'group',
      id : o.id,
      name : o.name,
      manager : o.manager || '',
      title : o.title || o.name,
      members : o.hasOwnProperty('members') ? createPersons(o.members) : null
    };

    return group;
  }

  function createGroups (o) {
    if (!o) {
      return [];
    }

    var
      group = null,
      groups = [],
      items = isArray(o) ? o : [o],
      itemsInd = 0;

    itemsInd = items ? items.length : 0;
    while (itemsInd--) {
      group = createGroup(items[itemsInd]);
      group ? groups.unshift(group) : null;
    }
    return groups;
  }

  function createPerson (o) {
    if (!o) {
      return null;
    }
    if (o.hasOwnProperty('email') && o.hasOwnProperty('contacts')) {
      o.contacts = [{ type : 'mail', value : o.email}].concat(o.contacts);
    }

    var
      person = null,
      crtdate = serializeDate(o.created || o.workFrom || o.workFromDate),
      trtdate = serializeDate(o.terminated || o.terminatedDate),
      displayname = o.displayName || o.firstName + ' ' + o.lastName,
      contacts = getContacts(o.contacts);

    person = {
      index : displayname.charAt(0).toLowerCase(),
      type : 'person',
      id : o.id,
      timestamp : crtdate ? crtdate.getTime() : 0,
      crtdate : crtdate,
      displayCrtdate : getDisplayDatetime(crtdate),
      displayDateCrtdate : getDisplayDate(crtdate),
      displayTimeCrtdate : getDisplayTime(crtdate),
      trtdate : trtdate,
      displayTrtdate : getDisplayDatetime(trtdate),
      displayDateTrtdate : getDisplayDate(trtdate),
      displayTimeTrtdate : getDisplayTime(trtdate),
      userName : o.userName || '',
      firstName : o.firstName || '',
      lastName : o.lastName || '',
      displayName : displayname || '',
      email : o.email || '',
      tel : contacts.telephones.length > 0 ? contacts.telephones[0].val : '',
      contacts : contacts,
      avatar : o.avatar || o.avatarSmall || defaultAvatar,
      avatarSmall : o.avatarSmall || defaultAvatarSmall,
      group : createGroup(o.groups && o.groups.length > 0 ? o.groups[0] || null : null),
      groups : createGroups(o.groups || []),
      isMe : myProfile ? myProfile.id === o.id : false,
      isManager : false,
      status : o.status || 0,
      sex : o.sex || '',
      location : o.location || '',
      title : o.title || '',
      notes : o.notes || ''
    };

    return person;
  }

  function createPersons (o) {
    if (!o) {
      return [];
    }

    var
      person = null,
      persons = [],
      items = isArray(o) ? o : [o],
      itemsInd = 0;

    itemsInd = items ? items.length : 0;
    while (itemsInd--) {
      person = createPerson(items[itemsInd]);
      person ? persons.unshift(person) : null;
    }
    return persons;
  }

  var sortCommentsByTree = function (comments) {
    if (comments.length === 0) {
      return comments;
    }
    var tree = clone(comments), commentParentId = null, commentsInd = 0, ind = 0;

    commentsInd = tree ? tree.length : 0;
    while (commentsInd--) {
      commentParentId = tree[commentsInd].parentId;
      if (!tree[commentsInd].comments) {
        tree[commentsInd].comments = [];
      }
      if (commentParentId === null) {
          continue;
      }

      ind = tree.length;
      while (ind--) {
        if (tree[ind].id == commentParentId) {
          if (!tree[ind].comments) {
            tree[ind].comments = [];
          }
          tree[ind].comments.unshift(tree[commentsInd]);
          break;
        }
      }
    }

    commentsInd = tree ? tree.length : 0;
    while (commentsInd--) {
      if (tree[commentsInd].parentId !== null) {
        tree.splice(commentsInd, 1);
      }
    }

    return tree ? tree : comments;
  };

  var createComment = function (o) {
    var
      crtdate = serializeDate(o.created);
      uptdate = serializeDate(o.updated);
      createdBy = createPerson(o.createdBy);

    return {
      type : 'comment',
      id : o.id,
      parentId : o.parentId === '00000000-0000-0000-0000-000000000000' ? null : o.parentId || null,
      timestamp : crtdate ? crtdate.getTime() : 0,
      crtdate : crtdate,
      displayCrtdate : getDisplayDatetime(crtdate),
      displayDateCrtdate : getDisplayDate(crtdate),
      displayTimeCrtdate : getDisplayTime(crtdate),
      displayDatetimeCrtdate : getDisplayDatetime(crtdate),
      uptdate : uptdate,
      displayUptdate : getDisplayDatetime(uptdate),
      displayDateUptdate : getDisplayDate(uptdate),
      displayTimeUptdate : getDisplayTime(uptdate),
      displayDatetimeUptdate : getDisplayDatetime(uptdate),
      createdBy : createdBy,
      updatedBy : createPerson(o.updatedBy || o.createdBy),
      isMine : myProfile && createdBy && myProfile.id == createdBy.id || false,
      comments : [],
      text : o.text || ''
    }
  };

  var createCommentsTree = function (o) {
    var
      comments = [],
      items = isArray(o) ? o : [o],
      itemsInd = 0;

    itemsInd = items ? items.length : 0;
    while (itemsInd--) {
      comments.unshift(createComment(items[itemsInd]));
    }

    return sortCommentsByTree(comments);
  };

  var createPoll = function (o) {
    if (!o) {
      return null;
    }

    var
      max = 0, all = 0, val = 0,
      votes = [],
      items = o.votes,
      itemsInd = 0;

    itemsInd = items ? items.length : 0;
    while (itemsInd--) {
      val = items[itemsInd].votes;
      all += val
      if (max < val) {
        max = val;
      }
    }

    itemsInd = items ? items.length : 0;
    while (itemsInd--) {
      val = items[itemsInd].votes;
      votes.unshift({
        title   : items[itemsInd].name,
        count   : val,
        percent : all !== 0 ? Math.round((val * 100) / all) : 0,
        leader  : max === val
      });
    }

    return {votes : votes, voted : o.voted, fullCount : all};
  };

  var factories = {
    storageusage : function (response) {
      return response;
    },

    authentication : function (response) {
      return {
        token : response.token
      };
    },

    isme : function (response) {
      myProfile = createPerson(response);
      myProfile.isMe = true;

      return myProfile;
    },

    profile : function (response) {
      return createPerson(response);
    },

    profiles : function (response) {
      return collection(response, createPerson, function (response) {
        return {};
      });
    },

    group : function (response) {
      return createGroup(response);
    },

    groups : function (response) {
      return collection(response, createGroup, function (response) {
        return {};
      });
    },

    settings : function (response) {
      portalSettings = {
        type : 'settings',
        culture : response.culture,
        timezone : response.timezone,
        trustedDomains : response.trustedDomains,
        trustedDomainsType : response.trustedDomainsType,
        utcHoursOffset : response.utcHoursOffset,
        utcOffset : response.utcOffset
      };

      return portalSettings;
    },

    quotas : function (response) {
      portalQuotas = {
        type : 'quotas',
        storageSize : response.storageSize,
        maxFileSize : response.maxFileSize,
        usedSize : response.usedSize,
        avilibleSize : response.avilibleSize,
        storageUsage : factories.storageusage(response.storageUsage)
      };

      return portalQuotas;
    },

    comment : function (response) {
      return createComment(response);
    },

    comments : function (response) {
      return createCommentsTree(response);
    },

    searchentryitems : function (response) {
      return collection(response, null, function (response) {
        var type = getSearchEntityTypeById(response.entityType);
        switch (type) {
          case 'file' :
            return factories.doc.file(response);
          case 'task' :
          case 'subtask' :
            return factories.prj.task(response);
          case 'discussion' :
            return factories.prj.discussion(response);
          case 'milestone' :
            return factories.prj.milestone(response);
          default :
            return {
              type : type
            };
        }
      });
    }
  };

  /* community */
  factories.cmt = {
    item : function (response) {
      var
        crtdate = serializeDate(response.created),
        uptdate = serializeDate(response.updated);

      return {
        id : response.id,
        timestamp : crtdate ? crtdate.getTime() : 0,
        crtdate : crtdate,
        displayCrtdate : getDisplayDatetime(crtdate),
        displayDateCrtdate : getDisplayDate(crtdate),
        displayTimeCrtdate : getDisplayTime(crtdate),
        displayDatetimeCrtdate : getDisplayDatetime(crtdate),
        uptdate : uptdate,
        displayUptdate : getDisplayDatetime(uptdate),
        displayDateUptdate : getDisplayDate(uptdate),
        displayTimeUptdate : getDisplayTime(uptdate),
        displayDatetimeUptdate : getDisplayDatetime(uptdate),
        createdBy : createPerson(response.createdBy || response.author),
        updatedBy : createPerson(response.updatedBy || response.createdBy || response.author),
        title : response.title || '',
        text : response.preview || response.text || response.description || ''
      };
    },

    tags : function (response) {
      return response || [];
    },

    blog : function (response) {
      return extend(this.item(response), {
        type : 'blog',
        tags : factories.cmt.tags(response.tags),
        comments : null
      });
    },

    poll : function (response) {
      return extend(this.item(response), {
        type : 'poll'
        //blah-blah-blah
      });
    },

    topic : function (response) {
      var firstPost = null;
      if (response.posts && response.posts.length > 0) {
        firstPost = response.posts[0];
        response.createdBy = firstPost.createdBy;
        response.text = firstPost.text;
        response.posts = response.posts.slice(1);
      }

      return extend(this.item(response), {
        type : 'forum',
        typeCode : response.type,
        statusCode : response.status,
        tags : factories.cmt.tags(response.tags),
        threadTitle : response.threadTitle || response.threadTitile,
        posts : createCommentsTree(response.posts || [])
      });
    },

    thread : function (response) {
      return extend(this.item(response), {
        type : 'thread'
      });
    },

    category : function (response) {
      return extend(this.item(response), {
        type : 'category',
        threads : factories.cmt.threads(response.threads)
      });
    },

    event : function (response) {
      var
        item = this.item(response);

      return extend(item, {
        type : 'event',
        typeCode : response.type,
        poll : createPoll(response.poll),
        comments : null
      });
    },

    bookmark : function (response) {
      return extend(this.item(response), {
        type : 'bookmark',
        url : response.url,
        thumbnail : response.thumbnail,
        comments : null
      });
    },

    blogs : function (response) {
      return collection(response, this.item, function (response) {
        return factories.cmt.blog(response);
      });
    },

    polls : function (response) {
      return collection(response, this.item, function (response) {
        return factories.cmt.poll(response);
      });
    },

    topics : function (response) {
      return collection(response, this.item, function (response) {
        return factories.cmt.topic(response);
      });
    },

    threads : function (response) {
      return collection(response, this.item, function (response) {
        return factories.cmt.thread(response);
      });
    },

    categories : function (response) {
      response = response.hasOwnProperty('categories') ? response.categories : response;

      return collection(response, this.item, function (response) {
        return factories.cmt.category(response);
      });
    },

    events : function (response) {
      return collection(response, this.item, function (response) {
        return factories.cmt.event(response);
      });
    },

    bookmarks : function (response) {
      return collection(response, this.item, function (response) {
        return factories.cmt.bookmark(response);
      });
    }
  };

  /* projects */
  factories.prj = {
    item : function (response) {
      var
        createdBy = createPerson(response.createdBy || response.author),
        responsible = createPerson(response.responsible ? response.responsible : (response.responsibles && response.responsibles.length > 0 ? response.responsibles[0] : null)),
        crtdate = serializeDate(response.created),
        uptdate = serializeDate(response.updated);

      return {
        id : response.id,
        timestamp : crtdate ? crtdate.getTime() : 0,
        crtdate : crtdate,
        displayCrtdate : getDisplayDatetime(crtdate),
        displayDateCrtdate : getDisplayDate(crtdate),
        displayTimeCrtdate : getDisplayTime(crtdate),
        uptdate : uptdate,
        displayUptdate : getDisplayDatetime(uptdate),
        displayDateUptdate : getDisplayDate(uptdate),
        displayTimeUptdate : getDisplayTime(uptdate),
        createdBy : createdBy,
        updatedBy : createPerson(response.updatedBy),
        responsible : responsible,
        responsibles : createPersons(response.responsibles || response.responsible),
        canEdit : response.canEdit || false,
        isPrivate : response.isPrivate || false,
        isMy : createdBy && myProfile ? createdBy.id === myProfile.id : false,
        forMe : responsible && myProfile ? responsible.id === myProfile.id : false,
        title : response.title || '',
        lowTitle : (response.title || '').toLowerCase(),
        description : response.description || '',
        status : response.status
      };
    },

    task : function (response) {
      var
        dlndate = serializeDate(response.deadline),
        todaydate = new Date(),
        tomorrowdate = new Date();

      todaydate = new Date(todaydate.getFullYear(), todaydate.getMonth(), todaydate.getDate(), 0, 0, 0, 0);
      tomorrowdate = new Date(tomorrowdate.getFullYear(), tomorrowdate.getMonth(), tomorrowdate.getDate() + 1, 0, 0, 0, 0);


      return extend(this.item(response), {
        type : 'task',
        projectId : response.hasOwnProperty('projectOwner') ? response.projectOwner.id : -1,
        projectTitle : response.hasOwnProperty('projectOwner') ? response.projectOwner.title : '',
        projectOwner : response.projectOwner,
        canWork : response.canWork,
        deadline : dlndate,
        displayDeadline : getDisplayDatetime(dlndate),
        displayDateDeadline : getDisplayDate(dlndate),
        displayTimeDeadline : getDisplayTime(dlndate),
        status : response.status,
        statusname : getTaskStatusName(response.status),
        priority : response.priority,
        subtasks : factories.prj.subtasks(response.subtasks),
        milestoneId : response.milestoneId,
        milestoneTitle : response.hasOwnProperty('milestone') && response.milestone ? response.milestone.title : '',
        milestone : response.hasOwnProperty('milestone') ? factories.prj.milestone(response.milestone) : null,
        deadlineToday : dlndate ? dlndate.getTime() >= todaydate.getTime() && dlndate.getTime() < tomorrowdate.getTime() : false,
        isOpened : response.status == taskStatuses.open.id,
        isExpired : response.isExpired || false
      });
    },

    milestone : function (response) {
      var
        dlndate = serializeDate(response.deadline),
        todaydate = new Date(),
        tomorrowdate = new Date();

      todaydate = new Date(todaydate.getFullYear(), todaydate.getMonth(), todaydate.getDate(), 0, 0, 0, 0);
      tomorrowdate = new Date(tomorrowdate.getFullYear(), tomorrowdate.getMonth(), tomorrowdate.getDate() + 1, 0, 0, 0, 0);

      return extend(this.item(response), {
        type : 'milestone',
        projectId : response.hasOwnProperty('projectOwner') ? response.projectOwner.id : -1,
        projectTitle : response.hasOwnProperty('projectOwner') ? response.projectOwner.title : '',
        deadline : dlndate,
        displayDeadline : getDisplayDatetime(dlndate),
        displayDateDeadline : getDisplayDate(dlndate),
        displayTimeDeadline : getDisplayTime(dlndate),
        status : response.status,
        deadlineToday : dlndate ? dlndate.getTime() >= todaydate.getTime() && dlndate.getTime() < tomorrowdate.getTime() : false,
        isKey : response.isKey,
        isNotify : response.isNotify,
        isExpired : response.isExpired || false,
        isOpened : response.status == milestoneStatuses.open.id,
        activeTaskCount : response.activeTaskCount,
        closedTaskCount : response.closedTaskCount
      });
    },

    discussion : function (response) {
      return extend(this.item(response), {
        type : 'discussion',
        projectId : response.hasOwnProperty('projectOwner') ? response.projectOwner.id : -1,
        projectTitle : response.hasOwnProperty('projectOwner') ? response.projectOwner.title : '',
        parentId : null,
        comments : null,
        text : response.text || ''
      });
    },

    project : function (response) {
      return extend(this.item(response), {
        type : 'project',
        status : response.status,
        taskCount : response.taskCount,
        milestoneCount : response.milestoneCount,
        participantCount : response.participantCount,
        canCreateMessage: response.hasOwnProperty('security') ? response.security.canCreateMessage : '',
        canCreateMilestone: response.hasOwnProperty('security') ? response.security.canCreateMilestone : '',
        canCreateTask: response.hasOwnProperty('security') ? response.security.canCreateTask : ''
      });
    },

    projectrequest : function (response) {
      return extend(this.item(response), {
        type : 'request'
        
      });
    },
    
    activity : function (response) {
      var date = serializeDate(response.date);
      return extend(this.item(response), {
        type: 'activity',
        projectId: response.projectId,
        projectTitle: response.projectTitle,
        title: response.title,
        url: response.url,
        actionText: response.actionText,
        displayDatetime : getDisplayDatetime(date),
        displayDate : getDisplayDate(date),
        displayTime : getDisplayTime(date),
        user : response.user,
        entityType: response.entityType,
        entityTitle: response.entityTitle
      });
    },

    subtasks : function (response) {
      return collection(response, this.item, function (response) {
        return {
          type : 'subtask',
          status : response.status
        };
      });
    },

    tasks : function (response) {
      return collection(response, this.item, function (response) {
        return factories.prj.task(response);
      });
    },

    milestones : function (response) {
      return collection(response, this.item, function (response) {
        return factories.prj.milestone(response);
      });
    },

    discussions : function (response) {
      return collection(response, this.item, function (response) {
        return {
          type : 'discussion',
          projectId : response.hasOwnProperty('projectOwner') ? response.projectOwner.id : -1,
          projectTitle : response.hasOwnProperty('projectOwner') ? response.projectOwner.title : '',
          text : response.text || '',
          commentsCount: response.commentsCount
        };
      });
    },

    projects : function (response) {
      return collection(response, this.item, function (response) {
        return factories.prj.project(response);
      });
    },

    projectperson: function(response) {
        var person = createPerson(response);
        person.canReadFiles = response.canReadFiles;
        person.canReadMessages = response.canReadMessages;
        person.canReadMilestones = response.canReadMilestones;
        person.canReadTasks = response.canReadTasks;
        person.isAdministrator = response.isAdministrator;
        return person;
    },

    projectpersons : function (response) {
      return collection(response, createPerson, function (response) {
        return {};
      });
    },

    searchentries : function (response) {
      return collection(response, this.item, function (response) {
        var projectOwner = response.projectOwner;
        return {
          id : projectOwner ? projectOwner.id : response.id || -1,
          type : 'project',
          title : projectOwner ? projectOwner.title : response.title || '',
          status : projectOwner ? projectOwner.status : response.status || '',
          items : response.items ? factories.searchentryitems(response.items) : []
        };
      });
    },
    
    activities : function (response) {
      return collection(response, this.item, function (response) {
        return factories.prj.activity(response);
      });
    }
  };

  /* documents */
  factories.doc = {
    item : function (response) {
      var
        crtdate = serializeDate(response.created),
        uptdate = serializeDate(response.updated);

      return {
        id : response.id,
        parentId : response.parentId || response.folderId || -1,
        folderId : response.folderId || response.parentId || -1,
        access : response.access,
        sharedByMe : response.sharedByMe,
        rootFolderType : response.rootFolderType,
        rootType : getRootFolderTypeById(response.rootFolderType),
        timestamp : crtdate ? crtdate.getTime() : 0,
        crtdate : crtdate,
        displayCrtdate : getDisplayDatetime(crtdate),
        displayDateCrtdate : getDisplayDate(crtdate),
        displayTimeCrtdate : getDisplayTime(crtdate),
        uptdate : uptdate,
        displayUptdate : getDisplayDatetime(uptdate),
        displayDateUptdate : getDisplayDate(uptdate),
        displayTimeUptdate : getDisplayTime(uptdate),
        createdBy : createPerson(response.createdBy || response.author),
        updatedBy : createPerson(response.updatedBy),
        canEdit : response.canEdit || false,
        title : response.title || '',
        description : response.description || ''       
      };      
    },

    rootFolder : function (response) {
      if (!response || response.length <= 1) {
          return null;
      }
      var first = response[0] || {};
      return {
        type : 'folder',
        id : first.key,
        title : first.path
      };
    },

    parentFolder : function (response) {
      if (!response || response.length <= 1) {
        return null;
      }
      var last = response[response.length - 2] || null;
      return !last ? null : {
        type : 'folder',
        id : last.key,
        title : last.path,
        path : last.path
      };
    },

    files : function (response) {               
      return collection(response, this.item, function (response) {       
        return factories.doc.file(response);
      });
    },

    folders : function (response) {
      return collection(response, this.item, function (response) {       
        return factories.doc.folder(response);        
      });
    },

    file : function (response) {           
      var extension = response.title;
      extension = (extension.substring(extension.lastIndexOf('.')) || '').toLowerCase();

      var filename = response.title;
      filename = filename.substring(0, filename.lastIndexOf('.'));
      filename = filename ? filename : response.title;

      return extend(this.item(response), {
        type : 'file',
        extension : extension == response.title ? '' : extension,
        filename : filename,
        filetype : getFileType(extension),
        version : response.version,
        fileStatus : response.fileStatus,
        contentLength : response.contentLength,
        viewUrl : response.viewUrl,
        viewUrl : fixUrl(response.viewUri || response.viewUrl || ''),
        fileUrl : fixUrl(response.fileUri || response.fileUrl || ''),
        isSupported : isSupportedFileType(extension),
        isUploaded : false
      });       
    },

    folder : function (response) {            
      var
        isThirdParty = !(response.id && isFinite(+response.id)),
        uploadedfilenames = response.hasOwnProperty('__filenames') ? response.__filenames : null,
        folders = factories.doc.folders(response.folders),
        files = factories.doc.files(response.files),
        pathParts = response.pathParts,
        response = response.current || response;        
      if (uploadedfilenames) {
        var
          filesInd = 0,
          uploadedfilename = '',
          uploadedfilenamesInd = uploadedfilenames.length;
        while (uploadedfilenamesInd--) {
          uploadedfilename = uploadedfilenames[uploadedfilenamesInd];
          filesInd = files.length;
          while (filesInd--) {
            if (files[filesInd].title === uploadedfilename) {
              files[filesInd].isUploaded = true;
            }
          }
        }
      }

      return extend(this.item(response), {
        type : 'folder',
        folders : folders,
        files : files,
        pathParts : pathParts,
        filesCount : response.filesCount,
        foldersCount : response.foldersCount,
        isThirdParty : isThirdParty,
        isShareable : response.isShareable || false,
        canAddItems : response.access == fileShareTypes.None || response.access == fileShareTypes.ReadWrite,
        rootFolder : factories.doc.rootFolder(pathParts),
        parentFolder : factories.doc.parentFolder(pathParts)
      });
    }
  };

  /* crm */
  factories.crm = {
    item : function (response) {
      var
        crtdate = serializeDate(response.created),
        uptdate = serializeDate(response.updated),
        expdate = serializeDate(response.expected || response.expectedCloseDate);

      return {
        id : response.id,
        timestamp : crtdate ? crtdate.getTime() : 0,
        crtdate : crtdate,
        displayCrtdate : getDisplayDatetime(crtdate),
        displayDateCrtdate : getDisplayDate(crtdate),
        displayTimeCrtdate : getDisplayTime(crtdate),
        uptdate : uptdate,
        displayUptdate : getDisplayDatetime(uptdate),
        displayDateUptdate : getDisplayDate(uptdate),
        displayTimeUptdate : getDisplayTime(uptdate),
        expdate : expdate,
        displayExpdate : getDisplayDatetime(expdate),
        displayDateExpdate : getDisplayDate(expdate),
        displayTimeExpdate : getDisplayTime(expdate),
        createdBy : createPerson(response.createdBy || response.createBy || response.author),
        updatedBy : createPerson(response.updatedBy),
        responsible : createPerson(response.responsible),
        title : response.title || '',
        description : response.description || ''
      };
    },

    bidCurrency : function (response) {
      return response;
    },

    customfield : function (response) {
      return response;
    },

    dealmilestone : function (response) {
      return response;
    },

    category : function (response) {
      return response;
    },

    contactstatus : function (response) {
      return response;
    },

    historycategory : function (response) {
      return response;
    },

    entity : function (response) {
      return response;
    },

    company : function (response) {
      return response;
    },

    address : function (response) {
      var
        categories = ['Home', 'Postal', 'Office', 'Billing', 'Other', 'Work'],
        infoTypes = ['Phone', 'Email', 'Website', 'Skype', 'Twitter', 'LinkedIn', 'Facebook', 'Address', 'LiveJournal', 'MySpace', 'GMail', 'Blogger', 'Yahoo', 'MSN', 'ICQ', 'Jabber', 'AIM'],
        category = response.category,
        infoType = response.infoType;

      return {
        id : response.id,
        infoTypeId : response.infoType,
        infoType : isFinite(+infoType) && +infoType >= 0 && +infoType < infoTypes.length ? infoTypes[+infoType] : infoType,
        categoryId : response.category,
        category : isFinite(+category) && +category >= 0 && +category < categories.length ? categories[+category] : category,
        data : response.data,
        value : response.data,
        isPrimary : response.isPrimary
      };
    },

    contact : function (response) {
        return extend(this.item(response), {
        type : 'contact',
        contactclass : response.lastName ? 'person' : 'company',
        displayName : response.displayName,
        firstName : response.firstName,
        lastName : response.lastName,
        isCompany : response.isCompany,
        isPrivate : response.isPrivate,
        smallFotoUrl : response.smallFotoUrl,
        company : response.company ? factories.crm.company(response.company) : null,
        customFields : factories.crm.customfields(response.customFields),
        about : response.about,
        industry : response.industry,
        accessList : response.accessList,
        addresses : response.addresses,
        commonData : response.commonData,
        haveLateTasks : response.haveLateTasks || false,
        taskCount : response.taskCount,
        contactType : response.contactType,
        canEdit : response.canEdit
      });
    },

    file : function (response) {
      return extend(this.item(response), {
        type : 'file',
        title : response.title,
        folderId : response.folderId,
        version : response.version,
        fileStatus : response.fileStatus,
        contentLength : response.contentLength,
        viewUrl : response.viewUrl,
        access : response.access,
        sharedByMe : response.sharedByMe,
        rootFolderType : response.rootFolderType,
        createdDate : serializeDate(response.created)
      });
    },

    files : function (response) {
      return collection(response, this.item, function (response) {
        return factories.crm.file(response);
      });
    },

    tag : function (response) {
      return response;
    },

    task : function (response) {
      var deadLine = serializeDate(response.deadLine);
      var deadLineString = deadLine && (deadLine.getHours() == 0 && deadLine.getMinutes() == 0 && deadLine.getSeconds() == 0) ? getDisplayDate(deadLine) : getDisplayDatetime(deadLine);

      return extend(this.item(response), {
        type : 'task',
        category : factories.crm.category(response.category),
        canEdit : response.canEdit,
        canWork : response.canWork,
        contact : response.contact ? factories.crm.contact(response.contact) : null,
        deadLine : deadLine,
        deadLineString : deadLineString,
        entity : response.entity ? factories.crm.entity(response.entity) : null,
        isClosed : response.isClosed
      });
    },

    opportunity : function (response) {
      var expectedCloseDate = serializeDate(response.expectedCloseDate);
      var actualCloseDate = serializeDate(response.actualCloseDate);

      return extend(this.item(response), {
        type : 'opportunity',
        accessList : response.accessList,
        actualCloseDate : actualCloseDate,
        actualCloseDateString : getDisplayDate(actualCloseDate),
        bidCurrency : factories.crm.bidCurrency(response.bidCurrency),
        bidType : response.bidType,
        bidValue : response.bidValue,
        contact : response.contact ? factories.crm.contact(response.contact) : null,
        customFields : factories.crm.customfields(response.customFields),
        stage : factories.crm.dealmilestone(response.stage),
        successProbability : response.successProbability,
        expectedCloseDate : expectedCloseDate,
        expectedCloseDateString : getDisplayDate(expectedCloseDate),
        isPrivate : response.isPrivate,
        perPeriodValue : response.perPeriodValue
      });
    },

    taskcategory : function (response) {
      return extend(this.item(response), {
        type : 'taskcategory',
        imagePath : response.imagePath,
        relativeItemsCount : response.relativeItemsCount,
        sortOrder : response.sortOrder
      });
    },

    cases : function (response) {
      return collection(response, this.item, function (response) {
        return {
          type : 'case',
          accessList : response.accessList,
          isClosed : response.isClosed,
          isPrivate : response.isPrivate
        };
      });
    },

    tags : function (response) {
      return response;
    },

    contacts : function (response) {
      return collection(response, this.item, function (response) {
        return factories.crm.contact(response);
      });
    },

    customfields : function (response) {
      return collection(response, this.item, function (response) {
        return factories.crm.customfield(response);
      });
    },

    customfields : function (response) {
      return collection(response, this.item, function (response) {
        return factories.crm.customfield(response);
      });
    },

    dealmilestones : function (response) {
      return collection(response, this.item, function (response) {
        return factories.crm.dealmilestone(response);
      });
    },

    contactstatuses : function (response) {
      return collection(response, this.item, function (response) {
        return factories.crm.contactstatus(response);
      });
    },

    historycategories : function (response) {
      return collection(response, this.item, function (response) {
        return factories.crm.historycategory(response);
      });
    },

    tasks : function (response) {
      return collection(response, this.item, function (response) {
        return factories.crm.task(response);
      });
    },

    opportunities : function (response) {
      return collection(response, this.item, function (response) {
        return factories.crm.opportunity(response);
      });
    },

    contacttasks : function (response) {
      var contacts = [];
      for (var fldInd in response) {
        if (response.hasOwnProperty(fldInd)) {
          contacts.push(factories.crm.task(response[fldInd]));
        }
      }
      return contacts;
    },

    taskcategories : function (response) {
      return collection(response, this.item, function (response) {
        return factories.crm.taskcategory(response);
      });
    },

    historyevent : function (response) {
      return extend(this.item(response), {
        type : 'event',
        createdDate : getDisplayDate(serializeDate(response.created)),
        canEdit : response.canEdit,
        category : factories.crm.category(response.category),
        contact : response.contact ? factories.crm.contact(response.contact) : null,
        content : response.content,
        entity : response.entity ? factories.crm.entity(response.entity) : null,
        files : response.files && response.files.length != 0 ? factories.crm.files(response.files) : null
      });
    },

    historyevents : function (response) {
      return collection(response, this.item, function (response) {
        return factories.crm.historyevent(response);
      });
    },

    rootfolder : function (response) {
      return {
        id : response
      };
    }
  };

  var create = function (apiurl, method, response, responses) {
    if (!response) {
        return null;
    }

    responses = responses || [response];
    apiurl = typeof method === 'string' ? apiurl.toLowerCase() : apiurl;
    method = typeof method === 'string' ? method.toLowerCase() : '';
    var apiAnchorsInd = apiAnchors.length;
    while (apiAnchorsInd--) {
      if (apiAnchors[apiAnchorsInd].re.test(apiurl)) {
        //console.log(apiurl, ' # ', apiAnchors[apiAnchorsInd].handler);
        if (!apiAnchors[apiAnchorsInd].hasOwnProperty('method')) {
          break;
        }
        if (apiAnchors[apiAnchorsInd].method === method) {
          break;
        }
      }
    }

    var
      handlername = apiAnchorsInd !== -1 ? apiAnchors[apiAnchorsInd].handler : apiurl,
      namespaces = handlername.split('-');

    if (namespaces.length > 1) {
      if (factories.hasOwnProperty(namespaces[0]) && typeof factories[namespaces[0]][namespaces[1]] === 'function') {
        return factories[namespaces[0]][namespaces[1]].apply(factories[namespaces[0]], [response, responses]);
      }
    }

    if (typeof factories[handlername] === 'function') {
      return factories[handlername].apply(factories, [response, responses]);
    }

    return onlyFactory === true ? null : response;
  };

  return {
    dateFormats : formats,
    contactTypes : contactTypes,
    nameCollections : nameCollections,

    init  : init,

    create : create,

    fixData             : fixData,
    formattingDate      : formattingDate,
    serializeDate       : serializeDate,
    serializeTimestamp  : serializeTimestamp,
    getDisplayTime      : getDisplayTime,
    getDisplayDate      : getDisplayDate,
    getDisplayDatetime  : getDisplayDatetime
  };
})()
