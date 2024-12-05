const fs = require("fs");

// Returns the euclidean distance between two cells
const getDistance = (cell1, cell2) => {
    const x = Math.abs(cell1.centerX - cell2.centerX);
    const y = Math.abs(cell1.centerY - cell2.centerY);
    return Math.sqrt(x ** 2 + y ** 2);
};

// Adds the table to a set of tables that someone is sitting at
const markTableAsUsed = (table, usedTables) => {
    const tableKey = `${table.centerX},${table.centerY}`;
    usedTables.add(tableKey);
};

// Returns a boolean indicating whether someone is already sitting at the table
const isTableAvailable = (table, usedTables) => {
    const tableKey = `${table.centerX},${table.centerY}`;
    const isAvailable = !usedTables.has(tableKey);
    return isAvailable;
};

// Calculates the penalty for a placement
const calculatePenalty = (placement, cells) => {
    const whiteboard = cells.find((cell) => cell.type === "whiteboard");
    let penalty = 0;

    // Check that the constraint object is not empty
    placement
        .filter((person) => person.constraints)
        .forEach((person) => {
            const table = cells.find((cell) => cell.centerX === person.table.centerX && cell.centerY === person.table.centerY);
            if (person.constraints.closeToWhiteboard) {
                penalty += getDistance(whiteboard, table);
            }

            // Constraints that are not going to be used in the final product below

            if (person.constraints.canNotSitNextTo) {
                placement
                    .filter((otherPerson) => otherPerson.name === person.constraints.canNotSitNextTo)
                    .forEach((otherPerson) => {
                        const otherPersonsTable = cells.find((cell) => cell.centerX === otherPerson.table.centerX && cell.centerY === otherPerson.table.centerY);

                        penalty += 5 / getDistance(table, otherPersonsTable);
                    });
            }

            if (person.constraints.mustSitNextTo) {
                placement
                    .filter((otherPerson) => otherPerson.name === person.constraints.mustSitNextTo)
                    .forEach((otherPerson) => {
                        const otherPersonsTable = cells.find((cell) => cell.centerX === otherPerson.table.centerX && cell.centerY === otherPerson.table.centerY);

                        penalty += 10 * getDistance(table, otherPersonsTable);
                    });
            }

            if (person.constraints.mustSitAt) {
                const mustSitAtTable = cells.find((cell) => cell.centerX === person.constraints.mustSitAt.x && cell.centerY === person.constraints.mustSitAt.y);

                if (table.centerX !== mustSitAtTable.centerX || table.centerY !== mustSitAtTable.centerY) {
                    penalty += 100;
                }
            }

            // Add additional constraints here
        });

    return penalty;
};

// Creates an initial random placement of people at tables
const initializePlacement = (people, cells) => {
    const tables = cells.filter((cell) => cell.type === "table");
    const usedTables = new Set();

    return people.map((person) => {
        const availableTables = tables.filter((table) => !usedTables.has(`${table.centerX},${table.centerY}`));

        if (availableTables.length === 0) {
            throw new Error("No available tables during initialization.");
        }

        const assignedTable = availableTables[Math.floor(Math.random() * availableTables.length)];
        usedTables.add(`${assignedTable.centerX},${assignedTable.centerY}`);

        return {
            ...person,
            table: assignedTable,
        };
    });
};

// Randomly reassigns people to tables with a certain probability
const mutate = (placement, cells, mutationRate) => {
    const tables = cells.filter((cell) => cell.type === "table");
    const usedTables = new Set(placement.map((person) => `${person.table.centerX},${person.table.centerY}`));

    return placement.map((person) => {
        if (Math.random() < mutationRate) {
            const currentTableKey = `${person.table.centerX},${person.table.centerY}`;
            usedTables.delete(currentTableKey); // Free current table

            const availableTables = tables.filter((table) => isTableAvailable(table, usedTables));

            if (availableTables.length === 0) {
                throw new Error("No available tables during mutation.");
            }

            const newTable = availableTables[Math.floor(Math.random() * availableTables.length)];
            markTableAsUsed(newTable, usedTables);

            return {
                ...person,
                table: newTable,
            };
        }
        return person;
    });
};

