namespace Gamerun.Shared;

/// <summary>
///     Represent a machine's graphical processing unit.
/// </summary>
/// <param name="FilePath">Path of the card.</param>
/// <param name="Name">name of the GPU.</param>
/// <param name="PciId">PCI ID of this GPU (ex. 01:00.0 for Nvidia cards).</param>
/// <param name="Driver">Driver name of the card. Mostly used for detecting if a Nvidia card is using Nvidia's drivers.</param>
/// <param name="VendorId">
///     Vendor ID part of <see cref="PciId" /> (for example on <c>01:00.0</c> this represents 01 which
///     is Nvidia).
/// </param>
/// <param name="DeviceId">
///     Device ID part of <see cref="PciId" /> (for example on <c>01:00.0</c> this represents 00 which
///     tells this is a graphics card).
/// </param>
/// <param name="Modes">Resolution of this card.</param>
/// <param name="DRMPath">Path of the DRM (Direct Rendering Mode) card.</param>
public record GpuInfo(
    string FilePath,
    string Name,
    string PciId,
    string Driver,
    string VendorId,
    string DeviceId,
    string Modes,
    string DRMPath);