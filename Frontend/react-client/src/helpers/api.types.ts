export interface IUserProfile {
    displayName: string,
    token: string,
    imageurl: string
}

export interface IApiData{
    // extend when new endpoint is defined 
    data: string | IUserProfile; 
}

export enum ErrorCodes{
    General = "general",
    Validation= "validation",
    Unhandled= "unhandled"
}

export enum Methods{
    Get = "GET"
}

export interface IError{
    errorCode: ErrorCodes;
    errorText: string;
}

export interface IDefaultReturn {
    success: boolean;
    data: IApiData | undefined;
    error: IError;
}


