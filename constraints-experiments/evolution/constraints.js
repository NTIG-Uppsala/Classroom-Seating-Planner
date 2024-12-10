const getDistance = (cell1, cell2) => {
    const x = Math.abs(cell1.centerX - cell2.centerX);
    const y = Math.abs(cell1.centerY - cell2.centerY);
    return Math.sqrt(x ** 2 + y ** 2);
};

const closeToWhiteboard = (person, cells) => {
    if (!person.constraints.closeToWhiteboard) {
        return 0;
    }

    const whiteboard = cells.find((cell) => cell.type === "whiteboard");
    return getDistance(whiteboard, person.table) * (person.constraints.closeToWhiteboard.weight || 1);
};

const constraints = [closeToWhiteboard];

module.exports = {
    constraints,
};
