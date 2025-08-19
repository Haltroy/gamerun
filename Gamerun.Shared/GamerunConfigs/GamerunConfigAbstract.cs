using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Gamerun.Shared.Exceptions;

namespace Gamerun.Shared;

/// <summary>
///     Abstract class for different settings classes for Gamerun.
/// </summary>
public abstract class GamerunConfigAbstract
{
    /// <summary>
    ///     Delegate for the save event.
    /// </summary>
    public delegate void GamerunSettingSaveDelegate();

    /// <summary>
    ///     Determines if this uses the defaults. Used to detect if a class needs saving to a file.
    /// </summary>
    public virtual bool IsDefaults => false;

    /// <summary>
    ///     Determines if this is the default configuration.
    /// </summary>
    public bool IsDefaultConfig { get; private set; }

    /// <summary>
    ///     Current version of this config.
    /// </summary>
    public abstract byte CurrentVersion { get; }

    /// <summary>
    ///     Version pairs for reading configurations across multiple versions.
    /// </summary>
    public abstract GamerunConfigVersionPair[] Pairs { get; }

    /// <summary>
    ///     Settings to write on <see cref="WriteSettings" />'s first bytes.
    ///     <para />
    ///     NOTE: <see cref="Name" /> and <see cref="CurrentVersion" /> and things related to it is handled by the root
    ///     abstract class so there's no need to add code specific to them here.
    /// </summary>
    public abstract bool[] Settings { get; }

    /// <summary>
    ///     Gets the visible name of this configuration.
    /// </summary>
    public string? Name
    {
        get => _name;
        set
        {
            _name = value;
            OnSave?.Invoke();
        }
    }

    /// <summary>
    ///     Gets and sets the file name of this configuration.
    /// </summary>
    public required string FileName
    {
        get => _fileName.SanitizeFilename();
        set
        {
            if (IsDefaultConfig) return;
            var save = !string.IsNullOrWhiteSpace(_fileName);
            _fileName = value.SanitizeFilename();
            if (save) OnSave?.Invoke();
        }
    }

    /// <summary>
    ///     Writes data to <paramref name="stream" /> on the current version.
    /// </summary>
    /// <param name="stream">Stream to write.</param>
    public virtual void WriteSettings(Stream stream)
    {
        var name = Encoding.UTF8.GetBytes(Name ?? string.Empty);
        var bools = new List<bool>(Settings);
        bools.Insert(0, name.Length < Tools.VLEMaxSize);
        bools.Insert(0, name.Length != 0);
        stream.WriteByte(CurrentVersion);
        var buffer = Tools.PackBoolsToBytes(bools.ToArray());
        stream.Write(buffer);

        if (name.Length <= 0) return;
        if (name.Length < Tools.VLEMaxSize)
        {
            Tools.WriteVarUInt(stream, (uint)name.Length);
        }
        else
        {
            buffer = BitConverter.GetBytes((uint)name.Length);
            stream.Write(buffer);
        }

        stream.Write(name);
    }

    /// <summary>
    ///     Reads a config file for this configuration class.
    /// </summary>
    /// <param name="stream">Stream to read.</param>
    /// <exception cref="GamerunEndOfStreamException">
    ///     Exception thrown when <paramref name="stream" /> has reached end
    ///     prematurely.
    /// </exception>
    /// <exception cref="GamerunVersionNotSupportedException">
    ///     Exception thrown when the config file's version is not supported.
    /// </exception>
    public void Read(Stream stream)
    {
        var bufferByte = stream.ReadByte();
        if (bufferByte is -1 or > byte.MaxValue or < byte.MinValue)
            throw new GamerunEndOfStreamException(stream.Position);
        var found = Pairs.Where(x => x.Detect((byte)bufferByte)).ToArray();
        if (found.Length == 0) throw new GamerunVersionNotSupportedException(bufferByte, CurrentVersion, FileName);
        bufferByte = stream.ReadByte();
        if (bufferByte is -1) throw new GamerunEndOfStreamException(stream.Position);
        var buffer = new byte[bufferByte];
        var bufferRead = stream.Read(buffer);
        if (bufferRead != buffer.Length) throw new GamerunEndOfStreamException(stream.Position);
        var decoded = Tools.UnpackBytesToBools(buffer, bufferByte * 8);
        if (decoded[0])
        {
            int length;
            if (decoded[1])
            {
                length = (int)Tools.DecodeVarUInt(stream);
            }
            else
            {
                buffer = new byte[sizeof(uint)];
                bufferRead = stream.Read(buffer, 0, buffer.Length);
                if (bufferRead != buffer.Length) throw new GamerunEndOfStreamException(stream.Position);
                length = (int)BitConverter.ToUInt32(buffer);
            }

            buffer = new byte[length];
            bufferRead = stream.Read(buffer, 0, buffer.Length);
            if (bufferRead != buffer.Length) throw new GamerunEndOfStreamException(stream.Position);
            Name = Encoding.UTF8.GetString(buffer);
        }

        var decodedToSend = new bool[decoded.Length - 2];
        for (var i = 0; i < decodedToSend.Length; i++) decodedToSend[i] = decoded[i + 2];
        found[0].Read(decodedToSend, stream);
    }

    /// <summary>
    ///     Generates the command, environment variables and start/end script(s).
    /// </summary>
    /// <param name="args">Arguments from previous module(s).</param>
    /// <returns>Command to run.</returns>
    public virtual GamerunStartArguments GenerateArgs(GamerunStartArguments args)
    {
        return args;
    }

    /// <summary>
    ///     Sets this setting as the default setting to be used.
    /// </summary>
    internal virtual void SetAsDefault()
    {
        IsDefaultConfig = true;
    }

    /// <summary>
    ///     This event will be fired when a setting changes to dynamically save it.
    /// </summary>
    public virtual event GamerunSettingSaveDelegate? OnSave;


    #region PRIVATES

    private string? _name;
    private string _fileName = string.Empty;

    #endregion PRIVATES
}