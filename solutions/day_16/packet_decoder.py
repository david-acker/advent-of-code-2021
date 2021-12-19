hex_values = ['0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F']
binary_values = ['0000', '0001', '0010', '0011', '0100', '0101', '0110', '0111', '1000', '1001', '1010', '1011', '1100', '1101', '1110', '1111']

hex_to_binary: dict[str, str] = dict(zip(hex_values, binary_values))

class Packet:
    def __init__(self, version: int, type_id: int):
        self.version: int = version
        self.type_id: int = type_id

        self.sub_packets: list[Packet] = []
        self.value: int = None

class Transmission:
    def __init__(self, hex_transmission: str):
        self.data: str = ''.join([hex_to_binary[x] for x in hex_transmission])
        self.packets: list[Packet] = []

    def parse_packet(self, position: int) -> tuple[Packet, int]:

        version = int(self.data[position:position + 3], 2)
        type_id = int(self.data[position + 3:position + 6], 2)
        position += 6

        packet = Packet(version, type_id)
        self.packets.append(packet)

        if packet.type_id == 4:
            literal_value = ''
            while True:
                group = self.data[position:position + 5]
                position += 5

                literal_value += group[1:]

                if group[0] == '0':
                    packet.value = int(literal_value, 2) 
                    break
        else:
            length_type_id = self.data[position]
            position += 1

            if length_type_id == '0':
                length = int(self.data[position:position + 15], 2)
                position += 15

                packet_end = position + length

                while position < packet_end:
                    sub_packet, position = self.parse_packet(position)
                    packet.sub_packets.append(sub_packet)
            else:
                length = int(self.data[position:position + 11], 2)
                position += 11

                for x in range(length):
                    sub_packet, position = transmission.parse_packet(position)
                    packet.sub_packets.append(sub_packet)
    
        return (packet, position)

with open('C:/Users/david/source/repos/advent-of-code-2021/input/day_16.txt') as file:
    hex_transmission = file.readline().strip()

# Part One
transmission = Transmission(hex_transmission)
transmission.parse_packet(0)

partOneResult = sum([p.version for p in transmission.packets])
print(partOneResult)