namespace MIS.Models.DB
{
    public class DbTokenBlackList
    {
        public Guid id { get; set; }
        public string token {  get; set; }
        public DateTime expirationTime { get; set; } = DateTime.UtcNow;
    }
}
