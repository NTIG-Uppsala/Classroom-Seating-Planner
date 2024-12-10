const fs = require("node:fs");

const interpretStudentConstraints = (name, rawConstraints) => {
    const functionLookupTable = {
        nära: { type: "distance", arguments: [name, "near", undefined], priority: 1 },
        intenära: { type: "distance", arguments: [name, "far", undefined], priority: 1 },
        långtfrån: { type: "distance", arguments: [name, "far", undefined], priority: 1 },
        bredvid: { type: "adjacent", arguments: [name, "yes", undefined], priority: 1 },
        intebredvid: { type: "adjacent", arguments: [name, "no", undefined], priority: 1 },
    };

    const recipientLookupTable = {
        tavlan: "whiteboardCover",
        tavla: "whiteboardCover",
        whiteboard: "whiteboardCover",
        whiteboards: "whiteboardCover",
        svartatavlan: "whiteboardCover",
        klösbrädan: "whiteboardCover",
        dörren: "door",
        dörr: "door",
        fönstret: "window",
        fönster: "window",
        vindöga: "window",
    };

    const interpretedConstraints = [];

    const constraintList = rawConstraints.split("/").map((constraint) => constraint.trim());

    constraintList.forEach((rawConstraint) => {
        // Find the function name from the functionLookupTable
        const trimmedConstraint = rawConstraint.replace(/\s/g, "").toLowerCase();
        const functionName = Object.keys(functionLookupTable)
            .filter((functionName) => trimmedConstraint.startsWith(functionName))
            .at(0);

        // Create the template constraint object
        const interpretedConstraint = functionLookupTable[functionName];

        if (!interpretedConstraint) return;

        // Isolate the recipient string
        let recipient = rawConstraint
            .replace(functionName, "")
            .replace(/\(.*\)/, "") // Remove priority (N)
            .trim();

        // Clean up the recipient string using lookup table
        if (recipientLookupTable[recipient.replace(/\s/g, "").toLowerCase()]) {
            recipient = recipientLookupTable[recipient];
        }
        interpretedConstraint.arguments[2] = recipient;

        // Find the priority of the constraint
        // Priority is the number inside the parenthesis (N)
        const priority = parseFloat(rawConstraint.match(/\(([^)]+)\)/)?.[1] || 1); // Default priority is 1
        interpretedConstraint.priority = priority;

        interpretedConstraints.push(interpretedConstraint);
    });

    return interpretedConstraints || null; // Return null if no constraints are assigned
};

const constraintFunctions = {
    distance: (source, target, nearOrFar, priority, references) => {
        if (!target) return 0;

        // Get all the possible distances between the target and every classroom element
        const allDistances = references.classroomElements
            .filter((cell) => cell !== target) // Remove self as a candidate
            .filter((cell) => cell.cellType === "table") // Only consider tables
            .map((cell) => {
                return Math.sqrt((cell.x - target.x) ** 2 + (cell.y - target.y) ** 2);
            });

        const maxDistance = Math.max(...allDistances);
        const minDistance = Math.min(...allDistances);

        const actualDistance = Math.sqrt((target.x - source.x) ** 2 + (target.y - source.y) ** 2);

        // Between 0 and 1
        const relativeDistance = (actualDistance - minDistance) / (maxDistance - minDistance);

        switch (nearOrFar) {
            case "near": {
                return (1 - relativeDistance) * priority;
            }
            case "far": {
                return relativeDistance * priority;
            }
            default: {
                return 0; // In case the arguments are somehow messed up
            }
        }
    },
    adjacent: (source, target, yesOrNo, priority, references) => {
        if (!target) return 0;

        // Check if source is adjacent to target
        const xDiff = Math.abs(source.x - target.x);
        const yDiff = Math.abs(source.y - target.y);
        const isAdjacent = (xDiff === 1 && yDiff === 0) || (xDiff === 0 && yDiff === 1);

        switch (yesOrNo) {
            case "yes" && isAdjacent: {
                return priority;
            }
            case "no" && !isAdjacent: {
                return priority;
            }
            default: {
                return 0;
            }
        }
    },
};

const getStudentListFromFile = () => {
    const students = fs
        .readFileSync("./scripts/constraints-algorithm-proof-of-concept/data/names.txt", "utf-8")
        .split("\n")
        .map((row) => row.trim())
        .filter(Boolean) // Remove empty lines
        .map((row) => {
            const [name, constraints] = row.split(":");
            const student = { name: name.trim() };

            if (constraints?.trim()) {
                student.constraints = interpretStudentConstraints(name.trim(), constraints);
            }

            return student;
        })
        .filter((student) => student.name); // Remove nameless students

    return students;
};

