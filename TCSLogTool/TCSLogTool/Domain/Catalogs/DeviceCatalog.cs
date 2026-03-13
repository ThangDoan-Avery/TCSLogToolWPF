namespace TCSLogTool.Domain.Catalogs;

public static class DeviceCatalog
{
    private static readonly Dictionary<string, DeviceType> map = new()
    {
        ["TC_R1"] = DeviceType.Robot,
        ["TC_R2"] = DeviceType.Robot,
        ["TC_R3"] = DeviceType.Robot,
        ["TC_R4"] = DeviceType.Robot,

        ["LL_Lift1"] = DeviceType.Lift,
        ["LL_Lift2"] = DeviceType.Lift,

        ["LL_DOOR1"] = DeviceType.Door,
        ["LL_DOOR2"] = DeviceType.Door,

        ["LL_SV1"] = DeviceType.SlitValve,
        ["LL_SV2"] = DeviceType.SlitValve,
        ["LL_SV3"] = DeviceType.SlitValve,
        ["LL_SV4"] = DeviceType.SlitValve,
        ["A_SV1"] = DeviceType.SlitValve,
        ["A_SV2"] = DeviceType.SlitValve,
        ["A_SV3"] = DeviceType.SlitValve,
        ["A_SV4"] = DeviceType.SlitValve,

        ["LL_PG"] = DeviceType.PressureGauge,
        ["TC_PG"] = DeviceType.PressureGauge,
        ["LL_FLPG"] = DeviceType.PressureGauge,
        ["TC_FLPG"] = DeviceType.PressureGauge,

        ["LL_ISOV"] = DeviceType.Valve,
        ["LL_FVV"] = DeviceType.Valve,
        ["LL_SVV"] = DeviceType.Valve,
        ["LL_EQV1"] = DeviceType.Valve,
        ["LL_EQV2"] = DeviceType.Valve,
        ["LL_BPV"] = DeviceType.Valve,
        ["TC_ISOV"] = DeviceType.Valve,
        ["TC_FVV"] = DeviceType.Valve,
        ["TC_SVV"] = DeviceType.Valve,
        ["TC_EQV"] = DeviceType.Valve,

        ["LL_PUMP"] = DeviceType.Pump,
        ["TC_PUMP"] = DeviceType.Pump,

        ["TC_MFC"] = DeviceType.MFC,
        ["TC_TV"] = DeviceType.ThrottleValve,
        ["TC_PCTRL"] = DeviceType.PressureCtrl,
        ["LL_HTR1-4"] = DeviceType.Heater,
    };

    public static DeviceType GetType(string? device)
    {
        if (device == null)
            return DeviceType.Other;

        if (map.TryGetValue(device, out var type))
            return type;

        return DeviceType.Other;
    }
}