/// <summary>
///     Types of processor cores inside of a CPU or SoC.
/// </summary>
public enum ProcessorCoreType
{
    /// <summary>
    ///     The low power cores, "E"fficiency on Intel, "LITTLE" on ARM etc.
    /// </summary>
    LowPower,

    /// <summary>
    ///     Core that has no specific power type.
    /// </summary>
    Normal,

    /// <summary>
    ///     The high power cores, "P"erformance on Intel, "big" on ARM etc.
    /// </summary>
    HighPower
}