dotnet tool install --global --configfile nuget.config Cake.Tool 
cd buildscripts
dotnet cake --IsLocalBuild=true --Branch=local --Revision=1