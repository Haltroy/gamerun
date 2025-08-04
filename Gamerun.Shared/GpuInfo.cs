namespace Gamerun.Shared;

/// <summary>
///     Represent a machine's graphical processing unit.
/// </summary>
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
public record GpuInfo(string PciId, string Driver, string VendorId, string DeviceId, string Modes);