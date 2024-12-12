# Layout Elements

## Changing Which Data Is Stored In Each Element

The data for each element is stored in its `HelpText`. To change what data is stored in the `HelpText`, navigate to where `System.Windows.Automation.AutomationProperties.SetHelpText` is called in [Cell.cs](../Classroom-Seating-Planner/Classroom-Seating-Planner/cells/Cell.cs) and change the string that is passed.

The `HelpText` is formatted as follows:
```json
<... HelpText="keyOne: This is a string; keyTwo: true   ;keyThree : 1701">
```
Leading and trailing whitespace is ignored. Keys and values are separated by colons, `:` and key value pairs are separated by semicolons, `;`. 

---

[Back to README.](../README.md)
