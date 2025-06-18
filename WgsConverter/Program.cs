using WgsConverter;

var baseOutput = "./out/";

// C:\Users\user\AppData\Local\Packages\39EA002F.DQB2XB_n746a19ndrrjg\SystemAppData\wgs\0009000001EAEEAD_00000000000000000000000067EB0CE8
var appDataLocal = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
var packagesBase = Path.Combine(appDataLocal, "Packages");
var packages = Directory.GetDirectories(packagesBase);
var gamePackages = new Dictionary<string, string>();
foreach (var p in packages)
{
    var wgsPath = Path.Combine(p, "SystemAppData", "wgs");
    if (!Directory.Exists(wgsPath))
        continue;

    var wgsDataFolders = Directory.GetDirectories(wgsPath).Select(Path.GetFileName).ToArray();
    if (wgsDataFolders is [])
        continue;

    if (wgsDataFolders is not ["t", _] and not [_, "t"])
        throw new InvalidOperationException($"Expected exactly two folders one of which is 't', but found {wgsDataFolders.Length}: {string.Join(',', wgsDataFolders)}");

    gamePackages[Path.GetFileName(p)] = Path.Combine(wgsPath, wgsDataFolders.First(n => n is not "t")!);
}
if (gamePackages.Count is 0)
{
    Console.WriteLine("No save data found. You have to have the game installed and launched at least once to sync the save data.");
    return -1;
}

var idList = new List<string>(gamePackages.Count);
var maxName = gamePackages.Keys.Max(n => n.Length);
Console.WriteLine($"Found {gamePackages.Count} packages with save data:");
var i = 0;
foreach (var (k, v) in gamePackages)
{
    Console.WriteLine($"[{i++,2}] {k.PadRight(maxName)}: {Path.GetFileName(v)}");
    idList.Add(k);
}
/*
Console.Write("Enter package number to export: ");
var id = Console.ReadLine();
if (!int.TryParse(id, out var idx)
    || idx < 0 || idx >= idList.Count)
{
    Console.WriteLine("Invalid id");
    return;
}

var pkgName = idList[idx];
*/
foreach (var (pkgName, path) in gamePackages)
{
    Console.WriteLine($"Exporting {pkgName} save data…");
    var pkgOutPath = Path.Combine(baseOutput, pkgName);
    var wgsContainerIndexPath = Path.Combine(path, "containers.index");
    await using var stream = File.Open(wgsContainerIndexPath, FileMode.Open, FileAccess.Read, FileShare.Read);
    var containerIndex = WgsContainersIndex.Read(stream);
    Console.WriteLine($"There are {containerIndex.Containers.Length} containers in {containerIndex.Title} ({containerIndex.TitleId})");
    foreach (var c in containerIndex.Containers)
    {
        var containerOutPath = Path.Combine(pkgOutPath, c.Filename.EscapeInvalidPath());
        if (!Directory.Exists(containerOutPath))
            Directory.CreateDirectory(containerOutPath);
        
        var wgsFolderName = c.ContainerFolder.ToString("N").ToUpper();
        Console.WriteLine($"\t{c.Filename} ({wgsFolderName})");
        var containerPath = Path.Combine(path, wgsFolderName, $"container.{c.ContainerId}");
        using var cstream = File.Open(containerPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        var container = WgsContainer.Read(cstream);
        foreach (var b in container.Blobs)
        {
            var wgsFilename = b.WgsFilename.ToString("N").ToUpper();
            Console.WriteLine($"\t\t{b.Name} ({wgsFilename})");

            var srcPath = Path.Combine(path, wgsFolderName, wgsFilename);
            var blobFilename = Path.Combine(containerOutPath, b.Name.EscapeInvalidFilename());
            File.Copy(srcPath, blobFilename);
        }
    }
}

Console.WriteLine("Done.");
return 0;