using System.Text.RegularExpressions;

namespace DataPrioritization.WebApp.Utility;


public static class Helper
{
    private static readonly Regex regX = new Regex(@"\s+");

    public static string ReplaceWhitespace(this string input, string replacement)
    {
        return regX.Replace(input, replacement);
    }


    public static void ClearFolder(string path)
    {
        DirectoryInfo folder = new DirectoryInfo(path);
        foreach (FileInfo file in folder.GetFiles())
        {
            file.Delete();
        }

        foreach (DirectoryInfo dir in folder.GetDirectories())
        {
            dir.Delete();
        }
    }

    public static void InitializeFolders(string rootPath)
    {
        DirectoryInfo dataFolder = new DirectoryInfo(Path.Combine(rootPath, "Data"));
        if (!dataFolder.Exists)
        {
            dataFolder.Create();
        }

        DirectoryInfo filesFolder = new DirectoryInfo(Path.Combine(dataFolder.FullName, "Files"));
        if (!filesFolder.Exists)
        {
            filesFolder.Create();
        }

        DirectoryInfo inputFolder = new DirectoryInfo(Path.Combine(filesFolder.FullName, "Input"));
        if (!inputFolder.Exists)
        {
            inputFolder.Create();
        }

        DirectoryInfo inputTempFolder = new DirectoryInfo(Path.Combine(filesFolder.FullName, "Input_Temp"));
        if (!inputTempFolder.Exists)
        {
            inputTempFolder.Create();
        }

        DirectoryInfo outputFolder = new DirectoryInfo(Path.Combine(filesFolder.FullName, "Output"));
        if (!outputFolder.Exists)
        {
            outputFolder.Create();
        }

        DirectoryInfo settingsFolder = new DirectoryInfo(Path.Combine(filesFolder.FullName, "Settings"));
        if (!settingsFolder.Exists)
        {
            settingsFolder.Create();
        }

    }

    public static List<string> ReadLinesFromFile(string folderPath)
    {
        DirectoryInfo folder = new DirectoryInfo(folderPath);
        var lines = new List<string>();

        foreach (FileInfo file in folder.GetFiles())
        {
            var filePath = Path.Combine(folderPath, file.Name);
            var rawLines = File.ReadAllLines(filePath).ToList();
            foreach (var line in rawLines)
            {
                lines.Add(line.ReplaceWhitespace(string.Empty));
            }
        }
        return lines;
    }
}
