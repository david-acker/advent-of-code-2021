from functools import reduce

hex_values = ['0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F']
binary_values = ['0000', '0001', '0010', '0011', '0100', '0101', '0110', '0111', '1000', '1001', '1010', '1011', '1100', '1101', '1110', '1111']

hex_to_binary: dict[str, str] = dict(zip(hex_values, binary_values))

class Packet:
    def __init__(self, version: int, type_id: int):
        self.version: int = version
        self.type_id: int = type_id

        self.sub_packets: list[Packet] = []
        self.value: int = None

    def get_value(self):
        if self.value is not None:
            return self.value
        
        sub_packet_values = [sp.get_value() for sp in self.sub_packets]
        
        match self.type_id:
            case 0:
                return sum(sub_packet_values)
            case 1:
                return reduce(lambda x, y: x * y, sub_packet_values)
            case 2:
                return min(sub_packet_values)
            case 3:
                return max(sub_packet_values)
            case 5:
                return int(sub_packet_values[0] > sub_packet_values[1])
            case 6:
                return int(sub_packet_values[0] < sub_packet_values[1])
            case 7:
                return int(sub_packet_values[0] == sub_packet_values[1])
            case _:
                return 0

class Transmission:
    def __init__(self, hex_transmission: str):
        self.binary: str = ''.join([hex_to_binary[x] for x in hex_transmission])
        self.packets: list[Packet] = []

    def parse_packet(self, position: int) -> tuple[Packet, int]:

        version = int(self.binary[position:position + 3], 2)
        type_id = int(self.binary[position + 3:position + 6], 2)
        position += 6

        packet = Packet(version, type_id)
        self.packets.append(packet)

        if packet.type_id == 4:
            literal_value = ''
            while True:
                group = self.binary[position:position + 5]
                position += 5

                literal_value += group[1:]

                if group[0] == '0':
                    packet.value = int(literal_value, 2) 
                    break
        else:
            length_type_id = self.binary[position]
            position += 1

            if length_type_id == '0':
                length = int(self.binary[position:position + 15], 2)
                position += 15

                packet_end = position + length

                while position < packet_end:
                    sub_packet, position = self.parse_packet(position)
                    packet.sub_packets.append(sub_packet)
            else:
                length = int(self.binary[position:position + 11], 2)
                position += 11

                for x in range(length):
                    sub_packet, position = transmission.parse_packet(position)
                    packet.sub_packets.append(sub_packet)
    
        return (packet, position)

with open('C:/Users/david/source/repos/advent-of-code-2021/input/day_16.txt') as file:
    hex_transmission = file.readline().strip()

# Part One
transmission = Transmission(hex_transmission)
packet, _ = transmission.parse_packet(0)

partOneResult = sum([p.version for p in transmission.packets])
print(partOneResult)

# Part Two
partTwoResult = packet.get_value()
print(partTwoResult)