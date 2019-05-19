namespace Api.Settings
{
    public class StorageSettings
    {
        public string AzureStorageConnectionString { get; set; }
        public int MaxUploadSizeMb { get; set; }
        public string[] AuthorizedImageFormats { get; set; }
    }
}