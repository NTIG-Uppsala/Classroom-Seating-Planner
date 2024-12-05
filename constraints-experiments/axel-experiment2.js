const fs = require("fs");

const getDistance = (cell1, cell2) => {
    const x = Math.abs(cell1.centerX - cell2.centerX);
    const y = Math.abs(cell1.centerY - cell2.centerY);
    return Math.sqrt(x ** 2 + y ** 2);
};

const markTableAsUsed = (table, usedTables) => {
    const tableKey = `${table.centerX},${table.centerY}`;
    usedTables.add(tableKey);
};

const isTableAvailable = (table, usedTables) => {
    const tableKey = `${table.centerX},${table.centerY}`;
    const isAvailable = !usedTables.has(tableKey);
    return isAvailable;
};

const calculatePenalty = (placement, cells) => {
    const whiteboard = cells.find((cell) => cell.type === "whiteboard");
    let penalty = 0;

    placement.forEach((person) => {
        const table = cells.find((cell) => cell.centerX === person.table.centerX && cell.centerY === person.table.centerY);
        if (person.constraints.closeToWhiteboard) {
            penalty += getDistance(whiteboard, table);
        }

        if (person.constraints.canNotSitNextTo) {
            placement.forEach((otherPerson) => {
                if (otherPerson.name === person.name) {
                    return;
                }

                const otherPersonsTable = cells.find(
                    (cell) => cell.centerX === otherPerson.table.centerX && cell.centerY === otherPerson.table.centerY
                );

                penalty += 5 / getDistance(table, otherPersonsTable);
            });
        }

        // Add additional constraints here
    });

    return penalty;
};

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

const runGeneticAlgorithm = (people, cells, populationSize, generations, mutationRate) => {
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
    }

    return { placement: bestPlacement, penalty: bestFitness };
};

const drawResultToConsole = (result, cells, whiteboards) => {
    // Remove the average whiteboard position from the cells
    cells = cells.filter((cell) => cell.type === "table");

    // Add all the whiteboards to the cells

    whiteboards.forEach((whiteboard) => {
        cells.push(whiteboard);
    });

    fs.writeFileSync("constraints-experiments/result.json", JSON.stringify(result, null, 2));

    console.log(result);
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

    filledGrid.forEach((row) => {
        console.log(row.join(""));
    });
};

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

    // Turn the whiteboards into a single cell with the average position
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

    // Correct the return to return both values as an object or array
    return { cells, whiteboards };
};

const layout = fs.readFileSync("constraints-experiments/classroom.txt", "utf-8");

// Destructure the returned object to get cells and whiteboards
const { cells, whiteboards } = format(layout);

// console.log(whiteboards);

const people = JSON.parse(fs.readFileSync("constraints-experiments/list.json", "utf8"));

const populationSize = 100;
const generations = 500;
const mutationRate = 0.25;

const result = runGeneticAlgorithm(people, cells, populationSize, generations, mutationRate);
drawResultToConsole(result, cells, whiteboards);
