StudioSimpleUserSelector = new function()
{
    this.OpenDialog = function(additionalID)
    {   
        try{
            jq.blockUI({message:jq("#studio_simpleUserSelectorDialog"+additionalID),
                   css: { 
                        opacity: '1',
                        border: 'none',
                        padding: '0px',
                        width: '400px',
                        height: '300px',
                        cursor: 'default',
                        textAlign : 'left',
                        'background-color': 'Transparent',
                        'margin-left': '-200px',
                        'top': '25%'
                    },
                    
                    overlayCSS: { 
                        backgroundColor: '#aaaaaa',
                        cursor: 'default',
                        opacity: '0.3' 
                    },                  
                    focusInput : false,
                    fadeIn: 0,
                    fadeOut: 0
            });     
        }
        catch(e){}; 
        
        PopupKeyUpActionProvider.ClearActions();  
        PopupKeyUpActionProvider.EnterAction = jq('[id$="_selectEnterCode"]').val();
    
    };
    this.Cancel = function()
    {
        PopupKeyUpActionProvider.ClearActions();  
        jq.unblockUI();
    }
    this.SelectCallback = function(){alert('empty callback')}
    
    this.Select = function(additionalID,callback)
    {
        callback( jq("#studio_simple_user_selector"+additionalID).val() );
    }
}