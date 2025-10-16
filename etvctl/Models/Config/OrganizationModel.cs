namespace etvctl.Models.Config;

public class OrganizationModel
{
    public FileOrganization? Default { get; set; }
    public OrganizationOverridesModel? Overrides { get; set; }
}
