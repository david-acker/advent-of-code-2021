from __future__ import annotations
import ast
import itertools
import math

class Node:
    def __init__(self, value, parent = None):
        self.parent: Node | None = parent

        self.level: int = 0 if self.parent is None else self.parent.level + 1

        self.left: Node | None = None
        self.right: Node | None = None
        self.value: int | None = None
        
        if type(value) == list:
            self.left: Node | None = Node(value[0], self)
            self.right: Node | None = Node(value[-1], self)
        elif type(value) == int:
            self.value: int | None = value
        else:
            raise TypeError(f"Invalid value type: {type(value)}")

    def get_all_nodes(self) -> list[Node | list]:
        nodes = [self]
        if self.value is None:
            nodes += self.left.get_all_nodes()
            nodes += self.right.get_all_nodes()

        return nodes

    def to_list(self) -> list[int | list]:
        if self.value is None:
            return [self.left.to_list(), self.right.to_list()]

        return self.value

    @property
    def magnitude(self) -> int:
        if self.value is None:
            return (self.left.magnitude * 3) + (self.right.magnitude * 2)

        return self.value

    def farthest_right_node(self) -> Node:
        if self.value is None:
            return self.right.farthest_right_node()
        
        return self

    def closest_left_node(self) -> Node | None:
        if self.parent is None:
            return None
        
        if self == self.parent.left:
            return self.parent.closest_left_node()
        else:
            return self.parent.left.farthest_right_node()

    def farthest_left_node(self) -> Node:
        if self.value is None:
            return self.left.farthest_left_node()

        return self

    def closest_right_node(self) -> Node | None:
        if self.parent is None:
            return None
        
        if self == self.parent.right:
            return self.parent.closest_right_node()
        else:
            return self.parent.right.farthest_left_node()

    @property
    def has_left_value(self) -> bool:
        return (self.left is not None) and (self.left.value is not None)

    @property
    def has_right_value(self) -> bool:
        return (self.right is not None) and (self.right.value is not None)

    @property
    def can_explode(self) -> bool:
        if self.level != 4:
            return False

        return self.has_left_value and self.has_right_value

    def explode(self) -> None:
        closest_left_node = self.closest_left_node()
        if closest_left_node:
            closest_left_node.value += self.left.value

        closest_right_node = self.closest_right_node()
        if closest_right_node:
            closest_right_node.value += self.right.value

        self.left = None
        self.right = None
        self.value = 0

    @property
    def can_split(self) -> bool:
        return (self.value is not None) and (self.value >= 10)

    def split(self) -> None:
        self.left = Node(math.floor(self.value / 2), self)
        self.right = Node(math.ceil(self.value / 2), self)
        self.value = None

    def reduce(self) -> Node:
        operation_switch = True
        completed = False

        while True:
            completed = True
            
            if operation_switch:
                has_exploded = False
                for node in self.get_all_nodes():
                    if node.can_explode:
                        node.explode()

                        has_exploded = True
                        completed = False
                        break
                
                operation_switch = has_exploded
            
            if not operation_switch:
                has_split = False
                for node in self.get_all_nodes():
                    if node.can_split:
                        node.split()

                        has_split = True
                        completed = False
                        break
                
                operation_switch = has_split
        
            if completed:
                break

        return self

with open('C:/Users/david/source/repos/advent-of-code-2021/input/day_18.txt') as file:
    snailfish_numbers: list[str] = [sn for sn in file.readlines()]

# Part One
result_node = None
for sn in snailfish_numbers:
    if result_node is None:
        node = Node(ast.literal_eval(sn))
    else:
        node = Node([result_node.to_list(), ast.literal_eval(sn)])

    result_node = node.reduce()

print(f"Part One Result: {result_node.magnitude}")

# Part Two
largest_magnitude = 0
for first_number, second_number in list(itertools.permutations(snailfish_numbers, 2)):
    node = Node([ast.literal_eval(first_number), ast.literal_eval(second_number)])
    
    largest_magnitude = max(node.reduce().magnitude, largest_magnitude)

print(f"Part Two Result: {largest_magnitude}")