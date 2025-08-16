using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Microsoft.AspNetCore.Http;
//using FileUploader.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client.Extensions.Msal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace File;

public class FileFunction
{
    public interface IStorage
    {
        Task Save(Stream fileStream, string name);
        Task<IEnumerable<string>> GetNames();
        Task<Stream> Load(string name);
    }

    public class AzureStorageConfig
    {
        public string ConnectionString { get; set; }
        public string FileContainerName { get; set; }
    }

    public class BlobStorage : IStorage
    {
        private readonly AzureStorageConfig storageConfig;

        public BlobStorage(AzureStorageConfig storageConfig)
        {
            this.storageConfig = storageConfig;
        }

        public Task Initialize()
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(storageConfig.ConnectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(storageConfig.FileContainerName);
            return containerClient.CreateIfNotExistsAsync();
        }

        //[Function("SaveFiles")]
        public Task Save(Stream fileStream, string name)
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(storageConfig.ConnectionString);

            // Get the container (folder) the file will be saved in
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(storageConfig.FileContainerName);

            // Get the Blob Client used to interact with (including create) the blob
            BlobClient blobClient = containerClient.GetBlobClient(name);

            // Upload the blob
            return blobClient.UploadAsync(fileStream);
        }

        public async Task<IEnumerable<string>> GetNames()
        {
            List<string> names = new List<string>();

            BlobServiceClient blobServiceClient = new BlobServiceClient(storageConfig.ConnectionString);

            // Get the container the blobs are saved in
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(storageConfig.FileContainerName);

            // This gets the info about the blobs in the container
            AsyncPageable<BlobItem> blobs = containerClient.GetBlobsAsync();

            await foreach (var blob in blobs)
            {
                names.Add(blob.Name);
            }
            return names;
        }

        //[Function("LoadFiles")]
        public Task<Stream> Load(string name)
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(storageConfig.ConnectionString);

            // Get the container the blobs are saved in
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(storageConfig.FileContainerName);

            // Get a client to operate on the blob so we can read it.
            BlobClient blobClient = containerClient.GetBlobClient(name);

            return blobClient.OpenReadAsync();
        }
    }

    private readonly ILogger<FileFunction> _logger;

    private readonly IStorage storage;

    public FileFunction(ILogger<FileFunction> logger, IStorage storage)
    {
        _logger = logger;
        this.storage = storage;
    }

    private const int MaxFilenameLength = 50;
    private static readonly Regex filenameRegex = new Regex("[^a-zA-Z0-9._]");


    public FileFunction(IStorage storage)
    {
        this.storage = storage;
    }

    [Function("GetFileNames")]
    public static async Task<HttpResponseData> GetFilesNames([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req,
    FunctionContext executionContext)
    {
        var storageConfig = new AzureStorageConfig
        {
            ConnectionString = "",//Environment.GetEnvironmentVariable("AzureWebJobsStorage"),
            FileContainerName = "fileuploaderblob"//Environment.GetEnvironmentVariable("FileContainerName")
        };
        var blobStorage = new BlobStorage(storageConfig);
        blobStorage.Initialize().GetAwaiter().GetResult();
        var names = await blobStorage.GetNames();
        var response = req.CreateResponse(HttpStatusCode.OK);
        string responseData = JsonConvert.SerializeObject(names);
        await response.WriteStringAsync(responseData);
        return response;
        //var baseUrl = Request.Path.Value;

        //var urls = names.Select(n => $"{baseUrl}/{n}");

        //return Ok(names);
    }


    private static string SanitizeFilename(string filename)
    {
        var sanitizedFilename = filenameRegex.Replace(filename, "").TrimEnd('.');

        if (sanitizedFilename.Length > MaxFilenameLength)
        {
            sanitizedFilename = sanitizedFilename.Substring(0, MaxFilenameLength);
        }

        return sanitizedFilename;
    }
}