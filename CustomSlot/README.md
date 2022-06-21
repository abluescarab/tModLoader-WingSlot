This is a collection of files you can use to easily create your own custom slot in tModLoader.

**This project reuses a lot of vanilla code.** If you want to help, read Terraria's source code in [ILSpy](https://github.com/icsharpcode/ILSpy) or another decompiler and make a pull request.

*This project is under the [GNU General Public License v3.0.](LICENSE)*

## Contents
1. [How to use these files](#how-to-use-these-files)
2. [Reporting bugs and making suggestions](#reporting-bugs-and-making-suggestions)
3. [Known issues](#known-issues)

## How to use these files
To add the files to your project, you can either [download this repository](https://github.com/abluescarab/tModLoader-CustomSlot/archive/master.zip) or add it as a submodule in Git using a GUI or the command line. To clone the repository into a subfolder named `CustomSlot`:
```
git submodule add https://github.com/abluescarab/tModLoader-CustomSlot.git CustomSlot
```

To update the files when new commits are made:
```
git submodule update --remote --merge
git commit
```

Then go to the [wiki](../../wiki/Creating-a-custom-slot) for further instructions.

## Reporting bugs and making suggestions
Please use GitHub's [issues section](https://github.com/abluescarab/tModLoader-CustomSlot/issues) on this repository to report bugs, make suggestions, or request new features.

## Known issues
* Right-clicking empty accessory slots makes a sound
