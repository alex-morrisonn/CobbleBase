namespace stu_plugin_api.Models;

public class StudentScoreRequest
{
    public List<double> Scores { get; set; }
    public List<double> Weights { get; set; }
}
