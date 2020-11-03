#import <Foundation/NSProcessInfo.h>

#define EXPORT(_returnType_) extern "C" _returnType_ __attribute__ ((visibility ("default")))

namespace
{
    // Values must match UnityEngine.XR.ARFoundation.Samples.ThermalStateForIOS.ThermalState
    enum ThermalState
    {
        kThermalStateUnknown = 0,
        kThermalStateNominal = 1,
        kThermalStateFair = 2,
        kThermalStateSerious = 3,
        kThermalStateCritical = 4,
    };

    inline ThermalState ConvertThermalState(NSProcessInfoThermalState thermalState)
    {
        ThermalState returnValue;

        switch (thermalState)
        {
            case NSProcessInfoThermalStateNominal:
                returnValue = kThermalStateNominal;
                break;
            case NSProcessInfoThermalStateFair:
                returnValue = kThermalStateFair;
                break;
            case NSProcessInfoThermalStateSerious:
                returnValue = kThermalStateSerious;
                break;
            case NSProcessInfoThermalStateCritical:
                returnValue = kThermalStateCritical;
                break;
            default:
                returnValue = kThermalStateUnknown;
                break;
        }

        return returnValue;
    }
}

EXPORT(ThermalState) ARFoundationSamples_GetCurrentThermalState()
{
    return ::ConvertThermalState([[NSProcessInfo processInfo] thermalState]);
}
