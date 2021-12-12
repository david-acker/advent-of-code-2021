import { readFileSync } from 'fs';

class Octopus {
    public x: number;
    public y: number;

    public energyLevel;

    public hasFlashed: boolean = false;

    constructor(x: number, y: number, startingEnergyLevel: number) {
        this.x = x;
        this.y = y;

        this.energyLevel = startingEnergyLevel;
    }
}

function getSurroundingOctopi(octopus: Octopus, octopiGrid: Octopus[][]) {
    const gridHeight: number = octopiGrid.length;
    const gridWidth: number = octopiGrid[0].length;

    const surroundingOctopi: Octopus[] = [];

    for (let xShift = -1; xShift <= 1; xShift++) {
        for (let yShift = -1; yShift <= 1; yShift++) {

            if (xShift == 0 && yShift == 0) {
                continue;
            }

            const newX: number = octopus.x + xShift;
            const newY: number = octopus.y + yShift;

            if ((newX >= 0 && newX < gridHeight) && (newY >= 0 && newY < gridWidth)) {
                surroundingOctopi.push(octopiGrid[newX][newY])
            }
        }
    }

    return surroundingOctopi;
}

function increaseEnergyLevel(octopus: Octopus, octopiGrid: Octopus[][]) {
    octopus.energyLevel += 1;

    if (octopus.energyLevel > 9 && !octopus.hasFlashed) {
        octopus.hasFlashed = true;

        getSurroundingOctopi(octopus, octopiGrid)
            .forEach(octopus => increaseEnergyLevel(octopus, octopiGrid));
    }
}

function moveToNextStep(octopi: Octopus[], octopiGrid: Octopus[][]): void {
    octopi.forEach(octopus => {
        if (octopus.hasFlashed == true) {
            octopus.hasFlashed = false;
            octopus.energyLevel = 0;
        }
    });

    octopi.forEach(octopus => increaseEnergyLevel(octopus, octopiGrid));
}

function getStartingOctopi(): [Octopus[], Octopus[][]] {
    const inputData: string[] = readFileSync('C:/Users/david/source/repos/advent-of-code-2021/input/day_11.txt', 'utf-8').split('\r\n');

    const octopi: Octopus[] = [];
    const octopiGrid: Octopus[][] = [];

    for (let x = 0; x < inputData.length; x++) {

        octopiGrid[x] = [];
        for (let y = 0; y < inputData[0].length; y++) {
            const octopus = new Octopus(x, y, Number(inputData[x][y]));

            octopi.push(octopus);
            octopiGrid[x][y] = octopus;
        }
    }

    return [octopi, octopiGrid];
}


// Part One
let [ octopi, octopiGrid ] = getStartingOctopi();

let partOneResult: number = 0;
for (let i = 0; i < 100; i++) {
    moveToNextStep(octopi, octopiGrid);
    partOneResult += octopi.filter(octopus => octopus.hasFlashed).length;
}

console.log(partOneResult);

// Part Two
[ octopi, octopiGrid ] = getStartingOctopi();

let step: number = 0;
while (true) {
    step += 1;
    moveToNextStep(octopi, octopiGrid);

    if (octopi.every(octopus => octopus.hasFlashed)) {
        break;
    }
}

console.log(step);