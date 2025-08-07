namespace Infrastructure.Models.Web;

public class BodyMetricsModel
{
    public List<string> TimeLine { get; set; } = new List<string>();
    public List<double> Weight { get; set; } = new List<double>();
    public List<double> Body { get; set; } = new List<double>();
}
