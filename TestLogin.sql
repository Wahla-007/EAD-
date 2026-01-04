-- Query to see all users with their password hashes
SELECT TOP 5
    Username,
    Role,
    IsActive,
    LEFT(PasswordHash, 20) + '...' as HashPreview,
    LEFT(PasswordSalt, 20) + '...' as SaltPreview,
    LEN(PasswordHash) as HashLength,
    LEN(PasswordSalt) as SaltLength,
    CreatedDate
FROM Users
ORDER BY UserId DESC;

-- Find the most recently created user
SELECT TOP 1 *
FROM Users
WHERE Role = 'Teacher'
ORDER BY CreatedDate DESC;