const getLayoutFromFile = () => {
    // A layout is a list of strings where each string represents a row in the classroom
    return fs.readFileSync("./scripts/constraints-algorithm-proof-of-concept/data/classroom.txt", "utf-8").split("\n");
};

const getWhiteboardCover = (classroomElements) => {
    const whiteboardCells = classroomElements.filter((element) => element.cellType === "whiteboard");

    // Sum up the coordinates of all whiteboard cells to find the average position
    const coordinatesSum = whiteboardCells.reduce(
        (coordinates, whiteboard) => {
            coordinates.x += whiteboard.x;
            coordinates.y += whiteboard.y;
            return coordinates;
        },
        { x: 0, y: 0 }
    );

    const whiteboardCover = {
        cellType: "whiteboardCover",
        x: coordinatesSum.x / whiteboardCells.length, // Average
        y: coordinatesSum.y / whiteboardCells.length, // Average
    };

    return whiteboardCover;
};

const getClassroomElementsFromLayout = (layout) => {
    const classroomElements = [];

    // Lookup table for the different cell types
    const cellTypes = {
        B: "table",
        T: "whiteboard",
        D: "door",
        F: "window",
    };

    // Parse layout character by character and add them to the classroomElements list
    layout.forEach((row, y) => {
        row.split("").forEach((cell, x) => {
            if (cellTypes[cell]) {
                classroomElements.push({ x, y, cellType: cellTypes[cell] });
            }
        });
    });

    // Add the whiteboard cover as a separate element to find more seamlessly later
    if (classroomElements.some((element) => element.cellType === "whiteboard")) {
        classroomElements.push(getWhiteboardCover(classroomElements));
    }

    return classroomElements;
};

