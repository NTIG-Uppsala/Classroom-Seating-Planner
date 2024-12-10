import copy
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

# have list with constraints : different types (boolean/distance) and levels of importance (0-10)
constraints = [
    {
        "element": "A",
        "constraint": "closeTo",
        "constraintType": "distance",
        "constraintElement": "whiteboard",
        "importance": 10,
    },
    {
        "element": "B",
        "constraint": "closeTo",
        "constraintType": "distance",
        "constraintElement": "window",
        "importance": 5,
    },
    {
        "element": "C",
        "constraint": "closeTo",
        "constraintType": "distance",
        "constraintElement": "door",
        "importance": 5,
    },
    {
        "element": "D",
        "constraint": "farFrom",
        "constraintType": "distance",
        "constraintElement": "E",
        "importance": 10,
    },
    {
        "element": "E",
        "constraint": "closeTo",
        "constraintType": "distance",
        "constraintElement": "C",
        "importance": 5,
    },
    {
        "element": "E",
        "constraint": "notNextTo",
        "constraintType": "boolean",
        "constraintElement": "F",
        "importance": 10,
    },
]

grid = [[" ", " ", " "], [" ", " ", " "], [" ", " ", " "]]


# ---------------------------------------------
# ---------------------------------------------


def reset():
    available_students = students.copy()
    grid = [[" ", " ", " "], [" ", " ", " "], [" ", " ", " "]]


def calculate_distance(x1, y1, x2, y2):
    xDiff = abs(x1 - x2)
    yDiff = abs(y1 - y2)
    return math.sqrt(xDiff**2 + yDiff**2)


targets = {
    "whiteboard": {
        "target": "whiteboard",
        "x": 1,
        "y": 0,
        "max_distance": calculate_distance(1, 0, 2, 2),
    },
    "window": {
        "target": "window",
        "x": 2,
        "y": 0,
        "max_distance": calculate_distance(2, 0, 0, 2),
    },
    "door": {
        "target": "door",
        "x": 0,
        "y": 2,
        "max_distance": calculate_distance(
            0, 2, 2, 0
        ),  # TODO - function for finding coordinates furthest from object, also make "farthest point" another table
    },
}


def close_to_constraint_check(
    target, x, y, importance, debug
):  # TODO - implement that student can be close to another student
    if target in targets:
        distance_to_target = calculate_distance(
            x1=x, y1=y, x2=targets[target]["x"], y2=targets[target]["y"]
        )
        max_distance_to_target = targets[target]["max_distance"]

        constraint_score = (
            (max_distance_to_target - distance_to_target)
            / max_distance_to_target  # TODO - consider using (max_distance_to_target - min_distance_to_target)
            * importance
        )

        if debug:
            print(f"constraint score: {constraint_score}, constraint: closeTo {target}")
        return constraint_score
    else:
        return 0


def far_from_constraint_check(
    target, x, y, importance, debug
):  # TODO - implement that student can be far from an object
    if debug:
        print("farFrom")
    if target in available_students:
        if debug:
            print(
                f"constraint score: {importance}, constraint: farFrom {target} (not placed)"
            )
        return importance
    for table in tables:
        if table["occupant"] == target:
            distance_to_occupant = calculate_distance(
                x1=x, y1=y, x2=table["x"], y2=table["y"]
            )
            max_distance_to_occupant = calculate_distance(x1=0, y1=0, x2=2, y2=2)

            constraint_score = (
                distance_to_occupant / max_distance_to_occupant * importance
            )

            if debug:
                print(
                    f"constraint score: {constraint_score}, constraint: farFrom {target}"
                )
            return constraint_score


def not_next_to_constraint_check(target, x, y, importance, debug):
    if target in available_students:
        if debug:
            print(
                f"constraint score: {importance}, constraint: notNextTo, {target} (not placed)"
            )
        return importance

    for table in tables:
        if table["occupant"] == target:
            if not (
                (abs(x - table["x"]) == 1 and abs(y - table["y"]) == 0)
                or (abs(x - table["x"]) == 0 and abs(y - table["y"]) == 1)
            ):
                if debug:
                    print(
                        f"constraint score: {importance}, constraint: notNextTo, {target}"
                    )
                return importance
    return 0


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

            # Check what type of constraint it is TODO - closeTo for other students
            if constraint["constraint"] == "closeTo":
                seating_score += close_to_constraint_check(
                    constraint["constraintElement"],
                    x,
                    y,
                    constraint["importance"],
                    debug,
                )

            # Check what type of constraint it is
            if (
                constraint["constraint"] == "farFrom"
            ):  # TODO - something that makes it easy to check if constraint is regarding another student or if constraint is regarding an object
                seating_score += far_from_constraint_check(
                    constraint["constraintElement"],
                    x,
                    y,
                    constraint["importance"],
                    debug,
                )

            if constraint["constraint"] == "notNextTo":
                seating_score += not_next_to_constraint_check(
                    constraint["constraintElement"],
                    x,
                    y,
                    constraint["importance"],
                    debug,
                )

        elif constraint["constraintElement"] == element:
            if constraint["constraint"] == "farFrom":
                seating_score += far_from_constraint_check(
                    constraint["element"], x, y, constraint["importance"], debug
                )

            if constraint["constraint"] == "notNextTo":
                seating_score += not_next_to_constraint_check(
                    constraint["element"], x, y, constraint["importance"], debug
                )
    # print(f"Element: {element}, Table: {target_table['table']}, Score: {seating_score}")
    return seating_score


