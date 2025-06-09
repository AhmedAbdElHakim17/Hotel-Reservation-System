export interface IPayment {
    id: number;
    reservationId: number;
    amount: number;
    transactionDate: Date;
    paymentMethod: string;
    paymentStatus: string;
}
