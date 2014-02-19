#import "DicePlus/DicePlus.h"

@interface Wrapper : NSObject<DPDiceManagerDelegate, DPDieDelegate> {
    @public
    NSMutableDictionary * dictionary;
}

@end

static const int ERR_VERSION_MISSMATCH = 0x1;
