namespace WgsConverter;

public static class PathExtensions
{
    private static readonly  HashSet<char> InvalidPathChars = new(Path.GetInvalidPathChars());
    private static readonly  HashSet<char> InvalidFilenameChars = new(Path.GetInvalidFileNameChars());

    public static string EscapeInvalidPath(this string path)
    {
        Span<char> tmp = stackalloc char[path.Length];
        for (var i = 0; i < path.Length; i++)
        {
            var c = path[i];
            tmp[i] = InvalidPathChars.Contains(c) ? '_' : c;
        }
        return new(tmp);
    }

    public static string EscapeInvalidFilename(this string name)
    {
        Span<char> tmp = stackalloc char[name.Length];
        for (var i = 0; i < name.Length; i++)
        {
            var c = name[i];
            tmp[i] = InvalidFilenameChars.Contains(c) ? '_' : c;
        }
        return new(tmp);
    }
}