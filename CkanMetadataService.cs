using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace CKAN_Ui3
{
    public class CkanMetadataService
    {
        private const string MetadataUrl = "https://codeload.github.com/KSP-CKAN/CKAN-meta/zip/refs/heads/master";
        private const string TempZipPath = "ckan_meta.zip";
        private const string ExtractedFolderPath = "CKAN-meta";

        public async Task<List<ModModel>> LoadCkanMetadataAsync()
        {
            List<ModModel> mods;
            try
            {
                await DownloadMetadataZipAsync();
                ExtractMetadataZip();
                mods = await ParseAllCkanFilesAsync();
            }
            catch
            {
                mods = GetSampleMods(); // Load sample mods on failure
            }
            return mods;
        }

        private async Task DownloadMetadataZipAsync()
        {
            using HttpClient client = new HttpClient();
            var response = await client.GetAsync(MetadataUrl);
            response.EnsureSuccessStatusCode();

            await using FileStream fs = new FileStream(TempZipPath, FileMode.Create);
            await response.Content.CopyToAsync(fs);
        }

        private void ExtractMetadataZip()
        {
            if (Directory.Exists(ExtractedFolderPath))
            {
                Directory.Delete(ExtractedFolderPath, true);
            }
            ZipFile.ExtractToDirectory(TempZipPath, ExtractedFolderPath);
        }

        private async Task<List<ModModel>> ParseAllCkanFilesAsync()
        {
            var mods = new List<ModModel>();
            foreach (var filePath in Directory.GetFiles(ExtractedFolderPath, "*.ckan", SearchOption.AllDirectories))
            {
                var mod = await ParseCkanFileAsync(filePath);
                if (mod != null)
                {
                    mods.Add(mod);
                }
            }
            return mods;
        }

        private async Task<ModModel> ParseCkanFileAsync(string filePath)
        {
            await using FileStream stream = File.OpenRead(filePath);
            var modData = await JsonSerializer.DeserializeAsync<CkanModData>(stream);
            return modData != null ? ConvertToModModel(modData) : null;
        }

        private ModModel ConvertToModModel(CkanModData modData)
        {
            return new ModModel
            {
                Name = modData.identifier,
                Author = modData.author,
                Description = modData.description,
                Compatibility = modData.ksp_version,
                DownloadUrl = modData.download
            };
        }

        private List<ModModel> GetSampleMods()
        {
            return new List<ModModel>
            {
                new ModModel
                {
                    Name = "Kerbal Engineer Redux",
                    Author = "C. Jenkins",
                    Description = "Provides delta-v information to help with planning maneuvers.",
                    Compatibility = "1.11",
                    DownloadUrl = "https://example.com/download/kerbal_engineer.zip"
                },
                new ModModel
                {
                    Name = "MechJeb 2",
                    Author = "r4m0n",
                    Description = "Adds autopilot, customizable HUD, and other useful tools.",
                    Compatibility = "1.11",
                    DownloadUrl = "https://example.com/download/mechjeb2.zip"
                },
                new ModModel
                {
                    Name = "Environmental Visual Enhancements",
                    Author = "WazWaz",
                    Description = "Enhances visual atmospheric effects for a realistic look.",
                    Compatibility = "1.10",
                    DownloadUrl = "https://example.com/download/eve.zip"
                },
                new ModModel
                {
                    Name = "Real Solar System",
                    Author = "NathanKell",
                    Description = "Transforms Kerbol into a realistic model of our Solar System.",
                    Compatibility = "1.8",
                    DownloadUrl = "https://example.com/download/rss.zip"
                }
            };
        }
    }

    public class CkanModData
    {
        public string identifier { get; set; }
        public string author { get; set; }
        public string description { get; set; }
        public string ksp_version { get; set; }
        public string download { get; set; }
    }
}
