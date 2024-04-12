
export interface BaseResponseModel<TDataItem> {
    code: number
    message: string
    data: TDataItem[]
}

