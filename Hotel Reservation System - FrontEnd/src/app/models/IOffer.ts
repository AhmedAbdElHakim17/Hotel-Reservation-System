import { RoomType } from "../Enums/RoomType.enum";

export interface IOffer {
    id: number,
    title: string,
    description: string,
    discount: number,
    startDate: Date,
    endDate: Date,
    roomType: RoomType
}
