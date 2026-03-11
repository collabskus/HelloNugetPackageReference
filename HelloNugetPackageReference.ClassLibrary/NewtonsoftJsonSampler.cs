using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

public static class NewtonsoftJsonSampler
{
    public static void Run()
    {
        // 1. Serialize an object to JSON
        var person = new Person
        {
            Name = "Alice",
            Age = 30,
            Hobbies = ["cycling", "reading"],
            InternalNote = "should not appear",
            Email = "alice@example.com"
        };

        string json = JsonConvert.SerializeObject(person, Formatting.Indented);
        Console.WriteLine("=== Serialized ===");
        Console.WriteLine(json);

        // 2. Deserialize JSON back to an object
        string incoming = """
            {
                "Name": "Bob",
                "Age": 25,
                "Hobbies": ["gaming", "cooking"],
                "contact_email": "bob@example.com"
            }
            """;

        Person bob = JsonConvert.DeserializeObject<Person>(incoming)!;
        Console.WriteLine("\n=== Deserialized ===");
        Console.WriteLine($"Name: {bob.Name}, Age: {bob.Age}, Email: {bob.Email}");
        Console.WriteLine($"Hobbies: {string.Join(", ", bob.Hobbies)}");

        // 3. Deserialize to a dynamic list
        string teamJson = """
            [
                { "Name": "Carol", "Age": 28 },
                { "Name": "Dave",  "Age": 35 }
            ]
            """;

        var team = JsonConvert.DeserializeObject<List<Person>>(teamJson)!;
        Console.WriteLine("\n=== Team ===");
        team.ForEach(p => Console.WriteLine($"  {p.Name} ({p.Age})"));

        // 4. Ad-hoc manipulation with JObject / JToken
        JObject jObj = JObject.Parse(json);
        jObj["Age"] = 31;                        // mutate a value
        jObj["Country"] = "Canada";              // add a new field
        jObj.Remove("Hobbies");                  // remove a field

        Console.WriteLine("\n=== Mutated JObject ===");
        Console.WriteLine(jObj.ToString(Formatting.Indented));

        // 5. Custom serializer settings
        var settings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore,
            Formatting = Formatting.Indented
        };

        var sparse = new Person { Name = "Eve" };   // Age=0, Email=null
        Console.WriteLine("\n=== Sparse (nulls/defaults ignored) ===");
        Console.WriteLine(JsonConvert.SerializeObject(sparse, settings));
    }
}
