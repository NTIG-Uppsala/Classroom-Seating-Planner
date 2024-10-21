# Publishing

## Publishing a build

To create a release follow these steps in Visual Studio:

1. Click `Build` on the menu bar and choose `Configuration Manager...`. Set `Classroom-Seating-Planner`'s `Configuration` to `Release`.
2. Click `Build` on the menu bar and choose `Clean Solution`.
3. Click `Build` on the menu bar and choose `Build Solution`.
4. Click `View` on the menu bar and choose `Solution Explorer`.
5. Right click `Classroom-Seating-Planner` in the solution explorer and choose `Publish...`.
6. If it is the first time you are publishing, follow the steps under [Configuring the profile](#configuring-the-profile).
7. Click `Publish`.
8. Navigate to `Classroom-Seating-Planner\Classroom-Seating-Planner\bin\Release\net8.0-windows\win-x64\publish\win-x64`.
9. The executable is in that directory.

## Configuring the profile

The following steps only needs to be done the first time:

1. You are prompted to choose a `Target`. Choose `Folder`.
2. You are prompted to choose a `Specific target`. Choose `Folder`.
3. You are prompted to choose a `Location`. Just press `Finish`.
4. Click `Show all settings` and set `Deployment mode` to `Self-contained`.
5. Click `Save`.

---

[Back to README.](../README.md)
