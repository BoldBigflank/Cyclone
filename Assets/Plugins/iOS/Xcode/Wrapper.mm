#import "Wrapper.h"

@interface CBUUID (StringExtraction)

- (NSString *)representativeString;

@end

@implementation CBUUID (StringExtraction)

// Converts C style string to NSString
NSString* CreateNSString (const char* string)
{
	if (string)
		return [NSString stringWithUTF8String: string];
	else
		return [NSString stringWithUTF8String: ""];
}

NSString* wrapError (NSError * error)
{
	if (error != nil)
        return [NSString stringWithFormat:@"error:%@", error.description];
	else
		return @"";
}

// Helper method to create C string copy
char* MakeStringCopy (const char* string)
{
	if (string == NULL)
		return NULL;
	
	char* res = (char*)malloc(strlen(string) + 1);
	strcpy(res, string);
	return res;
}

- (NSString *)representativeString;
{
    NSData *data = [self data];
    
    NSUInteger bytesToConvert = [data length];
    const unsigned char *uuidBytes = (const unsigned char *)[data bytes];
    NSMutableString *outputString = [NSMutableString stringWithCapacity:16];
    
    for (NSUInteger currentByteIndex = 0; currentByteIndex < bytesToConvert; currentByteIndex++)
    {
        switch (currentByteIndex)
        {
            case 3:
            case 5:
            case 7:
            case 9:[outputString appendFormat:@"%02x-", uuidBytes[currentByteIndex]]; break;
            default:[outputString appendFormat:@"%02x", uuidBytes[currentByteIndex]];
        }
        
    }
    
    return outputString;
}

@end

@implementation Wrapper

- (id)init
{
    self = [super init];
    dictionary = [[NSMutableDictionary alloc] init];
    return self;
}

- (void)centralManagerDidUpdateState:(CBCentralManagerState)state {
    switch(state)
    {
        case CBCentralManagerStatePoweredOn:
            UnitySendMessage("DicePlusConnector", "onBluetoothStateChanged", "ready");
            break;
        case CBCentralManagerStateUnsupported:
            UnitySendMessage("DicePlusConnector", "onBluetoothStateChanged", "unsupported");
            break;
        default:
            UnitySendMessage("DicePlusConnector", "onBluetoothStateChanged", "not_ready");
            break;
    }
}

- (void)diceManager:(DPDiceManager *)scanner didDiscoverDie:(DPDie *)die
{
    //NSLog(@"DELEGATE: discovered die: %@", die.name);
    
    CBUUID * uuid = die.UUID;
    NSString * key = [NSString stringWithFormat:@"%@", [uuid representativeString]];
    NSString * msg = [NSString stringWithFormat:@"%@#%d", [uuid representativeString], die.RSSI.intValue];
    
    [dictionary setObject:die forKey:key];
    
    UnitySendMessage("DicePlusConnector", "onNewDie", msg.UTF8String);
}

- (void)diceManager:(DPDiceManager *)manager didConnectDie:(DPDie *)die
{
    CBUUID * uuid = die.UUID;
    NSString * result = [NSString stringWithFormat:@"%@", [uuid representativeString]];
    UnitySendMessage("DicePlusConnector", "onConnectionEstablished", result.UTF8String);
    //NSLog(@"DELEGATE: connected die");
}

-(void)diceManagerStoppedScan:(DPDiceManager *)manager
{
    NSString * result = [NSString stringWithFormat:@"%@", @"false"];
    UnitySendMessage("DicePlusConnector", "onScanFinished", result.UTF8String);
    //NSLog(@"DELEGATE: stopped scan");
    
}

- (void)diceManager:(DPDiceManager *)manager didDisconnectDie:(DPDie *)die error:(NSError *)error
{
    CBUUID * uuid = die.UUID;
    NSString * result = [NSString stringWithFormat:@"%@", [uuid representativeString]];
    UnitySendMessage("DicePlusConnector", "onConnectionLost", result.UTF8String);
    //NSLog(@"DELEGATE: disconnected die");
}

- (void)diceManager:(DPDiceManager *)manager failedConnectingDie:(DPDie *)die error:(NSError *)error
{
    CBUUID * uuid = die.UUID;
    
    int errorCode = 0;
    if (error.code == 5 && [error.domain isEqualToString:@"DicePlus"]) {
        errorCode = ERR_VERSION_MISSMATCH;
    }
    NSString * result = [NSString stringWithFormat:@"%@#%d#%@", [uuid representativeString], errorCode, wrapError(error)];
    UnitySendMessage("DicePlusConnector", "onConnectionFailed", result.UTF8String);
    //NSLog(@"DELEGATE: disconnected die");
}

