Framework 4.5.1
Include "packages\Hangfire.Build.0.2.6\tools\psake-common.ps1"

Task Default -Depends Collect

Task RestoreCore {
    Exec { dotnet restore }
}

Task CompileCore -Depends RestoreCore {
    Exec { dotnet build -c Release }
}

Task Test -Depends CompileCore -Description "Run unit and integration tests." {
    # Exec { dotnet test "tests\Hangfire.QuickJump.Tests\Hangfire.QuickJump.Tests.csproj" }
}

Task Collect -Depends Test -Description "Copy all artifacts to the build folder." {
    Collect-Assembly "Hangfire.QuickJump" "net45"
    Collect-Assembly "Hangfire.QuickJump" "netstandard1.3"
    Collect-Assembly "Hangfire.QuickJump" "netstandard2.0"

    Collect-File "LICENSE"
}

Task Pack -Depends Collect -Description "Create NuGet packages and archive files." {
    $version = Get-PackageVersion

    Create-Archive "Hangfire.QuickJump-$version"

    Create-Package "Hangfire.QuickJump" $version
}
