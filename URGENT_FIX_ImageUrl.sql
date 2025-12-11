-- ========================================
-- SCRIPT DE CORRECTION URGENT
-- Ajouter la colonne ImageUrl à la table Rooms
-- ========================================

-- ?? ÉTAPE 1: Vérifier la base de données courante
PRINT '?? Base de données courante: ' + DB_NAME();

-- ?? ÉTAPE 2: Lister les colonnes actuelles de la table Rooms
PRINT '?? Colonnes actuelles de la table Rooms:';
SELECT 
    COLUMN_NAME, 
    DATA_TYPE, 
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Rooms' 
AND TABLE_SCHEMA = 'dbo'
ORDER BY ORDINAL_POSITION;

-- ??? ÉTAPE 3: Ajouter la colonne ImageUrl si elle n'existe pas
IF NOT EXISTS (
    SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'Rooms' 
    AND COLUMN_NAME = 'ImageUrl'
    AND TABLE_SCHEMA = 'dbo'
)
BEGIN
    PRINT '?? CORRECTION: Ajout de la colonne ImageUrl...';
    ALTER TABLE [dbo].[Rooms] ADD [ImageUrl] NVARCHAR(500) NULL;
    PRINT '? Colonne ImageUrl ajoutée avec succès !';
END
ELSE
BEGIN
    PRINT '? La colonne ImageUrl existe déjà.';
END

-- ?? ÉTAPE 4: Vérification finale
PRINT '?? Vérification finale - Colonnes après correction:';
SELECT 
    COLUMN_NAME, 
    DATA_TYPE, 
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Rooms' 
AND TABLE_SCHEMA = 'dbo'
ORDER BY ORDINAL_POSITION;

PRINT '?? CORRECTION TERMINÉE ! Vous pouvez maintenant créer des chambres.';