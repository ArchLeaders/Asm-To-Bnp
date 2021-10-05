# Asm To Bnp

Compiles ASM files to a BCML readable BNP (7z)

## Setup:

Download the latest release.

## How to use:

Just drag any ASM file over the included executable. It will ask for a mod name, enter anything you please and the application does the rest.

**Auto-Fill Commands:** If set to `true`, when dragging a .asm file over the main executable, it will not ask for a mod name but get it from the .asm file.

E.g. `patch_MasterModeTweaks.asm` will output `MasterModeTweaks.bnp`

To run the commands, open CMD (type `cmd` in the file explorer path) where `AsmToBnp.exe` resides. Then type in the console window one of the following commands:

```
  AsmToBnp.exe auto=true
```
```
  AsmToBnp.exe auto=false
```

_A guide on the manual process can be found [here](https://gamebanana.com/tuts/14400). Thanks, [Torph!](https://github.com/Torphedo)_

### Notice!

[BCML](https://github.com/NiceneNerd/BCML) must be installed for this to function correctly.
