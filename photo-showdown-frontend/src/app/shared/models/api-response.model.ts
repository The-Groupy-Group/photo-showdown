export interface EmptyAPIResponse {
	isSuccess: boolean;
	message: string;
}
export interface APIResponse<T> extends EmptyAPIResponse {
	data: T;
}
