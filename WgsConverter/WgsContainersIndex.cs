using System.Text;

namespace WgsConverter;

public sealed record WgsContainerEntry(string Filename, string Revision, byte ContainerId, Guid ContainerFolder, ulong Timestamp, int Filesize);

public sealed record WgsContainersIndex(string Title, ulong Timestamp, string TitleId, WgsContainerEntry[] Containers)
{
    public static WgsContainersIndex Read(Stream stream)
    {
        using var reader = new BinaryReader(stream, BinaryReaderExtensions.Utf16, true);
        _ = reader.ReadInt32(); // unk1
        var entryCount = reader.ReadInt32();
        var tmp = reader.ReadInt32(); // unk2
        if (tmp != 0)
            throw new InvalidDataException("Expected unk2 to be 0");

        var title = reader.ReadUtf16String();
        var ts = reader.ReadUInt64(); // unk3
        _ = reader.ReadInt32(); // unk4 = 3?
        var titleId = reader.ReadUtf16String();
        _ = reader.ReadInt32(); // unk5 = 0x80000000 ?
        var entries = new WgsContainerEntry[entryCount];
        for (var i = 0; i < entryCount; i++)
        {
            tmp = reader.ReadInt32(); // unk1
            if (tmp != 0)
                throw new InvalidDataException($"Expected unk1 in container entry #{i} to be 0");

            var cloudFilename = reader.ReadUtf16String();
            var localFilename = reader.ReadUtf16String();
            var rev = reader.ReadUtf16String();
            var id = reader.ReadByte();
            tmp = reader.ReadInt32(); // unk2
            if (tmp != 1)
                throw new InvalidDataException($"Expected unk2 in container entry #{i} to be 1");

            var wgsFolder = reader.ReadGuid();
            var cts = reader.ReadUInt64(); // unk3
            var tmp64 = reader.ReadInt64(); // unk4
            if (tmp64 != 0L)
                throw new InvalidDataException($"Expected unk4 in container entry #{i} to be 0");

            var fsize = reader.ReadInt32();

            entries[i] = new(localFilename, rev, id, wgsFolder, cts, fsize);
        }
        tmp = reader.ReadInt32(); // unk6
        if (tmp != 0)
            throw new InvalidDataException("Expected unk6 to be 0");

        return new(title, ts, titleId, entries);
    }
}