-- Script SQL pour ajouter des images de test aux chambres existantes
-- Images depuis Unsplash avec des thèmes de chambres d'hôtel

UPDATE Rooms 
SET ImageUrl = CASE 
    WHEN Number = 101 THEN 'https://images.unsplash.com/photo-1611892440504-42a792e24d32?w=400&h=300&fit=crop&crop=center'
    WHEN Number = 102 THEN 'https://images.unsplash.com/photo-1590490360182-c33d57733427?w=400&h=300&fit=crop&crop=center'
    WHEN Number = 103 THEN 'https://images.unsplash.com/photo-1578683010236-d716f9a3f461?w=400&h=300&fit=crop&crop=center'
    WHEN Number = 201 THEN 'https://images.unsplash.com/photo-1560448204-e02f11c3d0e2?w=400&h=300&fit=crop&crop=center'
    WHEN Number = 202 THEN 'https://images.unsplash.com/photo-1522771739844-6a9f6d5f14af?w=400&h=300&fit=crop&crop=center'
    WHEN Number = 203 THEN 'https://images.unsplash.com/photo-1566073771259-6a8506099945?w=400&h=300&fit=crop&crop=center'
    WHEN Number = 301 THEN 'https://images.unsplash.com/photo-1582719478250-c89cae4dc85b?w=400&h=300&fit=crop&crop=center'
    WHEN Number = 302 THEN 'https://images.unsplash.com/photo-1564013799919-ab600027ffc6?w=400&h=300&fit=crop&crop=center'
    WHEN Number = 303 THEN 'https://images.unsplash.com/photo-1571896349842-33c89424de2d?w=400&h=300&fit=crop&crop=center'
    ELSE 'https://images.unsplash.com/photo-1584132967334-10e028bd69f7?w=400&h=300&fit=crop&crop=center'
END
WHERE ImageUrl IS NULL OR ImageUrl = '';

-- Vérifier les résultats
SELECT 
    Number,
    Type,
    ImageUrl,
    CASE WHEN ImageUrl IS NOT NULL THEN 'Image présente' ELSE 'Pas d\'image' END as Status
FROM Rooms
ORDER BY Number;