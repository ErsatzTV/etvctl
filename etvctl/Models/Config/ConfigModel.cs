namespace etvctl.Models.Config;

public class ConfigModel
{
    public string Server { get; set; } = "http://localhost:8409";
    public string? Template { get; set; }
    public OrganizationModel? Organization { get; set; }
}
