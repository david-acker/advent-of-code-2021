package main

import (
	"bufio"
	"container/list"
	"fmt"
	"log"
	"os"
	"sort"
	"strings"
)

func main() {
	file, err := os.Open("C:/Users/david/source/repos/advent-of-code-2021/input/day_10.txt")
	if err != nil {
		log.Fatal(err)
	}
	defer file.Close()

	scanner := bufio.NewScanner(file)
	scanner.Split(bufio.ScanLines)

	var lines []string
	for scanner.Scan() {
		line := strings.Trim(scanner.Text(), "\r\n")
		lines = append(lines, line)
	}

	// Part One
	partOneResult := 0
	for _, line := range lines {
		illegalCharacter := GetFirstIllegalCharacter(line)
		partOneResult += GetPointsForIllegalCharacter(illegalCharacter)
	}

	fmt.Println(partOneResult)

	// Part Two
	var closingSequenceScores []int
	for _, line := range lines {
		closingSequence := GetClosingSequence(line)
		if len(closingSequence) > 0 {
			closingSequenceScores = append(closingSequenceScores,
				GetScoreForClosingSequence(closingSequence))
		}
	}

	sort.Slice(closingSequenceScores, func(x, y int) bool {
		return closingSequenceScores[x] < closingSequenceScores[y]
	})

	partTwoResult := closingSequenceScores[len(closingSequenceScores)/2]
	fmt.Println(partTwoResult)
}

func GetFirstIllegalCharacter(line string) rune {
	closeToOpenMap := map[rune]rune{')': '(', ']': '[', '}': '{', '>': '<'}

	openingQueue := list.New()
	for _, char := range line {
		if matchingOpen, isClosing := closeToOpenMap[char]; isClosing {
			lastOpen := openingQueue.Front()
			openingQueue.Remove(lastOpen)

			if lastOpen.Value != matchingOpen {
				return char
			}
		} else {
			openingQueue.PushFront(char)
		}
	}

	return ' '
}

func GetClosingSequence(line string) []rune {
	closeToOpenMap := map[rune]rune{')': '(', ']': '[', '}': '{', '>': '<'}
	openToCloseMap := map[rune]rune{'(': ')', '[': ']', '{': '}', '<': '>'}

	openingQueue := list.New()
	for _, char := range line {
		if matchingOpen, isClosing := closeToOpenMap[char]; isClosing {
			lastOpen := openingQueue.Front()
			openingQueue.Remove(lastOpen)

			if lastOpen.Value != matchingOpen {
				// Skip invalid lines
				return make([]rune, 0)
			}
		} else {
			openingQueue.PushFront(char)
		}
	}

	var closingSequence []rune
	for char := openingQueue.Front(); char != nil; char = char.Next() {
		matchingClose := openToCloseMap[char.Value.(rune)]

		closingSequence = append(closingSequence, matchingClose)
	}

	return closingSequence
}

func GetScoreForClosingSequence(closingSequence []rune) int {

	totalScore := 0
	for _, char := range closingSequence {
		totalScore *= 5
		totalScore += GetPointsForClosingCharacter(char)
	}

	return totalScore
}

func GetPointsForIllegalCharacter(char rune) int {
	switch char {
	case ')':
		return 3
	case ']':
		return 57
	case '}':
		return 1197
	case '>':
		return 25137
	default:
		return 0
	}
}

func GetPointsForClosingCharacter(char rune) int {
	switch char {
	case ')':
		return 1
	case ']':
		return 2
	case '}':
		return 3
	case '>':
		return 4
	default:
		return 0
	}
}
