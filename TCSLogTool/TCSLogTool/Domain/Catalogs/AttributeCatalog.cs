namespace TCSLogTool.Domain.Catalogs;

public static class AttributeCatalog
{
    /*
        ATTRIBUTE DEFINITIONS
    */

    private static readonly Dictionary<string, AttributeDefinition> attributes
        = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Status"] = new AttributeDefinition
            {
                Name = "Status",
                Type = AttributeType.Discrete
            },

            ["State"] = new AttributeDefinition
            {
                Name = "State",
                Type = AttributeType.Discrete
            },

            ["Pressure"] = new AttributeDefinition
            {
                Name = "Pressure",
                Type = AttributeType.Continuous,
                Unit = "mTorr"
            },

            ["Flow"] = new AttributeDefinition
            {
                Name = "Flow",
                Type = AttributeType.Continuous,
                Unit = "sccm"
            },

            ["SetpointPosition"] = new AttributeDefinition
            {
                Name = "SetpointPosition",
                Type = AttributeType.Continuous,
                Unit = "%"
            }
        };

    /*
        STATUS LABELS
    */

    private static readonly Dictionary<DeviceType, StatusDefinition> status = new()
    {
        [DeviceType.Robot] = new StatusDefinition
        {
            DeviceType = DeviceType.Robot,
            Labels = new()
            {
                [0] = "Idle",
                [1] = "Stopping",
                [2] = "Moving"
            }
        },

        [DeviceType.Door] = new StatusDefinition
        {
            DeviceType = DeviceType.Door,
            Labels = new()
            {
                [0] = "Unknown",
                [1] = "Opening",
                [2] = "Opened",
                [3] = "Closing",
                [4] = "Closed"
            }
        },

        [DeviceType.SlitValve] = new StatusDefinition
        {
            DeviceType = DeviceType.SlitValve,
            Labels = new()
            {
                [0] = "Unknown",
                [1] = "Opening",
                [2] = "Opened",
                [3] = "Closing",
                [4] = "Closed"
            }
        },

        [DeviceType.Lift] = new StatusDefinition
        {
            DeviceType = DeviceType.Lift,
            Labels = new()
            {
                [0] = "FIRelease",
                [1] = "FILift",
                [2] = "FIXfer",
                [3] = "TCRelease",
                [4] = "TCLift",
                [5] = "TCXfer",
                [6] = "Home",
                [7] = "Moving",
                [8] = "Heating",
                [9] = "Cooling",
                [10] = "Unknown"
            }
        },

        [DeviceType.Valve] = new StatusDefinition
        {
            DeviceType = DeviceType.Valve,
            Labels = new()
            {
                [0] = "Closed",
                [1] = "Opened"
            }
        },

        [DeviceType.Pump] = new StatusDefinition
        {
            DeviceType = DeviceType.Pump,
            Labels = new()
            {
                [0] = "Off",
                [1] = "On"
            }
        },

        [DeviceType.PressureCtrl] = new StatusDefinition
        {
            DeviceType = DeviceType.PressureCtrl,
            Labels = new()
            {
                [0] = "InActive",
                [1] = "AtTarget",
                [2] = "NotInTarget"
            }
        },

        [DeviceType.Heater] = new StatusDefinition
        {
            DeviceType = DeviceType.Heater,
            Labels = new()
            {
                [0] = "Heating",
                [1] = "Stable",
                [2] = "TurningOff",
                [3] = "Off"
            }
        }
    };

    public static AttributeType GetType(string? attribute)
    {
        if (attribute == null)
            return AttributeType.Continuous;

        if (attributes.TryGetValue(attribute, out var def))
            return def.Type;

        return AttributeType.Continuous;
    }

    public static string? GetUnit(string? attribute)
    {
        if (attribute == null)
            return null;

        if (attributes.TryGetValue(attribute, out var def))
            return def.Unit;

        return null;
    }

    public static string? GetStatusLabel(DeviceType deviceType, int value)
    {
        if (!status.TryGetValue(deviceType, out var def))
            return null;

        if (!def.Labels.TryGetValue(value, out var label))
            return null;

        return label;
    }
}