using Newtonsoft.Json;

namespace HelloNugetPackageReference.ClassLibrary;

public class Person
{
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public string[] Hobbies { get; set; } = [];

    [JsonIgnore]
    public string InternalNote { get; set; } = "hidden";

    [JsonProperty("contact_email")]
    public string Email { get; set; } = string.Empty;
}
