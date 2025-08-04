/// <summary>
/// Represents a single core in a CPU or SoC.
/// </summary>
public class ProcessorCore
{
    #region EVENTS
    /// <summary>
    /// Delegate used for saving the configuration when pinning or parking status has changed on this core.
    /// </summary>
    public delegate void ParkedPinnedChangedDelegate();
    /// <summary>
    /// Used  for saving the configuration when pinning or parking status has changed on this core.
    /// </summary>
    public event ParkedPinnedChangedDelegate? ParkedPinnedChanged;
    #endregion EVENTS
    
    #region PRIVATES
    private bool? parked;
    private bool? pinned;
    #endregion PRIVATES
    
    #region PUBLIC PROPERTIES
    /// <summary>
    /// Determines the type of core in this processor.
    /// </summary>
    public ProcessorCoreType Type { get; set; }
    /// <summary>
    /// Index of the core.
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// Determines if this core should be parked (disabled) or not.
    /// </summary>
    public bool? IsParked
    {
        get => parked;
        set
        {
            parked = value;
            ParkedPinnedChanged?.Invoke();
        }
    }
    /// <summary>
    /// Determines if this core should be pinned to app process or not.
    /// </summary>
    public bool? IsPinned
    {
        get => pinned;
        set
        {
            pinned = value;
            ParkedPinnedChanged?.Invoke();
        }
    }
    #endregion PUBLIC PROPERTIES
}