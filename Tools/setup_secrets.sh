git submodule add -f git@github.com:ekrixi-14/secrets.git Secrets
ln -s ./Secrets/ServerCode ./Content.ServerSecret/Code
ln -s ./Secrets/SecretResources/Prototypes ./Resources/Prototypes/SecretPrototypes
ln -s ./Secrets/SecretResources/Textures ./Resources/Textures/SecretTextures

echo Done.
