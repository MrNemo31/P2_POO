param(
    [int]$PortaWeb = 5000
)

$ErrorActionPreference = 'Stop'
$projectPath = Join-Path $PSScriptRoot 'LiteArt.Poo\LiteArt.Poo.csproj'
$configPath = Join-Path $PSScriptRoot 'LiteArt.Poo\appsettings.Local.json'
$toolsRoot = Join-Path ([Environment]::GetFolderPath('MyDocuments')) 'Codex\work'
$dotnetPath = Join-Path $toolsRoot 'liteart-tools\dotnet\dotnet.exe'
$pgBin = Join-Path $toolsRoot 'liteart-postgres-test\pgsql\bin'
$clusterPath = Join-Path $env:LOCALAPPDATA 'LiteArtPoo\postgres'

foreach ($requiredPath in @($projectPath, $configPath, $dotnetPath, (Join-Path $pgBin 'pg_ctl.exe'), (Join-Path $clusterPath 'PG_VERSION'))) {
    if (!(Test-Path -LiteralPath $requiredPath)) {
        throw "Arquivo local necessario nao encontrado: $requiredPath"
    }
}

$config = Get-Content -LiteralPath $configPath -Raw | ConvertFrom-Json
$connection = $config.ConnectionStrings.DefaultConnection
if ([string]::IsNullOrWhiteSpace($connection)) {
    throw 'Configure ConnectionStrings.DefaultConnection em LiteArt.Poo\appsettings.Local.json.'
}
$connectionParts = [System.Data.Common.DbConnectionStringBuilder]::new()
$connectionParts.set_ConnectionString($connection)
if ([string]$connectionParts.get_Item('Host') -ne '127.0.0.1') {
    throw 'Este iniciador foi preparado para o banco local em 127.0.0.1.'
}
$databasePort = [int]$connectionParts.get_Item('Port')

$previousPassword = $env:PGPASSWORD
$previousRoot = $env:DOTNET_ROOT
$previousCliHome = $env:DOTNET_CLI_HOME
try {
    & (Join-Path $pgBin 'pg_ctl.exe') -D $clusterPath status *> $null
    if ($LASTEXITCODE -ne 0) {
        Write-Host 'Iniciando PostgreSQL local...'
        & (Join-Path $pgBin 'pg_ctl.exe') -D $clusterPath -l (Join-Path $env:LOCALAPPDATA 'LiteArtPoo\postgres.log') -o "-h 127.0.0.1 -p $databasePort" -w start
        if ($LASTEXITCODE -ne 0) { throw 'Nao foi possivel iniciar o PostgreSQL. Consulte o log em %LOCALAPPDATA%\LiteArtPoo\postgres.log.' }
    }

    $env:PGPASSWORD = [string]$connectionParts.get_Item('Password')
    $queryArgs = @('-h', '127.0.0.1', '-p', "$databasePort", '-U', [string]$connectionParts.get_Item('Username'), '-d', [string]$connectionParts.get_Item('Database'), '-w', '-X', '-q', '-A', '-t', '-v', 'ON_ERROR_STOP=1')
    $check = & (Join-Path $pgBin 'psql.exe') @queryArgs -c "SELECT to_regclass('public.administradores') IS NOT NULL AND to_regclass('public.materias') IS NOT NULL AND to_regclass('public.alunos') IS NOT NULL;"
    if ($LASTEXITCODE -ne 0) { throw 'Falha ao conectar ao banco. Confira a DefaultConnection no arquivo local.' }
    if (($check | Out-String).Trim() -ne 't') { throw 'O banco nao tem as tabelas necessarias. Verifique a configuracao local.' }

    $webConnection = [Net.Sockets.TcpClient]::new()
    $portOccupied = $false
    try {
        $webConnection.Connect('127.0.0.1', $PortaWeb)
        $portOccupied = $true
    } catch [Net.Sockets.SocketException] {
        # Nenhum servidor escutando nessa porta.
    } finally {
        $webConnection.Dispose()
    }
    if ($portOccupied) {
        throw "A porta $PortaWeb ja esta em uso. Pare a aplicacao antiga com Ctrl+C antes de iniciar novamente."
    }

    $env:DOTNET_ROOT = Split-Path -Parent $dotnetPath
    $env:DOTNET_CLI_HOME = Join-Path $toolsRoot 'liteart-tools\cli-home'
    Write-Host "Banco conectado. Abrindo o projeto em http://localhost:$PortaWeb"
    Write-Host 'Mantenha este terminal aberto. Use Ctrl+C para parar a aplicacao.'
    & $dotnetPath run --project $projectPath --no-launch-profile -- --urls "http://localhost:$PortaWeb"
    if ($LASTEXITCODE -ne 0) { throw 'A aplicacao encerrou com erro. Confira as mensagens acima e se a porta ja esta ocupada.' }
} finally {
    $env:PGPASSWORD = $previousPassword
    $env:DOTNET_ROOT = $previousRoot
    $env:DOTNET_CLI_HOME = $previousCliHome
}
