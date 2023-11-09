using System.Security.Cryptography;
using System.Text;
using Luban.Serialization;
using Luban.Utils;
using NLog;

namespace Luban.Custom;

public static class DataManifestGeneration
{
    private class FileDescription
    {
        public string Name;
        public string Tag;
        public string Hash;
        public int Size;

        public FileDescription(string name, string tag, string hash, int size)
        {
            Name = name;
            Tag = tag;
            Hash = hash;
            Size = size;
        }
    }

    private static ILogger s_logger = LogManager.GetCurrentClassLogger();

    private const string OutputManifestDirOptionName = "outputManifestDir";
    private const string SearchExtensionForManifestOptionName = "searchExtensionForManifest";

    public static void Generate()
    {
        if (!EnvManager.Current.TryGetOption("", OutputManifestDirOptionName, false, out string outputManifestDir))
        {
            s_logger.Info("Data manifest generation skip~");
            return;
        }

        if (Directory.Exists(outputManifestDir))
        {
            Directory.Delete(outputManifestDir, true);
        }
        Directory.CreateDirectory(outputManifestDir);

        string extension = EnvManager.Current.GetOptionOrDefault("", SearchExtensionForManifestOptionName, false, "bytes");
        var outputDataDir = EnvManager.Current.GetOption("", BuiltinOptionNames.OutputDataDir, false);
        var directoryInfo = new DirectoryInfo(outputDataDir);
        var fileDescriptions = new List<FileDescription>();
        ProcessDataFiles(directoryInfo, "", extension, fileDescriptions, outputManifestDir);

        var byteBuf = new ByteBuf();
        byteBuf.WriteInt(fileDescriptions.Count);
        foreach (var fileDescription in fileDescriptions)
        {
            byteBuf.WriteString(fileDescription.Name);
            byteBuf.WriteString(fileDescription.Tag);
            byteBuf.WriteString(fileDescription.Hash);
            byteBuf.WriteInt(fileDescription.Size);
        }

        var bytes = byteBuf.CopyData();
        
        File.WriteAllBytes(Path.Combine(outputManifestDir, $"manifest.{extension}"), EncryptionUtil.Encrypt("manifest", byteBuf.CopyData()));
    }

    private static void ProcessDataFiles(DirectoryInfo parent, string tag, string extension,
        List<FileDescription> fileDescriptions, string outputPath)
    {
        foreach (var fileInfo in parent.GetFiles($"*.{extension}"))
        {
            string name = FileUtil.GetFileNameWithoutExt(fileInfo.Name);
            var nameData = Encoding.ASCII.GetBytes(fileInfo.Name);
            var fileData = File.ReadAllBytes(fileInfo.FullName);
            var fileSize = fileData.Length;
            var hashData = SHA1.HashData(nameData.Concat(fileData).ToArray());
            string hash = Convert.ToHexString(hashData).ToLower();
            s_logger.Info("add manifest: name={} tag={} hash={} size={}", name, tag, hash, fileSize);
            File.Copy(fileInfo.FullName, Path.Combine(outputPath, $"{hash}.{extension}"));
            fileDescriptions.Add(new FileDescription(name, tag, hash, fileSize));
        }

        foreach (var directoryInfo in parent.GetDirectories())
        {
            string innerTag = tag.Length > 0 ? $"{tag}_{directoryInfo.Name}" : directoryInfo.Name;
            ProcessDataFiles(directoryInfo, innerTag, extension, fileDescriptions, outputPath);
        }
    }
}