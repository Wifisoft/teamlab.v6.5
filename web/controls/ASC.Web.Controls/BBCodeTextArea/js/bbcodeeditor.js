var IsDocumentReadyForBBCodeEditor = false;
jq(document).ready(function(){   
   IsDocumentReadyForBBCodeEditor = true;   
});

var BBCodeTextAreaEditorPrototype = function(bbcodeEditorVarName, textareaID) 
{
    this.VarName = bbcodeEditorVarName;
    this.InsField = null;
    
    if(IsDocumentReadyForBBCodeEditor)
    {
        if(jq('#'+textareaID).length>0)
             eval(bbcodeEditorVarName+".InsField = document.getElementById(textareaID);");     
    }
    else
    {
        jq(document).ready(function(){
           if(jq('#'+textareaID).length>0)
             eval(bbcodeEditorVarName+".InsField = document.getElementById(textareaID);");
        });
    }
    
    this.InsBeg=1;        
    this.BrkL="[";
    this.BrkR="]";
    this.Selted='';
    
    jq(document).mouseup(function(){
         if(self.opera)str=document.getSelection();
    });   
    
    this.InsPic = function(s1,s2,s3)
    {
        
        if(!document.all&&this.Selted==''&&s3==3)
          {s1=s2;s2=this.BrkL+'/'+s2+this.BrkR;s3='';}
          
        if(s3==2||s3==3){s1+=s2;s2='';}
            s1=this.BrkL+s1+(s2==this.BrkR?'=':this.BrkR); //'[b]' or '[b='
        var isPic=s2==null;  //insert picture: cursor to end
        var s=this.InsField.value;
        var insPosL,insPosR;
        
        if(this.InsField.selectionEnd==null)
        {   var ch=0; //IE
            if(document.selection&&document.selection.createRange)
            {
              var tR=document.selection.createRange();
              var ch='character';
              var tR1=document.body.createTextRange();
            }
           
          if(!ch||tR.parentElement&&tR.parentElement()!=this.InsField)
          {
                insPosL=insPosR=s.length;         
          }
          else
          {         
            
            insPosL=tR.text.length;
            if(this.InsField.type=='textarea')
            {
                  tR1.moveToElementText(this.InsField);
                  tR.setEndPoint('StartToStart',tR1);
                  insPosR=tR.text.length;
            }
            else
            {
                tR.moveStart('textedit',-1);
                insPosR=tR.text.length;
            }
            insPosL=insPosR-insPosL;  
          }
        }
        else
        {
         
          insPosL=this.InsField.selectionStart;
          insPosR=this.InsField.selectionEnd;
          if(this.InsBeg&&self.opera&&!insPosL&&!insPosR)
            {insPosL=insPosR=s.length;this.InsBeg=0;}
         
        }
        var insText=s.substring(insPosL,insPosR);
        
        if((isInSel=this.Selted==insText)&&s3==3)
          {isInSel=insText.length;insText='';}
        
        if(document.all)this.InsField.defaultValue=s;
        else defa=s;
        if(isPic&&!(s3==2&&insText!='')){s2=s1;s1='';} //picture
        
        this.InsField.value=s.substring(0,insPosL)+s1+insText+s2+s.substring(insPosR,s.length);
        if(isInSel&&s3==3)insPosR-=isInSel;
        var insCursor=insPosR+s1.length+(isPic||insPosL!=insPosR?s2.length:0);
       
        /*for selectional cursor*/var insCursorL=insCursor;
        if(s3==1){insCursorL=insPosL+s1.length;
          insCursor=s1.length+insPosR;}//end "for"
        
        if(document.body.createTextRange){setTimeout( //IE
        "var t="+this.VarName+".InsField.createTextRange();t.collapse();t.moveEnd('"
          +ch+"',"+insCursor+");t.moveStart('"+ch+"',"+insCursorL
          +");t.select();",1);  
        }else{  if(document.all)this.InsField.focus(); //FF,Opera
          if(this.InsField.selectionEnd!=null){
            this.InsField.selectionStart=insCursorL;
            this.InsField.selectionEnd=insCursor
              +(document.all?1:0) 
            setTimeout(this.VarName+".InsField.focus();",111);
          if(document.all){var tR=document.selection.createRange();
            if(insCursorL==insCursor)tR.collapse();
            else tR.moveEnd('character',-1);tR.select();}
        }}
    };
    
    this.InsTag = function(s,c)
    {
        this.InsPic(s,(c?c:'')+this.BrkL+'/'+s+this.BrkR +(c?', ':''),c?2:null);
    };   //'b','[/b]' | 'c[/b], '
    
    this.InsTagSel = function(s)
    {   
        this.InsPic(s,this.BrkL+'/'+s+this.BrkR,1);
    };
       //'b','[/b]',1
    this.InsTagArg = function(s)
    {
        this.InsPic(s,this.BrkR);
    };  //'b',']'
    
    this.InsTagArgW = function(s)
    {   
       this.InsPic(s+'=',this.BrkL+'/'+s+this.BrkR,1);
    };
    
    this.InsTagArgURL = function(s)
    {
        this.InsPic(s+'="http://"',this.BrkL+'/'+s+this.BrkR,1);
    };
    
    this.InsTagArgQuote = function (s)
    {
        this.InsPic(s+'=""',this.BrkL+'/'+s+this.BrkR,1);        
    };
    
    this.InsTagArgWPre = function(s,pre)
    {
        this.InsPic(s+'='+pre,this.BrkL+'/'+s+this.BrkR,1);    
    };
    
    this.InsBack = function()
    {
        with(this.InsField)
        {
            var s=document.all?value:defa;
            value=document.all?defaultValue:defa;
            if(document.all)defaultValue=s;else defa=s;
        }
    };
    
    this.InsSmile = function(s)
    {
         this.BrkL="";
         this.BrkR="";
         this.InsPic(s);

         this.BrkL="[";
         this.BrkR="]";
    };
   
      //--for selection capture in old Opera (>7,<8) only
    this.InsCapt = function(s)
    {
        this.InsPic(s+this.BrkR+(this.Selted=(document.getSelection?(self.str?str:(document.all?(document.getSelection()?document.getSelection():document.selection.createRange().text):getSelection())):(document.selection?document.selection.createRange().text:'')))+this.BrkL+'/',s,3);
    };  //'b]selection[/',b,3
   
    
    
    this.ShowSmileBox = function(boxID)
    {
         var pos = jq('#sm_button_'+boxID).offset();
         var height = jq('#sm_button_'+boxID).outerHeight()-1; 
                        
            jq('div[id^="studio_taskBarItemBox_"]').hide();
                   
            jq('#sm_box_'+boxID).css({	  
                'left':pos.left+"px",
                'top': pos.top+height+2+'px',
                'display':'block',
                'position':'absolute'
            });      
            
            jq('body').click(function(event){
            
                var elt = (event.target) ? event.target : event.srcElement;
                var isHide = true;
                if(jq(elt).is('[id="sm_box_'+boxID+'"]'))
                    isHide = false;            
                       
                if(isHide)    
                    jq(elt).parents().each(function(){
                            if(jq(this).is('[id="sm_box_'+boxID+'"]'))
                            {
                                isHide = false
                                return false;
                            }
                     });
                            
                 if(isHide)  
                 {
                        jq('#sm_box_'+boxID).hide();
                        jq("body").unbind("click");
                 }
            }); 
    };
    
    this.HideSmileBox = function(boxID)
    {
        jq('#sm_box_'+boxID).hide();
    }
}
