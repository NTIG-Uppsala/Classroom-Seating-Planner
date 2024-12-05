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
const generateInitialPlacement = (students, cells) => {
    let usedTables = new Set();
    let placement = [];

    // Randomly assign students to tables
    students.forEach((student) => {
        let table = cells[Math.floor(Math.random() * cells.length)];

        while (usedTables.has(table)) {
            table = cells[Math.floor(Math.random() * cells.length)];
        }

        usedTables.add(table);
        placement.push({
            name: student,
            table: table,
            constraints: constraints.getConstraints(student),
        });
    });

    return placement;
};

// Mutates the placement of people at tables
const mutate = (placement, cells, mutationRate) => {
    const tables = cells.filter((cell) => cell.type === "table");
    const usedTables = new Set(placement.map((person) => `${person.table.centerX},${person.table.centerY}`));

    // Randomly reassign people to tables with a certain probability based on the mutation rate
    return placement.map((person) => {
        if (Math.random() < mutationRate) {
            // Get a list of tables that are not currently being used
            const availableTables = tables.filter((table) => !usedTables.has(`${table.centerX},${table.centerY}`));

            // Move the person to a new table
            const assignedTable = availableTables[Math.floor(Math.random() * availableTables.length)];
            usedTables.delete(`${person.table.centerX},${person.table.centerY}`);
            usedTables.add(`${assignedTable.centerX},${assignedTable.centerY}`);

            return {
                ...person,
                table: assignedTable,
            };
        }

        return person;
    });
};

// Using a genetic algorithm to find the best seating arrangement in a reasonable amount of time
const getSeatingArrangement = (students, cells, populationSize, generations) => {
    // The chance that a person will be moved to a different table
    let mutationChance = 0.5;

    // Generate the initial population of seating arrangements and sort them by how good they are
    let seatingArrangementsPopulation = Array.from({ length: populationSize }, () => generateInitialPlacement(students, cells));
    seatingArrangementsPopulation = seatingArrangementsPopulation.sort((a, b) => calculatePenalty(a, cells) - calculatePenalty(b, cells));

    // Find the best placement and its penalty in the initial population
    let bestPlacement = seatingArrangementsPopulation[0];
    let bestPenalty = calculatePenalty(bestPlacement, cells);

    for (let generation = 0; generation < generations; generation++) {
        const newSeatingArrangementsPopulation = [];

        // Create a new population based on the best placement
        for (let index = 0; index < populationSize; index++) {
            newSeatingArrangementsPopulation.push(mutate(bestPlacement, cells, mutationChance));
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
