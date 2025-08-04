using System.Net.Sockets;
using System.Text;

namespace Gamerun;

public static class SimpleDBus
{
    private static string GetSessionBusAddress()
    {
        var addr = Environment.GetEnvironmentVariable("DBUS_SESSION_BUS_ADDRESS");
        if (string.IsNullOrEmpty(addr))
            throw new Exception("DBUS_SESSION_BUS_ADDRESS not set.");

        if (!addr.StartsWith("unix:path="))
            throw new NotSupportedException("Only unix:path addresses are supported.");

        return addr["unix:path=".Length..];
    }

    public static void Send(
        string destination,
        string path,
        string interfaceName,
        string method,
        params object[] args)
    {
        var address = GetSessionBusAddress();
        using var socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.IP);
        socket.Connect(new UnixDomainSocketEndPoint(address));

        var msg = BuildMessage(destination, path, interfaceName, method, args);

        socket.Send(msg);
    }

    private static byte[] BuildMessage(
        string destination,
        string path,
        string interfaceName,
        string method,
        object[] args)
    {
        using var ms = new MemoryStream();
        using var writer = new BinaryWriter(ms, Encoding.UTF8, leaveOpen: true);

        // Header
        writer.Write((byte)'l'); // little endian
        writer.Write((byte)1); // DBus major version
        writer.Write((byte)1); // message type: method call
        writer.Write((byte)0); // flags
        writer.Write((byte)1); // protocol version

        const uint serial = 1;
        writer.Write(serial); // serial number (dummy)

        // Prepare header fields (fields 1, 2, 3, 6, 7)
        var headerFields = new MemoryStream();
        var headerWriter = new BinaryWriter(headerFields, Encoding.UTF8, leaveOpen: true);

        WriteHeaderField(1, path);
        WriteHeaderField(2, interfaceName);
        WriteHeaderField(3, method);
        WriteHeaderField(6, destination);

        // Signature
        var signature = BuildSignature(args);
        if (!string.IsNullOrEmpty(signature))
            WriteHeaderField(8, signature);

        // Align to 8 before writing body
        Align(writer, 8);
        var headerArray = headerFields.ToArray();
        var headerLen = (uint)headerArray.Length;

        ms.Position = 4;
        writer.Write(headerLen); // header length
        writer.Write((uint)0); // body length (we'll patch later)

        ms.Position = ms.Length;
        writer.Write(headerArray);

        Align(writer, 8);

        var bodyStart = ms.Position;
        foreach (var arg in args)
            WriteVariant(writer, arg);

        var bodyEnd = ms.Position;
        var bodyLen = (uint)(bodyEnd - bodyStart);

        // Patch body length
        ms.Position = 8;
        writer.Write(bodyLen);

        return ms.ToArray();

        void WriteHeaderField(byte code, string value)
        {
            headerWriter.Write(code);
            headerWriter.Write((byte)'s'); // signature = string
            Align(headerWriter, 4);
            WriteDBusString(headerWriter, value);
        }
    }

    private static string BuildSignature(object[] args)
    {
        var sb = new StringBuilder();
        foreach (var arg in args)
        {
            sb.Append(arg switch
            {
                string => "s",
                int => "i",
                bool => "b",
                double => "d",
                string[] => "as",
                _ => throw new NotSupportedException($"Unsupported type {arg.GetType()}")
            });
        }

        return sb.ToString();
    }

    private static void WriteVariant(BinaryWriter writer, object arg)
    {
        switch (arg)
        {
            case string str:
                WriteDBusString(writer, str);
                break;
            case int i:
                Align(writer, 4);
                writer.Write(i);
                break;
            case bool b:
                Align(writer, 4);
                writer.Write(b ? 1 : 0);
                break;
            case double d:
                Align(writer, 8);
                writer.Write(d);
                break;
            case string[] arr:
                Align(writer, 4);
                var start = writer.BaseStream.Position;
                writer.Write(0); // placeholder for array length
                foreach (var s in arr)
                {
                    WriteDBusString(writer, s);
                }

                var end = writer.BaseStream.Position;
                var length = (int)(end - start - 4);
                writer.BaseStream.Position = start;
                writer.Write(length);
                writer.BaseStream.Position = end;
                break;
            default:
                throw new NotSupportedException($"Unsupported type {arg.GetType()}");
        }
    }

    private static void WriteDBusString(BinaryWriter writer, string value)
    {
        var bytes = Encoding.UTF8.GetBytes(value);
        writer.Write(bytes.Length);
        writer.Write(bytes);
        writer.Write((byte)0); // null terminator
    }

    private static void Align(BinaryWriter writer, int alignment)
    {
        var pos = writer.BaseStream.Position;
        var pad = (int)((alignment - (pos % alignment)) % alignment);
        for (var i = 0; i < pad; i++)
            writer.Write((byte)0);
    }
}