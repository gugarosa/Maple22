param(
  [switch]$Fix
)

Write-Host "Running dotnet format..." -ForegroundColor Cyan

$verify = $Fix ? "" : "--verify-no-changes"

dotnet format whitespace $verify --exclude 'Maple2.Server.World/Migrations/*.cs'
dotnet format style $verify --severity info --exclude 'Maple2.Server.World/Migrations/*.cs'
dotnet format analyzers $verify --severity info --exclude 'Maple2.Server.World/Migrations/*.cs'

if ($LASTEXITCODE -eq 0) { Write-Host "Formatting OK" -ForegroundColor Green } else { exit $LASTEXITCODE }

