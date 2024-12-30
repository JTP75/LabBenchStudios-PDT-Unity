# Programming Digital Twins - [LabBenchStudios-PDT-UnityPlugin](https://github.com/programming-digital-twins/LabBenchStudios-PDT-UnityPlugin)
This is the plugin repository for [Unity](https://unity.com/)-based software and other application-specific components (written in C#)
related to my Building Digital Twins course at Northeastern University. The intent of this repository is to provide students with a
simple [Unity](https://unity.com/) plugin that can provide the basis for a simple digital twin implementation for personal testing
and validation related to the lab module assignments that are part of the Building Digital Twins course.

For convenience to the reader, some of the basic functionality has already been implemented, with other key components requiring implementation
by users of the repository (e.g., students taking my Digital Twins Programming course).

## Links, Exercises, Updates, Errata, and Clarifications

Kanban Board Exercises: [Programming Digital Twins Requirements](https://github.com/orgs/programming-digital-twins/projects/1)

For other references, see the following links to access exercises for this project. Please note that many of the exercises and sample source code
in this repository is based on many of the examples outlined in the [Programming the IoT Kanban Board](https://github.com/orgs/programming-the-iot/projects/1),
which is aligned with my book, [Programming the Internet of Things Book](https://learning.oreilly.com/library/view/programming-the-internet/9781492081401/).
 - [Original Constrained Device Application Source Code Template](https://github.com/programming-the-iot/python-components)
 - [Programming Digital Twins Edge Components Source Code Template](https://github.com/programming-digital-twins/pdt-edge-components)
 - [Programming the Internet of Things Book](https://learning.oreilly.com/library/view/programming-the-internet/9781492081401/)

## How to use this repository
If you're reading [Programming the Internet of Things: An Introduction to Building Integrated, Device to Cloud IoT Solutions](https://learning.oreilly.com/library/view/programming-the-internet/9781492081401),
you'll see a partial tie-in with the exercises described in each chapter and this repository.

## This repository aligns to exercises in Programming Digital Twins, and partially to Programming the Internet of Things
These components are all written in C# and are partially based on, although different from, the exercises designed for my book
[Programming the Internet of Things: An Introduction to Building Integrated, Device to Cloud IoT Solutions](https://learning.oreilly.com/library/view/programming-the-internet/9781492081401).

## How to navigate the directory structure of this repository
This repository is comprised of the following key paths:
- [LBS-PDT-UnityPlugin](https://github.com/programming-digital-twins/LabBenchStudios-PDT-UnityPlugin): All other assets are contained within this path.
  - [Documentation](https://github.com/programming-digital-twins/LabBenchStudios-PDT-UnityPlugin/Documentation): Contains plugin documentation.
  - [Models](https://github.com/programming-digital-twins/LabBenchStudios-PDT-UnityPlugin/Models): Contains both DTDL and type mapping JSON models.
    - [Dtdl](https://github.com/programming-digital-twins/LabBenchStudios-PDT-UnityPlugin/Models/Dtdl): Digital Twin Definition Language (DTDL) JSON models.
    - [Types](https://github.com/programming-digital-twins/LabBenchStudios-PDT-UnityPlugin/Models/Types): JSON-based type and constraint mapping models.
  - [Plugins](https://github.com/programming-digital-twins/LabBenchStudios-PDT-UnityPlugin/Plugins): Contains all pre-built open-source DLL dependencies (see [Third Party Notices.md](https://github.com/programming-digital-twins/LabBenchStudios-PDT-UnityPlugin/blob/default/Third%20Party%20Notices.md)).
  - [Runtime](https://github.com/programming-digital-twins/LabBenchStudios-PDT-UnityPlugin/Runtime): Contains all Unity-specific prefabs and C# scripts.
    - [ProgrammingDigitalTwins](https://github.com/programming-digital-twins/LabBenchStudios-PDT-UnityPlugin/Runtime/ProgrammingDigitalTwins): Containing folder.
      - [Prefabs](https://github.com/programming-digital-twins/LabBenchStudios-PDT-UnityPlugin/Runtime/ProgrammingDigitalTwins/Prefabs): Unity-specific prefabs.
      - [Scripts](https://github.com/programming-digital-twins/LabBenchStudios-PDT-UnityPlugin/Runtime/ProgrammingDigitalTwins/Scripts): Unity-specific C# scripts.
        - [Unity](https://github.com/programming-digital-twins/LabBenchStudios-PDT-UnityPlugin/Runtime/ProgrammingDigitalTwins/Scripts/Unity): Namespace containing folder (to help avoid naming collisions).
          - [Common](https://github.com/programming-digital-twins/LabBenchStudios-PDT-UnityPlugin/Runtime/ProgrammingDigitalTwins/Scripts/Unity/Common): Base game object and utility classes.
          - [Controller](https://github.com/programming-digital-twins/LabBenchStudios-PDT-UnityPlugin/Runtime/ProgrammingDigitalTwins/Scripts/Unity/Controller): Simple animation controller classes.
          - [Dashboard](https://github.com/programming-digital-twins/LabBenchStudios-PDT-UnityPlugin/Runtime/ProgrammingDigitalTwins/Scripts/Unity/Dashboard): Simple dashboard controllers (3D embeddable).
          - [Hud](https://github.com/programming-digital-twins/LabBenchStudios-PDT-UnityPlugin/Runtime/ProgrammingDigitalTwins/Scripts/Unity/Hud): Simple HUD controllers (2D display).
          - [Manager](https://github.com/programming-digital-twins/LabBenchStudios-PDT-UnityPlugin/Runtime/ProgrammingDigitalTwins/Scripts/Unity/Manager): Primary system and keyboard manager components.
          - [Model](https://github.com/programming-digital-twins/LabBenchStudios-PDT-UnityPlugin/Runtime/ProgrammingDigitalTwins/Scripts/Unity/Model): Simple collision controller classes.
          - [Sample](https://github.com/programming-digital-twins/LabBenchStudios-PDT-UnityPlugin/Runtime/ProgrammingDigitalTwins/Scripts/Unity/Sample): Sample threshold crossing animation controller classes.

Here are some other files at the top level that are important to review:
- [README.md](https://github.com/programming-digital-twins/pdt-unity-components/blob/alpha/README.md): This README.
- [LICENSE](https://github.com/programming-digital-twins/pdt-unity-components/blob/alpha/LICENSE): The repository's non-code artifact LICENSE file (e.g., documentation, prefabs, etc.)
- [LICENSE-CODE](https://github.com/programming-digital-twins/pdt-unity-components/blob/alpha/LICENSE-CODE): The repository's code artifact LICENSE file (e.g., source code [mostly C#])

NOTE: The directory structure and all files are subject to change based on feedback I receive from readers of my book and students in my IoT class,
as well as improvements I find to be helpful for overall repo betterment.

# Other things to know

## Pull requests
PR's are disabled while the codebase is being developed.

## Updates
Much of this repository, and in particular unit and integration tests, will continue to evolve, so please check back regularly for potential updates.
Please note that API changes can - and likely will - occur at any time.

# REFERENCES
This repository has external dependencies on other open source projects. I'm grateful to the open source community and authors / maintainers of the following libraries:

- [pdt-cfw-components](https://github.com/programming-digital-twins/pdt-cfw-components)
  - Reference: Andrew D. King. Programming Digital Twins Client Framework Components. (2024) [Online]. Available: https://github.com/programming-digital-twins/pdt-cfw-components.
  - NOTE: [pdt-cfw-components](https://github.com/programming-digital-twins/pdt-cfw-components) has its own dependencies, which can be found in its README.md documentation.

# OTHER REFERENCES
This repository is intended to be used as part of a Unity 3D project (see reference below). It is NOT intended to be used standalone.

- [Unity 6](https://unity.com/)
  - Reference: Unity Technologies. Unity Editor. (2024) [Online]. Available: https://unity.com/.
  - NOTE: This codebase and its references may function with other Unity Editor versions; however, all testing currently utilizes version 6000.0.32f1.

- Third party libraries and references
  - See [Third Party Notices.md](https://github.com/programming-digital-twins/LabBenchStudios-PDT-UnityPlugin/blob/default/Third%20Party%20Notices.md).


NOTE: This list will be updated as others are incorporated.

# FAQ
For typical questions (and answers) to the repositories of the Programming the IoT project, please see the [FAQ](https://github.com/programming-the-iot/book-exercise-tasks/blob/default/FAQ.md).

# IMPORTANT NOTES
This code base is under active development.

If any code samples or other technology this work contains, describes, and / or is subject to open source licenses or the intellectual property rights
of others, it is your responsibility to ensure that your use thereof complies with such licenses and/or rights.

# LICENSE
Assets and Models: Please see [LICENSE](https://github.com/programming-digital-twins/pdt-unity-components/blob/alpha/LICENSE) if you plan to use these assets.
Source Code: Please see [LICENSE-CODE](https://github.com/programming-digital-twins/pdt-unity-components/blob/alpha/LICENSE-CODE) if you plan to use this code.
