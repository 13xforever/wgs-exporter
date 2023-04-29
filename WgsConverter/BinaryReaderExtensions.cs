using System.Text;

namespace WgsConverter;

public static class BinaryReaderExtensions
{
    public static readonly Encoding Utf16 = new UnicodeEncoding(false, false);

    public static string ReadUtf16String(this BinaryReader reader)
    {
        var len = reader.ReadInt32();
        Span<byte> buf = stackalloc byte[len * 2];
        reader.BaseStream.ReadExactly(buf);
        return Utf16.GetString(buf);
    }

    public static Guid ReadGuid(this BinaryReader reader)
    {
        Span<byte> buf = stackalloc byte[16];
        reader.BaseStream.ReadExactly(buf);
        return new(buf);
    }

}