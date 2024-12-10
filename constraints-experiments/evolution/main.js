const { constraints } = require("./constraints.js");

// Calculate how good a placement is based on the constraints
const calculatePenalty = (placement, cells) => {
    let penalty = 0;

    placement
        .filter((person) => person.constraints)
        .forEach((person) => {
            constraints.forEach((constraint) => {
                penalty += constraint(person, cells);
            });
        });
    return penalty;
};

// Generates an initial random placement of people at tables
const generateInitialPlacement = (students, tables) => {
    let usedTables = new Set();
    let placement = [];

    // Randomly assign students to tables
    students.forEach((student) => {
        let table = tables[Math.floor(Math.random() * tables.length)];

        while (usedTables.has(table)) {
            table = tables[Math.floor(Math.random() * tables.length)];
        }

        usedTables.add(table);
        placement.push({
            name: student,
            table: table,
            constraints: student.constraints,
        });
    });

    return placement;
};

const mutate = (placement, mutationRate, cells) => {
    // Clone the placement to avoid modifying the original directly
    const newPlacement = [...placement.map((person) => ({ ...person }))];

    // Find the whiteboard in the cells array (if present)
    const whiteboard = cells.find((cell) => cell.type === "whiteboard");
    const whiteboardIndex = newPlacement.findIndex((person) => person.table === whiteboard?.table);

    for (let i = 0; i < newPlacement.length; i++) {
        // Decide whether to mutate this person's placement
        if (Math.random() < mutationRate) {
            // Pick a random person to swap tables with
            const swapIndex = Math.floor(Math.random() * newPlacement.length);
            const person = newPlacement[i];
            const swapPerson = newPlacement[swapIndex];

            // Skip mutation if either person is at the whiteboard's table
            if (person.table === whiteboard?.table || swapPerson.table === whiteboard?.table) {
                continue; // Skip the mutation and proceed to the next iteration
            }

            // Perform the swap
            [person.table, swapPerson.table] = [swapPerson.table, person.table];
        }
    }

    return newPlacement;
};

// Using a genetic algorithm to find the best seating arrangement in a reasonable amount of time
const getSeatingArrangement = (students, cells, populationSize, generations) => {
    // Get all the tables in the room
    const tables = cells.filter((cell) => cell.type === "table");

    // The chance that a person will be moved to a different table
    let mutationChance = 0.5;

    // Generate the initial population of seating arrangements and sort them by how good they are
    let seatingArrangementsPopulation = Array.from({ length: populationSize }, () => generateInitialPlacement(students, tables));
    seatingArrangementsPopulation = seatingArrangementsPopulation.sort((a, b) => calculatePenalty(a, cells) - calculatePenalty(b, cells));

    // Find the best placement and its penalty in the initial population
    let bestPlacement = seatingArrangementsPopulation[0];
    let bestPenalty = calculatePenalty(bestPlacement, cells);

    for (let generation = 0; generation < generations; generation++) {
        const newSeatingArrangementsPopulation = [];

        // Create a new population based on the best placement
        for (let index = 0; index < populationSize; index++) {
            newSeatingArrangementsPopulation.push(mutate(bestPlacement, mutationChance, cells));
        }

        // Sorts the new population by how good the placement is
        seatingArrangementsPopulation = newSeatingArrangementsPopulation.sort((a, b) => calculatePenalty(a, cells) - calculatePenalty(b, cells));

        // Check if the new generation has a better placement
        let currentPenalty = calculatePenalty(seatingArrangementsPopulation[0], cells);
        if (currentPenalty < bestPenalty) {
            bestPlacement = seatingArrangementsPopulation[0];
            bestPenalty = currentPenalty;
        }

        // Decrease the mutation chance per generation. This allows the algorithm to find a rough solution quickly and then fine-tune it.
        mutationChance *= 0.995;
    }

    return bestPlacement;
};

module.exports = {
    getSeatingArrangement,
    calculatePenalty,
};
