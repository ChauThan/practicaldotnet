# Setting Up EF Core
This is a sample guide on how to set up EF Core with sqlite . Here are some important notes:
 - Add a `Target` task to the csproj file to create `sqlite-data` folder after publishing.
```xml
  <Target Name="CreateSqliteDataFolder" AfterTargets="AfterPublish">
    <MakeDir Directories="$(PublishDir)sqlite-data" Condition="!Exists('$(PublishDir)sqlite-data')" />
  </Target>
```
 - The database file is stored in the `sqlite-data` folder. Added a `.gitignore` entry to prevent this file from being committed to the source repository.
Let's get started!