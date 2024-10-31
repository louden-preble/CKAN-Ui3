namespace CKAN_Ui3
{
    public class ModModel
    {
        public string Name { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public string ShortDescription => Description.Length > 50 ? Description.Substring(0, 50) + "..." : Description;
        public string Compatibility { get; set; }
        public string DownloadUrl { get; set; }
    }
}
