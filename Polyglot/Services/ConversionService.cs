﻿using CsvHelper;
using Polyglot.Models;
using System.Globalization;
using System.Text.Json;

namespace Polyglot.Services;

public static class ConversionService
{
    public static IEnumerable<MasterModel> ConvertInputFile(Stream stream, string inputFormat)
    {
        stream.Position = 0;
        IEnumerable<MasterModel> processedLinks = [];

        return inputFormat switch
        {
            "GoodLinks" => ImportGoodlinks(stream),
            "Instapaper" => ImportInstapaper(stream),
            "Omnivore" => ImportOmnivore(stream),
            "Raindrop" => ImportRaindrop(stream),
            _ => processedLinks
        };
    }

    public static Stream GenerateOutputFile(IEnumerable<MasterModel> processedLinks, string outputFormat)
    {
        byte[] outputFile = [];

        outputFile = outputFormat switch
        {
            "GoodLinks" => ExportGoodlinks(processedLinks),
            "Instapaper" => ExportInstapaper(processedLinks),
            "Raindrop" => ExportRaindrop(processedLinks),
            "Readwise" => ExportReadwise(processedLinks),
            _ => outputFile
        };

        return new MemoryStream(outputFile);
    }

    private static IEnumerable<MasterModel> ImportGoodlinks(Stream stream)
    {
        var baseDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        foreach (GoodLinksModel importedLink in JsonSerializer.Deserialize<IEnumerable<GoodLinksModel>>(stream) ?? [])
        {
            var processedLink = new MasterModel
            {
                DateSaved = baseDateTime.AddSeconds(importedLink.addedAt),
                Favorite = importedLink.starred,
                Tags = [.. importedLink.tags],
                Title = importedLink.title,
                Url = importedLink.url
            };

            if (importedLink.readAt != 0) processedLink.Archived = true;

            yield return processedLink;
        }
    }

    private static IEnumerable<MasterModel> ImportInstapaper(Stream stream)
    {
        var baseDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        using var reader = new StreamReader(stream);
        using var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);

        // Todo: check for valid headers
        foreach (InstapaperModel importedLink in csvReader.GetRecords<InstapaperModel>())
        {
            var processedLink = new MasterModel
            {
                DateSaved = baseDateTime.AddSeconds(importedLink.Timestamp),
                Title = importedLink.Title,
                Url = importedLink.URL
            };

            if (importedLink.Folder.Contains("archive", StringComparison.InvariantCultureIgnoreCase))
                processedLink.Archived = true;
            else if (importedLink.Folder != "Unread")
                processedLink.Tags.Add(importedLink.Folder);

            if (!String.IsNullOrEmpty(importedLink.Tags))
                processedLink.Tags = [.. importedLink.Tags.Split(',', StringSplitOptions.TrimEntries)];

            yield return processedLink;
        }
    }

    private static IEnumerable<MasterModel> ImportOmnivore(Stream stream)
    {
        foreach (OmnivoreInputModel importedLink in JsonSerializer.Deserialize<IEnumerable<OmnivoreInputModel>>(stream) ?? [])
        {
            var processedLink = new MasterModel
            {
                DateSaved = importedLink.savedAt,
                Tags = [.. importedLink.labels],
                Title = importedLink.title,
                Url = importedLink.url
            };

            if (importedLink.state == "Archived") processedLink.Archived = true;

            yield return processedLink;
        }
    }

    private static IEnumerable<MasterModel> ImportRaindrop(Stream stream)
    {
        using var reader = new StreamReader(stream);
        using var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);

        // Todo: check for valid headers
        foreach (RaindropModel importedLink in csvReader.GetRecords<RaindropModel>())
        {
            var processedLink = new MasterModel
            {
                DateSaved = DateTime.Parse(importedLink.created).ToUniversalTime(),
                Favorite = importedLink.favorite,
                Notes = importedLink.note,
                Title = importedLink.title,
                Url = importedLink.url
            };

            if (!String.IsNullOrEmpty(importedLink.tags))
                processedLink.Tags = [.. importedLink.tags.Split(',', StringSplitOptions.TrimEntries)];

            if (importedLink.folder.Contains("archive", StringComparison.InvariantCultureIgnoreCase))
                processedLink.Archived = true;

            yield return processedLink;
        }
    }

    private static byte[] ExportInstapaper(IEnumerable<MasterModel> processedLinks)
    {
        List<InstapaperModel> outputLinks = [];

        foreach (MasterModel link in processedLinks)
        {
            var outputLink = new InstapaperModel
            {
                Title = link.Title,
                URL = link.Url
            };

            if (link.Archived) outputLink.Folder = "Archive";
            else outputLink.Folder = "Unread";

            if (link.Tags.Count != 0) outputLink.Tags = String.Join(",", link.Tags);

            var createdUtc = new DateTimeOffset(link.DateSaved);
            outputLink.Timestamp = createdUtc.ToUnixTimeSeconds();

            outputLinks.Add(outputLink);
        }

        using var outputStream = new MemoryStream();
        using var writer = new StreamWriter(outputStream);
        using var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture);
        csvWriter.WriteRecords(outputLinks);
        csvWriter.Flush();

        return outputStream.ToArray();
    }

    private static byte[] ExportGoodlinks(IEnumerable<MasterModel> processedLinks)
    {
        List<GoodLinksModel> outputLinks = [];

        foreach (MasterModel link in processedLinks)
        {
            var outputLink = new GoodLinksModel
            {
                starred = link.Favorite,
                tags = [.. link.Tags],
                title = link.Title,
                url = link.Url.Replace("http:", "https:")
            };

            if (link.Archived) outputLink.readAt = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();

            var createdUtc = new DateTimeOffset(link.DateSaved);
            outputLink.addedAt = createdUtc.ToUnixTimeSeconds();

            outputLinks.Add(outputLink);
        }

        return JsonSerializer.SerializeToUtf8Bytes(outputLinks);
    }

    private static byte[] ExportRaindrop(IEnumerable<MasterModel> processedLinks)
    {
        List<RaindropModel> outputLinks = [];

        foreach (MasterModel link in processedLinks)
        {
            var outputLink = new RaindropModel
            {
                created = link.DateSaved.ToString("s"),
                favorite = link.Favorite,
                note = link.Notes,
                title = link.Title,
                url = link.Url
            };

            if (link.Archived) outputLink.folder = "Archive";
            else outputLink.folder = "Inbox";

            if (link.Tags.Count != 0)
                outputLink.tags = String.Join(",", link.Tags);

            outputLinks.Add(outputLink);
        }

        using var outputStream = new MemoryStream();
        using var writer = new StreamWriter(outputStream);
        using var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture);
        csvWriter.WriteRecords(outputLinks);
        csvWriter.Flush();

        return outputStream.ToArray();
    }

    private static byte[] ExportReadwise(IEnumerable<MasterModel> processedLinks)
    {
        List<ReadwiseModel> outputLinks = [];

        foreach (MasterModel link in processedLinks)
        {
            var outputLink = new ReadwiseModel
            {
                Title = link.Title,
                URL = link.Url
            };

            var createdUtc = new DateTimeOffset(link.DateSaved);
            outputLink.Timestamp = createdUtc.ToUnixTimeSeconds().ToString();

            if (link.Archived) outputLink.Folder = "Archive";

            outputLinks.Add(outputLink);
        }

        using var outputStream = new MemoryStream();
        using var writer = new StreamWriter(outputStream);
        using var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture);
        csvWriter.WriteRecords(outputLinks);
        csvWriter.Flush();

        return outputStream.ToArray();
    }
}
