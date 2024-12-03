import math
import random

# have list with students
students = ["A", "B", "C", "D", "E", "F", "G", "H", "I", "J"]

# have list with available students
available_students = students.copy()

# have list with tables
tables = [
    {"table": "1", "occupant": " ", "x": 0, "y": 0},
    {"table": "2", "occupant": " ", "x": 1, "y": 0},
    {"table": "3", "occupant": " ", "x": 2, "y": 0},
    {"table": "4", "occupant": " ", "x": 0, "y": 1},
    {"table": "5", "occupant": " ", "x": 1, "y": 1},
    {"table": "6", "occupant": " ", "x": 2, "y": 1},
    {"table": "7", "occupant": " ", "x": 0, "y": 2},
    {"table": "8", "occupant": " ", "x": 1, "y": 2},
    {"table": "9", "occupant": " ", "x": 2, "y": 2},
]

# have list with available tables
available_tables = [table["table"] for table in tables if table["occupant"] == " "]
# print(available_tables)

# have list with constraints : different types (binary/dynamic) and levels of importance (0-10)
constraints = [
    {
        "element": "A",
        "constraint": "closeTo",
        "constraintType": "dynamic",
        "constraintElement": "whiteboard",
        "importance": 10,
    },
    {
        "element": "B",
        "constraint": "closeTo",
        "constraintType": "dynamic",
        "constraintElement": "window",
        "importance": 5,
    },
    {
        "element": "C",
        "constraint": "closeTo",
        "constraintType": "dynamic",
        "constraintElement": "door",
        "importance": 5,
    },
    {
        "element": "D",
        "constraint": "farFrom",
        "constraintType": "dynamic",
        "constraintElement": "E",
        "importance": 10,
    },
    {
        "element": "E",
        "constraint": "closeTo",
        "constraintType": "dynamic",
        "constraintElement": "C",
        "importance": 5,
    },
    {
        "element": "E",
        "constraint": "notNextTo",
        "constraintType": "binary",
        "constraintElement": "F",
        "importance": 10,
    },
]
grid = [[" ", " ", " "], [" ", " ", " "], [" ", " ", " "]]


def calculate_distance(x1, y1, x2, y2):
    xDiff = abs(x1 - x2)
    yDiff = abs(y1 - y2)
    # print(math.sqrt(xDiff**2 + yDiff**2))
    return math.sqrt(xDiff**2 + yDiff**2)


