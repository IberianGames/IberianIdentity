//
//
// Copyright (c) 2017 Fyber. All rights reserved.
//
//

#import "FYBURLParametersProvider.h"
#import "FYBBaseURLProvider.h"


@class FYBCredentials;


@interface FYBURLGenerator : NSObject

@property (nonatomic, copy) NSString *baseURLString;

@property (nonatomic, strong) NSDictionary *customParameters;

+ (FYBURLGenerator *)URLGeneratorWithBaseURLString:(NSString *)baseUrl;
+ (FYBURLGenerator *)URLGeneratorWithEndpoint:(FYBURLEndpoint)endpointKey;

+ (void)setGlobalCustomParametersProvider:(id<FYBURLParametersProvider>)provider;
+ (void)addGlobalCustomParametersProvider:(id<FYBURLParametersProvider>)provider;

- (id)initWithBaseURLString:(NSString *)baseURLString;
- (id)initWithEndpoint:(FYBURLEndpoint)endpoint;

- (void)setCredentials:(FYBCredentials *)credentials;
- (void)setCurrencyName:(NSString *)currencyName;
- (void)setRewardedParam;
- (void)setVideoAdFormatParam;
- (NSString *)generateRequestId;   // generates a random request_id, sets and returns it
- (void)setAnswerReceived:(NSString *)answerReceived;
- (void)setActionId:(NSString *)actionId;
- (void)setPlacementId:(NSString *)placementId;
- (void)setCurrencyId:(NSString *)currencyId;
- (void)setOfferWallCloseButton:(NSString *)value;
- (void)setLatestTransactionId:(NSString *)latestTransactionId;
- (void)addParametersProvider:(id<FYBURLParametersProvider>)paramsProvider;

- (NSURL *)generatedURL;
- (NSURL *)signedURLWithSecretToken:(NSString *)secretToken;

@end
