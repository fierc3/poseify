namespace Core.Services.Estimations
{
    public class EstimationCleanService : IEstimationCleanService
    {

        private readonly ILogger<EstimationCleanService> _logger;
        private readonly IConfiguration _configuration;

        public EstimationCleanService(ILogger<EstimationCleanService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public bool CleanAllData(string estimationId)
        {
            var directory = _configuration["UploadDirectory"];

            if (!Directory.Exists(directory))
            {
                _logger.LogCritical($"UploadDirectory is not valid: {directory}");
                throw new DirectoryNotFoundException("UploadDirectory not found: {directory}");
            }

            string[] files = Directory.GetFiles(directory);
            bool allDeleted = true;
            foreach (var file in
            // Loop through each file in the directory
            from string file in files// Check if the filename contains the specified GUID and is followed up with a .
            where Path.GetFileName(file).Contains(estimationId.ToString() + ".")
            select file)
            {
                try
                {
                    // Delete the file
                    File.Delete(file);
                    _logger.LogDebug($"Deleted file: {file}");
                }
                catch (IOException ex)
                {
                    // Handle potential IO exceptions, like file being in use
                    _logger.LogWarning($"Error deleting file {file}: {ex.Message}");
                    allDeleted = false;
                }
            }

            return allDeleted;
        }
    }
}
