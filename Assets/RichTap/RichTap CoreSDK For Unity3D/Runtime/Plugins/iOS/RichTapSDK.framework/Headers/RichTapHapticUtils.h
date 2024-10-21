//
//  AACHapticUtils.h
//  iOS_AAC_RichTap
//
//  Created by AAC Acoustic Technologies (Shanghai) Co., Ltd. on 2020/7/23.
//  Copyright © 2020 AAC Acoustic Technologies (Shanghai) Co., Ltd.. All rights reserved.
//

#import <RichTapSDK/RichTapEnum.h>
NS_ASSUME_NONNULL_BEGIN


@interface RichTapHapticUtils : NSObject

/// 查看是否支持core haptic
+(BOOL)supportRichTap;

/// 退出core haptic
/// @param error 如果quit操作失败，将返回错误信息
+(BOOL)quit:(NSError *_Nullable *_Nullable)error;

/// 停止震动
/// @param error 如果stop操作失败，将返回错误信息
+(BOOL)stop:(NSError *_Nullable *_Nullable)error;


/// 调用通用效果库中的效果,目前定义了49个效果
/// @param effect 指定哪个效果，取值范围[0,49]
/// @param strength 指定效果的强度，取值范围是[0, 511]
/// @param error 如果playExtPreBaked操作失败，将返回错误信息
+(void)playExtPreBaked:(PrebakedEffectId)effect strength:(int)strength error:(NSError *_Nullable *_Nullable)error;

/// 播放he效果文件
/// @param str 文件路径或he格式json字符串
/// @param loop 文件循环次数，取值范围 [-1-+∞]
/// @param error 如果playHaptic操作失败，将返回错误信息
+(void)playHaptic:(NSString*)str loop:(int)loop error:(NSError *_Nullable *_Nullable)error;

/// 播放he效果文件，添加强度参数
/// @param str  文件路径或he格式json字符串
/// @param loop 文件循环次数，取值范围 [-1-+∞]
/// @param amplitude 播放强度，取值范围 [0-511]
/// @param error 如果playHaptic操作失败，将返回错误信息
+(void)playHaptic:(NSString*)str loop:(int)loop amplitude:(int)amplitude error:(NSError *_Nullable *_Nullable)error;

/// 播放he效果文件，添加强度参数&间隔参数
/// @param str  文件路径或he格式json字符串
/// @param loop 文件循环次数，取值范围 [-1 - +∞]
/// @param amplitude 播放强度，取值范围 [0 - 511]
/// @param interval 循环播放间隔，取值范围 [0 - ∞]ms
/// @param error 如果playHaptic操作失败，将返回错误信息
+(void)playHaptic:(NSString*)str loop:(int)loop amplitude:(int)amplitude interval:(int)interval error:(NSError *_Nullable *_Nullable)error;

/// 播放he效果文件，添加强度参数、频率参数&间隔参数
/// @param str  文件路径或he格式json字符串
/// @param loop 文件循环次数，取值范围 [-1 - +∞]
/// @param amplitude 播放强度，取值范围 [0 - 511]
/// @param interval 循环播放间隔，取值范围 [0 - +∞]ms
/// @param freq 表示频率，取值范围是[-100 - 100]
/// @param error 如果playHaptic操作失败，将返回错误信息
+(void)playHaptic:(NSString*)str loop:(int)loop amplitude:(int)amplitude interval:(int)interval freq:(int)freq error:(NSError *_Nullable *_Nullable)error;

/// 用来调整Loop Pattern的缩放幅度和间隔，必须在loop>0时，才有效
/// @param intensity 表示缩放值，取值范围是[0 - 511]
/// @param interval 表示每次播放结束到下次开始的时间，取值范围是[0 - +∞]ms
/// @param error 如果sendLoopParameter操作失败，将返回错误信息
+(void)sendLoopParameter:(int)intensity interval:(int)interval error:(NSError *_Nullable *_Nullable)error;


/// 用来调整Loop Pattern的缩放幅度、频率和间隔，必须在loop>0时，才有效
/// @param intensity 表示缩放值，取值范围是[0 - 511]
/// @param interval 表示每次播放结束到下次开始的时间，取值范围是[0 - +∞]ms
/// @param freq 表示频率，取值范围是[-100 - 100]
/// @param error 如果sendLoopParameter操作失败，将返回错误信息
+(void)sendLoopParameter:(int)intensity interval:(int)interval freq:(int)freq error:(NSError *_Nullable *_Nullable)error;

@end

NS_ASSUME_NONNULL_END
