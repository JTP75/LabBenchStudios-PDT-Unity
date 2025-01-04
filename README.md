# Programming Digital Twins - Unity Package
# [LabBenchStudios-PDT-Unity](https://github.com/programming-digital-twins/LabBenchStudios-PDT-Unity)

This is the plugin repository for [Unity](https://unity.com/)-based software and other application-specific components (written in C#)
related to my Building Digital Twins course at Northeastern University. The intent of this repository is to provide students with a
simple [Unity](https://unity.com/) plugin that can provide the basis for a simple digital twin implementation for personal testing
and validation related to the lab module assignments that are part of the Building Digital Twins course.

For convenience to the reader, some of the basic functionality has already been implemented, with other key components requiring implementation
by users of the repository (e.g., students taking my Building Digital Twins course).

## Links, Exercises, Updates, Errata, and Clarifications

Kanban Board Exercises: [Programming Digital Twins Requirements](https://github.com/orgs/programming-digital-twins/projects/1)

For other references, see the following links to access exercises for this project. Please note that many of the exercises and sample source code
in this repository is based on many of the examples outlined in the [Programming the IoT Kanban Board](https://github.com/orgs/programming-the-iot/projects/1),
which is aligned with my book, [Programming the Internet of Things Book](https://learning.oreilly.com/library/view/programming-the-internet/9781492081401/).
 - [Original Constrained Device Application Source Code Template](https://github.com/programming-the-iot/python-components)
 - [Programming Digital Twins Edge Components Source Code Template](https://github.com/programming-digital-twins/pdt-edge-components)
 - [Programming the Internet of Things Book](https://learning.oreilly.com/library/view/programming-the-internet/9781492081401/)

## How to use this repository

### Installation (within Unity 6)

- Open Unity 6
  - Select: Window -> Package Manager -> Install package from git URL
  - Enter: This repository's git URL
  - Click: Install

### Usage

- Follow exercise instructions for the Digital Twin App (DTA) in the [Programming Digital Twins Kanban Board](https://github.com/orgs/programming-digital-twins/projects/1)

Note: Check back regularly for version updates, as this package is under active development and is in 'alpha' mode (e.g., UNRELEASED).

### About the package and its design

If you're reading [Programming the Internet of Things: An Introduction to Building Integrated, Device to Cloud IoT Solutions](https://learning.oreilly.com/library/view/programming-the-internet/9781492081401),
you'll see some design similarities to the exercises described in each chapter and the source code contained within this repository. While the software components contained herein
are written in C# and follow, they follow a similar design philosophy as that of the Java, or Gateway Device App, components written for the book and are part of my other course, Connected Devices.

## How to navigate the directory structure of this repository

This repository is comprised of the following key paths:
- [LabBenchStudios-PDT-Unity](https://github.com/programming-digital-twins/LabBenchStudios-PDT-Unity): All other assets are contained within this path.
  - [Documentation](https://github.com/programming-digital-twins/LabBenchStudios-PDT-Unity/blob/alpha/Documentation): Contains package documentation.
  - [Models](https://github.com/programming-digital-twins/LabBenchStudios-PDT-Unity/blob/alpha/Models): Contains both DTDL and type mapping JSON models.
    - [Dtdl](https://github.com/programming-digital-twins/LabBenchStudios-PDT-Unity/blob/alpha/Models/Dtdl): Digital Twin Definition Language (DTDL) JSON models.
    - [Types](https://github.com/programming-digital-twins/LabBenchStudios-PDT-Unity/blob/alpha/Models/Types): JSON-based type and constraint mapping models.
  - [Plugins](https://github.com/programming-digital-twins/LabBenchStudios-PDT-Unity/blob/alpha/Plugins): Contains relevant DLL dependencies (see [Third Party Notices.md](https://github.com/programming-digital-twins/LabBenchStudios-PDT-UnityPlugin/blob/alpha/Third%20Party%20Notices.md)).
  - [Runtime](https://github.com/programming-digital-twins/LabBenchStudios-PDT-Unity/blob/alpha/Runtime): Contains relevant Unity-specific prefabs and C# scripts.
    - [ProgrammingDigitalTwins](https://github.com/programming-digital-twins/LabBenchStudios-PDT-Unity/blob/alpha/Runtime/ProgrammingDigitalTwins): Containing folder.
      - [Prefabs](https://github.com/programming-digital-twins/LabBenchStudios-PDT-Unity/blob/alpha/Runtime/ProgrammingDigitalTwins/Prefabs): Unity-specific prefabs.
      - [Scripts](https://github.com/programming-digital-twins/LabBenchStudios-PDT-Unity/blob/alpha/Runtime/ProgrammingDigitalTwins/Scripts): Unity-specific C# scripts.
        - [Unity](https://github.com/programming-digital-twins/LabBenchStudios-PDT-Unity/blob/alpha/Runtime/ProgrammingDigitalTwins/Scripts/Unity): Namespace containing folder (to help avoid naming collisions).
          - [Common](https://github.com/programming-digital-twins/LabBenchStudios-PDT-Unity/blob/alpha/Runtime/ProgrammingDigitalTwins/Scripts/Unity/Common): Base game object and utility classes.
          - [Controller](https://github.com/programming-digital-twins/LabBenchStudios-PDT-Unity/blob/alpha/Runtime/ProgrammingDigitalTwins/Scripts/Unity/Controller): Simple animation controller classes.
          - [Dashboard](https://github.com/programming-digital-twins/LabBenchStudios-PDT-Unity/blob/alpha/Runtime/ProgrammingDigitalTwins/Scripts/Unity/Dashboard): Simple dashboard controllers (3D embeddable).
          - [Hud](https://github.com/programming-digital-twins/LabBenchStudios-PDT-Unity/blob/alpha/Runtime/ProgrammingDigitalTwins/Scripts/Unity/Hud): Simple HUD controllers (2D display).
          - [Manager](https://github.com/programming-digital-twins/LabBenchStudios-PDT-Unity/blob/alpha/Runtime/ProgrammingDigitalTwins/Scripts/Unity/Manager): Primary system and keyboard manager components.
          - [Model](https://github.com/programming-digital-twins/LabBenchStudios-PDT-Unity/blob/alpha/Runtime/ProgrammingDigitalTwins/Scripts/Unity/Model): Simple collision controller classes.
          - [Sample](https://github.com/programming-digital-twins/LabBenchStudios-PDT-Unity/blob/alpha/Runtime/ProgrammingDigitalTwins/Scripts/Unity/Sample): Sample threshold crossing animation controller classes.

NOTE: The directory structure and all files are subject to change based on feedback I receive from readers of my book and students in my IoT class,
as well as improvements I find to be helpful for overall repo betterment.

# Other things to know

## Pull requests

PR's are disabled while the codebase is being developed.

## Updates

Much of this repository, and in particular unit and integration tests, will continue to evolve, so please check back regularly for potential updates.
Please note that API changes can - and likely will - occur at any time.

# DEPENDENCIES and REFERENCES

This repository requires various [Unity Technologies Inc.](https://unity.com/) features and packages, and is intended to be installed within a Unity 6 environment.

This repository has external dependencies on other open source projects. I'm grateful to the open source community and authors / maintainers of the following libraries. More details can be found in [Third Party Notices.md](https://github.com/programming-digital-twins/LabBenchStudios-PDT-Unity/blob/alpha/Third%20Party%20Notices.md). References to this package's dependencies are as follows (as of 01 Jan 2025):

## Unity Application Requirement Reference (not included - must be installed separately)

This package is dependent upon the Unity 6 Editor and Game Engine.

- Unity Technologies Inc.: [Unity 6](https://unity.com/)
  - Reference: Unity Technologies Inc. Unity Editor. (2024) [Online]. Available: https://unity.com/.
  - Version: 6000.0.32f1
    - NOTE: This codebase and its references may function with other Unity Editor versions; however, all testing currently utilizes version 6000.0.32f1. This package is not intended to be used outside of a compatible Unity Editor environment.

## Unity Feature Dependency References (not included - must be installed separately)

This package requires the following Unity Features to be installed prior to this package's installation. NOTE: Other Unity-specific features may be required in the future.

- Unity Technologies Inc.: [2D - com.unity.feature.2d](https://docs.unity3d.com/6000.0/Documentation/Manual/2DFeature.html)
  - Reference: Unity Technologies Inc. 2D - com.unity.feature.2d. See: https://docs.unity3d.com/6000.0/Documentation/Manual/2DFeature.html.
  - Note: See feature 'Description' and 'Packages Included' for more detail.
  - Terms of Use: https://docs.unity3d.com/Manual/TermsOfUse.html

- Unity Technologies Inc.: [3D Characters and Animation]()
  - Reference: Unity Technologies Inc. 3D Characters and Animation - com.unity.feature.characters-animation. See: https://docs.unity3d.com/6000.0/Documentation/Manual/2DFeature.html.
  - Note: See feature 'Description' and 'Packages Included' for more detail.
  - Terms of Use: https://docs.unity3d.com/Manual/TermsOfUse.html

- Unity Technologies Inc.: [3D World Building](https://docs.unity3d.com/6000.0/Documentation/Manual/WorldBuildingFeature.html)
  - Reference: Unity Technologies Inc. 3D World Building - com.unity.feature.worldbuilding. See: https://docs.unity3d.com/6000.0/Documentation/Manual/WorldBuildingFeature.html.
  - Note: See feature 'Description' and 'Packages Included' for more detail.
  - Terms of Use: https://docs.unity3d.com/Manual/TermsOfUse.html

- Unity Technologies Inc.: [Cinematic Studio](https://docs.unity3d.com/6000.0/Documentation/Manual/CinematicStudioFeature.html)
  - Reference: Unity Technologies Inc. Cinematic Studio - com.unity.feature.cinematic. See: https://docs.unity3d.com/6000.0/Documentation/Manual/CinematicStudioFeature.html.
  - Note: See feature 'Description' and 'Packages Included' for more detail.
  - Terms of Use: https://docs.unity3d.com/Manual/TermsOfUse.html

- Unity Technologies Inc.: [Engineering](https://docs.unity3d.com/6000.0/Documentation/Manual/DeveloperToolsFeature.html)
  - Reference: Unity Technologies Inc. Engineering - com.unity.feature.development. See: https://docs.unity3d.com/6000.0/Documentation/Manual/DeveloperToolsFeature.html.
  - Note: See feature 'Description' and 'Packages Included' for more detail.
  - Terms of Use: https://docs.unity3d.com/Manual/TermsOfUse.html

- Unity Technologies Inc.: [Gameplay and Storytelling](https://docs.unity3d.com/6000.0/Documentation/Manual/GameplayStorytellingFeature.html)
  - Reference: Unity Technologies Inc. Gameplay and Storytelling - com.unity.feature.gameplay-storytelling. See: https://docs.unity3d.com/6000.0/Documentation/Manual/GameplayStorytellingFeature.html.
  - Note: See feature 'Description' and 'Packages Included' for more detail.
  - Terms of Use: https://docs.unity3d.com/Manual/TermsOfUse.html

## Unity Package Dependency References (not included - must be installed separately)

This package requires the following Unity packages to be installed prior to this package's installation. NOTE: Other Unity-specific packages may be required in the future.

- Unity Technologies Inc.: [Text Mesh Pro](https://docs.unity3d.com/Packages/com.unity.textmeshpro@4.0/manual/index.html)
  - Reference: Unity Technologies Inc. Text Mesh Pro - "Unity.TextMeshPro". See: https://docs.unity3d.com/Packages/com.unity.textmeshpro@4.0/manual/index.html.
  - Version: 4.0
  - License: https://docs.unity3d.com/Packages/com.unity.textmeshpro@4.0/license/LICENSE.html

- Unity Technologies Inc.: [Input System](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.11/manual/index.html)
  - Reference: Unity Technologies Inc. Input System - "Unity.InputSystem". See: https://docs.unity3d.com/Packages/com.unity.inputsystem@1.11/manual/index.html.
  - Version: 1.11
  - License: https://docs.unity3d.com/Packages/com.unity.inputsystem@1.11/license/LICENSE.html
 
- Unity Technologies Inc.: [Interior House Assets | URP](https://assetstore.unity.com/packages/3d/environments/interior-house-assets-urp-257122)
  - Reference: Unity Technologies Inc. Interior House Assets | URP. Available: https://assetstore.unity.com/packages/3d/environments/interior-house-assets-urp-257122.
  - Version: 1.0
  - License: https://unity.com/legal/as-terms

- Unity Technologies Inc.: [Starter Assets: Character Controllers | URP](https://assetstore.unity.com/packages/essentials/starter-assets-character-controllers-urp-267961)
  - Reference: Unity Technologies Inc. Starter Assets: Character Controllers | URP. Available: https://assetstore.unity.com/packages/essentials/starter-assets-character-controllers-urp-267961.
  - Version: 2.0.2
  - License: https://unity.com/legal/as-terms

- Unity Technologies Inc.: [Splines](https://docs.unity3d.com/Packages/com.unity.splines@2.7/manual/index.html)
  - Reference: Unity Technologies Inc. Splines. See: https://docs.unity3d.com/Packages/com.unity.splines@2.7/manual/index.html.
  - Version 2.7.2
  - License: https://docs.unity3d.com/Packages/com.unity.splines@2.7/license/LICENSE.html

## Library Dependency References (included as DLL references within the ./Plugins folder)

- LBS.PdtCfwComponents.dll: [pdt-cfw-components](https://github.com/programming-digital-twins/pdt-cfw-components)
  - Reference: Andrew D. King. Programming Digital Twins Client Framework Components. (2024) [Online]. Available: https://github.com/programming-digital-twins/pdt-cfw-components.
    - NOTE: [pdt-cfw-components](https://github.com/programming-digital-twins/pdt-cfw-components) has its own dependencies, which can be found in its README.md documentation. These dependencies, along with its own DLL (LBS.PdtCfwComponents.dll) are contained in this repository as DLL's.
  - DLL Version: 0.1.6 (alpha)
  - DLL License: [MIT](https://github.com/programming-digital-twins/pdt-cfw-components/blob/alpha/LICENSE-CODE).

- DTDLParser.dll: [DTDLParser](https://github.com/digitaltwinconsortium/DTDLParser)
  - Reference: Digital Twin Consortium and contributors. (2024) [Online]. Available: https://github.com/digitaltwinconsortium/DTDLParser.
  - DLL Version: 1.0.52
  - DLL License: [MIT](https://licenses.nuget.org/MIT).

- MQTTnet.dll [MQTTnet](https://github.com/dotnet/MQTTnet)
  - Reference: .NET Foundation and contributors. MQTTnet .NET library for MQTT communications. (2024) [Online]. Available: https://github.com/dotnet/MQTTnet.
  - DLL Version: 4.3.7.1207
  - DLL License: [MIT](https://licenses.nuget.org/MIT).

- MQTTnet.Extensions.ManagedClient.dll [MQTTnet.Extensions.ManagedClient](https://www.nuget.org/packages/MQTTnet.Extensions.ManagedClient)
  - Reference: .NET Foundation and contributors. MQTTnet .NET library for MQTT communications. (2024) [Online]. Available: https://github.com/dotnet/MQTTnet.
  - DLL Version: 4.3.7.1207
  - DLL License: [MIT](https://licenses.nuget.org/MIT).

- Newtonsoft.Json.dll: [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json)
  - Reference: James Newton-King. Json.NET JSON framework for .NET. (2024) [Online]. Available: https://github.com/JamesNK/Newtonsoft.Json.
  - DLL Version: 13.03
  - DLL License: [MIT](https://licenses.nuget.org/MIT).

- Microsoft.Bcl.AsyncInterfaces.dll
  - Reference: .NET Foundation and contributors. Microsoft.Bcl.AsyncInterfaces. (2025) [Online]. Available: https://www.nuget.org/packages/Microsoft.Bcl.AsyncInterfaces
  - DLL Version: 9.0.0
  - DLL License: [MIT](https://licenses.nuget.org/MIT).

- System.Buffers.dll
  - Reference: TBA. (2025) [Online]. Available: https://www.nuget.org/packages/System.Buffers
  - DLL Version: 4.6.0
  - DLL License: [MIT](https://licenses.nuget.org/MIT).

- System.IO.Pipelines.dll
  - Reference: TBA. (2025) [Online]. Available: https://www.nuget.org/packages/System.IO.Pipelines
  - DLL Version: 9.0.0
  - DLL License: [MIT](https://licenses.nuget.org/MIT).

- System.Memory.dll
  - Reference: TBA. (2025) [Online]. Available: https://www.nuget.org/packages/System.Memory
  - DLL Version: 4.6.0
  - DLL License: [MIT](https://licenses.nuget.org/MIT).

- System.Numerics.Vectors.dll
  - Reference: TBA. (2025) [Online]. Available: https://www.nuget.org/packages/System.Numerics.Vectors
  - DLL Version: 4.6.0
  - DLL License: [MIT](https://licenses.nuget.org/MIT).

- System.Runtime.CompilerServices.Unsafe.dll
  - Reference: TBA. (2025) [Online]. Available: https://www.nuget.org/packages/System.Runtime.CompilerServices.Unsafe
  - DLL Version: 6.1.0
  - DLL License: [MIT](https://licenses.nuget.org/MIT).

- System.Text.Encodings.Web.dll
  - Reference: TBA. (2025) [Online]. Available: https://www.nuget.org/packages/System.Text.Encodings.Web
  - DLL Version: 9.0.0
  - DLL License: [MIT](https://licenses.nuget.org/MIT).

- System.Text.Json.dll
  - Reference: TBA. (2025) [Online]. Available: https://www.nuget.org/packages/System.Text.Json
  - DLL Version: 9.0.0
  - DLL License: [MIT](https://licenses.nuget.org/MIT).

- System.Threading.Tasks.Extensions.dll
  - Reference: TBA. (2025) [Online]. Available: https://www.nuget.org/packages/System.Threading.Tasks.Extensions
  - DLL Version: 4.6.0
  - DLL License: [MIT](https://licenses.nuget.org/MIT).

- System.ValueTuple.dll
  - Reference: TBA. (2025) [Online]. Available: https://www.nuget.org/packages/System.ValueTuple
  - DLL Version: 4.5.0
  - DLL License: [MIT](https://licenses.nuget.org/MIT).

NOTE: This section and [Third Party Notices.md](https://github.com/programming-digital-twins/LabBenchStudios-PDT-Unity/blob/alpha/Third%20Party%20Notices.md) will be updated if / when other dependencies are incorporated.

# OTHER IMPORTANT NOTES

This code base is under active development.

If any code samples or other technology this work contains, describes, and / or is subject to open source licenses or the intellectual property rights
of others, it is your responsibility to ensure that your use thereof complies with such licenses and/or rights.

# LICENSE

Assets and Models: [LICENSE-ASSETS](https://github.com/programming-digital-twins/LabBenchStudios-PDT-Unity/blob/alpha/LICENSE-ASSETS.md). The repository's non-code artifacts LICENSE file (e.g., documentation, prefabs, etc.) These artifacts are contained within the ./Documentation, ./Models, and ./Runtime/ProgrammingDigitalTwins/Prefabs path.

Source Code: [LICENSE-CODE](https://github.com/programming-digital-twins/LabBenchStudios-PDT-Unity/blob/alpha/LICENSE-CODE.md). The repository's code artifacts LICENSE file (e.g., source code [mostly C#]). These artifacts are contained within this repository's ./Runtime/ProgrammingDigitalTwins/Scripts path.

Third Party Libraries: See REFERENCES above and [Third Party Notices.md](https://github.com/programming-digital-twins/LabBenchStudios-PDT-Unity/blob/alpha/Third%20Party%20Notices.md). The repository's DLL dependencies notices file. These artifacts are contained within this repository's ./Plugins path.
