# have list with students
students = ["A", "B", "C", "D", "E", "F", "G", "H", "I", "J"]
# have list with available students
available_students = students.copy()
# have list with tables
tables = [
    {"table": "1", "x": 0, "y": 0},
    {"table": "2", "x": 1, "y": 0},
    {"table": "3", "x": 2, "y": 0},
    {"table": "4", "x": 0, "y": 1},
    {"table": "5", "x": 1, "y": 1},
    {"table": "6", "x": 2, "y": 1},
    {"table": "7", "x": 0, "y": 2},
    {"table": "8", "x": 1, "y": 2},
    {"table": "9", "x": 2, "y": 2},
]
# have list with available tables
available_tables = [table["table"] for table in tables]
print(available_tables)
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
        "constraint": "notNextTo",
        "constraintType": "binary",
        "constraintElement": "F",
        "importance": 10,
    },
]
grid = [[" ", " ", " "], [" ", " ", " "], [" ", " ", " "]]

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


# function to generate a random seating arrangement

#   start calculating average score based on all students with constraints

#   sort students by importance of constraints

#       for each student with constraints

#           call function that checks how well constraints are met when student is placed at a table

#           ??make sure constraints are met at a level that is reflective of the importance of the constraints

#   return seating arrangement and score


# call function to generate a random seating arrangement until there are enough good seating arrangements

# pick one of the good seating arrangements and make sure it is different from the 5 most recent seating arrangements
