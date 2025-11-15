# Dev helper script: clean, restore, build and run the minimal-api
# Usage: powershell -ExecutionPolicy Bypass -File .\scripts\run-dev.ps1

Write-Host "[run-dev] Parando processos dotnet (se existirem)..."
Get-Process dotnet -ErrorAction SilentlyContinue | ForEach-Object {
    try {
        Stop-Process -Id $_.Id -Force -ErrorAction Stop
        Write-Host "[run-dev] Parado dotnet (PID $($_.Id))"
    } catch {
        Write-Host "[run-dev] Não foi possível parar dotnet PID $($_.Id): $($_.Exception.Message)"
    }
}

Write-Host "[run-dev] Removendo bin/obj do projeto minimal-api..."
Remove-Item -Path ".\minimal-api\bin" -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item -Path ".\minimal-api\obj" -Recurse -Force -ErrorAction SilentlyContinue

Write-Host "[run-dev] Limpando solution..."
dotnet clean ".\AspNetMinimalsApi.sln"

Write-Host "[run-dev] Restaurando pacotes..."
dotnet restore ".\minimal-api\minimal-api.csproj"

Write-Host "[run-dev] Compilando solution (Debug)..."
dotnet build ".\AspNetMinimalsApi.sln" -c Debug
if ($LASTEXITCODE -ne 0) {
    Write-Error "[run-dev] Build falhou. Abortando."
    exit $LASTEXITCODE
}

Write-Host "[run-dev] Iniciando API (minimal-api)..."
# Executa em primeiro plano para evitar locks e mostrar logs.
# Para encerrar, use Ctrl+C neste terminal.
dotnet run --project ".\minimal-api\minimal-api.csproj"

if ($LASTEXITCODE -ne 0) {
    Write-Error "[run-dev] Falha ao iniciar a API (exit code $LASTEXITCODE)."
    exit $LASTEXITCODE
}

Write-Host "[run-dev] API finalizada."