export interface IRoom {
    id: number,
    roomNum: number,
    pricePerNight: number,
    isAvailable: boolean,
    imageUrl: string,
    facilities: string,
    roomType: string
}
