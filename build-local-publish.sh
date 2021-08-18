revision=3

echo "gitUser:"
read git_user
echo "gitPassword:"
read git_password
echo "nuget apikey:"
read nuget_apiKey

dotnet tool install --global --configfile nuget.config Cake.Tool 
cd buildscripts
dotnet cake --target=BuildAndPublish --IsLocalBuild=false --Branch=local --Revision=$revision --nuGetPackageSource=https://api.nuget.org/v3/index.json --nuGetApiKey=$nuget_apiKey --gitUser=$git_user --gitPassword=$git_password