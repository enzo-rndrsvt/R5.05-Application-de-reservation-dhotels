# ?? Support des Images Multiples pour les Chambres - Documentation

## ? Fonctionnalités Implémentées

### ??? **1. Interface d'Upload Multiple**
- **Upload jusqu'à 5 images** simultanément
- **Validation des fichiers** : formats (JPG, PNG, GIF, WebP) et taille (max 5MB)
- **Barre de progression** en temps réel avec compteur d'upload
- **Gestion des erreurs** détaillée avec messages utilisateur

### ?? **2. Gestion Visuelle des Images**
- **Aperçu en temps réel** avec miniatures de 150px de hauteur
- **Réorganisation par glisser-déposer** avec boutons ?/?
- **Suppression individuelle** avec bouton corbeille
- **Badge de comptage** (ex: "Image 1/5")
- **Image principale automatique** (première de la liste)

### ?? **3. Logique Métier**
- **Limite stricte** : maximum 5 images par chambre
- **Rétrocompatibilité** : `ImageUrl` reste pour l'image principale
- **Nouvelle propriété** : `ImageUrls` pour la collection complète
- **Propriétés helper** : `AllImageUrls`, `PrimaryImageUrl`, `HasImages`, `ImageCount`

### ?? **4. API et Modèles**
- **RoomCreationDTO** étendu avec `ImageUrls`
- **Mapping automatique** via AutoMapper
- **Debugging complet** avec logs des images envoyées
- **Support backward compatibility** avec l'ancien système

### ?? **5. Interface Utilisateur**
- **Conseils d'utilisation** intégrés (astuce sur l'ordre des images)
- **États visuels** : upload, succès, erreur, limite atteinte
- **Responsive design** : cartes adaptatives en grid
- **Accessibilité** : titres, alt texts, aria-labels

---

## ??? **Architecture Technique**

### **Modèles Étendus :**
```csharp
// Nouvelles propriétés ajoutées
public List<string> ImageUrls { get; set; } = new();
public string? PrimaryImageUrl => ImageUrls.FirstOrDefault() ?? ImageUrl;
public List<string> AllImageUrls { get; } // Combine ImageUrl + ImageUrls
public bool HasImages { get; }
public int ImageCount { get; }
public bool CanAddMoreImages => ImageCount < 5;
```

### **Flux de Données :**
1. **Upload Frontend** ? `InputFile` multiple
2. **Validation** ? Taille, format, limite
3. **Upload Service** ? `ImageUploadService.UploadImageAsync()`
4. **Collection** ? Ajout à `roomModel.ImageUrls`
5. **API Call** ? Envoi avec `ImageUrls` + `ImageUrl`
6. **Mapping** ? AutoMapper vers `RoomDTO`
7. **Persistence** ? Base de données

---

## ?? **Comment Utiliser**

### **1. Créer une Chambre avec Images :**
1. Aller sur `/admin/rooms/create`
2. Remplir les informations de base
3. Dans la section "Images de la chambre" :
   - Cliquer sur "Choisir les fichiers" 
   - Sélectionner jusqu'à 5 images
   - Attendre l'upload avec la barre de progression
   - Réorganiser l'ordre avec les flèches ?/?
   - Supprimer des images avec ???

### **2. Voir le Résultat :**
- **Liste des chambres** (`/rooms`) : affiche l'image principale + badge du nombre
- **Page détails** (`/room/{id}`) : carrousel avec toutes les images
- **Miniatures cliquables** pour navigation rapide

---

## ?? **Extensions Futures**

### **Possibilités d'Amélioration :**
1. **Glisser-Déposer** : Interface drag & drop pour upload
2. **Redimensionnement** : Compression automatique côté client
3. **Galerie Avancée** : Zoom, plein écran, diaporama
4. **Base de Données** : Table séparée pour les images avec relations
5. **CDN Integration** : Stockage sur Amazon S3 ou Azure Blob
6. **Lazy Loading** : Chargement progressif des images
7. **WebP Conversion** : Optimisation automatique des formats

### **API Endpoints à Créer :**
- `GET /api/rooms/{id}/images` - Liste des images d'une chambre
- `POST /api/rooms/{id}/images` - Ajouter des images à une chambre
- `DELETE /api/rooms/{id}/images/{imageId}` - Supprimer une image
- `PUT /api/rooms/{id}/images/order` - Réorganiser l'ordre des images

---

## ?? **Notes Importantes**

### **Limitations Actuelles :**
- **Base de données** : Utilise encore `ImageUrl` pour l'image principale
- **Stockage** : Dépend de `ImageUploadService` (fichiers locaux)
- **Performance** : Pas de lazy loading pour de nombreuses images
- **Cache** : Pas de mise en cache des miniatures

### **Prérequis :**
- **Service Images** : `IImageUploadService` doit être configuré
- **Permissions** : Droits d'écriture sur le dossier d'images
- **Migration** : Colonne `ImageUrl` doit exister (script `URGENT_FIX_ImageUrl.sql`)

### **Configuration Recommandée :**
```json
{
  "ImageUpload": {
    "MaxFileSize": 5242880, // 5MB
    "AllowedFormats": ["jpg", "jpeg", "png", "gif", "webp"],
    "MaxImagesPerRoom": 5,
    "UploadPath": "wwwroot/images/rooms/"
  }
}
```

---

## ?? **Conclusion**

Le système d'images multiples est maintenant **entièrement fonctionnel** ! 

? **Upload de 1 à 5 images** par chambre  
? **Interface intuitive** avec aperçu en temps réel  
? **Gestion complète** : ajout, suppression, réorganisation  
? **Intégration seamless** avec l'existant  
? **Prêt pour la production** avec gestion d'erreurs  

L'architecture modulaire permet des extensions futures sans casser l'existant. ???