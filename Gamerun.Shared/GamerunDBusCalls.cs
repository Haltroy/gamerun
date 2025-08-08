namespace Gamerun.Shared;

/// <summary>
///     Used for doing DBus calls.
/// </summary>
/// <param name="Destination">Destination of DBus call.</param>
/// <param name="ObjectPath">Object to call <see cref="Method" /> from.</param>
/// <param name="Method">Method to call.</param>
/// <param name="Arguments">Arguments for the call.</param>
public record GamerunDBusCalls(string Destination, string ObjectPath, string Method, string[] Arguments);