



## JS
1. The file reading functions: (#119-#193)
  - Make sure C# code does what the JS code does + creating default files and such
  - getStudent, getLayout are the functions we're interested in porting

interpretConstraints():
- ConstraintsHandler.cs

constraintFunctions:
- ConstraintHandler.cs

seatStudent():
- SeatingHandler.cs

main() => Populate():
- SeatingHandler.cs

fancyDraw() => sumtin that just fixes the visible stuff
- SeatingHandler.cs

## CS 
ConstraintHandler.cs:
- interpretConstraints()
- constraintFunctions

REMEMBER: remove the constraints from the class list element

## Dokumentation
Axel


## Viggos vision
```js
  class ClassroomLayoutManager {
    constructor() {
        this.grid = {XAML grid}
    }
    
    updateGridSize(classroomElements) {
        // Clear the grid
        this.grid.children = null;
        this.grid.rowDefs = null;
        this.grid.colDefs = null;
        // Find the row and col count
        const rowCount = classroomElements.maxX
        const colCount = classroomElements.maxY
        foreach rowCount:
            this.grid.rowdefs.add(new rowdef)
        foreach colCount:
            this.grid.coldefs.add(new coldef)
    }
    Render(classroomElements) {
        this.updateGridSize(classroomElements)
        // Draw all children
        classroomElements.forEach(cell => cell.draw(parent = this.grid))
    } 
    }
    MainWindow:
    loaded() {
      handleFileIssues(this)

      list classroomElements = GetClassroomElementsFromLayout()
      list students = GetClassListFromFile()
      
      ClassListElementHandle.Populate(...) // todo - fixa så att pop hanterar objekt ist för strängar
      layoutManager.Render(classroomElements);


      Src.DynamicClassroomSizeHandler.UpdateClassroomLayoutSize(windowSize, this)
    }
    buttonClinked() {
      list classroomElements = GetClassroomElementsFrom1Layout()
      list students = GetClassListFromFile()
        
      GenerateSeatingArrangement(students, classroomElements)
      
      layoutManager.Render(classroomElements)
  }
```