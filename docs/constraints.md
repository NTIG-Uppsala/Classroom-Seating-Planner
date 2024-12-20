# Constraints


## Syntax 
Student constraints are written directly to the class list file. The syntax is somewhat flexible. The syntax is as follows:
```txt
# Elev utan begränsningar
Namn Namnsson

# Elev med en begränsning
Namn Namnsson: begränsningstyp måltavlan

# Elev med flera begränsningar
Namn Namnsson: begränsningstyp måltavlan / en annan begränsning måltavla

# Elev med en prioriterad begränsning
Namn Namnsson: begränsningstyp måltavlan (prioritet)

# Elev med flera prioriterade begränsningar
Namn Namnsson: begränsningstyp måltavlan (prioritet) / en annan begränsning måltavla (prioritet)

# Exempel:
Johan Andersson: nära tavlan / inte bredvid Anna Kamp (3)
Anna Kamp: bredvid Johan Andersson
Emil Vin: nära tavlan (10)
```
- `#` marks a line as a comment and will be ignored. A line that has a comment at the end will __not__ be ignored.
- `:` separates the name of the student from the constraints (if there are any).
- `/` separates different constraints for the same student.
- `()` contains the priority of the constraint. If no priority is set, the default priority is `1`. When setting a priority, a value between `1` and `10` is recommended.

### Constraint Types
Constraint types are **not** case sensitive not whitespace sensitive. The following constraint types are supported:

| Constraint type | Description                                                                                                     |
| --------------- | --------------------------------------------------------------------------------------------------------------- |
| `nära`          | The student should be seated close to the target.                                                               |
| `inte nära`     | The student should be seated far from the target.                                                               |
| `långt från`    | The student should be seated far from the target.                                                               |
| `långt ifrån`   | The student should be seated far from the target.                                                               |
| `bredvid`       | The student should be seated next to the target, but if that is not possible, the student gets seated randomly. |
| `inte bredvid`  | The student should not be seated next to the target.                                                            |

### Targets
A constraint can either target another student or a classroom element. 

#### To target a student
To target a student, simply write the name of the student. The name must be the same as the name of the student in the class list. The name **is** case sensitive **and** whitespace sensitive.
```txt
Namn Namnsson: begränsningstyp Mål Målsson
```

#### To target a classroom element
When targeting a classroom element you need to use one of its aliases. 

| Element    | Alias                                                                        |
| ---------- | ---------------------------------------------------------------------------- |
| Whiteboard | `tavlan`, `tavla`, `whiteboard`, `whiteboards`, `svartatavlan`, `klösbrädan` |
<!-- Row break needed to prevent formatter from messing with comment -->
<!-- | Door       | `dörren`, `dörr`                 | -->
<!-- | Window     | `fönstret`, `fönster`, `vindöga` | -->

### Good to Know
If anything with the input is wrong, the program carries on without it. The program will not crash, but the constraint will not be applied and no warnings are given.


## Algorithm

The algorithm that decides where a student is seated is as follows:
1. Sort the students by the sum of the priorities of their constraints. Therefore the students with the higher priorities, the pickiest, are placed first.
1. Then for every student:
   1. If the student has no constraints, place them in a random seat. Keep in mind that an unconstrained student will be placed after the constrained students, due to the previous sorting.
   1. The student judges every empty seat based on their own constraints and saves this score for every table.
   1. The empty tables are then sorted by the score the student gave them. 
   1. The empty tables are then grouped by their score. The groupings are made from how similar their scores are. How similar they have to be is determined by a function. The function is $t(p, m) = m \cdot 0.95^{p-1} \cdot 0.1$ where $p$ is the sum of the priorities for that student and $m$ is the highest score. $t(p, m)$ gives the threshold for how similar the scores have to be to be grouped together. For example, $[501, 500, 20, 18, 0, 0] => [[501, 500], [20, 18], [0, 0]]$
   1. The student is then placed at a random table from the best group.
      - The higher the priority of the constraint, the smaller the groups are which leads to the student being placed in a better seat while also making it the selection less random.  


