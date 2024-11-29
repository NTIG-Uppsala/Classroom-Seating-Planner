const fs = require("fs");

const drawGridToConsole = (grid, people) => {
    // Grid:
    // {
    //      "centerX": 1,
    //      "centerY": 0,
    //      "type": "whiteboard"
    //  },

    // People:
    // {
    //      "name": "John Doe",
    //      "constraints": {
    //          "closeToWhiteboard": true, // or undefined
    //      }
    // },

    const maxX = Math.max(...grid.map((cell) => cell.centerX));
    const maxY = Math.max(...grid.map((cell) => cell.centerY));

    const filledGrid = Array.from({ length: maxY + 1 }, () => Array(maxX + 1).fill(" "));

    grid.forEach((cell) => {
        filledGrid[cell.centerY][cell.centerX] = cell.type === "whiteboard" ? "T" : "B";
    });

    console.log(filledGrid);
};

const getDistance = (cell1, cell2) => {
    const x = Math.abs(cell1.centerX - cell2.centerX);
    const y = Math.abs(cell1.centerY - cell2.centerY);
    return Math.sqrt(x ** 2 + y ** 2);
};

const handleConstraints = (grid, people) => {
    // Should be easily extensible to other constraints using a penalty system
    // Current constraints:
    // - closeToWhiteboard

    // All people with at least one value in the constraint object being a truthy value
    const peopleWithConstraints = people.filter((person) => Object.values(person.constraints).some(Boolean));

    console.log(peopleWithConstraints);
};

const main = async () => {
    console.clear();
    const people = JSON.parse(fs.readFileSync("constraints-experiments/list.json", "utf-8"));
    const grid = JSON.parse(fs.readFileSync("constraints-experiments/data.json", "utf-8"));
    drawGridToConsole(grid, people);
    handleConstraints(grid, people);
};

main();
