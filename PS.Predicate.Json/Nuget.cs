using PS.Build.Nuget.Attributes;

[assembly: Nuget(ProjectUrl = "https://github.com/BlackGad/PS")]
[assembly: Nuget(LicenseUrl = "https://github.com/BlackGad/PS/blob/master/LICENSE")]
[assembly: NugetFilesFromTarget(IncludePDB = true)]
[assembly: NugetPackageDependenciesFromConfiguration]
[assembly: NugetPackageDependenciesFilter("PS.Build*")]
[assembly: NugetPackageDependency("PS.Predicate")]
[assembly: NugetBuild(@"{dir.solution}_Artifacts\{prop.configuration}.{prop.platform}")]
[assembly: NugetDebugSubstitution]
[assembly: Nuget(Tags = "PS")]