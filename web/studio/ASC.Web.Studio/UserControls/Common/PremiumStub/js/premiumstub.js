var PremiumStubManager = new function() {

    this.FeatureType = { PrivateProject: 0, AccessRigths: 1, SharedDocs: 2 };

    this.SwitchMessageType = function(type) {
        jq('#premiumStubText').html(jq('#premiumStubText_' + type).val());
        jq('#premiumStubType').val(type);
    }
    this.ShowDialog = function() {

        jq.blockUI({
            message: jq("#studio_premiumStubDialog"),
            css: {
                opacity: '1',
                border: 'none',
                padding: '0px',
                width: '600px',
                height: '500px',
                cursor: 'default',
                textAlign: 'left',
                backgroundColor: 'transparent',
                marginLeft: '-300px',
                top: '25%'
            },
            overlayCSS: {
                backgroundColor: '#AAA',
                cursor: 'default',
                opacity: '0.3'
            },
            focusInput: false,
            fadeIn: 0,
            fadeOut: 0
        });

        jq('#buyStubBtn').unbind('click');
        jq('#buyStubBtn').click(function() {

            var type = jq('#premiumStubType').val();            
            if (type == PremiumStubManager.FeatureType.PrivateProject)
                EventTracker.Track('buynow_privateProject');

            if (type == PremiumStubManager.FeatureType.AccessRigths)
                EventTracker.Track('buynow_docAccessRights');

            if (type == PremiumStubManager.FeatureType.SharedDocs)
                EventTracker.Track('buynow_docSharedDocs');

        })
    }

    this.CloseDialog = function() {
        jq.unblockUI();
    }
}