#import <Foundation/Foundation.h>
#import <AudioToolbox/AudioToolbox.h>
#import <UIKit/UIKit.h>

#import "EZHaptics.h"

#define USING_IPAD UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPad

@interface Vibration ()

@end

@implementation Vibration



//////////////////////////////////////////

#pragma mark - Vibrate

+ (BOOL)    hasVibrator {
    return !(USING_IPAD);
}
+ (void)    vibrate {
    AudioServicesPlaySystemSoundWithCompletion(1352, NULL);
}

@end
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

#pragma mark - "C"

extern "C" {
    
    //////////////////////////////////////////
    // Vibrate
    
    bool    _HasVibrator () {
        return [Vibration hasVibrator];
    }
 
    void    _Vibrate () {
        [Vibration vibrate];
    }
}

