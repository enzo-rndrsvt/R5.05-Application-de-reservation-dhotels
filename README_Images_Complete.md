# ??? Upload d'Images pour les Chambres - HotelBooking SkullKing

## ? **Fonctionnalités Implémentées**

### ?? **Upload d'Images**
- ? Interface d'upload dans la création de chambre (`/admin/rooms/create`)
- ? Validation des formats : JPG, PNG, GIF, WebP
- ? Taille maximale : 5MB
- ? Stockage local dans `/wwwroot/uploads/rooms/`
- ? Noms de fichiers uniques (GUID)
- ? Aperçu temps réel de l'image uploadée
- ? Bouton pour changer/supprimer l'image

### ?? **Affichage des Images**
- ? Images affichées dans la liste des chambres (`/rooms`)
- ? Images redimensionnées automatiquement (200px height)
- ? Placeholder élégant si pas d'image
- ? Affichage dans la modal de détails
- ? Responsive design

### ??? **Base de Données**
- ? Migration `AddImageUrlToRooms` créée
- ? Colonne `ImageUrl` ajoutée à la table `Rooms`
- ? Configuration Entity Framework mise à jour
- ? Tous les modèles DTO mis à jour

### ?? **Administration**
- ? Page d'administration BDD (`/admin/database`)
- ? Interface pour appliquer les migrations
- ? Service de gestion de base de données
- ? Menu admin réorganisé
- ? Script PowerShell pour les migrations

### ?? **Tests & Debug**
- ? Page de test d'upload (`/test-images`)
- ? Validation côté client et serveur
- ? Gestion des erreurs complète
- ? Pages de debug API maintenues

---

## ?? **Comment Utiliser**

### 1. **Appliquer les Migrations**
```bash
# Option 1: Via PowerShell (recommandé)
.\apply-migrations.ps1 update

# Option 2: Via ligne de commande
cd HotelBooking.Infrastructure
dotnet ef database update --startup-project ../HotelBooking.API

# Option 3: Via l'interface admin
# Aller sur /admin/database et cliquer sur "Appliquer les migrations"
```

### 2. **Créer une Chambre avec Image**
1. Se connecter en tant qu'admin
2. Aller sur `/admin/rooms/create`
3. Remplir le formulaire
4. Upload une image (formats supportés: JPG, PNG, GIF, WebP)
5. Sauvegarder

### 3. **Voir les Chambres**
1. Aller sur `/rooms`
2. Les images s'affichent automatiquement
3. Cliquer sur "Détails" pour voir l'image en grand

---

## ?? **Structure des Fichiers**

### **Nouveaux Fichiers Créés :**
```
HotelBooking.Web/
??? Services/
?   ??? IImageUploadService.cs          # Interface service upload
?   ??? ImageUploadService.cs           # Implémentation upload
?   ??? IDatabaseManagementService.cs   # Interface admin BDD
?   ??? DatabaseManagementService.cs    # Implémentation admin BDD
??? Components/Pages/
?   ??? AdminDatabase.razor             # Page admin BDD
?   ??? TestImages.razor                # Page test upload
??? wwwroot/uploads/
    ??? rooms/                          # Images des chambres
    ??? test/                           # Images de test

HotelBooking.Infrastructure/
??? Migrations/
?   ??? 20241211151800_AddImageUrlToRooms.cs
?   ??? 20241211151800_AddImageUrlToRooms.Designer.cs
?   ??? README_Images.md
??? Extensions/
    ??? ModelBuilderExtensions.cs       # Configuration ImageUrl

HotelBooking.Domain/
??? Models/
    ??? RoomAvailabilityInfo.cs         # Modèle disponibilité
    ??? Room/ (modèles mis à jour)

apply-migrations.ps1                     # Script migrations PowerShell
```

### **Fichiers Modifiés :**
- `HotelBooking.Web/Models/RoomCreation.cs` - Ajout IBrowserFile
- `HotelBooking.Web/Models/Room.cs` - Ajout ImageUrl
- `HotelBooking.Web/Components/Pages/Rooms.razor` - Affichage images
- `HotelBooking.Web/Components/Pages/AdminCreateRoom.razor` - Upload interface
- `HotelBooking.Web/Components/Layout/NavMenu.razor` - Menu admin
- `HotelBooking.Web/Program.cs` - Enregistrement services
- Tous les DTO/Tables - Ajout ImageUrl

---

## ?? **Prochaines Améliorations Possibles**

### **Court terme :**
- [ ] Redimensionnement automatique des images
- [ ] Support de plusieurs images par chambre
- [ ] Compression des images
- [ ] Galerie d'images dans les détails

### **Long terme :**
- [ ] Stockage cloud (Azure Blob, AWS S3)
- [ ] CDN pour les images
- [ ] Outil de recadrage d'image
- [ ] Images en WebP automatique
- [ ] Lazy loading des images

---

## ?? **Debug & Troubleshooting**

### **Si les images ne s'affichent pas :**
1. Vérifier que la migration est appliquée
2. Vérifier les permissions du dossier `/wwwroot/uploads/`
3. Vérifier que les URLs sont correctes (commence par `/uploads/`)

### **Pages de test disponibles :**
- `/test-images` - Test upload d'images
- `/debug-api` - Test API de disponibilité
- `/admin/database` - Administration BDD

### **Logs :**
Les logs sont disponibles dans la console pour le développement.

---

## ????? **Fait par l'équipage du SkullKing !**

Le système d'upload d'images est maintenant prêt pour que les pirates puissent voir leurs futures cabines ! ??????