$projects = Get-ChildItem -Recurse -Filter *.csproj
foreach ($proj in $projects) {
    dotnet pack $proj.FullName -o ./nupkgs
}