using HelloNugetPackageReference.ClassLibrary;
using Newtonsoft.Json;

NewtonsoftJsonSampler.Run();

Person person = new Person
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

Person? newPerson = JsonConvert.DeserializeObject<Person>(incoming);
if (null !=  newPerson)
{
    Console.WriteLine(newPerson.Name);
}
