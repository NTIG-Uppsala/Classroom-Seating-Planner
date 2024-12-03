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

const getAllPermutations = (grid, people) => {
    // All people with at least one value in the constraint object being a truthy value
    const peopleWithConstraints = people.filter((person) => Object.values(person.constraints).some(Boolean));
    const tables = grid.filter((cell) => cell.type === "table");
    const k = peopleWithConstraints.length;
    const n = tables.length;
    // Time complexity: O(n!/(n-k)!)
    const numberOfPermutations = getFactorial(n) / getFactorial(n - k);

    const permutations = [];

    // Create all possible permutations of peopleWithConstraints on the grid on cells that are type table

    for (let i = 0; i < numberOfPermutations; i++) {
        const permutation = [];
        const usedTables = new Set();
        for (let j = 0; j < k; j++) {
            let randomTableIndex;
            do {
                randomTableIndex = Math.floor(Math.random() * n);
            } while (usedTables.has(randomTableIndex));
            usedTables.add(randomTableIndex);
            permutation.push({
                person: peopleWithConstraints[j],
                table: tables[randomTableIndex],
            });
        }
        permutations.push(permutation);
    }

    return permutations;
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

    // Find all possible permutations of people with constraints on the grid
    const permutations = getAllPermutations(grid, people);

    // Shuffle the permutations array
    permutations.sort(() => Math.random() - 0.5);

    // Create an array of objects with the permutation and the penalty
    const permutationObjects = permutations.map((permutation) => {
        const penalty = 0;
        return {
            permutation,
            penalty,
        };
    });

    const whiteboard = grid.filter((cell) => cell.type === "whiteboard")[0];

    // IMPORTANT
    // Optimisation: Reduce the number of permutations

    permutationObjects.forEach((permutation) => {
        permutation.permutation.forEach((person) => {
            // Add penalty for each permutation depending on the constraints

            if (person.person.constraints.closeToWhiteboard) {
                const distance = getDistance(whiteboard, person.table);
                permutation.penalty += distance;
            }

            // Add the penalty for new constraints below
        });
    });

    // Sort the permutationObjects array by penalty, lowest to highest
    permutationObjects.sort((a, b) => a.penalty - b.penalty);

    // Log the five best permutations to the console using drawGridWithPeopleToConsole
    permutationObjects.slice(0, 5).forEach((permutationObject) => {
        drawGridWithPeopleToConsole(grid, permutationObject.permutation);
        console.log(`Penalty: ${permutationObject.penalty}`);
    });


};

const main = async () => {
    const people = JSON.parse(fs.readFileSync("constraints-experiments/list.json", "utf-8"));
    const grid = JSON.parse(fs.readFileSync("constraints-experiments/data.json", "utf-8"));
    // drawGridToConsole(grid, people);
    handleConstraints(grid, people);
};

main();