def check_seating_score(
    element, target_table, debug
):  # TODO - make sure that a score is set for constraints where the target constraint element is not placed
    seating_score = 0

    # Get constraints for element sorted by importance
    element_constraints = sorted(
        [
            constraint
            for constraint in constraints
            if (constraint["element"] == element)
            or (constraint["constraintElement"] == element)
        ],
        key=lambda x: x["importance"],
        reverse=True,
    )

    if debug:
        print(element)
        for constraint in element_constraints:
            print(constraint)

    # Get x and y coordinates for target_table
    x = target_table["x"]
    y = target_table["y"]

    # Loop through constraints
    for constraint in element_constraints:
        # Check if the element passed is the root element of the constraint
        if constraint["element"] == element:

            # Check what type of constraint it is
            if constraint["constraint"] == "closeTo":
                if constraint["constraintElement"] == "whiteboard":
                    distance_to_whiteboard = calculate_distance(x1=x, y1=y, x2=1, y2=0)
                    max_distance_to_whiteboard = calculate_distance(
                        x1=1, y1=0, x2=2, y2=2
                    )

                    constraint_score = (
                        (max_distance_to_whiteboard - distance_to_whiteboard)
                        / max_distance_to_whiteboard
                        * constraint["importance"]
                    )

                    seating_score += constraint_score
                    if debug:
                        print(
                            f"constraint score: {constraint_score}, constraint: {constraint["constraint"]}, {constraint["constraintElement"]}"
                        )

                elif constraint["constraintElement"] == "window":
                    distance_to_window = calculate_distance(x1=x, y1=y, x2=2, y2=0)
                    max_distance_to_window = calculate_distance(x1=2, y1=0, x2=0, y2=2)

                    constraint_score = (
                        (max_distance_to_window - distance_to_window)
                        / max_distance_to_window
                        * constraint["importance"]
                    )

                    seating_score += constraint_score
                    if debug:
                        print(
                            f"constraint score: {constraint_score}, constraint: {constraint["constraint"]}, {constraint["constraintElement"]}"
                        )

                elif constraint["constraintElement"] == "door":
                    distance_to_door = calculate_distance(x1=x, y1=y, x2=0, y2=2)
                    max_distance_to_door = calculate_distance(x1=0, y1=2, x2=2, y2=0)

                    constraint_score = (
                        (max_distance_to_door - distance_to_door)
                        / max_distance_to_door
                        * constraint["importance"]
                    )

                    seating_score += constraint_score
                    if debug:
                        print(
                            f"constraint score: {constraint_score}, constraint: {constraint["constraint"]}, {constraint["constraintElement"]}"
                        )

            # Check what type of constraing it is
            if (
                constraint["constraint"] == "farFrom"
            ):  # TODO - something that makes it easy to check if constraint is regarding another student or if constraint is regarding an object
                if debug:
                    print("farFrom")
                if constraint["constraintElement"] in available_students:
                    seating_score += constraint["importance"]
                    if debug:
                        print(
                            f"constraint score: {constraint["importance"]}, constraint: {constraint["constraint"]}, {constraint["constraintElement"]} (not placed)"
                        )
                    continue
                for table in tables:
                    print("checking table")
                    if table["occupant"] == constraint["constraintElement"]:
                        distance_to_occupant = calculate_distance(
                            x1=x, y1=y, x2=table["x"], y2=table["y"]
                        )
                        max_distance_to_occupant = calculate_distance(
                            x1=0, y1=0, x2=2, y2=2
                        )

                        constraint_score = (
                            distance_to_occupant
                            / max_distance_to_occupant
                            * constraint["importance"]
                        )

                        seating_score += constraint_score
                        if debug:
                            print(
                                f"constraint score: {constraint_score}, constraint: {constraint["constraint"]}, {constraint["constraintElement"]}"
                            )

            if constraint["constraint"] == "notNextTo":
                if constraint["constraintElement"] in available_students:
                    seating_score += constraint["importance"]
                    if debug:
                        print(
                            f"constraint score: {constraint["importance"]}, constraint: {constraint["constraint"]}, {constraint["constraintElement"]} (not placed)"
                        )
                    continue

                for table in tables:
                    if table["occupant"] == constraint["constraintElement"]:
                        if not (
                            (abs(x - table["x"]) == 1 and abs(y - table["y"]) == 0)
                            or (abs(x - table["x"]) == 0 and abs(y - table["y"]) == 1)
                        ):
                            seating_score += constraint["importance"]
                            if debug:
                                print(
                                    f"constraint score: {constraint["importance"]}, constraint: {constraint["constraint"]}, {constraint["constraintElement"]}"
                                )

        elif constraint["constraintElement"] == element:
            if constraint["constraint"] == "farFrom":
                if constraint["element"] in available_students:
                    seating_score += constraint["importance"]
                    if debug:
                        print(
                            f"constraint score: {constraint["importance"]}, constraint: {constraint["constraint"]}, {constraint["element"]} (not placed)"
                        )
                    continue

                for table in tables:
                    if table["occupant"] == constraint["element"]:
                        distance_to_occupant = calculate_distance(
                            x1=x, y1=y, x2=table["x"], y2=table["y"]
                        )
                        max_distance_to_occupant = calculate_distance(
                            x1=0, y1=0, x2=2, y2=2
                        )

                        constraint_score = (
                            distance_to_occupant
                            / max_distance_to_occupant
                            * constraint["importance"]
                        )

                        seating_score += constraint_score
                        if debug:
                            print(
                                f"constraint score: {constraint_score}, constraint: {constraint["constraint"]}, {constraint["element"]}"
                            )

            if constraint["constraint"] == "notNextTo":
                if constraint["element"] in available_students:
                    seating_score += constraint["importance"]

                    if debug:
                        print(
                            f"constraint score: {constraint["importance"]}, constraint: {constraint["constraint"]}, {constraint["element"]} (not placed)"
                        )
                    continue

                for table in tables:
                    if table["occupant"] == constraint["element"]:
                        if not (
                            (abs(x - table["x"]) == 1 and abs(y - table["y"]) == 0)
                            or (abs(x - table["x"]) == 0 and abs(y - table["y"]) == 1)
                        ):
                            seating_score += constraint["importance"]
                            if debug:
                                print(
                                    f"constraint score: {constraint["importance"]}, constraint: {constraint["constraint"]}, {constraint["element"]}"
                                )

    # print(f"Element: {element}, Table: {target_table['table']}, Score: {seating_score}")
    return seating_score


