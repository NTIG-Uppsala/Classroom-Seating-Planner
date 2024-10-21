# Publishing a New Release

## Publishing a Build

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
10. Navigate to `Classroom-Seating-Planner\Classroom-Seating-Planner\bin\Release\net8.0-windows\win-x64\publish\win-x64` in the file explorer.

## Configuring the Profile

The following steps only needs to be done the first time:

1. You are prompted to choose a `Target`. Choose `Folder`.
2. You are prompted to choose a `Specific target`. Choose `Folder`.
3. You are prompted to choose a `Location`. Just press `Finish`.
4. Press `Close`.
5. You are now on the publish page. Under `Settings` click `Show all settings`.
6. Set `Deployment mode` to `Self-contained`.
7. Click `Save`.

---

[Back to README.](../README.md)
