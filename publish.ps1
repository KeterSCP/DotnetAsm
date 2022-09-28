[CmdletBinding(PositionalBinding=$false)]
Param(
  [switch][Alias('h')]$help,
  [ValidateSet("win-x64", "win-x86", "win-arm", "win-arm64", "linux-x64", "linux-arm", "linux-arm64", "osx-x64")][string[]]$rids,
  [switch][Alias('sf')]$singlefile = $true,
  [switch][Alias('sc')]$selfcontained = $false
)

function Get-Help() {
  Write-Host "Parameters:"
  Write-Host "  -rids                   Runtime identifiers: win-x64, win-x86, win-arm, win-arm64, linux-x64, linux-arm, linux-arm64 or osx-x64"
  Write-Host "                          Pass a comma-separated list to publish for multiple RIDs"
  Write-Host "  -singlefile             Publish as single file. (default: true)"
  Write-Host "  -selfcontained          Publish self-contained. (default: false)"
}

if ($help -or $rids.Count -eq 0) {
  Get-Help
  exit 0
}

Push-Location "DotnetAsm.Frontend" -PassThru
Invoke-Expression "& npm run build"
Pop-Location -PassThru

foreach ($rid in $rids) {
	$arguments = "publish ./DotnetAsm.Api/DotnetAsm.Api.csproj -c Release -r ${rid} --nologo"

	if ($singlefile) {
		$arguments += " -p:PublishSingleFile=true"
	}
	if ($selfcontained) {
		$arguments += " --self-contained true -p:PublishTrimmed=true"
	}
	else {
		$arguments += " --no-self-contained"
	}
	if ($selfcontained -And $singlefile) {
		$arguments += " -p:EnableCompressionInSingleFile=true"
	}

	$publishDir = "artifacts/${rid}"
	Invoke-Expression "& dotnet ${arguments} -o ${publishDir}"

	Copy-Item -Path "DotnetAsm.Frontend/dist" -Destination ${publishDir} -Recurse -PassThru

	Invoke-Expression "& tar.exe --strip-components 2 -acf ${publishDir}$(if ($selfcontained) {'-selfcontained'} else {''}).zip ${publishDir}"

	rm ${publishDir} -r -Force
}
