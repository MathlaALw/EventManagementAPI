namespace EventManagementAPI.DTOs
{
    public class DailyBlock
    {
        public string[]? time { get; set; }
        public double[]? temperature_2m_max { get; set; }
        public double[]? temperature_2m_min { get; set; }
        public int[]? weathercode { get; set; }
    }
}