// Mutate the currently best placement a number of times and return the best one
const runGeneticAlgorithm = (people, cells, populationSize, generations) => {
    let mutationRate = 0.5;

    // Initialize the population
    let population = Array.from({ length: populationSize }, () => initializePlacement(people, cells));

    // Evaluate fitness for each placement
    const evaluateFitness = (placement) => calculatePenalty(placement, cells);
    let bestPlacement = population[0];
    let bestFitness = evaluateFitness(bestPlacement);

    for (let generation = 0; generation < generations; generation++) {
        // Sort population by fitness
        population = population.sort((a, b) => evaluateFitness(a) - evaluateFitness(b));

        // Keep the best placement
        if (evaluateFitness(population[0]) < bestFitness) {
            bestPlacement = population[0];
            bestFitness = evaluateFitness(bestPlacement);
        }

        // Create next generation
        const newPopulation = [];
        for (let i = 0; i < populationSize; i++) {
            // Clone and mutate
            const mutatedPlacement = mutate(population[i % population.length], cells, mutationRate);
            newPopulation.push(mutatedPlacement);
        }
        population = newPopulation;

        mutationRate *= 0.995;
    }

    return { placement: bestPlacement, penalty: bestFitness };
};

// Draws the result to the console
const drawResultToConsole = (result, cells, whiteboards) => {
    cells = cells.filter((cell) => cell.type === "table");

    whiteboards.forEach((whiteboard) => {
        cells.push(whiteboard);
    });

    fs.writeFileSync("constraints-experiments/result.json", JSON.stringify(result, null, 2));

    const maxX = Math.max(...cells.map((cell) => cell.centerX));
    const maxY = Math.max(...cells.map((cell) => cell.centerY));

    const filledGrid = Array.from({ length: maxY + 1 }, () => Array(maxX + 1).fill(" "));

    cells.forEach((cell) => {
        filledGrid[cell.centerY][cell.centerX] = cell.type === "whiteboard" ? "T" : "_";
    });

    const tableCounts = {};
    result.placement.forEach((person) => {
        const key = `${person.table.centerX},${person.table.centerY}`;
        tableCounts[key] = tableCounts[key] ? tableCounts[key] + 1 : 1;
    });

    for (const key in tableCounts) {
        if (tableCounts[key] > 1) {
            console.error("Two people are assigned to the same table!");
            return;
        }
    }

    result.placement.forEach((person) => {
        filledGrid[person.table.centerY][person.table.centerX] = person.name[0];
    });

    console.log("Penalty:", result.penalty);

    console.log("Result:\n");

    filledGrid.forEach((row) => {
        console.log(row.join(""));
    });
};

// Turns the layout string into an array of cells and an array of whiteboards
const format = (layout) => {
    const tables = [];
    const whiteboards = [];

    layout.split("\n").forEach((row, centerY) => {
        row.split("").forEach((cell, centerX) => {
            if (cell === "B") {
                tables.push({ centerX, centerY, type: "table" });
                return;
            }

            if (cell === "T") {
                whiteboards.push({ centerX, centerY, type: "whiteboard" });
                return;
            }
        });
    });

    const whiteboardPosAvg = whiteboards.reduce(
        (accumulator, current) => {
            accumulator.centerX += current.centerX;
            accumulator.centerY += current.centerY;
            return accumulator;
        },
        { centerX: 0, centerY: 0 }
    );

    whiteboardPosAvg.centerX /= whiteboards.length;
    whiteboardPosAvg.centerY /= whiteboards.length;

    const cells = [...tables, { ...whiteboardPosAvg, type: "whiteboard" }];

    return { cells, whiteboards };
};

const layout = fs.readFileSync("constraints-experiments/classroom.txt", "utf-8");

const { cells, whiteboards } = format(layout);

const people = JSON.parse(fs.readFileSync("constraints-experiments/list.json", "utf8"));

const populationSize = 500;
const generations = 500;

const result = runGeneticAlgorithm(people, cells, populationSize, generations);
drawResultToConsole(result, cells, whiteboards);