- (void)die:(DPDie*)die didUpdateNotificationStateForSensor:(DPSensor)sensor withError:(NSError*)error {
    //NSLog(@"DELEGATE: didUpdateNotificationStateForSensor\n%d", sensor);
    CBUUID * uuid = die.UUID;
    NSString * result = [NSString stringWithFormat:@"%@#%d#%@", [uuid representativeString], sensor, wrapError(error) ];
    
    UnitySendMessage("DicePlusConnector", "onSubscriptionChangeStatus", result.UTF8String);
}
- (void)die:(DPDie*)die didUpdatePowerMode:(DPPowerMode*)mode  error:(NSError*)error {
    CBUUID * uuid = die.UUID;
    if (error != nil) {
        NSString * result = [NSString stringWithFormat:@"%@#error:%@", [uuid representativeString], error.description];
        UnitySendMessage("DicePlusConnector", "onPowerMode", result.UTF8String);
        return;
    }
    NSString * result = [NSString stringWithFormat:@"%@#%d#%d", [uuid representativeString], mode.timestamp, mode.mode];
    UnitySendMessage("DicePlusConnector", "onPowerMode", result.UTF8String);
}


- (void)die:(DPDie*)die didChangeLedState:(DPLedState*)status error:(NSError*)error {
    CBUUID * uuid = die.UUID;
    if (error != nil) {
        NSString * result = [NSString stringWithFormat:@"%@#error:%@", [uuid representativeString], error.description];
        UnitySendMessage("DicePlusConnector", "onLedState", result.UTF8String);
        return;
    }
    NSString * result = [NSString stringWithFormat:@"%@#%d#%d#%d#%d", [uuid representativeString], status.timestamp, status.mask, status.animationId, status.type];
    UnitySendMessage("DicePlusConnector", "onLedState", result.UTF8String);
}

- (void)die:(DPDie *)die didRoll:(DPRoll *)roll error:(NSError*)error {
    CBUUID * uuid = die.UUID;
    if (error != nil) {
        NSString * result = [NSString stringWithFormat:@"%@#error:%@", [uuid representativeString], error.description];
        UnitySendMessage("DicePlusConnector", "onRoll", result.UTF8String);
        return;
    }
    NSString * result = [NSString stringWithFormat:@"%@#%d#%d#%d#%d", [uuid representativeString], roll.timestamp, roll.duration, roll.result, roll.flags];
    UnitySendMessage("DicePlusConnector", "onRoll", result.UTF8String);
}

- (void)die:(DPDie*)die didChangeFace:(DPFaceChange*)faceChange error:(NSError*)error {
    CBUUID * uuid = die.UUID;
    if (error != nil) {
        NSString * result = [NSString stringWithFormat:@"%@#error:%@", [uuid representativeString], error.description];
        UnitySendMessage("DicePlusConnector", "onFaceReadout", result.UTF8String);
        return;
    }
    NSString * result = [NSString stringWithFormat:@"%@#%d#%d", [uuid representativeString], faceChange.timestamp, faceChange.face];
    UnitySendMessage("DicePlusConnector", "onFaceReadout", result.UTF8String);
}


- (void)die:(DPDie *)die didAccelerate:(DPAcceleration *)acceleration error:(NSError*)error {
    CBUUID * uuid = die.UUID;
    if (error != nil) {
        NSString * result = [NSString stringWithFormat:@"%@#error:%@", [uuid representativeString], error.description];
        UnitySendMessage("DicePlusConnector", "onAccelerometerReadout", result.UTF8String);
        return;
    }
    NSString * result = [NSString stringWithFormat:@"%@#%d#%d#%d#%d#%d", [uuid representativeString], acceleration.timestamp, acceleration.x, acceleration.y, acceleration.z, acceleration.filter];
    UnitySendMessage("DicePlusConnector", "onAccelerometerReadout", result.UTF8String);
}

- (void)die:(DPDie*)die didUpdateMagnetometer:(DPMagnetometer*)heading error:(NSError*)error {
    CBUUID * uuid = die.UUID;
    if (error != nil) {
        NSString * result = [NSString stringWithFormat:@"%@#error:%@", [uuid representativeString], error.description];
        UnitySendMessage("DicePlusConnector", "onMagnetometerReadout", result.UTF8String);
        return;
    }
    NSString * result = [NSString stringWithFormat:@"%@#%d#%d#%d#%d#%d", [uuid representativeString], heading.timestamp, heading.x, heading.y, heading.z, heading.filter];
    UnitySendMessage("DicePlusConnector", "onMagnetometerReadout", result.UTF8String);
}

