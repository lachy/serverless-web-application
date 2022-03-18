
      $storage = Get-AzStorageAccount -ResourceGroupName "lachy-dev-rg" -Name "lachydev"
      $ctx = $storage.Context
	  #Write-Output $ctx
      Enable-AzStorageStaticWebsite -Context $ctx -IndexDocument index.html -ErrorDocument404Path notfound.html
      $output = $storage.PrimaryEndpoints.Web
      $output = $output.TrimEnd('/')
      $DeploymentScriptOutputs = @{}
      $DeploymentScriptOutputs['URL'] = $output
	  Write-Output $output