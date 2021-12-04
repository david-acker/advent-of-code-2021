package main

import (
	"bufio"
	"fmt"
	"log"
	"os"
	"strconv"
	"strings"
)

func main() {
	file, err := os.Open("C:/Users/david/source/repos/advent-of-code-2021/input/day_2.txt")
	if err != nil {
		log.Fatal(err)
	}
	defer file.Close()

	scanner := bufio.NewScanner(file)
	scanner.Split(bufio.ScanLines)

	var instructions []Instruction
	for scanner.Scan() {
		instructions = append(instructions,
			CreateInstruction(scanner.Text()))
	}

	// Part One
	position := GetFinalPosition(instructions)

	result := position.horizontal * position.vertical

	fmt.Println(result)

	// Part Two
	position = GetFinalPositionWithAim(instructions)

	result = position.horizontal * position.vertical

	fmt.Println(result)
}

type Position struct {
	horizontal int
	vertical   int
}

type Instruction struct {
	direction string
	units     int
}

const (
	Forward = "forward"
	Up      = "up"
	Down    = "down"
)

func CreateInstruction(input string) Instruction {
	result := strings.Split(input, " ")

	direction := result[0]
	units, err := strconv.Atoi(result[1])
	if err != nil {
		log.Fatal(err)
	}

	return Instruction{direction, units}
}

func GetFinalPosition(instructions []Instruction) Position {
	position := Position{0, 0}

	for _, instruction := range instructions {
		switch instruction.direction {
		case Forward:
			position.horizontal += instruction.units
		case Up:
			position.vertical -= instruction.units
		case Down:
			position.vertical += instruction.units
		default:
		}
	}

	return position
}

func GetFinalPositionWithAim(instructions []Instruction) Position {
	position := Position{0, 0}
	aim := 0

	for _, instruction := range instructions {
		switch instruction.direction {
		case Forward:
			position.horizontal += instruction.units
			position.vertical += (aim * instruction.units)
		case Up:
			aim -= instruction.units
		case Down:
			aim += instruction.units
		default:
		}
	}

	return position
}
