using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace CKAN_Ui3
{
    public class ModInstaller
    {
        private readonly string gameDirectory;

        public ModInstaller(string gameDirectory)
        {
            this.gameDirectory = gameDirectory;
        }

        public async Task InstallModAsync(ModModel mod)
        {
            if (string.IsNullOrEmpty(mod.DownloadUrl))
            {
                throw new ArgumentException("Mod download URL is invalid.");
            }

            string modFilePath = Path.Combine(gameDirectory, "Mods", $"{mod.Name}.zip");

            using HttpClient client = new HttpClient();
            var response = await client.GetAsync(mod.DownloadUrl);
            response.EnsureSuccessStatusCode();

            await using FileStream fs = new FileStream(modFilePath, FileMode.Create);
            await response.Content.CopyToAsync(fs);

            // Placeholder for unzipping logic if required
        }

        public void UninstallMod(ModModel mod)
        {
            string modFolder = Path.Combine(gameDirectory, "Mods", mod.Name);

            if (Directory.Exists(modFolder))
            {
                Directory.Delete(modFolder, true);
            }
        }
    }
}
