$rg = "CSSClod6823018"
$LabID = "6823018"
$cdbName = "cdb$($LabID)"
$webAppName = "lods$($LabID)"
$WebUri = "$($webAppName).azurewebsites.net/api/test/loadcosmosdb"

$key = (Invoke-AzureRmResourceAction -Action listKeys `
    -ResourceType "Microsoft.DocumentDb/databaseAccounts" `
    -ApiVersion "2015-04-08" `
    -ResourceGroupName $rg `
    -Force `
    -Name $cdbName).PrimaryMasterKey


$appSettings = @{
    "ListingsURI" = "https://$($cdbName).documents.azure.com:443";
    "ListingsKey" = $key;
    "TestType" = "32"
}

Set-AzureRmWebApp -ResourceGroupName $rg -Name $webAppName -AppSettings $appSettings
Invoke-RestMethod -Method Get -Uri $WebUri 
