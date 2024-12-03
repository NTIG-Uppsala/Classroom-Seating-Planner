import random

grid = [
    ["A", "B", "C", "D"],
    ["E", "F", "G", "H"],
    ["I", "J", "K", "L"],
    ["M", "N", "O", "P"],
]

constraints = [
    {
        "element": "B",
        "constraintType": "notNextTo",
        "constraintElement": "C",
    },
]


def gridContainsElement(expectedElement):
    for row in grid:
        if expectedElement in row:
            return True
    return False


def elementViolatesConstraint(checkElement, posY, posX):
    for constraint in constraints:
        if (
            constraint["element"] == checkElement
            or constraint["constraintElement"] == checkElement
        ):

            print(checkElement, "has constraints")

            if constraint["constraintType"] == "notNextTo":

                print(f"constraint type: notNextTo")
                # check if the other element is already placed
                if gridContainsElement(
                    constraint["constraintElement"]
                ) or gridContainsElement(constraint["element"]):

                    print("constraint element already placed")
                    indexOfConstraintElement = []
                    indexOfElement = []

                    # get the position of the other element
                    for row in grid:
                        if constraint["constraintElement"] in row:
                            indexOfConstraintElement = [
                                grid.index(row),
                                row.index(constraint["constraintElement"]),
                            ]
                        if constraint["element"] in row:
                            indexOfElement = [
                                grid.index(row),
                                row.index(constraint["element"]),
                            ]
                    if constraint["constraintElement"] == checkElement:
                        indexOfConstraintElement = [posY, posX]

                    elif constraint["element"] == checkElement:
                        indexOfElement = [posY, posX]

                    print("constraint element index", indexOfConstraintElement)
                    print("element index", indexOfElement)
                    # check if the other element is next to the current element
                    if (
                        (
                            indexOfElement[0] == indexOfConstraintElement[0] + 1
                            and indexOfElement[1] == indexOfConstraintElement[1]
                        )
                        or (
                            indexOfElement[0] == indexOfConstraintElement[0] - 1
                            and indexOfElement[1] == indexOfConstraintElement[1]
                        )
                        or (
                            indexOfElement[1] == indexOfConstraintElement[1] + 1
                            and indexOfElement[0] == indexOfConstraintElement[0]
                        )
                        or (
                            indexOfElement[1] == indexOfConstraintElement[1] - 1
                            and indexOfElement[0] == indexOfConstraintElement[0]
                        )
                    ):
                        print("Constraint violated")
                        return True
    print("No constraints violated")
    return False


def getListOfElementsInGrid():
    listOfElementsInGrid = []
    for row in grid:
        for element in row:
            listOfElementsInGrid.append(element)
    return listOfElementsInGrid


def getAvailablePositionsInGrid():
    availablePositionsInGrid = []
    for row in range(0, len(grid)):
        print("row:", row)
        for position in range(0, len(grid[row])):
            print("position:", position)
            if grid[row][position] == " ":
                availablePositionsInGrid.append([row, position])

    return availablePositionsInGrid


def clearGrid():
    for row in grid:
        for element in row:
            grid[grid.index(row)][row.index(element)] = " "


listOfElements = getListOfElementsInGrid()
print("grid:", grid)
clearGrid()
print("grid:", grid)
availablePositions = getAvailablePositionsInGrid()
unAvailablePositions = []  # make a function to get unavailable positions

print("list of elements:", listOfElements)
print("available positions:", availablePositions)
# listOfElements.random.shuffle()

# randomize the position of the elements in grid
for element in listOfElements:

    print("--------------------")
    print("element:", element)
    randomIndex = random.randint(0, len(availablePositions) - 1)
    # print("randomIndex:", randomIndex)
    # print("random position y, x:", availablePositions[randomIndex])
    randomY = availablePositions[randomIndex][0]
    randomX = availablePositions[randomIndex][1]
    # print("randomY:", randomY)
    # print("randomX:", randomX)

    tempAvailablePositions = availablePositions
    tempUnAvailablePositions = unAvailablePositions
    randomY = tempAvailablePositions[randomIndex][0]
    randomX = tempAvailablePositions[randomIndex][1]

    # Constraint handling goes here
    constraintsAreViolated = elementViolatesConstraint(element, randomY, randomX)
    while constraintsAreViolated:
        # Try to get a valid position from available random positions
        if len(tempAvailablePositions) > 0:
            tempUnAvailablePositions.append(tempAvailablePositions[randomIndex])
            tempAvailablePositions.pop(randomIndex)
            randomIndex = random.randint(0, len(tempAvailablePositions) - 1)
            randomY = tempAvailablePositions[randomIndex][0]
            randomX = tempAvailablePositions[randomIndex][1]

        else:
            randomIndex = random.randint(0, len(tempUnAvailablePositions) - 1)
            randomY = tempUnAvailablePositions[randomIndex][0]
            randomX = tempUnAvailablePositions[randomIndex][1]

        constraintsAreViolated = elementViolatesConstraint(element, randomY, randomX)

    # Place element in the new position and remove the position from availablePositions
    # If the position is available, place the element in the position
    if grid[randomY][randomX] == " ":
        grid[randomY][randomX] = element

        unAvailablePositions.append(availablePositions[randomIndex])
        availablePositions.pop(randomIndex)
    # If the position is not available, swap the element in the position with the element
    else:
        grid[availablePositions[0][0]][availablePositions[0][1]] = grid[randomY][
            randomX
        ]
        grid[randomY][randomX] = element

        # Constraint check is necessary for the swapped element
        unAvailablePositions.append(availablePositions[0])
        availablePositions.pop(0)

    # print("grid:", grid)
    # print("availablePositions:", availablePositions)

for row in grid:
    print(row)

# indexOfB = []
# indexOfC = []

# for row in grid:
#     if "B" in row:
#         indexOfB = [grid.index(row), row.index("B")]
#     if "C" in row:
#         indexOfC = [grid.index(row), row.index("C")]


# print(indexOfB)
# print(indexOfC)
