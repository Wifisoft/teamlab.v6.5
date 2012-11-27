StudioConfirm = new function()
{
    this.OpenDialog = function(additionalID)
    {   
        jq('#studio_confirmMessage').html('');
        try{
            jq.blockUI({message:jq("#studio_confirmDialog"+additionalID),
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
        PopupKeyUpActionProvider.EnterAction = jq('[id$="_confirmEnterCode"]').val();
    
    };
    this.Cancel = function()
    {
        PopupKeyUpActionProvider.ClearActions();  
        jq.unblockUI();
    }
    this.SelectCallback = function(){alert('empty callback')}
    
    this.Select = function(additionalID,callback)
    {
        callback( jq("#studio_confirmInput"+additionalID).val() );
    }
}