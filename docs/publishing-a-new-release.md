# Publishing a New Release

## Creating a Build in Visual Studio

To create a release follow these steps in Visual Studio:

1. Click `Build` on the menu bar and choose `Configuration Manager...`.
2. Set `Classroom-Seating-Planner`'s `Configuration` to `Release`.
3. Close the Configuration Manager.
4. Click `Build` on the menu bar and choose `Clean Solution`.
5. When the cleaning is completed, click `Build` on the menu bar and choose `Build Solution`.
6. Click `View` on the menu bar and choose `Solution Explorer`.
7. Right click `Classroom-Seating-Planner` in the solution explorer and choose `Publish...`.
8. If it is the first time you are publishing, follow the steps under [Configuring the Profile](#configuring-the-profile).
9. Click the `Publish` button that has an icon.
10. Navigate to [Classroom-Seating-Planner/Classroom-Seating-Planner/bin/Release/net8.0-windows/win-x64/publish/win-x64/](../Classroom-Seating-Planner/Classroom-Seating-Planner/bin/Release/net8.0-windows/win-x64/publish/win-x64/) in file explorer to find the built exe-file.

## Publishing a Build on GitHub

1. In GitHub, navigate to the [releases page](https://github.com/NTIG-Uppsala/Classroom-Seating-Planner/releases).
2. Click on `Draft a new release`.
3. Choose a tag, either choose an existing tag or create a new tag.
4. Choose a release name (follow the standard: `vN.N.N Release/Pre-Release`).
5. Write a description of the current release. If there is no content for any of the following headers, do not use the header. Headers should be defined with `##`.
    - Features
    - Changes
    - Fixes
6. Add your exe file as an attachment.
7. When product owner has approved the release, click on `Publish release`.

## Configuring the Profile

The following steps only needs to be done the first time:

1. You are prompted to choose a `Target`. Choose `Folder`.
2. You are prompted to choose a `Specific target`. Choose `Folder`.
3. You are prompted to choose a `Location`. Just press `Finish`.
4. Press `Close`.
5. You are now on the publish page. There is a `Settings` section. In that section, click `Show all settings`.
6. Set `Deployment mode` to `Self-contained`.
7. Click `Save`.
   [Back to Creating a Build in Visual Studio](#creating-a-build-in-visual-studio)

---

[Back to README.](../README.md)
