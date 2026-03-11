# HelloNugetPackageReference

A minimal, verifiable .NET 10 solution that proves a **`<PackageReference>`** in a class library project causes the referenced NuGet package's DLL to appear in the **consuming console app's `bin/` output folder** — ready for runtime use.

The package under test is [Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json/), one of the most widely used NuGet packages in the .NET ecosystem.

---

## Why Does This Repo Exist?

When you add a `<PackageReference>` to a **class library** (`.csproj`), a natural question arises:

> *"Will the NuGet package's DLL actually end up in the output folder of any application that references this library?"*

The answer is **yes** — but it's worth having a concrete, runnable proof.

This repo exists to confirm exactly that. When you build and run the console app, you can open its `bin/` folder and see `Newtonsoft.Json.dll` sitting right there alongside the app's own output, even though the `<PackageReference>` lives exclusively in the class library's `.csproj`.

This matters in practice because:

- You may be unsure whether transitive NuGet dependencies flow through project references.
- You want to confirm the DLL is present before deploying, publishing, or packaging.
- You want a clean, reproducible baseline to demonstrate the behavior to a colleague or team.

---

## Solution Structure

```
HelloNugetPackageReference/
├── ConsoleApp/
│   ├── ConsoleApp.csproj        ← References ClassLib project; no direct NuGet refs
│   └── Program.cs               ← Entry point; calls NewtonsoftJsonSampler.Run()
│
└── ClassLib/
    ├── ClassLib.csproj           ← Declares <PackageReference Include="Newtonsoft.Json" />
    └── NewtonsoftJsonSampler.cs  ← Exercises Newtonsoft.Json APIs
```

### Key Design Choice

The `<PackageReference>` for Newtonsoft.Json is declared **only** in `ClassLib.csproj`:

```xml
<!-- ClassLib/ClassLib.csproj -->
<ItemGroup>
  <PackageReference Include="Newtonsoft.Json" />
</ItemGroup>
```

`ConsoleApp.csproj` has **no direct NuGet reference** to Newtonsoft.Json. It only references the class library:

```xml
<!-- ConsoleApp/ConsoleApp.csproj -->
<ItemGroup>
  <ProjectReference Include="..\ClassLib\ClassLib.csproj" />
</ItemGroup>
```

This is the entire point: the NuGet dependency travels **transitively** from the library into the app's output.

---

## What the Code Demonstrates

`NewtonsoftJsonSampler.cs` walks through five practical Newtonsoft.Json scenarios:

| Step | Feature Demonstrated |
|------|----------------------|
| 1 | `JsonConvert.SerializeObject` with `Formatting.Indented` |
| 2 | `[JsonIgnore]` and `[JsonProperty]` attributes |
| 3 | `JsonConvert.DeserializeObject<T>` from a JSON string |
| 4 | Deserializing into `List<T>` |
| 5 | Dynamic `JObject` / `JToken` manipulation (mutate, add, remove fields) |
| 6 | `JsonSerializerSettings` for null/default value handling |

The code is intentionally simple and self-contained — it's a sampler, not a full application.

---

## How to Run

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)

### Build & Run

```bash
git clone https://github.com/collabskus/HelloNugetPackageReference.git
cd HelloNugetPackageReference

dotnet run --project ConsoleApp
```

### Verify the DLL is in `bin/`

After building, inspect the console app's output folder:

```bash
# Build without running
dotnet build ConsoleApp

# List the output folder
ls ConsoleApp/bin/Debug/net10.0/
```

You should see `Newtonsoft.Json.dll` in the output — even though `ConsoleApp.csproj` never directly referenced it.

On Windows (PowerShell):
```powershell
Get-ChildItem .\ConsoleApp\bin\Debug\net10.0\ | Where-Object { $_.Name -like "*Newtonsoft*" }
```

On macOS/Linux:
```bash
ls ConsoleApp/bin/Debug/net10.0/ | grep Newtonsoft
```

Expected output:
```
Newtonsoft.Json.dll
```

---

## Expected Console Output

```
=== Serialized ===
{
  "Name": "Alice",
  "Age": 30,
  "Hobbies": [
    "cycling",
    "reading"
  ],
  "contact_email": "alice@example.com"
}

=== Deserialized ===
Name: Bob, Age: 25, Email: bob@example.com
Hobbies: gaming, cooking

=== Team ===
  Carol (28)
  Dave (35)

=== Mutated JObject ===
{
  "Name": "Alice",
  "Age": 31,
  "contact_email": "alice@example.com",
  "Country": "Canada"
}

=== Sparse (nulls/defaults ignored) ===
{
  "Name": "Eve"
}
```

---

## How Transitive NuGet Dependencies Work

When the .NET SDK restores packages for a solution, it resolves a full **dependency graph**. A `<ProjectReference>` causes the referencing project to inherit the referenced project's NuGet closure. This means:

1. `ClassLib` declares `Newtonsoft.Json` as a direct dependency.
2. `ConsoleApp` declares `ClassLib` as a project reference.
3. At build time, the SDK copies all DLLs needed by `ClassLib` — including `Newtonsoft.Json.dll` — into `ConsoleApp`'s output folder.

This is standard MSBuild/NuGet behavior for SDK-style projects and applies to all `<PackageReference>` entries, not just Newtonsoft.Json.

---

## Target Framework

Both projects target **`net10.0`** with nullable reference types and implicit usings enabled.

---

## License

MIT
