using System.Windows.Controls;

namespace Classroom_Seating_Planner.Src
{
    internal class SeatingHandler
    {
        public static double SeatStudent(ConstraintsHandler.Student student, List<Cells.Cell> classroomElements)
        {
            List<Cells.TableCell> tables = classroomElements.OfType<Cells.TableCell>().ToList();
            List<Cells.TableCell> availableTables = tables.Where(table => table.student == null).ToList();

            // Silent error when there aren't enough tables - the program will warn the user when the file is read
            if (availableTables.Count.Equals(0)) return 0;

            Random random = new();
            Cells.TableCell randomTable = availableTables[random.Next(availableTables.Count)];

            // Randomly seat students if they don't have any constraints
            if (student.constraints == null)
            {
                randomTable.student = student;

                return 0;
            }

            // Wipe all tables scores since they are persistent
            availableTables.ForEach(table => table.score = 0);

            // Sirt the tables by how well they meet the constraints
            List<Cells.TableCell> rankedTables = availableTables
                .Select(table =>
                {
                    double score = 0;

                    //Try every constraint to get a students overall preference for a table
                    student.constraints.ForEach(constraint =>
                    {
                        double callConstraintFunction(Cells.Cell? target)
                        {
                            // Get function reference depending on the constraint type
                            Func<Cells.Cell, Cells.Cell, string, int, Dictionary<string, object>, double> function = ConstraintFunctions.functions[constraint.type];

                            // Define reference scopes that the constraint functions may need
                            Dictionary<string, object> references = new() { { "classroomElements", classroomElements } };

                            // Arguments: source, target, constraint specific parameter, priority, references to things beyond the constraint's scope
                            return function(table, target, constraint.arguments[1], constraint.priority, references);
                        }

                        // These are taken from the interpreted constraint and are strings
                        string caller = constraint.arguments[0];
                        string? recipient = constraint.arguments[2];

                        // If more classroom elements are added, add them here as well (TODO - ADD TO DOCUMENTATION WHEN IMPLEMENTING IN C#)
                        List<string> classroomElementNames = ["whiteboardCover"];

                        // If recipient is a classroom element, set it as the target
                        if (classroomElementNames.Contains(caller))
                        {
                            Cells.Cell? target = classroomElements.Where(element =>
                            {
                                return element.cellType.Equals(recipient);
                            }).FirstOrDefault();

                            score += callConstraintFunction(target);
                        }

                        // Else, the target must be student

                        // Depending on if this student is the caller or recipient in the constraint,
                        //  set the target of the constraint function to the other student's table
                        else if (student.name.Equals(caller))
                        {
                            // Try to find the table of the targeted student by looking for their name amongst the tables
                            Cells.TableCell? targetStudentTable = tables.Where(targetTable =>
                            {
                                if (targetTable.student == null) return false;
                                return (bool)targetTable.student?.name.Equals(recipient);// If a student is defined, we know that the student has a name
                            }).FirstOrDefault();

                            Cells.TableCell? target = targetStudentTable ?? null; // Target may not be seated yet

                            score += callConstraintFunction(target);
                        }
                        else if (student.name.Equals(recipient))
                        {
                            // Try to find the table of the targeted student by looking for their name among the tables
                            Cells.TableCell? targetStudentTable = tables.Where(targetTable =>
                            {
                                if (targetTable.student == null) return false;
                                return (bool)targetTable.student?.name.Equals(caller); // If a student is defined, we know that the student has a name
                            }).FirstOrDefault();

                            Cells.TableCell? target = targetStudentTable ?? null; // Target may not be seated yet

                            score += callConstraintFunction(target);
                        }
                    });

                    table.score = score;

                    return table;
                }).OrderByDescending(table => table.score).ToList();

            // Take the best scored tables. The amount is based on the student's constraints' summed priorities
            int prioritySum = student.constraints.Sum(constraint => constraint.priority);
            List<Cells.TableCell> bestTables = rankedTables.Take((int)Math.Ceiling(rankedTables.Count * (Math.Pow(0.85, prioritySum - 1) * 0.3))).ToList();

            // Pick a random table from the best tables and place the student there
            randomTable = bestTables[random.Next(bestTables.Count)];
            randomTable.student = student;

            // Main wants to know the score of the table
            return randomTable.score;
        }

        // TODO - either remove score (as return of seatstudent aswell) or implement multiple runs of this function
        public static double GenerateSeatingArrangement(List<ConstraintsHandler.Student> students, List<Cells.Cell> classroomElements)
        {
            static List<ConstraintsHandler.Constraint> getAllConstraints(List<ConstraintsHandler.Student> students)
            {
                return students
                    .Where(students => students.constraints != null)
                    .SelectMany(student => student.constraints) // It's fine. This can't be null at this point
                    .ToList();
            }

            static void sortStudentsByPriority(List<ConstraintsHandler.Student> students)
            {
                students.Sort((a, b) =>
                {
                    if (a.constraints == null) return -1;
                    if (b.constraints == null) return 1;

                    // Compare the students by the sum of their constraints' priorities
                    int aPriority = a.constraints.Select(constraint => constraint.priority).Sum();
                    int bPriority = b.constraints.Select(constraint => constraint.priority).Sum();

                    return bPriority - aPriority;
                });
            }

            static void nullifyAllStudentConstraints(List<ConstraintsHandler.Student> students)
            {
                students = students
                    .Where(student => student.constraints != null)
                    .Select(student =>
                    {
                        student.constraints = null;
                        return student;
                    }).ToList();
            }

            // Takes a list of all the constraints and give every student a copy the each constraint involving them
            static void assignAllConstraints(List<ConstraintsHandler.Student> students, List<ConstraintsHandler.Constraint> constraints)
            {
                constraints.ForEach(constraint =>
                {
                    students = students.Select(student =>
                    {
                        // Check if student is involved in the constraint, either as the caller or the recipient
                        if (constraint.arguments[2] == null) return student;

                        if (constraint.arguments[0].Equals(student.name) || constraint.arguments[2].Equals(student.name))
                        {
                            // Create an empty list for constraints if it does not already exist
                            student.constraints ??= [];

                            student.constraints.Add(constraint);
                        }

                        return student;
                    }).ToList();
                });
            }

            // Get all constraints as a list
            List<ConstraintsHandler.Constraint> constraints = getAllConstraints(students);

            // Make sure all constraints where a student is involved are assigned to the student
            nullifyAllStudentConstraints(students); // Reset all constraints so that we can reassign them
            assignAllConstraints(students, constraints);

            // Sort students to seat the pickiest students first
            sortStudentsByPriority(students);

            double seatingArrangementScore = 0;

            // Seat every student one at a time
            students.ForEach((student) =>
            {
                seatingArrangementScore += SeatStudent(student, classroomElements);
            });

            return seatingArrangementScore;
        }

        public static void Populate(List<TextBlock> tableElements, List<string> classList)
        {
            // Ensure we don't exceed the number of available tables
            int numberOfStudentsToBePlaced = Math.Min(classList.Count, tableElements.Count);

            // Update the tables with the new order
            for (int index = 0; index < numberOfStudentsToBePlaced; index++)
            {
                // Assign the student name to the corresponding table
                tableElements[index].Text = classList[index];
            }

            // If there are more tables than students, clear the remaining tables
            for (int index = numberOfStudentsToBePlaced; index < tableElements.Count; index++)
            {
                // Clear the table if it's not occupied
                tableElements[index].Text = string.Empty;
            }
        }
    }
}
