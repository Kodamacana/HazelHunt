#import <RichTapSDK/RichTapHapticUtils.h>
extern "C"
{
    bool InvokeIsRichTapEffectSupported()
    {
        return [RichTapHapticUtils supportRichTap];
    }

    void InvokePlayHaptics(char* data, int loop, int amplitude, int interval, int freq)
    {
        NSString *str = [NSString stringWithCString:data encoding:NSUTF8StringEncoding];
        [RichTapHapticUtils playHaptic:str loop:loop amplitude:amplitude interval:interval freq:freq error:nil];
    }

    void InvokePlayPrebake(int playId, int amplitude)
    {
        [RichTapHapticUtils playExtPreBaked:(PrebakedEffectId)(playId) strength:amplitude error:nil];
    }

    void InvokeStop()
    {
        [RichTapHapticUtils stop:nil];
    }

    void InvokeSendLoopParameters(int amplitude, int interval, int freq) 
    {
        [RichTapHapticUtils sendLoopParameter:amplitude interval:interval freq:freq error:nil];
    }

  
}