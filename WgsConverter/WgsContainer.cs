namespace WgsConverter;

public sealed record WgsBlob(string Name, Guid WgsFilename);

public sealed record WgsContainer(WgsBlob[] Blobs)
{
    public static WgsContainer Read(Stream stream)
    {
        using var reader = new BinaryReader(stream, BinaryReaderExtensions.Utf16, true);
        var tmp = reader.ReadInt32(); // unk1
        if (tmp != 4)
            throw new InvalidDataException("Expected unk1 to be 4");

        var count = reader.ReadInt32();
        var entries = new WgsBlob[count];
        Span<byte> buf = stackalloc byte[64*2]; 
        for (var i = 0; i < count; i++)
        {
            stream.ReadExactly(buf);
            var name = BinaryReaderExtensions.Utf16.GetString(buf).TrimEnd('\0');
            var guid1 = reader.ReadGuid();
            var guid2 = reader.ReadGuid();
            if (guid1 != guid2)
                throw new InvalidDataException($"Expected both guids to be the same in blob #{i}");

            entries[i] = new(name, guid2);
        }
        return new(entries);
    }
}