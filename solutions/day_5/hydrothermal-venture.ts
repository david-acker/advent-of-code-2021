import { readFileSync } from 'fs';

enum Dimension { X, Y }
enum ChangeType { Unchanged, Increasing, Decreasing }

class Coordinate {
    public x: number;
    public y: number;

    constructor(x: number, y: number) {
        this.x = x;
        this.y = y;
    }

    public getByDimension(dimension: Dimension): number { 
        return dimension === Dimension.X ? this.x : this.y 
    };
}

class DimensionChange {
    public type: ChangeType;
    public magnitude: number;

    constructor(delta: number) {
        this.type = this.getChangeType(delta);
        this.magnitude = Math.abs(delta);
    }

    private getChangeType(delta: number): ChangeType {
        if (delta === 0) {
            return ChangeType.Unchanged;
        }
        return delta > 0 ? ChangeType.Increasing : ChangeType.Decreasing;
    }
}

class CoordinatePair {
    public start: Coordinate;
    public end: Coordinate;

    public xChange: DimensionChange;
    public yChange: DimensionChange;

    constructor(first: Coordinate, second: Coordinate) {
        this.start = first;
        this.end = second;

        this.xChange = this.getCoordinateChange(Dimension.X);
        this.yChange = this.getCoordinateChange(Dimension.Y);
    }

    public get IsDiagonal(): boolean {
        return this.xChange.type !== ChangeType.Unchanged &&
            this.yChange.type !== ChangeType.Unchanged;
    }

    private getCoordinateChange(dimension: Dimension): DimensionChange {
        const delta: number = this.end.getByDimension(dimension) - this.start.getByDimension(dimension);
        return new DimensionChange(delta);
    }
}

function getCoordinatePairs() : CoordinatePair[] {
    const inputData: string[] = readFileSync('C:/Users/david/source/repos/advent-of-code-2021/input/day_5.txt', 'utf-8').split('\r\n');

    return inputData.map(x => {
        const coordinates: Coordinate[] = x.split('->')
            .map(x => x.trim())
            .map(x => {
                const numbers: number[] = x.split(',')
                    .map(x => parseInt(x));

                return new Coordinate(numbers[0], numbers[1]);
            });

        return new CoordinatePair(coordinates[0], coordinates[1])
    });
}

function recordCoordinateKeys(pair: CoordinatePair, keyMap: Map<string, number>) {
    const magnitude: number = Math.max(pair.xChange.magnitude, pair.yChange.magnitude);
    
    for (let i = 0; i < magnitude + 1; i++) {
        let xNext: number = pair.start.x;
        if (pair.xChange.type !== ChangeType.Unchanged) {
            xNext += (pair.xChange.type === ChangeType.Increasing ? i : -i);
        }

        let yNext: number = pair.start.y;
        if (pair.yChange.type !== ChangeType.Unchanged) {
            yNext += (pair.yChange.type === ChangeType.Increasing ? i : -i);
        }

        const coordinateKey: string = `${xNext},${yNext}`;
        keyMap.set(coordinateKey, (keyMap.get(coordinateKey) || 0) + 1);
    }
}

// Part One
let coordinateKeyMap: Map<string, number> = new Map();

getCoordinatePairs()
    .filter(pair => !pair.IsDiagonal)
    .forEach(pair => recordCoordinateKeys(pair, coordinateKeyMap));

const partOneResult: number = [...coordinateKeyMap.entries()].filter(x => x[1] > 1).length;
console.log(partOneResult);


// Part Two
coordinateKeyMap = new Map();

getCoordinatePairs()
    .forEach(pair => recordCoordinateKeys(pair, coordinateKeyMap));

const partTwoResult: number = [...coordinateKeyMap.entries()].filter(x => x[1] > 1).length;
console.log(partTwoResult);