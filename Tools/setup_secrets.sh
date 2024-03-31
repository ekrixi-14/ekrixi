git submodule add -f git@github.com:ekrixi-14/secrets.git Secrets
ln -s ./Secrets/ServerCode ./Content.Server/_SecretCode
ln -s ./Secrets/SecretResources/Prototypes ./Resources/Prototypes/SecretPrototypes
ln -s ./Secrets/ignoredPrototypes.yml ./Resources/IgnoredPrototypes/secret.yml
ln -s ./Secrets/SecretResources/Textures ./Resources/Textures/SecretTextures

echo Done.