- (void)die:(DPDie*)die didTap:(DPTap*)tap error:(NSError*)error {
    CBUUID * uuid = die.UUID;
    if (error != nil) {
        NSString * result = [NSString stringWithFormat:@"%@#error:%@", [uuid representativeString], error.description];
        UnitySendMessage("DicePlusConnector", "onTapReadout", result.UTF8String);
        return;
    }
    NSString * result = [NSString stringWithFormat:@"%@#%d#%d#%d#%d", [uuid representativeString], tap.timestamp, tap.x, tap.y, tap.z];
    UnitySendMessage("DicePlusConnector", "onTapReadout", result.UTF8String);
}


- (void)die:(DPDie*)die didUpdateOrientation:(DPOrientation*)orientation error:(NSError*)error {
    CBUUID * uuid = die.UUID;
    if (error != nil) {
        NSString * result = [NSString stringWithFormat:@"%@#error:%@", [uuid representativeString], error.description];
        UnitySendMessage("DicePlusConnector", "onOrientationReadout", result.UTF8String);
        return;
    }
    NSString * result = [NSString stringWithFormat:@"%@#%d#%d#%d#%d", [uuid representativeString], orientation.timestamp, orientation.roll, orientation.pitch, orientation.yaw];
    UnitySendMessage("DicePlusConnector", "onOrientationReadout", result.UTF8String);
}

- (void)die:(DPDie*)die didUpdateTemperature:(DPTemperature*)temperature error:(NSError*)error {
    CBUUID * uuid = die.UUID;
    if (error != nil) {
        NSString * result = [NSString stringWithFormat:@"%@#error:%@", [uuid representativeString], error.description];
        UnitySendMessage("DicePlusConnector", "onTemperatureReadout", result.UTF8String);
        return;
    }
    NSString * result = [NSString stringWithFormat:@"%@#%d#%f", [uuid representativeString], temperature.timestamp, temperature.temperature];
    UnitySendMessage("DicePlusConnector", "onTemperatureReadout", result.UTF8String);
}

- (void)die:(DPDie*)die didUpdateProximity:(DPProximity*)proximity error:(NSError*)error {
    CBUUID * uuid = die.UUID;
    if (error != nil) {
        NSString * result = [NSString stringWithFormat:@"%@#error:%@", [uuid representativeString], error.description];
        UnitySendMessage("DicePlusConnector", "onProximityReadout", result.UTF8String);
        return;
    }
    NSMutableString * result = [NSMutableString stringWithFormat:@"%@#%d", [uuid representativeString], proximity.timestamp];
    for (int i = 0; i < proximity.values.count; i++) {
        NSNumber * val = proximity.values[i];
        NSMutableString * tmp = [NSMutableString stringWithFormat:@"#%d", [val integerValue]];
        [result appendString:tmp];
    }
    
    UnitySendMessage("DicePlusConnector", "onProximityReadout", result.UTF8String);
    
}

- (void)die:(DPDie*)die didReceiveStatistics:(DPStatistics*)statistics error:(NSError*)error {
    CBUUID * uuid = die.UUID;
    if (error != nil) {
        NSString * result = [NSString stringWithFormat:@"%@#error:%@", [uuid representativeString], error.description];
        UnitySendMessage("DicePlusConnector", "onDiceStatistics", result.UTF8String);
        return;
    }
    NSMutableString * result = [NSMutableString stringWithFormat:@"%@#%d#%d#%d#%d#%d#%d#%d#%d", [uuid representativeString], statistics.totalRolls, statistics.validRolls, statistics.timesAuthenticated, statistics.chargingCycles, statistics.chargingTime, statistics.connectedTime, statistics.rollTime, statistics.wakeupCount];
    for (int i = 0; i < statistics.rolls.count; i++) {
        NSNumber * val = statistics.rolls[i];
        NSMutableString * tmp = [NSMutableString stringWithFormat:@"#%d", [val integerValue]];
        [result appendString:tmp];
    }
    UnitySendMessage("DicePlusConnector", "onDiceStatistics", result.UTF8String);
}

- (void)die:(DPDie*)die didUpdateBatteryStatus:(DPBattery*)status error:(NSError*)error {
    CBUUID * uuid = die.UUID;
    if (error != nil) {
        NSString * result = [NSString stringWithFormat:@"%@#error:%@", [uuid representativeString], error.description];
        UnitySendMessage("DicePlusConnector", "onBatteryState", result.UTF8String);
        return;
    }
    NSString * result = [NSString stringWithFormat:@"%@#%d#%d#%d", [uuid representativeString], status.isCharging ? 1 : 0, status.level, status.isLow ? 1 : 0];
    UnitySendMessage("DicePlusConnector", "onBatteryState", result.UTF8String);
}

