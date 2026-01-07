# Fix encoding issues in view files
$files = Get-ChildItem -Path "c:\Users\DELL\Documents\EAD_project\Views" -Recurse -Filter "*.cshtml"
foreach ($file in $files) {
    $content = Get-Content $file.FullName -Raw -Encoding UTF8
    if ($content -match [char]0x00E2) {
        # Replace mojibake sequences with HTML entities
        $content = $content -replace 'âœ"', '&check;'
        $content = $content -replace 'â†'', '&rarr;'
        $content = $content -replace 'â€"', '&mdash;'
        $content = $content -replace 'â€™', '&rsquo;'
        $content = $content -replace 'â€œ', '&ldquo;'
        $content = $content -replace 'â€', '&rdquo;'
        Set-Content $file.FullName -Value $content -Encoding UTF8
        Write-Host "Fixed: $($file.FullName)"
    }
}
Write-Host "Done!"
