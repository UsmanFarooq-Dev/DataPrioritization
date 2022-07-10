﻿using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Components.Forms;
using DataPrioritization.Models;
using DataPrioritization.WebApp.Utility;
//inject ILogger<FileUpload1> Logger
//inject IWebHostEnvironment Environment



namespace DataPrioritization.WebApp.Pages;

public partial class Input
{
    #region Fields
    //private IList<Employee>? employees;
    [Inject] IOptions<ConnectionStringModel> settings { get; set; }
    [Inject] IWebHostEnvironment env { get; set; }

    private string _pageTile = "Input";
    private IWebHostEnvironment _env;
    private List<IBrowserFile> loadedFiles = new();
    private IList<int> distinctNodes = new List<int>();
    private IList<int> allNodes = new List<int>();
    private string filesRootPath;
    private bool isLoading;
    #endregion


    #region Events
    protected override async Task OnInitializedAsync()
    {
        //var conString = settings.Value.ConnectionString;
        //employees = DB.GetEmployees(conString);

        _env = env;
        Helper.InitializeFolders(_env.ContentRootPath);
        filesRootPath  = Path.Combine(_env.ContentRootPath, "Data", "Files");
        LoadNodes();
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

    private void LoadNodes()
    {
        var lines = Helper.ReadLinesFromFile(Path.Combine(filesRootPath, "Input"));
        distinctNodes = GetNodes(lines, true, true);
        allNodes = GetNodes(lines);
    }

    private void ClearNodes()
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
