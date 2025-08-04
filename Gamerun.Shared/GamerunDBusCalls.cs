/// <summary>
/// Used for doing DBus calls.
/// </summary>
public class GamerunDBusCalls 
{
    /// <summary>
    /// Destination of DBus call.
    /// </summary>
    public string Destination { get; set; }
    /// <summary>
    /// Object to call <see cref="Method"/> from.
    /// </summary>
    public string ObjectPath { get; set; }
    /// <summary>
    /// Method to call.
    /// </summary>
    public string Method { get; set; }
    /// <summary>
    /// Arguments for the call.
    /// </summary>
    public string[] Arguments { get; set; }
}