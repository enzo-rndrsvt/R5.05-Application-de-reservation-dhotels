-- Script de correction pour ajouter la colonne ImageUrl à la table Rooms
-- À exécuter dans SQL Server Management Studio

-- 1. Vérifier si la colonne existe déjà
IF NOT EXISTS (
    SELECT * 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'Rooms' 
    AND COLUMN_NAME = 'ImageUrl'
)
BEGIN
    PRINT '?? La colonne ImageUrl n''existe pas. Ajout en cours...';
    
    -- 2. Ajouter la colonne ImageUrl
    ALTER TABLE Rooms ADD ImageUrl NVARCHAR(MAX);
    
    PRINT '? Colonne ImageUrl ajoutée avec succès !';
END
ELSE
BEGIN
    PRINT '? La colonne ImageUrl existe déjà.';
END

-- 3. Mettre à jour les chambres existantes qui n'ont pas d'ImageUrl
UPDATE Rooms 
SET ImageUrl = 'https://picsum.photos/800/600?random=' + CAST(Number AS NVARCHAR(10))
WHERE ImageUrl IS NULL;

PRINT '? Images par défaut ajoutées aux chambres existantes.';

-- 4. Vérifier le résultat
SELECT TOP 5 
    Id, 
    Number, 
    Type, 
    ImageUrl,
    LEN(ImageUrl) AS ImageUrl_Length,
    CASE 
        WHEN ImageUrl LIKE '%,%' THEN 'Multiple URLs'
        ELSE 'Single URL'
    END AS URL_Type
FROM Rooms 
ORDER BY CreationDate DESC;

PRINT '?? Script terminé ! Vous pouvez maintenant créer et modifier des chambres avec des images.';