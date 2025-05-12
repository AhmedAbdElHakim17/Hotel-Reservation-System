export interface ApiResponse<T> {
    message: string,
    data: T,
    isSuccess: boolean
}