const fancyDraw = (title, classroomElements, students, options = { drawLegend: true, drawStudentList: true, drawClassroom: true }) => {
    if (!options.drawClassroom && !options.drawStudentList && !options.drawLegend) {
        console.log("You disabled all drawing options you silly goose");
    }

    // Cool colors ░▒▓█
    const palette = {
        whiteboard: "T",
        window: "F",
        door: "D",
        tableOccupied: "a-z",
        tableEmpty: "▒",
        floor: "░",
    };

    const letterPool = ["a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "x", "y", "z", "å", "ä", "ö"];

    // Associate each student with a unique letter
    const studentLookup = {};
    students.forEach((student, index) => {
        studentLookup[student.name] = letterPool[index];
    });

    // Define all the classroom elements
    const tables = classroomElements.filter((element) => element.cellType === "table");
    const whiteboards = classroomElements.filter((element) => element.cellType === "whiteboard");
    const windows = classroomElements.filter((element) => element.cellType === "window");
    const doors = classroomElements.filter((element) => element.cellType === "door");

    // Get the bounds of the classroom and create a grid of that size
    const maxX = Math.max(...classroomElements.map((cells) => cells.x));
    const maxY = Math.max(...classroomElements.map((cells) => cells.y));
    const grid = Array.from({ length: maxY + 1 }, () => Array(maxX + 1).fill(palette.floor));

    // Populate grid with classroom elements
    whiteboards.forEach((whiteboard) => {
        grid[whiteboard.y][whiteboard.x] = palette.whiteboard;
    });
    doors.forEach((door) => {
        grid[door.y][door.x] = palette.door;
    });
    windows.forEach((window) => {
        grid[window.y][window.x] = palette.window;
    });
    // Tables are handled a bit differently
    tables.forEach((table) => {
        if (table.student) {
            // If occupied, use the student's letter
            grid[table.y][table.x] = studentLookup[table.student.name];
        } else {
            grid[table.y][table.x] = palette.tableEmpty;
        }
    });

    // Title
    if (title) {
        console.log(title);
    }
    console.log("------------------------------------");

    // Legend
    if (options.drawLegend) {
        console.log("Legend:");

        Object.keys(palette).forEach((key) => {
            console.log(`${key}: ${palette[key]}`);
        });

        console.log("");
    }

    if (options.drawStudentList) {
        console.log("Students: (▣: constrained), (▢: unconstrained)");
        tables
            .filter((table) => table.student)
            .sort((a, b) => {
                // Sort students by their letter

                const aLetter = studentLookup[a.student.name];
                const bLetter = studentLookup[b.student.name];

                return aLetter.localeCompare(bLetter);
            })
            .forEach((table) => {
                const letter = studentLookup[table.student.name];
                let constrained = "▢";
                let priority = null;

                if (table.student.constraints) {
                    constrained = "▣";
                    priority = table.student.constraints.reduce((sum, constraint) => sum + constraint.priority, 0);
                }

                console.log(`${letter} ${constrained}: ${table.student.name} ${priority ? "(" + priority + ")" : "(-)"}`);
            });

        console.log("");
    }

    if (options.drawClassroom) {
        // Draw entire grid
        grid.forEach((row) => {
            console.log(row.join(""));
        });
    }

    console.log("------------------------------------");

    console.log("");
};

const seatStudent = (student, classroomElements) => {
    const tables = classroomElements.filter((element) => element.cellType === "table");
    const availableTables = tables.filter((table) => !table.student);

    // If student has no constraints, place them randomly
    if (!student.constraints) {
        const randomTable = availableTables[Math.floor(Math.random() * availableTables.length)];

        randomTable.student = student;

        return 0;
    }

    // Wipe all tables scores since they are persistent
    availableTables.forEach((table) => {
        table.score = 0;
    });

    // Sort the tables by how well they meet the constraints
    const rankedTables = availableTables
        .map((table) => {
            let score = 0;

            // Try every constraint to get a students overall preference for a table
            student.constraints.forEach((constraint) => {
                const callConstraintFunction = (target) => {
                    // Call the relevant constraint function from the constraintFunctions object
                    // Arguments: source, target, constraint specific parameter, priority, references to things beyond the constraint's scope
                    return constraintFunctions[constraint.type](table, target, constraint.arguments[1], constraint.priority, { classroomElements });
                };

                // These are taken from the interpreted constraint and are strings
                const caller = constraint.arguments[0];
                const recipient = constraint.arguments[2];

                // If more classroom elements are added, add them here as well (TODO - ADD TO DOCUMENTATION WHEN IMPLEMENTING IN C#)
                const classroomElementsNames = ["whiteboardCover", "door", "window"];

                // If recipient is a classroom element, set it as the target
                if (classroomElementsNames.includes(recipient)) {
                    const target = classroomElements
                        .filter((element) => {
                            return element.cellType === recipient;
                        })
                        .at(0);

                    // Call the relevant constraint function
                    score += callConstraintFunction(target);
                }

                // Else, the target must be student

                // Depending on if this student is the caller or recipient in the constraint,
                //  set the target of the constraint function to the other student's table
                else if (student.name === caller) {
                    // Try to find the table of the targeted student, they might not be seated yet
                    const targetStudentTable = tables
                        .filter((table) => {
                            return table.student && table.student.name === recipient;
                        })
                        .at(0);

                    // If the student has been placed, set their table as the target. Otherwise set it to null
                    const target = targetStudentTable || null;

                    score += callConstraintFunction(target);
                } else if (student.name === recipient) {
                    // Try to find the table of the targeted student, they might not be seated yet
                    const targetStudentTable = tables
                        .filter((table) => {
                            return table.student && table.student.name === caller;
                        })
                        .at(0);

                    // If the student has been placed, set their table as the target. Otherwise set it to null
                    const target = targetStudentTable || null;

                    score += callConstraintFunction(target);
                }
            });

            table.score = score;

            return table;
        })
        .sort((a, b) => b.score - a.score);

    // Take the best scored tables. The amount is based on the student's constraints' summed priorities
    const prioritySum = student.constraints.reduce((sum, constraint) => sum + constraint.priority, 0);
    const bestTables = rankedTables.slice(0, Math.ceil(rankedTables.length * ((0.85 ** prioritySum / 0.85) * 0.3)));

    // Pick a random table from the best tables and place the student there
    const randomTable = bestTables[Math.floor(Math.random() * bestTables.length)];
    randomTable.student = student;

    // Main wants to know the score of the table
    return randomTable.score;
};

const main = (students, classroomElements) => {
    const getAllConstraints = (students) => {
        return students
            .filter((students) => students.constraints)
            .map((student) => student.constraints)
            .flat()
            .filter(Boolean);
    };

    const sortStudentsByPriority = (students) => {
        students.sort((a, b) => {
            if (!a.constraints) return 1;
            if (!b.constraints) return -1;

            // Compare the students by the sum of their constraints' priorities
            const aPriority = a.constraints.reduce((sum, constraint) => sum + constraint.priority, 0);
            const bPriority = b.constraints.reduce((sum, constraint) => sum + constraint.priority, 0);

            return bPriority - aPriority;
        });
    };

    const nullifyAllStudentConstraints = (students) => {
        students = students.map((student) => {
            student.constraints = null;
            return student;
        });
    };

    // Takes a list of all the constraints and give every student a copy the each constraint involving them
    const assignAllConstraints = (constraints, students) => {
        constraints.forEach((constraint) => {
            students = students.map((student) => {
                // Check if student is involved in the constraint, either as the caller or the recipient
                if (constraint.arguments[0] === student.name || constraint.arguments[2] === student.name) {
                    if (!student.constraints) {
                        student.constraints = [];
                    }
                    student.constraints.push(constraint);
                }

                return student;
            });
        });
    };

    // Get all constraints as a list
    const constraints = getAllConstraints(students);

    // Make sure all constraints where a student is involved are assigned to the student
    nullifyAllStudentConstraints(students); // Reset all constraints so that we can reassign them
    assignAllConstraints(constraints, students);

    // Sort students to seat the pickiest students first
    sortStudentsByPriority(students);

    let seatingArrangementScore = 0;

    // Seat every student one at a time
    students.forEach((student) => {
        seatingArrangementScore += seatStudent(student, classroomElements);
    });

    return seatingArrangementScore;
};

// Run the main function with the students and classroom elements
const runIterations = (iterationsCount, options) => {
    const startTime = Date.now();

    const layouts = [];
    const scoresList = [];
    const iterations = iterationsCount;

    for (let i = 0; i < iterations; i++) {
        const layout = getLayoutFromFile();
        const classroomElements = getClassroomElementsFromLayout(layout);
        const students = getStudentListFromFile();

        const score = main(students, classroomElements);

        layouts.push({ score, classroomElements });
        scoresList.push(score);
    }

    const timeTaken = Date.now() - startTime;

    // Fancy draw after the iterations are done since it takes considerable time
    if (options.fancyDraw.best) {
        const bestLayout = layouts.sort((a, b) => b.score - a.score)[0];
        fancyDraw("Best Layout", bestLayout.classroomElements, getStudentListFromFile(), options.fancyDraw.options); // Best
    }
    if (options.fancyDraw.worst) {
        const worstLayout = layouts.sort((a, b) => a.score - b.score)[0];
        fancyDraw("Worst Layout", worstLayout.classroomElements, getStudentListFromFile(), options.fancyDraw.options); // Worst
    }
    if (options.fancyDraw.median) {
        const medianLayout = layouts.sort((a, b) => a.score - b.score)[Math.floor(layouts.length / 2)];
        fancyDraw("Median Layout", medianLayout.classroomElements, getStudentListFromFile(), options.fancyDraw.options); // Median
    }
    if (options.fancyDraw.random) {
        const randomLayout = layouts[Math.floor(Math.random() * layouts.length)];
        fancyDraw("Random Layout", randomLayout.classroomElements, getStudentListFromFile(), options.fancyDraw.options); // Random
    }
    if (options.fancyDraw.lastN) {
        const lastNLayout = layouts.slice(layouts.length - options.fancyDraw.lastN);
        lastNLayout.forEach((layout, i) => {
            fancyDraw(`${options.fancyDraw.lastN - i} to Last Layout`, layout.classroomElements, getStudentListFromFile(), options.fancyDraw.options); // Last N
        });
    }

    if (options.logStats) {
        console.log("Stats:");
        console.log("Iterations:", iterations);
        console.log("Avg score:", parseFloat((scoresList.reduce((sum, curr) => (sum += curr), 0) / iterations).toFixed(2)));
        console.log("Median score:", parseFloat(scoresList.sort((a, b) => a - b)[Math.floor(iterations / 2)].toFixed(2)));
        console.log("Max score:", parseFloat(Math.max(...scoresList).toFixed(2)));
        console.log("Min score:", parseFloat(Math.min(...scoresList).toFixed(2)));
        console.log("Time taken:", timeTaken, "ms");
        console.log("");
        console.log("------------------------------------");
        console.log("");
    }
};

const options = {
    fancyDraw: {
        best: false,
        median: !false,
        random: false,
        worst: false,
        lastN: 0,
        options: {
            drawLegend: false,
            drawStudentList: !false,
            drawClassroom: true,
        },
    },
    logStats: true,
};

console.log("------------------------------------");
console.log("");

const globalStartTime = Date.now();

runIterations(1, options);
runIterations(10, options);
runIterations(100, options);
runIterations(1000, options);
runIterations(10000, options);

console.log("Total time taken:", Date.now() - globalStartTime, "ms");
console.log("");
