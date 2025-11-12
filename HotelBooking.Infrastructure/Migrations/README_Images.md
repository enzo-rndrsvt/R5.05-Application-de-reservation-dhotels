# Script pour appliquer la migration ImageUrl

Ce dossier contient les fichiers pour ajouter le support des images aux chambres.

## Pour appliquer la migration à la base de données :

1. Ouvrir un terminal dans le dossier racine du projet
2. Exécuter : `cd HotelBooking.Infrastructure`  
3. Exécuter : `dotnet ef database update --startup-project ../HotelBooking.API`

Ou directement depuis l'API :
4. Exécuter : `cd HotelBooking.API`
5. Exécuter : `dotnet ef database update --project ../HotelBooking.Infrastructure`

## Fonctionnalités ajoutées :

- ? Colonne `ImageUrl` ajoutée à la table Rooms
- ? Service d'upload d'images (`ImageUploadService`)
- ? Interface d'upload dans la création de chambre
- ? Affichage des images dans la liste des chambres
- ? Images stockées dans `/wwwroot/uploads/rooms/`
- ? Validation : formats JPG, PNG, GIF, WebP - max 5MB

## Test :

1. Se connecter en tant qu'admin
2. Aller dans `/admin/rooms/create`
3. Remplir le formulaire et uploader une image
4. Vérifier l'affichage dans `/rooms`