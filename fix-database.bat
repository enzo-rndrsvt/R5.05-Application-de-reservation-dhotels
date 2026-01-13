@echo off
echo ========================================
echo CORRECTION URGENTE BASE DE DONNEES
echo Ajout colonne ImageUrl a la table Rooms
echo ========================================

echo.
echo ?? Execution du script de correction...
echo.

REM Remplacez les paramètres suivants par vos vraies valeurs:
REM -S: Nom du serveur SQL
REM -d: Nom de la base de données
REM -E: Authentification Windows (ou utilisez -U username -P password)

sqlcmd -S localhost -d HotelBookingDb -E -i URGENT_FIX_ImageUrl.sql

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ? CORRECTION RÉUSSIE !
    echo ?? Vous pouvez maintenant créer des chambres !
) else (
    echo.
    echo ? ERREUR lors de l'exécution du script
    echo ?? Vérifiez vos paramètres de connexion à la base de données
)

pause