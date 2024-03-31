git submodule add -f https://github.com/ekrixi-14/secrets Secrets
New-Item -ItemType SymbolicLink -Path "Content.Server\_SecretCode" -Target "Secrets\ServerCode"
New-Item -ItemType SymbolicLink -Path "Resources\Prototypes\SecretPrototypes" -Target "Secrets\SecretResources\Prototypes"
New-Item -ItemType SymbolicLink -Path "Resources\IgnoredPrototypes\secret.yml" -Target "Secrets\ignoredPrototypes.yml"
New-Item -ItemType SymbolicLink -Path "Resources\Textures\SecretTextures" -Target "Secrets\SecretResources\Textures"
pause
