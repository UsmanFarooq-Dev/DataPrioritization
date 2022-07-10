﻿using DataPrioritization.Models;
using DataPrioritization.WebApp.Utility;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Options;
//inject ILogger<FileUpload1> Logger
//inject IWebHostEnvironment Environment



namespace DataPrioritization.WebApp.Pages;

public partial class Output
{
    #region Fields
    //private IList<Employee>? employees;
    [Inject] IOptions<ConnectionStringModel> options { get; set; }
    [Inject] IWebHostEnvironment env { get; set; }

    private string _pageTile = "Output";
    private IWebHostEnvironment _env;
    private List<IBrowserFile> loadedFiles = new();
    private IList<int> distinctNodes = null;
    private IList<int> allNodes = null;
    private string filesRootPath;
    private List<Tuple<string, List<int>>> settings;
    private bool isLoading;
    private List<int> que1;
    private List<int> que2;
    private List<int> que3;
    private List<int> que4;
    private List<int> que5;
    private List<int> output;
    #endregion


    #region Events
    protected override async Task OnInitializedAsync()
    {
        //var conString = settings.Value.ConnectionString;
        //employees = DB.GetEmployees(conString);

        _env = env;
        Helper.InitializeFolders(_env.ContentRootPath);
        filesRootPath  = Path.Combine(_env.ContentRootPath, "Data", "Files");

        // copy file
        DirectoryInfo folder = new DirectoryInfo(Path.Combine(filesRootPath, "Input"));
        Helper.ClearFolder(Path.Combine(filesRootPath, "Input_Temp"));
        var lines = new List<string>();

        foreach (FileInfo file in folder.GetFiles())
        {
            var sourceFilePath = Path.Combine(Path.Combine(filesRootPath, "Input", file.Name));
            var destFilePath = Path.Combine(Path.Combine(filesRootPath, "Input_Temp", file.Name));

            File.Copy(sourceFilePath, destFilePath, true);
        }
    }


    private async Task UploadFiles(InputFileChangeEventArgs e)
    {
        isLoading = true;
        loadedFiles.Clear();

        Helper.ClearFolder(Path.Combine(filesRootPath, "Input"));

        foreach (var file in e.GetMultipleFiles())
        {
            try
            {
                loadedFiles.Add(file);

                var autoGeneratedFileName = Path.GetRandomFileName();
                var path = Path.Combine(filesRootPath, "Input", autoGeneratedFileName);
                await using FileStream fs = new(path, FileMode.Create);
                await file.OpenReadStream().CopyToAsync(fs);
            }
            catch (Exception ex)
            {
                //Logger.LogError("File: {Filename} Error: {Error}",
                //    file.Name, ex.Message);
            }
        }
        isLoading = false;
    }

    private async Task StartProcess()
    {
        var lines_settings = Helper.ReadLinesFromFile(Path.Combine(filesRootPath, "Settings"));
        settings = GetSettings(lines_settings);




        var lines = Helper.ReadLinesFromFile(Path.Combine(filesRootPath, "Input_Temp"));
        allNodes = GetNodes(lines);
        output = new List<int>();
        List<int[]> batches = allNodes.Chunk(5).ToList();
        List<int[]> _batches = allNodes.Chunk(5).ToList();
        //Thread.Sleep(3000);
        for (int j = 0; j < batches.Count; j++) // each batch
        {
            await Task.Delay(4000);

            que1 = new List<int>();
            que2 = new List<int>();
            que3 = new List<int>();
            que4 = new List<int>();
            que5 = new List<int>();

            foreach (var item in batches[j]) // each item in batch
            {

                for (int i = 0; i < settings.Count; i++)
                {
                    var que = settings[i].Item1;
                    var nodes = settings[i].Item2;
                    if (nodes.Contains(item))
                    {
                        if (i == 0)
                        {
                            que1.Add(item);
                        }
                        else if (i == 1)
                        {
                            que2.Add(item);
                        }
                        else if (i == 2)
                        {
                            que3.Add(item);
                        }
                        else if (i == 3)
                        {
                            que4.Add(item);
                        }
                        else if (i == 4)
                        {
                            que5.Add(item);
                        }
                        break;
                    }
                }
            }
            output.AddRange(que1);
            output.AddRange(que2);
            output.AddRange(que3);
            output.AddRange(que4);
            output.AddRange(que5);

            if (_batches.Count > 0)
            {
                _batches.RemoveAt(0);
                allNodes.Clear();

                foreach (var bat in _batches)
                {
                    foreach (var b in bat)
                    {
                        allNodes.Add(b);
                    }
                }
            }
            //Thread.Sleep(500);
            StateHasChanged();

        }


    }

    private void LoadSettings()
    {
        var lines = Helper.ReadLinesFromFile(Path.Combine(filesRootPath, "Settings"));
        settings = GetSettings(lines);
    }

    private List<Tuple<string, List<int>>> GetSettings(List<string> lines)
    {
        var listOfNodes = new List<int>();
        var que = "";
        var tuples = new List<Tuple<string, List<int>>>();

        foreach (var line in lines)
        {
            que = line.Split(':')[0];
            string nodeString = line.Split(':')[1];

            var nodes = string.IsNullOrEmpty(nodeString) ? new List<int>() : (Array.ConvertAll(nodeString.Split(','), int.Parse)).ToList();

            tuples.Add(Tuple.Create(que, nodes));

        }

        return tuples;
    }

    private void StopProcess()
    {
        distinctNodes = null;
        allNodes = null;
    }

    #endregion


    #region Helper Methods
    private List<int> GetNodes(List<string> lines, bool isDistinct = false, bool isSorted = false)
    {
        var nodes = new List<int>();

        foreach (var line in isDistinct ? lines.Distinct() : lines)
        {
            var ln = line.Split(':')[1];
            nodes.Add(Convert.ToInt32(ln));
        }
        return isSorted ? nodes.OrderBy(x => x).ToList() : nodes;
    }
    #endregion





    private void CreateFile()
    {
        string folderPath = Path.Combine(filesRootPath, "Settings");
        string filePath = Path.Combine(folderPath, Path.GetRandomFileName());

        Helper.ClearFolder(folderPath);
        File.WriteAllLines(filePath, Helper.ReadLinesFromFile(Path.Combine(filesRootPath, "Input")));
    }
}

