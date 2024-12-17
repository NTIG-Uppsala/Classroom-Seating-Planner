# Constraints

This document outlines the constraint system used for generating optimal seating arrangements, including the algorithm, syntax, and available constraints.

## Algorithm

The program generates seating arrangements using the following algorithm:

1. Sort the students by the sum of the priority of their constraints.
1. For each student, do the following steps:
    1. In the following steps, the student is placed and a score is returned.
        - A higher score is given if the seating complies better with the student's constraints.
    1. If the student doesn't have any constraints, place them in a random seat and return a score of 0.
    1. Get a list of the tables sorted by how well each table follows the student's constraints.
        - This is calculated using the scoring system and the constraint methods.
    1. Select the best tables using the following function: $tableSelectionFactor(p) = 0.85^{p-1} \cdot 0.3$ where p is the sum of the priorities for that student and tableSelectionFactor(p) is a factor of how many tables are to be selected.
    1. Choose a random table among those previously selected.
    1. Return the score of that table

## Syntax

The syntax for writing constraints in `klasslista.txt` is as follows:

```txt
Name Surname: constraint target (priority) / other constraint target (priority)
```

-   `#` marks a comment and makes the program ignore that line.
-   `:` marks the divide between the name of the student and the list of constraints.
-   `/` marks the divide between different constraints.
-   Each constraint contains the following information:
    -   A constraint name (string).
    -   A constraint target such as the whiteboard or another student (string).
    -   A priority (number, optional - defaults to 1 if omitted).

```txt
#Example:
Johan Andersson: nära tavlan (3) / inte bredvid Anders Johansson (10)
#Translation: Johan Andersson: near whiteboard (3) / not next to Anders Johansson (10)
```

## Existing Constraints

The program currently has the following constraints:

-   `distance`:
    -   Parameters:
        -   source: Student calling the function
        -   target: Student or Classroom Element being checked against
        -   nearOrFar: If the source should be near or far from the target, allowing the inverse constraint of being seated far apart.
        -   priority: Multiplier for the constraint's score
        -   references: An object containing a list of all classroom elements (tables, the whiteboard, windows, doors).
-   `adjacent`:
    -   Parameters:
        -   source: Student calling the function
        -   target: Student or Classroom Element being checked against
        -   yesOrNo: If the source should be adjacent to the target or not, allowing the inverse constraint of not being adjacent.
        -   priority: Multiplier for the constraint's score
        -   references: An object containing a list of all classroom elements (tables, the whiteboard, windows, doors).

## Adding more Constraints

To add more constraints, follow these steps:

1. Navigate to `ConstraintFunctions` in [`ConstraintsHandler.cs`](../Classroom-Seating-Planner/Classroom-Seating-Planner/Src/ConstraintsHandler.cs).
1. Create a method for the constraint. It has to contain the same parameters as the ones listed above, except for the third, which should be a unique parameter indicating an inverse of the constraint.
1. Write the method logic for figuring out how good the seating is. This step is entirely dependent on the constraint being added.
1. Return a score depending on how good the seating is. It should follow this format:
    - Return a value between 0 and 1 where 1 is the optimal seating and 0 is the worst possible, multiplied with the priority.
1. In `functionLookupTable` in `InterpretStudentConstraints` in [`ConstraintsHandler.cs`](../Classroom-Seating-Planner/Classroom-Seating-Planner/Src/ConstraintsHandler.cs) add the possible aliases that the function could use:

    - ```csharp
      // Example:
      { "intebredvid",  new Constraint { type = "adjacent", arguments = [studentName, "no",   null], priority = 1 }},
      { "inte bredvid", new Constraint { type = "adjacent", arguments = [studentName, "no",   null], priority = 1 }},
      ```
    - The key represents the alias that a user might write in `klasslista.txt` to add the constraint to a student.
    - `type` is the method name for the constraint
    - `arguments` needs to contain three items where the first should always be `studentName` and the third should always be `null`. The second should indicate if the constraint should be inverse or not.
    - `priority` is the default priority of the constraint.

## Existing Classroom Layout Elements

The program currently supports the following Classroom Layout Elements:

-   Seats, represented by a `B` per seat in `bordskarta.txt`
-   Whiteboard, represented by one or multiple `T` in `bordskarta.txt`
-   Doors, represented by a `D` per door in `bordskarta.txt`
-   Windows, represented by a `F` per window in `bordskarta.txt`

Example layout in `bordskarta.txt`:

```txt
     TTTT

BBBB BBBB BBBB

BBBB BBBB BBBB

BBBB BBBB BBBB
```

## Adding more Classroom Layout Elements

To add new classroom layout elements that constraints might need, follow these steps:

1. Navigate to `recipientLookupTable` in `InterpretStudentConstraints` in [`ConstraintsHandler.cs`](../Classroom-Seating-Planner/Classroom-Seating-Planner/Src/ConstraintsHandler.cs).
1. Add the name aliases needed for the element:

    - ```csharp
      // Example:
      {"dörren",       "door"},
      {"dörr",         "door"},
      ```

    - The key should be what the user might type in when referring to the element.
    - The value should be a standardized name of the element.

1. Navigate to `cellTypes` in `GetClassroomElementsFromLayout` in [`FileHandler.cs`](../Classroom-Seating-Planner/Classroom-Seating-Planner/Src/FileHandler.cs)
1. Add the classroom layout aliases for the element:

    - ```csharp
      // Example:
      { 'B', "table" },
      { 'T', "whiteboard" },
      ```
    - The key should be the character used to mark the element in `bordskarta.txt`
    - The value should be the standardized name of the element.

1. Navigate to `classroomElementNames` in `SeatStudent` in [`SeatingHandler.cs`](../Classroom-Seating-Planner/Classroom-Seating-Planner/Src/SeatingHandler.cs).
1. Add the standardized to the list.

---

[Back to README.](../README.md)
