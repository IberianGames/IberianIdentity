//
//
// Copyright (c) 2017 Fyber. All rights reserved.
//
//

#import <Foundation/Foundation.h>

@protocol FYBOffer <NSObject>

@property (nonatomic, copy, readonly) NSString *networkName;
@property (nonatomic, copy, readonly) NSString *adId;
@property (nonatomic, copy, readonly) NSDictionary *trackingParams;

@end
