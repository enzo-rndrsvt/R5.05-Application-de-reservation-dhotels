namespace HotelBooking.Web.Services
{
    public interface IDatabaseManagementService
    {
        /// <summary>
        /// Apply pending migrations to the database
        /// </summary>
        Task<bool> ApplyMigrationsAsync();
        
        /// <summary>
        /// Seed test data to the database
        /// </summary>
        Task<bool> SeedTestDataAsync();
        
        /// <summary>
        /// Clean up orphaned files and temporary data
        /// </summary>
        Task<bool> CleanupDataAsync();
        
        /// <summary>
        /// Create a backup of the database
        /// </summary>
        Task<string?> CreateBackupAsync();
        
        /// <summary>
        /// Get database status information
        /// </summary>
        Task<DatabaseStatus> GetDatabaseStatusAsync();
    }
    
    public class DatabaseStatus
    {
        public bool IsConnected { get; set; }
        public string LastMigration { get; set; } = string.Empty;
        public DateTime LastMigrationDate { get; set; }
        public int TotalTables { get; set; }
        public int TotalRecords { get; set; }
        public long DatabaseSize { get; set; }
        public List<string> PendingMigrations { get; set; } = new();
    }
}