table = list(filter(lambda x: x["table"] == "1", tables))[0]

# print(check_seating_score("E", table))

# function to check how well constraints are met when student is placed at a table
#   calculate score based on constraints : average or sum - sum could be beneficial as someone could have a lot of importance 10
#               constraints and that should be reflected in the score of this students seating and in the score of the overall seating arrangement

#       if constraint is to be close to whiteboard
#           base score on distance to whiteboard and importance of constraint

#       if constraint is to be close to window
#           base score on distance to window and importance of constraint

#       if constraint is to be close to door
#           base score on distance to door and importance of constraint

#       if constraint is to be far from another student
#           base score on distance to other student and importance of constraint

#       if constraint is to not sit next to another student
#           base score whether constraint is met and importance of constraint

#       return score


def generate_random_seating_arrangement():

    seating_arrangement_score = 0

    sorted_constraints = sorted(
        constraints, key=lambda x: x["importance"], reverse=True
    )
    # print(sorted_constraints)

    # loop through constraints and place students referenced in constraints at tables
    for constraint in sorted_constraints:
        print("\n-----------------------------------\n")
        if constraint["element"] in available_students:
            # get available tables
            available_tables = [table for table in tables if table["occupant"] == " "]

            # get available tables that meet constraints
            available_tables_with_constraints = [
                table
                for table in available_tables
                if check_seating_score(constraint["element"], table, False) > 0
            ]

            # randomly select table from available tables with constraints
            if len(available_tables_with_constraints) > 0:
                random_table = random.choice(available_tables_with_constraints)
                seating_score = check_seating_score(
                    constraint["element"], random_table, True
                )
                seating_arrangement_score += seating_score
                print(
                    f"element: {constraint["element"]}, table: {random_table}, constraint: {constraint["constraint"]}, seating score: {seating_score}"
                )
                random_table["occupant"] = constraint["element"]
                available_students.remove(constraint["element"])

        if constraint["constraintElement"] in available_students:
            # print(
            #     f"constraint element: {constraint["constraintElement"]}, available students: {available_students}"
            # )
            # get available tables
            available_tables = [table for table in tables if table["occupant"] == " "]

            # get available tables that meet constraints
            available_tables_with_constraints = [
                table
                for table in available_tables
                if check_seating_score(constraint["constraintElement"], table, False)
                > 0
            ]
            print()
            # randomly select table from available tables with constraints
            if len(available_tables_with_constraints) > 0:
                random_table = random.choice(available_tables_with_constraints)
                seating_score = check_seating_score(
                    constraint["constraintElement"], random_table, True
                )
                seating_arrangement_score += seating_score
                print(
                    f"element: {constraint["constraintElement"]}, table: {random_table}, constraint: {constraint["constraint"]}, seating score: {seating_score}"
                )
                random_table["occupant"] = constraint["constraintElement"]
                available_students.remove(constraint["constraintElement"])

    # print(tables)
    # print(available_students)

    return seating_arrangement_score


print(generate_random_seating_arrangement())

for table in tables:
    grid[table["y"]][table["x"]] = table["occupant"]

for row in grid:
    print(row)

# function to generate a random seating arrangement

#   start calculating average score based on all students with constraints

#   sort students by importance of constraints

#       for each student with constraints

#           call function that checks how well constraints are met when student is placed at a table

#           ??make sure constraints are met at a level that is reflective of the importance of the constraints

#   return seating arrangement and score


# call function to generate a random seating arrangement until there are enough good seating arrangements

# pick one of the good seating arrangements and make sure it is different from the 5 most recent seating arrangements
