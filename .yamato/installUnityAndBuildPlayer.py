import os
import utility.BuildStandalonePlayer

# Install Unity with the requested version and components
utility.BuildStandalonePlayer.install_unity(os.getenv('unityVERSION'), os.getenv('includeAndroid'),
                                            os.getenv('includeUWP'), os.getenv('includeIL2CPP'),
                                            os.getenv('includeIOS'))

# Build the player with the previously installed Unity version.
utility.BuildStandalonePlayer.build_player(os.getenv('cmd'))