def place_student_at_table_and_return_score(element):
    available_tables = [table for table in tables if table["occupant"] == " "]

    # get available tables that meet constraints
    available_tables_with_constraints = (
        [  # TODO - make sure that a table can be selected
            table
            for table in available_tables
            if check_seating_score(element, table, False) > 0
        ]
    )

    # randomly select table from available tables with constraints
    if len(available_tables_with_constraints) > 0:
        random_table = random.choice(
            available_tables_with_constraints
        )  # TODO - make sure that it selects one of the best tables
        # print(
        #     f"element: {element}, table: {random_table}, constraint: {constraint["constraint"]}, seating score: {seating_score}"
        # )
        random_table["occupant"] = element
        available_students.remove(element)

        seating_score = check_seating_score(element, random_table, False)
        return seating_score
    else:
        return 0


def generate_random_seating_arrangement():

    seating_arrangement_score = 0

    sorted_constraints = (
        sorted(  # TODO - maybe sort by students total constraint importance
            constraints, key=lambda x: x["importance"], reverse=True
        )
    )

    # loop through constraints and place students referenced in constraints at tables
    for constraint in sorted_constraints:
        # print("\n-----------------------------------\n")
        if constraint["element"] in available_students:
            seating_arrangement_score += place_student_at_table_and_return_score(
                constraint["element"]
            )

        if constraint["constraintElement"] in available_students:
            seating_arrangement_score += place_student_at_table_and_return_score(
                constraint["constraintElement"]
            )

    return seating_arrangement_score


# --------------------------------------------- CODE BELOW IS FOR TESTING ---------------------------------------------

grid_generations = []

iterations = 10000

for i in range(iterations):
    # print("--------------------")
    available_students = copy.deepcopy(students)
    for table in tables:
        table["occupant"] = " "
        grid[table["y"]][table["x"]] = table["occupant"]

    reset()

    score = generate_random_seating_arrangement()
    for table in tables:
        grid[table["y"]][table["x"]] = table["occupant"]

    grid_generations.append({"score": score, "grid": copy.deepcopy(grid)})

# for grid_generation in grid_generations:
#     print("Score:", grid_generation["score"])
#     for row in grid_generation["grid"]:
#         print(row)
#     print("\n")

# print("--------------------")
grid_generations_total_score = 0
grid_generations_scores_list = []
scores_above_or_equal_to_30 = []
scores_above_or_equal_to_35 = []
scores_above_or_eqyal_to_38 = []
scores_above_or_equal_to_40 = []
scores_above_or_equal_to_50 = []
scores_above_or_equal_to_60 = []
for grid_generation in grid_generations:

    grid_generations_total_score += grid_generation["score"]

    if grid_generation["score"] >= 30:
        scores_above_or_equal_to_30.append(grid_generation["score"])

    if grid_generation["score"] >= 35:
        scores_above_or_equal_to_35.append(grid_generation["score"])

    if grid_generation["score"] >= 38:
        scores_above_or_eqyal_to_38.append(grid_generation["score"])

    if grid_generation["score"] >= 40:
        scores_above_or_equal_to_40.append(grid_generation["score"])

    if grid_generation["score"] >= 50:
        scores_above_or_equal_to_50.append(grid_generation["score"])

    if grid_generation["score"] >= 60:
        scores_above_or_equal_to_60.append(grid_generation["score"])
    grid_generations_scores_list.append(grid_generation["score"])


def print_score_stats(scores, threshold, iterations):
    max_count_string_length = len(str(iterations))
    scores_count_string = str(len(scores))
    scores_rate = len(scores) / iterations * 100
    scores_rate_string = str(scores_rate)
    if len(scores_count_string) < max_count_string_length:
        scores_count_string = (
            " " * (max_count_string_length - len(scores_count_string))
            + scores_count_string
        )

    if len(str(int(scores_rate))) < 3:
        scores_rate_string = " " * (3 - len(str(int(scores_rate)))) + scores_rate_string

    if len(scores) > 0:
        print(
            f"Scores above {threshold}: {scores_count_string} | {scores_rate_string}%\n"
        )


if not not not not (not not True) == (not not not False):  # print score stats

    print("")
    # print_score_stats(scores_above_or_equal_to_30, 30, iterations)
    # print_score_stats(scores_above_or_equal_to_35, 35, iterations)
    # print_score_stats(scores_above_or_eqyal_to_38, 38, iterations)
    print_score_stats(scores_above_or_equal_to_40, 40, iterations)
    print_score_stats(scores_above_or_equal_to_50, 50, iterations)
    print_score_stats(scores_above_or_equal_to_60, 60, iterations)
    print(f"Total score: {grid_generations_total_score}")
    print(f"Average score: {grid_generations_total_score / len(grid_generations)}")
    print(f"Max score in list: {max(grid_generations_scores_list)}\n")

# function to generate a random seating arrangement

#   start calculating average score based on all students with constraints

#   sort students by importance of constraints

#       for each student with constraints

#           call function that checks how well constraints are met when student is placed at a table

#           ??make sure constraints are met at a level that is reflective of the importance of the constraints

#   return seating arrangement and score


# call function to generate a random seating arrangement until there are enough good seating arrangements

# pick one of the good seating arrangements and make sure it is different from the 5 most recent seating arrangements

# ide: spara alla möjliga positioner för elev samt positionernas poäng i en lista och efter alla listor är genererade, beräkna bästa bordsplacering
# problem: hur hantera att en elev inte kan sitta bredvid en annan elev
# lösning?: backtracking algoritm

# ide: spara poäng för constraints som är relaterade till objekt som tavla och dörr i en lista och använd listan för att beräkna
# plats när elev har sådan constraint

# grid_list = []
# grid = [[" ", " ", " "], [" ", " ", " "], [" ", " ", " "]]
# for i in range(10):
# slumpa grid
# grid_list.append(grid)
# for grid in grid_list:
# for row in grid:
# print(row)
# print("\n")

# alla grids kommer vara likadana
