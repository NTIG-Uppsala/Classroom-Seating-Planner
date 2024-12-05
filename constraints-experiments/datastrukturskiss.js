layoutManager.students = [
    { name: 'Valeria Rossi', constraints: [ {type: 'CloseTo', target: 'Whiteboard', priority: 10} ] },
    { name: 'Chloé Dubois', constraints: [ {type: 'CloseTo', target: 'Maximilian Fischer', priority: 4}, {type: 'CloseTo', target: 'Whiteboard', priority: 10}] },
    { name: 'Maximilian Fischer', constraints: [ 'NearWhiteboard' ] },
    { name: 'Kai Nakamura', constraints: [ 'OtherConstraint' ] },
    { name: 'Dmitri Petrov', constraints: [ 'NearWhiteboard' ] },
    { name: 'Amina Abdi', constraints: [ 'NearWhiteboard' ] },
    { name: 'Léo Dupont', constraints: [ 'NearWhiteboard', 'OtherConstriant' ] },
    { name: 'Eero Virtanen', constraints: [ 'NearWhiteboard' ] },
    { name: 'Théodore Laurent', constraints: [ 'NearWhiteboard' ] },
    { name: 'Haruto Yamamoto', constraints: ['NearWhiteboard' ] },
    { name: 'Eleftherios Nikolaidis', constraints: [ 'NearWhiteboard' ] },
    { name: 'Yannis Papadopoulos', constraints: [ 'NearWhiteboard' ] },
]

// i ett scope
constraints = layoutManager.getContraints = () => {
    return students.map(student => {
        return student.constraints.map(constraint => {
            return {
                student: student.name,
                constraint: constraint.type,
                target: constraint.target,
                priority: constraint.priority,
            }
        }).flat()
    })
}