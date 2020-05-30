configuration WebApp
{
    Script FirewallRule
    {
        GetScript = { @{ Result = [string]$(netsh advfirewall firewall show rule name="http") } }
        SetScript = { netsh advfirewall firewall add rule name="http" dir=in action=allow protocol=TCP localport=80 }
        TestScript = { 
            if ((netsh advfirewall firewall show rule name="http") -match 'No rules match the specified criteria') { $false } 
            else { $true }
        }
    }
    Script IndexHtmlFile
    {
        GetScript = { @{ Result = (Test-Path -Path 'C:\inetpub\wwwroot\index.html') } }
        SetScript = {
            $outFile = 'C:\inetpub\wwwroot\index.html'
            Invoke-WebRequest 'https://raw.githubusercontent.com/grabinski/ais-007/master/index.html' -OutFile $outFile
            Unblock-File -Path $outFile
        }
        TestScript = { Test-Path -Path 'C:\inetpub\wwwroot\index.html' }
        DependsOn = '[WindowsFeature]IIS'
    }
    WindowsFeature IIS
    {
        Ensure = "Present"
        Name = "Web-Server"
    }
}