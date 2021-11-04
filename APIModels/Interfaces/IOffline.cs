namespace SOEWeb.Shared.Interfaces
{
    public static class OfflineConstants
    {
        public const int IdBase = 9000;
    }
    public interface IOffline
    {
        public bool IsOffline { get; set; }
    }
}
