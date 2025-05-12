
export interface IReservation {
    id: number,
    checkInDate: Date,
    checkOutDate: Date,
    reservationStatus: string,
    totalAmount: number,
    createdAt: Date,
    userName: string,
    roomNum: number
}
