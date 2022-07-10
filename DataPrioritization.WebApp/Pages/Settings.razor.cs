﻿


using DataPrioritization.WebApp.Data;
using DataPrioritization.DataAccess;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using DataPrioritization.WebApp.Utility;

using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Components.Forms;
using DataPrioritization.Models;
//inject ILogger<FileUpload1> Logger
//inject IWebHostEnvironment Environment



namespace DataPrioritization.WebApp.Pages;

public partial class Settings
{
    #region Fields
    //private IList<Employee>? employees;
    [Inject] IOptions<ConnectionStringModel> options { get; set; }
    [Inject] IWebHostEnvironment env { get; set; }

    private string _pageTile = "Settings";
    private IWebHostEnvironment _env;
    private List<IBrowserFile> loadedFiles = new();
    private IList<int> distinctNodes = new List<int>();
    private IList<int> allNodes = new List<int>();
    private string filesRootPath;
    private bool isLoading;
    private List<Tuple<string, List<int>>> settings;
    #endregion


    #region Events
    protected override async Task OnInitializedAsync()
    {
        _env = env;
        Helper.InitializeFolders(_env.ContentRootPath);
        filesRootPath  = Path.Combine(_env.ContentRootPath, "Data", "Files");
        LoadSettings();
    }


    private async Task UploadFiles(InputFileChangeEventArgs e)
    {
        isLoading = true;
        loadedFiles.Clear();

        Helper.ClearFolder(Path.Combine(filesRootPath, "Settings"));

        foreach (var file in e.GetMultipleFiles())
        {
            try
            {
                loadedFiles.Add(file);

                var autoGeneratedFileName = Path.GetRandomFileName();
                var path = Path.Combine(filesRootPath, "Settings", autoGeneratedFileName);
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

    private void LoadSettings()
    {
        var lines = Helper.ReadLinesFromFile(Path.Combine(filesRootPath, "Settings"));
        settings = GetSettings(lines);
    }

    private void Clear()
    {
        settings = null;
    }

    #endregion


    #region Helper Methods
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
    #endregion





    //private void CreateFile()
    //{
    //    string folderPath = Path.Combine(filesRootPath, "Settings");
    //    string filePath = Path.Combine(folderPath, Path.GetRandomFileName());

    //    Helper.ClearFolder(folderPath);
    //    File.WriteAllLines(filePath, Helper.ReadLinesFromFile(Path.Combine(filesRootPath, "Input")));
    //}
}

