-- Script pour tester les images multiples sur une chambre existante
-- À exécuter dans SQL Server Management Studio ou via l'interface debug

-- 1. D'abord, voir les chambres existantes
SELECT TOP 5 Id, Number, Type, HotelId, ImageUrl 
FROM Rooms 
ORDER BY CreationDate DESC;

-- 2. Ajouter des images multiples à la première chambre trouvée
-- (Remplacez l'ID par celui d'une vraie chambre de votre base)
UPDATE Rooms 
SET ImageUrl = '/images/pirate-room1.jpg,/images/pirate-room2.jpg,/images/pirate-room3.jpg,/images/pirate-room4.jpg'
WHERE Id = (
    SELECT TOP 1 Id FROM Rooms 
    ORDER BY CreationDate DESC
);

-- 3. Vérifier le résultat
SELECT Id, Number, Type, ImageUrl 
FROM Rooms 
WHERE ImageUrl LIKE '%,%'  -- Celles qui ont plusieurs URLs
ORDER BY ModificationDate DESC;

-- 4. Pour tester avec des URLs d'images réelles (si vous en avez)
/*
UPDATE Rooms 
SET ImageUrl = 'https://images.unsplash.com/photo-1566665797739-1674de7a421a?w=400,https://images.unsplash.com/photo-1578662996442-48f60103fc96?w=400,https://images.unsplash.com/photo-1582719478250-c89cae4dc85b?w=400'
WHERE Number = 101; -- Remplacez par le numéro de votre chambre
*/

-- Informations utiles
PRINT 'Script terminé !';
PRINT 'Maintenant allez sur la page de détails de la chambre modifiée pour voir les images multiples.';
PRINT 'Ou utilisez la page /test-room-images/{id} pour diagnostiquer.';