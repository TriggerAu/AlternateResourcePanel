	$GitHubName="AlternateResourcePanel"
	$PluginName="KSPAlternateResourcePanel"
	$Version="2.2.1.0"
	$UploadDir = "..\_Uploads\KSPAlternateResourcePanel"

	git add -A *
	git commit -m "Version history $($Version)"
	
	write-host -ForegroundColor Yellow "`r`nPUSHING DEVELOP TO GITHUB"
	git push

	$CommitDate = "{0:yyyy-MM-ddTHH:mm:ss+11:00}" -f $CommitDateValue.AddMinutes(1)
	Set-Commitdate $CommitDate

	git checkout master
	git merge --no-ff develop -m "Merge $($Version) to master"
	git tag -a "v$($Version)" -m "Released version $($Version)"

	write-host -ForegroundColor Yellow "`r`nPUSHING MASTER AND TAGS TO GITHUB"
	git push
	git push --tags
	
	write-host -ForegroundColor Yellow "----------------------------"
	write-host -ForegroundColor Yellow "Finished Version $($Version)"
	write-host -ForegroundColor Yellow "----------------------------"
	
	write-host -ForegroundColor Yellow "`r`n Creating Release"
	$readme = (Get-Content -Raw "PluginFiles\ReadMe-$($PluginName).txt")
	$reldescr = [regex]::match($readme,"Version\s$($Version).+?(?=[\r\n]*Version\s\d+|$)","singleline,ignorecase").Value

	#Now get the KSPVersion from the first line
	$KSPVersion = [regex]::match($reldescr,"KSP\sVersion\:.+?(?=[\r\n]|$)","singleline,ignorecase").Value
	
	#Now drop the first line
	$reldescr = [regex]::replace($reldescr,"^.+?\r\n","","singleline,ignorecase")
	
	$reldescr = $reldescr.Trim("`r`n")
	$reldescr = $reldescr.Replace("- ","* ")
	$reldescr = $reldescr.Replace("`r`n","\r\n")
	$reldescr = $reldescr.Replace("`"","\`"")
	
	$reldescr = "$($reldescr)\r\n\r\n``````$($KSPVersion)``````"

	$CreateBody = "{`"tag_name`":`"v$($Version)`",`"name`":`"v$($Version) Release`",`"body`":`"$($relDescr)`"}"
	
	$RestResult = Invoke-RestMethod -Method Post `
		-Uri "https://api.github.com/repos/TriggerAu/$($GitHubName)/releases" `
		-Headers @{"Accept"="application/vnd.github.v3+json";"Authorization"="token 585a29de3d6a38a3cb777f49335e8024572a23dc"} `
		-Body $CreateBody
	if ($?)
	{
		write-host -ForegroundColor Yellow "Uploading File"
		$File = get-item "$($UploadDir)\v$($Version)\$($pluginname)_$($Version).zip"
		$RestResult = Invoke-RestMethod -Method Post `
			-Uri "https://uploads.github.com/repos/TriggerAu/$($GitHubName)/releases/$($RestResult.id)/assets?name=$($File.Name)" `
			-Headers @{"Accept"="application/vnd.github.v3+json";"Authorization"="token 585a29de3d6a38a3cb777f49335e8024572a23dc";"Content-Type"="application/zip"} `
			-InFile $File.fullname
		
		"Result = $($RestResult.state)"
	}

	write-host -ForegroundColor Yellow "----------------------------"
	write-host -ForegroundColor Yellow "Finished Release $($Version)"
	write-host -ForegroundColor Yellow "----------------------------"