## Existing Constraints

The program currently has the following constraints:

- `distance(source, target, nearOrFar, priority, references)`
    - source: The table a student is currently judging.
    - target: Student or Classroom Element being checked against
    - nearOrFar: If the source should be near or far from the target, allowing the inverse constraint of being seated far apart.
    - priority: How important the constraint is. This will affect scoring.
    - references: An object containing a list of all classroom elements (tables, the whiteboard, windows, doors).
- `adjacent(source, target, yesOrNo, priority, references)`:
    - source: The table a student is currently judging.
    - target: Student or Classroom Element being checked against
    - yesOrNo: If the source should be adjacent to the target or not, allowing the inverse constraint of not being adjacent.
    - priority: How important the constraint is. This will affect scoring.
    - references: An object containing a list of all classroom elements (tables, the whiteboard, windows, doors).

## Adding More Constraints

To add more constraints, follow these steps:

1. Navigate to `ConstraintFunctions` in [`ConstraintsHandler.cs`](../Classroom-Seating-Planner/Classroom-Seating-Planner/Src/ConstraintsHandler.cs).
1. Create a method for the constraint. It has to contain the same parameters as the ones listed above. The third argument however can be tweaked as needed.
1. Write the method logic for figuring out how good the seating is. This step is entirely dependent on the constraint being added.
   - The function has to return a normalized score between 0 and 1, multiplied with the priority. A higher score is better.
1. In the same file, add new aliases for this constraint in `functionLookupTable`. Follow the same format as the existing constraints.
   

## Existing Classroom Layout Elements

The program currently supports reading the following Classroom Layout Elements that are read from the class room layout file:

| Element    | Character |
| ---------- | --------- |
| Table      | `B`       |
| Whiteboard | `T`       |
<!-- Row break needed to prevent formatter from messing with comment -->
<!-- | Door       | `D`       | -->
<!-- | Window     | `F`       | -->


## Adding More Classroom Layout Elements

To add new classroom layout elements that constraints might need, follow these steps:

1. Navigate to the `recipientLookupTable` dictionary in the `InterpretStudentConstraints` function in [`ConstraintsHandler.cs`](../Classroom-Seating-Planner/Classroom-Seating-Planner/Src/ConstraintsHandler.cs).
2. Add aliases the user can type in to refer to the element:
```c#
    // Example:
    // {"whattheusertypesin", "elementName"},
    {"dörren",               "door"},
    {"dörr",                 "door"},
```
   - The key is what the user types in the class list file. This should be all lowercase and without whitespace and in Swedish.
3. Create a class for the element in the `Cells` directory which extends `Cell`.
   - While extending `Cell`, make sure that the `cellType` is set to the same value as specified in the `recipientLookupTable`. Else the program will not be able to interpret the element.
   - Keep in mind that you can always compare to the existing cells for guidance.
4. Navigate to the `cellTypes` dictionary in `ParseClassroomElementsFromFile` function in [`FileHandler.cs`](../Classroom-Seating-Planner/Classroom-Seating-Planner/Src/FileHandler.cs)
5. Add the classroom layout aliases for the element:
```c#
    // Example:
    { 'B', (x, y) => new Cells.TableCell(x, y) },
    { 'T', (x, y) => new Cells.WhiteboardCell(x, y) },
```
   - The key is the character that will be interpreted from the classroom layout file.
   - The value is a function that instantiates the classroom elements class.
6. Navigate to `classroomElementNames` in `SeatStudent` in [`SeatingHandler.cs`](../Classroom-Seating-Planner/Classroom-Seating-Planner/Src/SeatingHandler.cs).
7. Add the name you gave the element in the `recipientLookupTable` to that list.

---

[Back to README.](../README.md)
