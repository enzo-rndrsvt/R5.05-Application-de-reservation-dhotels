-- Script de test pour vérifier l'affichage des images dans les réservations
-- Exécuter ce script pour ajouter rapidement une image à une chambre spécifique

-- Test rapide : ajouter une image à la chambre 101
UPDATE Rooms 
SET ImageUrl = 'https://images.unsplash.com/photo-1611892440504-42a792e24d32?w=400&h=300&fit=crop&crop=center' 
WHERE Number = 101;

-- Vérifier que l'image a été ajoutée
SELECT Id, Number, Type, ImageUrl 
FROM Rooms 
WHERE Number = 101;

-- Voir s'il y a des réservations pour cette chambre
SELECT 
    b.Id as BookingId,
    b.StartingDate,
    b.EndingDate,
    r.Number as RoomNumber,
    r.ImageUrl,
    CASE 
        WHEN r.ImageUrl IS NOT NULL THEN 'Image disponible en DB'
        ELSE 'Aucune image en DB'
    END as StatusImage
FROM Bookings b
INNER JOIN Rooms r ON b.RoomId = r.Id
WHERE r.Number = 101
ORDER BY b.CreationDate DESC;