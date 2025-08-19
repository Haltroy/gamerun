using System;
using System.IO;

namespace Gamerun.Shared;

/// <summary>
///     Provides a record for keeping configs compatible across all Gamerun & Gamerun Config versions.
/// </summary>
/// <param name="Detect">Function that detects if a version is compatible or not.</param>
/// <param name="Read">Function that reads the configuration if <see cref="Detect" /> return <c>true</c>.</param>
public record GamerunConfigVersionPair(Func<byte, bool> Detect, Action<bool[], Stream> Read);