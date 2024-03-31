git submodule add -f git@github.com:ekrixi-14/secrets.git Secrets
ln -sf $PWD/Secrets/ServerCode/ $PWD/Content.Server/_SecretCode
ln -sf $PWD/Secrets/SecretResources/Prototypes $PWD/Resources/Prototypes/SecretPrototypes
ln -sf $PWD/Secrets/ignoredPrototypes.yml $PWD/Resources/IgnoredPrototypes/secret.yml
ln -sf $PWD/Secrets/SecretResources/Textures $PWD/Resources/Textures/SecretTextures

echo Done.
