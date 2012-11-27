var TreeNodePro = function(id, parentID, value, text, startSelected)
{   
    this.ID = id;
    this.ParentID = parentID;
    this.Value = value;
    this.Text = text;
    if(startSelected=='undefined' || startSelected==undefined || startSelected==null || startSelected==true)
        this.StartSelected = startSelected;
    else    
        this.StartSelected = false;
    
    this.Children = new Array();
};

var TreeViewPrototype = function(varName, id, nodeCSSClass, selectNodeCSSClass) 
{
    this.ID = id;    
    
    var _varName = varName;
    var _selectedNodeID ='';
    
    this.SelectNodeCSSClass = selectNodeCSSClass;
    
    this.NodeCSSClass = nodeCSSClass;
    
    this.CheckedNodes = new Array();
    
    this.Selected = null;
    
    this.Nodes = new Array();
    
    this.OnSelected = null;
    
    this.OnChecked = null;
    
    this.ExpandCollapseNode = function(nodeID)
    {
        if(jq('#childs_'+nodeID).css('display')=='none')
        {
            jq('#childs_'+nodeID).show();
            jq('#minus_'+nodeID).hide();
            jq('#plus_'+nodeID).show();
        }
        else
        {            
            jq('#childs_'+nodeID).hide();
            jq('#minus_'+nodeID).show();
            jq('#plus_'+nodeID).hide();
        }
    };
    
    this.SelectNode = function(nodeID, withEvent)
    {          
        if(_selectedNodeID!='')        
            jq("#"+_selectedNodeID).attr('class', this.NodeCSSClass);                            
        
        _selectedNodeID = nodeID;
        jq("#"+_selectedNodeID).attr('class', this.SelectNodeCSSClass);
        window.open(jq("#action_"+nodeID).val(),'_self')
        
        if(withEvent=='undefined' || withEvent==undefined || withEvent==null || withEvent==true)
            if(this.OnSelected!=null)        
                this.OnSelected(nodeID);
    };
    
    this.RegistryNode = function(treeNode)
    {
        for(var i=0; i<this.Nodes.length; i++)
        {   
            if(RegNode(this.Nodes[i],treeNode))
                return;           
            
        }
        this.Nodes.push(treeNode);   
        if(treeNode.StartSelected)
        {
            _selectedNodeID = treeNode.ID;            
        }
    };
    
    //private
    var RegNode = function(node, newNode)
    {
        if(node.ID == newNode.ParentID)
        {
            node.Children.push(newNode);
            if(newNode.StartSelected)
                _selectedNodeID = newNode.ID;
                
            return true;
        }
        else
        {
            for(var i=0; i<node.Children.length; i++)
            {   
                if(RegNode(node.Children[i],newNode))
                    return true;
            }            
        }
        return false;        
    };
    
    this.GetNodeByID = function(nodeID)
    {    
        return this.GetChildNodeByID(this.Nodes, nodeID);        
    };
    
    this.GetChildNodeByID = function(nodes, nodeID)
    {       
        for(var i=0; i<nodes.length; i++)
        {
            if(nodes[i].ID == nodeID)
                return nodes[i];
                
            var node = this.GetChildNodeByID(nodes[i].Children,nodeID);
            if(node != null)
                return node;            
        }      
        
        return null;        
    };
    
    this.ExecFuncForNode = function(node, func)
    {
        func(node);
        for(var i=0; i<node.Children.length; i++)        
            this.ExecFuncForNode(node.Children[i],func);
    };
    
    this.CheckParentNodeState = function(node)
    {   
        if(node.ParentID=='')
            return;            
            
        var isParentChecked = jq('#node_state_'+node.ParentID).is(':checked');
        var isChecked = jq('#node_state_'+node.ID).is(':checked');       
       
        
        if(!isParentChecked && !isChecked)
            return;
        
        var parent = this.GetNodeByID(node.ParentID);
        if(isParentChecked && !isChecked)
        {
            jq('#node_state_'+node.ParentID).attr('checked',false);
            this.CheckParentNodeState(parent);
        }
        else if(!isParentChecked && isChecked)
        {   
            for(var i=0; i<parent.Children.length; i++)
            {
                if(jq('#node_state_'+parent.Children[i].ID).is(':checked')==false)
                    return;
            }
               
            jq('#node_state_'+node.ParentID).attr('checked',true);
            this.CheckParentNodeState(parent);
        }
        
    };  
    
    this.GetNodesByValue = function(value, nodes)
    {    
        if(nodes=='undefined' || nodes==undefined || nodes==null)        
            nodes = this.Nodes;        
            
        var findNodes = new Array();
        for(var i=0; i<nodes.length; i++)
        {
            if(nodes[i].Value == value)
               findNodes.push(nodes[i]);
                
            findNodes = findNodes.concat(this.GetNodesByValue(value, nodes[i].Children));            
        }        
        return findNodes;
    };
    
    this.UncheckAllNodes = function()
    {       
        for(var i=0; i<this.Nodes.length; i++)
        {        
            this.ExecFuncForNode(this.Nodes[i],function(n){
                        jq('#node_state_'+n.ID).attr('checked',false);
            });  
        }
        
        //jq(':checkbox[id^="node_state_"]').attr('checked',false);
    };
        
    this.CheckNode = function(nodeID, state, withEvent)
    {
        var node = this.GetNodeByID(nodeID);        
        if(node ==null)
            return;
            
        if(state=='undefined' || state==undefined || state==null)
        {
            if(jq('#node_state_'+node.ID).is(':checked'))
            {
                this.ExecFuncForNode(node,function(n){
                    jq('#node_state_'+n.ID).attr('checked',true);                
                });         
            }
            else
            {
                this.ExecFuncForNode(node,function(n){
                    jq('#node_state_'+n.ID).attr('checked',false);
                });
            }
        }
        else
        {
            this.ExecFuncForNode(node,function(n){
                    jq('#node_state_'+n.ID).attr('checked',state);
            });  
        }  
        
        this.CheckParentNodeState(node);        

        if(withEvent=='undefined' || withEvent==undefined || withEvent==null || withEvent==true)
            if(this.OnChecked!=null)
                this.OnChecked(nodeID);
    };
    
    this.GetChecked = function()
    {
        var GetCheckedChildren = function(node)
        {           
            var checkedNodes = new Array();
            if(jq('#node_state_'+node.ID).is(':checked'))
                checkedNodes.push(node);
                
            for(var i=0; i<node.Children.length; i++)        
                checkedNodes = checkedNodes.concat(GetCheckedChildren(node.Children[i]));
                
            return checkedNodes;
        };
        
        
        this.CheckedNodes = new Array();
        for(var i=0; i<this.Nodes.length; i++)
        {
            this.CheckedNodes = this.CheckedNodes.concat(GetCheckedChildren(this.Nodes[i]));
        }        
        return this.CheckedNodes;
    };
    
    this.GetSelected = function()
    {
        if(_selectedNodeID!='')        
            this.Selected = this.GetNodeByID(_selectedNodeID);        
        else
            this.Selected = null;
        
            
        return this.Selected;
    };
}