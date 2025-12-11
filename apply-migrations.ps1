# Script PowerShell pour appliquer les migrations de base de données
# Utilisé par la page d'administration pour mettre à jour la BDD

param(
    [Parameter(Mandatory=$false)]
    [string]$Action = "update"
)

Write-Host "???  Script de gestion de base de données - HotelBooking" -ForegroundColor Green
Write-Host "===============================================" -ForegroundColor Green

# Vérifier que nous sommes dans le bon répertoire
if (-not (Test-Path "HotelBooking.sln")) {
    Write-Host "? Erreur: Script doit être exécuté depuis la racine du projet" -ForegroundColor Red
    Write-Host "?? Répertoire actuel: $(Get-Location)" -ForegroundColor Yellow
    exit 1
}

Write-Host "?? Répertoire de travail: $(Get-Location)" -ForegroundColor Blue

switch ($Action.ToLower()) {
    "update" {
        Write-Host "?? Application des migrations..." -ForegroundColor Yellow
        
        try {
            # Se déplacer dans le projet Infrastructure
            Push-Location "HotelBooking.Infrastructure"
            
            Write-Host "?? Vérification des outils EF..." -ForegroundColor Blue
            
            # Vérifier si dotnet-ef est installé
            $efInstalled = Get-Command "dotnet-ef" -ErrorAction SilentlyContinue
            if (-not $efInstalled) {
                Write-Host "??  Installation de dotnet-ef..." -ForegroundColor Yellow
                dotnet tool install --global dotnet-ef
            }
            
            Write-Host "?? Application des migrations vers la base de données..." -ForegroundColor Green
            dotnet ef database update --startup-project ../HotelBooking.API
            
            if ($LASTEXITCODE -eq 0) {
                Write-Host "? Migrations appliquées avec succès!" -ForegroundColor Green
            } else {
                Write-Host "? Erreur lors de l'application des migrations" -ForegroundColor Red
                exit 1
            }
        }
        catch {
            Write-Host "? Exception: $($_.Exception.Message)" -ForegroundColor Red
            exit 1
        }
        finally {
            Pop-Location
        }
    }
    
    "list" {
        Write-Host "?? Liste des migrations..." -ForegroundColor Yellow
        
        Push-Location "HotelBooking.Infrastructure"
        dotnet ef migrations list --startup-project ../HotelBooking.API
        Pop-Location
    }
    
    "script" {
        Write-Host "?? Génération du script SQL..." -ForegroundColor Yellow
        
        $outputFile = "migration-script-$(Get-Date -Format 'yyyyMMdd-HHmmss').sql"
        
        Push-Location "HotelBooking.Infrastructure"
        dotnet ef migrations script --startup-project ../HotelBooking.API --output "../$outputFile"
        Pop-Location
        
        Write-Host "? Script généré: $outputFile" -ForegroundColor Green
    }
    
    "help" {
        Write-Host "?? Aide - Actions disponibles:" -ForegroundColor Cyan
        Write-Host "  update  - Applique les migrations à la BDD" -ForegroundColor White
        Write-Host "  list    - Liste toutes les migrations" -ForegroundColor White
        Write-Host "  script  - Génère un script SQL" -ForegroundColor White
        Write-Host "  help    - Affiche cette aide" -ForegroundColor White
        Write-Host ""
        Write-Host "Exemples:" -ForegroundColor Yellow
        Write-Host "  .\apply-migrations.ps1 update" -ForegroundColor Gray
        Write-Host "  .\apply-migrations.ps1 list" -ForegroundColor Gray
    }
    
    default {
        Write-Host "? Action inconnue: $Action" -ForegroundColor Red
        Write-Host "?? Utilisez 'help' pour voir les actions disponibles" -ForegroundColor Yellow
        exit 1
    }
}

Write-Host ""
Write-Host "????? SkullKing Database Manager - Terminé!" -ForegroundColor Green