# RuntimeTransformHandle

Runtime transform handle for Unity.

## Installation

#### Install Unity Package Manager

Using Package Manager is now the prefferred method, all releases should be updated immediately.

Add Scoped Registry into Package manager using Project Settings => Package Manager as below:  
Name:  
```
Shtif Registry
```  
URL:  
```
http://package.binaryego.com:4873
```  
Scopes:  
```
com.shtif
```

![Package Settings](https://i.imgur.com/Y85kaBn.png)

After this you can find the Runtime Transform Handle in My Registries inside Package Manager.

![Package Manager](https://i.imgur.com/WZ19e94.png)

Works with both the new and legacy input system. The asmdef of this package should automatically grab the reference to the asmdef of the new input system if it is present in your project, or ignore it since it cannot be found if you are using the legacy input system. If this is not the case Check the Assembly Definition References in RuntimeTransformHandle.asmdef. This should have a reference to Unity.InputSystem if you are using the new input system, or no reference if you are using the legacy input system.

In case you get this error:
```Assets/Plugins/RuntimeTransformHandle/Scripts/RuntimeTransformHandle.cs(3,19): error CS0234: The type or namespace name 'InputSystem' does not exist in the namespace 'UnityEngine' (are you missing an assembly reference?)
```
You probably have both input systems enabled (Project Settings/Player/Active Input Handling: Both) but the Input System package not installed. To fix this either set "Active Input Handling" to "Input Manager (old)" or install the Input System package through the package manager.
