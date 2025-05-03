# Define your GitHub token and username
$githubToken = "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"
$username = "lara-sorrenson"

# Define the URL
$url = "https://ghcr.io/v2/$username/_catalog"

# Define the log file path
$logFilePath = "error_log.txt"

# Perform the request and handle errors
try {
    # Perform the request
    $response = Invoke-RestMethod -Uri $url -Method Get -Headers @{
        "Authorization" = "Bearer $githubToken"
        "Accept" = "application/vnd.github.v3+json"
    }
    # Output the response
    $response | ConvertTo-Json | Write-Output
} catch {
    # Log the error message to a file
    $_ | Out-File -FilePath $logFilePath -Append
    # Also display the error message in the PowerShell console
    Write-Error "An error occurred. Details have been logged to $logFilePath"
}
