# Generate real password hashes for Test@123
$password = "Test@123"

Write-Host "Generating password hashes for: $password" -ForegroundColor Cyan
Write-Host ""

$users = @(
    @{Username = "admin"; Password = $password},
    @{Username = "teacher1"; Password = $password},
    @{Username = "student1"; Password = $password}
)

$sqlStatements = @()

foreach ($user in $users) {
    $hmac = [System.Security.Cryptography.HMACSHA512]::new()
    $salt = [Convert]::ToBase64String($hmac.Key)
    $hash = [Convert]::ToBase64String($hmac.ComputeHash([System.Text.Encoding]::UTF8.GetBytes($user.Password)))
    $hmac.Dispose()
    
    $sqlStatements += "UPDATE Users SET PasswordHash = '$hash', PasswordSalt = '$salt' WHERE Username = '$($user.Username)';"
}

$sqlContent = @"
-- Update users with real password hashes for: Test@123
USE master;
GO

$($sqlStatements -join "`r`n")

PRINT 'Passwords updated successfully!';
PRINT 'You can now login with:';
PRINT '  admin / Test@123';
PRINT '  teacher1 / Test@123';
PRINT '  student1 / Test@123';
"@

$sqlContent | Out-File -FilePath "UpdatePasswords.sql" -Encoding UTF8

Write-Host "SQL file created: UpdatePasswords.sql" -ForegroundColor Green
Write-Host ""
Write-Host "Next step: Run UpdatePasswords.sql in Visual Studio" -ForegroundColor Yellow
