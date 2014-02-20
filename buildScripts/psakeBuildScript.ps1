Properties {
  $build_dir = Split-Path $psake.build_script_file
  $root_dir = "$build_dir\.."
  $src_dir =  "$root_dir\source"
  $release_dir = "$root_dir\_release"
  $sln_path = "$root_dir\LogFlow.sln"
  $Configuration = "Release"
  $nuget_spec = "$src_dir\LogFlow\LogFlow.nuspec"
}



FormatTaskName {
   param($taskName)
   $s="$taskName "
   write-host ($s + ("-"* (70-$s.Length))) -foregroundcolor Cyan
}

Task Deploy -Depends Release, DeployNugetPackageToDev

Task default -Depends Release

Task Build -Depends Compile

Task nuget -Depends CompileNuget, CreateNugetPackage

Task Compile {
  Exec { msbuild $sln_path /p:Configuration=$Configuration /v:quiet }  
}

Task CompileNuget {
  Exec { msbuild $sln_path /p:Configuration=$Configuration /v:quiet /p:OutDir=$release_dir } 
}

Task CreateNugetPackage {
  $nuget_exe = (Get-ChildItem "$root_dir\.nuget\nuget.exe" | Select-Object -First 1)
  Exec { &$nuget_exe "pack" $nuget_spec }
}