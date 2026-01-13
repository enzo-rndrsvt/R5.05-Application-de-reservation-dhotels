# ?? Guide de Test - Images de Cabines

## ?? Problème à Résoudre
- **Symptôme** : Carré noir au lieu de l'image de la cabine
- **Objectif** : Afficher l'image de présentation depuis la base de données

## ?? Étapes de Test

### **1. Vérifier l'État Actuel**
1. Lancez l'application : `dotnet run --project HotelBooking.Web`
2. Allez sur `/debug-reservations` 
3. Cliquez "Tester les Réservations"
4. Vérifiez la colonne "Image" pour voir si c'est "DB" ou "Défaut"

### **2. Ajouter une Image de Test**
```sql
-- Exécutez ce SQL dans votre base de données
UPDATE Rooms 
SET ImageUrl = 'https://images.unsplash.com/photo-1611892440504-42a792e24d32?w=400&h=300&fit=crop&crop=center' 
WHERE Number = 101;

-- Vérifiez que l'image a été ajoutée
SELECT Number, Type, ImageUrl FROM Rooms WHERE Number = 101;
```

### **3. Tester l'Affichage**
1. Retournez sur `/debug-reservations`
2. Cliquez "Tester les Réservations"
3. Cherchez une réservation pour la Cabine 101
4. Vérifiez que la colonne "Image" affiche "DB" avec une prévisualisation

### **4. Vérifier le Profil**
1. Allez sur `/profile`
2. Cherchez une réservation pour la Cabine 101
3. **L'image doit maintenant s'afficher** au lieu du carré noir

## ?? Diagnostic des Problèmes

### **Si ça ne marche toujours pas :**

#### **A. Vérifiez les Données**
```sql
-- Cette requête vous montre quelles chambres ont des images
SELECT 
    r.Number,
    r.Type,
    r.ImageUrl,
    CASE WHEN r.ImageUrl IS NOT NULL THEN 'IMAGE OK' ELSE 'PAS D''IMAGE' END as Status
FROM Rooms r
ORDER BY r.Number;
```

#### **B. Vérifiez les Réservations**
```sql
-- Cette requête vous montre les réservations avec les images des chambres
SELECT 
    b.Id,
    r.Number as CabineNumber,
    r.ImageUrl,
    b.StartingDate,
    b.EndingDate
FROM Bookings b
INNER JOIN Rooms r ON b.RoomId = r.Id
ORDER BY b.CreationDate DESC;
```

#### **C. Utilisez la Page de Debug**
- Allez sur `/debug-reservations`
- Regardez les informations détaillées
- Vérifiez si `RoomImageUrl` est bien rempli

## ? Solution Rapide

Si vous voulez juste tester rapidement :

```sql
-- Ajoute des images à plusieurs chambres d'un coup
UPDATE Rooms 
SET ImageUrl = CASE 
    WHEN Number = 101 THEN 'https://images.unsplash.com/photo-1611892440504-42a792e24d32?w=400&h=300&fit=crop&crop=center'
    WHEN Number = 102 THEN 'https://images.unsplash.com/photo-1590490360182-c33d57733427?w=400&h=300&fit=crop&crop=center'
    WHEN Number = 103 THEN 'https://images.unsplash.com/photo-1578683010236-d716f9a3f461?w=400&h=300&fit=crop&crop=center'
    ELSE ImageUrl
END
WHERE Number IN (101, 102, 103);
```

## ?? Code Clé Modifié

### **Dans Profile.razor :**
```razor
@if (!string.IsNullOrEmpty(booking.RoomImageUrl))
{
    <!-- Image depuis la base de données -->
    <img src="@booking.RoomImageUrl" alt="Cabine @booking.RoomNumber" />
}
else
{
    <!-- Image par défaut -->
    <img src="@booking.PrimaryRoomImageUrl" alt="Cabine @booking.RoomNumber" />
}
```

### **Dans BookingForUser.cs :**
```csharp
// Vérifie seulement les vraies images de la base
public bool HasRoomImages => !string.IsNullOrEmpty(RoomImageUrl) || (RoomImageUrls?.Any() == true);
```

## ?? Résultat Attendu

Après avoir exécuté le script SQL, vous devriez voir :
- ? **Page `/profile`** : Images réelles au lieu de carrés noirs
- ? **Page `/debug-reservations`** : Colonne "Image" avec "DB" et prévisualisation
- ? **Titre** : "Cabine 101" au lieu de "SkullKing"

## ?? Si Ça Ne Marche Toujours Pas

1. Vérifiez que le script SQL s'est bien exécuté
2. Redémarrez l'application
3. Videz le cache du navigateur (Ctrl+F5)
4. Vérifiez la console développeur (F12) pour des erreurs d'images