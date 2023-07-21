git submodule add https://github.com/ekrixi-14/secrets secrets
New-Item -ItemType SymbolicLink -Path "Content.ServerSecret\Code" -Target "Secrets\ServerCode"
New-Item -ItemType SymbolicLink -Path "Resources\SecretResources" -Target "Secrets\Resources"
