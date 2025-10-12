using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace etvctl.Models.Config;

public class FileOrganizationTypeConverter : IYamlTypeConverter
{
    public bool Accepts(Type type)
    {
        return type == typeof(FileOrganization) || type == typeof(FileOrganization?);
    }

    public object? ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
    {
        string value = parser.Consume<Scalar>().Value;
        return value switch
        {
            "single_file" => FileOrganization.SingleFile,
            "file_per_type" => FileOrganization.FilePerType,
            "file_per_resource" => FileOrganization.FilePerResource,
            _ => throw new YamlException($"Invalid FileOrganization value: {value}")
        };
    }

    public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
    {
        var organization = (FileOrganization)value!;
        var yamlValue = organization switch
        {
            FileOrganization.SingleFile => "single_file",
            FileOrganization.FilePerType => "file_per_type",
            FileOrganization.FilePerResource => "file_per_resource",
            _ => throw new ArgumentOutOfRangeException()
        };

        emitter.Emit(new Scalar(yamlValue));
    }
}