- (void)die:(DPDie*)die didUpdateTouches:(DPTouch*)touch error:(NSError*)error {
    CBUUID * uuid = die.UUID;
    if (error != nil) {
        NSString * result = [NSString stringWithFormat:@"%@#error:%@", [uuid representativeString], error.description];
        UnitySendMessage("DicePlusConnector", "onTouchReadout", result.UTF8String);
        return;
    }
    NSString * result = [NSString stringWithFormat:@"%@#%d#%d#%d", [uuid representativeString], touch.timestamp, touch.currentStateMask, touch.changeMask];
    UnitySendMessage("DicePlusConnector", "onTouchReadout", result.UTF8String);
}

- (void)die:(DPDie*)die didFinishSetModeRequest:(NSError*)error
{
    //NSLog(@"DELEGATE: setModeRequestDoneWithError\n%@", error);
    CBUUID * uuid = die.UUID;
    NSString * result = [NSString stringWithFormat:@"%@#%@", [uuid representativeString], wrapError(error) ];
    
    UnitySendMessage("DicePlusConnector", "onSetModeStatus", result.UTF8String);
}


- (void)die:(DPDie*)die didFinishSleepRequest:(NSError*)error {
    CBUUID * uuid = die.UUID;
    NSString * result = [NSString stringWithFormat:@"%@#%@", [uuid representativeString], wrapError(error) ];
    UnitySendMessage("DicePlusConnector", "onSleepStatus", result.UTF8String);
}

- (void)die:(DPDie*)die failedToStartUpdatesForSensor:(DPSensor)sensor withError:(NSError *)error
{
    //NSLog(@"DELEGATE: failedToStartUpdatesForSensor\n%@", error);
    CBUUID * uuid = die.UUID;
    NSString * result = [NSString stringWithFormat:@"%@#%d#%d", [uuid representativeString], (error != nil?6:0), sensor ];
    UnitySendMessage("DicePlusConnector", "onSubscriptionChangeStatus", result.UTF8String);
}

- (void)die:(DPDie*)die failedToStopUpdatesForSensor:(DPSensor)sensor withError:(NSError *)error
{
    //NSLog(@"DELEGATE: failedToStopUpdatesForSensor\n%@", error);
    CBUUID * uuid = die.UUID;
    NSString * result = [NSString stringWithFormat:@"%@#%d#%d", [uuid representativeString], (error != nil?6:0), sensor ];
    UnitySendMessage("DicePlusConnector", "onSubscriptionChangeStatus", result.UTF8String);
}

- (void)die:(DPDie*)die startAnimationFailedWithError:(NSError *)error
{
    //NSLog(@"DELEGATE: startAnimationFailedWithError\n%@", error);
    CBUUID * uuid = die.UUID;
    NSString * result = [NSString stringWithFormat:@"%@#%@", [uuid representativeString], wrapError(error)];
    UnitySendMessage("DicePlusConnector", "onRunLedAnimationStatus", result.UTF8String);
}

- (void)die:(DPDie*)die didInitializePersistenStorage:(NSArray *)descriptors error:(NSError *)error
{
    //NSLog(@"DELEGATE: startAnimationFailedWithError\n%@", error);
    CBUUID * uuid = die.UUID;
    NSString * result = [NSString stringWithFormat:@"%@#%d#%@", [uuid representativeString], [descriptors count], wrapError(error)];
    UnitySendMessage("DicePlusConnector", "onPStorageCommunicationInitialized", result.UTF8String);
}

- (void)dieDidResetPersistentStorage:(DPDie *)die error:(NSError *)error
{
    //NSLog(@"DELEGATE: startAnimationFailedWithError\n%@", error);
    CBUUID * uuid = die.UUID;
    NSString * result = [NSString stringWithFormat:@"%@#%@", [uuid representativeString], wrapError(error)];
    UnitySendMessage("DicePlusConnector", "onPStorageReset", result.UTF8String);
}

- (void)die:(DPDie *)die didWritePersistentValueForHandle:(int)handle error:(NSError *)error
{
    //NSLog(@"DELEGATE: startAnimationFailedWithError\n%@", error);
    CBUUID * uuid = die.UUID;
    NSString * result = [NSString stringWithFormat:@"%@#%d#%@", [uuid representativeString], handle, wrapError(error)];
    UnitySendMessage("DicePlusConnector", "onPStorageValueWrite", result.UTF8String);
}

