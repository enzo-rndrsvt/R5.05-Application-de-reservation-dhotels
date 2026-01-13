-- Script simple pour ajouter des images aux chambres existantes en base de données
-- Ce script utilise seulement la colonne ImageUrl existante

-- Mise à jour de l'ImageUrl principale pour quelques chambres
UPDATE Rooms 
SET ImageUrl = CASE 
    WHEN Number = 101 THEN 'https://images.unsplash.com/photo-1611892440504-42a792e24d32?w=400&h=300&fit=crop&crop=center'
    WHEN Number = 102 THEN 'https://images.unsplash.com/photo-1590490360182-c33d57733427?w=400&h=300&fit=crop&crop=center'
    WHEN Number = 103 THEN 'https://images.unsplash.com/photo-1578683010236-d716f9a3f461?w=400&h=300&fit=crop&crop=center'
    WHEN Number = 201 THEN 'https://images.unsplash.com/photo-1560448204-e02f11c3d0e2?w=400&h=300&fit=crop&crop=center'
    WHEN Number = 202 THEN 'https://images.unsplash.com/photo-1522771739844-6a9f6d5f14af?w=400&h=300&fit=crop&crop=center'
    WHEN Number = 203 THEN 'https://images.unsplash.com/photo-1566073771259-6a8506099945?w=400&h=300&fit=crop&crop=center'
    ELSE ImageUrl
END
WHERE Number IN (101, 102, 103, 201, 202, 203);

-- Version simple pour toutes les chambres si vous voulez
-- UPDATE Rooms SET ImageUrl = 'https://images.unsplash.com/photo-1611892440504-42a792e24d32?w=400&h=300&fit=crop&crop=center' WHERE ImageUrl IS NULL;

-- Vérification : Afficher les chambres avec leurs images
SELECT 
    Number,
    Type,
    ImageUrl,
    CASE 
        WHEN ImageUrl IS NOT NULL 
        THEN 'Image disponible' 
        ELSE 'Image par défaut sera utilisée' 
    END as StatusImages
FROM Rooms
ORDER BY Number;

-- Afficher les réservations qui vont maintenant avoir des images
SELECT 
    b.Id as BookingId,
    b.StartingDate,
    b.EndingDate,
    b.Price,
    r.Number as RoomNumber,
    r.Type as RoomType,
    r.ImageUrl,
    CASE 
        WHEN r.ImageUrl IS NOT NULL 
        THEN 'Image de DB' 
        ELSE 'Image par défaut' 
    END as TypeImage,
    h.Name as HotelName
FROM Bookings b
INNER JOIN Rooms r ON b.RoomId = r.Id
INNER JOIN Hotels h ON r.HotelId = h.Id
ORDER BY b.CreationDate DESC;