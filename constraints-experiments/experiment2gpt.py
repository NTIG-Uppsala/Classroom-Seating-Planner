import math
import random

# Initial Data
students = ["A", "B", "C", "D", "E", "F", "G", "H", "I", "J"]
tables = [{"table": str(i + 1), "occupant": " ", "x": i % 3, "y": i // 3} for i in range(9)]
constraints = [
    {"element": "A", "constraint": "closeTo", "target": "whiteboard", "importance": 10},
    {"element": "B", "constraint": "closeTo", "target": "window", "importance": 5},
    {"element": "C", "constraint": "closeTo", "target": "door", "importance": 5},
    {"element": "D", "constraint": "farFrom", "target": "E", "importance": 10},
    {"element": "E", "constraint": "closeTo", "target": "C", "importance": 5},
    {"element": "E", "constraint": "notNextTo", "target": "F", "importance": 10},
]

# Utilities
def calculate_distance(x1, y1, x2, y2):
    return math.sqrt((x1 - x2) ** 2 + (y1 - y2) ** 2)

def check_seating_score(element, table):
    x, y = table["x"], table["y"]
    score = 0

    for constraint in constraints:
        if constraint["element"] != element:
            continue
        
        target, importance = constraint["target"], constraint["importance"]

        if target == "whiteboard":
            distance = calculate_distance(x, y, 1, 0)
            max_distance = calculate_distance(1, 0, 2, 2)
            score += (max_distance - distance) / max_distance * importance
        elif target == "window":
            distance = calculate_distance(x, y, 2, 0)
            max_distance = calculate_distance(2, 0, 0, 2)
            score += (max_distance - distance) / max_distance * importance
        elif target == "door":
            distance = calculate_distance(x, y, 0, 2)
            max_distance = calculate_distance(0, 2, 2, 0)
            score += (max_distance - distance) / max_distance * importance
        elif target in students:
            target_table = next((t for t in tables if t["occupant"] == target), None)
            if not target_table:
                score += importance
            else:
                tx, ty = target_table["x"], target_table["y"]
                distance = calculate_distance(x, y, tx, ty)
                max_distance = calculate_distance(0, 0, 2, 2)
                if constraint["constraint"] == "farFrom":
                    score += distance / max_distance * importance
                elif constraint["constraint"] == "notNextTo" and distance > 1:
                    score += importance
    return score

# Core Logic
def generate_seating_arrangement():
    random.shuffle(students)
    for table in tables:
        table["occupant"] = " "
    
    total_score = 0
    for student in students:
        best_table = max(
            [t for t in tables if t["occupant"] == " "],
            key=lambda t: check_seating_score(student, t),
            default=None,
        )
        if best_table:
            score = check_seating_score(student, best_table)
            best_table["occupant"] = student
            total_score += score

    grid = [[table["occupant"] for table in tables if table["y"] == y] for y in range(3)]
    return total_score, grid

def run_simulations(iterations=1000):
    results = []
    for _ in range(iterations):
        score, grid = generate_seating_arrangement()
        results.append((score, grid))

    best_score, best_grid = max(results, key=lambda x: x[0])
    avg_score = sum(score for score, _ in results) / iterations

    print(f"Best Score: {best_score}")
    print("Best Grid:")
    for row in best_grid:
        print(row)
    print(f"Average Score: {avg_score:.2f}")

# Run Simulation
run_simulations()