- (void)die:(DPDie *)die didReadPersistentValue:(id)value forHandle:(int)handle error:(NSError *)error
{
    CBUUID * uuid = die.UUID;
    NSString * result;
    
    DPPersistentStorageDescriptor* descriptor = [die.persistentStorageDescriptors objectAtIndex:handle];
    switch (descriptor.type) {
        case DPPersistentStorageValueTypeInt: {
            ///%d tu tylko int powinien byc mozliwy
            result = [NSString stringWithFormat:@"%@#%d#%@", [uuid representativeString], handle, (NSNumber*)value];
            UnitySendMessage("DicePlusConnector", "onPStorageIntValueRead", result.UTF8String);
            break;
        }
        case DPPersistentStorageValueTypeVector: {
            NSArray * array = (NSArray*)value;
            result = [NSString stringWithFormat:@"%@#%d#%@#%@#%@", [uuid representativeString], handle, [array objectAtIndex:0], [array objectAtIndex:1], [array objectAtIndex:2]];
            UnitySendMessage("DicePlusConnector", "onPStorageVec3ValueRead", result.UTF8String);
            break;
        }
        case DPPersistentStorageValueTypeString: {
            result = [NSString stringWithFormat:@"%@#%d#%@", [uuid representativeString], handle, (NSString*)value];
            UnitySendMessage("DicePlusConnector", "onPStorageStrValueRead", result.UTF8String);
            break;
        }
    }
    
}

- (void)die:(DPDie *)die didFinishPersistentStorageOperation:(NSError *)error
{
    //NSLog(@"DELEGATE: startAnimationFailedWithError\n%@", error);
    CBUUID * uuid = die.UUID;
    NSString * result = [NSString stringWithFormat:@"%@#%@", [uuid representativeString], wrapError(error)];
    UnitySendMessage("DicePlusConnector", "onPStorageOperationFailed", result.UTF8String);
}

@end

static Wrapper* delegateObject = nil;
static DPDiceManager* diceScanner = nil;

