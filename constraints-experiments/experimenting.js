const fs = require("fs");

// Grid:
// {
//      "centerX": 1,
//      "centerY": 0,
//      "type": "whiteboard"
//  },
// {
//     "centerX": 2,
//     "centerY": 3,
//     "type": "table",
//     "student": null
// }

// People:
// {
//      "name": "John Doe",
//      "constraints": {
//          "closeToWhiteboard": true, // or undefined
//      }
// },

const getDistance = (cell1, cell2) => {
    const x = Math.abs(cell1.centerX - cell2.centerX);
    const y = Math.abs(cell1.centerY - cell2.centerY);
    return Math.sqrt(x ** 2 + y ** 2);
};

const getFactorial = (n) => {
    return Array.from({ length: n }, (_, i) => i + 1).reduce((acc, curr) => acc * curr, 1);
};

const getConstraintCombinations = (people) => {
    const constraints = [];
    people.forEach((person) => {
        constraints.push(person.constraints);
    });
    return constraints.filter((value, index, self) => index === self.findIndex((t) => JSON.stringify(t) === JSON.stringify(value)));
};

const groupPeopleByConstraints = (people, constraints) => {
    return constraints.map((constraint) => {
        return people.filter((person) => {
            return Object.entries(constraint).every(([key, value]) => person.constraints[key] === value);
        });
    });
};

function* generateClassroomPermutations(groups) {
    // Generate all permutations of the "groups"
    function* permute(arr) {
        if (arr.length === 1) {
            yield arr;
        } else {
            for (let i = 0; i < arr.length; i++) {
                const current = arr[i];
                const rest = arr.slice(0, i).concat(arr.slice(i + 1));
                for (const perm of permute(rest)) {
                    yield [current, ...perm];
                }
            }
        }
    }

    // Generate the group permutations
    for (const groupPermutation of permute(groups)) {
        // Flatten the groupPermutation array and yield each unique seating arrangement
        yield groupPermutation.flat();
    }
}

const getBestPermutations = (grid, people) => {
    const peopleWithConstraints = people.filter((person) => Object.values(person.constraints).some(Boolean));
    const tables = grid.filter((cell) => cell.type === "table");
    const constraints = getConstraintCombinations(peopleWithConstraints);
    const k = constraints.length;
    const n = tables.length;
    const numberOfPermutations = getFactorial(n) / getFactorial(n - k);
    const whiteboard = grid.filter((cell) => cell.type === "whiteboard")[0];

    let highestPenalty;
    const bestPermutations = [];

    // Group people based on constraints
    const groupedSeats = groupPeopleByConstraints(people, constraints);

    // Generate each permutation of seating arrangements using the generator
    const generator = generateClassroomPermutations(groupedSeats);
    let result = generator.next();
    while (!result.done) {
        const permutation = result.value;
        let penalty = 0;

        // Iterate over the generated permutation and calculate penalties
        permutation.forEach((person) => {
            if (person.table) {
                // Check if the person is assigned a table
                if (person.constraints.closeToWhiteboard) {
                    const distance = getDistance(whiteboard, person.table);
                    penalty += distance;
                }

                // Add the penalty for other constraints below, e.g., closeToDoor, etc.
            }
        });

        const permutationObject = {
            permutation,
            penalty,
        };

        if (bestPermutations.length < 5) {
            bestPermutations.push(permutationObject);
            highestPenalty = Math.max(...bestPermutations.map((permutationObject) => permutationObject.penalty));
            result = generator.next(); // Move to next permutation
            continue;
        }

        if (penalty < highestPenalty) {
            const index = bestPermutations.findIndex((permutationObject) => permutationObject.penalty === highestPenalty);
            bestPermutations[index] = permutationObject;
            highestPenalty = Math.max(...bestPermutations.map((permutationObject) => permutationObject.penalty));
        }

        result = generator.next(); // Move to next permutation
    }

    return bestPermutations;
};




const drawGridWithPeopleToConsole = (grid, people) => {
    const maxX = Math.max(...grid.map((cell) => cell.centerX));
    const maxY = Math.max(...grid.map((cell) => cell.centerY));

    const filledGrid = Array.from({ length: maxY + 1 }, () => Array(maxX + 1).fill(" "));

    grid.forEach((cell) => {
        filledGrid[cell.centerY][cell.centerX] = cell.type === "whiteboard" ? "T" : "_";
    });

    people.forEach((person) => {
        filledGrid[person.table.centerY][person.table.centerX] = person.person.name[0];
    });

    console.log(filledGrid);
};

const handleConstraints = (grid, people) => {
    const permutationObjects = getBestPermutations(grid, people);

    // Sort the permutationObjects array by penalty, lowest to highest
    permutationObjects.sort((a, b) => a.penalty - b.penalty);

    // Log the five best permutations to the console using drawGridWithPeopleToConsole
    permutationObjects.slice(0, 5).forEach((permutationObject) => {
        console.log(`Penalty: ${permutationObject.penalty}`);
        drawGridWithPeopleToConsole(grid, permutationObject.permutation);
    });
};

const main = () => {
    const people = JSON.parse(fs.readFileSync("constraints-experiments/list.json", "utf-8"));
    const grid = JSON.parse(fs.readFileSync("constraints-experiments/data.json", "utf-8"));
    handleConstraints(grid, people);
};

main();
