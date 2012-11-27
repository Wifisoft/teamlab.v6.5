ASC.Projects.AdvansedFilter = (function() {
  var q=0;  
  var initialisation = function() {
    ASC.Controls.AnchorController.bind(/^(.+)*$/, onFilter);
  }
  
  var setFilterByURL = function(paramsList, getOnly) {  
    var person = jq.getAnchorParam('selected-person', paramsList),
        group = jq.getAnchorParam('selected-group', paramsList),
        status = jq.getAnchorParam('status', paramsList),
        noresponsible = jq.getAnchorParam('noresponsible', paramsList),        
        my = jq.getAnchorParam('my', paramsList),
        //unresponsible = jq.getAnchorParam('unresponsible', paramsList),
        myprojects = jq.getAnchorParam('myprojects', paramsList),
        mymilestones = jq.getAnchorParam('mymilestones', paramsList),
        milestone = jq.getAnchorParam('milestone', paramsList),
        project = jq.getAnchorParam('project', paramsList),
        deadline = jq.getAnchorParam('deadline', paramsList),
        text = jq.getAnchorParam('text', paramsList),                
        tag = jq.getAnchorParam('tag', paramsList),
        sortBy = jq.getAnchorParam('sortBy', paramsList),
        sortOrder = jq.getAnchorParam('sortOrder', paramsList);
    filters = [];
    sorters = [];            
    if (person.length>0) {
      filters.push({type : 'person', id : 'selected-person', params : {id : person}});
    } else {
      filters.push({type : 'person', id : 'selected-person', reset : true});
    }    
    if (my.length>0) {
      filters.push({type : 'person', id : 'my', isset : true, params : {id : serviceManager.getMyGUID()}});
    } else {
      filters.push({type : 'person', id : 'my', reset : true});
    }           
    if (group.length>0) {
      filters.push({type : 'group', id : 'selected-group', params : {id : group}});
    } else {
      filters.push({type : 'group', id : 'selected-group', reset : true});
    }                               
    if (status.length>0) { 
      filters.push({type : 'combobox', id : status, params : {value : status}});
    } else {
      filters.push({type : 'combobox', id : status, reset : true});
    }
    if (noresponsible.length>0) {
      filters.push({type : 'flag', id : 'noresponsible', isset : true, params : {}});
    } else {
      filters.push({type : 'flag', id : 'noresponsible', reset : true});
    }
    if (myprojects.length > 0) {
        filters.push({ type: 'flag', id: 'myprojects', isset: true, params: {} });
    } else {
        filters.push({ type: 'flag', id: 'myprojects', reset: true });
    }       
    if (mymilestones.length > 0) {
        filters.push({ type: 'flag', id: 'mymilestones', isset: true, params: {} });
    } else {
        filters.push({ type: 'flag', id: 'mymilestones', reset: true });
    }    
    /*if (unresponsible.length>0) {
      filters.push({type : 'flag', id : 'unresponsible', isset : true, params : {}});
    } else {
      filters.push({type : 'flag', id : 'unresponsible', reset : true});
    } */
    if (project.length>0) {
      filterOptions.milestoneStatus(true);
      filters.push({type : 'combobox', id : 'project', params : {value : project}});
    } else {
      filters.push({type : 'combobox', id : 'project', reset : true});
    }            
    if (milestone.length>0) {
      filters.push({type : 'combobox', id : 'milestone', params : {value : milestone}});
    } else {
      filters.push({type : 'combobox', id : 'milestone', reset : true});
    }
    if (deadline.length>0) {
      if (deadline.indexOf(',' === false)) {
        filters.push({type : 'combobox', id : deadline, params : {value : deadline}});
      } else {
        filters.push({type : 'daterange', id : 'deadline', params : {from : deadline.substr(0,deadline.indexOf(',')), to : deadline.substr(deadline.indexOf(',')+1)}});
      }
    } else {         
      filters.push({type : 'daterange', id : 'deadline', reset : true});
      filters.push({type : 'combobox', id : 'today', reset : true});
      filters.push({type : 'combobox', id : 'upcoming', reset : true});
      filters.push({type : 'combobox', id : 'overdue', reset : true});
    }    
    if (text.length>0) {
      filters.push({type : 'text', id : 'text', isset : true, params : {value : text}});
    } else {
      filters.push({type : 'text', id : 'text', params : {value : ''}});
    }
    if (tag.length>0) {
      filters.push({type : 'combobox', id : 'tag', isset : true, params : {value : tag}});
    } else {
      filters.push({type : 'combobox', id : 'tag', reset : true});
    }                                            
    if (sortBy.length>0) {      
      sorters.push({type : 'sorter', id : sortBy, selected : true});
    }
    if (sortOrder.length>0) {      
      sorters.push({type : 'sorter', id : sortBy, selected : true, sortOrder : sortOrder});
    }                   
    
    if (typeof getOnly!='undefined' && getOnly) {
      return {filters : filters, sorters : sorters};
    } else {
      jq('#AdvansedFilter').advansedFilter({filters : filters, sorters : sorters});
    }          
  }  
                                                                  
  var onFilter = function(params) {  
    if (anchorMoving) {
      setFilterByURL(params);              
    } else {
      anchorMoving = true;
    }
  }
  
  function equalData(obj1, obj2) {
  	for (var key in obj1) {
  	  if (key != 'Count' && key != 'StartIndex') {
    	  if (typeof obj2[key] == 'undefined') {return false;}
    	  if (obj1[key] != obj2[key]) {return false;}
    	}
  	}  
  	return true;
  }        
  
  var makeData = function ($container, type, storage) {
    var data = {}, anchor = '', filters = $container.advansedFilter();
    if (typeof storage != 'undefined') if (storage.length) {
      for (var filterInd = 0; filterInd < filters.length; filterInd++) {
        if (filters[filterInd].id == 'sorter') {                                                     
          data.sortBy = filters[filterInd].params.id;
          data.sortOrder = filters[filterInd].params.sortOrder; 
          anchor = jq.addParam(anchor, 'sortBy', data.sortBy);                    
          anchor = jq.addParam(anchor, 'sortOrder', data.sortOrder);       
        }      
      }
      filters = storage;
    }                    
    if (typeof serviceManager.projectId == 'string') {
      data.projectId = serviceManager.projectId;
    }
                              
    for (var filterInd = 0; filterInd < filters.length; filterInd++) {
      switch (filters[filterInd].id) {
        case 'selected-person':
          data.participant = filters[filterInd].params.id;
          anchor = jq.changeParamValue(anchor, 'selected-person', data.participant);  
          break;
        case 'my':
          data.participant = filters[filterInd].params.id;
          if (filters[filterInd].params.id == serviceManager.getMyGUID()) {          
            anchor = jq.changeParamValue(anchor, 'my', 'true'); 
          } else {
            anchor = jq.changeParamValue(anchor, 'selected-person', data.participant);
          }
          break;  
        case 'selected-group':
          data.departament = filters[filterInd].params.id; 
          anchor = jq.changeParamValue(anchor, 'selected-group', data.departament);
          break;
        /*case 'unresponsible':
          data.status = '4';
          anchor = jq.changeParamValue(anchor, 'unresponsible', 'true'); 
          break;*/                                            
        case 'status':                  
          data.status = filters[filterInd].params.value;
          anchor = jq.changeParamValue(anchor, 'status', data.status);           
          break;          
        case 'open':
          data.status = filters[filterInd].params.value;
          anchor = jq.changeParamValue(anchor, 'status', data.status);           
          break;          
        case 'closed':
          data.status = filters[filterInd].params.value;
          anchor = jq.changeParamValue(anchor, 'status', data.status);           
          break;
        case 'noresponsible':          
          data.participant = '00000000-0000-0000-0000-000000000000';
          anchor = jq.changeParamValue(anchor, 'noresponsible', 'true');                    
          break;                          
        case 'project':               
          data.projectId = filters[filterInd].params.value;                  
          anchor = jq.changeParamValue(anchor, 'project', data.projectId);
          break;
        case 'myprojects':
          data.myprojects = 'true';
          anchor = jq.changeParamValue(anchor, 'myprojects', 'true');
          break;   
        case 'mymilestones':
          data.mymilestones = 'true';
          anchor = jq.changeParamValue(anchor, 'mymilestones', 'true');
          break;          
        case 'milestone':        
          data.milestone = filters[filterInd].params.value;          
          anchor = jq.changeParamValue(anchor, 'milestone', data.milestone); 
          break;
        case 'deadline':
          data.deadlineStart = Teamlab.serializeTimestamp(new Date(filters[filterInd].params.from)); 
          data.deadlineStop = Teamlab.serializeTimestamp(new Date(filters[filterInd].params.to));
          anchor = jq.changeParamValue(anchor, 'deadline', filters[filterInd].params.from + ',' + filters[filterInd].params.to);
          break;
        case 'overdue':
          var now = new Date();
          switch (filters[filterInd].params.value) {
            case 'overdue':
              data.deadlineStop = Teamlab.serializeTimestamp(new Date(now.getYear()+1900, now.getMonth(), now.getDate()));
              break;            
            case 'today':
              data.deadlineStart = Teamlab.serializeTimestamp(new Date(now.getYear() + 1900, now.getMonth(), now.getDate()));
              data.deadlineStop = Teamlab.serializeTimestamp(new Date(now.getYear() + 1900, now.getMonth(), now.getDate(), 23, 59, 59));
              break;
            case 'upcoming':
              data.deadlineStart = Teamlab.serializeTimestamp(new Date(now.getYear() + 1900, now.getMonth(), now.getDate(), 23, 59, 59));
              break;                            
          }
          anchor = jq.changeParamValue(anchor, 'deadline', filters[filterInd].params.value); 
          break;          
        case 'today':            
          var now = new Date();
          switch (filters[filterInd].params.value) {
            case 'overdue':
              data.deadlineStop = Teamlab.serializeTimestamp(new Date(now.getYear()+1900, now.getMonth(), now.getDate()));
              break;            
            case 'today':
              data.deadlineStart = Teamlab.serializeTimestamp(new Date(now.getYear() + 1900, now.getMonth(), now.getDate()));
              data.deadlineStop = Teamlab.serializeTimestamp(new Date(now.getYear() + 1900, now.getMonth(), now.getDate(), 23, 59, 59));
              break;
            case 'upcoming':
              data.deadlineStart = Teamlab.serializeTimestamp(new Date(now.getYear() + 1900, now.getMonth(), now.getDate(), 23, 59, 59));
              break;                            
          }
          anchor = jq.changeParamValue(anchor, 'deadline', filters[filterInd].params.value); 
          break;
        case 'upcoming':
          var now = new Date();
          switch (filters[filterInd].params.value) {
            case 'overdue':
              data.deadlineStop = Teamlab.serializeTimestamp(new Date(now.getYear()+1900, now.getMonth(), now.getDate()));
              break;            
            case 'today':
              data.deadlineStart = Teamlab.serializeTimestamp(new Date(now.getYear() + 1900, now.getMonth(), now.getDate()));
              data.deadlineStop = Teamlab.serializeTimestamp(new Date(now.getYear() + 1900, now.getMonth(), now.getDate(), 23, 59, 59));
              break;
            case 'upcoming':
              data.deadlineStart = Teamlab.serializeTimestamp(new Date(now.getYear() + 1900, now.getMonth(), now.getDate(), 23, 59, 59));
              break;                            
          }
          anchor = jq.changeParamValue(anchor, 'deadline', filters[filterInd].params.value); 
          break;                    
        case 'text':
          data.FilterValue = filters[filterInd].params.value;
          anchor = jq.changeParamValue(anchor, 'text', data.FilterValue); 
          break;      
        case 'tag':          
          data.tag = filters[filterInd].params.value;
          anchor = jq.changeParamValue(anchor, 'tag', data.tag); 
          break;                                                         
        case 'sorter':
          data.sortBy = filters[filterInd].params.id;
          data.sortOrder = filters[filterInd].params.sortOrder;
          anchor = jq.changeParamValue(anchor, 'sortBy', data.sortBy);                    
          anchor = jq.changeParamValue(anchor, 'sortOrder', data.sortOrder);              
          break;            
      }
    } 
    if (type == 'anchor') {
      return anchor;
    } else {
      return data;
    } 
  }     
  
  var getTaskList = function (data) {            
    serviceManager.getFilteredTasks('tasks', { mode: 'onset' }, data);                
    LoadingBanner.displayLoading();        
  }  
  
  var init = false,
      anchorMoving = false,  
      basePath = '#sortBy=deadline&sortOrder=ascending',
      lastData = {'null':'null'},
      currentProjectId = 0,
      loadingTimeout = false, //don`t send request too often
      readyStatus = 3,
      loadStatus = 0; //nothing loaded: 0, tags xor projects loaded: 1, both tags and projects loaded: 2;         
  
  return {
    init : init, 
    initialisation : initialisation,
    setFilterByURL : setFilterByURL, 
    basePath : basePath,   
    loadStatus : loadStatus,
    readyStatus : readyStatus,
    onSetFilter: function(evt, $container, filter, params) {
      //console.log($container.advansedFilter());
      if (!init) {
        var margin = jq(".advansed-filter-container").css("marginLeft");
        jq(".presetContainer").css("marginLeft", margin);
        jq(".presetContainer").show();      
      }      
      data = makeData($container, 'data');
      path = ASC.Controls.AnchorController.getAnchor();            
      storage = jq('#AdvansedFilter').advansedFilter('storage');      
      if (!init && (path == basePath || !path.length)) {      
        anchorMoving = false;
        if (storage.length && !(storage.length==1 && storage[0].id=='sorter')) {
          path = makeData($container, 'anchor', storage);          
        } else {
          path = makeData($container, 'anchor') + '&my=true';        
        }                       
        ASC.Controls.AnchorController.move(path);
        setFilterByURL(path);        
        init = true;                                
      } else if (!(init || makeData($container, 'anchor') == path)) {      
        if (path == basePath || !path.length) {                
          getTaskList(data);
          anchorMoving = false;
          path = makeData($container, 'anchor'); 
          ASC.Controls.AnchorController.move(path);        
        }                              
        setFilterByURL(path);
        init = true;                     
      } else {                               
        getTaskList(data);
        anchorMoving = false;
        ASC.Controls.AnchorController.move(makeData($container, 'anchor'));                        
        init = true;
      } 
    },      
    onResetFilter: function (evt, $container, filter) {
      path = makeData($container, 'anchor', []);
      ASC.Controls.AnchorController.move(path);
      serviceManager.StartIndex = 0;
      serviceManager.getFilteredTasks('tasks', {mode:'onreset'}, makeData($container, 'data'));
      LoadingBanner.displayLoading();      
    }
  };        
})();  

jq(function() {
  ASC.Projects.AdvansedFilter.initialisation();   
});
