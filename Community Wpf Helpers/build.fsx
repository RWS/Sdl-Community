#r @"tools/FAKE.Core/tools/FakeLib.dll"

open Fake
open System

let authors = ["Sdl Community Developers"]

//project details
let projectName = "Sdl Community WPF Helpers"
let projectDescription="The Sdl Community WPF Helpers is a collection of helper functions. It simplifies and demonstrates common developer tasks building SDL Studio plugins."
let projectSummary = projectDescription

//directories
let buildDir = "./build"
let packagingRoot = "./packaging"
let packagingDir = packagingRoot @@ "sdlcommunitywpfhelpers"

let releaseNotes = 
    ReadFile "ReleaseNotes.md"
    |> ReleaseNotesHelper.parseReleaseNotes

let buildMode = getBuildParamOrDefault "buildMode" "Release"

MSBuildDefaults <-{
    MSBuildDefaults with 
        ToolsVersion = Some "15.0"
        Verbosity = Some MSBuildVerbosity.Minimal
        
}

Target "Clean"(fun _ ->
    CleanDirs[buildDir;packagingRoot;packagingDir]
)

open Fake.AssemblyInfoFile

Target "AssemblyInfo" (fun _ ->
    CreateCSharpAssemblyInfo "./SolutionInfo.cs"
      [ Attribute.Product projectName
        Attribute.Version releaseNotes.AssemblyVersion
        Attribute.FileVersion releaseNotes.AssemblyVersion
        Attribute.ComVisible false ]
)

let setParams defaults = {
    defaults with
        ToolsVersion = Some("15.0")
        Targets = ["Build"]
        Properties =
            [
                "Configuration", buildMode
                "OutputPath" , "./../build"
            ]
        
    }

Target "BuildApp" (fun _ ->
    build setParams "./Sdl.CommunityWpfHelpers.sln"
        |> DoNothing
)
//Publishing is not working from build script because of an issue in FAKE with 
//latest nuget versions - https://github.com/fsharp/FAKE/issues/1241
Target "CreateWpfHelperPackage" (fun _ ->
    let portableDir = packagingDir @@ "lib/net47/"
    CleanDirs [portableDir]

    CopyFile portableDir (buildDir @@ "Sdl.CommunityWpfHelpers.dll")
    CopyFile portableDir (buildDir @@ "Sdl.CommunityWpfHelpers.xml")
    CopyFile portableDir (buildDir @@ "Sdl.CommunityWpfHelpers.pdb")
    CopyFiles packagingDir ["LICENSE"; "README.md"; "ReleaseNotes.md"]

    NuGet (fun p -> 
        {p with
            Authors = authors
            Project = "Sdl.CommunityWpfHelpers"
            Description = projectDescription
            OutputPath = packagingRoot
            Summary = projectSummary
            WorkingDir = packagingDir
            Version = releaseNotes.AssemblyVersion
            ReleaseNotes = toLines releaseNotes.Notes
            AccessKey = getBuildParamOrDefault "nugetkey" ""
            Publish = hasBuildParam "nugetkey" }) "Sdl.CommunityWpfHelpers.nuspec"
)

Target "CreatePackages" DoNothing

Target "BuildAndCreatePackages" DoNothing

"BuildApp"
    ==> "CreateWpfHelperPackage"   
    ==> "CreatePackages"
    ==> "BuildAndCreatePackages"

"CreateWpfHelperPackage"
    ==> "CreatePackages"

"Clean"
   ==> "AssemblyInfo"
   ==> "BuildApp"

RunTargetOrDefault "BuildApp"