extern "C" {
    
	bool ios_create(int developerkey [])
	{
        if (delegateObject == nil) {
			delegateObject = [[Wrapper alloc] init];
        }
        
        if (diceScanner == nil) {
            diceScanner = [DPDiceManager sharedDiceManager];
            diceScanner.delegate = delegateObject;
            uint8_t key[8];
            for (int i = 0; i < 8; i++) {
                key[i] = developerkey[i];
            }
            [diceScanner setKey:key];
        }
        
        if (delegateObject != nil && diceScanner != nil) {
            return true;
        } else {
            return false;
        }
	}
    
    void ios_destroy() {
        if (delegateObject == nil) {
            return;
        }
        [diceScanner disconnectAllDice];
        
        diceScanner.delegate = nil;
        diceScanner = nil;
        [delegateObject release];
        delegateObject = nil;
    }
    
    void ios_disconnectAll() {
        [diceScanner disconnectAllDice];
	}
    
    void ios_startScan()
    {
        UnitySendMessage("DicePlusConnector", "onScanStarted", "");
        [diceScanner startScan];
        //NSLog(@"DELEGATE: started scan");
    }
    
    void ios_stopScan()
    {
        [diceScanner stopScan];
    }
    
    void ios_sleep(const char*  address) {
        NSString *key = CreateNSString(address);
        DPDie * die = [delegateObject->dictionary objectForKey:key];
        [die sleep];
    }
    
    void ios_disconnect(const char* address)
    {
        NSString *key = CreateNSString(address);
        DPDie * die = [delegateObject->dictionary objectForKey:key];
        die.delegate = nil;
        [diceScanner disconnectDie:die];
    }
    
    void ios_connect(const char* addressList)
    {
        [diceScanner stopScan];
        NSString *nAddresList = CreateNSString(addressList);
        NSArray *listItems = [nAddresList componentsSeparatedByString:@"#"];
		for (int i = 0; i < listItems.count; i++) {
            DPDie * die = [delegateObject->dictionary objectForKey:listItems[i]];
            [diceScanner connectDie:die];
            die.delegate = delegateObject;
		}
    }
    
    bool ios_connected(const char* address)
    {
        NSString *key = CreateNSString(address);
        DPDie * die = [delegateObject->dictionary objectForKey:key];
        return [die isConnected];
    }
    
    const char * ios_getModelNumber(const char* address)
    {
        NSString *key = CreateNSString(address);
        DPDie * die = [delegateObject->dictionary objectForKey:key];
        NSString * result = [NSString stringWithFormat:@"%d", [die modelNumber]];
        return MakeStringCopy(result.UTF8String);
    }
    
    const char * ios_getSoftwareVersion(const char* address)
    {
        NSString *key = CreateNSString(address);
        DPDie * die = [delegateObject->dictionary objectForKey:key];
        if ([die softwareVersion] == nil) {
            return MakeStringCopy((@"").UTF8String);
        } else {
            DPSoftwareVersion * sv = [die softwareVersion];
            NSString * result = [NSString stringWithFormat:@"%d.%d.%d", sv.major, sv.minor, sv.build];
            return MakeStringCopy(result.UTF8String);
        }
    }
    
    const char * ios_getHardwareVersion(const char* address)
    {
        NSString *key = CreateNSString(address);
        DPDie * die = [delegateObject->dictionary objectForKey:key];
        if ([die hardwareVersion] == nil) {
            return MakeStringCopy((@"").UTF8String);
        } else {
            DPHardwareVersion * sv = [die hardwareVersion];
            NSString * result = [NSString stringWithFormat:@"%d.%d", sv.major, sv.minor];
            return MakeStringCopy(result.UTF8String);
        }
    }
    
    const char * ios_getUID(const char* address)
    {
        NSString *key = CreateNSString(address);
        DPDie * die = [delegateObject->dictionary objectForKey:key];
        if ([die hardwareVersion] == nil) {
            return MakeStringCopy((@"").UTF8String);
        } else {
            NSString * result = [NSString stringWithFormat:@"%@", die.UID];
            return MakeStringCopy(result.UTF8String);
        }
    }
    
    const char * ios_getFaceCount(const char* address)
    {
        NSString *key = CreateNSString(address);
        DPDie * die = [delegateObject->dictionary objectForKey:key];
        if ([die faceCount] == nil) {
            return MakeStringCopy((@"").UTF8String);
        } else {
            NSString * result = [NSString stringWithFormat:@"%d", [die faceCount].integerValue];
            return MakeStringCopy(result.UTF8String);
        }
    }
    
    const char * ios_getStatus(const char* address)
    {
        NSString *key = CreateNSString(address);
        DPDie * die = [delegateObject->dictionary objectForKey:key];
        if ([die faceCount] == nil) {
            return MakeStringCopy((@"").UTF8String);
        } else {
            NSString * result = [NSString stringWithFormat:@"%d", [die systemStatus].integerValue];
            return MakeStringCopy(result.UTF8String);
        }
    }
    
    const char * ios_getLedCount(const char* address)
    {
        NSString *key = CreateNSString(address);
        DPDie * die = [delegateObject->dictionary objectForKey:key];
        if ([die ledCount] == nil) {
            return MakeStringCopy((@"").UTF8String);
        } else {
            NSString * result = [NSString stringWithFormat:@"%d", [die ledCount].integerValue];
            return MakeStringCopy(result.UTF8String);
        }
    }
    void ios_subscribeAccelerometerReadouts(const char*  address) {
        NSString *key = CreateNSString(address);
        DPDie * die = [delegateObject->dictionary objectForKey:key];
        [die startAccelerometerUpdates];
    }
    void ios_subscribeAndConfigureAccelerometerReadouts(const char*  address, int readFrequency, int readoutType) {
        NSString *key = CreateNSString(address);
        DPDie * die = [delegateObject->dictionary objectForKey:key];
        [die startAccelerometerUpdatesWithFrequency:readFrequency andFilter:(DPAccelerometerFilter)readoutType];
    }
	void ios_unsubscribeAccelerometerReadouts(const char*  address) {
        NSString *key = CreateNSString(address);
        DPDie * die = [delegateObject->dictionary objectForKey:key];
        [die stopAccelerometerUpdates];
    }
    
    void ios_subscribeMagnetometerReadouts(const char*  address) {
        NSString *key = CreateNSString(address);
        DPDie * die = [delegateObject->dictionary objectForKey:key];
        [die startMagnetometerUpdates];
    }
    void ios_subscribeAndConfigureMagnetometerReadouts(const char*  address, int readFrequency, int readoutType) {
        NSString *key = CreateNSString(address);
        DPDie * die = [delegateObject->dictionary objectForKey:key];
        [die startMagnetometerUpdatesWithFrequency:readFrequency andFilter:(DPMagnetometerFilter)readoutType];
    }
	void ios_unsubscribeMagnetometerReadouts(const char*  address) {
        NSString *key = CreateNSString(address);
        DPDie * die = [delegateObject->dictionary objectForKey:key];
        [die stopMagnetometerUpdates];
    }
    
    void ios_subscribeRolls(const char*  address) {
        NSString *key = CreateNSString(address);
        DPDie * die = [delegateObject->dictionary objectForKey:key];
        [die startRollUpdates];
    }
	void ios_unsubscribeRolls(const char*  address) {
        NSString *key = CreateNSString(address);
        DPDie * die = [delegateObject->dictionary objectForKey:key];
        [die stopRollUpdates];
    }
    
    void ios_subscribeTemperatureReadouts(const char*  address) {
        NSString *key = CreateNSString(address);
        DPDie * die = [delegateObject->dictionary objectForKey:key];
        [die startThermometerUpdates];
    }
	void ios_unsubscribeTemperatureReadouts(const char*  address) {
        NSString *key = CreateNSString(address);
        DPDie * die = [delegateObject->dictionary objectForKey:key];
        [die stopThermometerUpdates];
    }
    
    void ios_subscribeTouchReadouts(const char*  address) {
        NSString *key = CreateNSString(address);
        DPDie * die = [delegateObject->dictionary objectForKey:key];
        [die startTouchUpdates];
    }
	void ios_unsubscribeTouchReadouts(const char*  address) {
        NSString *key = CreateNSString(address);
        DPDie * die = [delegateObject->dictionary objectForKey:key];
        [die stopTouchUpdates];
    }
    
    void ios_subscribeProximityReadouts(const char*  address) {
        NSString *key = CreateNSString(address);
        DPDie * die = [delegateObject->dictionary objectForKey:key];
        [die startProximityUpdates];
    }
    void ios_subscribeAndConfigureProximityReadouts(const char*  address, int freq) {
        NSString *key = CreateNSString(address);
        DPDie * die = [delegateObject->dictionary objectForKey:key];
        [die startProximityUpdatesWithFrequency:freq];
    }
	void ios_unsubscribeProximityReadouts(const char*  address) {
        NSString *key = CreateNSString(address);
        DPDie * die = [delegateObject->dictionary objectForKey:key];
        [die stopProximityUpdates];
    }
    
    void ios_subscribeOrientationReadouts(const char*  address) {
        NSString *key = CreateNSString(address);
        DPDie * die = [delegateObject->dictionary objectForKey:key];
        [die startOrientationUpdates];
    }
    void ios_subscribeAndConfigureOrientationReadouts(const char*  address, int freq) {
        NSString *key = CreateNSString(address);
        DPDie * die = [delegateObject->dictionary objectForKey:key];
        [die startOrientationUpdatesWithFrequency:freq];
    }
	void ios_unsubscribeOrientationReadouts(const char*  address) {
        NSString *key = CreateNSString(address);
        DPDie * die = [delegateObject->dictionary objectForKey:key];
        [die stopOrientationUpdates];
    }
    
    void ios_subscribeLedState(const char*  address) {
        NSString *key = CreateNSString(address);
        DPDie * die = [delegateObject->dictionary objectForKey:key];
        [die startLedStateUpdates];
    }
    void ios_getLedState(const char*  address) {
        NSString *key = CreateNSString(address);
        DPDie * die = [delegateObject->dictionary objectForKey:key];
        [die readLedState];
    }
	void ios_unsubscribeLedState(const char*  address) {
        NSString *key = CreateNSString(address);
        DPDie * die = [delegateObject->dictionary objectForKey:key];
        [die stopLedStateUpdates];
    }
    
    void ios_subscribeBatteryState(const char*  address) {
        NSString *key = CreateNSString(address);
        DPDie * die = [delegateObject->dictionary objectForKey:key];
        [die startBatteryUpdates];
    }
	void ios_unsubscribeBatteryState(const char*  address) {
        NSString *key = CreateNSString(address);
        DPDie * die = [delegateObject->dictionary objectForKey:key];
        [die stopBatteryUpdates];
    }
    
    void ios_subscribePowerMode(const char*  address) {
        NSString *key = CreateNSString(address);
        DPDie * die = [delegateObject->dictionary objectForKey:key];
        [die startPowerModeUpdates];
    }
	void ios_unsubscribePowerMode(const char*  address) {
        NSString *key = CreateNSString(address);
        DPDie * die = [delegateObject->dictionary objectForKey:key];
        [die stopPowerModeUpdates];
    }
    
    void ios_subscribeTapReadouts(const char*  address) {
        NSString *key = CreateNSString(address);
        DPDie * die = [delegateObject->dictionary objectForKey:key];
        [die startTapUpdates];
    }
	void ios_unsubscribeTapReadouts(const char*  address) {
        NSString *key = CreateNSString(address);
        DPDie * die = [delegateObject->dictionary objectForKey:key];
        [die stopTapUpdates];
    }
    
    void ios_subscribeFaceReadouts(const char*  address) {
        NSString *key = CreateNSString(address);
        DPDie * die = [delegateObject->dictionary objectForKey:key];
        [die startFaceChangeUpdates];
    }
	void ios_unsubscribeFaceReadouts(const char*  address) {
        NSString *key = CreateNSString(address);
        DPDie * die = [delegateObject->dictionary objectForKey:key];
        [die stopFaceChangeUpdates];
    }
    
    void ios_getDiceStatistics(const char* address) {
        NSString *key = CreateNSString(address);
        DPDie * die = [delegateObject->dictionary objectForKey:key];
        [die readStatistics];
    }
    
    void ios_runBlinkAnimation(const char* address, int ledMask, int priority, int r, int g, int b, int ledOnPeriod, int ledCyclePeriod, int blinkNumber) {
        
        NSString *key = CreateNSString(address);
        DPDie * die = [delegateObject->dictionary objectForKey:key];
        [die startBlinkAnimationWithMask:ledMask priority:priority r:r g:g b:b onPeriod:ledOnPeriod cyclePeriod:ledCyclePeriod blinkCount:blinkNumber];
    }
    
    void ios_runFadeAnimation(const char* address, int ledMask, int priority, int r, int g, int b, int fadeInOutTime, int pauseTime) {
        NSString *key = CreateNSString(address);
        DPDie * die = [delegateObject->dictionary objectForKey:key];
        [die startFadeAnimationWithMask:ledMask priority:priority r:r g:g b:b fadeTime:fadeInOutTime pauseTime:pauseTime];
    }
    
    void ios_runStandardAnimation(const char* address, int ledMask, int priority, int animation) {
        NSString *key = CreateNSString(address);
        DPDie * die = [delegateObject->dictionary objectForKey:key];
        [die startStandardAnimationWithMask:ledMask priority:priority animation:(DPDieAnimation)animation];
    }
    
    void ios_setMode(const char*  address, int mode) {
        NSString *key = CreateNSString(address);
        DPDie * die = [delegateObject->dictionary objectForKey:key];
        DPDieMode dpmode = DPDieModeLedsOn;
        if (mode == 1) {
            dpmode = DPDieModeLedsOff;
        }
        [die setMode:dpmode];
    }
    
	void ios_writePStorageStrValue(const char* address, int handle, const char* str) {
        NSString *key = CreateNSString(address);
        DPDie * die = [delegateObject->dictionary objectForKey:key];
        [die writePersistentStringValue:CreateNSString(str) forHandle:handle];
    }
	void ios_writePStorageVecValue(const char* address, int handle, int x, int y, int z) {
        NSString *key = CreateNSString(address);
        DPDie * die = [delegateObject->dictionary objectForKey:key];
        [die writePersistentVectorValueWithX:x y:y andZ:z forHandle:handle];
    }
	void ios_writePStorageIntValue(const char* address, int handle, int intvalue) {
        NSString *key = CreateNSString(address);
        DPDie * die = [delegateObject->dictionary objectForKey:key];
        [die writePersistentIntValue:intvalue forHandle:handle];
    }
    void ios_readPStorageValue(const char*  address, int handle) {
        NSString *key = CreateNSString(address);
        DPDie * die = [delegateObject->dictionary objectForKey:key];
        [die readPersistentStorageValueForHandle:handle];
    }
    void ios_initializePStorageCommunication(const char* address) {
        NSString *key = CreateNSString(address);
        DPDie * die = [delegateObject->dictionary objectForKey:key];
        [die initializePersistentStorageCommunication];
    }
    const char * ios_getPStorageRecordDescription(const char* address, int handle) {
        NSString *key = CreateNSString(address);
        DPDie * die = [delegateObject->dictionary objectForKey:key];
        
        if ([die persistentStorageDescriptors].count > handle) {
            DPPersistentStorageDescriptor * descriptor = [die persistentStorageDescriptors][handle];
            
            NSString * defaultValue = @"";
            switch (descriptor.type) {
                case DPPersistentStorageValueTypeInt:
                    defaultValue = [NSString stringWithFormat:@"%d", ((NSNumber*)descriptor.defaultValue).intValue];
                    break;
                case DPPersistentStorageValueTypeString:
                    defaultValue = (NSString*)descriptor.defaultValue;
                    break;
                case DPPersistentStorageValueTypeVector:
                {
                    NSArray* value = (NSArray*)descriptor.defaultValue;
                    defaultValue = [NSString stringWithFormat:@"%d,%d,%d", ((NSNumber*)value[0]).intValue, ((NSNumber*)value[1]).intValue, ((NSNumber*)value[2]).intValue];
                }
                    break;
                default:
                    break;
            }
            
            
            NSString * result = [NSString stringWithFormat:@"%d#%d#%d#%@#%@#%@#%d#%d#%@", descriptor.handle, descriptor.type, descriptor.flags, descriptor.name, descriptor.description, descriptor.unit, descriptor.minValue, descriptor.maxValue, defaultValue ];
            return MakeStringCopy(result.UTF8String);
        } else {
            return MakeStringCopy((@"").UTF8String);
        }
        
    }
    void ios_resetPStorage(const char* address) {
        NSString *key = CreateNSString(address);
        DPDie * die = [delegateObject->dictionary objectForKey:key];
        [die resetPersistentStorage];
    }
}